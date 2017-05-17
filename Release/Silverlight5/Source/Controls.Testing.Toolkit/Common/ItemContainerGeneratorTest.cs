// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// ItemContainerGenerator unit tests.
    /// </summary>
    [TestClass]
    public partial class ItemContainerGeneratorTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the ItemContainerGeneratorTest class.
        /// </summary>
        public ItemContainerGeneratorTest()
            : base()
        {
        }

        /// <summary>
        /// Verify the items aren't created outside of the visual tree.
        /// </summary>
        [TestMethod]
        [Description("Verify the items aren't created outside of the visual tree.")]
        public virtual void NoItemsOutsideOfTree()
        {
            int[] source = new int[] { 1, 2, 3 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            for (int i = 0; i < items.Items.Count; i++)
            {
                Assert.AreEqual(source[i], items.Items[i], "Item {0} has unexpected value", i);
                Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(i), "Container {0} should be null!", i);
            }
        }

        /// <summary>
        /// Verify the items are created once it's in the visual tree.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the items are created once it's in the visual tree.")]
        public virtual void ItemsCreatedInTree()
        {
            int[] source = new int[] { 1, 2, 3 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            TestAsync(
                items,
                () =>
                {
                    for (int i = 0; i < items.Items.Count; i++)
                    {
                        Assert.AreEqual(source[i], items.Items[i], "Item {0} has unexpected value", i);
                        Assert.IsNotNull(items.ItemContainerGenerator.ContainerFromIndex(i), "Container {0} should not be null!", i);
                    }
                });
        }

        #region Container Mapping
        /// <summary>
        /// Get a container from its index.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get a container from its index.")]
        [Priority(0)]
        public virtual void GetContainerFromIndex()
        {
            int[] source = new int[] { 1, 2, 3 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            TestAsync(
                items,
                () =>
                {
                    for (int i = 0; i < items.Items.Count; i++)
                    {
                        DependencyObject container = items.ItemContainerGenerator.ContainerFromIndex(i);
                        Assert.IsNotNull(container, "Container {0} should not be null!", i);

                        ContentPresenter presenter = container as ContentPresenter;
                        Assert.IsNotNull(presenter, "Presenter {0} should not be null!", i);
                        Assert.AreEqual(items.Items[i], presenter.Content, "Item {0} has unexpected value", i);
                    }
                });
        }

        /// <summary>
        /// Try to get a container for an index out of range.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get a container for an index out of range.")]
        public virtual void GetContainerFromIndexOutOfRange()
        {
            int[] source = new int[] { 1, 2, 3 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            TestAsync(
                items,
                // () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(-1), "Index -1 should return null!"), WPF throws IndexOutOfRange
                () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(3), "Index 3 should return null!"),
                () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(4), "Index 3 should return null!"),
                // () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(int.MinValue), "Index int.MinValue should return null!"), WPF throws IndexOutOfRange
                () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(int.MaxValue), "Index int.MaxValue should return null!"));
        }

        /// <summary>
        /// Try to get a container when there is no ItemsHost.
        /// </summary>
        [TestMethod]
        [Description("Try to get a container when there is no ItemsHost.")]
        public virtual void GetContainerFromIndexNoItemsHost()
        {
            int[] source = new int[] { 1, 2, 3 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(0), "Index 0 should return null!");
            Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(1), "Index 1 should return null!");
            Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(2), "Index 2 should return null!");
        }

        /// <summary>
        /// Try to get a container when the Items have been cleared.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get a container when the Items have been cleared.")]
        public virtual void GetContainerFromIndexNoItems()
        {
            int[] source = new int[] { 1, 2, 3 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            TestAsync(
                items,
                () => Assert.IsNotNull(items.ItemContainerGenerator.ContainerFromIndex(0), "Index 0 should not return null!"),
                () => items.ItemsSource = null,
                () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromIndex(0), "Index 0 should return null after clearing!"));
        }

        /// <summary>
        /// Try to get the container from an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the container from an item.")]
        [Priority(0)]
        public virtual void GetContainerFromItem()
        {
            int[] source = new int[] { 1, 2, 3 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            TestAsync(
                items,
                () =>
                {
                    for (int i = 0; i < items.Items.Count; i++)
                    {
                        DependencyObject container = items.ItemContainerGenerator.ContainerFromItem(source[i]);
                        Assert.IsNotNull(container, "Container {0} should not be null!", i);

                        ContentPresenter presenter = container as ContentPresenter;
                        Assert.IsNotNull(presenter, "Presenter {0} should not be null!", i);
                        Assert.AreEqual(source[i], presenter.Content, "Item {0} has unexpected value", i);
                    }
                });
        }

        /// <summary>
        /// Try to get the container from an item using reference types.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the container from an item using reference types.")]
        public virtual void GetContainerFromItemWithReferenceTypes()
        {
            string[] source = new string[] { "Hello", "World" };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            TestAsync(
                items,
                () =>
                {
                    for (int i = 0; i < items.Items.Count; i++)
                    {
                        DependencyObject container = items.ItemContainerGenerator.ContainerFromItem(source[i]);
                        Assert.IsNotNull(container, "Container {0} should not be null!", i);

                        ContentPresenter presenter = container as ContentPresenter;
                        Assert.IsNotNull(presenter, "Presenter {0} should not be null!", i);
                        Assert.AreEqual(source[i], presenter.Content, "Item {0} has unexpected value", i);
                    }
                });
        }

        /// <summary>
        /// Try to get the container from an item not in the collection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the container from an item not in the collection.")]
        public virtual void GetContainerFromItemNotInCollection()
        {
            int[] source = new int[] { 1, 2, 3 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            TestAsync(
                items,
                () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromItem(4), "Found no container for 4!"),
                () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromItem(new object()), "Found no container for new object!"),
                () => Assert.IsNull(items.ItemContainerGenerator.ContainerFromItem(new object()), "Found no container for null!"));
        }

        /// <summary>
        /// Try to get a container for items with the same value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get a container for items with the same value.")]
        public virtual void GetContainerFromItemWithSameValue()
        {
            int[] source = new int[] { 1, 1, 1, 1, 1, 1, 1 };
            HeaderedItemsControl items = new HeaderedItemsControl { ItemsSource = source };
            TestAsync(
                items,
                () =>
                {
                    // Get the first item
                    DependencyObject first = items.ItemContainerGenerator.ContainerFromIndex(0);

                    for (int i = 0; i < items.Items.Count; i++)
                    {
                        DependencyObject container = items.ItemContainerGenerator.ContainerFromItem(source[i]);
                        Assert.AreEqual(first, container, "Container for item {0} should be the first in the list!", i);
                    }
                });
        }

        /// <summary>
        /// Try to get the index of a container.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the index of a container.")]
        [Priority(0)]
        public virtual void GetIndexFromContainer()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            TestAsync(
                view,
                () => Assert.AreEqual(0, view.ItemContainerGenerator.IndexFromContainer(first), "First should be at position 0!"),
                () => Assert.AreEqual(1, view.ItemContainerGenerator.IndexFromContainer(second), "Second should be at position 1!"));
        }

        /// <summary>
        /// Try to get the index of a null container.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Try to get the index of a null container.")]
        public virtual void GetIndexFromContainerNull()
        {
            TreeView view = new TreeView { ItemsSource = new[] { 1, 2, 3 } };
            view.ItemContainerGenerator.IndexFromContainer(null);
        }

        /// <summary>
        /// Try to get the index of a non-UIElement container.
        /// </summary>
        [TestMethod]
        [Description("Try to get the index of a non-UIElement container.")]
        public virtual void GetIndexFromContainerNonUIElement()
        {
            TreeView view = new TreeView { ItemsSource = new[] { 1, 2, 3 } };
            Assert.AreEqual(-1, view.ItemContainerGenerator.IndexFromContainer(new SolidColorBrush(Colors.Black)), "Expected to not find a non-UIElement DependencyObject!");
        }

        /// <summary>
        /// Try to get the index of a container not in the tree yet.
        /// </summary>
        [TestMethod]
        [Description("Try to get the index of a container not in the tree yet.")]
        public virtual void GetIndexFromContainerNotInTree()
        {
            TreeView view = new TreeView { ItemsSource = new[] { 1, 2, 3 } };
            TreeViewItem first = new TreeViewItem { Header = "First" };
            Assert.AreEqual(-1, view.ItemContainerGenerator.IndexFromContainer(first), "Expected not to find index of container!");
        }

        /// <summary>
        /// Try to get the item for a container.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the item for a container.")]
        [Priority(0)]
        [Bug("33637: ItemContainerGenerator.ItemFromContainer does not correctly return items", Fixed = true)]
        public virtual void GetItemFromContainer()
        {
            int[] source = new int[] { 1, 2, 3 };
            TreeView view = new TreeView { ItemsSource = source };

            TestAsync(
                view,
                () =>
                {
                    for (int i = 0; i < view.Items.Count; i++)
                    {
                        TreeViewItem item = view.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                        Assert.IsNotNull(item, "TreeViewItem {0} should not be null!", i);

                        Assert.AreEqual(source[i], view.ItemContainerGenerator.ItemFromContainer(item), "Item {0} has unexpected value", i);
                    }
                });
        }

        /// <summary>
        /// Try to get the item from a null container.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Try to get the item from a null container.")]
        public virtual void GetItemFromContainerNull()
        {
            TreeView view = new TreeView { ItemsSource = new[] { 1, 2, 3 } };
            view.ItemContainerGenerator.ItemFromContainer(null);
        }

        /// <summary>
        /// Try to get the item for a container not in the Items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the item for a container not in the Items.")]
        public virtual void GetItemFromContainerNotInItems()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);

            TestAsync(
                view,
                () => Assert.AreEqual(first, view.ItemContainerGenerator.ItemFromContainer(first), "First not found!"),
                () => Assert.AreEqual(DependencyProperty.UnsetValue, view.ItemContainerGenerator.ItemFromContainer(second), "Second should not be found!"));
        }
        #endregion Container Mapping

        #region ScrollIntoView
        /// <summary>
        /// Scroll an item into view.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Scroll an item into view.")]
        [Priority(0)]
        public virtual void ScrollIntoView()
        {
            TreeView view = new TreeView { Width = 20, Height = 20 };
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = 2 };
            view.Items.Add(first);
            view.Items.Add(second);

            TreeViewAutomationPeer viewPeer = null;
            TreeViewItemAutomationPeer firstPeer = null;
            IScrollItemProvider firstProvider = null;
            TreeViewItemAutomationPeer secondPeer = null;
            IScrollItemProvider secondProvider = null;

            TestAsync(
                view,
                () => viewPeer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => firstPeer = FrameworkElementAutomationPeer.CreatePeerForElement(first) as TreeViewItemAutomationPeer,
                () => secondPeer = FrameworkElementAutomationPeer.CreatePeerForElement(second) as TreeViewItemAutomationPeer,
                () => firstProvider = firstPeer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => secondProvider = secondPeer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => secondProvider.ScrollIntoView());
        }

        /// <summary>
        /// Scroll an item into view both horizontally and vertically.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Scroll an item into view both horizontally and vertically.")]
        public virtual void ScrollItemIntoViewBoth()
        {
            TreeView view = new TreeView { Width = 20, Height = 20 };
            TreeViewItem first = new TreeViewItem { Header = "First", IsExpanded = true };
            TreeViewItem second = new TreeViewItem { Header = 2 };
            view.Items.Add(first);
            first.Items.Add(second);

            TreeViewAutomationPeer viewPeer = null;
            TreeViewItemAutomationPeer firstPeer = null;
            IScrollItemProvider firstProvider = null;
            TreeViewItemAutomationPeer secondPeer = null;
            IScrollItemProvider secondProvider = null;

            TestAsync(
                view,
                () => viewPeer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => firstPeer = FrameworkElementAutomationPeer.CreatePeerForElement(first) as TreeViewItemAutomationPeer,
                () => secondPeer = FrameworkElementAutomationPeer.CreatePeerForElement(second) as TreeViewItemAutomationPeer,
                () => firstProvider = firstPeer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => secondProvider = secondPeer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => secondProvider.ScrollIntoView(),
                () => firstProvider.ScrollIntoView(),
                () => secondProvider.ScrollIntoView(),
                () => firstProvider.ScrollIntoView());
        }

        /// <summary>
        /// Scroll an item into view that is already in view.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Scroll an item into view that is already in view.")]
        public virtual void ScrollItemIntoViewAlreadyInView()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = 2 };
            view.Items.Add(first);
            view.Items.Add(second);

            TreeViewAutomationPeer viewPeer = null;
            TreeViewItemAutomationPeer firstPeer = null;
            IScrollItemProvider firstProvider = null;
            TreeViewItemAutomationPeer secondPeer = null;
            IScrollItemProvider secondProvider = null;

            TestAsync(
                view,
                () => viewPeer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => firstPeer = FrameworkElementAutomationPeer.CreatePeerForElement(first) as TreeViewItemAutomationPeer,
                () => secondPeer = FrameworkElementAutomationPeer.CreatePeerForElement(second) as TreeViewItemAutomationPeer,
                () => firstProvider = firstPeer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => secondProvider = secondPeer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => firstProvider.ScrollIntoView());
        }

        /// <summary>
        /// Scroll an item into view when there's no ScrollHost.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Scroll an item into view when there's no ScrollHost.")]
        public virtual void ScrollIntoViewNoScrollHost()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = 2 };
            view.Items.Add(first);
            view.Items.Add(second);

            TreeViewAutomationPeer viewPeer = null;
            TreeViewItemAutomationPeer firstPeer = null;
            IScrollItemProvider firstProvider = null;
            TreeViewItemAutomationPeer secondPeer = null;
            IScrollItemProvider secondProvider = null;

            // Create a template without a ScrollViewer
            XamlBuilder<ControlTemplate> builder = new XamlBuilder<ControlTemplate>
            {
                ExplicitNamespaces = new Dictionary<string, string> { { "ctls", XamlBuilder.GetNamespace(typeof(TreeView)) } },
                AttributeProperties = new Dictionary<string, string> { { "TargetType", "ctls:TreeView" } },
                Children = new List<XamlBuilder> { new XamlBuilder<ItemsPresenter>() }
            };
            view.Template = builder.Load();

            TestAsync(
                view,
                () => viewPeer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => firstPeer = FrameworkElementAutomationPeer.CreatePeerForElement(first) as TreeViewItemAutomationPeer,
                () => secondPeer = FrameworkElementAutomationPeer.CreatePeerForElement(second) as TreeViewItemAutomationPeer,
                () => firstProvider = firstPeer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => secondProvider = secondPeer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => firstProvider.ScrollIntoView());
        }
        #endregion

        #region Prepare
        /// <summary>
        /// Verify that ItemTemplates are applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ItemTemplates are applied.")]
        [Priority(0)]
        public virtual void ApplyItemTemplate()
        {
            XamlBuilder<Grid> builder = new XamlBuilder<Grid>
            {
                ExplicitNamespaces = new Dictionary<string, string>
                {
                    { "toolkit", XamlBuilder.GetNamespace(typeof(ObjectCollection)) },
                    { "ctl", XamlBuilder.GetNamespace(typeof(System.Windows.Controls.TreeView)) },
                    { "sys", XamlBuilder.GetNamespace(typeof(int)) },
                },
                ElementProperties = new Dictionary<string, XamlBuilder>
                {
                    {
                        "Resources",
                        new XamlBuilder<DataTemplate>
                        {
                            Key = "template",
                            Children = new List<XamlBuilder>
                            {
                                new XamlBuilder<ContentControl>
                                {
                                    Name = "TemplateContent",
                                    AttributeProperties = new Dictionary<string, string>
                                    {
                                        { "Content", "{Binding}" },
                                        { "Foreground", "Red" }
                                    }
                                }
                            }
                        }
                    }
                },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<TreeView>
                    {
                        AttributeProperties = new Dictionary<string, string> { { "ItemTemplate", "{StaticResource template}" } },
                        ElementProperties = new Dictionary<string, XamlBuilder>
                        {
                            {
                                "ItemsSource",
                                new XamlBuilder<ObjectCollection>
                                {
                                    Children = new List<XamlBuilder>
                                    {
                                        new XamlBuilder<int> { Content = "1" },
                                        new XamlBuilder<int> { Content = "2" },
                                        new XamlBuilder<int> { Content = "3" }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Grid root = builder.Load();
            TreeView view = null;
            TreeViewItem first = null;
            ContentControl header = null;

            TestAsync(
                5,
                root,
                () => view = root.Children[0] as TreeView,
                () => first = view.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => Assert.IsNotNull(first, "First item should not be null!"),
                () => Assert.AreEqual(1, first.Header, "Unexpected header!"),
                () => header = first.GetVisualChild("TemplateContent") as ContentControl,
                () => Assert.IsNotNull(header, "Header should not be null!"));
        }

        /// <summary>
        /// Verify that HierarchicalDataTemplates are applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that HierarchicalDataTemplates are applied.")]
        [Priority(0)]
        public virtual void ApplyHierarchicalDataTemplate()
        {
            XamlBuilder<Grid> builder = new XamlBuilder<Grid>
            {
                ExplicitNamespaces = new Dictionary<string, string>
                {
                    { "common", XamlBuilder.GetNamespace(typeof(HierarchicalDataTemplate)) },
                    { "ctl", XamlBuilder.GetNamespace(typeof(TreeViewItem)) },
                    { "sys", XamlBuilder.GetNamespace(typeof(int)) }
                },
                ElementProperties = new Dictionary<string, XamlBuilder>
                {
                    {
                        "Resources",
                        new XamlBuilder<HierarchicalDataTemplate>
                        {
                            Key = "template",
                            AttributeProperties = new Dictionary<string, string> { { "ItemsSource", "{Binding Children}" } },
                            Children = new List<XamlBuilder>
                            {
                                new XamlBuilder<ContentControl>
                                {
                                    Name = "TemplateContent",
                                    AttributeProperties = new Dictionary<string, string>
                                    {
                                        { "Content", "{Binding Element}" },
                                        { "Foreground", "Red" }
                                    }
                                }
                            }
                        }
                    }
                },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<TreeView>
                    {
                        AttributeProperties = new Dictionary<string, string> { { "ItemTemplate", "{StaticResource template}" } },
                    }
                }
            };

            Grid root = builder.Load();
            TreeView view = null;
            TreeViewItem item = null;
            Hierarchy textbox = null;

            List<Hierarchy> items = new List<Hierarchy>()
            {
                new Hierarchy { Element = "Brush" },
                new Hierarchy { Element = "Style" },
                new Hierarchy
                {
                    Element = "UIElement",
                    Children = new Collection<Hierarchy>
                    {
                        new Hierarchy
                        {
                            Element = "FrameworkElement",
                            Children = new Collection<Hierarchy>
                            {
                                new Hierarchy
                                {
                                    Element = "Control",
                                    Children = new Collection<Hierarchy>
                                    {
                                        new Hierarchy { Element = "TextBox" }
                                    }
                                }
                            }
                        }
                    }
                },
                new Hierarchy { Element = "VisualStateManager" }
            };

            TestAsync(
                5,
                root,
                () => view = root.Children[0] as TreeView,
                () => view.ItemsSource = items,
                () => item = view.ItemContainerGenerator.ContainerFromIndex(2) as TreeViewItem,
                () => item.IsExpanded = true,
                () => item = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => item.IsExpanded = true,
                () => item = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => item.IsExpanded = true,
                () => item = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => textbox = item.Header as Hierarchy,
                () => Assert.IsNotNull(textbox, "TextBox reference should not be null!"),
                () => Assert.AreEqual("TextBox", textbox.Element, "Unexpected Element!"));
        }

        /// <summary>
        /// Apply a HierarchicalDataTemplate with ItemContainerStyle set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Apply a HierarchicalDataTemplate with ItemContainerStyle set.")]
        [Bug("532192 - TreeView - HierarchicalDataTemplate.ItemContainerStyle doesn't get picked up", Fixed = true)]
        [Priority(0)]
        public virtual void ApplyHierarchicalDataTemplateWithStyle()
        {
            XamlBuilder<Grid> builder = new XamlBuilder<Grid>
            {
                ExplicitNamespaces = new Dictionary<string, string>
                {
                    { "common", XamlBuilder.GetNamespace(typeof(HierarchicalDataTemplate)) },
                    { "ctl", XamlBuilder.GetNamespace(typeof(TreeViewItem)) },
                    { "sys", XamlBuilder.GetNamespace(typeof(int)) }
                },
                ElementProperties = new Dictionary<string, XamlBuilder>
                {
                    {
                        "Resources",
                        new XamlBuilder<Style>
                        {
                            Key = "itemStyle",
                            AttributeProperties = new Dictionary<string, string> { { "TargetType", "ctl:TreeViewItem" } },
                            Children = new List<XamlBuilder>
                            {
                                new XamlBuilder<Setter>
                                {
                                    AttributeProperties = new Dictionary<string, string> 
                                    {
                                        { "Property", "FontSize" },
                                        { "Value", "15" }
                                    }
                                }
                            }
                        }
                    }
                },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<Grid>
                    {
                        ElementProperties = new Dictionary<string, XamlBuilder>
                        {
                            {
                                "Resources",
                                new XamlBuilder<HierarchicalDataTemplate>
                                {
                                    Key = "template",
                                    AttributeProperties = new Dictionary<string, string>
                                    {
                                        { "ItemsSource", "{Binding Children}" },
                                        { "ItemContainerStyle", "{StaticResource itemStyle}" }
                                    },
                                    Children = new List<XamlBuilder>
                                    {
                                        new XamlBuilder<ContentControl>
                                        {
                                            Name = "TemplateContent",
                                            AttributeProperties = new Dictionary<string, string>
                                            {
                                                { "Content", "{Binding Element}" }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Children = new List<XamlBuilder>
                        {
                            new XamlBuilder<TreeView>
                            {
                                AttributeProperties = new Dictionary<string, string>
                                {
                                    { "ItemTemplate", "{StaticResource template}" },
                                    { "ItemContainerStyle", "{StaticResource itemStyle}" }
                                },
                            }
                        }
                    }
                }
            };

            Grid root = builder.Load();
            TreeView view = null;
            TreeViewItem item = null;

            List<Hierarchy> items = new List<Hierarchy>()
            {
                new Hierarchy { Element = "Brush" },
                new Hierarchy { Element = "Style" },
                new Hierarchy
                {
                    Element = "UIElement",
                    Children = new Collection<Hierarchy>
                    {
                        new Hierarchy
                        {
                            Element = "FrameworkElement",
                            Children = new Collection<Hierarchy>
                            {
                                new Hierarchy
                                {
                                    Element = "Control",
                                    Children = new Collection<Hierarchy>
                                    {
                                        new Hierarchy { Element = "TextBox" }
                                    }
                                }
                            }
                        }
                    }
                },
                new Hierarchy { Element = "VisualStateManager" }
            };

            TestAsync(
                5,
                root,
                () => view = (root.Children[0] as Grid).Children[0] as TreeView,
                () => view.ItemsSource = items,
                () => item = view.ItemContainerGenerator.ContainerFromIndex(2) as TreeViewItem,
                () => Assert.AreEqual(15.0, item.FontSize, "Style not applied to first item!"),
                () => item.IsExpanded = true,
                () => item = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => Assert.AreEqual(15.0, item.FontSize, "Style not applied to second item!"),
                () => item.IsExpanded = true,
                () => item = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => Assert.AreEqual(15.0, item.FontSize, "Style not applied to third item!"),
                () => item.IsExpanded = true);
        }

        /// <summary>
        /// Apply a HierarchicalDataTemplate with ItemTemplate set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Apply a HierarchicalDataTemplate with ItemTemplate set.")]
        [Priority(0)]
        public virtual void ApplyHierarchicalDataTemplateWithItemTemplate()
        {
            XamlBuilder<Grid> builder = new XamlBuilder<Grid>
            {
                ExplicitNamespaces = new Dictionary<string, string>
                {
                    { "common", XamlBuilder.GetNamespace(typeof(HierarchicalDataTemplate)) },
                    { "ctl", XamlBuilder.GetNamespace(typeof(TreeViewItem)) },
                    { "sys", XamlBuilder.GetNamespace(typeof(int)) }
                },
                ElementProperties = new Dictionary<string, XamlBuilder>
                {
                    {
                        "Resources",
                        new XamlBuilder<DataTemplate>
                        {
                            Key = "itemTemplate",
                            Children = new List<XamlBuilder>
                            {
                                new XamlBuilder<ContentControl>
                                {
                                    Name = "TemplateContent",
                                    AttributeProperties = new Dictionary<string, string>
                                    {
                                        { "Content", "{Binding Element}" },
                                        { "FontWeight", "Bold" }
                                    }
                                }
                            }
                        }
                    }
                },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<Grid>
                    {
                        ElementProperties = new Dictionary<string, XamlBuilder>
                        {
                            {
                                "Resources",
                                new XamlBuilder<HierarchicalDataTemplate>
                                {
                                    Key = "template",
                                    AttributeProperties = new Dictionary<string, string>
                                    {
                                        { "ItemsSource", "{Binding Children}" },
                                        { "ItemTemplate", "{StaticResource itemTemplate}" }
                                    },
                                    Children = new List<XamlBuilder>
                                    {
                                        new XamlBuilder<ContentControl>
                                        {
                                            Name = "TemplateContent",
                                            AttributeProperties = new Dictionary<string, string>
                                            {
                                                { "Content", "{Binding Element}" },
                                                { "FontStyle", "Italic" }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Children = new List<XamlBuilder>
                        {
                            new XamlBuilder<TreeView>
                            {
                                AttributeProperties = new Dictionary<string, string>
                                {
                                    { "ItemTemplate", "{StaticResource template}" }
                                },
                            }
                        }
                    }
                }
            };

            Grid root = builder.Load();
            TreeView view = null;
            TreeViewItem item = null;

            List<Hierarchy> items = new List<Hierarchy>()
            {
                new Hierarchy { Element = "Brush" },
                new Hierarchy { Element = "Style" },
                new Hierarchy
                {
                    Element = "UIElement",
                    Children = new Collection<Hierarchy>
                    {
                        new Hierarchy
                        {
                            Element = "FrameworkElement",
                            Children = new Collection<Hierarchy>
                            {
                                new Hierarchy
                                {
                                    Element = "Control",
                                    Children = new Collection<Hierarchy>
                                    {
                                        new Hierarchy { Element = "TextBox" }
                                    }
                                }
                            }
                        }
                    }
                },
                new Hierarchy { Element = "VisualStateManager" }
            };

            TestAsync(
                5,
                root,
                () => view = (root.Children[0] as Grid).Children[0] as TreeView,
                () => view.ItemsSource = items,
                () => item = view.ItemContainerGenerator.ContainerFromIndex(2) as TreeViewItem,
                () => item.IsExpanded = true,
                () => item = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => Assert.AreEqual(0, item.Items.Count, "Only the first level should have nested items!"));
        }

        /// <summary>
        /// Apply a HierarchicalDataTemplate and then set ItemContainerStyle.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Apply a HierarchicalDataTemplate and then set ItemContainerStyle.")]
        public virtual void ApplyHierarchicalDataTemplateThenSetStyle()
        {
            XamlBuilder<Grid> builder = new XamlBuilder<Grid>
            {
                ExplicitNamespaces = new Dictionary<string, string>
                {
                    { "common", XamlBuilder.GetNamespace(typeof(HierarchicalDataTemplate)) },
                    { "ctl", XamlBuilder.GetNamespace(typeof(TreeViewItem)) },
                    { "sys", XamlBuilder.GetNamespace(typeof(int)) }
                },
                ElementProperties = new Dictionary<string, XamlBuilder>
                {
                    {
                        "Resources",
                        new XamlBuilder<HierarchicalDataTemplate>
                        {
                            Key = "template",
                            AttributeProperties = new Dictionary<string, string> { { "ItemsSource", "{Binding Children}" } },
                            Children = new List<XamlBuilder>
                            {
                                new XamlBuilder<ContentControl>
                                {
                                    Name = "TemplateContent",
                                    AttributeProperties = new Dictionary<string, string>
                                    {
                                        { "Content", "{Binding Element}" }
                                    }
                                }
                            }
                        }
                    }
                },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<TreeView>
                    {
                        AttributeProperties = new Dictionary<string, string> { { "ItemTemplate", "{StaticResource template}" } },
                    }
                }
            };

            Grid root = builder.Load();
            TreeView view = null;
            TreeViewItem item = null;

            List<Hierarchy> items = new List<Hierarchy>()
            {
                new Hierarchy { Element = "Brush" },
                new Hierarchy { Element = "Style" },
                new Hierarchy
                {
                    Element = "UIElement",
                    Children = new Collection<Hierarchy>
                    {
                        new Hierarchy
                        {
                            Element = "FrameworkElement",
                            Children = new Collection<Hierarchy>
                            {
                                new Hierarchy
                                {
                                    Element = "Control",
                                    Children = new Collection<Hierarchy>
                                    {
                                        new Hierarchy { Element = "TextBox" }
                                    }
                                }
                            }
                        }
                    }
                },
                new Hierarchy { Element = "VisualStateManager" }
            };

            Style outerStyle = new Style(typeof(TreeViewItem));
            outerStyle.Setters.Add(new Setter(TreeViewItem.FontSizeProperty, 20.0));

            Style innerStyle = new Style(typeof(TreeViewItem));
            innerStyle.Setters.Add(new Setter(TreeViewItem.FontSizeProperty, 10.0));

            TestAsync(
                5,
                root,
                () => view = root.Children[0] as TreeView,
                () => view.ItemsSource = items,
                () => item = view.ItemContainerGenerator.ContainerFromIndex(2) as TreeViewItem,
                () => item.IsExpanded = true,
                () => item = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => item.IsExpanded = true,
                () => view.ItemContainerStyle = outerStyle,
                () => item = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => item.IsExpanded = true,
                () => item.ItemContainerStyle = innerStyle,
                () => item.ItemContainerStyle = null);
        }

        /// <summary>
        /// Prepare a TreeViewItem where the Header is the Item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Prepare a TreeViewItem where the Header is the Item.")]
        public virtual void HeaderIsItem()
        {
            TreeView view = new TreeView { ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItem item = null;

            TestAsync(
                view,
                () => item = view.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => Assert.AreEqual(1, item.Header, "Unexpected item header!"));
        }
        #endregion Prepare

        /// <summary>
        /// Removed items are no longer mapped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Removed items are no longer mapped.")]
        public virtual void RemovedItemsAreNoLongerMapped()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            TestAsync(
                5,
                view,
                () => view.Items.Remove(first),
                () => Assert.AreEqual(-1, view.ItemContainerGenerator.IndexFromContainer(first), "First item should no longer be in the collection!"));
        }
    }
}
