//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls
{
    /// <summary>
    /// A FieldLabel displays caption and required field
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VSMSTATE_Normal, GroupName = VSMGROUP_CommonStates)]
    [TemplateVisualState(Name = VSMSTATE_Disabled, GroupName = VSMGROUP_CommonStates)]
    [TemplateVisualState(Name = VSMSTATE_NotRequired, GroupName = VSMGROUP_RequiredStates)]
    [TemplateVisualState(Name = VSMSTATE_Required, GroupName = VSMGROUP_RequiredStates)]
    public class FieldLabel : AssociatedControl
    {
        #region Static fields and constants

        private const string VSMGROUP_CommonStates = "CommonStates";
        private const string VSMGROUP_RequiredStates = "RequiredStates";

        private const string VSMSTATE_Normal = "Normal";
        private const string VSMSTATE_Disabled = "Disabled";
        private const string VSMSTATE_NotRequired = "NotRequired";
        private const string VSMSTATE_Required = "Required";

        #endregion Static fields and constants

        #region Member fields

        private bool _contentOverridden;
        private bool _isRequiredOverridden;

        #endregion Member fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the FieldLabel class.
        /// </summary>
        public FieldLabel()
        {
            this.DefaultStyleKey = typeof(FieldLabel);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.FieldLabel_IsEnabledChanged);
        }

        #endregion Constructors

        #region Dependency Properties

        #region Content

        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(FieldLabel),
            null);

        /// <summary>
        /// Gets or sets the Content of the FieldLabel.
        /// </summary>
        public object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }

            set
            {
                this._contentOverridden = true;
                SetValue(ContentProperty, value);
            }
        }

        #endregion Content

        #region ContentTemplate

        /// <summary>
        /// Identifies the ContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
            "ContentTemplate",
            typeof(DataTemplate),
            typeof(FieldLabel),
            null);

        /// <summary>
        /// Gets or sets the ContentTemplate of the FieldLabel.
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get { return GetValue(ContentTemplateProperty) as DataTemplate; }
            set { SetValue(ContentTemplateProperty, value); }
        }

        #endregion ContentTemplate

        #region IsRequired

        /// <summary>
        /// Identifies the IsRequired dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
            "IsRequired",
            typeof(bool),
            typeof(FieldLabel),
            new PropertyMetadata(OnIsRequiredPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the field is required.
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return (bool)GetValue(FieldLabel.IsRequiredProperty);
            }

            set
            {
                this._isRequiredOverridden = true;
                SetValue(FieldLabel.IsRequiredProperty, value);
            }
        }

        /// <summary>
        /// Handle the IsRequired field property change event.
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        private static void OnIsRequiredPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            // Dependency property changed
            FieldLabel fl = depObj as FieldLabel;
            if (fl != null)
            {
                fl.UpdateRequiredState();
            }
        }

        #endregion IsRequired

        #endregion Dependency Properties

        #region Methods

        /// <summary>
        /// When the template is applied, this loads all the template parts
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Set default states
            this.UpdateRequiredState();
        }

        /// <summary>
        /// Reload the metadata from the source target or DomainContext
        /// </summary>
        public override void Refresh()
        {
            this._contentOverridden = false;
            this._isRequiredOverridden = false;
            base.Refresh();
        }

        /// <summary>
        /// This method is called whenever the metadata is loaded.  When loaded, the caption and IsRequired fields are
        /// parsed and used to update the FieldLabel Content and IsRequired properties.
        /// </summary>
        protected override void OnMetadataLoaded()
        {
            // Update to the new VMD
            if (!this._contentOverridden)
            {
                object newContent = null;
                if (this.ValidationMetadata != null)
                {
                    newContent = this.ValidationMetadata.Caption;
                }
                SetValue(FieldLabel.ContentProperty, newContent);
            }
            if (!this._isRequiredOverridden)
            {
                bool isRequired = this.ValidationMetadata == null ? false : this.ValidationMetadata.IsRequired;
                SetValue(FieldLabel.IsRequiredProperty, isRequired);
            }
        }

        /// <summary>
        /// IsEnabled property change handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FieldLabel_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateCommonState();
        }

        /// <summary>
        /// Update the Common VSM state
        /// </summary>
        private void UpdateCommonState()
        {
            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, VSMSTATE_Normal, true);
            }
            else
            {
                VisualStateManager.GoToState(this, VSMSTATE_Disabled, true);
            }
        }

        /// <summary>
        /// Update the required field VSM state based on the IsRequired property.  
        /// </summary>
        private void UpdateRequiredState()
        {
            if (this.IsRequired)
            {
                VisualStateManager.GoToState(this, VSMSTATE_Required, true);
            }
            else
            {
                VisualStateManager.GoToState(this, VSMSTATE_NotRequired, true);
            }
        }

        #endregion Methods
    }
}
