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
    public class DescriptionViewerTests : SilverlightControlTest
    {
        #region Constructors

        [TestMethod]
        [Description("Instantiate a new DescriptionViewer and test all the default values.")]
        public void CreateInstance()
        {
            DescriptionViewer dv = new DescriptionViewer();
            Assert.IsNotNull(dv);
            Assert.IsNull(dv.Description);
            Assert.IsNull(dv.GlyphTemplate);
            Assert.IsNull(dv.ToolTipStyle);
        }

        [TestMethod]
        [Description("Test the constructor when not in XAML.")]
        [Asynchronous]
        public void CreateInstance_NonXAML()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                DescriptionViewer dv = new DescriptionViewer();
                dv.Target = page.nameTextBox;
            });
            EnqueueTestComplete();
        }

        #endregion Constructors

        #region Properties

        [TestMethod]
        [Description("Test the Description dependency property.  Updating the CLR property will set it into override mode.")]
        [Asynchronous]
        public void Description()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("This is your first name.", page.nameDescriptionViewer.Description);
                Assert.IsNotNull(page.nameDescriptionViewer.ToolTipStyle);
                Assert.AreEqual(typeof(ToolTip), page.nameDescriptionViewer.ToolTipStyle.TargetType);
                Assert.AreEqual("Your email address", page.emailDescriptionViewer.Description);

                // Overwrite description
                page.nameDescriptionViewer.Description = "new description";
                Assert.AreEqual("new description", page.nameDescriptionViewer.Description);

                // Refresh everything
                page.nameDescriptionViewer.Refresh();
                Assert.AreEqual("Your email address", page.emailDescriptionViewer.Description);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Test the GlyphTemplate dependency property.")]
        [Asynchronous]
        public void GlyphTemplate()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(page.nameDescriptionViewer.GlyphTemplate);
                ControlTemplate newTemplate = page.LayoutRoot.Resources["testTemplate"] as ControlTemplate;
                page.nameDescriptionViewer.GlyphTemplate = newTemplate;
                Assert.IsNotNull(page.nameDescriptionViewer.GlyphTemplate);
                Assert.AreEqual(newTemplate, page.nameDescriptionViewer.GlyphTemplate);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Test the ToolTipStyle dependency property.")]
        [Asynchronous]
        public void ToolTipStyle()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(page.nameDescriptionViewer.ToolTipStyle);
                Assert.AreEqual(typeof(ToolTip), page.nameDescriptionViewer.ToolTipStyle.TargetType);

                Style newStyle = page.LayoutRoot.Resources["testToopTipStyle"] as Style;
                page.nameDescriptionViewer.ToolTipStyle = newStyle;

                Assert.IsNotNull(page.nameDescriptionViewer.ToolTipStyle);
                Assert.AreEqual(newStyle, page.nameDescriptionViewer.ToolTipStyle);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Test IsEnabled")]
        [Asynchronous]
        public void IsEnabled()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameDescriptionViewer.IsEnabled = true;
                Assert.IsTrue(page.nameDescriptionViewer.IsEnabled);
                page.nameDescriptionViewer.IsEnabled = false;
                Assert.IsFalse(page.nameDescriptionViewer.IsEnabled);
            });
            EnqueueTestComplete();
        }

        #region Target

        [TestMethod]
        [Description("Changing the target results in the content being updated")]
        [Asynchronous]
        public void Target()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("This is your first name.", page.nameDescriptionViewer.Description);
                page.nameDescriptionViewer.Target = page.emailTextBox;
                Assert.AreEqual("Your email address", page.nameDescriptionViewer.Description);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to null results in the content being cleared")]
        [Asynchronous]
        public void Target_Null()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("This is your first name.", page.nameDescriptionViewer.Description);
                page.nameDescriptionViewer.Target = null;
                Assert.IsNull(page.nameDescriptionViewer.Description);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to a non-visual tree element behaves the same as any other element")]
        [Asynchronous]
        public void Target_NonVisualTreeElement()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("This is your first name.", page.nameDescriptionViewer.Description);
                TextBox tb = new TextBox();
                Binding b = new Binding("Email");
                tb.SetBinding(TextBox.TextProperty, b);
                tb.DataContext = page.DataContext;
                page.nameDescriptionViewer.Target = tb;
                Assert.AreEqual("Your email address", page.nameDescriptionViewer.Description);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting target to a non-visual tree element behaves the same as any other element")]
        [Asynchronous]
        public void Target_WhenNotInVisualTree()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                TextBox tb = new TextBox();
                Binding b = new Binding("Email");
                tb.SetBinding(TextBox.TextProperty, b);
                tb.DataContext = page.DataContext;

                DescriptionViewer dv = new DescriptionViewer();
                dv.Target = tb;
                Assert.AreEqual("Your email address", dv.Description);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Checking IsValid updates")]
        [Asynchronous]
        public void Target_ValidChecks()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("This is your first name.", page.nameDescriptionViewer.Description);
                Assert.IsTrue(page.nameDescriptionViewer.IsValid);
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameDescriptionViewer.IsValid);
                page.nameTextBox.Text = "ABC";
                Assert.IsTrue(page.nameDescriptionViewer.IsValid);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing targets should update error subscriptions.")]
        [Asynchronous]
        public void Target_ValidChecksWithTargetChanges()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("This is your first name.", page.nameDescriptionViewer.Description);
                Assert.IsTrue(page.nameDescriptionViewer.IsValid, "initial valid state");
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameDescriptionViewer.IsValid, "invalid data");
                page.nameDescriptionViewer.Target = page.emailTextBox;
                Assert.IsTrue(page.nameDescriptionViewer.IsValid, "switch to valid target");
                page.nameDescriptionViewer.Target = page.nameTextBox;
                Assert.IsFalse(page.nameDescriptionViewer.IsValid, "switch back to invalid target");
                page.nameTextBox.Text = "ABC";
                Assert.IsTrue(page.nameDescriptionViewer.IsValid, "turned valid again");
                page.emailTextBox.Text = "asdfkhask";
                page.nameDescriptionViewer.Target = page.emailTextBox;
                Assert.IsFalse(page.nameDescriptionViewer.IsValid, "switch to second, invalid target");
                page.nameDescriptionViewer.Target = null;
                Assert.IsTrue(page.nameDescriptionViewer.IsValid, "no target");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the target to an unbound control clears out metadata")]
        [Asynchronous]
        public void Target_Unbound()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("This is your first name.", page.nameDescriptionViewer.Description);
                Assert.IsTrue(page.nameDescriptionViewer.IsValid, "initial valid state");
                page.nameTextBox.Text = "ABCDEF!@#1";
                Assert.IsFalse(page.nameDescriptionViewer.IsValid, "invalid data");

                page.nameDescriptionViewer.Target = new TextBox();
                Assert.IsTrue(page.nameDescriptionViewer.IsValid, "no target");
                Assert.IsNull(page.nameDescriptionViewer.Description);
            });
            EnqueueTestComplete();
        }

        #endregion Target

        #endregion Properties

        #region Methods

        [TestMethod]
        [Description("Refreshing the metadata refreshes all overwritten fields and reparses the metadata from the entity")]
        [Asynchronous]
        public void Refresh()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return page.nameDescriptionViewer.Initialized; });
            this.EnqueueCallback(() =>
            {
                page.nameDescriptionViewer.Description = "new description";
                page.nameDescriptionViewer.Refresh();
                Assert.AreEqual("This is your first name.", page.nameDescriptionViewer.Description);
            });
            EnqueueTestComplete();
        }

        #endregion Methods
    }
}
