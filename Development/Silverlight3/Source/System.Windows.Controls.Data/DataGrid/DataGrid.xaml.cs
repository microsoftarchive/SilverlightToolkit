// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Displays data in a customizable grid.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [TemplatePart(Name = DataGrid.DATAGRID_elementRowsPresenterName, Type = typeof(DataGridRowsPresenter))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementColumnHeadersPresenterName, Type = typeof(DataGridColumnHeadersPresenter))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementFrozenColumnScrollBarSpacerName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementErrorsListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementHorizontalScrollbarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = DataGrid.DATAGRID_elementVerticalScrollbarName, Type = typeof(ScrollBar))]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateInvalid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    public partial class DataGrid : Control
    {
        #region Constants

        private const string DATAGRID_elementRowsPresenterName = "RowsPresenter";
        private const string DATAGRID_elementColumnHeadersPresenterName = "ColumnHeadersPresenter";
        private const string DATAGRID_elementErrorsListBox = "ErrorsListBox";
        private const string DATAGRID_elementFrozenColumnScrollBarSpacerName = "FrozenColumnScrollBarSpacer";
        private const string DATAGRID_elementHorizontalScrollbarName = "HorizontalScrollbar";
        private const string DATAGRID_elementRowHeadersPresenterName = "RowHeadersPresenter";
        private const string DATAGRID_elementTopLeftCornerHeaderName = "TopLeftCornerHeader";
        private const string DATAGRID_elementTopRightCornerHeaderName = "TopRightCornerHeader";
        private const string DATAGRID_elementVerticalScrollbarName = "VerticalScrollbar";

        private const bool DATAGRID_defaultAutoGenerateColumns = true;
        internal const bool DATAGRID_defaultCanUserReorderColumns = true;
        internal const bool DATAGRID_defaultCanUserResizeColumns = true;
        internal const bool DATAGRID_defaultCanUserSortColumns = true;
        private const DataGridRowDetailsVisibilityMode DATAGRID_defaultRowDetailsVisibility = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        private const DataGridSelectionMode DATAGRID_defaultSelectionMode = DataGridSelectionMode.Extended;

        private const double DATAGRID_horizontalGridLinesThickness = 1;
        private const double DATAGRID_minimumRowHeaderWidth = 4;
        private const double DATAGRID_minimumColumnHeaderHeight = 4;
        private const double DATAGRID_maxHeadersThickness = 32768;

        private const double DATAGRID_defaultRowHeight = 22;
        internal const double DATAGRID_defaultRowGroupSublevelIndent = 20;
        private const double DATAGRID_defaultMinColumnWidth = 20;
        private const double DATAGRID_defaultMaxColumnWidth = double.PositiveInfinity;

        #endregion Constants

        #region Data

        // DataGrid Template Parts
        private DataGridRowsPresenter _rowsPresenter;
        private DataGridColumnHeadersPresenter _columnHeadersPresenter;
        private ListBox _errorsListBox;
        private ScrollBar _hScrollBar;
        private ScrollBar _vScrollBar;
        
        private byte _autoGeneratingColumnOperationCount;
        private bool _bindingValidationError;
        private IndexToValueTable<Visibility> _collapsedSlotsTable;
        private Popup _columnDropLocationIndicatorPopup;
        private Control _columnDropLocationIndicator;
        // this is a workaround only for the scenarios where we need it, it is not all encompassing nor always updated
        private UIElement _clickedElement;
        private Queue<DataGridColumn> _clickedErrorColumns;
        private DataGridCellCoordinates _currentCellCoordinates;
        // used to store the current column during a Reset
        private int _desiredCurrentColumnIndex;
        private int _editingColumnIndex;
        private RoutedEventArgs _editingEventArgs;
        private bool _flushCurrentCellChanged;
        private bool _focusEditingControl;
        private DataGridRow _focusedRow;
        private FrameworkElement _frozenColumnScrollBarSpacer;
        private ObservableCollection<GroupDescription> _groupDescriptions;
        // the sum of the widths in pixels of the scrolling columns preceding 
        // the first displayed scrolling column
        private double _horizontalOffset;
        private byte _horizontalScrollChangesIgnored;
        private bool _ignoreNextScrollBarsLayout;
        // Nth row of rows 0..N that make up the RowHeightEstimate
        private int _lastEstimatedRow;
        private List<DataGridRow> _loadedRows;
        // prevents reentry into the VerticalScroll event handler
        private bool _makeFirstDisplayedCellCurrentCellPending;
        private bool _measured;
        private int? _mouseOverRowIndex;    // -1 is used for the 'new row'
        // the number of pixels of the firstDisplayedScrollingCol which are not displayed
        private double _negHorizontalOffset;
        // the number of pixels of DisplayData.FirstDisplayedScrollingRow which are not displayed
        private int _noCurrentCellChangeCount;
        private int _noSelectionChangeCount;
        private DataGridColumn _previousCurrentColumn;
        private object _previousCurrentItem;
        private ObservableCollection<Style> _rowGroupHeaderStyles;
        // To figure out what the old RowGroupHeaderStyle was for each level, we need to keep a copy
        // of the list.  The old style important so we don't blow away styles set directly on the RowGroupHeader
        private List<Style> _rowGroupHeaderStylesOld;
        private double[] _rowGroupHeightsByLevel;
        private double _rowHeaderDesiredWidth;
        private DataGridSelectedItemsCollection _selectedItems;
        private SortDescriptionCollection _sortDescriptions;
        private IndexToValueTable<Visibility> _showDetailsTable;
        private bool _successfullyUpdatedSelection;
        private bool _temporarilyResetCurrentCell;
        private ContentControl _topLeftCornerHeader; 
        private ContentControl _topRightCornerHeader;
        private object _uneditedValue; // Represents the original current cell value at the time it enters editing mode. 
        private ObservableCollection<ValidationResult> _validationResults;
        private byte _verticalScrollChangesIgnored;

        // An approximation of the sum of the heights in pixels of the scrolling rows preceding 
        // the first displayed scrolling row.  Since the scrolled off rows are discarded, the grid
        // does not know their actual height. The heights used for the approximation are the ones
        // set as the rows were scrolled off.
        private double _verticalOffset;

        #endregion Data

        #region Events
        /// <summary>
        /// Occurs one time for each public, non-static property in the bound data type when the 
        /// <see cref="P:System.Windows.Controls.DataGrid.ItemsSource" /> property is changed and the 
        /// <see cref="P:System.Windows.Controls.DataGrid.AutoGenerateColumns" /> property is true.
        /// </summary>
        public event EventHandler<DataGridAutoGeneratingColumnEventArgs> AutoGeneratingColumn;

        /// <summary>
        /// Occurs before a cell or row enters editing mode. 
        /// </summary>
        public event EventHandler<DataGridBeginningEditEventArgs> BeginningEdit;

        /// <summary>
        /// Occurs after cell editing has ended.
        /// </summary>
        public event EventHandler<DataGridCellEditEndedEventArgs> CellEditEnded;

        /// <summary>
        /// Occurs immediately before cell editing has ended.
        /// </summary>
        public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;

        /// <summary>
        /// Occurs when the <see cref="P:System.Windows.Controls.DataGridColumn.DisplayIndex" /> 
        /// property of a column changes.
        /// </summary>
        public event EventHandler<DataGridColumnEventArgs> ColumnDisplayIndexChanged;

        /// <summary>
        /// Occurs when the user drops a column header that was being dragged using the mouse.
        /// </summary>
        public event EventHandler<DragCompletedEventArgs> ColumnHeaderDragCompleted;

        /// <summary>
        /// Occurs one or more times while the user drags a column header using the mouse. 
        /// </summary>
        public event EventHandler<DragDeltaEventArgs> ColumnHeaderDragDelta;

        /// <summary>
        /// Occurs when the user begins dragging a column header using the mouse. 
        /// </summary>
        public event EventHandler<DragStartedEventArgs> ColumnHeaderDragStarted;

        /// <summary>
        /// Raised when column reordering ends, to allow subscribers to clean up.
        /// </summary>
        public event EventHandler<DataGridColumnEventArgs> ColumnReordered;
        
        /// <summary>
        /// Raised when starting a column reordering action.  Subscribers to this event can
        /// set tooltip and caret UIElements, constrain tooltip position, indicate that
        /// a preview should be shown, or cancel reordering.
        /// </summary>
        public event EventHandler<DataGridColumnReorderingEventArgs> ColumnReordering;

        /// <summary>
        /// Occurs when a different cell becomes the current cell.
        /// </summary>
        public event EventHandler<EventArgs> CurrentCellChanged;

        /// <summary>
        /// Occurs after a <see cref="T:System.Windows.Controls.DataGridRow" /> 
        /// is instantiated, so that you can customize it before it is used.
        /// </summary>
        public event EventHandler<DataGridRowEventArgs> LoadingRow;

        /// <summary>
        /// Occurs when a new row details template is applied to a row, so that you can customize 
        /// the details section before it is used.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> LoadingRowDetails;

        /// <summary>
        /// Occurs before a DataGridRowGroupHeader header is used.
        /// </summary>
        public event EventHandler<DataGridRowGroupHeaderEventArgs> LoadingRowGroup;

        /// <summary>
        /// Occurs when a cell in a <see cref="T:System.Windows.Controls.DataGridTemplateColumn" /> enters editing mode.
        /// 
        /// </summary>
        public event EventHandler<DataGridPreparingCellForEditEventArgs> PreparingCellForEdit;

        /// <summary>
        /// Occurs when the <see cref="P:System.Windows.Controls.DataGrid.RowDetailsVisibilityMode" /> 
        /// property value changes.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> RowDetailsVisibilityChanged;

        /// <summary>
        /// Occurs when the row has been successfully committed or cancelled.
        /// </summary>
        public event EventHandler<DataGridRowEditEndedEventArgs> RowEditEnded;

        /// <summary>
        /// Occurs immediately before the row has been successfully committed or cancelled.
        /// </summary>
        public event EventHandler<DataGridRowEditEndingEventArgs> RowEditEnding;

        /// <summary>
        /// Occurs before a row group collapses
        /// </summary>
        public event EventHandler<DataGridRowGroupHeaderToggleEventArgs> RowGroupCollapsing;

        /// <summary>
        /// Occurs before a row group expands
        /// </summary>
        public event EventHandler<DataGridRowGroupHeaderToggleEventArgs> RowGroupExpanding;

        /// <summary>
        /// Occurs when the <see cref="P:System.Windows.Controls.DataGrid.SelectedItem" /> or 
        /// <see cref="P:System.Windows.Controls.DataGrid.SelectedItems" /> property value changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when a <see cref="T:System.Windows.Controls.DataGridRow" /> 
        /// object becomes available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowEventArgs> UnloadingRow;

        /// <summary>
        /// Occurs when the DataGridRowGroupHeader is available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowGroupHeaderEventArgs> UnloadingRowGroup;

        /// <summary>
        /// Occurs when a row details element becomes available for reuse.
        /// </summary>
        public event EventHandler<DataGridRowDetailsEventArgs> UnloadingRowDetails;

        #endregion Events

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGrid" /> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Justification="_minRowHeight should be 0.")]
        public DataGrid()
        {
            this.TabNavigation = KeyboardNavigationMode.Once;
            this.KeyDown += new KeyEventHandler(DataGrid_KeyDown);
            this.KeyUp += new KeyEventHandler(DataGrid_KeyUp);
            this.GotFocus += new RoutedEventHandler(DataGrid_GotFocus);
            this.LostFocus += new RoutedEventHandler(DataGrid_LostFocus);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(DataGrid_IsEnabledChanged);

            this._clickedErrorColumns = new Queue<DataGridColumn>();
            this._loadedRows = new List<DataGridRow>();
            this._selectedItems = new DataGridSelectedItemsCollection(this);
            this._rowGroupHeaderStyles = new ObservableCollection<Style>();
            this._rowGroupHeaderStyles.CollectionChanged += RowGroupHeaderStyles_CollectionChanged;
            this._rowGroupHeaderStylesOld = new List<Style>();
            this.RowGroupHeadersTable = new IndexToValueTable<DataGridRowGroupInfo>();
            this._validationResults = new ObservableCollection<ValidationResult>();

            this.DisplayData = new DataGridDisplayData(this);
            this.ColumnsInternal = CreateColumnsInstance();

            this.RowHeightEstimate = DATAGRID_defaultRowHeight;
            this.RowDetailsHeightEstimate = 0;
            this._rowHeaderDesiredWidth = 0;

            this.DataConnection = new DataGridDataConnection(this);
            this._showDetailsTable = new IndexToValueTable<Visibility>();
            this._collapsedSlotsTable = new IndexToValueTable<Visibility>();

            this.AnchorSlot = -1;
            this._lastEstimatedRow = -1;
            this._editingColumnIndex = -1;
            this._mouseOverRowIndex = null;
            this.CurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);

            this.RowGroupHeaderHeightEstimate = DATAGRID_defaultRowHeight;

            DefaultStyleKey = typeof(DataGrid);
        }

        #region Dependency Properties

        #region AlternatingRowBackground

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint the background of odd-numbered rows.
        /// </summary>
        /// <returns>
        /// The brush that is used to paint the background of odd-numbered rows. The default is a 
        /// <see cref="T:System.Windows.Media.SolidColorBrush" /> with a 
        /// <see cref="P:System.Windows.Media.SolidColorBrush.Color" /> value of white (ARGB value #00FFFFFF).
        /// </returns>
        public Brush AlternatingRowBackground
        {
            get { return GetValue(AlternatingRowBackgroundProperty) as Brush; }
            set { SetValue(AlternatingRowBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.AlternatingRowBackground" /> 
        /// dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DataGrid.AlternatingRowBackground" /> 
        /// dependency property.
        /// </returns>
        public static readonly DependencyProperty AlternatingRowBackgroundProperty = 
            DependencyProperty.Register(
                "AlternatingRowBackground", 
                typeof(Brush), 
                typeof(DataGrid), 
                new PropertyMetadata(OnAlternatingRowBackgroundPropertyChanged));

        private static void OnAlternatingRowBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureBackground();
            }
        }
        #endregion AlternatingRowBackground

        #region AreRowDetailsFrozen
        /// <summary>
        /// Gets or sets a value indicating whether the horizontal ScrollBar of the DataGrid affects the
        /// details section of a row.
        /// </summary>
        public bool AreRowDetailsFrozen
        {
            get { return (bool)GetValue(AreRowDetailsFrozenProperty); }
            set { SetValue(AreRowDetailsFrozenProperty, value); }
        }

        /// <summary>
        /// Identifies the AreRowDetailsFrozen dependency property.
        /// </summary>
        public static readonly DependencyProperty AreRowDetailsFrozenProperty =
            DependencyProperty.Register(
                "AreRowDetailsFrozen",
                typeof(bool),
                typeof(DataGrid),
                null);
        #endregion AreRowDetailsFrozen

        #region AreRowGroupHeadersFrozen
        /// <summary>
        /// Gets or sets a value indicating whether the horizontal ScrollBar of the DataGrid affects the
        /// details section of a row.
        /// </summary>
        public bool AreRowGroupHeadersFrozen
        {
            get { return (bool)GetValue(AreRowGroupHeadersFrozenProperty); }
            set { SetValue(AreRowGroupHeadersFrozenProperty, value); }
        }

        /// <summary>
        /// Identifies the AreRowDetailsFrozen dependency property.
        /// </summary>
        public static readonly DependencyProperty AreRowGroupHeadersFrozenProperty =
            DependencyProperty.Register(
                "AreRowGroupHeadersFrozen",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true, OnAreRowGroupHeadersFrozenPropertyChanged));

        private static void OnAreRowGroupHeadersFrozenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            ProcessFrozenColumnCount(dataGrid);

            // Update elements in the RowGroupHeader that were previously frozen
            if ((bool)e.NewValue)
            {
                if (dataGrid._rowsPresenter != null)
                {
                    foreach (UIElement element in dataGrid._rowsPresenter.Children)
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            groupHeader.ClearFrozenStates();
                        }
                    }
                }
            }
        }
        #endregion AreRowGroupHeadersFrozen

        #region AutoGenerateColumns
        /// <summary>
        /// Gets or sets a value indicating whether columns are created automatically when the ItemsSource 
        /// property is set.
        /// </summary>
        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the AutoGenerateColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register(
                "AutoGenerateColumns",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(OnAutoGenerateColumnsPropertyChanged));

        private static void OnAutoGenerateColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            bool value = (bool)e.NewValue;
            if (value)
            {
                dataGrid.RefreshRowsAndColumns();
            }
            else
            {
                dataGrid.RemoveAutoGeneratedColumns();
            }
        }
        #endregion AutoGenerateColumns

        #region CanUserReorderColumns
        /// <summary>
        /// Gets or sets a value indicating whether users can reorder columns.
        /// </summary>
        public bool CanUserReorderColumns
        {
            get { return (bool)GetValue(CanUserReorderColumnsProperty); }
            set { SetValue(CanUserReorderColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserReorderColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserReorderColumnsProperty =
            DependencyProperty.Register(
                "CanUserReorderColumns",
                typeof(bool),
                typeof(DataGrid),
                null);
        #endregion CanUserReorderColumns

        #region CanUserResizeColumns
        /// <summary>
        /// Gets or sets a value indicating whether users can resize columns.
        /// </summary>
        public bool CanUserResizeColumns
        {
            get { return (bool)GetValue(CanUserResizeColumnsProperty); }
            set { SetValue(CanUserResizeColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserResizeColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserResizeColumnsProperty =
            DependencyProperty.Register(
                "CanUserResizeColumns",
                typeof(bool),
                typeof(DataGrid),
                null);
        #endregion CanUserResizeColumns

        #region CanUserSortColumns
        /// <summary>
        /// Gets or sets a value indicating whether users can sort columns.
        /// </summary>
        public bool CanUserSortColumns
        {
            get { return (bool)GetValue(CanUserSortColumnsProperty); }
            set { SetValue(CanUserSortColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the CanUserSortColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserSortColumnsProperty =
            DependencyProperty.Register(
                "CanUserSortColumns",
                typeof(bool),
                typeof(DataGrid),
                null);
        #endregion CanUserSortColumns

        #region CellStyle
        /// <summary>
        /// Gets or sets the style used by cells when they are rendered.
        /// </summary>
        public Style CellStyle
        {
            get { return GetValue(CellStyleProperty) as Style; }
            set { SetValue(CellStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.CellStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellStyleProperty =
            DependencyProperty.Register(
                "CellStyle",
                typeof(Style),
                typeof(DataGrid),
                new PropertyMetadata(OnCellStylePropertyChanged));

        private static void OnCellStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                Style previousStyle = e.OldValue as Style;
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    foreach (DataGridCell cell in row.Cells)
                    {
                        cell.EnsureStyle(previousStyle);
                    }
                    row.FillerCell.EnsureStyle(previousStyle);
                }
                dataGrid.InvalidateRowHeightEstimate();
            }
        }
        #endregion CellStyle

        #region ColumnHeaderHeight
        /// <summary>
        /// Gets or sets the suggested height of the grid's column headers.
        /// </summary>
        public double ColumnHeaderHeight
        {
            get { return (double)GetValue(ColumnHeaderHeightProperty); }
            set { SetValue(ColumnHeaderHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnHeaderHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderHeightProperty =
            DependencyProperty.Register(
                "ColumnHeaderHeight",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnColumnHeaderHeightPropertyChanged));

        /// <summary>
        /// ColumnHeaderHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnHeaderHeight.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnColumnHeaderHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                double value = (double)e.NewValue;
                if (value < DATAGRID_minimumColumnHeaderHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "ColumnHeaderHeight", DATAGRID_minimumColumnHeaderHeight);
                }
                if (value > DATAGRID_maxHeadersThickness)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "ColumnHeaderHeight", DATAGRID_maxHeadersThickness);
                }
                dataGrid.InvalidateMeasure();
            }
        }
        #endregion ColumnHeaderHeight

        #region ColumnHeaderStyle
        /// <summary>
        /// Gets or sets the style used by column headers when they are rendered. 
        /// </summary>
        public Style ColumnHeaderStyle
        {
            get { return GetValue(ColumnHeaderStyleProperty) as Style; }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnHeaderStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register("ColumnHeaderStyle", typeof(Style), typeof(DataGrid), new PropertyMetadata(OnColumnHeaderStylePropertyChanged));

        private static void OnColumnHeaderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                Style previousStyle = e.OldValue as Style;
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    column.HeaderCell.EnsureStyle(previousStyle);
                }
                if (dataGrid.ColumnsInternal.FillerColumn != null)
                {
                    dataGrid.ColumnsInternal.FillerColumn.HeaderCell.EnsureStyle(previousStyle);
                }
            }
        }
        #endregion ColumnHeaderStyle

        #region ColumnWidth
        /// <summary>
        /// Gets or sets the width of the grid's columns.
        /// </summary>
        public DataGridLength ColumnWidth
        {
            get { return (DataGridLength)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the ColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register(
                "ColumnWidth",
                typeof(DataGridLength),
                typeof(DataGrid),
                new PropertyMetadata(DataGridLength.Auto, OnColumnWidthPropertyChanged));

        /// <summary>
        /// ColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            dataGrid.EnsureHorizontalLayout();
        }
        #endregion ColumnWidth

        #region DragIndicatorStyle

        /// <summary>
        /// Gets or sets the style that is used when rendering the drag indicator that is 
        /// displayed while dragging column headers.
        /// </summary>
        public Style DragIndicatorStyle
        {
            get { return GetValue(DragIndicatorStyleProperty) as Style; }
            set { SetValue(DragIndicatorStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.DragIndicatorStyle" /> 
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty DragIndicatorStyleProperty =
            DependencyProperty.Register(
                "DragIndicatorStyle",
                typeof(Style),
                typeof(DataGrid),
                null);
        #endregion DragIndicatorStyle

        #region DropLocationIndicatorStyle

        /// <summary>
        /// Gets or sets the style that is used when rendering the column headers.
        /// </summary>
        public Style DropLocationIndicatorStyle
        {
            get { return GetValue(DropLocationIndicatorStyleProperty) as Style; }
            set { SetValue(DropLocationIndicatorStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.DropLocationIndicatorStyle" /> 
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty DropLocationIndicatorStyleProperty =
            DependencyProperty.Register(
                "DropLocationIndicatorStyle",
                typeof(Style),
                typeof(DataGrid),
                null);
        #endregion DropLocationIndicatorStyle

        #region FrozenColumnCount

        /// <summary>
        /// Gets or sets the number of columns that the user cannot scroll horizontally.
        /// </summary>
        public int FrozenColumnCount
        {
            get { return (int)GetValue(FrozenColumnCountProperty); }
            set { SetValue(FrozenColumnCountProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.FrozenColumnCount" /> 
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty FrozenColumnCountProperty =
            DependencyProperty.Register(
                "FrozenColumnCount",
                typeof(int),
                typeof(DataGrid),
                new PropertyMetadata(OnFrozenColumnCountPropertyChanged));

        private static void OnFrozenColumnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                if ((int)e.NewValue < 0)
                {
                    dataGrid.SetValueNoCallback(DataGrid.FrozenColumnCountProperty, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "FrozenColumnCount", 0);
                }
                ProcessFrozenColumnCount(dataGrid);
            }
        }

        private static void ProcessFrozenColumnCount(DataGrid dataGrid)
        {
            dataGrid.CorrectColumnFrozenStates();
            dataGrid.ComputeScrollBarsLayout();

            dataGrid.InvalidateColumnHeadersArrange();
            dataGrid.InvalidateCellsArrange();
        }

        #endregion FrozenColumnCount

        #region GridLinesVisibility
        /// <summary>
        /// Gets or sets a value that indicates whether horizontal or vertical gridlines for 
        /// the inner cells should be displayed.
        /// </summary>
        public DataGridGridLinesVisibility GridLinesVisibility
        {
            get { return (DataGridGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the GridLines dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register(
                "GridLinesVisibility",
                typeof(DataGridGridLinesVisibility),
                typeof(DataGrid),
                new PropertyMetadata(OnGridLinesVisibilityPropertyChanged));

        /// <summary>
        /// GridLinesProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its GridLines.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnGridLinesVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    row.EnsureGridLines();
                    row.InvalidateHorizontalArrange();
                }
            }
        }
        #endregion GridLinesVisibility

        #region HeadersVisibility
        /// <summary>
        /// Gets or sets a value that indicates whether column or row headers should be displayed.
        /// </summary>
        public DataGridHeadersVisibility HeadersVisibility
        {
            get { return (DataGridHeadersVisibility)GetValue(HeadersVisibilityProperty); }
            set { SetValue(HeadersVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the HeadersVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadersVisibilityProperty =
            DependencyProperty.Register(
                "HeadersVisibility",
                typeof(DataGridHeadersVisibility),
                typeof(DataGrid),
                new PropertyMetadata(OnHeadersVisibilityPropertyChanged));

        /// <summary>
        /// HeadersVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HeadersVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHeadersVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                DataGridHeadersVisibility newValue = (DataGridHeadersVisibility)e.NewValue;
                DataGridHeadersVisibility oldValue = (DataGridHeadersVisibility)e.OldValue;

                Func<DataGridHeadersVisibility, DataGridHeadersVisibility, bool> hasFlags = (DataGridHeadersVisibility value, DataGridHeadersVisibility flags) => ((value & flags) == flags);

                bool newValueCols = hasFlags(newValue, DataGridHeadersVisibility.Column);
                bool newValueRows = hasFlags(newValue, DataGridHeadersVisibility.Row);
                bool oldValueCols = hasFlags(oldValue, DataGridHeadersVisibility.Column);
                bool oldValueRows = hasFlags(oldValue, DataGridHeadersVisibility.Row);

                // Columns
                if (newValueCols != oldValueCols)
                {
                    if (dataGrid._columnHeadersPresenter != null)
                    {
                        dataGrid.EnsureColumnHeadersVisibility();
                        if (!newValueCols)
                        {
                            dataGrid._columnHeadersPresenter.Measure(Size.Empty);
                        }
                        else
                        {
                            dataGrid.EnsureVerticalGridLines();
                        }
                        dataGrid.InvalidateMeasure();
                    }
                }

                // Rows
                if (newValueRows != oldValueRows)
                {
                    if (dataGrid._rowsPresenter != null)
                    {
                        foreach (FrameworkElement element in dataGrid._rowsPresenter.Children)
                        {
                            DataGridRow row = element as DataGridRow;
                            if (row != null)
                            {
                                row.EnsureHeaderStyleAndVisibility(null);
                                if (newValueRows)
                                {
                                    row.ApplyState(false /*animate*/);
                                    row.EnsureHeaderVisibility();
                                }
                            }
                            else
                            {
                                DataGridRowGroupHeader rowGroupHeader = element as DataGridRowGroupHeader;
                                if (rowGroupHeader != null)
                                {
                                    rowGroupHeader.EnsureHeaderStyleAndVisibility(null);
                                }
                            }
                        }
                        dataGrid.InvalidateRowHeightEstimate();
                        dataGrid.InvalidateRowsMeasure(true /*invalidateIndividualElements*/);
                    }
                }

                // 

                if (dataGrid._topLeftCornerHeader != null)
                {
                    dataGrid._topLeftCornerHeader.Visibility = newValueRows && newValueCols ? Visibility.Visible : Visibility.Collapsed;
                    if (dataGrid._topLeftCornerHeader.Visibility == Visibility.Collapsed)
                    {
                        dataGrid._topLeftCornerHeader.Measure(Size.Empty);
                    }
                }
            }
        }
        #endregion HeadersVisibility

        #region HorizontalGridLinesBrush
        /// <summary>
        /// Gets or sets a brush that describes the horizontal gridlines color.
        /// </summary>
        public Brush HorizontalGridLinesBrush
        {
            get { return GetValue(HorizontalGridLinesBrushProperty) as Brush; }
            set { SetValue(HorizontalGridLinesBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalGridLinesBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalGridLinesBrushProperty =
            DependencyProperty.Register(
                "HorizontalGridLinesBrush",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(OnHorizontalGridLinesBrushPropertyChanged));

        /// <summary>
        /// HorizontalGridLinesBrushProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HorizontalGridLinesBrush.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHorizontalGridLinesBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended() && dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    row.EnsureGridLines();
                }
            }
        }
        #endregion HorizontalGridLinesBrush

        #region HorizontalScrollBarVisibility
        /// <summary>
        /// Gets or sets a value that indicates whether a horizontal ScrollBar should be displayed.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                "HorizontalScrollBarVisibility",
                typeof(ScrollBarVisibility),
                typeof(DataGrid),
                new PropertyMetadata(OnHorizontalScrollBarVisibilityPropertyChanged));

        /// <summary>
        /// HorizontalScrollBarVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its HorizontalScrollBarVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnHorizontalScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended() &&
                (ScrollBarVisibility)e.NewValue != (ScrollBarVisibility)e.OldValue &&
                dataGrid._hScrollBar != null)
            {
                dataGrid.InvalidateMeasure();
            }
        }
        #endregion HorizontalScrollBarVisibility

        #region IsReadOnly
        /// <summary>
        /// Gets or sets a value indicating whether the user can edit the cells of the DataGrid control.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(OnIsReadOnlyPropertyChanged));

        /// <summary>
        /// IsReadOnlyProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its IsReadOnly.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                bool value = (bool)e.NewValue;
                if (value && !dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                {
                    dataGrid.CancelEdit(DataGridEditingUnit.Row, false /*raiseEvents*/);
                }
            }
        }
        #endregion IsReadOnly

        #region IsValid
        /// <summary>
        /// Gets a value indicating that the DataGrid is in a valid state. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (bool)GetValue(IsValidProperty);
            }
            internal set
            {
                if (value != this.IsValid)
                {
                    if (value)
                    {
                        VisualStates.GoToState(this, true, VisualStates.StateValid);
                    }
                    else
                    {
                        VisualStates.GoToState(this, true, VisualStates.StateInvalid, VisualStates.StateValid);
                    }
                    this.SetValueNoCallback(IsValidProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the IsValid dependency property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
                "IsValid",
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true, (OnIsValidPropertyChanged)));

        /// <summary>
        /// IsValidProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its IsValid.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                dataGrid.SetValueNoCallback(DataGrid.IsValidProperty, e.OldValue);
                throw DataGridError.DataGrid.UnderlyingPropertyIsReadOnly("IsValid");
            }
        }
        #endregion IsValid

        #region ItemsSource
        /// <summary>
        /// Gets or sets a collection used to generate the content of the DataGrid.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as IEnumerable; }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(DataGrid),
                new PropertyMetadata(OnItemsSourcePropertyChanged));

        /// <summary>
        /// ItemsSourceProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ItemsSource.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                Debug.Assert(dataGrid.DataConnection != null);

                if (dataGrid.LoadingOrUnloadingRow)
                {
                    dataGrid.SetValueNoCallback(ItemsSourceProperty, e.OldValue);
                    throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
                }

                // Try to commit edit on the old ItemsSource, but force a cancel if it fails
                dataGrid.SetValueNoCallback(ItemsSourceProperty, e.OldValue);
                if (!dataGrid.CommitEdit())
                {
                    dataGrid.CancelEdit(DataGridEditingUnit.Row, false);
                }
                dataGrid.SetValueNoCallback(ItemsSourceProperty, e.NewValue);

                dataGrid.DataConnection.UnWireEvents(dataGrid.DataConnection.DataSource);
                dataGrid.DataConnection.ClearDataProperties();

                // The old selected indexes are no longer relevant. There's a perf benefit from
                // updating the selected indexes with a null DataSource, because we know that all
                // of the previously selected indexes have been removed from selection
                dataGrid.DataConnection.DataSource = null;
                dataGrid._selectedItems.UpdateIndexes();
                dataGrid.CoerceSelectedItem();

                // Wrap an IList in a PagedCollectionView if it's not already one
                IEnumerable newItemsSource = (IEnumerable)e.NewValue;
                if (newItemsSource != null && !(newItemsSource is ICollectionView))
                {
                    dataGrid.DataConnection.DataSource = new PagedCollectionView(newItemsSource);
                }
                else
                {
                    dataGrid.DataConnection.DataSource = newItemsSource;
                }

                if (dataGrid.DataConnection.DataSource != null)
                {
                    ICollectionView collectionView = dataGrid.DataConnection.CollectionView;

                    // Setup the SortDescriptions
                    if (dataGrid._sortDescriptions != null)
                    {
                        if (collectionView != null && collectionView.CanSort && collectionView.SortDescriptions.Count == 0)
                        {
                            foreach (SortDescription sortDescription in dataGrid._sortDescriptions)
                            {
                                collectionView.SortDescriptions.Add(sortDescription);
                            }
                        }
                        dataGrid._sortDescriptions = null;
                    }

                    // Setup the GroupDescriptions
                    if (dataGrid._groupDescriptions != null)
                    {
                        if (collectionView != null && collectionView.CanGroup && collectionView.GroupDescriptions.Count == 0)
                        {
                            foreach (GroupDescription groupDescription in dataGrid._groupDescriptions)
                            {
                                collectionView.GroupDescriptions.Add(groupDescription);
                            }
                        }
                        dataGrid._groupDescriptions = null;
                    }

                    dataGrid.DataConnection.WireEvents(dataGrid.DataConnection.DataSource);
                }

                // Treat this like the DataGrid has never been measured because all calculations at
                // this point are invalid until the next layout cycle.  For instance, the ItemsSource
                // can be set when the DataGrid is not part of the visual tree
                dataGrid._measured = false;
                dataGrid.InvalidateMeasure();
            }
        }

        #endregion ItemsSource

        #region MaxColumnWidth
        /// <summary>
        /// Gets or sets the width of the grid's columns.
        /// </summary>
        public double MaxColumnWidth
        {
            get { return (double)GetValue(MaxColumnWidthProperty); }
            set { SetValue(MaxColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxColumnWidthProperty =
            DependencyProperty.Register(
                "MaxColumnWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultMaxColumnWidth, OnMaxColumnWidthPropertyChanged));

        /// <summary>
        /// MaxColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMaxColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            double newValue = (double)e.NewValue;

            if (double.IsNaN(newValue))
            {
                dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MaxColumnWidth");
            }
            if (newValue < 0)
            {
                dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxColumnWidth", 0);
            }
            if (dataGrid.MinColumnWidth > newValue)
            {
                dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxColumnWidth", "MinColumnWidth");
            }
            dataGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
            dataGrid.InvalidateColumnHeadersMeasure();
            dataGrid.InvalidateRowsMeasure(true);
        }
        #endregion MaxColumnWidth

        #region MinColumnWidth
        /// <summary>
        /// Gets or sets the width of the grid's columns.
        /// </summary>
        public double MinColumnWidth
        {
            get { return (double)GetValue(MinColumnWidthProperty); }
            set { SetValue(MinColumnWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the MinColumnWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MinColumnWidthProperty =
            DependencyProperty.Register(
                "MinColumnWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(DATAGRID_defaultMinColumnWidth, OnMinColumnWidthPropertyChanged));

        /// <summary>
        /// MinColumnWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its ColumnWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMinColumnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            double newValue = (double)e.NewValue;

            if (double.IsNaN(newValue))
            {
                dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MinColumnWidth");
            }
            if (newValue < 0)
            {
                dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MinColumnWidth", 0);
            }
            if (double.IsPositiveInfinity(newValue))
            {
                dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("MinColumnWidth");
            }
            if (dataGrid.MaxColumnWidth < newValue)
            {
                dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "MinColumnWidth", "MaxColumnWidth");
            }
            dataGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
            dataGrid.InvalidateColumnHeadersMeasure();
            dataGrid.InvalidateRowsMeasure(true);
        }
        #endregion MinColumnWidth

        #region RowBackground
        /// <summary>
        /// Gets or sets a brush that describes the background of a row in the grid.
        /// </summary>
        public Brush RowBackground
        {
            get { return GetValue(RowBackgroundProperty) as Brush; }
            set { SetValue(RowBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowBackground" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowBackgroundProperty = DependencyProperty.Register("RowBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(OnRowBackgroundPropertyChanged));

        private static void OnRowBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            // Go through the Displayed rows and update the background
            foreach (DataGridRow row in dataGrid.GetAllRows())
            {
                row.EnsureBackground();
            }
        }
        #endregion RowBackground

        #region RowDetailsTemplate
        /// <summary>
        /// Gets or sets the DataTemplate used to display the details section of a row.
        /// </summary>
        public DataTemplate RowDetailsTemplate
        {
            get { return GetValue(RowDetailsTemplateProperty) as DataTemplate; }
            set { SetValue(RowDetailsTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the RowDetailsTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty RowDetailsTemplateProperty =
            DependencyProperty.Register(
                "RowDetailsTemplate",
                typeof(DataTemplate),
                typeof(DataGrid),
                new PropertyMetadata(OnRowDetailsTemplatePropertyChanged));

        private static void OnRowDetailsTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            // Update the RowDetails templates if necessary
            if (dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    if (dataGrid.GetRowDetailsVisibility(row.Index) == Visibility.Visible)
                    {
                        // DetailsPreferredHeight is initialized when the DetailsElement's size changes.
                        row.ApplyDetailsTemplate(false /*initializeDetailsPreferredHeight*/);
                    }
                }
            }

            dataGrid.UpdateRowDetailsHeightEstimate();
            dataGrid.InvalidateMeasure();
        }

        #endregion RowDetailsTemplate

        #region RowDetailsVisibilityMode
        /// <summary>
        /// Gets or sets a value that indicates when the details section of a row should be displayed.
        /// </summary>
        // 
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode
        {
            get { return (DataGridRowDetailsVisibilityMode)GetValue(RowDetailsVisibilityModeProperty); }
            set { SetValue(RowDetailsVisibilityModeProperty, value); }
        }

        /// <summary>
        /// Identifies the RowDetailsVisibilityMode dependency property.
        /// </summary>
        public static readonly DependencyProperty RowDetailsVisibilityModeProperty =
            DependencyProperty.Register(
                "RowDetailsVisibilityMode",
                typeof(DataGridRowDetailsVisibilityMode),
                typeof(DataGrid),
                new PropertyMetadata(OnRowDetailsVisibilityModePropertyChanged));

        /// <summary>
        /// RowDetailsVisibilityModeProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowDetailsVisibilityMode.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnRowDetailsVisibilityModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            dataGrid.UpdateRowDetailsVisibilityMode((DataGridRowDetailsVisibilityMode)e.NewValue);
        }
        #endregion RowDetailsVisibilityMode

        #region RowHeight
        /// <summary>
        /// Gets or sets the suggested height of the grid's rows.
        /// </summary>
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the RowHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(
                "RowHeight",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnRowHeightPropertyChanged));

        /// <summary>
        /// RowHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowHeight.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "This parameter is exposed to the user as a 'RowHeight' dependency property.")]
        private static void OnRowHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            if (!dataGrid.AreHandlersSuspended())
            {
                double value = (double)e.NewValue;

                if (value < DataGridRow.DATAGRIDROW_minimumHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "RowHeight", 0);
                }
                if (value > DataGridRow.DATAGRIDROW_maximumHeight)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "RowHeight", DataGridRow.DATAGRIDROW_maximumHeight);
                }

                dataGrid.InvalidateRowHeightEstimate();
                // Re-measure all the rows due to the Height change
                dataGrid.InvalidateRowsMeasure(true);
                // DataGrid needs to update the layout information and the ScrollBars
                dataGrid.InvalidateMeasure();
            }
        }
        #endregion RowHeight

        #region RowHeaderWidth
        /// <summary>
        /// Gets or sets the width of the grid's row headers.
        /// </summary>
        public double RowHeaderWidth
        {
            get { return (double)GetValue(RowHeaderWidthProperty); }
            set { SetValue(RowHeaderWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the RowHeaderWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderWidthProperty =
            DependencyProperty.Register(
                "RowHeaderWidth",
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(double.NaN, OnRowHeaderWidthPropertyChanged));

        /// <summary>
        /// RowHeaderWidthProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its RowHeaderWidth.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnRowHeaderWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                double value = (double)e.NewValue;

                if (value < DATAGRID_minimumRowHeaderWidth)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "RowHeaderWidth", DATAGRID_minimumRowHeaderWidth);
                }
                if (value > DATAGRID_maxHeadersThickness)
                {
                    dataGrid.SetValueNoCallback(e.Property, e.OldValue);
                    throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "RowHeaderWidth", DATAGRID_maxHeadersThickness);
                }
                dataGrid.EnsureRowHeaderWidth();
            }
        }
        #endregion RowHeaderWidth

        #region RowHeaderStyle
        /// <summary>
        /// Gets or sets the style used by row headers when they are rendered. 
        /// </summary>
        public Style RowHeaderStyle
        {
            get { return GetValue(RowHeaderStyleProperty) as Style; }
            set { SetValue(RowHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowHeaderStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderStyleProperty = DependencyProperty.Register("RowHeaderStyle", typeof(Style), typeof(DataGrid), new PropertyMetadata(OnRowHeaderStylePropertyChanged));

        private static void OnRowHeaderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null && dataGrid._rowsPresenter != null)
            {
                // Set HeaderStyle for displayed rows
                Style previousStyle = e.OldValue as Style;
                foreach (UIElement element in dataGrid._rowsPresenter.Children)
                {
                    DataGridRow row = element as DataGridRow;
                    if (row != null)
                    {
                        row.EnsureHeaderStyleAndVisibility(previousStyle);
                    }
                    else
                    {
                        DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                        if (groupHeader != null)
                        {
                            groupHeader.EnsureHeaderStyleAndVisibility(previousStyle);
                        }
                    }
                }
                dataGrid.InvalidateRowHeightEstimate();
            }
        }
        #endregion RowHeaderStyle

        #region RowStyle
        /// <summary>
        /// Gets or sets the style used by rows when they are rendered.
        /// </summary>
        public Style RowStyle
        {
            get { return GetValue(RowStyleProperty) as Style; }
            set { SetValue(RowStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DataGrid.RowStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowStyleProperty =
            DependencyProperty.Register(
                "RowStyle",
                typeof(Style),
                typeof(DataGrid),
                new PropertyMetadata(OnRowStylePropertyChanged));

        private static void OnRowStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                if (dataGrid._rowsPresenter != null)
                {
                    // Set the style for displayed rows if it has not already been set
                    foreach (DataGridRow row in dataGrid.GetAllRows())
                    {
                        EnsureElementStyle(row, e.OldValue as Style, e.NewValue as Style);
                    }
                }
                dataGrid.InvalidateRowHeightEstimate();
            }
        }
        #endregion RowStyle

        #region SelectionMode
        /// <summary>
        /// Gets or sets the selection behavior for a DataGrid.
        /// </summary>
        public DataGridSelectionMode SelectionMode
        {
            get { return (DataGridSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(DataGridSelectionMode),
                typeof(DataGrid),
                new PropertyMetadata(OnSelectionModePropertyChanged));

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectionMode.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                dataGrid.ClearRowSelection(true /*resetAnchorSlot*/);
            }
        }
        #endregion SelectionMode

        #region SelectedIndex
        /// <summary>
        /// Gets or sets the index of the current selection or returns -1 if the selection is empty.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(DataGrid),
                new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        /// <summary>
        /// SelectedIndexProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectedIndex.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended())
            {
                int index = (int)e.NewValue;
                
                // GetDataItem returns null if index is >= Count, we do not check newValue 
                // against Count here to avoid enumerating through an Enumerable twice
                // Setting SelectedItem coerces the finally value of the SelectedIndex
                object newSelectedItem = (index < 0) ? null : dataGrid.DataConnection.GetDataItem(index);
                dataGrid.SelectedItem = newSelectedItem;
                if (dataGrid.SelectedItem != newSelectedItem)
                {
                    d.SetValueNoCallback(e.Property, e.OldValue);
                }
            }
        }
        #endregion SelectedIndex

        #region SelectedItem
        /// <summary>
        /// Gets or sets the first item in the current selection or returns null if the selection is empty.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as object; }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(DataGrid),
                new PropertyMetadata(OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its SelectedItem.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            if (!dataGrid.AreHandlersSuspended())
            {
                int rowIndex = (e.NewValue == null) ? -1 : dataGrid.DataConnection.IndexOf(e.NewValue);
                if (rowIndex == -1)
                {
                    // If the Item is null or it's not found, clear the Selection
                    if (!dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                    {
                        // Edited value couldn't be committed or aborted
                        d.SetValueNoCallback(e.Property, e.OldValue);
                        return;
                    }

                    // Clear all row selections
                    dataGrid.ClearRowSelection(true /*resetAnchorSlot*/);
                }
                else
                {
                    dataGrid.SetValueNoCallback(DataGrid.SelectedIndexProperty, rowIndex);
                    int slot = dataGrid.SlotFromRowIndex(rowIndex);
                    if (slot != dataGrid.CurrentSlot)
                    {
                        if (!dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
                        {
                            // Edited value couldn't be committed or aborted
                            d.SetValueNoCallback(e.Property, e.OldValue);
                            return;
                        }
                        if (slot >= dataGrid.SlotCount || slot < -1)
                        {
                            if (dataGrid.DataConnection.CollectionView != null)
                            {
                                dataGrid.DataConnection.CollectionView.MoveCurrentToPosition(rowIndex);
                            }
                            return;
                        }
                    }

                    int oldCurrentSlot = dataGrid.CurrentSlot;
                    try
                    {
                        dataGrid._noSelectionChangeCount++;
                        int columnIndex = dataGrid.CurrentColumnIndex;

                        if (columnIndex == -1)
                        {
                            columnIndex = dataGrid.FirstDisplayedNonFillerColumnIndex;
                        }
                        if (dataGrid.IsSlotOutOfBounds(slot))
                        {
                            dataGrid.ClearRowSelection(slot /*slotException*/, true /*resetAnchorSlot*/);
                            return;
                        }

                        dataGrid.UpdateSelectionAndCurrency(columnIndex, slot, DataGridSelectionAction.SelectCurrent, false /*scrollIntoView*/);
                    }
                    finally
                    {
                        dataGrid.NoSelectionChangeCount--;
                    }

                    if (!dataGrid._successfullyUpdatedSelection)
                    {
                        dataGrid.SetValueNoCallback(DataGrid.SelectedIndexProperty, oldCurrentSlot);
                        d.SetValueNoCallback(e.Property, e.OldValue);
                    }
                }
            }
        }
        #endregion SelectedItem

        #region VerticalGridLinesBrush
        /// <summary>
        /// Gets or sets a brush that describes the vertical gridlines color.
        /// </summary>
        public Brush VerticalGridLinesBrush
        {
            get { return GetValue(VerticalGridLinesBrushProperty) as Brush; }
            set { SetValue(VerticalGridLinesBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalGridLinesBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalGridLinesBrushProperty =
            DependencyProperty.Register(
                "VerticalGridLinesBrush",
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(OnVerticalGridLinesBrushPropertyChanged));

        /// <summary>
        /// VerticalGridLinesBrushProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its VerticalGridLinesBrush.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnVerticalGridLinesBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended() && dataGrid._rowsPresenter != null)
            {
                foreach (DataGridRow row in dataGrid.GetAllRows())
                {
                    row.EnsureGridLines();
                }
            }
        }
        #endregion VerticalGridLinesBrush

        #region VerticalScrollBarVisibility
        /// <summary>
        /// Gets or sets a value that indicates whether a vertical ScrollBar should be displayed.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                "VerticalScrollBarVisibility",
                typeof(ScrollBarVisibility),
                typeof(DataGrid),
                new PropertyMetadata(OnVerticalScrollBarVisibilityPropertyChanged));

        /// <summary>
        /// VerticalScrollBarVisibilityProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGrid that changed its VerticalScrollBarVisibility.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnVerticalScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            if (!dataGrid.AreHandlersSuspended() &&
                (ScrollBarVisibility)e.NewValue != (ScrollBarVisibility)e.OldValue &&
                dataGrid._vScrollBar != null)
            {
                dataGrid.InvalidateMeasure();
            }
        }
        #endregion VerticalScrollBarVisibility

        #endregion Dependency Properties

        #region Public Properties    

        // 

        /// <summary>
        /// Gets the collection of columns currently present in the DataGrid.
        /// </summary>      
        public ObservableCollection<DataGridColumn> Columns
        {
            get
            {
                // we use a backing field here because the field's type
                // is a subclass of the property's
                return this.ColumnsInternal;
            }
        }

        /// <summary>
        /// Gets or sets the column that contains the cell that will go into 
        /// editing mode.
        /// </summary>
        public DataGridColumn CurrentColumn
        {
            get
            {
                if (this.CurrentColumnIndex == -1)
                {
                    return null;
                }
                Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
                return this.ColumnsItemsInternal[this.CurrentColumnIndex];
            }
            set
            {
                DataGridColumn dataGridColumn = value;
                if (dataGridColumn == null)
                {
                    throw DataGridError.DataGrid.ValueCannotBeSetToNull("value", "CurrentColumn");
                }
                if (this.CurrentColumn != dataGridColumn)
                {
                    if (dataGridColumn.OwningGrid != this)
                    {
                        // Provided column does not belong to this DataGrid
                        throw DataGridError.DataGrid.ColumnNotInThisDataGrid();
                    }
                    if (dataGridColumn.Visibility == Visibility.Collapsed)
                    {
                        // CurrentColumn cannot be set to an invisible column
                        throw DataGridError.DataGrid.ColumnCannotBeCollapsed();
                    }
                    if (this.CurrentSlot == -1)
                    {
                        // There is no current row so the current column cannot be set
                        throw DataGridError.DataGrid.NoCurrentRow();
                    }
                    bool beginEdit = this._editingColumnIndex != -1;
                    if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, this.ContainsFocus /*keepFocus*/, true /*raiseEvents*/))
                    {
                        // Edited value couldn't be committed or aborted
                        return;
                    }
                    bool success = SetCurrentCellCore(dataGridColumn.Index, this.CurrentSlot, true /*commitEdit*/, false);
                    Debug.Assert(success);
                    if (beginEdit && 
                        this._editingColumnIndex == -1 && 
                        this.CurrentSlot != -1 &&
                        this.CurrentColumnIndex != -1 && 
                        this.CurrentColumnIndex == dataGridColumn.Index &&
                        dataGridColumn.OwningGrid == this &&
                        !GetColumnEffectiveReadOnlyState(dataGridColumn))
                    {
                        // Returning to editing mode since the grid was in that mode prior to the EndCellEdit call above.
                        BeginCellEdit(new RoutedEventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Collection of GroupDescriptions for RowGrouping
        /// </summary>
        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                if (this.DataConnection.CollectionView == null || !this.DataConnection.CollectionView.CanGroup)
                {
                    if (this._groupDescriptions == null)
                    {
                        this._groupDescriptions = new ObservableCollection<GroupDescription>();
                    }
                    return this._groupDescriptions;
                }
                else
                {
                    return this.DataConnection.CollectionView.GroupDescriptions;
                }
            }
        }

        /* 



















*/

        /* 



















*/

        public ObservableCollection<Style> RowGroupHeaderStyles
        {
            get
            {
                return _rowGroupHeaderStyles;
            }
        }

        /// <summary>
        /// Gets the currently selected items.
        /// </summary>
        public IList SelectedItems
        {
            get { return _selectedItems as IList; }
        }

        /// <summary>
        /// Collection of SortDescriptions for sorting
        /// </summary>
        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (this.DataConnection.CollectionView == null || !this.DataConnection.CollectionView.CanSort)
                {
                    if (this._sortDescriptions == null)
                    {
                        this._sortDescriptions = new SortDescriptionCollection();
                    }
                    return this._sortDescriptions;
                }
                else
                {
                    return this.DataConnection.CollectionView.SortDescriptions;
                }
            }
        }

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Gets the item that will be edited during editing mode.
        /// </summary>
        protected object CurrentItem
        {
            get
            {
                if (this.CurrentSlot == -1 || this.ItemsSource /*this.DataConnection.DataSource*/ == null || this.RowGroupHeadersTable.Contains(this.CurrentSlot))
                {
                    return null;
                }
                return this.DataConnection.GetDataItem(RowIndexFromSlot(this.CurrentSlot));
            }
        }

        #endregion Protected Properties

        #region Internal Properties

        internal int AnchorSlot
        {
            get;
            private set;
        }

        internal double ActualRowHeaderWidth
        {
            get
            {
                if (!this.AreRowHeadersVisible)
                {
                    return 0;
                }
                else
                {
                    return !double.IsNaN(this.RowHeaderWidth) ? this.RowHeaderWidth : this.RowHeadersDesiredWidth;
                }
            }
        }

        internal double ActualRowsPresenterHeight
        {
            get
            {
                if (this._rowsPresenter != null)
                {
                    return this._rowsPresenter.ActualHeight;
                }
                return 0;
            }
        }

        internal bool AreColumnHeadersVisible
        {
            get
            {
                return (this.HeadersVisibility & DataGridHeadersVisibility.Column) == DataGridHeadersVisibility.Column;
            }
        }

        internal bool AreRowHeadersVisible
        {
            get
            {
                return (this.HeadersVisibility & DataGridHeadersVisibility.Row) == DataGridHeadersVisibility.Row;
            }
        }

        internal double AvailableSlotElementRoom
        {
            get;
            set;
        }

        // 









        
        // Height currently available for cells this value is smaller.  This height is reduced by the existence of ColumnHeaders
        // or a horizontal scrollbar.  Layout is asynchronous so changes to the ColumnHeaders or the horizontal scrollbar are 
        // not reflected immediately.
        internal double CellsHeight
        {
            get
            {
                return this.RowsPresenterAvailableSize.Height;
            }
        }

        // Width currently available for cells this value is smaller.  This width is reduced by the existence of RowHeaders
        // or a vertical scrollbar.  Layout is asynchronous so changes to the RowHeaders or the vertical scrollbar are
        // not reflected immediately
        internal double CellsWidth
        {
            get
            {
                if (double.IsPositiveInfinity(this.RowsPresenterAvailableSize.Width))
                {
                    // If we're given infinite width, then the cells will just grow to be as big as the columns
                    return this.ColumnsInternal.VisibleEdgedColumnsWidth;
                }
                else
                {
                    return Math.Max(0, this.RowsPresenterAvailableSize.Width - this.ActualRowHeaderWidth);
                }
            }
        }

        internal DataGridColumnHeadersPresenter ColumnHeaders
        {
            get
            {
                return this._columnHeadersPresenter;
            }
        }

        internal DataGridColumnCollection ColumnsInternal
        {
            get;
            private set;
        }

        internal List<DataGridColumn> ColumnsItemsInternal
        {
            get
            {
                return this.ColumnsInternal.ItemsInternal;
            }
        }

        internal Popup ColumnDropLocationIndicatorPopup
        {
            get
            {
                if (this._columnDropLocationIndicatorPopup == null)
                {
                    this._columnDropLocationIndicatorPopup = new Popup
                    {
                        Child = this.ColumnDropLocationIndicator,
                        IsOpen = false
                    };
                }

                return this._columnDropLocationIndicatorPopup;
            }
        }

        internal Control ColumnDropLocationIndicator
        {
            get
            {
                // 


                if (this._columnDropLocationIndicator == null ||
                    this._columnDropLocationIndicator.Style != this.DropLocationIndicatorStyle)
                {
                    this._columnDropLocationIndicator = new ContentControl();
                    this._columnDropLocationIndicator.SetStyleWithType(this.DropLocationIndicatorStyle);
                }

                return this._columnDropLocationIndicator;
            }
        }

        internal bool ContainsFocus
        {
            get;
            private set;
        }

        internal int CurrentColumnIndex
        {
            get
            {
                return this.CurrentCellCoordinates.ColumnIndex;
            }

            private set
            {
                this.CurrentCellCoordinates.ColumnIndex = value;
            }
        }

        internal int CurrentSlot
        {
            get
            {
                return this.CurrentCellCoordinates.Slot;
            }

            private set
            {
                this.CurrentCellCoordinates.Slot = value;
            }
        }

        internal DataGridDataConnection DataConnection
        {
            get;
            private set;
        }

        internal DataGridDisplayData DisplayData
        {
            get;
            private set;
        }

        internal int EditingColumnIndex
        {
            get
            {
                return this._editingColumnIndex;
            }
        }

        internal DataGridRow EditingRow
        {
            get;
            private set;
        }

        internal double FirstDisplayedScrollingColumnHiddenWidth
        {
            get
            {
                return this._negHorizontalOffset;
            }
        }

        internal static double HorizontalGridLinesThickness
        {
            get
            {
                return DATAGRID_horizontalGridLinesThickness;
            }
        }

        // the sum of the widths in pixels of the scrolling columns preceding 
        // the first displayed scrolling column
        internal double HorizontalOffset
        {
            get
            {
                return this._horizontalOffset;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                double widthNotVisible = Math.Max(0, this.ColumnsInternal.VisibleEdgedColumnsWidth - this.CellsWidth);
                if (value > widthNotVisible)
                {
                    value = widthNotVisible;
                }
                if (value == this._horizontalOffset)
                {
                    return;
                }

                if (this._hScrollBar != null && value != this._hScrollBar.Value)
                {
                    this._hScrollBar.Value = value;
                }
                this._horizontalOffset = value;

                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                // update the lastTotallyDisplayedScrollingCol
                ComputeDisplayedColumns();
            }
        }

        internal ScrollBar HorizontalScrollBar
        {
            get
            {
                return _hScrollBar;
            }
        }

        internal bool LoadingOrUnloadingRow
        {
            get;
            private set;
        }

        internal bool InDisplayIndexAdjustments
        {
            get;
            set;
        }

        internal int? MouseOverRowIndex
        {
            get
            {
                return this._mouseOverRowIndex;
            }
            set
            {
                if (this._mouseOverRowIndex != value)
                {
                    DataGridRow oldMouseOverRow = null;
                    if (_mouseOverRowIndex.HasValue)
                    {
                        int oldSlot = SlotFromRowIndex(_mouseOverRowIndex.Value);
                        if (IsSlotVisible(oldSlot))
                        {
                            oldMouseOverRow = this.DisplayData.GetDisplayedElement(oldSlot) as DataGridRow;
                        }
                    }

                    _mouseOverRowIndex = value;

                    // State for the old row needs to be applied after setting the new value
                    if (oldMouseOverRow != null)
                    {
                        oldMouseOverRow.ApplyState(true /*animate*/);
                    }

                    if (_mouseOverRowIndex.HasValue)
                    {
                        int newSlot = SlotFromRowIndex(_mouseOverRowIndex.Value);
                        if (IsSlotVisible(newSlot))
                        {
                            DataGridRow newMouseOverRow = this.DisplayData.GetDisplayedElement(newSlot) as DataGridRow;
                            Debug.Assert(newMouseOverRow != null);
                            if (newMouseOverRow != null)
                            {
                                newMouseOverRow.ApplyState(true /*animate*/);
                            }
                        }
                    }
                }
            }
        }

        internal double NegVerticalOffset
        {
            get;
            private set;
        }

        internal int NoCurrentCellChangeCount
        {
            get
            {
                return this._noCurrentCellChangeCount;
            }
            set
            {
                Debug.Assert(value >= 0);
                this._noCurrentCellChangeCount = value;
                if (value == 0)
                {
                    FlushCurrentCellChanged();
                }
            }
        }

        internal double RowDetailsHeightEstimate
        {
            get;
            private set;
        }

        internal double RowHeadersDesiredWidth
        {
            get
            {
                return _rowHeaderDesiredWidth;
            }
            set
            {
                // We only auto grow
                if (_rowHeaderDesiredWidth < value)
                {
                    double oldActualRowHeaderWidth = this.ActualRowHeaderWidth;
                    _rowHeaderDesiredWidth = value;
                    if (oldActualRowHeaderWidth != this.ActualRowHeaderWidth)
                    {
                        EnsureRowHeaderWidth();
                    }
                }
            }
        }

        internal double RowGroupHeaderHeightEstimate
        {
            get;
            private set;
        }

        internal IndexToValueTable<DataGridRowGroupInfo> RowGroupHeadersTable
        {
            get;
            private set;
        }

        internal double[] RowGroupSublevelIndents
        {
            get;
            private set;
        }

        internal double RowHeightEstimate
        {
            get;
            private set;
        }

        internal Size RowsPresenterAvailableSize
        {
            get;
            set;
        }

        // This flag indicates whether selection has actually changed during a selection operation,
        // and exists to ensure that FlushSelectionChanged doesn't unnecessarily raise SelectionChanged.
        internal bool SelectionHasChanged
        {
            get;
            set;
        }

        internal int SlotCount
        {
            get;
            private set;
        }

        internal bool UpdatedStateOnMouseLeftButtonDown
        {
            get;
            set;
        }

        internal ScrollBar VerticalScrollBar
        {
            get
            {
                return _vScrollBar;
            }
        }

        internal int VisibleSlotCount
        {
            get;
            set;
        }

        #endregion Internal Properties

        #region Private Properties

        private DataGridCellCoordinates CurrentCellCoordinates
        {
            get
            {
                return this._currentCellCoordinates;
            }

            set
            {
                this._currentCellCoordinates = value;
            }
        }

        private int FirstDisplayedNonFillerColumnIndex
        {
            get
            {
                DataGridColumn column = this.ColumnsInternal.FirstVisibleNonFillerColumn;
                if (column != null)
                {
                    if (column.IsFrozen)
                    {
                        return column.Index;
                    }
                    else
                    {
                        if (this.DisplayData.FirstDisplayedScrollingCol >= column.Index)
                        {
                            return this.DisplayData.FirstDisplayedScrollingCol;
                        }
                        else
                        {
                            return column.Index;
                        }
                    }
                }
                return -1;
            }
        }

        private int NoSelectionChangeCount
        {
            get
            {
                return this._noSelectionChangeCount;
            }
            set
            {
                Debug.Assert(value >= 0);
                this._noSelectionChangeCount = value;
                if (value == 0)
                {
                    FlushSelectionChanged();
                }
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Enters editing mode for the current cell and current row (if they're not already in editing mode).
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool BeginEdit()
        {
            return BeginEdit(null);
        }

        /// <summary>
        /// Enters editing mode for the current cell and current row (if they're not already in editing mode).
        /// </summary>
        /// <param name="editingEventArgs">Provides information about the user gesture that caused the call to BeginEdit. Can be null.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool BeginEdit(RoutedEventArgs editingEventArgs)
        {
            if (this.CurrentColumnIndex == -1 || !GetRowSelection(this.CurrentSlot))
            {
                return false;
            }

            Debug.Assert(this.CurrentColumnIndex >= 0);
            Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.CurrentSlot >= -1);
            Debug.Assert(this.CurrentSlot < this.SlotCount);
            Debug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot);

            if (GetColumnEffectiveReadOnlyState(this.CurrentColumn))
            {
                // Current column is read-only
                return false;
            }
            return BeginCellEdit(editingEventArgs);
        }

        /// <summary>
        /// Cancels editing mode and restores the original value.
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CancelEdit()
        {
            return CancelEdit(DataGridEditingUnit.Row);
        }

        /// <summary>
        /// Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CancelEdit(DataGridEditingUnit editingUnit)
        {
            return this.CancelEdit(editingUnit, true /*raiseEvents*/);
        }

        /// <summary>
        /// Commits editing mode and pushes changes to the backend.
        /// </summary>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CommitEdit()
        {
            return CommitEdit(DataGridEditingUnit.Row, true);
        }

        /// <summary>
        /// Commits editing mode for the specified DataGridEditingUnit and pushes changes to the backend.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to commit edit for a Cell or Row.</param>
        /// <param name="exitEditingMode">Editing mode is left if True.</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        public bool CommitEdit(DataGridEditingUnit editingUnit, bool exitEditingMode)
        {
            if (!EndCellEdit(DataGridEditAction.Commit, exitEditingMode, this.ContainsFocus /*keepFocus*/, true /*raiseEvents*/))
            {
                return false;
            }
            if (editingUnit == DataGridEditingUnit.Row)
            {
                return EndRowEdit(DataGridEditAction.Commit, exitEditingMode, true /*raiseEvents*/);
            }
            return true;
        }

        /// <summary>
        /// Returns the Group at the indicated level or null if the item is not in the ItemsSource
        /// </summary>
        /// <param name="item">item</param>
        /// <param name="groupLevel">groupLevel</param>
        /// <returns>The group the given item falls under or null if the item is not in the ItemsSource</returns>
        public CollectionViewGroup GetGroupFromItem(object item, int groupLevel)
        {
            int itemIndex = this.DataConnection.IndexOf(item);
            if (itemIndex == -1)
            {
                return null;
            }
            int groupHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(SlotFromRowIndex(itemIndex));
            DataGridRowGroupInfo rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
            while (rowGroupInfo != null && rowGroupInfo.Level != groupLevel)
            {
                groupHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(rowGroupInfo.Slot);
                rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
            }
            return rowGroupInfo == null ? null : rowGroupInfo.CollectionViewGroup;
        }

        /// <summary>
        /// Scrolls the specified item or RowGroupHeader and/or column into view.
        /// If item is not null: scrolls the row representing the item into view;
        /// If column is not null: scrolls the column into view;
        /// If both item and column are null, the method returns without scrolling.
        /// </summary>
        /// <param name="item">an item from the DataGrid's items source or a CollectionViewGroup from the PagedCollectionView</param>
        /// <param name="column">a column from the DataGrid's columns collection</param>
        public void ScrollIntoView(object item, DataGridColumn column)
        {
            if ((column == null && item == null) || (column != null && column.OwningGrid != this))
            {
                // no-op
                return;
            }
            if (item == null)
            {
                // scroll column into view
                this.ScrollSlotIntoView(column.Index, this.DisplayData.FirstScrollingSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
            }
            else
            {
                int slot = -1;
                CollectionViewGroup collectionViewGroup = item as CollectionViewGroup;
                DataGridRowGroupInfo rowGroupInfo = null;
                if (collectionViewGroup != null)
                {
                    rowGroupInfo = RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
                    if (rowGroupInfo == null)
                    {
                        Debug.Assert(false);
                        return;
                    }
                    slot = rowGroupInfo.Slot;
                }
                else
                {
                    // the row index will be set to -1 if the item is null or not in the list
                    int rowIndex = this.DataConnection.IndexOf(item);
                    if (rowIndex == -1)
                    {
                        return;
                    }
                    slot = SlotFromRowIndex(rowIndex);
                }

                int columnIndex = (column == null) ? this.FirstDisplayedNonFillerColumnIndex : column.Index;

                if (_collapsedSlotsTable.Contains(slot))
                {
                    // We need to expand all parent RowGroups so that the slot is visible
                    if (rowGroupInfo != null)
                    {
                        ExpandRowGroupParentChain(rowGroupInfo.Level - 1, rowGroupInfo.Slot);
                    }
                    else
                    {
                        rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(this.RowGroupHeadersTable.GetPreviousIndex(slot));
                        Debug.Assert(rowGroupInfo != null);
                        if (rowGroupInfo != null)
                        {
                            ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
                        }
                    }

                    // Update Scrollbar and display information
                    this.NegVerticalOffset = 0;
                    SetVerticalOffset(0);
                    ResetDisplayedRows();
                    this.DisplayData.FirstScrollingSlot = 0;
                    ComputeScrollBarsLayout();
                }

                ScrollSlotIntoView(columnIndex, slot, true /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Arranges the content of the <see cref="T:System.Windows.Controls.DataGridRow" />.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used by the <see cref="T:System.Windows.Controls.DataGridRow" />.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_makeFirstDisplayedCellCurrentCellPending)
            {
                MakeFirstDisplayedCellCurrentCell();
            }

            if (this.ActualWidth != finalSize.Width)
            {
                // If our final width has changed, we might need to update the filler
                InvalidateColumnHeadersArrange();
                InvalidateCellsArrange();
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Measures the children of a <see cref="T:System.Windows.Controls.DataGridRow" /> to prepare for 
        /// arranging them during the 
        /// <see cref="M:System.Windows.Controls.DataGridRow.ArrangeOverride(System.Windows.Size)" /> pass. 
        /// </summary>
        /// <returns>
        /// The size that the <see cref="T:System.Windows.Controls.DataGridRow" /> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that 
        /// child elements should not exceed.
        /// </param>
        protected override Size MeasureOverride(Size availableSize)
        {
            // Delay layout until after the initial measure to avoid invalid calculations when the 
            // DataGrid is not part of the visual tree
            if (!_measured)
            {
                _measured = true;

                RefreshRowsAndColumns();

                // Update our estimates now that the DataGrid has all of the information necessary
                UpdateRowDetailsHeightEstimate();

                // Update frozen columns to account for columns added prior to loading or autogenerated columns
                if (this.FrozenColumnCountWithFiller > 0)
                {
                    ProcessFrozenColumnCount(this);
                }
            }

            Size desiredSize;
            // This is a shortcut to skip layout if we don't have any columns
            if (this.ColumnsInternal.VisibleEdgedColumnsWidth == 0)
            {
                if (_hScrollBar != null && _hScrollBar.Visibility != Visibility.Collapsed)
                {
                    _hScrollBar.Visibility = Visibility.Collapsed;
                }
                if (_vScrollBar != null && _vScrollBar.Visibility != Visibility.Collapsed)
                {
                    _vScrollBar.Visibility = Visibility.Collapsed;
                }
                desiredSize = base.MeasureOverride(availableSize);
            }
            else
            {
                if (_rowsPresenter != null)
                {
                    _rowsPresenter.InvalidateMeasure();
                }

                InvalidateColumnHeadersMeasure();

                desiredSize = base.MeasureOverride(availableSize);

                // 
                ComputeScrollBarsLayout();
            }

            return desiredSize;
        }

        /// <summary>
        /// Builds the visual tree for the column header when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this._columnHeadersPresenter != null)
            {
                // If we're applying a new template, we want to remove the old column headers first
                this._columnHeadersPresenter.Children.Clear();
            }
            _columnHeadersPresenter = GetTemplateChild(DATAGRID_elementColumnHeadersPresenterName) as DataGridColumnHeadersPresenter;
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.OwningGrid = this;
                // Columns were added before before our Template was applied, add the ColumnHeaders now
                foreach (DataGridColumn column in this.ColumnsItemsInternal)
                {
                    InsertDisplayedColumnHeader(column);
                }
            }

            if (this._rowsPresenter != null)
            {
                // If we're applying a new template, we want to remove the old rows rist
                this._rowsPresenter.Children.Clear();
            }
            _rowsPresenter = GetTemplateChild(DATAGRID_elementRowsPresenterName) as DataGridRowsPresenter;
            if (_rowsPresenter != null)
            {
                _rowsPresenter.OwningGrid = this;
                InvalidateRowHeightEstimate();
                UpdateRowDetailsHeightEstimate();
            }
            _frozenColumnScrollBarSpacer = GetTemplateChild(DATAGRID_elementFrozenColumnScrollBarSpacerName) as FrameworkElement;

            if (_hScrollBar != null)
            {
                _hScrollBar.Scroll -= new ScrollEventHandler(HorizontalScrollBar_Scroll);
            }
            _hScrollBar = GetTemplateChild(DATAGRID_elementHorizontalScrollbarName) as ScrollBar;
            if (_hScrollBar != null)
            {
                _hScrollBar.IsTabStop = false;
                _hScrollBar.Maximum = 0.0;
                _hScrollBar.Orientation = Orientation.Horizontal;
                _hScrollBar.Visibility = Visibility.Collapsed;
                _hScrollBar.Scroll += new ScrollEventHandler(HorizontalScrollBar_Scroll);
            }

            if (_vScrollBar != null)
            {
                _vScrollBar.Scroll -= new ScrollEventHandler(VerticalScrollBar_Scroll);
            }
            _vScrollBar = GetTemplateChild(DATAGRID_elementVerticalScrollbarName) as ScrollBar;
            if (_vScrollBar != null)
            {
                _vScrollBar.IsTabStop = false;
                _vScrollBar.Maximum = 0.0;
                _vScrollBar.Orientation = Orientation.Vertical;
                _vScrollBar.Visibility = Visibility.Collapsed;
                _vScrollBar.Scroll += new ScrollEventHandler(VerticalScrollBar_Scroll);
            }

            _topLeftCornerHeader = GetTemplateChild(DATAGRID_elementTopLeftCornerHeaderName) as ContentControl;
            EnsureTopLeftCornerHeader(); // EnsureTopLeftCornerHeader checks for a null _topLeftCornerHeader;
            _topRightCornerHeader = GetTemplateChild(DATAGRID_elementTopRightCornerHeaderName) as ContentControl;

            if (_errorsListBox != null)
            {
                _errorsListBox.KeyDown -= new KeyEventHandler(ErrorsListBox_KeyDown);
                _errorsListBox.RemoveHandler(ListBox.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ErrorsListBox_MouseLeftButtonDown));
                _errorsListBox.SelectionChanged -= new SelectionChangedEventHandler(ErrorsListBox_SelectionChanged);
            }
            this._errorsListBox = GetTemplateChild(DATAGRID_elementErrorsListBox) as ListBox;
            if (_errorsListBox != null)
            {
                _errorsListBox.DisplayMemberPath = "ErrorMessage";
                _errorsListBox.ItemsSource = this._validationResults;
                _errorsListBox.SelectionMode = System.Windows.Controls.SelectionMode.Single;
                _errorsListBox.Visibility = Visibility.Collapsed;

                _errorsListBox.KeyDown += new KeyEventHandler(ErrorsListBox_KeyDown);
                _errorsListBox.AddHandler(ListBox.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ErrorsListBox_MouseLeftButtonDown), true);
                _errorsListBox.SelectionChanged += new SelectionChangedEventHandler(ErrorsListBox_SelectionChanged);
            }

            UpdateDisabledVisual();
        }

        /// <summary>
        /// Raises the AutoGeneratingColumn event.
        /// </summary>
        protected virtual void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            EventHandler<DataGridAutoGeneratingColumnEventArgs> handler = this.AutoGeneratingColumn;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the BeginningEdit event.
        /// </summary>
        protected virtual void OnBeginningEdit(DataGridBeginningEditEventArgs e)
        {
            EventHandler<DataGridBeginningEditEventArgs> handler = this.BeginningEdit;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the CellEditEnded event.
        /// </summary>
        protected virtual void OnCellEditEnded(DataGridCellEditEndedEventArgs e)
        {
            EventHandler<DataGridCellEditEndedEventArgs> handler = this.CellEditEnded;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the cell that just ended edit
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Cell, e.Column, e.Row);
            }
        }

        /// <summary>
        /// Raises the CellEditEnding event.
        /// </summary>
        protected virtual void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
        {
            EventHandler<DataGridCellEditEndingEventArgs> handler = this.CellEditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridAutomationPeer(this);
        }

        /// <summary>
        /// Raises the CurrentCellChanged event.
        /// </summary>
        protected virtual void OnCurrentCellChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.CurrentCellChanged;
            if (handler != null)
            {
                handler(this, e);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
            {
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationCellSelectedEvent(this.CurrentSlot, this.CurrentColumnIndex);
                }
            }
        }

        /// <summary>
        /// Raises the LoadingRow event for row preparation.
        /// </summary>
        protected virtual void OnLoadingRow(DataGridRowEventArgs e)
        {
            EventHandler<DataGridRowEventArgs> handler = this.LoadingRow;
            if (handler != null)
            {
                Debug.Assert(!this._loadedRows.Contains(e.Row));
                this._loadedRows.Add(e.Row);
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
                Debug.Assert(this._loadedRows.Contains(e.Row));
                this._loadedRows.Remove(e.Row);
            }
        }

        /// <summary>
        /// Raises the LoadingRowGroup event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnLoadingRowGroup(DataGridRowGroupHeaderEventArgs e)
        {
            EventHandler<DataGridRowGroupHeaderEventArgs> handler = this.LoadingRowGroup;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        /// <summary>
        /// Raises the LoadingRowDetails for row details preparation
        /// </summary>
        protected virtual void OnLoadingRowDetails(DataGridRowDetailsEventArgs e)
        {
            EventHandler<DataGridRowDetailsEventArgs> handler = this.LoadingRowDetails;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        /// <summary>
        /// Raises the PreparingCellForEdit event.
        /// </summary>
        protected virtual void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
        {
            EventHandler<DataGridPreparingCellForEditEventArgs> handler = this.PreparingCellForEdit;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the cell that just began edit because now
            // its editable content has been loaded
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Cell, e.Column, e.Row);
            }
        }

        /// <summary>
        /// Raises the RowEditEnded event.
        /// </summary>
        protected virtual void OnRowEditEnded(DataGridRowEditEndedEventArgs e)
        {
            EventHandler<DataGridRowEditEndedEventArgs> handler = this.RowEditEnded;
            if (handler != null)
            {
                handler(this, e);
            }

            // Raise the automation invoke event for the row that just ended edit because the edits
            // to its associated item have either been committed or reverted
            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Row, null, e.Row);
            }
        }

        /// <summary>
        /// Raises the RowEditEnding event.
        /// </summary>
        protected virtual void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            EventHandler<DataGridRowEditEndingEventArgs> handler = this.RowEditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the RowGroupCollapsingEvent
        /// </summary>
        /// <param name="e">CancelEventArgs</param>
        protected virtual void OnRowGroupCollapsing(DataGridRowGroupHeaderToggleEventArgs e)
        {
            EventHandler<DataGridRowGroupHeaderToggleEventArgs> handler = this.RowGroupCollapsing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the RowGroupExpandingEvent
        /// </summary>
        /// <param name="e">CancelEventArgs</param>
        protected virtual void OnRowGroupExpanding(DataGridRowGroupHeaderToggleEventArgs e)
        {
            EventHandler<DataGridRowGroupHeaderToggleEventArgs> handler = this.RowGroupExpanding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the SelectionChanged event and clears the _selectionChanged.
        /// This event won't get raised again until after _selectionChanged is set back to true.
        /// </summary>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) ||
                AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
            {
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationSelectionEvents(e);
                }
            }
        }

        /// <summary>
        /// Raises the UnloadingRow event for row recycling.
        /// </summary>
        protected virtual void OnUnloadingRow(DataGridRowEventArgs e)
        {
            EventHandler<DataGridRowEventArgs> handler = this.UnloadingRow;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        /// <summary>
        /// Raises the UnloadingRowDetails event
        /// </summary>
        protected virtual void OnUnloadingRowDetails(DataGridRowDetailsEventArgs e)
        {
            EventHandler<DataGridRowDetailsEventArgs> handler = this.UnloadingRowDetails;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        /// <summary>
        /// Raises the UnLoadingRowGroup event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnUnloadingRowGroup(DataGridRowGroupHeaderEventArgs e)
        {
            EventHandler<DataGridRowGroupHeaderEventArgs> handler = this.UnloadingRowGroup;
            if (handler != null)
            {
                this.LoadingOrUnloadingRow = true;
                handler(this, e);
                this.LoadingOrUnloadingRow = false;
            }
        }

        #endregion Protected Methods

        #region Internal Methods

        /// <summary>
        /// Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
        /// </summary>
        /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
        /// <param name="raiseEvents">Specifies whether or not to raise editing events</param>
        /// <returns>True if operation was successful. False otherwise.</returns>
        internal bool CancelEdit(DataGridEditingUnit editingUnit, bool raiseEvents)
        {
            if (!EndCellEdit(DataGridEditAction.Cancel, true, this.ContainsFocus /*keepFocus*/, raiseEvents))
            {
                return false;
            }
            if (editingUnit == DataGridEditingUnit.Row)
            {
                return EndRowEdit(DataGridEditAction.Cancel, true, raiseEvents);
            }
            return true;
        }

        /// <summary>
        /// call when: selection changes or SelectedItems object changes
        /// </summary>
        internal void CoerceSelectedItem()
        {
            object selectedItem = null;

            if (this.SelectionMode == DataGridSelectionMode.Extended &&
                this.CurrentSlot != -1 &&
                _selectedItems.ContainsSlot(this.CurrentSlot))
            {
                selectedItem = this.CurrentItem;
            }
            else if (_selectedItems.Count > 0)
            {
                selectedItem = _selectedItems[0];
            }

            this.SetValueNoCallback(SelectedItemProperty, selectedItem);

            // Update the SelectedIndex
            int newIndex = -1;
            if (selectedItem != null)
            {
                newIndex = this.DataConnection.IndexOf(selectedItem);
            }
            this.SetValueNoCallback(SelectedIndexProperty, newIndex);
        }

        internal bool ErrorsListBox_Click()
        {
            if (this.EditingRow == null || !ScrollSlotIntoView(this.EditingRow.Slot, false /*scrolledHorizontally*/))
            {
                return false;
            }

            // We need to focus the DataGrid in case the focused element gets removed when we end edit.
            if ((this._editingColumnIndex == -1 || (Focus() && EndCellEdit(DataGridEditAction.Commit, true, true, true))) &&
                this._errorsListBox.SelectedItem != null)
            {
                Debug.Assert(this._validationResults != null);
                Debug.Assert(this._clickedErrorColumns != null);

                if (this._clickedErrorColumns.Count > 0)
                {
                    DataGridColumn nextColumn = this._clickedErrorColumns.Peek();

                    // Ignore the next column if it has been removed from the DataGrid
                    if (!this.Columns.Contains(nextColumn))
                    {
                        this._clickedErrorColumns.Dequeue();
                        if (this._clickedErrorColumns.Count == 0)
                        {
                            return true;
                        }
                    }

                    // Begin editing the next relevant cell
                    UpdateSelectionAndCurrency(nextColumn.Index, this.EditingRow.Slot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                    if (this._successfullyUpdatedSelection)
                    {
                        BeginCellEdit(new RoutedEventArgs());
                        if (!IsColumnDisplayed(this.CurrentColumnIndex))
                        {
                            ScrollColumnIntoView(this.CurrentColumnIndex);
                        }
                    }

                    if (this._clickedErrorColumns.Count > 1)
                    {
                        // Reorder the columns that we cycle through
                        this._clickedErrorColumns.Enqueue(this._clickedErrorColumns.Dequeue());
                    }
                }
                return true;
            }
            return false;
        }

        internal static DataGridCell GetOwningCell(FrameworkElement element)
        {
            Debug.Assert(element != null);
            DataGridCell cell = element as DataGridCell;
            while (element != null && cell == null)
            {
                element = element.Parent as FrameworkElement;
                cell = element as DataGridCell;
            }
            return cell;
        }

        internal IEnumerable<object> GetSelectionInclusive(int startRowIndex, int endRowIndex)
        {
            int endSlot = SlotFromRowIndex(endRowIndex);
            foreach (int slot in _selectedItems.GetSlots(SlotFromRowIndex(startRowIndex)))
            {
                if (slot > endSlot)
                {
                    break;
                }
                yield return this.DataConnection.GetDataItem(RowIndexFromSlot(slot));
            }
        }

        // 
        internal bool IsDoubleClickRecordsClickOnCall(UIElement element)
        {
            if (_clickedElement == element)
            {
                _clickedElement = null;
                return true;
            }
            else
            {
                _clickedElement = element;
                return false;
            }
        }

        internal void OnCollectionReset(bool recycleRows)
        {
            // We want to persist selection throughout a reset, so store away the selected items
            List<object> selectedItemsCache = new List<object>(this._selectedItems.SelectedItemsCache);

            RefreshRows(recycleRows /*recycleRows*/);

            // Re-select the old items
            this._selectedItems.SelectedItemsCache = selectedItemsCache;
            if (this.RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.Collapsed)
            {
                UpdateRowDetailsVisibilityMode(this.RowDetailsVisibilityMode);
            }

            // The currently displayed rows may have incorrect visual states because of the selection change
            ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
        }

        internal void OnRowDetailsChanged()
        {
            // Update layout when RowDetails are expanded or collapsed, just updating the vertical scroll bar is not enough 
            // since rows could be added or removed
            InvalidateMeasure();
        }

        internal bool ProcessDownKey()
        {
            bool shift, ctrl;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            return ProcessDownKeyInternal(shift, ctrl);
        }

        internal bool ProcessEndKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            DataGridColumn dataGridColumn = this.ColumnsInternal.LastVisibleColumn;
            int lastVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            int lastVisibleSlot = this.LastVisibleSlot;
            if (lastVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }

            this._noSelectionChangeCount++;
            try
            {
                if (!ctrl)
                {
                    return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
                }
                else
                {
                    DataGridSelectionAction action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent 
                        : DataGridSelectionAction.SelectCurrent;
                    UpdateSelectionAndCurrency(lastVisibleColumnIndex, lastVisibleSlot, action, true /*scrollIntoView*/);
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        internal bool ProcessEnterKey()
        {
            bool ctrl, shift, endRowEdit = true;
            int oldCurrentSlot = this.CurrentSlot;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            if (!ctrl)
            {
                // If Enter was used by a TextBox, we shouldn't handle the key
                TextBox focusedTextBox = FocusManager.GetFocusedElement() as TextBox;
                if (focusedTextBox != null && focusedTextBox.AcceptsReturn)
                {
                    return false;
                }

                // Enter behaves like down arrow - it commits the potential editing and goes down one cell.
                endRowEdit = false;
                if (!ProcessDownKeyInternal(false, ctrl))
                {
                    return false;
                }
            }

            // Try to commit the potential editing
            if (oldCurrentSlot == this.CurrentSlot && EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/) && endRowEdit && this.EditingRow != null)
            {
                EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/);
            }

            return true;
        }

        internal bool ProcessHomeKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }
            this._noSelectionChangeCount++;
            try
            {
                if (!ctrl)
                {
                    return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
                }
                else
                {
                    DataGridSelectionAction action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent 
                        : DataGridSelectionAction.SelectCurrent;
                    UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, action, true /*scrollIntoView*/);
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        internal void ProcessHorizontalScroll(ScrollEventType scrollEventType)
        {
            if (this._horizontalScrollChangesIgnored > 0)
            {
                return;
            }

            // If the user scrolls with the buttons, we need to update the new value of the scroll bar since we delay
            // this calculation.  If they scroll in another other way, the scroll bar's correct value has already been set
            double scrollBarValueDifference = 0;
            if (scrollEventType == ScrollEventType.SmallIncrement)
            {
                scrollBarValueDifference = GetHorizontalSmallScrollIncrease();
            }
            else if (scrollEventType == ScrollEventType.SmallDecrement)
            {
                scrollBarValueDifference = -GetHorizontalSmallScrollDecrease();
            }
            this._horizontalScrollChangesIgnored++;
            try
            {
                if (scrollBarValueDifference != 0)
                {
                    Debug.Assert(this._horizontalOffset + scrollBarValueDifference >= 0);
                    this._hScrollBar.Value = this._horizontalOffset + scrollBarValueDifference;
                }
                UpdateHorizontalOffset(this._hScrollBar.Value);
            }
            finally
            {
                this._horizontalScrollChangesIgnored--;
            }

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationScrollEvents();
            }
        }

        internal bool ProcessLeftKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }
            int previousVisibleColumnIndex = -1;
            if (this.CurrentColumnIndex != -1)
            {
                dataGridColumn = this.ColumnsInternal.GetPreviousVisibleNonFillerColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                if (dataGridColumn != null)
                {
                    previousVisibleColumnIndex = dataGridColumn.Index;
                }
            }

            this._noSelectionChangeCount++;
            try
            {
                if (ctrl)
                {
                    return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
                }
                else
                {
                    if (this.RowGroupHeadersTable.Contains(this.CurrentSlot))
                    {
                        CollapseRowGroup(this.RowGroupHeadersTable.GetValueAt(this.CurrentSlot).CollectionViewGroup, false /*collapseAllSubgroups*/);
                    }
                    else if (this.CurrentColumnIndex == -1)
                    {
                        UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, DataGridSelectionAction.SelectCurrent, true /*scrollIntoView*/);
                    }
                    else
                    {
                        if (previousVisibleColumnIndex == -1)
                        {
                            return true;
                        }
                        UpdateSelectionAndCurrency(previousVisibleColumnIndex, this.CurrentSlot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                    }
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        internal bool ProcessNextKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            if (firstVisibleColumnIndex == -1 || this.DisplayData.FirstScrollingSlot == -1)
            {
                return false;
            }
            int nextPageSlot = this.CurrentSlot == -1 ? this.DisplayData.FirstScrollingSlot : this.CurrentSlot;
            Debug.Assert(nextPageSlot != -1);
            int slot = GetNextVisibleSlot(nextPageSlot);
            
            int scrollCount = this.DisplayData.NumTotallyDisplayedScrollingElements;
            while (scrollCount > 0 && slot < this.SlotCount)
            {
                nextPageSlot = slot;
                scrollCount--;
                slot = GetNextVisibleSlot(slot);
            }

            this._noSelectionChangeCount++;
            try
            {
                DataGridSelectionAction action;
                int columnIndex;
                if (this.CurrentColumnIndex == -1)
                {
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    columnIndex = this.CurrentColumnIndex;
                    action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? action = DataGridSelectionAction.SelectFromAnchorToCurrent
                        : action = DataGridSelectionAction.SelectCurrent;
                }
                UpdateSelectionAndCurrency(columnIndex, nextPageSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        internal bool ProcessPriorKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            if (firstVisibleColumnIndex == -1 || this.DisplayData.FirstScrollingSlot == -1)
            {
                return false;
            }

            int previousPageSlot = (this.CurrentSlot == -1) ? this.DisplayData.FirstScrollingSlot : this.CurrentSlot;
            Debug.Assert(previousPageSlot != -1);

            int scrollCount = this.DisplayData.NumTotallyDisplayedScrollingElements;
            int slot = GetPreviousVisibleSlot(previousPageSlot);
            while (scrollCount > 0 && slot != -1)
            {
                previousPageSlot = slot;
                scrollCount--;
                slot = GetPreviousVisibleSlot(slot);
            }
            Debug.Assert(previousPageSlot != -1);

            this._noSelectionChangeCount++;
            try
            {
                int columnIndex;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    columnIndex = this.CurrentColumnIndex;
                    action = (shift && this.SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                }
                UpdateSelectionAndCurrency(columnIndex, previousPageSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        internal bool ProcessRightKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            DataGridColumn dataGridColumn = this.ColumnsInternal.LastVisibleColumn;
            int lastVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (lastVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }
            int nextVisibleColumnIndex = -1;
            if (this.CurrentColumnIndex != -1)
            {
                dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                if (dataGridColumn != null)
                {
                    nextVisibleColumnIndex = dataGridColumn.Index;
                }
            }
            this._noSelectionChangeCount++;
            try
            {
                if (ctrl)
                {
                    return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
                }
                else
                {
                    if (this.RowGroupHeadersTable.Contains(this.CurrentSlot))
                    {
                        ExpandRowGroup(this.RowGroupHeadersTable.GetValueAt(this.CurrentSlot).CollectionViewGroup, false /*expandAllSubgroups*/);
                    }
                    else if (this.CurrentColumnIndex == -1)
                    {
                        UpdateSelectionAndCurrency(lastVisibleColumnIndex, firstVisibleSlot, DataGridSelectionAction.SelectCurrent, true /*scrollIntoView*/);
                    }
                    else
                    {
                        if (nextVisibleColumnIndex == -1)
                        {
                            return true;
                        }
                        UpdateSelectionAndCurrency(nextVisibleColumnIndex, this.CurrentSlot, DataGridSelectionAction.None, true /*scrollIntoView*/);
                    }
                }
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        internal void ProcessSelectionAndCurrency(int columnIndex, int slot, DataGridSelectionAction action, bool scrollIntoView)
        {
            this._noSelectionChangeCount++;
            this._noCurrentCellChangeCount++;
            try
            {
                switch (action)
                {
                    case DataGridSelectionAction.AddCurrentToSelection:
                        SetRowSelection(slot, true /*isSelected*/, true /*setAnchorIndex*/);
                        break;
                    case DataGridSelectionAction.RemoveCurrentFromSelection:
                        SetRowSelection(slot, false /*isSelected*/, false /*setAnchorRowIndex*/);
                        break;
                    case DataGridSelectionAction.SelectFromAnchorToCurrent:
                        if (this.SelectionMode == DataGridSelectionMode.Extended && this.AnchorSlot != -1)
                        {
                            int anchorSlot = this.AnchorSlot;
                            ClearRowSelection(slot /*slotException*/, false /*resetAnchorSlot*/);
                            if (slot <= anchorSlot)
                            {
                                SetRowsSelection(slot, anchorSlot);
                            }
                            else
                            {
                                SetRowsSelection(anchorSlot, slot);
                            }
                        }
                        else
                        {
                            goto case DataGridSelectionAction.SelectCurrent;
                        }
                        break;
                    case DataGridSelectionAction.SelectCurrent:
                        ClearRowSelection(slot /*rowIndexException*/, true /*setAnchorRowIndex*/);
                        break;
                    case DataGridSelectionAction.None:
                        break;
                }

                if (this.CurrentSlot != slot || (this.CurrentColumnIndex != columnIndex && columnIndex != -1))
                {
                    if (columnIndex == -1)
                    {
                        if (this.CurrentColumnIndex != -1)
                        {
                            columnIndex = this.CurrentColumnIndex;
                        }
                        else
                        {
                            DataGridColumn firstVisibleColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
                            if (firstVisibleColumn != null)
                            {
                                columnIndex = firstVisibleColumn.Index;
                            }
                        }
                    }
                    if (columnIndex != -1)
                    {
                        if (!SetCurrentCellCore(columnIndex, slot, true /*commitEdit*/, SlotFromRowIndex(this.SelectedIndex) != slot /*endRowEdit*/)
                            || (scrollIntoView && !ScrollSlotIntoView(columnIndex, slot, true /*forCurrentCellChange*/, false /*forceHorizontalScroll*/)))
                        {
                            return;
                        }
                    }
                }
                this._successfullyUpdatedSelection = true;
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
                this.NoSelectionChangeCount--;
            }
        }

        internal bool ProcessUpKey()
        {
            bool ctrl;
            bool shift;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleNonFillerColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int firstVisibleSlot = this.FirstVisibleSlot;
            if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
            {
                return false;
            }

            int previousVisibleSlot = (this.CurrentSlot != -1) ? GetPreviousVisibleSlot(this.CurrentSlot) : -1;

            this._noSelectionChangeCount++;

            try
            {
                int slot;
                int columnIndex;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    slot = firstVisibleSlot;
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else if (ctrl)
                {
                    if (shift)
                    {
                        // Both Ctrl and Shift
                        slot = firstVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = (this.SelectionMode == DataGridSelectionMode.Extended)
                            ? DataGridSelectionAction.SelectFromAnchorToCurrent
                            : DataGridSelectionAction.SelectCurrent; 
                    }
                    else
                    {
                        // Ctrl without Shift
                        slot = firstVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                else
                {
                    if (previousVisibleSlot == -1)
                    {
                        return true;
                    }
                    if (shift)
                    {
                        // Shift without Ctrl
                        slot = previousVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                    }
                    else
                    {
                        // Neither Shift nor Ctrl
                        slot = previousVisibleSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                UpdateSelectionAndCurrency(columnIndex, slot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        internal void ProcessVerticalScroll(ScrollEventType scrollEventType)
        {
            if (this._verticalScrollChangesIgnored > 0)
            {
                return;
            }
            Debug.Assert(DoubleUtil.LessThanOrClose(this._vScrollBar.Value, this._vScrollBar.Maximum));

            this._verticalScrollChangesIgnored++;
            try
            {
                Debug.Assert(this._vScrollBar != null);
                if (scrollEventType == ScrollEventType.SmallIncrement)
                {
                    this.DisplayData.PendingVerticalScrollHeight = GetVerticalSmallScrollIncrease();
                    double newVerticalOffset = this._verticalOffset + this.DisplayData.PendingVerticalScrollHeight;
                    if (newVerticalOffset > this._vScrollBar.Maximum)
                    {
                        this.DisplayData.PendingVerticalScrollHeight -= newVerticalOffset - this._vScrollBar.Maximum;
                    }
                }
                else if (scrollEventType == ScrollEventType.SmallDecrement)
                {
                    if (DoubleUtil.GreaterThan(this.NegVerticalOffset, 0))
                    {
                        this.DisplayData.PendingVerticalScrollHeight -= this.NegVerticalOffset;
                    }
                    else
                    {
                        if (this.DisplayData.FirstScrollingSlot > 0)
                        {
                            ScrollSlotIntoView(this.DisplayData.FirstScrollingSlot - 1, false /*scrolledHorizontally*/);
                        }
                        return;
                    }
                }
                else
                {
                    this.DisplayData.PendingVerticalScrollHeight = this._vScrollBar.Value - this._verticalOffset;
                }

                if (!DoubleUtil.IsZero(this.DisplayData.PendingVerticalScrollHeight))
                {
                    // Invalidate so the scroll happens on idle
                    InvalidateRowsMeasure(false /*invalidateIndividualElements*/);
                }
                // 
            }
            finally
            {
                this._verticalScrollChangesIgnored--;
            }
        }

        internal void RefreshRowsAndColumns()
        {
            if (_measured)
            {
                try
                {
                    this._noCurrentCellChangeCount++;
                    ClearRows(false);
                    if (this.AutoGenerateColumns)
                    {
                        //Column auto-generation refreshes the rows too
                        AutoGenerateColumnsPrivate();
                    }
                    foreach (DataGridColumn column in this.ColumnsItemsInternal)
                    {
                        //We don't need to refresh the state of AutoGenerated column headers because they're up-to-date
                        if (!column.IsAutoGenerated && column.HasHeaderCell)
                        {
                            column.HeaderCell.ApplyState(false);
                        }
                    }

                    RefreshRows(false);

                    if (this.Columns.Count > 0 && this.CurrentColumnIndex == -1)
                    {
                        MakeFirstDisplayedCellCurrentCell();
                    }
                }
                finally
                {
                    this.NoCurrentCellChangeCount--;
                }
            }
        }

        internal bool ScrollSlotIntoView(int columnIndex, int slot, bool forCurrentCellChange, bool forceHorizontalScroll)
        {
            Debug.Assert(columnIndex >= 0 && columnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.DisplayData.FirstDisplayedScrollingCol >= -1 && this.DisplayData.FirstDisplayedScrollingCol < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.DisplayData.LastTotallyDisplayedScrollingCol >= -1 && this.DisplayData.LastTotallyDisplayedScrollingCol < this.ColumnsItemsInternal.Count);
            Debug.Assert(!IsSlotOutOfBounds(slot));
            Debug.Assert(this.DisplayData.FirstScrollingSlot >= -1 && this.DisplayData.FirstScrollingSlot < this.SlotCount);
            Debug.Assert(this.ColumnsItemsInternal[columnIndex].Visibility == Visibility.Visible);

            if (this.CurrentColumnIndex >= 0 &&
                (this.CurrentColumnIndex != columnIndex || this.CurrentSlot != slot))
            {
                if (!CommitEditForOperation(columnIndex, slot, forCurrentCellChange) || IsInnerCellOutOfBounds(columnIndex, slot))
                {
                    return false;
                }
            }

            double oldHorizontalOffset = this.HorizontalOffset;

            //scroll horizontally unless we're on a RowGroupHeader and we're not forcing horizontal scrolling
            if ((forceHorizontalScroll || (slot != -1 && !this.RowGroupHeadersTable.Contains(slot)))
                && !ScrollColumnIntoView(columnIndex))
            {
                return false;
            }

            //scroll vertically
            if (!ScrollSlotIntoView(slot, oldHorizontalOffset != this.HorizontalOffset /*scrolledHorizontally*/))
            {
                return false;
            }

            DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationScrollEvents();
            }
            return true;
        }

        // Convenient overload that commits the current edit.
        internal bool SetCurrentCellCore(int columnIndex, int slot)
        {
            return SetCurrentCellCore(columnIndex, slot, true /*commitEdit*/, true /*endRowEdit*/);
        }

        internal void UpdateHorizontalOffset(double newValue)
        {
            if (this.HorizontalOffset != newValue)
            {
                this.HorizontalOffset = newValue;

                InvalidateColumnHeadersMeasure();
                InvalidateRowsMeasure(true);
            }
        }

        internal bool UpdateSelectionAndCurrency(int columnIndex, int slot, DataGridSelectionAction action, bool scrollIntoView)
        {
            this._successfullyUpdatedSelection = false;

            if (this.ColumnsInternal.RowGroupSpacerColumn.IsRepresented &&
                columnIndex == this.ColumnsInternal.RowGroupSpacerColumn.Index)
            {
                columnIndex = -1;
            }
            if ((IsSlotOutOfBounds(slot)) ||
                (columnIndex != -1 && IsColumnOutOfBounds(columnIndex)) ||
                (this.EditingRow != null && slot != this.EditingRow.Slot && !CommitEdit(DataGridEditingUnit.Row, true)))
            {
                return this._successfullyUpdatedSelection;
            }

            int newCurrentPosition = -1;
            if (!this.RowGroupHeadersTable.Contains(slot))
            {
                newCurrentPosition = RowIndexFromSlot(slot);
            }
            if (this.DataConnection.CollectionView != null &&
                this.DataConnection.CollectionView.CurrentPosition != newCurrentPosition)
            {
                this.DataConnection.MoveCurrentToPosition(newCurrentPosition, columnIndex, slot, action, scrollIntoView);
            }
            else
            {
                ProcessSelectionAndCurrency(columnIndex, slot, action, scrollIntoView);
            }

            return this._successfullyUpdatedSelection;
        }

        internal void UpdateStateOnCurrentChanged(object currentItem, int currentPosition)
        {
            if (currentItem == this.CurrentItem)
            {
                // The DataGrid's CurrentItem is already up-to-date, so we don't need to do anything
                return;
            }

            int columnIndex = this.CurrentColumnIndex == -1 ? this.FirstDisplayedNonFillerColumnIndex : this.CurrentColumnIndex;

            try
            {
                this._noSelectionChangeCount++;
                this._noCurrentCellChangeCount++;

                if (!this.CommitEdit())
                {
                    this.CancelEdit(DataGridEditingUnit.Row, false);
                }

                if (currentItem == null)
                {
                    SetCurrentCellCore(-1, -1);
                }
                else 
                {
                    int slot = SlotFromRowIndex(currentPosition);
                    if (!IsColumnOutOfBounds(columnIndex) && slot < this.SlotCount)
                    {
                        SetCurrentCellCore(columnIndex, slot);
                    }
                }

                this.SelectedItem = currentItem;
            }
            finally
            {
                this.NoCurrentCellChangeCount--;
                this.NoSelectionChangeCount--;
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal bool UpdateStateOnMouseLeftButtonDown(MouseButtonEventArgs mouseButtonEventArgs, int columnIndex, int slot, bool allowEdit)
        {
            bool ctrl, shift, beginEdit;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            Debug.Assert(slot >= 0);

            // Before changing selection, check if the current cell needs to be committed, and
            // check if the current row needs to be committed. If any of those two operations are required and fail, 
            // do no change selection, and do not change current cell.

            bool wasInEdit = this.EditingColumnIndex != -1;

            if (this.CurrentSlot != slot && !CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/))
            {
                // Edited value couldn't be committed or aborted
                return true;
            }
            if (IsSlotOutOfBounds(slot))
            {
                return true;
            }

            try
            {
                this._noSelectionChangeCount++;

                beginEdit = allowEdit &&
                            this.CurrentSlot == slot &&
                            columnIndex != -1 &&
                            (wasInEdit || this.CurrentColumnIndex == columnIndex) &&
                            !GetColumnEffectiveReadOnlyState(this.ColumnsItemsInternal[columnIndex]);

                DataGridSelectionAction action;
                if (this.SelectionMode == DataGridSelectionMode.Extended && shift)
                {
                    // Shift select multiple rows
                    action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                }
                else if (GetRowSelection(slot))  // Unselecting single row or Selecting a previously multi-selected row
                {
                    if (!ctrl && this.SelectionMode == DataGridSelectionMode.Extended && _selectedItems.Count != 0)
                    {
                        // Unselect everything except the row that was clicked on
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                    else if (ctrl && this.EditingRow == null)
                    {
                        action = DataGridSelectionAction.RemoveCurrentFromSelection;
                    }
                    else
                    {
                        action = DataGridSelectionAction.None;
                    }
                }
                else // Selecting a single row or multi-selecting with Ctrl
                {
                    if (this.SelectionMode == DataGridSelectionMode.Single || !ctrl)
                    {
                        // Unselect the currectly selected rows except the new selected row
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                    else
                    {
                        action = DataGridSelectionAction.AddCurrentToSelection;
                    }
                }
                UpdateSelectionAndCurrency(columnIndex, slot, action, false /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (this._successfullyUpdatedSelection && beginEdit && BeginCellEdit(mouseButtonEventArgs))
            {
                FocusEditingCell(true /*setFocus*/);
            }

            return true;
        }

        internal void UpdateVerticalScrollBar()
        {
            if (this._vScrollBar != null && this._vScrollBar.Visibility == Visibility.Visible)
            {
                UpdateVerticalScrollBar(true /*needVertScrollbar*/, false /*forceVertScrollbar*/, this.EdgedRowsHeightCalculated, this.CellsHeight);
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private void AddNewCellPrivate(DataGridRow row, DataGridColumn column)
        {
            DataGridCell newCell = new DataGridCell();
            PopulateCellContent(false /*isCellEdited*/, column, row, newCell);
            if (row.OwningGrid != null)
            {
                newCell.OwningColumn = column;
                newCell.Visibility = column.Visibility;
            }
            newCell.EnsureStyle(null);
            row.Cells.Insert(column.Index, newCell);
        }

        private bool BeginCellEdit(RoutedEventArgs editingEventArgs)
        {
            if (this.CurrentColumnIndex == -1 || !GetRowSelection(this.CurrentSlot))
            {
                return false;
            }

            Debug.Assert(this.CurrentColumnIndex >= 0);
            Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.CurrentSlot >= -1);
            Debug.Assert(this.CurrentSlot < this.SlotCount);
            Debug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot);
            Debug.Assert(!GetColumnEffectiveReadOnlyState(this.CurrentColumn));
            Debug.Assert(this.CurrentColumn.Visibility == Visibility.Visible);

            if (this._editingColumnIndex != -1)
            {
                // Current cell is already in edit mode
                Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
                return true;
            }

            // Get or generate the editing row if it doesn't exist
            DataGridRow dataGridRow = this.EditingRow;
            if (dataGridRow == null)
            {
                Debug.Assert(!this.RowGroupHeadersTable.Contains(this.CurrentSlot));
                if (this.IsSlotVisible(this.CurrentSlot))
                {
                    dataGridRow = this.DisplayData.GetDisplayedElement(this.CurrentSlot) as DataGridRow;
                    Debug.Assert(dataGridRow != null);
                }
                else
                {
                    dataGridRow = GenerateRow(RowIndexFromSlot(this.CurrentSlot), this.CurrentSlot);
                }
            }
            Debug.Assert(dataGridRow != null);

            // Cache these to see if they change later
            int currentRowIndex = this.CurrentSlot;
            int currentColumnIndex = this.CurrentColumnIndex;

            // Raise the BeginningEdit event
            DataGridCell dataGridCell = dataGridRow.Cells[this.CurrentColumnIndex];
            DataGridBeginningEditEventArgs e = new DataGridBeginningEditEventArgs(this.CurrentColumn, dataGridRow, editingEventArgs);
            OnBeginningEdit(e);
            if (e.Cancel
                || currentRowIndex != this.CurrentSlot
                || currentColumnIndex != this.CurrentColumnIndex
                || !GetRowSelection(this.CurrentSlot)
                || (this.EditingRow == null && !BeginRowEdit(dataGridRow)))
            {
                // If either BeginningEdit was canceled, currency/selection was changed in the event handler,
                // or we failed opening the row for edit, then we can no longer continue BeginCellEdit
                return false;
            }
            Debug.Assert(this.EditingRow != null);
            Debug.Assert(this.EditingRow.Slot == this.CurrentSlot);

            // Finally, we can prepare the cell for editing
            this._editingColumnIndex = this.CurrentColumnIndex;
            this._editingEventArgs = editingEventArgs;
            this.EditingRow.Cells[this.CurrentColumnIndex].ApplyCellState(true /*animate*/);
            PopulateCellContent(true /*isCellEdited*/, this.CurrentColumn, dataGridRow, dataGridCell);
            return true;
        }

        private bool BeginRowEdit(DataGridRow dataGridRow)
        {
            Debug.Assert(this.EditingRow == null);
            Debug.Assert(dataGridRow != null);

            Debug.Assert(this.CurrentSlot >= -1);
            Debug.Assert(this.CurrentSlot < this.SlotCount);

            if (this.DataConnection.BeginEdit(dataGridRow.DataContext))
            {
                this.EditingRow = dataGridRow;
                this.EditingRow.ApplyState(true /*animate*/);

                // Raise the automation invoke event for the row that just began edit
                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
                {
                    peer.RaiseAutomationInvokeEvents(DataGridEditingUnit.Row, null, dataGridRow);
                }
                return true;
             }
            return false;
        }

        private void CancelRowEdit(bool exitEditingMode)
        {
            if (this.EditingRow == null)
            {
                return;
            }
            Debug.Assert(this.EditingRow != null && this.EditingRow.Index >= -1);
            Debug.Assert(this.EditingRow.Slot < this.SlotCount);
            Debug.Assert(this.CurrentColumn != null);

            object dataItem = this.EditingRow.DataContext;
            if (!this.DataConnection.CancelEdit(dataItem))
            {
                return;
            }
            // 

            foreach (DataGridColumn column in this.Columns)
            {
                if (!exitEditingMode && column.Index == this._editingColumnIndex && column is DataGridBoundColumn)
                {
                    continue;
                }
                PopulateCellContent(!exitEditingMode && column.Index == this._editingColumnIndex /*isCellEdited*/, column, this.EditingRow, this.EditingRow.Cells[column.Index]);
            }
        }

        private bool CommitEditForOperation(int columnIndex, int slot, bool forCurrentCellChange)
        {
            if (forCurrentCellChange)
            {
                if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/))
                {
                    return false;
                }
                if (this.CurrentSlot != slot &&
                    !EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/))
                {
                    return false;
                }
            }
            // 

            if (IsColumnOutOfBounds(columnIndex))
            {
                return false;
            }
            if (slot >= this.SlotCount)
            {
                // Current cell was reset because the commit deleted row(s).
                // Since the user wants to change the current cell, we don't
                // want to end up with no current cell. We pick the last row 
                // in the grid which may be the 'new row'.
                int lastSlot = this.LastVisibleSlot;
                if (forCurrentCellChange &&
                    this.CurrentColumnIndex == -1 &&
                    lastSlot != -1)
                {
                    SetAndSelectCurrentCell(columnIndex, lastSlot, false /*forceCurrentCellSelection (unused here)*/);
                }
                // Interrupt operation because it has become invalid.
                return false;
            }
            return true;
        }

        private bool CommitRowEdit(bool exitEditingMode)
        {
            if (this.EditingRow == null)
            {
                return true;
            }
            Debug.Assert(this.EditingRow != null && this.EditingRow.Index >= -1);
            Debug.Assert(this.EditingRow.Slot < this.SlotCount);

            // 



            if (!ValidateEditingRow())
            {
                return false;
            }

            this.DataConnection.EndEdit(this.EditingRow.DataContext);

            if (!exitEditingMode)
            {
                this.DataConnection.BeginEdit(this.EditingRow.DataContext);
            }
            return true;
        }

        private void CompleteCellsCollection(DataGridRow dataGridRow)
        {
            Debug.Assert(dataGridRow != null);
            int cellsInCollection = dataGridRow.Cells.Count;
            if (this.ColumnsItemsInternal.Count > cellsInCollection)
            {
                for (int columnIndex = cellsInCollection; columnIndex < this.ColumnsItemsInternal.Count; columnIndex++)
                {
                    AddNewCellPrivate(dataGridRow, this.ColumnsItemsInternal[columnIndex]);
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ComputeScrollBarsLayout()
        {
            if (this._ignoreNextScrollBarsLayout)
            {
                this._ignoreNextScrollBarsLayout = false;
                // 


            }
            double cellsWidth = this.CellsWidth;
            double cellsHeight = this.CellsHeight;

            bool allowHorizScrollbar = false;
            bool forceHorizScrollbar = false;
            double horizScrollBarHeight = 0;
            if (_hScrollBar != null)
            {
                forceHorizScrollbar = this.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible;
                allowHorizScrollbar = forceHorizScrollbar || (this.ColumnsInternal.VisibleColumnCount > 0 &&
                    this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                    this.HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden);
                // Compensate if the horizontal scrollbar is already taking up space
                if (!forceHorizScrollbar && _hScrollBar.Visibility == Visibility.Visible)
                {
                    cellsHeight += this._hScrollBar.DesiredSize.Height;
                }
                horizScrollBarHeight = _hScrollBar.Height;
            }
            bool allowVertScrollbar = false;
            bool forceVertScrollbar = false;
            double vertScrollBarWidth = 0;
            if (_vScrollBar != null)
            {
                forceVertScrollbar = this.VerticalScrollBarVisibility == ScrollBarVisibility.Visible;
                allowVertScrollbar = forceVertScrollbar || (this.ColumnsItemsInternal.Count > 0 &&
                    this.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                    this.VerticalScrollBarVisibility != ScrollBarVisibility.Hidden);
                // Compensate if the vertical scrollbar is already taking up space
                if (!forceVertScrollbar && _vScrollBar.Visibility == Visibility.Visible)
                {
                    cellsWidth += _vScrollBar.DesiredSize.Width;
                }
                vertScrollBarWidth = _vScrollBar.Width;
            }

            // Now cellsWidth is the width potentially available for displaying data cells.
            // Now cellsHeight is the height potentially available for displaying data cells.

            bool needHorizScrollbar = false;
            bool needVertScrollbar = false;

            double totalVisibleWidth = this.ColumnsInternal.VisibleEdgedColumnsWidth;
            double totalVisibleFrozenWidth = this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();

            UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
            double totalVisibleHeight = this.EdgedRowsHeightCalculated;

            if (!forceHorizScrollbar && !forceVertScrollbar)
            {
                bool needHorizScrollbarWithoutVertScrollbar = false;

                if (allowHorizScrollbar &&
                    DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                    DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                    DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight))
                {
                    double oldDataHeight = cellsHeight;
                    cellsHeight -= horizScrollBarHeight;
                    Debug.Assert(cellsHeight >= 0);
                    needHorizScrollbarWithoutVertScrollbar = needHorizScrollbar = true;
                    if (allowVertScrollbar && (DoubleUtil.LessThanOrClose(totalVisibleWidth - cellsWidth, vertScrollBarWidth) ||
                        DoubleUtil.LessThanOrClose(cellsWidth - totalVisibleFrozenWidth, vertScrollBarWidth)))
                    {
                        // Would we still need a horizontal scrollbar without the vertical one?
                        UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                        if (this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                        {
                            needHorizScrollbar = DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth - vertScrollBarWidth);
                        }
                    }

                    if (!needHorizScrollbar)
                    {
                        // Restore old data height because turns out a horizontal scroll bar wouldn't make sense
                        cellsHeight = oldDataHeight;
                    }
                }

                UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                if (allowVertScrollbar &&
                    DoubleUtil.GreaterThan(cellsHeight, 0) &&
                    DoubleUtil.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                    this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                {
                    cellsWidth -= vertScrollBarWidth;
                    Debug.Assert(cellsWidth >= 0);
                    needVertScrollbar = true;
                }

                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                // we compute the number of visible columns only after we set up the vertical scroll bar.
                ComputeDisplayedColumns();

                if (allowHorizScrollbar &&
                    needVertScrollbar && !needHorizScrollbar &&
                    DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                    DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                    DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight))
                {
                    cellsWidth += vertScrollBarWidth;
                    cellsHeight -= horizScrollBarHeight;
                    Debug.Assert(cellsHeight >= 0);
                    needVertScrollbar = false;

                    UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                    if (cellsHeight > 0 &&
                        vertScrollBarWidth <= cellsWidth &&
                        this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                    {
                        cellsWidth -= vertScrollBarWidth;
                        Debug.Assert(cellsWidth >= 0);
                        needVertScrollbar = true;
                    }
                    if (needVertScrollbar)
                    {
                        needHorizScrollbar = true;
                    }
                    else
                    {
                        needHorizScrollbar = needHorizScrollbarWithoutVertScrollbar;
                    }
                }
            }
            else if (forceHorizScrollbar && !forceVertScrollbar)
            {
                if (allowVertScrollbar)
                {
                    if (cellsHeight > 0 &&
                        DoubleUtil.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                        this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount)
                    {
                        cellsWidth -= vertScrollBarWidth;
                        Debug.Assert(cellsWidth >= 0);
                        needVertScrollbar = true;
                    }
                    this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                    ComputeDisplayedColumns();
                }
                needHorizScrollbar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
            }
            else if (!forceHorizScrollbar && forceVertScrollbar)
            {
                if (allowHorizScrollbar)
                {
                    if (cellsWidth > 0 &&
                        DoubleUtil.LessThanOrClose(horizScrollBarHeight, cellsHeight) &&
                        DoubleUtil.GreaterThan(totalVisibleWidth, cellsWidth) &&
                        DoubleUtil.LessThan(totalVisibleFrozenWidth, cellsWidth))
                    {
                        cellsHeight -= horizScrollBarHeight;
                        Debug.Assert(cellsHeight >= 0);
                        needHorizScrollbar = true;
                        UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, cellsHeight);
                    }
                    this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                    ComputeDisplayedColumns();
                }
                needVertScrollbar = this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount;
            }
            else
            {
                Debug.Assert(forceHorizScrollbar && forceVertScrollbar);
                Debug.Assert(allowHorizScrollbar && allowVertScrollbar);
                this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                ComputeDisplayedColumns();
                needVertScrollbar = this.DisplayData.NumTotallyDisplayedScrollingElements != this.VisibleSlotCount;
                needHorizScrollbar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
            }

            UpdateHorizontalScrollBar(needHorizScrollbar, forceHorizScrollbar, totalVisibleWidth, totalVisibleFrozenWidth, cellsWidth);
            UpdateVerticalScrollBar(needVertScrollbar, forceVertScrollbar, totalVisibleHeight, cellsHeight);

            if (this._topRightCornerHeader != null)
            {
                // Show the TopRightHeaderCell based on vertical ScrollBar visibility
                if (this.AreColumnHeadersVisible &&
                    this._vScrollBar != null && this._vScrollBar.Visibility == Visibility.Visible)
                {
                    this._topRightCornerHeader.Visibility = Visibility.Visible;
                }
                else
                {
                    this._topRightCornerHeader.Visibility = Visibility.Collapsed;
                }
            }
            this.DisplayData.FullyRecycleElements();
        }

        // Makes sure horizontal layout is updated to reflect any changes that affect it
        private void EnsureHorizontalLayout()
        {
            this.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
            InvalidateColumnHeadersMeasure();
            InvalidateRowsMeasure(true);
            InvalidateMeasure();
        }

        private void EnsureRowHeaderWidth()
        {
            if (this.AreRowHeadersVisible)
            {
                if (this.AreColumnHeadersVisible)
                {
                    EnsureTopLeftCornerHeader();
                }

                if (_rowsPresenter != null)
                {

                    bool updated = false;

                    foreach (UIElement element in _rowsPresenter.Children)
                    {
                        DataGridRow row = element as DataGridRow;
                        if (row != null)
                        {
                            // If the RowHeader resulted in a different width the last time it was measured, we need
                            // to re-measure it
                            if (row.HeaderCell != null && row.HeaderCell.DesiredSize.Width != this.ActualRowHeaderWidth)
                            {
                                row.HeaderCell.InvalidateMeasure();
                                updated = true;
                            }
                        }
                        else
                        {
                            DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                            if (groupHeader != null && groupHeader.HeaderCell != null && groupHeader.HeaderCell.DesiredSize.Width != this.ActualRowHeaderWidth)
                            {
                                groupHeader.HeaderCell.InvalidateMeasure();
                                updated = true;
                            }
                        }
                    }

                    if (updated)
                    {
                        // We need to update the width of the horizontal scrollbar if the rowHeaders' width actually changed
                        InvalidateMeasure();
                    }
                }
            }
        }

        private void EnsureRowsPresenterVisibility()
        {
            if (_rowsPresenter != null)
            {
                // RowCount doesn't need to be considered, doing so might cause extra Visibility changes
                _rowsPresenter.Visibility = this.ColumnsInternal.FirstVisibleNonFillerColumn == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void EnsureTopLeftCornerHeader()
        {
            if (_topLeftCornerHeader != null)
            {
                _topLeftCornerHeader.Visibility = this.HeadersVisibility == DataGridHeadersVisibility.All ? Visibility.Visible : Visibility.Collapsed;

                if (_topLeftCornerHeader.Visibility == Visibility.Visible)
                {
                    if (!double.IsNaN(this.RowHeaderWidth))
                    {
                        // RowHeaderWidth is set explicitly so we should use that
                        _topLeftCornerHeader.Width = this.RowHeaderWidth;
                    }
                    else if (this.VisibleSlotCount > 0)
                    {
                        // RowHeaders AutoSize and we have at least 1 row so take the desired width
                        _topLeftCornerHeader.Width = this.RowHeadersDesiredWidth;
                    }
                }
            }
        }

        // Recursively expands parent RowGroupHeaders from the top down
        private void ExpandRowGroupParentChain(int level, int slot)
        {
            if (level < 0)
            {
                return;
            }
            int previousHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(slot + 1);
            DataGridRowGroupInfo rowGroupInfo = null;
            while (previousHeaderSlot >= 0)
            {
                rowGroupInfo = this.RowGroupHeadersTable.GetValueAt(previousHeaderSlot);
                Debug.Assert(rowGroupInfo != null);
                if (level == rowGroupInfo.Level)
                {
                    if (_collapsedSlotsTable.Contains(rowGroupInfo.Slot))
                    {
                        // Keep going up the chain
                        ExpandRowGroupParentChain(level - 1, rowGroupInfo.Slot - 1);
                    }
                    if (rowGroupInfo.Visibility != Visibility.Visible)
                    {
                        UpdateRowGroupVisibility(rowGroupInfo, Visibility.Visible, false);
                        rowGroupInfo.Visibility = Visibility.Visible;
                    }
                    return;
                }
                else
                {
                    previousHeaderSlot = this.RowGroupHeadersTable.GetPreviousIndex(previousHeaderSlot);
                }
            }
        }

        private void InvalidateCellsArrange()
        {
            foreach (DataGridRow row in GetAllRows())
            {
                row.InvalidateHorizontalArrange();
            }
        }
        
        private void InvalidateColumnHeadersArrange()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.InvalidateArrange();
            }
        }

        private void InvalidateColumnHeadersMeasure()
        {
            if (_columnHeadersPresenter != null)
            {
                EnsureColumnHeadersVisibility();
                _columnHeadersPresenter.InvalidateMeasure();
            }
        }
        
        private void InvalidateRowsArrange()
        {
            if (_rowsPresenter != null)
            {
                _rowsPresenter.InvalidateArrange();
            }
        }

        private void InvalidateRowsMeasure(bool invalidateIndividualElements)
        {
            if (_rowsPresenter != null)
            {
                _rowsPresenter.InvalidateMeasure();

                if (invalidateIndividualElements)
                {
                    foreach (UIElement element in _rowsPresenter.Children)
                    {
                        element.InvalidateMeasure();
                    }
                }
            }
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!this.ContainsFocus)
            {
                this.ContainsFocus = true;
                ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
                if (this.CurrentColumnIndex != -1 && this.IsSlotVisible(this.CurrentSlot))
                {
                    DataGridRow row = this.DisplayData.GetDisplayedElement(this.CurrentSlot) as DataGridRow;
                    if (row != null)
                    {
                        row.Cells[this.CurrentColumnIndex].ApplyCellState(true /*animate*/);
                    }
                }
            }

            // Keep track of which row contains the newly focused element
            DataGridRow focusedRow = null;
            DependencyObject focusedElement = e.OriginalSource as DependencyObject;
            while (focusedElement != null)
            {
                focusedRow = focusedElement as DataGridRow;
                if (focusedRow != null && focusedRow.OwningGrid == this && _focusedRow != focusedRow)
                {
                    ResetFocusedRow();
                    _focusedRow = focusedRow;
                    break;
                }
                focusedElement = VisualTreeHelper.GetParent(focusedElement);
            }
        }

        private void DataGrid_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateDisabledVisual();
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = ProcessDataGridKey(e);
            }
        }

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && this.CurrentColumnIndex != -1 && e.OriginalSource == this)
            {
                bool success = ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
                Debug.Assert(success);
                if (this.CurrentColumnIndex != -1 && this.SelectedItem == null)
                {
                    SetRowSelection(this.CurrentSlot, true /*isSelected*/, true /*setAnchorSlot*/);
                }
            }
        }

        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.ContainsFocus)
            {
                bool focusLeftDataGrid = true;
                bool dataGridWillReceiveRoutedEvent = true;
                object focusedObject = FocusManager.GetFocusedElement();
                DependencyObject focusedDependencyObject = focusedObject as DependencyObject;

                while (focusedDependencyObject != null)
                {
                    if (focusedDependencyObject == this)
                    {
                        focusLeftDataGrid = false;
                        break;
                    }

                    // Walk up the visual tree.  If we hit the root, try using the framework element's
                    // parent.  We do this because Popups behave differently with respect to the visual tree,
                    // and it could have a parent even if the VisualTreeHelper doesn't find it.
                    DependencyObject parent = VisualTreeHelper.GetParent(focusedDependencyObject);
                    if (parent == null)
                    {
                        FrameworkElement element = focusedDependencyObject as FrameworkElement;
                        if (element != null)
                        {
                            parent = element.Parent;
                            if (parent != null)
                            {
                                dataGridWillReceiveRoutedEvent = false;
                            }
                        }
                    }
                    focusedDependencyObject = parent;
                }

                if (focusLeftDataGrid)
                {
                    this.ContainsFocus = false;
                    if (this.EditingRow != null)
                    {
                        CommitEdit(DataGridEditingUnit.Row, true /*exitEditingMode*/);
                    }
                    ResetFocusedRow();
                    ApplyDisplayedRowsState(this.DisplayData.FirstScrollingSlot, this.DisplayData.LastScrollingSlot);
                    if (this.CurrentColumnIndex != -1 && this.IsSlotVisible(this.CurrentSlot))
                    {
                        DataGridRow row = this.DisplayData.GetDisplayedElement(this.CurrentSlot) as DataGridRow;
                        if (row != null)
                        {
                            row.Cells[this.CurrentColumnIndex].ApplyCellState(true /*animate*/);
                        }
                    }
                }
                else if (!dataGridWillReceiveRoutedEvent)
                {
                    FrameworkElement focusedElement = focusedObject as FrameworkElement;
                    if (focusedElement != null)
                    {
                        focusedElement.LostFocus += new RoutedEventHandler(ExternalEditingElement_LostFocus);
                    }
                }
            }
        }

        private void EditingElement_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                this._bindingValidationError = true;
            }
        }

        private void EditingElement_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= new RoutedEventHandler(EditingElement_Loaded);
            }
            PreparingCellForEditPrivate(element);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private bool EndCellEdit(DataGridEditAction editAction, bool exitEditingMode, bool keepFocus, bool raiseEvents)
        {
            if (this._editingColumnIndex == -1)
            {
                return true;
            }
            Debug.Assert(this.EditingRow != null);
            Debug.Assert(this._editingColumnIndex >= 0);
            Debug.Assert(this._editingColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
            Debug.Assert(this.EditingRow != null && this.EditingRow.Slot == this.CurrentSlot);

            // Cache these to see if they change later
            int currentSlot = this.CurrentSlot;
            int currentColumnIndex = this.CurrentColumnIndex;

            // We're ready to start ending, so raise the event
            DataGridCell editingCell = this.EditingRow.Cells[this._editingColumnIndex];
            FrameworkElement editingElement = editingCell.Content as FrameworkElement;
            if (editingElement == null)
            {
                return false;
            }
            if (raiseEvents)
            {
                DataGridCellEditEndingEventArgs e = new DataGridCellEditEndingEventArgs(this.CurrentColumn, this.EditingRow, editingElement, editAction);
                OnCellEditEnding(e);
                if (e.Cancel)
                {
                    // CellEditEnding has been cancelled
                    return false;
                }

                // Ensure that the current cell wasn't changed in the user's CellEditEnding handler
                if (this._editingColumnIndex == -1 ||
                    currentSlot != this.CurrentSlot ||
                    currentColumnIndex != this.CurrentColumnIndex)
                {
                    return true;
                }
                Debug.Assert(this.EditingRow != null);
                Debug.Assert(this.EditingRow.Slot == currentSlot);
                Debug.Assert(this._editingColumnIndex != -1);
                Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
            }

            // If we're canceling, let the editing column repopulate its old value if it wants
            if (editAction == DataGridEditAction.Cancel)
            {
                this.CurrentColumn.CancelCellEditInternal(editingElement, this._uneditedValue);

                // Ensure that the current cell wasn't changed in the user column's CancelCellEdit
                if (this._editingColumnIndex == -1 ||
                    currentSlot != this.CurrentSlot ||
                    currentColumnIndex != this.CurrentColumnIndex)
                {
                    return true;
                }
                Debug.Assert(this.EditingRow != null);
                Debug.Assert(this.EditingRow.Slot == currentSlot);
                Debug.Assert(this._editingColumnIndex != -1);
                Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
            }

            // If we're committing, explicitly update the source but watch out for any validation errors
            bool isValid = true;
            if (editAction == DataGridEditAction.Commit)
            {
                foreach (BindingInfo bindingData in this.CurrentColumn.GetInputBindings(editingElement, this.CurrentItem))
                {
                    bindingData.Element.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(EditingElement_BindingValidationError);
                    bindingData.BindingExpression.UpdateSource();
                }
                if (this._bindingValidationError)
                {
                    ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/);
                    this._bindingValidationError = false;
                    isValid = false;
                }
            }

            // Update the visual state of the cell
            editingCell.IsValid = isValid;
            editingCell.ApplyCellState(true);

            // Default the row and DataGrid IsValid to false if the cell is invalid
            if (!isValid)
            {
                this.IsValid = isValid;
                this.EditingRow.IsValid = isValid;
                this.EditingRow.ApplyState(true);
                return false;
            }

            // Revalidate the row if there are any entity-level errors associated with the cell
            // that just ended edit or not associated with any specific cell at all
            if (!this.EditingRow.IsValid)
            {
                if (this._validationResults.Count > 0)
                {
                    foreach (ValidationResult validationResult in this._validationResults)
                    {
                        bool revalidate = false;
                        foreach (string memberName in validationResult.MemberNames)
                        {
                            Debug.Assert(this.DataConnection.DataType != null);
                            if (memberName == this.DataConnection.DataType.Name ||
                                this.ColumnsItemsInternal[this.EditingColumnIndex].BindingPaths.Contains(memberName))
                            {
                                revalidate = true;
                                break;
                            }
                        }
                        if (revalidate)
                        {
                            ValidateEditingRow();
                            break;
                        }
                    }
                }
                else
                {
                    this.IsValid = true;
                    this.EditingRow.IsValid = true;
                    this.EditingRow.ApplyState(true);
                }
            }

            // Detach from any remaining BindingValidationError events
            // note: We do this outside of the Commit scope in case CancelEdit was called within the handler
            foreach (BindingInfo bindingData in this.CurrentColumn.GetInputBindings(editingElement, this.CurrentItem))
            {
                bindingData.Element.BindingValidationError -= new EventHandler<ValidationErrorEventArgs>(EditingElement_BindingValidationError);
            }

            if (exitEditingMode)
            {
                this._editingColumnIndex = -1;
                editingCell.ApplyCellState(true /*animate*/);

                //
                this.IsTabStop = true;
                if (keepFocus && editingElement.ContainsFocusedElement())
                {
                    this.Focus();
                }

                PopulateCellContent(!exitEditingMode /*isCellEdited*/, this.CurrentColumn, this.EditingRow, editingCell);
            }

            // We're done, so raise the CellEditEnded event
            if (raiseEvents)
            {
                OnCellEditEnded(new DataGridCellEditEndedEventArgs(this.CurrentColumn, this.EditingRow, editAction));
            }

            // There's a chance that somebody reopened this cell for edit within the CellEditEnded handler,
            // so we should return false if we were supposed to exit editing mode, but we didn't
            return !(exitEditingMode && currentColumnIndex == this._editingColumnIndex);
        }

        private bool EndRowEdit(DataGridEditAction editAction, bool exitEditingMode, bool raiseEvents)
        {
            if (this.EditingRow == null)
            {
                return true;
            }
            if (this._editingColumnIndex != -1 || (editAction == DataGridEditAction.Cancel && raiseEvents && !(this.EditingRow.DataContext is IEditableObject)))
            {
                // Ending the row edit will fail immediately under the following conditions:
                // 1. We haven't ended the cell edit yet.
                // 2. We're trying to cancel edit when the underlying DataType is not an IEditableObject,
                //    because we have no way to properly restore the old value.  We will only allow this to occur
                //    if raiseEvents == false, which means we're internally forcing a cancel.
                return false;
            }
            DataGridRow editingRow = this.EditingRow;

            if (raiseEvents)
            {
                DataGridRowEditEndingEventArgs e = new DataGridRowEditEndingEventArgs(this.EditingRow, editAction);
                OnRowEditEnding(e);
                if (e.Cancel)
                {
                    // RowEditEnding has been cancelled
                    return false;
                }

                // Editing states might have been changed in the RowEditEnding handlers
                if (this._editingColumnIndex != -1)
                {
                    return false;
                }
                if (editingRow != this.EditingRow)
                {
                    return true;
                }
            }

            // Call the appropriate commit or cancel methods
            if (editAction == DataGridEditAction.Commit)
            {
                if (!CommitRowEdit(exitEditingMode))
                {
                    return false;
                }
            }
            else
            {
                CancelRowEdit(exitEditingMode);
            }
            ResetValidationStatus();

            // Update the previously edited row's state
            if (exitEditingMode && editingRow == this.EditingRow)
            {
                ResetEditingRow();
                if (!this.IsSlotVisible(editingRow.Slot) && this._rowsPresenter != null && editingRow != this._focusedRow)
                {
                    // 
                    this._rowsPresenter.Children.Remove(editingRow);
                }
                else
                {
                    editingRow.ApplyState(true /*animate*/);
                }
            }

            // Raise the RowEditEnded event
            if (raiseEvents)
            {
                OnRowEditEnded(new DataGridRowEditEndedEventArgs(editingRow, editAction));
            }

            return true;
        }

        private void EnsureColumnHeadersVisibility()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Visibility = this.AreColumnHeadersVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Applies the given Style to the Row if it's supposed to use DataGrid.RowStyle
        private static void EnsureElementStyle(FrameworkElement element, Style oldDataGridStyle, Style newDataGridStyle)
        {
            Debug.Assert(element != null);

            // Apply the DataGrid style if the row was using the old DataGridRowStyle before
            if (element != null && (element.Style == null || element.Style == oldDataGridStyle))
            {
                element.SetStyleWithType(newDataGridStyle);
            }
        }

        private void EnsureVerticalGridLines()
        {
            if (this.AreColumnHeadersVisible)
            {
                double totalColumnsWidth = 0;
                foreach (DataGridColumn column in this.ColumnsInternal)
                {
                    totalColumnsWidth += column.ActualWidth;

                    column.HeaderCell.SeparatorVisibility = (column != this.ColumnsInternal.LastVisibleColumn || totalColumnsWidth < this.CellsWidth) ?
                        Visibility.Visible : Visibility.Collapsed;
                }
            }

            foreach (DataGridRow row in GetAllRows())
            {
                row.EnsureGridLines();
            }
        }

        private void ErrorsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = ErrorsListBox_Click();
            }
        }

        private void ErrorsListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = ErrorsListBox_Click();
        }

        private void ErrorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When the selected error changes, we want to mark related cells as Invalid
            this._clickedErrorColumns.Clear();
            foreach (DataGridColumn column in this.Columns)
            {
                bool isValid = true;
                if (e.AddedItems.Count == 1)
                {
                    ValidationResult selectedValidationResult = this._errorsListBox.SelectedItem as ValidationResult;
                    if (selectedValidationResult != null && selectedValidationResult.MemberNames != null)
                    {
                        foreach (string property in selectedValidationResult.MemberNames)
                        {
                            if (column.BindingPaths.Contains(property))
                            {
                                this._clickedErrorColumns.Enqueue(column);
                                isValid = false;
                                break;
                            }
                        }
                    }
                }
                if (this.EditingRow.Cells[column.Index].IsValid != isValid)
                {
                    this.EditingRow.Cells[column.Index].IsValid = isValid;
                    this.EditingRow.Cells[column.Index].ApplyCellState(true);
                }
            }
        }

        /// <summary>
        /// Exits editing mode without trying to commit or revert the editing, and 
        /// without repopulating the edited row's cell.
        /// </summary>
        private void ExitEdit(bool keepFocus)
        {
            if (this.EditingRow == null)
            {
                Debug.Assert(this._editingColumnIndex == -1);
                return;
            }

            if (this._editingColumnIndex != -1)
            {
                Debug.Assert(this._editingColumnIndex >= 0);
                Debug.Assert(this._editingColumnIndex < this.ColumnsItemsInternal.Count);
                Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
                Debug.Assert(this.EditingRow != null && this.EditingRow.Slot == this.CurrentSlot);

                this._editingColumnIndex = -1;
                this.EditingRow.Cells[this.CurrentColumnIndex].ApplyCellState(false /*animate*/);
            }
            //
            this.IsTabStop = true;
            if (this.IsSlotVisible(this.EditingRow.Slot))
            {
                this.EditingRow.ApplyState(true /*animate*/);
            }
            ResetEditingRow();
            if (keepFocus)
            {
                bool success = Focus();
                Debug.Assert(success);
            }
        }

        private void ExternalEditingElement_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                element.LostFocus -= new RoutedEventHandler(ExternalEditingElement_LostFocus);
                DataGrid_LostFocus(sender, e);
            }
        }

        private void FlushCurrentCellChanged()
        {
            if (this.SelectionHasChanged)
            {
                // selection is changing, don't raise CurrentCellChanged until it's done
                this._flushCurrentCellChanged = true;
                FlushSelectionChanged();
                return;
            }
            if (this.CurrentColumn != this._previousCurrentColumn
                || this.CurrentItem != this._previousCurrentItem)
            {
                this.CoerceSelectedItem();
                this._previousCurrentColumn = this.CurrentColumn;
                this._previousCurrentItem = this.CurrentItem;
                OnCurrentCellChanged(EventArgs.Empty);
            }
            this._flushCurrentCellChanged = false;
        }

        private void FlushSelectionChanged()
        {
            if (this.SelectionHasChanged && this._noSelectionChangeCount == 0)
            {
                this.CoerceSelectedItem();
                if (this.NoCurrentCellChangeCount != 0)
                {
                    // current cell is changing, don't raise SelectionChanged until it's done
                    return;
                }
                this.SelectionHasChanged = false;

                if (this._flushCurrentCellChanged)
                {
                    FlushCurrentCellChanged();
                }

                SelectionChangedEventArgs e = this._selectedItems.GetSelectionChangedEventArgs();
                if (e.AddedItems.Count > 0 || e.RemovedItems.Count > 0)
                {
                    OnSelectionChanged(e);
                }
            }
        }

        private bool FocusEditingCell(bool setFocus)
        {
            Debug.Assert(this.CurrentColumnIndex >= 0);
            Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.CurrentSlot >= -1);
            Debug.Assert(this.CurrentSlot < this.SlotCount);
            Debug.Assert(this.EditingRow != null && this.EditingRow.Slot == this.CurrentSlot);
            Debug.Assert(this._editingColumnIndex != -1);

            //

            this.IsTabStop = false;
            this._focusEditingControl = false;

            bool success = false;
            DataGridCell dataGridCell = this.EditingRow.Cells[this._editingColumnIndex];
            FrameworkElement editingElement = dataGridCell.Content as FrameworkElement;
            if (editingElement != null && setFocus)
            {
                success = editingElement.DeepFocus();
                this._focusEditingControl = !success;
            }
            return success;
        }

        // Calculates the amount to scroll for the ScrollLeft button
        // This is a method rather than a property to emphasize a calculation
        private double GetHorizontalSmallScrollDecrease()
        {
            // If the first column is covered up, scroll to the start of it when the user clicks the left button
            if (_negHorizontalOffset > 0)
            {
                return _negHorizontalOffset;
            }
            else
            {
                // The entire first column is displayed, show the entire previous column when the user clicks
                // the left button
                DataGridColumn previousColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(
                    this.ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]);
                if (previousColumn != null)
                {
                    return GetEdgedColumnWidth(previousColumn);
                }
                else
                {
                    // There's no previous column so don't move
                    return 0;
                }
            }
        }

        // Calculates the amount to scroll for the ScrollRight button
        // This is a method rather than a property to emphasize a calculation
        private double GetHorizontalSmallScrollIncrease()
        {
            if (this.DisplayData.FirstDisplayedScrollingCol >= 0)
            {
                return GetEdgedColumnWidth(this.ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]) - _negHorizontalOffset;
            }
            return 0;
        }

        // Calculates the amount the ScrollDown button should scroll
        // This is a method rather than a property to emphasize that calculations are taking place
        private double GetVerticalSmallScrollIncrease()
        {
            if (this.DisplayData.FirstScrollingSlot >= 0)
            {
                return GetExactSlotElementHeight(this.DisplayData.FirstScrollingSlot) - this.NegVerticalOffset;
            }
            return 0;
        }

        private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ProcessHorizontalScroll(e.ScrollEventType);
        }    

        private bool IsColumnOutOfBounds(int columnIndex)
        {
            return columnIndex >= this.ColumnsItemsInternal.Count || columnIndex < 0;
        }

        private bool IsInnerCellOutOfBounds(int columnIndex, int slot)
        {
            return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfBounds(slot);
        }

        private bool IsSlotOutOfBounds(int slot)
        {
            return slot >= this.SlotCount || slot < -1 || _collapsedSlotsTable.Contains(slot);
        }

        private void MakeFirstDisplayedCellCurrentCell()
        {
            if (this.CurrentColumnIndex != -1)
            {
                this._makeFirstDisplayedCellCurrentCellPending = false;
                return;
            }
            if (this.DisplayData.FirstScrollingSlot == -1)
            {
                this._makeFirstDisplayedCellCurrentCellPending = true;
                return;
            }

            // No current cell, therefore no selection either - try to set the current cell to the
            // ItemsSource's ICollectionView.CurrentItem if it exists, otherwise use the first displayed cell.
            int slot = 0;
            if (this.DataConnection.CollectionView != null
                && !this.DataConnection.CollectionView.IsCurrentBeforeFirst
                && !this.DataConnection.CollectionView.IsCurrentAfterLast)
            {
                slot = SlotFromRowIndex(this.DataConnection.CollectionView.CurrentPosition);
                if (this.DataConnection.CollectionView.CurrentPosition < 0
                    || slot >= this.SlotCount)
                {
                    _makeFirstDisplayedCellCurrentCellPending = true;
                    return;
                }
            }
            else
            {
                if (this.SelectedIndex == -1)
                {
                    // Try to default to the first row
                    slot = SlotFromRowIndex(0);
                    if (!this.IsSlotVisible(slot))
                    {
                        slot = -1;
                    }
                }
                else
                {
                    slot = SlotFromRowIndex(this.SelectedIndex);
                }
            }
            int columnIndex = this.FirstDisplayedNonFillerColumnIndex;
            if (_desiredCurrentColumnIndex >= 0 && _desiredCurrentColumnIndex < this.ColumnsItemsInternal.Count)
            {
                columnIndex = _desiredCurrentColumnIndex;
            }

            if (columnIndex != -1 && slot != -1 && this.SlotCount > 0)
            {
                SetAndSelectCurrentCell(columnIndex,
                                        slot,
                                        true /*forceCurrentCellSelection*/);
                this.AnchorSlot = slot;
                this._makeFirstDisplayedCellCurrentCellPending = false;
                this._desiredCurrentColumnIndex = -1;
            }
            else
            {
                this._makeFirstDisplayedCellCurrentCellPending = true;
            }
        }

        private void PopulateCellContent(bool isCellEdited,
                                         DataGridColumn dataGridColumn,
                                         DataGridRow dataGridRow,
                                         DataGridCell dataGridCell)
        {
            Debug.Assert(dataGridColumn != null);
            Debug.Assert(dataGridRow != null);
            Debug.Assert(dataGridCell != null);

            FrameworkElement element = null;
            DataGridBoundColumn dataGridBoundColumn = dataGridColumn as DataGridBoundColumn;
            if (isCellEdited)
            {
                // Generate EditingElement and apply column style if available
                element = dataGridColumn.GenerateEditingElementInternal(dataGridCell, dataGridRow.DataContext);
                if (element != null)
                {
                    if (dataGridBoundColumn != null && dataGridBoundColumn.EditingElementStyle != null)
                    {
                        element.SetStyleWithType(dataGridBoundColumn.EditingElementStyle);
                    }

                    // Subscribe to the new element's events
                    element.Loaded += new RoutedEventHandler(EditingElement_Loaded);
                }
            }
            else
            {
                // Generate Element and apply column style if available
                element = dataGridColumn.GenerateElementInternal(dataGridCell, dataGridRow.DataContext);
                if (element != null)
                {
                    if (dataGridBoundColumn != null && dataGridBoundColumn.ElementStyle != null)
                    {
                        element.SetStyleWithType(dataGridBoundColumn.ElementStyle);
                    }
                }
            }

            dataGridCell.Content = element;
        }

        private void PreparingCellForEditPrivate(FrameworkElement editingElement)
        {
            if (this._editingColumnIndex == -1 ||
                this.CurrentColumnIndex == -1 ||
                this.EditingRow.Cells[this.CurrentColumnIndex].Content != editingElement)
            {
                // The current cell has changed since the call to BeginCellEdit, so the fact
                // that this element has loaded is no longer relevant
                return;
            }

            Debug.Assert(this.EditingRow != null);
            Debug.Assert(this._editingColumnIndex >= 0);
            Debug.Assert(this._editingColumnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(this._editingColumnIndex == this.CurrentColumnIndex);
            Debug.Assert(this.EditingRow != null && this.EditingRow.Slot == this.CurrentSlot);

            FocusEditingCell(this.ContainsFocus || this._focusEditingControl /*setFocus*/);

            // Prepare the cell for editing and raise the PreparingCellForEdit event for all columns
            DataGridColumn dataGridColumn = this.CurrentColumn;
            this._uneditedValue = dataGridColumn.PrepareCellForEditInternal(editingElement, this._editingEventArgs);
            OnPreparingCellForEdit(new DataGridPreparingCellForEditEventArgs(dataGridColumn, this.EditingRow, this._editingEventArgs, editingElement));
        }

        private bool ProcessAKey()
        {
            bool ctrl, shift, alt;

            KeyboardHelper.GetMetaKeyState(out ctrl, out shift, out alt);

            if (ctrl && !shift && !alt && this.SelectionMode == DataGridSelectionMode.Extended)
            {
                SelectAll();
                return true;
            }
            return false;
        }

        private bool ProcessDataGridKey(KeyEventArgs e)
        {
            bool focusDataGrid = false;
            switch (e.Key)
            {
                case Key.Tab:
                    return ProcessTabKey(e);

                case Key.Up:
                    focusDataGrid = ProcessUpKey();
                    break;

                case Key.Down:
                    focusDataGrid = ProcessDownKey();
                    break;

                case Key.PageDown:
                    focusDataGrid = ProcessNextKey();
                    break;

                case Key.PageUp:
                    focusDataGrid = ProcessPriorKey();
                    break;

                case Key.Left:
                    focusDataGrid = ProcessLeftKey();
                    break;

                case Key.Right:
                    focusDataGrid = ProcessRightKey();
                    break;

                case Key.F2:
                    return ProcessF2Key(e);

                case Key.Home:
                    focusDataGrid = ProcessHomeKey();
                    break;

                case Key.End:
                    focusDataGrid = ProcessEndKey();
                    break;

                case Key.Enter:
                    focusDataGrid = ProcessEnterKey();
                    break;

                case Key.Escape:
                    return ProcessEscapeKey();

                case Key.A:
                    return ProcessAKey();
            }
            if (focusDataGrid && this.IsTabStop)
            {
                this.Focus();
            }
            return focusDataGrid;
        }

        private bool ProcessDownKeyInternal(bool shift, bool ctrl)
        {
            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleColumn;
            int firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            int lastSlot = this.LastVisibleSlot;
            if (firstVisibleColumnIndex == -1 || lastSlot == -1)
            {
                return false;
            }
            int nextSlot = -1;
            if (this.CurrentSlot != -1)
            {
                nextSlot = this.GetNextVisibleSlot(this.CurrentSlot);
                if (nextSlot >= this.SlotCount)
                {
                    nextSlot = -1;
                }
            }

            _noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                int columnIndex;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = lastSlot;
                    columnIndex = firstVisibleColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else if (ctrl)
                {
                    if (shift)
                    {
                        // Both Ctrl and Shift
                        desiredSlot = lastSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = (this.SelectionMode == DataGridSelectionMode.Extended)
                            ? DataGridSelectionAction.SelectFromAnchorToCurrent
                            : DataGridSelectionAction.SelectCurrent;
                    }
                    else
                    {
                        // Ctrl without Shift
                        desiredSlot = lastSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                else
                {
                    if (nextSlot == -1)
                    {
                        return true;
                    }
                    if (shift)
                    {
                        // Shift without Ctrl
                        desiredSlot = nextSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                    }
                    else
                    {
                        // Neither Ctrl nor Shift
                        desiredSlot = nextSlot;
                        columnIndex = this.CurrentColumnIndex;
                        action = DataGridSelectionAction.SelectCurrent;
                    }
                }
                UpdateSelectionAndCurrency(columnIndex, desiredSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessEscapeKey()
        {
            if (this._editingColumnIndex != -1)
            {
                // Revert the potential cell editing and exit cell editing.
                EndCellEdit(DataGridEditAction.Cancel, true /*exitEditingMode*/, true /*keepFocus*/, true /*raiseEvents*/);
                return true;
            }
            else if (this.EditingRow != null)
            {
                // Revert the potential row editing and exit row editing.
                EndRowEdit(DataGridEditAction.Cancel, true /*exitEditingMode*/, true /*raiseEvents*/);
                return true;
            }
            return false;
        }

        private bool ProcessF2Key(KeyEventArgs e)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            if (!shift && !ctrl &&
                this._editingColumnIndex == -1 && this.CurrentColumnIndex != -1 && GetRowSelection(this.CurrentSlot) &&
                !GetColumnEffectiveReadOnlyState(this.CurrentColumn))
            {
                if (ScrollSlotIntoView(this.CurrentColumnIndex, this.CurrentSlot, false /*forCurrentCellChange*/, true /*forceHorizontalScroll*/))
                {
                    BeginCellEdit(e);
                }
                return true;
            }

            return false;
        }

        // Ctrl Left <==> Home
        private bool ProcessLeftMost(int firstVisibleColumnIndex, int firstVisibleSlot)
        {
            this._noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = firstVisibleSlot;
                    action = DataGridSelectionAction.SelectCurrent;
                    Debug.Assert(_selectedItems.Count == 0);
                }
                else
                {
                    desiredSlot = this.CurrentSlot;
                    action = DataGridSelectionAction.None;
                }
                UpdateSelectionAndCurrency(firstVisibleColumnIndex, desiredSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        // Ctrl Right <==> End
        private bool ProcessRightMost(int lastVisibleColumnIndex, int firstVisibleSlot)
        {
            this._noSelectionChangeCount++;
            try
            {
                int desiredSlot;
                DataGridSelectionAction action;
                if (this.CurrentColumnIndex == -1)
                {
                    desiredSlot = firstVisibleSlot;
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    desiredSlot = this.CurrentSlot;
                    action = DataGridSelectionAction.None;
                }
                UpdateSelectionAndCurrency(lastVisibleColumnIndex, desiredSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }
            return this._successfullyUpdatedSelection;
        }

        private bool ProcessTabKey(KeyEventArgs e)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            if (ctrl || this._editingColumnIndex == -1 || this.IsReadOnly)
            {
                //Go to the next/previous control on the page when 
                // - Ctrl key is used
                // - Potential current cell is not edited, or the datagrid is read-only. 
                return false;
            }

            // Try to locate a writable cell before/after the current cell
            Debug.Assert(this.CurrentColumnIndex != -1);
            Debug.Assert(this.CurrentSlot != -1);

            int neighborVisibleWritableColumnIndex, neighborSlot;
            DataGridColumn dataGridColumn;
            if (shift)
            {
                dataGridColumn = this.ColumnsInternal.GetPreviousVisibleWritableColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                neighborSlot = GetPreviousVisibleSlot(this.CurrentSlot);
            }
            else
            {
                dataGridColumn = this.ColumnsInternal.GetNextVisibleWritableColumn(this.ColumnsItemsInternal[this.CurrentColumnIndex]);
                neighborSlot = GetNextVisibleSlot(this.CurrentSlot);
            }
            neighborVisibleWritableColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;

            if (neighborVisibleWritableColumnIndex == -1 && neighborSlot == -1)
            {
                // There is no previous/next row and no previous/next writable cell on the current row
                return false;
            }

            int targetSlot = -1, targetColumnIndex = -1;

            this._noSelectionChangeCount++;
            try
            {
                if (neighborVisibleWritableColumnIndex == -1)
                {
                    targetSlot = neighborSlot;
                    if (shift)
                    {
                        Debug.Assert(this.ColumnsInternal.LastVisibleWritableColumn != null);
                        targetColumnIndex = this.ColumnsInternal.LastVisibleWritableColumn.Index;
                    }
                    else
                    {
                        Debug.Assert(this.ColumnsInternal.FirstVisibleWritableColumn != null);
                        targetColumnIndex = this.ColumnsInternal.FirstVisibleWritableColumn.Index;
                    }
                }
                else
                {
                    targetSlot = this.CurrentSlot;
                    targetColumnIndex = neighborVisibleWritableColumnIndex;
                }

                DataGridSelectionAction action;
                if (targetSlot != this.CurrentSlot || (this.SelectionMode == DataGridSelectionMode.Extended))
                {
                    if (IsSlotOutOfBounds(targetSlot))
                    {
                        return true;
                    }
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    action = DataGridSelectionAction.None;
                }
                UpdateSelectionAndCurrency(targetColumnIndex, targetSlot, action, true /*scrollIntoView*/);
            }
            finally
            {
                this.NoSelectionChangeCount--;
            }

            if (this._successfullyUpdatedSelection && !this.RowGroupHeadersTable.Contains(targetSlot))
            {
                Debug.Assert(this.ContainsFocus);
                BeginCellEdit(e);
            }

            // Return true to say we handled the key event even if the operation was unsuccessful. If we don't
            // say we handled this event, the framework will continue to process the tab key and change focus.
            return true;
        }

        private void RemoveDisplayedColumnHeader(DataGridColumn dataGridColumn)
        {
            if (this.AreColumnHeadersVisible && _columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Children.Remove(dataGridColumn.HeaderCell);
            }
        }

        private void RemoveDisplayedColumnHeaders()
        {
            if (_columnHeadersPresenter != null)
            {
                _columnHeadersPresenter.Children.Clear();
            }
            this.ColumnsInternal.FillerColumn.IsRepresented = false;
        }

        private bool ResetCurrentCellCore()
        {
            return (this.CurrentColumnIndex == -1 || SetCurrentCellCore(-1, -1));
        } 

        private void ResetEditingRow()
        {
            if (this.EditingRow != null
                && this.EditingRow != this._focusedRow
                && !IsSlotVisible(this.EditingRow.Slot))
            {
                // Unload the old editing row if it's off screen
                this.EditingRow.Clip = null;
                UnloadRow(this.EditingRow);
                this.DisplayData.FullyRecycleElements();
            }
            this.EditingRow = null;
        }

        private void ResetFocusedRow()
        {
            if (this._focusedRow != null
                && this._focusedRow != this.EditingRow
                && !IsSlotVisible(this._focusedRow.Slot))
            {
                // Unload the old focused row if it's off screen
                this._focusedRow.Clip = null;
                UnloadRow(this._focusedRow);
                this.DisplayData.FullyRecycleElements();
            }
            this._focusedRow = null;
        }

        private void ResetValidationStatus()
        {
            // Clear the invalid status of the Cell, Row and DataGrid
            if (this.EditingRow != null)
            {
                foreach (DataGridCell cell in this.EditingRow.Cells)
                {
                    if (!cell.IsValid)
                    {
                        cell.IsValid = true;
                        cell.ApplyCellState(true);
                    }
                }
                this.IsValid = this.EditingRow.IsValid = true;
                this.EditingRow.ApplyState(true);
            }

            // Clear the previous validation results
            this._validationResults.Clear();

            // Hide the error list if validation succeeded
            if (this._errorsListBox != null && this._errorsListBox.Visibility != Visibility.Collapsed)
            {
                this._errorsListBox.Visibility = Visibility.Collapsed;
                int editingRowSlot = this.EditingRow.Slot;

                InvalidateMeasure();
                this.Dispatcher.BeginInvoke(delegate
                {
                    ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                });
            }
        }

        private void RowGroupHeaderStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_rowsPresenter != null)
            {
                Style oldLastStyle = _rowGroupHeaderStylesOld.Count > 0 ? _rowGroupHeaderStylesOld[_rowGroupHeaderStylesOld.Count - 1] : null;
                while (_rowGroupHeaderStylesOld.Count < _rowGroupHeaderStyles.Count)
                {
                    _rowGroupHeaderStylesOld.Add(oldLastStyle);
                }

                Style lastStyle = _rowGroupHeaderStyles.Count > 0 ? _rowGroupHeaderStyles[_rowGroupHeaderStyles.Count - 1] : null;
                foreach (UIElement element in _rowsPresenter.Children)
                {
                    DataGridRowGroupHeader groupHeader = element as DataGridRowGroupHeader;
                    if (groupHeader != null)
                    {
                        Style oldStyle = groupHeader.Level < _rowGroupHeaderStylesOld.Count ? _rowGroupHeaderStylesOld[groupHeader.Level] : oldLastStyle; 
                        Style newStyle = groupHeader.Level < _rowGroupHeaderStyles.Count ? _rowGroupHeaderStyles[groupHeader.Level] : lastStyle;
                        EnsureElementStyle(groupHeader, oldStyle, newStyle);
                    }
                }
            }
            _rowGroupHeaderStylesOld.Clear();
            foreach (Style style in _rowGroupHeaderStyles)
            {
                _rowGroupHeaderStylesOld.Add(style);
            }
        }

        private void SelectAll()
        {
            SetRowsSelection(0, this.SlotCount - 1);
        }

        private void SetAndSelectCurrentCell(int columnIndex,
                                             int slot,
                                             bool forceCurrentCellSelection)
        {
            DataGridSelectionAction action = forceCurrentCellSelection ? DataGridSelectionAction.SelectCurrent : DataGridSelectionAction.None;
            UpdateSelectionAndCurrency(columnIndex, slot, action, false /*scrollIntoView*/);
        }

        // columnIndex = 2, rowIndex = -1 --> current cell belongs to the 'new row'.
        // columnIndex = 2, rowIndex = 2 --> current cell is an inner cell
        // columnIndex = -1, rowIndex = -1 --> current cell is reset
        // columnIndex = -1, rowIndex = 2 --> Unexpected
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private bool SetCurrentCellCore(int columnIndex, int slot, bool commitEdit, bool endRowEdit)
        {
            Debug.Assert(columnIndex < this.ColumnsItemsInternal.Count);
            Debug.Assert(slot < this.SlotCount);
            Debug.Assert(columnIndex == -1 || this.ColumnsItemsInternal[columnIndex].Visibility == Visibility.Visible);

            Debug.Assert(!(columnIndex > -1 && slot == -1)); 

            if (columnIndex == this.CurrentColumnIndex &&
                slot == this.CurrentSlot)
            {
                Debug.Assert(this._editingColumnIndex == -1 || this._editingColumnIndex == this.CurrentColumnIndex);
                Debug.Assert(this.EditingRow == null || this.EditingRow.Slot == this.CurrentSlot);
                return true;
            }

            UIElement oldDisplayedElement = null;
            DataGridCellCoordinates oldCurrentCell = new DataGridCellCoordinates(this.CurrentCellCoordinates);

            if (this.CurrentColumnIndex > -1)
            {
                Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
                Debug.Assert(this.CurrentSlot < this.SlotCount);

                if (!this.RowGroupHeadersTable.Contains(oldCurrentCell.Slot) && !this._temporarilyResetCurrentCell)
                {
                    bool keepFocus = this.ContainsFocus;
                    if (commitEdit)
                    {
                        if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, keepFocus, true /*raiseEvents*/))
                        {
                            return false;
                        }
                        // Resetting the current cell: setting it to (-1, -1) is not considered setting it out of bounds
                        if ((columnIndex != -1 && slot != -1 && IsInnerCellOutOfBounds(columnIndex, slot)) ||
                            IsInnerCellOutOfBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot))
                        {
                            return false;
                        }
                        if (endRowEdit && !EndRowEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, true /*raiseEvents*/))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        this.CancelEdit(DataGridEditingUnit.Row, false);
                        ExitEdit(keepFocus);
                    }
                }

                if (!IsInnerCellOutOfBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot) &&
                    this.IsSlotVisible(oldCurrentCell.Slot))
                {
                    oldDisplayedElement = this.DisplayData.GetDisplayedElement(oldCurrentCell.Slot);
                }
            }

            this.CurrentColumnIndex = columnIndex;
            this.CurrentSlot = slot;

            if (this._temporarilyResetCurrentCell)
            {
                if (columnIndex != -1)
                {
                    this._temporarilyResetCurrentCell = false;
                }
            }
            if (!this._temporarilyResetCurrentCell && this._editingColumnIndex != -1)
            {
                this._editingColumnIndex = columnIndex;
            }

            if (oldDisplayedElement != null)
            {
                DataGridRow row = oldDisplayedElement as DataGridRow;
                if (row != null)
                {
                    // Don't reset the state of the current cell if we're editing it because that would put it in an invalid state
                    UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, !(this._temporarilyResetCurrentCell && row.IsEditing && this._editingColumnIndex == oldCurrentCell.ColumnIndex));
                }
                else
                {
                    UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, false /*applyCellState*/);
                }
            }

            if (this.CurrentColumnIndex > -1)
            {
                Debug.Assert(this.CurrentSlot > -1);
                Debug.Assert(this.CurrentColumnIndex < this.ColumnsItemsInternal.Count);
                Debug.Assert(this.CurrentSlot < this.SlotCount);
                if (this.IsSlotVisible(this.CurrentSlot))
                {
                    UpdateCurrentState(this.DisplayData.GetDisplayedElement(this.CurrentSlot), this.CurrentColumnIndex, true /*applyCellState*/);
                }
            }

            return true;
        }

        private void SetVerticalOffset(double newVerticalOffset)
        {
            _verticalOffset = newVerticalOffset;
            if (_vScrollBar != null && !DoubleUtil.AreClose(newVerticalOffset, _vScrollBar.Value))
            {
                _vScrollBar.Value = _verticalOffset;
            }
        }

        private void UpdateCurrentState(UIElement displayedElement, int columnIndex, bool applyCellState)
        {
            DataGridRow row = displayedElement as DataGridRow;
            if (row != null)
            {
                if (this.AreRowHeadersVisible)
                {
                    row.ApplyHeaderStatus(true /*animate*/);
                }
                DataGridCell cell = row.Cells[columnIndex];
                if (applyCellState)
                {
                    cell.ApplyCellState(true /*animate*/);
                }
            }
            else
            {
                DataGridRowGroupHeader groupHeader = displayedElement as DataGridRowGroupHeader;
                if (groupHeader != null)
                {
                    groupHeader.ApplyState(true /*useTransitions*/);
                    if (this.AreRowHeadersVisible)
                    {
                        groupHeader.ApplyHeaderStatus(true /*animate*/);
                    }
                }
            }
        }

        private void UpdateDisabledVisual()
        {
            if (this.IsEnabled)
            {
                VisualStates.GoToState(this, true, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, true, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
        }

        private void UpdateHorizontalScrollBar(bool needHorizScrollbar, bool forceHorizScrollbar, double totalVisibleWidth, double totalVisibleFrozenWidth, double cellsWidth)
        {
            if (this._hScrollBar != null)
            {
                if (needHorizScrollbar || forceHorizScrollbar)
                {
                    //          viewportSize
                    //        v---v
                    //|<|_____|###|>|
                    //  ^     ^
                    //  min   max

                    // we want to make the relative size of the thumb reflect the relative size of the viewing area
                    // viewportSize / (max + viewportSize) = cellsWidth / max
                    // -> viewportSize = max * cellsWidth / (max - cellsWidth)

                    // always zero
                    this._hScrollBar.Minimum = 0;
                    if (needHorizScrollbar)
                    {
                        // maximum travel distance -- not the total width
                        this._hScrollBar.Maximum = totalVisibleWidth - cellsWidth;
                        Debug.Assert(totalVisibleFrozenWidth >= 0);
                        if (this._frozenColumnScrollBarSpacer != null)
                        {
                            this._frozenColumnScrollBarSpacer.Width = totalVisibleFrozenWidth;
                        }
                        Debug.Assert(this._hScrollBar.Maximum >= 0);

                        // width of the scrollable viewing area
                        double viewPortSize = Math.Max(0, cellsWidth - totalVisibleFrozenWidth);
                        this._hScrollBar.ViewportSize = viewPortSize;
                        this._hScrollBar.LargeChange = viewPortSize;
                        // The ScrollBar should be in sync with HorizontalOffset at this point.  There's a resize case
                        // where the ScrollBar will coerce an old value here, but we don't want that
                        if (this._hScrollBar.Value != this._horizontalOffset)
                        {
                            this._hScrollBar.Value = this._horizontalOffset;
                        }
                    }
                    else
                    {
                        //
                        this._hScrollBar.Maximum = 0;
                        this._hScrollBar.ViewportSize = 0;
                    }

                    if (this._hScrollBar.Visibility != Visibility.Visible)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for
                        this._ignoreNextScrollBarsLayout = true;
                        // which no processing is needed.
                        this._hScrollBar.Visibility = Visibility.Visible;
                        if (this._hScrollBar.DesiredSize.Height == 0)
                        {
                            // We need to know the height for the rest of layout to work correctly so measure it now
                            this._hScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        }
                    }
                }
                else
                {
                    this._hScrollBar.Maximum = 0;
                    if (this._hScrollBar.Visibility != Visibility.Collapsed)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for 
                        // which no processing is needed.
                        this._hScrollBar.Visibility = Visibility.Collapsed;
                        this._ignoreNextScrollBarsLayout = true;
                    }
                }

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationScrollEvents();
                }
            }
        }

        private void UpdateRowDetailsVisibilityMode(DataGridRowDetailsVisibilityMode newDetailsMode)
        {
            int itemCount = this.DataConnection.Count;
            if (this._rowsPresenter != null && itemCount > 0)
            {
                Visibility newDetailsVisibility = Visibility.Collapsed;
                switch (newDetailsMode)
                {
                    case DataGridRowDetailsVisibilityMode.Visible:
                        newDetailsVisibility = Visibility.Visible;
                        this._showDetailsTable.AddValues(0, itemCount, Visibility.Visible);
                        break;
                    case DataGridRowDetailsVisibilityMode.Collapsed:
                        newDetailsVisibility = Visibility.Collapsed;
                        this._showDetailsTable.AddValues(0, itemCount, Visibility.Collapsed);
                        break;
                    case DataGridRowDetailsVisibilityMode.VisibleWhenSelected:
                        this._showDetailsTable.Clear();
                        break;
                }

                bool updated = false;
                foreach (DataGridRow row in this.GetAllRows())
                {
                    if (row.Visibility == Visibility.Visible)
                    {
                        if (newDetailsMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                        {
                            // For VisibleWhenSelected, we need to calculate the value for each individual row
                            newDetailsVisibility = this._selectedItems.ContainsSlot(row.Slot) ? Visibility.Visible : Visibility.Collapsed;
                        }
                        if (row.DetailsVisibility != newDetailsVisibility)
                        {
                            updated = true;
                            row.SetDetailsVisibilityInternal(newDetailsVisibility, true /* raiseNotification */, false /* animate */);
                        }
                    }
                }
                if (updated)
                {
                    UpdateDisplayedRows(this.DisplayData.FirstScrollingSlot, this.CellsHeight);
                    InvalidateRowsMeasure(false /*invalidateIndividualElements*/);
                }
            }
        }

        private void UpdateVerticalScrollBar(bool needVertScrollbar, bool forceVertScrollbar, double totalVisibleHeight, double cellsHeight)
        {
            if (this._vScrollBar != null)
            {
                if (needVertScrollbar || forceVertScrollbar)
                {
                    //          viewportSize
                    //        v---v
                    //|<|_____|###|>|
                    //  ^     ^
                    //  min   max

                    // we want to make the relative size of the thumb reflect the relative size of the viewing area
                    // viewportSize / (max + viewportSize) = cellsWidth / max
                    // -> viewportSize = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                    // ->              = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                    // ->              = max * cellsHeight / max
                    // ->              = cellsHeight

                    // always zero
                    this._vScrollBar.Minimum = 0;
                    if (needVertScrollbar)
                    {
                        // maximum travel distance -- not the total height
                        this._vScrollBar.Maximum = totalVisibleHeight - cellsHeight;
                        Debug.Assert(this._vScrollBar.Maximum >= 0);

                        // total height of the display area
                        this._vScrollBar.ViewportSize = cellsHeight;
                        this._vScrollBar.LargeChange = cellsHeight;
                    }
                    else
                    {
                        //
                        this._vScrollBar.Maximum = 0;
                        this._vScrollBar.ViewportSize = 0;
                    }

                    if (this._vScrollBar.Visibility != Visibility.Visible)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for 
                        // which no processing is needed.
                        this._vScrollBar.Visibility = Visibility.Visible;
                        if (this._vScrollBar.DesiredSize.Width == 0)
                        {
                            // We need to know the width for the rest of layout to work correctly so measure it now
                            this._vScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        }
                        this._ignoreNextScrollBarsLayout = true;
                    }
                }
                else
                {
                    this._vScrollBar.Maximum = 0;
                    if (this._vScrollBar.Visibility != Visibility.Collapsed)
                    {
                        // This will trigger a call to this method via Cells_SizeChanged for 
                        // which no processing is needed.
                        this._vScrollBar.Visibility = Visibility.Collapsed;
                        this._ignoreNextScrollBarsLayout = true;
                    }
                }

                DataGridAutomationPeer peer = DataGridAutomationPeer.FromElement(this) as DataGridAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationScrollEvents();
                }
            }
        }

        private bool ValidateEditingRow()
        {
            if (this.EditingRow != null)
            {
                List<ValidationResult> newValidationResults = new List<ValidationResult>();
                ValidationContext context = new ValidationContext(this.EditingRow.DataContext, null, null);

                if (!Validator.TryValidateObject(this.EditingRow.DataContext, context, newValidationResults, true))
                {
                    bool validationResultsChanged = false;

                    // Remove the validation results that have been fixed
                    List<ValidationResult> removedValidationResults = new List<ValidationResult>();
                    foreach (ValidationResult oldValidationResult in this._validationResults)
                    {
                        if (!newValidationResults.ContainsEqualValidationResult(oldValidationResult))
                        {
                            removedValidationResults.Add(oldValidationResult);
                            validationResultsChanged = true;
                        }
                    }
                    foreach (ValidationResult removedValidationResult in removedValidationResults)
                    {
                        this._validationResults.Remove(removedValidationResult);
                    }

                    // Add any validation results that were just introduced
                    foreach (ValidationResult newValidationResult in newValidationResults)
                    {
                        if (!this._validationResults.ContainsEqualValidationResult(newValidationResult))
                        {
                            this._validationResults.Add(newValidationResult);
                            validationResultsChanged = true;
                        }
                    }

                    // Show the error ListBox
                    if (validationResultsChanged)
                    {
                        int editingRowSlot = this.EditingRow.Slot;
                        if (this._errorsListBox != null)
                        {
                            if (this._errorsListBox.Visibility == Visibility.Collapsed)
                            {
                                this._errorsListBox.Visibility = Visibility.Visible;
                            }

                            InvalidateMeasure();
                            this.Dispatcher.BeginInvoke(delegate
                            {
                                ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                            });
                        }
                        else
                        {
                            ScrollSlotIntoView(editingRowSlot, false /*scrolledHorizontally*/);
                        }
                    }

                    // Update the validation visual states
                    foreach (DataGridColumn clickedErrorColumn in this._clickedErrorColumns)
                    {
                        this.EditingRow.Cells[clickedErrorColumn.Index].IsValid = false;
                        this.EditingRow.Cells[clickedErrorColumn.Index].ApplyCellState(true);
                    }
                    this.IsValid = this.EditingRow.IsValid = false;
                    this.EditingRow.ApplyState(true);
                    return false;
                }
            }

            // No validation errors, so reset the status
            ResetValidationStatus();
            return true;
        }

        private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ProcessVerticalScroll(e.ScrollEventType);
        }

        #endregion Private Methods
    }
}
