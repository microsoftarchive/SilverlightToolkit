//-----------------------------------------------------------------------
// <copyright file="MiscellaneousTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Controls.Test;
    using System.Windows.Data;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs unit tests for various features in the PagedCollectionView that
    /// require a special setup that does not require multiple datasources to run through
    /// the same tests.
    /// </summary>
    [TestClass]
    public class PagedCollectionView_MiscellaneousTests : SilverlightControlTest
    {
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
        /// Validate conditions in which we are not allowed to add new items.
        /// </summary>
        [TestMethod]
        [Description("Validate conditions in which we are not allowed to add new items.")]
        public void CannotAddNewTest()
        {
            // if the size of the collection is fixed, we are not allowed to add items to it.
            FixedSizeCollection<TestClass> fixedSizeCollection = new FixedSizeCollection<TestClass>();
            PagedCollectionView pcv1 = new PagedCollectionView(fixedSizeCollection);
            Assert.IsFalse(pcv1.CanAddNew);

            // if the item type does not have a constructor, we are not allowed to call AddNew.
            List<int> intList = new List<int>();
            PagedCollectionView pcv2 = new PagedCollectionView(intList);
            Assert.IsFalse(pcv2.CanAddNew);

            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedForView, "AddNew")),
                delegate
                {
                    pcv2.AddNew();
                });
        }

        /// <summary>
        /// Validate the NewItemPlaceholderPosition property.
        /// </summary>
        [TestMethod]
        [Description("Validate the NewItemPlaceholderPosition property.")]
        public void CannotChangeNewItemPlaceholderPositionTests()
        {
            List<TestClass> intList = new List<TestClass>() { new TestClass() };
            PagedCollectionView pcv = new PagedCollectionView(intList);

            Assert.AreEqual(NewItemPlaceholderPosition.None, pcv.NewItemPlaceholderPosition);

            PagedCollectionViewTest.AssertExpectedException(
                new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, 
                            PagedCollectionViewResources.InvalidEnumArgument, 
                            "value",
                            "123",
                            typeof(NewItemPlaceholderPosition).Name)),
                delegate
                {
                    pcv.NewItemPlaceholderPosition = (NewItemPlaceholderPosition)123;
                });
        }

        /// <summary>
        /// Verify that we throw an exception if we try to change PageSize while adding or editing an item.
        /// </summary>
        [TestMethod]
        [Description("Verify that we throw an exception if we try to change PageSize while adding or editing an item.")]
        public void CannotChangePageSize()
        {
            ObservableCollection<TestClass> collection = new ObservableCollection<TestClass>() { new TestClass() };
            PagedCollectionView pcv = new PagedCollectionView(collection);

            // show that we will throw an exception if we try to change the PageSize while adding
            pcv.AddNew();
            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(PagedCollectionViewResources.ChangingPageSizeNotAllowedDuringAddOrEdit),
                delegate
                {
                    pcv.PageSize = 10;
                });
            pcv.CancelNew();

            // show that we will throw an exception if we try to change the PageSize while editing
            pcv.EditItem(pcv[0]);
            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(PagedCollectionViewResources.ChangingPageSizeNotAllowedDuringAddOrEdit),
                delegate
                {
                    pcv.PageSize = 10;
                });
        }

        /// <summary>
        /// Verify that we throw an exception if we try to DeferRefresh while adding or editing an item.
        /// </summary>
        [TestMethod]
        [Description("Verify that we throw an exception if we try to DeferRefresh while adding or editing an item.")]
        public void CannotDeferRefresh()
        {
            ObservableCollection<TestClass> collection = new ObservableCollection<TestClass>() { new TestClass() };
            PagedCollectionView pcv = new PagedCollectionView(collection);

            // show that we will throw an exception if we try to change the PageSize while adding
            pcv.AddNew();
            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "DeferRefresh")),
                delegate
                {
                    pcv.DeferRefresh();
                });
            pcv.CancelNew();

            // show that we will throw an exception if we try to change the PageSize while editing
            pcv.EditItem(pcv[0]);
            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "DeferRefresh")),
                delegate
                {
                    pcv.DeferRefresh();
                });
        }

        /// <summary>
        /// Verify that our collection supports filtering.
        /// </summary>
        [TestMethod]
        [Description("Verify that our collection supports filtering.")]
        public void CanFilterTest()
        {
            PagedCollectionView pcv = new PagedCollectionView(new List<int>());
            Assert.IsTrue(pcv.CanFilter);
        }

        /// <summary>
        /// Validate that page moves are not allowed when PageSize is zero.
        /// </summary>
        [TestMethod]
        [Description("Validate that page moves are not allowed when PageSize is zero.")]
        public void CannotMoveToPageTest()
        {
            List<TestClass> intList = new List<TestClass>();
            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(0, pcv.PageSize);

            // if the PageSize is 0, you cannot move to page 0
            Assert.IsFalse(pcv.MoveToPage(0));

            // setting the PageSize to a positive number causes a 
            // move to page 0.
            pcv.PageSize = 1;
            Assert.AreEqual(0, pcv.PageIndex);
        }

        /// <summary>
        /// Validate conditions in which we are not allowed to refresh items.
        /// </summary>
        [TestMethod]
        [Description("Validate conditions in which we are not allowed to refresh items.")]
        public void CannotRefreshTest()
        {
            ObservableCollection<TestClass> collection = new ObservableCollection<TestClass>() { new TestClass() };
            PagedCollectionView pcv = new PagedCollectionView(collection);

            // show that we will throw an exception if we try to change the PageSize while adding
            pcv.AddNew();
            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Refresh")),
                delegate
                {
                    pcv.Refresh();
                });
            pcv.CancelNew();

            // show that we will throw an exception if we try to change the PageSize while editing
            pcv.EditItem(pcv[0]);
            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Refresh")),
                delegate
                {
                    pcv.Refresh();
                });
        }

        /// <summary>
        /// Validate conditions in which we are not allowed to remove items.
        /// </summary>
        [TestMethod]
        [Description("Validate conditions in which we are not allowed to remove items.")]
        public void CannotRemoveTest()
        {
            // if the size of the collection is fixed, we are not allowed to add items to it.
            FixedSizeCollection<TestClass> fixedSizeCollection = new FixedSizeCollection<TestClass>();
            PagedCollectionView pcv = new PagedCollectionView(fixedSizeCollection);
            Assert.IsFalse(pcv.CanRemove);
        }

        /// <summary>
        /// Validate conditions in which we are not allowed to sort items.
        /// </summary>
        [TestMethod]
        [Description("Validate conditions in which we are not allowed to sort items.")]
        public void CannotSortTest()
        {
            List<TestClass> intList = new List<TestClass>() { new TestClass() };
            PagedCollectionView pcv = new PagedCollectionView(intList);

            // we are not allowed to sort during an edit operation
            pcv.EditItem(pcv[0]);
            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Sorting")),
                delegate
                {
                    pcv.SortDescriptions.Clear();
                });
            pcv.CommitEdit();

            // we are not allowed to sort during an add new operation
            pcv.AddNew();
            PagedCollectionViewTest.AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "Sorting")),
                delegate
                {
                    pcv.SortDescriptions.Clear();
                });
        }

        /// <summary>
        /// Validate that the groups get updated when we update the PageSize.
        /// </summary>
        [TestMethod]
        [Description("Validate that the groups get updated when we update the PageSize")]
        public void ChangePageSizeWithGroupingTest()
        {
            List<int> intList = new List<int>() { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };
            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.GroupDescriptions.Add(new PropertyGroupDescription(""));
            Assert.AreEqual(10, pcv.Count);

            // set the initial PageSize
            pcv.PageSize = 5;
            Assert.AreEqual(5, pcv.Count);

            // change the PageSize and check that we update the
            // groups - which in turn updates the count.
            pcv.PageSize = 7;
            Assert.AreEqual(7, pcv.Count);

            // change the PageSize to a smaller number and 
            // test again
            pcv.PageSize = 3;
            Assert.AreEqual(3, pcv.Count);
        }

        /// <summary>
        /// Validate that the Count property always shows zero if we have no items.
        /// </summary>
        [TestMethod]
        [Description("Validate that the Count property always shows zero if we have no items.")]
        public void CountTestWithNoItems()
        {
            List<int> intList = new List<int>();
            PagedCollectionView pcv = new PagedCollectionView(intList);
            Assert.AreEqual(0, pcv.Count);

            // update the PageSize and verify that we still have a Count of 0.
            pcv.PageSize = 10;
            Assert.AreEqual(0, pcv.Count);

            // update the PageSize during a DeferRefresh and verify that we still have a Count of 0.
            pcv.PageSize = 0;
            using (pcv.DeferRefresh())
            {
                pcv.PageSize = 10;
                pcv.MoveToPage(1);
            }
            Assert.AreEqual(0, pcv.Count);

            // now try those same tests above with grouping
            pcv.GroupDescriptions.Add(new PropertyGroupDescription(""));
            pcv.PageSize = 0;
            Assert.AreEqual(0, pcv.Count);

            // update the PageSize and verify that we still have a Count of 0.
            pcv.PageSize = 10;
            Assert.AreEqual(0, pcv.Count);

            // update the PageSize during a DeferRefresh and verify that we still have a Count of 0.
            pcv.PageSize = 0;
            using(pcv.DeferRefresh())
            {
                pcv.PageSize = 5;
            }
            Assert.AreEqual(0, pcv.Count);
        }

        /// <summary>
        /// Validate the Culture property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is just being used to test against an exception being generated elsewhere; it is not itself being thrown.")]
        [Description("Validate the Culture property on the PagedCollectionView class.")]
        public void CultureTest()
        {
            // verify that Culture is set to null initially
            PagedCollectionView pcv = new PagedCollectionView(new List<int>());
            Assert.IsNull(pcv.Culture);

            // validate setting and retrieving the property
            pcv.Culture = CultureInfo.InvariantCulture;
            Assert.AreEqual(CultureInfo.InvariantCulture, pcv.Culture);

            pcv.Culture = CultureInfo.CurrentUICulture;
            Assert.AreEqual(CultureInfo.CurrentUICulture, pcv.Culture);

            // verify that if we try to set the value to null after it
            // has been set, we get an exception
            PagedCollectionViewTest.AssertExpectedException(
                new ArgumentNullException("value"),
                delegate
                {
                    pcv.Culture = null;
                });
        }

        /// <summary>
        /// Validate that when grouping is enabled and the collection is hooked up to a ListBox, updates are handled correctly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate that when grouping is enabled and the collection is hooked up to a ListBox, updates are handled correctly.")]
        public void GroupingInListBoxTest()
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
        /// Verify that if two classes implement the same property, that we can still sort on this property.
        /// </summary>
        [TestMethod]
        [Description("Verify that if two classes implement the same property, that we can still sort on this property.")]
        public void InsertWithSortingOnUnsharedPropertyTest()
        {
            // we will create a collection of type object, with instances of ClassA and ClassB,
            // which are not related classes. however, they share a property of the same name and type
            ObservableCollection<object> collection = new ObservableCollection<object>()
            {
                new ClassA(){IntProperty=1, SomeProperty="A"},
                new ClassB(){IntProperty=3, SomeProperty=true},
                new ClassA(){IntProperty=5, SomeProperty="B"},
                new ClassB(){IntProperty=7, SomeProperty=false},
            };

            PagedCollectionView pcv = new PagedCollectionView(collection);

            // first verify that we can sort
            pcv.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Descending));

            Assert.AreEqual(typeof(ClassB), pcv[0].GetType());
            Assert.AreEqual(typeof(ClassA), pcv[1].GetType());
            Assert.AreEqual(typeof(ClassB), pcv[2].GetType());
            Assert.AreEqual(typeof(ClassA), pcv[3].GetType());

            // next insert an item and verify that it gets correctly inserted based on
            // the correct sort index
            ClassA addItem = new ClassA() { IntProperty = 4, SomeProperty = "C" };
            collection.Add(addItem);
            Assert.AreEqual(2, pcv.IndexOf(addItem));

            // now test on the other property that has the same name
            // but different type. we should not get an error, and just
            // treat the variable as a null or default value
            pcv.SortDescriptions.Clear();
            pcv.SortDescriptions.Add(new SortDescription("SomeProperty", ListSortDirection.Ascending));

            Assert.AreEqual("3 True", pcv[0].ToString());
            Assert.AreEqual("7 False", pcv[1].ToString()); 
            Assert.AreEqual("1 A", pcv[2].ToString());
            Assert.AreEqual("5 B", pcv[3].ToString());
            Assert.AreEqual("4 C", pcv[4].ToString());
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
            this.AssertExpectedEvent(delegate { pcv2.Filter = new Predicate<object>(this.FilterNegativeNumbers); });
            this.CheckExpectedPropertyNamesFound();

            // Attempt to move to Page 0 should fail while PageSize is still 0.
            Assert.AreEqual(0, pcv1.PageSize);
            Assert.IsFalse(pcv1.MoveToPage(0));

            this._expectedPropertyNames.Clear();
            this._expectedPropertyNames.Add("PageSize");
            this.AssertExpectedEvent(delegate { pcv1.PageSize = 10; });
            this.CheckExpectedPropertyNamesFound();

            pcv1.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);
            pcv2.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.PagedCollectionViewPropertyChanged);
        }

        /// <summary>
        /// Validate the Refresh method on the PagedCollectionView class with a source that does not implement IList
        /// </summary>
        [TestMethod]
        [Description("Validate the Refresh method on the PagedCollectionView class with a source that does not implement IList.")]
        public void RefreshWithNonIListSourceTest()
        {
            TestEnumerableCollection list = new TestEnumerableCollection();
            list.Add(new TestClass { IntProperty = 1, StringProperty = "Test 1" });
            list.Add(new TestClass { IntProperty = 2, StringProperty = "Test 2" });

            PagedCollectionView pcv = new PagedCollectionView(list);
            pcv.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            Assert.AreEqual(2, pcv.Count);

            // add items to list and Refresh
            list.Add(new TestClass() { IntProperty = 3, StringProperty = "Test 3" });
            list.Add(new TestClass() { IntProperty = 4, StringProperty = "Test 4" });
            pcv.Refresh();
            Assert.AreEqual(4, pcv.Count);

            // remove items from list and Refresh
            list.RemoveAt(0);
            list.RemoveAt(0);
            pcv.Refresh();
            Assert.AreEqual(2, pcv.Count);
        }

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
        /// Set SortDescriptions and GroupDescriptions on the same nullable property.
        /// </summary>
        [TestMethod]
        [Description("Set SortDescriptions and GroupDescriptions on the same nullable property.")]
        public void SortAndGroupOnSameNullableProperty()
        {
            ObservableCollection<object> collection = new ObservableCollection<object>()
            {
                new ClassA(){NullableIntProperty=4, SomeProperty="A"},
                new ClassA(){NullableIntProperty=2, SomeProperty="B"},
                new ClassA(){NullableIntProperty=1, SomeProperty="C"},
                new ClassA(){NullableIntProperty=3, SomeProperty="D"},
            };

            PagedCollectionView pcv = new PagedCollectionView(collection);

            // add a sort description and a group description on the same nullable property
            pcv.SortDescriptions.Add(new SortDescription("NullableIntProperty", ListSortDirection.Ascending));
            pcv.GroupDescriptions.Add(new PropertyGroupDescription("NullableIntProperty"));

            // verify that the items appear in the correct order
            Assert.AreEqual("0 C", pcv[0].ToString());
            Assert.AreEqual("0 B", pcv[1].ToString());
            Assert.AreEqual("0 D", pcv[2].ToString());
            Assert.AreEqual("0 A", pcv[3].ToString());
        }

        /// <summary>
        /// Check that the SortFieldComparer will find the correct index to insert into.
        /// </summary>
        [TestMethod]
        [Description("Check that the SortFieldComparer will find the correct index to insert into.")]
        public void SortFieldComparerInsertIndex()
        {
            // create a sorted list of integers
            ObservableCollection<int> intList = new ObservableCollection<int>() { 1, 3, 5, 7, 9 };
            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.SortDescriptions.Add(new SortDescription(String.Empty, ListSortDirection.Ascending));

            // now check the SortFieldComparer's FindInsertIndex method to see if it would tell
            // us the correct index to insert into
            System.Windows.Data.PagedCollectionView.SortFieldComparer comparer =
                new PagedCollectionView.SortFieldComparer(pcv);
            Assert.AreEqual(1, comparer.FindInsertIndex(2, intList));
            Assert.AreEqual(2, comparer.FindInsertIndex(4, intList));
            Assert.AreEqual(3, comparer.FindInsertIndex(6, intList));
            Assert.AreEqual(4, comparer.FindInsertIndex(8, intList));
        }

        /// <summary>
        /// Verify that we can sort on a type directly by passing in an empty string.
        /// </summary>
        [TestMethod]
        [Description("Verify that we can sort on a type directly by passing in an empty string.")]
        public void SortingOnEmptyPropertyPathTest()
        {
            List<int> intList = new List<int>() { 5, 2, 4, 1, 3, 0 };

            // sort with an empty string as the path to sort directly on the type
            PagedCollectionView pcv = new PagedCollectionView(intList);
            pcv.SortDescriptions.Add(new SortDescription(String.Empty, ListSortDirection.Ascending));

            for (int i = 0; i < pcv.Count; i++)
            {
                Assert.AreEqual(i, (int)pcv[i]);
            }
        }

        /// <summary>
        /// Verify that we treat properties we cannot find as a null/default value.
        /// </summary>
        [TestMethod]
        [Description("Verify that we treat properties we cannot find as a null/default value.")]
        public void SortingOnNonExistentPropertyTest()
        {
            // we will create a collection of type object, with instances of ClassA and ClassB,
            // which are not related classes. however, they share a property of the same name and type
            ObservableCollection<object> collection = new ObservableCollection<object>()
            {
                new ClassA(){IntProperty=1, SomeProperty="A"},
                new ClassB(){IntProperty=3, SomeProperty=true},
                new ClassA(){IntProperty=5, SomeProperty="B"},
                new ClassB(){IntProperty=7, SomeProperty=false},
            };

            PagedCollectionView pcv = new PagedCollectionView(collection);

            // since this property is not defined, we should treat the property values
            // as null and so the items should stay in the same order
            pcv.SortDescriptions.Add(new SortDescription("NonExistentProperty", ListSortDirection.Descending));
            Assert.AreEqual("1 A", pcv[0].ToString());
            Assert.AreEqual("3 True", pcv[1].ToString());
            Assert.AreEqual("5 B", pcv[2].ToString());
            Assert.AreEqual("7 False", pcv[3].ToString());

            pcv.SortDescriptions.Clear();

            // add a null item and try to sort. we will still allow sorting,
            // but the value will come out as null, so it will be appended at
            // the beginning
            collection.Add(null);
            pcv.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            Assert.IsNull(pcv[0]);
            Assert.AreEqual("1 A", pcv[1].ToString());
            Assert.AreEqual("3 True", pcv[2].ToString());
            Assert.AreEqual("5 B", pcv[3].ToString());
            Assert.AreEqual("7 False", pcv[4].ToString());
        }

        /// <summary>
        /// Verify that when we apply a Culture to the PagedCollectionView, that it will correctly apply that culture when sorting.
        /// </summary>
        /// <remarks>
        /// This test is being ignored because of failures on the Mac OS 10.4 (Tiger) builds. I have verified that this build did 
        /// not support the culture/language that we are testing against, which causes CinCh to fail when the test is run against
        /// a Tiger machine. I have opened bug 81918 for this issue. We will need to update this test to use a different culture
        /// that is supported.
        /// </remarks>
        [TestMethod]
        [Ignore]
        [Description("Verify that when we apply a Culture to the PagedCollectionView, that it will correctly apply that culture when sorting.")]
        public void SortingWithLocalizationTest()
        {
            ObservableCollection<object> list = new ObservableCollection<object>() { "al:tinda", "ch:aque", "Cz:ech", "co:te", "hi:zli", "i:erigiyle" };

            PagedCollectionView view = new PagedCollectionView(list);

            // first test with the default InvariantCulture to see that it sorts in the correct order
            view.Culture = CultureInfo.InvariantCulture;
            view.SortDescriptions.Add(new SortDescription("", ListSortDirection.Descending));

            Assert.AreEqual(view[0], "i:erigiyle");
            Assert.AreEqual(view[1], "hi:zli");
            Assert.AreEqual(view[2], "Cz:ech");
            Assert.AreEqual(view[3], "co:te");
            Assert.AreEqual(view[4], "ch:aque");
            Assert.AreEqual(view[5], "al:tinda");

            // now test with a Slovik culture applied to make sure that it sorts in the correct order
            view.Culture = new CultureInfo("sk-SK");
            view.Refresh();

            Assert.AreEqual(view[0], "i:erigiyle");
            Assert.AreEqual(view[1], "ch:aque");
            Assert.AreEqual(view[2], "hi:zli");
            Assert.AreEqual(view[3], "Cz:ech");
            Assert.AreEqual(view[4], "co:te");
            Assert.AreEqual(view[5], "al:tinda");

            // now remove the second item and re-insert to verify that sorting with the CultureInfo 
            // allows it to be placed back in the same index
            view.RemoveAt(1);
            string str = "ch:aque";
            list.Add(str);
            Assert.AreEqual(str, view[1]);
        }

        #region UnitTest Helpers

        private bool _eventFired;
        private NotifyCollectionChangedAction _expectedAction;
        private List<string> _expectedPropertyNames = new List<string>();

        /// <summary>
        /// Helper function that verifies that the Action fires an event.
        /// </summary>
        /// <param name="test">The Action which should fire an event.</param>
        private void AssertExpectedEvent(Action test)
        {
            this._eventFired = false;
            test();
            Assert.IsTrue(this._eventFired);
        }

        /// <summary>
        /// Helper function that flags that makes sure all expected property names were found in PropertyChanged events.
        /// </summary>
        private void CheckExpectedPropertyNamesFound()
        {
            Assert.AreEqual(0, this._expectedPropertyNames.Count);
        }

        /// <summary>
        /// Helper function that will filter out all negative numbers
        /// </summary>
        /// <param name="item">TestClass that we want to apply the filter on</param>
        /// <returns></returns>
        private bool FilterNegativeNumbers(object item)
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
        /// Test class used in PagedCollectionView_MiscellaneousTests
        /// </summary>
        public class ClassA
        {
            public int IntProperty { get; set; }
            public int? NullableIntProperty { get; set; }
            public string SomeProperty { get; set; }

            public override string ToString()
            {
                return IntProperty + " " + SomeProperty;
            }
        }

        /// <summary>
        /// Test class used in PagedCollectionView_MiscellaneousTests
        /// </summary>
        public class ClassB
        {
            public int IntProperty { get; set; }
            public bool SomeProperty { get; set; }

            public override string ToString()
            {
                return IntProperty + " " + SomeProperty;
            }
        }

        #endregion UnitTest Helpers
    }
}
