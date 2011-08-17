// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if WINDOWS_PHONE_DESIGN
using Controls = Microsoft.Phone.Controls;
using DesignerProperties = Microsoft.Phone.Controls.Toolkit.Design.Properties;
#else
extern alias Silverlight;
using System.Windows.Controls.Design.Common;
using Controls = Silverlight::System.Windows.Controls;
using DesignerProperties = System.Windows.Controls.Properties;
#endif

using System.ComponentModel;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

#if WINDOWS_PHONE_DESIGN
namespace Microsoft.Phone.Controls.Design
#else
namespace System.Windows.Controls.Input.Design
#endif
{
    /// <summary>
    /// To register design time metadata for AutoCompleteBox.
    /// </summary>
    internal class AutoCompleteBoxMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for AutoCompleteBox.
        /// </summary>
        public AutoCompleteBoxMetadata()
            : base()
        {
#if WINDOWS_PHONE_DESIGN
            AddCustomAttributes(typeof(AutoCompleteBox), "FilterMode", new CategoryAttribute(DesignerProperties.Resources.CommonProperties));
            AddCustomAttributes(typeof(AutoCompleteBox), "InputScope", new CategoryAttribute(DesignerProperties.Resources.AutoComplete));
            AddCustomAttributes(typeof(AutoCompleteBox), "InputScope", new DescriptionAttribute("Gets or sets the InputScope used by the Text template part."));
            AddCustomAttributes(typeof(AutoCompleteBox), "IsDropDownOpen", new CategoryAttribute(DesignerProperties.Resources.AutoComplete));
            AddCustomAttributes(typeof(AutoCompleteBox), "IsTextCompletionEnabled", new CategoryAttribute(DesignerProperties.Resources.CommonProperties));
            AddCustomAttributes(typeof(AutoCompleteBox), "ItemFilter", new BrowsableAttribute(false));
            AddCustomAttributes(typeof(AutoCompleteBox), "ItemsSource", new NewItemTypesAttribute(typeof(string)));
            AddCustomAttributes(typeof(AutoCompleteBox), "MaxDropDownHeight", new CategoryAttribute(DesignerProperties.Resources.AutoComplete));
            AddCustomAttributes(typeof(AutoCompleteBox), "MinimumPopulateDelay", new CategoryAttribute(DesignerProperties.Resources.AutoComplete));
            AddCustomAttributes(typeof(AutoCompleteBox), "MinimumPrefixLength", new CategoryAttribute(DesignerProperties.Resources.AutoComplete));
            AddCustomAttributes(typeof(AutoCompleteBox), "Text", new CategoryAttribute(DesignerProperties.Resources.CommonProperties));
            AddCustomAttributes(typeof(AutoCompleteBox), "TextFilter", new BrowsableAttribute(false));
            AddCustomAttributes(typeof(AutoCompleteBox), "ValueMemberBinding", new CategoryAttribute(DesignerProperties.Resources.AutoComplete));
            AddCustomAttributes(typeof(AutoCompleteBox), "ValueMemberPath", new CategoryAttribute(DesignerProperties.Resources.AutoComplete));
            AddCustomAttributes(typeof(AutoCompleteBox), "Watermark", new CategoryAttribute(DesignerProperties.Resources.AutoComplete));
            AddCustomAttributes(typeof(AutoCompleteBox), "Watermark", new DescriptionAttribute("Gets or sets the string that appears in the TextBox when it has no input and does not have focus."));
#else
            AddCallback(
                typeof(SSWC.AutoCompleteBox),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.TextFilter),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.ItemFilter),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        "ValueMemberBinding",
                        new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.ValueMemberPath),
                        new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.MaxDropDownHeight),
                        new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.MinimumPopulateDelay),
                        new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.MinimumPrefixLength),
                        new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.IsDropDownOpen),
                        new CategoryAttribute(Properties.Resources.AutoComplete));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.FilterMode),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.Text),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.IsTextCompletionEnabled),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.ItemsSource),
                        new NewItemTypesAttribute(typeof(string)));
                    
#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.ItemTemplate),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.ValueMemberPath),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.SelectedItem),
                            false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.SelectedItem),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        "ValueMemberBinding",
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.SelectedItem),
                            true));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.ItemContainerStyle),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<Controls.AutoCompleteBox>(x => x.ItemsSource),
                            true));
#endif
                });
#endif
        }
    }
}