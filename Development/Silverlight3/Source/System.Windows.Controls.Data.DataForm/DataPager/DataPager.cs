//-----------------------------------------------------------------------
// <copyright file="DataPager.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Collections;

namespace System.Windows.Controls
{
    /// <summary>
    /// Handles paging for an IPagedCollectionView.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = "FirstPageButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PreviousPageButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "CurrentPageTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "TotalPagesTextBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "NextPageButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "LastPageButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "NumericButtonPanel", Type = typeof(Panel))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MoveEnabled", GroupName = "MoveStates")]
    [TemplateVisualState(Name = "MoveDisabled", GroupName = "MoveStates")]
    [TemplateVisualState(Name = "MoveFirstEnabled", GroupName = "MoveFirstStates")]
    [TemplateVisualState(Name = "MoveFirstDisabled", GroupName = "MoveFirstStates")]
    [TemplateVisualState(Name = "MovePreviousEnabled", GroupName = "MovePreviousStates")]
    [TemplateVisualState(Name = "MovePreviousDisabled", GroupName = "MovePreviousStates")]
    [TemplateVisualState(Name = "MoveNextEnabled", GroupName = "MoveNextStates")]
    [TemplateVisualState(Name = "MoveNextDisabled", GroupName = "MoveNextStates")]
    [TemplateVisualState(Name = "MoveLastEnabled", GroupName = "MoveLastStates")]
    [TemplateVisualState(Name = "MoveLastDisabled", GroupName = "MoveLastStates")]
    [TemplateVisualState(Name = "TotalPageCountKnown", GroupName = "TotalPageCountKnownStates")]
    [TemplateVisualState(Name = "TotalPageCountUnknown", GroupName = "TotalPageCountKnownStates")]
    [TemplateVisualState(Name = "FirstLastNumeric", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "FirstLastPreviousNext", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "FirstLastPreviousNextNumeric", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "Numeric", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "PreviousNext", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "PreviousNextNumeric", GroupName = "DisplayModeStates")]
    public class DataPager : Control
    {
        ////------------------------------------------------------
        ////
        ////  Static Fields and Constants
        ////
        ////------------------------------------------------------ 

        #region Static Fields

        /// <summary>
        /// Identifies the AutoEllipsis dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoEllipsisProperty =
            DependencyProperty.Register(
                "AutoEllipsis",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnAutoEllipsisPropertyChanged));

        /// <summary>
        /// Identifies the CanChangePage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanChangePageProperty =
            DependencyProperty.Register(
                "CanChangePage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CanMoveToFirstPage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToFirstPageProperty =
            DependencyProperty.Register(
                "CanMoveToFirstPage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CanMoveToLastPage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToLastPageProperty =
            DependencyProperty.Register(
                "CanMoveToLastPage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CanMoveToNextPage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToNextPageProperty =
            DependencyProperty.Register(
                "CanMoveToNextPage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CanMoveToPreviousPage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToPreviousPageProperty =
            DependencyProperty.Register(
                "CanMoveToPreviousPage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the DisplayMode dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
                "DisplayMode",
                typeof(PagerDisplayMode),
                typeof(DataPager),
                new PropertyMetadata(OnDisplayModePropertyChanged));

        /// <summary>
        /// Identifies the IsTotalItemCountFixed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTotalItemCountFixedProperty =
            DependencyProperty.Register(
                "IsTotalItemCountFixed",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnIsTotalItemCountFixedPropertyChanged));

        /// <summary>
        /// Identifies the ItemCount dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register(
                "ItemCount",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the NumericButtonCount dependency property.
        /// </summary>
        public static readonly DependencyProperty NumericButtonCountProperty =
            DependencyProperty.Register(
                "NumericButtonCount",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(OnNumericButtonCountPropertyChanged));

        /// <summary>
        /// Identifies the NumericButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty NumericButtonStyleProperty =
            DependencyProperty.Register(
                "NumericButtonStyle",
                typeof(Style),
                typeof(DataPager),
                new PropertyMetadata(OnNumericButtonStylePropertyChanged));

        /// <summary>
        /// Identifies the PageCount dependency property.
        /// </summary>
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register(
                "PageCount",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));
        
        /// <summary>
        /// Identifies the PageIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register(
                "PageIndex",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(-1, OnPageIndexPropertyChanged));

        /// <summary>
        /// Identifies the PageSize dependency property.
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(
                "PageSize",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(OnPageSizePropertyChanged));

        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                "Source",
                typeof(IEnumerable),
                typeof(DataPager),
                new PropertyMetadata(OnSourcePropertyChanged));

