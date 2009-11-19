//-----------------------------------------------------------------------
// <copyright file="CollectionGenerator.cs" company="Microsoft">
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

    /// <summary>
    /// This class will consist of static methods used to initialize collections of items
    /// to initially populate the PagedCollectionView.
    /// </summary>
    public class CollectionGenerator
    {
        #region Generate Collections of TestClass

        /// <summary>
        /// Gets a collection that does not implement IList or INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static EnumerableCollection<TestClass> GetEnumerableCollection()
        {
            EnumerableCollection<TestClass> collection = new EnumerableCollection<TestClass>();
            for (int i = 0; i < 25; i++)
            {
                collection.Add(GetTestClassInstance(i));
            }

            return collection;
        }

        /// <summary>
        /// Gets a collection that implements IList but not INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static List<TestClass> GetList()
        {
            List<TestClass> collection = new List<TestClass>();
            for (int i = 0; i < 25; i++)
            {
                collection.Add(GetTestClassInstance(i));
            }

            return collection;
        }

        /// <summary>
        /// Gets a list of TestClass objects that implements IList but not INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static ListOfTestClasses GetListOfTestClasses()
        {
            return new ListOfTestClasses(GetList());
        }

        /// <summary>
        /// Gets a collection that implements IList and INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static ObservableCollection<TestClass> GetObservableCollection()
        {
            ObservableCollection<TestClass> collection = new ObservableCollection<TestClass>();
            for (int i = 0; i < 25; i++)
            {
                collection.Add(GetTestClassInstance(i));
            }

            return collection;
        }

        /// <summary>
        /// Gets a collection that does not implement IList but implements INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static ObservableEnumerableCollection<TestClass> GetObservableEnumerableCollection()
        {
            ObservableEnumerableCollection<TestClass> collection = new ObservableEnumerableCollection<TestClass>();
            for (int i = 0; i < 25; i++)
            {
                collection.Add(GetTestClassInstance(i));
            }

            return collection;
        }

        #endregion Generate Collections of TestClass

        #region Generate Collections of EditableTestClass

        /// <summary>
        /// Gets a collection that does not implement IList or INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static EnumerableCollection<EditableTestClass> GetEditableEnumerableCollection()
        {
            EnumerableCollection<EditableTestClass> collection = new EnumerableCollection<EditableTestClass>();
            for (int i = 0; i < 25; i++)
            {
                collection.Add(GetEditableTestClassInstance(i));
            }

            return collection;
        }

        /// <summary>
        /// Gets a collection that implements IList but not INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static List<EditableTestClass> GetEditableList()
        {
            List<EditableTestClass> collection = new List<EditableTestClass>();
            for (int i = 0; i < 25; i++)
            {
                collection.Add(GetEditableTestClassInstance(i));
            }

            return collection;
        }

        /// <summary>
        /// Gets a collection that implements IList and INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static ObservableCollection<EditableTestClass> GetEditableObservableCollection()
        {
            ObservableCollection<EditableTestClass> collection = new ObservableCollection<EditableTestClass>();
            for (int i = 0; i < 25; i++)
            {
                collection.Add(GetEditableTestClassInstance(i));
            }

            return collection;
        }

        /// <summary>
        /// Gets a collection that does not implement IList but implements INotifyCollectionChanged
        /// </summary>
        /// <returns>Generated collection of items</returns>
        public static ObservableEnumerableCollection<EditableTestClass> GetEditableObservableEnumerableCollection()
        {
            ObservableEnumerableCollection<EditableTestClass> collection = new ObservableEnumerableCollection<EditableTestClass>();
            for (int i = 0; i < 25; i++)
            {
                collection.Add(GetEditableTestClassInstance(i));
            }

            return collection;
        }

        #endregion Generate Collections of EditableTestClass

        #region Generate TestClass or EditableTestClass

        /// <summary>
        /// Creates an instance of the TestClass, sets its properties
        /// based on the index it is provided with, and returns it.
        /// </summary>
        /// <param name="index">Index to use when calculating its properties</param>
        /// <returns>The TestClass we generate</returns>
        private static TestClass GetTestClassInstance(int index)
        {
            TestClass testClass = new TestClass();
            testClass.IntProperty = (index % 5) + 1;
            testClass.StringProperty = (index % 2 == 0) ? "A" : "B";
            testClass.InnerClassProperty = new InnerClass() { InnerIntProperty = ((index % 2) + 1), InnerStringProperty = "InnerString" };

            return testClass;
        }

        /// <summary>
        /// Creates an instance of the EditableTestClass, sets its properties
        /// based on the index it is provided with, and returns it.
        /// </summary>
        /// <param name="index">Index to use when calculating its properties</param>
        /// <returns>The EditableTestClass we generate</returns>
        private static EditableTestClass GetEditableTestClassInstance(int index)
        {
            EditableTestClass testClass = new EditableTestClass();
            testClass.IntProperty = (index % 5) + 1;
            testClass.StringProperty = (index % 2 == 0) ? "A" : "B";
            testClass.InnerClassProperty = new InnerClass() { InnerIntProperty = ((index % 2) + 1), InnerStringProperty = "InnerString" };

            return testClass;
        }

        #endregion Generate TestClass or EditableTestClass
    }
}
