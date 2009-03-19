using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class ValidationAttributeTest {
        private CultureInfo _defaultUiCulture;
        private CultureInfo _germanCulture;

        #region Setup and cleanup

        public TestContext TestContext {
            get;
            set;
        }

        [TestInitialize]
        public void SetUp() {
            _defaultUiCulture = CultureInfo.CurrentUICulture;
            _germanCulture = new CultureInfo("de");
        }

        [TestCleanup]
        public void TearDown() {
            Thread.CurrentThread.CurrentUICulture = _defaultUiCulture;
        }

        #endregion

        #region Attribute and resource mocks

        public class BaseTestAttribute : ValidationAttribute {
            public BaseTestAttribute() {
            }
            public BaseTestAttribute(Func<string> func)
                : base(func) {
            }
            public override bool IsValid(object value) {
                return true;
            }
            public new string ErrorMessageString {
                get {
                    return base.ErrorMessageString;
                }
            }
        }

        public class DefaultTestAttribute : BaseTestAttribute {
            public DefaultTestAttribute()
                : base() {
            }
        }

        public class CustomTestAttribte : BaseTestAttribute {
            public CustomTestAttribte()
                : base(() => ResourceClass.Resource1) {
            }
        }

        public class FailingTestAttribute : BaseTestAttribute {
            public override bool IsValid(object value) {
                return false;
            }
        }

        public class ResourceClass {
            public static string Resource1 {
                get { return IsGermanCulture() ? "Resource 1 Value DE" : "Resource 1 Value"; }
            }

            public static string Resource2 {
                get { return IsGermanCulture() ? "Resource 2 Value DE" : "Resource 2 Value"; }
            }

            public static object NonStringResource {
                get { return 1; }
            }

            private static bool IsGermanCulture() {
                return CultureInfo.CurrentUICulture.Name.StartsWith("de", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        #endregion

        #region Tests

        [TestMethod]
        public void Constructor_ResourceAccessor() {
            var attr = new BaseTestAttribute(null);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, "Either ErrorMessageString or ErrorMessageResourceName must be set, but not both.");
        }

        [TestMethod]
        public void ErrorMessage() {
            Assert.AreEqual<string>("Error message", new DefaultTestAttribute() { ErrorMessage = "Error message" }.ErrorMessage);
        }

        [TestMethod]
        public void ErrorMessageResourceName() {
            Assert.AreEqual<string>("ResourceName", new DefaultTestAttribute() { ErrorMessageResourceName = "ResourceName" }.ErrorMessageResourceName);
        }

        [TestMethod]
        public void ErrorMessageResourceType() {
            Assert.AreEqual<Type>(typeof(ResourceClass), new DefaultTestAttribute() { ErrorMessageResourceType = typeof(ResourceClass) }.ErrorMessageResourceType);
        }

        [TestMethod]
        public void Validate_Fail() {
            ExceptionHelper.ExpectException<ValidationException>(delegate() {
                new FailingTestAttribute().Validate(null, "prop");
            });
        }

        [TestMethod]
        public void Validate_Pass() {
            new DefaultTestAttribute().Validate(null, "prop");
        }

        [TestMethod]
        public void FormatErrorMessage() {
            Assert.AreEqual<string>("The field prop is invalid.", new DefaultTestAttribute().FormatErrorMessage("prop"));
        }

        [TestMethod]
        public void Construction_ResourceMode_NonexistantResourceName() {
            DefaultTestAttribute attr = new DefaultTestAttribute() {
                ErrorMessageResourceName = "NonexistantResourceName",
                ErrorMessageResourceType = typeof(ResourceClass)
            };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, "The resource type 'System.ComponentModel.DataAnnotations.Test.ValidationAttributeTest+ResourceClass' does not have a publicly visible static property named 'NonexistantResourceName'.");
        }

        [TestMethod]
        public void Construction_ResourceMode_NonStringResource() {
            DefaultTestAttribute attr = new DefaultTestAttribute() {
                ErrorMessageResourceName = "NonStringResource",
                ErrorMessageResourceType = typeof(ResourceClass)
            };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, "The property 'NonStringResource' on resource type 'System.ComponentModel.DataAnnotations.Test.ValidationAttributeTest+ResourceClass' is not a string type.");
        }

        [TestMethod]
        [Description("Setting all properties throws no exceptions")]
        public void ValidationAttribute_Set_All_Props_No_Exceptions() {
            DefaultTestAttribute attr = new DefaultTestAttribute();
            attr.ErrorMessage = "em";
            attr.ErrorMessageResourceName = "emName";
            attr.ErrorMessageResourceType = typeof(ResourceClass);

            attr.ErrorMessage = string.Empty;
            attr.ErrorMessageResourceName = string.Empty;

            attr.ErrorMessage = null;
            attr.ErrorMessageResourceName = null;
            attr.ErrorMessageResourceType = null;
        }

        [TestMethod]
        [Description("Setting only ResourceType but not ErrorMessageResourceName fails")]
        public void ValidationAttribute_Fail_ResourceType_Without_ResourceName() {
            DefaultTestAttribute attr = new DefaultTestAttribute() { ErrorMessageResourceType = typeof(ResourceClass) };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, "Either ErrorMessageString or ErrorMessageResourceName must be set, but not both.");
        }

        [TestMethod]
        [Description("Setting only ErrorMessageResourceName but not ResourceType fails")]
        public void ValidationAttribute_Fail_ResourceName_Without_ResourceType() {
            DefaultTestAttribute attr = new DefaultTestAttribute() { ErrorMessageResourceName = "x" };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, "Both ErrorMessageResourceType and ErrorMessageResourceName need to be set on this attribute.");
        }

        [TestMethod]
        [Description("Setting only ErrorMessage and ErrorMessageResourceName fails")]
        public void ValidationAttribute_Fail_ResourceName_And_ErrorMessage() {
            DefaultTestAttribute attr = new DefaultTestAttribute() { ErrorMessage = "x", ErrorMessageResourceName = "y" };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, "Either ErrorMessageString or ErrorMessageResourceName must be set, but not both.");
        }

        [TestMethod]
        [Description("Setting only ErrorMessage and ErrorMessageResourceType fails")]
        public void ValidationAttribute_Fail_ResourceType_And_ErrorMessage() {
            DefaultTestAttribute attr = new DefaultTestAttribute() { ErrorMessage = "x", ErrorMessageResourceType = typeof(ResourceClass) };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, "Both ErrorMessageResourceType and ErrorMessageResourceName need to be set on this attribute.");
        }

        [TestMethod]
        public void ErrorMessageString_NoOverrides_BaseDefaultMessage() {
            Assert.AreEqual<string>("The field {0} is invalid.", new DefaultTestAttribute().ErrorMessageString);
        }

        [TestMethod]
        public void ErrorMessageString_NoOverrides_AttributeDefaultMessage() {
            var attribute = new CustomTestAttribte();

            Assert.AreEqual<string>("Resource 1 Value", attribute.ErrorMessageString);

            Thread.CurrentThread.CurrentUICulture = _germanCulture; // change culture to simulate resource manager
            Assert.AreEqual<string>("Resource 1 Value DE", attribute.ErrorMessageString);
        }

        [TestMethod]
        public void ErrorMessageString_ExplicitOverride() {
            Assert.AreEqual<string>("My custom message", new DefaultTestAttribute() { ErrorMessage = "My custom message" }.ErrorMessageString);
            Assert.AreEqual<string>("My custom message", new CustomTestAttribte() { ErrorMessage = "My custom message" }.ErrorMessageString);
        }

        [TestMethod]
        public void ErrorMessageString_ResourceOverride() {
            var attribute = new DefaultTestAttribute() {
                ErrorMessageResourceName = "Resource2",
                ErrorMessageResourceType = typeof(ResourceClass)
            };

            Assert.AreEqual<string>("Resource 2 Value", attribute.ErrorMessageString);

            Thread.CurrentThread.CurrentUICulture = _germanCulture;
            Assert.AreEqual<string>("Resource 2 Value DE", attribute.ErrorMessageString);
        }

        #endregion
    }
}
