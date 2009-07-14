//-----------------------------------------------------------------------
// <copyright file="EditingTestClasses.cs" company="Microsoft">
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

    #region Tests on TestClass

    [TestClass]
    public class PagedCollectionView_EditingTests_EnumerableCollection : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEnumerableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_EditingTests_List : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_EditingTests_ListOfTestClasses : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetListOfTestClasses();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_EditingTests_ObservableCollection : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_EditingTests_ObservableEnumerableCollection : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableEnumerableCollection();
            base.Initialize();
        }
    }

    #endregion Tests on TestClass

    #region Tests on EditableTestClass

    [TestClass]
    public class PagedCollectionView_EditingTests_EditableEnumerableCollection : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEditableEnumerableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_EditingTests_EditableList : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEditableList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_EditingTests_EditableObservableCollection : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEditableObservableCollection();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_EditingTests_EditableObservableEnumerableCollection : EditingTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEditableObservableEnumerableCollection();
            base.Initialize();
        }
    }

    #endregion Tests on EditableTestClass

}
