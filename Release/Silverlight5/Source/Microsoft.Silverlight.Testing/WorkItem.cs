// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing
{
    /// <summary>
    /// A test work item is a task that is invoked until it is complete.  It 
    /// maintains its own state to be able to notify the caller when it is 
    /// finally complete, with no further work to be run.
    /// 
    /// It is possible that some implementations of a TestWorkItem may actually 
    /// contain a set of sub-tasks by implementing a composite pattern.
    /// </summary>
    public abstract class WorkItem
    {
        /// <summary>
        /// A value indicating whether the task can immediately execute.
        /// </summary>
        private bool _canExecuteImmediately;

        /// <summary>
        /// Invoke the task.  Return false only when the task is complete.
        /// </summary>
        /// <returns>True if there is additional work to be completed.  False 
        /// when there is none.</returns>
        public virtual bool Invoke()
        {
            return ! IsComplete;
        }

        /// <summary>
        /// Gets a value indicating whether the task's work is complete.
        /// </summary>
        public bool IsComplete
        {
            get;
            protected set;
        }

        /// <summary>
        /// Called by the task after the work is complete.
        /// </summary>
        protected virtual void WorkItemComplete()
        {
            IsComplete = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the work item can be
        /// executed immediately, and does not rely on special asynchronous
        /// operation. Used for performance improvements. The setter is also
        /// public.
        /// </summary>
        public virtual bool CanExecuteImmediately
        {
            get { return _canExecuteImmediately; }
            set { _canExecuteImmediately = value; }
        }
    }
}