// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#if WINDOWS_PHONE
namespace Microsoft.Phone.Controls
#else
namespace System.Windows.Controls
#endif
{
    /// <summary>
    /// PopupHelper is a simple wrapper type that helps abstract platform
    /// differences out of the Popup.
    /// </summary>
    internal class PopupHelper
    {
        /// <summary>
        /// A value indicating whether Silverlight has loaded at least once, 
        /// so that the wrapping canvas is not recreated.
        /// </summary>
        private bool _hasControlLoaded;

        /// <summary>
        /// The distance from the control to the <see cref="T:Popup"/> child.
        /// </summary>
        private const double PopupOffset = 2;

        /// <summary>
        /// Gets a value indicating whether a visual popup state is being used
        /// in the current template for the Closed state. Setting this value to
        /// true will delay the actual setting of Popup.IsOpen to false until
        /// after the visual state's transition for Closed is complete.
        /// </summary>
        public bool UsesClosingVisualState { get; private set; }

        /// <summary>
        /// Gets or sets the parent control.
        /// </summary>
        private Control Parent { get; set; }

#if SILVERLIGHT
        /// <summary>
        /// Gets or sets the expansive area outside of the popup.
        /// </summary>
        private Canvas OutsidePopupCanvas { get; set; }

        /// <summary>
        /// Gets or sets the canvas for the popup child.
        /// </summary>
        private Canvas PopupChildCanvas { get; set; }
#endif

        /// <summary>
        /// Gets or sets the maximum drop down height value.
        /// </summary>
        public double MaxDropDownHeight { get; set; }

        /// <summary>
        /// Gets the Popup control instance.
        /// </summary>
        public Popup Popup { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the actual Popup is open.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Provided for completeness.")]
        public bool IsOpen
        {
            get { return Popup.IsOpen; }
            set { Popup.IsOpen = value; }
        }

        /// <summary>
        /// Gets or sets the popup child framework element. Can be used if an
        /// assumption is made on the child type.
        /// </summary>
        private FrameworkElement PopupChild { get; set; }

        /// <summary>
        /// The Closed event is fired after the Popup closes.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Fired when the popup children have a focus event change, allows the
        /// parent control to update visual states or react to the focus state.
        /// </summary>
        public event EventHandler FocusChanged;

        /// <summary>
        /// Fired when the popup children intercept an event that may indicate
        /// the need for a visual state update by the parent control.
        /// </summary>
        public event EventHandler UpdateVisualStates;

        /// <summary>
        /// Initializes a new instance of the PopupHelper class.
        /// </summary>
        /// <param name="parent">The parent control.</param>
        public PopupHelper(Control parent)
        {
            Debug.Assert(parent != null, "Parent should not be null.");
            Parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the PopupHelper class.
        /// </summary>
        /// <param name="parent">The parent control.</param>
        /// <param name="popup">The Popup template part.</param>
        public PopupHelper(Control parent, Popup popup)
            : this(parent)
        {
            Popup = popup;
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Gets the <see cref="T:MatrixTransform"/> for the control.
        /// </summary>
        /// <returns>The <see cref="T:MatrixTransform"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This try-catch pattern is used by other popup controls to keep the runtime up.")]
        private MatrixTransform GetControlMatrixTransform()
        {
            // Getting the transform will throw if the popup is no longer in the visual tree.
            // This can happen if you first open the popup and then click on something else on the page that removes it from the live tree.
            try
            {
                return Parent.TransformToVisual(null) as MatrixTransform;
            }
            catch
            {
                OnClosed(EventArgs.Empty); // IsDropDownOpen = false;
                return null;
            }
        }

        /// <summary>
        /// Makes a <see cref="T:Point"/> from a <see cref="T:MatrixTransform"/>.
        /// </summary>
        /// <remarks>
        /// The control's margin is counted with the offset to make the <see cref="T:Point"/>.
        /// This makes the <see cref="T:Point"/> refer to the visible part of the control.
        /// </remarks>
        /// <param name="matrixTransform">The <see cref="T:MatrixTransform"/>.</param>
        /// <param name="margin">The margin.</param>
        /// <returns></returns>
        private static Point MatrixTransformPoint(MatrixTransform matrixTransform, Thickness margin)
        {
            double x = matrixTransform.Matrix.OffsetX + margin.Left;
            double y = matrixTransform.Matrix.OffsetY + margin.Top;
            return new Point(x, y);
        }

        /// <summary>
        /// Gets the <see cref="T:Size"/> of the visible part of the control (minus the margin).
        /// </summary>
        /// <remarks>
        /// The Parent <see cref="T:Size"/> is wrong if the orientation changed, so use the ArrangeOverride <see cref="T:Size"/> if it's available.
        /// </remarks>
        /// <param name="margin">The margin.</param>
        /// <param name="finalSize">The <see cref="M:AutoCompleteBox.ArrangeOverride"/> size.</param>
        /// <returns>The <see cref="T:Size"/>.</returns>
        private Size GetControlSize(Thickness margin, Size? finalSize)
        {
            double width = (finalSize.HasValue ? finalSize.Value.Width : Parent.ActualWidth) - margin.Left - margin.Right;
            double height = (finalSize.HasValue ? finalSize.Value.Height : Parent.ActualHeight) - margin.Top - margin.Bottom;
            return new Size(width, height);
        }

        /// <summary>
        /// Gets the margin for the control.
        /// </summary>
        /// <returns>The margin.</returns>
        private Thickness GetMargin()
        {
            Thickness? thickness = Popup.Resources["PhoneTouchTargetOverhang"] as Thickness?;
            if (thickness.HasValue)
            {
                return thickness.Value;
            }
            return new Thickness(0);
        }

        /// <summary>
        /// Determines whether <see cref="P:Popup.Child"/> is displayed above the control.
        /// </summary>
        /// <param name="displaySize">The <see cref="T:Size"/> not covered by the SIP.</param>
        /// <param name="controlSize">The <see cref="T:Size"/> of the control.</param>
        /// <param name="controlOffset">The position of the control.</param>
        /// <returns></returns>
        private static bool IsChildAbove(Size displaySize, Size controlSize, Point controlOffset)
        {
            double above = controlOffset.Y;
            double below = displaySize.Height - controlSize.Height - above;
            return above > below;
        }

        /// <summary>
        /// Gets the minimum of three numbers and floors it at zero.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        /// <returns>The result.</returns>
        private static double Min0(double x, double y)
        {
            return Math.Max(Math.Min(x, y), 0);
        }

        /// <summary>
        /// Computes the <see cref="T:Size"/> of <see cref="P:Popup.Child"/> if displayed above the control.
        /// </summary>
        /// <param name="controlSize">The <see cref="T:Size"/> of the control.</param>
        /// <param name="controlPoint">The position of the control.</param>
        /// <returns>The <see cref="T:Size"/>.</returns>
        private Size AboveChildSize(Size controlSize, Point controlPoint)
        {
            double width = controlSize.Width;
            double availableHeight = controlPoint.Y - PopupOffset;
            double customHeight = MaxDropDownHeight;
            double height = Min0(availableHeight, customHeight);
            return new Size(width, height);
        }

        /// <summary>
        /// Computes the <see cref="T:Size"/> of <see cref="P:Popup.Child"/> if displayed below the control.
        /// </summary>
        /// <param name="displaySize">The <see cref="T:Size"/> not covered by the SIP.</param>
        /// <param name="controlSize">The <see cref="T:Size"/> of the control.</param>
        /// <param name="controlPoint">The position of the control.</param>
        /// <returns>The <see cref="T:Size"/>.</returns>
        private Size BelowChildSize(Size displaySize, Size controlSize, Point controlPoint)
        {
            double width = controlSize.Width;
            double availableHeight = displaySize.Height - controlSize.Height - controlPoint.Y - PopupOffset;
            double customHeight = MaxDropDownHeight;
            double height = Min0(availableHeight, customHeight);
            return new Size(width, height);
        }

        /// <summary>
        /// The position of <see cref="P:Popup.Child"/> if displayed above the control.
        /// </summary>
        /// <param name="margin">The control's margin.</param>
        /// <returns>The position.</returns>
        private Point AboveChildPoint(Thickness margin)
        {
            double x = margin.Left;
            double y = margin.Top - PopupChild.ActualHeight - PopupOffset;
            return new Point(x, y);
        }

        /// <summary>
        /// The position of <see cref="P:Popup.Child"/> if displayed below the control.
        /// </summary>
        /// <param name="margin">The control's margin.</param>
        /// <param name="controlSize">The <see cref="T:Size"/> of the control.</param>
        /// <returns>The position.</returns>
        private static Point BelowChildPoint(Thickness margin, Size controlSize)
        {
            double x = margin.Left;
            double y = margin.Top + controlSize.Height + PopupOffset;
            return new Point(x, y);
        }

        /// <summary>
        /// Arrange the popup.
        /// <param name="finalSize">The <see cref="T:Size"/> from <see cref="M:AutoCompleteBox.ArrangeOverride"/>.</param>
        /// </summary>
        public void Arrange(Size? finalSize)
        {
            if (Popup == null ||
                PopupChild == null ||
                Application.Current == null ||
                OutsidePopupCanvas == null ||
                Application.Current.Host == null ||
                Application.Current.Host.Content == null)
            {
                return;
            }
            PhoneApplicationFrame frame;
            if (!PhoneHelper.TryGetPhoneApplicationFrame(out frame))
            {
                return;
            }
            Size frameSize = frame.GetUsefulSize();
            Thickness margin = GetMargin();
            Size controlSize = GetControlSize(margin, finalSize);
            MatrixTransform matrixTransform = GetControlMatrixTransform();
            if (matrixTransform == null)
            {
                return;
            }
            Point controlPoint = MatrixTransformPoint(matrixTransform, margin);
            Size displaySize = frame.GetSipUncoveredSize();
            bool isChildAbove = IsChildAbove(displaySize, controlSize, controlPoint);
            Size childSize =
                isChildAbove ?
                AboveChildSize(controlSize, controlPoint) :
                BelowChildSize(displaySize, controlSize, controlPoint);
            if (frameSize.Width == 0 || frameSize.Height == 0 || childSize.Height == 0)
            {
                return;
            }
            Point childPoint = isChildAbove ? AboveChildPoint(margin) : BelowChildPoint(margin, controlSize);
            
            Popup.HorizontalOffset = 0;
            Popup.VerticalOffset = 0;

            PopupChild.Width = PopupChild.MaxWidth = PopupChild.MinWidth = controlSize.Width;
            PopupChild.MinHeight = 0;
            PopupChild.MaxHeight = childSize.Height;
            PopupChild.HorizontalAlignment = HorizontalAlignment.Left;
            PopupChild.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(PopupChild, childPoint.X);
            Canvas.SetTop(PopupChild, childPoint.Y);

            OutsidePopupCanvas.Width = controlSize.Width;
            OutsidePopupCanvas.Height = frameSize.Height;
            Matrix fromFrameToControl = matrixTransform.Matrix;
            Matrix fromControlToFrame;
            fromFrameToControl.Invert(out fromControlToFrame);
            matrixTransform.Matrix = fromControlToFrame;
            OutsidePopupCanvas.RenderTransform = matrixTransform;
        }
#else // WINDOWS_PHONE
        /// <summary>
        /// Arrange the popup.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This try-catch pattern is used by other popup controls to keep the runtime up.")]
        public void Arrange()
        {
            if (Popup == null
                || PopupChild == null || Application.Current == null
#if SILVERLIGHT
                || OutsidePopupCanvas == null || Application.Current.Host == null || Application.Current.Host.Content == null
#endif
                || false)
            {
                return;
            }

#if SILVERLIGHT
            Content hostContent = Application.Current.Host.Content;
            double rootWidth = hostContent.ActualWidth;
            double rootHeight = hostContent.ActualHeight;
#else
            UIElement u = Parent;
            if (Application.Current.Windows.Count > 0)
            {
                // TODO: USE THE CURRENT WINDOW INSTEAD! WALK THE TREE!
                u = Application.Current.Windows[0];
            }
            while ((u as Window) == null && u != null)
            {
                u = VisualTreeHelper.GetParent(u) as UIElement;
            }
            Window w = u as Window;
            if (w == null)
            {
                return;
            }

            double rootWidth = w.ActualWidth;
            double rootHeight = w.ActualHeight;
#endif

            double popupContentWidth = PopupChild.ActualWidth;
            double popupContentHeight = PopupChild.ActualHeight;

            if (rootHeight == 0 || rootWidth == 0 || popupContentWidth == 0 || popupContentHeight == 0)
            {
                return;
            }

            double rootOffsetX = 0;
            double rootOffsetY = 0;

#if SILVERLIGHT
            // Getting the transform will throw if the popup is no longer in 
            // the visual tree.  This can happen if you first open the popup 
            // and then click on something else on the page that removes it 
            // from the live tree.
            MatrixTransform mt = null;
            try
            {
                mt = Parent.TransformToVisual(null) as MatrixTransform;
            }
            catch
            {
                OnClosed(EventArgs.Empty); // IsDropDownOpen = false;
            }
            if (mt == null)
            {
                return;
            }

            rootOffsetX = mt.Matrix.OffsetX;
            rootOffsetY = mt.Matrix.OffsetY;

            if (Parent.FlowDirection == FlowDirection.RightToLeft)
            {
                // In RTL mode the coordinate system is flipped horizontally,
                // right plugin edge corresponds to X==0, so we need to adjust
                // the rootOffsetX to be mirrored too, for the below code to
                // compute popup position correctly
                rootOffsetX = rootWidth - rootOffsetX;
            }
#endif

            double myControlHeight = Parent.ActualHeight;
            double myControlWidth = Parent.ActualWidth;

            // Use or come up with a maximum popup height.
            double popupMaxHeight = MaxDropDownHeight;
            if (double.IsInfinity(popupMaxHeight) || double.IsNaN(popupMaxHeight))
            {
                popupMaxHeight = (rootHeight - myControlHeight) * 3 / 5;
            }

            popupContentWidth = Math.Min(popupContentWidth, rootWidth);
            popupContentHeight = Math.Min(popupContentHeight, popupMaxHeight);
            popupContentWidth = Math.Max(myControlWidth, popupContentWidth);

            // We prefer to align the popup box with the left edge of the 
            // control, if it will fit.
            double popupX = rootOffsetX;
            if (rootWidth < popupX + popupContentWidth)
            {
                // Since it doesn't fit when strictly left aligned, we shift it 
                // to the left until it does fit.
                popupX = rootWidth - popupContentWidth;
                popupX = Math.Max(0, popupX);
            }

            // We prefer to put the popup below the combobox if it will fit.
            bool below = true;
            double popupY = rootOffsetY + myControlHeight;
            if (rootHeight < popupY + popupContentHeight)
            {
                below = false;
                // It doesn't fit below the combobox, lets try putting it above 
                // the combobox.
                popupY = rootOffsetY - popupContentHeight;
                if (popupY < 0)
                {
                    // doesn't really fit below either.  Now we just pick top 
                    // or bottom based on wich area is bigger.
                    if (rootOffsetY < (rootHeight - myControlHeight) / 2)
                    {
                        below = true;
                        popupY = rootOffsetY + myControlHeight;
                    }
                    else
                    {
                        below = false;
                        popupY = rootOffsetY - popupContentHeight;
                    }
                }
            }

            // Now that we have positioned the popup we may need to truncate 
            // its size.
            popupMaxHeight = below ? Math.Min(rootHeight - popupY, popupMaxHeight) : Math.Min(rootOffsetY, popupMaxHeight);

            Popup.HorizontalOffset = 0;
            Popup.VerticalOffset = 0;

#if SILVERLIGHT
            OutsidePopupCanvas.Width = rootWidth;
            OutsidePopupCanvas.Height = rootHeight;

            // Transform the transparent canvas to the plugin's coordinate 
            // space origin.
            Matrix transformToRootMatrix = mt.Matrix;
            Matrix newMatrix;
            transformToRootMatrix.Invert(out newMatrix);
            mt.Matrix = newMatrix;

            OutsidePopupCanvas.RenderTransform = mt;
#endif
            PopupChild.MinWidth = myControlWidth;
            PopupChild.MaxWidth = rootWidth;
            PopupChild.MinHeight = 0;
            PopupChild.MaxHeight = Math.Max(0, popupMaxHeight);

            PopupChild.Width = popupContentWidth;
            // PopupChild.Height = popupContentHeight;
            PopupChild.HorizontalAlignment = HorizontalAlignment.Left;
            PopupChild.VerticalAlignment = VerticalAlignment.Top;

            // Set the top left corner for the actual drop down.
            Canvas.SetLeft(PopupChild, popupX - rootOffsetX);
            Canvas.SetTop(PopupChild, popupY - rootOffsetY);
        }
#endif // WINDOWS_PHONE

        /// <summary>
        /// Fires the Closed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        private void OnClosed(EventArgs e)
        {
            EventHandler handler = Closed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Actually closes the popup after the VSM state animation completes.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPopupClosedStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            // Delayed closing of the popup until now
            if (e != null && e.NewState != null && e.NewState.Name == VisualStates.StatePopupClosed)
            {
                if (Popup != null)
                {
                    Popup.IsOpen = false;
                }
                OnClosed(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Should be called by the parent control before the base
        /// OnApplyTemplate method is called.
        /// </summary>
        public void BeforeOnApplyTemplate()
        {
            if (UsesClosingVisualState)
            {
                // Unhook the event handler for the popup closed visual state group.
                // This code is used to enable visual state transitions before 
                // actually setting the underlying Popup.IsOpen property to false.
                VisualStateGroup groupPopupClosed = VisualStates.TryGetVisualStateGroup(Parent, VisualStates.GroupPopup);
                if (null != groupPopupClosed)
                {
                    groupPopupClosed.CurrentStateChanged -= OnPopupClosedStateChanged;
                    UsesClosingVisualState = false;
                }
            }

            if (Popup != null)
            {
                Popup.Closed -= Popup_Closed;
            }
        }

        /// <summary>
        /// Should be called by the parent control after the base
        /// OnApplyTemplate method is called.
        /// </summary>
        public void AfterOnApplyTemplate()
        {
            if (Popup != null)
            {
                Popup.Closed += Popup_Closed;
            }

            VisualStateGroup groupPopupClosed = VisualStates.TryGetVisualStateGroup(Parent, VisualStates.GroupPopup);
            if (null != groupPopupClosed)
            {
                groupPopupClosed.CurrentStateChanged += OnPopupClosedStateChanged;
                UsesClosingVisualState = true;
            }

            // TODO: Consider moving to the DropDownPopup setter
            // TODO: Although in line with other implementations, what happens 
            // when the template is swapped out?
            if (Popup != null)
            {
                PopupChild = Popup.Child as FrameworkElement;

                // For Silverlight only, we just create the popup child with 
                // canvas a single time.
                if (PopupChild != null && !_hasControlLoaded)
                {
#if SILVERLIGHT
                    _hasControlLoaded = true;

                    // Replace the popup child with a canvas
                    PopupChildCanvas = new Canvas();
                    Popup.Child = PopupChildCanvas;

                    OutsidePopupCanvas = new Canvas();
                    OutsidePopupCanvas.Background = new SolidColorBrush(Colors.Transparent);
                    OutsidePopupCanvas.MouseLeftButtonDown += OutsidePopup_MouseLeftButtonDown;

                    PopupChildCanvas.Children.Add(OutsidePopupCanvas);
                    PopupChildCanvas.Children.Add(PopupChild);
#endif

                    PopupChild.GotFocus += PopupChild_GotFocus;
                    PopupChild.LostFocus += PopupChild_LostFocus;
                    PopupChild.MouseEnter += PopupChild_MouseEnter;
                    PopupChild.MouseLeave += PopupChild_MouseLeave;
                    PopupChild.SizeChanged += PopupChild_SizeChanged;
                }
            }
        }

        /// <summary>
        /// The size of the popup child has changed.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Arrange(null);
        }

        /// <summary>
        /// The mouse has clicked outside of the popup.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OutsidePopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Popup != null)
            {
                Popup.IsOpen = false;
            }
        }

        /// <summary>
        /// Connected to the Popup Closed event and fires the Closed event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void Popup_Closed(object sender, EventArgs e)
        {
            OnClosed(EventArgs.Empty);
        }

        /// <summary>
        /// Connected to several events that indicate that the FocusChanged 
        /// event should bubble up to the parent control.
        /// </summary>
        /// <param name="e">The event data.</param>
        private void OnFocusChanged(EventArgs e)
        {
            EventHandler handler = FocusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the UpdateVisualStates event.
        /// </summary>
        /// <param name="e">The event data.</param>
        private void OnUpdateVisualStates(EventArgs e)
        {
            EventHandler handler = UpdateVisualStates;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The popup child has received focus.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_GotFocus(object sender, RoutedEventArgs e)
        {
            OnFocusChanged(EventArgs.Empty);
        }

        /// <summary>
        /// The popup child has lost focus.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_LostFocus(object sender, RoutedEventArgs e)
        {
            OnFocusChanged(EventArgs.Empty);
        }

        /// <summary>
        /// The popup child has had the mouse enter its bounds.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_MouseEnter(object sender, MouseEventArgs e)
        {
            OnUpdateVisualStates(EventArgs.Empty);
        }

        /// <summary>
        /// The mouse has left the popup child's bounds.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_MouseLeave(object sender, MouseEventArgs e)
        {
            OnUpdateVisualStates(EventArgs.Empty);
        }
    }
}
