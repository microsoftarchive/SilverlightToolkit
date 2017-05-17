// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Controls.Internals;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// A set of extension methods for observables.
    /// </summary>
    internal static class ObservableExtensions
    {
        /// <summary>
        /// Asserts that a predicate is true.
        /// </summary>
        /// <typeparam name="T">The type of the items in the observable.
        /// </typeparam>
        /// <param name="observable">The observable.</param>
        /// <param name="predicate">The predicate to assert.</param>
        /// <returns>An observable.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Design of C# expressions requires nesting of generic types.")]
        public static IObservable<T> Assert<T>(this IObservable<T> observable, Expression<Func<bool>> predicate)
        {
            return observable.Do<T>(_ => Test.Assert(predicate));
        }

        /// <summary>
        /// Queues an action and returns an observable that will yield a null
        /// result when the action is completed.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>An observable that will return null when the action 
        /// completes.</returns>
        public static IObservable<object> Do(Action action)
        {
            return Observable.Return<object>(null).Do(action);
        }

        /// <summary>
        /// Queues an action and returns an observable that will yield a null
        /// result when the action is completed.
        /// </summary>
        /// <typeparam name="T">The type of items in the sequence.</typeparam>
        /// <param name="that">The observable sequence.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>An observable that will return null when the action 
        /// completes.</returns>
        public static IObservable<T> Do<T>(this IObservable<T> that, Action action)
        {
            return that.Do(_ => action());
        }

        /// <summary>
        /// Creates an object an updates the observers with that object.
        /// </summary>
        /// <typeparam name="T">The type of the element created.</typeparam>
        /// <param name="func">The factory function.</param>
        /// <returns>An observable that will return the object when the function 
        /// completes.</returns>
        public static IObservable<T> Create<T>(Func<T> func)
        {
            return Observable.Return(func).Select(f => f());
        }

        /// <summary>
        /// Waits for another observable to return a value.
        /// </summary>
        /// <typeparam name="TFirst">The type of the first observable.</typeparam>
        /// <typeparam name="TSecond">The type of the second observable.</typeparam>
        /// <param name="observable">The first observable.</param>
        /// <param name="nextObservable">A function that returns the second 
        /// observable when executed.</param>
        /// <returns>An observable object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Nested generics are necessary.")]
        public static IObservable<TSecond> WaitFor<TFirst, TSecond>(this IObservable<TFirst> observable, Func<IObservable<TSecond>> nextObservable)
        {
            return observable.SelectMany((_) => nextObservable());
        }

        /// <summary>
        /// Runs an asynchronous test and cleans up afterwards.
        /// </summary>
        /// <typeparam name="T">The type of the items in the sequence.</typeparam>
        /// <param name="observable">The asynchronous test.</param>
        /// <param name="presentationTest">The presentation test object.</param>
        public static void RunAsyncTest<T>(this IObservable<T> observable, PresentationTest presentationTest)
        {
            observable.Subscribe(
                _ =>
                {
                    presentationTest.TestPanel.Children.Clear();
                    presentationTest.EnqueueTestComplete();
                });
        }

        /// <summary>
        /// Converts an observable sequence into a sequence of unit values.
        /// </summary>
        /// <typeparam name="T">The type of the sequence.</typeparam>
        /// <param name="that">The sequence.</param>
        /// <returns>A sequence of unit values.</returns>
        public static IObservable<Unit> IgnoreAll<T>(this IObservable<T> that)
        {
            return that.Select(_ => new Unit());
        }
    }
}