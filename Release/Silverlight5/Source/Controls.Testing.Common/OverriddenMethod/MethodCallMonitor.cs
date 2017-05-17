// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Monitor whether a method is called.
    /// </summary>
    public sealed class MethodCallMonitor
    {
        /// <summary>
        /// Number of times the method had been called when we start monitoring
        /// (which we use to determine if it was called or not).
        /// </summary>
        private int _initialNumberOfTimesCalled;

        /// <summary>
        /// Gets the method to monitor.
        /// </summary>
        public OverriddenMethodBase Method { get; private set; }

        /// <summary>
        /// Prevent external instantiation of the MethodCallMonitor class.
        /// </summary>
        private MethodCallMonitor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MethodCallMonitor class.
        /// </summary>
        /// <param name="method">Method to monitor.</param>
        internal MethodCallMonitor(OverriddenMethodBase method)
        {
            Debug.Assert(method != null, "Method to monitor cannot be null!");
            Method = method;
            Reset();
        }

        /// <summary>
        /// Require that the method is called.
        /// </summary>
        public void AssertCalled()
        {
            AssertCalled("The required method call did not occur!");
        }

        /// <summary>
        /// Require that the method is called.
        /// </summary>
        /// <param name="message">Assertion message.</param>
        public void AssertCalled(string message)
        {
            Assert.AreNotEqual(
                _initialNumberOfTimesCalled,
                Method.NumberOfTimesCalled,
                message);
        }

        /// <summary>
        /// Require that the method is not called.
        /// </summary>
        public void AssertNotCalled()
        {
            AssertNotCalled("The forbidden method call occurred!");
        }

        /// <summary>
        /// Require that the method is not called.
        /// </summary>
        /// <param name="message">Assertion message.</param>
        public void AssertNotCalled(string message)
        {
            Assert.AreEqual(
                _initialNumberOfTimesCalled,
                Method.NumberOfTimesCalled,
                message);
        }

        /// <summary>
        /// Reset the monitor.
        /// </summary>
        public void Reset()
        {
            _initialNumberOfTimesCalled = Method.NumberOfTimesCalled;
        }
    }
}