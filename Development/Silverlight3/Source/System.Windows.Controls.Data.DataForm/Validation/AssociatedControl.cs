//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls.Common;
using System.Windows.Data;
using resources = System.Windows.Controls.Data.DataForm.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// AssociatedControl is a base class for controls that load metadata and error state information from an input control
    /// and its associated BindingExpression.  Metadata driven controls pull caption, required state details, validation errors, 
    /// and descriptions from the source entity objects so that they can be written once (as attributes) and used throughout 
    /// the application.  
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VSMSTATE_ValidFocused, GroupName = VSMGROUP_ValidationStates)]
    [TemplateVisualState(Name = VSMSTATE_ValidUnfocused, GroupName = VSMGROUP_ValidationStates)]
    [TemplateVisualState(Name = VSMSTATE_InvalidFocused, GroupName = VSMGROUP_ValidationStates)]
    [TemplateVisualState(Name = VSMSTATE_InvalidUnfocused, GroupName = VSMGROUP_ValidationStates)]
    public abstract class AssociatedControl : Control
    {
        #region Static fields and constants

        /// <summary>
        /// VSM Group for validation, containing focus details.  These details are not orthogonal and thus were combined.   See TextBox validation for precedence.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709", Justification = "Using VSM naming conventions.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707", Justification = "Using VSM naming conventions.")]
        private const string VSMGROUP_ValidationStates = "ValidationStates";

        /// <summary>
        /// VSM State for Valid and Focused
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709", Justification = "Using VSM naming conventions.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707", Justification = "Using VSM naming conventions.")]
        protected const string VSMSTATE_ValidFocused = "ValidFocused";

        /// <summary>
        /// VSM State for Valid and Unfocused
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709", Justification = "Using VSM naming conventions.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707", Justification = "Using VSM naming conventions.")]
        protected const string VSMSTATE_ValidUnfocused = "ValidUnfocused";

        /// <summary>
        /// VSM State for Invalid and Focused
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709", Justification = "Using VSM naming conventions.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707", Justification = "Using VSM naming conventions.")]
        protected const string VSMSTATE_InvalidFocused = "InvalidFocused";

        /// <summary>
        /// VSM State for Invalid and Focused
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709", Justification = "Using VSM naming conventions.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707", Justification = "Using VSM naming conventions.")]
        protected const string VSMSTATE_InvalidUnfocused = "InvalidUnfocused";

        #endregion Static fields and constants

        #region Member Fields

        private bool _initialized;

        #endregion Member Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AssociatedControl class.
        /// </summary>
        protected AssociatedControl()
        {
            this.Loaded += new RoutedEventHandler(this.AssociatedControl_Loaded);

            // Set binding to self for DataContext change notifications
            this.SetBinding(AssociatedControl.DataContextProperty, new Binding());
            this.IsValid = true;
        }

        #endregion Constructors

        #region AttachedProperties

        #region ValidationMetadata

        /// <summary>
        /// Gets or sets the ValidationMetadata, which represents all of the metadata associated with the binding path of the input control.  This includes
        /// IsFieldRequired, RequiredFieldMessage, Caption, and Description.
        /// </summary>
        internal static readonly DependencyProperty ValidationMetadataProperty = DependencyProperty.RegisterAttached(
            "ValidationMetadata",
            typeof(ValidationMetadata),
            typeof(AssociatedControl),
            null);

        /// <summary>
        /// Gets the ValidationMetadata property for the input control
        /// </summary>
        /// <param name="inputControl">The input control to get the ValidationMetadata property from.</param>
        /// <returns>The ValidationMetadata associated with the input control.</returns>
        internal static ValidationMetadata GetValidationMetadata(DependencyObject inputControl)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            return inputControl.GetValue(ValidationMetadataProperty) as ValidationMetadata;
        }

        /// <summary>
        /// Sets the ValidationMetadata property for the input control
        /// </summary>
        /// <param name="inputControl">The input control to set the ValidationMetadata property on.</param>
        /// <param name="value">The ValidationMetadata to associate with the input control.</param>
        internal static void SetValidationMetadata(DependencyObject inputControl, ValidationMetadata value)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            inputControl.SetValue(ValidationMetadataProperty, value);
        }

        #endregion ValidationMetadata

        #endregion AttachedProperties

        #region Dependency Properties

        #region DataContext

        /// <summary>
        /// Identifies the DataContext dependency property.
        /// </summary>
        private static new readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(
            "DataContext",
            typeof(object),
            typeof(AssociatedControl),
            new PropertyMetadata(OnDataContextPropertyChanged));

        private static void OnDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AssociatedControl ac = d as AssociatedControl;
            if (ac != null)
            {
                if (e.OldValue == null || e.NewValue == null || e.OldValue.GetType() != e.NewValue.GetType())
                {
                    // Refresh the metadata, but only if the DataContext's type has changed (or if either is null)
                    ac.LoadMetadata(false);
                }
            }
        }

        #endregion DataContext

        #region IsFocused

        /// <summary>
        /// Identifies the IsFocused dependency property
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
            "IsFocused",
            typeof(bool),
            typeof(AssociatedControl),
            new PropertyMetadata(false, OnIsFocusedPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the AssociatedControl's target is currently being focused.
        /// </summary>
        protected bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            private set { this.SetValueNoCallback(IsFocusedProperty, value); }
        }

        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AssociatedControl ac = d as AssociatedControl;
            if (ac != null && !ac.AreHandlersSuspended())
            {
                ac.SetValueNoCallback(AssociatedControl.IsFocusedProperty, e.OldValue);
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, resources.UnderlyingPropertyIsReadOnly, "IsFocused"));
            }
        }

        #endregion IsFocused

        #region IsValid

        /// <summary>
        /// Identifies the IsValid dependency property
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
            "IsValid",
            typeof(bool),
            typeof(AssociatedControl),
            new PropertyMetadata(true, OnIsValidPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the AssociatedControl has errors.
        /// </summary>
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            private set { this.SetValueNoCallback(IsValidProperty, value); }
        }

        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AssociatedControl ac = d as AssociatedControl;
            if (ac != null && !ac.AreHandlersSuspended())
            {
                ac.SetValueNoCallback(AssociatedControl.IsValidProperty, e.OldValue);
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, resources.UnderlyingPropertyIsReadOnly, "IsValid"));
            }
        }

        #endregion IsValid

        #region PropertyPath

        /// <summary>
        /// Identifies the PropertyPath dependency property
        /// </summary>
        public static readonly DependencyProperty PropertyPathProperty = DependencyProperty.Register(
            "PropertyPath",
            typeof(string),
            typeof(AssociatedControl),
            new PropertyMetadata(OnPropertyPathPropertyChanged));

        /// <summary>
        /// Gets or sets the property path
        /// </summary>
        public string PropertyPath
        {
            get { return GetValue(AssociatedControl.PropertyPathProperty) as string; }
            set { SetValue(AssociatedControl.PropertyPathProperty, value); }
        }

        private static void OnPropertyPathPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            AssociatedControl ac = depObj as AssociatedControl;
            if (ac != null && ac.Initialized)
            {
                ac.LoadMetadata(false);
                // Changing the PropertyPath sometimes requires an update for the validation state, since it might be stale.
                ac.ParseTargetValidState();
            }
        }

        #endregion PropertyPath

        #region Target

        /// <summary>
        /// Identifies the Target dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(
            "Target",
            typeof(FrameworkElement),
            typeof(AssociatedControl),
            new PropertyMetadata(OnTargetPropertyChanged));

        /// <summary>
        /// Gets or sets the targeted FrameworkElement from which to pull metadata.
        /// </summary>
        public FrameworkElement Target
        {
            get { return GetValue(TargetProperty) as FrameworkElement; }
            set { SetValue(TargetProperty, value); }
        }

        private static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AssociatedControl ac = d as AssociatedControl;
            if (ac != null)
            {
                bool targetFocused = e.NewValue == System.Windows.Input.FocusManager.GetFocusedElement();
                if (ac.IsFocused != targetFocused)
                {
                    ac.IsFocused = targetFocused;
                    ac.OnTargetFocusedChanged();
                }
                ac.LoadMetadata(false);
                ac.OnTargetLoaded(e.OldValue as FrameworkElement, e.NewValue as FrameworkElement);
            }
        }

        #endregion Target

        #endregion Dependency Properties

        #region Properties

        /// <summary>
        /// Test accessor for IsValid
        /// </summary>
        internal bool IsValidInternal
        {
            get { return this.IsValid; }
        }

        /// <summary>
        /// Internally get or set the ValidationMetadata.  
        /// </summary>
        internal ValidationMetadata ValidationMetadata
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the control has been initialized.
        /// </summary>
        protected internal bool Initialized
        {
            get { return this._initialized; }
        }

        #endregion Properties

        #region Methods

        #region Static Methods

        /// <summary>
        /// Parse metadata from a target FrameworkElement.  This will cache the metadata on the element as an attached property.
        /// </summary>
        /// <param name="element">The target FrameworkElement to pull metadata from.</param>
        /// <param name="forceUpdate">If set, will not pull metadata from cache.</param>
        /// <param name="entity">The entity used.</param>
        /// <param name="bindingExpression">The bindingExpression used.</param>
        /// <returns>Returns the metadata associated with the element.  Will be null if no metadata was found.</returns>
        internal static ValidationMetadata ParseMetadata(FrameworkElement element, bool forceUpdate, out object entity, out BindingExpression bindingExpression)
        {
            entity = null;
            bindingExpression = null;
            if (element == null)
            {
                return null;
            }

            if (!forceUpdate)
            {
                ValidationMetadata existingVMD = element.GetValue(ValidationMetadataProperty) as ValidationMetadata;
                if (existingVMD != null)
                {
                    return existingVMD;
                }
            }

            BindingExpression be = null;
            FieldInfo[] fields = element.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(DependencyProperty))
                {
                    // Found a dependency property
                    be = element.GetBindingExpression((DependencyProperty)field.GetValue(null));
                    if (be != null && be.ParentBinding != null && be.ParentBinding.Path != null)
                    {
                        // Found a BindingExpression, ensure it has valid data
                        entity = be.DataItem != null ? be.DataItem : element.DataContext;
                        if (entity != null)
                        {
                            if (be.ParentBinding.Mode == BindingMode.TwoWay)
                            {
                                bindingExpression = be;
                                // A twoway binding will be automatically chosen and the rest ignored
                                break;
                            }

                            // Perform an arbitrary sort on path (string), so the same DP is chosen consistently.
                            // Reflection ordering is not deterministic and if we just pick the first, we could be 
                            // matched with different DPs depending on the run.
                            if (bindingExpression == null || string.Compare(be.ParentBinding.Path.Path, bindingExpression.ParentBinding.Path.Path, StringComparison.Ordinal) < 0)
                            {
                                bindingExpression = be;
                            }
                        }
                    }
                }
            }
            if (bindingExpression != null)
            {
                return ParseMetadata(bindingExpression.ParentBinding.Path.Path, entity);
            }
            return null;
        }

        /// <summary>
        /// Parse metadata given a binding path and entity object.
        /// </summary>
        /// <param name="bindingPath">The bindingPath is the name of the property on the entity from which to pull metadata from.  This supports dot notation.</param>
        /// <param name="entity">The entity object from which to pull metadata from.</param>
        /// <returns>The validation metadata associated with the entity and binding path.  This will return null if none exists.</returns>
        internal static ValidationMetadata ParseMetadata(string bindingPath, object entity)
        {
            if (entity != null && !String.IsNullOrEmpty(bindingPath))
            {
                Type entityType = entity.GetType();
                PropertyInfo prop = GetProperty(entityType, bindingPath);
                if (prop != null)
                {
                    ValidationMetadata newVMD = new ValidationMetadata();
                    object[] attributes = prop.GetCustomAttributes(false);
                    bool displayAttributeFound = false;
                    foreach (object propertyAttribute in attributes)
                    {
                        // Loop through each attribute and update the VMD as appropriate

                        // RequiredField
                        RequiredAttribute reqAttribute = propertyAttribute as RequiredAttribute;
                        if (reqAttribute != null)
                        {
                            newVMD.IsRequired = true;
                            string errorMessage = reqAttribute.FormatErrorMessage(bindingPath);
                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                newVMD.RequiredFieldMessage = errorMessage;
                            }
                            continue;
                        }

                        // Display attribute parsing
                        DisplayAttribute displayAttribute = propertyAttribute as DisplayAttribute;
                        if (displayAttribute != null)
                        {
                            displayAttributeFound = true;
                            newVMD.Description = displayAttribute.GetDescription();
                            newVMD.Caption = displayAttribute.GetName();
                            continue;
                        }
                    }
                    if (!displayAttributeFound)
                    {
                        // If the DisplayAttribute is not defined or Name is null, use the property name. 
                        newVMD.Caption = prop.Name;
                    }
                    return newVMD;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the PropertyInfo for a given entity type.  Supports dot notation to represent nested objects.
        /// </summary>
        /// <param name="entityType">The type of the object.</param>
        /// <param name="propertyPath">The property path, supporting dot notation.</param>
        /// <returns>The PropertyInfo corresponding to the final property.</returns>
        private static PropertyInfo GetProperty(Type entityType, string propertyPath)
        {
            Debug.Assert(entityType != null, "Unexpected null entityType in AssociatedControl.GetProperty");
            Debug.Assert(propertyPath != null, "Unexpected null propertyPath in AssociatedControl.GetProperty");
            Type itemType = entityType;

            string[] propertyNames = propertyPath.Split('.');
            if (propertyNames != null)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    PropertyInfo propertyInfo = itemType.GetProperty(propertyNames[i]);
                    if (propertyInfo == null || !propertyInfo.CanRead)
                    {
                        return null;
                    }

                    if (i == propertyNames.Length - 1)
                    {
                        return propertyInfo;
                    }
                    else
                    {
                        itemType = propertyInfo.PropertyType;
                    }
                }
            }
            return null;
        }

        #endregion Static Methods

        #region Public Methods

        /// <summary>
        /// Update VSM states when the template is set
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Set default states
            this.UpdateValidationState();
        }

        /// <summary>
        /// Reload the metadata from the source target or DataContext, and update the VSM states.
        /// </summary>
        public virtual void Refresh()
        {
            this.LoadMetadata(true);
            this.ParseTargetValidState();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// This method is triggered when the metadata is loaded.  Deriving classes can override this to make sure of updated metadata.
        /// </summary>
        protected virtual void OnMetadataLoaded()
        {
        }

        /// <summary>
        /// Triggered when the target focus has changed
        /// </summary>
        protected virtual void OnTargetFocusedChanged()
        {
        }

        /// <summary>
        /// This method is called whenever the target is loaded.  Deriving classes should override this method to perform
        /// actions based on the target.
        /// </summary>
        /// <param name="oldTarget">The target FrameworkElement being replaced.</param>
        /// <param name="newTarget">The target FrameworkElement being added.</param>
        protected virtual void OnTargetLoaded(FrameworkElement oldTarget, FrameworkElement newTarget)
        {
            EventHandler<ValidationErrorEventArgs> bindingHandler = new EventHandler<ValidationErrorEventArgs>(this.Target_BindingValidationError);
            RoutedEventHandler gotFocusHandler = new RoutedEventHandler(this.Target_GotFocus);
            RoutedEventHandler lostFocusHandler = new RoutedEventHandler(this.Target_LostFocus);
            if (oldTarget != null)
            {
                oldTarget.BindingValidationError -= bindingHandler;
                oldTarget.GotFocus -= gotFocusHandler;
                oldTarget.LostFocus -= lostFocusHandler;
            }
            if (newTarget != null)
            {
                newTarget.BindingValidationError += bindingHandler;
                newTarget.GotFocus += gotFocusHandler;
                newTarget.LostFocus += lostFocusHandler;
            }
            this.ParseTargetValidState();
        }

        /// <summary>
        /// Update the validation VSM state
        /// </summary>
        protected virtual void UpdateValidationState()
        {
            if (this.IsValid)
            {
                VisualStateManager.GoToState(this, this.IsFocused ? VSMSTATE_ValidFocused : VSMSTATE_ValidUnfocused, true);
            }
            else
            {
                VisualStateManager.GoToState(this, this.IsFocused ? VSMSTATE_InvalidFocused : VSMSTATE_InvalidUnfocused, true);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Perform initialization code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this._initialized)
            {
                // Loading Metadata onload because the DP could have changed before load and before the target was initialized
                this.LoadMetadata(false);
                this._initialized = true;
                this.Loaded -= new RoutedEventHandler(this.AssociatedControl_Loaded);
            }
        }

        /// <summary>
        /// Load meta data and update the UI. 
        /// </summary>
        /// <param name="forceUpdate">If true, metadata will not be loaded from cache.</param>
        private void LoadMetadata(bool forceUpdate)
        {
            ValidationMetadata vmd = null;
            object entity = null;
            BindingExpression bindingExpression = null;
            if (!String.IsNullOrEmpty(this.PropertyPath))
            {
                entity = this.DataContext;
                // Pull metadata directly from the DataContext.  This isn't cached so it will be pulled every time.
                vmd = ParseMetadata(this.PropertyPath, entity);
            }
            else if (this.Target != null)
            {
                // Pull the metadata from the target FrameworkElement.  
                vmd = ParseMetadata(this.Target, forceUpdate, out entity, out bindingExpression);
            }
            if (this.ValidationMetadata != vmd)
            {
                this.ValidationMetadata = vmd;
                this.OnMetadataLoaded();
            }
        }

        /// <summary>
        /// Parse the target error state and update the IsValid property
        /// </summary>
        private void ParseTargetValidState()
        {
            if (!String.IsNullOrEmpty(this.PropertyPath))
            {
                // If PropertyPath is set, the IsValid state is not used and defaults to true, even if the PropertyPath is itself invalid.
                this.IsValid = true;
            }
            else if (this.Target != null)
            {
                this.IsValid = !Validation.GetHasError(this.Target);
            }
            else
            {
                // If no target is specified, IsValid state defaults back to true.
                this.IsValid = true;
            }
            this.UpdateValidationState();
        }

        /// <summary>
        /// Event handler for target control's BindingValidationError event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Target_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            this.ParseTargetValidState();
        }

        /// <summary>
        /// Event handler for the target control's GotFocus event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Target_GotFocus(object sender, RoutedEventArgs e)
        {
            this.IsFocused = true;
            this.OnTargetFocusedChanged();
            this.UpdateValidationState();
        }

        /// <summary>
        /// Event handler for the target control's LostFocus event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Target_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsFocused = false;
            this.OnTargetFocusedChanged();
            this.UpdateValidationState();
        }

        #endregion Private Methods

        #endregion Methods
    }
}
