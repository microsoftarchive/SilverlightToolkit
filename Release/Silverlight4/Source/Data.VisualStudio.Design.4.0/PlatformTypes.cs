// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Data.Design
{

    /// <summary>
    ///     Platform independent type and property references. Enables the creation and manipulation of types and properties via
    ///     the Model without a direct reference to Silverlight. This allows (1) the design time implementation to cleanly use WPF 
    ///     to implement any design time UI and (2) allows the design time to target both WPF and Silverlight if required
    /// </summary>
    internal class PlatformTypes
    {
        private const string DefaultUrl = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        public class FrameworkElement
        {
            public static readonly TypeIdentifier TypeId = new TypeIdentifier(DefaultUrl, "FrameworkElement");
            public static readonly PropertyIdentifier HeightProperty = new PropertyIdentifier(TypeId, "Height");
            public static readonly PropertyIdentifier WidthProperty = new PropertyIdentifier(TypeId, "Width");
        }

        public class Control : FrameworkElement
        {
        }

        public class DataGrid : Control
        {
            // Private constructor to keep the compiler from generating a default one
            private DataGrid()
            {
            }

            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            new public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.DataGrid");
            public static readonly PropertyIdentifier AlternatingRowBackgroundProperty = new PropertyIdentifier(TypeId, "AlternatingRowBackground");
            public static readonly PropertyIdentifier AreRowDetailsFrozenProperty = new PropertyIdentifier(TypeId, "AreRowDetailsFrozen");
            public static readonly PropertyIdentifier AutoGenerateColumnsProperty = new PropertyIdentifier(TypeId, "AutoGenerateColumns");
            public static readonly PropertyIdentifier CanUserReorderColumnsProperty = new PropertyIdentifier(TypeId, "CanUserReorderColumns");
            public static readonly PropertyIdentifier CanUserResizeColumnsProperty = new PropertyIdentifier(TypeId, "CanUserResizeColumns");
            public static readonly PropertyIdentifier CanUserSortColumnsProperty = new PropertyIdentifier(TypeId, "CanUserSortColumns");
            public static readonly PropertyIdentifier CellStyleProperty = new PropertyIdentifier(TypeId, "CellStyle");
            public static readonly PropertyIdentifier ColumnHeaderHeightProperty = new PropertyIdentifier(TypeId, "ColumnHeaderHeight");
            public static readonly PropertyIdentifier ColumnHeaderStyleProperty = new PropertyIdentifier(TypeId, "ColumnHeaderStyle");
            public static readonly PropertyIdentifier ColumnsProperty = new PropertyIdentifier(TypeId, "Columns");
            public static readonly PropertyIdentifier ColumnWidthProperty = new PropertyIdentifier(TypeId, "ColumnWidth");
            public static readonly PropertyIdentifier CurrentColumnProperty = new PropertyIdentifier(TypeId, "CurrentColumn");
            public static readonly PropertyIdentifier DragIndicatorStyleProperty = new PropertyIdentifier(TypeId, "DragIndicatorStyle");
            public static readonly PropertyIdentifier DropLocationIndicatorStyleProperty = new PropertyIdentifier(TypeId, "DropLocationIndicatorStyle");
            public static readonly PropertyIdentifier FrozenColumnCountProperty = new PropertyIdentifier(TypeId, "FrozenColumnCount");
            public static readonly PropertyIdentifier GridLinesVisibilityProperty = new PropertyIdentifier(TypeId, "GridLinesVisibility");
            public static readonly PropertyIdentifier HeadersVisibilityProperty = new PropertyIdentifier(TypeId, "HeadersVisibility");
            public static readonly PropertyIdentifier HorizontalGridLinesBrushProperty = new PropertyIdentifier(TypeId, "HorizontalGridLinesBrush");
            public static readonly PropertyIdentifier HorizontalScrollBarVisibilityProperty = new PropertyIdentifier(TypeId, "HorizontalScrollBarVisibility");
            public static readonly PropertyIdentifier IsReadOnlyProperty = new PropertyIdentifier(TypeId, "IsReadOnly");
            public static readonly PropertyIdentifier ItemsSourceProperty = new PropertyIdentifier(TypeId, "ItemsSource");
            public static readonly PropertyIdentifier MaxColumnWidthProperty = new PropertyIdentifier(TypeId, "MaxColumnWidth");
            public static readonly PropertyIdentifier MinColumnWidthProperty = new PropertyIdentifier(TypeId, "MinColumnWidth");
            public static readonly PropertyIdentifier RowBackgroundProperty = new PropertyIdentifier(TypeId, "RowBackground");
            public static readonly PropertyIdentifier RowDetailsTemplateProperty = new PropertyIdentifier(TypeId, "RowDetailsTemplate");
            public static readonly PropertyIdentifier RowDetailsVisibilityModeProperty = new PropertyIdentifier(TypeId, "RowDetailsVisibilityMode");
            public static readonly PropertyIdentifier RowGroupHeaderStylesProperty = new PropertyIdentifier(TypeId, "RowGroupHeaderStyles");
            public static readonly PropertyIdentifier RowHeaderStyleProperty = new PropertyIdentifier(TypeId, "RowHeaderStyle");
            public static readonly PropertyIdentifier RowHeaderWidthProperty = new PropertyIdentifier(TypeId, "RowHeaderWidth");
            public static readonly PropertyIdentifier RowHeightProperty = new PropertyIdentifier(TypeId, "RowHeight");
            public static readonly PropertyIdentifier RowStyleProperty = new PropertyIdentifier(TypeId, "RowStyle");
            public static readonly PropertyIdentifier SelectedIndexProperty = new PropertyIdentifier(TypeId, "SelectedIndex");
            public static readonly PropertyIdentifier SelectedItemProperty = new PropertyIdentifier(TypeId, "SelectedItem");
            public static readonly PropertyIdentifier VerticalGridLinesBrushProperty = new PropertyIdentifier(TypeId, "VerticalGridLinesBrush");
            public static readonly PropertyIdentifier VerticalScrollBarVisibilityProperty = new PropertyIdentifier(TypeId, "VerticalScrollBarVisibility");
        }

        public class DataGridColumn
        {
            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.DataGridColumn");
            public static readonly PropertyIdentifier CanUserResizeProperty = new PropertyIdentifier(TypeId, "CanUserResize");
            public static readonly PropertyIdentifier CanUserReorderProperty = new PropertyIdentifier(TypeId, "CanUserReorder");
            public static readonly PropertyIdentifier CanUserSortProperty = new PropertyIdentifier(TypeId, "CanUserSort");
            public static readonly PropertyIdentifier CellStyleProperty = new PropertyIdentifier(TypeId, "CellStyle");
            public static readonly PropertyIdentifier DragIndicatorStyleProperty = new PropertyIdentifier(TypeId, "DragIndicatorStyle");
            public static readonly PropertyIdentifier HeaderProperty = new PropertyIdentifier(TypeId, "Header");
            public static readonly PropertyIdentifier HeaderStyleProperty = new PropertyIdentifier(TypeId, "HeaderStyle");
            public static readonly PropertyIdentifier IsReadOnlyProperty = new PropertyIdentifier(TypeId, "IsReadOnly");
            public static readonly PropertyIdentifier MaxWidthProperty = new PropertyIdentifier(TypeId, "MaxWidth");
            public static readonly PropertyIdentifier MinWidthProperty = new PropertyIdentifier(TypeId, "MinWidth");
            public static readonly PropertyIdentifier VisibilityProperty = new PropertyIdentifier(TypeId, "Visibility");
            public static readonly PropertyIdentifier WidthProperty = new PropertyIdentifier(TypeId, "Width");
        }

        public class DataGridBoundColumn : DataGridColumn
        {
            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            new public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.DataGridBoundColumn");
            public static readonly PropertyIdentifier BindingProperty = new PropertyIdentifier(TypeId, "Binding");
            public static readonly PropertyIdentifier ElementStyleProperty = new PropertyIdentifier(TypeId, "ElementStyle");
            public static readonly PropertyIdentifier EditingElementStyleProperty = new PropertyIdentifier(TypeId, "EditingElementStyle");
        }

        public class DataGridTextColumn : DataGridBoundColumn
        {
            // Private constructor to keep the compiler from generating a default one
            private DataGridTextColumn()
            {
            }

            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            new public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.DataGridTextColumn");
            public static readonly PropertyIdentifier FontFamilyProperty = new PropertyIdentifier(TypeId, "FontFamily");
            public static readonly PropertyIdentifier FontSizeProperty = new PropertyIdentifier(TypeId, "FontSize");
            public static readonly PropertyIdentifier FontStyleProperty = new PropertyIdentifier(TypeId, "FontStyle");
            public static readonly PropertyIdentifier FontWeightProperty = new PropertyIdentifier(TypeId, "FontWeight");
            public static readonly PropertyIdentifier ForegroundProperty = new PropertyIdentifier(TypeId, "Foreground");
        }

        public class DataGridCheckBoxColumn : DataGridBoundColumn
        {
            // Private constructor to keep the compiler from generating a default one
            private DataGridCheckBoxColumn()
            {
            }

            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            new public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.DataGridCheckBoxColumn");
            public static readonly PropertyIdentifier IsThreeStateProperty = new PropertyIdentifier(TypeId, "IsThreeState");
        }

        public class DataGridTemplateColumn : DataGridColumn
        {
            // Private constructor to keep the compiler from generating a default one
            private DataGridTemplateColumn()
            {
            }

            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            new public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.DataGridTemplateColumn");
            public static readonly PropertyIdentifier CellTemplateProperty = new PropertyIdentifier(TypeId, "CellTemplate");
            public static readonly PropertyIdentifier CellEditingTemplateProperty = new PropertyIdentifier(TypeId, "CellEditingTemplate");
            public static readonly PropertyIdentifier SortMemberPathProperty = new PropertyIdentifier(TypeId, "SortMemberPath");
        }

        public class DataGridColumnHeader
        {
            // Private constructor to keep the compiler from generating a default one
            private DataGridColumnHeader()
            {
            }

            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.Primitives.DataGridColumnHeader");
            public static readonly PropertyIdentifier SeparatorVisibilityProperty = new PropertyIdentifier(TypeId, "SeparatorVisibility");
        }
        public class DataGridRowHeader
        {
            // Private constructor to keep the compiler from generating a default one
            private DataGridRowHeader()
            {
            }

            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.Primitives.DataGridRowHeader");
            public static readonly PropertyIdentifier SeparatorVisibilityProperty = new PropertyIdentifier(TypeId, "SeparatorVisibility");
        }
        public class DataGridRow : Control
        {
            // Private constructor to keep the compiler from generating a default one
            private DataGridRow()
            {
            }

            //Should use the xmlns here but that's different for SL & WPF so use the fully qualified type name instead
            public static readonly PropertyIdentifier HeaderProperty = new PropertyIdentifier(TypeId, "Header");
        }

        public class Binding
        {
            // Private constructor to keep the compiler from generating a default one
            private Binding()
            {
            }

            public static readonly TypeIdentifier TypeId = new TypeIdentifier(DefaultUrl, "Binding");
            public static readonly PropertyIdentifier PathProperty = new PropertyIdentifier(TypeId, "Path");
        }

        public class DataPager : Control
        {
            // Private constructor to keep the compiler from generating a default one
            private DataPager()
            {
            }

            new public static readonly TypeIdentifier TypeId = new TypeIdentifier("System.Windows.Controls.DataPager");

            public static readonly PropertyIdentifier AutoEllipsisProperty = new PropertyIdentifier(TypeId, "AutoEllipsis");
            public static readonly PropertyIdentifier DisplayModeProperty = new PropertyIdentifier(TypeId, "DisplayMode");
            public static readonly PropertyIdentifier PageIndexProperty = new PropertyIdentifier(TypeId, "PageIndex");
            public static readonly PropertyIdentifier SourceProperty = new PropertyIdentifier(TypeId, "Source");
            public static readonly PropertyIdentifier NumericButtonCountProperty = new PropertyIdentifier(TypeId, "NumericButtonCount");
        }

        public class TextBlock : FrameworkElement
        {
            // Private constructor to keep the compiler from generating a default one
            private TextBlock()
            {
            }

            new public static readonly TypeIdentifier TypeId = new TypeIdentifier(DefaultUrl, "TextBlock");
            public static readonly PropertyIdentifier TextProperty = new PropertyIdentifier(TypeId, "Text");
        }

        public class TextBox : Control
        {
            // Private constructor to keep the compiler from generating a default one
            private TextBox()
            {
            }

            new public static readonly TypeIdentifier TypeId = new TypeIdentifier(DefaultUrl, "TextBlock");
            public static readonly PropertyIdentifier TextProperty = new PropertyIdentifier(TypeId, "Text");
        }

        public class PropertyPath
        {
            // Private constructor to keep the compiler from generating a default one
            private PropertyPath()
            {
            }

            public static readonly TypeIdentifier TypeId = new TypeIdentifier(DefaultUrl, "PropertyPath");
            public static readonly PropertyIdentifier PathProperty = new PropertyIdentifier(TypeId, "Path");
        }

        public class DataTemplate
        {
            // Private constructor to keep the compiler from generating a default one
            private DataTemplate()
            {
            }

            public static readonly TypeIdentifier TypeId = new TypeIdentifier(DefaultUrl, "DataTemplate");
        }
    }
}
