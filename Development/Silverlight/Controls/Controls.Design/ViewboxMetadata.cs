// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Microsoft.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace Microsoft.Windows.Controls.Design
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
                typeof(Viewbox),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<Viewbox>(x => x.BorderThickness), new BrowsableAttribute(false));
                    b.AddCustomAttributes(Extensions.GetMemberName<Viewbox>(x => x.BorderBrush), new BrowsableAttribute(false));
                    b.AddCustomAttributes(Extensions.GetMemberName<Viewbox>(x => x.Background), new BrowsableAttribute(false));
                    b.AddCustomAttributes(Extensions.GetMemberName<Viewbox>(x => x.Foreground), new BrowsableAttribute(false));

                    b.AddCustomAttributes(Extensions.GetMemberName<Viewbox>(x => x.Child), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<Viewbox>(x => x.Stretch), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<Viewbox>(x => x.StretchDirection), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
