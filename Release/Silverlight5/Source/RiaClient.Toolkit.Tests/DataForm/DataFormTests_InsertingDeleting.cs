//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_InsertingDeleting.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests <see cref="DataForm"/> inserting and deleting.
    /// </summary>
    [TestClass]
    public class DataFormTests_InsertingDeleting : DataFormTests_Base
    {
        #region Helper Properties And Fields

        /// <summary>
        /// The entity list being used.
        /// </summary>
        private DataClassList dataClassList;

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
            this.dataClassList = DataClassList.GetDataClassList(5, ListOperations.All);
            this.DataForm.ItemsSource = this.dataClassList;
        }

        #endregion Initialization

        /// <summary>
        /// Ensure that the AddItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the AddItem functions properly.")]
        public void AddItem()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.CanAddItems);
                Assert.IsFalse(this.DataForm.CanCommitEdit);
                Assert.IsFalse(this.DataForm.CanCancelEdit);
                Assert.IsFalse(this.DataForm.IsAddingNew);

                this.GetInputControls();

                Assert.IsFalse(this.checkBox.IsEnabled);
                Assert.IsFalse(this.datePicker.IsEnabled);
                Assert.IsTrue(this.textBox.IsReadOnly);
                Assert.AreEqual("Display template", this.textBlock.Text);
                Assert.IsFalse(this.comboBox.IsEnabled);
                Assert.IsTrue(this.innerTextBox1.IsReadOnly);
                Assert.IsTrue(this.innerTextBox2.IsReadOnly);

                this.ExpectCurrentItemChange();
                Assert.IsTrue(this.DataForm.AddNewItem());
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(6, this.dataClassList.Count);
                Assert.AreEqual(5, this.DataForm.CurrentIndex);
                Assert.AreEqual(this.dataClassList[this.dataClassList.Count - 1], this.DataForm.CurrentItem);

                Assert.IsFalse(this.DataForm.CanAddItems);
                Assert.IsTrue(this.DataForm.CanCommitEdit);
                Assert.IsTrue(this.DataForm.CanCancelEdit);
                Assert.IsTrue(this.DataForm.IsAddingNew);

                this.GetInputControls();

                Assert.IsTrue(this.checkBox.IsEnabled);
                Assert.IsTrue(this.datePicker.IsEnabled);
                Assert.IsFalse(this.textBox.IsReadOnly);
                Assert.AreEqual("Insert template", this.textBlock.Text);
                Assert.IsTrue(this.comboBox.IsEnabled);
                Assert.IsFalse(this.innerTextBox1.IsReadOnly);
                Assert.IsFalse(this.innerTextBox2.IsReadOnly);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the AddItem functions properly and the new item can be submitted.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the AddItem functions properly and the new item can be submitted.")]
        public void AddItemWithSubmit()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.AddNewItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanAddItems);
                Assert.IsTrue(this.DataForm.CanCommitEdit);
                Assert.IsTrue(this.DataForm.CanCancelEdit);
                Assert.IsTrue(this.DataForm.IsAddingNew);

                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(6, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[this.dataClassList.Count - 1], this.DataForm.CurrentItem);

                Assert.IsTrue(this.DataForm.CanAddItems);
                Assert.IsFalse(this.DataForm.CanCommitEdit);
                Assert.IsFalse(this.DataForm.CanCancelEdit);
                Assert.IsFalse(this.DataForm.IsAddingNew);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the AddItem functions properly and the new item can be canceled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the AddItem functions properly and the new item can be canceled.")]
        public void AddItemWithCancel()
        {
            this.DataForm.CurrentItem = this.dataClassList[1];
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.AddNewItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanAddItems);
                Assert.IsTrue(this.DataForm.CanCommitEdit);
                Assert.IsTrue(this.DataForm.CanCancelEdit);
                Assert.IsTrue(this.DataForm.IsAddingNew);

                this.ExpectEditEnded();
                this.DataForm.CancelEdit();
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(5, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);

                Assert.IsTrue(this.DataForm.CanAddItems);
                Assert.IsFalse(this.DataForm.CanCommitEdit);
                Assert.IsFalse(this.DataForm.CanCancelEdit);
                Assert.IsFalse(this.DataForm.IsAddingNew);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that adding an item first checks to see if the current item is invalid, and cancels the addition if so.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that adding an item first checks to see if the current item is invalid, and cancels the addition if so.")]
        public void AddItemWithValidationError()
        {
            this.DataForm.ItemsSource = new List<DataClassWithValidation>()
            {
                new DataClassWithValidation()
                {
                    IntProperty = 1,
                    StringProperty = "test string 1"
                }
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
                this.textBox.Text = "1";
                Assert.IsFalse(this.DataForm.AddNewItem());
                Assert.IsFalse(this.DataForm.IsAddingNew);
                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.IsItemValid);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that AddingItem can be canceled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that AddingItem can be canceled.")]
        public void CancelAddingItem()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.AddingNewItem += new EventHandler<DataFormAddingNewItemEventArgs>(OnDataFormAddingItem);
                Assert.IsFalse(this.DataForm.AddNewItem());
            });

            // We won't get an event, so wait a litle bit to ensure that's the case.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsAddingNew);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DeletingItem can be canceled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DeletingItem can be canceled.")]
        public void CancelDeletingItem()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.DataForm.DeletingItem += new EventHandler<System.ComponentModel.CancelEventArgs>(OnDataFormDeletingItem);
                Assert.IsFalse(this.DataForm.DeleteItem());
            });

            // We won't get an event, so wait a litle bit to ensure that's the case.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(5, this.dataClassList.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that deleting all items functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that deleting all items functions properly.")]
        public void DeleteAllItems()
        {
            ButtonBase deleteItemButton = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");

                this.ExpectCurrentItemChange();
                this.DataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.IsEmpty);
                Assert.IsNull(this.DataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DeleteItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DeleteItem functions properly.")]
        public void DeleteItem()
        {
            ButtonBase deleteItemButton = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");
                Assert.IsTrue(deleteItemButton.IsEnabled);
                this.ExpectCurrentItemChange();
                Assert.IsTrue(this.DataForm.DeleteItem());
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(4, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[0], this.DataForm.CurrentItem);
                this.ExpectCurrentItemChange();
                this.DataForm.CurrentItem = this.dataClassList[1];
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(3, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
                this.ExpectCurrentItemChange();
                this.DataForm.CurrentItem = this.dataClassList[this.dataClassList.Count - 1];
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(2, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DeleteItem functions properly when deleting the last item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DeleteItem functions properly when deleting the last item.")]
        public void DeleteLastItem()
        {
            ButtonBase deleteItemButton = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");

                this.ExpectCurrentItemChange();
                this.DataForm.CurrentIndex = 4;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(4, this.DataForm.CurrentIndex);
                this.ExpectCurrentItemChange();
                InvokeButton(deleteItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(3, this.DataForm.CurrentIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the user is able to add and delete items with AutoEdit initialized to true and when the current item does not change in the ItemsSource change handler.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the user is able to add and delete items with AutoEdit initialized to true and when the current item does not change in the ItemsSource change handler.")]
        public void EnsureCanAddAndDeleteWithInitialAutoEdit()
        {
            DataFormApp_FieldsWithInitialAutoEdit dataFormApp = new DataFormApp_FieldsWithInitialAutoEdit();
            this.DataFormAppBase = dataFormApp;
            this.dataClassList = DataClassList.GetDataClassList(5, ListOperations.All);
            dataFormApp.dataForm.CurrentItem = this.dataClassList[0];
            dataFormApp.dataForm.ItemsSource = this.dataClassList;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.BeginEdit();
                Assert.IsTrue(dataFormApp.dataForm.CanAddItems);
                Assert.IsTrue(dataFormApp.dataForm.CanDeleteItems);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the cancel button is enabled in insert mode when AutoEdit is true.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the cancel button is enabled in insert mode when AutoEdit is true.")]
        public void EnsureCanCancelAfterAddWithAutoEdit()
        {
            this.DataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);
            this.DataForm.AutoEdit = true;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.GetTemplatePart<ButtonBase>("CancelButton").IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the cancel button is enabled in insert mode when the current item does not implement IEditableObject.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the cancel button is enabled in insert mode when the current item does not implement IEditableObject.")]
        public void EnsureCanCancelAfterAddWithoutIEditableObject()
        {
            this.DataForm.ItemsSource = new List<BasicDataClass>() { new BasicDataClass() };
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.GetTemplatePart<ButtonBase>("CancelButton").IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the user is able to delete after a new item is added.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the user is able to delete after a new item is added.")]
        public void EnsureCanDeleteAfterAdd()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                this.DataForm.AddNewItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                ButtonBase deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");

                Assert.IsTrue(this.DataForm.CanDeleteItems);
                Assert.IsTrue(deleteItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the user is still able to delete when in read-only mode.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the user is still able to delete when in read-only mode.")]
        public void EnsureCanDeleteWhenReadOnly()
        {
            this.DataForm.IsReadOnly = true;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.CanDeleteItems);

                this.ExpectCurrentItemChange();
                Assert.IsTrue(this.DataForm.DeleteItem());
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(4, this.DataForm.ItemsCount);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that no deletion is possible with deletion disabled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that no deletion is possible with deletion disabled.")]
        public void EnsureCannotDeleteWithDeletionDisabled()
        {
            this.DataForm.ItemsSource = new EnumerableCollection<int>();
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanDeleteItems);

                ButtonBase deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");

                Assert.IsFalse(deleteItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that no deletion is possible with no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that no deletion is possible with no items.")]
        public void EnsureCannotDeleteWithNoItems()
        {
            this.DataForm.ItemsSource = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanDeleteItems);

                ButtonBase deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");

                Assert.IsFalse(deleteItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that no insertion is possible with insertion disabled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that no insertion is possible with insertion disabled.")]
        public void EnsureCannotInsertWithInsertionDisabled()
        {
            this.DataForm.ItemsSource = new EnumerableCollection<int>();
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanAddItems);

                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                Assert.IsFalse(newItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }
        
        /// <summary>
        /// Ensure that no insertion is possible with IsReadOnly set to true.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that no insertion is possible with IsReadOnly set to true.")]
        public void EnsureCannotInsertWithIsReadOnly()
        {
            this.dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.IsReadOnly = true;
            this.DataForm.ItemsSource = this.dataClassList;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanAddItems);

                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                Assert.IsFalse(newItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that no insertion is possible with no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that no insertion is possible with no items.")]
        public void EnsureCannotInsertWithNoItems()
        {
            this.DataForm.ItemsSource = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanAddItems);

                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");

                Assert.IsFalse(newItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the delete item button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the delete item button functions properly.")]
        public void UseDeleteItemButton()
        {
            ButtonBase deleteItemButton = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");
                Assert.IsTrue(deleteItemButton.IsEnabled);
                this.ExpectCurrentItemChange();
                InvokeButton(deleteItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(4, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[0], this.DataForm.CurrentItem);
                this.ExpectCurrentItemChange();
                this.DataForm.CurrentItem = this.dataClassList[1];
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                InvokeButton(deleteItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(3, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
                this.ExpectCurrentItemChange();
                this.DataForm.CurrentItem = this.dataClassList[this.dataClassList.Count - 1];
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                InvokeButton(deleteItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(2, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the new item button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the new item button functions properly.")]
        public void UseNewItemButton()
        {
            ButtonBase newItemButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                Assert.IsTrue(newItemButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);
                Assert.IsFalse(this.DataForm.IsAddingNew);

                this.GetInputControls();

                Assert.IsFalse(this.checkBox.IsEnabled);
                Assert.IsFalse(this.datePicker.IsEnabled);
                Assert.IsTrue(this.textBox.IsReadOnly);
                Assert.AreEqual("Display template", this.textBlock.Text);
                Assert.IsFalse(this.comboBox.IsEnabled);
                Assert.IsTrue(this.innerTextBox1.IsReadOnly);
                Assert.IsTrue(this.innerTextBox2.IsReadOnly);

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(6, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[this.dataClassList.Count - 1], this.DataForm.CurrentItem);

                Assert.IsFalse(newItemButton.IsEnabled);
                Assert.IsTrue(commitButton.IsEnabled);
                Assert.IsTrue(cancelButton.IsEnabled);
                Assert.IsTrue(this.DataForm.IsAddingNew);

                this.GetInputControls();

                Assert.IsTrue(this.checkBox.IsEnabled);
                Assert.IsTrue(this.datePicker.IsEnabled);
                Assert.IsFalse(this.textBox.IsReadOnly);
                Assert.AreEqual("Insert template", this.textBlock.Text);
                Assert.IsTrue(this.comboBox.IsEnabled);
                Assert.IsFalse(this.innerTextBox1.IsReadOnly);
                Assert.IsFalse(this.innerTextBox2.IsReadOnly);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the new item button functions properly and the new item can be submitted.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the new item button functions properly and the new item can be submitted.")]
        public void UseNewItemButtonWithSubmit()
        {
            ButtonBase newItemButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(newItemButton.IsEnabled);
                Assert.IsTrue(commitButton.IsEnabled);
                Assert.IsTrue(cancelButton.IsEnabled);
                Assert.IsTrue(this.DataForm.IsAddingNew);

                this.ExpectEditEnded();
                InvokeButton(commitButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(6, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[this.dataClassList.Count - 1], this.DataForm.CurrentItem);

                Assert.IsTrue(newItemButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);
                Assert.IsFalse(this.DataForm.IsAddingNew);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the new item button functions properly and the new item can be canceled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the new item button functions properly and the new item can be canceled.")]
        public void UseNewItemButtonWithCancel()
        {
            ButtonBase newItemButton = null;
            ButtonBase commitButton = null;
            ButtonBase cancelButton = null;

            this.DataForm.CurrentItem = this.dataClassList[1];
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");

                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(newItemButton.IsEnabled);
                Assert.IsTrue(commitButton.IsEnabled);
                Assert.IsTrue(cancelButton.IsEnabled);
                Assert.IsTrue(this.DataForm.IsAddingNew);

                this.ExpectEditEnded();
                InvokeButton(cancelButton);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(5, this.dataClassList.Count);
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);

                Assert.IsTrue(newItemButton.IsEnabled);
                Assert.IsFalse(commitButton.IsEnabled);
                Assert.IsFalse(cancelButton.IsEnabled);
                Assert.IsFalse(this.DataForm.IsAddingNew);
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
        /// Handles the adding of an item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormAddingItem(object sender, DataFormAddingNewItemEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Handles the deleting of an item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormDeletingItem(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion Helper Methods
    }
}
