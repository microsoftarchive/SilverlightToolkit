// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// A subject that executes an action and then returns a single value.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    public class ActionSubject<T> : Subject<T>
    {
        /// <summary>
        /// Gets the action to execute when an observer is registered.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It's necessary to nest generic types.")]
        public Action<ActionSubject<T>, IObserver<T>> Action { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ActionSubject class.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It's necessary to nest generic types.")]
        public ActionSubject(Action<ActionSubject<T>, IObserver<T>> action)
        {
            this.Action = action;
        }

        /// <summary>
        /// Executes an action when an observer is registered.
        /// </summary>
        /// <param name="observer">The observer of the action.</param>
        /// <returns>A registration object that can be used to unregister.
        /// </returns>
        public override IDisposable Subscribe(IObserver<T> observer)
        {
            IDisposable registration = base.Subscribe(observer);
            Action(this, observer);
            return registration;
        }
    }
}