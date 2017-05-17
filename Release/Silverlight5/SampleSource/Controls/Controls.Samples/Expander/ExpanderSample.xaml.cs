// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the Expander.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expander", Justification = "Name of the control")]
    [Sample("Expander", DifficultyLevel.Basic, "Expander")]
    public partial class ExpanderSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ExpanderSample class.
        /// </summary>
        public ExpanderSample()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Load the demo page.
        /// </summary>
        /// <param name="sender">Sample page.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // initalize customization area
            lbCustDirection.SelectionChanged += (x, y) =>
                {
                    ExpandDirection d = (ExpandDirection)lbCustDirection.SelectedIndex;
                    expNoButton.ExpandDirection = d;
                    expBRButton.ExpandDirection = d;
                    expFade.ExpandDirection = d;
                    expScale.ExpandDirection = d;
                };

            // initialize interactive usage area
            btnChange.Click += ChangeSettings;

            tbHeader.Text = "Header";
            tbHeaderTemplate.Text = @"<DataTemplate><StackPanel Orientation=""Horizontal""><Ellipse Width=""10"" Height=""10"" Fill=""Red""/><Button Content=""{Binding}""/><Ellipse Width=""10"" Height=""10"" Fill=""Red""/></StackPanel></DataTemplate>";
            tbContent.Text = "Content";
            tbContentTemplate.Text = @"<DataTemplate><Button Content=""{Binding}""/></DataTemplate>";

            expander.Expanded += (x, y) => OutputExpander();
            expander.Collapsed += (x, y) => OutputExpander();
        }

        /// <summary>
        /// Change ContentControl settings (Header, HeaderTemplate, Content, ContentTemplate).
        /// </summary>
        /// <param name="sender">Sender Button.</param>
        /// <param name="e">Event args.</param>
        private void ChangeSettings(object sender, RoutedEventArgs e)
        {
            expander.ExpandDirection = (ExpandDirection) lbExpandDirection.SelectedIndex;
            expander.Header = tbHeader.Text;
            expander.Content = tbContent.Text;
            expander.IsEnabled = cbIsEnabled.IsChecked ?? true;

            if (String.IsNullOrEmpty(tbHeaderTemplate.Text))
            {
                expander.ClearValue(HeaderedContentControl.HeaderTemplateProperty);
            }
            else
            {
                try
                {
                    expander.HeaderTemplate = (DataTemplate) XamlReader.Load(AddXmlNS(tbHeaderTemplate.Text));
                    tbHeaderTemplate.Foreground = new SolidColorBrush(Colors.Black);
                }
                catch (XamlParseException)
                {
                    tbHeaderTemplate.Foreground = new SolidColorBrush(Colors.Red);
                }
            }

            if (string.IsNullOrEmpty(tbContentTemplate.Text))
            {
                expander.ClearValue(HeaderedContentControl.ContentTemplateProperty);
            }
            else
            {
                try
                {
                    expander.ContentTemplate = (DataTemplate) XamlReader.Load(AddXmlNS(tbContentTemplate.Text));
                    tbContentTemplate.Foreground = new SolidColorBrush(Colors.Black);
                }
                catch (XamlParseException)
                {
                    tbContentTemplate.Foreground = new SolidColorBrush(Colors.Red);
                }
            }

            OutputExpander();
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
                              xmlns:controls=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.InternalOnly"" ";
            int i = s.IndexOf(">", StringComparison.Ordinal);
            return (i == -1) ? ">/" : s.Substring(0, i) + xmlns + s.Substring(i);
        }

        /// <summary>
        /// Display the interactive Expander control's properties.
        /// </summary>
        private void OutputExpander()
        {
            string formatString = "\n\n" +
                " ExpandDirection:\t{0}\n" +
                " Header:\t{1}\n" +
                " Content:\t{2}\n" +
                " IsExpanded:\t{3}\n" +
                " IsEnabled:\t{4}\n";

            output.Text = string.Format(
                CultureInfo.InvariantCulture,
                formatString,
                expander.ExpandDirection,
                expander.Header,
                expander.Content,
                expander.IsExpanded,
                expander.IsEnabled);
        }
    }
}