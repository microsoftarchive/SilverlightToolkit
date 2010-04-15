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

namespace System.Windows.Controls.Navigation.Design
{
    /// <summary>
    /// To register design time metadata for Page.
    /// </summary>
    internal class PageMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Page.
        /// </summary>
        public PageMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.Page),
                b =>
                {
                    // Defaults
                    GenericDefaultInitializer.AddDefault<SSWC.Page>(x => x.Height, 100d);
                    GenericDefaultInitializer.AddDefault<SSWC.Page>(x => x.Width, 200d);

                    b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer)));
                    
                    b.AddCustomAttributes(new DefaultPropertyAttribute(Extensions.GetMemberName<SSWC.Page>(x => x.Title)));
                });
        }
    }
}
