// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if SILVERLIGHT
using System.Windows.Controls.DataVisualization.Collections;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Class implementing unit tests for the LeftLeaningRedBlackTree class.
    /// </summary>
    [TestClass]
    public class LeftLeaningRedBlackTreeTest
    {
        /// <summary>
        /// Number of scenarios to run.
        /// </summary>
        private const int Scenarios = 250;

        /// <summary>
        /// Time-dependent seed to use for variation across each test run.
        /// </summary>
        private int _seed = (int)DateTime.Now.Ticks;

        /// <summary>
        /// Initializes a new instance of the LeftLeaningRedBlackTreeTest class.
        /// </summary>
        public LeftLeaningRedBlackTreeTest()
        {
            // Output "random" seed to make reproducing failures easy
            Debug.WriteLine("LeftLeaningRedBlackTreeTest seed: " + _seed);
        }

        /// <summary>
        /// Verifies that a key comparer is required.
        /// </summary>
        [TestMethod]
        [Description("Verifies that a key comparer is required.")]
        [ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Controls.DataVisualization.Collections.LeftLeaningRedBlackTree`2<System.Int32,System.Int32>",
            Justification = "Need to call constructor to throw exception.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void NullKeyComparer()
        {
            new LeftLeaningRedBlackTree<int, int>(null, IntComparison);
        }

        /// <summary>
        /// Verifies that a value comparer is required.
        /// </summary>
        [TestMethod]
        [Description("Verifies that a value comparer is required.")]
        [ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Controls.DataVisualization.Collections.LeftLeaningRedBlackTree`2<System.Int32,System.Int32>",
            Justification = "Need to call constructor to throw exception.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void NullValueComparer()
        {
            new LeftLeaningRedBlackTree<int, int>(IntComparison, null);
        }

        /// <summary>
        /// Verifies that it is an error to call Remove(key) on a multi-dictionary.
        /// </summary>
        [TestMethod]
        [Description("Verifies that it is an error to call Remove(key) on a multi-dictionary.")]
        [ExpectedException(typeof(InvalidOperationException))]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void RemoveKeyMultiple()
        {
            LeftLeaningRedBlackTree<int, int> binaryTree = new LeftLeaningRedBlackTree<int, int>(IntComparison, IntComparison);
            binaryTree.Remove(-1);
        }

        /// <summary>
        /// Verifies that it is an error to call GetValueForKey(key) on a multi-dictionary.
        /// </summary>
        [TestMethod]
        [Description("Verifies that it is an error to call GetValueForKey(key) on a multi-dictionary.")]
        [ExpectedException(typeof(InvalidOperationException))]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void GetValueForKeyMultiple()
        {
            LeftLeaningRedBlackTree<int, int> binaryTree = new LeftLeaningRedBlackTree<int, int>(IntComparison, IntComparison);
            binaryTree.GetValueForKey(-1);
        }

        /// <summary>
        /// Verifies that calling GetValueForKey throws when the key is not present.
        /// </summary>
        [TestMethod]
        [Description("Verifies that calling GetValueForKey throws when the key is not present.")]
        [ExpectedException(typeof(KeyNotFoundException))]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void GetValueForMissingKey()
        {
            LeftLeaningRedBlackTree<int, int> binaryTree = new LeftLeaningRedBlackTree<int, int>(IntComparison);
            binaryTree.GetValueForKey(-1);
        }

        /// <summary>
        /// Runs the random scenarios on a normal (non-multi) dictionary with a static seed.
        /// </summary>
        [TestMethod]
        [Description("Runs the random scenarios on a normal (non-multi) dictionary with a static seed.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void RandomScenariosNormalStatic()
        {
            RandomScenarios(1, false);
        }

        /// <summary>
        /// Runs the random scenarios on a normal multi-dictionary with a static seed.
        /// </summary>
        [TestMethod]
        [Description("Runs the random scenarios on a normal multi-dictionary with a static seed.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void RandomScenariosMultipleStatic()
        {
            RandomScenarios(1, true);
        }

        /// <summary>
        /// Runs the random scenarios on a normal (non-multi) dictionary with a variable seed.
        /// </summary>
        [TestMethod]
        [Description("Runs the random scenarios on a normal (non-multi) dictionary with a variable seed.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void RandomScenariosNormalVariable()
        {
            RandomScenarios(_seed, false);
        }

        /// <summary>
        /// Runs the random scenarios on a normal multi-dictionary with a variable seed.
        /// </summary>
        [TestMethod]
        [Description("Runs the random scenarios on a normal multi-dictionary with a variable seed.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TestMethods can not be static.")]
        public void RandomScenariosMultipleVariable()
        {
            RandomScenarios(_seed, true);
        }

        /// <summary>
        /// Verifies all public methods with a large number of 'random' scenarios.
        /// </summary>
        /// <param name="seed">Seed for Random constructor.</param>
        /// <param name="isMultiDictionary">True if testing a multi-dictionary.</param>
        private static void RandomScenarios(int seed, bool isMultiDictionary)
        {
            // Use fixed sead for reproducability
            Random rand = new Random(seed);
            for (int scenario = 0; scenario < Scenarios; scenario++)
            {
                LeftLeaningRedBlackTree<int, int> binaryTree = isMultiDictionary ?
                        new LeftLeaningRedBlackTree<int, int>(IntComparison, IntComparison) :
                        new LeftLeaningRedBlackTree<int, int>(IntComparison);

                // Randomize parameters
                int elementBound = rand.Next(1, 100);
                int keyBound = rand.Next(1, 50);
                int valueBound = rand.Next(1, 50);

                // Add random elements
                List<KeyValuePair<int, int>> elements = new List<KeyValuePair<int, int>>();
                for (int i = 0; i < elementBound; i++)
                {
                    KeyValuePair<int, int> element = new KeyValuePair<int, int>(rand.Next(keyBound), rand.Next(valueBound));
                    if (!isMultiDictionary)
                    {
                        IEnumerable<KeyValuePair<int, int>> matches = elements.Where(p => p.Key == element.Key).ToList();
                        foreach (KeyValuePair<int, int> match in matches)
                        {
                            elements.Remove(match);
                        }
                    }
                    elements.Add(element);
                    binaryTree.Add(element.Key, element.Value);
                    CheckTree(binaryTree, elements, isMultiDictionary);
                }

                // Try to remove some elements that aren't present
                int removeBound = rand.Next(20);
                for (int i = 0; i < removeBound; i++)
                {
                    int key = rand.Next(keyBound);
                    int value = rand.Next(valueBound);
                    if (!elements.Where(p => (key == p.Key) && (!isMultiDictionary || (value == p.Value))).Any())
                    {
                        if (isMultiDictionary)
                        {
                            Assert.IsFalse(binaryTree.Remove(key, value));
                        }
                        else
                        {
                            Assert.IsFalse(binaryTree.Remove(key));
                        }
                        CheckTree(binaryTree, elements, isMultiDictionary);
                    }
                }

                // Remove all elements in random order
                while (0 < elements.Count)
                {
                    int index = rand.Next(elements.Count);
                    KeyValuePair<int, int> element = elements[index];
                    elements.RemoveAt(index);
                    if (isMultiDictionary)
                    {
                        Assert.IsTrue(binaryTree.Remove(element.Key, element.Value));
                    }
                    else
                    {
                        Assert.IsTrue(binaryTree.Remove(element.Key));
                    }
                    CheckTree(binaryTree, elements, isMultiDictionary);
                }

                // Final verification
                Assert.IsFalse(binaryTree.Remove(0, 0));
                elements.Clear();
                binaryTree.Clear();
                CheckTree(binaryTree, elements, isMultiDictionary);
            }
        }

        /// <summary>
        /// Checks all properties of the specified tree for correctness.
        /// </summary>
        /// <typeparam name="TKey">Type of keys.</typeparam>
        /// <typeparam name="TValue">Type of values.</typeparam>
        /// <param name="tree">Specified tree.</param>
        /// <param name="elements">Expected elements in the tree.</param>
        /// <param name="isMulti">True if testing the multi-dictionary scenario.</param>
        private static void CheckTree<TKey, TValue>(LeftLeaningRedBlackTree<TKey, TValue> tree, List<KeyValuePair<TKey, TValue>> elements, bool isMulti) where TKey : IComparable
        {
            // Check count and keys
            Assert.AreEqual(elements.Count, tree.Count);
            List<TKey> sortedKeys = elements.Select(e => e.Key).Distinct().OrderBy(k => k).ToList();
            List<TKey> binaryTreeKeys = tree.GetKeys().ToList();
            Assert.AreEqual(sortedKeys.Count, binaryTreeKeys.Count);

            // Check key values
            for (int i = 0; i < sortedKeys.Count; i++)
            {
                Assert.AreEqual(sortedKeys[i], binaryTreeKeys[i]);
                List<TValue> sortedKeyValues = elements.Where(e => 0 == e.Key.CompareTo(sortedKeys[i])).Select(e => e.Value).OrderBy(v => v).ToList();
                List<TValue> binaryTreeKeyValues = tree.GetValuesForKey(sortedKeys[i]).ToList();
                Assert.AreEqual(sortedKeyValues.Count, binaryTreeKeyValues.Count);
                for (int j = 0; j < sortedKeyValues.Count; j++)
                {
                    Assert.AreEqual(sortedKeyValues[j], binaryTreeKeyValues[j]);
                }
                if (!isMulti)
                {
                    Assert.AreEqual(tree.GetValueForKey(sortedKeys[i]), binaryTreeKeyValues.Single());
                }
            }

            // Check values
            List<TValue> sortedValues = sortedKeys.SelectMany(k => elements.Where(e => 0 == e.Key.CompareTo(k)).Select(e => e.Value).OrderBy(v => v)).ToList();
            List<TValue> binaryTreeValues = tree.GetValuesForAllKeys().ToList();
            Assert.AreEqual(sortedValues.Count, binaryTreeValues.Count);
            for (int i = 0; i < sortedValues.Count; i++)
            {
                Assert.AreEqual(sortedValues[i], binaryTreeValues[i]);
            }

            // Check additional properties
            if (0 < elements.Count)
            {
                Assert.AreEqual(sortedKeys[0], tree.MinimumKey);
                Assert.AreEqual(sortedKeys[sortedKeys.Count - 1], tree.MaximumKey);
                Assert.AreEqual(sortedValues[0], tree.MinimumValue);
                Assert.AreEqual(sortedValues[sortedValues.Count - 1], tree.MaximumValue);
            }
        }

        /// <summary>
        /// Comparison function for two ints.
        /// </summary>
        /// <param name="left">The left int.</param>
        /// <param name="right">The right int.</param>
        /// <returns>CompareTo-style results: -1 if left is less, 0 if equal, and 1 if greater than right.</returns>
        private static int IntComparison(int left, int right)
        {
            return left.CompareTo(right);
        }
    }
}
