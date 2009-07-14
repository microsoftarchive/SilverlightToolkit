// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    /// A class that facilitates the creation of anonymous IObservable objects.
    /// </summary>
    /// <typeparam name="T">The type of the items in the sequence.</typeparam>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class AnonymousObservable<T> : IObservable<T>
    {
        /// <summary>
        /// The function to invoke when the subscribe method is called.
        /// </summary>
        private Func<IObserver<T>, IDisposable> _subscribe;

        /// <summary>
        /// Initializes a new instance of the AnonymousObservable class.
        /// </summary>
        /// <param name="subscribe">The method to invoke when the subscribe 
        /// method is called.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to compensate for the lack of anonymous interface implementation.")]
        public AnonymousObservable(Func<IObserver<T>, IDisposable> subscribe)
        {
            Debug.Assert(subscribe != null, "subscribe must not be null.");
            this._subscribe = subscribe;
        }

        /// <summary>
        /// This method ensures that an observer is subscribing to the 
        /// information in the observable.
        /// </summary>
        /// <param name="observer">The observer that is subscribing to the 
        /// information in the observable.</param>
        /// <returns>An disposable object used to unsubscribe from the 
        /// observable.</returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }
            return _subscribe(observer);
        }
    }
}