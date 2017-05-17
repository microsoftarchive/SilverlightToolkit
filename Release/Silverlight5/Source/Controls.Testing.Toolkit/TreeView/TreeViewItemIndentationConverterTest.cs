// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the ItemsControlExtensions.
    /// </summary>
    [TestClass]
    [Tag("TreeViewItemIndentationConverter")]
    [Tag("TreeView")]
    [Tag("TreeViewItem")]
    [Tag("TreeViewExtensions")]
    public partial class TreeViewItemIndentationConverterTest : TestBase
    {
        /// <summary>
        /// The default culture information to use in the tests.
        /// </summary>
        private static readonly CultureInfo DefaultCulture = new CultureInfo("en-US");

        /// <summary>
        /// Initializes a new instance of the
        /// TreeViewItemIndentationConverterTest class.
        /// </summary>
        public TreeViewItemIndentationConverterTest()
        {
        }

        /// <summary>
        /// Create a new TreeViewItemIndentationConverter converter.
        /// </summary>
        [TestMethod]
        [Description("Create a new TreeViewItemIndentationConverter converter.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Controls.TreeViewItemIndentationConverter", Justification = "Unit test")]
        public virtual void Create()
        {
            new TreeViewItemIndentationConverter();
        }

        /// <summary>
        /// Verify that ConvertBack throws.
        /// </summary>
        [TestMethod]
        [Description("Verify that ConvertBack throws.")]
        [ExpectedException(typeof(NotSupportedException))]
        public virtual void CannotConvertBack()
        {
            TreeViewItemIndentationConverter converter = new TreeViewItemIndentationConverter();
            converter.ConvertBack(null, null, null, null);
        }

        /// <summary>
        /// Verify that the converter returns zero if not given a TreeViewItem.
        /// </summary>
        [TestMethod]
        [Description("Verify that the converter returns zero if not given a TreeViewItem.")]
        public virtual void ZeroIfNoTreeViewItem()
        {
            TreeViewItemIndentationConverter converter = new TreeViewItemIndentationConverter();
            Assert.AreEqual(0.0, converter.Convert(null, typeof(double), null, null));
        }

        /// <summary>
        /// Verify the converter returns the result in the desired type.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the converter returns the result in the desired type.")]
        public virtual void WrapInTheDesiredType()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder()
                .Items("first").Expand()
                    .Items("second").Expand()
                        .Item("third").Named(out item)
                    .EndItems()
                .EndItems();

            TreeViewItemIndentationConverter converter = new TreeViewItemIndentationConverter();
            TestAsync(
                view,
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(double), "15", DefaultCulture), typeof(double)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(Thickness), "15", DefaultCulture), typeof(Thickness)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(Point), "15", DefaultCulture), typeof(Point)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(Rect), "15", DefaultCulture), typeof(Rect)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(Size), "15", DefaultCulture), typeof(Size)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(GridLength), "15", DefaultCulture), typeof(GridLength)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(int), "15", DefaultCulture), typeof(double)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(float), "15", DefaultCulture), typeof(double)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(decimal), "15", DefaultCulture), typeof(double)),
                () => Assert.IsInstanceOfType(converter.Convert(item, typeof(string), "15", DefaultCulture), typeof(double)),
                () => Assert.IsInstanceOfType(converter.Convert(item, null, "15", DefaultCulture), typeof(double)));
        }

        /// <summary>
        /// Place the indent on the left when wrapping.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Place the indent on the left when wrapping.")]
        public virtual void WrapIndentOnTheLeft()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder()
                .Items("first").Expand()
                    .Items("second").Expand()
                        .Item("third").Named(out item)
                    .EndItems()
                .EndItems();

            TreeViewItemIndentationConverter converter = new TreeViewItemIndentationConverter();
            TestAsync(
                view,
                () =>
                {
                    Thickness value = (Thickness)converter.Convert(item, typeof(Thickness), "15", DefaultCulture);
                    Assert.AreNotEqual(0.0, value.Left, "Thickness.Left should be non-zero!");
                    Assert.AreEqual(0.0, value.Right, "Thickness.Right should be zero!");
                    Assert.AreEqual(0.0, value.Top, "Thickness.Top should be zero!");
                    Assert.AreEqual(0.0, value.Bottom, "Thickness.Bottom should be zero!");
                },
                () =>
                {
                    Point value = (Point)converter.Convert(item, typeof(Point), "15", DefaultCulture);
                    Assert.AreNotEqual(0.0, value.X, "Point.X should be non-zero!");
                    Assert.AreEqual(0.0, value.Y, "Point.Y should be zero!");
                },
                () =>
                {
                    Rect value = (Rect)converter.Convert(item, typeof(Rect), "15", DefaultCulture);
                    Assert.AreNotEqual(0.0, value.Left, "Rect.Left should be non-zero!");
                    Assert.AreEqual(0.0, value.Width, "Rect.Width should be zero!");
                    Assert.AreEqual(0.0, value.Top, "Rect.Top should be zero!");
                    Assert.AreEqual(0.0, value.Height, "Rect.Height should be zero!");
                },
                () =>
                {
                    Size value = (Size)converter.Convert(item, typeof(Size), "15", DefaultCulture);
                    Assert.AreNotEqual(0.0, value.Width, "Size.Width should be non-zero!");
                    Assert.AreEqual(0.0, value.Height, "Size.Height should be zero!");
                },
                () =>
                {
                    GridLength value = (GridLength)converter.Convert(item, typeof(GridLength), "15", DefaultCulture);
                    Assert.AreNotEqual(0.0, value.Value, "GridLength.Value should be non-zero!");
                    Assert.IsTrue(value.IsAbsolute, "GridLength should be absolute!");
                });
        }

        /// <summary>
        /// Verify basic indentation.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify basic indentation.")]
        [Priority(0)]
        public virtual void Indentation()
        {
            TreeViewItem first, second, third, fourth;
            TreeView view = new TreeViewBuilder()
                .Items("first").Expand().Named(out first)
                    .Items("second").Expand().Named(out second)
                        .Item("third").Named(out third)
                    .EndItems()
                .EndItems()
                .Item("fourth").Named(out fourth);

            TreeViewItemIndentationConverter converter = new TreeViewItemIndentationConverter();
            TestAsync(
                view,
                () => Assert.AreEqual(0.0, converter.Convert(first, typeof(double), "15", DefaultCulture), "first should not be indented!"),
                () => Assert.AreEqual(15.0, converter.Convert(second, typeof(double), "15", DefaultCulture), "second should be indented once!"),
                () => Assert.AreEqual(30.0, converter.Convert(third, typeof(double), "15", DefaultCulture), "third should be indented twice!"),
                () => Assert.AreEqual(0.0, converter.Convert(fourth, typeof(double), "15", DefaultCulture), "fourth should not be indented!"));
        }

        /// <summary>
        /// Verify the converter parameter is used to define the indent.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the converter parameter is used to define the indent.")]
        public virtual void ConverterParameterDefinesIndent()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder()
                .Items("first").Expand()
                    .Items("second").Expand()
                        .Item("third").Named(out item)
                    .EndItems()
                .EndItems()
                .Item("fourth");

            TreeViewItemIndentationConverter converter = new TreeViewItemIndentationConverter();
            TestAsync(
                view,
                () => Assert.AreEqual(30.0, converter.Convert(item, typeof(double), "15", DefaultCulture), "15"),
                () => Assert.AreEqual(30.0, converter.Convert(item, typeof(double), "15.0", DefaultCulture), "15.0"),
                () => Assert.AreEqual(30.0, converter.Convert(item, typeof(double), null, DefaultCulture), "(null)"),
                () => Assert.AreEqual(2.0, converter.Convert(item, typeof(double), "1", DefaultCulture), "1"),
                () => Assert.AreEqual(10.0, converter.Convert(item, typeof(double), "5", DefaultCulture), "5"));
        }

        /// <summary>
        /// Verify that invalid converter parameters result in the default
        /// value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that invalid converter parameters result in the default value.")]
        public virtual void InvalidConverterParameters()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder()
                .Items("first").Expand()
                    .Items("second").Expand()
                        .Item("third").Named(out item)
                    .EndItems()
                .EndItems()
                .Item("fourth");

            TreeViewItemIndentationConverter converter = new TreeViewItemIndentationConverter();
            TestAsync(
                view,
                () => Assert.AreEqual(30.0, converter.Convert(item, typeof(double), null, DefaultCulture), "(null)"),
                () => Assert.AreEqual(30.0, converter.Convert(item, typeof(double), string.Empty, DefaultCulture), "(empty)"),
                () => Assert.AreEqual(30.0, converter.Convert(item, typeof(double), 10, DefaultCulture), "10"),
                () => Assert.AreEqual(30.0, converter.Convert(item, typeof(double), 10.0, DefaultCulture), "10.0"),
                () => Assert.AreEqual(0.0, converter.Convert(item, typeof(double), "foo", DefaultCulture), "foo"));
        }

        /// <summary>
        /// Verify the CultureInfo is used to parse the converter parameter.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the CultureInfo is used to parse the converter parameter.")]
        public virtual void CultureInfoIsRespected()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder()
                .Items("first").Expand()
                    .Items("second").Expand()
                        .Item("third").Named(out item)
                    .EndItems()
                .EndItems()
                .Item("fourth");

            TreeViewItemIndentationConverter converter = new TreeViewItemIndentationConverter();
            TestAsync(
                view,
                () => Assert.AreEqual(30.0, converter.Convert(item, typeof(double), "15.0", new CultureInfo("en-US")), "en-US"),
                () => Assert.AreEqual(10.0, converter.Convert(item, typeof(double), "5,0", new CultureInfo("vi-VN")), "vi-VN"));
        }
    }
}