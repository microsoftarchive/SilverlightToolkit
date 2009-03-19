//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_DependencyProperties.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests <see cref="DataForm"/> dependency properties.
    /// </summary>
    [TestClass]
    public class DataFormTests_DependencyProperties : DataFormTests_Base
    {
        #region Helper Fields

        /// <summary>
        /// The <see cref="DataForm"/> being used.
        /// </summary>
        private DataForm dataForm;

        #endregion Helper Fields

        #region Initialization

        /// <summary>
        /// Initializes the test framework.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.dataForm = new DataForm();
        }

        #endregion Initialization

        /// <summary>
        /// Verify the AutoCommit dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the AutoCommit dependency property.")]
        public void AutoCommit()
        {
            VerifyDependencyPropertyWithName(
                "AutoCommit",
                typeof(bool),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                true,
                false,
                true);
        }

        /// <summary>
        /// Verify the AutoEdit dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the AutoEdit dependency property.")]
        public void AutoEdit()
        {
            VerifyDependencyPropertyWithName(
                "AutoEdit",
                typeof(bool),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                true,
                false,
                true);
        }

        /// <summary>
        /// Verify the AutoGenerateFields dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the AutoGenerateFields dependency property.")]
        public void AutoGenerateFields()
        {
            VerifyDependencyPropertyWithName(
                "AutoGenerateFields",
                typeof(bool),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                true,
                false,
                true);
        }

        /// <summary>
        /// Verify the CommandButtonsVisibility dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the CommandButtonsVisibility dependency property.")]
        public void CommandButtonsVisibility()
        {
            VerifyDependencyPropertyWithName(
                "CommandButtonsVisibility",
                typeof(DataFormCommandButtonsVisibility),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                DataFormCommandButtonsVisibility.None,
                DataFormCommandButtonsVisibility.Edit,
                DataFormCommandButtonsVisibility.All);
        }

        /// <summary>
        /// Verify the CommitButtonContent dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the CommitButtonContent dependency property.")]
        public void CommitButtonContent()
        {
            VerifyDependencyPropertyWithName(
                "CommitButtonContent",
                typeof(object),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                null,
                "Commit",
                "OK");
        }

        /// <summary>
        /// Verify the CanUserAddItems dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the CanUserAddItems dependency property.")]
        public void CanUserAddItems()
        {
            VerifyDependencyPropertyWithName(
                "CanUserAddItems",
                typeof(bool),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                true,
                false,
                true);
        }

        /// <summary>
        /// Verify the CanUserDeleteItems dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the CanUserDeleteItems dependency property.")]
        public void CanUserDeleteItems()
        {
            VerifyDependencyPropertyWithName(
                "CanUserDeleteItems",
                typeof(bool),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                true,
                false,
                true);
        }

        /// <summary>
        /// Verify the CurrentItem dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the CurrentItem dependency property.")]
        public void CurrentItem()
        {
            VerifyDependencyPropertyWithName(
                "CurrentItem",
                typeof(object),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                new object(),
                new BasicDataClass(),
                new DataClass());
        }

        /// <summary>
        /// Verify the DescriptionViewerPosition dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the DescriptionViewerPosition dependency property.")]
        public void DescriptionViewerPosition()
        {
            VerifyDependencyPropertyWithName(
                "DescriptionViewerPosition",
                typeof(DataFormDescriptionViewerPosition),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                DataFormDescriptionViewerPosition.Auto,
                DataFormDescriptionViewerPosition.BesideContent,
                DataFormDescriptionViewerPosition.BesideLabel);
        }

        /// <summary>
        /// Verify the DescriptionViewerStyle dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the DescriptionViewerStyle dependency property.")]
        public void DescriptionViewerStyle()
        {
            VerifyDependencyPropertyWithName(
                "DescriptionViewerStyle",
                typeof(Style),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm);
        }

        /// <summary>
        /// Verify the DisplayTemplate dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the DisplayTemplate dependency property.")]
        public void DisplayTemplate()
        {
            VerifyDependencyPropertyWithName(
                "DisplayTemplate",
                typeof(DataTemplate),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                new TemplateFieldDisplayTemplate(),
                new TemplateFieldEditTemplate(),
                new TemplateFieldInsertTemplate());
        }

        /// <summary>
        /// Verify the EditTemplate dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the EditTemplate dependency property.")]
        public void EditTemplate()
        {
            VerifyDependencyPropertyWithName(
                "EditTemplate",
                typeof(DataTemplate),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                new TemplateFieldDisplayTemplate(),
                new TemplateFieldEditTemplate(),
                new TemplateFieldInsertTemplate());
        }

        /// <summary>
        /// Verify the FieldLabelPosition dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the FieldLabelPosition dependency property.")]
        public void FieldLabelPosition()
        {
            VerifyDependencyPropertyWithName(
                "FieldLabelPosition",
                typeof(DataFormFieldLabelPosition),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                DataFormFieldLabelPosition.Auto,
                DataFormFieldLabelPosition.Left,
                DataFormFieldLabelPosition.Top);
        }

        /// <summary>
        /// Verify the FieldLabelStyle dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the FieldLabelStyle dependency property.")]
        public void FieldLabelStyle()
        {
            VerifyDependencyPropertyWithName(
                "FieldLabelStyle",
                typeof(Style),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm);
        }

        /// <summary>
        /// Verify the Header dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the Header dependency property.")]
        public void Header()
        {
            VerifyDependencyPropertyWithName(
                "Header",
                typeof(object),
                true /* expectGet */,
                true /* expectSet */,
                false /* hasCallback */,
                this.dataForm,
                null,
                "DataForm 1",
                "DataForm 2");
        }

        /// <summary>
        /// Verify the HeaderTemplate dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the HeaderTemplate dependency property.")]
        public void HeaderTemplate()
        {
            VerifyDependencyPropertyWithName(
                "HeaderTemplate",
                typeof(DataTemplate),
                true /* expectGet */,
                true /* expectSet */,
                false /* hasCallback */,
                this.dataForm,
                null,
                new TemplateFieldEditTemplate(),
                new TemplateFieldInsertTemplate());
        }

        /// <summary>
        /// Verify the InsertTemplate dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the InsertTemplate dependency property.")]
        public void InsertTemplate()
        {
            VerifyDependencyPropertyWithName(
                "InsertTemplate",
                typeof(DataTemplate),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                new TemplateFieldDisplayTemplate(),
                new TemplateFieldEditTemplate(),
                new TemplateFieldInsertTemplate());
        }

        /// <summary>
        /// Verify the IsReadOnly dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the IsReadOnly dependency property.")]
        public void IsReadOnly()
        {
            VerifyDependencyPropertyWithName(
                "IsReadOnly",
                typeof(bool),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                true,
                false,
                true);
        }

        /// <summary>
        /// Verify the ItemsSource dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the ItemsSource dependency property.")]
        public void ItemsSource()
        {
            VerifyDependencyPropertyWithName(
                "ItemsSource",
                typeof(IEnumerable),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                new Collection<string>(),
                new List<int>(),
                DataClassList.GetDataClassList(10, ListOperations.All));
        }

        /// <summary>
        /// Verify the Orientation dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the Orientation dependency property.")]
        public void Orientation()
        {
            VerifyDependencyPropertyWithName(
                "Orientation",
                typeof(Orientation),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                System.Windows.Controls.Orientation.Horizontal,
                System.Windows.Controls.Orientation.Vertical,
                System.Windows.Controls.Orientation.Horizontal);
        }

        /// <summary>
        /// Verify the WrapAfter dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the WrapAfter dependency property.")]
        public void WrapAfter()
        {
            VerifyDependencyPropertyWithName(
                "WrapAfter",
                typeof(int),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                0,
                1,
                2);
        }

        #region Helper Methods

        /// <summary>
        /// Verify the given dependency property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="expectGet">Whether its CLR property has a getter.</param>
        /// <param name="expectSet">Whether its CLR property has a setter.</param>
        /// <param name="hasCallback">Whether the property has a callback.</param>
        /// <param name="dataFormInstance">The <see cref="DataForm"/>.</param>
        /// <param name="valuesToSet">The values to test the setter with.</param>
        private static void VerifyDependencyPropertyWithName(string name, Type propertyType, bool expectGet, bool expectSet, bool hasCallback, DataForm dataFormInstance, params object[] valuesToSet)
        {
            // Ensure that the DependencyProperty itself exists.
            FieldInfo fieldInfo = typeof(DataForm).GetField(name + "Property", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(fieldInfo);
            Assert.AreEqual(typeof(DependencyProperty), fieldInfo.FieldType);

            DependencyProperty property = fieldInfo.GetValue(null) as DependencyProperty;
            Assert.IsNotNull(property);


            // Ensure that its corresponding CLR property exists.
            PropertyInfo propertyInfo = typeof(DataForm).GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(propertyType, propertyInfo.PropertyType);

            // Ensure that it can be got and set as expected.
            Assert.AreEqual(expectGet, propertyInfo.CanRead);
            Assert.AreEqual(expectSet, propertyInfo.CanWrite);

            if (expectSet)
            {
                // Make sure that what we set is what we get.
                foreach (object value in valuesToSet)
                {
                    propertyInfo.SetValue(dataFormInstance, value, null);
                    Assert.AreEqual(value, propertyInfo.GetValue(dataFormInstance, null));
                }
            }

            // Check to make sure that the callback is present or not present as expected.
            MethodInfo methodInfo = typeof(DataForm).GetMethod("On" + name + "PropertyChanged", BindingFlags.Static | BindingFlags.NonPublic);

            if (hasCallback)
            {
                Assert.IsNotNull(methodInfo);
            }
            else
            {
                Assert.IsNull(methodInfo);
            }
        }

        #endregion Helper Methods
    }
}
