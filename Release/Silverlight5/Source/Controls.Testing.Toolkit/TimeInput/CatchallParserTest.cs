// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the catch all parser.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    public class CatchallParserTest
    {
        /// <summary>
        /// Breadth test for the catch all parser.
        /// </summary>
        [TestMethod]
        [Description("Breadth test for the catch all parser.")]
        public virtual void ShouldParse()
        {
            CatchallTimeParser parser = new CatchallTimeParser();
            CultureInfo culture = new CultureInfo("en-US");
            
            Dictionary<string, DateTime> inputOutput =
                new Dictionary<string, DateTime>()
                    {
                        // regular hours
                        { "0", GetDate(0, 0, 0) },
                        { "1", GetDate(1, 0, 0) },
                        { "2", GetDate(2, 0, 0) },
                        { "3", GetDate(3, 0, 0) },
                        { "4", GetDate(4, 0, 0) },
                        { "9", GetDate(9, 0, 0) },
                        { "10", GetDate(10, 0, 0) },
                        { "11", GetDate(11, 0, 0) },
                        { "20", GetDate(20, 0, 0) },
                        { "21", GetDate(21, 0, 0) },
                        { "23", GetDate(23, 0, 0) },
                        { "00", GetDate(0, 0, 0) },
                        { "01", GetDate(1, 0, 0) },
                        { "02", GetDate(2, 0, 0) },
                        { "03", GetDate(3, 0, 0) },
                        { "09", GetDate(9, 0, 0) },
                        // hours is too big
                        { "25", GetDate(2, 5, 0) },
                        { "250", GetDate(2, 50, 0) },
                        { "35", GetDate(3, 5, 0) },
                        { "45", GetDate(4, 5, 0) },
                        { "95", GetDate(9, 5, 0) },
                        // pm designator
                        { "95p", GetDate(21, 5, 0) },
                        { "21p", GetDate(21, 0, 0) },
                        // regular minutes
                        { "60", GetDate(6, 0, 0) },
                        { "61", GetDate(6, 1, 0) },
                        { "69", GetDate(6, 9, 0) },
                        { "601", GetDate(6, 1, 0) },
                        { "611", GetDate(6, 11, 0) },
                        { "659", GetDate(6, 59, 0) },
                        // regular seconds
                        { "6011", GetDate(6, 1, 1) },
                        { "6012", GetDate(6, 1, 2) },
                        { "6019", GetDate(6, 1, 9) },
                        { "60110", GetDate(6, 1, 10) },
                        { "60119", GetDate(6, 1, 19) },
                        { "60159", GetDate(6, 1, 59) },
                    };

            foreach (KeyValuePair<string, DateTime> pair in inputOutput)
            {
                DateTime? result = null;
                Assert.IsTrue(parser.TryParse(pair.Key, culture, out result), String.Format(CultureInfo.InvariantCulture, "Could not parse {0}", pair.Key));
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Value.TimeOfDay, pair.Value.TimeOfDay);
            }
        }

        /// <summary>
        /// Helper function to quickly create a date.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <param name="minutes">The minutes.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns>A Date that corresponds to the hours, minutes and seconds
        /// passed in.</returns>
        private static DateTime GetDate(int hours, int minutes, int seconds)
        {
            return DateTime.Now.Date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        }
    }
}
