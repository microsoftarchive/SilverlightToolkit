// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
using System.Windows.Controls.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Test class for LayoutTransformer.
    /// </summary>
    [TestClass]
    public class LayoutTransformerTest : TestBase
    {
        /// <summary>
        /// Gets a stream of controls to test.
        /// </summary>
        public IEnumerable<ContentControl> ControlsToTest
        {
            get
            {
#if !SILVERLIGHT
                yield return new ContentControl();
#endif
                yield return new LayoutTransformer();
            }
        }

        /// <summary>
        /// Tests LayoutTransformer with no Transform.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with no Transform.")]
        public void NoTransform()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, 200, 100, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, 200, 100, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 200, 200, 100, 100, 200, 200, 200, 200, null));
        }

        /// <summary>
        /// Tests LayoutTransformer with no Width.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with no Width.")]
        public void NoWidth()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 0, 200, 0, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 0, 200, 0, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 0, 200, 0, 100, 200, 200, 200, 200, null));
        }

        /// <summary>
        /// Tests LayoutTransformer with no Height.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with no Height.")]
        public void NoHeight()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, 0, 100, 0, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, 0, 100, 0, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 200, 0, 100, 0, 200, 200, 200, 200, null));
        }

        /// <summary>
        /// Tests LayoutTransformer with infinite Width.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with infinite Width.")]
        public void InfiniteWidth()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, double.PositiveInfinity, 200, 100, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, double.PositiveInfinity, 200, 100, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, double.PositiveInfinity, 200, 100, 100, 200, 200, 200, 200, null));
        }

        /// <summary>
        /// Tests LayoutTransformer with infinite Height.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with infinite Height.")]
        public void InfiniteHeight()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, double.PositiveInfinity, 100, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, double.PositiveInfinity, 100, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 200, double.PositiveInfinity, 100, 100, 200, 200, 200, 200, null));
        }

        /// <summary>
        /// Tests LayoutTransformer with infinite Width and Height.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with infinite Width and Height.")]
        public void InfiniteWidthAndHeight()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, double.PositiveInfinity, double.PositiveInfinity, 100, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, double.PositiveInfinity, double.PositiveInfinity, 100, 100, 200, 200, 200, 200, null));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, double.PositiveInfinity, double.PositiveInfinity, 100, 100, 200, 200, 200, 200, null));
        }

        /// <summary>
        /// Tests a normal RotateTransform.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a normal RotateTransform.")]
        public void Rotate()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, 100, 82, 82, 100, 100, 82, 82, new RotateTransform { Angle = 15 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, 100, 82, 82, 100, 100, 100, 100, new RotateTransform { Angle = 15 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, 100, 82, 82, 100, 100, 100, 100, new RotateTransform { Angle = 15 }));
        }

        /// <summary>
        /// Tests a 45 degree RotateTransform.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a 45 degree RotateTransform.")]
        public void Rotate45()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, 200, 100, 100, 200, 200, 141, 141, new RotateTransform { Angle = 45 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, 200, 100, 100, 200, 200, 141, 141, new RotateTransform { Angle = 45 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 200, 200, 100, 100, 200, 200, 141, 141, new RotateTransform { Angle = 45 }));
        }

        /// <summary>
        /// Tests a constrained 45 degree RotateTransform and larger Width.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a constrained 45 degree RotateTransform and larger Width.")]
        public void Rotate45WidthLargerConstrained()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 110, 100, 71, 71, 110, 100, 71, 71, new RotateTransform { Angle = 45 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 110, 100, 71, 71, 110, 100, 100, 100, new RotateTransform { Angle = 45 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 110, 100, 71, 71, 110, 100, 100, 100, new RotateTransform { Angle = 45 }));
        }

        /// <summary>
        /// Tests a constrained 45 degree RotateTransform and larger Height.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a constrained 45 degree RotateTransform and larger Height.")]
        public void Rotate45HeightLargerConstrained()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, 110, 71, 71, 100, 110, 71, 71, new RotateTransform { Angle = 45 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, 110, 71, 71, 100, 110, 100, 100, new RotateTransform { Angle = 45 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, 110, 71, 71, 100, 110, 100, 100, new RotateTransform { Angle = 45 }));
        }

        /// <summary>
        /// Tests a 90 degree RotateTransform and infinite Width.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a 90 degree RotateTransform and infinite Width.")]
        public void Rotate90WidthInfinite()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, double.PositiveInfinity, 100, 100, 100, 200, 100, 100, 200, new RotateTransform { Angle = 90 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, double.PositiveInfinity, 100, 100, 100, 200, 100, 100, 200, new RotateTransform { Angle = 90 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, double.PositiveInfinity, 100, 100, 100, 200, 100, 100, 200, new RotateTransform { Angle = 90 }));
        }

        /// <summary>
        /// Tests a 90 degree RotateTransform and infinite Height.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a 90 degree RotateTransform and infinite Height.")]
        public void Rotate90HeightInfinite()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, double.PositiveInfinity, 100, 100, 100, 200, 200, 100, new RotateTransform { Angle = 90 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, double.PositiveInfinity, 100, 100, 100, 200, 200, 100, new RotateTransform { Angle = 90 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, double.PositiveInfinity, 100, 100, 100, 200, 200, 100, new RotateTransform { Angle = 90 }));
        }

        /// <summary>
        /// Tests a 180 degree RotateTransform and infinite Height.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a 180 degree RotateTransform and infinite Height.")]
        public void Rotate180WidthInfinite()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, double.PositiveInfinity, 100, 100, 100, 200, 100, 200, 100, new RotateTransform { Angle = 180 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, double.PositiveInfinity, 100, 100, 100, 200, 100, 200, 100, new RotateTransform { Angle = 180 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, double.PositiveInfinity, 100, 100, 100, 200, 100, 200, 100, new RotateTransform { Angle = 180 }));
        }

        /// <summary>
        /// Tests a 180 degree RotateTransform and infinite Width.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a 180 degree RotateTransform and infinite Width.")]
        public void Rotate180HeightInfinite()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, double.PositiveInfinity, 100, 100, 100, 200, 100, 200, new RotateTransform { Angle = 180 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, double.PositiveInfinity, 100, 100, 100, 200, 100, 200, new RotateTransform { Angle = 180 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, double.PositiveInfinity, 100, 100, 100, 200, 100, 200, new RotateTransform { Angle = 180 }));
        }

        /// <summary>
        /// Tests a normal ScaleTransform.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a normal ScaleTransform.")]
        public void Scale()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, 200, 100, 80, 200, 200, 400, 80, new ScaleTransform { ScaleX = 0.5, ScaleY = 2.5 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, 200, 100, 80, 200, 200, 400, 100, new ScaleTransform { ScaleX = 0.5, ScaleY = 2.5 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 200, 200, 100, 80, 200, 200, 400, 100, new ScaleTransform { ScaleX = 0.5, ScaleY = 2.5 }));
        }

        /// <summary>
        /// Tests a ScaleTransform having ScaleX=0.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a ScaleTransform having ScaleX=0.")]
        public void ScaleX0()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, 200, 0, 0, 200, 200, 0, 0, new ScaleTransform { ScaleX = 0, ScaleY = 2 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, 200, 0, 0, 200, 200, 100, 100, new ScaleTransform { ScaleX = 0, ScaleY = 2 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 200, 200, 0, 0, 200, 200, 100, 100, new ScaleTransform { ScaleX = 0, ScaleY = 2 }));
        }

        /// <summary>
        /// Tests a ScaleTransform having ScaleY=0.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a ScaleTransform having ScaleY=0.")]
        public void ScaleY0()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, 200, 0, 0, 200, 200, 0, 0, new ScaleTransform { ScaleX = 2, ScaleY = 0 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, 200, 0, 0, 200, 200, 100, 100, new ScaleTransform { ScaleX = 2, ScaleY = 0 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 200, 200, 0, 0, 200, 200, 100, 100, new ScaleTransform { ScaleX = 2, ScaleY = 0 }));
        }

        /// <summary>
        /// Tests a normal SkewTransform.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a normal SkewTransform.")]
        public void Skew()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, 100, 91, 100, 50, 50, 91, 100, new SkewTransform { AngleX = 5, AngleY = 0 }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, 100, 91, 100, 50, 50, 100, 100, new SkewTransform { AngleX = 5, AngleY = 0 }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, 100, 91, 100, 50, 50, 100, 100, new SkewTransform { AngleX = 5, AngleY = 0 }));
        }

        /// <summary>
        /// Tests a MatrixTransform having A=0.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a MatrixTransform having A=0.")]
        public void MatrixA0()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(0, 1, 1, 1, 0, 0) }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(0, 1, 1, 1, 0, 0) }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(0, 1, 1, 1, 0, 0) }));
        }

        /// <summary>
        /// Tests a MatrixTransform having B=0.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a MatrixTransform having B=0.")]
        public void MatrixB0()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 0, 1, 1, 0, 0) }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 0, 1, 1, 0, 0) }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 0, 1, 1, 0, 0) }));
        }

        /// <summary>
        /// Tests a MatrixTransform having C=0.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a MatrixTransform having C=0.")]
        public void MatrixC0()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 1, 0, 1, 0, 0) }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 1, 0, 1, 0, 0) }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 1, 0, 1, 0, 0) }));
        }

        /// <summary>
        /// Tests a MatrixTransform having D=0.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a MatrixTransform having D=0.")]
        public void MatrixD0()
        {
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 1, 1, 0, 0, 0) }));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 1, 1, 0, 0, 0) }));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 100, 100, 50, 50, 200, 200, 100, 100, new MatrixTransform { Matrix = new Matrix(1, 1, 1, 0, 0, 0) }));
        }

        /// <summary>
        /// Tests a TransformGroup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a TransformGroup.")]
        public void TransformGroup()
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new RotateTransform { Angle = 45 });
            transformGroup.Children.Add(new ScaleTransform { ScaleX = 3, ScaleY = 3 });
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, 200, 47, 47, 200, 200, 47, 47, transformGroup));
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, 200, 47, 47, 200, 200, 100, 100, transformGroup));
            TestScenario(new LayoutTransformerScenario(100, 100, false, true, 200, 200, 47, 47, 200, 200, 100, 100, transformGroup));
        }

        /// <summary>
        /// Tests a null Template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests LayoutTransformer with a null Template.")]
        public void NullTemplate()
        {
            List<ContentControl> controlsToTest = new List<ContentControl>(ControlsToTest);
            foreach (ContentControl control in controlsToTest)
            {
                control.Template = null;
            }
            TestScenario(new LayoutTransformerScenario(100, 100, false, false, 200, 200, 0, 0, 200, 200, 0, 0, null), controlsToTest);
            foreach (ContentControl control in controlsToTest)
            {
                (control.Parent as Panel).Children.Remove(control);
            }
            TestScenario(new LayoutTransformerScenario(100, 100, true, false, 200, 200, 0, 0, 200, 200, 0, 0, null), controlsToTest);
            foreach (ContentControl control in controlsToTest)
            {
                (control.Parent as Panel).Children.Remove(control);
            }
            TestScenario(new LayoutTransformerScenario(100, 100, true, true, 200, 200, 0, 0, 200, 200, 0, 0, null), controlsToTest);
        }

        /// <summary>
        /// Tests setting a null Transform.
        /// </summary>
        [TestMethod]
        [Description("Tests setting a null Transform.")]
        public void SetNullTransform()
        {
            List<ContentControl> controlsToTest = new List<ContentControl>(ControlsToTest);
            foreach (ContentControl control in controlsToTest)
            {
                LayoutTransformer layoutTransformer = control as LayoutTransformer;
                if (null != layoutTransformer)
                {
                    layoutTransformer.LayoutTransform = new RotateTransform { Angle = 10 };
                    layoutTransformer.LayoutTransform = null;
                }
            }
        }

        /// <summary>
        /// Tests a non-UI element in a LayoutTransformer with a ContentTemplate applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests a non-UI element in a LayoutTransformer with a ContentTemplate applied.")]
        public void NonUIElementContentInContentTemplate()
        {
            DataTemplate contentTemplate = (DataTemplate)XamlReader.
#if SILVERLIGHT
                Load(
#else
                Parse(
#endif
                    @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                        @"<Grid Width='150' Height='150'>" +
                            @"<ContentControl Content='{Binding}'/>" +
                        @"</Grid>" +
                    @"</DataTemplate>");
            Size containerSize = new Size(150, 150);
            Size contentSize = new Size(106, 106);
            LayoutTestPanel panel = new LayoutTestPanel(containerSize, containerSize);
            List<ContentControl> controlsToTest = new List<ContentControl>(ControlsToTest);
            foreach (ContentControl control in controlsToTest)
            {
                control.Content = 1.0;
                Transform transform = new RotateTransform { Angle = 45 };
                LayoutTransformer layoutTransformer = control as LayoutTransformer;
                if (null != layoutTransformer)
                {
                    layoutTransformer.LayoutTransform = transform;
                }
#if !SILVERLIGHT
                else
                {
                    control.LayoutTransform = transform;
                }
#endif
                control.ContentTemplate = contentTemplate;
                panel.Children.Add(control);
            }
            TestAsync(
                panel,
                () =>
                {
                    foreach (ContentControl control in controlsToTest)
                    {
                        control.ApplyTemplate();
                        ContentPresenter contentPresenter;
                        DependencyObject child = VisualTreeHelper.GetChild(control, 0);
                        contentPresenter = child as ContentPresenter;
                        if (null == contentPresenter)
                        {
                            contentPresenter = VisualTreeHelper.GetChild(child, 0) as ContentPresenter;
                        }
                        Assert.IsNotNull(contentPresenter);
#if !SILVERLIGHT
                        control.Measure(containerSize);
#endif
                        AssertAreEqual(contentSize, contentPresenter.DesiredSize, control, "Measure");
#if !SILVERLIGHT
                        control.Arrange(new Rect(new Point(), containerSize));
#endif
                        AssertAreEqual(contentSize, contentPresenter.RenderSize, control, "Arrange");
                    }
                });
        }

        /// <summary>
        /// Tests the specified LayoutTransformer scenario.
        /// </summary>
        /// <param name="scenario">Scenario to test.</param>
        private void TestScenario(LayoutTransformerScenario scenario)
        {
            TestScenario(scenario, new List<ContentControl>(ControlsToTest));
        }

        /// <summary>
        /// Tests the specified LayoutTransformer scenario.
        /// </summary>
        /// <param name="scenario">Scenario to test.</param>
        /// <param name="controlsToTest">Stream of controls to test.</param>
        private void TestScenario(LayoutTransformerScenario scenario, IEnumerable<ContentControl> controlsToTest)
        {
            LayoutTestPanel panel = new LayoutTestPanel(scenario.MeasureSize, scenario.ArrangeSize);
            foreach (ContentControl control in controlsToTest)
            {
                LayoutTransformer layoutTransformer = control as LayoutTransformer;
                if (null != layoutTransformer)
                {
                    layoutTransformer.LayoutTransform = scenario.Transform;
                }
#if !SILVERLIGHT
                else
                {
                    control.LayoutTransform = scenario.Transform;
                }
#endif
                control.Content = scenario.Child;
                panel.Children.Add(control);
            }
            TestAsync(
                panel,
                () =>
                {
                    foreach (ContentControl control in controlsToTest)
                    {
                        FrameworkElement child = control.Content as FrameworkElement;
#if !SILVERLIGHT
                        control.Measure(scenario.MeasureSize);
#endif
                        AssertAreEqual(scenario.DesiredSize, child.DesiredSize, control, "Measure");
#if !SILVERLIGHT
                        control.Arrange(new Rect(new Point(), scenario.ArrangeSize));
#endif
                        AssertAreEqual(scenario.RenderSize, child.RenderSize, control, "Arrange");
                    }
                });
        }

        /// <summary>
        /// Asserts the equality of two Sizes for a particular LayoutTransformer scenario.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="control">Control being tested.</param>
        /// <param name="detail">Detail message.</param>
        private static void AssertAreEqual(Size expected, Size actual, ContentControl control, string detail)
        {
            Assert.AreEqual(RoundedSize(expected), RoundedSize(actual), string.Format(CultureInfo.CurrentCulture, "{0}/{1}: Expected ({2}), actual ({3})", control.GetType().Name, detail, expected, actual));
        }

        /// <summary>
        /// Returns a rounded Size instance.
        /// </summary>
        /// <param name="size">Size instance.</param>
        /// <returns>Rounded Size instance.</returns>
        private static Size RoundedSize(Size size)
        {
            return new Size(Math.Round(size.Width), Math.Round(size.Height));
        }
    }
}