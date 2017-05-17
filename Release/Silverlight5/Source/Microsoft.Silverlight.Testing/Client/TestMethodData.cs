// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;
using Microsoft.Silverlight.Testing.Harness;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A data object that generates property change notifications and can
    /// be used for rich data binding to test results. Does keep a reference
    /// to all results.
    /// </summary>
    public class TestMethodData : PropertyChangedBase, IProvideResultReports
    {
        /// <summary>
        /// Stores the test method metadata.
        /// </summary>
        private ITestMethod _metadata;

        /// <summary>
        /// Parent data object.
        /// </summary>
        private TestClassData _parent;

        /// <summary>
        /// A value indicating whether the test is currently executing.
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// The test outcome.
        /// </summary>
        private ScenarioResult _result;

        /// <summary>
        /// Backing field for linked data.
        /// </summary>
        private TestMethodData _previous;

        /// <summary>
        /// Backing field for linked data.
        /// </summary>
        private TestMethodData _next;

        /// <summary>
        /// Backing field for linked data.
        /// </summary>
        private TestMethodData _previousFailure;

        /// <summary>
        /// Backing field for linked data.
        /// </summary>
        private TestMethodData _nextFailure;

        /// <summary>
        /// Initializes a new instance of the TestMethodData type.
        /// </summary>
        /// <param name="testMethod">The test method metadata.</param>
        /// <param name="parent">The test class that is the parent object.</param>
        public TestMethodData(ITestMethod testMethod, TestClassData parent)
        {
            if (testMethod == null)
            {
                throw new ArgumentNullException("testMethod");
            }

            _parent = parent;
            _metadata = testMethod;
        }

        /// <summary>
        /// Runs through the metadata for bugs.
        /// </summary>
        private void CheckForKnownBugs()
        {
            // TODO: Instead move this logic into the model manager.
            // This needs to be a set of events off of the main harness!

            List<string> knownBugs = new List<string>(2);
            List<string> fixedBugs = new List<string>(2);

            ICollection<Attribute> bugs = ReflectionUtility.GetAttributes(_metadata, typeof(BugAttribute), false);
            if (bugs != null && bugs.Count > 0)
            {
                foreach (Attribute attribute in bugs)
                {
                    BugAttribute bug = (BugAttribute)attribute;
                    if (bug.Fixed)
                    {
                        fixedBugs.Add(bug.Description);
                    }
                    else
                    {
                        knownBugs.Add(bug.Description);
                    }
                }
            }

            if (knownBugs.Count > 0)
            {
                KnownBugs = knownBugs;
            }
            if (fixedBugs.Count > 0)
            {
                FixedBugs = fixedBugs;
            }
        }

        /// <summary>
        /// Gets or sets the result of the test method.
        /// </summary>
        public ScenarioResult Result
        {
            get
            {
                return _result;
            }

            set
            {
                _result = value;

                EvaluateNotable();

                NotifyPropertyChanged("Result");
                NotifyPropertyChanged("Passed");
                NotifyPropertyChanged("HasOutcome");
                NotifyPropertyChanged("ReadableElapsedTime");
                NotifyPropertyChanged("SimplifiedExceptionName");
                NotifyPropertyChanged("SimplifiedExpectedExceptionName");

                CheckForKnownBugs();

                if (!Passed)
                {
                    Parent.Passed = false;
                }
            }
        }

        ///<summary>
        /// Gets the exception message from the result, xml decoding 
        /// any instances of less than or greater than.
        ///</summary>
        public string ExceptionMessage
        {
            get { return Result.Exception.Message.Replace("&lt;", "<").Replace("&gt;", ">"); }
        }

        /// <summary>
        /// Backing field for known bugs.
        /// </summary>
        private List<string> _knownBugs;

        /// <summary>
        /// Backing field for bugs that are marked fixed.
        /// </summary>
        private List<string> _fixedBugs;

        /// <summary>
        /// Gets the known bugs for display in the UI.
        /// </summary>
        public List<string> KnownBugs
        {
            get { return _knownBugs; }
            private set
            {
                _knownBugs = value;
                NotifyPropertyChanged("KnownBugs");
            }
        }

        /// <summary>
        /// Gets the fixed bugs for display.
        /// </summary>
        public List<string> FixedBugs
        {
            get { return _fixedBugs; }
            private set
            {
                _fixedBugs = value;
                NotifyPropertyChanged("FixedBugs");
            }
        }

        /// <summary>
        /// Gets the expected exception name for a negative test, if any.
        /// </summary>
        public string SimplifiedExpectedExceptionName
        {
            get
            {
                if (_result != null && _result.TestMethod != null && _result.TestMethod.ExpectedException != null && _result.TestMethod.ExpectedException.ExceptionType != null)
                {
                    return _result.TestMethod.ExpectedException.ExceptionType.Name;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a simplified exception stack trace that omits the trace below
        /// the invoke of the test method by the test framework.
        /// </summary>
        public string SimplifiedExceptionStackTrace
        {
            get
            {
                if (_result != null && _result.Exception != null && _result.Exception.StackTrace != null)
                {
                    string st = _result.Exception.StackTrace;
                    int i = st.IndexOf("   at Microsoft.Silverlight.Testing.Harness.CompositeWorkItem.Invoke(WorkItem& usedWorkItem)");
                    if (i >= 0)
                    {
                        return st.Substring(0, i);
                    }
                    return st;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the short, simple name of the exception type recorded in the
        /// test result, if any.
        /// </summary>
        public string SimplifiedExceptionName
        {
            get
            {
                if (_result != null && _result.Exception != null)
                {
                    Type t = _result.Exception.GetType();
                    return t.Name;
                }

                return null;
            }
        }

        // TODO: CONSIDER: Extend the interface to also store this at runtime.

        /// <summary>
        /// Gets a value indicating whether the method has results.
        /// </summary>
        public bool HasOutcome
        {
            get { return _result != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the method has passed. Returns 
        /// true until there is a result.
        /// </summary>
        public bool Passed
        {
            get
            {
                return _result == null ? true : _result.Result == TestOutcome.Passed;
            }
        }

        #region IsRunning

        /// <summary>
        /// Gets or sets a value indicating whether the test method is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }

            set
            {
                bool previous = _isRunning;
                _isRunning = value;
                if (previous != _isRunning)
                {
                    NotifyPropertyChanged("IsRunning");
                }
                EvaluateNotable();
            }
        }

        #endregion

        /// <summary>
        /// Calculates whether the item is considered "notable", in that it
        /// should have a visual cue or hint for the user.
        /// </summary>
        private void EvaluateNotable()
        {
            bool notable = (IsRunning || !Passed);
            if (notable != IsNotable)
            {
                IsNotable = notable;
            }

            if (IsExpanded != notable)
            {
                IsExpanded = notable;
            }
        }

        #region IsNotable

        /// <summary>
        /// Stores a value indicating whether the result is notable.
        /// </summary>
        private bool _isNotable;

        /// <summary>
        /// Gets or sets a value indicating whether the result is notable. 
        /// Notable is defined as either currently running, or not having 
        /// passed. This can allow a user interface to react to an 
        /// interesting result.
        /// </summary>
        public bool IsNotable
        {
            get
            {
                return _isNotable;
            }

            set
            {
                bool old = _isNotable;
                _isNotable = value;

                if (value != old)
                {
                    NotifyPropertyChanged("IsNotable");
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the parent data object.
        /// </summary>
        public TestClassData Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets an instance of the actual metadata object.
        /// </summary>
        public ITestMethod Metadata
        {
            get { return _metadata; }
        }

        /// <summary>
        /// Gets the name of the test method.
        /// </summary>
        public string Name
        {
            get { return _metadata.Name; }
        }

        /// <summary>
        /// Gets a visibility value to allow for easy showing or
        /// hiding of a user interface component that displays the
        /// description.
        /// </summary>
        /// <returns>Returns a visibility value.</returns>
        public Visibility HasDescriptionVisibility
        {
            get
            {
                return string.IsNullOrEmpty(_metadata.Description)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets the elapsed time in a readable format.
        /// </summary>
        /// <returns>Returns a string of the readable time elapsed.</returns>
        public string ReadableElapsedTime
        {
            get
            {
                if (_result != null)
                {
                    return TimeHelper.ElapsedReadableTime(
                        _result.Started,
                        _result.Finished);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the description of the test method.
        /// </summary>
        public string Description
        {
            get
            {
                string d = _metadata.Description;
                if (string.IsNullOrEmpty(d))
                {
                    d = "(No description)";
                }
                return d;
            }
        }

        /// <summary>
        /// Backing field for the checked property.
        /// </summary>
        private bool _checked;

        /// <summary>
        /// Gets or sets a value indicating whether the item is checked in the
        /// user interface.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _checked;
            }

            set
            {
                bool old = _checked;
                _checked = value;
                if (old != _checked)
                {
                    NotifyPropertyChanged("IsChecked");
                }
            }
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
        /// Gets or sets the previous result.
        /// </summary>
        public TestMethodData PreviousResult
        {
            get { return _previous; }
            set 
            { 
                _previous = value;
                if (_previous != null)
                {
                    _previous._next = this;
                }
            }
        }

        /// <summary>
        /// Gets or sets the next result.
        /// </summary>
        public TestMethodData NextResult
        {
            get { return _next; }
            set { _next = value; }
        }

        /// <summary>
        /// Gets or sets the previous failing result.
        /// </summary>
        public TestMethodData PreviousFailingResult
        {
            get { return _previousFailure; }
            set 
            { 
                _previousFailure = value;
                if (_previousFailure != null)
                {
                    _previousFailure._nextFailure = this;
                }
            }
        }

        /// <summary>
        /// Gets or sets the next failing result.
        /// </summary>
        public TestMethodData NextFailingResult
        {
            get { return _nextFailure; }
            set { _nextFailure = value; }
        }

        /// <summary>
        /// Retrieves the results report.
        /// </summary>
        /// <returns>Returns a string containing the report.</returns>
        public string GetResultReport()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Test Method: ");
            sb.AppendLine(Name);
            sb.AppendLine(this.Result.ToString());
            sb.AppendLine(Passed ? "Passed" : "Failed");

            // CONSIDER: Provide more information here

            return sb.ToString();
        }
    }
}