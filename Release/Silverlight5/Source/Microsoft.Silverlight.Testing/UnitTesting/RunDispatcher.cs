// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Windows.Threading;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Handle calling into the test framework as needed to perform the test
    /// run, process the internal test dispatcher queue, and keep execution
    /// moving forward.
    /// </summary>
    public class RunDispatcher
    {
        /// <summary>
        /// Delegate that returns whether more work remains and runs the next
        /// set of work.
        /// </summary>
        private Func<bool> _runNextStep;

        /// <summary>
        /// Event that is called when all work is complete.
        /// </summary>
        public event EventHandler Complete;

        /// <summary>
        /// Backing field for a value indicaing whether the dispatcher should be
        /// running.
        /// </summary>
        private bool _running;

        /// <summary>
        /// Creates a new RunMethodManager, taking in a conditional delegate to
        /// run for each step to see if more work remains.
        /// </summary>
        /// <param name="runNextStep">
        /// The conditional delegate that performs work and indicates whether
        /// additional work remains.
        /// </param>
        public RunDispatcher(Func<bool> runNextStep)
        {
            _runNextStep = runNextStep;
        }

        /// <summary>
        /// Create a new run method manager object.
        /// </summary>
        /// <param name="runNextStep">
        /// Conditional indicating whether more work will remain after
        /// performing the work.
        /// </param>
        /// <returns>
        /// Returns the run method manager. Typically depends on the execution 
        /// platform and environment.
        /// </returns>
        public static RunDispatcher Create(Func<bool> runNextStep)
        {
            RunDispatcher runMethodManager =
#if SILVERLIGHT
                new WebBrowserTick(runNextStep);
#else
                new RunDispatcher(runNextStep);
#endif
            return runMethodManager;
        }

        /// <summary>
        /// Create a new run method manager object.
        /// </summary>
        /// <param name="runNextStep">
        /// Conditional indicating whether more work will remain after
        /// performing the work.
        /// </param>
        /// <param name="dispatcher">A Dispatcher instance.</param>
        /// <returns>
        /// Returns the run method manager. Typically depends on the execution 
        /// platform and environment.
        /// </returns>
        public static RunDispatcher Create(Func<bool> runNextStep, Dispatcher dispatcher)
        {
            return new FastRunDispatcher(runNextStep, dispatcher);
        }

        /// <summary>
        /// A completely synchronous implementation, unless overridden, that
        /// calls RunNextStep() until the harness is finished.
        /// 
        /// The default implementation will not work with a presentation-rich
        /// test environment.
        /// </summary>
        public virtual void Run()
        {
            bool complete;
            _running = true;

            do
            {
                complete = _running ? true : RunNextStep();
            } 
            while (complete);

            OnComplete();
        }

        /// <summary>
        /// Calls a conditional delegate, and returns whether there is more work
        /// to be done.
        /// </summary>
        /// <returns>
        /// Returns a value indicating whether there is additional work
        /// remaining after executing the current work.
        /// </returns>
        protected bool RunNextStep()
        {
            return _runNextStep();
        }
        
        /// <summary>
        /// Calls the Complete event handler.
        /// </summary>
        protected void OnComplete()
        {
            EventHandler handler = Complete;
            if (handler != null) 
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dispatcher should be 
        /// running.
        /// </summary>
        public bool IsRunning
        {
            get { return _running; }
            set { _running = value; }
        }
    }
}