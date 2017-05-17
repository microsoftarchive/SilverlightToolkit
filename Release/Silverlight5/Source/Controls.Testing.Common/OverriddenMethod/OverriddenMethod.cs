// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overridden method tests for methods with no parameters.
    /// </summary>
    public sealed partial class OverriddenMethod : OverriddenMethodBase
    {
        /// <summary>
        /// Test action to perform before the base implementation is invoked.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Event used for testing.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Event used for testing.")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PreTest", Justification = "The naming is intentional.")]
        public event Action PreTest;

        /// <summary>
        /// Test action to perform after the base implementation was invoked.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Event used for testing.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Event used for testing.")]
        public event Action Test;

        /// <summary>
        /// Initializes a new instance of the OverriddenMethod class.
        /// </summary>
        public OverriddenMethod()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OverriddenMethod class.
        /// </summary>
        /// <param name="invariantTest">
        /// Test action to perform before and after the other tests.
        /// </param>
        public OverriddenMethod(Action invariantTest)
            : base(invariantTest)
        {
        }

        /// <summary>
        /// Invoke the test action.
        /// </summary>
        /// <param name="test">Test action to invoke.</param>
        /// <param name="parameters">Parameters to the test action.</param>
        private void InvokeTest(Action test, object[] parameters)
        {
            DoInvariantTest();

            ValidateParameters(parameters, 0);
            if (test != null)
            {
                test();
            }

            DoInvariantTest();
        }

        /// <summary>
        /// Perform the test action before the base implementation is invoked.
        /// </summary>
        /// <param name="parameters">Parameters to the test action.</param>
        public override void DoPreTest(params object[] parameters)
        {
            base.DoPreTest();
            InvokeTest(PreTest, parameters);
        }

        /// <summary>
        /// Perform the test action after the base implementation was invoked.
        /// </summary>
        /// <param name="parameters">Parameters to the test action.</param>
        public override void DoTest(params object[] parameters)
        {
            base.DoTest();
            InvokeTest(Test, parameters);
        }
    }
}