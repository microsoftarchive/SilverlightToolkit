using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class DisplayAttributeTest {
        private const string LocalizationFailedExceptionMessage = "Cannot retrieve property '{0}' because localization failed.  Type '{1}' is not public or does not contain a public static string property with the name '{2}'.";

        [TestMethod]
        [Description("DisplayAttribute handles literals for all string properties")]
        public void DisplayAttribute_Literal_Properties() {
            DisplayAttribute attr = new DisplayAttribute();

            Assert.IsNull(attr.ResourceType);
            Assert.IsNull(attr.ShortName);
            Assert.IsNull(attr.Name);
            Assert.IsNull(attr.Prompt);
            Assert.IsNull(attr.Description);
            Assert.IsNull(attr.GroupName);

            Assert.IsNull(attr.GetShortName());
            Assert.IsNull(attr.GetName());
            Assert.IsNull(attr.GetPrompt());
            Assert.IsNull(attr.GetDescription());
            Assert.IsNull(attr.GetGroupName());

            attr.ShortName = "theShortName";
            attr.Name = "theName";
            attr.Prompt = "thePrompt";
            attr.Description = "theDescription";
            attr.GroupName = "theGroupName";

            Assert.AreEqual("theShortName", attr.GetShortName());
            Assert.AreEqual("theName", attr.GetName());
            Assert.AreEqual("thePrompt", attr.GetPrompt());
            Assert.AreEqual("theDescription", attr.GetDescription());
            Assert.AreEqual("theGroupName", attr.GetGroupName());

            attr.ShortName = String.Empty;
            attr.Name = String.Empty;
            attr.Prompt = String.Empty;
            attr.Description = String.Empty;
            attr.GroupName = String.Empty;

            Assert.AreEqual(String.Empty, attr.GetShortName());
            Assert.AreEqual(String.Empty, attr.GetName());
            Assert.AreEqual(String.Empty, attr.GetPrompt());
            Assert.AreEqual(String.Empty, attr.GetDescription());
            Assert.AreEqual(String.Empty, attr.GetGroupName());
        }

        [TestMethod]
        [Description("DisplayAttribute handles resources for all string properties")]
        public void DisplayAttribute_Resourced_Properties() {
            DisplayAttribute attr = new DisplayAttribute();

            attr.ResourceType = typeof(DisplayAttribute_Resources);

            Assert.IsNull(attr.GetShortName());
            Assert.IsNull(attr.GetName());
            Assert.IsNull(attr.GetPrompt());
            Assert.IsNull(attr.GetDescription());
            Assert.IsNull(attr.GetGroupName());

            attr.ShortName = "Resource1";
            attr.Name = "Resource2";
            attr.Prompt = "Resource3";
            attr.Description = "Resource4";
            attr.GroupName = "Resource5";

            Assert.AreEqual("string1", attr.GetShortName());
            Assert.AreEqual("string2", attr.GetName());
            Assert.AreEqual("string3", attr.GetPrompt());
            Assert.AreEqual("string4", attr.GetDescription());
            Assert.AreEqual("string5", attr.GetGroupName());

            Assert.AreEqual("Resource1", attr.ShortName);
            Assert.AreEqual("Resource2", attr.Name);
            Assert.AreEqual("Resource3", attr.Prompt);
            Assert.AreEqual("Resource4", attr.Description);
            Assert.AreEqual("Resource5", attr.GroupName);
        }

        [TestMethod]
        [Description("DisplayAttribute using resource type throws InvalidOperationExceptions if no corresponding key")]
        public void DisplayAttribute_Resourced_Properties_Wrong_Keys() {
            DisplayAttribute attr = new DisplayAttribute();

            attr.ResourceType = typeof(DisplayAttribute_Resources);

            attr.ShortName = "notAKey1";
            attr.Name = "notAKey2";
            attr.Prompt = "notAKey3";
            attr.Description = "notAKey4";
            attr.GroupName = "notAKey5";

            Assert.AreEqual("notAKey1", attr.ShortName);
            Assert.AreEqual("notAKey2", attr.Name);
            Assert.AreEqual("notAKey3", attr.Prompt);
            Assert.AreEqual("notAKey4", attr.Description);
            Assert.AreEqual("notAKey5", attr.GroupName);

            string shortNameError = string.Format(LocalizationFailedExceptionMessage, "ShortName", attr.ResourceType.FullName, attr.ShortName);
            string nameError = string.Format(LocalizationFailedExceptionMessage, "Name", attr.ResourceType.FullName, attr.Name);
            string promptError = string.Format(LocalizationFailedExceptionMessage, "Prompt", attr.ResourceType.FullName, attr.Prompt);
            string descriptionError = string.Format(LocalizationFailedExceptionMessage, "Description", attr.ResourceType.FullName, attr.Description);
            string groupNameError = string.Format(LocalizationFailedExceptionMessage, "GroupName", attr.ResourceType.FullName, attr.GroupName);

            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetShortName(); }, shortNameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetName(); }, nameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetPrompt(); }, promptError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetDescription(); }, descriptionError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetGroupName(); }, groupNameError);
        }

        [TestMethod]
        [Description("DisplayAttribute can be attached to types, fields, parameters, and properties")]
        public void DisplayAttribute_Reflection_Test() {
            Type t = typeof(DisplayAttribute_Sample);

            FieldInfo fInfo = t.GetField("stringField");
            Assert.IsNotNull(fInfo);
            Assert.IsNotNull(fInfo.GetCustomAttributes(true).OfType<DisplayAttribute>().SingleOrDefault());

            PropertyInfo pInfo = t.GetProperty("LiteralStringProperty");
            Assert.IsNotNull(pInfo);
            DisplayAttribute attr = pInfo.GetCustomAttributes(true).OfType<DisplayAttribute>().SingleOrDefault();
            Assert.IsNotNull(attr);

            Assert.IsNull(attr.ResourceType);

            Assert.AreEqual("theShortName", attr.ShortName);
            Assert.AreEqual("theName", attr.Name);
            Assert.AreEqual("thePrompt", attr.Prompt);
            Assert.AreEqual("theDescription", attr.Description);
            Assert.AreEqual("theGroupName", attr.GroupName);


            Assert.AreEqual("theShortName", attr.GetShortName());
            Assert.AreEqual("theName", attr.GetName());
            Assert.AreEqual("thePrompt", attr.GetPrompt());
            Assert.AreEqual("theDescription", attr.GetDescription());
            Assert.AreEqual("theGroupName", attr.GetGroupName());


            pInfo = t.GetProperty("ResourcedStringProperty");
            Assert.IsNotNull(pInfo);
            attr = pInfo.GetCustomAttributes(true).OfType<DisplayAttribute>().SingleOrDefault();
            Assert.IsNotNull(attr);

            Assert.AreEqual(typeof(DisplayAttribute_Resources), attr.ResourceType);

            Assert.AreEqual("string1", attr.GetShortName());
            Assert.AreEqual("Resource1", attr.ShortName);

            Assert.AreEqual("string2", attr.GetName());
            Assert.AreEqual("Resource2", attr.Name);

            Assert.AreEqual("string3", attr.GetPrompt());
            Assert.AreEqual("Resource3", attr.Prompt);

            Assert.AreEqual("string4", attr.GetDescription());
            Assert.AreEqual("Resource4", attr.Description);

            Assert.AreEqual("string5", attr.GetGroupName());
            Assert.AreEqual("Resource5", attr.GroupName);
        }

        [TestMethod]
        [Description("DisplayAttribute fails for non public resource type")]
        public void DisplayAttribute_Fail_NonPublic_Resource_Type() {
            DisplayAttribute attr = new DisplayAttribute();
            attr.ResourceType = typeof(DisplayAttribute_Private_Resource);
            attr.ShortName = "Resource1";
            attr.Name = "Resource1";
            attr.Prompt = "Resource1";
            attr.Description = "Resource1";
            attr.GroupName = "Resource1";

            string shortNameError = string.Format(LocalizationFailedExceptionMessage, "ShortName", attr.ResourceType.FullName, attr.ShortName);
            string nameError = string.Format(LocalizationFailedExceptionMessage, "Name", attr.ResourceType.FullName, attr.Name);
            string promptError = string.Format(LocalizationFailedExceptionMessage, "Prompt", attr.ResourceType.FullName, attr.Prompt);
            string descriptionError = string.Format(LocalizationFailedExceptionMessage, "Description", attr.ResourceType.FullName, attr.Description);
            string groupNameError = string.Format(LocalizationFailedExceptionMessage, "GroupName", attr.ResourceType.FullName, attr.GroupName);

            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetShortName(); }, shortNameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetName(); }, nameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetPrompt(); }, promptError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetDescription(); }, descriptionError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetGroupName(); }, groupNameError);
        }

        [TestMethod]
        [Description("DisplayAttribute fails for non-public resource")]
        public void DisplayAttribute_NonPublic_Resource() {
            DisplayAttribute attr = new DisplayAttribute();
            attr.ResourceType = typeof(DisplayAttribute_Resources);
            attr.ShortName = "NotPublic";
            attr.Name = "NotPublic";
            attr.Prompt = "NotPublic";
            attr.Description = "NotPublic";
            attr.GroupName = "NotPublic";

            string shortNameError = string.Format(LocalizationFailedExceptionMessage, "ShortName", attr.ResourceType.FullName, attr.ShortName);
            string nameError = string.Format(LocalizationFailedExceptionMessage, "Name", attr.ResourceType.FullName, attr.Name);
            string promptError = string.Format(LocalizationFailedExceptionMessage, "Prompt", attr.ResourceType.FullName, attr.Prompt);
            string descriptionError = string.Format(LocalizationFailedExceptionMessage, "Description", attr.ResourceType.FullName, attr.Description);
            string groupNameError = string.Format(LocalizationFailedExceptionMessage, "GroupName", attr.ResourceType.FullName, attr.GroupName);

            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetShortName(); }, shortNameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetName(); }, nameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetPrompt(); }, promptError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetDescription(); }, descriptionError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetGroupName(); }, groupNameError);
        }

        [TestMethod]
        [Description("DisplayAttribute  fails for non-static resource")]
        public void DisplayAttribute_Fail_NonStatic_Resource() {
            DisplayAttribute attr = new DisplayAttribute();
            attr.ResourceType = typeof(DisplayAttribute_Resources);
            attr.ShortName = "NotStatic";
            attr.Name = "NotStatic";
            attr.Prompt = "NotStatic";
            attr.Description = "NotStatic";
            attr.GroupName = "NotStatic";

            string shortNameError = string.Format(LocalizationFailedExceptionMessage, "ShortName", attr.ResourceType.FullName, attr.ShortName);
            string nameError = string.Format(LocalizationFailedExceptionMessage, "Name", attr.ResourceType.FullName, attr.Name);
            string promptError = string.Format(LocalizationFailedExceptionMessage, "Prompt", attr.ResourceType.FullName, attr.Prompt);
            string descriptionError = string.Format(LocalizationFailedExceptionMessage, "Description", attr.ResourceType.FullName, attr.Description);
            string groupNameError = string.Format(LocalizationFailedExceptionMessage, "GroupName", attr.ResourceType.FullName, attr.GroupName);

            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetShortName(); }, shortNameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetName(); }, nameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetPrompt(); }, promptError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetDescription(); }, descriptionError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetGroupName(); }, groupNameError);
        }

        [TestMethod]
        [Description("DisplayAttribute fails for resource without getter")]
        public void DisplayAttribute_Fail_No_Getter_Resource() {
            DisplayAttribute attr = new DisplayAttribute();
            attr.ResourceType = typeof(DisplayAttribute_Resources);
            attr.ShortName = "NoGetter";
            attr.Name = "NoGetter";
            attr.Prompt = "NoGetter";
            attr.Description = "NoGetter";
            attr.GroupName = "NoGetter";

            string shortNameError = string.Format(LocalizationFailedExceptionMessage, "ShortName", attr.ResourceType.FullName, attr.ShortName);
            string nameError = string.Format(LocalizationFailedExceptionMessage, "Name", attr.ResourceType.FullName, attr.Name);
            string promptError = string.Format(LocalizationFailedExceptionMessage, "Prompt", attr.ResourceType.FullName, attr.Prompt);
            string descriptionError = string.Format(LocalizationFailedExceptionMessage, "Description", attr.ResourceType.FullName, attr.Description);
            string groupNameError = string.Format(LocalizationFailedExceptionMessage, "GroupName", attr.ResourceType.FullName, attr.GroupName);

            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetShortName(); }, shortNameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetName(); }, nameError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetPrompt(); }, promptError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetDescription(); }, descriptionError);
            ExceptionHelper.ExpectException<InvalidOperationException>(() => { string s = attr.GetGroupName(); }, groupNameError);
        }

        [TestMethod]
        [Description("Order getter == setter")]
        public void DisplayAttribute_Order_Property_BehavesCorrectly() {
            DisplayAttribute attr = new DisplayAttribute() { Order = 1 };
            Assert.AreEqual(1, attr.Order, "The Order property should match what was specified");

            Assert.AreEqual(0, new DisplayAttribute().Order, "Order should be 0 by default");
        }

        [TestMethod]
        [Description("AutoGenerateField defaults to false but can be set to true")]
        public void DisplayAttribute_AutoGenerateField_Defaults_And_Can_Be_Set() {
            DisplayAttribute attr = new DisplayAttribute();

            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.AutoGenerateField.ToString();
            }, "The AutoGenerateField property has not been set. Use the GetAutoGenerateField method to get the value.");
            Assert.IsNull(attr.GetAutoGenerateField(), "GetAutoGenerateField should be null by default");

            attr.AutoGenerateField = true;
            Assert.IsTrue(attr.AutoGenerateField, "AutoGenerateField should be true after setting it to true");
            Assert.IsTrue(attr.GetAutoGenerateField().Value, "GetAutoGenerateField should be true after setting it to true");
        }

        [TestMethod]
        [Description("AutoGenerateFilter defaults to false but can be set to true")]
        public void DisplayAttribute_AutoGenerateFilter_Defaults_And_Can_Be_Set() {
            DisplayAttribute attr = new DisplayAttribute();

            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.AutoGenerateFilter.ToString();
            }, "The AutoGenerateFilter property has not been set. Use the GetAutoGenerateFilter method to get the value.");
            Assert.IsNull(attr.GetAutoGenerateFilter(), "GetAutoGenerateFilter should be null by default");

            attr.AutoGenerateFilter = true;
            Assert.IsTrue(attr.AutoGenerateFilter, "AutoGenerateFilter should be true after setting it to true");
            Assert.IsTrue(attr.GetAutoGenerateFilter().Value, "GetAutoGenerateFilter should be true after setting it to true");
        }

        [TestMethod]
        [Description("GetShortName falls back onto GetName, using a non-localized Name")]
        public void DisplayAttribute_GetShortName_Falls_Back_Onto_NonLocalized_Name() {
            DisplayAttribute attr = new DisplayAttribute { Name = "Name" };
            Assert.AreEqual("Name", attr.GetShortName(), "GetShortName should return the Name when ShortName is null");
        }

        [TestMethod]
        [Description("GetShortName falls back onto GetName, using a localized Name")]
        public void DisplayAttribute_GetShortName_Falls_Back_Onto_Localized_Name() {
            DisplayAttribute attr = new DisplayAttribute { ResourceType = typeof(DisplayAttribute_Resources), Name = "Resource1" };
            Assert.AreEqual(DisplayAttribute_Resources.Resource1, attr.GetShortName(), "GetShortName should return the localized Name when ShortName is null");
        }

        [TestMethod]
        [Description("GetName does not fall back onto any other values when Name is null")]
        public void DisplayAttribute_GetName_Does_Not_Fall_Back() {
            DisplayAttribute attr = new DisplayAttribute { ShortName = "ShortName", Description = "Description", Prompt = "Prompt", GroupName = "GroupName" };
            Assert.IsNull(attr.GetName(), "GetName should NOT fall back onto any other values");
        }

        [TestMethod]
        [Description("GetDescription does not fall back onto any other values when Description is null")]
        public void DisplayAttribute_GetDescription_Does_Not_Fall_Back() {
            DisplayAttribute attr = new DisplayAttribute { ShortName = "ShortName", Name = "Name", Prompt = "Prompt", GroupName = "GroupName" };
            Assert.IsNull(attr.GetDescription(), "Description should NOT fall back onto any other values");
        }

        [TestMethod]
        [Description("GetPrompt does not fall back onto any other values when Prompt is null")]
        public void DisplayAttribute_GetPrompt_Does_Not_Fall_Back() {
            DisplayAttribute attr = new DisplayAttribute { ShortName = "ShortName", Name = "Name", Description = "Description", GroupName = "GroupName" };
            Assert.IsNull(attr.GetPrompt(), "GetPrompt should NOT fall back onto any other values");
        }

        [TestMethod]
        [Description("GetGroupName does not fall back onto any other values when GroupName is null")]
        public void DisplayAttribute_GetGroupName_Does_Not_Fall_Back() {
            DisplayAttribute attr = new DisplayAttribute { ShortName = "ShortName", Name = "Name", Description = "Description", Prompt = "Prompt" };
            Assert.IsNull(attr.GetGroupName(), "GetGroupName should NOT fall back onto any other values");
        }
    }

    public class DisplayAttribute_Resources {
        public static string Resource1 { get { return "string1"; } }
        public static string Resource2 { get { return "string2"; } }
        public static string Resource3 { get { return "string3"; } }
        public static string Resource4 { get { return "string4"; } }
        public static string Resource5 { get { return "string5"; } }

        public string NotStatic { get { return "notStaticString"; } }
        internal static string NotPublic { get { return "notPublicString"; } }
        public static string NoGetter { set { } }

    }

    // Private resource class should not be used
    internal class DisplayAttribute_Private_Resource {
        public static string Resource1 { get { return "string1"; } }
    }

    public class DisplayAttribute_Sample {
        [Display]  // verify works on field
        public string stringField;

        [Display(ShortName = "theShortName", Name = "theName", Prompt = "thePrompt", Description = "theDescription", GroupName = "theGroupName")]  // verify works on property
        public string LiteralStringProperty { get { return "hi"; } }

        [Display(ResourceType = typeof(DisplayAttribute_Resources), ShortName = "Resource1", Name = "Resource2", Prompt = "Resource3", Description = "Resource4", GroupName = "Resource5")]  // verify works on property
        public string ResourcedStringProperty { get { return "hi"; } }

        // verify works on parameter
        public string StringMethod([Display] string parameter) { return parameter; }
    }
}
