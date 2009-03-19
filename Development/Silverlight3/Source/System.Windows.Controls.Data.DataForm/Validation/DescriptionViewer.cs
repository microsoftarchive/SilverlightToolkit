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
    /// A DescriptionViewer displays caption and required field
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VSMSTATE_Normal, GroupName = VSMGROUP_CommonStates)]
    [TemplateVisualState(Name = VSMSTATE_Disabled, GroupName = VSMGROUP_CommonStates)]
    [TemplateVisualState(Name = VSMSTATE_NoDescription, GroupName = VSMGROUP_DescriptionStates)]
    [TemplateVisualState(Name = VSMSTATE_HasDescription, GroupName = VSMGROUP_DescriptionStates)]
    public class DescriptionViewer : AssociatedControl
    {
        #region Static fields and constants

        private const string VSMGROUP_CommonStates = "CommonStates";
        private const string VSMGROUP_DescriptionStates = "DescriptionStates";

        private const string VSMSTATE_Normal = "Normal";
        private const string VSMSTATE_Disabled = "Disabled";
        private const string VSMSTATE_NoDescription = "NoDescription";
        private const string VSMSTATE_HasDescription = "HasDescription";

        #endregion Static fields and constants

        #region Member fields

        private bool _descriptionOverridden;

        #endregion Member fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DescriptionViewer class.
        /// </summary>
        public DescriptionViewer()
        {
            this.DefaultStyleKey = typeof(DescriptionViewer);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.DescriptionViewer_IsEnabledChanged);
        }

        #endregion Constructors

        #region Dependency Properties

        #region Description

        /// <summary>
        /// Identifies the Description dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",
            typeof(string),
            typeof(DescriptionViewer),
            new PropertyMetadata(OnDescriptionPropertyChanged));

        /// <summary>
        /// Gets or sets the description text of the property.
        /// </summary>
        public string Description
        {
            get
            {
                return GetValue(DescriptionProperty) as string;
            }

            set
            {
                this._descriptionOverridden = true;
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Handle the Description field property change event.  This will update the the VSM state.
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        private static void OnDescriptionPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            // Dependency property changed
            DescriptionViewer dv = depObj as DescriptionViewer;
            if (dv != null)
            {
                dv.UpdateDescriptionState();
            }
        }

        #endregion Description

        #region GlyphTemplate

        /// <summary>
        /// Identifies the GlyphTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty GlyphTemplateProperty =
            DependencyProperty.Register(
            "GlyphTemplate",
            typeof(ControlTemplate),
            typeof(DescriptionViewer),
            null);

        /// <summary>
        /// Gets or sets the GlyphTemplate.
        /// </summary>
        public ControlTemplate GlyphTemplate
        {
            get { return GetValue(GlyphTemplateProperty) as ControlTemplate; }
            set { SetValue(GlyphTemplateProperty, value); }
        }

        #endregion GlyphTemplate

        #region ToolTipStyle

        /// <summary>
        /// Identifies the ToolTipStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolTipStyleProperty =
            DependencyProperty.Register(
            "ToolTipStyle",
            typeof(Style),
            typeof(DescriptionViewer),
            null);

        /// <summary>
        /// Gets or sets the ToolTipStyle.
        /// </summary>
        public Style ToolTipStyle
        {
            get { return GetValue(ToolTipStyleProperty) as Style; }
            set { SetValue(ToolTipStyleProperty, value); }
        }

        #endregion ToolTipStyle

        #endregion Dependency Properties

        #region Methods

        /// <summary>
        /// When the template is applied, this loads all the template parts
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Set default states
            this.UpdateDescriptionState();
        }

        /// <summary>
        /// Reload the metadata from the source target or DomainContext
        /// </summary>
        public override void Refresh()
        {
            this._descriptionOverridden = false;
            base.Refresh();
        }

        /// <summary>
        /// This method is called whenever the metadata is loaded.  When loaded, the description field is set.
        /// </summary>
        protected override void OnMetadataLoaded()
        {
            // Update to the new VMD
            if (!this._descriptionOverridden)
            {
                string description = null;
                if (this.ValidationMetadata != null)
                {
                    description = this.ValidationMetadata.Description;
                }
                SetValue(DescriptionProperty, description);
            }
        }

        /// <summary>
        /// When updating the validation state, check the focus state and update the VSM accordingly
        /// </summary>
        protected override void UpdateValidationState()
        {
            if (this.IsValid)
            {
                if (this.IsFocused && !String.IsNullOrEmpty(this.Description))
                {
                    VisualStateManager.GoToState(this, VSMSTATE_ValidFocused, true);
                }
                else
                {
                    VisualStateManager.GoToState(this, VSMSTATE_ValidUnfocused, true);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, this.IsFocused ? VSMSTATE_InvalidFocused : VSMSTATE_InvalidUnfocused, true);
            }
        }

        /// <summary>
        /// IsEnabled property change handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescriptionViewer_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
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
        /// Update the required field VSM state based on the description property.  
        /// </summary>
        private void UpdateDescriptionState()
        {
            if (!String.IsNullOrEmpty(this.Description))
            {
                VisualStateManager.GoToState(this, VSMSTATE_HasDescription, true);
            }
            else
            {
                VisualStateManager.GoToState(this, VSMSTATE_NoDescription, true);
            }
        }

        #endregion Methods
    }
}
