//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Data;

namespace System.Windows.Controls
{
    using System.Windows.Controls.Common;
    using System.Reflection;

    /// <summary>
    /// Base class of bound DataForm fields.  Specifies that the field will
    /// make use of a Binding property.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract class DataFormBoundField : DataFormLabeledField
    {
        /// <summary>
        /// Identifies the Binding dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(
                "Binding",
                typeof(Binding),
                typeof(DataFormBoundField),
                new PropertyMetadata(OnBindingPropertyChanged));

        /// <summary>
        /// Identifies the ElementStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementStyleProperty =
            DependencyProperty.Register(
                "ElementStyle",
                typeof(Style),
                typeof(DataFormBoundField),
                new PropertyMetadata(OnElementStylePropertyChanged));

        /// <summary>
        /// Identifies the EditingElementStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty EditingElementStyleProperty =
            DependencyProperty.Register(
                "EditingElementStyle",
                typeof(Style),
                typeof(DataFormBoundField),
                new PropertyMetadata(OnEditingElementStylePropertyChanged));

        /// <summary>
        /// Identifies the InsertElementStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty InsertElementStyleProperty =
            DependencyProperty.Register(
                "InsertElementStyle",
                typeof(Style),
                typeof(DataFormBoundField),
                new PropertyMetadata(OnInsertElementStylePropertyChanged));

        /// <summary>
        /// Gets or sets the binding to be given to the input control.
        /// </summary>
        public Binding Binding
        {
            get
            {
                return this.GetValue(BindingProperty) as Binding;
            }

            set
            {
                this.SetValue(BindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style to be given to the display input control.
        /// </summary>
        public Style ElementStyle
        {
            get
            {
                return this.GetValue(ElementStyleProperty) as Style;
            }

            set
            {
                this.SetValue(ElementStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style to be given to the edit input control.
        /// </summary>
        public Style EditingElementStyle
        {
            get
            {
                return this.GetValue(EditingElementStyleProperty) as Style;
            }

            set
            {
                this.SetValue(EditingElementStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style to be given to the insert input control.
        /// </summary>
        public Style InsertElementStyle
        {
            get
            {
                return this.GetValue(InsertElementStyleProperty) as Style;
            }

            set
            {
                this.SetValue(InsertElementStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the input control should be
        /// read-only in edit mode, taking into account both IsReadOnly and
        /// IsBoundPropertyReadOnly.
        /// </summary>
        internal override bool EffectiveIsEditModeReadOnly
        {
            get
            {
                if (this.IsBoundPropertyReadOnly)
                {
                    return true;
                }
                else
                {
                    return base.EffectiveIsEditModeReadOnly;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the input control should be
        /// read-only, taking into account both IsReadOnly and IsBoundPropertyReadOnly.
        /// </summary>
        internal override bool EffectiveIsReadOnly
        {
            get
            {
                if (this.IsBoundPropertyReadOnly)
                {
                    return true;
                }
                else
                {
                    return base.EffectiveIsReadOnly;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the bound property (if one exists) is read-only.
        /// </summary>
        private bool IsBoundPropertyReadOnly
        {
            get
            {
                if (this.OwningForm != null && this.OwningForm.CurrentItem != null && this.Binding != null && this.Binding.Path != null)
                {
                    PropertyInfo propertyInfo = this.OwningForm.CurrentItemType.GetPropertyInfo(this.Binding.Path.Path);

                    if (propertyInfo != null)
                    {
                        if (!propertyInfo.CanWrite)
                        {
                            return true;
                        }

                        // If we have an unknown type that we can't edit, the field is read-only.
                        Type nonNullablePropertyType = propertyInfo.PropertyType.GetNonNullableType();

                        if (!nonNullablePropertyType.IsEnum &&
                            nonNullablePropertyType != typeof(System.String) &&
                            nonNullablePropertyType != typeof(System.Char) &&
                            nonNullablePropertyType != typeof(System.DateTime) &&
                            nonNullablePropertyType != typeof(System.Boolean) &&
                            nonNullablePropertyType != typeof(System.Byte) &&
                            nonNullablePropertyType != typeof(System.SByte) &&
                            nonNullablePropertyType != typeof(System.Single) &&
                            nonNullablePropertyType != typeof(System.Double) &&
                            nonNullablePropertyType != typeof(System.Decimal) &&
                            nonNullablePropertyType != typeof(System.Int16) &&
                            nonNullablePropertyType != typeof(System.Int32) &&
                            nonNullablePropertyType != typeof(System.Int64) &&
                            nonNullablePropertyType != typeof(System.UInt16) &&
                            nonNullablePropertyType != typeof(System.UInt32) &&
                            nonNullablePropertyType != typeof(System.UInt64))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Binding property changed handler.
        /// </summary>
        /// <param name="d">DataFormBoundField that changed its Binding value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormBoundField boundField = d as DataFormBoundField;
            if (boundField != null && !boundField.AreHandlersSuspended())
            {
                if (boundField.OwningForm != null && boundField.OwningForm.IsEditing)
                {
                    boundField.OwningForm.ForceEndEdit();
                }

                if (boundField.Binding != null)
                {
                    if (boundField.Binding.Path != null && !String.IsNullOrEmpty(boundField.Binding.Path.Path) && !boundField.IsAutoGenerated)
                    {
                        boundField.Binding.Mode = BindingMode.TwoWay;
                        boundField.Binding.ValidatesOnExceptions = true;
                        boundField.Binding.NotifyOnValidationError = true;
                        boundField.Binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
                    }

                    if (boundField.Binding.Converter == null)
                    {
                        boundField.Binding.Converter = new DataFormValueConverter();
                    }

                    if (boundField.OwningForm != null && boundField.OwningForm.UIGenerated)
                    {
                        boundField.OwningForm.GenerateUI();
                    }
                }
            }
        }

        /// <summary>
        /// ElementStyle property changed handler.
        /// </summary>
        /// <param name="d">DataFormBoundField that changed its ElementStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnElementStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormBoundField boundField = d as DataFormBoundField;
            if (boundField != null && !boundField.AreHandlersSuspended())
            {
                if (boundField.OwningForm != null && !boundField.OwningForm.IsEditing)
                {
                    boundField.Element.Style = boundField.ElementStyle;
                }
            }
        }

        /// <summary>
        /// EditingElementStyle property changed handler.
        /// </summary>
        /// <param name="d">DataFormBoundField that changed its EditingElementStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnEditingElementStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormBoundField boundField = d as DataFormBoundField;
            if (boundField != null && !boundField.AreHandlersSuspended())
            {
                if (boundField.OwningForm != null && boundField.OwningForm.IsEditing && !boundField.OwningForm.IsAddingNew)
                {
                    boundField.Element.Style = boundField.EditingElementStyle;
                }
            }
        }

        /// <summary>
        /// InsertElementStyle property changed handler.
        /// </summary>
        /// <param name="d">DataFormBoundField that changed its InsertElementStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnInsertElementStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormBoundField boundField = d as DataFormBoundField;
            if (boundField != null && !boundField.AreHandlersSuspended())
            {
                if (boundField.OwningForm != null && boundField.OwningForm.IsAddingNew)
                {
                    boundField.Element.Style = boundField.InsertElementStyle;
                }
            }
        }
    }
}
