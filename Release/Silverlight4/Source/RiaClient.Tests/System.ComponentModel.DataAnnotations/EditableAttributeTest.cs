// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class EditableAttributeTest {
        [TestMethod]
        [Description("Can construct an EditableAttribute and its default value for AllowInitialValue is set to whatever was provided for allowEdit")]
        public void EditableAttribute_AllowInitialValue_Defaults_To_Match_AllowEdit() {
            EditableAttribute attr = new EditableAttribute(false);
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should default to false when AllowEdit is set to false");

            attr = new EditableAttribute(true);
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should default to true when AllowEdit is set to true");
        }

        [TestMethod]
        [Description("AllowEdit is set to match what was provided in the constructor")]
        public void EditableAttribute_AllowEdit_Matches_Constructor_Argument() {
            EditableAttribute attr = new EditableAttribute(false);
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false");

            attr = new EditableAttribute(true);
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true");
        }

        [TestMethod]
        [Description("Can set AllowInitialValue to true when AllowEdit = false")]
        public void EditableAttribute_AllowInitialValue_Can_Be_True() {
            EditableAttribute attr = new EditableAttribute(false) { AllowInitialValue = true };
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true after being set so");
        }

        [TestMethod]
        [Description("Can set AllowInitialValue to false when AllowEdit = true")]
        public void EditableAttribute_AllowInitialValue_Can_Be_False() {
            EditableAttribute attr = new EditableAttribute(true) { AllowInitialValue = false };
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false after being set so");
        }

        [TestMethod]
        [Description("Can discover the attribute on fields where the AllowInitialValue property is not specified")]
        public void EditableAttribute_Can_Discover_Fields_With_Default_AllowInitialValue() {
            Type employeeType = typeof(Employee);
            FieldInfo field = employeeType.GetField("EmployeeIdField");
            EditableAttribute attr = field.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on EmployeeIdField");
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false for EmployeeIdField");
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false for EmployeeIdField");

            field = employeeType.GetField("FirstNameField");
            attr = field.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on FirstNameField");
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true for FirstNameField");
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true for FirstNameField");
        }

        [TestMethod]
        [Description("Can discover the attribute on fields where the AllowInitialValue property is specified")]
        public void EditableAttribute_Can_Discover_Fields_With_Specified_AllowInitialValue() {
            Type employeeType = typeof(Employee);
            FieldInfo field = employeeType.GetField("EmployeeNumberField");
            EditableAttribute attr = field.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on EmployeeNumberField");
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false for EmployeeNumberField");
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true for EmployeeNumberField");

            field = employeeType.GetField("UsernameField");
            attr = field.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on UsernameField");
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true for UsernameField");
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false for UsernameField");
        }

        [TestMethod]
        [Description("Can discover the attribute on properties where the AllowInitialValue property is not specified")]
        public void EditableAttribute_Can_Discover_Properties_With_Default_AllowInitialValue() {
            Type employeeType = typeof(Employee);
            PropertyInfo prop = employeeType.GetProperty("EmployeeIdProperty");
            EditableAttribute attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on EmployeeIdProperty");
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false for EmployeeIdProperty");
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false for EmployeeIdProperty");

            prop = employeeType.GetProperty("FirstNameProperty");
            attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on FirstNameProperty");
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true for FirstNameProperty");
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true for FirstNameProperty");
        }

        [TestMethod]
        [Description("Can discover the attribute on properties where the AllowInitialValue property is specified")]
        public void EditableAttribute_Can_Discover_Properties_With_Specified_AllowInitialValue() {
            Type employeeType = typeof(Employee);
            PropertyInfo prop = employeeType.GetProperty("EmployeeNumberProperty");
            EditableAttribute attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on EmployeeNumberProperty");
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false for EmployeeNumberProperty");
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true for EmployeeNumberProperty");

            prop = employeeType.GetProperty("UsernameProperty");
            attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on UsernameProperty");
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true for UsernameProperty");
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false for UsernameProperty");
        }

        [TestMethod]
        [Description("Can discover the attribute on properties where the AllowInitialValue property is not specified, overriding a base class")]
        public void EditableAttribute_Can_Discover_Properties_With_Default_AllowInitialValue_Overriding_Base_Class() {
            Type employeeType = typeof(SuperEmployee);
            PropertyInfo prop = employeeType.GetProperty("EmployeeIdProperty");
            EditableAttribute attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on EmployeeIdProperty");
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true for EmployeeIdProperty");
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true for EmployeeIdProperty");

            prop = employeeType.GetProperty("FirstNameProperty");
            attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on FirstNameProperty");
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false for FirstNameProperty");
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false for FirstNameProperty");
        }

        [TestMethod]
        [Description("Can discover the attribute on properties where the AllowInitialValue property is specified, overriding a base class")]
        public void EditableAttribute_Can_Discover_Properties_With_Specified_AllowInitialValue_Overriding_Base_Class() {
            Type employeeType = typeof(SuperEmployee);
            PropertyInfo prop = employeeType.GetProperty("EmployeeNumberProperty");
            EditableAttribute attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on EmployeeNumberProperty");
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true for EmployeeNumberProperty");
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false for EmployeeNumberProperty");

            prop = employeeType.GetProperty("UsernameProperty");
            attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on UsernameProperty");
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false for UsernameProperty");
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true for UsernameProperty");
        }

        [TestMethod]
        [Description("Can discover the attribute on properties where the AllowInitialValue property is not specified, overriding a derived class")]
        public void EditableAttribute_Can_Discover_Properties_With_Default_AllowInitialValue_Overriding_Derived_Class() {
            Type employeeType = typeof(SuperDuperEmployee);
            PropertyInfo prop = employeeType.GetProperty("UsernameProperty");
            EditableAttribute attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on UsernameProperty");
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true for UsernameProperty");
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true for UsernameProperty");

            prop = employeeType.GetProperty("EmployeeNumberProperty");
            attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on EmployeeNumberProperty");
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false for EmployeeNumberProperty");
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false for EmployeeNumberProperty");
        }

        [TestMethod]
        [Description("Can discover the attribute on properties where the AllowInitialValue property is specified, overriding a derived class")]
        public void EditableAttribute_Can_Discover_Properties_With_Specified_AllowInitialValue_Overriding_Derived_Class() {
            Type employeeType = typeof(SuperDuperEmployee);
            PropertyInfo prop = employeeType.GetProperty("FirstNameProperty");
            EditableAttribute attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on FirstNameProperty");
            Assert.IsTrue(attr.AllowEdit, "AllowEdit should be true for FirstNameProperty");
            Assert.IsFalse(attr.AllowInitialValue, "AllowInitialValue should be false for FirstNameProperty");

            prop = employeeType.GetProperty("EmployeeIdProperty");
            attr = prop.GetCustomAttributes(typeof(EditableAttribute), true).FirstOrDefault() as EditableAttribute;
            Assert.IsNotNull(attr, "Should have found an editable attribute on EmployeeIdProperty");
            Assert.IsFalse(attr.AllowEdit, "AllowEdit should be false for EmployeeIdProperty");
            Assert.IsTrue(attr.AllowInitialValue, "AllowInitialValue should be true for EmployeeIdProperty");
        }

        private class Employee {
#pragma warning disable 0649
            [Editable(false)]
            public Guid EmployeeIdField;

            [Editable(false, AllowInitialValue = true)]
            public int EmployeeNumberField;

            [Editable(true)]
            public string FirstNameField;

            [Editable(true, AllowInitialValue = false)]
            public string UsernameField;
#pragma warning restore 0649

            [Editable(false)]
            public virtual Guid EmployeeIdProperty { get; set; }

            [Editable(false, AllowInitialValue = true)]
            public virtual int EmployeeNumberProperty { get; set; }

            [Editable(true)]
            public virtual string FirstNameProperty { get; set; }

            [Editable(true, AllowInitialValue = false)]
            public virtual string UsernameProperty { get; set; }
        }

        private class SuperEmployee : Employee {
            [Editable(true)]
            public override Guid EmployeeIdProperty {
                get {
                    return base.EmployeeIdProperty;
                }
                set {
                    base.EmployeeIdProperty = value;
                }
            }

            [Editable(true, AllowInitialValue = false)]
            public override int EmployeeNumberProperty {
                get {
                    return base.EmployeeNumberProperty;
                }
                set {
                    base.EmployeeNumberProperty = value;
                }
            }

            [Editable(false)]
            public override string FirstNameProperty {
                get {
                    return base.FirstNameProperty;
                }
                set {
                    base.FirstNameProperty = value;
                }
            }

            [Editable(false, AllowInitialValue = true)]
            public override string UsernameProperty {
                get {
                    return base.UsernameProperty;
                }
                set {
                    base.UsernameProperty = value;
                }
            }
        }

        private class SuperDuperEmployee : Employee {
            [Editable(false, AllowInitialValue = true)]
            public override Guid EmployeeIdProperty {
                get {
                    return base.EmployeeIdProperty;
                }
                set {
                    base.EmployeeIdProperty = value;
                }
            }

            [Editable(false)]
            public override int EmployeeNumberProperty {
                get {
                    return base.EmployeeNumberProperty;
                }
                set {
                    base.EmployeeNumberProperty = value;
                }
            }

            [Editable(true, AllowInitialValue = false)]
            public override string FirstNameProperty {
                get {
                    return base.FirstNameProperty;
                }
                set {
                    base.FirstNameProperty = value;
                }
            }

            [Editable(true)]
            public override string UsernameProperty {
                get {
                    return base.UsernameProperty;
                }
                set {
                    base.UsernameProperty = value;
                }
            }
        }
    }
}
