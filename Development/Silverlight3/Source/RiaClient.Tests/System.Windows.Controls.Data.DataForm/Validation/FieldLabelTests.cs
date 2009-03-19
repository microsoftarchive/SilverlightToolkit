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
    public class FieldLabelTests : SilverlightControlTest
    {
        #region Constructors

        [TestMethod]
        [Description("Instantiate a new ICB and test all the default values.")]
        public void CreateInstance()
        {
            FieldLabel fl = new FieldLabel();
            Assert.IsNotNull(fl);
            Assert.IsFalse(fl.Initialized);
            Assert.IsFalse(fl.IsRequired);
            Assert.IsNull(fl.Content);
        }

        #endregion Constructors

        #region Properties

        #region IsRequired

        [TestMethod]
        [Description("Test the IsRequired DP.  Updating the CLR property will set it into override mode.")]
        [Asynchronous]
        public void IsRequired()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(page.nameFieldLabel.IsRequired);
                Assert.IsFalse(page.emailFieldLabel.IsRequired);

                // name should now be overriden
                page.nameFieldLabel.IsRequired = false;
                Assert.IsFalse(page.nameFieldLabel.IsRequired);

                // This causes metadata to be reloaded, but it won't overwrite IsFieldRequired
                page.nameFieldLabel.PropertyPath = "Name";
                Assert.IsFalse(page.nameFieldLabel.IsRequired);

                // Refresh everything
                page.nameFieldLabel.Refresh();
                Assert.IsTrue(page.nameFieldLabel.IsRequired);
            });
            EnqueueTestComplete();
        }

        #endregion IsRequired

        #region Content

        [TestMethod]
        [Description("Testing the Content DP")]
        [Asynchronous]
        public void Content()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                page.nameFieldLabel.Content = "example";
                Assert.AreEqual("example", page.nameFieldLabel.Content);
                page.nameFieldLabel.Refresh();
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Testing the Content DP")]
        [Asynchronous]
        public void Content_Override()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                page.nameFieldLabel.Content = "example";
                Assert.AreEqual("example", page.nameFieldLabel.Content);
                page.nameFieldLabel.Target = page.emailTextBox;
                Assert.AreEqual("example", page.nameFieldLabel.Content);

                page.nameFieldLabel.Refresh();
                Assert.AreEqual("Alternate Email", page.nameFieldLabel.Content);
                page.nameFieldLabel.Target = page.nameTextBox;
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        #endregion Content

        #region ContentTemplate

        [TestMethod]
        [Description("Testing the ContentTemplate DP")]
        [Asynchronous]
        public void ContentTemplate()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                page.nameFieldLabel.Content = "example";
                Assert.IsNull(page.nameFieldLabel.ContentTemplate);
                DataTemplate testContentTemplate = page.Resources["testContentTemplate"] as DataTemplate;
                page.nameFieldLabel.ContentTemplate = testContentTemplate;
                Assert.AreEqual(testContentTemplate, page.nameFieldLabel.ContentTemplate);
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
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                page.nameFieldLabel.Target = page.emailTextBox;
                Assert.AreEqual("Alternate Email", page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to null results in the content being cleared")]
        [Asynchronous]
        public void Target_Null()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                page.nameFieldLabel.Target = null;
                Assert.IsNull(page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to a non-visual tree element behaves the same as any other element")]
        [Asynchronous]
        public void Target_NonVisualTreeElement()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                TextBox tb = new TextBox();
                Binding b = new Binding("Email");
                tb.SetBinding(TextBox.TextProperty, b);
                tb.DataContext = page.DataContext;
                page.nameFieldLabel.Target = tb;
                Assert.AreEqual("Email", page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to a non-visual tree element behaves the same as any other element")]
        [Asynchronous]
        public void Target_WhenNotInVisualTree()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                TextBox tb = new TextBox();
                Binding b = new Binding("Email");
                tb.SetBinding(TextBox.TextProperty, b);
                tb.DataContext = page.DataContext;

                FieldLabel fl = new FieldLabel();
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
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                Assert.IsTrue(page.nameFieldLabel.IsValidInternal);
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameFieldLabel.IsValidInternal);
                page.nameTextBox.Text = "ABC";
                Assert.IsTrue(page.nameFieldLabel.IsValidInternal);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing targets should update error subscriptions.")]
        [Asynchronous]
        public void Target_ValidChecksWithTargetChanges()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                Assert.IsTrue(page.nameFieldLabel.IsValidInternal, "initial valid state");
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameFieldLabel.IsValidInternal, "invalid data");
                page.nameFieldLabel.Target = page.emailTextBox;
                Assert.IsTrue(page.nameFieldLabel.IsValidInternal, "switch to valid target");
                page.nameFieldLabel.Target = page.nameTextBox;
                Assert.IsFalse(page.nameFieldLabel.IsValidInternal, "switch back to invalid target");
                page.nameTextBox.Text = "ABC";
                Assert.IsTrue(page.nameFieldLabel.IsValidInternal, "turned valid again");
                page.emailTextBox.Text = "asdfkhask";
                page.nameFieldLabel.Target = page.emailTextBox;
                Assert.IsFalse(page.nameFieldLabel.IsValidInternal, "switch to second, invalid target");
                page.nameFieldLabel.Target = null;
                Assert.IsTrue(page.nameFieldLabel.IsValidInternal, "no target");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the target to an unbound control clears out metadata")]
        [Asynchronous]
        public void Target_Unbound()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                Assert.IsTrue(page.nameFieldLabel.IsValidInternal, "initial valid state");
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameFieldLabel.IsValidInternal, "invalid data");

                page.nameFieldLabel.Target = new TextBox();
                Assert.IsTrue(page.nameFieldLabel.IsValidInternal, "no target");
                Assert.IsNull(page.nameFieldLabel.Content);
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
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                Assert.IsTrue(page.nameFieldLabel.IsRequired);
                page.nameFieldLabel.PropertyPath = "BirthDate";
                Assert.AreEqual("Birth date", page.nameFieldLabel.Content);
                Assert.IsFalse(page.nameFieldLabel.IsRequired);
                page.nameFieldLabel.PropertyPath = null;
                page.nameFieldLabel.DataContext = new Order();
                Assert.IsNull(page.nameFieldLabel.PropertyPath);
                page.nameFieldLabel.PropertyPath = "Details";
                Assert.AreEqual("OrderDetails", page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("PropertyPath is set when there is no DomainContext.  No exception will be thrown because Metadata is not pulled when there is no entity")]
        [Asynchronous]
        public void PropertyPath_NoDataContext()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameFieldLabel.DataContext = null;
                Assert.IsNull(page.nameFieldLabel.DataContext);
                page.nameFieldLabel.PropertyPath = "blah";
                page.nameFieldLabel.DataContext = page.DataContext;
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing DomainContext.  Test with no target.")]
        [Asynchronous]
        public void PropertyPath_ChangingDataContextWithNoTarget()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                page.nameFieldLabel.PropertyPath = "BirthDate";
                Assert.AreEqual("Birth date", page.nameFieldLabel.Content);
                page.nameFieldLabel.Target = null;
                page.nameFieldLabel.DataContext = new Order();
                Assert.IsFalse(page.nameFieldLabel.IsRequired);
                Assert.IsNull(page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing DomainContext.  Test with target.")]
        [Asynchronous]
        public void PropertyPath_ChangingDataContextWithTarget()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                page.nameFieldLabel.PropertyPath = "BirthDate";
                Assert.AreEqual("Birth date", page.nameFieldLabel.Content);
                page.nameFieldLabel.DataContext = new Order();
                Assert.IsFalse(page.nameFieldLabel.IsRequired);
                Assert.IsNull(page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Clearing the PropertyPath (when there is no Target), will clear out values.")]
        [Asynchronous]
        public void PropertyPath_Clear()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                Assert.IsTrue(page.nameFieldLabel.IsRequired);
                page.nameFieldLabel.PropertyPath = "BirthDate";
                Assert.IsFalse(page.nameFieldLabel.IsRequired);
                page.nameFieldLabel.PropertyPath = "Name";
                Assert.IsTrue(page.nameFieldLabel.IsRequired);
                page.nameFieldLabel.Target = null;
                page.nameFieldLabel.PropertyPath = null;
                Assert.IsFalse(page.nameFieldLabel.IsRequired);
                Assert.IsNull(page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("PropertyPath is used, regardless of Target")]
        [Asynchronous]
        public void PropertyPath_PrecedenceOverTarget()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                page.nameFieldLabel.Target = page.emailTextBox;
                Assert.AreEqual("Alternate Email", page.nameFieldLabel.Content);
                page.nameFieldLabel.PropertyPath = "BirthDate";
                Assert.AreEqual("Birth date", page.nameFieldLabel.Content);
                page.nameFieldLabel.Target = page.nameTextBox;
                Assert.AreEqual("Birth date", page.nameFieldLabel.Content);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("PropertyPath changes should trigger an update of the validation state.")]
        [Asynchronous]
        public void PropertyPath_UpdateValidStateWhenChanged()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);
            Binding b = new Binding();
            b.Path = new PropertyPath("Email");
            b.ValidatesOnExceptions = true;
            b.NotifyOnValidationError = true;
            b.Mode = BindingMode.TwoWay;
            page.emailTextBox.SetBinding(TextBox.TextProperty, b);
            FieldLabel fl = page.emailFieldLabel;

            this.EnqueueConditional(() => { return page.emailFieldLabel.Initialized; });
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
                Assert.IsTrue(fl.IsValid, "IsValid state should be cleared because we are no longer pointing to a target.");
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
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameFieldLabel.IsEnabled = true;
                Assert.IsTrue(page.nameFieldLabel.IsEnabled);
                page.nameFieldLabel.IsEnabled = false;
                Assert.IsFalse(page.nameFieldLabel.IsEnabled);
            });
            EnqueueTestComplete();
        }

        #endregion IsEnabled

        #endregion Properties

        #region Methods

        [TestMethod]
        [Description("Refreshing the metadata refreshes all overwritten fields and reparses the metadata from the entity")]
        [Asynchronous]
        public void Refresh()
        {
            FieldLabelTestPage page = new FieldLabelTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameFieldLabel.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameFieldLabel.Content = "example";
                page.nameFieldLabel.IsRequired = false;

                Assert.AreEqual("example", page.nameFieldLabel.Content);
                Assert.IsFalse(page.nameFieldLabel.IsRequired);
                page.nameFieldLabel.Refresh();
                Assert.AreEqual("Name", page.nameFieldLabel.Content);
                Assert.IsTrue(page.nameFieldLabel.IsRequired);
            });
            EnqueueTestComplete();
        }

        #endregion Methods
    }
}
