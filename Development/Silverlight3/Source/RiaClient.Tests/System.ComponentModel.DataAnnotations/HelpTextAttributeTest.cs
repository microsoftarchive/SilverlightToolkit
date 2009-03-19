using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class HelpTextAttributeTest {
        [TestMethod]
        public void HelpTextAttributeThrowsWithNullText() {
            HelpTextAttribute attribute = new HelpTextAttribute(null);
            ExceptionHelper.ExpectInvalidOperationException(() => {
                string s = attribute.HelpText;
            }, "Value cannot be null or empty.");
        }

        [TestMethod]
        public void HelpTextAttributeThrowsWithInvalidResourceManager() {
            HelpTextAttribute attribute = new HelpTextAttribute(typeof(MockType), "Foo");
            ExceptionHelper.ExpectInvalidOperationException(() => {
                string s = attribute.HelpText;
            }, "Could not find resource 'Foo' on type 'System.ComponentModel.DataAnnotations.Test.HelpTextAttributeTest+MockType'");
        }

        [TestMethod]
        public void HelpTextAttributeThrowsWithNullResourceManager() {
            HelpTextAttribute attribute = new HelpTextAttribute(null, "Foo");
            ExceptionHelper.ExpectInvalidOperationException(() => {
                string s = attribute.HelpText;
            }, "The resource manager type cannot be null.");
        }

        [TestMethod]
        public void HelpTextAttributeThrowsWithNullResourceValue() {
            HelpTextAttribute attribute = new HelpTextAttribute(typeof(HelpTextAttribute), null);
            ExceptionHelper.ExpectInvalidOperationException(() => {
                string s = attribute.HelpText;
            }, "Value cannot be null or empty.");
        }

        [TestMethod]
        public void HelpTextAttributeWithValidResourceManager() {
            HelpTextAttribute attribute = new HelpTextAttribute(typeof(MockResourceManager), "MockResource");

            Assert.AreEqual(attribute.HelpText, "Foo", "Values should should be the same");
        }

        [TestMethod]
        public void HelpTextAttributeWithValidResource() {
            HelpTextAttribute attribute = new System.ComponentModel.DataAnnotations.HelpTextAttribute("foo");

            Assert.AreEqual(attribute.HelpText, "foo", "Values should be the same");
        }


        public class MockResourceManager {
            public static string MockResource {
                get {
                    return "Foo";
                }
            }
        }

        private class MockType {

        }
    }
}
