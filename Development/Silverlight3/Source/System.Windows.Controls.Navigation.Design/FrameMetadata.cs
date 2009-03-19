// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Navigation.Design
{
    /// <summary>
    /// To register design time metadata for Frame.
    /// </summary>
    public class FrameMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Frame.
        /// </summary>
        public FrameMetadata()
            : base()
        {
            AddCallback(
                typeof(Frame),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<Frame>(x => x.JournalOwnership), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("Source", new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
