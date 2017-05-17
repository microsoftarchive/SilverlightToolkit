// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// To register design time metadata for SSWC.TimeUpDown.
    /// </summary>
    internal class TimeUpDownMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWC.TimeUpDown.
        /// </summary>
        public TimeUpDownMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.TimeUpDown),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.IsEditable),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Minimum),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Maximum),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Value),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.IsCyclic),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.TimeParsers),
                        new CategoryAttribute(Properties.Resources.TimeInput));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Format),
                        new CategoryAttribute(Properties.Resources.TimeInput));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Culture),
                        new CategoryAttribute(Properties.Resources.TimeInput));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.TimeGlobalizationInfo),
                        new CategoryAttribute(Properties.Resources.TimeInput));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Culture),
                        PropertyValueEditor.CreateEditorAttribute(typeof(CultureInfoEditor)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.TimeGlobalizationInfo),
                        new TypeConverterAttribute(typeof(ExpandableObjectConverter)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.ActualTimeParsers),
                        new BrowsableAttribute(false));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Minimum),
                        new PropertyOrderAttribute(PropertyOrder.Early));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Maximum),
                        new PropertyOrderAttribute(PropertyOrder.CreateAfter(PropertyOrder.Early)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Value),
                        new PropertyOrderAttribute(PropertyOrder.CreateAfter(PropertyOrder.Early)));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                        Extensions.GetMemberName<SSWC.TimeUpDown>(x => x.Value)));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.BasicControls, true));
#endif
                });
        }
    }
}