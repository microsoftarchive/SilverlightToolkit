//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Validation.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
        /// Ensure that entity-level validation functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that entity-level validation functions properly.")]
        public void TestEntityLevelValidation()
        {
            ErrorSummary errorSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                errorSummary = this.GetTemplatePart<ErrorSummary>("ErrorSummary");
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
                Assert.AreEqual(0, errorSummary.Errors.Count);

                this.ExpectItemEditEnded();
                this.DataForm.CommitItemEdit(true /* exitEditingMode */);

                Assert.IsTrue(this.DataForm.IsEditing);
                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.AreEqual(1, errorSummary.Errors.Count);
                Assert.IsNull(errorSummary.Errors[0].Control);
                Assert.AreEqual("IntProperty cannot be equal to StringProperty.", errorSummary.Errors[0].ErrorMessage);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();

                this.ExpectItemEditEnded();
                this.DataForm.CommitItemEdit(true /* exitEditingMode */);
            });

            this.WaitForItemEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsEditing);
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.AreEqual(0, errorSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that field-level validation functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that field-level validation functions properly.")]
        public void TestFieldLevelValidation()
        {
            DataFormTextField textField = null;
            ErrorSummary errorSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2] as DataFormTextField;
                errorSummary = this.GetTemplatePart<ErrorSummary>("ErrorSummary");
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                SetValue(this.textBox, string.Empty);
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.AreEqual(1, errorSummary.Errors.Count);
                Assert.AreEqual(this.textBox, errorSummary.Errors[0].Control);
                Assert.AreEqual("StringProperty is required.", errorSummary.Errors[0].ErrorMessage);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.AreEqual(0, errorSummary.Errors.Count);

                SetValue(this.textBox, "abcdefghijklmnopqrstuvwxyz");
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.AreEqual(1, errorSummary.Errors.Count);
                Assert.AreEqual(this.textBox, errorSummary.Errors[0].Control);
                Assert.AreEqual("StringProperty must be at most 20 characters.", errorSummary.Errors[0].ErrorMessage);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.AreEqual(0, errorSummary.Errors.Count);

                this.ExpectItemEditEnded();
                this.DataForm.CommitItemEdit(true /* exitEditingMode */);
            });

            this.WaitForItemEditEnded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.IsEditing);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that field-level validation functions properly with the TextChanged event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that field-level validation functions properly.")]
        public void TestFieldLevelValidationWithTextChanged()
        {
            DataFormTextField textField = null;
            ErrorSummary errorSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2] as DataFormTextField;
                errorSummary = this.GetTemplatePart<ErrorSummary>("ErrorSummary");
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForItemEditEnded();

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                SetValue(this.textBox, string.Empty);
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.AreEqual(1, errorSummary.Errors.Count);
                Assert.AreEqual(this.textBox, errorSummary.Errors[0].Control);
                Assert.AreEqual("StringProperty is required.", errorSummary.Errors[0].ErrorMessage);

                this.ExpectTextChangedOnTextBox();
                SetValue(this.textBox, "test string 1");
            });

            this.WaitForTextChanged();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.AreEqual(0, errorSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that field-level validation functions properly with two errors.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that field-level validation functions properly with two errors.")]
        public void TestFieldLevelValidationWithTwoErrors()
        {
            DataFormTextField textField = null;
            DataFormComboBoxField comboBoxField = null;
            ErrorSummary errorSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textField = this.DataForm.Fields[2] as DataFormTextField;
                comboBoxField = this.DataForm.Fields[4] as DataFormComboBoxField;
                errorSummary = this.GetTemplatePart<ErrorSummary>("ErrorSummary");
                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.IsTrue(comboBoxField.IsValid);
                this.DataForm.BeginEdit();
            });

            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                this.GetInputControls();

                SetValue(this.textBox, string.Empty);
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.IsTrue(comboBoxField.IsValid);
                Assert.AreEqual(1, errorSummary.Errors.Count);
                Assert.AreEqual(this.textBox, errorSummary.Errors[0].Control);
                Assert.AreEqual("StringProperty is required.", errorSummary.Errors[0].ErrorMessage);

                this.comboBox.SelectedItem = 8;
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsFalse(textField.IsValid);
                Assert.IsFalse(comboBoxField.IsValid);
                Assert.AreEqual(2, errorSummary.Errors.Count);
                Assert.AreEqual(this.textBox, errorSummary.Errors[0].Control);
                Assert.AreEqual("StringProperty is required.", errorSummary.Errors[0].ErrorMessage);
                Assert.AreEqual(this.comboBox, errorSummary.Errors[1].Control);
                Assert.AreEqual("IntProperty must be between 0 and 7.", errorSummary.Errors[1].ErrorMessage);

                SetValue(this.textBox, "test string 1");
                this.CommitAllFields();

                Assert.IsFalse(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.IsFalse(comboBoxField.IsValid);
                Assert.AreEqual(1, errorSummary.Errors.Count);
                Assert.AreEqual(this.comboBox, errorSummary.Errors[0].Control);
                Assert.AreEqual("IntProperty must be between 0 and 7.", errorSummary.Errors[0].ErrorMessage);

                this.comboBox.SelectedItem = 0;
                this.CommitAllFields();

                Assert.IsTrue(this.DataForm.IsItemValid);
                Assert.IsTrue(textField.IsValid);
                Assert.IsTrue(comboBoxField.IsValid);
                Assert.AreEqual(0, errorSummary.Errors.Count);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensure that metadata streaming works properly.")]
        public void TestMetadataStreaming()
        {
            ErrorSummary errorSummary = null;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                errorSummary = this.GetTemplatePart<ErrorSummary>("ErrorSummary");
                Assert.IsNotNull(errorSummary);
                Assert.IsTrue(errorSummary.FilteredErrors.Count == 0);
                DescriptionViewer description = this.DataForm.Descriptions[0] as DescriptionViewer;
                Assert.IsNotNull(description);
                Assert.AreEqual("Bool Property Description", description.Description);
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
            this.textBox = this.DataForm.InputControls[2] as TextBox;
            this.comboBox = this.DataForm.InputControls[4] as ComboBox;
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
