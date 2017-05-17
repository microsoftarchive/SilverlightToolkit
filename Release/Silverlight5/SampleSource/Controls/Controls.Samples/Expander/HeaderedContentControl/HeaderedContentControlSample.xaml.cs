// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the HeaderedContentControl.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Name of the control")]
    [Sample("HeaderedContentControl", DifficultyLevel.Basic, "Expander")]
    public partial class HeaderedContentControlSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the HeaderedContentControlSample class.
        /// </summary>
        public HeaderedContentControlSample()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Load the demonstration.
        /// </summary>
        /// <param name="sender">Sample page.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            btnChange.Click += ChangeSettings;
        }

        /// <summary>
        /// Change ContentControl settings (Header, HeaderTemplate, Content, ContentTemplate).
        /// </summary>
        /// <param name="sender">Sender Button.</param>
        /// <param name="e">Event args.</param>
        private void ChangeSettings(object sender, RoutedEventArgs e)
        {
            HCC.Header = tbHeader.Text;
            HCC.Content = tbContent.Text;

            if (String.IsNullOrEmpty(tbHeaderTemplate.Text))
            {
                HCC.ClearValue(HeaderedContentControl.HeaderTemplateProperty);
            }
            else
            {
                try
                {
                    HCC.HeaderTemplate = (DataTemplate)XamlReader.Load(AddXmlNS(tbHeaderTemplate.Text));
                    tbHeaderTemplate.Foreground = new SolidColorBrush(Colors.Black);
                }
                catch (XamlParseException)
                {
                    tbHeaderTemplate.Foreground = new SolidColorBrush(Colors.Red);
                }
            }

            if (string.IsNullOrEmpty(tbContentTemplate.Text))
            {
                HCC.ClearValue(HeaderedContentControl.ContentTemplateProperty);
            }
            else
            {
                try
                {
                    HCC.ContentTemplate = (DataTemplate)XamlReader.Load(AddXmlNS(tbContentTemplate.Text));
                    tbContentTemplate.Foreground = new SolidColorBrush(Colors.Black);
                }
                catch (XamlParseException)
                {
                    tbContentTemplate.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        /// <summary>
        /// Utility function to add xmlns to user inputed template string.
        /// </summary>
        /// <param name="s">User input template string.</param>
        /// <returns>Well formated xaml with xmlns added.</returns>
        private static string AddXmlNS(string s)
        {
            string xmlns = @" xmlns=""http://schemas.microsoft.com/client/2007""
                              xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                              xmlns:controls=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls"" ";
            int i = s.IndexOf(">", StringComparison.Ordinal);
            return (i == -1) ? ">/" : s.Substring(0, i) + xmlns + s.Substring(i);
        }
    }
}