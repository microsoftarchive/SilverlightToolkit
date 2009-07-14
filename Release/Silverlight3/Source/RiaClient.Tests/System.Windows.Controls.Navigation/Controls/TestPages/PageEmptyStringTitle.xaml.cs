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
using System.Windows.Data;
using System.ComponentModel;

namespace System.Windows.Controls.UnitTests
{
    public partial class PageEmptyStringTitle : Page
    {
        public PageEmptyStringTitle()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(PageEmptyStringTitle_Loaded);
        }

        private void PageEmptyStringTitle_Loaded(object sender, RoutedEventArgs e)
        {
            // Perform tasks on load (ex: fetch data, inspect this.NavigationContext, etc.)...
        }

    }
}
