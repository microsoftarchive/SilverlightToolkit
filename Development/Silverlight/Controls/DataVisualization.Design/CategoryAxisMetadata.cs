// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Microsoft.Windows.Controls.DataVisualization.Charting;
using Microsoft.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace Microsoft.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// To register design time metadata for CategoryAxis.
    /// </summary>
    internal class CategoryAxisMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for CategoryAxis.
        /// </summary>
        public CategoryAxisMetadata()
            : base()
        {
            AddCallback(
                typeof(CategoryAxis),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<CategoryAxis>(x => x.SortOrder), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
