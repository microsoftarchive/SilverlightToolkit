//-----------------------------------------------------------------------
// <copyright file="PagedCollectionViewTests.cs" company="Microsoft">
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
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs the unit tests for PagedCollectionView
    /// </summary>
    [TestClass]
    public class PagedCollectionViewTests : SilverlightTest
    {
        #region Constants

        private const int DefaultWaitTimeout = 2500;
        private const int VisualDelayInMilliseconds = 100;

        #endregion Constants

        #region Fields

        private bool _currentChanged;
        private bool _currentChanging;
        private bool _eventFired;
        private NotifyCollectionChangedAction _expectedAction;
        private List<NotifyCollectionChangedAction> _expectedActionSequence;
        private List<string> _expectedPropertyNames = new List<string>();
        private bool _pcvPageChanged;
        private DateTime _startedWaiting;

        #endregion Fields

        #region Constructor Tests

        /// <summary>
        /// Validate the constructor on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the constructor on the PagedCollectionView class.")]
        public void ConstructorTest()
        {
            List<int> intList = new List<int>();
            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsNotNull(pcv);
        }

        #endregion Constructor Tests

        #region Internal Collection (Snapshot) Tests

        /// <summary>
        /// Validate the LoadSnapshot method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the LoadSnapshot method on the PagedCollectionView class.")]
        public void LoadSnapshotTest()
        {
            ObservableCollection<int> intCollection = new ObservableCollection<int>();
            intCollection.Add(1);
            intCollection.Add(3);
            intCollection.Add(5);

            PagedCollectionView pcv = new PagedCollectionView(intCollection);

            // Clear() calls LoadSnapshot() since intCollection is of type INotifyCollectionChanged.
            intCollection.Clear();

            Assert.IsNull(pcv.CurrentItem);
            Assert.AreEqual(-1, pcv.CurrentPosition);
            Assert.IsTrue(pcv.IsCurrentAfterLast);
            Assert.IsTrue(pcv.IsCurrentBeforeFirst);
        }

        /// <summary>
        /// Validate the LoadSnapshotCore method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the LoadSnapshotCore method on the PagedCollectionView class.")]
        public void LoadSnapshotCoreTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            // The PagedCollectionView constructor calls LoadSnapshotCore().
            PagedCollectionView pcv = new PagedCollectionView(intList);

            Assert.AreEqual(1, pcv.GetItemAt(0));
            Assert.AreEqual(3, pcv.GetItemAt(1));
            Assert.AreEqual(5, pcv.GetItemAt(2));
        }

        /// <summary>
        /// Validate the EnsureSnapshot method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the EnsureSnapshot method on the PagedCollectionView class.")]
        public void EnsureSnapshotTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView pcv = new PagedCollectionView(intList);

            intList.Remove(3);

            // Contains() calls EnsureSnapshot().
            Assert.IsFalse(pcv.Contains(3));
        }

        /// <summary>
        /// Validate the ProcessCollectionChanged method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the ProcessCollectionChanged method on the PagedCollectionView class.")]
        public void ProcessCollectionChangedTest()
        {
            ObservableCollection<int> intCollection = new ObservableCollection<int>();
            intCollection.Add(1);
            intCollection.Add(3);
            intCollection.Add(5);

            PagedCollectionView pcv = new PagedCollectionView(intCollection);

            // All modifications to intCollection call ProcessCollectionChanged() since intCollection is of type INotifyCollectionChanged.

            // This creates a NotifyCollectionChangedAction.Add event.
            intCollection.Add(7);
            Assert.AreEqual(3, pcv.IndexOf(7));

            // This creates a NotifyCollectionChangedAction.Remove event.
            intCollection.Remove(7);
            Assert.AreEqual(-1, pcv.IndexOf(7));

            // This creates a NotifyCollectionChangedAction.Replace event.
            intCollection[0] = 9;
            Assert.AreEqual(0, pcv.IndexOf(9));

            // This creates a NotifyCollectionChangedAction.Reset event.
            intCollection.Clear();
            Assert.AreEqual(0, pcv.Count);
        }

        /// <summary>
        /// Validate the ProcessCollectionChanged method on the PagedCollectionView class when the we use the SourceCollection as the internal snapshot.
        /// </summary>
        [TestMethod]
        [Description("Validate the ProcessCollectionChanged method on the PagedCollectionView class when the we use the SourceCollection as the internal snapshot.")]
        public void ProcessCollectionChangedOnSourceCollectionTest()
        {
            ObservableCollection<int> intCollection = new ObservableCollection<int>() { 1, 2, 3 };

            // our optimization should set the snapshot to the intCollection
            PagedCollectionView pcv = new PagedCollectionView(intCollection);

            // add a group description so that UsesLocalArray = true
            pcv.GroupDescriptions.Add(new PropertyGroupDescription("SomeProperty"));

            // add an item to verify that we don't throw an exception
            try
            {
                intCollection.Add(4);
            }
            catch
            {
                Assert.Fail("We should not be re-handling the CollectionChanged event if our internal snapshot points to the source collection");
            }
        }

        #endregion Internal Collection (Snapshot) Tests

        #region Internal Collection Helpers Tests

        /// <summary>
        /// Validate the InternalContains method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the InternalContains method on the PagedCollectionView class.")]
        public void InternalContainsTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);

            // The PagedCollectionView constructor calls LoadSnapshotCore().
            PagedCollectionView pcv = new PagedCollectionView(intList);

            // Contains() calls InternalContains().
            Assert.IsTrue(pcv.Contains(1));
            Assert.IsFalse(pcv.Contains(2));
        }

        /// <summary>
        /// Validate the InternalCount property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the InternalCount property on the PagedCollectionView class.")]
        public void InternalCountTest()
        {
            List<int> intList = new List<int>();

            // The PagedCollectionView constructor calls LoadSnapshotCore().
            PagedCollectionView pcv = new PagedCollectionView(intList);

            // Count uses InternalCount.
            Assert.AreEqual(0, pcv.Count);
            intList.Add(1);
            Assert.AreEqual(1, pcv.Count);
            intList.Add(3);
            Assert.AreEqual(2, pcv.Count);
        }

        /// <summary>
        /// Validate the InternalIndexOf method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the InternalIndexOf method on the PagedCollectionView class.")]
        public void InternalIndexOfTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);

            // The PagedCollectionView constructor calls LoadSnapshotCore().
            PagedCollectionView pcv = new PagedCollectionView(intList);

            // IndexOf() calls InternalIndexOf().
            Assert.AreEqual(0, pcv.IndexOf(1));
            Assert.AreEqual(-1, pcv.IndexOf(2));
        }

        /// <summary>
        /// Validate the InternalItemAt method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the InternalItemAt method on the PagedCollectionView class.")]
        public void InternalItemAt()
        {
            List<int> intList = new List<int>();
            intList.Add(1);

            // The PagedCollectionView constructor calls LoadSnapshotCore().
            PagedCollectionView pcv = new PagedCollectionView(intList);

            // GetItemAt() calls InternalItemAt().
            Assert.AreEqual(1, pcv.GetItemAt(0));
        }

        #endregion Internal Collection Helpers Tests

        #region ICollectionView Tests

        #region Filtering

        /// <summary>
        /// Validate the Filter property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Filter property on the PagedCollectionView class.")]
        public void FilterTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                intList.Add(i % 10);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(100, pcv.Count);

            pcv.Filter = new Predicate<object>(this.FilterInt);
            Assert.AreEqual(50, pcv.Count);
        }

        /// <summary>
        /// Validate the CanFilter property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanFilter property on the PagedCollectionView class.")]
        public void CanFilterTest()
        {
            List<int> intList = new List<int>();
            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsTrue(pcv.CanFilter);
        }

        /// <summary>
        /// Validate the PassesFilter function on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the PassesFilter function on the PagedCollectionView class.")]
        public void PassesFilterTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);
            intList.Add(7);
            intList.Add(9);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.Filter = new Predicate<object>(this.FilterInt);

            Assert.IsFalse(pcv.PassesFilter(intList[0]));
            Assert.IsFalse(pcv.PassesFilter(intList[1]));
            Assert.IsTrue(pcv.PassesFilter(intList[2]));
            Assert.IsTrue(pcv.PassesFilter(intList[3]));
            Assert.IsTrue(pcv.PassesFilter(intList[4]));
        }

        #endregion Filtering

        #region Sorting

        /// <summary>
        /// Validate the SortDescriptions property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the SortDescriptions property on the PagedCollectionView class.")]
        public void SortDescriptionsTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            tcList.Add(new TestClass() { IntProperty = 3 });
            tcList.Add(new TestClass() { IntProperty = 1 });
            tcList.Add(new TestClass() { IntProperty = 9 });
            tcList.Add(new TestClass() { IntProperty = 7 });
            tcList.Add(new TestClass() { IntProperty = 5 });

            PagedCollectionView pcv = new PagedCollectionView(tcList);
            Assert.IsNotNull(pcv.SortDescriptions);

            pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));
            Assert.AreEqual(1, pcv.SortDescriptions.Count);

            // verify that the items are sorted in ascending order
            Assert.AreEqual(1, (pcv[0] as TestClass).IntProperty);
            Assert.AreEqual(3, (pcv[1] as TestClass).IntProperty);
            Assert.AreEqual(5, (pcv[2] as TestClass).IntProperty);
            Assert.AreEqual(7, (pcv[3] as TestClass).IntProperty);
            Assert.AreEqual(9, (pcv[4] as TestClass).IntProperty);

            pcv.SortDescriptions.Clear();
            pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Descending));

            // verify that the items are sorted in descending order
            Assert.AreEqual(9, (pcv[0] as TestClass).IntProperty);
            Assert.AreEqual(7, (pcv[1] as TestClass).IntProperty);
            Assert.AreEqual(5, (pcv[2] as TestClass).IntProperty);
            Assert.AreEqual(3, (pcv[3] as TestClass).IntProperty);
            Assert.AreEqual(1, (pcv[4] as TestClass).IntProperty);
        }

        /// <summary>
        /// Validate the CanSort property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanSort property on the PagedCollectionView class.")]
        public void CanSortTest()
        {
            List<int> intList = new List<int>();
            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsTrue(pcv.CanSort);
        }

        #endregion Sorting

        #region Sorting and Filtering

        /// <summary>
        /// Validate that we can apply both a Sort and Filter on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate that we can apply both a Sort and Filter on the PagedCollectionView class.")]
        public void SortAndFilterTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            tcList.Add(new TestClass() { IntProperty = 3 });
            tcList.Add(new TestClass() { IntProperty = 1 });
            tcList.Add(new TestClass() { IntProperty = 9 });
            tcList.Add(new TestClass() { IntProperty = 7 });
            tcList.Add(new TestClass() { IntProperty = 5 });

            PagedCollectionView pcv = new PagedCollectionView(tcList);
            Assert.IsNotNull(pcv.SortDescriptions);

            pcv.Filter = new Predicate<object>(this.FilterTestClass5);
            pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));

            // verify that the items are sorted in ascending order
            Assert.AreEqual(3, pcv.Count);
            Assert.AreEqual(5, (pcv[0] as TestClass).IntProperty);
            Assert.AreEqual(7, (pcv[1] as TestClass).IntProperty);
            Assert.AreEqual(9, (pcv[2] as TestClass).IntProperty);

            pcv.SortDescriptions.Clear();
            pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Descending));

            // verify that the items are sorted in descending order
            Assert.AreEqual(9, (pcv[0] as TestClass).IntProperty);
            Assert.AreEqual(7, (pcv[1] as TestClass).IntProperty);
            Assert.AreEqual(5, (pcv[2] as TestClass).IntProperty);
        }

        #endregion Sorting and Filtering

        #region Grouping

        /// <summary>
        /// Validate the CanGroup property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanGroup property on the PagedCollectionView class.")]
        public void CanGroupTest()
        {
            List<int> intList = new List<int>();
            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsTrue(pcv.CanGroup);
        }

        /// <summary>
        /// Validate the GroupDescriptions property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the GroupDescriptions property on the PagedCollectionView class.")]
        public void GroupDescriptionsTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 3, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 4, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 5, StringProperty = "Orange" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            Assert.AreEqual(0, cv.GroupDescriptions.Count);
            Assert.AreEqual("Apple", (cv[0] as TestClass).StringProperty);
            Assert.AreEqual("Orange", (cv[1] as TestClass).StringProperty);
            Assert.AreEqual("Orange", (cv[2] as TestClass).StringProperty);
            Assert.AreEqual("Apple", (cv[3] as TestClass).StringProperty);
            Assert.AreEqual("Orange", (cv[4] as TestClass).StringProperty);

            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
            Assert.AreEqual(1, cv.GroupDescriptions.Count);
            Assert.AreEqual("Apple", (cv[0] as TestClass).StringProperty);
            Assert.AreEqual("Apple", (cv[1] as TestClass).StringProperty);
            Assert.AreEqual("Orange", (cv[2] as TestClass).StringProperty);
            Assert.AreEqual("Orange", (cv[3] as TestClass).StringProperty);
            Assert.AreEqual("Orange", (cv[4] as TestClass).StringProperty);
        }

        /// <summary>
        /// Validate the Groups property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Groups property on the PagedCollectionView class.")]
        public void GroupsTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 3, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 4, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 5, StringProperty = "Orange" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            Assert.AreEqual(2, cv.Groups.Count);
            Assert.AreEqual("Apple", (cv.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(2, (cv.Groups[0] as CollectionViewGroup).ItemCount);
            Assert.AreEqual("Orange", (cv.Groups[1] as CollectionViewGroup).Name);
            Assert.AreEqual(3, (cv.Groups[1] as CollectionViewGroup).ItemCount);
        }

        /// <summary>
        /// Validate that when we have an empty collection with grouping, we can add items and that it will be correctly grouped.
        /// </summary>
        [TestMethod]
        [Description("Validate that when we have an empty collection with grouping, we can add items and that it will be correctly grouped.")]
        public void GroupingWithAddTest()
        {
            ObservableCollection<TestClass> testList = new ObservableCollection<TestClass>();

            PagedCollectionView cv = new PagedCollectionView(testList);
            Assert.IsNull(cv.Groups);

            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
            Assert.IsNotNull(cv.Groups);

            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            Assert.AreEqual(2, cv.Groups.Count);
        }

        /// <summary>
        /// Validate the Groups property with nested groups on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Groups property with nested groups on the PagedCollectionView class.")]
        public void NestedGroupsTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Apple" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
            cv.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));

            Assert.AreEqual(2, cv.Groups.Count);

            // Group "Apple"
            Assert.AreEqual("Apple", (cv.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(5, (cv.Groups[0] as CollectionViewGroup).ItemCount);
            Assert.AreEqual(2, (cv.Groups[0] as CollectionViewGroup).Items.Count);

            Assert.AreEqual(2, ((cv.Groups[0] as CollectionViewGroup).Items[0] as CollectionViewGroup).Name);
            Assert.AreEqual(1, ((cv.Groups[0] as CollectionViewGroup).Items[1] as CollectionViewGroup).Name);

            // Group "Orange"
            Assert.AreEqual("Orange", (cv.Groups[1] as CollectionViewGroup).Name);
            Assert.AreEqual(5, (cv.Groups[1] as CollectionViewGroup).ItemCount);
            Assert.AreEqual(2, (cv.Groups[1] as CollectionViewGroup).Items.Count);

            Assert.AreEqual(1, ((cv.Groups[1] as CollectionViewGroup).Items[0] as CollectionViewGroup).Name);
            Assert.AreEqual(2, ((cv.Groups[1] as CollectionViewGroup).Items[1] as CollectionViewGroup).Name);
        }

        /// <summary>
        /// Validate the events on the Groups property are fired correctly.
        /// </summary>
        [TestMethod]
        [Description("Validate the events on the Groups property are fired correctly.")]
        public void GroupingEventsTest()
        {
            ObservableCollection<TestClass> testList = new ObservableCollection<TestClass>()
            {
                new TestClass { IntProperty = 2, StringProperty = "Apple" },
                new TestClass { IntProperty = 1, StringProperty = "Orange" },
                new TestClass { IntProperty = 2, StringProperty = "Orange" },
                new TestClass { IntProperty = 1, StringProperty = "Apple" },
                new TestClass { IntProperty = 2, StringProperty = "Orange" },
                new TestClass { IntProperty = 2, StringProperty = "Apple" },
                new TestClass { IntProperty = 1, StringProperty = "Orange" },
                new TestClass { IntProperty = 2, StringProperty = "Orange" },
                new TestClass { IntProperty = 1, StringProperty = "Apple" },
                new TestClass { IntProperty = 2, StringProperty = "Apple" }
            };

            PagedCollectionView cv = new PagedCollectionView(testList);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            // check to see that we get a reset when we change the group descriptions
            (cv.Groups as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(this.PagedCollectionViewCollectionChangedSequence);
            this._expectedActionSequence = new List<NotifyCollectionChangedAction>()
            {
                NotifyCollectionChangedAction.Reset,  /* clearing the group */
                NotifyCollectionChangedAction.Add,    /* adding 'Apple' */
                NotifyCollectionChangedAction.Add     /* adding 'Orange' */
            };
            this.AssertExpectedEvent(delegate { cv.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty")); });

            // since it is only listening on the first node, it should not get events for items added/removed under it
            this.AssertNoEvent(delegate { testList.Add(new TestClass { IntProperty = 2, StringProperty = "Apple" }); });
            this.AssertNoEvent(delegate { testList.RemoveAt(testList.Count - 1); });

            (cv.Groups as INotifyCollectionChanged).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.PagedCollectionViewCollectionChangedSequence);

            // the grouping should currently look like this:
            //
            //
            //                        /-- (2)
            //         /-- (Apple) --/-- (1)
            // (Root)--
            //         \-- (Orange) --\-- (1)
            //                         \-- (2)
            //
            Assert.AreEqual(2, cv.Groups.Count);

            // we need to listen to INotifyPropertyChanged for "ItemCount" changes on the children.

            // test adding/removing item under Root-Apple-2
            Assert.AreEqual(3, ((cv.Groups[0] as CollectionViewGroup).Items[0] as CollectionViewGroup).ItemCount);

            ((cv.Groups[0] as CollectionViewGroup).Items[0] as INotifyPropertyChanged).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);
            this._expectedPropertyNames = new List<string> { "ItemCount" };
            this.AssertExpectedEvent(delegate { testList.Add(new TestClass { IntProperty = 2, StringProperty = "Apple" }); });
            Assert.AreEqual(4, ((cv.Groups[0] as CollectionViewGroup).Items[0] as CollectionViewGroup).ItemCount);

            this.AssertExpectedEvent(delegate { testList.RemoveAt(testList.Count - 1); });
            Assert.AreEqual(3, ((cv.Groups[0] as CollectionViewGroup).Items[0] as CollectionViewGroup).ItemCount);
            ((cv.Groups[0] as CollectionViewGroup).Items[0] as INotifyPropertyChanged).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);

            // test adding/removing new node under Root-Apple-(3)
            Assert.AreEqual(2, (cv.Groups[0] as CollectionViewGroup).Items.Count);
            Assert.AreEqual(5, (cv.Groups[0] as CollectionViewGroup).ItemCount);
            (cv.Groups[0] as INotifyPropertyChanged).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);

            this.AssertExpectedEvent(delegate { testList.Add(new TestClass { IntProperty = 3, StringProperty = "Apple" }); });
            Assert.AreEqual(3, (cv.Groups[0] as CollectionViewGroup).Items.Count);
            Assert.AreEqual(6, (cv.Groups[0] as CollectionViewGroup).ItemCount);

            this.AssertExpectedEvent(delegate { testList.RemoveAt(testList.Count - 1); });
            Assert.AreEqual(2, (cv.Groups[0] as CollectionViewGroup).Items.Count);
            Assert.AreEqual(5, (cv.Groups[0] as CollectionViewGroup).ItemCount);

            (cv.Groups[0] as INotifyPropertyChanged).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);
        }

        /// <summary>
        /// Validate the Groups property when no GroupDescriptions are specified.
        /// </summary>
        [TestMethod]
        [Description("Validate the Groups property when no GroupDescriptions are specified.")]
        public void EmptyGroupsTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            Assert.AreEqual(0, cv.GroupDescriptions.Count);
            Assert.IsNull(cv.Groups);
        }

