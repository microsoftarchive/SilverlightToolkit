//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls.Common;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Specialized;
using resources = System.Windows.Controls.Data.DataForm.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// The ErrorSummary will display validation errors for the given ErrorsSource.  This can  show the entity level 
    /// errors or the aggregate of all errors from each input control or both.  There is an Errors collection that 
    /// represents all errors added to the ErrorSummary and the FilteredErrors list that contains all the errors
    /// matching the display filter.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = PART_ErrorsListBox, Type = typeof(ListBox))]
    [TemplateVisualState(Name = VSMSTATE_Normal, GroupName = VSMGROUP_CommonStates)]
    [TemplateVisualState(Name = VSMSTATE_Disabled, GroupName = VSMGROUP_CommonStates)]
    [TemplateVisualState(Name = VSMSTATE_Empty, GroupName = VSMGROUP_ValidationStates)]
    [TemplateVisualState(Name = VSMSTATE_HasErrors, GroupName = VSMGROUP_ValidationStates)]
    public class ErrorSummary : Control
    {
        #region Static Fields and Constants

        private const string PART_ErrorsListBox = "ErrorsListBox";
        private const string PART_HeaderContentControl = "HeaderContentControl";
        private const string VSMGROUP_CommonStates = "CommonStates";
        private const string VSMGROUP_ValidationStates = "ValidationStates";
        private const string VSMSTATE_Normal = "Normal";
        private const string VSMSTATE_Disabled = "Disabled";
        private const string VSMSTATE_Empty = "Empty";
        private const string VSMSTATE_HasErrors = "HasErrors";

        #endregion Static Fields and Constants

        #region Member Fields

        private ValidationErrorCollection _errors;
        private ListBox _errorsListBox;
        private ValidationErrorCollection _filteredErrors;
        private ContentControl _headerContentControl;
        private bool _initialized;
        private FrameworkElement _registeredParent;

        #endregion Member Fields

        #region Events

        /// <summary>
        /// Event triggered when an Error is clicked on.
        /// </summary>
        public event EventHandler<ErrorSummaryItemEventArgs> ErrorClicked;

        #endregion Events

        #region	Constructors

        /// <summary>
        /// Initializes a new instance of the ErrorSummary class.
        /// </summary>
        public ErrorSummary()
        {
            this.DefaultStyleKey = typeof(ErrorSummary);
            this._errors = new ValidationErrorCollection();
            this._filteredErrors = new ValidationErrorCollection();
            this._errors.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.Errors_CollectionChanged);
            this.Loaded += new RoutedEventHandler(this.ErrorSummary_Loaded);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.ErrorSummary_IsEnabledChanged);
        }

        #endregion Constructors

        #region Attached Properties

        #region ShowErrorsInSummary

        /// <summary>
        /// Gets or sets a value indicating whether the field errors belonging to the input control should be shown in the ErrorSummary. 
        /// Errors are added to the FilteredErrors list depending on this flag.  The base Errors list, however, will always contain all
        /// the errors.
        /// </summary>
        public static readonly DependencyProperty ShowErrorsInSummaryProperty = DependencyProperty.RegisterAttached(
            "ShowErrorsInSummary",
            typeof(bool),
            typeof(ErrorSummary),
            new PropertyMetadata(true, OnShowErrorsInSummaryPropertyChanged));

        /// <summary>
        /// Gets the ShowErrorsInSummary property of the specified DependencyObject.
        /// </summary>
        /// <param name="inputControl">The input control to get the ShowErrorsInSummary property from.</param>
        /// <returns>The value indicating whether errors on the input control should be shown.</returns>
        public static bool GetShowErrorsInSummary(DependencyObject inputControl)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            return (bool)inputControl.GetValue(ShowErrorsInSummaryProperty);
        }

        /// <summary>
        /// Sets the ShowErrorsInSummary property of the specified DependencyObject.
        /// </summary>
        /// <param name="inputControl">The input control with which to associate the specified dependency property.</param>
        /// <param name="value">The value indicating whether errors on the input control should be shown.</param>
        public static void SetShowErrorsInSummary(DependencyObject inputControl, bool value)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            inputControl.SetValue(ShowErrorsInSummaryProperty, value);
        }

        private static void OnShowErrorsInSummaryPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement page = (Application.Current != null) ? Application.Current.RootVisual as FrameworkElement : null;
            if (page != null)
            {
                UpdateFilteredErrorsOnAllErrorSummaries(page);
            }
        }

        #endregion ShowErrorsInSummary

        #endregion Attached Properties

        #region Dependency Properties

        #region ErrorsListBoxStyle

        /// <summary>
        /// Identifies the ErrorsPanel dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorsListBoxStyleProperty =
            DependencyProperty.Register(
            "ErrorsListBoxStyle",
            typeof(Style),
            typeof(ErrorSummary),
            null);

        /// <summary>
        /// Gets or sets the ItemsPanel for the Errors ListBox.
        /// </summary>
        public Style ErrorsListBoxStyle
        {
            get { return GetValue(ErrorsListBoxStyleProperty) as Style; }
            set { SetValue(ErrorsListBoxStyleProperty, value); }
        }

        #endregion ErrorsListBoxStyle

        #region ErrorsSource

        /// <summary>
        /// Identifies the ErrorsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorsSourceProperty =
            DependencyProperty.Register(
            "ErrorsSource",
            typeof(UIElement),
            typeof(ErrorSummary),
            new PropertyMetadata(OnErrorsSourcePropertyChanged));

        /// <summary>
        /// Gets or sets the source of errors for the ErrorSummary.
        /// </summary>
        public UIElement ErrorsSource
        {
            get { return GetValue(ErrorsSourceProperty) as UIElement; }
            set { SetValue(ErrorsSourceProperty, value); }
        }

        private static void OnErrorsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement oldElement = e.OldValue as FrameworkElement;
            ErrorSummary es = d as ErrorSummary;
            EventHandler<ValidationErrorEventArgs> handler = new EventHandler<ValidationErrorEventArgs>(es.ErrorsSource_BindingValidationError);
            if (es._registeredParent != null)
            {
                es._registeredParent.BindingValidationError -= handler;
                es._registeredParent = null;
            }
            if (oldElement != null)
            {
                oldElement.BindingValidationError -= handler;
            }
            FrameworkElement newElement = e.NewValue as FrameworkElement;
            if (newElement != null)
            {
                newElement.BindingValidationError += handler;
            }

            // Clear the old property binding errors
            es._errors.ClearErrors(ErrorType.PropertyError);
            es.UpdateFilteredErrors();
        }

        #endregion ErrorsSource

        #region ErrorStyle

        /// <summary>
        /// Identifies the ErrorStyle dependency property
        /// </summary>
        public static readonly DependencyProperty ErrorStyleProperty =
            DependencyProperty.Register(
            "ErrorStyle",
            typeof(Style),
            typeof(ErrorSummary),
            null);

        /// <summary>
        /// Gets or sets the style for the Error item container.
        /// </summary>
        public Style ErrorStyle
        {
            get { return GetValue(ErrorStyleProperty) as Style; }
            set { SetValue(ErrorStyleProperty, value); }
        }

        #endregion ErrorStyle

        #region Filter

        /// <summary>
        /// Identifies the Filter dependency property
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(
            "Filter",
            typeof(ErrorSummaryFilters),
            typeof(ErrorSummary),
            new PropertyMetadata(ErrorSummaryFilters.All, OnFilterPropertyChanged));

        /// <summary>
        /// Gets or sets the Filter property to indicate which types of errors should be displayed.
        /// </summary>
        public ErrorSummaryFilters Filter
        {
            get { return (ErrorSummaryFilters)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        private static void OnFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ErrorSummary es = d as ErrorSummary;
            if (es != null)
            {
                es.UpdateFilteredErrors();
            }
        }

        #endregion Filter

        #region FocusControlsOnClick

        /// <summary>
        /// Identifies the FocusControlsOnClick dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusControlsOnClickProperty =
            DependencyProperty.Register(
            "FocusControlsOnClick",
            typeof(bool),
            typeof(ErrorSummary),
            new PropertyMetadata(true, null));

        /// <summary>
        /// Gets or sets a value indicating whether focus should be set on the input control
        /// when clicked on it
        /// </summary>
        public bool FocusControlsOnClick
        {
            get { return (bool)GetValue(FocusControlsOnClickProperty); }
            set { SetValue(FocusControlsOnClickProperty, value); }
        }

        #endregion FocusControlsOnClick

        #region HasErrors

        /// <summary>
        /// Identifies the HasErrors dependency property
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register(
            "HasErrors",
            typeof(bool),
            typeof(ErrorSummary),
            new PropertyMetadata(false, OnHasErrorsPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the ErrorSummary has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            internal set { this.SetValueNoCallback(HasErrorsProperty, value); }
        }

        private static void OnHasErrorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ErrorSummary es = d as ErrorSummary;
            if (es != null && !es.AreHandlersSuspended())
            {
                es.SetValueNoCallback(ErrorSummary.HasErrorsProperty, e.OldValue);
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, resources.UnderlyingPropertyIsReadOnly, "HasErrors"));
            }
        }

        #endregion HasErrors

        #region Header

        /// <summary>
        /// Identifies the Header dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(ErrorSummary),
            new PropertyMetadata(OnHasHeaderPropertyChanged));

        /// <summary>
        /// Gets or sets the Header
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static void OnHasHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ErrorSummary es = d as ErrorSummary;
            if (es != null)
            {
                es.UpdateHeaderText();
            }
        }

        #endregion Header

        #region HeaderTemplate

        /// <summary>
        /// Identifies the HeaderTemplate dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(ErrorSummary),
            null);

        /// <summary>
        /// Gets or sets the Header DataTemplate
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty) as DataTemplate; }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        #endregion HeaderTemplate

        #endregion Dependency Properties

        #region Properties

        /// <summary>
        /// Gets the collection of errors
        /// </summary>
        public ObservableCollection<ErrorSummaryItem> Errors
        {
            get { return this._errors; }
        }

        /// <summary>
        /// Gets the filtered list of errors that are shown.
        /// </summary>
        public ReadOnlyObservableCollection<ErrorSummaryItem> FilteredErrors
        {
            get { return new ReadOnlyObservableCollection<ErrorSummaryItem>(this._filteredErrors); }
        }

        /// <summary>
        /// Gets a value indicating whether the ErrorSummary is initialized.
        /// </summary>
        internal bool Initialized
        {
            get { return this._initialized; }
        }

        /// <summary>
        /// Gets the ErrorsListBox template part
        /// </summary>
        internal ListBox ErrorsListBoxInternal
        {
            get { return this._errorsListBox; }
        }

        /// <summary>
        /// Gets the HeaderContentControl template part
        /// </summary>
        internal ContentControl HeaderContentControlInternal
        {
            get { return this._headerContentControl; }
        }

        #endregion Properties

        #region	Methods

        #region Static Methods

        /// <summary>
        /// Compare ErrorSummaryItems for display in the ErrorSummary
        /// </summary>
        /// <param name="x">The first reference used for comparison.</param>
        /// <param name="y">The second reference used for comparison.</param>
        /// <returns>The result of the comparison check between the two references.</returns>
        internal static int CompareErrorSummaryItems(ErrorSummaryItem x, ErrorSummaryItem y)
        {
            int returnVal;
            if (!ReferencesAreValid(x, y, out returnVal))
            {
                // Do a null comparison check if one (or both) are null
                return returnVal;
            }

            // Compare ErrorSource
            if (TryCompareReferences(x.ErrorType, y.ErrorType, out returnVal))
            {
                return returnVal;
            }

            // Compare Control
            if (x.Control != y.Control)
            {
                if (!ReferencesAreValid(x.Control, y.Control, out returnVal))
                {
                    // Do a null comparison check if one is null
                    return returnVal;
                }
                Control c1 = x.Control as Control;
                Control c2 = y.Control as Control;
                if (c1 != c2)
                {
                    if (!ReferencesAreValid(c1, c2, out returnVal))
                    {
                        // Do a null comparison check if one is null
                        return returnVal;
                    }

                    // Both are controls
                    if (c1.TabIndex != c2.TabIndex)
                    {
                        // Sort by TabIndex
                        return c1.TabIndex.CompareTo(c2.TabIndex);
                    }

                    // TabIndexes are the same, move to next check
                }

                if (TryCompareReferences(x.Control.Name, y.Control.Name, out returnVal))
                {
                    return returnVal;
                }
            }

            // If we reached this point, we could not compare by Control, TabIndex, nor Name.  
            // Compare by Field
            if (TryCompareReferences(x.PropertyName, y.PropertyName, out returnVal))
            {
                return returnVal;
            }

            // Compare by ErrorMessage
            TryCompareReferences(x.ErrorMessage, y.ErrorMessage, out returnVal);
            return returnVal;
        }

        /// <summary>
        /// Try to compare two references, but only if they they are comparable
        /// </summary>
        /// <param name="x">The first reference to compare.</param>
        /// <param name="y">The second reference to compare.</param>
        /// <param name="returnVal">The comparison value.</param>
        /// <returns>Returns true if the two references were able to be compared.</returns>
        private static bool TryCompareReferences(object x, object y, out int returnVal)
        {
            // If the two references are equal, then they should not be used for comparison purposes (in this context)
            // and we should try to use the next set of candidate properties.
            if ((x == null && y == null) || (x != null && x.Equals(y)))
            {
                returnVal = 0;
                return false;
            }

            // Do a reference level comparison, such as if one field is null whereas the other is not null.
            if (!ReferencesAreValid(x, y, out returnVal))
            {
                return true;
            }

            // If both references are valid (non null), try a standard comparison.
            IComparable xComparable = x as IComparable;
            IComparable yComparable = y as IComparable;
            if (xComparable != null && yComparable != null)
            {
                returnVal = xComparable.CompareTo(yComparable);
                return true;
            }

            // Could not compare
            returnVal = 0;
            return false;
        }

        /// <summary>
        /// Perform a null comparison check if one (or both) are null
        /// </summary>
        /// <param name="x">The first reference to compare.</param>
        /// <param name="y">The second reference to compare.</param>
        /// <param name="val">The comparison value.</param>
        /// <returns>Returns true if both references are non-null</returns>
        private static bool ReferencesAreValid(object x, object y, out int val)
        {
            if (x == null)
            {
                val = (y == null) ? 0 : -1;
                return false;
            }
            else if (y == null)
            {
                val = 1;
                return false;
            }
            val = 0;
            return true;
        }

        /// <summary>
        /// When one of the input controls has its ShowErrorsInSummary property changed, we have to go through all the ErrorSummaries on the page and update them
        /// </summary>
        /// <param name="parent"></param>
        private static void UpdateFilteredErrorsOnAllErrorSummaries(DependencyObject parent)
        {
            if (parent == null)
            {
                return;
            }

            ErrorSummary es = parent as ErrorSummary;
            if (es != null)
            {
                es.UpdateFilteredErrors();
                return;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                UpdateFilteredErrorsOnAllErrorSummaries(child);
            }
        }

        #endregion Static Methods

        #region Public Methods

        /// <summary>
        /// When the template is applied, this loads all the template parts
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MouseButtonEventHandler mouseUpHandler = new MouseButtonEventHandler(this.ErrorsListBox_MouseLeftButtonUp);
            KeyEventHandler keyDownHandler = new KeyEventHandler(this.ErrorsListBox_KeyDown);

            if (this._errorsListBox != null)
            {
                // If the ErrorsListBox was already set (due to multiple calls to OnApplyTemplate), unload the handlers first.
                this._errorsListBox.MouseLeftButtonUp -= mouseUpHandler;
                this._errorsListBox.KeyDown -= keyDownHandler;
            }

            this._errorsListBox = GetTemplateChild(PART_ErrorsListBox) as ListBox;
            if (this._errorsListBox != null)
            {
                this._errorsListBox.MouseLeftButtonUp += mouseUpHandler;
                this._errorsListBox.KeyDown += keyDownHandler;
                this._errorsListBox.ItemsSource = this.FilteredErrors;
            }
            this._headerContentControl = GetTemplateChild(PART_HeaderContentControl) as ContentControl;
            this.UpdateFilteredErrors();
            this.UpdateCommonState(false);
            this.UpdateValidationState(false);
        }

        /// <summary>
        /// OnErrorClicked is invoked when an error in the ErrorSummary is clicked, via either the mouse or keyboard.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnErrorClicked(ErrorSummaryItemEventArgs e)
        {
            EventHandler<ErrorSummaryItemEventArgs> handler = this.ErrorClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Simulate a click
        /// </summary>
        internal void ExecuteClickInternal()
        {
            this.ExecuteClick(this.ErrorsListBoxInternal);
        }

        internal string GetHeaderString()
        {
            string errorString = this._filteredErrors.Count == 1 ? resources.ErrorSummaryHeaderError : String.Format(CultureInfo.InvariantCulture, resources.ErrorSummaryHeaderErrors, this._filteredErrors.Count);
            return errorString;
        }

        #endregion Internal Methods

        #region Private Methods

        private void ErrorsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ExecuteClick(sender);
            }
        }

        private void ErrorsListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ExecuteClick(sender);
        }

        private void ErrorsSource_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            FrameworkElement inputControl = e.OriginalSource as FrameworkElement;
            if (e != null && e.Error != null && e.Error.Exception != null)
            {
                ErrorSummaryItem existingError = this._errors.FindError(ErrorType.PropertyError, e.Error.Exception.Message, inputControl);
                if (e.Action == ValidationErrorEventAction.Added)
                {
                    // New error
                    if (existingError == null)
                    {
                        string propertyName = null;
                        object entity;
                        BindingExpression be;
                        ValidationMetadata vmd = AssociatedControl.ParseMetadata(inputControl, false, out entity, out be);
                        if (vmd != null)
                        {
                            propertyName = vmd.Caption;
                        }
                        ErrorSummaryItem esi = new ErrorSummaryItem(e.Error.Exception.Message, ErrorType.PropertyError, null, inputControl, propertyName);
                        this._errors.Add(esi);
                    }
                }
                else
                {
                    // Removing error
                    if (existingError != null)
                    {
                        this._errors.Remove(existingError);
                    }
                }
            }
        }

        private void ErrorSummary_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ErrorsSource == null && this._registeredParent == null)
            {
                this._registeredParent = VisualTreeHelper.GetParent(this) as FrameworkElement;
                if (this._registeredParent != null)
                {
                    this._registeredParent.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(this.ErrorsSource_BindingValidationError);
                }
            }
            this._initialized = true;
        }

        private void ErrorSummary_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateCommonState(true);
        }

        private void Errors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.UpdateFilteredErrors();
        }

        private void ExecuteClick(object sender)
        {
            ListBox lb = sender as ListBox;
            if (lb != null)
            {
                ErrorSummaryItem esi = lb.SelectedItem as ErrorSummaryItem;
                if (esi != null)
                {
                    if (this.FocusControlsOnClick)
                    {
                        Control c = esi.Control as Control;
                        if (c != null)
                        {
                            c.Focus();
                        }
                    }
                    this.OnErrorClicked(new ErrorSummaryItemEventArgs(esi));
                }
            }
        }

        private void UpdateFilteredErrors()
        {
            System.Collections.Generic.List<ErrorSummaryItem> newErrors = new System.Collections.Generic.List<ErrorSummaryItem>();
            Debug.Assert(this.Errors != null, "ErrorSummary.Errors should not be null");
            foreach (ErrorSummaryItem esi in this.Errors)
            {
                if (esi != null && (esi.Control == null || GetShowErrorsInSummary(esi.Control)))
                {
                    if ((esi.ErrorType == ErrorType.EntityError && (this.Filter & ErrorSummaryFilters.EntityErrors) != 0) ||
                        (esi.ErrorType == ErrorType.PropertyError && (this.Filter & ErrorSummaryFilters.PropertyErrors) != 0))
                    {
                        newErrors.Add(esi);
                    }
                }
            }
            newErrors.Sort(CompareErrorSummaryItems);
            this._filteredErrors.Clear();
            foreach (ErrorSummaryItem esi in newErrors)
            {
                this._filteredErrors.Add(esi);
            }
            this.UpdateValidationState(true);
            this.UpdateHeaderText();
        }

        private void UpdateHeaderText()
        {
            if (this._headerContentControl != null)
            {
                if (this.Header != null)
                {
                    this._headerContentControl.Content = this.Header;
                }
                else
                {
                    this._headerContentControl.Content = this.GetHeaderString();
                }
            }
        }

        private void UpdateValidationState(bool useTransitions)
        {
            this.HasErrors = this._filteredErrors.Count > 0;
            VisualStateManager.GoToState(this, this.HasErrors ? VSMSTATE_HasErrors : VSMSTATE_Empty, useTransitions);
        }

        private void UpdateCommonState(bool useTransitions)
        {
            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, VSMSTATE_Normal, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, VSMSTATE_Disabled, useTransitions);
            }
        }

        private void Warnings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateHeaderText();
            this.UpdateValidationState(true);
        }

        #endregion Private Methods

        #endregion Methods
    }
}
