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
using System.Reflection;

namespace System.Windows.Controls.Data.Input.Design
{
    /// <summary>
    /// To register design time metadata for ValidationSummaryItem.
    /// </summary>
    internal class ValidationSummaryItemMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for ValidationSummaryItem.
        /// </summary>
        public ValidationSummaryItemMetadata()
            : base()
        {
            AssemblyName asmName = typeof(SSWC.ValidationSummary).Assembly.GetName();
            string validationSummaryItemName = "System.Windows.Controls.ValidationSummaryItem" + ", " + asmName.FullName;

            AddCallback(
                Type.GetType(validationSummaryItemName),
                b =>
                {
                    // Common
                    b.AddCustomAttributes("ItemType", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("Message", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("MessageHeader", new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
