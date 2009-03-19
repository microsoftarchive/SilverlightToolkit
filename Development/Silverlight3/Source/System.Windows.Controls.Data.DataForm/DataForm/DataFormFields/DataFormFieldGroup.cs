//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Collections.Specialized;

namespace System.Windows.Controls
{
    /// <summary>
    /// Group field for the DataForm.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "DataFormFieldGroup is more than a collection.")]
    [ContentProperty("Fields")]
    public class DataFormFieldGroup : DataFormField
    {
        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(DataFormFieldGroup),
                new PropertyMetadata(OnOrientationPropertyChanged));

        /// <summary>
        /// Identifies the WrapAfter dependency property.
        /// </summary>
        public static readonly DependencyProperty WrapAfterProperty =
            DependencyProperty.Register(
                "WrapAfter",
                typeof(int),
                typeof(DataFormFieldGroup),
                new PropertyMetadata(OnWrapAfterPropertyChanged));

        /// <summary>
        /// Private accessor for Fields.
        /// </summary>
        private ObservableCollection<DataFormField> _fields;

        /// <summary>
        /// Constructs a new instance of DataFormFieldGroup.
        /// </summary>
        public DataFormFieldGroup()
        {
            this._fields = new ObservableCollection<DataFormField>();
            this._fields.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.OnFieldsCollectionChanged);
            this.Orientation = Orientation.Vertical;
        }

        /// <summary>
        /// Gets the collection of child fields for this group field.
        /// </summary>
        public ObservableCollection<DataFormField> Fields
        {
            get
            {
                return this._fields;
            }
        }

        /// <summary>
        /// Gets or sets the desired orientation of this group of fields.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }

            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets how many items to wrap after.  A value of 0 implies no wrapping.
        /// </summary>
        public int WrapAfter
        {
            get
            {
                return (int)this.GetValue(WrapAfterProperty);
            }

            set
            {
                this.SetValue(WrapAfterProperty, value);
            }
        }

        /// <summary>
        /// Handles the situation where a field's hosted state has changed.
        /// </summary>
        /// <param name="e">The event args for this event.</param>
        internal override void OnOwningFormChanged(EventArgs e)
        {
            base.OnOwningFormChanged(e);

            if (this._fields != null)
            {
                foreach (DataFormField field in this._fields)
                {
                    field.OwningForm = this.OwningForm;
                }
            }
        }

        /// <summary>
        /// Handles the situation where the status of the DataForm's current item
        /// has changed.
        /// </summary>
        /// <param name="e">The event args for this event.</param>
        internal override void OnIsDataFormCurrentItemNullChanged(EventArgs e)
        {
            base.OnIsDataFormCurrentItemNullChanged(e);

            if (this._fields != null)
            {
                foreach (DataFormField field in this._fields)
                {
                    field.IsDataFormCurrentItemNull = this.IsDataFormCurrentItemNull;
                }
            }
        }

        /// <summary>
        /// Generates the UI for the child fields.
        /// </summary>
        /// <returns>The UI for the child fields.</returns>
        protected override FrameworkElement GenerateElement()
        {
            return this.GenerateUI();
        }

        /// <summary>
        /// Generates the UI for the child fields.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The UI for the child fields.</returns>
        protected override FrameworkElement GenerateEditingElement(bool isReadOnly)
        {
            return this.GenerateUI();
        }

        /// <summary>
        /// Generates the UI for the child fields.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The UI for the child fields.</returns>
        protected override FrameworkElement GenerateInsertElement(bool isReadOnly)
        {
            return this.GenerateUI();
        }

        /// <summary>
        /// Orientation property changed handler.
        /// </summary>
        /// <param name="d">DataFormFieldGroup that changed its Orientation value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormFieldGroup groupField = d as DataFormFieldGroup;
            if (groupField != null)
            {
                if (groupField.OwningForm != null && groupField.OwningForm.UIGenerated)
                {
                    groupField.OwningForm.GenerateUI();
                }
            }
        }

        /// <summary>
        /// WrapAfter property changed handler.
        /// </summary>
        /// <param name="d">DataFormFieldGroup that changed its WrapAfter value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnWrapAfterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormFieldGroup groupField = d as DataFormFieldGroup;
            if (groupField != null)
            {
                if (groupField.OwningForm != null && groupField.OwningForm.UIGenerated)
                {
                    groupField.OwningForm.GenerateUI();
                }
            }
        }

        /// <summary>
        /// Generates the UI for the child fields.
        /// </summary>
        /// <returns>The UI for the child fields.</returns>
        private FrameworkElement GenerateUI()
        {
            if (this.OwningForm != null)
            {
                return this.OwningForm.GeneratePanelUI(this.Mode, this.Fields, this.Orientation, this.WrapAfter);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Attaches a field to this field group.
        /// </summary>
        /// <param name="field">The field to attach.</param>
        private void AttachField(DataFormField field)
        {
            field.OwningForm = this.OwningForm;
            field.IsDataFormCurrentItemNull = this.IsDataFormCurrentItemNull;
        }

        /// <summary>
        /// Detaches a field from this field group.
        /// </summary>
        /// <param name="field">The field to detach.</param>
        private static void DetachField(DataFormField field)
        {
            field.OwningForm = null;
            field.IsDataFormCurrentItemNull = true;
        }

        /// <summary>
        /// Handles the case where the fields collection has changed.
        /// </summary>
        /// <param name="sender">The fields collection.</param>
        /// <param name="e">The event args</param>
        private void OnFieldsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (DataFormField newField in e.NewItems)
                    {
                        this.AttachField(newField);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (DataFormField oldField in e.OldItems)
                    {
                        DetachField(oldField);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (DataFormField oldField in e.OldItems)
                    {
                        DetachField(oldField);
                    }

                    foreach (DataFormField newField in e.NewItems)
                    {
                        this.AttachField(newField);
                    }

                    break;
            }
        }
    }
}
