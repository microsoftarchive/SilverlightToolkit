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
    /// To register design time metadata for DataForm.
    /// </summary>
    internal class DataFormMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataForm.
        /// </summary>
        public DataFormMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataForm), b =>
                {
                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(Extensions.GetMemberName<SSWC.DataForm>(x => x.ItemsSource)));

                    // Common Properties
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.AutoCommit), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.AutoEdit), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.AutoGenerateFields), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CurrentIndex), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ItemsSource), new CategoryAttribute(Properties.Resources.CommonProperties));

                    // Layout
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.DescriptionViewerPosition), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.LabelPosition), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.HeaderVisibility), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.HeaderVisibility), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CommandButtonsVisibility), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CommandButtonsVisibility), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));

                    // DataForm Styling
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.DataFieldStyle), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.EditTemplate), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ValidationSummaryStyle), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.HeaderTemplate), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.NewItemTemplate), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ReadOnlyTemplate), new CategoryAttribute(Properties.Resources.DataFormStyling));

                    // TextBoxEditor
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CancelButtonContent), PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CommitButtonContent), PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.Header), PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));

                    // DataContextValueSource
                    string itemsSourceName = Extensions.GetMemberName<SSWC.DataForm>(x => x.ItemsSource);
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.EditTemplate), new DataContextValueSourceAttribute(itemsSourceName, true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.HeaderTemplate), new DataContextValueSourceAttribute(Extensions.GetMemberName<SSWC.DataForm>(x => x.Header), true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.NewItemTemplate), new DataContextValueSourceAttribute(itemsSourceName, true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ReadOnlyTemplate), new DataContextValueSourceAttribute(itemsSourceName, true));
                });
        }
    }
}
