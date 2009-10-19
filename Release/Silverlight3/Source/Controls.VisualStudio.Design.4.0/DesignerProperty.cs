// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Collections.Generic;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// Represents a property that we are going to store the state of.
    /// Allows access to values without casting via "public static T GetDesignerProperty&lt;T&gt;".
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    internal class DesignerProperty<T>
    {
        /// <summary>
        /// Property name field.
        /// </summary>
        private string _name;

        /// <summary>
        /// Constructor to take the property name.
        /// </summary>
        /// <param name="name">Designer property name.</param>
        internal DesignerProperty(string name)
        {
            _name = name;
        }

        /// <summary>
        /// ToString override to output property name.
        /// </summary>
        /// <returns>Designer property name.</returns>
        public override string ToString()
        {
            return _name;
        }
    }
}