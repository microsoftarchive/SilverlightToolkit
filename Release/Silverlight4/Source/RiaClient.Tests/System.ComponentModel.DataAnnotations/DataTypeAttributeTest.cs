// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class DataTypeAttributeTest {
        [TestMethod]
        public void DataTypeProperty() {
            Assert.AreEqual<DataType>(DataType.Password, new DataTypeAttribute(DataType.Password).DataType);
            Assert.AreEqual<DataType>(DataType.Date, new DataTypeAttribute(DataType.Date).DataType);

            Assert.AreEqual<DataType>(DataType.Custom, new DataTypeAttribute("aaaa").DataType);
        }

        [TestMethod]
        public void CustomDataType() {
            Assert.AreEqual<string>("aaaa", new DataTypeAttribute("aaaa").CustomDataType);
            Assert.AreEqual<string>("bbb", new DataTypeAttribute("bbb").CustomDataType);
        }

        [TestMethod]
        public void Constructor_Invalid() {
            var attr = new DataTypeAttribute(null);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(null);
            }, Resources.DataAnnotationsResources.DataTypeAttribute_EmptyDataTypeString);

            attr = new DataTypeAttribute(String.Empty);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(null);
            }, Resources.DataAnnotationsResources.DataTypeAttribute_EmptyDataTypeString);
        }
    }
}
