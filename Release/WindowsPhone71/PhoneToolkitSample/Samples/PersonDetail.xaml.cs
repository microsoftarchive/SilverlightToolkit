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
using Microsoft.Phone.Controls;
using PhoneToolkitSample.Data;
using System.Windows.Navigation;

namespace PhoneToolkitSample.Samples
{
    public partial class PersonDetail : PhoneApplicationPage
    {
        public PersonDetail()
        {
            InitializeComponent();

            quote.Text = 
                LoremIpsum.GetParagraph(4) + System.Environment.NewLine + System.Environment.NewLine + 
                LoremIpsum.GetParagraph(8) + System.Environment.NewLine + System.Environment.NewLine + 
                LoremIpsum.GetParagraph(6);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string idParam;
            if (NavigationContext.QueryString.TryGetValue("ID", out idParam))
            {
                int id = Int32.Parse(idParam);
                DataContext = AllPeople.Current[id];
            }
        }
    }
}