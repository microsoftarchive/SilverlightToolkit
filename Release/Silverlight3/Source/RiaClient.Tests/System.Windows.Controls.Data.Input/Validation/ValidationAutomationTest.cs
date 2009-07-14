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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    [Tag("Validation")]
    public class ValidationAutomationTest : SilverlightControlTest
    {
        [TestMethod]
        [Description("Tests the ValidationSummary AutomationPeer")]
        [Asynchronous]
        public void ValidationSummaryPeer()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                ValidationSummaryAutomationPeer peer = new ValidationSummaryAutomationPeer(vs);
                Assert.IsNotNull(peer);
                Assert.AreEqual(AutomationControlType.Group, peer.GetAutomationControlType());
                Assert.AreEqual("ValidationSummary", peer.GetClassName());
                Assert.AreEqual("0 Errors", peer.GetName());

                vs.Errors.Add(new ValidationSummaryItem("header1", "msg1", ValidationSummaryItemType.PropertyError, null, null));
                Assert.AreEqual("1 Error", peer.GetName());
                vs.Errors.Add(new ValidationSummaryItem("header2", "msg2", ValidationSummaryItemType.PropertyError, null, null));
                Assert.AreEqual("2 Errors", peer.GetName());
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Tests the ValidationSummary Invoke functionality")]
        [Asynchronous]
        public void ValidationSummaryPeerInvoke()
        {
            ValidationSummaryTestPage page = new ValidationSummaryTestPage();
            this.TestPanel.Children.Add(page);
            ValidationSummary vs = page.validationSummary;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                bool clicked = false;
                FocusingInvalidControlEventArgs eArgs = null;
                ValidationSummaryItem vsi = null;
                vs.FocusingInvalidControl += new EventHandler<FocusingInvalidControlEventArgs>(delegate(object o, FocusingInvalidControlEventArgs e)
                {
                    clicked = true;
                    eArgs = e;
                    vsi = e.Item;
                });

                ValidationSummaryAutomationPeer peer = new ValidationSummaryAutomationPeer(vs);
                Assert.IsNotNull(peer);
                ((IInvokeProvider)peer).Invoke();
                Assert.IsFalse(clicked, "No error is selected, so the event should not fire");

                ValidationSummaryItem newEsi = new ValidationSummaryItem(null, "test error", ValidationSummaryItemType.ObjectError, new ValidationSummaryItemSource("property name", page.nameTextBox), this);
                vs.Errors.Add(newEsi);
                vs.ErrorsListBoxInternal.SelectedItem = newEsi;
                ((IInvokeProvider)peer).Invoke();
                Assert.IsTrue(clicked, "Invoking with a selected ESI triggers the event to fire");
                Assert.AreEqual(newEsi, vsi, "The ESI should match the selected item");
                Assert.AreEqual("property name", eArgs.Target.PropertyName, "The source should match the selected item");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Tests the DescriptionViewer AutomationPeer")]
        [Asynchronous]
        public void DescriptionViewerPeer()
        {
            DescriptionViewerTestPage page = new DescriptionViewerTestPage();
            this.TestPanel.Children.Add(page);
            DescriptionViewer dv = page.nameDescriptionViewer;

            this.EnqueueConditional(() => { return page.validationSummary.Initialized; });
            this.EnqueueCallback(() =>
            {
                DescriptionViewerAutomationPeer peer = new DescriptionViewerAutomationPeer(dv);
                Assert.IsNotNull(peer);
                Assert.AreEqual(AutomationControlType.Text, peer.GetAutomationControlType());
                Assert.AreEqual("DescriptionViewer", peer.GetClassName());
                Assert.AreEqual("This is your first name.", peer.GetName());

                dv.Description = "new description";
                Assert.AreEqual(dv.Description, peer.GetName());

            });
            EnqueueTestComplete();
        }
    }
}
