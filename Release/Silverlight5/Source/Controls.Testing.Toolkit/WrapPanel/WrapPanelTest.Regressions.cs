// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Bug regression tests for System.Windows.Controls.WrapPanel.
    /// </summary>
    public partial class WrapPanelTest
    {
        /// <summary>
        /// Ensure a WrapPanel can have its background property styled.
        /// </summary>
        [TestMethod]
        [Description("Ensure a WrapPanel can have its background property styled.")]
        [Asynchronous]
        [Bug("528098 - WrapPanel - Cannot be styled in XAML, e.g. setting background", Fixed = true)]
        public void StyleBackgroundInXaml()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Height = 400;
            panel.Width = 600;

            XamlBuilder<Style> styleBuilder = new XamlBuilder<Style>
            {
                ExplicitNamespaces = new Dictionary<string, string> { { "controlsToolkit", XamlBuilder.GetNamespace(typeof(WrapPanel)) } },
                AttributeProperties = new Dictionary<string, string> { { "TargetType", "controlsToolkit:WrapPanel" } },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<Setter>
                    {
                        AttributeProperties = new Dictionary<string, string> { { "Property", "Background" } },
                        ElementProperties = new Dictionary<string, XamlBuilder>
                        {
                            {
                                "Value",
                                new XamlBuilder<SolidColorBrush>
                                {
                                    AttributeProperties = new Dictionary<string, string> { { "Color", "Gray" } }
                                }
                            }
                        }
                    }
                }
            };

            TestAsync(
                panel,
                () => panel.Style = styleBuilder.Load());
        }

        /// <summary>
        /// Ensure a WrapPanel can have its background property styled.
        /// </summary>
        [TestMethod]
        [Description("Ensure a WrapPanel can have its background property styled.")]
        [Asynchronous]
        public void StyleBackground()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Height = 400;
            panel.Width = 600;

            Style style = new Style(typeof(WrapPanel));
            style.Setters.Add(new Setter(WrapPanel.BackgroundProperty, new SolidColorBrush(Colors.Gray)));

            TestAsync(
                panel,
                () => panel.Style = style);
        }
    }
}