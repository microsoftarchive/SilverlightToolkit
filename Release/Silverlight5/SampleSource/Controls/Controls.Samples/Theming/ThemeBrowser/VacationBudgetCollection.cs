// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Tracks vacation budget across a period of time.
    /// </summary>
    public class VacationBudgetCollection : IEnumerable<Budget>
    {
        /// <summary>
        /// Returns a stream of expenses across a period of time.
        /// </summary>
        /// <returns>A stream of expenses for this vacation.</returns>
        public IEnumerator<Budget> GetEnumerator()
        {
            yield return new Budget { Date = DateTime.Parse("9/6/2008", CultureInfo.InvariantCulture), ExpenseValue = 53 };
            yield return new Budget { Date = DateTime.Parse("9/2/2008", CultureInfo.InvariantCulture), ExpenseValue = 42 };
            yield return new Budget { Date = DateTime.Parse("8/31/2008", CultureInfo.InvariantCulture), ExpenseValue = 43 };
            yield return new Budget { Date = DateTime.Parse("8/30/2008", CultureInfo.InvariantCulture), ExpenseValue = 43 };
            yield return new Budget { Date = DateTime.Parse("8/23/2008", CultureInfo.InvariantCulture), ExpenseValue = 47 };
            yield return new Budget { Date = DateTime.Parse("8/22/2008", CultureInfo.InvariantCulture), ExpenseValue = 45 };
            yield return new Budget { Date = DateTime.Parse("8/21/2008", CultureInfo.InvariantCulture), ExpenseValue = 40 };
            yield return new Budget { Date = DateTime.Parse("8/19/2008", CultureInfo.InvariantCulture), ExpenseValue = 39 };
            yield return new Budget { Date = DateTime.Parse("8/17/2008", CultureInfo.InvariantCulture), ExpenseValue = 42 };
            yield return new Budget { Date = DateTime.Parse("8/16/2008", CultureInfo.InvariantCulture), ExpenseValue = 43 };
            yield return new Budget { Date = DateTime.Parse("8/15/2008", CultureInfo.InvariantCulture), ExpenseValue = 42 };
            yield return new Budget { Date = DateTime.Parse("8/12/2008", CultureInfo.InvariantCulture), ExpenseValue = 47 };
            yield return new Budget { Date = DateTime.Parse("8/10/2008", CultureInfo.InvariantCulture), ExpenseValue = 43 };
            yield return new Budget { Date = DateTime.Parse("8/7/2008", CultureInfo.InvariantCulture), ExpenseValue = 38 };
            yield return new Budget { Date = DateTime.Parse("8/3/2008", CultureInfo.InvariantCulture), ExpenseValue = 39 };
            yield return new Budget { Date = DateTime.Parse("8/2/2008", CultureInfo.InvariantCulture), ExpenseValue = 41 };
            yield return new Budget { Date = DateTime.Parse("8/1/2008", CultureInfo.InvariantCulture), ExpenseValue = 41 };
            yield return new Budget { Date = DateTime.Parse("7/28/2008", CultureInfo.InvariantCulture), ExpenseValue = 44 };
            yield return new Budget { Date = DateTime.Parse("7/26/2008", CultureInfo.InvariantCulture), ExpenseValue = 49 };
            yield return new Budget { Date = DateTime.Parse("7/25/2008", CultureInfo.InvariantCulture), ExpenseValue = 42 };
            yield return new Budget { Date = DateTime.Parse("7/22/2008", CultureInfo.InvariantCulture), ExpenseValue = 40 };
            yield return new Budget { Date = DateTime.Parse("7/19/2008", CultureInfo.InvariantCulture), ExpenseValue = 41 };
            yield return new Budget { Date = DateTime.Parse("7/12/2008", CultureInfo.InvariantCulture), ExpenseValue = 46 };
            yield return new Budget { Date = DateTime.Parse("7/11/2008", CultureInfo.InvariantCulture), ExpenseValue = 40 };
            yield return new Budget { Date = DateTime.Parse("7/10/2008", CultureInfo.InvariantCulture), ExpenseValue = 41 };
            yield return new Budget { Date = DateTime.Parse("7/9/2008", CultureInfo.InvariantCulture), ExpenseValue = 39 };
        }

        /// <summary>
        /// Returns a stream of vacation expenses.
        /// </summary>
        /// <returns>A stream of vacation expenses.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Budget>)this).GetEnumerator();
        }
    }
}