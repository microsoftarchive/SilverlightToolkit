//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls.Common
{
    /// <summary>
    /// Utility class for operations.
    /// </summary>
    internal static class Extensions
    {
        #region Static Fields and Constants

        private static bool _areHandlersSuspended;

        #endregion Static Fields and Constants

        #region Static Methods

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
                
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801", Justification = "obj parameter is needed for the extension method.")]
        public static bool AreHandlersSuspended(this DependencyObject obj)
        {
            return _areHandlersSuspended;
        }

        #endregion Static Methods
    }
}
