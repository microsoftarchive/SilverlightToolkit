// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for Microsoft.Windows.LengthConverter.
    /// </summary>
    [TestClass]
    public sealed partial class LengthConverterTest : TypeConverterTest<double>
    {
        /// <summary>
        /// Gets the TypeConverter to test.
        /// </summary>
        public override TypeConverter Converter
        {
            get { return new LengthConverter(); }
        }

        /// <summary>
        /// Gets objects and their converted values.
        /// </summary>
        public override IEnumerable<KeyValuePair<object, double>> ExpectedValues
        {
            get
            {
                yield return new KeyValuePair<object, double>("Auto", double.NaN);
                yield return new KeyValuePair<object, double>("AUTO", double.NaN);
                yield return new KeyValuePair<object, double>("auto", double.NaN);
                yield return new KeyValuePair<object, double>("aUtO", double.NaN);
                yield return new KeyValuePair<object, double>("AutO", double.NaN);
                yield return new KeyValuePair<object, double>("0", 0.0);
                yield return new KeyValuePair<object, double>("0 ", 0.0);
                yield return new KeyValuePair<object, double>(" 0", 0.0);
                yield return new KeyValuePair<object, double>(" 0 ", 0.0);
                yield return new KeyValuePair<object, double>("  0  ", 0.0);
                yield return new KeyValuePair<object, double>("1", 1.0);
                yield return new KeyValuePair<object, double>("10", 10.0);
                yield return new KeyValuePair<object, double>("-2", -2.0);
                yield return new KeyValuePair<object, double>("1px", 1.0);
                yield return new KeyValuePair<object, double>("1in", 96.0);
                yield return new KeyValuePair<object, double>("1cm", 37.795275590551178);
                yield return new KeyValuePair<object, double>("1pt", 1.3333333333333333);
                yield return new KeyValuePair<object, double>("1 px", 1.0);
                yield return new KeyValuePair<object, double>("1 in", 96.0);
                yield return new KeyValuePair<object, double>("1 cm", 37.795275590551178);
                yield return new KeyValuePair<object, double>("1 pt", 1.3333333333333333);
                yield return new KeyValuePair<object, double>(1.0, 1.0);
                yield return new KeyValuePair<object, double>(1, 1.0);

                // The following tests fail when running under cultures other
                // than en-US
                if ("en-US" == Thread.CurrentThread.CurrentCulture.Name)
                {
                    yield return new KeyValuePair<object, double>("0.0", 0.0);
                    yield return new KeyValuePair<object, double>(".0", 0.0);
                    yield return new KeyValuePair<object, double>("1.0", 1.0);
                    yield return new KeyValuePair<object, double>("1.5", 1.5);
                    yield return new KeyValuePair<object, double>("10.5484854", 10.5484854);
                    yield return new KeyValuePair<object, double>("10.0", 10.0);
                    yield return new KeyValuePair<object, double>("10.", 10.0);
                    yield return new KeyValuePair<object, double>("0.5484854", 0.5484854);
                    yield return new KeyValuePair<object, double>(".5484854", 0.5484854);
                    yield return new KeyValuePair<object, double>("-2.0", -2.0);
                    yield return new KeyValuePair<object, double>("2.5px", 2.5);
                    yield return new KeyValuePair<object, double>("2.5in", 2.5 * 96.0);
                    yield return new KeyValuePair<object, double>("2.5cm", 2.5 * 37.795275590551178);
                    yield return new KeyValuePair<object, double>("2.5pt", 2.5 * 1.3333333333333333);
                }
            }
        }

        /// <summary>
        /// Gets objects and the types of exceptions they should throw.
        /// </summary>
        public override IEnumerable<KeyValuePair<object, Type>> ExpectedFailures
        {
            get
            {
                yield return new KeyValuePair<object, Type>(null, typeof(NotSupportedException));
                yield return new KeyValuePair<object, Type>("", typeof(FormatException));
                yield return new KeyValuePair<object, Type>(" ", typeof(FormatException));
                yield return new KeyValuePair<object, Type>("Auto ", typeof(FormatException));
                yield return new KeyValuePair<object, Type>(" Auto", typeof(FormatException));
                yield return new KeyValuePair<object, Type>(" Auto ", typeof(FormatException));
                yield return new KeyValuePair<object, Type>("- 2", typeof(FormatException));
                yield return new KeyValuePair<object, Type>("1ft", typeof(FormatException));
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
                yield return new KeyValuePair<Type, bool>(typeof(short), true);
                yield return new KeyValuePair<Type, bool>(typeof(ushort), true);
                yield return new KeyValuePair<Type, bool>(typeof(int), true);
                yield return new KeyValuePair<Type, bool>(typeof(uint), true);
                yield return new KeyValuePair<Type, bool>(typeof(long), true);
                yield return new KeyValuePair<Type, bool>(typeof(ulong), true);
                yield return new KeyValuePair<Type, bool>(typeof(float), true);
                yield return new KeyValuePair<Type, bool>(typeof(double), true);
                yield return new KeyValuePair<Type, bool>(typeof(decimal), true);
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
                yield return new KeyValuePair<Type, bool>(typeof(double), true);
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
                // yield return new KeyValuePair<Type, bool>(null, false);
            }
        }

        /// <summary>
        /// Gets properties the converter should be defined on.
        /// </summary>
        public override IEnumerable<PropertyInfo> PropertiesToConvert
        {
            get
            {
                yield return typeof(WrapPanel).GetProperty("ItemHeight");
                yield return typeof(WrapPanel).GetProperty("ItemWidth");
            }
        }
    }
}