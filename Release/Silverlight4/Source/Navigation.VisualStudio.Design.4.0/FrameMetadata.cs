// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Navigation.Design
{
    /// <summary>
    /// To register design time metadata for Frame.
    /// </summary>
    internal class FrameMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Frame.
        /// </summary>
        public FrameMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.Frame),
                b =>
                {
                    // Defaults
                    GenericDefaultInitializer.AddDefault<SSWC.Frame>(x => x.Height, 100d);
                    GenericDefaultInitializer.AddDefault<SSWC.Frame>(x => x.Width, 200d);

                    b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer)));

                    b.AddCustomAttributes(new DefaultEventAttribute("Navigated"));
                    b.AddCustomAttributes(new DefaultPropertyAttribute("Source"));

                    b.AddCustomAttributes(new TypeConverterAttribute(typeof(ExpandableObjectConverter)));
                });
        }
    }
}
