// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class EnumDataTypeAttributeTest {

        [TestMethod]
        public void EnumDataTypeAttribute() {
            Assert.AreEqual(typeof(RegularEnum), new EnumDataTypeAttribute(typeof(RegularEnum)).EnumType);

            Assert.AreEqual("Enumeration", new EnumDataTypeAttribute(typeof(RegularEnum)).CustomDataType);
        }

        [TestMethod()]
        public void EnumDataTypeAttribute_Null() {
            EnumDataTypeAttribute target = new EnumDataTypeAttribute(null);
            ExceptionHelper.ExpectInvalidOperationException(delegate() { target.IsValid("any value"); }, Resources.DataAnnotationsResources.EnumDataTypeAttribute_TypeCannotBeNull);
        }

        [TestMethod()]
        public void EnumDataTypeAttribute_NotEnum() {
            EnumDataTypeAttribute target = new EnumDataTypeAttribute(typeof(string));
            ExceptionHelper.ExpectInvalidOperationException(delegate() { target.IsValid("any value"); }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.EnumDataTypeAttribute_TypeNeedsToBeAnEnum, typeof(string).FullName));
        }

        [TestMethod()]
        public void IsValid() {
            Assert.AreEqual(true, new EnumDataTypeAttribute(typeof(RegularEnum)).IsValid(null), "Null");
            Assert.AreEqual(true, new EnumDataTypeAttribute(typeof(RegularEnum)).IsValid(String.Empty), "Empty string");

            IsValidHelper<RegularEnum>(true, "10");
            IsValidHelper<RegularEnum>(true, RegularEnum.A.ToString());
            IsValidHelper<RegularEnum>(true, 10);
            IsValidHelper<RegularEnum>(true, (short)10);
            IsValidHelper<RegularEnum>(true, (byte)10);
            IsValidHelper<RegularEnum>(true, (long)10);
            IsValidHelper<RegularEnum>(true, RegularEnum.A);

            IsValidHelper<FlagsEnum>(true, FlagsEnum.A | FlagsEnum.B);
            IsValidHelper<FlagsEnum>(true, (FlagsEnum.A | FlagsEnum.B).ToString());
            IsValidHelper<FlagsEnum>(true, FlagsEnum.D);
            IsValidHelper<FlagsEnum>(true, (FlagsEnum.D).ToString());
            IsValidHelper<FlagsEnum>(true, FlagsEnum.A | FlagsEnum.B | FlagsEnum.C | FlagsEnum.D);
            IsValidHelper<FlagsEnum>(true, 3);
            IsValidHelper<FlagsEnum>(true, 9);
            IsValidHelper<FlagsEnum>(true, "3");

            IsValidHelper<RegularEnum>(false, "15");
            IsValidHelper<RegularEnum>(false, "asdfasdf");
            IsValidHelper<RegularEnum>(false, "15.23");
            IsValidHelper<RegularEnum>(false, Enum.Parse(typeof(RegularEnum), "15", false));
            IsValidHelper<RegularEnum>(false, 15);
            IsValidHelper<RegularEnum>(false, (short)15);
            IsValidHelper<RegularEnum>(false, -10);
            IsValidHelper<RegularEnum>(false, new object());
            IsValidHelper<RegularEnum>(false, OtherEnum.A);
            IsValidHelper<RegularEnum>(false, OtherEnum.B);
            IsValidHelper<RegularEnum>(false, false);
            IsValidHelper<RegularEnum>(false, 2.5);
            IsValidHelper<RegularEnum>(false, 10.0);
            IsValidHelper<RegularEnum>(false, 'a');

            IsValidHelper<FlagsEnum>(false, 13);
            IsValidHelper<FlagsEnum>(false, 0);
            IsValidHelper<FlagsEnum>(false, "13");
        }

        private void IsValidHelper<T>(bool expected, object value) {
            EnumDataTypeAttribute attribute = new EnumDataTypeAttribute(typeof(T));
            Assert.AreEqual(expected, attribute.IsValid(value), "Value = {0} ; Type = {1}", value, value.GetType());
        }

        private enum RegularEnum {
            A = 10,
            B = 20
        }

        [Flags]
        private enum FlagsEnum {
            A = 1,
            B = 2,
            C = 8,
            D = A | C,
        }

        private enum OtherEnum {
            A = 10,
            B = 20
        }
    }
}
