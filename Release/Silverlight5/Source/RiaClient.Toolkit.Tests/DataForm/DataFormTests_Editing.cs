//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Editing.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests <see cref="DataForm"/> editing.
    /// </summary>
    [TestClass]
    public class DataFormTests_Editing : DataFormTests_Base
    {
        #region Helper Properties And Fields

        /// <summary>
        /// The data class being used.
        /// </summary>
        private DataClass dataClass;

        /// <summary>
        /// The check box from the associated field.
        /// </summary>
        private CheckBox checkBox;

        /// <summary>
        /// The date picker from the associated field.
        /// </summary>
        private DatePicker datePicker;

        /// <summary>
        /// The text box from the associated field.
        /// </summary>
        private TextBox textBox;

        /// <summary>
        /// The text block from the associated field.
        /// </summary>
        private TextBlock textBlock;

        /// <summary>
        /// The combo box from the associated field.
        /// </summary>
        private ComboBox comboBox;

        /// <summary>
        /// The first inner text box from the associated field.
        /// </summary>
        private TextBox innerTextBox1;

        /// <summary>
        /// The second inner text box from the associated field.
        /// </summary>
        private TextBox innerTextBox2;

        /// <summary>
        /// Gets the <see cref="DataForm"/> app.
        /// </summary>
        private DataFormApp_Fields DataFormApp
        {
            get
            {
                return this.DataFormAppBase as DataFormApp_Fields;
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
        public override void Initialize()
        {
            base.Initialize();
            this.DataFormAppBase = new DataFormApp_Fields();
            this.dataClass = DataClassList.GetDataClassList(1, ListOperations.All)[0];
            this.DataForm.CurrentItem = this.dataClass;
        }

        #endregion Initialization

        /// <summary>
        /// Ensure that BeginEdit functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that BeginEdit functions properly.")]
        public void BeginEdit()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsFalse(this.checkBox.IsEnabled);
                Assert.IsFalse(this.datePicker.IsEnabled);
                Assert.IsTrue(this.textBox.IsReadOnly);
                Assert.AreEqual("Display template", this.textBlock.Text);
                Assert.IsFalse(this.comboBox.IsEnabled);
                Assert.IsTrue(this.innerTextBox1.IsReadOnly);
                Assert.IsTrue(this.innerTextBox2.IsReadOnly);

                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(this.DataForm.CanBeginEdit);
                Assert.IsFalse(this.DataForm.CanCommitEdit);
                Assert.IsFalse(this.DataForm.CanCancelEdit);

                this.ExpectContentLoaded();
                Assert.IsTrue(this.DataForm.BeginEdit());
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsTrue(this.checkBox.IsEnabled);
                Assert.IsTrue(this.datePicker.IsEnabled);
                Assert.IsFalse(this.textBox.IsReadOnly);
                Assert.AreEqual("Edit template", this.textBlock.Text);
                Assert.IsTrue(this.comboBox.IsEnabled);
                Assert.IsFalse(this.innerTextBox1.IsReadOnly);
                Assert.IsFalse(this.innerTextBox2.IsReadOnly);

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.CanBeginEdit);
                Assert.IsTrue(this.DataForm.CanCommitEdit);
                Assert.IsTrue(this.DataForm.CanCancelEdit);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that BeginningEdit can be canceled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that BeginningEdit can be canceled.")]
        public void CancelBeginningEdit()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginningEdit += new EventHandler<System.ComponentModel.CancelEventArgs>(CancelBeginningEdit);
                Assert.IsFalse(this.DataForm.BeginEdit());
            });

            // We shouldn't get an event, so we need to wait a little while to ensure that no event occurs.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                // The beginning of the edit should have been cancelled by the event handler.
                Assert.IsFalse(this.DataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CancelEdit functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CancelEdit functions properly.")]
        public void CancelEdit()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsTrue(this.checkBox.IsEnabled);
                Assert.IsTrue(this.datePicker.IsEnabled);
                Assert.IsFalse(this.textBox.IsReadOnly);
                Assert.AreEqual("Edit template", this.textBlock.Text);
                Assert.IsTrue(this.comboBox.IsEnabled);
                Assert.IsFalse(this.innerTextBox1.IsReadOnly);
                Assert.IsFalse(this.innerTextBox2.IsReadOnly);
                Assert.IsTrue(this.DataForm.IsEditing);

                this.ExpectEditEnded();
                Assert.IsTrue(this.DataForm.CancelEdit());
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsFalse(this.checkBox.IsEnabled);
                Assert.IsFalse(this.datePicker.IsEnabled);
                Assert.IsTrue(this.textBox.IsReadOnly);
                Assert.AreEqual("Display template", this.textBlock.Text);
                Assert.IsFalse(this.comboBox.IsEnabled);
                Assert.IsTrue(this.innerTextBox1.IsReadOnly);
                Assert.IsTrue(this.innerTextBox2.IsReadOnly);
                Assert.IsFalse(this.DataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CancelEdit functions properly with changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CancelEdit functions properly with changes.")]
        public void CancelEditWithChanges()
        {
            DateTime newDateTime = new DateTime(2001, 2, 3);

            bool? oldIsChecked = null;
            DateTime? oldSelectedDate = null;
            string oldText = null;
            object oldSelectedItem = null;

            bool? oldBool = false;
            DateTime oldDateTime = new DateTime();
            string oldString = null;
            int oldInt = 0;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                oldIsChecked = this.checkBox.IsChecked;
                oldSelectedDate = this.datePicker.SelectedDate;
                oldText = this.textBox.Text;
                oldSelectedItem = this.comboBox.SelectedItem;

                oldBool = this.dataClass.BoolProperty;
                oldDateTime = this.dataClass.DateTimeProperty;
                oldString = this.dataClass.StringProperty;
                oldInt = this.dataClass.IntProperty;

                Assert.IsFalse(this.DataForm.IsItemChanged);

                this.checkBox.IsChecked = true;
                this.datePicker.SelectedDate = newDateTime;
                this.textBox.Text = "text";
                this.comboBox.SelectedItem = 5;

                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemChanged);
                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);

                this.ExpectEditEnded();
                this.DataForm.CancelEdit();
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsFalse(this.DataForm.IsItemChanged);

                Assert.AreEqual(oldIsChecked, this.checkBox.IsChecked);
                Assert.AreEqual(oldSelectedDate, this.datePicker.SelectedDate);
                Assert.AreEqual(oldText, this.textBox.Text);
                Assert.AreEqual(oldSelectedItem, this.comboBox.SelectedItem);

                Assert.AreEqual(oldBool, this.dataClass.BoolProperty);
                Assert.AreEqual(oldDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual(oldString, this.dataClass.StringProperty);
                Assert.AreEqual(oldInt, this.dataClass.IntProperty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CancelEdit functions properly with changes when there is a validation error.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CancelEdit functions properly with changes when there is a validation error.")]
        public void CancelEditWithChangesAndValidationError()
        {
            this.DataForm.CurrentItem = new DataClassWithValidation()
            {
                StringProperty = "test string 1"
            };

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                this.textBox.Text = string.Empty;

                this.CommitAllFields();

                Assert.AreEqual(string.Empty, this.textBox.Text);

                this.ExpectEditEnded();
                this.DataForm.CancelEdit();
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.AreEqual("test string 1", this.textBox.Text);

                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.AreEqual("test string 1", this.textBox.Text);
                Assert.IsFalse(Validation.GetHasError(this.textBox));

                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CancelEdit functions properly with changes when there is a validation error and AutoEdit = true.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CancelEdit functions properly with changes when there is a validation error and AutoEdit = true.")]
        public void CancelEditWithChangesAndValidationErrorAutoEdit()
        {
            this.DataForm.CurrentItem = new DataClassWithValidation()
            {
                StringProperty = "test string 1"
            };
            this.DataForm.AutoEdit = true;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginEdit();
                this.GetInputControls();
                this.textBox.Text = string.Empty;

                this.CommitAllFields();

                Assert.AreEqual(string.Empty, this.textBox.Text);

                this.ExpectEditEnded();
                this.DataForm.CancelEdit();
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.AreEqual("test string 1", this.textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that ItemEditEnding can be canceled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that ItemEditEnding can be canceled.")]
        public void CancelItemEditEnding()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.DataForm.EditEnding += new EventHandler<DataFormEditEndingEventArgs>(CancelItemEditEnding);
                Assert.IsFalse(this.DataForm.CommitEdit(true /* exitEditingMode */));
            });

            // We shouldn't get an event, so we need to wait a little while to ensure that no event occurs.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.CancelEdit());
            });

            // We shouldn't get an event here either.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                // The ending of the edit should have been cancelled by the event handler.
                Assert.IsTrue(this.DataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitEdit functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitEdit functions properly.")]
        public void CommitEdit()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsTrue(this.checkBox.IsEnabled);
                Assert.IsTrue(this.datePicker.IsEnabled);
                Assert.IsFalse(this.textBox.IsReadOnly);
                Assert.AreEqual("Edit template", this.textBlock.Text);
                Assert.IsTrue(this.comboBox.IsEnabled);
                Assert.IsFalse(this.innerTextBox1.IsReadOnly);
                Assert.IsFalse(this.innerTextBox2.IsReadOnly);
                Assert.IsTrue(this.DataForm.IsEditing);

                this.ExpectEditEnded();
                Assert.IsTrue(this.DataForm.CommitEdit(true /* exitEditingMode */));
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsFalse(this.checkBox.IsEnabled);
                Assert.IsFalse(this.datePicker.IsEnabled);
                Assert.IsTrue(this.textBox.IsReadOnly);
                Assert.AreEqual("Display template", this.textBlock.Text);
                Assert.IsFalse(this.comboBox.IsEnabled);
                Assert.IsTrue(this.innerTextBox1.IsReadOnly);
                Assert.IsTrue(this.innerTextBox2.IsReadOnly);
                Assert.IsFalse(this.DataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitEdit does not cause an exception to be thrown when AutoCommit is false and data shaping exists.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitEdit does not cause an exception to be thrown when AutoCommit is false and data shaping exists.")]
        public void CommitEditWithAutoCommitFalseAndDataShaping()
        {
            this.DataForm.AutoCommit = false;
            this.DataForm.CurrentItem = null;

            PagedCollectionView pcv = new PagedCollectionView(DataClassList.GetDataClassList(3, ListOperations.All));
            pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("StringProperty", System.ComponentModel.ListSortDirection.Ascending));

            this.DataForm.ItemsSource = pcv;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                this.textBox.Text = "new string";

                this.ExpectContentLoaded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);
            });

            this.WaitForContentLoaded();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitEdit functions properly with changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitEdit functions properly with changes.")]
        public void CommitEditWithChanges()
        {
            DateTime newDateTime = new DateTime(2001, 2, 3);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsFalse(this.DataForm.IsItemChanged);

                this.checkBox.IsChecked = true;
                this.datePicker.SelectedDate = newDateTime;
                this.textBox.Text = "text";
                this.comboBox.SelectedItem = 5;

                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemChanged);
                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);

                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsTrue(this.DataForm.IsItemChanged);

                Assert.AreEqual(true, this.checkBox.IsChecked);
                Assert.AreEqual(newDateTime, this.datePicker.SelectedDate);
                Assert.AreEqual("text", this.textBox.Text);
                Assert.AreEqual(5, this.comboBox.SelectedItem);

                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitEdit without exiting edit mode functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitEdit without exiting edit mode functions properly.")]
        public void CommitEditWithoutExitingEditMode()
        {
            DateTime newDateTime = new DateTime(2001, 2, 3);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsFalse(this.DataForm.IsItemChanged);

                this.checkBox.IsChecked = true;
                this.datePicker.SelectedDate = newDateTime;
                this.textBox.Text = "text";
                this.comboBox.SelectedItem = 5;

                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemChanged);
                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);

                this.ExpectEditEnded();
                this.DataForm.CommitEdit(false /* exitEditingMode */);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsTrue(this.DataForm.IsItemChanged);

                Assert.AreEqual(true, this.checkBox.IsChecked);
                Assert.AreEqual(newDateTime, this.datePicker.SelectedDate);
                Assert.AreEqual("text", this.textBox.Text);
                Assert.AreEqual(5, this.comboBox.SelectedItem);

                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);

                this.ExpectEditEnded();
                this.DataForm.CancelEdit();
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsTrue(this.DataForm.IsItemChanged);

                Assert.AreEqual(true, this.checkBox.IsChecked);
                Assert.AreEqual(newDateTime, this.datePicker.SelectedDate);
                Assert.AreEqual("text", this.textBox.Text);
                Assert.AreEqual(5, this.comboBox.SelectedItem);

                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitEdit does not throw an exception when committing a template field with a read-only text box.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitEdit does not throw an exception when committing a template field with a read-only text box.")]
        public void CommitEditWithReadOnlyTemplateField()
        {
            DataFormApp_ReadOnlyProperty2 dataFormApp = new DataFormApp_ReadOnlyProperty2();
            dataFormApp.dataForm.CurrentItem = new DataClassWithReadOnlyProperty();
            this.DataFormAppBase = dataFormApp;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectEditEnded();
                Assert.IsTrue(dataFormApp.dataForm.CommitEdit(true /* exitEditingMode */));
            });

            this.WaitForEditEnded();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitEdit does not throw an exception when committing an item with a read-only text box in the template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitEdit does not throw an exception when committing an item with a read-only text box in the template.")]
        public void CommitEditWithReadOnlyTemplateItem()
        {
            DataFormApp_ReadOnlyProperty3 dataFormApp = new DataFormApp_ReadOnlyProperty3();
            dataFormApp.dataForm.CurrentItem = new DataClassWithReadOnlyProperty();
            this.DataFormAppBase = dataFormApp;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectEditEnded();
                Assert.IsTrue(dataFormApp.dataForm.CommitEdit(true /* exitEditingMode */));
            });

            this.WaitForEditEnded();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitEdit does not throw an exception when committing a read-only text field.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitEdit does not throw an exception when committing a read-only text field.")]
        public void CommitEditWithReadOnlyTextField()
        {
            DataFormApp_ReadOnlyProperty1 dataFormApp = new DataFormApp_ReadOnlyProperty1();
            dataFormApp.dataForm.CurrentItem = new DataClassWithReadOnlyProperty();
            this.DataFormAppBase = dataFormApp;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectEditEnded();
                Assert.IsTrue(dataFormApp.dataForm.CommitEdit(true /* exitEditingMode */));
            });

            this.WaitForEditEnded();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that, if an edit has been begun in a PagedCollectionView when DataForm.BeginEdit() is called,
        /// PCV.EditItem() is not called again.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that, if an edit has been begun in a PagedCollectionView when DataForm.BeginEdit() is called, PCV.EditItem() is not called again.")]
        public void EditInPagedCollectionViewBeforeEditingInDataForm()
        {
            PagedCollectionView pcv = new PagedCollectionView(DataClassList.GetDataClassList(3, ListOperations.All));
            pcv.EditItem(pcv.CurrentItem);
            DataClass dataClass = pcv.CurrentItem as DataClass;
            dataClass.StringProperty = "new string 1";

            this.DataForm.CurrentItem = null;
            this.DataForm.ItemsSource = pcv;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                this.textBox.Text = "new string 2";
                this.CommitAllFields();
                this.ExpectEditEnded();
                this.DataForm.CancelEdit();
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.AreEqual("test string 0", this.textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that AutoEdit functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that AutoEdit functions properly.")]
        public void EnsureAutoEditFunctionsProperly()
        {
            this.DataForm.AutoEdit = true;
            this.DataForm.CurrentItem = null;
            this.DataForm.ItemsSource = DataClassList.GetDataClassList(2, ListOperations.All);
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginEdit();
                this.GetInputControls();
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                Assert.IsTrue(this.DataForm.IsEditing);
                this.textBox.Text = "text 2";
                this.ExpectEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginEdit();
                this.GetInputControls();
                ButtonBase nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.AreEqual("text 2", this.textBox.Text);
                this.ExpectCurrentItemChange();
                InvokeButton(nextItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginEdit();
                this.GetInputControls();
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                Assert.IsTrue(this.DataForm.IsEditing);
                this.textBox.Text = "text 2";
                this.ExpectEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginEdit();
                this.GetInputControls();
                ButtonBase cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");
                Assert.IsTrue(this.DataForm.IsEditing);
                this.textBox.Text = "text 3";
                this.ExpectEditEnded();
                InvokeButton(cancelButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginEdit();
                this.GetInputControls();
                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.AreEqual("text 2", this.textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that calling begin edit then end edit does not raise an exception.
        /// </summary>
        [TestMethod]
        [Description("Ensure that calling begin edit then end edit does not raise an exception.")]
        public void EnsureBeginEditThenEndEditDoesNotRaiseException()
        {
            DataForm df = new DataForm();
            df.CurrentItem = new DataClass();
            df.BeginEdit();

            bool success = false;

            try
            {
                df.CommitEdit(true /* exitEditingMode */);
                success = true;
            }
            catch (NullReferenceException)
            {
            }

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Ensure that having AutoEdit set to true does not prevent addition and deletion.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that having AutoEdit set to true does not prevent addition and deletion.")]
        public void EnsureCanAddAndDeleteWithAutoEdit()
        {
            this.DataForm.AutoEdit = true;
            this.DataForm.ItemsSource = DataClassList.GetDataClassList(2, ListOperations.All);
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginEdit();
                Assert.AreEqual(DataFormMode.Edit, this.DataForm.Mode);
                this.ExpectContentLoaded();
                this.DataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.AddNew, this.DataForm.Mode);
                this.DataForm.CancelEdit();
                Assert.AreEqual(DataFormMode.Edit, this.DataForm.Mode);
                this.ExpectCurrentItemChange();
                this.DataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(1, this.DataForm.ItemsCount);
                Assert.AreEqual(DataFormMode.Edit, this.DataForm.Mode);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the user is able to cancel an edit with AutoEdit initialized to true and when the current item does not change in the ItemsSource change handler.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the user is able to cancel an edit with AutoEdit initialized to true and when the current item does not change in the ItemsSource change handler.")]
        public void EnsureCanCancelEditWithInitialAutoEdit()
        {
            DataFormApp_FieldsWithInitialAutoEdit dataFormApp = new DataFormApp_FieldsWithInitialAutoEdit();
            this.DataFormAppBase = dataFormApp;
            DataClassList dataClassList = DataClassList.GetDataClassList(5, ListOperations.All);
            dataFormApp.dataForm.CurrentItem = dataClassList[0];
            dataFormApp.dataForm.ItemsSource = dataClassList;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.BeginEdit();
                this.GetInputControls();
                this.textBox.Text = "new string";
                Assert.IsTrue(dataFormApp.dataForm.CanCancelEdit);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CancelEdit does not throw an exception when not in editing mode.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CanceEdit does not throw an exception when not in editing mode.")]
        public void EnsureCancelEditDoesNotThrowExceptionWhenNotEditing()
        {
            DataClassList dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.ItemsSource = dataClassList;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.CancelEdit();
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that editing is possible without an object that implements IEditableObject.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that editing is possible without an object that implements IEditableObject.")]
        public void EnsureCanEditWithoutIEditableObject()
        {
            DateTime newDateTime = new DateTime(2001, 2, 3);

            ButtonBase editButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;

            this.DataForm.CurrentItem = new BasicDataClass();
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(this.DataForm.CanBeginEdit);
                Assert.IsFalse(this.DataForm.CanCommitEdit);
                Assert.IsFalse(this.DataForm.CanCancelEdit);
                Assert.IsTrue(editButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.CanBeginEdit);
                Assert.IsTrue(this.DataForm.CanCommitEdit);
                Assert.IsFalse(this.DataForm.CanCancelEdit);
                Assert.IsFalse(editButton.IsEnabled);
                Assert.IsTrue(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);

                this.checkBox.IsChecked = true;
                this.datePicker.SelectedDate = newDateTime;
                this.textBox.Text = "text";
                this.comboBox.SelectedItem = 5;

                Assert.AreEqual(true, this.checkBox.IsChecked);
                Assert.AreEqual(newDateTime, this.datePicker.SelectedDate);
                Assert.AreEqual("text", this.textBox.Text);
                Assert.AreEqual(5, this.comboBox.SelectedItem);

                this.ExpectEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(this.DataForm.CanBeginEdit);
                Assert.IsFalse(this.DataForm.CanCommitEdit);
                Assert.IsFalse(this.DataForm.CanCancelEdit);
                Assert.IsTrue(editButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);

                Assert.AreEqual(true, this.checkBox.IsChecked);
                Assert.AreEqual(newDateTime, this.datePicker.SelectedDate);
                Assert.AreEqual("text", this.textBox.Text);
                Assert.AreEqual(5, this.comboBox.SelectedItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the user can move off of the current item when editing with AutoCommit set to true.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the user can move off of the current item when editing with AutoCommit set to true.")]
        public void EnsureCanMoveWithAutoCommit()
        {
            DataClassList dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.ItemsSource = dataClassList;
            this.DataForm.CurrentItem = dataClassList[1];
            this.DataForm.AutoCommit = true;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                ButtonBase firstItemButton = this.GetTemplatePart<ButtonBase>("FirstItemButton");
                ButtonBase lastItemButton = this.GetTemplatePart<ButtonBase>("LastItemButton");
                ButtonBase nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                ButtonBase previousItemButton = this.GetTemplatePart<ButtonBase>("PreviousItemButton");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                ButtonBase deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");

                Assert.IsTrue(firstItemButton.IsEnabled);
                Assert.IsTrue(lastItemButton.IsEnabled);
                Assert.IsTrue(nextItemButton.IsEnabled);
                Assert.IsTrue(previousItemButton.IsEnabled);
                Assert.IsTrue(newItemButton.IsEnabled);
                Assert.IsTrue(deleteItemButton.IsEnabled);
                Assert.IsTrue(this.DataForm.CanMoveToFirstItem);
                Assert.IsTrue(this.DataForm.CanMoveToLastItem);
                Assert.IsTrue(this.DataForm.CanMoveToNextItem);
                Assert.IsTrue(this.DataForm.CanMoveToPreviousItem);
                Assert.IsTrue(this.DataForm.CanAddItems);
                Assert.IsTrue(this.DataForm.CanDeleteItems);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the commit button is still disabled in AutoEdit mode with no changes even when IEditableObject is not implemented.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the commit button is still disabled in AutoEdit mode with no changes even when IEditableObject is not implemented.")]
        public void EnsureCannotCommitWithAutoEditAndNoChanges()
        {
            this.DataForm.CurrentItem = new BasicDataClass();
            this.DataForm.AutoEdit = true;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.BeginEdit();
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                Assert.IsFalse(commitButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that no editing is possible with no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that no editing is possible with no items.")]
        public void EnsureCannotEditWithNoItems()
        {
            this.DataForm.CurrentItem = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanBeginEdit);

                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                Assert.IsFalse(editButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that no editing is possible when IsReadOnly is true.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that no editing is possible when IsReadOnly is true.")]
        public void EnsureCannotEditWithReadOnly()
        {
            this.DataForm.IsReadOnly = true;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanBeginEdit);

                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                Assert.IsFalse(editButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the user cannot move off of the current item when editing with AutoCommit set to false and with edits to the current item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the user cannot move off of the current item when editing with AutoCommit set to false and with edits to the current item.")]
        public void EnsureCannotMoveWithNoAutoCommit()
        {
            DataClassList dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.ItemsSource = dataClassList;
            this.DataForm.CurrentItem = dataClassList[1];
            this.DataForm.AutoCommit = false;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                ButtonBase firstItemButton = this.GetTemplatePart<ButtonBase>("FirstItemButton");
                ButtonBase lastItemButton = this.GetTemplatePart<ButtonBase>("LastItemButton");
                ButtonBase nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                ButtonBase previousItemButton = this.GetTemplatePart<ButtonBase>("PreviousItemButton");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                ButtonBase deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");

                Assert.IsTrue(firstItemButton.IsEnabled);
                Assert.IsTrue(lastItemButton.IsEnabled);
                Assert.IsTrue(nextItemButton.IsEnabled);
                Assert.IsTrue(previousItemButton.IsEnabled);
                Assert.IsTrue(newItemButton.IsEnabled);
                Assert.IsTrue(deleteItemButton.IsEnabled);
                Assert.IsTrue(this.DataForm.CanMoveToFirstItem);
                Assert.IsTrue(this.DataForm.CanMoveToLastItem);
                Assert.IsTrue(this.DataForm.CanMoveToNextItem);
                Assert.IsTrue(this.DataForm.CanMoveToPreviousItem);
                Assert.IsTrue(this.DataForm.CanAddItems);
                Assert.IsTrue(this.DataForm.CanDeleteItems);

                this.GetInputControls();
                this.textBox.Text = "new string";
                
                // Don't call CommitAllFields(), because that directly calls SetCanXXXX(),
                // which we don't want - it should be called when a property changes.
                BindingExpression be = this.textBox.GetBindingExpression(TextBox.TextProperty);
                Assert.IsNotNull(be);
                be.UpdateSource();

                Assert.IsFalse(firstItemButton.IsEnabled);
                Assert.IsFalse(lastItemButton.IsEnabled);
                Assert.IsFalse(nextItemButton.IsEnabled);
                Assert.IsFalse(previousItemButton.IsEnabled);
                Assert.IsFalse(newItemButton.IsEnabled);
                Assert.IsFalse(deleteItemButton.IsEnabled);
                Assert.IsFalse(this.DataForm.CanMoveToFirstItem);
                Assert.IsFalse(this.DataForm.CanMoveToLastItem);
                Assert.IsFalse(this.DataForm.CanMoveToNextItem);
                Assert.IsFalse(this.DataForm.CanMoveToPreviousItem);
                Assert.IsFalse(this.DataForm.CanAddItems);
                Assert.IsFalse(this.DataForm.CanDeleteItems);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that IsReadOnly can be set to true in edit mode and that it ends the edit
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that IsReadOnly can be set to true in edit mode and that it ends the edit.")]
        public void EnsureCanSetIsReadOnlyWhenEditing()
        {
            DataClassList dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.ItemsSource = dataClassList;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectEditEnded();
                this.DataForm.IsReadOnly = true;
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitItemEdit returns false when not in editing mode.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitItemEdit returns false when not in editing mode.")]
        public void EnsureCommitItemEditReturnsFalseWhenNotEditing()
        {
            DataClassList dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.ItemsSource = dataClassList;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CommitEdit(true /* exitEditingMode */));
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure, when considering whether or not there are local changes through TextChanged, that type conversion works.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure, when considering whether or not there are local changes through TextChanged, that type conversion works.")]
        public void EnsureConversionWorksInDeterminingEditedState()
        {
            DataFormApp_FieldsWithDifferentBinding dataFormApp = new DataFormApp_FieldsWithDifferentBinding();
            dataFormApp.dataForm.CurrentItem = new DataClass();
            ButtonBase commitButton = null;

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsFalse(commitButton.IsEnabled);
                this.textBox.Text = "True";
                Assert.IsTrue(commitButton.IsEnabled);
                this.textBox.Text = "False";
                Assert.IsFalse(commitButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that a DataField bound to an uneditable type is coerced to read-only when IsReadOnly has not been explicitly set and when auto-generating fields.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that a DataField bound to an uneditable type is coerced to read-only when IsReadOnly has not been explicitly set and when auto-generating fields.")]
        public void EnsureUneditableTypeCoercesFieldToReadOnlyWhenAutogenerating()
        {
            DataFormApp_AutoGeneration dataFormApp = new DataFormApp_AutoGeneration();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.CurrentItem = new DataClassWithUneditableType();

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = this.DataFormInputControls[5] as TextBox;
                Assert.IsTrue(textBox.IsReadOnly);
                this.ExpectContentLoaded();
                Assert.IsTrue(dataFormApp.dataForm.BeginEdit());
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = this.DataFormInputControls[5] as TextBox;
                Assert.IsTrue(textBox.IsReadOnly);

                DataField dataField = dataFormApp.dataForm.Fields[5];
                dataField.IsReadOnly = false;
            });

            // Wait a short while to let the DataField re-generate its UI.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                TextBox textBox = this.DataFormInputControls[5] as TextBox;
                Assert.IsFalse(textBox.IsReadOnly);

                DataField dataField = dataFormApp.dataForm.Fields[5];
                dataField.IsReadOnly = true;
            });

            // Wait a short while to let the DataField re-generate its UI.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                TextBox textBox = this.DataFormInputControls[5] as TextBox;
                Assert.IsTrue(textBox.IsReadOnly);

                this.ExpectContentLoaded();
                dataFormApp.dataForm.CommitEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(dataFormApp.dataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that a DataField bound to an uneditable type is not coerced to read-only when not auto-generating fields.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that a DataField bound to an uneditable type is not coerced to read-only when not auto-generating fields.")]
        public void EnsureUneditableTypeDoesNotCoerceFieldToReadOnlyWhenNotAutogenerating()
        {
            DataFormApp_FieldsWithUneditableType dataFormApp = new DataFormApp_FieldsWithUneditableType();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.CurrentItem = new DataClassWithUneditableType();

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = this.DataFormInputControls[0] as TextBox;
                Assert.IsTrue(textBox.IsReadOnly);
                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = this.DataFormInputControls[0] as TextBox;
                Assert.IsFalse(textBox.IsReadOnly);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that having unsubmitted changes do not cause the header to be visible when it wasn't before.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that having unsubmitted changes do not cause the header to be visible when it wasn't before.")]
        public void EnsureUnsubmittedChangesDoNotShowHeader()
        {
            this.DataForm.Header = null;
            this.DataForm.CommandButtonsVisibility = DataFormCommandButtonsVisibility.None;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(Visibility.Collapsed, this.DataForm.HeaderVisibility);

                this.GetInputControls();
                this.textBox.Text = "new string";
                this.CommitAllFields();

                Assert.AreEqual(Visibility.Collapsed, this.DataForm.HeaderVisibility);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that having a validation error does not prevent the user from attempting to commit changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that having a validation error does not prevent the user from attempting to commit changes.")]
        public void EnsureValidationErrorDoesNotPreventCommit()
        {
            this.DataForm.ItemsSource = new List<DataClassWithValidation>()
            {
                new DataClassWithValidation()
                {
                    IntProperty = 1,
                    StringProperty = "test string 1"
                },

                new DataClassWithValidation()
                {
                    IntProperty = 2,
                    StringProperty = "test string 2"
                },

                new DataClassWithValidation()
                {
                    IntProperty = 3,
                    StringProperty = "test string 3"
                }
            };
            this.DataForm.CurrentIndex = 1;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                ButtonBase firstItemButton = this.GetTemplatePart<ButtonBase>("FirstItemButton");
                ButtonBase previousItemButton = this.GetTemplatePart<ButtonBase>("PreviousItemButton");
                ButtonBase nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                ButtonBase lastItemButton = this.GetTemplatePart<ButtonBase>("LastItemButton");
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                ValidationSummary validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");

                Assert.IsTrue(firstItemButton.IsEnabled);
                Assert.IsTrue(previousItemButton.IsEnabled);
                Assert.IsTrue(nextItemButton.IsEnabled);
                Assert.IsTrue(lastItemButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);

                this.GetInputControls();
                this.textBox.Text = "2";
                this.DataForm.ValidateItem();

                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.IsTrue(firstItemButton.IsEnabled);
                Assert.IsTrue(previousItemButton.IsEnabled);
                Assert.IsTrue(nextItemButton.IsEnabled);
                Assert.IsTrue(lastItemButton.IsEnabled);
                Assert.IsTrue(commitButton.IsEnabled);

                this.textBox.Text = "test string 2";
                this.DataForm.ValidateItem();

                Assert.AreEqual(0, validationSummary.Errors.Count);
                Assert.IsTrue(firstItemButton.IsEnabled);
                Assert.IsTrue(previousItemButton.IsEnabled);
                Assert.IsTrue(nextItemButton.IsEnabled);
                Assert.IsTrue(lastItemButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that moving off of an item being edited with AutoCommit set to true properly commits the edit.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that moving off of an item being edited with AutoCommit set to true properly commits the edit.")]
        public void MoveWithAutoCommit()
        {
            DateTime newDateTime = new DateTime(2001, 2, 3);

            DataClassList dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.ItemsSource = dataClassList;
            this.DataForm.CurrentItem = dataClassList[0];
            this.DataForm.AutoCommit = true;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                this.checkBox.IsChecked = true;
                this.datePicker.SelectedDate = newDateTime;
                this.textBox.Text = "text";
                this.comboBox.SelectedItem = 5;

                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.AreEqual(true, dataClassList[0].BoolProperty);
                Assert.AreEqual(newDateTime, dataClassList[0].DateTimeProperty);
                Assert.AreEqual("text", dataClassList[0].StringProperty);
                Assert.AreEqual(5, dataClassList[0].IntProperty);

                this.ExpectCurrentItemChange();
                this.DataForm.MoveToNextItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.AreEqual(true, dataClassList[0].BoolProperty);
                Assert.AreEqual(newDateTime, dataClassList[0].DateTimeProperty);
                Assert.AreEqual("text", dataClassList[0].StringProperty);
                Assert.AreEqual(5, dataClassList[0].IntProperty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the cancel button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the cancel button functions properly.")]
        public void UseCancelButton()
        {
            ButtonBase editButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsTrue(this.checkBox.IsEnabled);
                Assert.IsTrue(this.datePicker.IsEnabled);
                Assert.IsFalse(this.textBox.IsReadOnly);
                Assert.AreEqual("Edit template", this.textBlock.Text);
                Assert.IsTrue(this.comboBox.IsEnabled);
                Assert.IsFalse(this.innerTextBox1.IsReadOnly);
                Assert.IsFalse(this.innerTextBox2.IsReadOnly);

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(editButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsTrue(cancelButton.IsEnabled);

                this.ExpectEditEnded();
                InvokeButton(cancelButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsFalse(this.checkBox.IsEnabled);
                Assert.IsFalse(this.datePicker.IsEnabled);
                Assert.IsTrue(this.textBox.IsReadOnly);
                Assert.AreEqual("Display template", this.textBlock.Text);
                Assert.IsFalse(this.comboBox.IsEnabled);
                Assert.IsTrue(this.innerTextBox1.IsReadOnly);
                Assert.IsTrue(this.innerTextBox2.IsReadOnly);

                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(editButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the cancel button functions properly with changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the cancel button functions properly with changes.")]
        public void UseCancelButtonWithChanges()
        {
            ButtonBase editButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;
            DateTime newDateTime = new DateTime(2001, 2, 3);

            bool? oldIsChecked = null;
            DateTime? oldSelectedDate = null;
            string oldText = null;
            object oldSelectedItem = 0;

            bool? oldBool = false;
            DateTime oldDateTime = new DateTime();
            string oldString = null;
            int oldInt = 0;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                oldIsChecked = this.checkBox.IsChecked;
                oldSelectedDate = this.datePicker.SelectedDate;
                oldText = this.textBox.Text;
                oldSelectedItem = this.comboBox.SelectedItem;

                oldBool = this.dataClass.BoolProperty;
                oldDateTime = this.dataClass.DateTimeProperty;
                oldString = this.dataClass.StringProperty;
                oldInt = this.dataClass.IntProperty;

                Assert.IsFalse(this.DataForm.IsItemChanged);

                this.checkBox.IsChecked = true;
                this.datePicker.SelectedDate = newDateTime;
                this.comboBox.SelectedItem = 5;
                this.textBox.Text = "text";

                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemChanged);
                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);

                this.ExpectEditEnded();
                InvokeButton(cancelButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsFalse(this.DataForm.IsItemChanged);

                Assert.AreEqual(oldIsChecked, this.checkBox.IsChecked);
                Assert.AreEqual(oldSelectedDate, this.datePicker.SelectedDate);
                Assert.AreEqual(oldText, this.textBox.Text);
                Assert.AreEqual(oldSelectedItem, this.comboBox.SelectedItem);

                Assert.AreEqual(oldBool, this.dataClass.BoolProperty);
                Assert.AreEqual(oldDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual(oldString, this.dataClass.StringProperty);
                Assert.AreEqual(oldInt, this.dataClass.IntProperty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the edit button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the edit button functions properly.")]
        public void UseEditButton()
        {
            ButtonBase editButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                this.GetInputControls();

                Assert.IsFalse(this.checkBox.IsEnabled);
                Assert.IsFalse(this.datePicker.IsEnabled);
                Assert.IsTrue(this.textBox.IsReadOnly);
                Assert.AreEqual("Display template", this.textBlock.Text);
                Assert.IsFalse(this.comboBox.IsEnabled);
                Assert.IsTrue(this.innerTextBox1.IsReadOnly);
                Assert.IsTrue(this.innerTextBox2.IsReadOnly);

                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(editButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsTrue(this.checkBox.IsEnabled);
                Assert.IsTrue(this.datePicker.IsEnabled);
                Assert.IsFalse(this.textBox.IsReadOnly);
                Assert.AreEqual("Edit template", this.textBlock.Text);
                Assert.IsTrue(this.comboBox.IsEnabled);
                Assert.IsFalse(this.innerTextBox1.IsReadOnly);
                Assert.IsFalse(this.innerTextBox2.IsReadOnly);

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(editButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsTrue(cancelButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the submit button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the submit button functions properly.")]
        public void UseCommitButton()
        {
            ButtonBase editButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsTrue(this.checkBox.IsEnabled);
                Assert.IsTrue(this.datePicker.IsEnabled);
                Assert.IsFalse(this.textBox.IsReadOnly);
                Assert.AreEqual("Edit template", this.textBlock.Text);
                Assert.IsTrue(this.comboBox.IsEnabled);
                Assert.IsFalse(this.innerTextBox1.IsReadOnly);
                Assert.IsFalse(this.innerTextBox2.IsReadOnly);

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(editButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsTrue(cancelButton.IsEnabled);

                this.dataClass.StringProperty = "new string";
                Assert.IsTrue(commitButton.IsEnabled);

                this.ExpectEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                Assert.IsFalse(this.checkBox.IsEnabled);
                Assert.IsFalse(this.datePicker.IsEnabled);
                Assert.IsTrue(this.textBox.IsReadOnly);
                Assert.AreEqual("Display template", this.textBlock.Text);
                Assert.IsFalse(this.comboBox.IsEnabled);
                Assert.IsTrue(this.innerTextBox1.IsReadOnly);
                Assert.IsTrue(this.innerTextBox2.IsReadOnly);

                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(editButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the submit button functions properly with changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the submit button functions properly with changes.")]
        public void UseCommitButtonWithChanges()
        {
            ButtonBase editButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;
            DateTime newDateTime = new DateTime(2001, 2, 3);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsFalse(this.DataForm.IsItemChanged);

                this.checkBox.IsChecked = true;
                this.datePicker.SelectedDate = newDateTime;
                this.textBox.Text = "text";
                this.comboBox.SelectedItem = 5;

                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemChanged);
                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);

                this.ExpectEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.IsTrue(this.DataForm.IsItemChanged);

                Assert.AreEqual(true, this.checkBox.IsChecked);
                Assert.AreEqual(newDateTime, this.datePicker.SelectedDate);
                Assert.AreEqual("text", this.textBox.Text);
                Assert.AreEqual(5, this.comboBox.SelectedItem);

                Assert.AreEqual(true, this.dataClass.BoolProperty);
                Assert.AreEqual(newDateTime, this.dataClass.DateTimeProperty);
                Assert.AreEqual("text", this.dataClass.StringProperty);
                Assert.AreEqual(5, this.dataClass.IntProperty);
            });

            this.EnqueueTestComplete();
        }

        #region Helper Methods

        /// <summary>
        /// Retrieves the input controls.
        /// </summary>
        private void GetInputControls()
        {
            this.checkBox = this.DataFormInputControls[0] as CheckBox;
            this.datePicker = this.DataFormInputControls[1] as DatePicker;
            this.textBox = this.DataFormInputControls[2] as TextBox;
            this.textBlock = this.DataFormInputControls[3] as TextBlock;
            this.comboBox = this.DataFormInputControls[4] as ComboBox;

            if (this.DataFormInputControls[5].GetType() == typeof(Grid))
            {
                this.innerTextBox1 = this.DataFormInputControls[6] as TextBox;
                this.innerTextBox2 = this.DataFormInputControls[7] as TextBox;
            }
            else
            {
                this.innerTextBox1 = this.DataFormInputControls[5] as TextBox;
                this.innerTextBox2 = this.DataFormInputControls[6] as TextBox;
            }
        }

        /// <summary>
        /// Handles the beginning of an edit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void CancelBeginningEdit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the ending of a field edit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void CancelItemEditEnding(object sender, DataFormEditEndingEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion Helper Methods
    }
}
