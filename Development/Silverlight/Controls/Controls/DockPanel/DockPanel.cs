// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Defines an area where you can arrange child elements either horizontally
    /// or vertically, relative to each other.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class DockPanel : Panel
    {
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private static bool _ignorePropertyChange;

        #region public bool LastChildFill
        /// <summary>
        /// Gets or sets a value indicating whether the last child element
        /// within a DockPanel stretches to fill the remaining available space.
        /// </summary>
        public bool LastChildFill
        {
            get { return (bool) GetValue(LastChildFillProperty); }
            set { SetValue(LastChildFillProperty, value); }
        }

        /// <summary>
        /// Identifies the LastChildFill dependency property.
        /// </summary>
        public static readonly DependencyProperty LastChildFillProperty =
            DependencyProperty.Register(
                "LastChildFill",
                typeof(bool),
                typeof(DockPanel),
                new PropertyMetadata(true, OnLastChildFillPropertyChanged));

        /// <summary>
        /// LastChildFillProperty property changed handler.
        /// </summary>
        /// <param name="d">DockPanel that changed its LastChildFill.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnLastChildFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockPanel source = d as DockPanel;
            source.InvalidateArrange();
        }
        #endregion public bool LastChildFill

        #region public attached Dock Dock
        /// <summary>
        /// Gets the value of the Dock attached property for a specified
        /// UIElement.
        /// </summary>
        /// <param name="element">
        /// The element from which the property value is read.
        /// </param>
        /// <returns>The Dock property value for the element.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "DockPanel only has UIElement children")]
        public static Dock GetDock(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (Dock)element.GetValue(DockProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="dock">The needed Dock value.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "DockPanel only has UIElement children")]
        public static void SetDock(UIElement element, Dock dock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(DockProperty, dock);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty DockProperty =
            DependencyProperty.RegisterAttached(
                "Dock",
                typeof(Dock),
                typeof(DockPanel),
                new PropertyMetadata(Dock.Left, OnDockPropertyChanged));

        /// <summary>
        /// DockProperty property changed handler.
        /// </summary>
        /// <param name="d">UIElement that changed its Dock.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Almost always set from the attached property CLR setter.")]
        private static void OnDockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Ignore the change if requested
            if (_ignorePropertyChange)
            {
                _ignorePropertyChange = false;
                return;
            }

            UIElement element = (UIElement)d;
            Dock value = (Dock)e.NewValue;

            // Validate the Dock property
            if ((value != Dock.Left) &&
                (value != Dock.Top) &&
                (value != Dock.Right) &&
                (value != Dock.Bottom))
            {
                // Reset the property to its original state before throwing
                _ignorePropertyChange = true;
                element.SetValue(DockProperty, (Dock)e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.DockPanel_OnDockPropertyChanged_InvalidValue,
                    value);
                throw new ArgumentException(message, "value");
            }

            // Cause the DockPanel to update its layout when a child changes
            DockPanel panel = VisualTreeHelper.GetParent(element) as DockPanel;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }
        #endregion public attached Dock Dock

        /// <summary>
        /// Initializes a new instance of the DockPanel class.
        /// </summary>
        public DockPanel()
        {
        }

        /// <summary>
        /// Measures the children of a DockPanel in anticipation of arranging
        /// them during the ArrangeOverride pass.
        /// </summary>
        /// <param name="constraint">A maximum Size to not exceed.</param>
        /// <returns>The desired size of the DockPanel.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        protected override Size MeasureOverride(Size constraint)
        {
            double usedWidth = 0.0;
            double usedHeight = 0.0;
            double maximumWidth = 0.0;
            double maximumHeight = 0.0;

            // Measure each of the Children
            foreach (UIElement element in Children)
            {
                // Get the child's desired size
                Size remainingSize = new Size(
                    Math.Max(0.0, constraint.Width - usedWidth),
                    Math.Max(0.0, constraint.Height - usedHeight));
                element.Measure(remainingSize);
                Size desiredSize = element.DesiredSize;

                // Decrease the remaining space for the rest of the children
                switch (GetDock(element))
                {
                    case Dock.Left:
                    case Dock.Right:
                        maximumHeight = Math.Max(maximumHeight, usedHeight + desiredSize.Height);
                        usedWidth += desiredSize.Width;
                        break;
                    case Dock.Top:
                    case Dock.Bottom:
                        maximumWidth = Math.Max(maximumWidth, usedWidth + desiredSize.Width);
                        usedHeight += desiredSize.Height;
                        break;
                }
            }

            maximumWidth = Math.Max(maximumWidth, usedWidth);
            maximumHeight = Math.Max(maximumHeight, usedHeight);
            return new Size(maximumWidth, maximumHeight);
        }

        /// <summary>
        /// Arranges the content (child elements) of a DockPanel element.
        /// </summary>
        /// <param name="arrangeSize">
        /// The Size the DockPanel uses to arrange its child elements.
        /// </param>
        /// <returns>The arranged size of the DockPanel.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double left = 0.0;
            double top = 0.0;
            double right = 0.0;
            double bottom = 0.0;

            // Arrange each of the Children
            UIElementCollection children = Children;
            int dockedCount = children.Count - (LastChildFill ? 1 : 0);
            int index = 0;
            foreach (UIElement element in children)
            {
                // Determine the remaining space left to arrange the element
                Rect remainingRect = new Rect(
                    left,
                    top,
                    Math.Max(0.0, arrangeSize.Width - left - right),
                    Math.Max(0.0, arrangeSize.Height - top - bottom));
                
                // Trim the remaining Rect to the docked size of the element
                // (unless the element should fill the remaining space because
                // of LastChildFill)
                if (index < dockedCount)
                {
                    Size desiredSize = element.DesiredSize;
                    switch (GetDock(element))
                    {
                        case Dock.Left:
                            left += desiredSize.Width;
                            remainingRect.Width = desiredSize.Width;
                            break;
                        case Dock.Top:
                            top += desiredSize.Height;
                            remainingRect.Height = desiredSize.Height;
                            break;
                        case Dock.Right:
                            right += desiredSize.Width;
                            remainingRect.X = Math.Max(0.0, arrangeSize.Width - right);
                            remainingRect.Width = desiredSize.Width;
                            break;
                        case Dock.Bottom:
                            bottom += desiredSize.Height;
                            remainingRect.Y = Math.Max(0.0, arrangeSize.Height - bottom);
                            remainingRect.Height = desiredSize.Height;
                            break;
                    }
                }

                element.Arrange(remainingRect);
                index++;
            }

            return arrangeSize;
        }
    }
}