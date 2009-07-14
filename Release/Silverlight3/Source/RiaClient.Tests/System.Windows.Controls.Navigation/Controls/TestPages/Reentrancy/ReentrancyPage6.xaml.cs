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
    public partial class ReentrancyPage6 : PageThatRecordsVirtuals
    {
        public ReentrancyPage6()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.Uri.OriginalString.EndsWith("4.xaml"))
            {
                e.Cancel = true;
            }
        }

    }
}
