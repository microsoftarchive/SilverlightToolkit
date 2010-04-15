//-----------------------------------------------------------------------
// <copyright file="PagedCollectionViewTest.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows.Controls.Test;
    using System.Windows.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class will contain the helper functions that we will use
    /// for all the PagedCollectionView tests
    /// </summary>
    public class PagedCollectionViewTest : SilverlightControlTest
    {
        #region Variables

        protected List<EventNotification> _expectedEventQueue = new List<EventNotification>();
        protected bool _collectionChangeFired;
        protected bool _currentChangingFired;
        protected bool _currentChangedFired;
        protected bool _propertyChangedFired;
        protected bool _propertyChangedTracked;

        #endregion Variables

        #region Initialization and Cleanup Methods

        /// <summary>
        /// Initializes the PagedCollectionView to be used in testing.
        /// </summary>
        [TestInitialize]
        public virtual void Initialize()
        {
            CollectionView = new PagedCollectionView(SourceCollection);

            // clear expectedEventQueue
            _expectedEventQueue.Clear();
            // property changes are not tracked by default
            _propertyChangedTracked = false;

            // hook up events
            CollectionView.CurrentChanging += new CurrentChangingEventHandler(CollectionView_CurrentChanging);
            CollectionView.CurrentChanged += new EventHandler(CollectionView_CurrentChanged);
            CollectionView.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionView_CollectionChanged);
            CollectionView.PropertyChanged += new PropertyChangedEventHandler(CollectionView_PropertyChanged);
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            // unhook events
            CollectionView.CurrentChanging -= new CurrentChangingEventHandler(CollectionView_CurrentChanging);
            CollectionView.CurrentChanged -= new EventHandler(CollectionView_CurrentChanged);
            CollectionView.CollectionChanged -= new NotifyCollectionChangedEventHandler(CollectionView_CollectionChanged);
            CollectionView.PropertyChanged -= new PropertyChangedEventHandler(CollectionView_PropertyChanged);
        }

        #endregion Initialization Methods

        #region Properties

        protected PagedCollectionView CollectionView { get; set; }

        protected bool ImplementsIList { get { return SourceCollection is IList; } }

        protected IEnumerable SourceCollection { get; set; }

        #endregion Properties

        #region Helper Methods

        /// <summary>
        /// Struct used to help validate that the correct events
        /// were fired in the correct order
        /// </summary>
        public struct EventNotification
        {
            public string EventType { get; set; }
            public string Parameter { get; set; }
        }

        /// <summary>
        /// Helper function that verifies that the Action fires a CollectionChanged event.
        /// </summary>
        /// <param name="test">The Action which should fire an event.</param>
        public void AssertExpectedCollectionChanged(Action test)
        {
            this._collectionChangeFired = false;
            test();
            Assert.IsTrue(this._collectionChangeFired);
        }

        /// <summary>
        /// Helper function that verifies that the Action fires a CurrentChanged event.
        /// </summary>
        /// <param name="test">The Action which should fire an event.</param>
        public void AssertExpectedCurrentChanged(Action test)
        {
            this._currentChangedFired = false;
            test();
            Assert.IsTrue(this._currentChangedFired);
        }

        /// <summary>
        /// Helper function that verifies that the Action fires a CurrentChanging event.
        /// </summary>
        /// <param name="test">The Action which should fire an event.</param>
        public void AssertExpectedCurrentChanging(Action test)
        {
            this._currentChangingFired = false;
            test();
            Assert.IsTrue(this._currentChangingFired);
        }

        /// <summary>
        /// Helper function that verifies that the test delegate raises the specified exception.
        /// </summary>
        /// <typeparam name="TException">Type of exception</typeparam>
        /// <param name="exceptionPrototype">Exception prototype, with the expected exception message populated.</param>
        /// <param name="test">Action delegate to expect exception from.</param>
        public static void AssertExpectedException<TException>(TException exceptionPrototype, Action test)
            where TException : Exception
        {
            TException exception = null;

            try
            {
                test();
            }
            catch (TException e)
            {
                // looking for exact matches
                if (e.GetType() == typeof(TException))
                {
                    exception = e;
                }
            }

            if (exception == null)
            {
                Assert.Fail("Expected {0} with message \"{1}\". \nActual: none.", typeof(TException).FullName, exceptionPrototype.Message);
            }
            else if (exception.Message != exceptionPrototype.Message)
            {
                Assert.Fail("Expected {0} with message \"{1}\". \nActual: {2} => \"{3}\".", typeof(TException).FullName, exceptionPrototype.Message, exception.GetType().FullName, exception.Message);
            }
        }

        /// <summary>
        /// Helper function that verifies that the Action does not fire an event.
        /// </summary>
        /// <param name="test">The Action which should not fire an event.</param>
        public void AssertNoEvent(Action test)
        {
            this._collectionChangeFired = false;
            this._currentChangingFired = false;
            this._currentChangedFired = false;
            test();
            Assert.IsFalse(this._collectionChangeFired);
            Assert.IsFalse(this._currentChangingFired);
            Assert.IsFalse(this._currentChangedFired);
        }

        public void CollectionView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _collectionChangeFired = true;

            if (_expectedEventQueue.Count > 0)
            {
                EventNotification eventNotification = _expectedEventQueue[0];
                if (eventNotification.EventType == "CollectionChanged")
                {
                    if (eventNotification.Parameter == e.Action.ToString())
                    {
                        _expectedEventQueue.RemoveAt(0);
                    }
                    else
                    {
                        Assert.Fail("CollectionChanged event was fired with the wrong action type. Got: " + e.Action.ToString() + ", Expected:" + eventNotification.Parameter);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(eventNotification.Parameter))
                    {
                        Assert.Fail("CollectionChanged(" + e.Action.ToString() + ") event was fired when we were expecting a " +
                            eventNotification.EventType + " event.");
                    }
                    else
                    {
                        Assert.Fail("CollectionChanged(" + e.Action.ToString() + ") event was fired when we were expecting a " +
                            eventNotification.EventType + " event with Parameter: " + eventNotification.Parameter);
                    }
                }
            }
        }

        public void CollectionView_CurrentChanged(object sender, EventArgs e)
        {
            _currentChangedFired = true;

            if (_expectedEventQueue.Count > 0)
            {
                EventNotification eventNotification = _expectedEventQueue[0];
                if (eventNotification.EventType == "CurrentChanged")
                {
                    _expectedEventQueue.RemoveAt(0);
                }
                else
                {
                    if (string.IsNullOrEmpty(eventNotification.Parameter))
                    {
                        Assert.Fail("CurrentChanged event was fired when we were expecting a " +
                            eventNotification.EventType + " event.");
                    }
                    else
                    {
                        Assert.Fail("CurrentChanged event was fired when we were expecting a " +
                            eventNotification.EventType + " event with Parameter: " + eventNotification.Parameter);
                    }
                }
            }
        }

        public void CollectionView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            _currentChangingFired = true;

            if (_expectedEventQueue.Count > 0)
            {
                EventNotification eventNotification = _expectedEventQueue[0];
                if (eventNotification.EventType == "CurrentChanging")
                {
                    _expectedEventQueue.RemoveAt(0);
                }
                else
                {
                    if (string.IsNullOrEmpty(eventNotification.Parameter))
                    {
                        Assert.Fail("CurrentChanging event was fired when we were expecting a " +
                            eventNotification.EventType + " event.");
                    }
                    else
                    {
                        Assert.Fail("CurrentChanging event was fired when we were expecting a " +
                            eventNotification.EventType + " event with Parameter: " + eventNotification.Parameter);
                    }
                }
            }
        }

        public void CollectionView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_propertyChangedTracked)
            {
                return;
            }

            _propertyChangedFired = true;

            if (_expectedEventQueue.Count > 0)
            {
                EventNotification eventNotification = _expectedEventQueue[0];
                if (eventNotification.EventType == "PropertyChanged")
                {
                    if (eventNotification.Parameter == e.PropertyName)
                    {
                        _expectedEventQueue.RemoveAt(0);
                    }
                    else
                    {
                        Assert.Fail("PropertyChanged event was fired with the wrong property name. Got: " + e.PropertyName + ", Expected: " + eventNotification.Parameter);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(eventNotification.Parameter))
                    {
                        Assert.Fail("PropertyChanged(" + e.PropertyName + ") event was fired when we were expecting a " +
                            eventNotification.EventType + " event.");
                    }
                    else
                    {
                        Assert.Fail("PropertyChanged(" + e.PropertyName + ") event was fired when we were expecting a " +
                            eventNotification.EventType + " event with Parameter: " + eventNotification.Parameter);
                    }
                }
            }
        }

        public bool FilterNegativeNumbers(object item)
        {
            TestClass testClass = item as TestClass;

            if (testClass == null)
            {
                return false;
            }
            else
            {
                return testClass.IntProperty >= 0;
            }
        }

        public bool FilterOutOnes(object item)
        {
            TestClass testClass = item as TestClass;

            if (testClass == null)
            {
                return false;
            }
            else
            {
                return testClass.IntProperty != 1;
            }
        }

        #endregion Helper Methods

    }
}
