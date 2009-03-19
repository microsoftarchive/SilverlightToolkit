// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Input.Design
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
                    b.AddCustomAttributes(Extensions.GetMemberName<AutoCompleteBox>(x => x.ValueMemberBinding), new CategoryAttribute(Properties.Resources.AutoComplete));
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
