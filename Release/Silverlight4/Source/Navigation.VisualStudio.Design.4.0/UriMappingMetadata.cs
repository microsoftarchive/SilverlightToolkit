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
    /// To register design time metadata for UriMapping.
    /// </summary>
    internal class UriMappingMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for UriMapping.
        /// </summary>
        public UriMappingMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWN.UriMapping),
                b =>
                {
                    // Common
                    b.AddCustomAttributes("MappedUri", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("Uri", new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
