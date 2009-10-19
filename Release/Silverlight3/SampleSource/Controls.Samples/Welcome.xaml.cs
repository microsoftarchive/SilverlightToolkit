// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

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
    [Sample("Welcome", DifficultyLevel.None)]
    [Category("Controls")]
    public partial class Welcome : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the Welcome sample page.
        /// </summary>
        public Welcome()
        {
            InitializeComponent();
        }
    }
}