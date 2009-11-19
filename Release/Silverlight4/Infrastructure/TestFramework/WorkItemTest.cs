// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Silverlight.Testing.UnitTesting;
using UnitTestHarness = Microsoft.Silverlight.Testing.UnitTesting.Harness.UnitTestHarness;

namespace Microsoft.Silverlight.Testing
{
    /// <summary>
    /// Custom test class that provides the ability to perform semi-asynchronous 
    /// test tasks on the main thread.  Requires the custom unit test harness 
    /// that manages and invokes test work items on the thread when needed.
    /// 
    /// Tests using this functionality will not be compatible with the full 
    /// desktop framework's Visual Studio Team Test environment.
    /// </summary>
    public class WorkItemTest : CustomFrameworkUnitTest
    {
        /// <summary>
        /// Signal that a test is complete when using Async testing.  
        /// 
        /// WARNING: If you use your own methods for completing, such as an 
        /// HtmlTimer or other threading method, it is possible that this call 
        /// will occur *AFTER* the test has timed out when using Timeouts.  As 
        /// such, be very careful as you could complete the call to the *next* 
        /// test.
        /// </summary>
        public virtual void TestComplete()
        {
            WorkItemContainer.WorkItemCompleteInternal();
        }

        /// <summary>
        /// Add a task object to the test queue.  For a test that is currently 
        /// executing, all tasks contained within the queue are executed to 
        /// completion (unless an Exception is thrown) -before- moving on to 
        /// the next test.
        /// 
        /// The test task queue replaces the PumpMessages(...) system that 
        /// permitted a single callback.  This enables specialized tasks, such 
        /// as DOM bridge tasks, sleep tasks, and conditional continue tasks.
        /// </summary>
        /// <param name="testTaskObject">Asynchronous test task 
        /// instance.</param>
        public virtual void EnqueueWorkItem(IWorkItem testTaskObject)
        {
            if (UnitTestHarness.DispatcherStack.CurrentCompositeWorkItem != null)
            {
                UnitTestHarness.DispatcherStack.CurrentCompositeWorkItem.Enqueue(testTaskObject);
            }
            else
            {
                throw new InvalidOperationException(Properties.UnitTestMessage.WorkItemTest_EnqueueWorkItem_AsynchronousFeatureUnavailable);
            }
        }

        /// <summary>
        /// Delay a minimum amount of time before continuing. Similar to a sleep
        /// call that is not a blocking call.
        /// </summary>
        /// <param name="delay">The minimum time span to wait before continuing.</param>
        public virtual void EnqueueDelay(TimeSpan delay)
        {
            EnqueueWorkItem(new SleepWorkItem(delay));
        }

        /// <summary>
        /// Delay a minimum amount of time before continuing. Similar to a sleep
        /// call that is not a blocking call.
        /// </summary>
        /// <param name="milliseconds">The minimum number of milliseconds to wait
        /// until the delay is finished.</param>
        public void EnqueueDelay(double milliseconds)
        {
            EnqueueDelay(TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Requires a bool returning delegate to be passed in. Instructs the 
        /// test task queue to wait until the conditional call returns True to 
        /// continue executing other test tasks and/or ending the test method.
        /// </summary>
        /// <param name="conditionalDelegate">Conditional method or delegate. 
        /// Test will halt until this condition returns True.</param>
        public virtual void EnqueueConditional(Func<bool> conditionalDelegate)
        {
            ConditionalWorkItem conditionalTask = new ConditionalWorkItem(conditionalDelegate);
            EnqueueWorkItem(conditionalTask);
        }

        /// <summary>
        /// Enqueue a test task which calls the TestComplete method of 
        /// SilverlightTest.
        /// </summary>
        public virtual void EnqueueTestComplete()
        {
            EnqueueCallback(() => TestComplete());
        }

        /// <summary>
        /// Add a Callback method into the test task queue.  Similar to the 
        /// PumpMessages(...) call, with the difference being that there is no 
        /// longer a single requirement: you can enqueue several callback 
        /// methods and other test tasks, all of which will execute before the 
        /// test completes and/or the engine continues.
        /// </summary>
        /// <param name="testCallbackDelegate">Void-returning delegate, 
        /// anonymous delegates work fine too.</param>
        public virtual void EnqueueCallback(Action testCallbackDelegate)
        {
            EnqueueWorkItem(new CallbackWorkItem(testCallbackDelegate));
        }

        /// <summary>
        /// Adds a number of callback methods into the test task queue.
        /// </summary>
        /// <param name="actions">Set of Action instances.</param>
        public void EnqueueCallback(params Action[] actions)
        {
            if (actions != null)
            {
                foreach (Action action in actions)
                {
                    EnqueueCallback(action);
                }
            }
        }

        // TODO: Remove obsolete methods in the next release
        #region Obsolete methods (marked obsolete in Nov. 2008)
        /// <summary>
        /// Sleep a minimum number of milliseconds before calling a test 
        /// callback delegate.
        /// </summary>
        /// <param name="sleepMillisecondsMinimum">Minimum number of 
        /// milliseconds to sleep.  The only guarantee to the tester 
        /// is that the sleep will be >= this amount of ms, and NOT 
        /// that there is precision or an exact time.</param>
        /// <param name="testCallback">Callback method to 
        /// execute after the minimum amount of time has 
        /// elapsed.</param>
        [Obsolete("The Sleep method, composed of a EnqueueDelay and EnqueueCallback method call, is obsolete and will be replaced in a future test framework release.")]
        public virtual void Sleep(int sleepMillisecondsMinimum, Action testCallback)
        {
            EnqueueDelay(sleepMillisecondsMinimum);
            EnqueueCallback(testCallback);
        }

        /// <summary>
        /// Enqueue an action.  A shortcut for the EnqueueCallback.
        /// </summary>
        /// <param name="action">The action to enqueue.</param>
        [Obsolete("The Enqueue method is redundant with EnqueueCallback. Enqueue will be removed in a future release.")]
        public virtual void Enqueue(Action action)
        {
            EnqueueCallback(action);
        }

        /// <summary>
        /// Sleep a minimum number of milliseconds.  This is the simplified 
        /// overload which requires no callback.
        /// </summary>
        /// <param name="sleepMillisecondsMinimum">Minimum number of 
        /// milliseconds to sleep.  The only guarantee to the tester is that the
        /// sleep will be >= this amount of ms, and NOT that there is precision 
        /// or an exact time.</param>
        [Obsolete("The EnqueueSleep API has been renamed to EnqueueDelay. The EnqueueSleep method will be removed from a future release.")]
        public virtual void EnqueueSleep(int sleepMillisecondsMinimum)
        {
            EnqueueDelay(sleepMillisecondsMinimum);
        }
        #endregion
    }
}