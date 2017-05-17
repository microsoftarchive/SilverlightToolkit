// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Silverlight.Testing.Controls;
using Microsoft.Silverlight.Testing.Harness;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A user control designed for mobile platforms. The control should be used
    /// as the root visual for a Silverlight plugin if developers would like to 
    /// use the advanced TestSurface functionality.
    /// </summary>
    public partial class MobileTestPage : UserControl, ITestPage, IMobileTestPage
    {
        /// <summary>
        /// Contains the slide manager for the primitive user interface
        /// navigation system.
        /// </summary>
        private SlideManager _slides;

        /// <summary>
        /// Backing field for the unit test harness instance.
        /// </summary>
        private UnitTestHarness _harness;

        /// <summary>
        /// Backing field for the startup timer.
        /// </summary>
        private DispatcherTimer _delayedInitializationTimer;

        /// <summary>
        /// Backing field for the model manager.
        /// </summary>
        private DataManager _model;

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
        /// Initializes a new instance of the MobileTestPage class.
        /// </summary>
        public MobileTestPage()
        {
            InitializeComponent();

            _slides = new SlideManager();

            _delayedInitializationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            _delayedInitializationTimer.Tick += OnDelayedInitializationTick;
            _delayedInitializationTimer.Start();
        }

        /// <summary>
        /// Initializes the MobileTestPage object.
        /// </summary>
        /// <param name="harness">The test harness instance.</param>
        public MobileTestPage(UnitTestHarness harness)
            : this()
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
        private void OnDelayedInitializationTick(object sender, EventArgs e)
        {
            if (_harness != null && _harness.Settings != null)
            {
                _delayedInitializationTimer.Stop();
                _delayedInitializationTimer.Tick -= OnDelayedInitializationTick;
                _delayedInitializationTimer = null;

                // Tag expression support
                if (Application.Current != null &&
                    Application.Current.Host != null &&
                    Application.Current.Host.Source != null &&
                    Application.Current.Host.Source.Query != null &&
                    Application.Current.Host.Source.Query.Contains("test_run"))
                {
                    StartTestRun();
                }
                else
                {
                    MobileStartup tagEditor;
                    if (_harness.Settings != null)
                    {
                        tagEditor = new MobileStartup(_harness.Settings.TagExpression);
                    }
                    else
                    {
                        tagEditor = new MobileStartup();
                    }

                    CreateAndInsertSlide("Tag Expressions", tagEditor);

                    // overlayGrid.Children.Add(tagEditor);
                    // overlayGrid.Visibility = System.Windows.Visibility.Visible;
                    tagEditor.Complete += OnTagExpressionSelected;
                }
            }
        }

        /// <summary>
        /// Creates a new slide and inserts it into the slide manager, plus
        /// visual tree.
        /// </summary>
        /// <param name="header">The text header to use.</param>
        /// <param name="content">The content to inside the slide.</param>
        /// <returns>Returns the new Slide instance.</returns>
        private Slide CreateAndInsertSlide(string header, object content)
        {
            Slide slide = new Slide();
            slide.Content = content;
            slide.Header = new ContentControl
            {
                Content = header,
                Style = (Style)Resources["TextHeaderContent"],
            };

            Slides.Children.Add(slide);
            _slides.InsertFirst(slide); // Front

            return slide;
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

            // overlayGrid.Children.Clear();
            // overlayGrid.Visibility = System.Windows.Visibility.Collapsed;

            if (_harness != null)
            {
                if (e != null && _harness.Settings != null)
                {
                    _harness.Settings.TagExpression = e.TagExpression;
                }

                // Start the test run
                StartTestRun();
            }
        }

        /// <summary>
        /// Starts the test run.
        /// </summary>
        private void StartTestRun()
        {
            if (!(DataContext is TestRunData))
            {
                _model = DataManager.Create(_harness);
                _model.Hook();

                DataContext = _model.Data;

                _slides.Add(SlideTestsRunning);
                _slides.MoveTo(SlideTestsRunning);

                _harness.TestHarnessCompleted += OnTestHarnessCompleted;
            }

            _harness.Run();
            RunOverview.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles the test harness complete event, to display results.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestHarnessCompleted(object sender, TestHarnessCompletedEventArgs e)
        {
            _slides.Add(SlideTestAssemblies);
            _slides.MoveTo(SlideTestAssemblies);

            _slides.Add(SlideTestClasses);
            _slides.Add(SlideTestMethods);
            _slides.Add(SlideTestMethodDetails);

            if (_model.Data.TestAssemblies.Count > 0)
            {
                SlideTestClasses.DataContext = _model.Data.TestAssemblies[0];

                // Auto-matically navigate, just one result
                if (_model.Data.TestAssemblies.Count == 1)
                {
                    _slides.MoveTo(SlideTestClasses);
                }
            }
        }

        /// <summary>
        /// Handles the movement back to the test assemblies list.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMoveBackToTestAssembliesClick(object sender, RoutedEventArgs e)
        {
            _slides.MoveTo(SlideTestAssemblies);
        }

        /// <summary>
        /// Handles the movement back to the test classes list.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMoveBackToTestClassesClick(object sender, RoutedEventArgs e)
        {
            _slides.MoveTo(SlideTestClasses);
        }

        /// <summary>
        /// Handles the movement back to the test methods list.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMoveBackToTestMethodsClick(object sender, RoutedEventArgs e)
        {
            _slides.MoveTo(SlideTestMethods);
        }

        /// <summary>
        /// Handles the selection of a test assembly.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTestAssemblySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            object selected = lb.SelectedItem;
            if (selected != null)
            {
                lb.SelectedItem = null;
                SlideTestClasses.DataContext = selected;
                _slides.MoveTo(SlideTestClasses);
            }
        }

        /// <summary>
        /// Handles the selection of a test class.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTestClassSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            object selected = lb.SelectedItem;
            if (selected != null)
            {
                lb.SelectedItem = null;
                SlideTestMethods.DataContext = selected;
                _slides.MoveTo(SlideTestMethods);
            }
        }

        /// <summary>
        /// Handles the selection of a test method.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTestMethodsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            object selected = lb.SelectedItem;
            if (selected != null)
            {
                lb.SelectedItem = null;
                SlideTestMethodDetails.DataContext = selected;
                _slides.MoveTo(SlideTestMethodDetails);
            }
        }

        /// <summary>
        /// Requests navigation back a page.
        /// </summary>
        /// <returns>A value indicating whether the operation was successful.</returns>
        public bool NavigateBack()
        {
            // If there are no previous slides, allow exit back
            if (_slides.Current != SlideTestAssemblies)
            {
                if (_slides.Current == SlideTestMethodDetails)
                {
                    _slides.MoveTo(SlideTestMethods);
                }
                else if (_slides.Current == SlideTestMethods)
                {
                    _slides.MoveTo(SlideTestClasses);
                }
                else if (_slides.Current == SlideTestClasses)
                {
                    _slides.MoveTo(SlideTestAssemblies);
                }
                else
                {
                    _slides.Previous();
                }

                return true;
            }

            return false;
        }
    }
}