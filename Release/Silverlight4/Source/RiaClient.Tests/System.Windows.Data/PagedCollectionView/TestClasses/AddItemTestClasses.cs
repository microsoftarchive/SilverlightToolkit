//-----------------------------------------------------------------------
// <copyright file="AddItemTestClasses.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace System.ComponentModel.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Silverlight.Testing;

    //// Note: we don't run tests on collections that don't implement
    //// IList for these tests, because CanAdd returns false

    #region Tests on TestClass

    [TestClass]
    public class PagedCollectionView_AddItemTests_List : AddItemTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_AddItemTests_ListOfTestClasses : AddItemTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetListOfTestClasses();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_AddItemTests_ObservableCollection : AddItemTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetObservableCollection();
            base.Initialize();
        }
    }

    #endregion Tests on TestClass

    #region Tests on EditableTestClass

    [TestClass]
    public class PagedCollectionView_AddItemTests_EditableList : AddItemTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEditableList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_AddItemTests_EditableObservableCollection : AddItemTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEditableObservableCollection();
            base.Initialize();
        }
    }

    #endregion Tests on EditableTestClass

}
