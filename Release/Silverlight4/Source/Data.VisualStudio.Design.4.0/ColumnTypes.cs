// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls.Data.Design
{
    // Provides a list of strings that represent column types.
    internal static class ColumnTypes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="PropertyColumnEditor ComboBox is bound to this")]
        public static string[] GetColumnTypes() 
        {
            return new string[]
            {
                Properties.Resources.Text_Column,
                Properties.Resources.CheckBox_Column,
                Properties.Resources.Template_Column,
            };
        }
    }
}
