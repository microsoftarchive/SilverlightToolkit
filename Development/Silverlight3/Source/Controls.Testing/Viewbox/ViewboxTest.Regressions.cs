// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Bug regression tests for the System.Windows.Controls.ViewBox class.
    /// </summary>
    public partial class ViewboxTest
    {
        /// <summary>
        /// Tests that if an image that is placed within a viewbox can be scaled and displayed correctly.
        /// </summary>
        [TestMethod]
        [Description("Tests that if an image that is placed within a viewbox can be scaled and displayed properly.")]
        [Asynchronous]
        [Bug("525126 - ViewBox - Image cannot be displayed within a ViewBox", Fixed = true)]
        public void TestImageInViewBox()
        {
            Viewbox vb = new Viewbox();
            vb.Height = 400;
            vb.Width = 400;
            vb.Stretch = Stretch.Fill;
            Image image = CreateImage(@"System.Windows.Controls.Testing.", "Viewbox.Bamboo.jpg");
            vb.Child = image;
            ContentPresenter cp = new ContentPresenter();
            ScaleTransform st = new ScaleTransform();
            TestAsync(
               vb,
               () => cp = (ContentPresenter)VisualTreeHelper.GetParent(image),
               () => st = cp.RenderTransform as ScaleTransform,
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualHeight, image.ActualHeight * st.ScaleY)),
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualWidth, image.ActualWidth * st.ScaleX)));
        }

        /// <summary>
        /// Tests that if a rectangle that is placed within a viewbox can be scaled and displayed correctly.
        /// </summary>
        [TestMethod]
        [Description("Tests that if a rectangle that is placed within a viewbox can be scaled and displayed properly.")]
        [Asynchronous]
        [Bug("525308 - ViewBox - Rectangle and Ellipse cannot be displayed within ViewBox", Fixed = true)]
        public void TestRectangleInViewBox()
        {
            Viewbox vb = new Viewbox();
            vb.Height = 300;
            vb.Width = 300;
            vb.Stretch = Stretch.Fill;
            Rectangle rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Colors.Yellow);
            rect.Height = 120;
            rect.Width = 120;
            vb.Child = rect;
            ContentPresenter cp = new ContentPresenter();
            ScaleTransform st = new ScaleTransform();
            TestAsync(
               vb,
               () => cp = (ContentPresenter)VisualTreeHelper.GetParent(rect),
               () => st = cp.RenderTransform as ScaleTransform,
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualHeight, rect.ActualHeight * st.ScaleY)),
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualWidth, rect.ActualWidth * st.ScaleX)));
        }

        /// <summary>
        /// Tests that if an ellipse that is placed within a viewbox can be scaled and displayed correctly.
        /// </summary>
        [TestMethod]
        [Description("Tests that if an ellipse that is placed within a viewbox can be scaled and displayed properly.")]
        [Asynchronous]
        [Bug("525308 - ViewBox - Rectangle and Ellipse cannot be displayed within ViewBox", Fixed = true)]
        public void TestEllipseInViewBox()
        {
            Viewbox vb = new Viewbox();
            vb.Height = 300;
            vb.Width = 300;
            vb.Stretch = Stretch.Fill;
            Ellipse eip = new Ellipse();
            eip.Fill = new SolidColorBrush(Colors.Purple);
            eip.Height = 150;
            eip.Width = 150;
            vb.Child = eip;
            ContentPresenter cp = new ContentPresenter();
            ScaleTransform st = new ScaleTransform();
            TestAsync(
               vb,
               () => cp = (ContentPresenter)VisualTreeHelper.GetParent(eip),
               () => st = cp.RenderTransform as ScaleTransform,
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualHeight, eip.ActualHeight * st.ScaleY)),
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualWidth, eip.ActualWidth * st.ScaleX)));
        }

        /// <summary>
        /// Tests that when a checkbox is placed in a viewbox with Stretch = Uniform, if the checkbox is positioned in the middle.
        /// </summary>
        [TestMethod]
        [Description("Tests that when a checkbox is placed in a viewbox with Stretch = Uniform, if the checkbox is positioned in the middle.")]
        [Asynchronous]
        [Bug("525203 - ViewBox - Incompatbility with WPF when setting Stretch property to Uniform", Fixed = true)]
        public void TestViewBoxUniformStretch()
        {
            Viewbox vb = new Viewbox();
            vb.Height = 300;
            vb.Width = 300;
            vb.Stretch = Stretch.Uniform;
            CheckBox cb = new CheckBox();
            cb.Content = "Check box";
            vb.Child = cb;
            Point pt = new Point();
            Grid cp = new Grid();
            TestAsync(
               vb,
               () => cp = (Grid)VisualTreeHelper.GetParent(vb),
               () => pt = vb.TransformToVisual(cp).Transform(new Point(0, 0)),
               () => Assert.IsTrue(TestExtensions.AreClose((cp.ActualHeight - vb.ActualHeight) / 2, pt.Y)));
        }

