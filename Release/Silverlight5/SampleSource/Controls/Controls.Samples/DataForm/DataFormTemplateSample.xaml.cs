// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DataForm with templates.
    /// </summary>
    [Sample("Template-driven DataForm", DifficultyLevel.Intermediate, "DataForm")]
    public partial class DataFormTemplateSample : UserControl
    {
        /// <summary>
        /// Initializes a DataFormTemplateSample.
        /// </summary>
        public DataFormTemplateSample()
        {
            InitializeComponent();
            DataContext = Contact.People;
        }
    }
}