#if NewItemPlaceholderSupported
        /// <summary>
        /// Validate the Groups property with a NewItemPlaceholder.
        /// </summary>
        [TestMethod]
        [Description("Validate the Groups property with a NewItemPlaceholder.")]
        public void GroupsTestWithNewItemPlaceholder()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            cv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtBeginning;

            Assert.AreEqual(2, cv.Groups.Count);
            Assert.AreEqual(-1, cv.Groups.IndexOf(cv[0] /*NewItemPlaceholder*/)); 
        }
#endif

        /// <summary>
        /// Validate the Enumerator from the Groups property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Enumerator from the Groups property on the PagedCollectionView class.")]
        public void GroupsEnumeratorTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 3, StringProperty = "Orange" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            // Test the Enumerator on the Groups property
            IEnumerator groupEnumerator = cv.Groups.GetEnumerator();

            groupEnumerator.MoveNext();
            Assert.AreEqual(cv.Groups[0], groupEnumerator.Current);
            groupEnumerator.MoveNext();
            Assert.AreEqual(cv.Groups[1], groupEnumerator.Current);

            // Test the Enumerator on the PagedCollectionView with groups applied
            IEnumerator cvEnumerator = cv.GetEnumerator();

            cvEnumerator.MoveNext();
            Assert.AreEqual(cv[0], cvEnumerator.Current);
            cvEnumerator.MoveNext();
            Assert.AreEqual(cv[1], cvEnumerator.Current);
            cvEnumerator.MoveNext();
            Assert.AreEqual(cv[2], cvEnumerator.Current);
        }

        /// <summary>
        /// Validate the Groups property on the PagedCollectionView class while we add/remove items.
        /// </summary>
        [TestMethod]
        [Description("Validate the Groups property on the PagedCollectionView class while we add/remove items.")]
        public void GroupingWhileAddAndRemoveTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 3, StringProperty = "Orange" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            Assert.AreEqual(2, cv.Groups.Count);

            // Add another item in a new group
            TestClass newItem = cv.AddNew() as TestClass;
            newItem.IntProperty = 4;
            newItem.StringProperty = "Banana";
            cv.CommitNew();

            Assert.AreEqual(3, cv.Groups.Count);
            Assert.AreEqual("Banana", (cv.Groups[2] as CollectionViewGroup).Name);

            // Remove item from first group
            cv.RemoveAt(0);
            Assert.AreEqual(2, cv.Groups.Count);
            Assert.AreEqual("Orange", (cv.Groups[0] as CollectionViewGroup).Name);
        }

        /// <summary>
        /// Validate the Groups property on the PagedCollectionView class with paging.
        /// </summary>
        [TestMethod]
        [Description("Validate the Groups property on the PagedCollectionView class with paging.")]
        public void GroupsWithPagingTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 3, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 4, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 5, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 6, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 7, StringProperty = "Apple" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            cv.PageSize = 3;
            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            // check first page
            Assert.AreEqual(0, cv.PageIndex);
            Assert.AreEqual(1, cv.Groups.Count);
            Assert.AreEqual("Apple", (cv.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(3, (cv.Groups[0] as CollectionViewGroup).ItemCount);

            // check second page
            cv.MoveToPage(1);
            Assert.AreEqual(1, cv.PageIndex);
            Assert.AreEqual(2, cv.Groups.Count);
            Assert.AreEqual("Apple", (cv.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(1, (cv.Groups[0] as CollectionViewGroup).ItemCount);
            Assert.AreEqual("Orange", (cv.Groups[1] as CollectionViewGroup).Name);
            Assert.AreEqual(2, (cv.Groups[1] as CollectionViewGroup).ItemCount);

            // check last page
            cv.MoveToPage(2);
            Assert.AreEqual(2, cv.PageIndex);
            Assert.AreEqual(1, cv.Groups.Count);
            Assert.AreEqual("Orange", (cv.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(1, (cv.Groups[0] as CollectionViewGroup).ItemCount);
        }

        /// <summary>
        /// Validate the Groups property on the PagedCollectionView class with sorting.
        /// </summary>
        [TestMethod]
        [Description("Validate the Groups property on the PagedCollectionView class with sorting.")]
        public void GroupsWithSortingTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 7, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 3, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 5, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 6, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 4, StringProperty = "Apple" });

            PagedCollectionView cv = new PagedCollectionView(testList);
            cv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));
            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            // check groups
            Assert.AreEqual("Orange", (cv.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual("Apple", (cv.Groups[1] as CollectionViewGroup).Name);

            // check sort order
            Assert.AreEqual(1, (cv[0] as TestClass).IntProperty);
            Assert.AreEqual(3, (cv[1] as TestClass).IntProperty);
            Assert.AreEqual(6, (cv[2] as TestClass).IntProperty);
            Assert.AreEqual(2, (cv[3] as TestClass).IntProperty);

            Assert.AreEqual(4, (cv[4] as TestClass).IntProperty);
            Assert.AreEqual(5, (cv[5] as TestClass).IntProperty);
            Assert.AreEqual(7, (cv[6] as TestClass).IntProperty);
        }

        /// <summary>
        /// Validate the IsDataInGroupOrder property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IsDataInGroupOrder property on the PagedCollectionView class.")]
        public void IsDataInGroupOrderTest()
        {
            List<TestClass> testList = new List<TestClass>();
            testList.Add(new TestClass { IntProperty = 1, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 2, StringProperty = "Orange" });
            testList.Add(new TestClass { IntProperty = 3, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 4, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 5, StringProperty = "Apple" });
            testList.Add(new TestClass { IntProperty = 6, StringProperty = "Orange" });

            PagedCollectionView cv = new PagedCollectionView(testList, false, true);

            cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            // we have {Orange, Apple, Orange} because of the ordering
            Assert.AreEqual(3, cv.Groups.Count);

            Assert.AreEqual(2, (cv.Groups[0] as CollectionViewGroup).Items.Count);
            Assert.AreEqual("Orange", (cv.Groups[0] as CollectionViewGroup).Name);

            Assert.AreEqual(3, (cv.Groups[1] as CollectionViewGroup).Items.Count);
            Assert.AreEqual("Apple", (cv.Groups[1] as CollectionViewGroup).Name);

            Assert.AreEqual(1, (cv.Groups[2] as CollectionViewGroup).Items.Count);
            Assert.AreEqual("Orange", (cv.Groups[2] as CollectionViewGroup).Name);
        }

        #endregion Grouping

        #region Currency

        /// <summary>
        /// Validate the CurrentItem property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CurrentItem property on the PagedCollectionView class.")]
        public void CurrentItemTest()
        {
            List<int> intList1 = new List<int>();
            List<int> intList2 = new List<int>();
            intList2.Add(1);

            PagedCollectionView pcv1 = new PagedCollectionView(intList1);
            Assert.IsNull(pcv1.CurrentItem);

            PagedCollectionView pcv2 = new PagedCollectionView(intList2);
            Assert.AreEqual(1, pcv2.CurrentItem);
        }

        /// <summary>
        /// Validate the CurrentPosition property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CurrentPosition property on the PagedCollectionView class.")]
        public void CurrentPositionTest()
        {
            List<int> intList1 = new List<int>();
            List<int> intList2 = new List<int>();
            List<int> intList3 = new List<int>() { 1, 2, 3, 4, 5 };
            intList2.Add(1);

            PagedCollectionView pcv1 = new PagedCollectionView(intList1);
            Assert.AreEqual(-1, pcv1.CurrentPosition);

            PagedCollectionView pcv2 = new PagedCollectionView(intList2);
            Assert.AreEqual(0, pcv2.CurrentPosition);

            PagedCollectionView pcv3 = new PagedCollectionView(intList3);

            pcv3.MoveCurrentToPosition(2);
            pcv3.Remove(pcv3.CurrentItem);

            Assert.AreEqual(2, pcv3.CurrentPosition);
        }

        /// <summary>
        /// Validate currency when we add and remove items on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate currency when we add and remove items on the PagedCollectionView class.")]
        public void CurrencyWithAddRemoveTest()
        {
            ObservableCollection<int> intCollection = new ObservableCollection<int>() { 1, 2, 3, 4, 5 };
            PagedCollectionView pcv = new PagedCollectionView(intCollection);

            pcv.CurrentChanged += new EventHandler(this.PagedCollectionViewCurrentChanged);
            pcv.CurrentChanging += new CurrentChangingEventHandler(this.PagedCollectionViewCurrentChanging);

            this.AssertCurrencyEvents(delegate { pcv.MoveCurrentTo(3); });

            Assert.AreEqual(2, pcv.CurrentPosition);
            Assert.AreEqual(3, pcv.CurrentItem);

            // remove an item from the intCollection which will fire
            // an INCC that the PagedCollectionView will handle
            this.AssertCurrencyEvents(delegate { intCollection.Remove(3); });

            // {1, 2, [4], 5}
            Assert.AreEqual(2, pcv.CurrentPosition);
            Assert.AreEqual(4, pcv.CurrentItem);

            // remove an item through the PagedCollectionView directly
            this.AssertCurrencyEvents(delegate { pcv.RemoveAt(2); });

            // {1, 2, [5]}
            Assert.AreEqual(2, pcv.CurrentPosition);
            Assert.AreEqual(5, pcv.CurrentItem);

            // remove the last item.
            this.AssertCurrencyEvents(delegate { pcv.Remove(5); });

            // {1, [2]}
            Assert.AreEqual(1, pcv.CurrentPosition);
            Assert.AreEqual(2, pcv.CurrentItem);

            // now add an item through the intCollection
            this.AssertCurrencyEvents(delegate { intCollection.Insert(0, 1); });

            // {0, 1, [2]}
            Assert.AreEqual(2, pcv.CurrentPosition);
            Assert.AreEqual(2, pcv.CurrentItem);

            // now add an item at the end
            this.AssertNoCurrencyEvents(delegate { intCollection.Add(3); });

            // {0, 1, [2], 3}
            Assert.AreEqual(2, pcv.CurrentPosition);
            Assert.AreEqual(2, pcv.CurrentItem);

            pcv.CurrentChanged -= new EventHandler(this.PagedCollectionViewCurrentChanged);
            pcv.CurrentChanging -= new CurrentChangingEventHandler(this.PagedCollectionViewCurrentChanging);
        }

        /// <summary>
        /// Validate the IsCurrentAfterLast property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IsCurrentAfterLast property on the PagedCollectionView class.")]
        public void IsCurrentAfterLastTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsFalse(pcv.IsCurrentAfterLast);
            pcv.MoveCurrentToNext();
            Assert.IsTrue(pcv.IsCurrentAfterLast);
        }

        /// <summary>
        /// Validate the IsCurrentBeforeFirst property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IsCurrentBeforeFirst property on the PagedCollectionView class.")]
        public void IsCurrentBeforeFirstTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsFalse(pcv.IsCurrentBeforeFirst);
            pcv.MoveCurrentToPrevious();
            Assert.IsTrue(pcv.IsCurrentBeforeFirst);
        }

        /// <summary>
        /// Validate the MoveCurrentToFirst method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveCurrentToFirst method on the PagedCollectionView class.")]
        public void MoveCurrentToFirstTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);
            intList.Add(7);
            intList.Add(9);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.MoveCurrentToPosition(2);
            Assert.IsTrue(pcv.MoveCurrentToFirst());
            Assert.AreEqual(0, pcv.CurrentPosition);
            Assert.AreEqual(1, pcv.CurrentItem);
        }

        /// <summary>
        /// Validate the MoveCurrentToPrevious method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveCurrentToPrevious method on the PagedCollectionView class.")]
        public void MoveCurrentToPreviousTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);
            intList.Add(7);
            intList.Add(9);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.MoveCurrentToPosition(2);
            Assert.IsTrue(pcv.MoveCurrentToPrevious());
            Assert.AreEqual(1, pcv.CurrentPosition);
            Assert.AreEqual(3, pcv.CurrentItem);
            pcv.MoveCurrentToPosition(0);
            Assert.IsFalse(pcv.MoveCurrentToPrevious());
            Assert.AreEqual(-1, pcv.CurrentPosition);
            Assert.IsNull(pcv.CurrentItem);
        }

        /// <summary>
        /// Validate the MoveCurrentToNext method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveCurrentToNext method on the PagedCollectionView class.")]
        public void MoveCurrentToNextTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);
            intList.Add(7);
            intList.Add(9);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.MoveCurrentToPosition(2);
            Assert.IsTrue(pcv.MoveCurrentToNext());
            Assert.AreEqual(3, pcv.CurrentPosition);
            Assert.AreEqual(7, pcv.CurrentItem);
            pcv.MoveCurrentToPosition(4);
            Assert.IsFalse(pcv.MoveCurrentToNext());
            Assert.AreEqual(5, pcv.CurrentPosition);
            Assert.IsNull(pcv.CurrentItem);
        }

        /// <summary>
        /// Validate the MoveCurrentToLast method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveCurrentToLast method on the PagedCollectionView class.")]
        public void MoveCurrentToLastTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);
            intList.Add(7);
            intList.Add(9);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.MoveCurrentToPosition(2);
            Assert.IsTrue(pcv.MoveCurrentToLast());
            Assert.AreEqual(4, pcv.CurrentPosition);
            Assert.AreEqual(9, pcv.CurrentItem);
        }

        /// <summary>
        /// Validate the MoveCurrentTo method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveCurrentTo method on the PagedCollectionView class.")]
        public void MoveCurrentToTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);
            intList.Add(7);
            intList.Add(9);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsTrue(pcv.MoveCurrentTo(5));
            Assert.AreEqual(2, pcv.CurrentPosition);
            Assert.AreEqual(5, pcv.CurrentItem);
            Assert.IsFalse(pcv.MoveCurrentTo(2));
            Assert.AreEqual(-1, pcv.CurrentPosition);
            Assert.IsNull(pcv.CurrentItem);
        }

        /// <summary>
        /// Validate the MoveCurrentToPosition method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is just being used to test against an exception being generated elsewhere; it is not itself being thrown.")]
        [Description("Validate the MoveCurrentToPosition method on the PagedCollectionView class.")]
        public void MoveCurrentToPositionTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);
            intList.Add(7);
            intList.Add(9);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsTrue(pcv.MoveCurrentToPosition(2));
            Assert.AreEqual(2, pcv.CurrentPosition);
            Assert.AreEqual(5, pcv.CurrentItem);
            Assert.IsFalse(pcv.MoveCurrentToPosition(-1));
            Assert.AreEqual(-1, pcv.CurrentPosition);
            Assert.IsNull(pcv.CurrentItem);
            Assert.IsFalse(pcv.MoveCurrentToPosition(5));
            Assert.AreEqual(5, pcv.CurrentPosition);
            Assert.IsNull(pcv.CurrentItem);

            AssertExpectedException(
                new ArgumentOutOfRangeException("position"),
                delegate
                {
                    pcv.MoveCurrentToPosition(-2);
                });
            AssertExpectedException(
                new ArgumentOutOfRangeException("position"),
                delegate
                {
                    pcv.MoveCurrentToPosition(6);
                });
        }

        /// <summary>
        /// Validate the CurrentChanging event on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CurrentChanging event on the PagedCollectionView class.")]
        public void CurrentChangingTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView pcv = new PagedCollectionView(intList);

            this.AssertNoEvent(delegate { pcv.MoveCurrentToNext(); });

            pcv.CurrentChanging += new System.ComponentModel.CurrentChangingEventHandler(this.CancelChanging);
            this.AssertExpectedEvent(delegate { pcv.MoveCurrentToNext(); });
            pcv.CurrentChanging -= new System.ComponentModel.CurrentChangingEventHandler(this.CancelChanging);
            Assert.AreEqual(1, pcv.CurrentPosition);
            Assert.AreEqual(3, pcv.CurrentItem);

            pcv.CurrentChanging += new System.ComponentModel.CurrentChangingEventHandler(this.AllowChanging);
            this.AssertExpectedEvent(delegate { pcv.MoveCurrentToNext(); });
            pcv.CurrentChanging -= new System.ComponentModel.CurrentChangingEventHandler(this.AllowChanging);
            Assert.AreEqual(2, pcv.CurrentPosition);
            Assert.AreEqual(5, pcv.CurrentItem);
        }

        /// <summary>
        /// Validate the CurrentChanged event on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CurrentChanged event on the PagedCollectionView class.")]
        public void CurrentChangedTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);

            PagedCollectionView pcv = new PagedCollectionView(intList);

            this.AssertNoEvent(delegate { pcv.MoveCurrentToNext(); });

            pcv.CurrentChanged += new EventHandler(this.PagedCollectionViewCurrentChanged);
            pcv.CurrentChanging += new CurrentChangingEventHandler(this.PagedCollectionViewCurrentChanging);
            this.AssertCurrencyEvents(delegate { pcv.MoveCurrentToNext(); });
            pcv.CurrentChanged -= new EventHandler(this.PagedCollectionViewCurrentChanged);
            pcv.CurrentChanging -= new CurrentChangingEventHandler(this.PagedCollectionViewCurrentChanging);
        }

        #endregion Currency

        #region Refresh

        /// <summary>
        /// Validate the NeedsRefresh property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the NeedsRefresh property on the PagedCollectionView class.")]
        public void NeedsRefreshTest()
        {
            List<int> intList = new List<int>();
            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.IsFalse(pcv.NeedsRefresh);

            pcv.DeferRefresh();

            // In order for a RefreshOrDefer to be called
            pcv.SortDescriptions.Clear();
            Assert.IsTrue(pcv.NeedsRefresh);
        }

        /// <summary>
        /// Validate the DeferRefresh method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the DeferRefresh method on the PagedCollectionView class.")]
        public void DeferRefreshTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            TestClass fb = new TestClass();
            tcList.Add(fb);

            PagedCollectionView pcv = new PagedCollectionView(tcList);
            using (pcv.DeferRefresh())
            {
                Assert.IsFalse(pcv.NeedsRefresh);
                pcv.SortDescriptions.Clear();
                Assert.IsTrue(pcv.NeedsRefresh);
            }

            Assert.IsFalse(pcv.NeedsRefresh);

            pcv.AddNew();
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "DeferRefresh")),
                delegate
                {
                    pcv.DeferRefresh();
                });
            pcv = new PagedCollectionView(tcList);
            pcv.EditItem(fb);
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "DeferRefresh")),
                delegate
                {
                    pcv.DeferRefresh();
                });
        }

        /// <summary>
        /// Validate the DeferRefresh method with sorting and grouping on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the DeferRefresh method with sorting and grouping on the PagedCollectionView class.")]
        public void DeferRefreshWithSortAndGroupTest()
        {
            ObservableCollection<TestClass> oc = new ObservableCollection<TestClass>()
            {
                new TestClass() { IntProperty=1, StringProperty="A" },
                new TestClass() { IntProperty=1, StringProperty="B" },
                new TestClass() { IntProperty=1, StringProperty="C" },
                new TestClass() { IntProperty=2, StringProperty="A" },
                new TestClass() { IntProperty=2, StringProperty="B" },
                new TestClass() { IntProperty=2, StringProperty="C" },
            };

            PagedCollectionView pcv = new PagedCollectionView(oc);
            pcv.PageSize = 2;

            pcv.MoveToPage(1);

            Assert.AreEqual(1, pcv.PageIndex);
            Assert.IsNull(pcv.Groups);

            using (pcv.DeferRefresh())
            {
                pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("StringProperty", System.ComponentModel.ListSortDirection.Descending));
                pcv.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));

                // values shouldn't change until after the EndDefer
                Assert.AreEqual(1, pcv.PageIndex);
                Assert.IsNull(pcv.Groups);
            }

            // values should now be updated
            Assert.AreEqual(0, pcv.PageIndex);
            Assert.IsNotNull(pcv.Groups);
        }

        /// <summary>
        /// Validate the Refresh method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Refresh method on the PagedCollectionView class.")]
        public void RefreshTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            TestClass fb1 = new TestClass { IntProperty = 1, StringProperty = "Test 1" };
            TestClass fb2 = new TestClass { IntProperty = 2, StringProperty = "Test 2" };
            tcList.Add(fb1);
            tcList.Add(fb2);

            PagedCollectionView pcv = new PagedCollectionView(tcList);
            tcList.RemoveAt(0);
            pcv.Refresh();
            Assert.AreEqual(0, pcv.CurrentPosition);
            Assert.AreEqual(2, (pcv.CurrentItem as TestClass).IntProperty);

            pcv.AddNew();
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Refresh")),
                delegate
                {
                    pcv.Refresh();
                });
            pcv = new PagedCollectionView(tcList);
            pcv.EditItem(fb2);
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Refresh")),
                delegate
                {
                    pcv.Refresh();
                });
        }

        #endregion Refresh

        /// <summary>
        /// Validate the Culture property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is just being used to test against an exception being generated elsewhere; it is not itself being thrown.")]
        [Description("Validate the Culture property on the PagedCollectionView class.")]
        public void CultureTest()
        {
            List<string> strList = new List<string>();
            PagedCollectionView pcv = new PagedCollectionView(strList);
            Assert.IsNull(pcv.Culture);

            pcv.Culture = CultureInfo.InvariantCulture;
            Assert.AreEqual(CultureInfo.InvariantCulture, pcv.Culture);

            pcv.Culture = CultureInfo.CurrentUICulture;
            Assert.AreEqual(CultureInfo.CurrentUICulture, pcv.Culture);

            AssertExpectedException(
                new ArgumentNullException("value"),
                delegate
                {
                    pcv.Culture = null;
                });
        }

        /// <summary>
        /// Validate the Contains method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Contains method on the PagedCollectionView class.")]
        public void ContainsTest()
        {
            List<string> strList1 = new List<string>();
            List<string> strList2 = new List<string>();
            strList2.Add("Test 1");
            strList2.Add("Test 2");
            strList2.Add("Test 3");

            PagedCollectionView pcv1 = new PagedCollectionView(strList1);
            Assert.IsFalse(pcv1.Contains("Test 1"));

            PagedCollectionView pcv2 = new PagedCollectionView(strList2);
            Assert.IsTrue(pcv2.Contains("Test 1"));
        }

        /// <summary>
        /// Validate the SourceCollection property method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the SourceCollection property on the PagedCollectionView class.")]
        public void SourceCollectionTest()
        {
            List<string> strList1 = new List<string>();
            List<string> strList2 = new List<string>();

            PagedCollectionView pcv = new PagedCollectionView(strList1);
            Assert.AreEqual(strList1, pcv.SourceCollection);
            Assert.AreNotEqual(strList2, pcv.SourceCollection);
        }

        #endregion ICollectionView Tests

        #region IEditableCollectionView Tests

