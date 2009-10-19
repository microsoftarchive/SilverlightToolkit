// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;

namespace System.Windows.Controls.Data.Design
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used by PropertyColumnEditor")]
    internal class IsUseMetadataEnabledConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColumnInfo columnGenerationInfo = value as ColumnInfo;
            return (columnGenerationInfo != null && columnGenerationInfo.HeaderFromAttribute != null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This should not be used
            throw new NotImplementedException();
        }

        #endregion
    }
}
