//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Controls.Test;
using System.Windows.Data;
using System.Windows.Data.Test.Utilities;
using System.Windows.Markup;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    [Tag("Validation")]
    public class ValidationSummaryTests : SilverlightControlTest
    {
        #region Constructors

        [TestMethod]
        [Description("Create a ValidationSummary control")]
        public void CreateInstance()
        {
            ValidationSummary vs = new ValidationSummary();
            Assert.IsNotNull(vs);
        }

        [TestMethod]
        [Description("Create an ValidationSummary in XAML markup.")]
        [Asynchronous]
        public void CreateInXaml()
        {
            object result = XamlReader.Load("<av:ValidationSummary xmlns='http://schemas.microsoft.com/client/2007'" +
            " xmlns:av=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.Input\" />");
            ValidationSummary vs = result as ValidationSummary;
            Assert.IsNotNull(vs);

            this.TestPanel.Children.Add(vs);

            this.EnqueueConditional(() => { return vs.Initialized; });
            this.EnqueueCallback(() =>
            {
                //check default values
                Assert.AreEqual(0, vs.Errors.Count);
                Assert.IsNull(vs.SummaryListBoxStyle);
                Assert.IsNull(vs.Target);
                Assert.AreEqual(ValidationSummaryFilters.All, vs.Filter);
                Assert.AreEqual(0, ((ICollection)vs.DisplayedErrors).Count);
                Assert.IsTrue(vs.FocusControlsOnClick);
                Assert.IsNotNull(vs.HeaderTemplate);
                Assert.IsFalse(vs.HasDisplayedErrors);
                Assert.IsTrue(vs.IsEnabled);
            });

            EnqueueTestComplete();
        }

        #endregion Constructors

        #region Attached Properties

        [TestMethod]
        [Description("Setting ShowErrorsInSummary determines whether errors from the associated control are displayed in the filtered list or not.")]
        [Asynchronous]
        public void ShowErrorsInSummary()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                ExceptionHelper.ExpectArgumentNullException(delegate() { ValidationSummary.GetShowErrorsInSummary(null); }, "inputControl");
                ExceptionHelper.ExpectArgumentNullException(delegate() { ValidationSummary.SetShowErrorsInSummary(null, true); }, "inputControl");
                Assert.IsTrue(ValidationSummary.GetShowErrorsInSummary(page.emailTextBox));
                ValidationSummary.SetShowErrorsInSummary(page.emailTextBox, false);
                Assert.IsFalse(ValidationSummary.GetShowErrorsInSummary(page.emailTextBox));
                page.emailTextBox.Text = "1234";
                ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(page.emailTextBox);
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual(0, ((ICollection)vs.DisplayedErrors).Count);

                // Now set ShowErrorsInSummary to true and the errors will be added
                ValidationSummary.SetShowErrorsInSummary(page.emailTextBox, true);
                Assert.IsTrue(ValidationSummary.GetShowErrorsInSummary(page.emailTextBox));
                //Assert.AreEqual(0, ((ICollection)vs.DisplayedErrors).Count);
                //Assert.AreEqual(errors[0].Exception.Message, vs.DisplayedErrors[0].Message);
            });
            EnqueueTestComplete();
        }

        #endregion Attached Properties

        #region Dependency Properties

        [TestMethod]
        [Description("Setting the ErrorListBoxStyle property")]
        [Asynchronous]
        public void ErrorListBoxStyle()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNull(vs.SummaryListBoxStyle);
                Assert.IsNotNull(vs.ErrorsListBoxInternal);
                Style newListBoxStyle = page.LayoutRoot.Resources["testListBoxStyle"] as Style;
                Assert.IsNotNull(newListBoxStyle);
                vs.SummaryListBoxStyle = newListBoxStyle;
                Assert.AreEqual(newListBoxStyle, vs.ErrorsListBoxInternal.Style);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the Target property")]
        [Asynchronous]
        public void Target()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(page.mainGrid, vs.Target);
                Assert.AreEqual(0, vs.Errors.Count);
                vs.Target = page.emailTextBox;
                page.emailTextBox.Text = "abcd";
                Assert.AreEqual(1, vs.Errors.Count);
                Assert.AreEqual("Email", vs.Errors[0].Sources[0].PropertyName);
                page.nameTextBox.Text = "ABCD!@#$";
                Assert.AreEqual(1, vs.Errors.Count);

                vs.Target = page.nameTextBox;
                page.nameTextBox.Text = "ABCD!@#$%";
                Assert.AreEqual(1, vs.Errors.Count);
                Assert.AreEqual("Name", vs.Errors[0].Sources[0].PropertyName);

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Not setting the Target in XAML will no longer set the parent as the default")]
        [Asynchronous]
        public void TargetDefaultsToNull()
        {
            object result = XamlReader.Load("<av:ValidationSummary xmlns='http://schemas.microsoft.com/client/2007'" +
            " xmlns:av=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.Input\" />");
            ValidationSummary vs = result as ValidationSummary;
            Assert.IsNotNull(vs);

            this.TestPanel.Children.Add(vs);
            this.EnqueueConditional(() => { return vs.Initialized; });
            this.EnqueueCallback(() =>
            {
                //check default values
                Assert.IsNull(vs.Target);
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the ErrorStyle property")]
        [Asynchronous]
        public void ErrorStyle()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs.ErrorsListBoxInternal);
                Assert.IsNotNull(vs.ErrorStyle);
                Style testErrorStyle = page.LayoutRoot.Resources["testErrorStyle"] as Style;
                vs.ErrorStyle = testErrorStyle;
                Assert.AreEqual(testErrorStyle, vs.ErrorStyle);
                Assert.AreEqual(testErrorStyle, vs.ErrorsListBoxInternal.ItemContainerStyle);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the Filter property")]
        [Asynchronous]
        public void Filter()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs);
                ICollection filteredErrors = vs.DisplayedErrors;
                Assert.IsNotNull(filteredErrors);
                Assert.AreEqual(0, filteredErrors.Count);
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, filteredErrors.Count);
                page.emailTextBox.Text = "abcd";
                Assert.AreEqual(2, filteredErrors.Count);

                vs.Errors.Add(new ValidationSummaryItem(null, "custom1", ValidationSummaryItemType.ObjectError, null, null));
                Assert.AreEqual(3, filteredErrors.Count);
                vs.Errors.Add(new ValidationSummaryItem(null, "custom2", ValidationSummaryItemType.ObjectError, null, null));
                Assert.AreEqual(4, filteredErrors.Count);

                vs.Filter = ValidationSummaryFilters.ObjectErrors;
                Assert.AreEqual(2, filteredErrors.Count);

                vs.Filter = ValidationSummaryFilters.PropertyErrors;
                Assert.AreEqual(2, filteredErrors.Count);
                vs.Filter = ValidationSummaryFilters.None;
                Assert.AreEqual(0, filteredErrors.Count);
                vs.Filter = ValidationSummaryFilters.ObjectErrors | ValidationSummaryFilters.PropertyErrors;
                Assert.AreEqual(4, filteredErrors.Count, "or'ing entity and property");
                vs.Filter = ValidationSummaryFilters.All;
                Assert.AreEqual(4, filteredErrors.Count, "all");

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the FocusErrorsOnClick property")]
        [Asynchronous]
        public void FocusErrorsOnClick()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs);
                Assert.IsTrue(vs.FocusControlsOnClick);
                vs.FocusControlsOnClick = false;
                Assert.IsFalse(vs.FocusControlsOnClick);

                // 
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the HasErrors property")]
        [Asynchronous]
        public void HasErrors()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs, "ValidationSummary is null");
                Assert.IsFalse(vs.HasDisplayedErrors, "ValidationSummary should have no errors");
                vs.HasDisplayedErrors = true;
                Assert.IsTrue(vs.HasDisplayedErrors, "HasErrors should now be true");
                vs.HasDisplayedErrors = false;
                Assert.IsFalse(vs.HasDisplayedErrors, "HasErrors set to false");

                page.nameTextBox.Text = "ABCD!@#$";
                page.nameTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            });
            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(vs.HasDisplayedErrors, "HasErrors set to true from error");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the HasErrors property")]
        [Asynchronous]
        public void HasErrorsReadOnly()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs);
                Assert.IsFalse(vs.HasErrors);
                string ExpectedExceptionMessage = String.Format(System.Windows.Controls.Data.Input.Resources.UnderlyingPropertyIsReadOnly, "HasErrors");
                ExceptionHelper.ExpectInvalidOperationException(delegate() { vs.SetValue(ValidationSummary.HasErrorsProperty, true); }, ExpectedExceptionMessage);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the HasDisplayedErrors property")]
        [Asynchronous]
        public void HasDisplayedErrors()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs, "ValidationSummary is null");
                Assert.IsFalse(vs.HasDisplayedErrors, "ValidationSummary should have no errors");
                vs.HasDisplayedErrors = true;
                Assert.IsTrue(vs.HasDisplayedErrors, "HasDisplayedErrors should now be true");
                vs.HasDisplayedErrors = false;
                Assert.IsFalse(vs.HasDisplayedErrors, "HasDisplayedErrors set to false");

                page.nameTextBox.Text = "ABCD!@#$";
                page.nameTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            });
            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(vs.HasDisplayedErrors, "HasDisplayedErrors set to true from error");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the HasDisplayedErrors property")]
        [Asynchronous]
        public void HasDisplayedErrorsReadOnly()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs);
                Assert.IsFalse(vs.HasDisplayedErrors);
                string ExpectedExceptionMessage = String.Format(System.Windows.Controls.Data.Input.Resources.UnderlyingPropertyIsReadOnly, "HasDisplayedErrors");
                ExceptionHelper.ExpectInvalidOperationException(delegate() { vs.SetValue(ValidationSummary.HasDisplayedErrorsProperty, true); }, ExpectedExceptionMessage);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the Header property")]
        [Asynchronous]
        public void Header()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(null, vs.Header);
                vs.Header = "newheader";
                Assert.AreEqual("newheader", vs.Header);
                Assert.AreEqual(vs.Header, vs.HeaderContentControlInternal.Content);
                vs.Header = null;
                Assert.IsNull(vs.Header);
                Assert.AreEqual("0 Errors", vs.HeaderContentControlInternal.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the HeaderTemplate property")]
        [Asynchronous]
        public void HeaderTemplate()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs);
                Assert.IsNotNull(vs.HeaderTemplate);
                DataTemplate testDataTemplate = page.LayoutRoot.Resources["testDataTemplate"] as DataTemplate;
                Assert.IsNotNull(testDataTemplate);
                vs.HeaderTemplate = testDataTemplate;
                Assert.AreEqual(testDataTemplate, vs.HeaderTemplate);
                Assert.AreEqual(testDataTemplate, vs.HeaderContentControlInternal.ContentTemplate);
            });
            EnqueueTestComplete();
        }

        #endregion Dependency Properties

        #region Errors

        [TestMethod]
        [Description("Test updates to the ValidationSummary.Errors collection in response to validation errors in the UI controls.")]
        [Asynchronous]
        public void ErrorUpdates()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs);
                BindingExpression be = page.nameTextBox.GetBindingExpression(TextBox.TextProperty);
                Assert.IsNotNull(be);

                // Cause a validation error via the input control
                page.nameTextBox.Text = "ABCDEFG!@#$";
                ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(page.nameTextBox);
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual(1, vs.Errors.Count);
                Assert.AreEqual(errors[0].Exception.Message, vs.Errors[0].Message);
                Assert.IsTrue(vs.HasErrors);

                // Fix the error
                page.nameTextBox.Text = "abcd";
                errors = Validation.GetErrors(page.nameTextBox);
                Assert.AreEqual(0, errors.Count);
                Assert.AreEqual(0, vs.Errors.Count);
                Assert.IsFalse(vs.HasErrors);

                // Check ValidationErrorCollection
                page.nameTextBox.Text = "ABCDEFG!@#$";
                ValidationSummaryItem newError = new ValidationSummaryItem(null, "new error", ValidationSummaryItemType.ObjectError, null, null);
                ValidationItemCollection errorCollection = vs.Errors as ValidationItemCollection;

                System.Collections.Specialized.NotifyCollectionChangedEventHandler handler =
                    new System.Collections.Specialized.NotifyCollectionChangedEventHandler(
                        delegate(object o, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                        {
                            Assert.AreEqual(newError, e.NewItems[0], "new error does not match");
                        });

                errorCollection.CollectionChanged += handler;
                Assert.AreEqual(1, errorCollection.Count);
                errorCollection.Add(newError);
                errorCollection.CollectionChanged -= handler;
                Assert.AreEqual(2, errorCollection.Count);
                errorCollection.ClearErrors(ValidationSummaryItemType.ObjectError);
                Assert.AreEqual(1, errorCollection.Count);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Test sorting of ErrorInfos")]
        public void ErrorInfoSorting()
        {
            ValidationSummaryItem ei1 = new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null);
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();

            // 1. Compare ErrorInfo reference
            Assert.AreEqual<int>(0, ValidationSummary.CompareValidationSummaryItems(null, null), "1. null null");
            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(null, ei1), "1. null ei1");
            Assert.AreEqual<int>(1, ValidationSummary.CompareValidationSummaryItems(ei1, null), "1. ei1 null");

            // 2. Compare ErrorType
            Assert.AreEqual<int>(0, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null)),
                "2. None None");

            Assert.AreEqual<int>(0, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null)),
                "2. EntityError EntityError");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.PropertyError, null, null)),
                "2. EntityError PropertyError");

            Assert.AreEqual<int>(1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.PropertyError, null, null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null)),
                "2. PropertyError EntityError");

            // 3. Compare Control
            Assert.AreEqual<int>(0, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, null, null)),
                "3. null null");

            Assert.AreEqual<int>(0, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "3. tb1 tb1");

            Assert.AreEqual<int>(1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, null), null)),
                "3. tb1 null");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, null), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "3. null tb1");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb2), null)),
                "3. tb1 tb2");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb2), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb4), null)),
                "3. tb2 tb4 increasing tabindex");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb6), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb2), null)),
                "3. tb6 tb2 decreasing tabindex");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb3), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb5), null)),
                "3. tb3 tb5 increasing tabindex");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb3), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb7), null)),
                "3. tb3 tb7 Name compare");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb7), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb6), null)),
                "3. tb7 tb6 ordering compare");

            TextBox tbNoName = new TextBox();
            tbNoName.TabIndex = 2;
            Assert.AreEqual<int>(1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb7), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, tbNoName), null)),
                "3. tb7 tbNoName Name compare");

            // 4. MessageHeader compare
            Assert.AreEqual<int>(0, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "4. null null");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem("", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "4. null ''");

            Assert.AreEqual<int>(1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem("blah", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "4. blah null");

            // 5. ErrorMessage
            Assert.AreEqual<int>(0, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "5. null null");

            Assert.AreEqual<int>(1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem("a", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "5. a null");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem(null, null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem("a", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "5. null a");

            Assert.AreEqual<int>(1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem("b", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem("a", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "5. b a");

            Assert.AreEqual<int>(-1, ValidationSummary.CompareValidationSummaryItems(
                new ValidationSummaryItem("a", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null),
                new ValidationSummaryItem("b", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource(null, page.tb1), null)),
                "5. a b");

        }

        #endregion Errors

        #region DisplayedErrors

        [TestMethod]
        [Description("When ValidaitonSummaryItem.ItemType changes, the ValidationSummary should refilter.")]
        [Asynchronous]
        public void DisplayedErrors_ItemTypeUpdates()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(vs);
                BindingExpression be = page.nameTextBox.GetBindingExpression(TextBox.TextProperty);
                Assert.IsNotNull(be);

                // Cause a validation error via the input control
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, vs.DisplayedErrors.Count);
                Assert.IsTrue(vs.HasErrors);
                Assert.IsTrue(vs.HasDisplayedErrors);

                // Add object error
                ValidationSummaryItem newError = new ValidationSummaryItem(null, "new error", ValidationSummaryItemType.ObjectError, null, null);
                vs.Errors.Add(newError);
                Assert.AreEqual(2, vs.DisplayedErrors.Count);
                Assert.IsTrue(vs.HasErrors);
                Assert.IsTrue(vs.HasDisplayedErrors);

                // Change filter
                vs.Filter = ValidationSummaryFilters.ObjectErrors;
                Assert.AreEqual(1, vs.DisplayedErrors.Count);
                Assert.IsTrue(vs.HasErrors);
                Assert.IsTrue(vs.HasDisplayedErrors);

                // Change ItemType so that there's no object errors
                newError.ItemType = ValidationSummaryItemType.PropertyError;
                Assert.AreEqual(0, vs.DisplayedErrors.Count);
                Assert.IsTrue(vs.HasErrors);
                Assert.IsFalse(vs.HasDisplayedErrors);
            });
            EnqueueTestComplete();
        }

        #endregion DisplayedErrors

        #region Methods

        [TestMethod]
        [Description("Clicking on errors results in the ErrorClicked event")]
        [Asynchronous]
        public void FocusingInvalidControl()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, vs.Errors.Count);
                bool clicked = false;

                // Setup the delegate to capture the event
                vs.ErrorsListBoxInternal.SelectedIndex = 0;

                FocusingInvalidControlEventArgs eArgs = null;
                ValidationSummaryItem vsi = null;
                vs.FocusingInvalidControl += new EventHandler<FocusingInvalidControlEventArgs>(delegate(object o, FocusingInvalidControlEventArgs e)
                {
                    clicked = true;
                    eArgs = e;
                    vsi = e.Item;
                });

                // Simulate a click on the first item
                vs.ExecuteClickInternal();
                Assert.IsTrue(clicked);
                Assert.IsNotNull(eArgs);
                Assert.IsNotNull(vsi);
                Assert.AreEqual("Name", vsi.Sources[0].PropertyName);

                // Set the flag to false, clicks should no longer occur, as it only affects focus
                vs.FocusControlsOnClick = false;
                clicked = false;
                eArgs = null;
                vs.ExecuteClickInternal();
                Assert.IsFalse(clicked);
                Assert.IsNull(eArgs);

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Clicking on errors results in the ErrorClicked event on the selected item")]
        [Asynchronous]
        public void ErrorClickedSelectedItem()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, vs.Errors.Count);

                ValidationSummaryItem newVsi = new ValidationSummaryItem("test error", null, ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource("property name", page.nameTextBox), this);
                vs.Errors.Add(newVsi);

                bool clicked = false;

                // Setup the delegate to capture the event
                vs.ErrorsListBoxInternal.SelectedItem = newVsi;
                FocusingInvalidControlEventArgs eArgs = null;
                ValidationSummaryItem vsi = null;
                vs.FocusingInvalidControl += new EventHandler<FocusingInvalidControlEventArgs>(delegate(object o, FocusingInvalidControlEventArgs e)
                {
                    clicked = true;
                    eArgs = e;
                    vsi = e.Item;
                });

                // Simulate a click on the first item
                vs.ExecuteClickInternal();
                Assert.IsTrue(clicked);
                Assert.IsNotNull(vsi);
                Assert.AreEqual("test error", vsi.Message);
                Assert.AreEqual(ValidationSummaryItemType.ObjectError, vsi.ItemType);
                Assert.AreEqual(this, vsi.Context);
                Assert.AreEqual(page.nameTextBox, vsi.Sources[0].Control);
                Assert.AreEqual("property name", vsi.Sources[0].PropertyName);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Clicking on errors results in the ErrorClicked event on the selected item")]
        [Asynchronous]
        public void FocusCycling()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, vs.Errors.Count);

                ValidationSummaryItem newVsi = new ValidationSummaryItem(null, "test error", ValidationSummaryItemType.ObjectError, null, null);

                ValidationSummaryItemSource source1 = new ValidationSummaryItemSource("prop1", page.nameTextBox);
                newVsi.Sources.Add(source1);
                ValidationSummaryItemSource source2 = new ValidationSummaryItemSource("prop2", page.emailTextBox);
                newVsi.Sources.Add(source2);

                vs.Errors.Add(newVsi);
                ValidationSummaryItem newVsi2 = new ValidationSummaryItem(null, "test error", ValidationSummaryItemType.ObjectError, null, null);
                vs.Errors.Add(newVsi2);

                // Setup the delegate to capture the event
                vs.ErrorsListBoxInternal.SelectedItem = newVsi;
                ValidationSummaryItemSource selectedSource = null;
                vs.FocusingInvalidControl += new EventHandler<FocusingInvalidControlEventArgs>(delegate(object o, FocusingInvalidControlEventArgs e)
                {
                    selectedSource = e.Target;
                });

                // First click
                vs.ExecuteClickInternal(); 
                Assert.AreEqual(source1, selectedSource);

                // Second
                vs.ExecuteClickInternal();
                Assert.AreEqual(source2, selectedSource);

                // Cycle back
                vs.ExecuteClickInternal();
                Assert.AreEqual(source1, selectedSource);

                // Change the ESI
                vs.ErrorsListBoxInternal.SelectedItem = newVsi2;

                // First click with new ESI
                vs.ExecuteClickInternal();
                Assert.AreEqual(null, selectedSource);

            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing the selected error")]
        [Asynchronous]
        public void SelectionChanged()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, vs.Errors.Count);

                ValidationSummaryItem newEsi = new ValidationSummaryItem(null, "test error", ValidationSummaryItemType.ObjectError, null, null);

                ValidationSummaryItemSource source1 = new ValidationSummaryItemSource("prop1", page.nameTextBox);
                newEsi.Sources.Add(source1);
                ValidationSummaryItemSource source2 = new ValidationSummaryItemSource("prop2", page.emailTextBox);
                newEsi.Sources.Add(source2);

                vs.Errors.Add(newEsi);
                ValidationSummaryItem newEsi2 = new ValidationSummaryItem(null, "test error", ValidationSummaryItemType.ObjectError, null, null);
                vs.Errors.Add(newEsi2);

                // Setup the delegate to capture the event
                bool selectionChanged = false;
                vs.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(delegate(object o, SelectionChangedEventArgs e)
                {
                    selectionChanged = true;
                });

                vs.ErrorsListBoxInternal.SelectedItem = newEsi;
                vs.ErrorsListBoxInternal.SelectedItem = newEsi2;
                // First click
                vs.ExecuteClickInternal();
                Assert.IsTrue(selectionChanged);
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Cause validation errors which should be reflected in the ValidationSummary")]
        [Asynchronous]
        public void GetHeaderString()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                // Add errors and see the header string update
                Assert.AreEqual("0 Errors", vs.GetHeaderString());
                vs.Errors.Add(new ValidationSummaryItem(null, "", ValidationSummaryItemType.PropertyError, null, null));
                Assert.AreEqual("1 Error", vs.GetHeaderString());
                vs.Errors.Add(new ValidationSummaryItem(null, "", ValidationSummaryItemType.PropertyError, null, null));
                Assert.AreEqual("2 Errors", vs.GetHeaderString());
            });
            EnqueueTestComplete();
        }
   
        [TestMethod]
        [Description("Test sorting ValidationSummaryItems by visual tree order.")]
        [Asynchronous]
        public void SortByVisualTreeOrdering()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, ValidationSummary.SortByVisualTreeOrdering(null, null), "both null");
                Assert.AreEqual(0, ValidationSummary.SortByVisualTreeOrdering(null, page.emailTextBox), "null 1");
                Assert.AreEqual(0, ValidationSummary.SortByVisualTreeOrdering(page.nameTextBox, null), "null 2");
                Assert.AreEqual(-1, ValidationSummary.SortByVisualTreeOrdering(page.nameTextBox, page.emailTextBox), "same level");
                Assert.AreEqual(0, ValidationSummary.SortByVisualTreeOrdering(page.mainPanel, page.mainPanel), "same reference");
                Assert.AreEqual(1, ValidationSummary.SortByVisualTreeOrdering(page.emailTextBox, page.nameTextBox), "inverse");
                Assert.AreEqual(0, ValidationSummary.SortByVisualTreeOrdering(page.emailTextBox, new TextBox()), "not in visual tree");
                Assert.AreEqual(-1, ValidationSummary.SortByVisualTreeOrdering(page.emailTextBox, page.tb1), "nested");
                Assert.AreEqual(1, ValidationSummary.SortByVisualTreeOrdering(page.lastTextBox, page.tb1), "nested after");
                Assert.AreEqual(-1, ValidationSummary.SortByVisualTreeOrdering(page.mainPanel, page.tb1), "parent child");
                Assert.AreEqual(1, ValidationSummary.SortByVisualTreeOrdering(page.tb1, page.mainPanel), "parent child reverse");
            });
            EnqueueTestComplete();
        }

        #endregion Methods
    }
}