        #endregion Static Fields

        ////------------------------------------------------------
        ////
        ////  Private Fields
        ////
        ////------------------------------------------------------ 

        #region Private Fields

        /// <summary>
        /// Private accessor for the current page text box.
        /// </summary>
        private TextBox _currentPageTextBox;

        /// <summary>
        /// Private accessor for the first page ButtonBase.
        /// </summary>
        private ButtonBase _firstPageButtonBase;

        /// <summary>
        /// Private accessor for the last page ButtonBase.
        /// </summary>
        private ButtonBase _lastPageButtonBase;

        /// <summary>
        /// Private accessor for the next page ButtonBase.
        /// </summary>
        private ButtonBase _nextPageButtonBase;

        /// <summary>
        /// Private accessor for the panel hosting the buttons.
        /// </summary>
        private Panel _numericButtonPanel;

        /// <summary>
        /// Private accessor for the previous page ButtonBase.
        /// </summary>
        private ButtonBase _previousPageButtonBase;

        /// <summary>
        /// The new index of the current page, used to change the
        /// current page when a user enters something into the
        /// current page text box.
        /// </summary>
        private int _requestedPageIndex;

        /// <summary>
        /// Private accessor for the selected button.
        /// </summary>
        private ToggleButton _selectedButton;

        /// <summary>
        /// Private accessor for the total pages text block.
        /// </summary>
        private TextBlock _totalPagesTextBlock;

        #endregion Private Fields

        ////------------------------------------------------------
        ////
        ////  Constructors
        ////
        ////------------------------------------------------------ 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DataPager class.
        /// </summary>
        public DataPager()
        {
            this.DefaultStyleKey = typeof(DataPager);
        }

        #endregion Constructors

        ////------------------------------------------------------
        ////
        ////  Events
        ////
        ////------------------------------------------------------ 

        #region Events

        /// <summary>
        /// EventHandler for when PageIndex is changing.
        /// </summary>
        public event EventHandler<CancelEventArgs> PageIndexChanging;

        /// <summary>
        /// EventHandler for when PageIndex has changed.
        /// </summary>
        public event EventHandler<EventArgs> PageIndexChanged;

        #endregion Events

