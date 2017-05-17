// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// The Welcome page is placed at the top of the samples list and is shown 
    /// when the page initially loads.
    /// </summary>
    /// <remarks>The SampleAttribute value is prefixed with a period to enable 
    /// it to show up at the top of the samples list. The period is removed in 
    /// the sample browser control.</remarks>
    [Sample("Welcome", DifficultyLevel.None, "Controls")]
    public partial class Welcome : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the Welcome sample page.
        /// </summary>
        public Welcome()
        {
            InitializeComponent();
            OnInstallStateChanged(this, EventArgs.Empty);

            // Make sure that we are connected to the installation state change
            // at least once.
            Application.Current.InstallStateChanged += OnInstallStateChanged;
        }

        /// <summary>
        /// Shows or hides the installation button.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnInstallStateChanged(object sender, EventArgs e)
        {
            switch (App.Current.InstallState)
            {
                case InstallState.Installing:
                    InstallButton.Visibility = Visibility.Visible;
                    InstallButton.IsEnabled = false;
                    InstallButton.Content = "Installing...";
                    break;

                case InstallState.Installed:
                    InstallButton.Visibility = Visibility.Collapsed;
                    InstallText.Text = "The samples have been installed on this computer.";
                    break;

                case InstallState.NotInstalled:
                    InstallButton.Visibility = Visibility.Visible;
                    InstallButton.IsEnabled = true;
                    break;

                case InstallState.InstallFailed:
                    InstallButton.Visibility = Visibility.Collapsed;
                    InstallText.Text = "The Out of Browser installation failed.";
                    break;
            }

            InstallApplicationPanel.Visibility = App.Current.IsRunningOutOfBrowser ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Installs the application.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event connected in XAML.")]
        private void InstallSamplesClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Install();
        }
    }
}