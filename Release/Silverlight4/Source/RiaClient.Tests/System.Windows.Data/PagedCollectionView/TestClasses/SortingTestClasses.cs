//-----------------------------------------------------------------------
// <copyright file="SortingTestClasses.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PagedCollectionView_SortingTests_EnumerableCollection : SortingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEnumerableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_SortingTests_List : SortingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_SortingTests_ListOfTestClasses : SortingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetListOfTestClasses();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_SortingTests_ObservableCollection : SortingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_SortingTests_ObservableEnumerableCollection : SortingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableEnumerableCollection();
            base.Initialize();
        }
    }
}
