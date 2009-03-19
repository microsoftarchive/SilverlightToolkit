//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Miscellaneous.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests miscellaneous aspects of the <see cref="DataForm"/>.
    /// </summary>
    [TestClass]
    public class DataFormTests_Miscellaneous : DataFormTests_Base
    {
        /// <summary>
        /// Ensure that event handlers are not added multiple times when OnApplyTemplate is called multiple times
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that event handlers are not added multiple times when OnApplyTemplate is called multiple times.")]
        public void CallOnApplyTemplateMultipleTimes()
        {
            DataFormApp_Standard dataFormApp = new DataFormApp_Standard();
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.OnApplyTemplate();
                dataFormApp.dataForm.OnApplyTemplate();

                // The only buttons that would have any additional effect if there were multiple event handlers
                // would be next and previous, since the others make it so you can't do the same action again
                // once you do it the first time.
                ButtonBase nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                this.ExpectCurrentItemChange();
                InvokeButton(nextItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(1, dataFormApp.dataForm.CurrentIndex);
                dataFormApp.dataForm.CurrentIndex = 2;

                ButtonBase previousItemButton = this.GetTemplatePart<ButtonBase>("PreviousItemButton");
                this.ExpectCurrentItemChange();
                InvokeButton(previousItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(1, dataFormApp.dataForm.CurrentIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that when the DataForm is handed a collection view with no currency set, it sets it to the first item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that when the DataForm is handed a collection view with no currency set, it sets it to the first item.")]
        public void EnsureCollectionViewAtBeforeFirstIsMovedToFirst()
        {
            DataFormApp_Standard dataFormApp = new DataFormApp_Standard();
            PagedCollectionView cv = new PagedCollectionView(DataClassList.GetDataClassList(2, ListOperations.All));
            cv.MoveCurrentToPosition(-1);
            dataFormApp.dataForm.ItemsSource = cv;

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, cv.CurrentPosition);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that calling BeginEdit and then calling CommitItemEdit when the current item is already invalid does not end the edit.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that calling BeginEdit and then calling CommitItemEdit when the current item is already invalid does not end the edit.")]
        public void EnsureBeginEditThenCommitItemEditWithInvalidObjectDoesNotEndEdit()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            // DataClassWithValidation starts out with a blank StringProperty, which makes it start in an invalid state
            // if that property is not set.
            dataFormApp.dataForm.CurrentItem = new DataClassWithValidation();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.BeginEdit();
                dataFormApp.dataForm.CommitItemEdit(true /* exitEditingMode */);
                Assert.IsTrue(dataFormApp.dataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the first editable field is focused after calling BeginEdit.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the first editable field is focused after calling BeginEdit.")]
        public void EnsureFirstEditableFieldFocused()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.BeginEdit();
            });

            // Need to wait for a short while to give the content time to load.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(dataFormApp.dataForm.InputControls[0], FocusManager.GetFocusedElement());
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the editable field with the lowest TabIndex in a template is focused after calling BeginEdit.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the editable field with the lowest TabIndex in a template is focused after calling BeginEdit.")]
        public void EnsureFirstEditableFieldFocusedByTabIndex()
        {
            DataFormApp_TemplatesWithTabIndex dataFormApp = new DataFormApp_TemplatesWithTabIndex();
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.BeginEdit();
            });

            // Need to wait for a short while to give the content time to load.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                TextBox textBox = FocusManager.GetFocusedElement() as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Text Box 2", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that filling in the content of grouped fields does not cause non-grouped fields to shrink.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that filling in the content of grouped fields does not cause non-grouped fields to shrink.")]
        public void EnsureNonGroupedFieldsDoNotShrink()
        {
            TextBox outerTextBox = null;
            double previousTextBoxWidth = 0;
            DataFormApp_FieldsGroupWithNonGrouped dataFormApp = new DataFormApp_FieldsGroupWithNonGrouped();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                TextBox innerTextBox = dataFormApp.dataForm.InputControls[1] as TextBox;
                outerTextBox = dataFormApp.dataForm.InputControls[3] as TextBox;
                previousTextBoxWidth = outerTextBox.ActualWidth;
                innerTextBox.Text = "looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong";
            });

            // Give the text box time to change its size if it's going to.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(previousTextBoxWidth, outerTextBox.ActualWidth);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that GetFieldElement() works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that GetFieldElement() works properly.")]
        public void GetFieldElement()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(dataFormApp.dataForm.InputControls[0], dataFormApp.dataForm.GetFieldElement(dataFormApp.dataForm.Fields[0]));
                Assert.AreEqual(dataFormApp.dataForm.InputControls[1], dataFormApp.dataForm.GetFieldElement(dataFormApp.dataForm.Fields[1]));
                Assert.AreEqual(dataFormApp.dataForm.InputControls[2], dataFormApp.dataForm.GetFieldElement(dataFormApp.dataForm.Fields[2]));
                Assert.AreEqual(dataFormApp.dataForm.InputControls[3], dataFormApp.dataForm.GetFieldElement(dataFormApp.dataForm.Fields[3]));
                Assert.AreEqual(dataFormApp.dataForm.InputControls[4], dataFormApp.dataForm.GetFieldElement(dataFormApp.dataForm.Fields[4]));
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that moving the current item to null when appending makes CancelAppend get called.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that moving the current item to null when appending makes CancelAppend get called.")]
        public void MoveCurrencyToNullWhenAppending()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            List<DataClassWithValidation> dataClassList =
                new List<DataClassWithValidation>()
                {
                    new DataClassWithValidation()
                    {
                        StringProperty = "string 1"
                    }
                };

            PagedCollectionView pcv = new PagedCollectionView(dataClassList);

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.ItemsSource = pcv;
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                dataFormApp.dataForm.AddItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(2, pcv.Count);
                this.ExpectCurrentItemChange();
                pcv.MoveCurrentToPosition(-1);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(1, pcv.Count);
            });

            this.EnqueueTestComplete();
        }


        /// <summary>
        /// Ensure that changing a DataGrid's selected item with a DataForm's current item bound to it does not succeed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changing a DataGrid's selected item with a DataForm's current item bound to it does not succeed.")]
        public void MoveDataGridSelectionWithValidationError()
        {
            DataFormApp_FieldsWithDataGrid dataFormApp = new DataFormApp_FieldsWithDataGrid();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
                dataFormApp.dataGrid.ItemsSource =
                    new List<DataClassWithValidation>()
                    {
                        new DataClassWithValidation()
                        {
                            StringProperty = "string 1"
                        },

                        new DataClassWithValidation()
                        {
                            StringProperty = "string 2"
                        }
                    };
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = dataFormApp.dataForm.InputControls[2] as TextBox;
                textBox.Text = string.Empty;
                dataFormApp.dataForm.CommitItemEdit(true /* exitEditingMode */);
                Assert.IsFalse(dataFormApp.dataForm.IsItemValid);
                dataFormApp.dataGrid.SelectedIndex = 1;
            });

            // Give the DataGrid time to have its selected item put back.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, dataFormApp.dataGrid.SelectedIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting the current item after the DataForm loads and with auto-generation on does not throw an exception.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the current item after the DataForm loads and with auto-generation on does not throw an exception.")]
        public void SetCurrentItemAfterLoadWithAutoGeneration()
        {
            DataFormApp_AutoGeneration dataFormApp = new DataFormApp_AutoGeneration();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting the current item to null on the DataForm works properly when the DataForm is bound to a set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the current item to null on the DataForm works properly when the DataForm is bound to a set.")]
        public void SetCurrentItemToNullWithSet()
        {
            DataFormApp_Standard dataFormApp = new DataFormApp_Standard();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
                dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.CurrentItem = null;
                Assert.IsNull(dataFormApp.dataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting DataForm.FieldLabelPosition to Top and then setting it back to Left does not raise an exception.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting DataForm.FieldLabelPosition to Top and then setting it back to Left does not raise an exception.")]
        public void SetFieldLabelPositionTwice()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.FieldLabelPosition = DataFormFieldLabelPosition.Top;
                dataFormApp.dataForm.FieldLabelPosition = DataFormFieldLabelPosition.Left;
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting the header on the DataForm works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the header on the DataForm works properly.")]
        public void SetHeader()
        {
            ContentControl headerElement = null;
            DataFormApp_Standard dataFormApp = new DataFormApp_Standard();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                headerElement = this.GetTemplatePart<ContentControl>("HeaderElement");

                dataFormApp.dataForm.Header = "header";
                Assert.AreEqual("header", headerElement.Content);

                dataFormApp.dataForm.Header = "DataForm 1";
                Assert.AreEqual("DataForm 1", headerElement.Content);

                dataFormApp.dataForm.Header = string.Empty;
                Assert.AreEqual(string.Empty, headerElement.Content);

                dataFormApp.dataForm.Header = null;
                Assert.IsNull(headerElement.Content);

                dataFormApp.dataForm.HeaderTemplate = new HeaderTemplate();
                Assert.IsNotNull(headerElement.ContentTemplate);
                TextBlock textBlock = headerElement.ContentTemplate.LoadContent() as TextBlock;
                Assert.IsNotNull(textBlock);
                Assert.AreEqual("DataForm", textBlock.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that giving a text box a lot of content does not change its size.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that giving a text box a lot of content does not change its size.")]
        public void SetTextBoxToLongContent()
        {
            TextBox textBox = null;
            double previousTextBoxWidth = 0;
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textBox = dataFormApp.dataForm.InputControls[2] as TextBox;
                previousTextBoxWidth = textBox.ActualWidth;
                textBox.Text = "looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong";
            });

            // Give the text box time to change its size if it's going to.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(previousTextBoxWidth, textBox.ActualWidth);
            });

            this.EnqueueTestComplete();
        }
    }
}
