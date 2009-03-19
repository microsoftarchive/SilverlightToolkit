// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// To register design time metadata for TimeUpDown.
    /// </summary>
    internal class TimeUpDownMetadata : AttributeTableBuilder
    {        
        /// <summary>
        /// To register design time metadata for TimeUpDown.
        /// </summary>
        public TimeUpDownMetadata()
            : base()
        {
            AddCallback(
                typeof(TimeUpDown),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.IsEditable), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Minimum), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Maximum), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Value), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.IsCyclic), new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.TimeParsers), new CategoryAttribute(Properties.Resources.TimeInput));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Format), new CategoryAttribute(Properties.Resources.TimeInput));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Culture), new CategoryAttribute(Properties.Resources.TimeInput));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.TimeGlobalizationInfo), new CategoryAttribute(Properties.Resources.TimeInput));

                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Culture), PropertyValueEditor.CreateEditorAttribute(typeof(CultureInfoEditor)));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.TimeGlobalizationInfo), new TypeConverterAttribute(typeof(ExpandableObjectConverter)));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.ActualTimeParsers), new BrowsableAttribute(false));

                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Minimum), new PropertyOrderAttribute(PropertyOrder.Early));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Maximum), new PropertyOrderAttribute(PropertyOrder.CreateAfter(PropertyOrder.Early)));
                    b.AddCustomAttributes(Extensions.GetMemberName<TimeUpDown>(x => x.Value), new PropertyOrderAttribute(PropertyOrder.CreateAfter(PropertyOrder.Early)));
                });
        }
    }
}
