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
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for Expander.
    /// </summary>
    internal class ExpanderMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Expander.
        /// </summary>
        public ExpanderMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.Expander),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Expander>(x => x.Header),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Expander>(x => x.ExpandDirection),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Expander>(x => x.IsExpanded),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                        Extensions.GetMemberName<SSWC.Expander>(x => x.Header)));

                    b.AddCustomAttributes(new FeatureAttribute(typeof(ExpanderParentAdapter)));

#if MWD40
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Expander>(x => x.Content),
                        new AlternateContentPropertyAttribute());

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));

                    b.AddCustomAttributes(new FeatureAttribute(typeof(ExpanderIsExpandedDesignModeValueProvider)));
                    b.AddCustomAttributes(new FeatureAttribute(typeof(ExpanderIsExpandedDesignModeValueProvider.AdornerProxy)));
#endif
                });
        }
    }
}