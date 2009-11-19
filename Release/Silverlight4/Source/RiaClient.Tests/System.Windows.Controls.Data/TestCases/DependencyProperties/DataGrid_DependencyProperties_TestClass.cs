// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Data.Test
{
    [TestClass]
    /// <summary>
    /// These tests don't depend on entity types
    /// </summary>
    public partial class DataGrid_DependencyProperties_TestClass : SilverlightTest
    {
        private int _counter = 0;
        private SelectionChangedEventArgs _selectionChangedEventArgs;

        void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._counter++;
            this._selectionChangedEventArgs = e;
        }
    }
}
