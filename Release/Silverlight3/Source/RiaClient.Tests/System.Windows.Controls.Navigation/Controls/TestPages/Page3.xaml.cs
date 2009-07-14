//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Collections.ObjectModel;

namespace System.Windows.Controls.UnitTests
{
    public partial class Page3 : Page
    {
        static int _instances;

        public Collection<string> VirtualsCalled { get; private set; }

        public Page3()
        {
            InitializeComponent();
            _instances++;
            _txtBlock.Text += string.Format(CultureInfo.InvariantCulture, ", Instance #{0}", _instances);
            VirtualsCalled = new Collection<string>();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            VirtualsCalled.Add("OnNavigatedTo");
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            VirtualsCalled.Add("OnNavigatingFrom");
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            VirtualsCalled.Add("OnNavigatedFrom");
        }

        protected override void OnFragmentNavigation(System.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            VirtualsCalled.Add("OnFragmentNavigation");
        }
    }
}
