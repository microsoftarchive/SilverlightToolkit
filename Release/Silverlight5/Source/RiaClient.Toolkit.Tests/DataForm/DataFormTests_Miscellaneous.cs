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
using System.Windows.Media;
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
        #region Fields

        private bool _dataFormLayoutUpdated;
        private bool _isAppLoaded;

        #endregion

        /// <summary>
        /// Ensure that event handlers are not added multiple times when OnApplyTemplate is called multiple times.
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
                dataFormApp.dataForm.CommitEdit(true /* exitEditingMode */);
                Assert.IsTrue(dataFormApp.dataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the cancel button is shown before an edit begins when in AutoEdit mode and when IEditableObject is implemented.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the cancel button is shown before an edit begins when in AutoEdit mode and when IEditableObject is implemented.")]
        public void EnsureCancelButtonShownBeforeEditBeginsWithAutoEdit()
        {
            DataFormApp_FieldsWithInitialAutoEdit dataFormApp = new DataFormApp_FieldsWithInitialAutoEdit();
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                ButtonBase cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");
                Assert.AreEqual(Visibility.Visible, cancelButton.Visibility);
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
        /// Ensure that moving the collection view's currency to after the last item does not make CurrentIndex get out of sync.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that moving the collection view's currency to after the last item does not make CurrentIndex get out of sync.")]
        public void EnsureCurrentIndexSyncedAfterCollectionViewMovesToAfterLast()
        {
            DataFormApp_Standard dataFormApp = new DataFormApp_Standard();
            PagedCollectionView pcv = new PagedCollectionView(DataClassList.GetDataClassList(2, ListOperations.All));
            dataFormApp.dataForm.ItemsSource = pcv;

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, pcv.CurrentPosition);
                Assert.AreEqual(pcv[0], dataFormApp.dataForm.CurrentItem);
                this.ExpectCurrentItemChange();
                pcv.MoveCurrentToPosition(2);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(2, pcv.CurrentPosition);
                Assert.AreEqual(2, dataFormApp.dataForm.CurrentIndex);
                Assert.IsNull(dataFormApp.dataForm.CurrentItem);
                this.ExpectCurrentItemChange();
                pcv.MoveCurrentToPosition(0);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                dataFormApp.dataForm.CurrentIndex = 2;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(2, pcv.CurrentPosition);
                Assert.AreEqual(2, dataFormApp.dataForm.CurrentIndex);
                Assert.IsNull(dataFormApp.dataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that applying an EditableAttribute to a property is honored.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that applying an EditableAttribute to a property is honored.")]
        public void EnsureEditableAttributeHonored()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.ItemsSource =
                new List<DataClassWithEditableAttribute>()
                {
                    new DataClassWithEditableAttribute(),
                    new DataClassWithEditableAttribute()
                };

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
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
                Assert.IsFalse(dataFormApp.dataForm.Fields[0].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[1].IsReadOnly);
                Assert.IsFalse(dataFormApp.dataForm.Fields[2].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[4].IsReadOnly);

                this.ExpectContentLoaded();
                dataFormApp.dataForm.CancelEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(dataFormApp.dataForm.Fields[0].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[1].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[2].IsReadOnly);
                Assert.IsFalse(dataFormApp.dataForm.Fields[4].IsReadOnly);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that changing CurrentItem without ItemsSource being set when editing and with AutoCommit false causes an exception to be thrown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changing CurrentItem without ItemsSource being set when editing and with AutoCommit false causes an exception to be thrown.")]
        public void EnsureExceptionThrownWhenCurrencyChangesWhenEditingWithAutoCommitFalse()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.AutoCommit = false;
            DataClassWithValidation dataClass = new DataClassWithValidation() { StringProperty = "test string 0" };
            dataFormApp.dataForm.CurrentItem = dataClass;

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
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
                TextBox textBox = this.DataFormInputControls[2] as TextBox;
                textBox.Text = "new string";
                BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);

                Assert.IsNotNull(bindingExpression);
                bindingExpression.UpdateSource();

                bool changingCurrentItemSucceeded = false;

                try
                {
                    dataFormApp.dataForm.CurrentItem = new DataClass();
                    changingCurrentItemSucceeded = true;
                }
                catch (InvalidOperationException)
                {
                }

                Assert.IsFalse(changingCurrentItemSucceeded);
                Assert.AreEqual(dataClass, dataFormApp.dataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that changing CurrentItem without ItemsSource being set with a validation error causes an exception to be thrown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changing CurrentItem without ItemsSource being set with a validation error causes an exception to be thrown.")]
        public void EnsureExceptionThrownWhenCurrencyChangesWithValidationError()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            DataClassWithValidation dataClass = new DataClassWithValidation() { StringProperty = "test string 0" };
            dataFormApp.dataForm.CurrentItem = dataClass;

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
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
                TextBox textBox = this.DataFormInputControls[2] as TextBox;
                textBox.Text = string.Empty;
                dataFormApp.dataForm.CommitEdit(true /* exitEditingMode */);
                Assert.IsFalse(dataFormApp.dataForm.IsItemValid);

                bool changingCurrentItemSucceeded = false;

                try
                {
                    dataFormApp.dataForm.CurrentItem = new DataClass();
                    changingCurrentItemSucceeded = true;
                }
                catch (InvalidOperationException)
                {
                }

                Assert.IsFalse(changingCurrentItemSucceeded);
                Assert.AreEqual(dataClass, dataFormApp.dataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that references to fields are not left over at the root of the DataForm when UI is regenerated.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that references to fields are not left over at the root of the DataForm when UI is regenerated.")]
        public void EnsureFieldsAreRemovedWhenRegeneratingUI()
        {
            Panel dataFormRoot = null;
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.AutoEdit = true;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormRoot = VisualTreeHelper.GetChild(dataFormApp.dataForm, 0) as Panel;
                Assert.IsNotNull(dataFormRoot);
                Assert.AreEqual(7, (dataFormRoot.GetValue(DataField.GroupedFieldListProperty) as IList<DataField>).Count);
                dataFormApp.dataForm.BeginEdit();
                Assert.AreEqual(7, (dataFormRoot.GetValue(DataField.GroupedFieldListProperty) as IList<DataField>).Count);
                this.ExpectContentLoaded();
                dataFormApp.dataForm.CancelEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(7, (dataFormRoot.GetValue(DataField.GroupedFieldListProperty) as IList<DataField>).Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that FindNameInContent works to find elements within DataFields in all modes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that FindNameInContent works to find elements within DataFields in all modes.")]
        public void EnsureFindNameInContentWorksInAllModes()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(dataFormApp.dataForm.FindNameInContent("textBox"));
                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(dataFormApp.dataForm.FindNameInContent("textBox"));
                this.ExpectContentLoaded();
                dataFormApp.dataForm.CancelEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(dataFormApp.dataForm.FindNameInContent("textBox"));
                this.ExpectContentLoaded();
                dataFormApp.dataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(dataFormApp.dataForm.FindNameInContent("textBox"));
                this.ExpectContentLoaded();
                dataFormApp.dataForm.CancelEdit();
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that FindName works to find elements within DataFields in all modes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that FindName works to find elements within DataFields in all modes.")]
        public void EnsureFindNameWorksInAllModes()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);
            dataFormApp.dataForm.ContentLoaded += new EventHandler<DataFormContentLoadEventArgs>(this.OnDataFormContentLoaded);

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
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
                this.ExpectContentLoaded();
                dataFormApp.dataForm.CancelEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.CancelEdit();
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
                this.ExpectContentLoaded();
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.DataFormInputControls[0], FocusManager.GetFocusedElement());
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
                this.ExpectContentLoaded();
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = FocusManager.GetFocusedElement() as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("Text Box 2", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the foreground set on the DataForm propagates down into the DataFields.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the foreground set on the DataForm propagates down into the DataFields.")]
        public void EnsureForegroundPropagates()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.CurrentItem = new DataClass();
            dataFormApp.dataForm.Foreground = new SolidColorBrush(Colors.Red);

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                foreach (DataField field in dataFormApp.dataForm.Fields)
                {
                    Assert.AreEqual(Colors.Red, (field.Foreground as SolidColorBrush).Color);
                }
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure having no buttons present causes the header to become collapsed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure having no buttons present causes the header to become collapsed.")]
        public void EnsureNoButtonsCausesHeaderToDisappear()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);
            dataFormApp.dataForm.CommandButtonsVisibility = DataFormCommandButtonsVisibility.None;

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(Visibility.Collapsed, dataFormApp.dataForm.HeaderVisibility);
                dataFormApp.dataForm.CommandButtonsVisibility = DataFormCommandButtonsVisibility.All;
                Assert.AreEqual(Visibility.Visible, dataFormApp.dataForm.HeaderVisibility);
                dataFormApp.dataForm.CommandButtonsVisibility = DataFormCommandButtonsVisibility.None;
                Assert.AreEqual(Visibility.Collapsed, dataFormApp.dataForm.HeaderVisibility);
                dataFormApp.dataForm.HeaderVisibility = Visibility.Visible;
                Assert.AreEqual(Visibility.Visible, dataFormApp.dataForm.HeaderVisibility);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that missing descriptions and repositioned labels and descriptions do not break the layout.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that missing descriptions and repositioned labels and descriptions do not break the layout.")]
        public void EnsureLayoutOfFieldsIsCorrect()
        {
            DataFormApp_FieldsWithLayoutChanges dataFormApp = new DataFormApp_FieldsWithLayoutChanges();
            dataFormApp.dataForm.CurrentItem = new DataClassWithMissingDescriptions();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                TextBox textBox1 = this.DataFormInputControls[0] as TextBox;
                TextBox textBox2 = this.DataFormInputControls[1] as TextBox;
                TextBox textBox3 = this.DataFormInputControls[2] as TextBox;
                TextBox textBox4 = this.DataFormInputControls[3] as TextBox;

                Assert.IsTrue(this.GetPosition(textBox1).X == this.GetPosition(textBox2).X);
                Assert.IsTrue(textBox1.ActualWidth == textBox2.ActualWidth);
                Assert.IsTrue(this.GetPosition(textBox2).X == this.GetPosition(textBox3).X);
                Assert.IsTrue(textBox2.ActualWidth == textBox3.ActualWidth);
                Assert.IsTrue(this.GetPosition(textBox3).X == this.GetPosition(textBox4).X);
                Assert.IsTrue(textBox3.ActualWidth == textBox4.ActualWidth);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting the ItemsSource does not prevent the DataForm from being garbage collected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the ItemsSource does not prevent the DataForm from being garbage collected.")]
        public void EnsureNoMemoryLeaks()
        {
            DataFormApp_NoStrongReference dataFormApp = new DataFormApp_NoStrongReference();
            WeakReference dataFormReference = new WeakReference(dataFormApp.LayoutRoot.Children[0]);
            PagedCollectionView pcv = new PagedCollectionView(DataClassList.GetDataClassList(3, ListOperations.All));
            (dataFormReference.Target as DataForm).ItemsSource = pcv;

            this.EnqueueCallback(() =>
            {
                dataFormApp.Loaded += new RoutedEventHandler(this.OnDataFormAppLoaded);
                this.TestPanel.Children.Add(dataFormApp);
            });

            this.EnqueueConditional(() => this._isAppLoaded);

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(dataFormReference.IsAlive);
                dataFormApp.LayoutRoot.Children.Clear();
            });

            // 


            this.EnqueueCallback(() =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            });

            this.EnqueueCallback(() =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Assert.IsFalse(dataFormReference.IsAlive);
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
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                TextBox innerTextBox = this.DataFormInputControls[1] as TextBox;
                outerTextBox = this.DataFormInputControls[2] as TextBox;
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
        /// Ensure that applying an ReadOnlyAttribute to a class is honored.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that applying an ReadOnlyAttribute to a class is honored.")]
        public void EnsureReadOnlyAttributeOnClassHonored()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.ItemsSource =
                new List<ReadOnlyDataClass>()
                {
                    new ReadOnlyDataClass(),
                    new ReadOnlyDataClass()
                };

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
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
                Assert.IsTrue(dataFormApp.dataForm.Fields[0].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[1].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[2].IsReadOnly);

                this.ExpectContentLoaded();
                dataFormApp.dataForm.CancelEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(dataFormApp.dataForm.Fields[0].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[1].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[2].IsReadOnly);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that applying an ReadOnlyAttribute to a property is honored.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that applying an ReadOnlyAttribute to a property is honored.")]
        public void EnsureReadOnlyAttributeOnPropertyHonored()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.ItemsSource =
                new List<DataClassWithReadOnlyAttribute>()
                {
                    new DataClassWithReadOnlyAttribute(),
                    new DataClassWithReadOnlyAttribute()
                };

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
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
                Assert.IsFalse(dataFormApp.dataForm.Fields[0].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[1].IsReadOnly);
                Assert.IsFalse(dataFormApp.dataForm.Fields[2].IsReadOnly);

                this.ExpectContentLoaded();
                dataFormApp.dataForm.CancelEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(dataFormApp.dataForm.Fields[0].IsReadOnly);
                Assert.IsTrue(dataFormApp.dataForm.Fields[1].IsReadOnly);
                Assert.IsFalse(dataFormApp.dataForm.Fields[2].IsReadOnly);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that when ShortName is set in a DisplayAttribute, but Name is not, ShortName is used for the label content.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that when ShortName is set in a DisplayAttribute, but Name is not, ShortName is used for the label content.")]
        public void EnsureShortNameFallbackApplied()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.CurrentItem = new DataClassWithShortNames();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Bool Property", dataFormApp.dataForm.Fields[0].InternalLabel.Content);
                Assert.AreEqual("Date Time Property", dataFormApp.dataForm.Fields[1].InternalLabel.Content);
                Assert.AreEqual("String Property", dataFormApp.dataForm.Fields[2].InternalLabel.Content);
                Assert.AreEqual("Int Property", dataFormApp.dataForm.Fields[4].InternalLabel.Content);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the setter for the template in DataForm.DataFieldStyle is correctly picked up.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the setter for the template in DataForm.DataFieldStyle is correctly picked up.")]
        public void EnsureTemplateInDataFieldStyleGetsPickedUp()
        {
            DataFormApp_FieldsWithStylesWithTemplates dataFormApp = new DataFormApp_FieldsWithStylesWithTemplates();
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(1, VisualTreeHelper.GetChildrenCount(dataFormApp.dataForm.Fields[0]));
                ContentControl contentControl = VisualTreeHelper.GetChild(dataFormApp.dataForm.Fields[0], 0) as ContentControl;
                Assert.IsNotNull(contentControl);
                Assert.IsNotNull(contentControl.Content);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that giving the DataForm a new template at runtime does not cause an exception to be thrown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that giving the DataForm a new template at runtime does not cause an exception to be thrown.")]
        public void EnsureTemplateInDataFormStyleDoesNotThrowException()
        {
            DataFormApp_FieldsWithStylesWithTemplates dataFormApp = new DataFormApp_FieldsWithStylesWithTemplates();
            dataFormApp.dataForm.CurrentItem = new DataClass();
            DateTime timeStartedWaiting = new DateTime();
            bool timeExpired = false;

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.LayoutUpdated += new EventHandler(this.OnDataFormLayoutUpdated);
                timeStartedWaiting = DateTime.Now;
                dataFormApp.dataForm.Style = dataFormApp.DataFormStyle;
            });

            this.EnqueueConditional(() =>
            {
                if ((DateTime.Now - timeStartedWaiting).TotalMilliseconds > 1000)
                {
                    timeExpired = true;
                    return true;
                }

                return this._dataFormLayoutUpdated;
            });

            this.EnqueueCallback(() =>
            {
                if (timeExpired)
                {
                    Assert.Fail("The DataForm's style was not applied.");
                }
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that updating the buttons when the DataForm is disabled does not disable them.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that updating the buttons when the DataForm is disabled does not disable them.")]
        public void EnsureUpdateButtonsWhenDisabledDoesNotDisableButtons()
        {
            DataFormApp_Standard dataFormApp = new DataFormApp_Standard();
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(4, ListOperations.All);
            dataFormApp.dataForm.CurrentIndex = 1;

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.IsEnabled = false;
                this.ExpectCurrentItemChange();
                dataFormApp.dataForm.DeleteItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.IsEnabled = true;

                ButtonBase firstItemButton = this.GetTemplatePart<ButtonBase>("FirstItemButton");
                ButtonBase previousItemButton = this.GetTemplatePart<ButtonBase>("PreviousItemButton");
                ButtonBase nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                ButtonBase lastItemButton = this.GetTemplatePart<ButtonBase>("LastItemButton");
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                ButtonBase deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");

                Assert.IsTrue(firstItemButton.IsEnabled);
                Assert.IsTrue(previousItemButton.IsEnabled);
                Assert.IsTrue(nextItemButton.IsEnabled);
                Assert.IsTrue(lastItemButton.IsEnabled);
                Assert.IsTrue(newItemButton.IsEnabled);
                Assert.IsTrue(deleteItemButton.IsEnabled);
                Assert.IsTrue(editButton.IsEnabled);
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
                dataFormApp.dataForm.AddNewItem();
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
        /// Ensure that setting DataFormLabeledField.DescriptionViewerStyle at runtime works.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting DataFormLabeledField.DescriptionViewerStyle at runtime works.")]
        public void SetDescriptionViewerStyleAtRuntime()
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
                DataField field = dataFormApp.dataForm.Fields[0];
                Style style = new Style(typeof(DescriptionViewer));
                style.Setters.Add(new Setter(DescriptionViewer.FontSizeProperty, 20));
                field.DescriptionViewerStyle = style;
                Assert.AreEqual(20, (this.DataFormDescriptions[0] as DescriptionViewer).FontSize);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting DataForm.LabelPosition to Top at runtime correctly changes its horizontal alignment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting DataForm.LabelPosition to Top at runtime correctly changes its horizontal alignment.")]
        public void SetLabelPositionAtRuntime()
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
                Assert.AreEqual(HorizontalAlignment.Right, (this.DataFormLabels[0] as Label).HorizontalAlignment);

                this.ExpectContentLoaded();
                DataField field = dataFormApp.dataForm.Fields[0];
                field.LabelPosition = DataFieldLabelPosition.Top;
                this.ExpectFieldContentLoaded(field);
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(HorizontalAlignment.Left, (this.DataFormLabels[0] as Label).HorizontalAlignment);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting DataForm.LabelPosition to Top and then setting it back to Left does not raise an exception.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting DataForm.LabelPosition to Top and then setting it back to Left does not raise an exception.")]
        public void SetLabelPositionTwice()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                dataFormApp.dataForm.LabelPosition = DataFieldLabelPosition.Top;
                dataFormApp.dataForm.LabelPosition = DataFieldLabelPosition.Left;
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
        /// Ensure that setting DataField.PropertyPath is correctly propagated down to the label and description.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting DataField.PropertyPath is correctly propagated down to the label and description.")]
        public void SetPropertyPath()
        {
            DataFormApp_FieldsWithPropertyPath dataFormApp = new DataFormApp_FieldsWithPropertyPath();
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Label label = this.DataFormLabels[0];
                DescriptionViewer descriptionViewer = this.DataFormDescriptions[0];

                Assert.AreEqual("String Property", label.Content);
                Assert.AreEqual("String Property Description", descriptionViewer.Description);
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
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textBox = this.DataFormInputControls[2] as TextBox;
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

        #region Helper Methods

        /// <summary>
        /// Handles the case where a DataForm's content has loaded.
        /// </summary>
        /// <param name="sender">The DataForm.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormContentLoaded(object sender, DataFormContentLoadEventArgs e)
        {
            Assert.IsNotNull(e.Content.FindName("textBox"));
        }

        /// <summary>
        /// Handles the case where a DataForm app has loaded
        /// </summary>
        /// <param name="sender">The DataForm app.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormAppLoaded(object sender, RoutedEventArgs e)
        {
            this._isAppLoaded = true;
        }

        /// <summary>
        /// Handles the case where a DataForm's layout has been updated.
        /// </summary>
        /// <param name="sender">The DataForm.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormLayoutUpdated(object sender, EventArgs e)
        {
            this._dataFormLayoutUpdated = true;
        }

        #endregion Helper Methods
    }
}
