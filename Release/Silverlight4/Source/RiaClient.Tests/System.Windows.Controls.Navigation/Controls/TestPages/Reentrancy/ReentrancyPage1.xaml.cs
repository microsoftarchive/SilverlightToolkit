// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections.ObjectModel;

namespace System.Windows.Controls.UnitTests
{
    public partial class ReentrancyPage1 : PageThatRecordsVirtuals
    {
        private static readonly string TestPagesPath = @"/System.Windows.Controls.Navigation/Controls/TestPages/";
        private Uri ReentrancyUri(string pageNumber)
        {
            return new Uri(TestPagesPath + "Reentrancy/ReentrancyPage" + pageNumber + ".xaml", UriKind.Relative);
        }

        public ReentrancyPage1()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.NavigationService.Navigate(ReentrancyUri("2"));
        }
    }
}
