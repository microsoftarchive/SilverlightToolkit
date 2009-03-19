// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Windows.Controls.DataVisualization.Collections
{
    /// <summary>
    /// Implements a basic binary tree that allows duplicates.
    /// </summary>
    /// <typeparam name="TKey">Type for keys.</typeparam>
    /// <typeparam name="TValue">Type for values.</typeparam>
    internal class BinaryTree<TKey, TValue>
    {
        /// <summary>
        /// Represents a node of the binary tree.
        /// </summary>
        private class Node
        {
            /// <summary>
            /// Gets or sets the left node.
            /// </summary>
            public Node Left { get; set; }

            /// <summary>
            /// Gets or sets the right node.
            /// </summary>
            public Node Right { get; set; }

            /// <summary>
            /// Gets or sets the node's key.
            /// </summary>
            public TKey Key { get; set; }

            /// <summary>
            /// Gets or sets the node's value.
            /// </summary>
            public TValue Value { get; set; }
        }

        /// <summary>
        /// Gets the count of key/value pairs in the dictionary.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Stores the key comparison function.
        /// </summary>
        private Comparison<TKey> _keyComparison;

        /// <summary>
        /// Stores the value comparison function.
        /// </summary>
        private Comparison<TValue> _valueComparison;

        /// <summary>
        /// Stores the root node of the binary tree.
        /// </summary>
        private Node _rootNode;

        /// <summary>
        /// Tracks whether the next add should go left to help maintain balance.
        /// </summary>
        private bool _addGoesLeft;

        /// <summary>
        /// Tracks whether the next deletion should go left to help maintain balance.
        /// </summary>
        private bool _deleteGoesLeft;

        /// <summary>
        /// Initializes a new instance of the BinaryTree class.
        /// </summary>
        /// <param name="keyComparison">The key comparison function.</param>
        /// <param name="valueComparison">The value comparison function.</param>
        public BinaryTree(Comparison<TKey> keyComparison, Comparison<TValue> valueComparison)
        {
            Debug.Assert(null != keyComparison, "Must provide a Comparer for TKey.");
            Debug.Assert(null != valueComparison, "Must provide a Comparer for TValue.");
            _keyComparison = keyComparison;
            _valueComparison = valueComparison;
        }

        /// <summary>
        /// Clears all the items in the tree.
        /// </summary>
        public void Clear()
        {
            _rootNode = null;
            Count = 0;
        }

        /// <summary>
        /// Adds a key/value pair to the binary tree.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        public void Add(TKey key, TValue value)
        {
            Count++;
            Node newNode = new Node { Key = key, Value = value };
            if (null == _rootNode)
            {
                // Create new root node
                _rootNode = newNode;
            }
            else
            {
                // Find appropriate place
                Node currentNode = _rootNode;
                while (true)
                {
                    int comparisonResult = KeyAndValueComparison(newNode.Key, newNode.Value, currentNode.Key, currentNode.Value);
                    if (comparisonResult < 0)
                    {
                        if (null == currentNode.Left)
                        {
                            // Add to left
                            currentNode.Left = newNode;
                            break;
                        }
                        else
                        {
                            currentNode = currentNode.Left;
                        }
                    }
                    else if (0 < comparisonResult)
                    {
                        if (null == currentNode.Right)
                        {
                            // Add to right
                            currentNode.Right = newNode;
                            break;
                        }
                        else
                        {
                            currentNode = currentNode.Right;
                        }
                    }
                    else
                    {
                        // Alternate adds to the left/right to keep tree from unbalancing
                        if (_addGoesLeft)
                        {
                            newNode.Left = currentNode.Left;
                            currentNode.Left = newNode;
                        }
                        else
                        {
                            newNode.Right = currentNode.Right;
                            currentNode.Right = newNode;
                        }
                        _addGoesLeft = !_addGoesLeft;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a key/value pair from the binary tree.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        /// <param name="value">Value to remove.</param>
        /// <returns>True if key/value present and removed.</returns>
        public bool Remove(TKey key, TValue value)
        {
            bool removed;
            Node removeResult = Remove(out removed, _rootNode, key, value);
            if (removed)
            {
                // Removed a node; update root node
                _rootNode = removeResult;
                Count--;
                return true;
            }
            else
            {
                // Key/value did not exist
                return false;
            }
        }

        /// <summary>
        /// Helper function to remove a key/value pair from the binary tree.
        /// </summary>
        /// <param name="removed">Set to true if a node was removed.</param>
        /// <param name="currentNode">Current node.</param>
        /// <param name="key">Key to remove.</param>
        /// <param name="value">Value to remove.</param>
        /// <returns>True if key/value present and removed.</returns>
        private Node Remove(out bool removed, Node currentNode, TKey key, TValue value)
        {
            if (null == currentNode)
            {
                // No action taken
                removed = false;
                return null;
            }
            int comparisonResult = KeyAndValueComparison(key, value, currentNode.Key, currentNode.Value);
            if (comparisonResult < 0)
            {
                // Recurse on left
                currentNode.Left = Remove(out removed, currentNode.Left, key, value);
            }
            else if (0 < comparisonResult)
            {
                // Recurse on right
                currentNode.Right = Remove(out removed, currentNode.Right, key, value);
            }
            else
            {
                removed = true;
                if (null == currentNode.Left)
                {
                    // Remove by replacing with right
                    return currentNode.Right;
                }
                else if (null == currentNode.Right)
                {
                    // Remove by replacing with left
                    return currentNode.Left;
                }
                else
                {
                    // Remove by swapping with left/right child - alternate to help keep tree from unbalancing
                    Node replacement;
                    if (_deleteGoesLeft)
                    {
                        replacement = currentNode.Left;
                        while (null != replacement.Right)
                        {
                            replacement = replacement.Right;
                        }
                        currentNode.Key = replacement.Key;
                        currentNode.Value = replacement.Value;
                        currentNode.Left = Remove(out removed, currentNode.Left, replacement.Key, replacement.Value);
                    }
                    else
                    {
                        replacement = currentNode.Right;
                        while (null != replacement.Left)
                        {
                            replacement = replacement.Left;
                        }
                        currentNode.Key = replacement.Key;
                        currentNode.Value = replacement.Value;
                        currentNode.Right = Remove(out removed, currentNode.Right, replacement.Key, replacement.Value);
                    }
                    _deleteGoesLeft = !_deleteGoesLeft;
                }
            }
            return currentNode;
        }

        /// <summary>
        /// Gets a sorted list of the keys.
        /// </summary>
        /// <returns>Sorted list of keys.</returns>
        public IEnumerable<TKey> GetKeys()
        {
            return Traverse(n => true, n => n.Key).DistinctOfSorted();
        }

        /// <summary>
        /// Gets a list of the values associated with the specified key.
        /// </summary>
        /// <param name="key">Specified key.</param>
        /// <returns>List of values.</returns>
        public IEnumerable<TValue> GetValuesForKey(TKey key)
        {
            return Traverse(n => 0 == _keyComparison(n.Key, key), n => n.Value);
        }

        /// <summary>
        /// Gets a list of all the values in the binary tree.
        /// </summary>
        /// <returns>List of all values.</returns>
        public IEnumerable<TValue> GetValuesForAllKeys()
        {
            return Traverse(n => true, n => n.Value);
        }

        /// <summary>
        /// Gets the minimum key.
        /// </summary>
        public TKey MinimumKey
        {
            get
            {
                return GetExtreme(n => n.Left, n => n.Key);
            }
        }

        /// <summary>
        /// Gets the maximum key.
        /// </summary>
        public TKey MaximumKey
        {
            get
            {
                return GetExtreme(n => n.Right, n => n.Key);
            }
        }

        /// <summary>
        /// Gets the minimum key's minimum value.
        /// </summary>
        public TValue MinimumValue
        {
            get
            {
                return GetExtreme(n => n.Left, n => n.Value);
            }
        }

        /// <summary>
        /// Gets the maximum key's maximum value.
        /// </summary>
        public TValue MaximumValue
        {
            get
            {
                return GetExtreme(n => n.Right, n => n.Value);
            }
        }

        /// <summary>
        /// Gets an extreme (minimum/maximum) value.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="successor">Successor function.</param>
        /// <param name="selector">Selector function.</param>
        /// <returns>Extreme value.</returns>
        private T GetExtreme<T>(Func<Node, Node> successor, Func<Node, T> selector)
        {
            T extreme = default(T);
            Node current = _rootNode;
            while (null != current)
            {
                extreme = selector(current);
                current = successor(current);
            }
            return extreme;
        }

        /// <summary>
        /// Traverses a subset of the list of nodes in order, selecting the specified nodes.
        /// </summary>
        /// <typeparam name="T">Type of elements.</typeparam>
        /// <param name="condition">Condition to apply to each node.</param>
        /// <param name="selector">Selection method.</param>
        /// <returns>Sequence of selected nodes.</returns>
        private IEnumerable<T> Traverse<T>(Func<Node, bool> condition, Func<Node, T> selector)
        {
            Stack<Node> stack = new Stack<Node>();
            Node current = _rootNode;
            while (null != current)
            {
                if (null != current.Left)
                {
                    // Save current state and go left
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    do
                    {
                        // Select current node if relevant
                        if (condition(current))
                        {
                            yield return selector(current);
                        }
                        // Go right - or up if nothing to the right
                        current = current.Right;
                    }
                    while ((null == current) &&
                           (0 < stack.Count) &&
                           (null != (current = stack.Pop())));
                }
            }
        }

        /// <summary>
        /// Compares the keys (first) and values (second) specified.
        /// </summary>
        /// <param name="leftKey">The left key.</param>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightKey">The right key.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>CompareTo-style results: -1 if left is less, 0 if equal, and 1 if greater than right.</returns>
        private int KeyAndValueComparison(TKey leftKey, TValue leftValue, TKey rightKey, TValue rightValue)
        {
            int comparisonResult = _keyComparison(leftKey, rightKey);
            if (0 == comparisonResult)
            {
                comparisonResult = _valueComparison(leftValue, rightValue);
            }
            return comparisonResult;
        }
    }
}