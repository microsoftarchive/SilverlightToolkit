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
    /// Tracks the popularity of Doodads.
    /// </summary>
    public class DoodadPopularityPollCollection : IEnumerable<PopularityPoll>
    {
        /// <summary>
        /// Returns a stream of popularity polls.
        /// </summary>
        /// <returns>A stream of popularity polls.</returns>
        public IEnumerator<PopularityPoll> GetEnumerator()
        {
            yield return new PopularityPoll { Date = DateTime.Parse("9/6/2008", CultureInfo.InvariantCulture), Percent = 24 };
            yield return new PopularityPoll { Date = DateTime.Parse("9/2/2008", CultureInfo.InvariantCulture), Percent = 22 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/31/2008", CultureInfo.InvariantCulture), Percent = 20 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/30/2008", CultureInfo.InvariantCulture), Percent = 23 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/23/2008", CultureInfo.InvariantCulture), Percent = 22 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/22/2008", CultureInfo.InvariantCulture), Percent = 23 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/21/2008", CultureInfo.InvariantCulture), Percent = 25 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/19/2008", CultureInfo.InvariantCulture), Percent = 27 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/17/2008", CultureInfo.InvariantCulture), Percent = 29 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/16/2008", CultureInfo.InvariantCulture), Percent = 29 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/15/2008", CultureInfo.InvariantCulture), Percent = 25 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/12/2008", CultureInfo.InvariantCulture), Percent = 28 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/10/2008", CultureInfo.InvariantCulture), Percent = 24 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/7/2008", CultureInfo.InvariantCulture), Percent = 26 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/3/2008", CultureInfo.InvariantCulture), Percent = 29 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/2/2008", CultureInfo.InvariantCulture), Percent = 30 };
            yield return new PopularityPoll { Date = DateTime.Parse("8/1/2008", CultureInfo.InvariantCulture), Percent = 31 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/28/2008", CultureInfo.InvariantCulture), Percent = 30 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/26/2008", CultureInfo.InvariantCulture), Percent = 32 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/25/2008", CultureInfo.InvariantCulture), Percent = 33 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/22/2008", CultureInfo.InvariantCulture), Percent = 37 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/19/2008", CultureInfo.InvariantCulture), Percent = 38 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/12/2008", CultureInfo.InvariantCulture), Percent = 35 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/11/2008", CultureInfo.InvariantCulture), Percent = 37 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/10/2008", CultureInfo.InvariantCulture), Percent = 39 };
            yield return new PopularityPoll { Date = DateTime.Parse("7/9/2008", CultureInfo.InvariantCulture), Percent = 38 };
        }

        /// <summary>
        /// Returns a stream of popularity polls.
        /// </summary>
        /// <returns>A stream of popularity polls.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<PopularityPoll>)this).GetEnumerator();
        }
    }
}