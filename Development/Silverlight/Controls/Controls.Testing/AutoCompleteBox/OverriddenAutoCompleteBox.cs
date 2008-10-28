// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// A derived AutoCompleteBox class that exposes template parts to enable 
    /// testing.
    /// </summary>
    public class OverriddenAutoCompleteBox : AutoCompleteBox
    {
        /// <summary>
        /// Gets or sets the TextBox template part.
        /// </summary>
        public TextBox TextBox { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButton template part.
        /// </summary>
        public ToggleButton DropDownToggle { get; set; }

        /// <summary>
        /// Initializes a new instance of the OverriddenAutoCompleteBox type.
        /// </summary>
        public OverriddenAutoCompleteBox()
        {
        }

        /// <summary>
        /// Overrides the OnApplyTemplate method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TextBox = GetTemplateChild("Text") as TextBox;
            DropDownToggle = GetTemplateChild("DropDownToggle") as ToggleButton;
        }

        /// <summary>
        /// Overrides the SelectionAdapter template part get method to inject 
        /// the OverriddenSelectionAdapter type for Selector objects.
        /// </summary>
        /// <param name="value">The template part.</param>
        /// <returns>Returns a new ISelectionAdapter instance or null.</returns>
        protected override ISelectionAdapter TryGetSelectionAdapter(object value)
        {
            Selector selector = value as Selector;
            return selector != null ? new OverriddenSelectionAdapter(selector) : base.TryGetSelectionAdapter(value);
        }
    }
}