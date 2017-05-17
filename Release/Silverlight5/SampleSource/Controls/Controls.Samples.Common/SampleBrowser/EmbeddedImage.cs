// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// ImageLoader is used to load an Embedded Resource Image.
    /// </summary>
    [TemplatePart(Name = EmbeddedImage.ImageLoaderName, Type = typeof(Image))]
    public class EmbeddedImage : Control
    {
        /// <summary>
        /// Name of the Image part.
        /// </summary>
        private const string ImageLoaderName = "Image";

        /// <summary>
        /// Name of the resource to be loaded.
        /// </summary>
        private string _resourceName;

        /// <summary>
        /// Initializes a new instance of the ImageLoader class.
        /// </summary>
        public EmbeddedImage()
        {
            DefaultStyleKey = typeof(EmbeddedImage);
        }

        /// <summary>
        /// Retrieve select elements from a control template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetEmbeddedImage(_resourceName);
        }

        /// <summary>
        /// Gets or sets the name of the image to be loaded.
        /// </summary>
        public string ResourceName
        {
            get
            {
                return _resourceName;
            }
            set
            {
                 _resourceName = value;
                 SetEmbeddedImage(_resourceName);
            }
        }

        /// <summary>
        /// Helper function to set the embedded image source.
        /// </summary>
        /// <param name="resourceName">Resource name to be loaded.</param>
        private void SetEmbeddedImage(string resourceName)
        {
            Image image = GetTemplateChild(ImageLoaderName) as Image;
            Image embeddedImage = SharedResources.GetIcon(resourceName);
            if (image != null & embeddedImage != null)
            {
                image.Source = embeddedImage.Source;
            }
        }
    }
}