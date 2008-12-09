// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// The Welcome page is placed at the top of the samples list and is shown 
    /// when the page initially loads.
    /// </summary>
    /// <remarks>The SampleAttribute value is prefixed with a period to enable 
    /// it to show up at the top of the samples list. The period is removed in 
    /// the sample browser control.</remarks>
    [Sample("Welcome", DifficultyLevel.None)]
    [Category("Controls")]
    public partial class Welcome : UserControl
    {
        /// <summary>
        /// A token used for replacement of the assembly name.
        /// </summary>
        private const string AssemblyToken = "{ASSEMBLY}";

        /// <summary>
        /// A token used for replacement of the sample assembly name.
        /// </summary>
        private const string SampleAssemblyToken = "{SAMPLE_ASSEMBLY}";

        /// <summary>
        /// Initializes a new instance of the Welcome sample page.
        /// </summary>
        public Welcome()
        {
            InitializeComponent();

            // Replace the text tokens if the sample assembly token is found
            Loaded += new RoutedEventHandler(Welcome_Loaded);
        }

        /// <summary>
        /// The loaded event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void Welcome_Loaded(object sender, RoutedEventArgs e)
        {
            string welcome = WelcomeText.Text;
            if (welcome.Contains(SampleAssemblyToken))
            {
                // Extract the sample assembly name
                Assembly me = typeof(Welcome).Assembly;
                string friendlyAssembly = me.FullName;
                int comma = friendlyAssembly.IndexOf(',');
                if (comma >= 0)
                {
                    friendlyAssembly = friendlyAssembly.Substring(0, comma);
                }

                // Update the text value
                welcome = welcome.Replace(SampleAssemblyToken, friendlyAssembly);

                // Extract and set the sample assembly name
                if (welcome.Contains(AssemblyToken))
                {
                    string assembly = friendlyAssembly.Replace(".Samples", "");
                    welcome = welcome.Replace(AssemblyToken, assembly);
                }

                WelcomeText.Text = welcome;
            }
        }
    }
}