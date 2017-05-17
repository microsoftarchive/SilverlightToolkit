// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Internals;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// LinearClipper unit tests.
    /// </summary>
    [TestClass]
    [Tag("RatingTest")]
    public class LinearClipperTest : ClipperTest
    {
        /// <summary>
        /// Initializes a new instance of the LinearClipperTest class.
        /// </summary>
        public LinearClipperTest()
        {
        }
        
        /// <summary>
        /// Gets an instance of the LinearClipper class.
        /// </summary>
        public override Clipper DefaultClipperToTest
        {
            get { return new LinearClipper(); }
        }

        /// <summary>
        /// Gets a sequence of clippers to test.
        /// </summary>
        public override System.Collections.Generic.IEnumerable<Clipper> ClippersToTest
        {
            get { yield return new LinearClipper(); }
        }

        /// <summary>
        /// Tests that the clip property is null when the RatioVisible is 1.0 
        /// and the ExpandDirection is Right.
        /// </summary>
        [TestMethod]
        [Description("Tests that the clip property is null when RatioVisible is 1.0 and the ExpandDirection is Right.")]
        [Asynchronous]
        public void TestRightVisible()
        {
            LinearClipper clipper = (LinearClipper)DefaultClipperToTest;
            clipper.Content = new TextBlock { Text = "Testing" };
            TestPanel.Children.Add(clipper);
            TestPanel
                .GetLayoutUpdated()
                .Take(1)
                .Do(_ =>
                    {
                        clipper.ExpandDirection = ExpandDirection.Right;
                        clipper.RatioVisible = 1.0;
                    })
                .Assert(() => ((RectangleGeometry)clipper.Clip).Rect.Width == clipper.RenderSize.Width)
                .Subscribe(_ =>
                    {
                        TestPanel.Children.Remove(clipper);
                        EnqueueTestComplete();
                    });
        }

        /// <summary>
        /// Tests that the clip property is null when the RatioVisible is 1.0 
        /// and the ExpandDirection is Left.
        /// </summary>
        [TestMethod]
        [Description("Tests that the clip property is null when the RatioVisible is 1.0 and the ExpandDirection is Left.")]
        [Asynchronous]
        public void TestLeftVisible()
        {
            LinearClipper clipper = (LinearClipper)DefaultClipperToTest;
            clipper.Content = new TextBlock { Text = "Testing" };
            TestPanel.Children.Add(clipper);
            TestPanel
                .GetLayoutUpdated()
                .Take(1)
                .Do(_ =>
                {
                    clipper.ExpandDirection = ExpandDirection.Left;
                    clipper.RatioVisible = 1.0;
                })
                .Assert(() => ((RectangleGeometry)clipper.Clip).Rect.Width == clipper.RenderSize.Width)
                .Subscribe(_ =>
                {
                    TestPanel.Children.Remove(clipper);
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// Tests that the clip property is null when the RatioVisible is 1.0 
        /// and the ExpandDirection is Up.
        /// </summary>
        [TestMethod]
        [Description("Tests that the clip property is null when the RatioVisible is 1.0 and the ExpandDirection is Up.")]
        [Asynchronous]
        public void TestUpVisible()
        {
            LinearClipper clipper = (LinearClipper)DefaultClipperToTest;
            clipper.Content = new TextBlock { Text = "Testing" };
            TestPanel.Children.Add(clipper);
            TestPanel
                .GetLayoutUpdated()
                .Take(1)
                .Do(_ =>
                {
                    clipper.ExpandDirection = ExpandDirection.Up;
                    clipper.RatioVisible = 1.0;
                })
                .Assert(() => ((RectangleGeometry)clipper.Clip).Rect.Height == clipper.RenderSize.Height)
                .Subscribe(_ =>
                {
                    TestPanel.Children.Remove(clipper);
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// Tests that the clip property is null when the RatioVisible is 1.0 
        /// and the ExpandDirection is Down.
        /// </summary>
        [TestMethod]
        [Description("Tests that the clip property is null when the RatioVisible is 1.0 and the ExpandDirection is Down.")]
        [Asynchronous]
        public void TestDownVisible()
        {
            LinearClipper clipper = (LinearClipper)DefaultClipperToTest;
            clipper.Content = new TextBlock { Text = "Testing" };
            TestPanel.Children.Add(clipper);
            TestPanel
                .GetLayoutUpdated()
                .Take(1)
                .Do(_ =>
                {
                    clipper.ExpandDirection = ExpandDirection.Down;
                    clipper.RatioVisible = 1.0;
                })
                .Assert(() => ((RectangleGeometry)clipper.Clip).Rect.Height == clipper.RenderSize.Height)
                .Subscribe(_ =>
                {
                    TestPanel.Children.Remove(clipper);
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// Tests that the clip property is null when the RatioVisible is 1.0 
        /// and the ExpandDirection is Right.
        /// </summary>
        [TestMethod]
        [Description("Tests that the clip property is null when the RatioVisible is 1.0 and the ExpandDirection is Right.")]
        [Asynchronous]
        public void TestRightEmpty()
        {
            LinearClipper clipper = (LinearClipper)DefaultClipperToTest;
            clipper.Content = new TextBlock { Text = "Testing" };
            TestPanel.Children.Add(clipper);
            TestPanel
                .GetLayoutUpdated()
                .Take(1)
                .Do(_ =>
                {
                    clipper.ExpandDirection = ExpandDirection.Right;
                    clipper.RatioVisible = 0.0;
                })
                .Assert(() => ((RectangleGeometry)clipper.Clip).Rect.Width == 0)
                .Subscribe(_ =>
                {
                    TestPanel.Children.Remove(clipper);
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// Tests that the clip property is null when the RatioVisible is 1.0 
        /// and the ExpandDirection is Left.
        /// </summary>
        [TestMethod]
        [Description("Tests that the clip property is null when the RatioVisible is 1.0 and the ExpandDirection is Left.")]
        [Asynchronous]
        public void TestLeftEmpty()
        {
            LinearClipper clipper = (LinearClipper)DefaultClipperToTest;
            clipper.Content = new TextBlock { Text = "Testing" };
            TestPanel.Children.Add(clipper);
            TestPanel
                .GetLayoutUpdated()
                .Take(1)
                .Do(_ =>
                {
                    clipper.ExpandDirection = ExpandDirection.Left;
                    clipper.RatioVisible = 0.0;
                })
                .Assert(() => ((RectangleGeometry)clipper.Clip).Rect.Width == 0)
                .Subscribe(_ =>
                {
                    TestPanel.Children.Remove(clipper);
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// Tests that the clip property is null when the RatioVisible is 1.0 
        /// and the ExpandDirection is Up.
        /// </summary>
        [TestMethod]
        [Description("Tests that the clip property is null when the RatioVisible is 1.0 and the ExpandDirection is Up.")]
        [Asynchronous]
        public void TestUpEmpty()
        {
            LinearClipper clipper = (LinearClipper)DefaultClipperToTest;
            clipper.Content = new TextBlock { Text = "Testing" };
            TestPanel.Children.Add(clipper);
            TestPanel
                .GetLayoutUpdated()
                .Take(1)
                .Do(_ =>
                {
                    clipper.ExpandDirection = ExpandDirection.Up;
                    clipper.RatioVisible = 0.0;
                })
                .Assert(() => ((RectangleGeometry)clipper.Clip).Rect.Height == 0)
                .Subscribe(_ =>
                {
                    TestPanel.Children.Remove(clipper);
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// Tests that the clip property is null when the RatioVisible is 1.0 
        /// and the ExpandDirection is Down.
        /// </summary>
        [TestMethod]
        [Description("Tests that the clip property is null when the RatioVisible is 1.0 and the ExpandDirection is Down.")]
        [Asynchronous]
        public void TestDownEmpty()
        {
            LinearClipper clipper = (LinearClipper)DefaultClipperToTest;
            clipper.Content = new TextBlock { Text = "Testing" };
            TestPanel.Children.Add(clipper);
            TestPanel
                .GetLayoutUpdated()
                .Take(1)
                .Do(_ =>
                {
                    clipper.ExpandDirection = ExpandDirection.Down;
                    clipper.RatioVisible = 0.0;
                })
                .Assert(() => ((RectangleGeometry)clipper.Clip).Rect.Height == 0)
                .Subscribe(_ =>
                {
                    TestPanel.Children.Remove(clipper);
                    EnqueueTestComplete();
                });
        }
    }
}