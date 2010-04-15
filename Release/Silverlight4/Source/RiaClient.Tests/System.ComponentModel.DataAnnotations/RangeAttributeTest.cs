// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class RangeAttributeTest {

        [TestMethod]
        [Description("All legal ctor forms succeed and set properties expected")]
        public void RangeAttribute_All_Legal_Ctors() {
            RangeAttribute attr = new RangeAttribute(1, 2);
            Assert.AreEqual(1, attr.Minimum);
            Assert.AreEqual(2, attr.Maximum);
            Assert.AreEqual(typeof(int), attr.OperandType);

            attr = new RangeAttribute(3.0, 4.0);
            Assert.AreEqual(3.0, attr.Minimum);
            Assert.AreEqual(4.0, attr.Maximum);
            Assert.AreEqual(typeof(double), attr.OperandType);

            attr = new RangeAttribute(typeof(decimal), "1M", "2M");
            Assert.AreEqual("1M", attr.Minimum);
            Assert.AreEqual("2M", attr.Maximum);
            Assert.AreEqual(typeof(decimal), attr.OperandType);
        }

        [TestMethod]
        [Description("All illegal ctor forms succeed and set properties expected without throwing exceptions")]
        public void RangeAttribute_All_Illegal_Ctors_No_Exceptions() {
            RangeAttribute attr = new RangeAttribute(2, 1);
            Assert.AreEqual(2, attr.Minimum);
            Assert.AreEqual(1, attr.Maximum);
            Assert.AreEqual(typeof(int), attr.OperandType);

            attr = new RangeAttribute(4.0, 3.0);
            Assert.AreEqual(4.0, attr.Minimum);
            Assert.AreEqual(3.0, attr.Maximum);
            Assert.AreEqual(typeof(double), attr.OperandType);

            attr = new RangeAttribute(null, null, null);
            Assert.IsNull(attr.Minimum);
            Assert.IsNull(attr.Maximum);
            Assert.IsNull(attr.OperandType);
        }

        [TestMethod]
        public void RangeAttribute_Fail_Constructor_ArbitraryType_NullParameter() {
            var attr = new RangeAttribute(null, "1", "10");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(2);
            }, Resources.DataAnnotationsResources.RangeAttribute_Must_Set_Operand_Type);
        }

        [TestMethod]
        public void RangeAttribute_Fail_Constructor_ArbitraryType_NotComparableType() {
            var attr = new RangeAttribute(typeof(RangeAttributeTest), "1", "10");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(2);
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RangeAttribute_ArbitraryTypeNotIComparable, typeof(RangeAttributeTest).FullName, typeof(System.IComparable).FullName));
        }

        [TestMethod]
        public void RangeAttribute_Constructor_Double_MinGreaterThanMax() {
            var attr = new RangeAttribute(1000000000.5, 10.5);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(10.0);
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RangeAttribute_MinGreaterThanMax, 10.5, 1000000000.5));
        }


        [TestMethod]
        public void RangeAttribute_IsValid_Int32() {
            AssertAreTrue(new RangeAttribute(0, 100), 0, 50, 100);
            AssertAreFalse(new RangeAttribute(0, 100), -100, 200);
        }

        [TestMethod]
        public void RangeAttribute_IsValid_Double() {
            AssertAreTrue(new RangeAttribute(0.0, 100.0), 0.0, 50.0, 100.0);
            AssertAreFalse(new RangeAttribute(0.0, 100.0), -100.0, 200.0);
        }

        [TestMethod]
        [Description("Int32 succeeds when passed in as typed strings as well")]
        public void RangeAttribute_IsValid_Int32_As_String() {
            AssertAreTrue(new RangeAttribute(typeof(int), "0", "100"), 0, 50, 100);
            AssertAreFalse(new RangeAttribute(typeof(int), "0", "100"), -100, 200);
        }

        [TestMethod]
        [Description("Double succeeds when passed in as typed strings as well")]
        public void RangeAttribute_IsValid_Double_As_String() {
            AssertAreTrue(new RangeAttribute(typeof(double), Convert.ToString(0.0), Convert.ToString(100.0)), 0.0, 50.0, 100.0);
            AssertAreFalse(new RangeAttribute(typeof(double), Convert.ToString(0.0), Convert.ToString(100.0)), -100.0, 200.0);
        }

        [TestMethod]
        public void RangeAttribute_IsValid_VariousNumericTypes() {
            object[] values = new object[] { 10, 10.5, (short)10, (long)10, 10.5f, (byte)10, "10", true };
            AssertAreTrue(new RangeAttribute(0, 100), values.Union(new object[] { (char)10 }).ToArray());
            AssertAreTrue(new RangeAttribute(0.0, 100.0), values);
        }

        [TestMethod]
        public void RangeAttribute_IsValid_ArbitraryType() {
            AssertAreTrue(new RangeAttribute(typeof(DateTime), DateTime.Now.AddDays(-3).ToLongDateString(), DateTime.Now.AddDays(3).ToLongDateString()),
                DateTime.Now, DateTime.Now.ToLongDateString());
        }

        [TestMethod]
        public void RangeAttribute_IsValid_ArbitraryType_Cannot_Convert() {
            RangeAttribute attr = new RangeAttribute(typeof(DateTime), DateTime.Now.AddDays(-3).ToLongDateString(), DateTime.Now.AddDays(3).ToLongDateString());
            Assert.IsFalse(attr.IsValid("xxxx"));   // format error is invalid, not an exception
            Assert.IsFalse(attr.IsValid("2.0"));    // string which is valid double but not valid DateTime
            Assert.IsFalse(attr.IsValid(2.0));      // wrong type is invalid, not an exception
        }

        [TestMethod]
        public void RangeAttribute_FormatErrorMessage() {
            var attrib = new RangeAttribute(-100.3, 10000000.5);
            Assert.AreEqual(String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RangeAttribute_ValidationError, "SOME_NAME", -100.3, 10000000.5), attrib.FormatErrorMessage("SOME_NAME"));
        }

        [TestMethod]
        [Description("Int32 form fails at runtime when min exceeds max")]
        public void RangeAttribute_Fail_Int32_Min_Exceed_Max() {
            var attr = new RangeAttribute(3, 1);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(2);
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RangeAttribute_MinGreaterThanMax, 1, 3));
        }

        [TestMethod]
        [Description("Double form fails at runtime when min exceeds max")]
        public void RangeAttribute_Fail_Double_Min_Exceed_Max() {
            var attr = new RangeAttribute(3.0, 1.0);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(2.0);
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RangeAttribute_MinGreaterThanMax, 1, 3));
        }

        [TestMethod]
        [Description("String form fails at runtime when min exceeds max")]
        public void RangeAttribute_Fail_String_Min_Exceed_Max() {
            var attr = new RangeAttribute(typeof(float), Convert.ToString(3.0), Convert.ToString(1.0));

            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(2.0f);
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RangeAttribute_MinGreaterThanMax, 1, 3));
        }

        private void AssertAreTrue(RangeAttribute range, params object[] values) {
            foreach (var v in values) {
                Assert.IsTrue(range.IsValid(v), "value = " + v);
            }
        }

        private void AssertAreFalse(RangeAttribute range, params object[] values) {
            foreach (var v in values) {
                Assert.IsFalse(range.IsValid(v), "value = " + v);
            }
        }
    }
}
