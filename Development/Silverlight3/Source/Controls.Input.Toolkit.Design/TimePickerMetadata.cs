// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// Metadata for the TimePicker control.
    /// </summary>
    internal class TimePickerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for TimePicker.
        /// </summary>
        public TimePickerMetadata()
            : base()
        {
            AddCallback(
                typeof(TimePicker),
                b =>
                    {
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Minimum), new CategoryAttribute(Properties.Resources.CommonProperties));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Maximum), new CategoryAttribute(Properties.Resources.CommonProperties));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Value), new CategoryAttribute(Properties.Resources.CommonProperties));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Popup), new CategoryAttribute(Properties.Resources.CommonProperties));

                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.TimeParsers), new CategoryAttribute(Properties.Resources.TimeInput));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Format), new CategoryAttribute(Properties.Resources.TimeInput));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Culture), new CategoryAttribute(Properties.Resources.TimeInput));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.TimeGlobalizationInfo), new CategoryAttribute(Properties.Resources.TimeInput));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.PopupMinutesInterval), new CategoryAttribute(Properties.Resources.TimeInput));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.PopupSecondsInterval), new CategoryAttribute(Properties.Resources.TimeInput));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.PopupTimeSelectionMode), new CategoryAttribute(Properties.Resources.TimeInput));

                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Culture), PropertyValueEditor.CreateEditorAttribute(typeof(CultureInfoEditor)));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.TimeGlobalizationInfo), new TypeConverterAttribute(typeof(ExpandableObjectConverter)));

                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.ActualTimeParsers), new BrowsableAttribute(false));

                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Minimum), new PropertyOrderAttribute(PropertyOrder.Early));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Maximum), new PropertyOrderAttribute(PropertyOrder.CreateAfter(PropertyOrder.Early)));
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Value), new PropertyOrderAttribute(PropertyOrder.CreateAfter(PropertyOrder.Early)));
#if MWD40
                        b.AddCustomAttributes(Extensions.GetMemberName<TimePicker>(x => x.Popup), new AlternateContentPropertyAttribute());
#endif
                    });
        }
    }
}
