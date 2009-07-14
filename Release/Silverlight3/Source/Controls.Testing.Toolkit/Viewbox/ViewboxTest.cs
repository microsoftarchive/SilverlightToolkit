// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Viewbox unit tests.
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Viewbox", Justification = "Consistency with WPF")]
    public partial class ViewboxTest : ControlTest
    {
        #region Controls to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return DefaultViewboxToTest; }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get { return ViewboxesToTest.OfType<Control>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { return OverriddenViewboxesToTest.OfType<IOverriddenControl>(); }
        }
        #endregion Controls to test

        #region Viewboxes to test
        /// <summary>
        /// Gets a default instance of Viewbox (or a derived type) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Viewbox", Justification = "Consistency with WPF")]
        public virtual Viewbox DefaultViewboxToTest
        {
            get { return new Viewbox(); }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Viewboxes", Justification = "Consistency with WPF")]
        public virtual IEnumerable<Viewbox> ViewboxesToTest
        {
            get
            {
                yield return DefaultViewboxToTest;

                Viewbox vb = new Viewbox()
                {
                    Width = 400,
                    Height = 400,
                    Background = new SolidColorBrush(Colors.Green),
                    Stretch = Stretch.Fill,
                    StretchDirection = StretchDirection.DownOnly,
                };

                string xmlns = " xmlns=\"http://schemas.microsoft.com/client/2007\" " +
                    " xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" " +
                    " xmlns:controls=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls\" ";
                string xmlChild = "<Grid " + xmlns + " Background=\"Yellow\" > " +
                    "   <Grid.ColumnDefinitions> " +
                    "       <ColumnDefinition Width=\"*\" /> " +
                    "       <ColumnDefinition Width=\"*\" /> " +
                    "   </Grid.ColumnDefinitions> " +
                    "   <controlsToolkit:Viewbox Stretch=\"None\" Background=\"Chocolate\" HorizontalContentAlignment=\"Center\" VerticalContentAlignment=\"Center\" > " +
                    "       <Rectangle Width=\"2000\" Height=\"10\" Stroke=\"Fuchsia\" StrokeThickness=\"2\" Fill=\"Violet\" Stretch=\"UniformToFill\"></Rectangle> " +
                    "   </controlsToolkit:Viewbox> " +
                    "   <controlsToolkit:Viewbox Grid.Column=\"1\" Stretch=\"Fill\"> " +
                    "       <ScrollViewer> " +
                    "           <Rectangle Width=\"2000\" Height=\"10\" Stroke=\"Fuchsia\" StrokeThickness=\"2\" Fill=\"Violet\" Stretch=\"Uniform\"></Rectangle> " +
                    "       </ScrollViewer> " +
                    "   </controlsToolkit:Viewbox> " +
                    "</Grid> ";
                UIElement child = XamlReader.Load(xmlChild) as UIElement;
                Assert.IsNotNull(child, "XamlReader.Load(xmlChild) as UIElement returned null");
                vb.Child = child;

                yield return vb;
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenViewbox (or derived types) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Viewboxes", Justification = "Consistency with WPF")]
        public virtual IEnumerable<IOverriddenViewbox> OverriddenViewboxesToTest
        {
            get { yield break; }
        }
        #endregion Controls to test

        /// <summary>
        /// Gets Stretch dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Viewbox, Stretch> StretchProperty { get; private set; }

        /// <summary>
        /// Gets StretchDirection dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Viewbox, StretchDirection> StretchDirectionProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ViewboxTest class.
        /// </summary>
        public ViewboxTest()
            : base()
        {
            Func<Viewbox> initializer = () => DefaultViewboxToTest;

            StretchProperty = new DependencyPropertyTest<Viewbox, Stretch>(this, "Stretch")
            {
                Property = Viewbox.StretchProperty,
                Initializer = initializer,
                DefaultValue = Stretch.Uniform,
                OtherValues = new Stretch[] { Stretch.None, Stretch.Fill, Stretch.UniformToFill },
                InvalidValues = new Dictionary<Stretch, Type>
                {
                    { (Stretch)(-1), typeof(ArgumentException) },
                    { (Stretch)4, typeof(ArgumentException) },
                    { (Stretch)5, typeof(ArgumentException) },
                    { (Stretch)500, typeof(ArgumentException) },
                    { (Stretch)int.MaxValue, typeof(ArgumentException) },
                    { (Stretch)int.MinValue, typeof(ArgumentException) }
                }
            };

            StretchDirectionProperty = new DependencyPropertyTest<Viewbox, StretchDirection>(this, "StretchDirection")
            {
                Property = Viewbox.StretchDirectionProperty,
                Initializer = initializer,
                DefaultValue = StretchDirection.Both,
                OtherValues = new StretchDirection[] { StretchDirection.DownOnly, StretchDirection.UpOnly },
                InvalidValues = new Dictionary<StretchDirection, Type>
                {
                    { (StretchDirection)(-1), typeof(ArgumentException) },
                    { (StretchDirection)3, typeof(ArgumentException) },
                    { (StretchDirection)4, typeof(ArgumentException) },
                    { (StretchDirection)500, typeof(ArgumentException) },
                    { (StretchDirection)int.MaxValue, typeof(ArgumentException) },
                    { (StretchDirection)int.MinValue, typeof(ArgumentException) }
                }
            };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // Remove tests inherited from ControlTest that require setting template
            // because Viewbox doesn't allow template to be redefined.
            tests.RemoveTests(BackgroundProperty.TemplateBindTest);
            tests.RemoveTests(BorderBrushProperty.TemplateBindTest);
            tests.RemoveTests(BorderThicknessProperty.TemplateBindTest);
            tests.RemoveTests(FontFamilyProperty.TemplateBindTest);
            tests.RemoveTests(FontSizeProperty.TemplateBindTest);
            tests.RemoveTests(FontStretchProperty.TemplateBindTest);
            tests.RemoveTests(FontStyleProperty.TemplateBindTest);
            tests.RemoveTests(FontWeightProperty.TemplateBindTest);
            tests.RemoveTests(ForegroundProperty.TemplateBindTest);
            tests.RemoveTests(HorizontalAlignmentProperty.TemplateBindTest);
            tests.RemoveTests(HorizontalContentAlignmentProperty.TemplateBindTest);
            tests.RemoveTests(IsEnabledProperty.TemplateBindTest);
            tests.RemoveTests(PaddingProperty.TemplateBindTest);
            tests.RemoveTests(VerticalAlignmentProperty.TemplateBindTest);
            tests.RemoveTests(VerticalContentAlignmentProperty.TemplateBindTest);

            // Remove other DP tests for properties inherited from Control 
            // but don't exist in WPF's Viewbox?
            //   BorderThickness, BorderBrush, Background, Foreground
            //   FontFamily, FontSize, FontStretch, FontStyle, FontWeight
            //   IsTabStop, Padding, TabIndex, Template
            //   HorizontalContentAlignment, VerticalContentAlignment
            tests.RemoveTests(HorizontalContentAlignmentProperty.CheckDefaultValueTest);
            tests.RemoveTests(HorizontalContentAlignmentProperty.ClearValueResetsDefaultTest);
            tests.RemoveTests(VerticalContentAlignmentProperty.CheckDefaultValueTest);
            tests.RemoveTests(VerticalContentAlignmentProperty.ClearValueResetsDefaultTest);

            // StretchProperty tests
            tests.Add(StretchProperty.CheckDefaultValueTest);
            tests.Add(StretchProperty.ChangeClrSetterTest);
            tests.Add(StretchProperty.ChangeSetValueTest);
            tests.Add(StretchProperty.ClearValueResetsDefaultTest);
            tests.Add(StretchProperty.InvalidValueFailsTest);
            tests.Add(StretchProperty.InvalidValueIsIgnoredTest);
            tests.Add(StretchProperty.CanBeStyledTest);
            ////tests.Add(StretchProperty.TemplateBindTest);
            tests.Add(StretchProperty.DoesNotChangeVisualStateTest(Stretch.None, Stretch.Fill));
            tests.Add(StretchProperty.DoesNotChangeVisualStateTest(Stretch.Fill, Stretch.Uniform));
            tests.Add(StretchProperty.DoesNotChangeVisualStateTest(Stretch.Uniform, Stretch.UniformToFill));
            tests.Add(StretchProperty.DoesNotChangeVisualStateTest(Stretch.UniformToFill, Stretch.None));
            tests.Add(StretchProperty.SetXamlAttributeTest);
            tests.Add(StretchProperty.SetXamlElementTest);

            // StretchDirectionProperty tests
            tests.Add(StretchDirectionProperty.CheckDefaultValueTest);
            tests.Add(StretchDirectionProperty.ChangeClrSetterTest);
            tests.Add(StretchDirectionProperty.ChangeSetValueTest);
            tests.Add(StretchDirectionProperty.ClearValueResetsDefaultTest);
            tests.Add(StretchDirectionProperty.InvalidValueFailsTest);
            tests.Add(StretchDirectionProperty.InvalidValueIsIgnoredTest);
            tests.Add(StretchDirectionProperty.CanBeStyledTest);
            ////tests.Add(StretchDirectionProperty.TemplateBindTest);
            tests.Add(StretchDirectionProperty.DoesNotChangeVisualStateTest(StretchDirection.DownOnly, StretchDirection.UpOnly));
            tests.Add(StretchDirectionProperty.DoesNotChangeVisualStateTest(StretchDirection.UpOnly, StretchDirection.Both));
            tests.Add(StretchDirectionProperty.DoesNotChangeVisualStateTest(StretchDirection.Both, StretchDirection.DownOnly));
            tests.Add(StretchDirectionProperty.SetXamlAttributeTest);
            tests.Add(StretchDirectionProperty.SetXamlElementTest);

            return tests;
        }

        #region layout tests
        /// <summary>
        /// Test all layout possibilities.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Exhaustive test of all layout possibiities for Viewbox")]
        public void TestLayoutAsync()
        {
            // prepare paramters for TestBase.TestTaskAsync
            Viewbox vb = new Viewbox()
            {
                Width = 400,
                Height = 400
            };
            ScrollViewer sv = new ScrollViewer()
            {
                Background = new SolidColorBrush(Colors.Blue)
            };
            Rectangle r = new Rectangle()
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Red),
                Fill = new SolidColorBrush(Colors.Green)
            };
            List<Action> actions = new List<Action>();

            // this is ugly, because there is no Enum.GetValues() in Silverlight
            Stretch[] stretches = { Stretch.None, Stretch.Fill, Stretch.Uniform, Stretch.UniformToFill };
            StretchDirection[] stretchDirections = { StretchDirection.UpOnly, StretchDirection.DownOnly, StretchDirection.Both };

            // generate all possible test cases
            // Viewbox properties: Stretch & StretchDirection
            foreach (Stretch s in stretches)
            {
                foreach (StretchDirection sd in stretchDirections)
                {
                    // Rectangle properties: Width, Height
                    for (double width = 4; width <= 800; width *= 200)
                    {
                        for (double height = 4; height <= 800; height *= 200)
                        {
                            // capture loop variables
                            Stretch cs = s;
                            StretchDirection csd = sd;
                            double cw = width;
                            double ch = height;

                            // First, add Rectangle into Viewbox directly
                            actions.Add(() =>
                                {
                                    vb.Stretch = cs;
                                    vb.StretchDirection = csd;
                                    r.Width = cw;
                                    r.Height = ch;

                                    sv.Content = null; // free r for next line
                                    vb.Child = r;
                                });

                            actions.Add(() =>
                                {
                                    Rect original = new Rect(0, 0, cw, ch);
                                    Rect expected = CalculateExpectedLayout(vb, r);
                                    Rect actual = CalculateActualLayout(vb, r);

                                    Size sizeExpected = new Size(expected.Width, expected.Height);
                                    Size sizeActual = new Size(actual.Width, actual.Height);

                                    Assert.AreEqual(
                                        sizeExpected,
                                        sizeActual,
                                        "Viewbox({0},{1})|Rect({2},{3}): orignal [{4}], expected [{5}] actual [{6}].",
                                        vb.Stretch,
                                        vb.StretchDirection,
                                        r.HorizontalAlignment,
                                        r.VerticalAlignment,
                                        original,
                                        expected,
                                        actual);
                                });

                            // Then, add Rectangle into Viewbox via a ScrollViewer
                            actions.Add(() =>
                                {
                                    vb.Child = sv;
                                    sv.Content = r;
                                });

                            actions.Add(() =>
                                {
                                    Rect original = new Rect(0, 0, cw, ch);
                                    Rect expected = original; // ScrollViewer prevent Viewbox stretching
                                    Rect actual = new Rect(0, 0, r.ActualWidth, r.ActualHeight);

                                    Size sizeExpected = new Size(expected.Width, expected.Height);
                                    Size sizeActual = new Size(actual.Width, actual.Height);

                                    Assert.AreEqual(
                                        sizeExpected,
                                        sizeActual,
                                        "Viewbox({0},{1})|ScrollViewer|Rect({2},{3}): orignal [{4}], expected [{5}] actual [{6}].",
                                        vb.Stretch,
                                        vb.StretchDirection,
                                        r.HorizontalAlignment,
                                        r.VerticalAlignment,
                                        original,
                                        expected,
                                        actual);
                                });
                        }
                    }
                }
            }

            TestAsync(5, vb, actions.ToArray());
        }

        /// <summary>
        /// Calculate expected layout Rect of Rectangle with Viewbox.
        /// </summary>
        /// <param name="vb">Containing Viewbox.</param>
        /// <param name="r">Contained Rectangle.</param>
        /// <returns>Expected layout Rect of Rectangle relative to its containing Viewbox.</returns>
        private static Rect CalculateExpectedLayout(Viewbox vb, Rectangle r)
        {
            Rect viewbox = new Rect(0, 0, vb.ActualWidth, vb.ActualHeight);
            Rect original = new Rect(0, 0, r.ActualWidth, r.ActualHeight);
            Rect expected = original;

            switch (vb.Stretch)
            {
                case Stretch.None:
                    expected = original;
                    break;
                case Stretch.Fill:
                    expected = viewbox;
                    break;
                case Stretch.Uniform:
                case Stretch.UniformToFill:
                    {
                        double scaleX = viewbox.Width / original.Width;
                        double scaleY = viewbox.Height / original.Height;
                        double scale = vb.Stretch == Stretch.Uniform ?
                            Math.Min(scaleX, scaleY) : Math.Max(scaleX, scaleY);

                        expected.Width *= scale;
                        expected.Height *= scale;
                    }
                    break;
            }

            switch (vb.StretchDirection)
            {
                case StretchDirection.Both:
                    break;
                case StretchDirection.UpOnly:
                    if (expected.Width < original.Width || expected.Height < original.Height)
                    {
                        expected = original;
                    }
                    break;
                case StretchDirection.DownOnly:
                    if (expected.Width > original.Width || expected.Height > original.Height)
                    {
                        expected = original;
                    }
                    break;
            }

            return expected;
        }

        /// <summary>
        /// Calculate actual layout Rect of Rectangle relative to Viewbox.
        /// </summary>
        /// <param name="vb">Containing Viewbox.</param>
        /// <param name="r">Contained Rectangle.</param>
        /// <returns>Actual layout Rect of contained Rectangle relative to containing Viewbox.</returns>
        private static Rect CalculateActualLayout(Viewbox vb, Rectangle r)
        {
            Assert.AreEqual(VisualTreeHelper.GetChildrenCount(vb), 1, "Viewbox template changed!");
            Assert.IsInstanceOfType(VisualTreeHelper.GetChild(vb, 0), typeof(ContentPresenter), "Viewbox template changed!");

            ContentPresenter container = VisualTreeHelper.GetChild(vb, 0) as ContentPresenter;
            ScaleTransform scale = container.RenderTransform as ScaleTransform;
            Assert.IsNotNull(scale, "Viewbox implementation changed!");

            Point offset = r.TransformToVisual(vb).Transform(new Point(0, 0));
            offset = scale.Transform(offset);
            Rect actual = new Rect(offset.X, offset.Y, r.ActualWidth * scale.ScaleX, r.ActualHeight * scale.ScaleY);

            return actual;
        }
        #endregion 
    }
}