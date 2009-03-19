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
    using System.Windows.Markup;

    /// <summary>
    /// Header for the DataForm.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [ContentProperty("Content")]
    public class DataFormHeader : DataFormField
    {
        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(DataFormHeader),
                new PropertyMetadata(OnContentPropertyChanged));

        private ContentPresenter _contentPresenter;
        private ContentPresenter _lastContentPresenter;

        /// <summary>
        /// Gets or sets the content of the header.
        /// </summary>
        public object Content
        {
            get
            {
                return this.GetValue(ContentProperty);
            }

            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Generates the display input control.
        /// </summary>
        /// <returns>The display input control.</returns>
        protected override FrameworkElement GenerateElement()
        {
            return this.GenerateContentPresenter();
        }

        /// <summary>
        /// Generates the edit input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The edit input control.</returns>
        protected override FrameworkElement GenerateEditingElement(bool isReadOnly)
        {
            return this.GenerateContentPresenter();
        }

        /// <summary>
        /// Generates the insert input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The insert input control.</returns>
        protected override FrameworkElement GenerateInsertElement(bool isReadOnly)
        {
            return this.GenerateContentPresenter();
        }

        /// <summary>
        /// Content property changed handler.
        /// </summary>
        /// <param name="d">DataFormHeader that changed its Content value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormHeader header = d as DataFormHeader;
            if (header != null && !header.AreHandlersSuspended())
            {
                if (header._contentPresenter != null)
                {
                    header._contentPresenter.Content = header.Content;
                }
            }
        }
        
        /// <summary>
        /// Generates a content presenter.
        /// </summary>
        /// <returns>The content presenter.</returns>
        private ContentPresenter GenerateContentPresenter()
        {
            if (this.ShouldGenerateNewElement)
            {
                this._contentPresenter = new ContentPresenter();
            }

            // An exception gets thrown if the content is not removed from the last content presenter.
            if (this._lastContentPresenter != null)
            {
                this._lastContentPresenter.Content = null;
            }

            this._contentPresenter.Content = this.Content;

            if (this.ShouldGenerateNewElement)
            {
                this._lastContentPresenter = this._contentPresenter;
            }

            this._contentPresenter.Margin = new Thickness(0, 4, 0, 4);
            return this._contentPresenter;
        }
    }
}
