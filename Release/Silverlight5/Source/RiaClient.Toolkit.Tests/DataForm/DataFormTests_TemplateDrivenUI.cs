//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_TemplateDrivenUI.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
        /// Ensure that element name binding works in an edit template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that element name binding works in an edit template.")]
        public void EnsureElementNameBindingWorks()
        {
            DataFormApp_TemplatesWithElementNameBinding dataFormApp = new DataFormApp_TemplatesWithElementNameBinding();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                ContentPresenter contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                StackPanel stackPanel = contentPresenter.Content as StackPanel;
                Label fieldLabel = stackPanel.Children[0] as Label;
                Assert.AreEqual("String Property", fieldLabel.Content);
                DescriptionViewer descriptionViewer = stackPanel.Children[2] as DescriptionViewer;
                Assert.AreEqual("String Property Description", descriptionViewer.Description);
            });

            this.EnqueueTestComplete();
        }

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
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");

                TextBox textBox = contentPresenter.Content as TextBox;
                textBox.Text = "new string";

                this.ExpectEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.AreEqual("new string", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that changes made to an item in direct content mode can be seen once edit mode ends.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changes made to an item in direct content mode can be seen once edit mode ends.")]
        public void EnsureUIUpdatedAfterEditDirectContent()
        {
            DataFormApp_DirectContent dataFormApp = new DataFormApp_DirectContent();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");

                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectBeginEdit();
                InvokeButton(editButton);
            });

            this.WaitForBeginEdit();

            this.EnqueueCallback(() =>
            {
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");

                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
                Assert.AreEqual(DataFormMode.Edit, dataFormApp.dataForm.Mode);
                textBox.Text = "new string";

                this.ExpectEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
                Assert.AreEqual(DataFormMode.ReadOnly, dataFormApp.dataForm.Mode);
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
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
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
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
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
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that replacing the direct content at runtime works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that replacing the direct content at runtime works properly.")]
        public void ReplaceDirectContent()
        {
            DataFormApp_DirectContent dataFormApp = new DataFormApp_DirectContent();
            dataFormApp.dataForm.CurrentItem = new DataClass();
            this.DataFormAppBase = dataFormApp;
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);

                this.ExpectContentLoaded();

                textBox = new TextBox();
                textBox.SetBinding(TextBox.TextProperty, new Binding("Age") { Mode = BindingMode.TwoWay });
                DataField dataField = new DataField() { Label = "Age", PropertyPath = "Age", Content = textBox };
                StackPanel stackPanel = new StackPanel();
                stackPanel.Children.Add(dataField);
                dataFormApp.dataForm.Content = stackPanel;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, dataFormApp.dataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                StackPanel stackPanel = contentPresenter.Content as StackPanel;
                Assert.IsNotNull(stackPanel);
                Assert.AreEqual(dataFormApp.dataForm.Content, stackPanel);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that replacing the direct content with the edit template at runtime works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that replacing the direct content with the edit template at runtime works properly.")]
        public void ReplaceDirectContentWithEditTemplate()
        {
            DataFormApp_DirectContent dataFormApp = new DataFormApp_DirectContent();
            dataFormApp.dataForm.CurrentItem = new DataClass();
            this.DataFormAppBase = dataFormApp;
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);

                this.ExpectContentLoaded();

                dataFormApp.dataForm.EditTemplate = ((DataFormApp_DirectContent)dataFormApp).editTemplate;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, dataFormApp.dataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);
                Assert.AreNotEqual(dataFormApp.dataForm.Content, textBox);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that replacing the display template at runtime works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that replacing the display template at runtime works properly.")]
        public void ReplaceDisplayTemplate()
        {
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Display Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.ReadOnlyTemplate = this.DataFormApp.displayTemplate2;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Display Template 2", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting the display template to null at runtime works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the display template to null at runtime works properly.")]
        public void ReplaceDisplayTemplateWithEditTemplate()
        {
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Display Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.ReadOnlyTemplate = null;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.ReadOnlyTemplate = this.DataFormApp.displayTemplate2;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Display Template 2", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that replacing the edit template at runtime works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that replacing the edit template at runtime works properly.")]
        public void ReplaceEditTemplate()
        {
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.EditTemplate = this.DataFormApp.editTemplate2;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.Edit, this.DataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template 2", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting the edit template to null at runtime works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the edit template to null at runtime works properly.")]
        public void ReplaceEditTemplateWithInsertTemplate()
        {
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.EditTemplate = null;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.EditTemplate = this.DataFormApp.editTemplate2;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.Edit, this.DataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template 2", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that replacing the insert template at runtime works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that replacing the insert template at runtime works properly.")]
        public void ReplaceInsertTemplate()
        {
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.NewItemTemplate = this.DataFormApp.insertTemplate2;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.AddNew, this.DataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template 2", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting the insert template to null at runtime works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the insert template to null at runtime works properly.")]
        public void ReplaceInsertTemplateWithEditTemplate()
        {
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.NewItemTemplate = null;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.NewItemTemplate = this.DataFormApp.insertTemplate2;
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.AddNew, this.DataForm.Mode);
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Insert Template 2", textBox.Text);
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
            dataFormApp.dataForm.CurrentItem = new DataClass();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.ReadOnlyTemplate = null;
            dataFormApp.dataForm.EditTemplate = null;
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
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
            dataFormApp.dataForm.CurrentItem = new DataClass();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.ReadOnlyTemplate = null;
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
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
            dataFormApp.dataForm.NewItemTemplate = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                TextBox textBox = contentPresenter.Content as TextBox;
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
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
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
            dataFormApp.dataForm.NewItemTemplate = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Edit Template", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that direct content is used when there are no templates set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that direct content is used when there are no templates set.")]
        public void TestFallbackNoTemplates()
        {
            DataFormApp_Templates dataFormApp = new DataFormApp_Templates();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.NewItemTemplate = null;
            dataFormApp.dataForm.EditTemplate = null;
            dataFormApp.dataForm.ReadOnlyTemplate = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            ContentPresenter contentPresenter = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Direct Content", textBox.Text);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
            });

            this.EnqueueTestComplete();
        }
    }
}
