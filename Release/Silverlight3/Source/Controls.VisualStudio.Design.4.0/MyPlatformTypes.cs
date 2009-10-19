// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// Platform independent type and property references. 
    /// Enables the creation and manipulation of types and properties 
    /// via the Model without a direct reference to Silverlight. 
    /// This allows the design time implementation to
    /// 1) cleanly use WPF to implement any design time UI.
    /// 2) target both WPF and Silverlight if required.
    /// </summary>
    internal static class MyPlatformTypes
    {
        /// <summary>
        /// TabControl identifiers.
        /// </summary>
        public static class TabControl
        {
            /// <summary>
            /// TabControl TypeIdentifier.
            /// </summary>
            // Should use the xmlns here but that's different for SL & WPF 
            // so use the fully qualified type name instead
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.TabControl");

            /// <summary>
            /// SelectedIndex PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier SelectedIndexProperty = new PropertyIdentifier(TypeId, "SelectedIndex");
        }

        /// <summary>
        /// TabItem identifiers.
        /// </summary>
        public static class TabItem
        {
            /// <summary>
            /// TabItem TypeIdentifier.
            /// </summary>
            // Should use the xmlns here but that's different for SL & WPF 
            // so use the fully qualified type name instead
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.TabItem");

            /// <summary>
            /// Header PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier HeaderProperty = new PropertyIdentifier(TypeId, "Header");

            /// <summary>
            /// IsSelected PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier IsSelectedProperty = new PropertyIdentifier(TypeId, "IsSelected");
        }

        /// <summary>
        /// FrameworkElement identifiers.
        /// </summary>
        public static class FrameworkElement
        {
            /// <summary>
            /// FrameworkElement TypeIdentifier.
            /// </summary>
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "FrameworkElement");

            /// <summary>
            /// Height PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier HeightProperty = new PropertyIdentifier(TypeId, "Height");

            /// <summary>
            /// Width PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier WidthProperty = new PropertyIdentifier(TypeId, "Width");

            /// <summary>
            /// HorizontalAlignment PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier HorizontalAlignmentProperty = new PropertyIdentifier(TypeId, "HorizontalAlignment");

            /// <summary>
            /// Margin PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier MarginProperty = new PropertyIdentifier(TypeId, "Margin");

            /// <summary>
            /// VerticalAlignment PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier VerticalAlignmentProperty = new PropertyIdentifier(TypeId, "VerticalAlignment");
        }

        /// <summary>
        /// Grid identifiers.
        /// </summary>
        public static class Grid
        {
            /// <summary>
            /// Grid TypeIdentifier.
            /// </summary>
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Grid");
        }

        /// <summary>
        /// Panel identifiers.
        /// </summary>
        public static class Panel
        {
            /// <summary>
            /// Panel TypeIdentifier.
            /// </summary>
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Panel");
        }
        
        /// <summary>
        /// ContentControl identifiers.
        /// </summary>
        public static class ContentControl
        {
            /// <summary>
            /// ContentControl TypeIdentifier.
            /// </summary>
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "ContentControl");
        }
    }
}
