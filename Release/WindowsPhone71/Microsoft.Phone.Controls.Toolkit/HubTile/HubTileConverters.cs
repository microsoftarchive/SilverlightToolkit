// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Converts a multi-line string into a single line string.
    /// </summary>
    internal class MultipleToSingleLineStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string multiline = value as string;

            if (string.IsNullOrEmpty(multiline))
                return string.Empty;

            return multiline.Replace(Environment.NewLine, " ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// If there is a new notification (value)
    /// Returns a Visible value for the notification block (parameter).
    /// Or a Collapsed value for the message block (parameter).
    /// Returns a opposite values otherwise.
    /// </summary>
    internal class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value ^ (bool)parameter)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Returns the Hub tile width corresponding to a tile size.
    /// </summary>
    public class TileSizeToWidthConverter : IValueConverter
    {
        /// <summary>
        /// Converts from a tile size to the corresponding width.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double baseWidth = 0;

            switch ((TileSize)value)
            {
                case TileSize.Default:
                    baseWidth = 173;
                    break;

                case TileSize.Small:
                    baseWidth = 99;
                    break;

                case TileSize.Medium:
                    baseWidth = 210;
                    break;

                case TileSize.Large:
                    baseWidth = 432;
                    break;
            }

            double multiplier;

            if (parameter == null || double.TryParse(parameter.ToString(), out multiplier) == false)
            {
                multiplier = 1;
            }

            return baseWidth * multiplier;
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Returns the Hub tile height corresponding to a tile size.
    /// </summary>
    public class TileSizeToHeightConverter : IValueConverter
    {
        /// <summary>
        /// Converts from a tile size to the corresponding height.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double baseHeight = 0;

            switch ((TileSize)value)
            {
                case TileSize.Default:
                    baseHeight = 173;
                    break;

                case TileSize.Small:
                    baseHeight = 99;
                    break;

                case TileSize.Medium:
                    baseHeight = 210;
                    break;

                case TileSize.Large:
                    baseHeight = 210;
                    break;
            }

            double multiplier;

            if (parameter == null || double.TryParse(parameter.ToString(), out multiplier) == false)
            {
                multiplier = 1;
            }

            return baseHeight * multiplier;
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
