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
    /// A simple wrapper for used Tfs components. Will attempt to load the
    /// Team Foundation Server types that are needed, and gracefully fall
    /// back if they cannot be loaded.
    /// </summary>
    internal static class TryTfs
    {
        /// <summary>
        /// Attempts to checkout the path, and return a value indicating
        /// whether the checkout was successful.
        /// </summary>
        /// <param name="path">The filename on the local system.</param>
        /// <returns>Returns a value indicating whether the checkout with TFS
        /// was sucessful.</returns>
        public static bool Checkout(string path)
        {
            return TryLoadingTfsAssembliesForCheckout(path);
        }

        /// <summary>
        /// Attempts to checkout the path, and return a value indicating
        /// whether the checkout was successful.
        /// </summary>
        /// <param name="path">The filename on the local system.</param>
        /// <returns>Returns a value indicating whether the checkout with TFS
        /// was sucessful.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design to enable use on systems without Team Foundation Server.")]
        private static bool TryLoadingTfsAssembliesForCheckout(string path)
        {
            try
            {
                return TryCheckout(path);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to checkout the path, and return a value indicating
        /// whether the checkout was successful.
        /// </summary>
        /// <param name="path">The filename on the local system.</param>
        /// <returns>Returns a value indicating whether the checkout with TFS
        /// was sucessful.</returns>
        private static bool TryCheckout(string path)
        {
            return Tfs.TryCheckout(path);
        }
    }
}