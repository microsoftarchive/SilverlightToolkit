// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
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
            }, Resources.DataAnnotationsResources.UIHintImplementation_NeedEvenNumberOfControlParameters);
        }

        [TestMethod]
        public void ConstructorControlParameters_NonStringKey() {
            var attr = new UIHintAttribute("", "", 1, "value");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.UIHintImplementation_ControlParameterKeyIsNotAString, 0, 1));
        }

        [TestMethod]
        public void ConstructorControlParameters_NullKey() {
            var attr = new UIHintAttribute("", "", null, "value");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.UIHintImplementation_ControlParameterKeyIsNull, 0));
        }

        [TestMethod]
        public void ConstructorControlParameters_DuplicateKey() {
            var attr = new UIHintAttribute("", "", "key", "value1", "key", "value2");
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                var v = attr.ControlParameters;
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.UIHintImplementation_ControlParameterKeyOccursMoreThanOnce, 2, "key"));
        }

        [TestMethod]
        public void Equals_DifferentObjectType() {
            Assert.IsFalse(new UIHintAttribute("foo", "bar").Equals(new object()));
        }

        [TestMethod]
        public void Equals_NullObject() {
            Assert.IsFalse(new UIHintAttribute("foo").Equals(null));
        }

        [TestMethod]
        public void Equals_SameObjectType() {
            var a1 = new UIHintAttribute("foo");
            var a2 = new UIHintAttribute("foo");
            var b1 = new UIHintAttribute("foo", "bar");
            var b2 = new UIHintAttribute("foo", "bar");

            Assert.IsTrue(a1.Equals(a2));
            Assert.IsTrue(a2.Equals(a1));

            Assert.IsTrue(b1.Equals(b2));
            Assert.IsTrue(b2.Equals(b1));

            Assert.IsFalse(a1.Equals(b1));
            Assert.IsFalse(b1.Equals(a1));
        }

        [TestMethod]
        public void Equals_SameObjectType_WithParamsDictionary() {
            var a1 = new UIHintAttribute("foo", "bar", "a", 1, "b", false);
            var a2 = new UIHintAttribute("foo", "bar", "b", false, "a", 1);

            Assert.IsTrue(a1.Equals(a2));
            Assert.IsTrue(a2.Equals(a1));
        }

        [TestMethod]
        public void Equals_DoesNotThrow() {
            var a1 = new UIHintAttribute("foo", "bar");
            var a2 = new UIHintAttribute("foo", "bar", 1);

            Assert.IsFalse(a1.Equals(a2));
            Assert.IsFalse(a2.Equals(a1));
        }

#if !SILVERLIGHT
        [TestMethod]
        public void TypeId_ReturnsDifferentValuesForDifferentInstances()
        {
            Assert.AreNotEqual(new UIHintAttribute("foo").TypeId, new UIHintAttribute("bar").TypeId);
        }
#endif
    }
}

