// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

namespace Microsoft.Phone.Controls.Design
{
    /// <summary>
    /// Provides design metadata.
    /// </summary>
    public class MetadataStore : IProvideAttributeTable
    {
        /// <summary>
        /// Stores the string used to refer to the "Common Properties" section.
        /// </summary>
        public static readonly string CommonProperties = "Common Properties";

        /// <summary>
        /// Gets the attribute table.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Simple method happens to be highly coupled.")]
        public AttributeTable AttributeTable
        {
            get
            {
                return new ToggleSwitchMetadata().CreateTable();
            }
        }
    }
}