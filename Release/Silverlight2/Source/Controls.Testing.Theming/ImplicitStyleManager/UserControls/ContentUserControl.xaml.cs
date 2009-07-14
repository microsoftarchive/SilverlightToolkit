// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// A user control with a content property.
    /// </summary>
    public partial class ContentUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ContentUserControl.
        /// </summary>
        public ContentUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds content to the layout root.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.LayoutRoot != null && _content != null)
            {
                this.LayoutRoot.Children.Add(_content);
            }
        }

        /// <summary>
        /// The content of the control.
        /// </summary>
        private UIElement _content;

        /// <summary>
        /// Gets or sets the content property.
        /// </summary>
        public new UIElement Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (value == null && _content != null && this.LayoutRoot != null)
                {
                    this.LayoutRoot.Children.Clear();
                }
                _content = value;
                if (_content != null && this.LayoutRoot != null)
                {
                    this.LayoutRoot.Children.Add(_content);
                }
            }
        }
    }
}
