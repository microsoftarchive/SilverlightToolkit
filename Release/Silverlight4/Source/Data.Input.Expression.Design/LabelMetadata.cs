// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Data.Input.Design
{
    /// <summary>
    /// To register design time metadata for Label.
    /// </summary>
    internal class LabelMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Label.
        /// </summary>
        public LabelMetadata()
            : base()
        {
            AssemblyName asmName = typeof(SSWC.ValidationSummary).Assembly.GetName();
            string labelName = "System.Windows.Controls.Label" + ", " + asmName.FullName;

            AddCallback(
                Type.GetType(labelName),
                b =>
                {
                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute("Target"));

                    // DataContextValueSource
                    b.AddCustomAttributes("ContentTemplate", new DataContextValueSourceAttribute("Content", true));
                });
        }
    }
}
