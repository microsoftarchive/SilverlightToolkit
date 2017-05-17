//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Validation.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests <see cref="DataForm"/> validation.
    /// </summary>
    [TestClass]
    public class DataFormTests_Validation : DataFormTests_Base
    {
        #region Helper Properties And Fields

        /// <summary>
        /// The text box from the associated field.
        /// </summary>
        private TextBox textBox;

        /// <summary>
        /// The combo box from the associated field.
        /// </summary>
        private ComboBox comboBox;

        /// <summary>
        /// Holds whether or not a text box's text has changed.
        /// </summary>
        private bool textBoxTextChanged;

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
            this.DataForm.CurrentItem = new DataClassWithValidation()
            {
                IntProperty = 1,
                StringProperty = "test string 1"
            };
        }

        #endregion Initialization

        /// <summary>
        /// Ensure that validation occurs properly when the current item changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that externally added errors are not removed when validating the item.")]
        public void EnsureExternalErrorsAreNotRemoved()
        {
            ValidationSummary validationSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                validationSummary.Errors.Add(new ValidationSummaryItem("Message", "Header", ValidationSummaryItemType.ObjectError, null, null));
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                Assert.AreEqual(1, validationSummary.Errors.Count);
                SetValue(this.textBox, "1");
                this.CommitAllFields();
                Assert.AreEqual(1, validationSummary.Errors.Count);

                this.DataForm.ValidateItem();
                Assert.AreEqual(2, validationSummary.Errors.Count);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();
                this.DataForm.ValidateItem();

                Assert.AreEqual(1, validationSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that validation occurs properly when the current item changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that validation occurs properly when the current item changes.")]
        public void EnsureExternalErrorCausesValidationSummaryToShow()
        {
            PagedCollectionView pcv = new PagedCollectionView(new List<DataClassWithValidation>()
            {
                new DataClassWithValidation(),
                new DataClassWithValidation(),
                new DataClassWithValidation(),
            });

            this.DataForm.CurrentItem = null;
            this.DataForm.ItemsSource = pcv;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(Visibility.Visible, this.DataForm.ValidationSummary.Visibility);
                Assert.AreEqual(0, this.DataForm.ValidationSummary.ActualHeight);
                this.DataForm.ValidationSummary.Errors.Add(new ValidationSummaryItem("Message", "Header", ValidationSummaryItemType.ObjectError, null, null));
            });

            // Wait a short time for the error summary to get a height.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(Visibility.Visible, this.DataForm.ValidationSummary.Visibility);
                Assert.AreNotEqual(0, this.DataForm.ValidationSummary.ActualHeight);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that object-level errors are not shown when at least one of the sources is involved in a property-level validation error.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that object-level errors are not shown when at least one of the sources is involved in a property-level validation error.")]
        public void EnsureObjectLevelErrorsInvolvingInvalidFieldsAreNotShown()
        {
            ValidationSummary validationSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();
                this.textBox.Text = "1";

                this.comboBox.SelectedItem = 9;
                this.DataForm.ValidateItem();
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual("IntProperty must be between 0 and 7.", validationSummary.Errors[0].Message);

                this.comboBox.SelectedItem = 1;
                this.DataForm.ValidateItem();
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual("IntProperty cannot be equal to StringProperty.", validationSummary.Errors[0].Message);

                this.comboBox.SelectedItem = 9;
                this.DataForm.ValidateItem();
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual("IntProperty must be between 0 and 7.", validationSummary.Errors[0].Message);

                this.comboBox.SelectedItem = 2;
                Assert.AreEqual(0, validationSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that property-level validation errors are not added to the list twice.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that property-level validation errors are not added to the list twice.")]
        public void EnsurePropertyLevelErrorsAreNotAddedTwice()
        {
            ValidationSummary validationSummary = null;
            this.DataForm.ItemsSource = new List<DataClassWithValidation>() { new DataClassWithValidation() };

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                this.ExpectContentLoaded();
                this.DataForm.AddNewItem();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.DataForm.ValidateItem();
                Assert.AreEqual(1, this.DataForm.ValidationSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that validation occurs properly when the current item changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that validation occurs properly when the current item changes.")]
        public void EnsureValidatesOnCurrentItemChanged()
        {
            PagedCollectionView pcv = new PagedCollectionView(new List<DataClassWithValidation>()
            {
                new DataClassWithValidation(),
                new DataClassWithValidation(),
                new DataClassWithValidation(),
            });

            this.DataForm.CurrentItem = null;
            this.DataForm.ItemsSource = pcv;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.IsItemValid);
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                SetValue(this.textBox, "0");
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);

                pcv.MoveCurrentToNext();

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.AreEqual(0, pcv.CurrentPosition);

                SetValue(this.textBox, "test string 0");

                this.ExpectContentLoaded();
                pcv.MoveCurrentToNext();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.AreEqual(1, pcv.CurrentPosition);

                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                pcv.MoveCurrentToNext();
            });

            this.WaitForContentLoaded();
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensure that metadata streaming works properly.")]
        public void TestMetadataStreaming()
        {
            ValidationSummary validationSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                Assert.IsNotNull(validationSummary);
                Assert.IsTrue(validationSummary.DisplayedErrors.Count == 0);
                DescriptionViewer description = this.DataFormDescriptions[0] as DescriptionViewer;
                Assert.IsNotNull(description);
                Assert.AreEqual("Bool Property Description", description.Description);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that object-level validation functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that object-level validation functions properly.")]
        public void TestObjectLevelValidation()
        {
            ValidationSummary validationSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                Assert.IsTrue(this.DataForm.IsItemValid);
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                SetValue(this.textBox, "1");
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.comboBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[1].Control);
                Assert.AreEqual("IntProperty cannot be equal to StringProperty.", validationSummary.Errors[0].Message);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();
                this.DataForm.ValidateItem();

                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that object-level validation functions properly when external changes are made to the current item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that object-level validation functions properly when external changes are made to the current item.")]
        public void TestObjectLevelValidationFromExternalChanges()
        {
            ValidationSummary validationSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                Assert.IsTrue(this.DataForm.IsItemValid);
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                SetValue(this.textBox, "1");
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.comboBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[1].Control);
                Assert.AreEqual("IntProperty cannot be equal to StringProperty.", validationSummary.Errors[0].Message);

                DataClassWithValidation dataClass = this.DataForm.CurrentItem as DataClassWithValidation;
                dataClass.StringProperty = "test string 1";

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that property-level validation functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that property-level validation functions properly.")]
        public void TestPropertyLevelValidation()
        {
            DataField textField = null;
            ValidationSummary validationSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2];
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2];
                this.GetInputControls();

                SetValue(this.textBox, string.Empty);
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("StringProperty is required.", validationSummary.Errors[0].Message);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                SetValue(this.textBox, "abcdefghijklmnopqrstuvwxyz");
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("StringProperty must be at most 20 characters.", validationSummary.Errors[0].Message);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that property-level validation functions properly with the TextChanged event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that property-level validation functions properly with the TextChanged event.")]
        public void TestPropertyLevelValidationWithTextChanged()
        {
            DataField textField = null;
            ValidationSummary validationSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2];
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2];
                this.GetInputControls();

                SetValue(this.textBox, string.Empty);
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("StringProperty is required.", validationSummary.Errors[0].Message);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, "test string 1");
            });

            this.WaitForTextChanged();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, string.Empty);
            });

            this.WaitForTextChanged();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("StringProperty is required.", validationSummary.Errors[0].Message);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, "test string 1");
            });

            this.WaitForTextChanged();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, string.Empty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that property-level validation functions properly with the TextChanged event inside of direct content.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that property-level validation functions properly with the TextChanged event inside of direct content.")]
        public void TestPropertyLevelValidationWithTextChangedInDirectContent()
        {
            DataFormApp_DirectContent dataFormApp = new DataFormApp_DirectContent();
            dataFormApp.dataForm.CurrentItem = new DataClassWithValidation()
            {
                IntProperty = 1,
                StringProperty = "test string 1"
            };

            this.DataFormAppBase = dataFormApp;
            ContentPresenter contentPresenter = null;
            ValidationSummary validationSummary = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                this.ExpectBeginEdit();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForBeginEdit();

            this.EnqueueCallback(() =>
            {
                this.textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);

                SetValue(this.textBox, string.Empty);
                this.CommitAllFields();

                Assert.IsFalse(dataFormApp.dataForm.IsItemValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
                Assert.AreEqual(DataFormMode.Edit, dataFormApp.dataForm.Mode);
                Assert.AreEqual("StringProperty is required.", validationSummary.Errors[0].Message);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, "test string 2");
            });

            this.WaitForTextChanged();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(dataFormApp.dataForm.IsItemValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                this.textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
                Assert.AreEqual("test string 2", textBox.Text);

                this.ExpectEditEnded();
                dataFormApp.dataForm.CancelEdit();
            });

            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
                Assert.AreEqual(DataFormMode.ReadOnly, dataFormApp.dataForm.Mode);
                Assert.AreEqual("test string 1", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that property-level validation functions properly with the TextChanged event outside of a DataField.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that property-level validation functions properly with the TextChanged event outside of a DataField.")]
        public void TestPropertyLevelValidationWithTextChangedInTemplate()
        {
            DataFormApp_TemplatesWithBinding dataFormApp = new DataFormApp_TemplatesWithBinding();
            dataFormApp.dataForm.CurrentItem = new DataClassWithValidation()
            {
                IntProperty = 1,
                StringProperty = "test string 1"
            };

            this.DataFormAppBase = dataFormApp;
            ContentPresenter contentPresenter = null;
            ValidationSummary validationSummary = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.textBox = contentPresenter.Content as TextBox;

                SetValue(this.textBox, string.Empty);
                this.CommitAllFields();

                Assert.IsFalse(dataFormApp.dataForm.IsItemValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("StringProperty is required.", validationSummary.Errors[0].Message);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, "test string 1");
            });

            this.WaitForTextChanged();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(dataFormApp.dataForm.IsItemValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, string.Empty);
            });

            this.WaitForTextChanged();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(dataFormApp.dataForm.IsItemValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("StringProperty is required.", validationSummary.Errors[0].Message);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, "test string 1");
            });

            this.WaitForTextChanged();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(dataFormApp.dataForm.IsItemValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, string.Empty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that property-level validation functions properly with two errors.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that property-level validation functions properly with two errors.")]
        public void TestPropertyLevelValidationWithTwoErrors()
        {
            DataField textField = null;
            DataField comboBoxField = null;
            ValidationSummary validationSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2];
                comboBoxField = this.DataForm.Fields[4];
                validationSummary = this.GetTemplatePart<ValidationSummary>("ValidationSummary");
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.IsTrue(comboBoxField.IsValid);
                this.DataForm.BeginEdit();
            });

            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2];
                comboBoxField = this.DataForm.Fields[4];
                this.GetInputControls();

                SetValue(this.textBox, string.Empty);
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.IsTrue(comboBoxField.IsValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("StringProperty is required.", validationSummary.Errors[0].Message);

                this.comboBox.SelectedItem = 8;
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.IsFalse(comboBoxField.IsValid);
                Assert.AreEqual(2, validationSummary.Errors.Count);
                Assert.AreEqual(this.textBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("StringProperty is required.", validationSummary.Errors[0].Message);
                Assert.AreEqual(this.comboBox, validationSummary.Errors[1].Sources[0].Control);
                Assert.AreEqual("IntProperty must be between 0 and 7.", validationSummary.Errors[1].Message);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.IsFalse(comboBoxField.IsValid);
                Assert.AreEqual(1, validationSummary.Errors.Count);
                Assert.AreEqual(this.comboBox, validationSummary.Errors[0].Sources[0].Control);
                Assert.AreEqual("IntProperty must be between 0 and 7.", validationSummary.Errors[0].Message);

                this.comboBox.SelectedItem = 0;
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.IsTrue(comboBoxField.IsValid);
                Assert.AreEqual(0, validationSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        #region Helper Methods

        /// <summary>
        /// Expects a text change on the text box.
        /// </summary>
        private void ExpectTextChangedOnTextBox()
        {
            this.textBoxTextChanged = false;
            this.textBox.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);
        }

        /// <summary>
        /// Retrieve the input controls.
        /// </summary>
        private void GetInputControls()
        {
            this.textBox = this.DataFormInputControls[2] as TextBox;
            this.comboBox = this.DataFormInputControls[4] as ComboBox;
        }

        /// <summary>
        /// Handles the case where a text box's text has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.textBoxTextChanged = true;
        }

        /// <summary>
        /// Wait for a text change on a text box.
        /// </summary>
        private void WaitForTextChanged()
        {
            this.EnqueueConditional(() => this.textBoxTextChanged);
        }

        #endregion Helper Methods
    }
}
