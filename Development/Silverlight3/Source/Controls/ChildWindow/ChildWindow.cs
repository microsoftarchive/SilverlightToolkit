// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace System.Windows.Controls
{
    /// <summary>
    /// Child window control allows having interaction with a certain part of a screen. 
    /// It can be used to display blocking/modal dialogs with any content.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = PART_Chrome, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ContentRoot, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Overlay, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_Root, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VSMSTATE_StateClosing, GroupName = VSMGROUP_Window)]
    [TemplateVisualState(Name = VSMSTATE_StateOpen, GroupName = VSMGROUP_Window)]
    [TemplateVisualState(Name = VSMSTATE_StateOpening, GroupName = VSMGROUP_Window)]
    public class ChildWindow : ContentControl
    {
        #region Static Fields and Constants

        /// <summary>
        /// The name of the Chrome template part.
        /// </summary>
        private const string PART_Chrome = "Chrome";

        /// <summary>
        /// The name of the CloseButton template part.
        /// </summary>
        private const string PART_CloseButton = "CloseButton";

        /// <summary>
        /// The name of the ContentRoot template part.
        /// </summary>
        private const string PART_ContentRoot = "ContentRoot";

        /// <summary>
        /// The name of the Overlay template part.
        /// </summary>
        private const string PART_Overlay = "Overlay";

        /// <summary>
        /// The name of the Root template part.
        /// </summary>
        private const string PART_Root = "Root";

        /// <summary>
        /// The name of the WindowStates VSM group.
        /// </summary>
        private const string VSMGROUP_Window = "WindowStates";

        /// <summary>
        /// The name of the Closing VSM state.
        /// </summary>
        private const string VSMSTATE_StateClosing = "Closing";

        /// <summary>
        /// The name of the Open VSM state.
        /// </summary>
        private const string VSMSTATE_StateOpen = "Open";

        /// <summary>
        /// The name of the Opening VSM state.
        /// </summary>
        private const string VSMSTATE_StateOpening = "Opening";

        /// <summary>
        /// Title of the ChildWindow.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
            "Title",
            typeof(object),
            typeof(ChildWindow),
            null);

        /// <summary>
        /// Gets the root visual element.
        /// </summary>
        private static Control RootVisual
        {
            get
            {
                return Application.Current == null ? null : (Application.Current.RootVisual as Control);
            }
        }

        #endregion Static Fields and Constants

        #region Member Fields

        /// <summary>
        /// Private accessor for the Chrome.
        /// </summary>
        private FrameworkElement _chrome;

        /// <summary>
        /// Private accessor for the click point on the chrome.
        /// </summary>
        private Point _clickPoint;

        /// <summary>
        /// Private accessor for the close button.
        /// </summary>
        private ButtonBase _closeButton;

        /// <summary>
        /// Private accessor for the Closing storyboard.
        /// </summary>
        private Storyboard _closed;

        /// <summary>
        /// Content area desired width.
        /// </summary>
        private double _desiredContentWidth;

        /// <summary>
        /// Content area desired height.
        /// </summary>
        private double _desiredContentHeight;

        /// <summary>
        /// Private accessor for the Dialog Result property.
        /// </summary>
        private bool? _dialogresult;

        /// <summary>
        /// Boolean value that specifies whether the window is in closing state or not.
        /// </summary>
        private bool _isClosing;

        /// <summary>
        /// Boolean value that specifies whether the application is exit or not.
        /// </summary>
        private bool _isAppExit;

        /// <summary>
        /// Boolean value that specifies whether the window is in opening state or not.
        /// </summary>
        private bool _isOpening;

        /// <summary>
        /// Private accessor for the Opening storyboard.
        /// </summary>
        private Storyboard _opened;

        /// <summary>
        /// Private accessor for the Overlay of the window.
        /// </summary>
        private FrameworkElement _overlay;

        /// <summary>
        /// Boolean value that specifies whether the mouse is captured or not.
        /// </summary>
        private bool _isMouseCaptured;

        /// <summary>
        /// Private accessor for the Root of the window.
        /// </summary>
        private FrameworkElement _root;

        #endregion Member Fields

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ChildWindow()
        {
            this.DefaultStyleKey = typeof(ChildWindow);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when the window is closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs directly after Close is called, and can be handled to cancel window closure. 
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the internal accessor for the ContentRoot of the window.
        /// </summary>
        internal FrameworkElement ContentRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the DialogResult property.
        /// </summary>
        public bool? DialogResult
        {
            get
            {
                return this._dialogresult;
            }

            set
            {
                if (this._dialogresult != value)
                {
                    this._dialogresult = value;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Gets the internal accessor for the PopUp of the window.
        /// </summary>
        internal Popup ChildWindowPopup
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the Title property.
        /// </summary>
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the PopUp is open or not.
        /// </summary>
        private bool IsOpen
        {
            get
            {
                return (this.ChildWindowPopup != null && this.ChildWindowPopup.IsOpen);
            }
        }

        #endregion Properties

        #region Static Methods

        /// <summary>
        /// Inverts the input matrix.
        /// </summary>
        /// <param name="matrix">The matrix values that is to be inverted.</param>
        /// <returns>Returns a value indicating whether the inversion was successful or not.</returns>
        private static bool InvertMatrix(ref Matrix matrix)
        {
            double determinant = (matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21);

            if (determinant == 0.0)
            {
                return false;
            }

            Matrix matCopy = matrix;
            matrix.M11 = matCopy.M22 / determinant;
            matrix.M12 = -1 * matCopy.M12 / determinant;
            matrix.M21 = -1 * matCopy.M21 / determinant;
            matrix.M22 = matCopy.M11 / determinant;
            matrix.OffsetX = ((matCopy.OffsetY * matCopy.M21) - (matCopy.OffsetX * matCopy.M22)) / determinant;
            matrix.OffsetY = ((matCopy.OffsetX * matCopy.M12) - (matCopy.OffsetY * matCopy.M11)) / determinant;

            return true;
        }

        #endregion Static Methods

        #region Methods

        /// <summary>
        /// Executed when the application is exited.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event args.</param>
        internal void Application_Exit(object sender, EventArgs e)
        {
            if (this.IsOpen)
            {
                this._isAppExit = true;
                try
                {
                    this.Close();
                }
                finally
                {
                    this._isAppExit = false;
                }
            }
        }

        /// <summary>
        /// Changes the visual state of the ChildWindow.
        /// </summary>
        private void ChangeVisualState()
        {
            if (this._isClosing)
            {
                VisualStateManager.GoToState(this, VSMSTATE_StateClosing, false);
            }
            else
            {
                if (this._isOpening)
                {
                    VisualStateManager.GoToState(this, VSMSTATE_StateOpening, false);
                }
                else
                {
                    VisualStateManager.GoToState(this, VSMSTATE_StateOpen, false);
                }
            }
        }

        /// <summary>
        /// Executed when ChildWindow size is changed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Size changed event args.</param>
        private void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this._overlay != null)
            {
                if (e.NewSize.Height != this._overlay.Height)
                {
                    this._desiredContentHeight = e.NewSize.Height;
                }

                if (e.NewSize.Width != this._overlay.Width)
                {
                    this._desiredContentWidth = e.NewSize.Width;
                }
            }

            if (this.IsOpen)
            {
                this.UpdateOverlaySize();
            }
        }

        /// <summary>
        /// Closes a Window. 
        /// </summary>
        public void Close()
        {
            CancelEventArgs e = new CancelEventArgs();

            this.OnClosing(e);

            // On ApplicationExit, close() cannot be cancelled
            if (!e.Cancel || this._isAppExit)
            {
                if (RootVisual != null)
                {
                    RootVisual.IsEnabled = true;
                }

                // Close Popup
                if (this.IsOpen)
                {
                    if (this._closed != null)
                    {
                        // Popup will be closed when the storyboard ends
                        this._isClosing = true;
                        try
                        {
                            this.ChangeVisualState();
                        }
                        finally
                        {
                            this._isClosing = false;
                        }
                    }
                    else
                    {
                        // If no closing storyboard is defined, close the Popup
                        this.ChildWindowPopup.IsOpen = false;
                    }

                    if (!this._dialogresult.HasValue)
                    {
                        // If close action is not happening because of DialogResult property change action,
                        // Dialogresult is always false:
                        this._dialogresult = false;
                    }

                    this.OnClosed(EventArgs.Empty);
                    this.UnSubscribeFromEvents();
                    this.UnsubscribeFromTemplatePartEvents();
                    
                    if (Application.Current.RootVisual != null)
                    {
                        Application.Current.RootVisual.GotFocus -= new RoutedEventHandler(this.RootVisual_GotFocus);
                    }
                }
            }
            else
            {
                // If the Close is cancelled, DialogResult should always be NULL:
                this._dialogresult = null;
            }
        }

        /// <summary>
        /// Executed when the CloseButton is clicked.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        internal void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Executed when the Closing storyboard ends.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Closing_Completed(object sender, EventArgs e)
        {
            if (this.ChildWindowPopup != null)
            {
                this.ChildWindowPopup.IsOpen = false;
            }

            if (this._closed != null)
            {
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
            }
        }

        /// <summary>
        /// Executed when the a key is presses when the window is open.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Key event args.</param>
        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            ChildWindow ew = sender as ChildWindow;
            Debug.Assert(ew != null, "ChildWindow instance is null.");

            // Ctrl+Shift+F4 closes the ChildWindow
            if (e != null && !e.Handled && e.Key == Key.F4 &&
                ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) &&
                ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
            {
                ew.Close();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Executed when the window loses focus.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        private void ChildWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            // If the ChildWindow loses focus but the popup is still open,
            // it means another popup is opened. To get the focus back when the
            // popup is closed, we handle GotFocus on the RootVisual
            // TODO: Something else could get focus and handle the GotFocus event right.  
            // Try listening to routed events that were Handled (new SL 3 feature)
            if (this.IsOpen && Application.Current != null && Application.Current.RootVisual != null)
            {
                Application.Current.RootVisual.GotFocus += new RoutedEventHandler(this.RootVisual_GotFocus);
            }
        }

        /// <summary>
        /// Executed when mouse left button is down on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse button event args.</param>
        private void Chrome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this._chrome != null)
            {
                this._chrome.CaptureMouse();
                this._isMouseCaptured = true;
                this._clickPoint = e.GetPosition(sender as UIElement);
            }
        }

        /// <summary>
        /// Executed when mouse left button is up on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse button event args.</param>
        private void Chrome_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this._chrome != null)
            {
                this._chrome.ReleaseMouseCapture();
                this._isMouseCaptured = false;
            }
        }

        /// <summary>
        /// Executed when mouse moves on the chrome.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse event args.</param>
        private void Chrome_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._isMouseCaptured && this.ContentRoot != null)
            {
                // If the child window is dragged out of the page, return
                if (Application.Current != null && Application.Current.RootVisual != null &&
                    (e.GetPosition(Application.Current.RootVisual).X < 0 || e.GetPosition(Application.Current.RootVisual).Y < 0))
                {
                    return;
                }

                TransformGroup transformGroup = this.ContentRoot.RenderTransform as TransformGroup;

                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(this.ContentRoot.RenderTransform);
                }

                TranslateTransform t = new TranslateTransform();
                t.X = e.GetPosition(this.ContentRoot).X - this._clickPoint.X;
                t.Y = e.GetPosition(this.ContentRoot).Y - this._clickPoint.Y;
                if (transformGroup != null)
                {
                    transformGroup.Children.Add(t);
                    this.ContentRoot.RenderTransform = transformGroup;
                }
            }
        }

        /// <summary>
        /// When the template is applied, this loads all the template parts.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "No need to split the code into two parts.")]
        public override void OnApplyTemplate()
        {
            this.UnsubscribeFromTemplatePartEvents();

            base.OnApplyTemplate();

            this._closeButton = GetTemplateChild(PART_CloseButton) as ButtonBase;

            if (this._closed != null)
            {
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
            }

            if (this._opened != null)
            {
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
            }

            this._root = GetTemplateChild(PART_Root) as FrameworkElement;

            if (this._root != null)
            {
                Collection<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(this._root) as Collection<VisualStateGroup>;

                if (groups != null)
                {
                    System.Collections.IList states = (from stategroup in groups
                                                       where stategroup.Name == ChildWindow.VSMGROUP_Window
                                                       select stategroup.States).FirstOrDefault();
                    Collection<VisualState> statesCol = states as Collection<VisualState>;

                    if (statesCol != null)
                    {
                        this._closed = (from state in statesCol
                                        where state.Name == ChildWindow.VSMSTATE_StateClosing
                                        select state.Storyboard).FirstOrDefault();

                        this._opened = (from state in statesCol
                                        where state.Name == ChildWindow.VSMSTATE_StateOpening
                                        select state.Storyboard).FirstOrDefault();
                    }
                }
            }

            this.ContentRoot = GetTemplateChild(PART_ContentRoot) as FrameworkElement;

            this._chrome = GetTemplateChild(PART_Chrome) as FrameworkElement;

            this._overlay = GetTemplateChild(PART_Overlay) as FrameworkElement;

            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();

            // Update overlay size
            if (this.IsOpen)
            {
                this._desiredContentHeight = this.Height;
                this._desiredContentWidth = this.Width;
                this.UpdateOverlaySize();
                this.UpdateRenderTransform();
                this._isOpening = true;
                try
                {
                    this.ChangeVisualState();
                }
                finally
                {
                    this._isOpening = false;
                }
            }
        }

        /// <summary>
        /// Raises the Closed event. 
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnClosed(EventArgs e)
        {
            EventHandler handler = this.Closed;

            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">A CancelEventArgs that contains the event data.</param>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.Closing;

            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Executed when the opening storyboard finishes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Opening_Completed(object sender, EventArgs e)
        {
            this.ChangeVisualState();
            this.Focus();

            if (this._opened != null)
            {
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
            }
        }

        /// <summary>
        /// Executed when the page resizes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event args.</param>
        private void Page_Resized(object sender, EventArgs e)
        {
            if (this.ChildWindowPopup != null)
            {
                this.UpdateOverlaySize();
            }
        }

        /// <summary>
        /// Executed when the root visual gets focus.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Routed event args.</param>
        private void RootVisual_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        /// <summary>
        /// Opens a child window. The interaction with the underlying UI is disabled but it is not a
        /// blocking call.
        /// </summary>
        public void Show()
        {
            this.SubscribeToEvents();
            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();

            if (this.ChildWindowPopup == null)
            {
                this.ChildWindowPopup = new Popup();
                this.ChildWindowPopup.Child = this;
            }
            // Margin, MaxHeight and MinHeight properties should not be overwritten:
            this.Margin = new Thickness(0);
            this.MaxHeight = double.PositiveInfinity;
            this.MaxWidth = double.PositiveInfinity;

            if (this.ChildWindowPopup != null && Application.Current.RootVisual != null)
            {
                this.ChildWindowPopup.IsOpen = true;

                // while the ChildWindow is open, the DialogResult is always NULL:
                this._dialogresult = null;
            }

            // disable the underlying UI
            if (RootVisual != null)
            {
                RootVisual.IsEnabled = false;
            }

            // if the template is already loaded, display loading visuals animation
            if (this.ContentRoot != null)
            {
                this._isOpening = true;
                try
                {
                    this.ChangeVisualState();
                }
                finally
                {
                    this._isOpening = false;
                }
            }
        }

        /// <summary>
        /// Subscribes to events when the ChildWindow is opened.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit += new EventHandler(this.Application_Exit);
                Application.Current.Host.Content.Resized += new EventHandler(this.Page_Resized);
            }
            this.KeyDown += new KeyEventHandler(this.ChildWindow_KeyDown);
            this.LostFocus += new RoutedEventHandler(this.ChildWindow_LostFocus);
            this.SizeChanged += new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
        }

        /// <summary>
        /// Subscribes to events that are on the storyboards. 
        /// Unsubscribing from these events happen in the event handlers individually.
        /// </summary>
        private void SubscribeToStoryBoardEvents()
        {
            if (this._closed != null)
            {
                this._closed.Completed += new EventHandler(this.Closing_Completed);
            }

            if (this._opened != null)
            {
                this._opened.Completed += new EventHandler(this.Opening_Completed);
            }
        }

        /// <summary>
        /// Subscribes to events on the template parts.
        /// </summary>
        private void SubscribeToTemplatePartEvents()
        {
            if (this._closeButton != null)
            {
                this._closeButton.Click += new RoutedEventHandler(this.CloseButton_Click);
            }

            if (this._chrome != null)
            {
                this._chrome.MouseLeftButtonDown += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.MouseLeftButtonUp += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.MouseMove += new MouseEventHandler(this.Chrome_MouseMove);
            }
        }

        /// <summary>
        /// Unsubscribe from events when the ChildWindow is closed.
        /// </summary>
        private void UnSubscribeFromEvents()
        {
            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                Application.Current.Exit -= new EventHandler(this.Application_Exit);
                Application.Current.Host.Content.Resized -= new EventHandler(this.Page_Resized);
            }
            this.KeyDown -= new KeyEventHandler(this.ChildWindow_KeyDown);
            this.LostFocus -= new RoutedEventHandler(this.ChildWindow_LostFocus);
            this.SizeChanged -= new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
        }

        /// <summary>
        /// Unsubscribe from the events that are subscribed on the template part elements.
        /// </summary>
        private void UnsubscribeFromTemplatePartEvents()
        {
            if (this._closeButton != null)
            {
                this._closeButton.Click -= new RoutedEventHandler(this.CloseButton_Click);
            }

            if (this._chrome != null)
            {
                this._chrome.MouseLeftButtonDown -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.MouseMove -= new MouseEventHandler(this.Chrome_MouseMove);
            }
        }

        /// <summary>
        /// Updates the size of the overlay of the window.
        /// </summary>
        private void UpdateOverlaySize()
        {
            if (this._overlay != null && Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                this.Height = Application.Current.Host.Content.ActualHeight;
                this.Width = Application.Current.Host.Content.ActualWidth;
                this._overlay.Height = this.Height;
                this._overlay.Width = this.Width;

                if (this.ContentRoot != null)
                {
                    this.ContentRoot.Width = this._desiredContentWidth;
                    this.ContentRoot.Height = this._desiredContentHeight;
                }
            }
        }

        /// <summary>
        /// Updates the render transform applied on the overlay.
        /// </summary>
        private void UpdateRenderTransform()
        {
            if (this._root != null && this.ContentRoot != null)
            {
                // The _overlay part should not be affected by the render transform applied on the
                // ChildWindow. In order to achieve this, we adjust an identity matrix to represent
                // the _root's transformation, invert it, apply the inverted matrix on the _root, so that 
                // nothing is affected by the rendertransform, and apply the original transform only on the Content
                GeneralTransform gt = this._root.TransformToVisual(null);
                if (gt != null)
                {
                    Point p10 = new Point(1, 0);
                    Point p01 = new Point(0, 1);
                    Point transform10 = gt.Transform(p10);
                    Point transform01 = gt.Transform(p01);

                    Matrix transformToRootMatrix = Matrix.Identity;
                    transformToRootMatrix.M11 = transform10.X;
                    transformToRootMatrix.M12 = transform10.Y;
                    transformToRootMatrix.M21 = transform01.X;
                    transformToRootMatrix.M22 = transform01.Y;

                    MatrixTransform original = new MatrixTransform();
                    original.Matrix = transformToRootMatrix;

                    InvertMatrix(ref transformToRootMatrix);
                    MatrixTransform mt = new MatrixTransform();
                    mt.Matrix = transformToRootMatrix;

                    TransformGroup tg = this._root.RenderTransform as TransformGroup;

                    if (tg != null)
                    {
                        tg.Children.Add(mt);
                    }
                    else
                    {
                        this._root.RenderTransform = mt;
                    }

                    tg = this.ContentRoot.RenderTransform as TransformGroup;

                    if (tg != null)
                    {
                        tg.Children.Add(original);
                    }
                    else
                    {
                        this.ContentRoot.RenderTransform = original;
                    }
                }
            }
        }

        #endregion Methods
    }
}
