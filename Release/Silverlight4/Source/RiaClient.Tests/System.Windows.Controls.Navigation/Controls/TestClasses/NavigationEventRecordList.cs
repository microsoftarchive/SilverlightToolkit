// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace System.Windows.Controls.UnitTests
{
    internal static class NavigationEventRecordList
    {
        public static readonly DependencyProperty ShouldRecordProperty =
            DependencyProperty.RegisterAttached(
                "ShouldRecord",
                typeof(bool),
                typeof(NavigationEventRecordList),
                null);

        public static bool GetShouldRecord(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(ShouldRecordProperty);
        }

        public static void SetShouldRecord(DependencyObject depObj, bool shouldRecord)
        {
            depObj.SetValue(ShouldRecordProperty, shouldRecord);
        }

        public static ObservableCollection<NavigationEventRecord> EventRecords { get; private set; }

        static NavigationEventRecordList()
        {
            Reset();
        }

        public static void Reset()
        {
            EventRecords = new ObservableCollection<NavigationEventRecord>();
        }
    }

    internal class NavigationEventRecord
    {
        public Type GeneratedBy { get; private set; }
        public string Name { get; private set; }
        public Uri Uri { get; private set; }

        public NavigationEventRecord(Type generatedBy, string name, Uri uri)
        {
            this.GeneratedBy = generatedBy;
            this.Name = name;
            this.Uri = uri;
        }
    }
}
