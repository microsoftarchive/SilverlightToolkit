// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// A dictionary that merges a resource dictionary with a parent dictionary.
    /// </summary>
    internal class MergedStyleResourceDictionary : BaseMergedStyleDictionary
    {
        /// <summary>
        /// The resource dictionary to check for a value before checking the 
        /// parent.
        /// </summary>
        private ResourceDictionary _resourceDictionary;

        /// <summary>
        /// Initializes a new instance of the MergedResourceDictionary class.
        /// </summary>
        /// <param name="resourceDictionary">A resource dictionary to check for 
        /// a value before checking the parent.</param>
        /// <param name="parentResourceDictionary">The parent merged resource 
        /// dictionary to check if no match is found in the resource 
        /// dictionary.</param>
        public MergedStyleResourceDictionary(ResourceDictionary resourceDictionary, BaseMergedStyleDictionary parentResourceDictionary)
        {
            Debug.Assert(resourceDictionary != null, "resourceDictionary cannot be null.");
            _resourceDictionary = resourceDictionary;
            Parent = parentResourceDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the MergedResourceDictionary class.
        /// </summary>
        /// <param name="resourceDictionary">A resource dictionary to check for a value 
        /// before checking the parent.</param>
        public MergedStyleResourceDictionary(ResourceDictionary resourceDictionary)
        {
            Debug.Assert(resourceDictionary != null, "resourceDictionary cannot be null.");
            _resourceDictionary = resourceDictionary;
        }

        /// <summary>
        /// Retrieves an item using a key.  If the item is not found in the 
        /// local dictionary a lookup is attempted on the parent.
        /// </summary>
        /// <param name="key">The key to use to retrieve the item.</param>
        /// <returns>A style corresponding to the key.</returns>
        internal override Style this[string key]
        {
            get
            {
                Debug.Assert(key != null, "key cannot be null.");

                Style value = null;
                value = _resourceDictionary[key] as Style;
                if (value == null && Parent != null)
                {
                    value = Parent[key];
                }

                return value;
            }
        }
    }
}