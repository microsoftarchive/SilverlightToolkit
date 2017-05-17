// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Silverlight.Testing.Harness;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// A method container.
    /// </summary>
    public class MethodContainer : CompositeWorkItem
    {
        /// <summary>
        /// The timeout time.
        /// </summary>
        private DateTime _timeout = DateTime.MaxValue;

        /// <summary>
        /// The test method metadata.
        /// </summary>
        private ITestMethod _method;

        /// <summary>
        /// Constructs a new method container.
        /// </summary>
        private MethodContainer() { }

        /// <summary>
        /// Constructs a new method container.
        /// </summary>
        /// <param name="instance">An instance of the method's type.</param>
        /// <param name="method">The method reflection object.</param>
        /// <param name="testMethod">The test method.</param>
        public MethodContainer(object instance, MethodInfo method, ITestMethod testMethod)
            : base()
        {
            _methodTask = new MethodInvokeWorkItem(instance, method, testMethod);
            _methodInfo = method;
            _method = testMethod;
        }

        /// <summary>
        /// The task that involves the method, and contains its own internal 
        /// test queue, if needed for asynchronous tasks.
        /// </summary>
        private MethodInvokeWorkItem _methodTask;

        /// <summary>
        /// The reflection object for the method.
        /// </summary>
        private MethodInfo _methodInfo;

        /// <summary>
        /// Invoke into the method.
        /// </summary>
        /// <returns>Returns the condition of any remaining work.</returns>
        public override bool Invoke()
        {
            if (DateTime.Now > _timeout)
            {
                OnUnhandledException(new TimeoutException(string.Format(CultureInfo.CurrentUICulture, "A timeout has occurred. The test method did not complete after {0} milliseconds.", (int)_method.Timeout)));
                WorkItemComplete();
            }

            return base.Invoke();
        }

        /// <summary>
        /// On the first invoke, make sure there's a task to call the method.
        /// </summary>
        protected override void FirstInvoke()
        {
            if (_method != null && _method.Timeout != null)
            {
                _timeout = DateTime.Now + TimeSpan.FromMilliseconds((int)_method.Timeout);
            }
            Enqueue(_methodTask);
        }

        /// <summary>
        /// Gets the method's reflection object.
        /// </summary>
        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
        }
    }
}