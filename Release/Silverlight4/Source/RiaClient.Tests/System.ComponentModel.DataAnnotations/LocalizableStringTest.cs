// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class LocalizableStringTest {
        [TestMethod]
        [Description("LocalizedValue returns Value when resourceType is null")]
        public void LocalizableString_Literal_Value_Returned() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "theValue";
            Assert.AreEqual("theValue", prop.Value);
            Assert.AreEqual("theValue", prop.GetLocalizableValue());
        }

        [TestMethod]
        [Description("LocalizedValue is null when Value is null regardless of the resource type provided")]
        public void LocalizableString_Null_Value_Returned() {
            LocalizableString prop = new LocalizableString("MyProperty");
            Assert.IsNull(prop.Value);

            Assert.IsNull(prop.GetLocalizableValue(), "LocalizedValue should be null when Value and resourceType are null");

            prop.ResourceType = typeof(LocalizableStringTest);
            Assert.IsNull(prop.GetLocalizableValue(), "LocalizedValue should be null when Value is null and resourceType is not null");
        }

        [TestMethod]
        [Description("Value getter always equals what has been set, defaulting to null")]
        public void LocalizableString_Value_Getter_Equals_Setter() {
            LocalizableString prop = new LocalizableString("MyProperty");
            Assert.IsNull(prop.Value, "Value should be null by default");

            prop.Value = "foo";
            Assert.AreEqual("foo", prop.Value, "Value getter should equal setter after setting it to 'foo'");

            prop.Value = string.Empty;
            Assert.AreEqual(string.Empty, prop.Value, "Value should be string.Empty after setting it to such");

            prop.Value = null;
            Assert.IsNull(prop.Value, "Value should be null after setting it to null");
        }

        [TestMethod]
        [Description("Public Static Int Property Throws InvalidOperationException")]
        public void LocalizableString_PublicStaticIntProperty_Throws() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "PublicStaticIntProperty";
            prop.ResourceType = typeof(LocalizableString_Resources);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { prop.GetLocalizableValue(); }, GetExpectedErrorMessage(prop.Value));
        }

        [TestMethod]
        [Description("Public Instance String Property Throws InvalidOperationException")]
        public void LocalizableString_PublicInstanceStringProperty_Throws() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "PublicInstanceStringProperty";
            prop.ResourceType = typeof(LocalizableString_Resources);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { prop.GetLocalizableValue(); }, GetExpectedErrorMessage(prop.Value));
        }

        [TestMethod]
        [Description("Public Static String Field Throws InvalidOperationException")]
        public void LocalizableString_PublicStaticStringField_Throws() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "PublicStaticStringField";
            prop.ResourceType = typeof(LocalizableString_Resources);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { prop.GetLocalizableValue(); }, GetExpectedErrorMessage(prop.Value));
        }

        [TestMethod]
        [Description("Protected Static String Property Throws InvalidOperationException")]
        public void LocalizableString_ProtectedStaticStringProperty_Throws() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "ProtectedStaticStringProperty";
            prop.ResourceType = typeof(LocalizableString_Resources);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { prop.GetLocalizableValue(); }, GetExpectedErrorMessage(prop.Value));
        }

        [TestMethod]
        [Description("Public Const String Throws InvalidOperationException")]
        public void LocalizableString_PublicConstString_Throws() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "PublicConstString";
            prop.ResourceType = typeof(LocalizableString_Resources);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { prop.GetLocalizableValue(); }, GetExpectedErrorMessage(prop.Value));
        }

        [TestMethod]
        [Description("Public Static String Property on Internal Class Throws InvalidOperationException")]
        public void LocalizableString_PublicStaticStringProperty_On_Internal_Class_Throws() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "PublicStaticString";
            prop.ResourceType = typeof(LocalizableString_Resources_NonPublic);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { prop.GetLocalizableValue(); }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.LocalizableString_LocalizationFailed, "MyProperty", typeof(LocalizableString_Resources_NonPublic).FullName, prop.Value));
        }

        [TestMethod]
        [Description("Public Static String Property on Public Class gets Localized")]
        public void LocalizableString_PublicStaticStringProperty_On_Public_Class_Succeeds() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "PublicStaticString";
            prop.ResourceType = typeof(LocalizableString_Resources);
            Assert.AreEqual("Public Static String", prop.GetLocalizableValue(), "LocalizedValue should contain the value of the public static string property");
        }

        [TestMethod]
        [Description("Ensure that the LocalizableString class can successfully find resources in an actual resource file")]
        public void LocalizableString_Resource_File_Can_Be_Used() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "LocalizableStringTest_Resource";
            prop.ResourceType = typeof(DataAnnotationsTestResources);

            string expected = "This is a localized string";
            Assert.AreEqual(expected, prop.GetLocalizableValue(), "LocalizedValue should contain the localized string found in the resources file");
        }

        [TestMethod]
        [Description("GetLocalizedValue respects the current UI culture")]
        public void LocalizableString_GetLocalizedValue_Respects_Current_UI_Culture() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = "Language";
            prop.ResourceType = typeof(LocalizableString_Resources_WithGermanSupport);

            CultureInfo defaultCulture = CultureInfo.CurrentUICulture;

            try {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                Assert.AreEqual("English", prop.GetLocalizableValue(), "GetLocalizableValue should return 'English' with the 'en' culture");

                Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");
                Assert.AreEqual("German", prop.GetLocalizableValue(), "GetLocalizableValue should return 'German' with the 'de' culture");
            } finally {
                Thread.CurrentThread.CurrentUICulture = defaultCulture;
            }
        }

        [TestMethod]
        [Description("Localization fails when the value specified is empty and ResourceType has been specified")]
        public void LocalizableString_GetLocalizedValue_Fails_When_Resource_Key_Is_Empty() {
            LocalizableString prop = new LocalizableString("MyProperty");
            prop.Value = string.Empty;
            prop.ResourceType = typeof(LocalizableString_Resources);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { prop.GetLocalizableValue(); }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.LocalizableString_LocalizationFailed, "MyProperty", prop.ResourceType.FullName, prop.Value));
        }

        private string GetExpectedErrorMessage(string propertyValue) {
            return String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.LocalizableString_LocalizationFailed, "MyProperty", typeof(LocalizableString_Resources).FullName, propertyValue);
        }
    }

    public class LocalizableString_Resources {
        public static int PublicStaticIntProperty { get { return 5; } }
        public string PublicInstanceStringProperty { get { return "Public Instance String Property"; } }
        public static string PublicStaticStringField = "Public Static String Field";
        protected static string ProtectedStaticStringProperty { get { return "Protected Static String Property"; } }
        public const string PublicConstString = "Public Const String";
        public static string PublicStaticString { get { return "Public Static String"; } }
    }

    internal class LocalizableString_Resources_NonPublic {
        public static string PublicStaticString { get { return "Public Static String"; } }
    }

    public class LocalizableString_Resources_WithGermanSupport {
        public static string Language {
            get {
                return CultureInfo.CurrentUICulture.Name.StartsWith("de", StringComparison.InvariantCultureIgnoreCase) ? "German" : "English";
            }
        }
    }
}
