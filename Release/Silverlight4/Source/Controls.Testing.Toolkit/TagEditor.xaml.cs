// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Silverlight.Testing;

namespace System.Windows.Controls.Testing.Toolkit
{
    /// <summary>
    /// The TagEditor control provides a brief user interface allowing a user to
    /// apply a set of test tags to their current test run.
    /// </summary>
    public partial class TagEditor : UserControl
    {
        /// <summary>
        /// Key used to lookup the TagHistory site setting.
        /// </summary>
        private const string TagHistoryKey = "TagHistory";

        /// <summary>
        /// Number of seconds to wait before running the test.
        /// </summary>
        private const int SecondsToWait = 5;

        /// <summary>
        /// Gets or sets the timer used to automatically run tests if no tag is
        /// entered.
        /// </summary>
        private DispatcherTimer Timer { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds already waited.
        /// </summary>
        private int SecondsWaited { get; set; }

        /// <summary>
        /// Gets or sets the tag history.
        /// </summary>
        private List<string> TagHistory { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the Tag class.
        /// </summary>
        public TagEditor()
        {
            InitializeComponent();

            // Test runs skip this page
            if (HtmlPage.IsEnabled && HtmlPage.Document.QueryString.ContainsKey("test_run"))
            {
                Root.Children.Clear();
                Root.Children.Add(UnitTestSystem.CreateTestPage());
                return;
            }

            // Get the tag history
            List<string> history;
            IsolatedStorageSettings.SiteSettings.TryGetValue(TagHistoryKey, out history);
            TagHistory = history;
            if (TagHistory == null)
            {
                TagHistory = new List<string>();
            }
            
            // Get the latest tag
            txtTag.Text = (TagHistory.Count > 0) ?
                TagHistory[0] :
                "All";
            
            // Fill in the history list
            if (TagHistory.Count <= 0)
            {
                pnlHistory.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < TagHistory.Count; i++)
                {
                    Button b = new Button { Content = TagHistory[i], Style = pnlEditor.Resources["TagButton"] as Style };
                    b.Click += OnTagButtonClicked;
                    pnlHistory.Children.Add(b);
                }
            }

            // Setup the auto-run timer
            txtTime.Text = SecondsToWait.ToString(CultureInfo.InvariantCulture);
            SecondsWaited = -1;
            Timer = new DispatcherTimer();
            Timer.Tick += OnTimerTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        /// <summary>
        /// Handle selection of a tag button.
        /// </summary>
        /// <param name="sender">Tag button.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used in XAML")]
        private void OnTagButtonClicked(object sender, RoutedEventArgs e)
        {
            Button example = sender as Button;
            if (example == null)
            {
                return;
            }

            string tag = example.Content as string;
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            txtTag.Text = tag;
            txtTag.Focus();
        }

        /// <summary>
        /// Handle changes to the Tag text.
        /// </summary>
        /// <param name="sender">Tag TextBox.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used in XAML")]
        private void OnTagTouched(object sender, RoutedEventArgs e)
        {
            StopTimer();
        }

        /// <summary>
        /// Handle clicks to the Run button.
        /// </summary>
        /// <param name="sender">Run Button.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used in XAML")]
        private void OnRunClicked(object sender, RoutedEventArgs e)
        {
            StopTimer();
            RunTests();
        }

        /// <summary>
        /// Handle timer ticks.
        /// </summary>
        /// <param name="sender">The timer.</param>
        /// <param name="e">Event arguments.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            SecondsWaited++;
            txtTime.Text = (SecondsToWait - SecondsWaited).ToString(CultureInfo.InvariantCulture);
            if (SecondsWaited >= SecondsToWait)
            {
                StopTimer();
                RunTests();
            }
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void StopTimer()
        {
            Timer.Stop();
            pnlAutoRun.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Run the unit tests.
        /// </summary>
        private void RunTests()
        {
            // Update the history
            string tag = txtTag.Text;
            TagHistory.Remove(tag);
            TagHistory.Insert(0, tag);
            while (TagHistory.Count > 5)
            {
                TagHistory.RemoveAt(5);
            }
            IsolatedStorageSettings.SiteSettings[TagHistoryKey] = TagHistory;

            // Launch Ignite with the test tag
            UnitTestSettings settings = UnitTestSystem.CreateDefaultSettings();
            settings.TagExpression = tag;
            Root.Children.Clear();
            Root.Children.Add(UnitTestSystem.CreateTestPage(settings));
        }
    }
}