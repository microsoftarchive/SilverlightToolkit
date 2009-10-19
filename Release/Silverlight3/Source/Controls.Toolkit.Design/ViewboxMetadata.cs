// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for Viewbox.
    /// </summary>
    internal class ViewboxMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Viewbox.
        /// </summary>
        public ViewboxMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.Viewbox),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.Background),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.BorderBrush),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.BorderThickness),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.Foreground),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.HorizontalContentAlignment),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.VerticalContentAlignment),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.Template),
                        new BrowsableAttribute(false));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.Child),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.Stretch),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.StretchDirection),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                        Extensions.GetMemberName<SSWC.Viewbox>(x => x.Content)));

#if MWD40
                    b.AddCustomAttributes(new FeatureAttribute(typeof(EmptyDefaultInitializer)));

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));
#endif
                });
        }
    }
}
