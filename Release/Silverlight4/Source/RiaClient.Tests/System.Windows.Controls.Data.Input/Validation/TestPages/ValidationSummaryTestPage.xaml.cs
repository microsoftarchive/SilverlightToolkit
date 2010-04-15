//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls.UnitTests
{
    public partial class ValidationSummaryTestPage : UserControl
    {
        private Customer _c;
        public ValidationSummaryTestPage()
        {
            InitializeComponent();
            _c = new Customer();
            this.DataContext = _c;
        }

        public Customer Customer
        {
            get { return _c; }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
