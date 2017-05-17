//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Navigation.cs">
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
    /// Tests <see cref="DataForm"/> navigation.
    /// </summary>
    [TestClass]
    public class DataFormTests_Navigation : DataFormTests_Base
    {
        #region Helper Properties And Fields

        /// <summary>
        /// The entity list being used.
        /// </summary>
        private DataClassList dataClassList;

        /// <summary>
        /// Gets the <see cref="DataForm"/> app.
        /// </summary>
        private DataFormApp_Standard DataFormApp
        {
            get
            {
                return this.DataFormAppBase as DataFormApp_Standard;
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
            this.DataFormAppBase = new DataFormApp_Standard();
            this.dataClassList = DataClassList.GetDataClassList(3, ListOperations.All);
            this.DataForm.ItemsSource = this.dataClassList;
        }

        #endregion Initialization

        /// <summary>
        /// Ensure that no navigation is possible with no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that no navigation is possible with no items.")]
        public void EnsureCannotMoveWithNoItems()
        {
            this.DataForm.ItemsSource = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this.DataForm.CanMoveToFirstItem);
                Assert.IsFalse(this.DataForm.CanMoveToLastItem);
                Assert.IsFalse(this.DataForm.CanMoveToNextItem);
                Assert.IsFalse(this.DataForm.CanMoveToPreviousItem);

                ButtonBase firstItemButton = this.GetTemplatePart<ButtonBase>("FirstItemButton");
                ButtonBase lastItemButton = this.GetTemplatePart<ButtonBase>("LastItemButton");
                ButtonBase nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                ButtonBase previousItemButton = this.GetTemplatePart<ButtonBase>("PreviousItemButton");

                Assert.IsFalse(firstItemButton.IsEnabled);
                Assert.IsFalse(lastItemButton.IsEnabled);
                Assert.IsFalse(nextItemButton.IsEnabled);
                Assert.IsFalse(previousItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the current index cannot be set with a singleton or no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the current index cannot be set with a singleton or no items.")]
        public void EnsureCannotSetCurrentIndexWithSingletonOrNoItems()
        {
            this.DataForm.ItemsSource = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(-1, this.DataForm.CurrentIndex);
                this.DataForm.CurrentIndex = 3;
                Assert.AreEqual(-1, this.DataForm.CurrentIndex);

                this.DataForm.CurrentItem = this.dataClassList[0];

                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                this.DataForm.CurrentIndex = 3;
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that currency can be moved when using direct content.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that currency can be moved when using direct content.")]
        public void MoveCurrencyWithDirectContent()
        {
            DataFormApp_DirectContent dataFormApp = new DataFormApp_DirectContent();
            dataFormApp.dataForm.ItemsSource = this.dataClassList;
            ContentPresenter contentPresenter = null;
            dataFormApp.dataForm.CurrentItem = this.dataClassList[2];
            this.DataFormAppBase = dataFormApp;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                contentPresenter = this.GetTemplatePart<ContentPresenter>("ContentPresenter");
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
                Assert.AreEqual(this.dataClassList[2], textBox.DataContext);

                Assert.IsTrue(dataFormApp.dataForm.CanMoveToFirstItem);
                this.ExpectCurrentItemChange();
                dataFormApp.dataForm.MoveToFirstItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
                Assert.AreEqual(this.dataClassList[0], textBox.DataContext);
                Assert.AreEqual(this.dataClassList[0], dataFormApp.dataForm.CurrentItem);

                Assert.IsFalse(dataFormApp.dataForm.CanMoveToFirstItem);
                Assert.IsTrue(dataFormApp.dataForm.CanMoveToNextItem);
                this.ExpectCurrentItemChange();
                dataFormApp.dataForm.MoveToNextItem();
            });
            
            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = contentPresenter.Content as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual(dataFormApp.dataForm.Content, textBox);
                Assert.AreEqual(this.dataClassList[1], textBox.DataContext);
                Assert.AreEqual(this.dataClassList[1], dataFormApp.dataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that MoveToFirstItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that MoveToFirstItem functions properly.")]
        public void MoveToFirstItem()
        {
            this.DataForm.CurrentItem = this.dataClassList[2];
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.CanMoveToFirstItem);
                this.ExpectCurrentItemChange();
                this.DataForm.MoveToFirstItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[0], this.DataForm.CurrentItem);
                Assert.IsFalse(this.DataForm.CanMoveToFirstItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that you can move to the first item using the current index.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that you can move to the first item using the current index.")]
        public void MoveToFirstItemThroughCurrentIndex()
        {
            this.DataForm.CurrentItem = this.dataClassList[2];
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(2, this.DataForm.CurrentIndex);
                this.ExpectCurrentItemChange();
                this.DataForm.CurrentIndex = 0;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[0], this.DataForm.CurrentItem);
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that MoveToLastItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that MoveToLastItem functions properly.")]
        public void MoveToLastItem()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.CanMoveToLastItem);
                this.ExpectCurrentItemChange();
                this.DataForm.MoveToLastItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[2], this.DataForm.CurrentItem);
                Assert.IsFalse(this.DataForm.CanMoveToLastItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that you can move to the last item using the current index.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that you can move to the last item using the current index.")]
        public void MoveToLastItemThroughCurrentIndex()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                this.ExpectCurrentItemChange();
                this.DataForm.CurrentIndex = 2;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[2], this.DataForm.CurrentItem);
                Assert.AreEqual(2, this.DataForm.CurrentIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that MoveToNextItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that MoveToNextItem functions properly.")]
        public void MoveToNextItem()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.CanMoveToNextItem);
                this.ExpectCurrentItemChange();
                this.DataForm.MoveToNextItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
                Assert.IsTrue(this.DataForm.CanMoveToNextItem);

                this.ExpectCurrentItemChange();
                this.DataForm.MoveToNextItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[2], this.DataForm.CurrentItem);
                Assert.IsFalse(this.DataForm.CanMoveToNextItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that you can move to the next item using the current index.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that you can move to the next item using the current index.")]
        public void MoveToNextItemThroughCurrentIndex()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                this.ExpectCurrentItemChange();
                this.DataForm.CurrentIndex++;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
                Assert.AreEqual(1, this.DataForm.CurrentIndex);
                this.DataForm.CurrentIndex++;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[2], this.DataForm.CurrentItem);
                Assert.AreEqual(2, this.DataForm.CurrentIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that MoveToPreviousItem functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that MoveToPreviousItem functions properly.")]
        public void MoveToPreviousItem()
        {
            this.DataForm.CurrentItem = this.dataClassList[2];
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.DataForm.CanMoveToPreviousItem);
                this.ExpectCurrentItemChange();
                this.DataForm.MoveToPreviousItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
                Assert.IsTrue(this.DataForm.CanMoveToPreviousItem);

                this.ExpectCurrentItemChange();
                this.DataForm.MoveToPreviousItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[0], this.DataForm.CurrentItem);
                Assert.IsFalse(this.DataForm.CanMoveToPreviousItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that you can move to the previous item using the current index.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that you can move to the previous item using the current index.")]
        public void MoveToPreviousItemThroughCurrentIndex()
        {
            this.DataForm.CurrentItem = this.dataClassList[2];
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(2, this.DataForm.CurrentIndex);
                this.ExpectCurrentItemChange();
                this.DataForm.CurrentIndex--;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
                Assert.AreEqual(1, this.DataForm.CurrentIndex);
                this.DataForm.CurrentIndex--;
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[0], this.DataForm.CurrentItem);
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the current index can be set to valid values, but cannot be set to an invalid value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the current index can be set to valid values, but cannot be set to an invalid value.")]
        public void SetCurrentIndex()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                Assert.AreEqual(dataClassList[0], this.DataForm.CurrentItem);
                this.DataForm.CurrentIndex = 4;
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                Assert.AreEqual(dataClassList[0], this.DataForm.CurrentItem);
                this.DataForm.CurrentIndex = -2;
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                Assert.AreEqual(dataClassList[0], this.DataForm.CurrentItem);
                this.DataForm.CurrentIndex = -1;
                Assert.AreEqual(-1, this.DataForm.CurrentIndex);
                Assert.IsNull(this.DataForm.CurrentItem);
                this.DataForm.CurrentIndex = 0;
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                Assert.AreEqual(dataClassList[0], this.DataForm.CurrentItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the first item button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the first item button functions properly.")]
        public void UseFirstItemButton()
        {
            ButtonBase firstItemButton = null;
            this.DataForm.CurrentItem = this.dataClassList[2];
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                firstItemButton = this.GetTemplatePart<ButtonBase>("FirstItemButton");
                Assert.IsTrue(firstItemButton.IsEnabled);

                this.ExpectCurrentItemChange();
                InvokeButton(firstItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[0], this.DataForm.CurrentItem);
                Assert.IsFalse(firstItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the last item button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the last item button functions properly.")]
        public void UseLastItemButton()
        {
            ButtonBase lastItemButton = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                lastItemButton = this.GetTemplatePart<ButtonBase>("LastItemButton");
                Assert.IsTrue(lastItemButton.IsEnabled);

                this.ExpectCurrentItemChange();
                InvokeButton(lastItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[2], this.DataForm.CurrentItem);
                Assert.IsFalse(lastItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the next item button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the next item button functions properly.")]
        public void UseNextItemButton()
        {
            ButtonBase nextItemButton = null;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                nextItemButton = this.GetTemplatePart<ButtonBase>("NextItemButton");
                Assert.IsTrue(nextItemButton.IsEnabled);

                this.ExpectCurrentItemChange();
                InvokeButton(nextItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
                Assert.IsTrue(nextItemButton.IsEnabled);

                this.ExpectCurrentItemChange();
                InvokeButton(nextItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[2], this.DataForm.CurrentItem);
                Assert.IsFalse(nextItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the previous item button functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the previous item button functions properly.")]
        public void UsePreviousItemButton()
        {
            ButtonBase previousItemButton = null;
            this.DataForm.CurrentItem = this.dataClassList[2];
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                previousItemButton = this.GetTemplatePart<ButtonBase>("PreviousItemButton");
                Assert.IsTrue(previousItemButton.IsEnabled);

                this.ExpectCurrentItemChange();
                InvokeButton(previousItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[1], this.DataForm.CurrentItem);
                Assert.IsTrue(previousItemButton.IsEnabled);

                this.ExpectCurrentItemChange();
                InvokeButton(previousItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(this.dataClassList[0], this.DataForm.CurrentItem);
                Assert.IsFalse(previousItemButton.IsEnabled);
            });

            this.EnqueueTestComplete();
        }
    }
}
