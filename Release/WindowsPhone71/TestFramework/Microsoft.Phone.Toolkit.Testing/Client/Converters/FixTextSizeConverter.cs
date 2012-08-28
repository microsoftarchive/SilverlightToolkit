// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Phone.Testing.Client.Converters
{
  /// <summary>
  /// A Text font size selector based on the length in characters of the value 
  /// being presented.
  /// </summary>
  public class FixTextLengthStyleConverter : IValueConverter
  {
    /// <summary>
    /// Provides a <see cref="T:System.Windows.Style"/> based on the digit
    /// length of a given <see cref="T:System.Int32"/> value.
    /// </summary>
    /// <param name="value">The given <see cref="T:System.Int32"/> value.</param>
    /// <param name="targetType">The type corresponding to the binding property, 
    /// which must be of <see cref="T:System.Windows.Style"/>.</param>
    /// <param name="parameter">(Not used).</param>
    /// <param name="culture">(Not used).</param>
    /// <returns>A <see cref="T:System.Windows.Style"/> appropriate for the length of the content.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if (value is Int32)
        {
          string sValue = ((int)value).ToString();
          switch (sValue.Length)
          {
            case 1:
            case 2:
            case 3:
              return Application.Current.Resources["PhoneTextTitle1Style"] as Style;
            case 4:
              return Application.Current.Resources["PhoneTextExtraLargeStyle"] as Style;
            default:
              return Application.Current.Resources["PhoneTextLargeStyle"] as Style;
          }
        }
      }

      return Application.Current.Resources["PhoneTextTitle1Style"] as Style;
    }

    /// <summary>
    /// Not Supported.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
