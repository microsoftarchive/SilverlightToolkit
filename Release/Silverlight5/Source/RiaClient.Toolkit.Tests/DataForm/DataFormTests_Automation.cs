//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_AutoGeneration.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests the <see cref="DataFormAutomationPeer"/>.
    /// </summary>
    [TestClass]
    public class DataFormTests_Automation : DataFormTests_Base
    {
        #region Helper Fields and Properties

        /// <summary>
        /// Holds the cancel button peer.
        /// </summary>
        private ButtonAutomationPeer _cancelButtonAutomationPeer;

        /// <summary>
        /// Holds the change indicator peer.
        /// </summary>
        private TextBlockAutomationPeer _changeIndicatorAutomationPeer;

        /// <summary>
        /// Holds the commit button peer.
        /// </summary>
        private ButtonAutomationPeer _commitButtonAutomationPeer;

        /// <summary>
        /// Holds the <see cref="DataFormAutomationPeer"/>.
        /// </summary>
        private DataFormAutomationPeer _dataFormAutomationPeer;

        /// <summary>
        /// Holds the delete item button peer.
        /// </summary>
        private ButtonAutomationPeer _deleteItemButtonAutomationPeer;

        /// <summary>
        /// Holds the peers for the form descriptions.
        /// </summary>
        private IList<DescriptionViewerAutomationPeer> _descriptionAutomationPeers;

        /// <summary>
        /// Holds the edit button peer.
        /// </summary>
        private ButtonAutomationPeer _editButtonAutomationPeer;
        
        /// <summary>
        /// Holds the first item button peer.
        /// </summary>
        private ButtonAutomationPeer _firstItemButtonAutomationPeer;

        /// <summary>
        /// Holds the peers for the form contents.
        /// </summary>
        private IList<AutomationPeer> _inputControlAutomationPeers;

        /// <summary>
        /// Holds the peers for the form labels.
        /// </summary>
        private IList<TextBlockAutomationPeer> _labelAutomationPeers;

        /// <summary>
        /// Holds the last item button peer.
        /// </summary>
        private ButtonAutomationPeer _lastItemButtonAutomationPeer;

        /// <summary>
        /// Holds the new item button peer.
        /// </summary>
        private ButtonAutomationPeer _newItemButtonAutomationPeer;

        /// <summary>
        /// Holds the next item button peer.
        /// </summary>
        private ButtonAutomationPeer _nextItemButtonAutomationPeer;

        /// <summary>
        /// Holds the previous item button peer.
        /// </summary>
        private ButtonAutomationPeer _previousItemButtonAutomationPeer;

        /// <summary>
        /// Holds the scroll viewer peer.
        /// </summary>
        private ScrollViewerAutomationPeer _scrollViewerAutomationPeer;

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

        #endregion Helper Fields and Properties

        #region Initialization

        /// <summary>
        /// Initializes the test framework.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.DataFormAppBase = new DataFormApp_Fields();
        }

        #endregion Initialization

        /// <summary>
        /// Ensure that GetClassName() works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that GetClassName() works properly.")]
        public void EnsureClassNameIsCorrect()
        {
            this.EnqueueCallback(() =>
            {
                this.DataForm.CurrentItem = DataClassList.GetDataClassList(1, ListOperations.All)[0];
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                Assert.AreEqual("DataForm", this._dataFormAutomationPeer.GetClassName());
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that all of the child automation peers cen be retrieved properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that all of the child automation peers cen be retrieved properly.")]
        public void GetAllChildAutomationPeers()
        {
            this.EnqueueCallback(() =>
            {
                this.DataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                this.GetChildAutomationPeers();

                Assert.IsNotNull(this._changeIndicatorAutomationPeer);
                Assert.IsNotNull(this._firstItemButtonAutomationPeer);
                Assert.IsNotNull(this._previousItemButtonAutomationPeer);
                Assert.IsNotNull(this._nextItemButtonAutomationPeer);
                Assert.IsNotNull(this._lastItemButtonAutomationPeer);
                Assert.IsNotNull(this._editButtonAutomationPeer);
                Assert.IsNotNull(this._newItemButtonAutomationPeer);
                Assert.IsNotNull(this._deleteItemButtonAutomationPeer);
                Assert.IsNotNull(this._commitButtonAutomationPeer);
                Assert.IsNotNull(this._cancelButtonAutomationPeer);
                Assert.IsNotNull(this._scrollViewerAutomationPeer);

                Assert.AreEqual(7, this._labelAutomationPeers.Count);
                Assert.AreEqual(7, this._inputControlAutomationPeers.Count);
                Assert.AreEqual(7, this._descriptionAutomationPeers.Count);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the DataFormAutomationPeer can be retrieved properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the DataFormAutomationPeer can be retrieved properly.")]
        public void GetDataFormAutomationPeer()
        {
            this.EnqueueCallback(() =>
            {
                this.DataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                Assert.IsNotNull(this._dataFormAutomationPeer);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that all of the add/delete buttons can be invoked properly through their automation peers.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that all of the add/delete buttons can be invoked properly through their automation peers.")]
        public void InvokeAddDeleteButtons()
        {
            IInvokeProvider invokeProvider;

            this.EnqueueCallback(() =>
            {
                this.DataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                this.GetChildAutomationPeers();

                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
                invokeProvider = this._newItemButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.AddNew, this.DataForm.Mode);
                DataClass dataClass = (this.DataForm.CurrentItem as DataClass);
                dataClass.StringProperty = "new string";
                invokeProvider = this._commitButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
                invokeProvider = this._newItemButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.AddNew, this.DataForm.Mode);
                invokeProvider = this._cancelButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
                Assert.AreEqual(4, this.DataForm.ItemsCount);
                invokeProvider = this._deleteItemButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectCurrentItemChange();
                invokeProvider.Invoke();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(3, this.DataForm.ItemsCount);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that all of the edit buttons can be invoked properly through their automation peers.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that all of the edit buttons can be invoked properly through their automation peers.")]
        public void InvokeEditButtons()
        {
            IInvokeProvider invokeProvider;

            this.EnqueueCallback(() =>
            {
                this.DataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                this.GetChildAutomationPeers();

                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
                invokeProvider = this._editButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.Edit, this.DataForm.Mode);
                DataClass dataClass = (this.DataForm.CurrentItem as DataClass);
                dataClass.StringProperty = "new string";
                invokeProvider = this._commitButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
                invokeProvider = this._editButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.Edit, this.DataForm.Mode);
                invokeProvider = this._cancelButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFormMode.ReadOnly, this.DataForm.Mode);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that all of the navigation buttons can be invoked properly through their automation peers.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that all of the navigation buttons can be invoked properly through their automation peers.")]
        public void InvokeNavigationButtons()
        {
            IInvokeProvider invokeProvider;

            this.EnqueueCallback(() =>
            {
                this.DataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                this.GetChildAutomationPeers();

                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                invokeProvider = this._nextItemButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectCurrentItemChange();
                invokeProvider.Invoke();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(1, this.DataForm.CurrentIndex);
                invokeProvider = this._previousItemButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectCurrentItemChange();
                invokeProvider.Invoke();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
                invokeProvider = this._lastItemButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectCurrentItemChange();
                invokeProvider.Invoke();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(2, this.DataForm.CurrentIndex);
                invokeProvider = this._firstItemButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectCurrentItemChange();
                invokeProvider.Invoke();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this.DataForm.CurrentIndex);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting values in the form's content through automation peers works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting values in the form's content through automation peers works properly.")]
        public void SetValuesWithAutomationPeers()
        {
            DataClass dataClass = null;
            IInvokeProvider invokeProvider;

            this.EnqueueCallback(() =>
            {
                dataClass = DataClassList.GetDataClassList(1, ListOperations.All)[0];
                this.DataForm.CurrentItem = dataClass;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                this.GetChildAutomationPeers();

                Assert.AreEqual(true, dataClass.BoolProperty);
                Assert.AreEqual("test string 0", dataClass.StringProperty);
                invokeProvider = this._editButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                this.GetChildAutomationPeers();

                IToggleProvider toggleProvider = (this._inputControlAutomationPeers[0] as CheckBoxAutomationPeer).GetPattern(PatternInterface.Toggle) as IToggleProvider;
                IValueProvider valueProvider = (this._inputControlAutomationPeers[2] as TextBoxAutomationPeer).GetPattern(PatternInterface.Value) as IValueProvider;

                toggleProvider.Toggle();
                valueProvider.SetValue("new string");

                invokeProvider = this._commitButtonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                this.ExpectContentLoaded();
                invokeProvider.Invoke();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(false, dataClass.BoolProperty);
                Assert.AreEqual("new string", dataClass.StringProperty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that IsContentElement() works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that IsContentElement() works properly.")]
        public void TestIsContentElement()
        {
            this.EnqueueCallback(() =>
            {
                this.DataForm.CurrentItem = DataClassList.GetDataClassList(1, ListOperations.All)[0];
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                Assert.IsTrue(this._dataFormAutomationPeer.IsContentElement());
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that IsControlElement() works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that IsControlElement() works properly.")]
        public void TestIsControlElement()
        {
            this.EnqueueCallback(() =>
            {
                this.DataForm.CurrentItem = DataClassList.GetDataClassList(1, ListOperations.All)[0];
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                Assert.IsTrue(this._dataFormAutomationPeer.IsControlElement());
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that IsKeyboardFocusable() works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that IsKeyboardFocusable() works properly.")]
        public void TestIsKeyboardFocusable()
        {
            this.EnqueueCallback(() =>
            {
                this.DataForm.CurrentItem = DataClassList.GetDataClassList(1, ListOperations.All)[0];
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.GetAutomationPeer();
                Assert.IsTrue(this._dataFormAutomationPeer.IsKeyboardFocusable());
                this.DataForm.IsEnabled = false;
                Assert.IsFalse(this._dataFormAutomationPeer.IsKeyboardFocusable());
            });

            this.EnqueueTestComplete();
        }

        #region Helper Methods

        /// <summary>
        /// Gets the <see cref="DataFormAutomationPeer"/> from the <see cref="DataForm"/>.
        /// </summary>
        private void GetAutomationPeer()
        {
            this._dataFormAutomationPeer = DataFormAutomationPeer.CreatePeerForElement(this.DataForm) as DataFormAutomationPeer;
        }

        /// <summary>
        /// Gets the <see cref="AutomationPeer"/>s from the <see cref="DataFormAutomationPeer"/>.
        /// </summary>
        private void GetChildAutomationPeers()
        {
            this._firstItemButtonAutomationPeer = null;
            this._previousItemButtonAutomationPeer = null;
            this._nextItemButtonAutomationPeer = null;
            this._lastItemButtonAutomationPeer = null;
            this._editButtonAutomationPeer = null;
            this._newItemButtonAutomationPeer = null;
            this._deleteItemButtonAutomationPeer = null;
            this._commitButtonAutomationPeer = null;
            this._cancelButtonAutomationPeer = null;

            this._labelAutomationPeers = new List<TextBlockAutomationPeer>();
            this._inputControlAutomationPeers = new List<AutomationPeer>();
            this._descriptionAutomationPeers = new List<DescriptionViewerAutomationPeer>();

            List<AutomationPeer> automationPeers = this._dataFormAutomationPeer.GetChildren();

            foreach (AutomationPeer automationPeer in automationPeers)
            {
                string className = automationPeer.GetClassName();

                if (className == "Button")
                {
                    ButtonAutomationPeer buttonAutomationPeer = automationPeer as ButtonAutomationPeer;
                    Assert.IsNotNull(buttonAutomationPeer);

                    string automationId = automationPeer.GetAutomationId();

                    switch (automationId)
                    {
                        case "FirstItemButton":
                            this._firstItemButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        case "PreviousItemButton":
                            this._previousItemButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        case "NextItemButton":
                            this._nextItemButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        case "LastItemButton":
                            this._lastItemButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        case "EditButton":
                            this._editButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        case "NewItemButton":
                            this._newItemButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        case "DeleteItemButton":
                            this._deleteItemButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        case "CommitButton":
                            this._commitButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        case "CancelButton":
                            this._cancelButtonAutomationPeer = buttonAutomationPeer;
                            break;

                        default:
                            Assert.Fail("Unexpected ButtonAutomationPeer.");
                            break;
                    }
                }
                else if (className == "TextBlock")
                {
                    this._changeIndicatorAutomationPeer = automationPeer as TextBlockAutomationPeer;
                }
                else if (className == "ScrollViewer")
                {
                    this._scrollViewerAutomationPeer = automationPeer as ScrollViewerAutomationPeer;
                    Assert.IsNotNull(this._scrollViewerAutomationPeer);
                    List<AutomationPeer> formAutomationPeers = this._scrollViewerAutomationPeer.GetChildren();

                    for (int i = 0; i < formAutomationPeers.Count; i++)
                    {
                        if (i % 3 == 0)
                        {
                            TextBlockAutomationPeer textBlockAutomationPeer = formAutomationPeers[i] as TextBlockAutomationPeer;

                            if (textBlockAutomationPeer != null)
                            {
                                this._labelAutomationPeers.Add(textBlockAutomationPeer);
                            }
                        }
                        else if (i % 3 == 1)
                        {
                            this._inputControlAutomationPeers.Add(formAutomationPeers[i]);
                        }
                        else
                        {
                            DescriptionViewerAutomationPeer descriptionViewerAutomationPeer = formAutomationPeers[i] as DescriptionViewerAutomationPeer;

                            if (descriptionViewerAutomationPeer != null)
                            {
                                this._descriptionAutomationPeers.Add(descriptionViewerAutomationPeer);
                            }
                        }
                    }
                }
            }
        }

        #endregion Helper Methods
    }
}
