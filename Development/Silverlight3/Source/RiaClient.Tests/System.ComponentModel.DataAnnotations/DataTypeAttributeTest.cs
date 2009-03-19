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
            }, "The custom DataType string cannot be null or empty.");

            attr = new DataTypeAttribute(String.Empty);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid(null);
            }, "The custom DataType string cannot be null or empty.");
        }
    }
}
