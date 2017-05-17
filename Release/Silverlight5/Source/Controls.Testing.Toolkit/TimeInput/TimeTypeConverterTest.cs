// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TestDescription = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests TimeTypeConverter.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    public class TimeTypeConverterTest : TypeConverterTest<DateTime?>
    {
        /// <summary>
        /// Gets the TypeConverter to test.
        /// </summary>
        public override TypeConverter Converter
        {
            get { return new TimeTypeConverter(); }
        }

        /// <summary>
        /// Gets objects and their converted values.
        /// </summary>
        public override IEnumerable<KeyValuePair<object, DateTime?>> ExpectedValues
        {
            get
            {
                // short time
                yield return new KeyValuePair<object, DateTime?>("8:03 AM", DateTime.Now.Date.AddHours(8).AddMinutes(3));
                yield return new KeyValuePair<object, DateTime?>("2:03 PM", DateTime.Now.Date.AddHours(14).AddMinutes(3));
                // long time
                yield return new KeyValuePair<object, DateTime?>("8:03:12 AM", DateTime.Now.Date.AddHours(8).AddMinutes(3).AddSeconds(12));
                yield return new KeyValuePair<object, DateTime?>("2:03:12 PM", DateTime.Now.Date.AddHours(14).AddMinutes(3).AddSeconds(12));
                // short date plus short time
                yield return new KeyValuePair<object, DateTime?>("05/08/2000 8:03 AM", new DateTime(2000, 05, 08, 8, 3, 0));
                yield return new KeyValuePair<object, DateTime?>("05/08/2000 2:03 PM", new DateTime(2000, 05, 08, 14, 3, 0));
                // short date plus long time
                yield return new KeyValuePair<object, DateTime?>("05/08/2000 8:03:12 AM", new DateTime(2000, 05, 08, 8, 3, 12));
                yield return new KeyValuePair<object, DateTime?>("05/08/2000 2:03:12 PM", new DateTime(2000, 05, 08, 14, 3, 12));
                // military time short
                yield return new KeyValuePair<object, DateTime?>("08:03", DateTime.Now.Date.AddHours(8).AddMinutes(3));
                yield return new KeyValuePair<object, DateTime?>("14:03", DateTime.Now.Date.AddHours(14).AddMinutes(3));
                // military time long
                yield return new KeyValuePair<object, DateTime?>("08:03:12", DateTime.Now.Date.AddHours(8).AddMinutes(3).AddSeconds(12));
                yield return new KeyValuePair<object, DateTime?>("14:03:12", DateTime.Now.Date.AddHours(14).AddMinutes(3).AddSeconds(12));
                // short date plus military short time
                yield return new KeyValuePair<object, DateTime?>("05/08/2000 08:03", new DateTime(2000, 05, 08, 8, 3, 0));
                yield return new KeyValuePair<object, DateTime?>("05/08/2000 14:03", new DateTime(2000, 05, 08, 14, 3, 0));
                // short date plus military long time
                yield return new KeyValuePair<object, DateTime?>("05/08/2000 08:03:12", new DateTime(2000, 05, 08, 8, 3, 12));
                yield return new KeyValuePair<object, DateTime?>("05/08/2000 14:03:12", new DateTime(2000, 05, 08, 14, 3, 12));
                // null
                yield return new KeyValuePair<object, DateTime?>(null, null);
                yield return new KeyValuePair<object, DateTime?>(String.Empty, null);
                yield return new KeyValuePair<object, DateTime?>("", null);
            }
        }

        /// <summary>
        /// Gets objects and the types of exceptions they should throw.
        /// </summary>
        public override IEnumerable<KeyValuePair<object, Type>> ExpectedFailures
        {
            get
            {
                yield return new KeyValuePair<object, Type>(" ", typeof(FormatException));
                yield return new KeyValuePair<object, Type>("random text", typeof(FormatException));
                yield return new KeyValuePair<object, Type>(new object(), typeof(InvalidCastException));
            }
        }

        /// <summary>
        /// Gets types and whether they should be supported by the converter.
        /// </summary>
        public override IEnumerable<KeyValuePair<Type, bool>> CanConvertFrom
        {
            get
            {
                yield return new KeyValuePair<Type, bool>(typeof(short), false);
                yield return new KeyValuePair<Type, bool>(typeof(ushort), false);
                yield return new KeyValuePair<Type, bool>(typeof(int), false);
                yield return new KeyValuePair<Type, bool>(typeof(uint), false);
                yield return new KeyValuePair<Type, bool>(typeof(long), false);
                yield return new KeyValuePair<Type, bool>(typeof(ulong), false);
                yield return new KeyValuePair<Type, bool>(typeof(float), false);
                yield return new KeyValuePair<Type, bool>(typeof(double), false);
                yield return new KeyValuePair<Type, bool>(typeof(decimal), false);
                yield return new KeyValuePair<Type, bool>(typeof(string), true);
                yield return new KeyValuePair<Type, bool>(typeof(object), false);
                yield return new KeyValuePair<Type, bool>(typeof(byte), false);
                yield return new KeyValuePair<Type, bool>(typeof(sbyte), false);
                yield return new KeyValuePair<Type, bool>(null, false);
            }
        }

        /// <summary>
        /// Gets types and whether they should be supported by the converter.
        /// </summary>
        public override IEnumerable<KeyValuePair<Type, bool>> CanConvertTo
        {
            get
            {
                yield return new KeyValuePair<Type, bool>(typeof(double), false);
                yield return new KeyValuePair<Type, bool>(typeof(string), true);
                yield return new KeyValuePair<Type, bool>(typeof(object), true);
                yield return new KeyValuePair<Type, bool>(typeof(short), false);
                yield return new KeyValuePair<Type, bool>(typeof(ushort), false);
                yield return new KeyValuePair<Type, bool>(typeof(int), false);
                yield return new KeyValuePair<Type, bool>(typeof(uint), false);
                yield return new KeyValuePair<Type, bool>(typeof(long), false);
                yield return new KeyValuePair<Type, bool>(typeof(ulong), false);
                yield return new KeyValuePair<Type, bool>(typeof(float), false);
                yield return new KeyValuePair<Type, bool>(typeof(decimal), false);
                yield return new KeyValuePair<Type, bool>(typeof(byte), false);
                yield return new KeyValuePair<Type, bool>(typeof(sbyte), false);
            }
        }

        /// <summary>
        /// Gets properties the converter should be defined on.
        /// </summary>
        public override IEnumerable<PropertyInfo> PropertiesToConvert
        {
            get
            {
                yield return typeof(TimePicker).GetProperty("Value");
                yield return typeof(TimePicker).GetProperty("Minimum");
                yield return typeof(TimePicker).GetProperty("Maximum");
                yield return typeof(TimeUpDown).GetProperty("Value");
                yield return typeof(TimeUpDown).GetProperty("Minimum");
                yield return typeof(TimeUpDown).GetProperty("Maximum");
            }
        }

        /// <summary>
        /// Ensure the type converter properly converts the expected values.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestDescription("Ensure the type converter properly converts the expected values.")]
        public virtual void ConvertTimeToString()
        {
            // todo: remove when baseclass implements these better.

            TypeConverter converter = Converter;

            Assert.AreEqual("05:06:07", converter.ConvertTo(new DateTime(2002, 3, 4, 5, 6, 7), typeof(string)));
        }
    }
}