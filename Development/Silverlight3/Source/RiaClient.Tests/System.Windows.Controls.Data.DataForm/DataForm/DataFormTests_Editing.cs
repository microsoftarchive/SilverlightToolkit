//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Editing.cs">
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

                this.ExpectItemEditEnded();
                Assert.IsTrue(this.DataForm.CancelItemEdit());
            });

            this.WaitForItemEditEnded();

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

                this.ExpectItemEditEnded();
                this.DataForm.CancelItemEdit();
            });

            this.WaitForItemEditEnded();

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
        /// Ensure that FieldEditEnding can be canceled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that FieldEditEnding can be canceled.")]
        public void CancelFieldEditEnding()
        {
            string originalString = null;
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
                originalString = this.dataClass.StringProperty;
                this.textBox.Text = "testing";
                this.DataForm.FieldEditEnding += new EventHandler<DataFormFieldEditEndingEventArgs>(CancelFieldEditEnding);
                Assert.IsFalse(this.DataForm.CommitFieldEdit(this.DataForm.Fields[2]));
            });

            this.EnqueueCallback(() =>
            {
                // The committing of the edit should have been cancelled by the event handler.
                Assert.AreEqual(originalString, this.dataClass.StringProperty);
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
                this.DataForm.ItemEditEnding += new EventHandler<DataFormItemEditEndingEventArgs>(CancelItemEditEnding);
                Assert.IsFalse(this.DataForm.CommitItemEdit(true /* exitEditingMode */));
            });

            // We shouldn't get an event, so we need to wait a little while to ensure that no event occurs.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.CancelItemEdit());
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
        /// Ensure that CommitFieldEdit functions properly when committing an invalid value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitFieldEdit functions properly when committing an invalid value.")]
        public void CommitFieldEditWithInvalidValue()
        {
            this.DataForm.CurrentItem = new DataClassWithValidation() { StringProperty = "string 1" };

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
                Assert.IsFalse(this.DataForm.CommitFieldEdit(this.DataForm.Fields[2]));
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitItemEdit functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitItemEdit functions properly.")]
        public void CommitItemEdit()
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

                this.ExpectItemEditEnded();
                Assert.IsTrue(this.DataForm.CommitItemEdit(true /* exitEditingMode */));
            });

            this.WaitForItemEditEnded();

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
        /// Ensure that CommitItemEdit functions properly with changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitItemEdit functions properly with changes.")]
        public void CommitItemEditWithChanges()
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

                this.ExpectItemEditEnded();
                this.DataForm.CommitItemEdit(true /* exitEditingMode */);
            });

            this.WaitForItemEditEnded();

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
        /// Ensure that CommitItemEdit without exiting edit mode functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitItemEdit without exiting edit mode functions properly.")]
        public void CommitItemEditWithoutExitingEditMode()
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

                this.ExpectItemEditEnded();
                this.DataForm.CommitItemEdit(false /* exitEditingMode */);
            });

            this.WaitForItemEditEnded();

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

                this.ExpectItemEditEnded();
                this.DataForm.CancelItemEdit();
            });

            this.WaitForItemEditEnded();

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
        /// Ensure that CommitItemEdit does not throw an exception when committing a template field with a read-only text box.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitItemEdit does not throw an exception when committing a template field with a read-only text box.")]
        public void CommitItemEditWithReadOnlyTemplateField()
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
                this.ExpectItemEditEnded();
                Assert.IsTrue(dataFormApp.dataForm.CommitItemEdit(true /* exitEditingMode */));
            });

            this.WaitForItemEditEnded();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitItemEdit does not throw an exception when committing an item with a read-only text box in the template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitItemEdit does not throw an exception when committing an item with a read-only text box in the template.")]
        public void CommitItemEditWithReadOnlyTemplateItem()
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
                this.ExpectItemEditEnded();
                Assert.IsTrue(dataFormApp.dataForm.CommitItemEdit(true /* exitEditingMode */));
            });

            this.WaitForItemEditEnded();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CommitItemEdit does not throw an exception when committing a read-only text field.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CommitItemEdit does not throw an exception when committing a read-only text field.")]
        public void CommitItemEditWithReadOnlyTextField()
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
                this.ExpectItemEditEnded();
                Assert.IsTrue(dataFormApp.dataForm.CommitItemEdit(true /* exitEditingMode */));
            });

            this.WaitForItemEditEnded();
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
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                Assert.IsTrue(this.DataForm.IsEditing);
                this.ExpectItemEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForItemEditEnded();

            this.EnqueueCallback(() =>
            {
                ButtonBase nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                Assert.IsFalse(this.DataForm.IsEditing);
                this.ExpectCurrentItemChange();
                InvokeButton(nextItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.IsEditing);
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
                df.CommitItemEdit(true /* exitEditingMode */);
                success = true;
            }
            catch (NullReferenceException)
            {
            }

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Ensure that CancelItemEdit does not throw an exception when not in editing mode.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CancelItemEdit does not throw an exception when not in editing mode.")]
        public void EnsureCancelItemEditDoesNotThrowExceptionWhenNotEditing()
        {
            DataClassList dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.ItemsSource = dataClassList;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.CancelItemEdit();
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

                this.ExpectItemEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForItemEditEnded();

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
                Assert.IsTrue(this.DataForm.IsUserAbleToAddItems);
                Assert.IsTrue(this.DataForm.IsUserAbleToDeleteItems);
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
        /// Ensure that the user cannot move off of the current item when editing with AutoCommit set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the user cannot move off of the current item when editing with AutoCommit set to false.")]
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
                Assert.IsFalse(this.DataForm.IsUserAbleToAddItems);
                Assert.IsFalse(this.DataForm.IsUserAbleToDeleteItems);
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
                this.ExpectItemEditEnded();
                this.DataForm.IsReadOnly = true;
            });

            this.WaitForItemEditEnded();

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
                Assert.IsFalse(this.DataForm.CommitItemEdit(true /* exitEditingMode */));
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
                Assert.IsTrue(commitButton.IsEnabled);
                Assert.IsTrue(cancelButton.IsEnabled);

                this.ExpectItemEditEnded();
                InvokeButton(cancelButton);
            });

            this.WaitForItemEditEnded();

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

                this.ExpectItemEditEnded();
                InvokeButton(cancelButton);
            });

            this.WaitForItemEditEnded();

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
                Assert.IsTrue(commitButton.IsEnabled);
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
                Assert.IsTrue(commitButton.IsEnabled);
                Assert.IsTrue(cancelButton.IsEnabled);

                this.ExpectItemEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForItemEditEnded();

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

                this.ExpectItemEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForItemEditEnded();

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
            this.checkBox = this.DataForm.InputControls[0] as CheckBox;
            this.datePicker = this.DataForm.InputControls[1] as DatePicker;
            this.textBox = this.DataForm.InputControls[2] as TextBox;
            this.textBlock = this.DataForm.InputControls[3] as TextBlock;
            this.comboBox = this.DataForm.InputControls[4] as ComboBox;

            if (this.DataForm.InputControls[5].GetType() == typeof(Grid))
            {
                this.innerTextBox1 = this.DataForm.InputControls[6] as TextBox;
                this.innerTextBox2 = this.DataForm.InputControls[7] as TextBox;
            }
            else
            {
                this.innerTextBox1 = this.DataForm.InputControls[5] as TextBox;
                this.innerTextBox2 = this.DataForm.InputControls[6] as TextBox;
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
        private void CancelFieldEditEnding(object sender, DataFormFieldEditEndingEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the ending of a field edit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void CancelItemEditEnding(object sender, DataFormItemEditEndingEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion Helper Methods
    }
}
