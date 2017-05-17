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
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;
using System.Collections.ObjectModel;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A data object storing the hierarchical results for a test assembly in a
    /// test run.
    /// </summary>
    public class TestAssemblyData : PropertyChangedBase, IProvideResultReports
    {
        /// <summary>
        /// Initializes a new instance of the TestAssemblyData type.
        /// </summary>
        /// <param name="testAssembly">The test assembly metadata.</param>
        public TestAssemblyData(IAssembly testAssembly)
        {
            if (testAssembly == null)
            {
                throw new ArgumentNullException("testAssembly");
            }

            _classes = new ObservableCollection<TestClassData>();

            // A weak reference or snap of data can be used to prevent leaks.
            Name = testAssembly.Name;

            // Always expand test assemblies
            IsExpanded = true;
        }

        /// <summary>
        /// Backing field for the expanded property.
        /// </summary>
        private bool _expanded;

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
        /// Gets or sets the checked value. Don't think this is actually used.
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Backing field for a passed value.
        /// </summary>
        private bool _passed = true;

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
            }
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public string Name { get; private set; }

        #region TestClasses

        /// <summary>
        /// Backing store for the set of test class.
        /// </summary>
        private ObservableCollection<TestClassData> _classes;

        /// <summary>
        /// Gets an observable collection of test class data objects.
        /// </summary>
        public ObservableCollection<TestClassData> TestClasses
        {
            get { return _classes; }
        }

        #endregion

        /// <summary>
        /// Retrieves the results report.
        /// </summary>
        /// <returns>Returns a string containing the report.</returns>
        public string GetResultReport()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Test Assembly: ");
            sb.AppendLine(Name);
            sb.AppendLine(Passed ? "Passed" : "Failed");

            foreach (TestClassData tcd in TestClasses)
            {
                sb.AppendLine(tcd.GetResultReport());
            }

            return sb.ToString();
        }
    }
}