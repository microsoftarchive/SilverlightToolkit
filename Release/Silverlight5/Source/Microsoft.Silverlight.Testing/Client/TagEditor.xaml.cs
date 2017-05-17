// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// The TagEditor control provides a brief user interface allowing for the
    /// selection of a set of tests, used to filter the test run.
    /// </summary>
    public partial class TagEditor : UserControl
    {
        /// <summary>
        /// Key used to lookup the TagHistory site setting.
        /// </summary>
        private const string TagHistoryKey = "TagHistory";

        /// <summary>
        /// Key used to lookup whether the last run used tag expressions.
        /// </summary>
        private const string TagLastRunHistoryKey = "TagUsedLastRun";

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
        /// Backing field for the last run used value.
        /// </summary>
        private bool _lastRunUsedExpressions;

        /// <summary>
        /// An event that indicates that the tag editor is complete. This can be
        /// in response to many actions: the user entering a tag expression, the
        /// time expiring and the default being selected, or the selection being
        /// canceled.
        /// </summary>
        public event EventHandler<TagExpressionEventArgs> Complete;

        /// <summary>
        /// Initializes a new instance of the TagEditor type. Also includes a
        /// set of sample tags for display to the end user.
        /// </summary>
        /// <param name="initialTagExpression">The tag expression to use.</param>
        /// <param name="sampleTags">Sample tags to display.</param>
        public TagEditor(string initialTagExpression, IList<string> sampleTags)
            : this(initialTagExpression)
        {
            if (sampleTags != null && sampleTags.Count > 0)
            {
                SampleTags.Children.Clear();
                foreach (string sample in sampleTags)
                {
                    SampleTags.Children.Add(CreateTagButton(sample));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the TagEditor type.
        /// </summary>
        /// <param name="initialTagExpression">The tag expression to use.</param>
        public TagEditor(string initialTagExpression) : this()
        {
            if (!string.IsNullOrEmpty(initialTagExpression))
            {
                txtTag.Text = initialTagExpression;
            }
        }

        /// <summary>
        /// Initializes a new instance of the TagEditor type.
        /// </summary>
        public TagEditor()
        {
            InitializeComponent();

            if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue(TagLastRunHistoryKey, out _lastRunUsedExpressions))
            {
                _lastRunUsedExpressions = false;
            }

            Loaded += (x, xe) => 
            {
                Button button = _lastRunUsedExpressions ? runTestsButton : CancelButton;
                button.Focus();
            };
            CancelButton.FontWeight = _lastRunUsedExpressions ? FontWeights.Normal : FontWeights.Bold;
            runTestsButton.FontWeight = _lastRunUsedExpressions ? FontWeights.Bold : FontWeights.Normal;

            List<string> history;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(TagHistoryKey, out history);
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
                    pnlHistory.Children.Add(CreateTagButton(TagHistory[i]));
                }
            }

            // Setup the auto-run timer
            txtTime.Text = SecondsToWait.ToString(CultureInfo.InvariantCulture);
            SecondsWaited = 0;
            Timer = new DispatcherTimer();
            Timer.Tick += OnTimerTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        /// <summary>
        /// Handles the key down event.
        /// </summary>
        /// <param name="e">The key event arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OnCancelClicked(this, new RoutedEventArgs());
            }

            if (e.Key == Key.Enter)
            {
                if (_lastRunUsedExpressions)
                {
                    OnRunClicked(this, new RoutedEventArgs());
                }
                else
                {
                    OnCancelClicked(this, new RoutedEventArgs());
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Fires the Complete event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnComplete(TagExpressionEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings[TagLastRunHistoryKey] = _lastRunUsedExpressions;

            EventHandler<TagExpressionEventArgs> handler = Complete;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Creates a new button.
        /// </summary>
        /// <param name="content">The button content.</param>
        /// <returns>Returns a new Button instance.</returns>
        private Button CreateTagButton(string content)
        {
            Button button = new Button
            {
                Content = content,
                Style = pnlEditor.Resources["TagButton"] as Style,
            };
            button.Click += OnTagButtonClicked;
            return button;
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
                if (!_lastRunUsedExpressions)
                {
                    txtTag.Text = string.Empty;
                }
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
            bool useTag = !string.IsNullOrEmpty(tag);
            if (useTag)
            {
                _lastRunUsedExpressions = true;
                TagHistory.Remove(tag);
                TagHistory.Insert(0, tag);
                while (TagHistory.Count > 5)
                {
                    TagHistory.RemoveAt(5);
                }
                IsolatedStorageSettings.ApplicationSettings[TagHistoryKey] = TagHistory;
            }
            OnComplete(new TagExpressionEventArgs(tag));
        }

        /// <summary>
        /// Cancels the selection of a tag expression.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            txtTag.Text = string.Empty;
            _lastRunUsedExpressions = false;
            StopTimer();
            OnComplete(new TagExpressionEventArgs(null));
        }
    }
}