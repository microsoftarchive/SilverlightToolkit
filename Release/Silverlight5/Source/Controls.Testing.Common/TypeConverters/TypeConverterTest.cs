// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDescription = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for type converters.
    /// </summary>
    /// <typeparam name="T">Type of values being converted to.</typeparam>
    [Tag("TypeConverter")]
    public abstract partial class TypeConverterTest<T> : TestBase
    {
        /// <summary>
        /// Gets the TypeConverter to test.
        /// </summary>
        public abstract TypeConverter Converter { get; }

        /// <summary>
        /// Gets objects and their converted values.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Test only API")]
        public abstract IEnumerable<KeyValuePair<object, T>> ExpectedValues { get; }

        /// <summary>
        /// Gets objects and the types of exceptions they should throw.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Test only API")]
        public abstract IEnumerable<KeyValuePair<object, Type>> ExpectedFailures { get; }

        /// <summary>
        /// Gets types and whether they should be supported by the converter.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Test only API")]
        public abstract IEnumerable<KeyValuePair<Type, bool>> CanConvertFrom { get; }

        /// <summary>
        /// Gets types and whether they should be supported by the converter.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Test only API")]
        public abstract IEnumerable<KeyValuePair<Type, bool>> CanConvertTo { get; }

        /// <summary>
        /// Gets properties the converter should be defined on.
        /// </summary>
        public abstract IEnumerable<PropertyInfo> PropertiesToConvert { get; }

        /// <summary>
        /// Ensure the type converter notifies of the types it can convert from.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestDescription("Ensure the type converter notifies of the types it can convert from.")]
        public virtual void CanConvertFromExpectedTypes()
        {
            TypeConverter converter = Converter;
            foreach (KeyValuePair<Type, bool> pair in CanConvertFrom)
            {
                Assert.AreEqual(pair.Value, converter.CanConvertFrom(pair.Key), pair.Key != null ? pair.Key.Name : "null");
            }
        }

        /// <summary>
        /// Ensure the type converter properly converts the expected values.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestDescription("Ensure the type converter properly converts the expected values.")]
        public virtual void ConvertFromExpectedValues()
        {
            TypeConverter converter = Converter;
            foreach (KeyValuePair<object, T> pair in ExpectedValues)
            {
                object convertedValue = converter.ConvertFrom(pair.Key);
                Assert.AreEqual(pair.Value, convertedValue, "Key: {0}", pair.Key);
            }
        }

        /// <summary>
        /// Ensure the type converter properly throws exceptions for expected
        /// failures.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestDescription("Ensure the type converter properly throws exceptions for expected failures.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for testing")]
        public void ConvertFromExpectedFailures()
        {
            TypeConverter converter = Converter;
            foreach (KeyValuePair<object, Type> pair in ExpectedFailures)
            {
                try
                {
                    converter.ConvertFrom(pair.Key);
                    Assert.Fail("{0} failed to throw an exception of type {1}", pair.Key, pair.Value);
                }
                catch (Exception e)
                {
                    Assert.IsInstanceOfType(e, pair.Value);
                }
            }
        }

        /////// <summary>
        /////// Ensure the type converter's ConvertFrom fails correctly with no
        /////// converter.
        /////// </summary>
        ////[TestMethod]
        ////[TestDescription("Ensure the type converter's ConvertFrom fails correctly with no converter.")]
        ////[ExpectedException(typeof(ArgumentNullException))]
        ////public void ConvertFromNoConverter()
        ////{
        ////    TypeConverters.ConvertFrom<object>(null, null);
        ////}

        /// <summary>
        /// Ensure the type converter's ConvertFromString matches ConvertFrom.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestDescription("Ensure the type converter's ConvertFromString matches ConvertFrom.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for testing")]
        public void ConvertFromStringMatches()
        {
            TypeConverter converter = Converter;

            // Check expected values
            foreach (KeyValuePair<object, T> pair in ExpectedValues)
            {
                string text = pair.Key as string;
                if ((text == null) && (pair.Key != null))
                {
                    continue;
                }

                object convertFrom = converter.ConvertFrom(pair.Key);
                object convretFromString = converter.ConvertFromString(text);
                Assert.AreEqual(convertFrom, convretFromString);
            }

            // Check expected failures
            foreach (KeyValuePair<object, Type> pair in ExpectedFailures)
            {
                string text = pair.Key as string;
                if ((text == null) && (pair.Key != null))
                {
                    continue;
                }

                Exception convertFrom = null;
                try
                {
                    converter.ConvertFrom(pair.Key);
                    Assert.Fail("ConvertFrom {0} failed to throw an exception of type {1}", pair.Key, pair.Value);
                }
                catch (Exception e)
                {
                    convertFrom = e;
                }

                Exception convertFromString = null;
                try
                {
                    converter.ConvertFromString(text);
                    Assert.Fail("ConvertFromString {0} failed to throw an exception of type {1}", pair.Key, pair.Value);
                }
                catch (Exception e)
                {
                    convertFromString = e;
                }
                Assert.AreEqual(
                    convertFrom != null ? convertFrom.GetType() : null,
                    convertFromString != null ? convertFrom.GetType() : null);
            }
        }

        /////// <summary>
        /////// Ensure the type converter's ConvertFromString fails correctly
        /////// with no KnownValues.
        /////// </summary>
        ////[TestMethod]
        ////[TestDescription("Ensure the type converter's ConvertFromString fails correctly with no KnownValues.")]
        ////[ExpectedException(typeof(ArgumentNullException))]
        ////public void ConvertFromStringNoKnownValues()
        ////{
        ////    TypeConverters.ConvertFromString<object>(null, null);
        ////}

        /// <summary>
        /// Ensure the type converter's CanConvertTo works correctly.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestDescription("Ensure the type converter's CanConvertTo works correctly.")]
        public virtual void CanConvertToExpectedTypes()
        {
            TypeConverter converter = Converter;
            foreach (KeyValuePair<Type, bool> pair in CanConvertTo)
            {
                Assert.AreEqual(pair.Value, converter.CanConvertTo(pair.Key), pair.Key != null ? pair.Key.Name : "null");
            }
        }

        /// <summary>
        /// Ensure the type converter's ConvertTo throws a
        /// NotImplementedException.
        /// </summary>
        ////[TestMethod]  // TODO: Add TypeConverter.ConvertTo tests
        [TestDescription("Ensure the type converter's ConvertTo throws a NotImplementedException.")]
        [ExpectedException(typeof(NotImplementedException))]
        public virtual void ConvertToFails()
        {
            Converter.ConvertTo(null, typeof(T));
        }

        /// <summary>
        /// Ensure the type converter's ConvertToString throws a
        /// NotImplementedException.
        /// </summary>
        ////[TestMethod]  // TODO: Add TypeConverter.ConvertTo tests
        [TestDescription("Ensure the type converter's ConvertToString throws a NotImplementedException.")]
        [ExpectedException(typeof(NotImplementedException))]
        public virtual void ConvertToStringFails()
        {
            Converter.ConvertToString(null);
        }

        /// <summary>
        /// Ensure the TypeConverterAttribute is applied to the required
        /// properties.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestDescription("Ensure the TypeConverterAttribute is applied to the required properties.")]
        public void AttributeIsDefined()
        {
            TypeConverter converter = Converter;
            foreach (PropertyInfo property in PropertiesToConvert)
            {
                Assert.IsNotNull(property);
                object[] attributes = property.GetCustomAttributes(typeof(TypeConverterAttribute), true);
                Assert.AreEqual(1, attributes.Length);

                TypeConverterAttribute attribute = attributes[0] as TypeConverterAttribute;
                Assert.IsNotNull(attribute);

                Assert.AreEqual(attribute.ConverterTypeName, converter.GetType().AssemblyQualifiedName);
            }
        }
    }
}