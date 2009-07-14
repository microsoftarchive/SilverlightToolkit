// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Collection of functions for functional programming tasks.
    /// </summary>
    internal static partial class FunctionalProgramming
    {
        /// <summary>
        /// Traverses a tree by accepting an initial value and a function that 
        /// retrieves the child nodes of a node.
        /// </summary>
        /// <typeparam name="T">The type of the stream.</typeparam>
        /// <param name="initialNode">The initial node.</param>
        /// <param name="getChildNodes">A function that retrieves the child
        /// nodes of a node.</param>
        /// <param name="traversePredicate">A predicate that evaluates a node
        /// and returns a value indicating whether that node and it's children
        /// should be traversed.</param>
        /// <returns>A stream of nodes.</returns>
        internal static IEnumerable<T> TraverseDepthFirst<T>(
            T initialNode,
            Func<T, IEnumerable<T>> getChildNodes,
            Func<T, bool> traversePredicate)
        {
            Stack<T> stack = new Stack<T>();
            stack.Push(initialNode);

            while (stack.Count > 0)
            {
                T node = stack.Pop();
                if (traversePredicate(node))
                {
                    yield return node;
                    IEnumerable<T> childNodes = getChildNodes(node);
                    foreach (T childNode in childNodes)
                    {
                        stack.Push(childNode);
                    }
                }
            }
        }

        /// <summary>
        /// Traverses a tree by accepting an initial value and a function that 
        /// retrieves the child nodes of a node.
        /// </summary>
        /// <typeparam name="T">The type of the stream.</typeparam>
        /// <param name="initialNode">The initial node.</param>
        /// <param name="getChildNodes">A function that retrieves the child
        /// nodes of a node.</param>
        /// <param name="traversePredicate">A predicate that evaluates a node
        /// and returns a value indicating whether that node and it's children
        /// should be traversed.</param>
        /// <returns>A stream of nodes.</returns>
        internal static IEnumerable<T> TraverseBreadthFirst<T>(
            T initialNode,
            Func<T, IEnumerable<T>> getChildNodes,
            Func<T, bool> traversePredicate)
        {
            Queue<T> queue = new Queue<T>();
            queue.Enqueue(initialNode);

            while (queue.Count > 0)
            {
                T node = queue.Dequeue();
                if (traversePredicate(node))
                {
                    yield return node;
                    IEnumerable<T> childNodes = getChildNodes(node);
                    foreach (T childNode in childNodes)
                    {
                        queue.Enqueue(childNode);
                    }
                }
            }
        }

        /// <summary>
        /// Appends an item to the front of a stream.
        /// </summary>
        /// <typeparam name="T">The type of the stream.</typeparam>
        /// <param name="that">The stream.</param>
        /// <param name="value">The value to append to the front of the stream.
        /// </param>
        /// <returns>A stream with the item appended to the front.</returns>
        internal static IEnumerable<T> Append<T>(this IEnumerable<T> that, T value)
        {
            yield return value;
            foreach (T item in that)
            {
                yield return item;
            }
        }
    }
}