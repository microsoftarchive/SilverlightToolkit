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

namespace System.Windows.Controls.Data.Input.Design
{
    /// <summary>
    /// To register design time metadata for ValidationSummary.
    /// </summary>
    internal class ValidationSummaryMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for ValidationSummary.
        /// </summary>
        public ValidationSummaryMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.ValidationSummary),
                b =>
                {
                    // BrowsableAttribute
                    b.AddCustomAttributes("DisplayedErrors", new BrowsableAttribute(false));

                    // Common
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.ValidationSummary>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));

                    // DataContextValueSource
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.ValidationSummary>(x => x.ErrorStyle), new DataContextValueSourceAttribute("Errors", true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.ValidationSummary>(x => x.HeaderTemplate), new DataContextValueSourceAttribute(Extensions.GetMemberName<SSWC.ValidationSummary>(x => x.Header), false));
                });
        }
    }
}