#if NewItemPlaceholderSupported
        /// <summary>
        /// Validate the NewItemPlaceholder property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the NewItemPlaceholder property on the PagedCollectionView class.")]
        public void NewItemPlaceholderTest()
        {
            // Nothing to do here yet.
        }

        /// <summary>
        /// Validate the NewItemPlaceholderPosition property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the NewItemPlaceholderPosition property on the PagedCollectionView class.")]
        public void NewItemPlaceholderPositionTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            PagedCollectionView pcv = new PagedCollectionView(tcList);

            pcv.AddNew();
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "NewItemPlaceholderPosition", "AddNew")),
                delegate
                {
                    pcv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtBeginning;
                });
        }
#endif

        #region Add

        /// <summary>
        /// Validate the IsAddingNew property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IsAddingNew property on the PagedCollectionView class.")]
        public void IsAddingNewTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            PagedCollectionView pcv = new PagedCollectionView(tcList);

            Assert.IsFalse(pcv.IsAddingNew);
            pcv.AddNew();
            Assert.IsTrue(pcv.IsAddingNew);
            pcv.CancelNew();
            Assert.IsFalse(pcv.IsAddingNew);
            pcv.AddNew();
            Assert.IsTrue(pcv.IsAddingNew);
            pcv.CommitNew();
            Assert.IsFalse(pcv.IsAddingNew);
        }

        /// <summary>
        /// Validate the CurrentAddItem property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CurrentAddItem property on the PagedCollectionView class.")]
        public void CurrentAddItemTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            PagedCollectionView pcv = new PagedCollectionView(tcList);

            object item = pcv.AddNew();
            Assert.AreEqual(item, pcv.CurrentAddItem);
        }

        /// <summary>
        /// Validate the CanAddNew property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanAddNew property on the PagedCollectionView class.")]
        public void CanAddNewTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv1 = new PagedCollectionView(efbList);

            Assert.IsTrue(pcv1.CanAddNew);

            // Having an item being edited makes it so we can't AddNew().
            pcv1.EditItem(efb);
            Assert.IsFalse(pcv1.CanAddNew);

            List<TestClass> tcList = new List<TestClass>();

            // A fixed capacity makes it so we can't AddNew().
            FixedSizeCollection<TestClass> fsfbList = new FixedSizeCollection<TestClass>();
            PagedCollectionView pcv2 = new PagedCollectionView(tcList);
            PagedCollectionView pcv3 = new PagedCollectionView(fsfbList);

            Assert.IsTrue(pcv2.CanAddNew);
            Assert.IsFalse(pcv3.CanAddNew);

            List<TestClass> fbList2 = new List<TestClass>();

            // A type without a constructor makes it so we can't AddNew().
            List<int> intList = new List<int>();
            PagedCollectionView pcv4 = new PagedCollectionView(fbList2);
            PagedCollectionView pcv5 = new PagedCollectionView(intList);

            Assert.IsTrue(pcv4.CanAddNew);
            Assert.IsFalse(pcv5.CanAddNew);
        }

        /// <summary>
        /// Validate the AddNew method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the AddNew method on the PagedCollectionView class.")]
        public void AddNewTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);

            object item = pcv.AddNew();
            Assert.AreEqual(typeof(EditableTestClass), item.GetType());
            pcv.CommitNew();

            pcv.AddNew();
            pcv.AddNew();
            pcv.CommitNew();
            Assert.AreEqual(4, pcv.Count);

            pcv.EditItem(efb);
            pcv.AddNew();
            Assert.IsFalse(pcv.IsEditingItem);

            List<int> intList = new List<int>();
            PagedCollectionView pcv2 = new PagedCollectionView(intList);
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedForView, "AddNew")),
                delegate
                {
                    pcv2.AddNew();
                });
        }

        /// <summary>
        /// Validate the AddNew method on an IEditableCollection in the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the AddNew method on an IEditableCollection in the PagedCollectionView class.")]
        public void AddNewWithEditableCollectionTest()
        {
            TestEditableCollection<TestClass> ecList = new TestEditableCollection<TestClass>();
            ecList.Add(new TestClass { IntProperty = 1, StringProperty = "A" });
            ecList.Add(new TestClass { IntProperty = 2, StringProperty = "B" });
            ecList.Add(new TestClass { IntProperty = 3, StringProperty = "C" });
            ecList.Add(new TestClass { IntProperty = 4, StringProperty = "D" });
            ecList.Add(new TestClass { IntProperty = 5, StringProperty = "E" });

            Assert.IsTrue(ecList.CanAdd);

            PagedCollectionView pcv = new PagedCollectionView(ecList);
            pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));

            TestClass item = pcv.AddNew() as TestClass;
            item.IntProperty = 6;
            item.StringProperty = "F";
            pcv.CommitNew();

            Assert.AreEqual(6, pcv.Count);
            Assert.AreEqual("F", (pcv[5] as TestClass).StringProperty);

            pcv.PageSize = 3;            
            TestClass pagedItem = pcv.AddNew() as TestClass;
            pagedItem.IntProperty = 0;
            pagedItem.StringProperty = "New";
            Assert.AreEqual(2, pcv.IndexOf(pagedItem));
            pcv.CommitNew();
            Assert.AreEqual(0, pcv.IndexOf(pagedItem));
        }

        /// <summary>
        /// Validate the AddNew method with some more advanced scenarios.
        /// </summary>
        [TestMethod]
        [Description("Validate the AddNew method with some more advanced scenarios.")]
        public void AddNewWithSortAndPageTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>()
            {
                new EditableTestClass() { IntProperty = 1, StringProperty = "A" },
                new EditableTestClass() { IntProperty = 2, StringProperty = "B" },
                new EditableTestClass() { IntProperty = 3, StringProperty = "C" },
                new EditableTestClass() { IntProperty = 4, StringProperty = "D" },
                new EditableTestClass() { IntProperty = 5, StringProperty = "E" }
            };

            PagedCollectionView pcv = new PagedCollectionView(efbList);
            pcv.PageSize = 3;
            pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));

            // Check current items on page before adding
            Assert.AreEqual("A", (pcv[0] as EditableTestClass).StringProperty);
            Assert.AreEqual("B", (pcv[1] as EditableTestClass).StringProperty);
            Assert.AreEqual("C", (pcv[2] as EditableTestClass).StringProperty);

            // Check current items on page after adding
            EditableTestClass item = pcv.AddNew() as EditableTestClass;
            item.IntProperty = 6;
            item.StringProperty = "New";

            Assert.AreEqual("A", (pcv[0] as EditableTestClass).StringProperty);
            Assert.AreEqual("B", (pcv[1] as EditableTestClass).StringProperty);
            Assert.AreEqual("New", (pcv[2] as EditableTestClass).StringProperty);

            // Check current items on page after committing
            pcv.CommitNew();
            Assert.AreEqual("A", (pcv[0] as EditableTestClass).StringProperty);
            Assert.AreEqual("B", (pcv[1] as EditableTestClass).StringProperty);
            Assert.AreEqual("C", (pcv[2] as EditableTestClass).StringProperty);

            // Check the next page to validate the new item was moved there
            pcv.MoveToNextPage();
            Assert.AreEqual("D", (pcv[0] as EditableTestClass).StringProperty);
            Assert.AreEqual("E", (pcv[1] as EditableTestClass).StringProperty);
            Assert.AreEqual("New", (pcv[2] as EditableTestClass).StringProperty);
        }

        /// <summary>
        /// Validate the CommitNew method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CommitNew method on the PagedCollectionView class.")]
        public void CommitNewTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);
            object item = pcv.AddNew();
            pcv.CommitNew();
            Assert.AreEqual(2, pcv.Count);
            Assert.AreEqual(item, pcv[1]);

            pcv.EditItem(efb);
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CommitNew", "EditItem")),
                delegate
                {
                    pcv.CommitNew();
                });
        }

        /// <summary>
        /// Validate the CancelNew method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CancelNew method on the PagedCollectionView class.")]
        public void CancelNewTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);
            pcv.AddNew();
            pcv.CancelNew();
            Assert.AreEqual(1, pcv.Count);

            pcv.EditItem(efb);
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CancelNew", "EditItem")),
                delegate
                {
                    pcv.CancelNew();
                });
        }

        /// <summary>
        /// Validate inserting an item with CollectionChanged notification on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate inserting an item with CollectionChanged notification on the PagedCollectionView class.")]
        public void InsertTest()
        {
            ObservableCollection<int> ints = new ObservableCollection<int>() { 2, 3, 4 };
            PagedCollectionView cv = new PagedCollectionView(ints);
            ints.Insert(0, 1);

            string str = String.Empty;
            foreach (int i in cv)
            {
                str += i.ToString();
            }

            Assert.AreEqual("1234", str);
        }

        #endregion Add

        #region Edit

        /// <summary>
        /// Validate the IsEditingItem property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IsEditingItem property on the PagedCollectionView class.")]
        public void IsEditingItemTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);

            Assert.IsFalse(pcv.IsEditingItem);
            pcv.EditItem(efb);
            Assert.IsTrue(pcv.IsEditingItem);
            pcv.CancelEdit();
            Assert.IsFalse(pcv.IsEditingItem);
            pcv.EditItem(efb);
            Assert.IsTrue(pcv.IsEditingItem);
            pcv.CommitEdit();
        }

        /// <summary>
        /// Validate the CurrentEditItem property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CurrentEditItem property on the PagedCollectionView class.")]
        public void CurrentEditItemTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            PagedCollectionView pcv = new PagedCollectionView(tcList);

            object item = pcv.AddNew();
            Assert.AreEqual(item, pcv.CurrentAddItem);
        }

        /// <summary>
        /// Validate the EditItem method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is just being used to test against an exception being generated elsewhere; it is not itself being thrown.")]
        [Description("Validate the EditItem method on the PagedCollectionView class.")]
        public void EditItemTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);
            Assert.IsFalse(pcv.IsEditingItem);
            object item = pcv.AddNew();
            pcv.EditItem(item);
            Assert.IsFalse(pcv.IsEditingItem);
            pcv.CommitNew();

            pcv.AddNew();
            pcv.EditItem(efb);
            Assert.AreEqual(efb, pcv.CurrentEditItem);
            Assert.IsTrue(pcv.IsEditingItem);
            Assert.IsFalse(pcv.IsAddingNew);

            pcv.CommitEdit();
            AssertExpectedException(
                new ArgumentException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.CannotEditPlaceholder), "item"),
                delegate
                {
                    pcv.EditItem(PagedCollectionView.NewItemPlaceholder);
                });
        }

        /// <summary>
        /// Validate the EditItem method with an IEditableCollection on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is just being used to test against an exception being generated elsewhere; it is not itself being thrown.")]
        [Description("Validate the EditItem method with an IEditableCollection on the PagedCollectionView class.")]
        public void EditItemWithEditableCollectionTest()
        {
            TestEditableCollection<TestClass> ecList = new TestEditableCollection<TestClass>();
            ecList.Add(new TestClass { IntProperty = 1, StringProperty = "A" });
            ecList.Add(new TestClass { IntProperty = 1, StringProperty = "B" });
            ecList.Add(new TestClass { IntProperty = 3, StringProperty = "C" });
            ecList.Add(new TestClass { IntProperty = 3, StringProperty = "D" });
            ecList.Add(new TestClass { IntProperty = 4, StringProperty = "E" });

            Assert.IsTrue(ecList.CanEdit);

            PagedCollectionView cv = new PagedCollectionView(ecList);
            cv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));

            // should always be able to cancel for an IEditableCollection
            Assert.IsTrue(cv.CanCancelEdit);

            cv.PageSize = 2;
            cv.MoveToNextPage();
            Assert.AreEqual(1, cv.PageIndex);

            TestClass tc = cv[1] as TestClass;
            Assert.AreEqual("D", tc.StringProperty);

            cv.EditItem(tc);

            // verify that we cannot change pages until we commit the operation.
            Assert.IsFalse(cv.MoveToNextPage());
            Assert.IsFalse(cv.MoveToPreviousPage());

            // verify that the item is not moved yet.
            tc.IntProperty = 2;
            Assert.AreEqual(1, cv.IndexOf(tc));

            // verify that the item is moved after the commit and that we can now change pages.
            cv.CommitEdit();
            Assert.AreEqual(0, cv.IndexOf(tc));
            Assert.IsTrue(cv.MoveToNextPage());
            Assert.IsTrue(cv.MoveToPreviousPage());

            // verify that a cancel will revert to the old values
            cv.EditItem(tc);
            tc.IntProperty = 100;
            Assert.AreEqual(100, tc.IntProperty);
            cv.CancelEdit();
            Assert.AreEqual(2, tc.IntProperty);
        }

        /// <summary>
        /// Validate the EditItem method with an item that implements IEditableObject.
        /// </summary>
        [TestMethod]
        [Description("Validate the EditItem method with an item that implements IEditableObject.")]
        public void EditItemWithIEditableObject()
        {
            ObservableCollection<EditableTestClass> oc = new ObservableCollection<EditableTestClass>()
            {
                new EditableTestClass() { IntProperty = 1, StringProperty = "A" },
                new EditableTestClass() { IntProperty = 2, StringProperty = "B" },
                new EditableTestClass() { IntProperty = 3, StringProperty = "C" }
            };

            PagedCollectionView cv = new PagedCollectionView(oc);

            Assert.IsNull((cv[0] as EditableTestClass).DebugString);

            cv.EditItem(cv[0]);

            // verify that BeginEdit was called on the IEditable interface
            Assert.AreEqual("BeginEdit", (cv[0] as EditableTestClass).DebugString);

            cv.CancelEdit();

            // verify that CancelEdit was called on the IEditable interface
            Assert.AreEqual("CancelEdit", (cv[0] as EditableTestClass).DebugString);

            cv.EditItem(cv[0]);
            cv.CommitEdit();

            // verify that EndEdit was called on the IEditable interface
            Assert.AreEqual("EndEdit", (cv[0] as EditableTestClass).DebugString);

            // verify that when adding a new item, it will call BeginEdit on it
            EditableTestClass etc = cv.AddNew() as EditableTestClass;
            Assert.AreEqual("BeginEdit", etc.DebugString);

            // verify that when canceling the new item, it will call CancelEdit on it.
            cv.CancelNew();
            Assert.AreEqual("CancelEdit", etc.DebugString);

            // verify that when committing a new item, it will call EndEdit on it.
            etc = cv.AddNew() as EditableTestClass;
            cv.CommitNew();
            Assert.AreEqual("EndEdit", etc.DebugString);
        }

        /// <summary>
        /// Validate the CommitEdit method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CommitEdit method on the PagedCollectionView class.")]
        public void CommitEditTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);
            pcv.EditItem(efb);
            efb.IntProperty = 10;
            efb.StringProperty = "test";
            pcv.CommitEdit();
            Assert.AreEqual(10, (pcv[0] as EditableTestClass).IntProperty);
            Assert.AreEqual("test", (pcv[0] as EditableTestClass).StringProperty);

            pcv.AddNew();
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CommitEdit", "AddNew")),
                delegate
                {
                    pcv.CommitEdit();
                });
        }

        /// <summary>
        /// Validate the CanCancelEdit property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanCancelEdit property on the PagedCollectionView class.")]
        public void CanCancelEditTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv1 = new PagedCollectionView(efbList);
            pcv1.EditItem(efb);
            Assert.IsTrue(pcv1.CanCancelEdit);

            List<TestClass> tcList = new List<TestClass>();
            TestClass fb = new TestClass();
            tcList.Add(fb);

            PagedCollectionView pcv2 = new PagedCollectionView(tcList);
            pcv2.EditItem(fb);
            Assert.IsFalse(pcv2.CanCancelEdit);

            pcv1.AddNew();

            // This tests to see if cancelling an edit while adding a new item throws an exception.
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CancelEdit", "AddNew")),
                delegate
                {
                    pcv1.CancelEdit();
                });

            // This tests to see if cancelling an edit when you don't have an item being edited throws an exception.
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.CancelEditNotSupported)),
                delegate
                {
                    pcv2.CancelEdit();
                });
        }

        /// <summary>
        /// Validate the CancelEdit method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CancelEdit method on the PagedCollectionView class.")]
        public void CancelEditTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);
            pcv.EditItem(efb);
            Assert.IsTrue(efb.IsEditing);
            efb.IntProperty = 10;
            efb.StringProperty = "test";
            pcv.CancelEdit();
            Assert.AreEqual(0, (pcv[0] as EditableTestClass).IntProperty);
            Assert.AreEqual(null, (pcv[0] as EditableTestClass).StringProperty);
        }

        #endregion Edit

        #region Remove

        /// <summary>
        /// Validate the CanRemove property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanRemove property on the PagedCollectionView class.")]
        public void CanRemoveTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            PagedCollectionView pcv1 = new PagedCollectionView(efbList);

            Assert.IsTrue(pcv1.CanRemove);
            object item = pcv1.AddNew();
            Assert.IsFalse(pcv1.CanRemove);
            pcv1.CommitNew();
            Assert.IsTrue(pcv1.CanRemove);
            pcv1.EditItem(item);
            Assert.IsFalse(pcv1.CanRemove);
            pcv1.CommitEdit();
            Assert.IsTrue(pcv1.CanRemove);

            FixedSizeCollection<EditableTestClass> fsefbList = new FixedSizeCollection<EditableTestClass>();
            PagedCollectionView pcv2 = new PagedCollectionView(fsefbList);
            Assert.IsFalse(pcv2.CanRemove);
        }

        /// <summary>
        /// Validate the RemoveAt method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AtTest", Justification = "AtTest is correct - it is not 'Attest', nor is At superfluous or part of Hungarian notation.")]
        [Description("Validate the RemoveAt method on the PagedCollectionView class.")]
        public void RemoveAtTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);
            pcv.RemoveAt(0);
            Assert.AreEqual(0, pcv.Count);
            Assert.IsFalse(pcv.Contains(efb));
            efb = pcv.AddNew() as EditableTestClass;

            pcv.EditItem(efb);
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "RemoveAt")),
                delegate
                {
                    pcv.RemoveAt(0);
                });
            pcv.AddNew();
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "RemoveAt")),
                delegate
                {
                    pcv.RemoveAt(0);
                });

            List<object> objList = new List<object>();
            objList.Add(PagedCollectionView.NewItemPlaceholder);

            PagedCollectionView pcv2 = new PagedCollectionView(objList);
            AssertExpectedException(
                new InvalidOperationException(PagedCollectionViewResources.RemovingPlaceholder),
                delegate
                {
                    pcv2.RemoveAt(0);
                });
        }

        /// <summary>
        /// Validate the Remove method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Remove method on the PagedCollectionView class.")]
        public void RemoveTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            TestClass fb = new TestClass();
            tcList.Add(fb);

            PagedCollectionView pcv = new PagedCollectionView(tcList);
            pcv.Remove(fb);
            Assert.AreEqual(0, pcv.Count);
            Assert.IsFalse(pcv.Contains(fb));
        }

        /// <summary>
        /// Validate the Remove method with an IEditableCollection on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Remove method with an IEditableCollection on the PagedCollectionView class.")]
        public void RemoveWithEditableCollectionTest()
        {
            TestEditableCollection<TestClass> ecList = new TestEditableCollection<TestClass>();
            ecList.Add(new TestClass { IntProperty = 1, StringProperty = "A" });
            ecList.Add(new TestClass { IntProperty = 2, StringProperty = "B" });
            ecList.Add(new TestClass { IntProperty = 3, StringProperty = "C" });
            ecList.Add(new TestClass { IntProperty = 4, StringProperty = "D" });
            ecList.Add(new TestClass { IntProperty = 5, StringProperty = "E" });

            Assert.IsTrue(ecList.CanRemove);

            PagedCollectionView cv = new PagedCollectionView(ecList);
            cv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));

            cv.Remove(cv[0]);
            cv.RemoveAt(0);
            Assert.AreEqual(3, cv.Count);

            IEnumerator enumerator = ecList.GetEnumerator();
            Assert.AreEqual(true, enumerator.MoveNext());
            Assert.AreEqual("C", (enumerator.Current as TestClass).StringProperty);
        }

        /// <summary>
        /// Validate the Remove method on an invalid index.
        /// </summary>
        [TestMethod]
        [Description("Validate the Remove method on an invalid index.")]
        public void RemoveAtInvalidIndexTest()
        {
            List<TestClass> tcList = new List<TestClass>();

            PagedCollectionView pcv = new PagedCollectionView(tcList);

            // remove at an index < 0
            AssertExpectedException(
                new ArgumentOutOfRangeException("index", PagedCollectionViewResources.IndexOutOfRange),
                delegate
                {
                    pcv.RemoveAt(-1);
                });

            // remove at an index >= count
            AssertExpectedException(
                new ArgumentOutOfRangeException("index", PagedCollectionViewResources.IndexOutOfRange),
                delegate
                {
                    pcv.RemoveAt(1);
                });
        }

