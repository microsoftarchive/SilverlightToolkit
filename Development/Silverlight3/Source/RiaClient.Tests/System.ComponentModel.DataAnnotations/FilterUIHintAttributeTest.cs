using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class FilterUIHintAttributeTest {
        [TestMethod]
        [Description("Simple ctors set expected properties.")]
        public void FilterUIHintAttribute_Simple_Ctors_Set_Properties() {
            var attr = new FilterUIHintAttribute(null, null);
            Assert.IsNull(attr.FilterUIHint);
            Assert.IsNull(attr.PresentationLayer);
            Assert.AreEqual(0, attr.ControlParameters.Count);

            attr = new FilterUIHintAttribute(string.Empty, string.Empty);
            Assert.AreEqual(string.Empty, attr.FilterUIHint);
            Assert.AreEqual(string.Empty, attr.PresentationLayer);
            Assert.AreEqual(0, attr.ControlParameters.Count);

            attr = new FilterUIHintAttribute("theHint");
            Assert.AreEqual("theHint", attr.FilterUIHint);
            Assert.IsNull(attr.PresentationLayer);
            Assert.AreEqual(0, attr.ControlParameters.Count);

            attr = new FilterUIHintAttribute("theHint", "theLayer");
            Assert.AreEqual("theHint", attr.FilterUIHint);
            Assert.AreEqual("theLayer", attr.PresentationLayer);
            Assert.AreEqual(0, attr.ControlParameters.Count);
        }

        [TestMethod]
        public void ConstructorControlParameters() {
            Assert.AreEqual<int>(2, new FilterUIHintAttribute("", "", "a", 1, "b", 2).ControlParameters.Keys.Count);
        }

        [TestMethod]
        public void ConstructorControlParameters_NoParams() {
            Assert.AreEqual<int>(0, new FilterUIHintAttribute("", "", new object[0]).ControlParameters.Keys.Count);
            Assert.AreEqual<int>(0, new FilterUIHintAttribute("", "", (object[])null).ControlParameters.Keys.Count);
            Assert.AreEqual<int>(0, new FilterUIHintAttribute("", "").ControlParameters.Keys.Count);
            Assert.AreEqual<int>(0, new FilterUIHintAttribute("").ControlParameters.Keys.Count);
        }

        [TestMethod]
        public void ConstructorControlParameters_UnevenNumber() {
            var attr = new FilterUIHintAttribute("", "", "");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, "The number of control parameters must be even.");
        }

        [TestMethod]
        public void ConstructorControlParameters_NonStringKey() {
            var attr = new FilterUIHintAttribute("", "", 1, "value");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, "The key parameter at position 0 with value '1' is not a string. Every key control parameter must be a string.");
        }

        [TestMethod]
        public void ConstructorControlParameters_NullKey() {
            var attr = new FilterUIHintAttribute("", "", null, "value");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, "The key parameter at position 0 is null. Every key control parameter must be a string.");
        }

        [TestMethod]
        public void ConstructorControlParameters_DuplicateKey() {
            var attr = new FilterUIHintAttribute("", "", "key", "value1", "key", "value2");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, "The key parameter at position 2 with value 'key' occurs more than once.");
        }
    }
}
