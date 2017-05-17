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

namespace System.Windows.Controls.Data.DataForm.Toolkit.Design
{
    /// <summary>
    /// To register design time metadata for DataField.
    /// </summary>
    internal class DataFieldMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataField.
        /// </summary>
        public DataFieldMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataField),
                b =>
                {
                    // Common
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.Content),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.Description),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.IsReadOnly),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.IsRequired),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.IsValid),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.Label),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.Mode),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.PropertyPath),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    // Layout
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.DescriptionViewerPosition),
                        new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.DescriptionViewerVisibility),
                        new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.DescriptionViewerVisibility),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.LabelPosition),
                        new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.LabelVisibility),
                        new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.LabelVisibility),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced));

                    // TextBoxEditor
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.Label),
                        PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));

#if MWD40
                    // DataContextValueSource
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.DescriptionViewerStyle),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.DataField>(x => x.Description),
                            true));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.LabelStyle),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.DataField>(x => x.Label),
                            true));
#endif
                });
        }
    }
}
