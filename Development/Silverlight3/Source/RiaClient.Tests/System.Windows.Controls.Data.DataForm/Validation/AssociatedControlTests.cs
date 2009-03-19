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

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    [Tag("Validation")]
    public class AssociatedControlTests : SilverlightControlTest
    {
        #region Constructors

        [TestMethod]
        [Description("Instantiate a new ac and test all the default values.")]
        public void CreateDerivedInstance()
        {
            TestAssociatedControl ac = new TestAssociatedControl();
            Assert.IsNotNull(ac);
            Assert.IsFalse(ac.Initialized);
            Assert.IsNull(ac.ValidationMetadata);
            Assert.IsNull(ac.PropertyPath);
            Assert.IsNull(ac.Target);
        }

        #endregion Constructors

        #region Attached Properties

        [TestMethod]
        [Description("Get and set the ValidationMetadata attached property.")]
        public void ValidationMetadata()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            Assert.IsNull(AssociatedControl.GetValidationMetadata(page.nameContentControl));
            ValidationMetadata vmd = new ValidationMetadata();
            vmd.Caption = "ValidationMetadata";
            AssociatedControl.SetValidationMetadata(page.nameContentControl, vmd);
            Assert.AreEqual(vmd, AssociatedControl.GetValidationMetadata(page.nameContentControl));
            AssociatedControl.SetValidationMetadata(page.nameContentControl, null);
            Assert.IsNull(AssociatedControl.GetValidationMetadata(page.nameContentControl));
            ExceptionHelper.ExpectArgumentNullException(
                delegate { AssociatedControl.GetValidationMetadata(null); }, 
                "inputControl");
            ExceptionHelper.ExpectArgumentNullException(
                delegate { AssociatedControl.SetValidationMetadata(null, vmd); }, 
                "inputControl");
        }

        #endregion Attached Properties

        #region Dependency Properties

        #region PropertyPath

        [TestMethod]
        [Description("Setting the property path will result in the VMD being loaded.")]
        [Asynchronous]
        public void PropertyPath()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.PropertyPath = "Name";
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(page.Customer, nameAC.DataContext);
                Assert.IsNull(nameAC.Target);
                Assert.IsTrue(nameAC.MetadataLoaded);
                Assert.IsFalse(nameAC.TargetLoaded);
                Assert.IsNotNull(nameAC.ValidationMetadata);
                Assert.AreEqual("Name", nameAC.ValidationMetadata.Caption);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the property path overrides whatever control is set with Target.")]
        [Asynchronous]
        public void PropertyPathOverridesTarget()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.Target = page.emailTextBox; // ignored
            nameAC.PropertyPath = "Name";
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(page.Customer, nameAC.DataContext);
                Assert.IsNotNull(nameAC.Target);
                Assert.IsTrue(nameAC.MetadataLoaded);
                Assert.IsTrue(nameAC.TargetLoaded);
                Assert.IsNotNull(nameAC.ValidationMetadata);
                Assert.AreEqual("Name", nameAC.ValidationMetadata.Caption);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the property path will result in the VMD being loaded.")]
        [Asynchronous]
        public void PropertyPathDifferentDataContext()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.PropertyPath = "Name";
            Customer newCustomer = new Customer();
            newCustomer.Name = "Foo";
            nameAC.DataContext = newCustomer;
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreNotEqual(page.Customer, nameAC.DataContext);
                Assert.AreEqual(newCustomer, nameAC.DataContext);
                Assert.IsNull(nameAC.Target);
                Assert.IsTrue(nameAC.MetadataLoaded);
                Assert.IsFalse(nameAC.TargetLoaded);
                Assert.IsNotNull(nameAC.ValidationMetadata);
                Assert.AreEqual("Name", nameAC.ValidationMetadata.Caption);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Changing the property path after load should update the VMD")]
        [Asynchronous]
        public void PropertyPathUpdates()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.PropertyPath = "Details";
            Order order = new Order();
            order.Details = "Foo";
            order.Date = new DateTime(2008, 6, 1);
            nameAC.DataContext = order;
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.AreNotEqual(page.Customer, nameAC.DataContext);
                Assert.AreEqual(order, nameAC.DataContext);
                Assert.IsNull(nameAC.Target);
                Assert.IsTrue(nameAC.MetadataLoaded);
                Assert.IsFalse(nameAC.TargetLoaded);
                Assert.IsNotNull(nameAC.ValidationMetadata);
                Assert.AreEqual("OrderDetails", nameAC.ValidationMetadata.Caption);

                nameAC.PropertyPath = "Date";
                Assert.AreEqual(order, nameAC.DataContext);
                Assert.IsNull(nameAC.Target);
                Assert.IsTrue(nameAC.MetadataLoaded);
                Assert.IsFalse(nameAC.TargetLoaded);
                Assert.IsNotNull(nameAC.ValidationMetadata);
                Assert.AreEqual("OrderDate", nameAC.ValidationMetadata.Caption);
            });
            EnqueueTestComplete();
        }

        #endregion PropertyPath

        #region Target

        [TestMethod]
        [Description("The target can be updated at runtime, triggering updates to VMD")]
        [Asynchronous]
        public void TargetUpdates()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.Target = page.nameTextBox;
            Assert.IsTrue(nameAC.TargetLoaded);
            Assert.IsNotNull(nameAC.ValidationMetadata);
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(nameAC.MetadataLoaded);
                Assert.IsTrue(nameAC.TargetLoaded);
                Assert.AreEqual(null, nameAC.OnTargetLoadedOldTarget);
                Assert.AreEqual(page.nameTextBox, nameAC.OnTargetLoadedNewTarget);
                Assert.IsNotNull(nameAC.ValidationMetadata);
                Assert.AreEqual("Name", nameAC.ValidationMetadata.Caption);

                nameAC.Reset();
                nameAC.Target = page.emailTextBox;
                Assert.IsTrue(nameAC.MetadataLoaded);
                Assert.IsTrue(nameAC.TargetLoaded);
                Assert.AreEqual(page.nameTextBox, nameAC.OnTargetLoadedOldTarget);
                Assert.AreEqual(page.emailTextBox, nameAC.OnTargetLoadedNewTarget);
                Assert.AreEqual("Email", nameAC.ValidationMetadata.Caption);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("The target can be set to Null, clearning the VMD")]
        [Asynchronous]
        public void TargetNull()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.Target = page.nameTextBox;
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(nameAC.ValidationMetadata);
                nameAC.Target = null;
                Assert.IsNull(nameAC.ValidationMetadata);
                Assert.AreEqual(page.nameTextBox, nameAC.OnTargetLoadedOldTarget);
                Assert.AreEqual(null, nameAC.OnTargetLoadedNewTarget);
            });
            EnqueueTestComplete();
        }

        #endregion Target

        #region IsValid

        [TestMethod]
        [Description("Setting the IsValid property")]
        [Asynchronous]
        public void IsValidReadOnly()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.Target = page.nameTextBox;
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(nameAC);
                Assert.IsTrue(nameAC.IsValid);
                ExceptionHelper.ExpectInvalidOperationException(delegate() { nameAC.SetValue(AssociatedControl.IsValidProperty, false); }, "IsValid cannot be set because the underlying property is ReadOnly.");
            });
            EnqueueTestComplete();
        }

        #endregion IsValid

        #region IsFocused

        [TestMethod]
        [Description("Test the IsFocused DP.")]
        [Asynchronous]
        public void IsFocused()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.PropertyPath = "Name";
            page.nameContentControl.Content = nameAC;
            nameAC.Target = page.nameTextBox;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(nameAC.IsTargetFocused);
                page.nameTextBox.Focus();
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Description("Setting the IsFocused property")]
        [Asynchronous]
        public void IsFocusedReadOnly()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.Target = page.nameTextBox;
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(nameAC.IsTargetFocused);
                ExceptionHelper.ExpectInvalidOperationException(delegate() { nameAC.SetValue(AssociatedControl.IsFocusedProperty, true); }, "IsFocused cannot be set because the underlying property is ReadOnly.");
            });
            EnqueueTestComplete();
        }

        #endregion IsFocused

        #endregion Dependency Properties

        #region Methods

        [TestMethod]
        [Description("Refreshing the metadata calls LoadMetadata.")]
        [Asynchronous]
        public void RefreshMetadata()
        {
            AssociatedControlTestPage page = new AssociatedControlTestPage();
            TestAssociatedControl nameAC = new TestAssociatedControl();
            nameAC.Target = page.nameTextBox;
            page.nameContentControl.Content = nameAC;
            this.TestPanel.Children.Add(page);

            this.EnqueueConditional(() => { return nameAC.Initialized; });
            this.EnqueueCallback(() =>
            {
                nameAC.Reset();
                nameAC.Refresh();
                Assert.IsTrue(nameAC.MetadataLoaded);
            });
            EnqueueTestComplete();
        }

        #endregion Methods

        #region TestAssociatedControl

        private class TestAssociatedControl : AssociatedControl
        {
            public bool MetadataLoaded
            {
                get;
                private set;
            }

            public bool TargetLoaded
            {
                get;
                private set;
            }

            public bool IsTargetFocused
            {
                get;
                private set;
            }

            public FrameworkElement OnTargetLoadedOldTarget
            {
                get;
                private set;
            }

            public FrameworkElement OnTargetLoadedNewTarget
            {
                get;
                private set;
            }

            public void Reset()
            {
                this.MetadataLoaded = false;
                this.TargetLoaded = false;
                this.OnTargetLoadedOldTarget = null;
                this.OnTargetLoadedNewTarget = null;
            }

            protected override void OnMetadataLoaded()
            {
                base.OnMetadataLoaded();
                this.MetadataLoaded = true;
            }

            protected override void OnTargetLoaded(FrameworkElement oldTarget, FrameworkElement newTarget)
            {
                base.OnTargetLoaded(oldTarget, newTarget);
                this.TargetLoaded = true;
                this.OnTargetLoadedOldTarget = oldTarget;
                this.OnTargetLoadedNewTarget = newTarget;
            }

            protected override void OnTargetFocusedChanged()
            {
                this.IsTargetFocused = this.IsFocused;
            }
        }

        #endregion TestAssociatedControl

    }
}
