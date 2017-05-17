// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.DataVisualization;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// SquaringAlgorithmTest unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeMap")]
    public class SquaringAlgorithmTest : TestBase
    {
        /// <summary>
        /// Delta used for double precision checks.
        /// </summary>
        private const double DELTA = 0.000001;

        /// <summary>
        /// 0 Thickness used in number of tests to simplify calculations.
        /// </summary>
        private static readonly Thickness NoThickness = new Thickness(0);

        /// <summary>
        /// Verifies that parents without children do not generate sub-rectangles.
        /// </summary>
        [TestMethod]
        [Description("Check if parents without children do not generate sub-rectangles.")]
        public void NullChildrenReturnsEmptyEnumeration()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 1.0,
                Children = null
            };

            IEnumerable<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();

            Assert.IsFalse(result.Any());
        }

        /// <summary>
        /// Verifies that parents with 0 size children do not generate sub-rectangles.
        /// </summary>
        [TestMethod]
        [Description("Check if parents with 0 size children do not generate sub-rectangles.")]
        public void ZeroParentAreaReturnsEmptyEnumeration()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 0.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 0.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();

            Assert.IsFalse(result.Any());
        }

        /// <summary>
        /// Verifies that parents with >0 size children do generate sub-rectangles for
        /// children >0.
        /// </summary>
        [TestMethod]
        [Description("Check if parents with >0 size children do generate sub-rectangles.")]
        public void ZeroChildAreaReturnsNonemptyEnumeration()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 1.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 0.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 80, 100), node, NoThickness).ToArray();

            Assert.AreEqual(1, result.Count);
        }

        /// <summary>
        /// Verifies that a single child takes all the space in parent.
        /// </summary>
        [TestMethod]
        [Description("Check if a single child takes all the space in parent.")]
        public void SingleChildTakesAllSpace()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 1.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(100, result[0].Item1.Width, DELTA);
            Assert.AreEqual(100, result[0].Item1.Height, DELTA);
        }

        /// <summary>
        /// Verifies that a two children the area as they should (smaller first).
        /// </summary>
        [TestMethod]
        [Description("Verifies that a two children the area as they should (smaller first).")]
        public void TwoOrderedChildrenSplitAreaProperly()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 9.0, Children = null },
                    new TreeMapNode { Area = 1.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(9000, result[0].Item1.Width * result[0].Item1.Height, DELTA);
            Assert.AreEqual(1000, result[1].Item1.Width * result[1].Item1.Height, DELTA);
        }

        /// <summary>
        /// Verifies that a two children the area as they should (bigger first).
        /// </summary>
        [TestMethod]
        [Description("Verifies that a two children the area as they should (bigger first).")]
        public void TwoReverseOrderedChildrenSplitAreaProperly()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 9.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(9000, result[0].Item1.Width * result[0].Item1.Height, DELTA);
            Assert.AreEqual(1000, result[1].Item1.Width * result[1].Item1.Height, DELTA);
        }

        /// <summary>
        /// Verifies that children with smaller sum than parent do not take the whole parent area.
        /// </summary>
        [TestMethod]
        [Description("Verifies that children with smaller sum than parent do not take the whole parent area.")]
        public void ChildrenNotAddingToParent()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 2.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(result[0].Item1.Width * result[0].Item1.Height, 1999, 2001);
            Assert.AreEqual(result[1].Item1.Width * result[1].Item1.Height, 999, 1001);
        }

        /// <summary>
        /// Verifies that parent with greater area than its children has correct size.
        /// </summary>
        [TestMethod]
        [Description("Verifies that parent with greater area than its children has correct size.")]
        public void PlaceholderRectangleIsCorrect()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode
                    {
                        Area = 10.0,
                        Children = new List<TreeMapNode>
                        {
                            new TreeMapNode { Area = 2.0, Children = null },
                            new TreeMapNode { Area = 1.0, Children = null }
                        }
                    }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();
            Assert.AreEqual(new Rect(0, 0, 100, 100), result[0].Item1);
        }

        /// <summary>
        /// Verifies that Tuple Rect, TreeMapNode has correct data.
        /// </summary>
        [TestMethod]
        [Description("Verifies that Tuple<Rect, TreeMapNode> has correct data.")]
        public void TupleRectTreeMapNodeTakesCorrectNode()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 9.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();

            Assert.IsTrue(Object.ReferenceEquals(node.Children.First(), result[1].Item2));
        }

        /// <summary>
        /// Verifies that Rectangles do not overlap.
        /// </summary>
        [TestMethod]
        [Description("Verifies that Rectangles do not overlap.")]
        public void RectanglesDoNotOverlap()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 9.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 100), node, NoThickness).ToArray();

            Assert.AreEqual(0, result[0].Item1.Top, DELTA);
            Assert.AreEqual(90, result[0].Item1.Bottom, DELTA);
            Assert.AreEqual(90, result[1].Item1.Top, DELTA);
            Assert.AreEqual(100, result[1].Item1.Bottom, DELTA);
        }

        /// <summary>
        /// Verifies that Multiple Vertical Children take all horizontal space.
        /// </summary>
        [TestMethod]
        [Description("Multiple Vertical Children take all horizontal space.")]
        public void MultipleVerticalChildrenTakeAllHorizontalSpace()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 9.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(0, 0, 100, 150), node, NoThickness).ToArray();

            Assert.AreEqual(0, result[0].Item1.Left, DELTA);
            Assert.AreEqual(100, result[0].Item1.Right, DELTA);
            Assert.AreEqual(0, result[1].Item1.Left, DELTA);
            Assert.AreEqual(100, result[1].Item1.Right, DELTA);
        }

        /// <summary>
        /// Verifies that Children have correct sizes if initial offset x,y > 0.
        /// </summary>
        [TestMethod]
        [Description("Verifies that Children have correct sizes if initial offset x,y > 0.")]
        public void CanSplitRectanglesWithTranslatedViewport()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 9.0, Children = null }
                }
            };

            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(50, 50, 80, 100), node, NoThickness).ToArray();

            Assert.AreEqual(50, result[0].Item1.Top, DELTA);
            Assert.AreEqual(140, result[0].Item1.Bottom, DELTA);
            Assert.AreEqual(140, result[1].Item1.Top, DELTA);
            Assert.AreEqual(150, result[1].Item1.Bottom, DELTA);
            Assert.AreEqual(50, result[0].Item1.Left, DELTA);
            Assert.AreEqual(130, result[0].Item1.Right,  DELTA);
            Assert.AreEqual(50, result[1].Item1.Left, DELTA);
            Assert.AreEqual(130, result[1].Item1.Right, DELTA);
        }

        /// <summary>
        /// Verifies that thickness is subtracted from rectangles.
        /// </summary>
        [TestMethod]
        [Description("Verifies that thickness is subtracted from rectangles.")]
        public void ThicknessIsSubtractedFromRectangleSize()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 9.0, Children = null }
                }
            };
            Thickness thickness = new Thickness(10, 30, 20, 40);
            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(10, 30, 130, 170), node, thickness).ToArray();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(9000, result[0].Item1.Width * result[0].Item1.Height);
            Assert.AreEqual(1000, result[1].Item1.Width * result[1].Item1.Height);
        }

        /// <summary>
        /// Verifies that rectangles with thickness are correctly split.
        /// </summary>
        [TestMethod]
        [Description("Verifies that rectangles with thickness are correctly split.")]
        public void CanSplitRectanglesWithThickness()
        {
            SquaringAlgorithm rect = new SquaringAlgorithm();
            TreeMapNode node = new TreeMapNode
            {
                Area = 10.0,
                Children = new List<TreeMapNode>
                {
                    new TreeMapNode { Area = 1.0, Children = null },
                    new TreeMapNode { Area = 9.0, Children = null }
                }
            };

            Thickness thickness = new Thickness(10, 30, 20, 40);
            IList<Tuple<Rect, TreeMapNode>> result = rect.Split(new Rect(40, 20, 110, 170), node, thickness).ToArray();

            Assert.AreEqual(50, result[0].Item1.Top, DELTA);
            Assert.AreEqual(140, result[0].Item1.Bottom, DELTA);
            Assert.AreEqual(140, result[1].Item1.Top, DELTA);
            Assert.AreEqual(150, result[1].Item1.Bottom, DELTA);
            Assert.AreEqual(50, result[0].Item1.Left, DELTA);
            Assert.AreEqual(130, result[0].Item1.Right, DELTA);
            Assert.AreEqual(50, result[1].Item1.Left, DELTA);
            Assert.AreEqual(130, result[1].Item1.Right, DELTA);
        }
    }
}