#if NewItemPlaceholderSupported
        /// <summary>
        /// Validate the Remove method on the PagedCollectionView class when there is a NewItemPlaceholder.
        /// </summary>
        [TestMethod]
        [Description("Validate the Remove method on the PagedCollectionView class when there is a NewItemPlaceholder.")]
        public void RemoveWithPlaceholderTest()
        {
            List<TestClass> tcList = new List<TestClass>() 
            { 
                new TestClass() { IntProperty = 1 }, new TestClass() { IntProperty = 2 },
                new TestClass() { IntProperty = 3 }, new TestClass() { IntProperty = 4 },
                new TestClass() { IntProperty = 5 }, new TestClass() { IntProperty = 6 },
                new TestClass() { IntProperty = 7 }, new TestClass() { IntProperty = 8 },
                new TestClass() { IntProperty = 9 }, new TestClass() { IntProperty = 10 } 
            };

            PagedCollectionView pcv = new PagedCollectionView(tcList);
            pcv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtEnd;

            Assert.AreEqual(11, pcv.Count);
            pcv.RemoveAt(0);
            Assert.AreEqual(10, pcv.Count);

            pcv.PageSize = 3;
            Assert.AreEqual(2, (pcv[0] as TestClass).IntProperty);
            Assert.AreEqual(3, (pcv[1] as TestClass).IntProperty);
            Assert.AreEqual(4, (pcv[2] as TestClass).IntProperty);
            pcv.RemoveAt(1);
            Assert.AreEqual(2, (pcv[0] as TestClass).IntProperty);
            Assert.AreEqual(4, (pcv[1] as TestClass).IntProperty);
            Assert.AreEqual(5, (pcv[2] as TestClass).IntProperty);

            AssertExpectedException(
                new InvalidOperationException(PagedCollectionViewResources.RemovingPlaceholder),
                delegate
                {
                    pcv.RemoveAt(3);
                });
        }
