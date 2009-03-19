//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

[assembly: ProvideMetadata(typeof(System.Windows.Controls.Data.Design.MetadataRegistration))]

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// MetadataRegistration class.
    /// </summary>
    public partial class MetadataRegistration : MetadataRegistrationBase, IProvideAttributeTable
    {
        /// <summary>
        /// Design time metadata registration class.
        /// </summary>
        public MetadataRegistration()
            : base()
        {
            AssemblyName asmName = typeof(DataGrid).Assembly.GetName();
            XmlResourceName = asmName.Name + ".Design." + asmName.Name + ".XML";
            AssemblyFullName = ", " + asmName.FullName;
        }

        /// <summary>
        /// Gets the AttributeTable for design time metadata.
        /// </summary>
        public AttributeTable AttributeTable
        {
            get
            {
                return BuildAttributeTable();
            }
        }
    }
}
