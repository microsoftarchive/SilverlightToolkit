// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Bug regression tests for the System.Windows.Controls.DockPanel class.
    /// </summary>
    public partial class DockPanelTest
    {
        /// <summary>
        /// Ensure a DockPanel can have its background property styled in XAML.
        /// </summary>
        [TestMethod]
        [Description("Ensure a DockPanel can have its background property styled in XAML.")]
        [Asynchronous]
        [Bug("528095 - DockPanel - Cannot be styled in XAML, e.g. setting background", Fixed = true)]
        public void StyleBackgroundInXaml()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.Height = 400;
            panel.Width = 600;

            XamlBuilder<Style> styleBuilder = new XamlBuilder<Style>
            {
                ExplicitNamespaces = new Dictionary<string, string> { { "controlsToolkit", XamlBuilder.GetNamespace(typeof(DockPanel)) } },
                AttributeProperties = new Dictionary<string, string> { { "TargetType", "controlsToolkit:DockPanel" } },
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
        /// Ensure a DockPanel can have its background property styled.
        /// </summary>
        [TestMethod]
        [Description("Ensure a DockPanel can have its background property styled.")]
        [Asynchronous]
        public void StyleBackground()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.Height = 400;
            panel.Width = 600;

            Style style = new Style(typeof(DockPanel));
            style.Setters.Add(new Setter(DockPanel.BackgroundProperty, new SolidColorBrush(Colors.Gray)));

            TestAsync(
                panel,
                () => panel.Style = style);
        }
    }
}