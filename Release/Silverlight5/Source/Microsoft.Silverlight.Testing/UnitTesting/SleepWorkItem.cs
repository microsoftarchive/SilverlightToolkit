// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Test work item type that does not complete until the sleep time has 
    /// elapsed.  This is NOT a blocking Sleep.
    /// </summary>
    public class SleepWorkItem : WorkItem
    {
        /// <summary>
        /// The amount of time to delay for.
        /// </summary>
        private TimeSpan _delay;

        /// <summary>
        /// The DateTime that marks the point in time the task is complete.
        /// </summary>
        private DateTime? _expires;

        /// <summary>
        /// Create a new Sleep work item, including the number of 
        /// milliseconds to wait until continuing.
        /// </summary>
        /// <param name="delay">Amount of time to wait/delay.</param>
        public SleepWorkItem(TimeSpan delay)
        {
            _delay = delay;
            _expires = null;
        }

        /// <summary>
        /// On the first time, will calculate the final DateTime.  Otherwise, 
        /// null operation (returns) until that time.
        /// </summary>
        /// <returns>Returns a value indicating whether there is more work to be
        /// done.</returns>
        public override bool Invoke()
        {
            if (_expires == null)
            {
                _expires = DateTime.Now + _delay;
            }
            if (DateTime.Now > _expires) 
            {
                WorkItemComplete();
            }
            return base.Invoke();
        }
    }
}