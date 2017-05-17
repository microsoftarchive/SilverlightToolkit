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
            this.dataForm = new DataForm() { AutoEdit = false };
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
                typeof(DataFormCommandButtonsVisibility?),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                null,
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
        /// Verify the Content dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the Content dependency property.")]
        public void Content()
        {
            VerifyDependencyPropertyWithName(
                "Content",
                typeof(FrameworkElement),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                null,
                new TextBox(),
                new Button(),
                new StackPanel());
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
                typeof(DataFieldDescriptionViewerPosition),
                true /* expectGet */,
                true /* expectSet */,
                false /* hasCallback */,
                this.dataForm,
                DataFieldDescriptionViewerPosition.Auto,
                DataFieldDescriptionViewerPosition.BesideContent,
                DataFieldDescriptionViewerPosition.BesideLabel);
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
        /// Verify the LabelPosition dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the LabelPosition dependency property.")]
        public void LabelPosition()
        {
            VerifyDependencyPropertyWithName(
                "LabelPosition",
                typeof(DataFieldLabelPosition),
                true /* expectGet */,
                true /* expectSet */,
                false /* hasCallback */,
                this.dataForm,
                DataFieldLabelPosition.Auto,
                DataFieldLabelPosition.Left,
                DataFieldLabelPosition.Top);
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
                true /* hasCallback */,
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
                true /* hasCallback */,
                this.dataForm,
                null,
                new TemplateFieldEditTemplate(),
                new TemplateFieldInsertTemplate());
        }

        /// <summary>
        /// Verify the HeaderVisibility dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the HeaderVisibility dependency property.")]
        public void HeaderVisibility()
        {
            VerifyDependencyPropertyWithName(
                "HeaderVisibility",
                typeof(Visibility),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                Visibility.Collapsed,
                Visibility.Visible,
                Visibility.Collapsed);
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
        /// Verify the NewItemTemplate dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the NewItemTemplate dependency property.")]
        public void NewItemTemplate()
        {
            VerifyDependencyPropertyWithName(
                "NewItemTemplate",
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
        /// Verify the ReadOnlyTemplate dependency property.
        /// </summary>
        [TestMethod]
        [Description("Verify the ReadOnlyTemplate dependency property.")]
        public void ReadOnlyTemplate()
        {
            VerifyDependencyPropertyWithName(
                "ReadOnlyTemplate",
                typeof(DataTemplate),
                true /* expectGet */,
                true /* expectSet */,
                true /* hasCallback */,
                this.dataForm,
                new TemplateFieldDisplayTemplate(),
                new TemplateFieldEditTemplate(),
                new TemplateFieldInsertTemplate());
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

            // 


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
