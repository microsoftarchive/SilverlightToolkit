// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Base class of the unit testing hierarchy for control and layout
    /// container tests.
    /// </summary>
    public abstract partial class TestBase : SilverlightTest
    {
        /// <summary>
        /// Gets or sets the number of milliseconds to wait between actions in
        /// TestAsync, TestTaskAsync, and TestSequenceAsync.
        /// </summary>
        protected internal static int DefaultVisualDelayInMilliseconds { get; set; }

        /// <summary>
        /// The default timeout that can easily be used by any derived test. A
        /// default timeout value can allow for EnqueueConditional statements to
        /// wait for an event to fire, or otherwise Timeout.
        /// </summary>
        protected const int DefaultTimeout = 4000;

        /// <summary>
        /// Initializes static members of the TestBase class.
        /// </summary>
        static TestBase()
        {
            DefaultVisualDelayInMilliseconds = 0;
        }

        /// <summary>
        /// Initializes a new instance of the TestBase class.
        /// </summary>
        protected TestBase()
        {
        }

        /// <summary>
        /// Sets up the cached composition tests. 
        /// Examines if the host if GPU accelerated and if so
        /// changes the TestPanel to CacheMode=BitmapCache. 
        /// </summary>
        [TestInitialize]
        public void SetupCachedCompositionTest()
        {
            if (Application.Current.Host.Settings.EnableGPUAcceleration)
            {
                TestPanel.CacheMode = new BitmapCache();
            }
        }

        /// <summary>
        /// Add an element to the test surface and perform a series of test
        /// actions with a pause in between each allowing the test surface to be
        /// updated.  This task does not complete the async test and a call to
        /// EnqueueTestCompleted is still required.
        /// </summary>
        /// <param name="element">Element to test.</param>
        /// <param name="actions">Test actions.</param>
        /// <remarks>
        /// TestTaskAsync should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected internal void TestTaskAsync(FrameworkElement element, params Action[] actions)
        {
            TestTaskAsync(DefaultVisualDelayInMilliseconds, element, actions);
        }

        /// <summary>
        /// Add an element to the test surface and perform a series of test
        /// actions with a pause in between each allowing the test surface to be
        /// updated.  This task does not complete the async test and a call to
        /// EnqueueTestCompleted is still required.
        /// </summary>
        /// <param name="visualDelay">
        /// The visual delay in milliseconds to wait between asynchronous test
        /// actions.
        /// </param>
        /// <param name="element">Element to test.</param>
        /// <param name="actions">Test actions.</param>
        /// <remarks>
        /// TestTaskAsync should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected internal void TestTaskAsync(int visualDelay, FrameworkElement element, params Action[] actions)
        {
            Assert.IsNotNull(element, "Element to test should not be null!");
            actions = actions ?? new Action[] { };

            // Add a handler to determine when the element is loaded
            bool isLoaded = false;
            element.Loaded += delegate { isLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            EnqueueCallback(() => TestPanel.Children.Add(element));
            EnqueueConditional(() => isLoaded);

            // Perform the test actions
            foreach (Action action in actions)
            {
                Action capturedAction = action;
                EnqueueCallback(() => capturedAction());
                EnqueueVisualDelay(visualDelay);
            }

            // Remove the element from the test surface and finish the test
            EnqueueCallback(() => TestPanel.Children.Remove(element));
        }

        /// <summary>
        /// Add an element to the test surface and perform a series of test
        /// actions with a pause in between each allowing the test surface to be
        /// updated.
        /// </summary>
        /// <param name="element">Element to test.</param>
        /// <param name="actions">Test actions.</param>
        /// <remarks>
        /// TestAsync should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected internal void TestAsync(FrameworkElement element, params Action[] actions)
        {
            TestAsync(DefaultVisualDelayInMilliseconds, element, actions);
        }

        /// <summary>
        /// Add an element to the test surface and perform a series of test
        /// actions with a pause in between each allowing the test surface to be
        /// updated.
        /// </summary>
        /// <param name="visualDelay">
        /// The visual delay in milliseconds to wait between asynchronous test
        /// actions.
        /// </param>
        /// <param name="element">Element to test.</param>
        /// <param name="actions">Test actions.</param>
        /// <remarks>
        /// TestAsync should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected internal void TestAsync(int visualDelay, FrameworkElement element, params Action[] actions)
        {
            TestTaskAsync(visualDelay, element, actions);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Add a series of elements to the test surface one at a time and
        /// perform a series of test actions with a pause between each action
        /// allowing the test surface to be updated.
        /// </summary>
        /// <typeparam name="T">Type of elements to test.</typeparam>
        /// <param name="elements">Sequence of elements to test.</param>
        /// <param name="actions">Test actions for an element.</param>
        /// <remarks>
        /// TestSequenceAsync should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected internal void TestSequenceAsync<T>(IEnumerable<T> elements, params Action<T>[] actions)
            where T : FrameworkElement
        {
            TestSequenceAsync(DefaultVisualDelayInMilliseconds, elements, actions);
        }

        /// <summary>
        /// Add a series of elements to the test surface one at a time and
        /// perform a series of test actions with a pause between each action
        /// allowing the test surface to be updated.
        /// </summary>
        /// <typeparam name="T">Type of elements to test.</typeparam>
        /// <param name="visualDelay">
        /// The visual delay in milliseconds to wait between asynchronous test
        /// actions.
        /// </param>
        /// <param name="elements">Sequence of elements to test.</param>
        /// <param name="actions">Test actions for an element.</param>
        /// <remarks>
        /// TestSequenceAsync should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected internal void TestSequenceAsync<T>(int visualDelay, IEnumerable<T> elements, params Action<T>[] actions)
            where T : FrameworkElement
        {
            Assert.IsNotNull(elements, "Elements to test should not be null!");
            foreach (T element in elements)
            {
                T capturedElement = element;

                Assert.IsNotNull(capturedElement, "Element to test should not be null!");
                actions = actions ?? new Action<T>[] { };

                // Add a handler to determine when the element is loaded
                bool isLoaded = false;
                capturedElement.Loaded += delegate { isLoaded = true; };

                // Add the element to the test surface and wait until it's
                // loaded
                EnqueueCallback(() => TestPanel.Children.Add(capturedElement));
                EnqueueConditional(() => isLoaded);

                // Perform the test actions
                foreach (Action<T> action in actions)
                {
                    Action<T> capturedAction = action;
                    EnqueueCallback(() => capturedAction(capturedElement));
                    EnqueueVisualDelay(visualDelay);
                }

                // Remove the element from the test surface when finished
                EnqueueCallback(() => TestPanel.Children.Remove(capturedElement));
            }
           
            // Finish the test
            EnqueueTestComplete();
        }

        /// <summary>
        /// Enqueue a visual delay.
        /// </summary>
        /// <param name="visualDelay">
        /// The visual delay in milliseconds to wait between asynchronous test
        /// actions.
        /// </param>
        protected void EnqueueVisualDelay(int visualDelay)
        {
            if (visualDelay > 0)
            {
                EnqueueDelay(visualDelay);
            }
        }
    }
}
