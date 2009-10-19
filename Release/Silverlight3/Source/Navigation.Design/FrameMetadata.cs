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
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.Frame>(x => x.CacheSize), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.Frame>(x => x.JournalOwnership), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("Source", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute("Source"));

#if MWD40
                    b.AddCustomAttributes(new FeatureAttribute(typeof(EmptyDefaultInitializer)));
#endif

                });
        }
    }
}
