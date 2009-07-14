//-----------------------------------------------------------------------
// <copyright file="PagingTestClasses.cs" company="Microsoft">
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
    public class PagedCollectionView_PagingTests_EnumerableCollection : PagingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEnumerableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_PagingTests_List : PagingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_PagingTests_ObservableCollection : PagingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_PagingTests_ObservableEnumerableCollection : PagingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableEnumerableCollection();
            base.Initialize();
        }
    }
}
