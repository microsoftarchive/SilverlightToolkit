// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// A helper class that provides useful utilities to
    /// work with weak references.
    /// </summary>
    public static class WeakReferenceHelper
    {
        /// <summary>
        /// Determine if there is a reference to a known target in an object
        /// that implements IEnumerable.
        /// </summary>
        /// <param name="references">
        /// The object that implements IEnumerable.
        /// </param>
        /// <param name="target">
        /// The known target.
        /// </param>
        /// <returns>
        /// True if a reference to the known target exists in the list. False otherwise.
        /// </returns>
        public static bool ContainsTarget(IEnumerable<WeakReference> references, object target)
        {
            if (references == null)
            {
                return false;
            }

            foreach (WeakReference r in references)
            {
                if (target == r.Target)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Remove a reference to a known target from an object
        /// that implement IList.
        /// </summary>
        /// <param name="references">The list to be examined.</param>
        /// <param name="target">The known target.</param>
        /// <returns>
        /// Try if the target was successsfully removed.
        /// </returns>
        public static bool TryRemoveTarget(IList<WeakReference> references, object target)
        {
            if (references == null)
            {
                return false;
            }

            for (int i = 0; i < references.Count; i++)
            {
                if (references[i].Target == target)
                {
                    references.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all the weak references with null targets from an object
        /// that implements IList.
        /// </summary>
        /// <param name="references">
        /// The object that implements IList.
        /// </param>
        public static void RemoveNullTargetReferences(IList<WeakReference> references)
        {
            if (references == null)
            {
                return;
            }

            for (int i = 0; i < references.Count; i++)
            {
                if (references[i].Target == null)
                {
                    references.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}