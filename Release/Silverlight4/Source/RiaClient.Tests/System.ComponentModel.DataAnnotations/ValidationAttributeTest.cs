// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class ValidationAttributeTest {
        private CultureInfo _defaultUiCulture;
        private CultureInfo _englishCulture;
        private CultureInfo _germanCulture;

        #region Setup and cleanup

        public TestContext TestContext {
            get;
            set;
        }

        [TestInitialize]
        public void SetUp() {
            _defaultUiCulture = CultureInfo.CurrentUICulture;
            _englishCulture = new CultureInfo("en");
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
            protected override ValidationResult IsValid(object value, ValidationContext context) {
                return ValidationResult.Success;
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
            protected override ValidationResult IsValid(object value, ValidationContext context) {
                return new ValidationResult(null);
            }
        }

        public class NotImplementedAttribute : ValidationAttribute {
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

            internal static string InternalResource {
                get { return "InternalResource 1"; }
            }

            private static string PrivateResource {
                get { return "PrivateResource 1"; }
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
            }, Resources.DataAnnotationsResources.ValidationAttribute_Cannot_Set_ErrorMessage_And_Resource);
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
                object instance = new object();
                new FailingTestAttribute().Validate(instance, new ValidationContext(instance, null, null) { MemberName = "prop" });
            });
        }

        [TestMethod]
        public void Validate_Pass() {
            object instance = new object();
            new DefaultTestAttribute().Validate(instance, new ValidationContext(instance, null, null) { MemberName = "prop" });
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
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.ValidationAttribute_ResourceTypeDoesNotHaveProperty, typeof(ValidationAttributeTest.ResourceClass).FullName, "NonexistantResourceName"));
        }
#if !SILVERLIGHT
        [TestMethod]
        public void Construction_ResourceMode_PrivateResourceName() {
            // Setup
            DefaultTestAttribute attr = new DefaultTestAttribute() {
                ErrorMessageResourceName = "PrivateResource",
                ErrorMessageResourceType = typeof(ResourceClass)
            };

            // Execute & Verify
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.ValidationAttribute_ResourceTypeDoesNotHaveProperty, typeof(ValidationAttributeTest.ResourceClass).FullName, "PrivateResource"));
        }

        [TestMethod]
        public void Construction_ResourceMode_InternalResourceName() {
            // Setup
            DefaultTestAttribute attr = new DefaultTestAttribute() {
                ErrorMessageResourceName = "InternalResource",
                ErrorMessageResourceType = typeof(ResourceClass)
            };

            // Execute
            attr.FormatErrorMessage("");

            // Verify
            Assert.AreEqual("InternalResource 1", attr.ErrorMessageString);
            Assert.IsTrue(attr.CustomErrorMessageSet);
        }
#endif

        [TestMethod]
        public void Construction_ResourceMode_NonStringResource() {
            DefaultTestAttribute attr = new DefaultTestAttribute() {
                ErrorMessageResourceName = "NonStringResource",
                ErrorMessageResourceType = typeof(ResourceClass)
            };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.ValidationAttribute_ResourcePropertyNotStringType, "NonStringResource", typeof(ValidationAttributeTest.ResourceClass).FullName));
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
            }, Resources.DataAnnotationsResources.ValidationAttribute_Cannot_Set_ErrorMessage_And_Resource);
        }

        [TestMethod]
        [Description("Setting only ErrorMessageResourceName but not ResourceType fails")]
        public void ValidationAttribute_Fail_ResourceName_Without_ResourceType() {
            DefaultTestAttribute attr = new DefaultTestAttribute() { ErrorMessageResourceName = "x" };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, Resources.DataAnnotationsResources.ValidationAttribute_NeedBothResourceTypeAndResourceName);
        }

        [TestMethod]
        [Description("Setting only ErrorMessage and ErrorMessageResourceName fails")]
        public void ValidationAttribute_Fail_ResourceName_And_ErrorMessage() {
            DefaultTestAttribute attr = new DefaultTestAttribute() { ErrorMessage = "x", ErrorMessageResourceName = "y" };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, Resources.DataAnnotationsResources.ValidationAttribute_Cannot_Set_ErrorMessage_And_Resource);
        }

        [TestMethod]
        [Description("Setting only ErrorMessage and ErrorMessageResourceType fails")]
        public void ValidationAttribute_Fail_ResourceType_And_ErrorMessage() {
            DefaultTestAttribute attr = new DefaultTestAttribute() { ErrorMessage = "x", ErrorMessageResourceType = typeof(ResourceClass) };
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.FormatErrorMessage("");
            }, Resources.DataAnnotationsResources.ValidationAttribute_NeedBothResourceTypeAndResourceName);
        }

        [TestMethod]
        public void ErrorMessageString_NoOverrides_BaseDefaultMessage() {
            Assert.AreEqual<string>("The field {0} is invalid.", new DefaultTestAttribute().ErrorMessageString);
        }

        [TestMethod]
        public void ErrorMessageString_NoOverrides_AttributeDefaultMessage() {
            var attribute = new CustomTestAttribte();

            Thread.CurrentThread.CurrentUICulture = _englishCulture;
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

            Thread.CurrentThread.CurrentUICulture = _englishCulture;
            Assert.AreEqual<string>("Resource 2 Value", attribute.ErrorMessageString);

            Thread.CurrentThread.CurrentUICulture = _germanCulture;
            Assert.AreEqual<string>("Resource 2 Value DE", attribute.ErrorMessageString);
        }

        [TestMethod]
        public void ValidationAttribute_Throws_NotImplementedException_From_GetValidationResult() {
            var attribute = new NotImplementedAttribute();
            var instance = new object();
            ExceptionHelper.ExpectException<NotImplementedException>(delegate() {
                attribute.GetValidationResult(instance, new ValidationContext(instance, null, null));
            });
        }

        [TestMethod]
        public void ValidationAttribute_Throws_NotImplementedException_From_IsValid() {
            var attribute = new NotImplementedAttribute();
            ExceptionHelper.ExpectException<NotImplementedException>(delegate() {
                attribute.IsValid(null);
            });
        }

        #endregion
    }
}
