//-----------------------------------------------------------------------
// <copyright file="RemoveItemTestClasses.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #region Tests on TestClass

    [TestClass]
    public class PagedCollectionView_RemoveItemTests_List : RemoveItemTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_RemoveItemTests_ObservableCollection : RemoveItemTests
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
    public class PagedCollectionView_RemoveItemTests_EditableList : RemoveItemTests
    {
        [TestInitialize]
        public override void Initialize()
        {
            SourceCollection = CollectionGenerator.GetEditableList();
            base.Initialize();
        }
    }

    [TestClass]
    public class PagedCollectionView_RemoveItemTests_EditableObservableCollection : RemoveItemTests
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
