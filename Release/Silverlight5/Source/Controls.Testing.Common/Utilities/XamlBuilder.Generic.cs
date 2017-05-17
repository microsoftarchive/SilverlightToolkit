// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// The XamlBuilder provides simplified programmatic creation of XAML
    /// markup for testing.
    /// </summary>
    /// <typeparam name="T">Type of the XAML element.</typeparam>
    public sealed partial class XamlBuilder<T> : XamlBuilder
    {
        /// <summary>
        /// Initializes a new instance of the XamlBuilder class.
        /// </summary>
        public XamlBuilder()
            : base()
        {
            ElementType = typeof(T);
        }

        /// <summary>
        /// Load the object corresponding to the XamlBuilder tree.
        /// </summary>
        /// <returns>The object corresponding to the XamlBuilder tree.</returns>
        public new T Load()
        {
            return (T) base.Load();
        }
    }
}