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
    /// Template field for the DataForm.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormTemplateField : DataFormLabeledField
    {
        /// <summary>
        /// Identifies the DisplayTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayTemplateProperty =
            DependencyProperty.Register(
                "DisplayTemplate",
                typeof(DataTemplate),
                typeof(DataFormTemplateField),
                null);

        /// <summary>
        /// Identifies the EditTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty EditTemplateProperty =
            DependencyProperty.Register(
                "EditTemplate",
                typeof(DataTemplate),
                typeof(DataFormTemplateField),
                null);

        /// <summary>
        /// Identifies the InsertTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty InsertTemplateProperty =
            DependencyProperty.Register(
                "InsertTemplate",
                typeof(DataTemplate),
                typeof(DataFormTemplateField),
                null);

        /// <summary>
        /// Gets or sets the display template.
        /// </summary>
        public DataTemplate DisplayTemplate
        {
            get
            {
                return this.GetValue(DisplayTemplateProperty) as DataTemplate;
            }

            set
            {
                if (value != this.DisplayTemplate)
                {
                    this.SetValue(DisplayTemplateProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the edit template.
        /// </summary>
        public DataTemplate EditTemplate
        {
            get
            {
                return this.GetValue(EditTemplateProperty) as DataTemplate;
            }

            set
            {
                if (value != this.EditTemplate)
                {
                    this.SetValue(EditTemplateProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the insert template.
        /// </summary>
        public DataTemplate InsertTemplate
        {
            get
            {
                return this.GetValue(InsertTemplateProperty) as DataTemplate;
            }

            set
            {
                if (value != this.InsertTemplate)
                {
                    this.SetValue(InsertTemplateProperty, value);
                }
            }
        }

        /// <summary>
        /// Generates the display input control.
        /// </summary>
        /// <returns>The display input control.</returns>
        protected override FrameworkElement GenerateElement()
        {
            return this.GenerateTemplateContent(DataFormMode.Display);
        }

        /// <summary>
        /// Generates the edit input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The edit input control.</returns>
        protected override FrameworkElement GenerateEditingElement(bool isReadOnly)
        {
            return this.GenerateTemplateContent(DataFormMode.Edit);
        }

        /// <summary>
        /// Generates the insert input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The insert input control.</returns>
        protected override FrameworkElement GenerateInsertElement(bool isReadOnly)
        {
            return this.GenerateTemplateContent(DataFormMode.Insert);
        }

        /// <summary>
        /// Generates the template content for a given mode.
        /// </summary>
        /// <param name="mode">The mode to be used.</param>
        /// <returns>The template content</returns>
        private FrameworkElement GenerateTemplateContent(DataFormMode mode)
        {
            FrameworkElement frameworkElement = this.GetTemplateContent(mode);

            if (frameworkElement != null)
            {
                frameworkElement.VerticalAlignment = VerticalAlignment.Center;

                if (this.OwningForm != null)
                {
                    this.OwningForm.UpdateBindingsOnElement(frameworkElement);
                }
            }

            return frameworkElement;
        }

        /// <summary>
        /// Applies the template fallbacks.
        /// </summary>
        /// <param name="mode">The mode to be used.</param>
        /// <returns>The template content for the mode or the appropriate fallback template.</returns>
        private FrameworkElement GetTemplateContent(DataFormMode mode)
        {
            DataTemplate dataTemplate = null;

            if (mode == DataFormMode.Display)
            {
                if (this.DisplayTemplate != null)
                {
                    dataTemplate = this.DisplayTemplate;
                }
                else if (this.EditTemplate != null)
                {
                    dataTemplate = this.EditTemplate;
                }
                else if (this.InsertTemplate != null)
                {
                    dataTemplate = this.InsertTemplate;
                }
            }
            else if (mode == DataFormMode.Edit)
            {
                if (this.EditTemplate != null)
                {
                    dataTemplate = this.EditTemplate;
                }
                else if (this.InsertTemplate != null)
                {
                    dataTemplate = this.InsertTemplate;
                }
                else if (this.DisplayTemplate != null)
                {
                    dataTemplate = this.DisplayTemplate;
                }
            }
            else if (mode == DataFormMode.Insert)
            {
                if (this.InsertTemplate != null)
                {
                    dataTemplate = this.InsertTemplate;
                }
                else if (this.EditTemplate != null)
                {
                    dataTemplate = this.EditTemplate;
                }
                else if (this.DisplayTemplate != null)
                {
                    dataTemplate = this.DisplayTemplate;
                }
            }

            if (dataTemplate != null)
            {
                return dataTemplate.LoadContent() as FrameworkElement;
            }

            return null;
        }
    }
}
