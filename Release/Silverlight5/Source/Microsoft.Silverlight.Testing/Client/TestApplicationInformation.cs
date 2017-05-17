// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// This class contains information about the test application and its
    /// deployment settings.
    /// </summary>
    public class TestApplicationInformation : PropertyChangedBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TestApplicationInformation()
        {
            Application.Current.InstallStateChanged += OnInstallStateChanged;
            OnInstallStateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the install state changed event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnInstallStateChanged(object sender, EventArgs e)
        {
            OutOfBrowserApplicationInstalled = Application.Current.InstallState == InstallState.Installed;
            NotifyPropertyChanged("InstallationStateText");
            NotifyPropertyChanged("SupportsOutOfBrowserAndNotInstalled");
        }

        /// <summary>
        /// Backing field for an installed property.
        /// </summary>
        private bool _installed;

        /// <summary>
        /// Gets a value indicating whether the test application is currently
        /// installed out of browser.
        /// </summary>
        public bool OutOfBrowserApplicationInstalled
        {
            get
            {
                return _installed;
            }

            private set
            {
                bool old = _installed;
                _installed = value;

                if (old != value)
                {
                    NotifyPropertyChanged("OutOfBrowserApplicationInstalled");
                }
            }
        }

        /// <summary>
        /// Gets the text to show the user the state of the out of browser test
        /// application.
        /// </summary>
        public string InstallationStateText
        {
            get
            {
                switch (Application.Current.InstallState)
                {
                    case InstallState.Installed:
                        return "Installed";

                    case InstallState.InstallFailed:
                        return "Install failed";

                    case InstallState.Installing:
                        return "Installing...";

                    case InstallState.NotInstalled:
                        return "Install test application";

                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether out of browser is both supported and
        /// not currently installed.
        /// </summary>
        public bool SupportsOutOfBrowserAndNotInstalled
        {
            get
            {
                return SupportsOutOfBrowser && !OutOfBrowserApplicationInstalled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether out of browser is supported by this
        /// test application.
        /// </summary>
        public bool SupportsOutOfBrowser
        {
            get
            {
                return Deployment.Current.OutOfBrowserSettings != null;
            }
        }
    }
}