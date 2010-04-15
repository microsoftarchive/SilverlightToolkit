// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Windows.Controls.UnitTests
{
    public class PageThatRecordsVirtuals : Page
    {
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (NavigationEventRecordList.GetShouldRecord(this.NavigationService.Host))
            {
                NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(this.GetType(), "OnNavigatingFrom", e.Uri));
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (NavigationEventRecordList.GetShouldRecord(this.NavigationService.Host))
            {
                NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(this.GetType(), "OnNavigatedFrom", e.Uri));
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationEventRecordList.GetShouldRecord(this.NavigationService.Host))
            {
                NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(this.GetType(), "OnNavigatedTo", e.Uri));
            }
        }

        protected override void OnFragmentNavigation(System.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            base.OnFragmentNavigation(e);
            if (NavigationEventRecordList.GetShouldRecord(this.NavigationService.Host))
            {
                NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(this.GetType(), "OnFragmentNavigation", new Uri("#" + e.Fragment, UriKind.Relative)));
            }
        }
    }
}
