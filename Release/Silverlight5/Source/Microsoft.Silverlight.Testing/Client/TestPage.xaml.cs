// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Silverlight.Testing.Controls;
using Microsoft.Silverlight.Testing.Harness;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A user control that should be used as the root visual for a Silverlight 
    /// plugin if developers would like to use the advanced TestSurface 
    /// functionality within Microsoft.Silverlight.Testing. The TestSurface is 
    /// automatically cleared after each test scenario completes, eliminating 
    /// the need for many additional cleanup methods.
    /// </summary>
    public partial class TestPage : UserControl, ITestPage
    {
        /// <summary>
        /// Backing field for the unit test harness instance.
        /// </summary>
        private UnitTestHarness _harness;

        /// <summary>
        /// Backing field for the model manager.
        /// </summary>
        private DataManager _model;

        /// <summary>
        /// Backing field for the startup timer.
        /// </summary>
        private DispatcherTimer _startupTimer;

        /// <summary>
        /// Backing field for a value indicating whether the clipboard feature
        /// is supported.
        /// </summary>
        private bool? _clipboardFeatureSupported = null;

        /// <summary>
        /// Gets the test surface, a dynamic Panel that removes its children 
        /// elements after each test completes.
        /// </summary>
        public Panel TestPanel
        {
            get { return TestStage; }
        }

        /// <summary>
        /// Gets the unit test harness instance.
        /// </summary>
        public UnitTestHarness UnitTestHarness
        {
            get { return _harness; }
        }

        /// <summary>
        /// Initializes the TestPage object.
        /// </summary>
        public TestPage()
        {
            InitializeComponent();

            unitTestFrameworkBuild.Text = UnitTestSystem.FrameworkFileVersion;

            // Startup timer
            _startupTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            _startupTimer.Tick += StartupMonitor;
            _startupTimer.Start();
        }

        /// <summary>
        /// Initializes the TestPage object.
        /// </summary>
        /// <param name="harness">The test harness instance.</param>
        public TestPage(UnitTestHarness harness) : this()
        {
            harness.TestPage = this;
            _harness = harness;
        }

        /// <summary>
        /// Waits for the Settings to become available, either by the service or
        /// system setting the instance property.
        /// </summary>
        /// <param name="sender">The source timer.</param>
        /// <param name="e">The event arguments.</param>
        private void StartupMonitor(object sender, EventArgs e)
        {
            if (_harness != null && _harness.Settings != null)
            {
                _startupTimer.Stop();
                _startupTimer.Tick -= StartupMonitor;
                _startupTimer = null;

                // Tag expression support
                if (Application.Current.IfApplicationHostSource() &&
                    Application.Current.Host.Source.Query != null &&
                    Application.Current.Host.Source.Query.Contains("test_run"))
                {
                    StartTestRun();
                }
                else
                {
                    TagEditor tagEditor;
                    if (_harness.Settings != null)
                    {
                        tagEditor = _harness.Settings.StartRunImmediately ? null : _harness.Settings.SampleTags != null ? new TagEditor(_harness.Settings.TagExpression, _harness.Settings.SampleTags) : new TagEditor(_harness.Settings.TagExpression);
                    }
                    else
                    {
                        tagEditor = new TagEditor();
                    }

                    if (tagEditor == null)
                    {
                        StartTestRun();
                        return;
                    }

                    overlayGrid.Children.Add(tagEditor);
                    overlayGrid.Visibility = Visibility.Visible;
                    tagEditor.Complete += OnTagExpressionSelected;
                }
            }
        }

        /// <summary>
        /// Starts the test run.
        /// </summary>
        private void StartTestRun()
        {
            _model = DataManager.Create(_harness);
            _model.Hook();

            DataContext = _model.Data;

            ScrollViewer sv = resultsTreeView.GetScrollHost();

            // Keep the current test visible in the tree view control
            _harness.TestClassStarting += (x, xe) =>
                {
                    if (resultsTreeView != null)
                    {
                        resultsTreeView.SelectItem(_model.GetClassModel(xe.TestClass));
                    };
                };
            _harness.TestMethodStarting += (x, xe) =>
                {
                    if (sv != null)
                    {
                        sv.ScrollToBottom();
                    }
                };
            _harness.TestMethodCompleted += OnTestMethodCompleted;
            _harness.TestHarnessCompleted += OnTestHarnessCompleted;
            _harness.TestAssemblyStarting += OnTestAssemblyStarting;

            _harness.Run();
        }

        /// <summary>
        /// Handles the test assembly starting event to expand the test stage
        /// height.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestAssemblyStarting(object sender, TestAssemblyStartingEventArgs e)
        {
            ExpandCollapseTestStage(true);
        }

        /// <summary>
        /// Handles the test harness complete event, to display results.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestHarnessCompleted(object sender, TestHarnessCompletedEventArgs e)
        {
            ExpandCollapseTestStage(false);
        }

        /// <summary>
        /// Handles the click on the test stage.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestStageExpanderClick(object sender, EventArgs e)
        {
            ExpandCollapseTestStage(TestStageBorder.Visibility == Visibility.Collapsed);
        }

        /// <summary>
        /// Expand and collapse the test stage.
        /// </summary>
        /// <param name="expand">A value indicating whether to expand the stage.
        /// </param>
        private void ExpandCollapseTestStage(bool expand)
        {
            TestStageRowDefinition.Height = expand ? new GridLength(3, GridUnitType.Star) : GridLength.Auto;
            TestStageBorder.Visibility = expand ? Visibility.Visible : Visibility.Collapsed;
            TestStageExpander.Angle = expand ? 0 : 180;
        }

        /// <summary>
        /// Handles the completion of a test method.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTestMethodCompleted(object sender, TestMethodCompletedEventArgs e)
        {
            ScenarioResult result = e.Result;

            if (result.Result != TestOutcome.Passed)
            {
                TestMethodData tmd = _model.GetMethodModel(
                    e.Result.TestMethod,
                    _model.GetClassModel(e.Result.TestClass));
            }
        }

        /// <summary>
        /// Handles the completion event on the tag expression editor to begin
        /// the test run using the user-provided settings.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnTagExpressionSelected(object sender, TagExpressionEventArgs e)
        {
            TagEditor te = sender as TagEditor;
            if (te != null)
            {
                te.Complete -= OnTagExpressionSelected;
            }

            overlayGrid.Children.Clear();
            overlayGrid.Visibility = Visibility.Collapsed;

            if (_harness != null)
            {
                if (e != null && _harness.Settings != null)
                {
                    _harness.Settings.TagExpression = e.TagExpression;

                    // Provide clear indication of a tag expression when in use.
                    if (!string.IsNullOrEmpty(e.TagExpression))
                    {
                        Notifications.AddNotification(new Notification
                        {
                            Title = "Tag Expression in Use",
                            Content = e.TagExpression,
                        });
                    }
                }

                // Start the test run
                StartTestRun();
            }
        }

        /// <summary>
        /// Gets the tree view instance.
        /// </summary>
        public TreeView TreeView
        {
            get { return resultsTreeView; }
        }

        #region Toolbar events

        /// <summary>
        /// Handles navigation back or forward.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnResultNavigationClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string tag = (string)b.Tag;

            TestMethodData tmd = (TestMethodData)b.DataContext;
            TestMethodData to = null;

            switch (tag)
            {
                case "Previous":
                    to = tmd.PreviousResult;
                    break;

                case "PreviousFailure":
                    to = tmd.PreviousFailingResult;
                    break;

                case "NextFailure":
                    to = tmd.NextFailingResult;
                    break;

                case "Next":
                    to = tmd.NextResult;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            if (to != null)
            {
                resultsTreeView.SelectItem(to);
            }
        }

        /// <summary>
        /// Installs the application.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnInstallClick(object sender, EventArgs e)
        {
            if (Application.Current.InstallState != InstallState.Installed)
            {
                Application.Current.Install();
            }
        }

        /// <summary>
        /// Offers clipboard interface support for copying test run results.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnClipboardButtonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            string tag = (string)b.Tag;
            string text = string.Empty;

            switch (tag)
            {
                case "CopyAllChecked":
                    StringBuilder sb = new StringBuilder();
                    int count = 0;
                    foreach (KeyValuePair<object, TreeViewItem> item in resultsTreeView.GetCheckedItemsAndContainers())
                    {
                        // Only want the actual leaf nodes
                        TestMethodData tmd = item.Key as TestMethodData;
                        if (tmd != null)
                        {
                            sb.AppendLine(tmd.GetResultReport());
                            count++;
                        }
                    }
                    if (count == 0)
                    {
                        sb.AppendLine("There were no checked results.");
                        text = sb.ToString();
                    }
                    else
                    {
                        text = "There were " + 
                            count.ToString() + 
                            " results checked." + 
                            Environment.NewLine + 
                            Environment.NewLine + 
                            sb.ToString();
                    }
                    
                    break;

                case "CopyResults":
                    IProvideResultReports result = resultsTreeView.SelectedItem as IProvideResultReports;
                    if (result != null)
                    {
                        text = result.GetResultReport();
                    }
                    break;

                default:
                case "Close":
                    ClipboardContents.Text = string.Empty;
                    ClipboardHelperGrid.Visibility = Visibility.Collapsed;
                    return;
            }

            SetClipboardText(text);
        }

        #endregion

        /// <summary>
        /// Handles the click on a play/pause button for the run dispatcher.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPlayPauseClick(object sender, RoutedEventArgs e)
        {
            _harness.IsDispatcherRunning = !_harness.IsDispatcherRunning;
        }

        /// <summary>
        /// Copies text into the clipboard. If the Silverlight runtime on the
        /// system does not support the clipboard API, then it reverts to a
        /// large text box that allows the user to manually copy and paste.
        /// </summary>
        /// <param name="text">The text to set.</param>
        private void SetClipboardText(string text)
        {
            if (_clipboardFeatureSupported == null)
            {
                _clipboardFeatureSupported = ClipboardHelper.IsClipboardFeatureSupported;
            }

            bool useAlternative = true;
            if (_clipboardFeatureSupported == true)
            {
                try
                {
                    ClipboardHelper.SetText(text);
                    useAlternative = false;
                }
                catch (SecurityException)
                {
                    useAlternative = true;
                }
            }

            if (useAlternative)
            {
                ClipboardContents.Text = text;
                ClipboardHelperGrid.Visibility = Visibility.Visible;
                ClipboardContents.SelectAll();
                ClipboardContents.Focus();
            }
        }
    }
}