#endif
        #endregion Remove

        #region Replace

        /// <summary>
        /// Validate a replace operation on the source collection for the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate a replace operation on the source collection for the PagedCollectionView class.")]
        public void ReplaceTest()
        {
            ObservableCollection<TestClass> testCollection = new ObservableCollection<TestClass>()
            {
                new TestClass() { IntProperty = 1, StringProperty = "C" },
                new TestClass() { IntProperty = 2, StringProperty = "A" },
                new TestClass() { IntProperty = 3, StringProperty = "D" },
                new TestClass() { IntProperty = 4, StringProperty = "B" },
            };

            PagedCollectionView cv = new PagedCollectionView(testCollection);

            Assert.AreEqual("C", (cv[0] as TestClass).StringProperty);
            Assert.AreEqual("A", (cv[1] as TestClass).StringProperty);
            Assert.AreEqual("D", (cv[2] as TestClass).StringProperty);
            Assert.AreEqual("B", (cv[3] as TestClass).StringProperty);

            // replace first item
            testCollection[0] = new TestClass() { IntProperty = 5, StringProperty = "E" };

            // verify new data (still in same index)
            Assert.AreEqual("E", (cv[0] as TestClass).StringProperty);
            Assert.AreEqual("A", (cv[1] as TestClass).StringProperty);
            Assert.AreEqual("D", (cv[2] as TestClass).StringProperty);
            Assert.AreEqual("B", (cv[3] as TestClass).StringProperty);

            // now sort and page the data
            cv.PageSize = 2;
            cv.SortDescriptions.Add(new System.ComponentModel.SortDescription("StringProperty", System.ComponentModel.ListSortDirection.Ascending));

            // ----------------
            // |Page | 0  | 1  |
            // ----------------
            // | 0   | A  | B  | <---- Current Page
            // | 1   | D  | E  |
            Assert.AreEqual("A", (cv[0] as TestClass).StringProperty);
            Assert.AreEqual("B", (cv[1] as TestClass).StringProperty);

            // replace 'B', which is the 4th item in the collection
            testCollection[3] = new TestClass() { IntProperty = 6, StringProperty = "F" };

            // ----------------
            // |Page | 0  | 1  |
            // ----------------
            // | 0   | A  | D  | <---- Current Page
            // | 1   | E  | F  | (because of sorting/paging)
            Assert.AreEqual("A", (cv[0] as TestClass).StringProperty);
            Assert.AreEqual("D", (cv[1] as TestClass).StringProperty);
            cv.MoveToNextPage();
            Assert.AreEqual("E", (cv[0] as TestClass).StringProperty);
            Assert.AreEqual("F", (cv[1] as TestClass).StringProperty);
        }

        #endregion Replace

        #endregion IEditableCollectionView Tests

        #region IIndexableCollection Tests

        /// <summary>
        /// Validate the Index property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Index property on the PagedCollectionView class.")]
        public void IndexTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(1, pcv[0]);
            Assert.AreEqual(3, pcv[1]);
            Assert.AreEqual(5, pcv[2]);

            pcv.PageSize = 2;
            Assert.AreEqual(1, pcv[0]);
            Assert.AreEqual(3, pcv[1]);

            pcv.MoveToNextPage();
            Assert.AreEqual(5, pcv[0]);
        }

        #endregion IIndexableCollection Tests

        #region IEnumerable Tests

        /// <summary>
        /// Validate the GetEnumerator method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the GetEnumerator method on the PagedCollectionView class.")]
        public void GetEnumeratorTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView pcv = new PagedCollectionView(intList);

            int index = 0;

            // foreach implicitly calls GetEnumerator() and uses it to iterate through the items.
            foreach (int item in pcv)
            {
                Assert.IsTrue(index < 3);

                switch (index)
                {
                    case 0:
                        Assert.AreEqual(1, item);
                        break;
                    case 1:
                        Assert.AreEqual(3, item);
                        break;
                    case 2:
                        Assert.AreEqual(5, item);
                        break;
                }

                index++;
            }
        }

#if NewItemPlaceholderSupported
        /// <summary>
        /// Validate the GetEnumerator method on the PagedCollectionView class when a NewItemPlaceholder is applied.
        /// </summary>
        [TestMethod]
        [Description("Validate the GetEnumerator method on the PagedCollectionView class when a NewItemPlaceholder is applied.")]
        public void GetEnumeratorWithNewItemPlaceholderTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView cv = new PagedCollectionView(intList);
            cv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtBeginning;
            cv.PageSize = 2;

            int index = 0;

            // foreach implicitly calls GetEnumerator() and uses it to iterate through the items.
            foreach (object item in cv)
            {
                Assert.IsTrue(index < 3);

                switch (index)
                {
                    case 0:
                        Assert.AreEqual(PagedCollectionView.NewItemPlaceholder, item);
                        break;
                    case 1:
                        Assert.AreEqual(1, item);
                        break;
                    case 2:
                        Assert.AreEqual(3, item);
                        break;
                }

                index++;
            }

            // now go through the second page
            cv.MoveToNextPage();
            index = 0;
            foreach (object item in cv)
            {
                Assert.IsTrue(index < 2);

                switch (index)
                {
                    case 0:
                        Assert.AreEqual(PagedCollectionView.NewItemPlaceholder, item);
                        break;
                    case 1:
                        Assert.AreEqual(5, item);
                        break;
                }

                index++;
            }
        }
#endif
        #endregion IEnumerable Tests

        #region INotifyCollectionChanged Tests

        /// <summary>
        /// Validate the OnCollectionChanged method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the OnCollectionChanged method on the PagedCollectionView class.")]
        public void OnCollectionChangedTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            ObservableCollection<TestClass> fbCollection = new ObservableCollection<TestClass>();

            PagedCollectionView pcv1 = new PagedCollectionView(efbList);
            PagedCollectionView pcv2 = new PagedCollectionView(fbCollection);
            pcv1.CollectionChanged += new NotifyCollectionChangedEventHandler(this.PagedCollectionViewCollectionChanged);
            pcv2.CollectionChanged += new NotifyCollectionChangedEventHandler(this.PagedCollectionViewCollectionChanged);

            this._expectedAction = NotifyCollectionChangedAction.Reset;
            this.AssertExpectedEvent(delegate { pcv1.Refresh(); });

            this._expectedAction = NotifyCollectionChangedAction.Reset;
            this.AssertExpectedEvent(delegate { fbCollection.Clear(); });

            this._expectedAction = NotifyCollectionChangedAction.Add;
            this.AssertExpectedEvent(delegate { fbCollection.Add(new TestClass()); });

            EditableTestClass efb;
            this._expectedAction = NotifyCollectionChangedAction.Add;
            this.AssertExpectedEvent(delegate { efb = pcv1.AddNew() as EditableTestClass; });

            pcv1.CommitNew();

            // Add, then Cancel to fire a Remove
            this._expectedAction = NotifyCollectionChangedAction.Add;
            this.AssertExpectedEvent(delegate { pcv1.AddNew(); });

            this._expectedAction = NotifyCollectionChangedAction.Remove;
            this.AssertExpectedEvent(delegate { pcv1.CancelNew(); });

            // Set PageSize to 1 to Reset
            this._expectedAction = NotifyCollectionChangedAction.Reset;
            this.AssertExpectedEvent(delegate { pcv1.PageSize = 1; });

            // Remove an Item
            this._expectedAction = NotifyCollectionChangedAction.Remove;
            this.AssertExpectedEvent(delegate { pcv1.RemoveAt(0); });

            pcv2.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.PagedCollectionViewCollectionChanged);
            pcv1.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.PagedCollectionViewCollectionChanged);
        }

        #endregion INotifyCollectionChanged Tests

        #region INotifyPropertyChanged Tests

        /// <summary>
        /// Validate the OnPropertyChanged method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the OnPropertyChanged method on the PagedCollectionView class.")]
        public void OnPropertyChangedTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            ObservableCollection<TestClass> fbCollection = new ObservableCollection<TestClass>();

            PagedCollectionView pcv1 = new PagedCollectionView(efbList);
            PagedCollectionView pcv2 = new PagedCollectionView(fbCollection);
            pcv1.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);
            pcv2.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);

            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("Count");
            this._expectedPropertyNames.Add("IsEmpty");
            this._expectedPropertyNames.Add("IsCurrentAfterLast");
            this.AssertExpectedEvent(delegate { fbCollection.Add(new TestClass()); });
            this.CheckExpectedPropertyNamesFound();

            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("IsCurrentBeforeFirst");
            this._expectedPropertyNames.Add("CurrentPosition");
            this._expectedPropertyNames.Add("CurrentItem");
            this.AssertExpectedEvent(delegate { pcv2.MoveCurrentToFirst(); });
            this.CheckExpectedPropertyNamesFound();

            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("SortDescriptions");
            this.AssertExpectedEvent(delegate { pcv1.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending)); });
            this.CheckExpectedPropertyNamesFound();

            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("Culture");
            this.AssertExpectedEvent(delegate { pcv1.Culture = CultureInfo.InvariantCulture; });
            this.CheckExpectedPropertyNamesFound();

            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("Filter");
            this.AssertExpectedEvent(delegate { pcv2.Filter = new Predicate<object>(this.FilterTestClass5); });
            this.CheckExpectedPropertyNamesFound();

#if NewItemPlaceholderSupported
            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("NewItemPlaceholderPosition");
            this.AssertExpectedEvent(delegate { pcv1.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtEnd; });
            this.CheckExpectedPropertyNamesFound();
