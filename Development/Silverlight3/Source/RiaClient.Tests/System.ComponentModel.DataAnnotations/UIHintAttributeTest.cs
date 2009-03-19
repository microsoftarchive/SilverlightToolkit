using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class UIHintAttributeTest {
        [TestMethod]
        [Description("Simple ctors set expected properties.")]
        public void UIHintAttribute_Simple_Ctors_Set_Properties() {
            var attr = new UIHintAttribute(null, null);
            Assert.IsNull(attr.UIHint);
            Assert.IsNull(attr.PresentationLayer);
            Assert.AreEqual(0, attr.ControlParameters.Count);

            attr = new UIHintAttribute(string.Empty, string.Empty);
            Assert.AreEqual(string.Empty, attr.UIHint);
            Assert.AreEqual(string.Empty, attr.PresentationLayer);
            Assert.AreEqual(0, attr.ControlParameters.Count);

            attr = new UIHintAttribute("theHint");
            Assert.AreEqual("theHint", attr.UIHint);
            Assert.IsNull(attr.PresentationLayer);
            Assert.AreEqual(0, attr.ControlParameters.Count);

            attr = new UIHintAttribute("theHint", "theLayer");
            Assert.AreEqual("theHint", attr.UIHint);
            Assert.AreEqual("theLayer", attr.PresentationLayer);
            Assert.AreEqual(0, attr.ControlParameters.Count);
        }

        [TestMethod]
        public void ConstructorControlParameters() {
            Assert.AreEqual<int>(2, new UIHintAttribute("", "", "a", 1, "b", 2).ControlParameters.Keys.Count);
        }

        [TestMethod]
        public void ConstructorControlParameters_NoParams() {
            Assert.AreEqual<int>(0, new UIHintAttribute("", "", new object[0]).ControlParameters.Keys.Count);
            Assert.AreEqual<int>(0, new UIHintAttribute("", "", (object[])null).ControlParameters.Keys.Count);
            Assert.AreEqual<int>(0, new UIHintAttribute("", "").ControlParameters.Keys.Count);
            Assert.AreEqual<int>(0, new UIHintAttribute("").ControlParameters.Keys.Count);
        }

        [TestMethod]
        public void ConstructorControlParameters_UnevenNumber() {
            var attr = new UIHintAttribute("", "", "");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, "The number of control parameters must be even.");
        }

        [TestMethod]
        public void ConstructorControlParameters_NonStringKey() {
            var attr = new UIHintAttribute("", "", 1, "value");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, "The key parameter at position 0 with value '1' is not a string. Every key control parameter must be a string.");
        }

        [TestMethod]
        public void ConstructorControlParameters_NullKey() {
            var attr = new UIHintAttribute("", "", null, "value");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, "The key parameter at position 0 is null. Every key control parameter must be a string.");
        }

        [TestMethod]
        public void ConstructorControlParameters_DuplicateKey() {
            var attr = new UIHintAttribute("", "", "key", "value1", "key", "value2");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, "The key parameter at position 2 with value 'key' occurs more than once.");
        }
    }
}
