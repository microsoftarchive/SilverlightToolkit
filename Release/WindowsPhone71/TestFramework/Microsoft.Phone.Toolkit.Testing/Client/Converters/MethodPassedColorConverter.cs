// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Phone.Testing.Client.Converters
{
  /// <summary>
  /// A color selection converter for translating a bool result into
  /// a color.
  /// </summary>
  public class MethodPassedColorConverter : IValueConverter
  {
    /// <summary>
    /// Converts a boolean value into a <see cref="T:System.Windows.Media.SolidColorBrush"/> value.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <param name="targetType">(not used)</param>
    /// <param name="parameter">(not used)</param>
    /// <param name="culture">(not used)</param>
    /// <returns>A <see cref="T:System.Windows.Media.SolidColorBrush"/> representing pass or failure.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool passValue = true;

      if (value != null)
      {
        if (value is bool)
        {
          passValue = (bool)value;
        }
      }

      ResourceDictionary frameworkResources = new ResourceDictionary();

      Application.LoadComponent(frameworkResources, new Uri("/Microsoft.Phone.Toolkit.Testing;component/Client/Resources/FrameworkResources.xaml", UriKind.Relative));

      if (!passValue)
      {
        return frameworkResources["FailBrush"] as SolidColorBrush;
      }

      return frameworkResources["PassBrush"] as SolidColorBrush;
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
