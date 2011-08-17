// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;

namespace PhoneToolkitSample.Data
{
    public class DateTimeCollection : ObservableCollection<DateTimeObject>
    {
        public DateTimeCollection()
            : base()
        {
            Add(new DateTimeObject(DateTime.Now.AddYears(-1)));
            Add(new DateTimeObject(DateTime.Now.AddMonths(-10)));
            Add(new DateTimeObject(DateTime.Now.AddMonths(-3)));
            Add(new DateTimeObject(DateTime.Now.AddMonths(-1)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-28)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-21)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-7)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-6)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-5)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-4)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-3)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-2)));
            Add(new DateTimeObject(DateTime.Now.AddDays(-1)));
            Add(new DateTimeObject(DateTime.Now.AddHours(-22)));
            Add(new DateTimeObject(DateTime.Now.AddHours(-21)));
            Add(new DateTimeObject(DateTime.Now.AddHours(-14)));
            Add(new DateTimeObject(DateTime.Now.AddHours(-11)));
            Add(new DateTimeObject(DateTime.Now.AddHours(-7)));
            Add(new DateTimeObject(DateTime.Now.AddHours(-3).AddMinutes(-10)));
            Add(new DateTimeObject(DateTime.Now.AddHours(-3)));
            Add(new DateTimeObject(DateTime.Now.AddMinutes(-59)));
            Add(new DateTimeObject(DateTime.Now.AddMinutes(-30)));
            Add(new DateTimeObject(DateTime.Now.AddMinutes(-1)));
            Add(new DateTimeObject(DateTime.Now.AddSeconds(-3)));
            Add(new DateTimeObject(DateTime.Now));
        }
    }
}
