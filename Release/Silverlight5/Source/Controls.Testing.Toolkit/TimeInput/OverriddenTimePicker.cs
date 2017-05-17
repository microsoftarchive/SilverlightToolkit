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
        /// <param name="useOverriddenTimeUpDownTemplate">If set to <c>True</c> use overridden time up down template.</param>
        public OverriddenTimePicker(bool useOverriddenTimeUpDownTemplate) : base()
        {
            if (useOverriddenTimeUpDownTemplate)
            {
                XamlBuilder<ControlTemplate> template = new XamlBuilder<ControlTemplate>
                                                            {
                                                                    Name = "controlTemplate",
                                                                    ExplicitNamespaces = new Dictionary<string, string>
                                                                                             {
                                                                                                     {
                                                                                                             "ctrl", XamlBuilder.GetNamespace(GetType())
                                                                                                     }
                                                                                             },
                                                                    AttributeProperties = new Dictionary<string, string>
                                                                                              {
                                                                                                      {
                                                                                                              "TargetType", "ctrl:" + GetType().Name
                                                                                                      }
                                                                                              },
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverriddenTimePicker"/> class.
        /// Will use normal template.
        /// </summary>
        public OverriddenTimePicker() : base()
        {
        }

        /// <summary>
        /// Gets the actual time picker popup.
        /// </summary>
        /// <returns>The Popup that is used by the TimePicker.</returns>
        public new TimePickerPopup ActualTimePickerPopup
        {
            get
            {
                return base.ActualTimePickerPopup;
            }
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
