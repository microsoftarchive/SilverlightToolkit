// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWCT = Silverlight::System.Windows.Controls.Theming;

namespace System.Windows.Controls.Theming.Design
{
    /// <summary>
    /// To register design time metadata for Theme.
    /// </summary>
    internal class ThemeMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Theme.
        /// </summary>
        public ThemeMetadata()
            : base()
        {
        }
    }
}
