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

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for BusyIndicator.
    /// </summary>
    internal class BusyIndicatorMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for BusyIndicator.
        /// </summary>
        public BusyIndicatorMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.BusyIndicator),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.BusyIndicator>(x => x.IsBusy),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.BusyIndicator>(x => x.BusyContent),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.BusyIndicator>(x => x.DisplayAfter),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.BusyIndicator>(x => x.BusyContent),
                        new TypeConverterAttribute(typeof(StringConverter)));

#if MWD40
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.BusyIndicator>(x => x.BusyContent),
                        new AlternateContentPropertyAttribute());

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));
#endif
                });
        }
    }
}