// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Base class for overridden method tests.
    /// </summary>
    public abstract partial class OverriddenMethodBase
    {
        /// <summary>
        /// Gets the number of times the method has been called.
        /// </summary>
        public int NumberOfTimesCalled { get; private set; }

        /// <summary>
        /// Gets a test action to perform before and after the other tests.
        /// </summary>
        public Action InvariantTest { get; private set; }

        /// <summary>
        /// Initializes a new instance of the OverriddenMethodBase class.
        /// </summary>
        protected OverriddenMethodBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OverriddenMethodBase class.
        /// </summary>
        /// <param name="invariantTest">
        /// Test action to perform before and after the other tests.
        /// </param>
        protected OverriddenMethodBase(Action invariantTest)
        {
            InvariantTest = invariantTest;
            if (invariantTest != null)
            {
                invariantTest();
            }
        }

        /// <summary>
        /// Perform the invariant test action.
        /// </summary>
        public void DoInvariantTest()
        {
            if (InvariantTest != null)
            {
                InvariantTest();
            }
        }

        /// <summary>
        /// Perform the test action before the base implementation is invoked.
        /// </summary>
        /// <param name="parameters">Parameters to the test action.</param>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PreTest", Justification = "The naming is intentional.")]
        public virtual void DoPreTest(params object[] parameters)
        {
            NumberOfTimesCalled++;
        }

        /// <summary>
        /// Perform the test action after the base implementation was invoked.
        /// </summary>
        /// <param name="parameters">Parameters to the test action.</param>
        public virtual void DoTest(params object[] parameters)
        {
        }

        /// <summary>
        /// Validate the parameters for a test action.
        /// </summary>
        /// <param name="parameters">
        /// Parameters supplied to a test action.
        /// </param>
        /// <param name="expectedLength">
        /// Expected number of parameters for the test action.
        /// </param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It is intended to be overridden")]
        protected void ValidateParameters(object[] parameters, int expectedLength)
        {
            if (parameters == null)
            {
                if (expectedLength != 0)
                {
                    throw new ArgumentNullException("parameters");
                }
            }
            else if (parameters.Length != expectedLength)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Expected {0} parameters, not {1}!",
                        expectedLength,
                        parameters.Length),
                    "parameters");
            }
        }

        /// <summary>
        /// Get a parameter for a test action of a specific type.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the parameters for the test action.
        /// </typeparam>
        /// <param name="parameters">Parameters for a test action.</param>
        /// <param name="index">Index of the desired parameter.</param>
        /// <returns>Parameter for a test action as a specific type.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It is intended to be overridden")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This is by design")]
        protected T GetParameter<T>(object[] parameters, int index)
        {
            Debug.Assert(parameters != null, "parameters should not be null!");
            Debug.Assert(index >= 0 && index < parameters.Length, "index is out of range!");

            object value = parameters[index];
            try
            {
                return (T) value;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot cast parameter {0} of type {2} to type {1}!",
                        index,
                        typeof(T).FullName,
                        value == null ? "null" : value.GetType().FullName),
                    "parameters");
            }
        }

        /// <summary>
        /// Create a method call monitor to track whether the method is called.
        /// </summary>
        /// <returns>
        /// Method call monitor to track whether the method is called.
        /// </returns>
        public MethodCallMonitor CreateMonitor()
        {
            return new MethodCallMonitor(this);
        }
    }
}