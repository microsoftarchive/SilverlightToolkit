//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls.Test;
using System.Windows.Data.Test.Utilities;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Data;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    [Tag("Validation")]
    public class LabelTests : SilverlightControlTest
    {
        #region Constructors

        [TestMethod]
        [Description("Instantiate a new ICB and test all the default values.")]
        public void CreateInstance()
        {
            Label fl = new Label();
            Assert.IsNotNull(fl);
            Assert.IsFalse(fl.Initialized);
            Assert.IsFalse(fl.IsRequired);
            Assert.IsNull(fl.Content);
        }

        #endregion Constructors

        #region Properties

        #region IsRequired

        [TestMethod]
        [Description("Test the IsRequired dependency property.  Updating the CLR property will set it into override mode.")]
        [Asynchronous]
        public void IsRequired()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(page.nameLabel.IsRequired);
                Assert.IsFalse(page.emailLabel.IsRequired);

                // name should now be overriden
                page.nameLabel.IsRequired = false;
                Assert.IsFalse(page.nameLabel.IsRequired);

                // This causes metadata to be reloaded, but it won't overwrite IsFieldRequired
                page.nameLabel.PropertyPath = "Name";
                Assert.IsFalse(page.nameLabel.IsRequired);

                // Refresh everything
                page.nameLabel.Refresh();
                Assert.IsTrue(page.nameLabel.IsRequired);
            });
            EnqueueTestComplete();
        }

        #endregion IsRequired

        #region Content

        [TestMethod]
        [Description("Testing the Content dependency property")]
        [Asynchronous]
        public void Content()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                page.nameLabel.Content = "example";
                Assert.AreEqual("example", page.nameLabel.Content);
                page.nameLabel.Refresh();
                Assert.AreEqual("Name", page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Testing the Content dependency property")]
        [Asynchronous]
        public void Content_Override()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                page.nameLabel.Content = "example";
                Assert.AreEqual("example", page.nameLabel.Content);
                page.nameLabel.Target = page.emailTextBox;
                Assert.AreEqual("example", page.nameLabel.Content);

                page.nameLabel.Refresh();
                Assert.AreEqual("Alternate Email", page.nameLabel.Content);
                page.nameLabel.Target = page.nameTextBox;
                Assert.AreEqual("Name", page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        #endregion Content

        #region ContentTemplate

        [TestMethod]
        [Description("Testing the ContentTemplate dependency property")]
        [Asynchronous]
        public void ContentTemplate()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                page.nameLabel.Content = "example";
                Assert.IsNull(page.nameLabel.ContentTemplate);
                DataTemplate testContentTemplate = page.Resources["testContentTemplate"] as DataTemplate;
                page.nameLabel.ContentTemplate = testContentTemplate;
                Assert.AreEqual(testContentTemplate, page.nameLabel.ContentTemplate);
            });
            EnqueueTestComplete();
        }

        #endregion ContentTemplate

        #region Target

        [TestMethod]
        [Description("Changing the target results in the content being updated")]
        [Asynchronous]
        public void Target()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                page.nameLabel.Target = page.emailTextBox;
                Assert.AreEqual("Alternate Email", page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to null results in the content being cleared")]
        [Asynchronous]
        public void Target_Null()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                page.nameLabel.Target = null;
                Assert.IsNull(page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to a non-visual tree element behaves the same as any other element")]
        [Asynchronous]
        public void Target_NonVisualTreeElement()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                TextBox tb = new TextBox();
                Binding b = new Binding("Email");
                tb.SetBinding(TextBox.TextProperty, b);
                tb.DataContext = page.DataContext;
                page.nameLabel.Target = tb;
                Assert.AreEqual("Email", page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to a non-visual tree element behaves the same as any other element")]
        [Asynchronous]
        public void Target_WhenNotInVisualTree()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                TextBox tb = new TextBox();
                Binding b = new Binding("Email");
                tb.SetBinding(TextBox.TextProperty, b);
                tb.DataContext = page.DataContext;

                Label fl = new Label();
                fl.Target = tb;
                Assert.AreEqual("Email", fl.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Checking IsValid updates")]
        [Asynchronous]
        public void Target_ValidChecks()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                Assert.IsTrue(page.nameLabel.IsValid);
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameLabel.IsValid);
                page.nameTextBox.Text = "ABC";
                Assert.IsTrue(page.nameLabel.IsValid);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing targets should update error subscriptions.")]
        [Asynchronous]
        public void Target_ValidChecksWithTargetChanges()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                Assert.IsTrue(page.nameLabel.IsValid, "initial valid state");
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameLabel.IsValid, "invalid data");
                page.nameLabel.Target = page.emailTextBox;
                Assert.IsTrue(page.nameLabel.IsValid, "switch to valid target");
                page.nameLabel.Target = page.nameTextBox;
                Assert.IsFalse(page.nameLabel.IsValid, "switch back to invalid target");
                page.nameTextBox.Text = "ABC";
                Assert.IsTrue(page.nameLabel.IsValid, "turned valid again");
                page.emailTextBox.Text = "asdfkhask";
                page.nameLabel.Target = page.emailTextBox;
                Assert.IsFalse(page.nameLabel.IsValid, "switch to second, invalid target");
                page.nameLabel.Target = null;
                Assert.IsTrue(page.nameLabel.IsValid, "no target");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the target to an unbound control clears out metadata")]
        [Asynchronous]
        public void Target_Unbound()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                Assert.IsTrue(page.nameLabel.IsValid, "initial valid state");
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameLabel.IsValid, "invalid data");

                page.nameLabel.Target = new TextBox();
                Assert.IsTrue(page.nameLabel.IsValid, "no target");
                Assert.IsNull(page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        #endregion Target

        #region PropertyPath

        [TestMethod]
        [Description("PropertyPath is used, regardless of Target")]
        [Asynchronous]
        public void PropertyPath()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                Assert.IsTrue(page.nameLabel.IsRequired);
                page.nameLabel.PropertyPath = "BirthDate";
                Assert.AreEqual("Birth date", page.nameLabel.Content);
                Assert.IsFalse(page.nameLabel.IsRequired);
                page.nameLabel.PropertyPath = null;
                page.nameLabel.DataContext = new Order();
                Assert.IsNull(page.nameLabel.PropertyPath);
                page.nameLabel.PropertyPath = "Details";
                Assert.AreEqual("OrderDetails", page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("PropertyPath is set when there is no DomainContext.  No exception will be thrown because Metadata is not pulled when there is no entity")]
        [Asynchronous]
        public void PropertyPath_NoDataContext()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameLabel.DataContext = null;
                Assert.IsNull(page.nameLabel.DataContext);
                page.nameLabel.PropertyPath = "blah";
                page.nameLabel.DataContext = page.DataContext;
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing DataContext.  Test with no target.")]
        [Asynchronous]
        public void PropertyPath_ChangingDataContextWithNoTarget()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                page.nameLabel.PropertyPath = "BirthDate";
                Assert.AreEqual("Birth date", page.nameLabel.Content);
                page.nameLabel.Target = null;
                page.nameLabel.DataContext = new Order();
                Assert.IsFalse(page.nameLabel.IsRequired);
                Assert.IsNull(page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing DataContext.  Test with target.")]
        [Asynchronous]
        public void PropertyPath_ChangingDataContextWithTarget()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                page.nameLabel.PropertyPath = "BirthDate";
                Assert.AreEqual("Birth date", page.nameLabel.Content);
                page.nameLabel.DataContext = new Order();
                Assert.IsFalse(page.nameLabel.IsRequired);
                Assert.IsNull(page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Clearing the PropertyPath (when there is no Target), will clear out values.")]
        [Asynchronous]
        public void PropertyPath_Clear()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                Assert.IsTrue(page.nameLabel.IsRequired);
                page.nameLabel.PropertyPath = "BirthDate";
                Assert.IsFalse(page.nameLabel.IsRequired);
                page.nameLabel.PropertyPath = "Name";
                Assert.IsTrue(page.nameLabel.IsRequired);
                page.nameLabel.Target = null;
                page.nameLabel.PropertyPath = null;
                Assert.IsFalse(page.nameLabel.IsRequired);
                Assert.IsNull(page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("PropertyPath is used, regardless of Target")]
        [Asynchronous]
        public void PropertyPath_PrecedenceOverTarget()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameLabel.Content);
                page.nameLabel.Target = page.emailTextBox;
                Assert.AreEqual("Alternate Email", page.nameLabel.Content);
                page.nameLabel.PropertyPath = "BirthDate";
                Assert.AreEqual("Birth date", page.nameLabel.Content);
                page.nameLabel.Target = page.nameTextBox;
                Assert.AreEqual("Birth date", page.nameLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("PropertyPath changes should trigger an update of the validation state.")]
        [Asynchronous]
        public void PropertyPath_UpdateValidStateWhenChanged()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);
            Binding b = new Binding();
            b.Path = new PropertyPath("Email");
            b.ValidatesOnExceptions = true;
            b.NotifyOnValidationError = true;
            b.Mode = BindingMode.TwoWay;
            page.emailTextBox.SetBinding(TextBox.TextProperty, b);
            Label fl = page.emailLabel;

            this.EnqueueConditional(() => { return page.emailLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Email", fl.Content);
                Assert.IsTrue(fl.IsValid);
                Assert.IsTrue(fl.IsRequired);
                Assert.AreEqual("Email", fl.Content);
                page.emailTextBox.Text = "asdfasdl;kj#@$";
                page.emailTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Assert.IsFalse(fl.IsValid);

                // Set PropertyPath
                fl.PropertyPath = "BirthDate"; // Not required
                Assert.IsFalse(fl.IsValid, "IsValid state should be unchanged because target is still set.");
                Assert.IsFalse(fl.IsRequired);
                Assert.AreEqual("Birth date", fl.Content);
                Assert.AreEqual(page.emailTextBox, fl.Target);

                fl.PropertyPath = null;
                Assert.IsTrue(fl.IsRequired);
                Assert.AreEqual("Email", fl.Content);
                Assert.AreEqual(page.emailTextBox, fl.Target);
                Assert.IsFalse(fl.IsValid);
            });
            EnqueueTestComplete();
        }

        #endregion PropertyPath

        #region IsEnabled

        [TestMethod]
        [Description("Test IsEnabled")]
        [Asynchronous]
        public void IsEnabled()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameLabel.IsEnabled = true;
                Assert.IsTrue(page.nameLabel.IsEnabled);
                page.nameLabel.IsEnabled = false;
                Assert.IsFalse(page.nameLabel.IsEnabled);
            });
            EnqueueTestComplete();
        }

        #endregion IsEnabled

        #region IsValid

        [TestMethod]
        [Description("Setting the IsValid property")]
        [Asynchronous]
        public void IsValidReadOnly()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(page.nameLabel);
                Assert.IsTrue(page.nameLabel.IsValid);
                string ExpectedExceptionMessage = String.Format(System.Windows.Controls.Data.Input.Resources.UnderlyingPropertyIsReadOnly, "IsValid");
                ExceptionHelper.ExpectInvalidOperationException(delegate() { page.nameLabel.SetValue(Label.IsValidProperty, false); }, ExpectedExceptionMessage);
            });
            EnqueueTestComplete();
        }

        #endregion IsValid

        #endregion Properties

        #region Methods

        [TestMethod]
        [Description("Refreshing the metadata refreshes all overwritten fields and reparses the metadata from the entity")]
        [Asynchronous]
        public void Refresh()
        {
            LabelTestPage page = new LabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameLabel.Content = "example";
                page.nameLabel.IsRequired = false;

                Assert.AreEqual("example", page.nameLabel.Content);
                Assert.IsFalse(page.nameLabel.IsRequired);
                page.nameLabel.Refresh();
                Assert.AreEqual("Name", page.nameLabel.Content);
                Assert.IsTrue(page.nameLabel.IsRequired);
            });
            EnqueueTestComplete();
        }

        #endregion Methods
    }
}
