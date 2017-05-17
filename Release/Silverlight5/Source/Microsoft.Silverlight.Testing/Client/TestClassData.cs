// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A data object that generates property change notifications and can
    /// be used for rich data binding to test results. Does keep a reference
    /// to all results.
    /// </summary>
    public class TestClassData : PropertyChangedBase, IProvideResultReports
    {
        /// <summary>
        /// Parent object reference.
        /// </summary>
        private TestAssemblyData _parent;

        /// <summary>
        /// Initializes a new instance of the TestClassData type.
        /// </summary>
        /// <param name="testClass">The test class metadata.</param>
        /// <param name="parent">The parent test assembly data object.</param>
        public TestClassData(ITestClass testClass, TestAssemblyData parent)
        {
            _methods = new ObservableCollection<TestMethodData>();
            _parent = parent;

            Name = testClass.Name;
            Namespace = testClass.Namespace;
        }

        /// <summary>
        /// Gets the parent data object.
        /// </summary>
        public TestAssemblyData Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the name of the test class.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the namespace for the test class.
        /// </summary>
        public string Namespace { get; private set; }

        #region TestMethods

        /// <summary>
        /// Backing store for the set of test class.
        /// </summary>
        private ObservableCollection<TestMethodData> _methods;

        /// <summary>
        /// Gets an observable collection of test class data objects.
        /// </summary>
        public ObservableCollection<TestMethodData> TestMethods
        {
            get { return _methods; }
        }

        #endregion

        /// <summary>
        /// Backing field for the expanded property.
        /// </summary>
        private bool _expanded;

        /// <summary>
        /// Collapses the test class node unless there is at least one child
        /// test method that failed.
        /// </summary>
        public void CollapseUnlessFailures()
        {
            bool collapse = true;
            foreach (TestMethodData tmd in _methods)
            {
                if (!tmd.Passed)
                {
                    collapse = false;
                    break;
                }
            }

            IsExpanded = !collapse;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded in
        /// a hierarchical display.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return _expanded;
            }

            set
            {
                bool old = _expanded;
                _expanded = value;
                if (old != _expanded)
                {
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// Backing field for a passed value.
        /// </summary>
        private bool _passed = true;

        /// <summary>
        /// Gets or sets the checked value. Don't think this is actually used.
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the test passed. If failed,
        /// will propagate to the parent metadata object.
        /// </summary>
        public bool Passed
        {
            get { return _passed; }

            set
            {
                bool old = _passed;
                _passed = value;
                if (old != _passed)
                {
                    NotifyPropertyChanged("Passed");
                }

                // Propagate fail
                if (!_passed)
                {
                    Parent.Passed = _passed;
                }
            }
        }

        /// <summary>
        /// Retrieves the results report.
        /// </summary>
        /// <returns>Returns a string containing the report.</returns>
        public string GetResultReport()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Test Class: ");
            sb.AppendLine(Name);
            sb.AppendLine(Namespace);
            sb.AppendLine(Passed ? "Passed" : "Failed");

            foreach (TestMethodData tmd in this.TestMethods)
            {
                sb.AppendLine(tmd.GetResultReport());
            }

            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}