// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using System.Windows.Controls.Theming;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating StyleHelper control.
    /// </summary>
    [Sample("ImplicitStyleManager", DifficultyLevel.Basic)]
    [Category("ImplicitStyleManager")]
    public partial class ImplicitStyleManagerSample : UserControl
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        public ImplicitStyleManagerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Applies styles to a container manually.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Xaml uses it.")]
        private void ApplyStylesManuallyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ImplicitStyleManager.Apply(manualContainer);
        }

        /// <summary>
        /// Adds a button to a container to style it.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "XAML uses it.")]
        private void AddButtonToContainerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dynamicContainer.Children.Add(new Button { Content = "New styled button" });
        }
    }
}