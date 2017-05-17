// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDescription = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for TimeFormatConverter.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    public class TimeFormatConverterTest : TypeConverterTest<ITimeFormat>
    {
        /// <summary>
        /// Gets the TypeConverter to test.
        /// </summary>
        /// <value></value>
        public override TypeConverter Converter
        {
            get { return new TimeFormatConverter(); }
        }

        /// <summary>
        /// Gets objects and their converted values.
        /// </summary>
        /// <value></value>
        public override IEnumerable<KeyValuePair<object, ITimeFormat>> ExpectedValues
        {
            get
            {
                yield return new KeyValuePair<object, ITimeFormat>("Short", new ShortTimeFormat());
                yield return new KeyValuePair<object, ITimeFormat>("short", new ShortTimeFormat());
                yield return new KeyValuePair<object, ITimeFormat>("Long", new LongTimeFormat());
                yield return new KeyValuePair<object, ITimeFormat>("long", new LongTimeFormat());
                yield return new KeyValuePair<object, ITimeFormat>("hh:mm:ss", new CustomTimeFormat("hh:mm:ss"));
                yield return new KeyValuePair<object, ITimeFormat>("random with a m, s or h", new CustomTimeFormat("random with a m, s or h"));
            }
        }

        /// <summary>
        /// Gets objects and the types of exceptions they should throw.
        /// </summary>
        /// <value></value>
        public override IEnumerable<KeyValuePair<object, Type>> ExpectedFailures
        {
            get
            {
                yield return new KeyValuePair<object, Type>(null, typeof(NotSupportedException));
                yield return new KeyValuePair<object, Type>("", typeof(FormatException));
                yield return new KeyValuePair<object, Type>(" ", typeof(FormatException));
                yield return new KeyValuePair<object, Type>("adfwer", typeof(FormatException));
                yield return new KeyValuePair<object, Type>(new object(), typeof(InvalidCastException));
            }
        }

        /// <summary>
        /// Gets types and whether they should be supported by the converter.
        /// </summary>
        /// <value></value>
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
        /// <value></value>
        public override IEnumerable<KeyValuePair<Type, bool>> CanConvertTo
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
                yield return new KeyValuePair<Type, bool>(typeof(object), true);
                yield return new KeyValuePair<Type, bool>(typeof(byte), false);
                yield return new KeyValuePair<Type, bool>(typeof(sbyte), false);
                // yield return new KeyValuePair<Type, bool>(null, false);
            }
        }

        /// <summary>
        /// Gets properties the converter should be defined on.
        /// </summary>
        /// <value></value>
        public override IEnumerable<PropertyInfo> PropertiesToConvert
        {
            get
            {
                yield return typeof(TimePicker).GetProperty("Format");
                yield return typeof(TimeUpDown).GetProperty("Format");
            }
        }

        /// <summary>
        /// Ensure the type converter properly converts the expected values.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestDescription("Ensure the type converter properly converts the expected values.")]
        public virtual void ConvertTimeFormatToString()
        {
            // todo: remove when baseclass implements these better.

            TypeConverter converter = Converter;

            Assert.AreEqual("Short", converter.ConvertTo(new ShortTimeFormat(), typeof(string)));
            Assert.AreEqual("Long", converter.ConvertTo(new LongTimeFormat(), typeof(string)));
            Assert.AreEqual("ss:mm:hh", converter.ConvertTo(new CustomTimeFormat("ss:mm:hh"), typeof(string)));
        }
    }
}