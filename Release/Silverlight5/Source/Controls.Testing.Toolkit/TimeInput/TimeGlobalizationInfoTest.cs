// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Silverlight.Testing;
using System.Collections.Generic;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests the TimeGlobalizationInfo class.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    public class TimeGlobalizationInfoTest : TestBase
    {
        /// <summary>
        /// Tests that formats are sanitized correctly.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [Description("Tests that formats are sanitized correctly.")]
        public virtual void ShouldAllowSpecificCharactersInFormats()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            Assert.AreEqual("hh:mm.ss", strategy.GetTransformedFormat("hh:mm.ss zzzz"));
        }

        /// <summary>
        /// Tests that the catchall parser is set as default.
        /// </summary>
        [TestMethod]
        [Description("Tests that the catchall parser is set as default.")]
        public virtual void ShouldHaveCatchallParser()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();

            Assert.IsTrue(strategy.GetActualTimeParsers(null).Count() == 1);
        }

        /// <summary>
        /// Tests that default strategy passes the parsers.
        /// </summary>
        [TestMethod]
        [Description("Tests that default strategy passes a copy of the parsers.")]
        public virtual void ShouldPassThroughCopyOfParsers()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();

            List<TimeParser> sourceParsers = new List<TimeParser>();
            sourceParsers.Add(new CustomTimeParser());

            Assert.AreNotSame(sourceParsers, strategy.GetActualTimeParsers(sourceParsers));
            Assert.IsTrue(strategy.GetActualTimeParsers(sourceParsers).Count() == 2);
        }

        /// <summary>
        /// Tests that a datetime is correctly formatted.
        /// </summary>
        [TestMethod]
        [Description("Tests that a datetime is correctly formatted.")]
        public virtual void ShouldFormatDateTimeAccordingToTimeFormat()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Assert.AreEqual("8:03 AM", strategy.FormatTime(new DateTime(1900, 2, 3, 8, 3, 12), new ShortTimeFormat()));
                Assert.AreEqual("8:03:12 AM", strategy.FormatTime(new DateTime(1900, 2, 3, 8, 3, 12), new LongTimeFormat()));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            Assert.AreEqual("08.03.12", strategy.FormatTime(new DateTime(1900, 2, 3, 8, 3, 12), new CustomTimeFormat("HH.mm.ss")));
        }

        /// <summary>
        /// Tests that a datetime format can be filtered.
        /// </summary>
        [TestMethod]
        [Description("Tests that a datetime format can be filtered.")]
        public virtual void ShouldFormatDateTimeWithFilter()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Assert.AreEqual("8 AM", strategy.FormatTime(new DateTime(1900, 2, 3, 8, 3, 12), new ShortTimeFormat(), 'H', 'h', 't', ' '));
                Assert.AreEqual("03", strategy.FormatTime(new DateTime(1900, 2, 3, 8, 3, 12), new LongTimeFormat(), 'm'));

                Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
                Assert.IsTrue(strategy.FormatTime(new DateTime(1900, 2, 3, 8, 3, 12), new ShortTimeFormat(), 'H', 'h', 't', ' ').EndsWith("8", StringComparison.Ordinal));
                Assert.AreEqual("03", strategy.FormatTime(new DateTime(1900, 2, 3, 8, 3, 12), new LongTimeFormat(), 'm'));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Tests that ArgumentNullException is thrown when no TimeFormat is 
        /// passed.
        /// </summary>
        [TestMethod]
        [Description("Tests that ArgumentNullException is thrown when no TimeFormat is passed.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ShouldValidateTimeFormatWhenFormatting()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            strategy.FormatTime(DateTime.Now, null);
        }

        /// <summary>
        /// Tests that a string can be parsed.
        /// </summary>
        [TestMethod]
        [Description("Tests that a string can be parsed.")]
        public virtual void ShouldParseTextToDateTime()
        {
            // culture is internal, need to test with a known culture.
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
                Assert.IsTrue(strategy.ActualCulture.Name == "en-US", "This test should run under en-US culture, setting did not work as expected.");

                TimeParserCollection parsers = new TimeParserCollection { new CustomTimeParser() };

                Assert.AreEqual(DateTime.Today.Date.AddHours(8).AddMinutes(30), strategy.ParseTime("8:30 AM", new ShortTimeFormat(), parsers));
                Assert.AreEqual(DateTime.Today.Date.AddHours(20).AddMinutes(30), strategy.ParseTime("8:30 PM", new ShortTimeFormat(), parsers));

                Assert.AreEqual(DateTime.Today.Date.AddHours(8).AddMinutes(30).AddSeconds(12), strategy.ParseTime("8:30:12 AM", new LongTimeFormat(), parsers));
                Assert.AreEqual(DateTime.Today.Date.AddHours(20).AddMinutes(30).AddSeconds(12), strategy.ParseTime("8:30:12 PM", new LongTimeFormat(), parsers));

                Assert.AreEqual(DateTime.Today.Date.AddHours(8).AddMinutes(30).AddSeconds(12), strategy.ParseTime("08:30:12", new CustomTimeFormat("hh:mm:ss"), parsers));

                Assert.AreEqual(DateTime.MinValue.Date.AddHours(15).AddMinutes(40), strategy.ParseTime("3 uur 40", new CustomTimeFormat("hh:mm:ss"), parsers));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Tests that parsing throws exception if it can't parse.
        /// </summary>
        [TestMethod]
        [Description("Tests that parsing throws exception if it can't parse.")]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void ShouldThrowExceptionDuringParse()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            strategy.ParseTime("cannot parse this", new LongTimeFormat(), null);
        }

        /// <summary>
        /// Tests that parsing throws exception if format is null.
        /// </summary>
        [TestMethod]
        [Description("Tests that parsing throws exception if format is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ShouldThrowExceptionDuringParseIfFormatIsNull()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            strategy.ParseTime("4:12 AM", null, null);
        }

        /// <summary>
        /// Tests determining correct timeunit at positions.
        /// </summary>
        [TestMethod]
        [Description("Tests determining correct timeunit at positions.")]
        public virtual void ShouldDetermineTimeUnitAtPositionsInString()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            string date = "04:12:22";
            ITimeFormat military = new CustomTimeFormat("HH:mm:ss");

            Assert.AreEqual(TimeSpan.FromHours(1), strategy.GetTimeUnitAtTextPosition(date, 0, military));
            Assert.AreEqual(TimeSpan.FromHours(1), strategy.GetTimeUnitAtTextPosition(date, 1, military));
            Assert.AreEqual(TimeSpan.FromHours(1), strategy.GetTimeUnitAtTextPosition(date, 2, military));
            Assert.AreEqual(TimeSpan.FromMinutes(10), strategy.GetTimeUnitAtTextPosition(date, 3, military));
            Assert.AreEqual(TimeSpan.FromMinutes(1), strategy.GetTimeUnitAtTextPosition(date, 4, military));
            Assert.AreEqual(TimeSpan.FromMinutes(1), strategy.GetTimeUnitAtTextPosition(date, 5, military));
            Assert.AreEqual(TimeSpan.FromSeconds(10), strategy.GetTimeUnitAtTextPosition(date, 6, military));
            Assert.AreEqual(TimeSpan.FromSeconds(1), strategy.GetTimeUnitAtTextPosition(date, 7, military));
            Assert.AreEqual(TimeSpan.FromSeconds(1), strategy.GetTimeUnitAtTextPosition(date, 8, military));

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                date = "4:12:22 PM";
                ITimeFormat longFormat = new LongTimeFormat();

                Assert.AreEqual(TimeSpan.FromHours(1), strategy.GetTimeUnitAtTextPosition(date, 0, longFormat));
                Assert.AreEqual(TimeSpan.FromSeconds(1), strategy.GetTimeUnitAtTextPosition(date, 7, longFormat));
                Assert.AreEqual(TimeSpan.FromHours(12), strategy.GetTimeUnitAtTextPosition(date, 8, longFormat));
                Assert.AreEqual(TimeSpan.FromHours(12), strategy.GetTimeUnitAtTextPosition(date, 9, longFormat));
                Assert.AreEqual(TimeSpan.FromHours(12), strategy.GetTimeUnitAtTextPosition(date, 10, longFormat));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Tests that time unit is returned that is closest to caret when at whitespace.
        /// </summary>
        [TestMethod]
        [Description("Tests that time unit is returned that is closest to caret when at whitespace.")]
        public virtual void ShouldUseTimeUnitClosestAtCaretWhenAtWhiteSpace()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                string date = "4:12   PM";
                ITimeFormat longFormat = new CustomTimeFormat("h:mm   tt");

                Assert.AreEqual(TimeSpan.FromMinutes(1), strategy.GetTimeUnitAtTextPosition(date, 4, longFormat));
                Assert.AreEqual(TimeSpan.FromMinutes(1), strategy.GetTimeUnitAtTextPosition(date, 5, longFormat));
                Assert.AreEqual(TimeSpan.FromHours(12), strategy.GetTimeUnitAtTextPosition(date, 6, longFormat));
                Assert.AreEqual(TimeSpan.FromHours(12), strategy.GetTimeUnitAtTextPosition(date, 7, longFormat));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Tests that determining timeunit without a format will throw exception.
        /// </summary>
        [TestMethod]
        [Description("Tests that determining timeunit without a format will throw exception.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ShouldThrowExceptionDuringDeterminingTimeUnit()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            strategy.ParseTime("4:12:22 AM", null, null);
        }

        /// <summary>
        /// Tests that position in text is determined correctly given a TimeSpan.
        /// </summary>
        [TestMethod]
        [Description("Tests that position in text is determined correctly given a TimeSpan.")]
        public virtual void ShouldDetermineTextPositionInDateTime()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            string date = "04:12:22";
            ITimeFormat military = new CustomTimeFormat("HH:mm:ss");

            Assert.AreEqual(0, strategy.GetTextPositionForTimeUnit(date, TimeSpan.FromHours(1), military));
            Assert.AreEqual(3, strategy.GetTextPositionForTimeUnit(date, TimeSpan.FromMinutes(10), military));
            Assert.AreEqual(4, strategy.GetTextPositionForTimeUnit(date, TimeSpan.FromMinutes(1), military));
            Assert.AreEqual(6, strategy.GetTextPositionForTimeUnit(date, TimeSpan.FromSeconds(10), military));
            Assert.AreEqual(7, strategy.GetTextPositionForTimeUnit(date, TimeSpan.FromSeconds(1), military));

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                date = "4:12:22 PM";
                ITimeFormat longFormat = new LongTimeFormat();

                Assert.AreEqual(0, strategy.GetTextPositionForTimeUnit(date, TimeSpan.FromHours(1), longFormat));
                Assert.AreEqual(8, strategy.GetTextPositionForTimeUnit(date, TimeSpan.FromHours(12), longFormat));
                Assert.AreEqual(8, strategy.GetTextPositionForTimeUnit(date, TimeSpan.FromHours(12), longFormat));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Tests that local AM/PM values are honored.
        /// </summary>
        [TestMethod]
        [Description("Tests that local AM/PM values are honored.")]
        public virtual void ShouldUseCultureSpecificTimeDesignators()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ko-KR");
                Assert.AreEqual("오전", strategy.AMDesignator);
                Assert.AreEqual("오후", strategy.PMDesignator);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Tests that local cultures is used during formatting.
        /// </summary>
        [TestMethod]
        [Description("Tests that local cultures is used during formatting.")]
        public virtual void ShouldUseCultureDuringFormatting()
        {
            OverriddenTimeGlobalizationInfo strategy = new OverriddenTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ko-KR");
                string formatted = strategy.FormatTime(new DateTime(1900, 1, 1, 5, 5, 5), new ShortTimeFormat());
                Assert.AreEqual("오전 5:05", formatted);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Tests mapping of global characters.
        /// </summary>
        [TestMethod]
        [Description("Tests mapping of global characters.")]
        public virtual void ShouldMapGlobalCharacters()
        {
            ArabicTimeGlobalizationInfo strategy = new ArabicTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
                string formatted = strategy.FormatTime(new DateTime(1900, 1, 1, 5, 6, 7), new CustomTimeFormat("HH:mm:ss"));
                Assert.AreEqual("٠٥:٠٦:٠٧", formatted);

                DateTime? result;
                Assert.IsTrue(strategy.TryParseTime("٠٥:٠٦:٠٧", new CustomTimeFormat("HH:mm:ss"), null, out result));
                Assert.AreEqual(new DateTime(1900, 1, 1, 5, 6, 7).TimeOfDay, result.Value.TimeOfDay);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Tests that Arabic culture works.
        /// </summary>
        [TestMethod]
        [Description("Tests that Arabic culture works.")]
        public virtual void ShouldHandleArabicCulture()
        {
            ArabicTimeGlobalizationInfo strategy = new ArabicTimeGlobalizationInfo();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ar-EG");
                string formatted = strategy.FormatTime(new DateTime(1900, 1, 1, 14, 2, 0), new CustomTimeFormat("HH:mm:ss"));
                Assert.AreEqual("١٤:٠٢:٠٠", formatted);

                DateTime? result;
                Assert.IsTrue(strategy.TryParseTime("١٤:٠٢:٠٠", new CustomTimeFormat("HH:mm:ss"), null, out result));
                Assert.AreEqual(new DateTime(1900, 1, 1, 14, 2, 0).TimeOfDay, result.Value.TimeOfDay);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }
    }
}
