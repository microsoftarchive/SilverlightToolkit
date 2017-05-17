// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWCP = Silverlight::System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for GlobalCalendarItem.
    /// </summary>
    internal class GlobalCalendarItemMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for GlobalCalendarItem.
        /// </summary>
        public GlobalCalendarItemMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCP.GlobalCalendarItem),
                b =>
                {
#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.ControlParts, false));
#endif
                });
        }
    }
}
