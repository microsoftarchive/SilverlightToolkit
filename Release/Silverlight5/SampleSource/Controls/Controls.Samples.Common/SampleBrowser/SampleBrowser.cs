// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Browser;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Samples.SyntaxHighlighting;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// The SampleBrowser is used to display interactive samples of controls,
    /// styles, and scenarios.
    /// </summary>
    [ContentProperty("Samples")]
    [TemplatePart(Name = SampleBrowser.SampleSelectionName, Type = typeof(TreeView))]
    [TemplatePart(Name = SampleBrowser.FullScreenButtonName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = SampleBrowser.SourceListName, Type = typeof(ListBox))]
    [TemplatePart(Name = SampleBrowser.SourceTextName, Type = typeof(TextBox))]
    [TemplatePart(Name = SampleBrowser.SamplesTabName, Type = typeof(TabControl))]
    [TemplatePart(Name = SampleBrowser.SourceExpanderName, Type = typeof(Expander))]
    [TemplatePart(Name = SampleBrowser.SampleGridSplitterName, Type = typeof(GridSplitter))]
    [TemplatePart(Name = SampleBrowser.SampleHeaderName, Type = typeof(TextBlock))]
    [TemplatePart(Name = SampleBrowser.SampleLevelName, Type = typeof(Label))]
    [TemplatePart(Name = SampleBrowser.SampleDisplayAreaName, Type = typeof(Grid))]
    [TemplatePart(Name = SampleBrowser.RootName, Type = typeof(Grid))]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Samples demonstrate a lot of types.")]
    public sealed class SampleBrowser : Control
    {
        /// <summary>
        /// SampleDisplayArea Grid row number where the source viewer is 
        /// located.
        /// </summary>
        private const int SourceViewerRow = 3;

        /// <summary>
        /// Deep Path Link QueryString Parameter Name.
        /// </summary>
        private const string DeepPathLinkQueryStringParamName = "path";

        #region TemplateParts
        /// <summary>
        /// Name of the sample selection TreeView.
        /// </summary>
        private const string SampleSelectionName = "SampleSelection";

        /// <summary>
        /// Name of the samples TabControl .
        /// </summary>
        private const string SamplesTabName = "SamplesTab";

        /// <summary>
        /// Name of the button that toggles full screen.
        /// </summary>
        private const string FullScreenButtonName = "FullScreenButton";

        /// <summary>
        /// Name of the source list ListBox.
        /// </summary>
        private const string SourceListName = "SourceList";

        /// <summary>
        /// Name of the source text TextBox.
        /// </summary>
        private const string SourceTextName = "SourceText";

        /// <summary>
        /// Name of root layout for the Sample Browser.
        /// </summary>
        private const string RootName = "Root";

        /// <summary>
        /// Name of the grid containing the sample and source file viewer.
        /// </summary>
        private const string SampleDisplayAreaName = "DisplayArea";

        /// <summary>
        /// Name of the expander containing the source file viewer.
        /// </summary>
        private const string SourceExpanderName = "SourceExpander";

        /// <summary>
        /// Name of the GridSplitter that separates the sample from the source
        /// file viewer.
        /// </summary>
        private const string SampleGridSplitterName = "SampleGridSplitter";

        /// <summary>
        /// Name of the TabItem style defined in generic.xaml.
        /// </summary>
        private const string TabItemStyleName = "TabItemStyle";

        /// <summary>
        /// Name of the Sample Header.
        /// </summary>
        private const string SampleHeaderName = "SampleHeader";

        /// <summary>
        /// Name of the Label containing the Sample Level defined in 
        /// generic.xaml.
        /// </summary>
        private const string SampleLevelName = "SampleLevel";

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
                    _sampleSelection.LayoutUpdated += OnSampleLayoutUpdated;
                }
            }
        }

        /// <summary>
        /// Gets or sets the root layout.
        /// </summary>
        private Grid SampleRoot { get; set; }

        /// <summary>
        /// Gets or sets the sample display area grid.
        /// </summary>
        private Grid SampleDisplayArea { get; set; }

        /// <summary>
        /// Gets or sets the grid splitter that separates the sample
        /// from the source viewer.
        /// </summary>
        private GridSplitter SampleGridSplitter { get; set; }

        /// <summary>
        /// Gets or sets the style for the sample tab items.
        /// </summary>
        private Style SampleTabItemStyle { get; set; }

        /// <summary>
        /// Gets or sets The expander that holds the list of files and content area where
        /// they are rendered.
        /// </summary>
        private Expander _sourceExpander { get; set; }

        /// <summary>
        /// Gets or sets the Expander that hosts the samples.
        /// </summary>
        private Expander SourceExpander
        {
            get { return _sourceExpander; }
            set
            {
                if (_sourceExpander != null)
                {
                    _sourceExpander.Expanded -= OnSourceExpanded;
                    _sourceExpander.Collapsed -= OnSourceCollapsed;
                }
                _sourceExpander = value;

                if (_sourceExpander != null)
                {
                    _sourceExpander.Expanded += OnSourceExpanded;
                    _sourceExpander.Collapsed += OnSourceCollapsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tabControl that hosts the samples.
        /// </summary>
        private TabControl SamplesTab { get; set; }

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
        private SyntaxHighlightingTextBox SourceText { get; set; }

        /// <summary>
        /// Gets or sets the sample header label.
        /// </summary>
        private TextBlock SampleHeader { get; set; }

        /// <summary>
        /// Gets or sets the sample difficulty level label.
        /// </summary>
        private Label SampleDifficultyLevel { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets a reference to the current SampleBrowser control.
        /// </summary>
        internal static SampleBrowser Current { get; set; }

        /// <summary>
        /// Gets or sets the items used to populate the TreeView.
        /// </summary>
        private IEnumerable<SampleTreeItem> SampleTreeItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the SourceView expander 
        /// is collapsed by the user.
        /// </summary>
        private bool IsSourceViewCollapsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the current source file displayed.
        /// </summary>
        private SourceFile CurrentSourceFile { get; set; }

        /// <summary>
        /// Initializes a new instance of the SampleBrowser class.
        /// </summary>
        /// <param name="sampleTreeItems">Items used to populate the TreeView.</param>
        public SampleBrowser(IEnumerable<SampleTreeItem> sampleTreeItems)
        {
            SampleTreeItems = sampleTreeItems;
            Current = this;
            DefaultStyleKey = typeof(SampleBrowser);
            IsSourceViewCollapsed = true;

            // Get notifications when full screen mode changes
            if (Application.Current != null)
            {
                Application.Current.Host.Content.FullScreenChanged += OnFullScreenChanged;
            }

            CompositionInitializer.SatisfyImports(this);
        }

        /// <summary>
        /// Gets or sets the set of Samples. This must be public because SL doesn't support
        /// non-public reflection which MEF needs to set this property.
        /// </summary>
        [ImportMany("Sample")]
        public Lazy<FrameworkElement, ISampleMetadata>[] Samples { get; set; }

        /// <summary>
        /// Populate the Tab with samples name for a given control.
        /// </summary>
        /// <param name="controlName">Name of the control.</param>
        private void PopulateSamplesTab(string controlName)
        {
            if (SamplesTab == null)
            {
                return;
            }

            // Unhook this event when dynamically before adding new tabs.
            SamplesTab.SelectionChanged -= OnSamplesTabSelectionChanged;

            if (SamplesTab.Items.Count > 0)
            {
                SamplesTab.Items.Clear();
            }
            
            // Construct a sorted list of samples
            IList<SampleBrowserItem> samples = new List<SampleBrowserItem>();
 
            foreach (var sample in Samples)
            {
                if (controlName.Equals(sample.Metadata.Category))
                {
                    SampleBrowserItem.AddSample(samples, sample);
                }
            }
            // Populate the Tab Control with the samples.
            foreach (SampleBrowserItem item in samples)
            {
                TabItem tabItem = new TabItem();
                if (SampleTabItemStyle != null)
                {
                    tabItem.Style = SampleTabItemStyle;
                }

                tabItem.Header = item;
                SamplesTab.Items.Add(tabItem);
            }

            // Hook up this event when done creating the Sample tabs.
            SamplesTab.SelectionChanged += OnSamplesTabSelectionChanged;

            // Select first tab
            if (SamplesTab.Items.Count > 0)
            {
                (SamplesTab.Items[0] as TabItem).IsSelected = true;
                LoadSample();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Deep Path Link query string parameter was specified.
        /// </summary>
        private static bool IsDeepPathLinkSpecified
        {
            get
            {
                if (!HtmlPage.IsEnabled)
                {
                    return false;
                }

                if (HtmlPage.Document == null)
                {
                    return false;
                } 

                return HtmlPage.Document.QueryString.Keys.Contains(DeepPathLinkQueryStringParamName);
            }
        }

        /// <summary>
        /// Returns the Initial Deep Path Link Lookup.
        /// </summary>
        /// <param name="level">Deep path link level.</param>
        /// <returns>The deep path for that level.</returns>
        private static string GetPathForLevel(int level)
        {
            if (!IsDeepPathLinkSpecified)
            {
                return string.Empty;
            }

            string DeepLinkPathRawValue = HtmlPage.Document.QueryString[DeepPathLinkQueryStringParamName];
            string DeepLinkPathDecodedValue = HttpUtility.UrlDecode(DeepLinkPathRawValue);
            string[] deepLinkPath = DeepLinkPathDecodedValue.Split('|');

            if (deepLinkPath.Length - 1 >= level)
            {
                return deepLinkPath[level];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Expands the Sample TreeView.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSampleLayoutUpdated(object sender, EventArgs e)
        {
            TreeViewItem item = _sampleSelection.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            item.IsSelected = true;
            _sampleSelection.LayoutUpdated -= OnSampleLayoutUpdated;
        }

        /// <summary>
        /// Retrieve select elements from a control template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SampleSelection = GetTemplateChild(SampleSelectionName) as TreeView;
            FullScreenButton = GetTemplateChild(FullScreenButtonName) as ToggleButton;
            SourceList = GetTemplateChild(SourceListName) as ListBox;
            SourceText = GetTemplateChild(SourceTextName) as SyntaxHighlightingTextBox;
            SamplesTab = GetTemplateChild(SamplesTabName) as TabControl;
            SampleRoot = GetTemplateChild(RootName) as Grid;
            SampleDisplayArea = GetTemplateChild(SampleDisplayAreaName) as Grid;
            SourceExpander = GetTemplateChild(SourceExpanderName) as Expander;
            SampleGridSplitter = GetTemplateChild(SampleGridSplitterName) as GridSplitter;
            SampleDifficultyLevel = GetTemplateChild(SampleLevelName) as Label;
            SampleHeader = GetTemplateChild(SampleHeaderName) as TextBlock;

            // Get the Style of the Tab Item to be used when generating the tabs
            if (SampleRoot != null)
            {
                SampleTabItemStyle = SampleRoot.Resources[TabItemStyleName] as Style;
            }
            // Populate the samples
            if (SampleSelection != null)
            {
                SampleSelection.ItemsSource = SampleTreeItems;
            }

            SelectNavigationOnDeepLinkPath();

            // Show the TreeView
            Show(SampleSelection);
        }

        /// <summary>
        /// Select a TreeViewItem and a TabItem used for navigation based on the query string
        /// deep link path. 
        /// </summary>
        private void SelectNavigationOnDeepLinkPath()
        {
            // Select a TreeViewItem based on the 1st and 2nd deep path levels
            if (IsDeepPathLinkSpecified)
            {
                if (!string.IsNullOrEmpty(GetPathForLevel(0)))
                {
                    SampleTreeItem firstDeepPathLevelSampleItem =
                        SampleSelection.Items.OfType<SampleTreeItem>().SingleOrDefault(
                            t => t.TreeItemName == GetPathForLevel(0));

                    if (firstDeepPathLevelSampleItem != null)
                    {
                        if (string.IsNullOrEmpty(GetPathForLevel(1)))
                        {
                            SelectTreeViewItemFor(firstDeepPathLevelSampleItem);
                        }
                        else
                        {
                            SampleTreeItem secondDeepPathLevelSampleItem =
                                firstDeepPathLevelSampleItem.Items.OfType<SampleTreeItem>().SingleOrDefault(
                                    t => t.TreeItemName == GetPathForLevel(1));

                            if (secondDeepPathLevelSampleItem != null)
                            {
                                SelectTreeViewItemFor(firstDeepPathLevelSampleItem, secondDeepPathLevelSampleItem);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Selects a tree view item based on an item.
        /// </summary>
        /// <param name="firstLevelItem">Item that correlates to a the first level TreeViewItem.</param>
        /// <param name="secondLevelItem">Item that correlates to a the second level TreeViewItem.</param>
        private void SelectTreeViewItemFor(SampleTreeItem firstLevelItem, SampleTreeItem secondLevelItem)
        {
            SampleSelection.Loaded += (s, args) =>
            {
                TreeViewItem firstLevelTreeViewItem = (TreeViewItem)
                    SampleSelection.ItemContainerGenerator.ContainerFromItem(firstLevelItem);
                TreeViewItem secondLevelTreeViewItem = (TreeViewItem)
                    firstLevelTreeViewItem.ItemContainerGenerator.ContainerFromItem(secondLevelItem);
                secondLevelTreeViewItem.IsSelected = true;

                // check if 3rd level deep link path has a correlating TabItem
                if (!string.IsNullOrEmpty(GetPathForLevel(2)))
                {
                    TabItem tabItemToSelect = SamplesTab.Items.OfType<TabItem>().SingleOrDefault(
                    t => ((SampleBrowserItem) t.Header).Name == GetPathForLevel(2));

                    if (tabItemToSelect != null)
                    {
                        tabItemToSelect.IsSelected = true;
                    }
                }
            };
        }

        /// <summary>
        /// Selects a tree view item based on an item.
        /// </summary>
        /// <param name="firstLevelItem">Item that correlates to a the TreeViewItem.</param>
        private void SelectTreeViewItemFor(SampleTreeItem firstLevelItem)
        {
            SampleSelection.Loaded += (s, args) =>
                {
                    ((TreeViewItem)SampleSelection.ItemContainerGenerator.ContainerFromItem(firstLevelItem)).IsSelected = true;
                };
        }

        /// <summary>
        /// Change the selected sample.
        /// </summary>
        /// <param name="sender">The samples TreeView.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSelectedSampleChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SampleTreeItem item = SampleSelection.SelectedItem as SampleTreeItem;
            if (item != null)
            {
                PopulateSamplesTab(item.TreeItemName);
                if (SampleHeader != null)
                {
                    SampleHeader.Text = item.TreeItemName;
                }
            }

            // Reset the CurrentSourceFile tracker
            CurrentSourceFile = null;
        }

        /// <summary>
        /// Collapse the Source File Viewer.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCollapsed(object sender, RoutedEventArgs e)
        {
            ExpandSampleDisplayArea();
            if (SampleGridSplitter != null)
            {
                SampleGridSplitter.Visibility = Visibility.Collapsed;
            }

            IsSourceViewCollapsed = true;
        }

        /// <summary>
        /// Expands the Source File Viewer.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceExpanded(object sender, RoutedEventArgs e)
        {
            ExpandSourceViewerArea();
            if (SampleGridSplitter != null)
            {
                SampleGridSplitter.Visibility = Visibility.Visible;
            }

            IsSourceViewCollapsed = false;
        }

        /// <summary>
        /// SamplesTab selection changed.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSamplesTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the previous tab item
            TabItem previousTab = e.RemovedItems[0] as TabItem;
            previousTab.Content = null;
            LoadSample();

            // Reset the CurrentSourceFile tracker
            CurrentSourceFile = null;
        }

        /// <summary>
        /// Toggle the full screen view of the samples.
        /// </summary>
        /// <param name="sender">The ToggleButton.</param>
        /// <param name="e">Event arguments.</param>
        private void OnToggleFullScreen(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = (bool)FullScreenButton.IsChecked;
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
        private void LoadSample()
        {
            SetSourceViewer(null);
            TabItem currentTab = SamplesTab.SelectedItem as TabItem;

            if (currentTab == null)
            {
                return;
            }
            
            // Get the new sample
            SampleBrowserItem item = currentTab.Header as SampleBrowserItem;
            currentTab.Content = item.GetSample();
            UpdateDifficultyLevel(item);
        }

        /// <summary>
        /// Updates the Sample Name and the Level labels.
        /// </summary>
        /// <param name="item">SampleBrowserItem containing Sample related
        /// information.</param>
        private void UpdateDifficultyLevel(SampleBrowserItem item)
        {
            Color SampleDifficultyLevelColor = Colors.Gray;
            SampleDifficultyLevel.Visibility = Visibility.Visible;
            if (SampleDifficultyLevel != null)
            {
                if (item.SampleLevel == DifficultyLevel.Basic)
                {
                    SampleDifficultyLevelColor = Colors.Green;
                }
                else if (item.SampleLevel == DifficultyLevel.Scenario)
                {
                    SampleDifficultyLevelColor = Colors.Orange;
                }
                else if (item.SampleLevel == DifficultyLevel.Advanced)
                {
                    SampleDifficultyLevelColor = Colors.Red;
                }
                else if (item.SampleLevel == DifficultyLevel.Scenario)
                {
                    SampleDifficultyLevelColor = Colors.DarkGray;
                }
                else if (item.SampleLevel == DifficultyLevel.None)
                {
                    SampleDifficultyLevel.Visibility = Visibility.Collapsed;
                }
                SampleDifficultyLevel.Background = new SolidColorBrush(SampleDifficultyLevelColor);
                SampleDifficultyLevel.Content = item.SampleLevel.ToString();
            }
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
                SourceText.Text = string.Empty;
            }

            // Set up the components
            if (viewer == null)
            {
                SourceExpander.Visibility = Visibility.Collapsed;
                SampleGridSplitter.Visibility = Visibility.Collapsed;
                ExpandSampleDisplayArea();
            }
            else
            {
                if (SourceList != null)
                {
                    SourceExpander.Visibility = Visibility.Visible;
                    SampleGridSplitter.Visibility = Visibility.Visible;

                    // Populate the source files
                    foreach (SourceFile file in viewer.Files)
                    {
                        string name = Path.GetFileName(file.Path);
                        string extention = Path.GetExtension(file.Path);
                        StackPanel menuItem = new StackPanel();

                        if (extention.Equals(".cs"))
                        {
                            menuItem = CreateMenuItem("cslogo.png", name);
                        }
                        else if (extention.Equals(".vb"))
                        {
                            menuItem = CreateMenuItem("vblogo.png", name);
                        }
                        else if (extention.Equals(".xaml"))
                        {
                            menuItem = CreateMenuItem("xamllogo.png", name);
                        }
                        SourceList.Items.Add(new ListBoxItem { Content = menuItem, Tag = file });
                    }

                    if (IsSourceViewCollapsed)
                    {
                        ExpandSampleDisplayArea();
                    }
                    else
                    {
                        ExpandSourceViewerArea();
                        Show(SourceList);
                    }

                    if (SourceList.Items.Count > 0)
                    {
                        SourceList.SelectedIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Expands the Source Viewer Area when there are source files to 
        /// display.
        /// </summary>
        private void ExpandSourceViewerArea()
        {
            if (SampleDisplayArea != null)
            {
                SampleDisplayArea.RowDefinitions[SourceViewerRow].Height = new GridLength(SampleDisplayArea.ActualHeight / 2, GridUnitType.Pixel);
            }
        }

        /// <summary>
        /// Expands the Sample Display Area when there is no Source File to 
        /// display. 
        /// </summary>
        private void ExpandSampleDisplayArea()
        {
            if (SampleDisplayArea != null)
            {
                SampleDisplayArea.RowDefinitions[SourceViewerRow].Height = new GridLength(1, GridUnitType.Auto);
            }
        }

        /// <summary>
        /// Helper method to begin a storyboard associated with a control.
        /// </summary>
        /// <param name="control">Control containing a storyboard to be played.
        /// </param>
        private static void Show(Control control)
        {
            Storyboard storyBoard = control.Resources["Show"] as Storyboard;
            if (storyBoard != null)
            {
                storyBoard.Begin();
            }
        }

        /// <summary>
        /// Helper method to create menu items.
        /// </summary>
        /// <param name="resourceName">Name of the resource to be loaded.</param>
        /// <param name="text">Text representing the menu item.</param>
        /// <returns>Returns a stack panel with Image and Text.</returns>
        private static StackPanel CreateMenuItem(string resourceName, string text)
        {
            StackPanel sampleStackPanel = new StackPanel();
            sampleStackPanel.Orientation = Orientation.Horizontal;
            TextBlock textBlock = CreateTextBlock(text);
            Image icon = SharedResources.GetIcon(resourceName);
            icon.Stretch = Stretch.None;
            sampleStackPanel.Children.Add(icon);
            sampleStackPanel.Children.Add(textBlock);
            return sampleStackPanel;
        }

        /// <summary>
        /// Helper function used to generate TextBlock that share common
        /// properties.
        /// </summary>
        /// <param name="text">Text to be used when creating the TextBlock.
        /// </param>
        /// <returns>TextBlock control with common properties set.</returns>
        private static TextBlock CreateTextBlock(string text)
        {
            TextBlock textBlock = new TextBlock { Text = text, FontSize = 11, Margin = new Thickness(3, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            return textBlock;
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

            // Remove any previously displayed Sample 
            SourceText.Text = string.Empty;

            if (view != null && view.Tag != null)
            {
                // Get the source file
                SourceFile file = view.Tag as SourceFile;
                if (CurrentSourceFile != file)
                {
                    SourceText.SourceLanguage = file.SourceType;
                    SourceText.Text = file.Source ?? string.Empty;

                    // Persist user source file view across sample selection
                    SourceExpander.IsExpanded = (null != CurrentSourceFile) || !IsSourceViewCollapsed;

                    CurrentSourceFile = file;
                }
            }
        }
    }
}
