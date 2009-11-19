// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System
{
    /// <summary>
    /// A class that facilitates the creation of anonymous IDisposable objects.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal class AnonymousDisposable : IDisposable
    {
        /// <summary>
        /// The action to execute when the Dispose method is called.
        /// </summary>
        private Action _dispose;

        /// <summary>
        /// Creates a new instance of the GenericDispose class.
        /// </summary>
        /// <param name="dispose">The action to execute when the Dispose method
        /// is called.</param>
        public AnonymousDisposable(Action dispose)
        {
            _dispose = dispose;
        }

        /// <summary>
        /// Executes the dispose action.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _dispose();
        }
    }
}