#endif
            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("PageIndex");
            this.AssertExpectedEvent(delegate { pcv1.MoveToPage(0); });
            this.CheckExpectedPropertyNamesFound();

            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("PageSize");
            this.AssertExpectedEvent(delegate { pcv1.PageSize = 10; });
            this.CheckExpectedPropertyNamesFound();

            pcv1.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);
            pcv2.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);
        }

        #endregion INotifyPropertyChanged Tests

        #region IPagedCollectionView Tests

        /// <summary>
        /// Validate the CanChangePage property on the IPagedCollection implementation.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the CanChangePage property on the IPagedCollection implementation.")]
        public void CanChangePageTest()
        {
            TestEditablePagedCollection<int> intList = new TestEditablePagedCollection<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.PageChanged += new EventHandler<EventArgs>(this.PagedCollectionViewPageChanged);

            EnqueueCallback(() =>
            {
                this.ResetPageChanged();
                Assert.AreEqual(-1, pcv.PageIndex);
                pcv.PageSize = 5;
                Assert.AreEqual(-1, pcv.PageIndex);
                Assert.IsTrue(intList.CanChangePage);
            });

            this.AssertPageChanged();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, pcv.PageIndex);
                intList.CanChangePage = false;
                Assert.IsFalse(intList.CanChangePage);
                Assert.IsFalse(pcv.MoveToLastPage());
                Assert.IsFalse(pcv.MoveToNextPage());
                Assert.IsFalse(pcv.MoveToPage(1));
                Assert.AreEqual(0, pcv.PageIndex);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the ItemCount property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the ItemCount property on the PagedCollectionView class.")]
        public void ItemCountTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(9, pcv.ItemCount);

            pcv.Filter = new Predicate<object>(this.FilterInt);
            Assert.AreEqual(4, pcv.ItemCount);

            pcv.PageSize = 5;
            Assert.AreEqual(4, pcv.ItemCount);
        }

        /// <summary>
        /// Validate the PageIndex property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageIndex property on the PagedCollectionView class.")]
        public void PageIndexTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(-1, pcv.PageIndex);

            pcv.PageSize = 5;
            pcv.MoveToNextPage();
            Assert.AreEqual(1, pcv.PageIndex);
        }

        /// <summary>
        /// Validate the MoveToFirstPage method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveToFirstPage method on the PagedCollectionView class.")]
        public void MoveToFirstPageTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);

            pcv.PageSize = 5;
            pcv.MoveToPage(1);
            pcv.MoveToFirstPage();
            Assert.AreEqual(0, pcv.PageIndex);
            pcv.MoveToPage(1);

            using (pcv.DeferRefresh())
            {
                AssertExpectedException(
                    new InvalidOperationException(PagedCollectionViewResources.NoCheckOrChangeWhenDeferred),
                    delegate
                    {
                        pcv.MoveToFirstPage();
                    });
            }
        }

        /// <summary>
        /// Validate the MoveToFirstPage method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveToFirstPage method on the PagedCollectionView class.")]
        public void MoveToLastPageTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);

            pcv.PageSize = 5;
            Assert.IsTrue(pcv.MoveToLastPage());
            Assert.AreEqual(1, pcv.PageIndex);
            pcv.MoveToPage(0);

            using (pcv.DeferRefresh())
            {
                AssertExpectedException(
                    new InvalidOperationException(PagedCollectionViewResources.NoCheckOrChangeWhenDeferred),
                    delegate
                    {
                        pcv.MoveToLastPage();
                    });
            }
        }

        /// <summary>
        /// Validate the MoveToNextPage method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveToNextPage method on the PagedCollectionView class.")]
        public void MoveToNextPageTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);

            pcv.PageSize = 5;
            Assert.IsTrue(pcv.MoveToNextPage());
            Assert.AreEqual(1, pcv.PageIndex);
            Assert.IsFalse(pcv.MoveToNextPage());
            pcv.MoveToPage(0);

            using (pcv.DeferRefresh())
            {
                AssertExpectedException(
                    new InvalidOperationException(PagedCollectionViewResources.NoCheckOrChangeWhenDeferred),
                    delegate
                    {
                        pcv.MoveToNextPage();
                    });
            }
        }

        /// <summary>
        /// Validate the MoveToPreviousPage method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveToPreviousPage method on the PagedCollectionView class.")]
        public void MoveToPreviousPageTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);

            pcv.PageSize = 5;
            pcv.MoveToPage(1);
            Assert.IsTrue(pcv.MoveToPreviousPage());
            Assert.AreEqual(0, pcv.PageIndex);
            Assert.IsFalse(pcv.MoveToPreviousPage());
            pcv.MoveToPage(1);

            using (pcv.DeferRefresh())
            {
                AssertExpectedException(
                    new InvalidOperationException(PagedCollectionViewResources.NoCheckOrChangeWhenDeferred),
                    delegate
                    {
                        pcv.MoveToPreviousPage();
                    });
            }
        }

        /// <summary>
        /// Validate the MoveToPage method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the MoveToPage method on the PagedCollectionView class.")]
        public void MoveToPageTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);

            pcv.PageSize = 5;
            Assert.IsTrue(pcv.MoveToPage(1));
            Assert.AreEqual(1, pcv.PageIndex);

            using (pcv.DeferRefresh())
            {
                AssertExpectedException(
                    new InvalidOperationException(PagedCollectionViewResources.NoCheckOrChangeWhenDeferred),
                    delegate
                    {
                        pcv.MoveToPage(0);
                    });
            }

            Assert.IsFalse(pcv.MoveToPage(-1));
            Assert.IsFalse(pcv.MoveToPage(2));
        }

        /// <summary>
        /// Validate that MoveToPage(0) works when there are no items.
        /// </summary>
        [TestMethod]
        [Description("Validate that MoveToPage(0) works when there are no items.")]
        public void MoveToPageZeroWithNoItemsTest()
        {
            List<int> intList = new List<int>();

            PagedCollectionView pcv = new PagedCollectionView(intList);

            pcv.PageSize = 5;
            pcv.MoveToPage(0);

            Assert.AreEqual(0, pcv.Count);
            Assert.AreEqual(0, pcv.PageIndex);
        }

        /// <summary>
        /// Validate the StartPageIndex property used with paging on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the StartPageIndex property used with paging on the PagedCollectionView class.")]
        public void StartPageIndexWithPagingTest()
        {
            TestEditablePagedCollection<int> sourceCollection = new TestEditablePagedCollection<int>();
            for (int i = 0; i < 25; i++)
            {
                sourceCollection.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(sourceCollection);
            pcv.PageChanged += new EventHandler<EventArgs>(this.PagedCollectionViewPageChanged);

            EnqueueCallback(() =>
            {
                pcv.PageSize = 5;
                sourceCollection.StartPageIndex = 10;
                sourceCollection.ItemCount = 75;
                this.ResetPageChanged();
                Assert.AreEqual(5, pcv.PageSize);
                Assert.AreEqual(10, sourceCollection.StartPageIndex);
                Assert.AreEqual(75, sourceCollection.ItemCount);
                Assert.IsTrue(pcv.MoveToPage(11));
                Assert.IsTrue(pcv.IsPageChanging);
            });

            this.AssertPageChanged();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(11, pcv.PageIndex);

                Assert.AreEqual(5, pcv[0]);
                Assert.AreEqual(6, pcv[1]);
                Assert.AreEqual(7, pcv[2]);
                Assert.AreEqual(8, pcv[3]);
                Assert.AreEqual(9, pcv[4]);

                Assert.IsTrue(pcv.MoveToPage(14));
                Assert.IsTrue(pcv.MoveToPage(15));
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the PageSize property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageSize property on the PagedCollectionView class.")]
        public void PageSizeTest()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(0, pcv.PageSize);

            pcv.PageSize = 5;
            Assert.AreEqual(5, pcv.PageSize);
            Assert.AreEqual(0, pcv.PageIndex);

            pcv.MoveToPage(1);
            pcv.PageSize = 0;
            Assert.AreEqual(-1, pcv.PageIndex);
        }

        #endregion IPagedCollectionView Tests

        #region Public Properties Tests

        /// <summary>
        /// Validate the IsEmpty property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IsEmpty property on the PagedCollectionView class.")]
        public void IsEmptyTest()
        {
            List<string> strList = new List<string>();
            PagedCollectionView pcv = new PagedCollectionView(strList);
            Assert.IsTrue(pcv.IsEmpty);

            strList.Add("Test 1");
            Assert.IsFalse(pcv.IsEmpty);
        }

        /// <summary>
        /// Validate the Count property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Count property on the PagedCollectionView class.")]
        public void CountTest()
        {
            List<string> strList = new List<string>();
            PagedCollectionView pcv1 = new PagedCollectionView(strList);

            Assert.AreEqual(0, pcv1.Count);

            strList.Add("Test 1");
            Assert.AreEqual(1, pcv1.Count);

            strList.Add("Test 2");
            Assert.AreEqual(2, pcv1.Count);

            List<int> intList = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                intList.Add(i);
            }

            PagedCollectionView pcv2 = new PagedCollectionView(intList);
            Assert.AreEqual(9, pcv2.Count);

            pcv2.PageSize = 5;
            Assert.AreEqual(5, pcv2.Count);

            pcv2.MoveToNextPage();
            Assert.AreEqual(4, pcv2.Count);
        }

#if NewItemPlaceholderSupported
        /// <summary>
        /// Validate the Count property on the PagedCollectionView class when a NewItemPlaceholder is applied.
        /// </summary>
        [TestMethod]
        [Description("Validate the Count property on the PagedCollectionView class when a NewItemPlaceholder is applied.")]
        public void CountWithNewItemPlaceholderTest()
        {
            List<string> strList = new List<string>();
            PagedCollectionView cv = new PagedCollectionView(strList);
            Assert.AreEqual(0, cv.Count);

            cv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtBeginning;
            Assert.AreEqual(1, cv.Count);

            cv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtEnd;
            Assert.AreEqual(1, cv.Count);

            cv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.None;
            Assert.AreEqual(0, cv.Count);
        }
#endif
        #endregion Public Properties Tests

        #region Public Methods Tests

        /// <summary>
        /// Validate the IndexOf method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IndexOf method on the PagedCollectionView class.")]
        public void IndexOfTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(0, pcv.IndexOf(1));
            Assert.AreEqual(1, pcv.IndexOf(3));
            Assert.AreEqual(2, pcv.IndexOf(5));

            pcv.PageSize = 2;
            Assert.AreEqual(0, pcv.IndexOf(1));
            Assert.AreEqual(1, pcv.IndexOf(3));
            Assert.AreEqual(-1, pcv.IndexOf(5));

            pcv.MoveToNextPage();
            Assert.AreEqual(-1, pcv.IndexOf(1));
            Assert.AreEqual(-1, pcv.IndexOf(3));
            Assert.AreEqual(0, pcv.IndexOf(5));
        }

#if NewItemPlaceholderSupported
        /// <summary>
        /// Validate the IndexOf method on the PagedCollectionView class when 
        /// we also apply a NewItemPlaceholderPosition.
        /// </summary>
        [TestMethod]
        [Description("Validate the IndexOf method on the PagedCollectionView class when we also apply a NewItemPlaceholderPosition.")]
        public void IndexWithNewItemPlaceholderTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView cv = new PagedCollectionView(intList);
            cv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtBeginning;

            Assert.AreEqual(0, cv.IndexOf(PagedCollectionView.NewItemPlaceholder));
            Assert.AreEqual(1, cv.IndexOf(1));
            Assert.AreEqual(2, cv.IndexOf(3));
            Assert.AreEqual(3, cv.IndexOf(5));

            cv.PageSize = 2;
            Assert.AreEqual(0, cv.IndexOf(PagedCollectionView.NewItemPlaceholder));
            Assert.AreEqual(1, cv.IndexOf(1));
            Assert.AreEqual(2, cv.IndexOf(3));
            Assert.AreEqual(-1, cv.IndexOf(5));

            cv.MoveToNextPage();
            Assert.AreEqual(0, cv.IndexOf(PagedCollectionView.NewItemPlaceholder));
            Assert.AreEqual(-1, cv.IndexOf(1));
            Assert.AreEqual(-1, cv.IndexOf(3));
            Assert.AreEqual(1, cv.IndexOf(5));

            cv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtEnd;
            cv.PageSize = 0;
            Assert.AreEqual(0, cv.IndexOf(1));
            Assert.AreEqual(1, cv.IndexOf(3));
            Assert.AreEqual(2, cv.IndexOf(5));
            Assert.AreEqual(3, cv.IndexOf(PagedCollectionView.NewItemPlaceholder));

            cv.PageSize = 2;
            Assert.AreEqual(0, cv.IndexOf(1));
            Assert.AreEqual(1, cv.IndexOf(3));
            Assert.AreEqual(2, cv.IndexOf(PagedCollectionView.NewItemPlaceholder));
            Assert.AreEqual(-1, cv.IndexOf(5));

            cv.MoveToNextPage();
            Assert.AreEqual(-1, cv.IndexOf(1));
            Assert.AreEqual(-1, cv.IndexOf(3));
            Assert.AreEqual(0, cv.IndexOf(5));
            Assert.AreEqual(1, cv.IndexOf(PagedCollectionView.NewItemPlaceholder));
        }
#endif

        /// <summary>
        /// Validate the GetItemAt method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AtTest", Justification = "AtTest is correct - it is not 'Attest', nor is At superfluous or part of Hungarian notation.")]
        [Description("Validate the GetItemAt method on the PagedCollectionView class.")]
        public void GetItemAtTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(1, pcv.GetItemAt(0));
            Assert.AreEqual(3, pcv.GetItemAt(1));
            Assert.AreEqual(5, pcv.GetItemAt(2));

            pcv.PageSize = 2;
            Assert.AreEqual(1, pcv.GetItemAt(0));
            Assert.AreEqual(3, pcv.GetItemAt(1));

            pcv.MoveToNextPage();
            Assert.AreEqual(5, pcv.GetItemAt(0));
        }

#if NewItemPlaceholderSupported
        /// <summary>
        /// Validate the GetItemAt method on the PagedCollectionView class when a NewItemPlaceholder is applied.
        /// </summary>
        [TestMethod]
        [Description("Validate the GetItemAt method on the PagedCollectionView class when a NewItemPlaceholder is applied.")]
        public void GetItemAtWithNewItemPlaceholderTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);
            intList.Add(5);

            PagedCollectionView cv = new PagedCollectionView(intList);
            Assert.AreEqual(1, cv.GetItemAt(0));
            Assert.AreEqual(3, cv.GetItemAt(1));
            Assert.AreEqual(5, cv.GetItemAt(2));

            cv.NewItemPlaceholderPosition = System.ComponentModel.NewItemPlaceholderPosition.AtBeginning;
            Assert.AreEqual(PagedCollectionView.NewItemPlaceholder, cv.GetItemAt(0));
            Assert.AreEqual(1, cv.GetItemAt(1));
            Assert.AreEqual(3, cv.GetItemAt(2));
            Assert.AreEqual(5, cv.GetItemAt(3));

            cv.PageSize = 2;
            Assert.AreEqual(PagedCollectionView.NewItemPlaceholder, cv.GetItemAt(0));
            Assert.AreEqual(1, cv.GetItemAt(1));
            Assert.AreEqual(3, cv.GetItemAt(2));

            cv.MoveToNextPage();
            Assert.AreEqual(PagedCollectionView.NewItemPlaceholder, cv.GetItemAt(0));
            Assert.AreEqual(5, cv.GetItemAt(1));
        }
