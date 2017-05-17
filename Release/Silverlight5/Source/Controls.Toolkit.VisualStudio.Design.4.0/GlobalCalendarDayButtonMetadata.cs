// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using SSWCP = Silverlight::System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for GlobalCalendarDayButton.
    /// </summary>
    internal class GlobalCalendarDayButtonMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for GlobalCalendarDayButton.
        /// </summary>
        public GlobalCalendarDayButtonMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCP.GlobalCalendarDayButton),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWCP.GlobalCalendarDayButton>(x => x.Content)));
                });
        }
    }
}