//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_TemplateDrivenUI.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls.Primitives;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests <see cref="DataForm"/> template-driven UI.
    /// </summary>
    [TestClass]
    public class DataFormTests_TemplateDrivenUI : DataFormTests_Base
    {
        #region Helper Properties And Fields

        /// <summary>
        /// Gets the <see cref="DataForm"/> app.
        /// </summary>
        private DataFormApp_Templates DataFormApp
        {
            get
            {
                return this.DataFormAppBase as DataFormApp_Templates;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataForm"/>.
        /// </summary>
        private DataForm DataForm
        {
            get
            {
                return this.DataFormApp.dataForm;
            }
        }

        #endregion Helper Properties

        #region Initialization

        /// <summary>
        /// Initializes the test framework.
        /// </summary>
        /// <summary>
        /// Initializes the test framework.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.DataFormAppBase = new DataFormApp_Templates();
            this.DataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
        }

        #endregion Initialization

        /// <summary>
        /// Ensure that changes made to an item in template mode can be seen once edit mode ends.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changes made to an item in template mode can be seen once edit mode ends.")]
        public void EnsureUIUpdatedAfterEdit()
        {
            DataFormApp_TemplatesWithBinding dataFormApp = new DataFormApp_TemplatesWithBinding();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");

                TextBox textBox = fieldsPresenter.Content as TextBox;
                textBox.Text = "new string";

                this.ExpectItemEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForItemEditEnded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.AreEqual("new string", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the display template is used properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the display template is used properly.")]
        public void GetDisplayTemplate()
        {
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Display Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the edit template is used properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the edit template is used properly.")]
        public void GetEditTemplate()
        {
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the insert template is used properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the insert template is used properly.")]
        public void GetInsertTemplate()
        {
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that template fallback works properly with no display or edit template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that template fallback works properly with no display or edit template.")]
        public void TestFallbackNoDisplayOrEditTemplate()
        {
            DataFormApp_Templates dataFormApp = new DataFormApp_Templates();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.DisplayTemplate = null;
            dataFormApp.dataForm.EditTemplate = null;
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that template fallback works properly with no display template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that template fallback works properly with no display template.")]
        public void TestFallbackNoDisplayTemplate()
        {
            DataFormApp_Templates dataFormApp = new DataFormApp_Templates();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.DisplayTemplate = null;
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the DataForm is coerced to read-only mode if there is a display template but no edit or insert templates.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the DataForm is coerced to read-only mode if there is a display template but no edit or insert templates.")]
        public void TestFallbackNoEditOrInsertTemplate()
        {
            DataFormApp_Templates dataFormApp = new DataFormApp_Templates();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.EditTemplate = null;
            dataFormApp.dataForm.InsertTemplate = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Display Template", textBox.Text);
                Assert.IsTrue(dataFormApp.dataForm.EffectiveIsReadOnly);
                Assert.IsFalse(editButton.IsEnabled);
                Assert.IsFalse(newItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that template fallback works properly with no edit template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that template fallback works properly with no edit template.")]
        public void TestFallbackNoEditTemplate()
        {
            DataFormApp_Templates dataFormApp = new DataFormApp_Templates();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.EditTemplate = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that template fallback works properly with no insert template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that template fallback works properly with no insert template.")]
        public void TestFallbackNoInsertTemplate()
        {
            DataFormApp_Templates dataFormApp = new DataFormApp_Templates();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.InsertTemplate = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            ContentPresenter fieldsPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                fieldsPresenter = this.GetTemplatePart<ContentPresenter>("FieldsPresenter");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = fieldsPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }
    }
}
