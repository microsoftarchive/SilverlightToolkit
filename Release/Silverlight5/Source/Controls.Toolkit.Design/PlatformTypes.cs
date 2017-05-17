// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// Platform independent type and property references. 
    /// Enables the creation and manipulation of types and properties 
    /// via the Model without a direct reference to Silverlight. 
    /// This allows the design time implementation to
    /// 1) cleanly use WPF to implement any design time UI.
    /// 2) target both WPF and Silverlight if required.
    /// </summary>
    internal static class PlatformTypes
    {
        /// <summary>
        /// DockPanel identifiers.
        /// </summary>
        public static class DockPanel
        {
            /// <summary>
            /// DockPanel TypeIdentifier.
            /// </summary>
            // Should use the xmlns here but that's different for SL & WPF 
            // so use the fully qualified type name instead
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.DockPanel");

            /// <summary>
            /// Dock PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier DockProperty = new PropertyIdentifier(TypeId, "Dock");
        }

        /// <summary>
        /// HeaderedContentControl identifiers.
        /// </summary>
        public class HeaderedContentControl : ContentControl
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            private HeaderedContentControl() { }

            /// <summary>
            /// HeaderedContentControl TypeIdentifier.
            /// </summary>
            public static readonly new TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.HeaderedContentControl");

            /// <summary>
            /// Header PropertyIdentifier.
            /// </summary>
            public static readonly PropertyIdentifier HeaderProperty = new PropertyIdentifier(TypeId, "Header");
        }

        /// <summary>
        /// ContentControl identifiers.
        /// </summary>
        public class ContentControl
        {
            /// <summary>
            /// ContentControl TypeIdentifier.
            /// </summary>
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.ContentControl");
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
        /// Panel identifiers.
        /// </summary>
        public static class Panel
        {
            /// <summary>
            /// Panel TypeIdentifier.
            /// </summary>
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Panel");
        }
    }
}
