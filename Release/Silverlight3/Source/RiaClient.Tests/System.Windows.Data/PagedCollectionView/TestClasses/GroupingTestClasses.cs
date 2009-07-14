//-----------------------------------------------------------------------
// <copyright file="GroupingTestClasses.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PagedCollectionView_GroupingTests_EnumerableCollection : GroupingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEnumerableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_GroupingTests_List : GroupingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_GroupingTests_ListOfTestClasses : GroupingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetListOfTestClasses();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_GroupingTests_ObservableCollection : GroupingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_GroupingTests_ObservableEnumerableCollection : GroupingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableEnumerableCollection();
            base.Initialize();
        }
    }
}
