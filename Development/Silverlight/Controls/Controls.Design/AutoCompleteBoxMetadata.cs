// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Microsoft.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace Microsoft.Windows.Controls.Design
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
            AddCallback(
                typeof(AutoCompleteBox),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.TextFilter), new BrowsableAttribute(false));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.ItemFilter), new BrowsableAttribute(false));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.Converter), new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.ConverterCulture), new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.ConverterParameter), new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.MaxDropDownHeight), new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.MinimumPopulateDelay), new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.MinimumPrefixLength), new CategoryAttribute(Properties.Resources.AutoComplete));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.IsDropDownOpen), new CategoryAttribute(Properties.Resources.AutoComplete));

                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.SearchMode), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.Text), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.IsTextCompletionEnabled), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
