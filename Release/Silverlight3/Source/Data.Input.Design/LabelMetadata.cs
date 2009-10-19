// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
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
                    // Common
                    b.AddCustomAttributes("Content", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("PropertyPath", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("Target", new CategoryAttribute(Properties.Resources.CommonProperties));

#if MWD40
                    // DataContextValueSource
                    b.AddCustomAttributes("ContentTemplate", new DataContextValueSourceAttribute("Content", true));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<Type.GetType(labelName)>(x => x.PropertyPath),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<Type.GetType(labelName)>(x => x.Target),
                            false));
#endif
                });
        }
    }
}
