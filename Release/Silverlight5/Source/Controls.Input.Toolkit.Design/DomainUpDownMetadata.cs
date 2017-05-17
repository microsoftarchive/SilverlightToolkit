// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// To register design time metadata for SSWC.DomainUpDown.
    /// </summary>
    internal class DomainUpDownMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="DomainUpDownMetadata"/> class.
        /// </summary>
        public DomainUpDownMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DomainUpDown),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.IsCyclic),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.ItemsSource),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.ValueMemberPath),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        "ValueMemberBinding",
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.ItemTemplate),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.CurrentIndex),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.DomainUpDown>(x => x.ItemsSource),
                            true));
#endif
                });
        }
    }
}