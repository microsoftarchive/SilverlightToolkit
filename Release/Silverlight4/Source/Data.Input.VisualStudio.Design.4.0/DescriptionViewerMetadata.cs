// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;
using Microsoft.Windows.Design.Features;

namespace System.Windows.Controls.Data.Input.Design
{
    /// <summary>
    /// To register design time metadata for DescriptionViewer.
    /// </summary>
    internal class DescriptionViewerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DescriptionViewer.
        /// </summary>
        public DescriptionViewerMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DescriptionViewer),
                b =>
                {
                    // Defaults
                    GenericDefaultInitializer.AddDefault<SSWC.DescriptionViewer>(x => x.Height, 15d);
                    GenericDefaultInitializer.AddDefault<SSWC.DescriptionViewer>(x => x.Width, 15d);

                    b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer)));

                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute("Target", Extensions.GetMemberName<SSWC.DescriptionViewer>(x => x.PropertyPath)));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(Extensions.GetMemberName<SSWC.DescriptionViewer>(x => x.Description)));
                });
        }
    }
}
