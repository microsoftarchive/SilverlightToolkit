// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// Utility methods for working with Team Foundation Server.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tfs", Justification = "Standard abbreviaion for TeamFoundationServer (which is an existing type).")]
    public static class Tfs
    {
        /// <summary>
        /// Cache of workspaces.
        /// </summary>
        private static List<Workspace> Workspaces = new List<Workspace>();

        /// <summary>
        /// Get a Workspace associated with a path.
        /// </summary>
        /// <param name="path">Path contained in the workspace.</param>
        /// <returns>Associated Workspace.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Simplified setup for this parameter.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Optional step in most build tasks that should never throw exceptions")]
        public static Workspace GetWorkspace(string path)
        {
            try
            {
                // First try the cached worksapces
                foreach (Workspace workspace in Workspaces)
                {
                    if (workspace.IsLocalPathMapped(path))
                    {
                        return workspace;
                    }
                }
            }
            catch
            {
            }

            // Otherwise try to create a new workspace
            try
            {
                // Get workspace info for the desired file
                Workstation local = Workstation.Current;
                if (local != null)
                {
                    WorkspaceInfo workspaceInfo = local.GetLocalWorkspaceInfo(path);
                    if (workspaceInfo != null)
                    {
                        // TODO: This only uses the local users credentials
                        // (we can either consider passing in credentials or
                        // also trying to use the build host - like VS - to
                        // obtain them)
                        TeamFoundationServer server = new TeamFoundationServer(workspaceInfo.ServerUri.ToString());
                        Workspace workspace = workspaceInfo.GetWorkspace(server);
                        Workspaces.Add(workspace);
                        return workspace;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Determine if a file is version controlled by TFS.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>
        /// True if the file is version controlled, false otherwise.
        /// </returns>
        public static bool IsVersionControlled(string path)
        {
            return GetWorkspace(path) != null;
        }

        /// <summary>
        /// Try to check out a file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>True if the checkout succeeded, false otherwise.</returns>
        public static bool TryCheckout(string path)
        {
            Workspace workspace = GetWorkspace(path);
            if (workspace != null)
            {
                return TryCheckout(workspace, path);
            }
            return false;
        }

        /// <summary>
        /// Try to check out a file.
        /// </summary>
        /// <param name="workspace">TFS workspace.</param>
        /// <param name="path">Path to the file.</param>
        /// <returns>True if the checkout succeeded, false otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Optional step in most build tasks that should never throw exceptions")]
        public static bool TryCheckout(Workspace workspace, string path)
        {
            if (workspace == null)
            {
                return false;
            }

            try
            {
                // Mark the file as a pending edit
                workspace.PendEdit(path);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determine if a file is checked out.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>True if file is checked out, false otherwise.</returns>
        public static bool IsCheckedOut(string path)
        {
            Workspace workspace = GetWorkspace(path);
            if (workspace != null)
            {
                return IsCheckedOut(workspace, path);
            }
            return false;
        }

        /// <summary>
        /// Determine if a file is checked out.
        /// </summary>
        /// <param name="workspace">TFS workspace.</param>
        /// <param name="path">Path to the file.</param>
        /// <returns>True if file is checked out, false otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Optional step in most build tasks that should never throw exceptions")]
        public static bool IsCheckedOut(Workspace workspace, string path)
        {
            if (workspace == null)
            {
                return false;
            }

            try
            {
                foreach (PendingChange change in workspace.GetPendingChanges(path))
                {
                    if ((change.ChangeType & (ChangeType.Edit | ChangeType.Add)) > 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }
    }
}