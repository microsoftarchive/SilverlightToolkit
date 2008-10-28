// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Windows.Controls.Theming;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// The SampleBrowser is used to display interactive samples of controls,
    /// styles, and scenarios.
    /// </summary>
    [ContentProperty("Samples")]
    [TemplatePart(Name = SampleBrowser.SampleSelectionName, Type = typeof(TreeView))]
    [TemplatePart(Name = SampleBrowser.ThemeSelectorName, Type = typeof(ComboBox))]
    [TemplatePart(Name = SampleBrowser.FullScreenButtonName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = SampleBrowser.SourceListName, Type = typeof(ListBox))]
    [TemplatePart(Name = SampleBrowser.SampleScrollerName, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = SampleBrowser.SampleViewerName, Type = typeof(Panel))]
    [TemplatePart(Name = SampleBrowser.SourceTextName, Type = typeof(TextBox))]
    public sealed class SampleBrowser : Control
    {
        #region TemplateParts
        /// <summary>
        /// Name of the sample selection TreeView.
        /// </summary>
        private const string SampleSelectionName = "SampleSelection";

        /// <summary>
        /// Name of the theme selector.
        /// </summary>
        private const string ThemeSelectorName = "ThemeSelector";

        /// <summary>
        /// Name of the button that toggles full screen.
        /// </summary>
        private const string FullScreenButtonName = "FullScreenButton";

        /// <summary>
        /// Name of the ScrollViewer around the sample.
        /// </summary>
        private const string SampleScrollerName = "SampleScroller";

        /// <summary>
        /// Name of the placeholder of the selected sample.
        /// </summary>
        private const string SampleViewerName = "SampleViewer";

        /// <summary>
        /// Name of the source list ListBox.
        /// </summary>
        private const string SourceListName = "SourceList";

        /// <summary>
        /// Name of the source text TextBox.
        /// </summary>
        private const string SourceTextName = "SourceText";

        /// <summary>
        /// The TreeView used for selecting samples.
        /// </summary>
        private TreeView _sampleSelection;

        /// <summary>
        /// Gets or sets the TreeView used for selecting samples.
        /// </summary>
        private TreeView SampleSelection
        {
            get { return _sampleSelection; }
            set
            {
                if (_sampleSelection != null)
                {
                    _sampleSelection.SelectedItemChanged -= OnSelectedSampleChanged;
                }

                _sampleSelection = value;

                if (_sampleSelection != null)
                {
                    _sampleSelection.SelectedItemChanged += OnSelectedSampleChanged;
                }
            }
        }

        /// <summary>
        /// Selector used to choose the theme.
        /// </summary>
        private ComboBox _themeSelector;

        /// <summary>
        /// Gets or sets the selector used to choose the theme.
        /// </summary>
        private ComboBox ThemeSelector
        {
            get { return _themeSelector; }
            set
            {
                if (_themeSelector != null)
                {
                    _themeSelector.SelectionChanged -= OnThemeChanged;
                }

                _themeSelector = value;

                if (_themeSelector != null)
                {
                    _themeSelector.SelectionChanged += OnThemeChanged;
                }
            }
        }

        /// <summary>
        /// The button that toggles full screen view.
        /// </summary>
        private ToggleButton _fullscreenButton;

        /// <summary>
        /// Gets or sets the button that toggles full screen view.
        /// </summary>
        private ToggleButton FullScreenButton
        {
            get { return _fullscreenButton; }
            set
            {
                if (_fullscreenButton != null)
                {
                    _fullscreenButton.Click -= OnToggleFullScreen;
                }

                _fullscreenButton = value;
                if (_fullscreenButton != null)
                {
                    _fullscreenButton.Click += OnToggleFullScreen;
                }
            }
        }

        /// <summary>
        /// Gets or sets the placeholder for the selected sample.
        /// </summary>
        private ScrollViewer SampleScroller { get; set; }

        /// <summary>
        /// Gets or sets the placeholder for the selected sample.
        /// </summary>
        private Panel SampleViewer { get; set; }

        /// <summary>
        /// ListBox used to select the sample or source file to view.
        /// </summary>
        private ListBox _sourceList;

        /// <summary>
        /// Gets or sets the ListBox used to select the sample or source file to
        /// view.
        /// </summary>
        private ListBox SourceList
        {
            get { return _sourceList; }
            set
            {
                if (_sourceList != null)
                {
                    _sourceList.SelectionChanged -= OnSourceFileChanged;
                }

                _sourceList = value;

                if (_sourceList != null)
                {
                    _sourceList.SelectionChanged += OnSourceFileChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets the source text TextBox.
        /// </summary>
        private TextBox SourceText { get; set; }
        #endregion

        #region public string Title
        /// <summary>
        /// Gets or sets the title of the samples being browsed.
        /// </summary>
        public string Title
        {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(SampleBrowser),
                new PropertyMetadata(null));
        #endregion public string Title

        /// <summary>
        /// Gets or sets a reference to the current SampleBrowser control.
        /// </summary>
        internal static SampleBrowser Current { get; set; }

        /// <summary>
        /// A collection of themes to use for styling the samples.
        /// </summary>
        private static Dictionary<string, Type> Themes = new Dictionary<string, Type>
        {
            { "Default Theme", null }
        };

        /// <summary>
        /// Gets the samples being browsed.
        /// </summary>
        public IList<SampleBrowserItem> Samples { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SampleBrowser class.
        /// </summary>
        /// <param name="instance">Instance from the samples assembly.</param>
        public SampleBrowser(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            DefaultStyleKey = typeof(SampleBrowser);
            Current = this;
            
            // Setup the samples
            Samples = new List<SampleBrowserItem>();
            PopulateSamples(instance.GetType().Assembly);

            // Get notifications when full screen mode changes
            if (Application.Current != null)
            {
                Application.Current.Host.Content.FullScreenChanged += OnFullScreenChanged;
            }
        }

        /// <summary>
        /// Populate the sample tree from the types in the assembly decorated
        /// with a SampleAttribute.
        /// </summary>
        /// <param name="assembly">
        /// Assembly containing the samples to browse.
        /// </param>
        private void PopulateSamples(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                SampleAttribute attribute = type.GetCustomAttributes(typeof(SampleAttribute), false).OfType<SampleAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    SampleBrowserItem.AddSample(Samples, type, attribute);
                }
            }
        }

        /// <summary>
        /// Retrieve select elements from a control template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SampleSelection = GetTemplateChild(SampleSelectionName) as TreeView;
            ThemeSelector = GetTemplateChild(ThemeSelectorName) as ComboBox;
            FullScreenButton = GetTemplateChild(FullScreenButtonName) as ToggleButton;
            SourceList = GetTemplateChild(SourceListName) as ListBox;
            SampleScroller = GetTemplateChild(SampleScrollerName) as ScrollViewer;
            SampleViewer = GetTemplateChild(SampleViewerName) as Panel;
            SourceText = GetTemplateChild(SourceTextName) as TextBox;

            // Hide source viewer components
            if (SourceList != null)
            {
                SourceList.Visibility = Visibility.Collapsed;
            }
            if (SourceText != null)
            {
                SourceText.Visibility = Visibility.Collapsed;
            }

            // Populate the themes
            if (ThemeSelector != null)
            {
                foreach (KeyValuePair<string, Type> themes in Themes)
                {
                    ThemeSelector.Items.Add(new ComboBoxItem { Content = themes.Key, Tag = themes.Value });
                }
                ThemeSelector.SelectedIndex = 0;
            }

            // Populate the samples
            if (SampleSelection != null)
            {
                Queue<SampleBrowserItem> remaining = new Queue<SampleBrowserItem>(Samples);
                while (remaining.Count > 0)
                {
                    SampleBrowserItem item = remaining.Dequeue();

                    // Support a dot prefix convention to jump ahead of the 
                    // ordinal sorting
                    string displayName = item.Name;
                    if (displayName.StartsWith(".", StringComparison.Ordinal) && displayName.Length > 1)
                    {
                        displayName = displayName.Substring(1);
                    }

                    // Map the item
                    TreeViewItem view = new TreeViewItem { Header = displayName, Tag = item };
                    item.TreeViewItem = view;

                    // Add the item to the tree
                    if (item.Parent == null)
                    {
                        SampleSelection.Items.Add(view);
                    }
                    else
                    {
                        item.Parent.TreeViewItem.Items.Add(view);
                    }

                    // Add all of the children
                    foreach (SampleBrowserItem child in item.Items)
                    {
                        remaining.Enqueue(child);
                    }
                }

                // Select the first item automatically
                if (SampleSelection.Items.Count > 0)
                {
                    Dispatcher.BeginInvoke(() => { (SampleSelection.Items[0] as TreeViewItem).IsSelected = true; });
                }
            }
        }

        /// <summary>
        /// Change the selected sample.
        /// </summary>
        /// <param name="sender">The samples TreeView.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSelectedSampleChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateSample();
        }

        /// <summary>
        /// Choose the theme for the samples.
        /// </summary>
        /// <param name="sender">Theme selector.</param>
        /// <param name="e">Event arguments.</param>
        private void OnThemeChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSample();
        }

        /// <summary>
        /// Toggle the full screen view of the samples.
        /// </summary>
        /// <param name="sender">The ToggleButton.</param>
        /// <param name="e">Event arguments.</param>
        private void OnToggleFullScreen(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = (bool) FullScreenButton.IsChecked;
        }

        /// <summary>
        /// Handle full screen changes to the application.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OnFullScreenChanged(object sender, EventArgs e)
        {
            // Uncheck the ToggleButton if the application quit full screen mode
            if (!Application.Current.Host.Content.IsFullScreen && FullScreenButton != null)
            {
                FullScreenButton.IsChecked = false;
            }
        }

        /// <summary>
        /// Change the selected sample.
        /// </summary>
        private void UpdateSample()
        {
            if (SampleViewer == null)
            {
                return;
            }

            // Remove any previously displayed Sample 
            SampleViewer.Children.Clear();
            SetSourceViewer(null);

            TreeViewItem view = SampleSelection.SelectedItem as TreeViewItem;
            if (view == null)
            {
                return;
            }

            // Get the new sample
            SampleBrowserItem item = view.Tag as SampleBrowserItem;
            FrameworkElement sample = item.GetSample();

            // Optionally wrap the sample in a theme
            ComboBoxItem themeView = ThemeSelector.SelectedItem as ComboBoxItem;
            if (themeView != null)
            {
                Type themeType = themeView.Tag as Type;
                if (themeType != null)
                {
                    Theme theme = Activator.CreateInstance(themeType) as Theme;
                    if (theme != null)
                    {
                        theme.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                        theme.Padding = new Thickness(5);
                        theme.Content = sample;

                        // Since most of the samples are user controls, the
                        // actual controls to theme will be defined in a
                        // different namescope.  By attaching ISM to the root of
                        // the user control, we can style controls in that
                        // namescope and thye will resolve with styles from the
                        // theme.
                        UserControl control = sample as UserControl;
                        if (control != null)
                        {
                            theme.Dispatcher.BeginInvoke(() =>
                                {
                                    if (VisualTreeHelper.GetChildrenCount(control) > 0)
                                    {
                                        FrameworkElement root = VisualTreeHelper.GetChild(control, 0) as FrameworkElement;
                                        if (root != null)
                                        {
                                            ImplicitStyleManager.SetApplyMode(root, ImplicitStylesApplyMode.OneTime);
                                            ImplicitStyleManager.Apply(root);
                                        }
                                    }
                                });
                        }

                        sample = theme;
                    }
                }
            }

            // Reset the ScrollViewer when we change samples
            if (SampleScroller != null)
            {
                SampleScroller.ScrollToHorizontalOffset(0);
                SampleScroller.ScrollToVerticalOffset(0);
            }

            SampleViewer.Children.Add(sample);
        }

        /// <summary>
        /// Set the SourceViewer for the current sample.
        /// </summary>
        /// <param name="viewer">The SourceViewer.</param>
        /// <remarks>
        /// This is called by the SourceViewer when loaded as a way to notify
        /// the browser that it has source without resorting to walking the
        /// visual tree, etc.
        /// </remarks>
        public void SetSourceViewer(SourceViewer viewer)
        {
            // Clean up the source viewer components
            if (SourceList != null)
            {
                SourceList.Items.Clear();
            }
            if (SourceText != null)
            {
                SourceText.Text = "";
                SourceText.Visibility = Visibility.Collapsed;
            }

            // Set up the components
            if (viewer == null)
            {
                if (SourceList != null)
                {
                    SourceList.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (SourceList != null)
                {
                    // Populate the source files
                    SourceList.Items.Add(new ListBoxItem { Content = "Sample", Tag = null });
                    foreach (SourceFile file in viewer.Files)
                    {
                        string name = Path.GetFileName(file.Path);
                        SourceList.Items.Add(new ListBoxItem { Content = name, Tag = file });
                    }

                    SourceList.Visibility = Visibility.Visible;
                    SourceList.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Choose the source file to view.
        /// </summary>
        /// <param name="sender">The SourceList.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceFileChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceText == null)
            {
                return;
            }

            // Get the selected source file
            ListBoxItem view = SourceList.SelectedItem as ListBoxItem;

            if (view == null || view.Tag == null)
            {
                // Show the sample if there was no source file
                SourceText.Visibility = Visibility.Collapsed;
                SourceText.Text = "";
                SampleScroller.Visibility = Visibility.Visible;
            }
            else
            {
                // Remove any previously displayed Sample 
                SourceText.Text = "";

                // Get the source file
                SourceFile file = view.Tag as SourceFile;
                SourceText.Text = file.Source ?? "";

                // Show the source
                SampleScroller.Visibility = Visibility.Collapsed;
                SourceText.Visibility = Visibility.Visible;
            }
        }
    }
}