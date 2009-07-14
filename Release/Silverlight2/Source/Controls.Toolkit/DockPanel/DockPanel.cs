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

namespace System.Windows.Controls
{
    /// <summary>
    /// Arranges child elements around the edges of the panel. Optionally, 
    /// the last added child element can occupy the remaining space.
    /// </summary>
    /// <remarks>
    /// <para>
    /// DockPanel is one of the Panel controls that enables layout. 
    /// DockPanel is used when you want to arrange a set of objects around the 
    /// edges of a panel. You specify where a child element is located in 
    /// the DockPanel with the Dock property. If you set the LastChildFill 
    /// property to true (the default value) and the last element is allowed 
    /// to resize, the last element added to the panel will resize to fill the 
    /// remaining space. If the last element is set to a specific size, 
    /// the last element will be set to the specified size and positioned in
    /// the middle of the remaining space.
    /// </para>
    /// <para>
    /// The order in which elements are added to the DockPanel is important. 
    /// For example, if you add three child elements that have their Dock 
    /// properties set to Left, they will stack from the left side of the 
    /// control with the  last element added the furthest right. If elements 
    /// are added that exceed the space of the DockPanel, the last elements that 
    /// are added that exceed the space of the panel will be truncated.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows a DockPanel with child elements positioned 
    /// at the edges of the panel and the last child filling the remaining 
    /// space.
    /// <code language="XAML">
    /// <![CDATA[
    /// <Border BorderBrush="Black" BorderThickness="3" >
    ///    <StackPanel x:Name="LayoutRoot" Background="White">
    ///        <TextBlock Margin="5" Text="Dock Panel" />
    ///        <Border BorderBrush="Black" BorderThickness="3" >
    ///            <controls:DockPanel LastChildFill="true" Height="265">
    ///                <Button  Content="Dock: Left" controls:DockPanel.Dock ="Left" />
    ///                <Button Content="Dock: Right" controls:DockPanel.Dock ="Right" />
    ///                <Button  Content="Dock: Top" controls:DockPanel.Dock ="Top" />
    ///                <Button  Content="Dock: Bottom" controls:DockPanel.Dock ="Bottom" />
    ///                <Button Content="Last Child" />
    ///            </controls:DockPanel>
    ///        </Border>
    ///    </StackPanel>
    /// </Border>
    /// ]]>
    /// </code>
    /// </example>
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
        /// added to a DockPanel resizes to fill the remaining space.
        /// </summary>
        /// <value>
        /// True if the last element added resizes to fill the remaining space, 
        /// false to indicate the last element does not resize. The default is 
        /// true.
        /// </value>
        /// <remarks>
        /// When the LastChildFill property is set to true and the last 
        /// element is allowed to resize, the last element added will fill the 
        /// remaining space in the panel, regardless of the Dock property value. 
        /// </remarks>
        public bool LastChildFill
        {
            get { return (bool)GetValue(LastChildFillProperty); }
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
        /// Gets the value of the Dock attached property for the specified 
        /// element.
        /// </summary>
        /// <param name="element">
        /// The element from which the property value is read.
        /// </param>
        /// <returns>The Dock property value for the element.</returns>
        /// <remarks>
        /// The GetDock and SetDock methods enable you to get and set the 
        /// value of the Dock property for an element in code.
        /// </remarks>
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
        /// Sets the value of the Dock attached property for the specified 
        /// element to the specified dock value.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is assigned.
        /// </param>
        /// <param name="dock">
        /// The dock value to assign to the specified element.
        /// </param>
        /// <remarks>
        /// The GetDock and SetDock methods enable you to get and set the 
        /// value of the Dock property for an element in code.
        /// </remarks>
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
        /// Measures the child elements of a DockPanel in preparation for 
        /// arranging them during the ArrangeOverride pass.
        /// </summary>
        /// <param name="constraint">
        /// The area available to the DockPanel.
        /// </param>
        /// <returns>The desired size of the DockPanel.</returns>
        /// <remarks>
        /// The MeasureOverride method measures each child element in the 
        /// DockPanel and returns constraint, or the combined size of all of 
        /// the elements in the DockPanel, depending on which is larger. 
        /// </remarks>
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
        /// Arranges the child elements of the DockPanel control.
        /// </summary>
        /// <param name="arrangeSize">
        /// The area in the parent element that the DockPanel should use to 
        /// arrange its child elements.
        /// </param>
        /// <returns>
        /// The actual size of the DockPanel after the child elements are 
        /// arranged. The actual size should always equal arrangeSize.
        /// </returns>
        /// <remarks>
        /// If child elements are added that exceed arrangeSize, 
        /// the elements will be truncated. The ArrangeOverride method is 
        /// called during the second layout pass of the control. 
        /// </remarks>
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