#endif

        #endregion Public Methods Tests

        #region Protected Properties Tests

        /// <summary>
        /// Validate the IsCurrentInSync property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IsCurrentInSync property on the PagedCollectionView class.")]
        public void IsCurrentInSyncTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(3);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.CurrentChanged += new EventHandler(this.PagedCollectionViewCurrentChanged);
            pcv.CurrentChanging += new CurrentChangingEventHandler(this.PagedCollectionViewCurrentChanging);

            // This references IsCurrentInSync, which should make it so that CurrentChanged is not fired since IsCurrentInSync = true.
            this.AssertNoCurrencyEvents(delegate { pcv.MoveCurrentToPosition(0); });

            intList.Remove(1);

            // This references IsCurrentInSync, which should make it so that CurrentChanged is fired since IsCurrentInSync = false.
            this.AssertCurrencyEvents(delegate { pcv.MoveCurrentToPosition(0); });

            pcv.CurrentChanged -= new EventHandler(this.PagedCollectionViewCurrentChanged);
            pcv.CurrentChanging -= new CurrentChangingEventHandler(this.PagedCollectionViewCurrentChanging);
        }

        #endregion Protected Properties Tests

        #region Private Properties Tests

        /// <summary>
        /// Validate the IsCurrentInView property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the IsCurrentInView property on the PagedCollectionView class.")]
        public void IsCurrentInViewTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);

            PagedCollectionView pcv = new PagedCollectionView(intList);

            // MoveCurrentToPosition() returns the value of IsCurrentInView after the operation.
            Assert.IsTrue(pcv.MoveCurrentToPosition(0));
            Assert.IsFalse(pcv.MoveCurrentToPosition(-1));
        }

        #endregion Private Properties Tests

        #region Private Methods Tests

        /// <summary>
        /// Validate the SortDescriptionsChanged method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the SortDescriptionsChanged method on the PagedCollectionView class.")]
        public void SortDescriptionsChangedTest()
        {
            List<EditableTestClass> efbList = new List<EditableTestClass>();
            EditableTestClass efb = new EditableTestClass();
            efbList.Add(efb);

            PagedCollectionView pcv = new PagedCollectionView(efbList);

            // SortDescriptions.Clear() calls SortDescriptionsChanged().
            pcv.SortDescriptions.Clear();
            pcv.EditItem(efb);
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Sorting")),
                delegate
                {
                    pcv.SortDescriptions.Clear();
                });
            pcv.CancelEdit();
            pcv.AddNew();
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Sorting")),
                delegate
                {
                    pcv.SortDescriptions.Clear();
                });
        }

        /// <summary>
        /// Validate the SortList method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the SortList method on the PagedCollectionView class.")]
        public void SortListTest()
        {
            List<TestClass> tcList = new List<TestClass>();
            PagedCollectionView pcv = new PagedCollectionView(tcList);

            pcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));
        }

        /// <summary>
        /// Validate the GetItemType method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the GetItemType method on the PagedCollectionView class.")]
        public void GetItemTypeTest()
        {
            List<object> objList = new List<object>();
            objList.Add(null);

            PagedCollectionView cv1 = new PagedCollectionView(objList);

            // calling AddNew will check the ItemType
            Assert.AreEqual(typeof(object), cv1.AddNew().GetType());

            // we will now test with a non-generic list
            List<TestClass> testList = new List<TestClass>() { new TestClass() };
            Array array = testList.ToArray();
            PagedCollectionView cv2 = new PagedCollectionView(testList);

            // calling AddNew will check the ItemType
            Assert.AreEqual(typeof(TestClass), cv2.AddNew().GetType());
        }

        #endregion Private Methods Tests

        #region ListBox Interaction Tests

        /// <summary>
        /// Validate the Filter functionality when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the Filter functionality when hooked up to a ListBox.")]
        public void FilterWithListBoxTest()
        {
            TestEditableCollection<TestClass> ecList = new TestEditableCollection<TestClass>();
            ecList.Add(new TestClass { IntProperty = 1, StringProperty = "A" });
            ecList.Add(new TestClass { IntProperty = 2, StringProperty = "B" });
            ecList.Add(new TestClass { IntProperty = 3, StringProperty = "C" });
            ecList.Add(new TestClass { IntProperty = 4, StringProperty = "D" });
            ecList.Add(new TestClass { IntProperty = 5, StringProperty = "E" });

            PagedCollectionView cv = new PagedCollectionView(ecList);

            ListBox lb = new ListBox();
            lb.ItemsSource = cv;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    Assert.AreEqual(5, lb.Items.Count);

                    cv.Filter = this.FilterTestClass5;
                    Assert.AreEqual(1, lb.Items.Count);

                    cv.Filter = null;
                    Assert.AreEqual(5, lb.Items.Count);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the Reset event is fired correctly when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the Reset event is fired correctly when hooked up to a ListBox.")]
        public void ResetWithListBoxTest()
        {
            ObservableCollection<int> oc = new ObservableCollection<int>() { 1, 2, 4, 5 };
            PagedCollectionView cv = new PagedCollectionView(oc);

            ListBox lb = new ListBox();
            lb.ItemsSource = cv;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    Assert.AreEqual(4, lb.Items.Count);
                    cv.Refresh();

                    Assert.AreEqual(4, lb.Items.Count);
                    oc.Insert(2, 3);

                    Assert.AreEqual(5, lb.Items.Count);
                    cv.Refresh();

                    Assert.AreEqual(5, lb.Items.Count);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the AddNew functionality when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the AddNew functionality when hooked up to a ListBox.")]
        public void NewItemWithListBoxTest()
        {
            TestEditableCollection<TestClass> ecList = new TestEditableCollection<TestClass>();
            ecList.Add(new TestClass { IntProperty = 1, StringProperty = "A" });
            ecList.Add(new TestClass { IntProperty = 2, StringProperty = "B" });
            ecList.Add(new TestClass { IntProperty = 3, StringProperty = "C" });
            ecList.Add(new TestClass { IntProperty = 4, StringProperty = "D" });
            ecList.Add(new TestClass { IntProperty = 5, StringProperty = "E" });

            PagedCollectionView cv = new PagedCollectionView(ecList);
            cv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));
            cv.PageSize = 2;

            ListBox lb = new ListBox();
            lb.ItemsSource = cv;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 1  | 2  | <---- Current Page
                    // | 1   | 3  | 4  |
                    // | 2   | 5  |    |
                    Assert.AreEqual(2, lb.Items.Count);
                    cv.MoveToLastPage();

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 1  | 2  |
                    // | 1   | 3  | 4  |
                    // | 2   | 5  |    | <---- Current Page
                    Assert.AreEqual(1, lb.Items.Count);

                    TestClass item = cv.AddNew() as TestClass;
                    item.IntProperty = 6;
                    item.StringProperty = "F";
                    Assert.AreEqual(2, lb.Items.Count);
                    cv.CommitNew();

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 1  | 2  |
                    // | 1   | 3  | 4  |
                    // | 2   | 5  | 6  | <---- Current Page
                    Assert.AreEqual(5, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(6, (lb.Items[1] as TestClass).IntProperty);

                    cv.MoveToPage(1);
                    item = cv.AddNew() as TestClass;
                    item.StringProperty = "A";

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 1  | 2  |
                    // | 1   | 3  |-0- | <---- Current Page
                    // | 2   | 4  | 5  | 
                    // | 3   | 6  |    | 
                    Assert.AreEqual(0, (lb.Items[1] as TestClass).IntProperty);
                    cv.CommitNew();

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 0  | 1  |
                    // | 1   | 2  | 3  | <---- Current Page
                    // | 2   | 4  | 5  | 
                    // | 3   | 6  |    | 
                    Assert.AreEqual(2, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(3, (lb.Items[1] as TestClass).IntProperty);

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 0  | 1  |
                    // | 1   | 2  |-0- | <---- Current Page
                    // | 2   | 3  | 4  | 
                    // | 3   | 5  | 6  | 
                    item = cv.AddNew() as TestClass;
                    Assert.AreEqual(2, lb.Items.Count);
                    Assert.AreEqual(2, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(0, (lb.Items[1] as TestClass).IntProperty);

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 0  | 1  |
                    // | 1   | 2  | 3  | <---- Current Page
                    // | 2   | 4  | 5  | 
                    // | 3   | 6  |    | 
                    cv.CancelNew();
                    Assert.AreEqual(2, lb.Items.Count);
                    Assert.AreEqual(2, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(3, (lb.Items[1] as TestClass).IntProperty);

                    // now test with grouping as well
                    cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
                    item = cv.AddNew() as TestClass;

                    // ------------------
                    // |Page | 0   | 1   |
                    // ------------------
                    // | 0   | 0A  | 1A  |
                    // | 1   | 2B  | -0- | <---- Current Page
                    // | 2   | 3C  | 4D  | 
                    // | 3   | 5E  | 6F  | 

                    item.IntProperty = 5;
                    item.StringProperty = "D";
                    cv.CommitNew();

                    // ------------------
                    // |Page | 0   | 1   |
                    // ------------------
                    // | 0   | 0A  | 1A  |
                    // | 1   | 2B  | 3C  | <---- Current Page
                    // | 2   | 4D  |-5D- | 
                    // | 3   | 5E  | 6F  | 

                    Assert.AreEqual(2, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual("B", (lb.Items[0] as TestClass).StringProperty);
                    Assert.AreEqual(3, (lb.Items[1] as TestClass).IntProperty);
                    Assert.AreEqual("C", (lb.Items[1] as TestClass).StringProperty);

                    cv.MoveToNextPage();
                    Assert.AreEqual(4, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual("D", (lb.Items[0] as TestClass).StringProperty);
                    Assert.AreEqual(5, (lb.Items[1] as TestClass).IntProperty);
                    Assert.AreEqual("D", (lb.Items[1] as TestClass).StringProperty);

                    // call add/cancel to verify that our view will not change
                    item = cv.AddNew() as TestClass;
                    cv.CancelNew();
                    Assert.AreEqual(4, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual("D", (lb.Items[0] as TestClass).StringProperty);
                    Assert.AreEqual(5, (lb.Items[1] as TestClass).IntProperty);
                    Assert.AreEqual("D", (lb.Items[1] as TestClass).StringProperty);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the EditItem functionality when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the EditItem functionality when hooked up to a ListBox.")]
        public void EditItemWithListBoxTest()
        {
            TestEditableCollection<TestClass> ecList = new TestEditableCollection<TestClass>();
            ecList.Add(new TestClass { IntProperty = 1, StringProperty = "A" });
            ecList.Add(new TestClass { IntProperty = 2, StringProperty = "B" });
            ecList.Add(new TestClass { IntProperty = 3, StringProperty = "C" });
            ecList.Add(new TestClass { IntProperty = 4, StringProperty = "D" });
            ecList.Add(new TestClass { IntProperty = 5, StringProperty = "E" });

            PagedCollectionView cv = new PagedCollectionView(ecList);
            cv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));
            cv.PageSize = 2;

            ListBox lb = new ListBox();
            lb.ItemsSource = cv;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    cv.MoveToLastPage();

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 1  | 2  |
                    // | 1   | 3  | 4  |
                    // | 2   | 5  |    | <---- Current Page
                    Assert.AreEqual(5, (lb.Items[0] as TestClass).IntProperty);

                    TestClass item = cv[0] as TestClass;
                    cv.EditItem(cv[0]);
                    item.IntProperty = 0;

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 1  | 2  |
                    // | 1   | 3  | 4  |
                    // | 2   | 0  |    | <---- Current Page
                    Assert.AreEqual(0, (lb.Items[0] as TestClass).IntProperty);
                    cv.CommitEdit();

                    // ----------------
                    // |Page | 0  | 1  | (Commit => Sort)
                    // ----------------
                    // | 0   | 0  | 1  |
                    // | 1   | 2  | 3  |
                    // | 2   | 4  |    | <---- Current Page
                    Assert.AreEqual(4, (lb.Items[0] as TestClass).IntProperty);
                    cv.MoveToFirstPage();
                    Assert.AreEqual(0, (lb.Items[0] as TestClass).IntProperty);

                    cv.MoveToPage(1);
                    Assert.AreEqual(2, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(3, (lb.Items[1] as TestClass).IntProperty);

                    item = cv[0] as TestClass;
                    cv.EditItem(cv[0]);
                    item.IntProperty = 5;
                    cv.CommitEdit();

                    // ----------------
                    // |Page | 0  | 1  | (Commit => Sort)
                    // ----------------
                    // | 0   | 0  | 1  |
                    // | 1   | 3  | 4  | <---- Current Page
                    // | 2   | 5  |    |
                    Assert.AreEqual(3, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(4, (lb.Items[1] as TestClass).IntProperty);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the Remove functionality when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the Remove functionality when hooked up to a ListBox.")]
        public void RemoveItemWithListBoxTest()
        {
            TestEditableCollection<TestClass> ecList = new TestEditableCollection<TestClass>();
            ecList.Add(new TestClass { IntProperty = 1, StringProperty = "A" });
            ecList.Add(new TestClass { IntProperty = 2, StringProperty = "B" });
            ecList.Add(new TestClass { IntProperty = 3, StringProperty = "A" });
            ecList.Add(new TestClass { IntProperty = 4, StringProperty = "B" });
            ecList.Add(new TestClass { IntProperty = 5, StringProperty = "A" });

            PagedCollectionView cv = new PagedCollectionView(ecList);
            cv.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));
            cv.PageSize = 2;

            ListBox lb = new ListBox();
            lb.ItemsSource = cv;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    cv.MoveToLastPage();

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 1  | 2  |
                    // | 1   | 3  | 4  |
                    // | 2   | 5  |    | <---- Current Page
                    Assert.AreEqual(5, (lb.Items[0] as TestClass).IntProperty);

                    cv.Remove(cv[0]);

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 1  | 2  |
                    // | 1   | 3  | 4  | <---- Current Page
                    Assert.AreEqual(3, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(4, (lb.Items[1] as TestClass).IntProperty);

                    cv.MoveToFirstPage();
                    cv.RemoveAt(0);

                    // ----------------
                    // |Page | 0  | 1  |
                    // ----------------
                    // | 0   | 2  | 3  | <---- Current Page
                    // | 1   | 4  |    | 
                    Assert.AreEqual(2, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(3, (lb.Items[1] as TestClass).IntProperty);
                    cv.MoveToLastPage();
                    Assert.AreEqual(4, (lb.Items[0] as TestClass).IntProperty);

                    // now add a group description and test
                    cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
                    cv.MoveToFirstPage();

                    // ------------------
                    // |Page | 0   | 1   |
                    // ------------------
                    // | 0   | 2B  | 4B  | <---- Current Page
                    // | 1   | 3A  |     | 

                    Assert.AreEqual(2, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual(4, (lb.Items[1] as TestClass).IntProperty);
                    cv.MoveToNextPage();
                    Assert.AreEqual(1, cv.PageIndex);
                    Assert.AreEqual(3, (lb.Items[0] as TestClass).IntProperty);

                    // should trigger a page change
                    cv.RemoveAt(0);
                    Assert.AreEqual(0, cv.PageIndex);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the Add and Remove events are fired correctly from an ObservableCollection source when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the Add and Remove events are fired correctly from an ObservableCollection source when hooked up to a ListBox.")]
        public void AddRemoveEventsWithListBoxTest()
        {
            ObservableCollection<TestClass> collection = new ObservableCollection<TestClass>()
            {
                new TestClass { IntProperty = 1, StringProperty = "A" },
                new TestClass { IntProperty = 1, StringProperty = "C" },
                new TestClass { IntProperty = 2, StringProperty = "D" }
            };

            PagedCollectionView cv = new PagedCollectionView(collection);

            ListBox lb = new ListBox();
            lb.ItemsSource = cv;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    Assert.AreEqual(3, lb.Items.Count);

                    cv.AddNew();
                    cv.CommitNew();
                    Assert.AreEqual(4, lb.Items.Count);

                    cv.RemoveAt(3);
                    Assert.AreEqual(3, lb.Items.Count);

                    cv.SortDescriptions.Add(new System.ComponentModel.SortDescription("StringProperty", System.ComponentModel.ListSortDirection.Ascending));

                    TestClass newItem1 = new TestClass() { StringProperty = "B", IntProperty = 2 };
                    collection.Add(newItem1);

                    // Should have inserted into due to sorting
                    // {A, [B], C, D}
                    Assert.AreEqual(1, cv.IndexOf(newItem1));
                    Assert.AreEqual(1, lb.Items.IndexOf(newItem1));

                    cv.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));

                    // Should now be grouped as
                    // {1A, 1C}
                    // {2B, 2D}
                    Assert.AreEqual(2, lb.Items.IndexOf(newItem1));

                    TestClass newItem2 = new TestClass() { StringProperty = "E", IntProperty = 1 };
                    collection.Add(newItem2);

                    // Should have inserted into due here to sorting/grouping
                    // {1A, 1C, [1E]}
                    // {2B, 2D}
                    Assert.AreEqual(2, cv.IndexOf(newItem2));
                    Assert.AreEqual(2, lb.Items.IndexOf(newItem2));

                    // Testing that with sorting/grouping, item is removed from the correct index
                    cv.RemoveAt(1);
                    Assert.AreEqual(newItem2, lb.Items[1]);

                    // Test 'Replace' operation.
                    TestClass newItem3 = new TestClass() { StringProperty = "F", IntProperty = 2 };
                    TestClass replacedItem = collection[0];
                    collection[0] = newItem3;

                    // This operation should have deleted old and added new
                    // {[-deleted-], 1E}
                    // {2B, 2D, [2F]}
                    Assert.AreEqual(-1, lb.Items.IndexOf(replacedItem));
                    Assert.AreEqual(3, lb.Items.IndexOf(newItem3));
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate that events for the collection are fired correctly when Grouping is enabled and the collection is hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate that events for the collection are fired correctly when Grouping is enabled and the collection is hooked up to a ListBox.")]
        public void EventsWithGroupingInListBoxTest()
        {
            ObservableCollection<TestClass> collection = new ObservableCollection<TestClass>()
            {
                new TestClass { IntProperty = 1, StringProperty = "A" },
                new TestClass { IntProperty = 2, StringProperty = "B" }, 
                new TestClass { IntProperty = 3, StringProperty = "A" },
                new TestClass { IntProperty = 4, StringProperty = "B" } 
            };

            PagedCollectionView cv = new PagedCollectionView(collection);

            ListBox lb = new ListBox();
            lb.ItemsSource = cv;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    Assert.AreEqual(4, lb.Items.Count);

                    cv.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

                    Assert.AreEqual(4, lb.Items.Count);
                    Assert.AreEqual("A", (lb.Items[0] as TestClass).StringProperty);
                    Assert.AreEqual("A", (lb.Items[1] as TestClass).StringProperty);
                    Assert.AreEqual("B", (lb.Items[2] as TestClass).StringProperty);
                    Assert.AreEqual("B", (lb.Items[3] as TestClass).StringProperty);

                    cv.PageSize = 3;
                    cv.MoveToPage(1);

                    Assert.AreEqual(1, lb.Items.Count);
                    Assert.AreEqual("B", (lb.Items[0] as TestClass).StringProperty);

                    TestClass newItem = cv.AddNew() as TestClass;
                    newItem.StringProperty = "A";
                    newItem.IntProperty = 5;

                    // ---------------------
                    // |Page | 0  | 1  | 2  |
                    // ---------------------
                    // | 0   | A  | A  | B  |
                    // | 1   | B  |-A- |    | <---- Current Page
                    Assert.AreEqual("B", (lb.Items[0] as TestClass).StringProperty);
                    Assert.AreEqual("A", (lb.Items[1] as TestClass).StringProperty);
                    cv.CommitNew();

                    // ---------------------
                    // |Page | 0  | 1  | 2  |
                    // ---------------------
                    // | 0   | A  | A  |-A- |
                    // | 1   | B  | B  |    | <---- Current Page
                    Assert.AreEqual("B", (lb.Items[0] as TestClass).StringProperty);
                    Assert.AreEqual("B", (lb.Items[1] as TestClass).StringProperty);

                    cv.MoveToFirstPage();
                    Assert.AreEqual("A", (lb.Items[0] as TestClass).StringProperty);
                    Assert.AreEqual("A", (lb.Items[1] as TestClass).StringProperty);
                    Assert.AreEqual("A", (lb.Items[2] as TestClass).StringProperty);                    
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate moving to a previous page when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate moving to a previous page when hooked up to a ListBox.")]
        public void MoveToPreviousPageWithListBoxTest()
        {
            TestEditablePagedCollection<int> intList = new TestEditablePagedCollection<int>();
            intList.Add(6);
            intList.Add(7);
            intList.Add(8);
            intList.Add(9);
            intList.Add(10);

            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.PageChanged += new EventHandler<EventArgs>(this.PagedCollectionViewPageChanged);

            ListBox lb = new ListBox();
            lb.ItemsSource = pcv;
            
            EnqueueCallback(() =>
            {
                // assume the role of the DomainDataSource in providing 
                // "server-side" paging.
                intList.ItemCount = 5;
                intList.PageSize = 5;
                intList.StartPageIndex = 1;
                this.ResetPageChanged();
                pcv.MoveToPage(1);
            });

            this.AssertPageChanged();

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    Assert.AreEqual(5, lb.Items.Count);

                    // because the StartPageIndex (1) > PageIndex (0),
                    // we should not display any data until the server
                    // loads in new items.
                    pcv.MoveToPage(0);
                    Assert.IsTrue(pcv.IsPageChanging);
                });

            this.AssertPageChanged();

            EnqueueCallback(() =>
            {
                Assert.IsFalse(pcv.IsPageChanging);
                Assert.AreEqual(0, pcv.PageIndex);
                Assert.AreEqual(0, lb.Items.Count);

                // once the server starts to load the data, it will
                // set the StartPage index to a valid location.
                intList.Clear();
                intList.StartPageIndex = 0;
                intList.Add(1);
                intList.Add(2);
                intList.Add(3);
                intList.Add(4);
                intList.Add(5);
                Assert.AreEqual(5, lb.Items.Count);
            });

            EnqueueTestComplete();
        }

        #endregion ListBox Interaction Tests

        #region Internal Properties Tests

        /// <summary>
        /// Validate the VerifyRefreshNotDeferred method on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the VerifyRefreshNotDeferred method on the PagedCollectionView class.")]
        public void VerifyRefreshNotDeferredTest()
        {
            List<int> intList = new List<int>();
            intList.Add(1);

            PagedCollectionView pcv = new PagedCollectionView(intList);

            // Contains() calls VerifyRefreshNotDeferred().
            pcv.Contains(1);
            pcv.DeferRefresh();
            AssertExpectedException(
                new InvalidOperationException(PagedCollectionViewResources.NoCheckOrChangeWhenDeferred),
                delegate
                {
                    pcv.Contains(1);
                });
        }

        #endregion Internal Properties Tests

        #region Helper Functions

        /// <summary>
        /// Helper function that verifies that the test delegate raises the specified exception.
        /// </summary>
        /// <typeparam name="TException">Type of exception</typeparam>
        /// <param name="exceptionPrototype">Exception prototype, with the expected exception message populated.</param>
        /// <param name="test">Action delegate to expect exception from.</param>
        private static void AssertExpectedException<TException>(TException exceptionPrototype, Action test)
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
        /// Helper function used to filter out numbers smaller than 5.
        /// </summary>
        /// <param name="item">The item to consider for filtering.</param>
        /// <returns>Whether or not to filter <paramref name="item"/>.</returns>
        private bool FilterInt(object item)
        {
            try
            {
                int num = int.Parse(item.ToString(), CultureInfo.InvariantCulture);
                return num >= 5;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Helper function used to filter out numbers smaller than 5.
        /// </summary>
        /// <param name="item">The item to consider for filtering.</param>
        /// <returns>Whether or not to filter <paramref name="item"/>.</returns>
        private bool FilterTestClass5(object item)
        {
            TestClass fb = item as TestClass;
            if (fb == null)
            {
                return false;
            }
            else
            {
                return fb.IntProperty >= 5;
            }
        }

        /// <summary>
        /// Helper function that cancels a CurrentChanging event and flags that the event has been fired.
        /// </summary>
        /// <param name="sender">The object generating this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void CancelChanging(object sender, System.ComponentModel.CurrentChangingEventArgs e)
        {
            Assert.IsTrue(e.IsCancelable);
            e.Cancel = true;
            this._eventFired = true;
        }

        /// <summary>
        /// Helper function that allows a CurrentChanging event and flags that the event has been fired.
        /// </summary>
        /// <param name="sender">The object generating this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void AllowChanging(object sender, System.ComponentModel.CurrentChangingEventArgs e)
        {
            this._eventFired = true;
        }

        /// <summary>
        /// Helper function that flags that a CurrentChanging event has been fired.
        /// </summary>
        /// <param name="sender">The object generating this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void PagedCollectionViewCurrentChanging(object sender, System.ComponentModel.CurrentChangingEventArgs e)
        {
            this._currentChanging = true;
        }

        /// <summary>
        /// Helper function that flags that a CurrentChanged event has been fired.
        /// </summary>
        /// <param name="sender">The object generating this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void PagedCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            this._currentChanged = true;
        }

        /// <summary>
        /// Helper function that flags that a CollectionChanged event has been fired.
        /// </summary>
        /// <param name="sender">The object generating this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void PagedCollectionViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Assert.AreEqual(this._expectedAction, e.Action);
            this._eventFired = true;
        }

        /// <summary>
        /// Helper function that flags that a sequence of CollectionChanged events have been fired.
        /// </summary>
        /// <param name="sender">The object generating this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void PagedCollectionViewCollectionChangedSequence(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._expectedActionSequence != null && this._expectedActionSequence.Count > 0)
            {
                Assert.AreEqual(this._expectedActionSequence[0], e.Action);
                this._expectedActionSequence.RemoveAt(0);
            }
            else
            {
                Assert.Fail("Unexpected NotifyCollectionChanged event was detected.");
            }

            this._eventFired = true;
        }

        /// <summary>
        /// Helper function that flags that a PropertyChanged event has been fired by removing it from the expected list.
        /// </summary>
        /// <param name="sender">The object generating this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void PagedCollectionViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this._expectedPropertyNames.Remove(e.PropertyName);
            this._eventFired = true;
        }

        /// <summary>
        /// Helper function that flags that makes sure all expected property names were found in PropertyChanged events.
        /// </summary>
        private void CheckExpectedPropertyNamesFound()
        {
            Assert.AreEqual(0, this._expectedPropertyNames.Count);
        }

        /// <summary>
        /// Assert CurrentChanging and CurrentChanged event get fired
        /// </summary>
        /// <param name="test">The Action which should fire the events.</param>
        public void AssertCurrencyEvents(Action test)
        {
            this._currentChanging = false;
            this._currentChanged = false;
            test();
            Assert.IsTrue(this._currentChanging && this._currentChanged);
        }

        /// <summary>
        /// Helper function that verifies that the Action fires an event.
        /// </summary>
        /// <param name="test">The Action which should fire an event.</param>
        public void AssertExpectedEvent(Action test)
        {
            this._eventFired = false;
            test();
            Assert.IsTrue(this._eventFired);
        }

        /// <summary>
        /// Helper function that verifies that the Action does not fire an event.
        /// </summary>
        /// <param name="test">The Action which should not fire an event.</param>
        public void AssertNoEvent(Action test)
        {
            this._eventFired = false;
            test();
            Assert.IsFalse(this._eventFired);
        }

        /// <summary>
        /// Helper function that verifies that the Action does not fire the currency events.
        /// </summary>
        /// <param name="test">The Action which should fire the events.</param>
        public void AssertNoCurrencyEvents(Action test)
        {
            this._currentChanging = false;
            this._currentChanged = false;
            test();
            Assert.IsFalse(this._currentChanging || this._currentChanged);
        }

        /// <summary>
        /// Waits for the PageChanged event to be raised.
        /// </summary>
        private void AssertPageChanged()
        {
            EnqueueCallback(() => this._startedWaiting = DateTime.Now);
            EnqueueConditional(() => this._pcvPageChanged || (DateTime.Now - this._startedWaiting).TotalMilliseconds > DefaultWaitTimeout);

            EnqueueCallback(() =>
            {
                if (!this._pcvPageChanged)
                {
                    Assert.Fail("Timed out while waiting for PagedCollectionView to raise PageChanged.");
                }
            });
        }

        /// <summary>
        /// Called when the PagedCollectionView raises its PageChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PagedCollectionViewPageChanged(object sender, EventArgs e)
        {
            this._pcvPageChanged = true;
        }

        /// <summary>
        /// Clears the _pcvPageChanged flag
        /// </summary>
        private void ResetPageChanged()
        {
            this._pcvPageChanged = false;
        }

        /// <summary>
        /// Add an element to the test surface and perform a series of test
        /// actions with a pause in between each allowing the test surface to be
        /// updated.  This task does not complete the async test and a call to
        /// EnqueueTestCompleted is still required.
        /// </summary>
        /// <param name="element">Element that we are creating the task for.</param>
        /// <param name="actions">Test actions.</param>
        /// <remarks>
        /// CreateAsyncTask should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected void CreateAsyncTask(FrameworkElement element, params Action[] actions)
        {
            Assert.IsNotNull(element);
            actions = actions ?? new Action[] { };

            // Add a handler to determine when the element is loaded
            bool isLoaded = false;
            element.Loaded += delegate { isLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            TestPanel.Children.Add(element);
            EnqueueConditional(() => isLoaded);

            // Perform the test actions
            foreach (Action action in actions)
            {
                Action capturedAction = action;
                EnqueueCallback(() => capturedAction());
                EnqueueDelay(VisualDelayInMilliseconds);
            }

            // Remove the element from the test surface and finish the test
            EnqueueCallback(() => TestPanel.Children.Remove(element));
        }

        /// <summary>
        /// Add an element to the test surface and perform a series of test
        /// actions with a pause in between each allowing the test surface to be
        /// updated.
        /// </summary>
        /// <param name="element">Element that we are creating the task for.</param>
        /// <param name="actions">Test actions.</param>
        /// <remarks>
        /// CreateAsyncTest should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected void CreateAsyncTest(FrameworkElement element, params Action[] actions)
        {
            this.CreateAsyncTask(element, actions);
            EnqueueTestComplete();
        }

        #endregion Helper Functions
    }
}
