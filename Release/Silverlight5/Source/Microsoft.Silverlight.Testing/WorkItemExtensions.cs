// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing
{
    /// <summary>
    /// A class contains extension methods and helpers for dealing with WorkItem
    /// instances and improving framework performance.
    /// </summary>
    internal static class WorkItemExtensions
    {
        /// <summary>
        /// Enqueues a work item into the task queue. The work item will run
        /// immediately following the previous work item, and may not leave any
        /// time before executing the next. This is a specialized method to be
        /// used for performance improvements.
        /// </summary>
        /// <param name="test">The work item test.</param>
        /// <param name="workItem">The unit of work.</param>
        public static void EnqueueQuickWorkItem(this WorkItemTest test, WorkItem workItem)
        {
            workItem.CanExecuteImmediately = true;
            test.EnqueueWorkItem(workItem);
        }

        /// <summary>
        /// Enqueues a method into the task queue. The method will run
        /// immediately following the previous work item, and may not leave any
        /// time before executing the next. This is a specialized method to be
        /// used for performance improvements.
        /// </summary>
        /// <param name="test">The work item test.</param>
        /// <param name="callback">The callback action or method.</param>
        public static void EnqueueQuickCallback(this WorkItemTest test, Action callback)
        {
            test.EnqueueQuickWorkItem(test.CreateCallback(callback));
        }

        /// <summary>
        /// Enqueues a conditional statement into the task queue. The method will
        /// run immediately following the previous work item, and may not leave
        /// any time before executing the next. This is a specialized method to
        /// be used for performance improvements.
        /// </summary>
        /// <param name="test">The work item test.</param>
        /// <param name="conditional">The conditional function or statement.</param>
        public static void EnqueueQuickConditional(this WorkItemTest test, Func<bool> conditional)
        {
            test.EnqueueQuickWorkItem(test.CreateConditional(conditional));
        }
    }
}