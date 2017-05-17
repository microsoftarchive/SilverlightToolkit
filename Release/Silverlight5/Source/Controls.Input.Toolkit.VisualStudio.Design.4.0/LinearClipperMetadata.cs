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
using SSWCP = Silverlight::System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// To register design time metadata for Accordion.
    /// </summary>
    internal class LinearClipperMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Accordion.
        /// </summary>
        public LinearClipperMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCP.LinearClipper),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute("ExpandDirection"));
                });
        }
    }
}