// TODO: Integrate the Silverlight 2 DataGrid scenario to Dev3
#if SILVERLIGHT2_DATAGRID_AVAILABLE
        /// <summary>
        /// Tests that when a datagrid is placed inside a viewbox, if the datagrid is displayed properly.
        /// </summary>
        [TestMethod]
        [Description("Tests that when a datagrid is placed inside a viewbox, if the datagrid is displayed properly.")]
        [Asynchronous]
        [Bug("525988 - ViewBox - Datagrid does not work inside ViewBox", Fixed = true)]
        public void TestDataGridInViewBox()
        {
            Viewbox vb = new Viewbox();
            vb.Height = 300;
            vb.Width = 300;
            vb.Stretch = Stretch.UniformToFill;
            DataGrid dg = new DataGrid();
            dg.ItemsSource = DataForDataGrid.GenerateRecordList(30);
            dg.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            vb.Child = dg;
            ContentPresenter cp = new ContentPresenter();
            ScaleTransform st = new ScaleTransform();
            TestAsync(
               vb,
               () => cp = (ContentPresenter)VisualTreeHelper.GetParent(dg),
               () => st = cp.RenderTransform as ScaleTransform,
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualHeight, dg.ActualHeight * st.ScaleY)),
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualWidth, dg.ActualWidth * st.ScaleX)));
        }
#endif

        /// <summary>
        /// Tests that if a TextBlock that is placed within a Viewbox, it can be
        /// scaled and displayed correctly.
        /// </summary>
        [TestMethod]
        [Description("Tests that if a TextBlock that is placed within a Viewbox, it can be scaled and displayed properly.")]
        [Asynchronous]
        [Bug("529641 - ViewBox - The content made of a TextBlock will not be rendered", Fixed = true)]
        public void TestTextBlockInViewBox()
        {
            Viewbox vb = new Viewbox { Height = 300, Width = 300, Stretch = Stretch.Fill };
            TextBlock text = new TextBlock { Text = "Test" };
            vb.Child = text;

            ContentPresenter cp = new ContentPresenter();
            ScaleTransform st = new ScaleTransform();

            TestAsync(
               vb,
               () => cp = (ContentPresenter) VisualTreeHelper.GetParent(text),
               () => st = cp.RenderTransform as ScaleTransform,
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualHeight, text.ActualHeight * st.ScaleY)),
               () => Assert.IsTrue(TestExtensions.AreClose(vb.ActualWidth, text.ActualWidth * st.ScaleX)));
        }

        /// <summary>
        /// Verify FindName works on the Child of a Viewbox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify FindName works on the Child of a Viewbox.")]
        [Bug("555647 - Viewbox - Viewbox named child does not populate on InitializeComponent", Fixed = true)]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Viewbox", Justification = "Correct spelling.")]
        public virtual void FindNameWorksOnViewboxChild()
        {
            XamlBuilder xaml = new XamlBuilder<Viewbox>()
            {
                ExplicitNamespaces = new Dictionary<string, string> { { "controls", XamlBuilder.GetNamespace(typeof(Viewbox)) } },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<TextBlock>
                    {
                        Name = "Foo",
                        AttributeProperties = new Dictionary<string, string> { { "Text", "Test" } }
                    }
                }
            };

            Viewbox view = xaml.Load() as Viewbox;
            TextBlock text = null;
            TestAsync(
                view,
                () => text = view.FindName("Foo") as TextBlock,
                () => Assert.IsNotNull(text, "Failed to find named Child!"));
        }

        #region Helper Methods
        /// <summary>
        /// A helper method to load the embedded image.
        /// </summary>
        /// <param name="prefix">The name of the assembly.</param>
        /// <param name="name">The name of the image in a specific folder.</param>
        /// <returns>Return image.</returns>
        public Image CreateImage(string prefix, string name)
        {
            Image image = new Image { Tag = name };
            Assembly assembly = this.GetType().Assembly;
            string resourceName = prefix + name;
            using (Stream resource = assembly.GetManifestResourceStream(resourceName))
            {
                if (resource != null)
                {
                    BitmapImage source = new BitmapImage();
                    source.SetSource(resource);
                    image.Source = source;
                }
            }
            return image;
        }
        #endregion Helper Methods
    }
}