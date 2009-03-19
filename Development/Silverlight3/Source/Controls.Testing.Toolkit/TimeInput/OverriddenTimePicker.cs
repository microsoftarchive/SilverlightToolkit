// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Media;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overridden TimePicker.
    /// </summary>
    public class OverriddenTimePicker : TimePicker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverriddenTimePicker"/> class.
        /// </summary>
        public OverriddenTimePicker()
        {
            XamlBuilder<ControlTemplate> template = new XamlBuilder<ControlTemplate>
            {
                Name = "controlTemplate",
                ExplicitNamespaces = new Dictionary<string, string> { { "ctrl", XamlBuilder.GetNamespace(GetType()) } },
                AttributeProperties = new Dictionary<string, string> { { "TargetType", "ctrl:" + GetType().Name } },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<OverriddenTimeUpDown>
                    {
                        Name = "TimeUpDown"
                    }
                }
            };
            Template = template.Load();
        }

        /// <summary>
        /// Gets the Overridden TimeUpDown child.
        /// </summary>
        internal OverriddenTimeUpDown OverriddenTimeUpDown 
        {
            get { return VisualTreeHelper.GetChild(this, 0) as OverriddenTimeUpDown; }
        }
    }
}
