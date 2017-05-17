// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// A type which handles preparing the underlying dispatcher or timer from 
    /// which the test work items execute.
    /// </summary>
    public class FastRunDispatcher : RunDispatcher
    {
        /// <summary>
        /// Stored Dispatcher instance.
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// Sets up a new run method manager.
        /// </summary>
        /// <param name="runNextStep">
        /// Conditional delegate which returns true as long as there is 
        /// additional work.
        /// </param>
        /// <param name="dispatcher">An instance of the dispatcher to use.</param>
        public FastRunDispatcher(Func<bool> runNextStep, Dispatcher dispatcher)
            : base(runNextStep)
        {
            // Last attempt to setup the dispatcher
            if (dispatcher == null)
            {
                FrameworkElement element = new System.Windows.Shapes.Rectangle();
                dispatcher = element.Dispatcher;
            }

            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Begin the execution process by hooking up the underlying 
        /// DispatcherTimer to call into the test framework regularly and 
        /// perform test work items.
        /// </summary>
        public override void Run()
        {
            if (IsRunning || RunNextStep())
            {
                _dispatcher.BeginInvoke(Run);
            }
            else
            {
                OnComplete();
            }
        }
    }
}