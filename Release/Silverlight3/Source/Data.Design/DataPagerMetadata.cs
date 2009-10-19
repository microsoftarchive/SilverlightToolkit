// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using System.Windows.Controls.Data.Design;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataPager.
    /// </summary>
    internal class DataPagerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataPager.
        /// </summary>
        public DataPagerMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataPager), b =>
                {
                    // Common
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataPager>(x => x.AutoEllipsis), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataPager>(x => x.DisplayMode), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataPager>(x => x.PageSize), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("Source", new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
