// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Testing.Metadata.VisualStudio;

namespace Microsoft.Phone.Testing.Client.Converters
{
  /// <summary>
  /// Formats an Exception into a valid Uri for documentation Page.
  /// </summary>
  public sealed class DocViewUriConverter : IValueConverter
  {
    /// <summary>
    /// Convert an <see cref="T:System.Exception"/> object into 
    /// a well formed Uri string.
    /// </summary>
    /// <param name="value">The given Exception or ExpectedException instance.</param>
    /// <param name="targetType">The type corresponding to the binding property, 
    /// which must be of <see cref="T:System.String"/>.</param>
    /// <param name="parameter">(not used).</param>
    /// <param name="culture">(not used).</param>
    /// <returns>Returns a Uri string.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string typeName = String.Empty;

      if (value != null)
      {

        if (value is ExpectedException)
        {
          typeName = (value as ExpectedException).ExceptionType.FullName;
        }

        if (value is Exception)
        {
          Type t = (value as Exception).GetType();
          typeName = t.FullName;
        }
      }

      return String.Format("/Microsoft.Phone.Toolkit.Testing;component/Client/DocView.xaml?t={0}", typeName);
    }

    /// <summary>
    /// Not Implemented
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
