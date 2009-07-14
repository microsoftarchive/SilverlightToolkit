// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// Internal extension methods specific to the DataGrid Designer dll
    /// </summary>
    internal static class DataGridDesignerExtensions
    {
        private static bool _areHandlersSuspended;

        /// <summary>
        /// Returns true if event handlers are suspended for this dependency object
        /// </summary>
        /// <param name="obj">object that is checking</param>
        /// <returns>true if event handlers are suspended for this dependency object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "obj")]
        public static bool AreHandlersSuspended(this DependencyObject obj)
        {
            return _areHandlersSuspended;
        }

        /// <summary>
        /// Helper mechanism for setting DependencyProperties without a callback
        /// </summary>
        /// <param name="obj">object that is setting the DependencyProperty</param>
        /// <param name="property">Property to set</param>
        /// <param name="value">Value to set</param>
        public static void SetValueNoCallback(this DependencyObject obj, DependencyProperty property, object value)
        {
            _areHandlersSuspended = true;
            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                _areHandlersSuspended = false;
            }
        }
    }
}
