// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the ContextMenu.
    /// </summary>
    [Sample("ContextMenu", DifficultyLevel.Basic, "ContextMenu")]
    public partial class ContextMenuSample : UserControl
    {
        /// <summary>
        /// Stores a reference to a random number generator (initialized to a constant seed value).
        /// </summary>
        private readonly Random _rand = new Random(0);

        /// <summary>
        /// Stores a reference to the collection of sample Email objects.
        /// </summary>
        private readonly ObservableCollection<Email> _emails = new ObservableCollection<Email>();

        /// <summary>
        /// Gets an ICommand instance for changing the font face of a control.
        /// </summary>
        public ICommand ChangeFontFaceCommand { get; private set; }

        /// <summary>
        /// Gets an ICommand instance for changing the font size of a control.
        /// </summary>
        public ICommand ChangeFontSizeCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ContextMenuSample class.
        /// </summary>
        public ContextMenuSample()
        {
            InitializeComponent();

            // Initialize variables
            EmailBody.DataContext = this;
            ChangeFontFaceCommand = new ChangeControlFontFaceCommand(EmailBody);
            ChangeFontSizeCommand = new ChangeControlFontSizeCommand(EmailBody);

            // Suppress Silverlight right-click menu for clicks on ListBoxItem elements
            EmailList.MouseRightButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
            };

            // Create sample emails
            for (int i = 1; i < 16; i++)
            {
                _emails.Add(new Email { Subject = "Email Number " + i, Body = CreateRandomText(_rand.Next(20, 300)) });
            }
            EmailList.ItemsSource = _emails;

            // Select the top email
            EmailList.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles clicks on the Delete context menu item.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Email email = ((MenuItem)sender).DataContext as Email;
            if (null != email)
            {
                _emails.Remove(email);
            }
        }

        /// <summary>
        /// Handles clicks on the Move Up context menu item.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            Email email = ((MenuItem)sender).DataContext as Email;
            if (null != email)
            {
                int index = _emails.IndexOf(email);
                if (0 <= index - 1)
                {
                    // .Move is simpler, but is not available on Silverlight
                    // _emails.Move(index, index - 1);
                    bool wasSelected = EmailList.SelectedItem == email;
                    _emails.RemoveAt(index);
                    _emails.Insert(index - 1, email);
                    if (wasSelected)
                    {
                        EmailList.SelectedItem = email;
                    }
                }
            }
        }

        /// <summary>
        /// Handles clicks on the Move Down context menu item.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            Email email = ((MenuItem)sender).DataContext as Email;
            if (null != email)
            {
                int index = _emails.IndexOf(email);
                if (index + 1 < _emails.Count)
                {
                    // .Move is simpler, but is not available on Silverlight
                    // _emails.Move(index, index + 1);
                    bool wasSelected = EmailList.SelectedItem == email;
                    _emails.RemoveAt(index);
                    _emails.Insert(index + 1, email);
                    if (wasSelected)
                    {
                        EmailList.SelectedItem = email;
                    }
                }
            }
        }

        /// <summary>
        /// Creates an arbitrary amount of random text in paragraph form.
        /// </summary>
        /// <param name="words">Number of words to create.</param>
        /// <returns>String of random text.</returns>
        private string CreateRandomText(int words)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < words; i++)
            {
                sb.Append(_rand.NextDouble() < 0.05 ? "\n\n" : " ");
                sb.Append("text");
            }
            return sb.ToString().TrimStart();
        }
    }
}
