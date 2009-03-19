// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Data.DataForm.Design
{
    /// <summary>
    /// To register design time metadata for ErrorSummary.
    /// </summary>
    public class ErrorSummaryMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for ErrorSummary.
        /// </summary>
        public ErrorSummaryMetadata()
            : base()
        {
            AddCallback(
                typeof(ErrorSummary),
                b =>
                {
                    b.AddCustomAttributes("FilteredErrors", new BrowsableAttribute(false));
                    b.AddCustomAttributes("Errors", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<ErrorSummary>(x => x.Filter), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<ErrorSummary>(x => x.FocusControlsOnClick), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<ErrorSummary>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
