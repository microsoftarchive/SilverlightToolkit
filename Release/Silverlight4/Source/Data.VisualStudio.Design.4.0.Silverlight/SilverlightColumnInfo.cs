// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Reflection;

namespace System.Windows.Controls.Data.Design.Silverlight
{
    /// <summary>
    /// Design time column information for the Silverlight DataGrid
    /// </summary>
    internal class SilverlightColumnInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerFromAttribute"></param>
        /// <param name="propertyInfo"></param>
        public SilverlightColumnInfo(string headerFromAttribute, PropertyInfo propertyInfo)
        {
            this.HeaderFromAttribute = headerFromAttribute;
            this.PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Header that would be populated from the DisplayAttribute
        /// </summary>
        public string HeaderFromAttribute
        {
            get;
            private set;
        }

        /// <summary>
        /// PropertyInfo for the column information
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }
    }
}
