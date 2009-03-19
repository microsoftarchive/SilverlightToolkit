//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Controls.Test;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Data.Test.Utilities;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    [Tag("Validation")]
    public class ErrorSummaryTests : SilverlightControlTest
    {
        #region Constructors

        [TestMethod]
        [Description("Create a ErrorSummary control")]
        public void CreateInstance()
        {
            ErrorSummary es = new ErrorSummary();
            Assert.IsNotNull(es);
        }

        [TestMethod]
        [Description("Create an ErrorSummary in XAML markup.")]
        [Asynchronous]
        public void CreateInXaml()
        {
            object result = XamlReader.Load("<av:ErrorSummary xmlns='http://schemas.microsoft.com/client/2007'" +
            " xmlns:av=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.DataForm\" />");
            ErrorSummary es = result as ErrorSummary;
            Assert.IsNotNull(es);

            this.TestPanel.Children.Add(es);

            this.EnqueueConditional(() => { return es.Initialized; });
            this.EnqueueCallback(() =>
            {
                //check default values
                Assert.AreEqual(0, es.Errors.Count);
                Assert.IsNull(es.ErrorsListBoxStyle);
                Assert.IsNull(es.ErrorsSource);
                Assert.AreEqual(ErrorSummaryFilters.All, es.Filter);
                Assert.AreEqual(0, ((ICollection)es.FilteredErrors).Count);
                Assert.IsTrue(es.FocusControlsOnClick);
                Assert.IsNotNull(es.HeaderTemplate);
                Assert.IsFalse(es.HasErrors);
                Assert.IsTrue(es.IsEnabled);
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
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                ExceptionHelper.ExpectArgumentNullException(delegate() { ErrorSummary.GetShowErrorsInSummary(null); }, "inputControl");
                ExceptionHelper.ExpectArgumentNullException(delegate() { ErrorSummary.SetShowErrorsInSummary(null, true); }, "inputControl");
                Assert.IsTrue(ErrorSummary.GetShowErrorsInSummary(page.emailTextBox));
                ErrorSummary.SetShowErrorsInSummary(page.emailTextBox, false);
                Assert.IsFalse(ErrorSummary.GetShowErrorsInSummary(page.emailTextBox));
                page.emailTextBox.Text = "1234";
                ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(page.emailTextBox);
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual(0, ((ICollection)es.FilteredErrors).Count);

                // Now set ShowErrorsInSummary to true and the errors will be added
                ErrorSummary.SetShowErrorsInSummary(page.emailTextBox, true);
                Assert.IsTrue(ErrorSummary.GetShowErrorsInSummary(page.emailTextBox));
                Assert.AreEqual(1, ((ICollection)es.FilteredErrors).Count);
                Assert.AreEqual(errors[0].Exception.Message, es.FilteredErrors[0].ErrorMessage);
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
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNull(es.ErrorsListBoxStyle);
                Assert.IsNotNull(es.ErrorsListBoxInternal);
                Style newListBoxStyle = page.LayoutRoot.Resources["testListBoxStyle"] as Style;
                Assert.IsNotNull(newListBoxStyle);
                es.ErrorsListBoxStyle = newListBoxStyle;
                Assert.AreEqual(newListBoxStyle, es.ErrorsListBoxInternal.Style);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the ErrorsSource property")]
        [Asynchronous]
        public void ErrorsSource()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(page.mainGrid, es.ErrorsSource);
                Assert.AreEqual(0, es.Errors.Count);
                es.ErrorsSource = page.emailTextBox;
                page.emailTextBox.Text = "abcd";
                Assert.AreEqual(1, es.Errors.Count);
                Assert.AreEqual("Email", es.Errors[0].PropertyName);
                page.nameTextBox.Text = "ABCD!@#$";
                Assert.AreEqual(1, es.Errors.Count);

                es.ErrorsSource = page.nameTextBox;
                page.nameTextBox.Text = "ABCD!@#$%";
                Assert.AreEqual(1, es.Errors.Count);
                Assert.AreEqual("Name", es.Errors[0].PropertyName);

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Not setting the ErrorsSource in XAML will no longer set the parent as the default")]
        [Asynchronous]
        public void ErrorsSourceDefaultsToNull()
        {
            object result = XamlReader.Load("<av:ErrorSummary xmlns='http://schemas.microsoft.com/client/2007'" +
            " xmlns:av=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.DataForm\" />");
            ErrorSummary es = result as ErrorSummary;
            Assert.IsNotNull(es);

            this.TestPanel.Children.Add(es);
            this.EnqueueConditional(() => { return es.Initialized; });
            this.EnqueueCallback(() =>
            {
                //check default values
                Assert.IsNull(es.ErrorsSource);
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the ErrorStyle property")]
        [Asynchronous]
        public void ErrorStyle()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(es.ErrorsListBoxInternal);
                Assert.IsNotNull(es.ErrorStyle);
                Style testErrorStyle = page.LayoutRoot.Resources["testErrorStyle"] as Style;
                es.ErrorStyle = testErrorStyle;
                Assert.AreEqual(testErrorStyle, es.ErrorStyle);
                Assert.AreEqual(testErrorStyle, es.ErrorsListBoxInternal.ItemContainerStyle);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the Filter property")]
        [Asynchronous]
        public void Filter()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(es);
                ICollection filteredErrors = es.FilteredErrors;
                Assert.IsNotNull(filteredErrors);
                Assert.AreEqual(0, filteredErrors.Count);
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, filteredErrors.Count);
                page.emailTextBox.Text = "abcd";
                Assert.AreEqual(2, filteredErrors.Count);

                es.Errors.Add(new ErrorSummaryItem("custom1", ErrorType.EntityError));
                Assert.AreEqual(3, filteredErrors.Count);
                es.Errors.Add(new ErrorSummaryItem("custom2", ErrorType.EntityError));
                Assert.AreEqual(4, filteredErrors.Count);

                es.Filter = ErrorSummaryFilters.EntityErrors;
                Assert.AreEqual(2, filteredErrors.Count);

                es.Filter = ErrorSummaryFilters.PropertyErrors;
                Assert.AreEqual(2, filteredErrors.Count);
                es.Filter = ErrorSummaryFilters.None;
                Assert.AreEqual(0, filteredErrors.Count);
                es.Filter = ErrorSummaryFilters.EntityErrors | ErrorSummaryFilters.PropertyErrors;
                Assert.AreEqual(4, filteredErrors.Count, "or'ing entity and property");
                es.Filter = ErrorSummaryFilters.All;
                Assert.AreEqual(4, filteredErrors.Count, "all");

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the FocusErrorsOnClick property")]
        [Asynchronous]
        public void FocusErrorsOnClick()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(es);
                Assert.IsTrue(es.FocusControlsOnClick);
                es.FocusControlsOnClick = false;
                Assert.IsFalse(es.FocusControlsOnClick);

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the HasErrors property")]
        [Asynchronous]
        public void HasErrors()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(es, "ErrorSummary is null");
                Assert.IsFalse(es.HasErrors, "ErrorSummary should have no errors");
                es.HasErrors = true;
                Assert.IsTrue(es.HasErrors, "HasErrors should now be true");
                es.HasErrors = false;
                Assert.IsFalse(es.HasErrors, "HasErrors set to false");

                page.nameTextBox.Text = "ABCD!@#$";
                page.nameTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            });
            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(es.HasErrors, "HasErrors set to true from error");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the HasErrors property")]
        [Asynchronous]
        public void HasErrorsReadOnly()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(es);
                Assert.IsFalse(es.HasErrors);
                ExceptionHelper.ExpectInvalidOperationException(delegate() { es.SetValue(ErrorSummary.HasErrorsProperty, true); }, "HasErrors cannot be set because the underlying property is ReadOnly.");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the Header property")]
        [Asynchronous]
        public void Header()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(null, es.Header);
                es.Header = "newheader";
                Assert.AreEqual("newheader", es.Header);
                Assert.AreEqual(es.Header, es.HeaderContentControlInternal.Content);
                es.Header = null;
                Assert.IsNull(es.Header);
                Assert.AreEqual("0 Errors", es.HeaderContentControlInternal.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the HeaderTemplate property")]
        [Asynchronous]
        public void HeaderTemplate()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(es);
                Assert.IsNotNull(es.HeaderTemplate);
                DataTemplate testDataTemplate = page.LayoutRoot.Resources["testDataTemplate"] as DataTemplate;
                Assert.IsNotNull(testDataTemplate);
                es.HeaderTemplate = testDataTemplate;
                Assert.AreEqual(testDataTemplate, es.HeaderTemplate);
                Assert.AreEqual(testDataTemplate, es.HeaderContentControlInternal.ContentTemplate);
            });
            EnqueueTestComplete();
        }

        #endregion Dependency Properties

        #region Errors

        [TestMethod]
        [Description("Cause validation errors which should be reflected in the ErrorSummary")]
        [Asynchronous]
        public void ErrorUpdates()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(es);
                BindingExpression be = page.nameTextBox.GetBindingExpression(TextBox.TextProperty);
                Assert.IsNotNull(be);
                page.nameTextBox.Text = "ABCDEFG!@#$";
                ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(page.nameTextBox);
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual(1, es.Errors.Count);
                Assert.AreEqual(errors[0].Exception.Message, es.Errors[0].ErrorMessage);

                page.nameTextBox.Text = "abcd";
                errors = Validation.GetErrors(page.nameTextBox);
                Assert.AreEqual(0, errors.Count);
                Assert.AreEqual(0, es.Errors.Count);

                // Check ValidationErrorCollection
                page.nameTextBox.Text = "ABCDEFG!@#$";
                ErrorSummaryItem newError = new ErrorSummaryItem("new error", ErrorType.EntityError);
                ValidationErrorCollection errorCollection = es.Errors as ValidationErrorCollection;

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
                errorCollection.ClearErrors(ErrorType.EntityError);
                Assert.AreEqual(1, errorCollection.Count);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Test sorting of ErrorInfos")]
        public void ErrorInfoSorting()
        {
            ErrorSummaryItem ei1 = new ErrorSummaryItem(null, ErrorType.EntityError);
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();

            // 1. Compare ErrorInfo reference
            Assert.AreEqual<int>(0, ErrorSummary.CompareErrorSummaryItems(null, null), "1. null null");
            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(null, ei1), "1. null ei1");
            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(ei1, null), "1. ei1 null");

            // 2. Compare ErrorType
            Assert.AreEqual<int>(0, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError),
                new ErrorSummaryItem(null, ErrorType.EntityError)),
                "2. None None");

            Assert.AreEqual<int>(0, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError),
                new ErrorSummaryItem(null, ErrorType.EntityError)),
                "2. EntityError EntityError");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError),
                new ErrorSummaryItem(null, ErrorType.PropertyError)),
                "2. EntityError PropertyError");

            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.PropertyError),
                new ErrorSummaryItem(null, ErrorType.EntityError)),
                "2. PropertyError EntityError");

            // 3. Compare Control
            Assert.AreEqual<int>(0, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError),
                new ErrorSummaryItem(null, ErrorType.EntityError)),
                "3. null null");

            Assert.AreEqual<int>(0, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null)),
                "3. tb1 tb1");

            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, null, null)),
                "3. tb1 null");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, null, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null)),
                "3. null tb1");

            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.sp1, null)),
                "3. tb1 sp1");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.sp1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null)),
                "3. sp1 tb1");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb2, null)),
                "3. tb1 tb2");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb2, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb4, null)),
                "3. tb2 tb4 increasing tabindex");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb6, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb2, null)),
                "3. tb6 tb2 decreasing tabindex");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb3, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb5, null)),
                "3. tb3 tb5 increasing tabindex");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb3, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb7, null)),
                "3. tb3 tb7 Name compare");

            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb7, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb6, null)),
                "3. tb7 tb6 Name compare");

            TextBox tbNoName = new TextBox();
            tbNoName.TabIndex = 2;
            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb7, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, tbNoName, null)),
                "3. tb7 tbNoName Name compare");

            // 4. Field compare
            Assert.AreEqual<int>(0, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null)),
                "4. null null");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, "")),
                "4. null ''");

            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, "blah"),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null)),
                "4. blah null");

            // 5. ErrorMessage
            Assert.AreEqual<int>(0, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null)),
                "5. null null");

            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem("a", ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null)),
                "5. a null");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem(null, ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem("a", ErrorType.EntityError, null, page.tb1, null)),
                "5. null a");

            Assert.AreEqual<int>(1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem("b", ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem("a", ErrorType.EntityError, null, page.tb1, null)),
                "5. b a");

            Assert.AreEqual<int>(-1, ErrorSummary.CompareErrorSummaryItems(
                new ErrorSummaryItem("a", ErrorType.EntityError, null, page.tb1, null),
                new ErrorSummaryItem("b", ErrorType.EntityError, null, page.tb1, null)),
                "5. a b");

        }

        #endregion Errors

        #region Methods

        [TestMethod]
        [Description("Clicking on errors results in the ErrorClicked event")]
        [Asynchronous]
        public void ErrorClicked()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, es.Errors.Count);
                bool clicked = false;

                // Setup the delegate to capture the event
                es.ErrorsListBoxInternal.SelectedIndex = 0;
                ErrorSummaryItemEventArgs esiArgs = null;
                ErrorSummaryItem esi = null;
                es.ErrorClicked += new EventHandler<ErrorSummaryItemEventArgs>(delegate(object o, ErrorSummaryItemEventArgs e)
                {
                    clicked = true;
                    esiArgs = e;
                    esi = e.ErrorSummaryItem;
                });

                // Simulate a click on the first item
                es.ExecuteClickInternal();
                Assert.IsTrue(clicked);
                Assert.IsNotNull(esiArgs);
                Assert.IsNotNull(esi);
                Assert.AreEqual("Name", esi.PropertyName);

                // Set the setting to false, clicks should still occur, as it only affects focus
                es.FocusControlsOnClick = false;
                clicked = false;
                esiArgs = null;
                es.ExecuteClickInternal();
                Assert.IsTrue(clicked);
                Assert.IsNotNull(esiArgs);

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Clicking on errors results in the ErrorClicked event on the selected item")]
        [Asynchronous]
        public void ErrorClickedSelectedItem()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameTextBox.Text = "ABCDEFG!@#$";
                Assert.AreEqual(1, es.Errors.Count);

                ErrorSummaryItem newEsi = new ErrorSummaryItem("test error", ErrorType.EntityError, this, page.nameTextBox, "property name");
                es.Errors.Add(newEsi);

                bool clicked = false;

                // Setup the delegate to capture the event
                es.ErrorsListBoxInternal.SelectedItem = newEsi;
                ErrorSummaryItemEventArgs esiArgs = null;
                ErrorSummaryItem esi = null;
                es.ErrorClicked += new EventHandler<ErrorSummaryItemEventArgs>(delegate(object o, ErrorSummaryItemEventArgs e)
                {
                    clicked = true;
                    esiArgs = e;
                    esi = e.ErrorSummaryItem;
                });

                // Simulate a click on the first item
                es.ExecuteClickInternal();
                Assert.IsTrue(clicked);
                Assert.IsNotNull(esiArgs);
                Assert.IsNotNull(esi);
                Assert.AreEqual("test error", esi.ErrorMessage);
                Assert.AreEqual(ErrorType.EntityError, esi.ErrorType);
                Assert.AreEqual(this, esi.Context);
                Assert.AreEqual(page.nameTextBox, esi.Control);
                Assert.AreEqual("property name", esi.PropertyName);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Cause validation errors which should be reflected in the ErrorSummary")]
        [Asynchronous]
        public void GetHeaderString()
        {
            ErrorSummaryTestPage page = new ErrorSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ErrorSummary es = page.errorSummary;

            this.EnqueueConditional(() => { return page.errorSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                // Add errors and see the header string update
                Assert.AreEqual("0 Errors", es.GetHeaderString());
                es.Errors.Add(new ErrorSummaryItem("", ErrorType.PropertyError));
                Assert.AreEqual("1 Error", es.GetHeaderString());
                es.Errors.Add(new ErrorSummaryItem("", ErrorType.PropertyError));
                Assert.AreEqual("2 Errors", es.GetHeaderString());
            });
            EnqueueTestComplete();
        }

        #endregion Methods
    }
}
