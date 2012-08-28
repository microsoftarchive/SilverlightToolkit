// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Globalization;

namespace Microsoft.Phone.Testing.Client
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
    /// Gets or sets the tag history.
    /// </summary>
    private List<string> TagHistory { get; set; }

    /// <summary>
    /// Holds a reference to the Application Bar run button.
    /// </summary>
    private ApplicationBarIconButton _runButton;

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
        if (TagHistory.Count == 0)
        {
          SuggestionsListBox.Items.Clear();
        }
        else
        {
          List<ListBoxItem> itemsToRemove = new List<ListBoxItem>();
          foreach (ListBoxItem item in SuggestionsListBox.Items)
          {
            if (item.Content is StackPanel)
            {
              itemsToRemove.Add(item);
            }
          }

          foreach (ListBoxItem item in itemsToRemove)
          {
            SuggestionsListBox.Items.Remove(item);
          }
        }
        foreach (string sample in sampleTags)
        {
          SuggestionsListBox.Items.Add(CreateSuggestedTag(sample));
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the TagEditor type.
    /// </summary>
    /// <param name="initialTagExpression">The tag expression to use.</param>
    public TagEditor(string initialTagExpression)
      : this()
    {
      if (!string.IsNullOrEmpty(initialTagExpression))
      {
        TagExpressionTextBox.Text = initialTagExpression;
      }
    }

    /// <summary>
    /// Initializes a new instance of the TagEditor type.
    /// </summary>
    public TagEditor()
    {
      Microsoft.Phone.Controls.LocalizedResources.ControlResources.Culture = CultureInfo.InvariantCulture;

      InitializeComponent();



      // Get the tag history
      List<string> history;
      IsolatedStorageSettings.ApplicationSettings.TryGetValue(TagHistoryKey, out history);
      TagHistory = history;
      if (TagHistory == null)
      {
        TagHistory = new List<string>();
      }

      // Fill in the history list
      if (TagHistory.Count > 0)
      {
        // Show the Tag history list in reverse order (recently added entries first)
        for (int i = TagHistory.Count - 1; i > -1; i--)
        {
          SuggestionsListBox.Items.Insert(0, CreateSuggestedTag(TagHistory[i]));
        }
      }

      // Configure the Application Bar button.
      UIElement rootVisual = Application.Current.RootVisual;
      if (rootVisual is PhoneApplicationFrame)
      {
        IApplicationBar appBar;
        PhoneApplicationPage mainPage = (((PhoneApplicationFrame)rootVisual).Content as PhoneApplicationPage);

        // Replace the application bar.
        appBar = new ApplicationBar();
        mainPage.ApplicationBar = appBar;
        appBar.IsMenuEnabled = false;

        _runButton = new ApplicationBarIconButton();
        _runButton.IconUri = new Uri("/Toolkit.Content/appbar.transport.play.rest.png", UriKind.Relative);
        _runButton.Text = "run tests";
        _runButton.Click += OnRunButtonClick;
        _runButton.IsEnabled = true;

        appBar.Buttons.Add(_runButton);
      }
    }

    /// <summary>
    /// Handle the click event for the Run Tests button.
    /// </summary>
    /// <param name="sender">Run Button.</param>
    /// <param name="e">Event arguments.</param>
    private void OnRunButtonClick(object sender, EventArgs e)
    {
      RunTests();
    }

    /// <summary>
    /// Fires the Complete event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnComplete(TagExpressionEventArgs e)
    {
      EventHandler<TagExpressionEventArgs> handler = Complete;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    /// <summary>
    /// Creates a new suggested tag item for sample and previous tags.
    /// </summary>
    /// <param name="content">The tag content.</param>
    /// <returns>Returns a new instance.</returns>
    private ListBoxItem CreateSuggestedTag(string content)
    {
      ListBoxItem tagItem = new ListBoxItem();
      TextBlock tagTextBlock = new TextBlock();
      tagTextBlock.Style = Application.Current.Resources["PhoneTextLargeStyle"] as Style;
      tagTextBlock.Text = content;
      tagTextBlock.Margin = new Thickness(12, 0, 0, 17);
      tagItem.Content = tagTextBlock;
      return tagItem;
    }

    /// <summary>
    /// Handles the selection of a tag expression from the list.
    /// </summary>
    /// <param name="sender">The <see cref="T:System.Windows.Controls.ListBox"/> that fires the event.</param>
    /// <param name="e">Information about the tap event.</param>
    private void OnSampleTagTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      ListBox listBox = sender as ListBox;
      if (listBox == null)
      {
        return;
      }

      string tag = "";

      ListBoxItem selectedItem = listBox.SelectedItem as ListBoxItem;

      if (selectedItem != null)
      {
        // if part of the initial samples
        if (selectedItem.Content is StackPanel)
        {
          tag = ((selectedItem.Content as StackPanel).Children[0] as TextBlock).Text;
        }
        // if part of the history or pre-configured
        if (selectedItem.Content is TextBlock)
        {
          tag = (selectedItem.Content as TextBlock).Text;
        }
      }

      if (string.IsNullOrEmpty(tag))
      {
        return;
      }

      TagExpressionTextBox.Text = tag;
    }

    /// <summary>
    /// Handles the selection of test execution schema.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TagsToggle_Checked(object sender, RoutedEventArgs e)
    {
      TagExpressionTextBox.Text = (TagHistory.Count > 0) ? TagHistory[0] : "All";
    }

    /// <summary>
    /// Handles the selection of test execution schema.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TagsToggle_Unchecked(object sender, RoutedEventArgs e)
    {
      _runButton.IsEnabled = true;
    }

    /// <summary>
    /// Run the unit tests.
    /// </summary>
    private void RunTests()
    {
      // Update the history
      string tag = TagExpressionTextBox.Text;

      if (!string.IsNullOrEmpty(tag))
      {
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
    /// Handles the changes on the tag expression to run tests.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TagExpressionTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (String.IsNullOrEmpty(TagExpressionTextBox.Text))
        _runButton.IsEnabled = false;
      else
        _runButton.IsEnabled = true;
    }

  }
}