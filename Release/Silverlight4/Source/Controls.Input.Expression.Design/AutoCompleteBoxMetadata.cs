// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

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
                typeof(SSWC.AutoCompleteBox),
                b =>
                {
                    b.AddCustomAttributes(
                        new DefaultBindingPropertyAttribute(
                            Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.ItemsSource)));
                });
        }
    }
}