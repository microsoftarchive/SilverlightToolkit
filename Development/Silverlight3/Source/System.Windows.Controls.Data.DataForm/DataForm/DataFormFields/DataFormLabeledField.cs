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
    using System.Windows.Controls.Common;

    /// <summary>
    /// Base class of labeled DataForm fields.  Specifies that the field will
    /// have a label.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract class DataFormLabeledField : DataFormField
    {
        /// <summary>
        /// Identifies the DescriptionViewerPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionViewerPositionProperty =
            DependencyProperty.Register(
                "DescriptionViewerPosition",
                typeof(DataFormDescriptionViewerPosition),
                typeof(DataFormLabeledField),
                new PropertyMetadata(OnDescriptionViewerPositionPropertyChanged));

        /// <summary>
        /// Identifies the DescriptionViewerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionViewerStyleProperty =
            DependencyProperty.Register(
                "DescriptionViewerStyle",
                typeof(Style),
                typeof(DataFormLabeledField),
                new PropertyMetadata(OnDescriptionViewerStylePropertyChanged));

        /// <summary>
        /// Identifies the Label dependency property.
        /// </summary>
        public static readonly DependencyProperty FieldLabelContentProperty =
            DependencyProperty.Register(
                "FieldLabelContent",
                typeof(object),
                typeof(DataFormLabeledField),
                new PropertyMetadata(OnFieldLabelContentPropertyChanged));

        /// <summary>
        /// Identifies the FieldLabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty FieldLabelPositionProperty =
            DependencyProperty.Register(
                "FieldLabelPosition",
                typeof(DataFormFieldLabelPosition),
                typeof(DataFormLabeledField),
                new PropertyMetadata(OnFieldLabelPositionPropertyChanged));

        /// <summary>
        /// Identifies the FieldLabelStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty FieldLabelStyleProperty =
            DependencyProperty.Register(
                "FieldLabelStyle",
                typeof(Style),
                typeof(DataFormLabeledField),
                new PropertyMetadata(OnFieldLabelStylePropertyChanged));

        /// <summary>
        /// Description generated from GenerateDescription().
        /// </summary>
        private DescriptionViewer _description;

        /// <summary>
        /// Label generated from GenerateLabel().
        /// </summary>
        private FieldLabel _label;

        /// <summary>
        /// Private accessor to the parent description position.
        /// </summary>
        private DataFormDescriptionViewerPosition _parentDescriptionViewerPosition;

        /// <summary>
        /// Private accessor to the parent label position.
        /// </summary>
        private DataFormFieldLabelPosition _parentFieldLabelPosition;

        /// <summary>
        /// Constructs a new DataFormLabeledField.
        /// </summary>
        protected DataFormLabeledField()
            : base()
        {
            this.DescriptionViewerPosition = DataFormDescriptionViewerPosition.Auto;
            this.ParentDescriptionViewerPosition = DataFormDescriptionViewerPosition.Auto;
            this.FieldLabelPosition = DataFormFieldLabelPosition.Auto;
            this.ParentFieldLabelPosition = DataFormFieldLabelPosition.Auto;
        }

        /// <summary>
        /// Gets or sets the desired position of the description viewer.
        /// </summary>
        public DataFormDescriptionViewerPosition DescriptionViewerPosition
        {
            get
            {
                return (DataFormDescriptionViewerPosition)this.GetValue(DescriptionViewerPositionProperty);
            }

            set
            {
                this.SetValue(DescriptionViewerPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the description viewer.
        /// </summary>
        public Style DescriptionViewerStyle
        {
            get
            {
                return this.GetValue(DescriptionViewerStyleProperty) as Style;
            }

            set
            {
                this.SetValue(DescriptionViewerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the label.
        /// </summary>
        public object FieldLabelContent
        {
            get
            {
                return this.GetValue(FieldLabelContentProperty);
            }

            set
            {
                this.SetValue(FieldLabelContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the desired position of the label.
        /// </summary>
        public DataFormFieldLabelPosition FieldLabelPosition
        {
            get
            {
                return (DataFormFieldLabelPosition)this.GetValue(FieldLabelPositionProperty);
            }

            set
            {
                this.SetValue(FieldLabelPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the label.
        /// </summary>
        public Style FieldLabelStyle
        {
            get
            {
                return this.GetValue(FieldLabelStyleProperty) as Style;
            }

            set
            {
                this.SetValue(FieldLabelStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets the effective field label position, which is equal to FieldLabelPosition if it's
        /// not equal to Default, or ParentFieldLabelPosition if it is.
        /// </summary>
        internal DataFormDescriptionViewerPosition EffectiveDescriptionViewerPosition
        {
            get
            {
                if (this.DescriptionViewerPosition != DataFormDescriptionViewerPosition.Auto)
                {
                    return this.DescriptionViewerPosition;
                }
                else
                {
                    return this.ParentDescriptionViewerPosition;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent field label position for this field.
        /// </summary>
        internal DataFormDescriptionViewerPosition ParentDescriptionViewerPosition
        {
            get
            {
                return this._parentDescriptionViewerPosition;
            }

            set
            {
                if (value != this._parentDescriptionViewerPosition)
                {
                    this._parentDescriptionViewerPosition = value;

                    if (this.OwningForm != null && this.OwningForm.UIGenerated)
                    {
                        this.OwningForm.GenerateUI();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the effective field label position, which is equal to FieldLabelPosition if it's
        /// not equal to Default, or ParentFieldLabelPosition if it is.
        /// </summary>
        internal DataFormFieldLabelPosition EffectiveFieldLabelPosition
        {
            get
            {
                if (this.FieldLabelPosition != DataFormFieldLabelPosition.Auto)
                {
                    return this.FieldLabelPosition;
                }
                else
                {
                    return this.ParentFieldLabelPosition;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent field label position for this field.
        /// </summary>
        internal DataFormFieldLabelPosition ParentFieldLabelPosition
        {
            get
            {
                return this._parentFieldLabelPosition;
            }

            set
            {
                if (value != this._parentFieldLabelPosition)
                {
                    this._parentFieldLabelPosition = value;

                    if (this.OwningForm != null && this.OwningForm.UIGenerated)
                    {
                        this.OwningForm.GenerateUI();
                    }
                }
            }
        }

        /// <summary>
        /// Generates the description.
        /// </summary>
        /// <returns>The description.</returns>
        internal DescriptionViewer GenerateDescriptionInternal()
        {
            if (this._description == null)
            {
                this._description = this.GenerateDescription();

                if (this.DescriptionViewerStyle != null)
                {
                    this._description.Style = this.DescriptionViewerStyle;
                }
                else if (this.OwningForm != null && this.OwningForm.DescriptionViewerStyle != null)
                {
                    this._description.Style = this.OwningForm.DescriptionViewerStyle;
                }
            }

            return this._description;
        }

        /// <summary>
        /// Generates the label.
        /// </summary>
        /// <returns>The label.</returns>
        internal FieldLabel GenerateLabelInternal()
        {
            if (this._label == null)
            {
                this._label = this.GenerateLabel();
                this.SetLabelContent();
            }

            return this._label;
        }

        /// <summary>
        /// Generates the description.
        /// </summary>
        /// <returns>The description.</returns>
        protected virtual DescriptionViewer GenerateDescription()
        {
            return new DescriptionViewer();
        }

        /// <summary>
        /// Generates the label.
        /// </summary>
        /// <returns>The label.</returns>
        protected virtual FieldLabel GenerateLabel()
        {
            return new FieldLabel();
        }

        /// <summary>
        /// DescriptionViewerPosition property changed handler.
        /// </summary>
        /// <param name="d">DataFormLabeledField that changed its DescriptionViewerPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDescriptionViewerPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormLabeledField labeledField = d as DataFormLabeledField;
            if (labeledField != null && !labeledField.AreHandlersSuspended())
            {
                if (labeledField.OwningForm != null && labeledField.OwningForm.UIGenerated)
                {
                    labeledField.OwningForm.GenerateUI();
                }
            }
        }

        /// <summary>
        /// DescriptionViewerStyle property changed handler.
        /// </summary>
        /// <param name="d">DataFormLabeledField that changed its DescriptionViewerStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDescriptionViewerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormLabeledField labeledField = d as DataFormLabeledField;
            if (labeledField != null && !labeledField.AreHandlersSuspended())
            {
                if (labeledField._label != null)
                {
                    labeledField._label.Style = labeledField.FieldLabelStyle;
                }
            }
        }

        /// <summary>
        /// FieldLabelContent property changed handler.
        /// </summary>
        /// <param name="d">DataFormLabeledField that changed its FieldLabelContent value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnFieldLabelContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormLabeledField labeledField = d as DataFormLabeledField;
            if (labeledField != null && !labeledField.AreHandlersSuspended())
            {
                if (labeledField._label != null)
                {
                    labeledField.SetLabelContent();
                }
            }
        }

        /// <summary>
        /// FieldLabelPosition property changed handler.
        /// </summary>
        /// <param name="d">DataFormLabeledField that changed its FieldLabelPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnFieldLabelPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormLabeledField labeledField = d as DataFormLabeledField;
            if (labeledField != null && !labeledField.AreHandlersSuspended())
            {
                if (labeledField.OwningForm != null && labeledField.OwningForm.UIGenerated)
                {
                    labeledField.OwningForm.GenerateUI();
                }
            }
        }

        /// <summary>
        /// FieldLabelStyle property changed handler.
        /// </summary>
        /// <param name="d">DataFormLabeledField that changed its FieldLabelStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnFieldLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormLabeledField labeledField = d as DataFormLabeledField;
            if (labeledField != null && !labeledField.AreHandlersSuspended())
            {
                if (labeledField._label != null)
                {
                    labeledField._label.Style = labeledField.FieldLabelStyle;
                }
            }
        }

        /// <summary>
        /// Sets the label content.
        /// </summary>
        private void SetLabelContent()
        {
            if (this.FieldLabelContent != null)
            {
                this._label.Content = this.FieldLabelContent;
            }

            if (this.FieldLabelStyle != null)
            {
                this._label.Style = this.FieldLabelStyle;
            }
            else if (this.OwningForm != null)
            {
                this._label.Style = this.OwningForm.FieldLabelStyle;
            }

            if (this.OwningForm.FieldLabelStyle == null && this.EffectiveFieldLabelPosition != DataFormFieldLabelPosition.Top)
            {
                this._label.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
    }
}