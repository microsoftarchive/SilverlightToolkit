// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design.Metadata;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Features;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataPager.
    /// </summary>
    internal class DataPagerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataPager.
        /// </summary>
        public DataPagerMetadata()
            : base()
        {
            AddCallback(
                SilverlightTypes.DataPager, b =>
                {
                    // Defaults
                    GenericDefaultInitializer.AddDefault(SilverlightTypes.DataPager, "Height", 26d);
                    GenericDefaultInitializer.AddDefault(SilverlightTypes.DataPager, "Width", 200d);
                    GenericDefaultInitializer.AddDefault(SilverlightTypes.DataPager, "PageSize", 10);

                    b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer)));

                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute("Source"));
                    b.AddCustomAttributes(new DefaultEventAttribute("PageIndexChanged"));
                    b.AddCustomAttributes(new DefaultPropertyAttribute("Source"));

                    // Common
                    b.AddCustomAttributes(PlatformTypes.DataPager.PageIndexProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));

                    // Appearance
                    b.AddCustomAttributes(PlatformTypes.DataPager.NumericButtonCountProperty.Name, new CategoryAttribute(Properties.Resources.Appearance));
                });
        }
    }
}