        ////------------------------------------------------------
        ////
        ////  Public Properties
        ////
        ////------------------------------------------------------ 

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether or not to use an ellipsis as the last button.
        /// </summary>
        public bool AutoEllipsis
        {
            get
            {
                return (bool)GetValue(AutoEllipsisProperty);
            }
            set
            {
                SetValue(AutoEllipsisProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the user is allowed to move to another page
        /// </summary>
        public bool CanChangePage
        {
            get
            {
                return (bool)GetValue(CanChangePageProperty);
            }
            private set
            {
                this.SetValueNoCallback(CanChangePageProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the DataPager will allow the user to attempt to move to the first page if CanChangePage is true.
        /// </summary>
        public bool CanMoveToFirstPage
        {
            get
            {
                return (bool)GetValue(CanMoveToFirstPageProperty);
            }
            private set
            {
                this.SetValueNoCallback(CanMoveToFirstPageProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the DataPager will allow the user to attempt to move to the last page if CanChangePage is true.
        /// </summary>
        public bool CanMoveToLastPage
        {
            get
            {
                return (bool)GetValue(CanMoveToLastPageProperty);
            }
            private set
            {
                this.SetValueNoCallback(CanMoveToLastPageProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the DataPager will allow the user to attempt to move to the next page if CanChangePage is true.
        /// </summary>
        public bool CanMoveToNextPage
        {
            get
            {
                return (bool)GetValue(CanMoveToNextPageProperty);
            }
            private set
            {
                this.SetValueNoCallback(CanMoveToNextPageProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the DataPager will allow the user to attempt to move to the previous page if CanChangePage is true.
        /// </summary>
        public bool CanMoveToPreviousPage
        {
            get
            {
                return (bool)GetValue(CanMoveToPreviousPageProperty);
            }
            private set
            {
                this.SetValueNoCallback(CanMoveToPreviousPageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DisplayMode, which determines the UI that is displayed
        /// </summary>
        public PagerDisplayMode DisplayMode
        {
            get
            {
                return (PagerDisplayMode)GetValue(DisplayModeProperty);
            }
            set
            {
                SetValue(DisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the total number of items in the collection is fixed.
        /// </summary>
        public bool IsTotalItemCountFixed
        {
            get
            {
                return (bool)GetValue(IsTotalItemCountFixedProperty);
            }
            set
            {
                SetValue(IsTotalItemCountFixedProperty, value);
            }
        }

        /// <summary>
        /// Gets the current number of known items in the IPagedCollectionView.
        /// </summary>
        public int ItemCount
        {
            get
            {
                return (int)GetValue(ItemCountProperty);
            }
            private set
            {
                this.SetValueNoCallback(ItemCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the IPagedCollectionView.
        /// </summary>
        public IPagedCollectionView Source
        {
            get
            {
                return GetValue(SourceProperty) as IPagedCollectionView;
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the NumericButtonCount, which determines the number of page
        /// buttons shown on the DataPager
        /// </summary>
        public int NumericButtonCount
        {
            get
            {
                return (int)GetValue(NumericButtonCountProperty);
            }
            set
            {
                SetValue(NumericButtonCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style that will be used for the Numeric Buttons.
        /// </summary>
        public Style NumericButtonStyle
        {
            get
            {
                return (Style)GetValue(NumericButtonStyleProperty);
            }
            set
            {
                SetValue(NumericButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets the current number of known pages in the IPagedCollectionView.
        /// </summary>
        public int PageCount
        {
            get
            {
                return (int)GetValue(PageCountProperty);
            }
            private set
            {
                this.SetValueNoCallback(PageCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current PageIndex in the IPagedCollectionView.
        /// </summary>
        public int PageIndex
        {
            get
            {
                return (int)GetValue(PageIndexProperty);
            }
            set
            {
                SetValue(PageIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current PageSize in the IPagedCollectionView.
        /// </summary>
        public int PageSize
        {
            get
            {
                return (int)GetValue(PageSizeProperty);
            }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

        #endregion Public Properties

        ////------------------------------------------------------
        ////
        ////  Public Methods
        ////
        ////------------------------------------------------------ 

        #region Public Methods

        /// <summary>
        /// Applies the control's template, retrieves the elements
        /// within it, and sets up events.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // unsubscribe event handlers for previous template parts
            if (this._firstPageButtonBase != null)
            {
                this._firstPageButtonBase.Click -= new RoutedEventHandler(this.OnFirstPageButtonBaseClick);
            }

            if (this._previousPageButtonBase != null)
            {
                this._previousPageButtonBase.Click -= new RoutedEventHandler(this.OnPreviousPageButtonBaseClick);
            }

            if (this._nextPageButtonBase != null)
            {
                this._nextPageButtonBase.Click -= new RoutedEventHandler(this.OnNextPageButtonBaseClick);
            }

            if (this._lastPageButtonBase != null)
            {
                this._lastPageButtonBase.Click -= new RoutedEventHandler(this.OnLastPageButtonBaseClick);
            }

            if (this._currentPageTextBox != null)
            {
                this._currentPageTextBox.KeyDown -= new System.Windows.Input.KeyEventHandler(this.OnCurrentPageTextBoxKeyDown);
                this._currentPageTextBox.LostFocus -= new RoutedEventHandler(this.OnCurrentPageTextBoxLostFocus);
            }

            // get new template parts
            this._firstPageButtonBase = GetTemplateChild("FirstPageButton") as ButtonBase;
            this._previousPageButtonBase = GetTemplateChild("PreviousPageButton") as ButtonBase;
            this._nextPageButtonBase = GetTemplateChild("NextPageButton") as ButtonBase;
            this._lastPageButtonBase = GetTemplateChild("LastPageButton") as ButtonBase;

            if (this._firstPageButtonBase != null)
            {
                this._firstPageButtonBase.Click += new RoutedEventHandler(this.OnFirstPageButtonBaseClick);
            }

            if (this._previousPageButtonBase != null)
            {
                this._previousPageButtonBase.Click += new RoutedEventHandler(this.OnPreviousPageButtonBaseClick);
            }

            if (this._nextPageButtonBase != null)
            {
                this._nextPageButtonBase.Click += new RoutedEventHandler(this.OnNextPageButtonBaseClick);
            }

            if (this._lastPageButtonBase != null)
            {
                this._lastPageButtonBase.Click += new RoutedEventHandler(this.OnLastPageButtonBaseClick);
            }

            // remove previous panel + buttons.
            if (this._numericButtonPanel != null)
            {
                this._numericButtonPanel.Children.Clear();
            }

            this._numericButtonPanel = GetTemplateChild("NumericButtonPanel") as Panel;

            // add new buttons to panel
            if (this._numericButtonPanel != null)
            {
                if (this._numericButtonPanel.Children.Count > 0)
                {
                    throw new InvalidOperationException(PagerResources.InvalidButtonPanelContent);
                }

                this.UpdateButtonCount();
            }

            this._currentPageTextBox = GetTemplateChild("CurrentPageTextBox") as TextBox;
            this._totalPagesTextBlock = GetTemplateChild("TotalPagesTextBlock") as TextBlock;

            if (this._currentPageTextBox != null)
            {
                this._currentPageTextBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.OnCurrentPageTextBoxKeyDown);
                this._currentPageTextBox.LostFocus += new RoutedEventHandler(this.OnCurrentPageTextBoxLostFocus);
            }

            this.UpdateControl();
        }

        #endregion Public Methods

        ////------------------------------------------------------
        ////
        ////  Private Static Methods
        ////
        ////------------------------------------------------------ 

        #region Private Static Methods

        /// <summary>
        /// AutoEllipsis property changed handler.
        /// </summary>
        /// <param name="d">NumericButton that changed its AutoEllipsis.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnAutoEllipsisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            pager.UpdateButtonDisplay();
        }

        /// <summary>
        /// DisplayMode property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its DisplayMode.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            pager.UpdateControl();
        }

        /// <summary>
        /// IsTotalItemCountFixed property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed IsTotalItemCountFixed.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsTotalItemCountFixedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            pager.UpdateControl();
        }

        /// <summary>
        /// NumericButtonCount property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its NumericButtonCount.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnNumericButtonCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            if (!pager.AreHandlersSuspended())
            {
                if ((int)e.NewValue <= 0)
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            CultureInfo.InvariantCulture,
                            PagerResources.ValueMustBeGreaterThanOrEqualTo,
                            "NumericButtonCount",
                            1));
                }

                pager.UpdateButtonCount();
            }
        }

        /// <summary>
        /// NumericButtonStyle property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its NumericButtonStyle.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnNumericButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            if (pager._numericButtonPanel != null)
            {
                // update button styles
                foreach (UIElement uiElement in pager._numericButtonPanel.Children)
                {
                    ToggleButton button = uiElement as ToggleButton;

                    if (button != null)
                    {
                        button.Style = pager.NumericButtonStyle;
                    }
                }
            }
        }
        
        /// <summary>
        /// PageIndex property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its PageIndex.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnPageIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            if (!pager.AreHandlersSuspended())
            {
                int newPageIndex = (int)e.NewValue;

                if ((pager.Source == null || pager.PageSize == 0) && newPageIndex != -1)
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentOutOfRangeException(
                        "value",
                        PagerResources.PageIndexMustBeNegativeOne);
                }

                if (pager.Source != null && pager.PageSize != 0 && newPageIndex < 0)
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            CultureInfo.InvariantCulture,
                            PagerResources.ValueMustBeGreaterThanOrEqualTo,
                            "PageIndex",
                            0));
                }

                if (pager.Source != null)
                {
                    int oldPageIndex = pager.Source.PageIndex;
                    if (newPageIndex != oldPageIndex)
                    {
                        CancelEventArgs cancelArgs = new CancelEventArgs(false);
                        pager.RaisePageIndexChanging(cancelArgs);

                        if (!cancelArgs.Cancel && pager.Source.MoveToPage(newPageIndex))
                        {
                            if (oldPageIndex != pager.Source.PageIndex)
                            {
                                pager.RaisePageIndexChanged();
                            }
                        }
                        else
                        {
                            // revert back to old value, since operation was canceled or failed
                            pager.SetValueNoCallback(e.Property, e.OldValue);
                        }
                    }
                }
                else
                {
                    // keep value set to -1 if there is no source
                    pager.SetValueNoCallback(e.Property, -1);
                }
            }
        }

        /// <summary>
        /// PageSize property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its PageSize.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnPageSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            if (!pager.AreHandlersSuspended())
            {
                int newPageSize = (int)e.NewValue;

                if (newPageSize < 0)
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            CultureInfo.InvariantCulture,
                            PagerResources.ValueMustBeGreaterThanOrEqualTo,
                            "PageSize",
                            0));
                }

                if (pager.Source != null)
                {
                    try
                    {
                        pager.Source.PageSize = newPageSize;
                    }
                    catch
                    {
                        pager.SetValueNoCallback(e.Property, e.OldValue);
                        throw;
                    }
                }

                pager.UpdateControl();
            }
        }

        /// <summary>
        /// Called when a Read-Only dependency property is changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            if (pager != null && !pager.AreHandlersSuspended())
            {
                pager.SetValueNoCallback(e.Property, e.OldValue);
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        PagerResources.UnderlyingPropertyIsReadOnly,
                        e.Property.ToString()));
            }
        }

        /// <summary>
        /// SourceProperty property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its Source.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            INotifyPropertyChanged oldNotifyPropertyChanged = e.OldValue as INotifyPropertyChanged;
            if (oldNotifyPropertyChanged != null)
            {
                oldNotifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(pager.OnSourcePropertyChanged);
            }

            INotifyPropertyChanged newNotifyPropertyChanged = e.NewValue as INotifyPropertyChanged;
            if (newNotifyPropertyChanged != null)
            {
                newNotifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(pager.OnSourcePropertyChanged);
            }

            IPagedCollectionView newPagedCollectionView = e.NewValue as IPagedCollectionView;
            if (newPagedCollectionView != null)
            {
                if (pager.PageSize != 0)
                {
                    newPagedCollectionView.PageSize = pager.PageSize;
                }
                else
                {
                    pager.PageSize = newPagedCollectionView.PageSize;
                }
                pager.ItemCount = newPagedCollectionView.ItemCount;
                pager.UpdatePageCount();
            }
            else
            {
                pager.ItemCount = 0;
                pager.PageCount = 0;
            }

            pager.UpdateControl();
        }

        #endregion Private Static Methods

        ////------------------------------------------------------
        ////
        ////  Private Methods
        ////
        ////------------------------------------------------------ 

        #region Private Methods

        /// <summary>
        /// Gets the starting index that our buttons should be labeled with.
        /// </summary>
        /// <returns>Starting index for our buttons</returns>
        private int GetButtonStartIndex()
        {
            // Because we have a starting PageIndex, we want to try and center the current pages
            // around this value. But if we are at the end of the collection, we display the last 
            // available buttons.
            return Math.Min(
                Math.Max((this.PageIndex + 1) - (this.NumericButtonCount / 2), 1), /* center buttons around pageIndex */
                Math.Max(this.PageCount - this.NumericButtonCount + 1, 1));        /* lastPage - number of buttons */
        }

        /// <summary>
        /// Attempts to move the current page index to the value
        /// in the current page textbox.
        /// </summary>
        private void MoveCurrentPageToTextboxValue()
        {
            if (this._currentPageTextBox.Text != (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture))
            {
                if (this.Source != null && this.TryParseTextBoxPage())
                {
                    this.MoveToRequestedPage();
                }
                this._currentPageTextBox.Text = (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Given the new value of _requestedPageIndex, this method will attempt a page move 
        /// and set the _currentPageIndex variable accordingly.
        /// </summary>
        private void MoveToRequestedPage()
        {
            if (this._requestedPageIndex >= 0 && this._requestedPageIndex < this.PageCount)
            {
                // Requested page is within the known range
                this.PageIndex = this._requestedPageIndex;
            }
            else if (this._requestedPageIndex >= this.PageCount)
            {
                if (this.IsTotalItemCountFixed && this.Source.TotalItemCount != -1)
                {
                    this.PageIndex = this.PageCount - 1;
                }
                else
                {
                    this.PageIndex = this._requestedPageIndex;
                }
            }
        }

        /// <summary>
        /// Handler for when the numeric buttons in the pager are clicked
        /// </summary>
        /// <param name="sender">Button that fired the event</param>
        /// <param name="e">EventArgs for the event</param>
        private void OnButtonClick(object sender, EventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            int pageNum = -1;

            Debug.Assert(button != null, "Sender should not be null");

            button.IsChecked = false;

            int index = this._numericButtonPanel.Children.IndexOf(button);

            // the '-1' is to convert to a '0' based index.
            pageNum = this.GetButtonStartIndex() + index - 1;

            if (this.Source != null)
            {
                this.Source.MoveToPage(pageNum);
            }

            // check to see if this button is still the selected one
            // as the start index may have shifted
            if ((index + this.GetButtonStartIndex() - 1) == pageNum)
            {
                button.IsChecked = true;
            }
        }

        /// <summary>
        /// Handles the KeyDown event on the current page text box.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnCurrentPageTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.MoveCurrentPageToTextboxValue();
            }
        }

        /// <summary>
        /// Handles the loss of focus for the current page text box.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnCurrentPageTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.MoveCurrentPageToTextboxValue();
        }

        /// <summary>
        /// Handles the click of the first page ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnFirstPageButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (this.Source != null)
            {
                this.Source.MoveToFirstPage();
            }
        }

        /// <summary>
        /// Handles the click of the last page ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnLastPageButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (this.Source != null)
            {
                this.Source.MoveToLastPage();
            }
        }

        /// <summary>
        /// Handles the click of the next page ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnNextPageButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (this.Source != null)
            {
                this.Source.MoveToNextPage();
            }
        }

        /// <summary>
        /// Handles the click of the previous page ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnPreviousPageButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (this.Source != null)
            {
                this.Source.MoveToPreviousPage();
            }
        }

        /// <summary>
        /// Handles a property change within the IPagedCollectionView.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Count":
                case "ItemCount":
                    this.ItemCount = this.Source.ItemCount;
                    this.UpdatePageCount();
                    this.UpdateControl();
                    break;

                case "PageIndex":
                    this.PageIndex = this.Source.PageIndex;
                    this.RaisePageIndexChanged();
                    break;

                case "PageSize":
                    this.PageSize = this.Source.PageSize;
                    this.UpdatePageCount();
                    this.UpdateControl();
                    break;

                case "CanChangePage":
                case "Filter":
                case "TotalItemCount":
                case "SortDescriptions":
                    this.UpdateControl();
                    break;
            }
        }

        /// <summary>
        /// Raises the PageIndexChanged event.
        /// </summary>
        private void RaisePageIndexChanged()
        {
            this.UpdateControl();

            EventHandler<EventArgs> handler = this.PageIndexChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the PageIndexChanging event.
        /// </summary>
        /// <param name="e">The event args to use for the event.</param>
        private void RaisePageIndexChanging(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.PageIndexChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Update DataPager UI for paging enabled.
        /// </summary>
        /// <param name="needPage">Boolean that specifies if a page is needed</param>
        private void SetCannotChangePage(bool needPage)
        {
            if (this._currentPageTextBox != null && !needPage)
            {
                this._currentPageTextBox.Text = String.Empty;
            }

            if (this._totalPagesTextBlock != null && !needPage)
            {
                this._totalPagesTextBlock.Text = String.Empty;
            }

            VisualStateManager.GoToState(this, "MoveDisabled", true);
            VisualStateManager.GoToState(this, "MoveFirstDisabled", true);
            VisualStateManager.GoToState(this, "MovePreviousDisabled", true);
            VisualStateManager.GoToState(this, "MoveNextDisabled", true);
            VisualStateManager.GoToState(this, "MoveLastDisabled", true);
            VisualStateManager.GoToState(this, "TotalPageCountUnknown", true);
        }

        /// <summary>
        /// Update DataPager UI for paging disabled.
        /// </summary>
        private void SetCanChangePage()
        {
            VisualStateManager.GoToState(this, "MoveEnabled", true);

            if (this._currentPageTextBox != null)
            {
                this._currentPageTextBox.Text = (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture);
            }

            if (this.Source.TotalItemCount != -1)
            {
                if (this._totalPagesTextBlock != null)
                {
                    this._totalPagesTextBlock.Text = this.PageCount.ToString(CultureInfo.CurrentCulture);
                }
                VisualStateManager.GoToState(this, "TotalPageCountKnown", true);
            }
            else
            {
                if (this._totalPagesTextBlock != null)
                {
                    this._totalPagesTextBlock.Text = String.Empty;
                }
                VisualStateManager.GoToState(this, "TotalPageCountUnknown", true);
            }
        }

        /// <summary>
        /// Attempts to put the integer value of the string in _currentPageTextBox into _requestedPageIndex.
        /// </summary>
        /// <returns>Whether or not the parsing of the string succeeded.</returns>
        private bool TryParseTextBoxPage()
        {
            bool successfullyParsed = int.TryParse(this._currentPageTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out this._requestedPageIndex);

            if (successfullyParsed)
            {
                // Subtract one to make it zero-based.
                this._requestedPageIndex--;
            }

            return successfullyParsed;
        }

        /// <summary>
        /// Updates the visual display of the number of buttons that we display.
        /// </summary>
        private void UpdateButtonCount()
        {
            // what we should use as the button count
            int buttonCount = Math.Min(this.NumericButtonCount, this.PageCount);

            if (this._numericButtonPanel != null)
            {
                // add new
                while (this._numericButtonPanel.Children.Count < buttonCount)
                {
                    ToggleButton button = new ToggleButton();
                    button.Style = this.NumericButtonStyle;
                    button.Click += new RoutedEventHandler(this.OnButtonClick);
                    this._numericButtonPanel.Children.Add(button);
                }

                // remove excess
                while (this._numericButtonPanel.Children.Count > buttonCount)
                {
                    ToggleButton button = this._numericButtonPanel.Children[0] as ToggleButton;
                    if (button != null)
                    {
                        button.Click -= new RoutedEventHandler(this.OnButtonClick);
                        if (button == this._selectedButton)
                        {
                            this._selectedButton = null;
                        }

                        this._numericButtonPanel.Children.Remove(button);
                    }
                }

                this.UpdateButtonDisplay();
            }
        }

        /// <summary>
        /// Updates the visual content and style of the buttons that we display.
        /// </summary>
        private void UpdateButtonDisplay()
        {
            // If we are not hooked up to an IPagedCollectionView, then many of the
            // properties will return -1. Our buttons start with a 1 index, so we call
            // Math.Max(var, 1), to ensure our button content will show '1' at the minimum

            // what we should use as the start index
            int startIndex = this.GetButtonStartIndex();

            // what we should use as the button count
            int buttonCount = Math.Min(this.NumericButtonCount, this.PageCount);

            // what we should use as the selectedIndex index
            int selectedIndex = Math.Max(this.PageIndex + 1, 1);

            if (this._numericButtonPanel != null)
            {
                int index = startIndex;
                foreach (UIElement ui in this._numericButtonPanel.Children)
                {
                    ToggleButton button = ui as ToggleButton;
                    if (button != null)
                    {
                        if (index == selectedIndex)
                        {
                            // clear selected visual from previous selection
                            if (this._selectedButton != null)
                            {
                                this._selectedButton.IsChecked = false;
                            }

                            this._selectedButton = button;
                            this._selectedButton.IsChecked = true;
                        }

                        if (this.AutoEllipsis && index == startIndex + buttonCount - 1 &&
                            (index != this.PageCount))
                        {
                            button.Content = PagerResources.AutoEllipsisString;
                        }
                        else
                        {
                            button.Content = index;
                        }
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the current page, the total pages, and the
        /// state of the control.
        /// </summary>
        private void UpdateControl()
        {
            this.UpdatePageModeDisplay();
            this.UpdateButtonCount();

            bool needPage = this.Source != null && this.Source.PageSize > 0;

            this.CanMoveToFirstPage = needPage && this.PageIndex > 0;

            this.CanMoveToPreviousPage = this.CanMoveToFirstPage;

            this.CanMoveToNextPage = needPage &&
                (!this.IsTotalItemCountFixed || this.Source.TotalItemCount == -1 || this.PageIndex < this.PageCount - 1);

            this.CanMoveToLastPage = needPage && this.Source.TotalItemCount != -1 && this.PageIndex < this.PageCount - 1;

            this.CanChangePage = needPage && this.Source.CanChangePage;

            if (!needPage || !this.CanChangePage)
            {
                this.SetCannotChangePage(needPage);
                return;
            }

            this.SetCanChangePage();
            this.UpdateCanPageFirstAndPrevious();
            this.UpdateCanPageNextAndLast();
        }

        /// <summary>
        /// Updates the states of whether the pager can page to the first
        /// and to the previous page.
        /// </summary>
        private void UpdateCanPageFirstAndPrevious()
        {
            VisualStateManager.GoToState(this, this.CanMoveToFirstPage ? "MoveFirstEnabled" : "MoveFirstDisabled", true);
            VisualStateManager.GoToState(this, this.CanMoveToPreviousPage ? "MovePreviousEnabled" : "MovePreviousDisabled", true);
        }

        /// <summary>
        /// Updates the states of whether the pager can page to the next
        /// and to the last page.
        /// </summary>
        private void UpdateCanPageNextAndLast()
        {
            VisualStateManager.GoToState(this, this.CanMoveToNextPage ? "MoveNextEnabled" : "MoveNextDisabled", true);
            VisualStateManager.GoToState(this, this.CanMoveToLastPage ? "MoveLastEnabled" : "MoveLastDisabled", true);
        }

        /// <summary>
        /// Updates the visual display to show the current page mode
        /// we have selected.
        /// </summary>
        private void UpdatePageModeDisplay()
        {
            VisualStateManager.GoToState(this, Enum.GetName(typeof(PagerDisplayMode), this.DisplayMode), true);
        }

        /// <summary>
        /// Updates the page count based on the number of items and the page size.
        /// </summary>
        private void UpdatePageCount()
        {
            if (this.Source.PageSize > 0)
            {
                this.PageCount = Math.Max(1, (int)Math.Ceiling((double)this.Source.ItemCount / this.Source.PageSize));
            }
            else
            {
                this.PageCount = 1;
            }
        }

        #endregion Private Methods
    }
}
