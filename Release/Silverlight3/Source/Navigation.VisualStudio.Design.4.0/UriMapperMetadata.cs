// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using SSWN = Silverlight::System.Windows.Navigation;

namespace System.Windows.Controls.Navigation.Design
{
    /// <summary>
    /// To register design time metadata for UriMapper.
    /// </summary>
    internal class UriMapperMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for UriMapper.
        /// </summary>
        public UriMapperMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWN.UriMapper),
                b =>
                {
                    // Common
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWN.UriMapper>(x => x.UriMappings), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
