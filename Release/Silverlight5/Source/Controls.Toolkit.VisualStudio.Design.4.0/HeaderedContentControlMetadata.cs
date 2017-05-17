// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for HeaderedContentControl.
    /// </summary>
    internal class HeaderedContentControlMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for HeaderedContentControl.
        /// </summary>
        public HeaderedContentControlMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.HeaderedContentControl),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.HeaderedContentControl>(x => x.Header)));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.HeaderedContentControl>(x => x.Header),
                        new TypeConverterAttribute(typeof(StringConverter)));
                });
        }
    }
}