// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Simple implementation of ObjectCollection that avoids the need to
    /// reference the Controls.Toolkit project (which causes packaging problems).
    /// </summary>
    public class TestObjectCollection : Collection<object>
    {
        /// <summary>
        /// Initializes a new instance of the TestObjectCollection class.
        /// </summary>
        public TestObjectCollection()
        {
        }
    }
}