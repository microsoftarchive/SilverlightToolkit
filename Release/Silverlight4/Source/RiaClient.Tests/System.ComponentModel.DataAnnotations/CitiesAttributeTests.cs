// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Ria.Data;
using Cities;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {

    [TestClass]
    public class CitiesAttributeTests : SilverlightTest {
        [TestMethod]
        [Description("State.Name has all expected validation attributes")]
        public void Attributes_Cities_State_Name_Has_Expected_Validation_Attributes() {
            RequiredAttribute r = ExpectPropertyAttribute<RequiredAttribute>(typeof(State), "Name");
            StringLengthAttribute sl = ExpectPropertyAttribute<StringLengthAttribute>(typeof(State), "Name");
            Assert.AreEqual(2, sl.MaximumLength);

            RegularExpressionAttribute regEx = ExpectPropertyAttribute<RegularExpressionAttribute>(typeof(State), "Name");
            Assert.AreEqual("^[A-Z]*", regEx.Pattern);

            CustomValidationAttribute custom = ExpectPropertyAttribute<CustomValidationAttribute>(typeof(State), "Name");
            Assert.AreEqual(typeof(StateNameValidator), custom.ValidatorType);
            Assert.AreEqual("IsStateNameValid", custom.Method);
        }
        
        #region DisplayFormatAttribute
        [TestMethod]
        [Description("[DisplayFormat] has been code genned correctly")]
        public void Attributes_Cities_DisplayFormatAttribute() {

            DisplayFormatAttribute fmt = ExpectPropertyAttribute<DisplayFormatAttribute>(typeof(Zip), "Code");
            Assert.AreEqual("nnnnn", fmt.DataFormatString);
        }
        #endregion DisplayFormatAttribute
        
        #region DescriptionAttribute
        [TestMethod]
        [Description("[Description] has been code genned correctly on type")]
        public void Attributes_Cities_DescriptionAttribute_Type() {

            System.ComponentModel.DescriptionAttribute attr = ExpectTypeAttribute<System.ComponentModel.DescriptionAttribute>(typeof(Zip));
            Assert.AreEqual("Zip code entity", attr.Description);
        }

        [TestMethod]
        [Description("[Description] has been code genned correctly on property")]
        public void Attributes_Cities_DescriptionAttribute_Property() {

            System.ComponentModel.DescriptionAttribute attr = ExpectPropertyAttribute<System.ComponentModel.DescriptionAttribute>(typeof(Zip), "Code");
            Assert.AreEqual("Zip codes must be 5 digits starting with 9", attr.Description);
        }
        #endregion DescriptionAttribute
        
        #region UIHintAttribute
        [TestMethod]
        [Description("[UIHint] has been code genned correctly")]
        public void Attributes_Cities_UIHintAttribute() {
            UIHintAttribute ui = ExpectPropertyAttribute<UIHintAttribute>(typeof(Zip), "FourDigit");
            Assert.AreEqual("DataGrid", ui.UIHint);
            Assert.AreEqual("Jolt", ui.PresentationLayer);
            IDictionary<string, object> controlParams = ui.ControlParameters;
            Assert.IsNotNull(controlParams);
            Assert.AreEqual(2, controlParams.Count);
            Assert.IsTrue(controlParams.ContainsKey("stringParam"));
            Assert.AreEqual("hello", controlParams["stringParam"]);
            Assert.IsTrue(controlParams.ContainsKey("doubleParam"));
            Assert.AreEqual(2.0, controlParams["doubleParam"]);
        }
        #endregion UIHintAttribute
        
        #region DomainIdentifierAttribute
        [TestMethod]
        [Description("[DomainIdentifier] has been code genned on data context")]
        public void Attributes_Cities_DomainService_Has_DomainIdentifier()
        {
            DomainIdentifierAttribute attr = ExpectTypeAttribute<DomainIdentifierAttribute>(typeof(CityDomainContext));
            Assert.AreEqual("CityProvider", attr.Name);
        }

        [TestMethod]
        [Description("[DomainIdentifier] has been code genned on an entity")]
        public void Attributes_Cities_Entity_Has_DomainIdentifier() {
            DomainIdentifierAttribute attr = ExpectTypeAttribute<DomainIdentifierAttribute>(typeof(Zip));
            Assert.AreEqual("ZipPattern", attr.Name);
        }
        #endregion DomainIdentifierAttribute

        #region Bindable
        [Ignore] // 
        [TestMethod]
        [Description("[Bindable] has been code genned on an entity type")]
        public void Attributes_Cities_Entity_Bindable_On_Class() {
            BindableAttribute attr = ExpectTypeAttribute<BindableAttribute>(typeof(City));
            Assert.AreEqual(true, attr.Bindable);
            Assert.AreEqual(BindingDirection.OneWay, attr.Direction);
        }

        [TestMethod]
        [Description("[Bindable] has been code genned on an entity property")]
        public void Attributes_Cities_Entity_Bindable_On_Property() {
            BindableAttribute attr = ExpectPropertyAttribute<BindableAttribute>(typeof(City), "ZoneName");
            Assert.AreEqual(false, attr.Bindable);
            Assert.AreEqual(BindingDirection.OneWay, attr.Direction);
        }

        [TestMethod]
        [Description("[Bindable] with TwoWay has been code genned on an entity type")]
        public void Attributes_Cities_Entity_Bindable_On_Property_TwoWay() {
            BindableAttribute attr = ExpectPropertyAttribute<BindableAttribute>(typeof(City), "ZoneID");
            Assert.AreEqual(true, attr.Bindable);
            Assert.AreEqual(BindingDirection.TwoWay, attr.Direction);
        }

        #endregion Bindable

        #region DisplayAttribute
        [TestMethod]
        [Description("[Display] with resources has been code genned on property")]
        public void Attributes_Cities_Entity_Display_On_Property_Resourced() {
            DisplayAttribute attr = ExpectPropertyAttribute<DisplayAttribute>(typeof(City), "Name");
            Assert.AreEqual("CityCaption", attr.ShortName);
            Assert.AreEqual("Name of City", attr.GetShortName());

            Assert.AreEqual("CityName", attr.Name);
            Assert.AreEqual("CityName", attr.GetName());

            Assert.AreEqual("CityPrompt", attr.Prompt);
            Assert.AreEqual("Enter the city name", attr.GetPrompt());

            Assert.AreEqual("CityHelpText", attr.Description);
            Assert.AreEqual("This is the name of the city", attr.GetDescription());

            Assert.AreEqual(typeof(Cities.Cities_Resources), attr.ResourceType); ;
        }
        #endregion DisplayAttribute

        private T ExpectPropertyAttribute<T>(Type type, string propertyName) where T : Attribute {
            PropertyInfo pInfo = type.GetProperty(propertyName);
            Assert.IsNotNull(pInfo, "Could not locate property " + propertyName + " in type " + type.Name);
            object[] attributes = pInfo.GetCustomAttributes(false);
            T attribute = attributes.OfType<T>().SingleOrDefault<T>();
            Assert.IsNotNull(attribute, "Expected to find [" + typeof(T).Name + "] on property " + propertyName + " in type " + type.Name);
            return attribute;
        }

        private T ExpectTypeAttribute<T>(Type type) where T : Attribute {
            object[] attributes = type.GetCustomAttributes(false);
            T attribute = attributes.OfType<T>().SingleOrDefault<T>();
            Assert.IsNotNull(attribute, "Expected to find [" + typeof(T).Name + "] on type " + type.Name);
            return attribute;
        }
    }
}
