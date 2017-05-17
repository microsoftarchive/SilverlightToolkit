//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Editing.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls.Primitives;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests the events on the <see cref="DataForm"/>.
    /// </summary>
    [TestClass]
    public class DataFormTests_Events : DataFormTests_Base
    {
        #region Helper Properties And Fields

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
            this.DataForm.ItemsSource = DataClassList.GetDataClassList(2, ListOperations.All);
        }

        #endregion Initialization

        /// <summary>
        /// Ensure that AddingNewItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that AddingNewItem functions properly.")]
        public void AddingNewItem()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<DataFormAddingNewItemEventArgs>("AddingNewItem");
                this.DataForm.AddNewItem();
            });

            this.WaitForAllEvents();

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that AutoGeneratingField functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that AutoGeneratingField functions properly.")]
        public void AutoGeneratingField()
        {
            DataFormApp_AutoGeneration dataFormApp = new DataFormApp_AutoGeneration();
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(2, ListOperations.All);

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<DataFormAutoGeneratingFieldEventArgs>("AutoGeneratingField");
                dataFormApp.dataForm.AutoGenerateFields = true;
            });

            this.AddToPanelAndWaitForLoad();
            this.WaitForAllEvents();

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that BeginningEdit functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that BeginningEdit functions properly.")]
        public void BeginningEdit()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<System.ComponentModel.CancelEventArgs>("BeginningEdit");
                this.DataForm.BeginEdit();
            });

            this.WaitForAllEvents();

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that ContentLoaded functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that ContentLoaded functions properly.")]
        public void ContentLoaded()
        {
            this.ExpectEvent<DataFormContentLoadEventArgs>("ContentLoaded");
            this.AddToPanelAndWaitForLoad();
            this.WaitForAllEvents();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that ContentLoading functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that ContentLoading functions properly.")]
        public void ContentLoading()
        {
            this.ExpectEvent<DataFormContentLoadEventArgs>("ContentLoading");
            this.AddToPanelAndWaitForLoad();
            this.WaitForAllEvents();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that CurrentItemChanged functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that CurrentItemChanged functions properly.")]
        public void CurrentItemChanged()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<EventArgs>("CurrentItemChanged");
                this.DataForm.MoveToNextItem();
            });

            this.WaitForAllEvents();

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DeletingItemItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DeletingItemItem functions properly.")]
        public void DeletingItem()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<System.ComponentModel.CancelEventArgs>("DeletingItem");
                this.DataForm.DeleteItem();
            });

            this.WaitForAllEvents();

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that EditEnded functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that EditEnded functions properly.")]
        public void EditEnded()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<DataFormEditEndedEventArgs>("EditEnded");
                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);
            });

            this.WaitForAllEvents();
            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<DataFormEditEndedEventArgs>("EditEnded");
                this.DataForm.CancelEdit();
            });

            this.WaitForAllEvents();

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that EditEnding functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that EditEnding functions properly.")]
        public void EditEnding()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<DataFormEditEndingEventArgs>("EditEnding");
                this.ExpectEditEnded();
                this.DataForm.CommitEdit(true /* exitEditingMode */);
            });

            this.WaitForAllEvents();
            this.WaitForEditEnded();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                this.DataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<DataFormEditEndingEventArgs>("EditEnding");
                this.DataForm.CancelEdit();
            });

            this.WaitForAllEvents();

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that ValidatingItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that ValidatingItem functions properly.")]
        public void ValidatingItem()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectEvent<System.ComponentModel.CancelEventArgs>("ValidatingItem");
                this.DataForm.ValidateItem();
            });

            this.WaitForAllEvents();

            this.EnqueueTestComplete();
        }
    }
}
