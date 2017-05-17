// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls.DataVisualization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeMap unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeMap")]
    public partial class TreeMapTest : ControlTest
    {   
        /// <summary>
        /// Constant used with double calculations.
        /// </summary>
        private const double Epsilon = 0.001;

        #region Controls to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return DefaultTreeMapToTest; }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override System.Collections.Generic.IEnumerable<Control> ControlsToTest
        {
            get
            {
                return
                    TreeMapsToTest.OfType<Control>()
                    .Concat(TreeMapsToTest.Select(
                        control =>
                        {
                            control.ItemsSource = new string[] { "Item 1", "Item 2", "Item 3", "Item 4" };
                            return (Control)control;
                        }));
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override System.Collections.Generic.IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { return OverriddenTreeMapsToTest.OfType<IOverriddenControl>(); }
        }
        #endregion Controls to test

        #region TreeMaps to test
        
        /// <summary>
        /// XAML for non-trivial TreeMap tests.
        /// </summary>
        private const string InterpolatedTreeMapToTestXaml = @"
            <datavis:TreeMap
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                xmlns:datavis=""clr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit""
                Height=""200"" Width=""200"">
                <datavis:TreeMap.Interpolators>
                    <datavis:SolidColorBrushInterpolator TargetName=""itemBorder"" TargetProperty=""Background""
                                               DataRangeBinding=""{Binding Wins}"" From=""Black"" To=""White"" />
                </datavis:TreeMap.Interpolators>

                <datavis:TreeMap.ItemDefinition>
                    <datavis:TreeMapItemDefinition ItemsSource=""{Binding Children}"" ValueBinding=""{Binding Points}"" ChildItemPadding=""0"">
                        <DataTemplate>
                            <Border x:Name=""itemBorder"" Tag=""{Binding Name}"" BorderBrush=""Black"" BorderThickness=""1"">
                                <TextBlock x:Name=""textB"" Foreground=""White"" Text=""{Binding Name}"" VerticalAlignment=""Center"" Margin=""2,2,0,0""
                                           TextWrapping=""Wrap"" TextAlignment=""Center"" />
                            </Border>
                        </DataTemplate>
                    </datavis:TreeMapItemDefinition>
                </datavis:TreeMap.ItemDefinition>
            </datavis:TreeMap>";

        /// <summary>
        /// XAML for a simple ItemTemplate DataTemplate.
        /// </summary>
        private const string SimpleItemTemplate = @"
            <DataTemplate
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <Border/>
            </DataTemplate>";

        /// <summary>
        /// Default collection used for ItemsSource in testing.
        /// </summary>
        private readonly List<SimpleNode> InterpolatedTreeMapItemsSource = new List<SimpleNode>
         {
             new SimpleNode
                 {
                     Name = "A",
                     Points = 20,
                     Wins = 100,
                     Children = new SimpleNode[0],
                 },
             new SimpleNode
                 {
                     Name = "B",
                     Children = new SimpleNode[]
                                    {
                                        new SimpleNode
                                            {
                                                Name = "C",
                                                Points = 50,
                                                Wins = 20,
                                                Children = null,
                                            },
                                        new SimpleNode
                                            {
                                                Name = "D",
                                                Points = 30,
                                                Wins = 0,
                                                Children = new SimpleNode[0],
                                            },
                                    }
                 },
         };

        /// <summary>
        /// Gets a default instance of TreeMap (or a derived type) to test.
        /// </summary>
        public virtual TreeMap DefaultTreeMapToTest
        {
            get
            {
                return new TreeMap();
            }
        }

        /// <summary>
        /// Gets an instance of TreeMap (or a derived type) to test with interpolators.
        /// </summary>
        public virtual TreeMap InterpolatedTreeMapToTest
        {
            get
            {
                TreeMap testTreeMap = (TreeMap)XamlReader.Load(InterpolatedTreeMapToTestXaml);
                return testTreeMap;
            }
        }

        /// <summary>
        /// Gets instances of TreeMap (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<TreeMap> TreeMapsToTest
        {
            get
            {
                yield return DefaultTreeMapToTest;
            }
        }

        /// <summary>
        /// Gets instances of OverriddenTreeMap (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<OverriddenTreeMap> OverriddenTreeMapsToTest
        {
            get { yield return new OverriddenTreeMap(); }
        }
        #endregion TreeMaps to test

        /// <summary>
        /// Gets the TreeMapItemDefinitionSelector dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeMap, TreeMapItemDefinitionSelector> TreeMapItemDefinitionSelectorProperty { get; private set; }

        /// <summary>
        /// Gets the ItemDefinition dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeMap, TreeMapItemDefinition> ItemDefinitionProperty { get; private set; }

        /// <summary>
        /// Gets the ItemsSource dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeMap, IEnumerable> ItemsSourceProperty { get; private set; }

        /// <summary>
        /// Gets the Interpolators dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeMap, IList> InterpolatorsProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TreeMapTest class.
        /// </summary>
        public TreeMapTest()
            : base()
        {
            Func<TreeMap> initializer = () => DefaultTreeMapToTest;
            TreeMapItemDefinitionSelectorProperty = new DependencyPropertyTest<TreeMap, TreeMapItemDefinitionSelector>(this, "TreeMapItemDefinitionSelector")
            {
                Property = TreeMap.ItemDefinitionSelectorProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new OverriddenTreeMapItemDefinitionSelector() }
            };
            ItemDefinitionProperty = new DependencyPropertyTest<TreeMap, TreeMapItemDefinition>(this, "ItemDefinition")
            {
                Property = TreeMap.ItemDefinitionProperty,
                Initializer = initializer,
                OtherValues = new TreeMapItemDefinition[] { new TreeMapItemDefinition() }
            };

            ItemsSourceProperty = new DependencyPropertyTest<TreeMap, IEnumerable>(this, "ItemsSource")
            {
                Property = TreeMap.ItemsSourceProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new IList[] { new string[] { }, null }
                // OtherValues = new IList[] { new string[] { "hello", "world" }, null } - not used as ItemDefinition must be set
            };

            InterpolatorsProperty = new DependencyPropertyTest<TreeMap, IList>(this, "Interpolators")
            {
                Property = TreeMap.InterpolatorsProperty,
                Initializer = initializer,
                DefaultValue = new ObservableCollection<Interpolator>(),
                OtherValues = new IList[] { new List<Interpolator>() { } }
            };
        }

        /// <summary>
        /// Verifies the Control's TemplateParts.
        /// </summary>
        [TestMethod]
        [Description("Verifies the Control's TemplateParts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(1, templateParts.Count);
            Assert.AreSame(typeof(Canvas), templateParts["Container"]);
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // ItemDefinitionProperty tests
            tests.Add(ItemDefinitionProperty.BindingTest);
            tests.Add(ItemDefinitionProperty.CanBeStyledTest);
            tests.Add(ItemDefinitionProperty.ChangeClrSetterTest);
            tests.Add(ItemDefinitionProperty.ChangeSetValueTest);
            tests.Add(ItemDefinitionProperty.SetNullTest);
            // tests.Add(ItemDefinitionProperty.SetXamlAttributeTest.Bug("Serialize needed"));
            // tests.Add(ItemDefinitionProperty.SetXamlElementTest.Bug("Serialize needed"));
            tests.Add(ItemDefinitionProperty.TemplateBindTest);

            // ItemsSourceProperty tests
            tests.Add(ItemsSourceProperty.BindingTest);
            tests.Add(ItemsSourceProperty.CheckDefaultValueTest);
            tests.Add(ItemsSourceProperty.ChangeClrSetterTest);
            tests.Add(ItemsSourceProperty.ChangeSetValueTest);
            tests.Add(ItemsSourceProperty.SetNullTest);
            tests.Add(ItemsSourceProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemsSourceProperty.TemplateBindTest);
            tests.Add(ItemsSourceProperty.SetXamlAttributeTest);
            tests.Add(ItemsSourceProperty.SetXamlElementTest);
            tests.Add(ItemsSourceProperty.IsContentPropertyTest);

            // InterpolatorsProperty tests
            tests.Add(InterpolatorsProperty.CheckDefaultValueTest.Bug("Not null value requires ObservableCollection to override Object.Equals"));
            tests.Add(InterpolatorsProperty.IsReadOnlyTest);
            tests.Add(ItemsSourceProperty.TemplateBindTest);

            return tests;
        }

        /// <summary>
        /// Finds element in the visual tree with a given tag.
        /// </summary>
        /// <param name="element">Reference to the root search element.</param>
        /// <param name="tag">Tag name to find.</param>
        /// <returns>Element with a tag or null.</returns>
        protected virtual FrameworkElement FindElementWithTag(FrameworkElement element, string tag)
        {
            int children = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < children; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child != null)
                {
                    string childTag = child.Tag as string;
                    if (String.Compare(childTag, tag, StringComparison.Ordinal) == 0)
                    {
                        return child;
                    }
                    else
                    {
                        FrameworkElement el = FindElementWithTag(child, tag);
                        if (el != null)
                        {
                            return el;  
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Check if area taken by a Framework element is as expected. ItemsSpacing
        /// should be set to 0.
        /// </summary>
        /// <param name="treeMap">TreeMap reference.</param>
        /// <param name="name">Element Tag Name.</param>
        /// <param name="targetRatio">Ratio: Element size / Total size.</param>
        protected virtual void AssertSize(FrameworkElement treeMap, string name, double targetRatio)
        {
            FrameworkElement el = FindElementWithTag(treeMap, name);
            if (el == null)
            {
                throw new ArgumentException("The child FrameworkElement in TreeMap not found");
            }

            double totalSize = treeMap.ActualHeight * treeMap.ActualWidth;
            double actualArea = el.ActualHeight * el.ActualWidth;
            double expectedArea = targetRatio * totalSize;
            Assert.AreEqual(expectedArea, actualArea, Epsilon);
        }

        /// <summary>
        /// Verifies simple TreeMap layout.
        /// </summary>
        /// <remarks>Values chosen not to cause visual rounding.</remarks>
        [TestMethod, Asynchronous]
        [Description("Verifies a simple TreeMap layout.")]
        public void SimpleTreeMapTest()
        {
            TreeMap treeMapControl = InterpolatedTreeMapToTest;
            treeMapControl.ItemsSource = InterpolatedTreeMapItemsSource;

            TestAsync(
                treeMapControl,
                () => AssertSize(treeMapControl, "A", 0.2),
                () => AssertSize(treeMapControl, "B", 0.5 + 0.3),
                () => AssertSize(treeMapControl, "C", 0.5),
                () => AssertSize(treeMapControl, "D", 0.3));
        }

        /// <summary>
        /// Verifies simple TreeMap layout with 0 and negative size elements.
        /// </summary>
        /// <remarks>Values chosen not to cause visual rounding.</remarks>
        [TestMethod, Asynchronous]
        [Description("Verifies simple TreeMap layout with 0 and negative size elements.")]
        public void EdgeAreaValuesTreeMapTest()
        {
            TreeMap treeMapControl = InterpolatedTreeMapToTest;
            List<SimpleNode> itemList = new List<SimpleNode>
            {
             new SimpleNode
                 {
                     Name = "A",
                     Points = 0.0,
                     Wins = 100,
                     Children = new SimpleNode[0],
                 },
             new SimpleNode
                 {
                     Name = "B",
                     Children = new SimpleNode[]
                                    {
                                        new SimpleNode
                                            {
                                                Name = "C",
                                                Points = 80.0,
                                                Wins = 20,
                                                Children = null,
                                            },
                                        new SimpleNode
                                            {
                                                Name = "D",
                                                Points = 20.0,
                                                Wins = 0,
                                                Children = new SimpleNode[0],
                                            },
                                    }
                 },
            };
            treeMapControl.ItemsSource = itemList;

            TestAsync(
                treeMapControl,
                () => Assert.IsNull(FindElementWithTag(treeMapControl, "A")),
                () => AssertSize(treeMapControl, "B", 1),
                () => AssertSize(treeMapControl, "C", 0.8),
                () => AssertSize(treeMapControl, "D", 0.2),
                () => itemList[0].Points = -100,
                () => treeMapControl.ItemsSource = null,
                () => treeMapControl.ItemsSource = itemList,
                () => Assert.IsNull(FindElementWithTag(treeMapControl, "A")),
                () => AssertSize(treeMapControl, "B", 1),
                () => AssertSize(treeMapControl, "C", 0.8),
                () => AssertSize(treeMapControl, "D", 0.2),
                () => itemList[0].Points = 0.01,
                () => treeMapControl.ItemsSource = null,
                () => treeMapControl.ItemsSource = itemList,
                () => Assert.IsNotNull(FindElementWithTag(treeMapControl, "A")),
                () => AssertSize(treeMapControl, "B", 1),
                () => AssertSize(treeMapControl, "C", 0.8),
                () => AssertSize(treeMapControl, "D", 0.2));
        }

        /// <summary>
        /// Verifies horizontal snap to pixels TreeMap layout behaviour.
        /// </summary>
        /// <remarks>Values chosen to cause visual rounding.</remarks>
        [TestMethod, Asynchronous]
        [Description("Verifies horizontal snap to pixels TreeMap layout behaviour.")]
        public void RoundingHorizontalTreeMapTest()
        {
            TreeMap treeMapControl = InterpolatedTreeMapToTest;
            treeMapControl.ItemsSource = InterpolatedTreeMapItemsSource;

            TestAsync(
                treeMapControl,
                () => treeMapControl.Width = 199.50,
                () => AssertSize(treeMapControl, "A", 0.2),
                () => AssertSize(treeMapControl, "B", 0.5 + 0.3),
                () => AssertSize(treeMapControl, "C", 0.5),
                () => AssertSize(treeMapControl, "D", 0.3),
                () => treeMapControl.Width = 200.49,
                () => AssertSize(treeMapControl, "A", 0.2),
                () => AssertSize(treeMapControl, "B", 0.5 + 0.3),
                () => AssertSize(treeMapControl, "C", 0.5),
                () => AssertSize(treeMapControl, "D", 0.3));
        }

        /// <summary>
        /// Verifies vertical snap to pixels TreeMap layout behaviour.
        /// </summary>
        /// <remarks>Values chosen to cause visual rounding.</remarks>
        [TestMethod, Asynchronous]
        [Description("Verifies vertical snap to pixels TreeMap layout behaviour.")]
        public void RoundingVerticalTreeMapTest()
        {
            TreeMap treeMapControl = InterpolatedTreeMapToTest;
            treeMapControl.ItemsSource = InterpolatedTreeMapItemsSource;

            TestAsync(
                treeMapControl,
                () => treeMapControl.Height = 199.50,
                () => AssertSize(treeMapControl, "A", 0.2),
                () => AssertSize(treeMapControl, "B", 0.5 + 0.3),
                () => AssertSize(treeMapControl, "C", 0.5),
                () => AssertSize(treeMapControl, "D", 0.3),
                () => treeMapControl.Height = 200.49,
                () => AssertSize(treeMapControl, "A", 0.2),
                () => AssertSize(treeMapControl, "B", 0.5 + 0.3),
                () => AssertSize(treeMapControl, "C", 0.5),
                () => AssertSize(treeMapControl, "D", 0.3));
        }

        /// <summary>
        /// Verifies fixed minimum and maximum values in the interpolators.
        /// </summary>
        [TestMethod, Asynchronous]
        [Description("Verifies fixed minimum and maximum values in the interpolators.")]
        [Bug("706249: Implement TreeMap API review changes", Fixed = true)]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Simplifies the test.")]
        public void FixedMinMaxTreeMapTest()
        {
            TreeMap testTreeMap = InterpolatedTreeMapToTest;
            testTreeMap.Interpolators.Clear();
            DoubleInterpolator testInterpolator = new DoubleInterpolator
                                                      {
                                                          From = 11,
                                                          To = 20,
                                                          DataRangeBinding = new Binding("Points"),
                                                          TargetName = "textB",
                                                          TargetProperty = "FontSize"
                                                      };
            testTreeMap.Interpolators.Add(testInterpolator);
            testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource;

            TestAsync(
                testTreeMap,
                () => Assert.AreEqual(50, testInterpolator.ActualDataMaximum),
                () => Assert.AreEqual(20, testInterpolator.ActualDataMinimum),
                // Max set
                () => testInterpolator.DataMaximum = 100,
                () => testTreeMap.ItemsSource = null,
                () => testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource,
                () => Assert.AreEqual(100, testInterpolator.ActualDataMaximum),
                () => Assert.AreEqual(20, testInterpolator.ActualDataMinimum),
                // Min set
                () => testInterpolator.DataMinimum = 0,
                () => testInterpolator.DataMaximum = Double.NaN,
                () => testTreeMap.ItemsSource = null,
                () => testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource,
                () => Assert.AreEqual(50, testInterpolator.ActualDataMaximum),
                () => Assert.AreEqual(0, testInterpolator.ActualDataMinimum),
                // Min & Max set
                () => testInterpolator.DataMinimum = 0,
                () => testInterpolator.DataMaximum = 100,
                () => testTreeMap.ItemsSource = null,
                () => testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource,
                () => Assert.AreEqual(100, testInterpolator.ActualDataMaximum),
                () => Assert.AreEqual(0, testInterpolator.ActualDataMinimum),
                // Double ItemsSource set, Min & Max in the middle of the range
                () => testInterpolator.DataMinimum = 15,
                () => testInterpolator.DataMaximum = 30,
                () => testTreeMap.ItemsSource = null,
                () => testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource,
                () => testTreeMap.ItemsSource = null,
                () => testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource,
                () => Assert.AreEqual(30, testInterpolator.ActualDataMaximum),
                () => Assert.AreEqual(15, testInterpolator.ActualDataMinimum));
        }

        /// <summary>
        /// Verifies the interpolation mode for TreeMap.
        /// </summary>
        [TestMethod, Asynchronous]
        [Description("Verifies the interpolation mode for TreeMap.")]
        [Bug("706889: Auto interpolation for non-leave nodes", Fixed = true)]
        public void InterpolatorsModeTreeMapTest()
        {
            TreeMap testTreeMap = InterpolatedTreeMapToTest;
            testTreeMap.Interpolators.Clear();
            DoubleInterpolator testInterpolator = new DoubleInterpolator
                                                      {
                                                          From = 0.2,
                                                          To = 0.5,
                                                          DataRangeBinding = new Binding("Points"),
                                                          TargetName = "itemBorder",
                                                          TargetProperty = "Opacity"
                                                      };
            testTreeMap.Interpolators.Add(testInterpolator);
            // We do not aggregate for interpolators as properties can be non additive
            InterpolatedTreeMapItemsSource[1].Points = 80;
            testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource;

            TestAsync(
                testTreeMap,
                // Check LeafNodesOnly behavior by default
                () => Assert.AreEqual(50, testInterpolator.ActualDataMaximum),
                () => Assert.AreEqual(20, testInterpolator.ActualDataMinimum),
                () => Assert.AreEqual(0.2, FindElementWithTag(testTreeMap, "A").Opacity, Epsilon),
                () => Assert.AreEqual(1, FindElementWithTag(testTreeMap, "B").Opacity, Epsilon),
                () => Assert.AreEqual(0.5, FindElementWithTag(testTreeMap, "C").Opacity, Epsilon),
                () => Assert.AreEqual(0.3, FindElementWithTag(testTreeMap, "D").Opacity, Epsilon),
                // Check AllNodes calculations
                () => testInterpolator.InterpolationMode = InterpolationMode.AllNodes,
                () => testInterpolator.To = 0.8,
                () => testTreeMap.ItemsSource = null,
                () => testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource,
                () => Assert.AreEqual(80, testInterpolator.ActualDataMaximum),
                () => Assert.AreEqual(20, testInterpolator.ActualDataMinimum),
                () => Assert.AreEqual(0.2, FindElementWithTag(testTreeMap, "A").Opacity, Epsilon),
                () => Assert.AreEqual(0.8, FindElementWithTag(testTreeMap, "B").Opacity, Epsilon),
                () => Assert.AreEqual(0.5, FindElementWithTag(testTreeMap, "C").Opacity, Epsilon),
                () => Assert.AreEqual(0.3, FindElementWithTag(testTreeMap, "D").Opacity, Epsilon));
        }

        /// <summary>
        /// Verifies that providing a null TreeMapItemDefinition.ValueBinding doesn't cause a crash.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that providing a null TreeMapItemDefinition.ValueBinding doesn't cause a crash.")]
        [Bug("714729: If TreeMapItemDefinition.ValueBinding or .ItemTemplate is null, TreeMap code throws a NullReferenceException", Fixed = true)]
        public void ProvidingEmptyTreeMapItemDefinitionWorks()
        {
            TreeMap treeMap = new TreeMap();
            treeMap.ItemDefinition = new TreeMapItemDefinition();
            TestAsync(
                treeMap,
                // +1 because of the Border in default template
                () => treeMap.ItemsSource = new int[] { 1, 2, 3 },
                () => Assert.AreEqual(0 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()));
        }

        /// <summary>
        /// Verifies that providing a null TreeMapItemDefinition.ItemTemplate doesn't cause a crash.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that providing a null TreeMapItemDefinition.ItemTemplate doesn't cause a crash.")]
        [Bug("714729: If TreeMapItemDefinition.ValueBinding or .ItemTemplate is null, TreeMap code throws a NullReferenceException", Fixed = true)]
        public void ProvidingNullTreeMapItemDefinitionItemTemplateWorks()
        {
            TreeMap treeMap = new TreeMap();
            treeMap.ItemDefinition = new TreeMapItemDefinition { ValueBinding = new Binding() };
            TestAsync(
                treeMap,
                // +1 because of the Border in default template
                () => treeMap.ItemsSource = new int[] { 1, 2, 3 },
                () => Assert.AreEqual(0 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()));
        }

        /// <summary>
        /// Verifies that a simple TreeMap successfully renders basic UI.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that a simple TreeMap successfully renders basic UI.")]
        [Bug("707817: Create default ItemTemplate for TreeMap", Fixed = true)]
        public void TreeMapRendersSimpleData()
        {
            TreeMap treeMap = new TreeMap();
            treeMap.ItemsSource = new int[] { 1, 2, 3 };
            TestAsync(
                treeMap,
                // +1 because of the Border in default template
                () => Assert.AreEqual(3 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()));
        }

        /// <summary>
        /// Verifies that clearing ItemsSource removes any child elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that clearing ItemsSource removes any child elements.")]
        [Bug("713797: Setting TreeMap.ItemsSource to null for a TreeMap with items does not correctly clear out the TreeMap", Fixed = true)]
        public void ClearingItemsSourceRemovesChildren()
        {
            TreeMap treeMap = new TreeMap();
            TestAsync(
                treeMap,
                // +1 because of the Border in default template
                () => Assert.AreEqual(0 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => treeMap.ItemsSource = new int[] { 1, 2, 3 },
                () => Assert.AreEqual(3 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => treeMap.ItemsSource = null,
                () => Assert.AreEqual(0 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()));
        }

        /// <summary>
        /// Verifies that changing TreeMapItemDefinition.ItemsSource updates the corresponding TreeMap.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that changing TreeMapItemDefinition.ItemsSource updates the corresponding TreeMap.")]
        [Bug("714392: TreeMap not updating when TreeMapItemDefinition.ItemsSource is changed (or for that matter when ANY TreeMapItemDefinition properties change)", Fixed = true)]
        public void ChangingTreeMapItemDefinitionItemsSourceUpdatesTreeMap()
        {
            TreeMap treeMap = new TreeMap();
            TreeMapItemDefinition itemDefinition = new TreeMapItemDefinition { ValueBinding = new Binding() };
            itemDefinition.ItemTemplate = (DataTemplate)XamlReader.Load(SimpleItemTemplate);
            treeMap.ItemDefinition = itemDefinition;
            treeMap.ItemsSource = new KeyValuePair<int, int[]>[] { new KeyValuePair<int, int[]>(1, new int[] { 2, 3, 4, 5 }) };
            TestAsync(
                treeMap,
                // +1 because of the Border in default template
                () => Assert.AreEqual(1 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => itemDefinition.ItemsSource = new Binding("Value"),
                () => Assert.AreEqual(5 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()));
        }

        /// <summary>
        /// Verifies that changing the contents of an ObservableCollection ItemsSource updates the TreeMap.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that changing the contents of an ObservableCollection ItemsSource updates the TreeMap.")]
        public void ChangingContentsOfObservableCollectionUpdatesTreeMap()
        {
            TreeMap treeMap = new TreeMap();
            ObservableCollection<int> itemsSource = new ObservableCollection<int>();
            treeMap.ItemsSource = itemsSource;
            TestAsync(
                treeMap,
                // +1 because of the Border in default template
                () => Assert.AreEqual(0 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => itemsSource.Add(1),
                () => Assert.AreEqual(1 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => itemsSource.Add(1),
                () => Assert.AreEqual(2 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => itemsSource.Clear(),
                () => Assert.AreEqual(0 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()));
        }

        /// <summary>
        /// Verifies that changing the contents of a child's ObservableCollection ItemsSource updates the TreeMap.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that changing the contents of a child's ObservableCollection ItemsSource updates the TreeMap.")]
        public void ChangingContentsOfNestedObservableCollectionUpdatesTreeMap()
        {
            TreeMap treeMap = new TreeMap();
            TreeMapItemDefinition itemDefinition = new TreeMapItemDefinition
            {
                ValueBinding = new Binding(),
                ItemsSource = new Binding("Value"),
            };
            itemDefinition.ItemTemplate = (DataTemplate)XamlReader.Load(SimpleItemTemplate);
            treeMap.ItemDefinition = itemDefinition;
            ObservableCollection<int> nestedItemsSourceA = new ObservableCollection<int>();
            ObservableCollection<int> nestedItemsSourceB = new ObservableCollection<int>();
            treeMap.ItemsSource = new KeyValuePair<int, ObservableCollection<int>>[]
            {
                new KeyValuePair<int, ObservableCollection<int>>(0, nestedItemsSourceA),
                new KeyValuePair<int, ObservableCollection<int>>(0, nestedItemsSourceB),
            };
            TestAsync(
                treeMap,
                // +1 because of the Border in default template
                () => Assert.AreEqual(0 + 2 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => nestedItemsSourceA.Add(1),
                () => Assert.AreEqual(1 + 2 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => nestedItemsSourceB.Add(2),
                () => Assert.AreEqual(2 + 2 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => nestedItemsSourceA.Add(3),
                () => Assert.AreEqual(3 + 2 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => nestedItemsSourceB.Clear(),
                () => Assert.AreEqual(2 + 2 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()),
                () => nestedItemsSourceA.Clear(),
                () => Assert.AreEqual(0 + 2 + 1, treeMap.GetVisualDescendents().OfType<Border>().Count()));
        }

        /// <summary>
        /// Verifies that a TreeMap in a Grid is the full size of the Grid.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that a TreeMap in a Grid is the full size of the Grid.")]
        [Bug("707816: TreeMap's Measure/Arrange should be changed to give more platform-consistent behavior", Fixed = true)]
        public void TreeMapInGridIsFullSize()
        {
            TreeMap treeMap = new TreeMap();
            Grid panel = new Grid { Width = 100, Height = 200 };
            panel.Children.Add(treeMap);
            TestAsync(
                panel,
                () => Assert.AreEqual(panel.ActualWidth, treeMap.ActualWidth),
                () => Assert.AreEqual(panel.ActualHeight, treeMap.ActualHeight));
        }

        /// <summary>
        /// Verifies that a TreeMap in a vertical StackPanel has its minimum height.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that a TreeMap in a vertical StackPanel has its minimum height.")]
        [Bug("707816: TreeMap's Measure/Arrange should be changed to give more platform-consistent behavior", Fixed = true)]
        public void TreeMapInVerticalStackPanelHasMinHeight()
        {
            TreeMap treeMap = new TreeMap();
            StackPanel panel = new StackPanel { Width = 100, Height = 200, Orientation = Orientation.Vertical };
            panel.Children.Add(treeMap);
            TestAsync(
                panel,
                () => Assert.AreEqual(panel.ActualWidth, treeMap.ActualWidth),
                () => Assert.AreEqual(treeMap.MinHeight, treeMap.ActualHeight));
        }

        /// <summary>
        /// Verifies that a TreeMap in a horizontal StackPanel has its minimum width.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that a TreeMap in a horizontal StackPanel has its minimum width.")]
        [Bug("707816: TreeMap's Measure/Arrange should be changed to give more platform-consistent behavior", Fixed = true)]
        public void TreeMapInHorizontalStackPanelHasMinWidth()
        {
            TreeMap treeMap = new TreeMap();
            StackPanel panel = new StackPanel { Width = 100, Height = 200, Orientation = Orientation.Horizontal };
            panel.Children.Add(treeMap);
            TestAsync(
                panel,
                () => Assert.AreEqual(treeMap.MinWidth, treeMap.ActualWidth),
                () => Assert.AreEqual(panel.ActualHeight, treeMap.ActualHeight));
        }

        /// <summary>
        /// Verifies that a TreeMap correctly exposes protected virtual interface.
        /// </summary>
        /// <remarks>We assume that default values are tested via GetDependencyPropertyTests.</remarks>
        [TestMethod]
        [Description("Verifies that a TreeMap correctly exposes protected virtual interface.")]
        [Bug("706876: Check remaining low prori code review feedback from davidans", Fixed = true)]
        public void TreeMapInheritViaProtectedVirtual()
        {
            InheritedTreeMap treeMap = new InheritedTreeMap();

            // Set initial values.
            TreeMapItemDefinitionSelector selectorOld = new SampleTemplateSelector();
            treeMap.SetValue(TreeMap.ItemDefinitionSelectorProperty, selectorOld);
            TreeMapItemDefinition definitionOld = new TreeMapItemDefinition { ChildItemPadding = new Thickness(0) };
            treeMap.SetValue(TreeMap.ItemDefinitionProperty, definitionOld);
            int[] itemsOld = new int[] { 1 };
            treeMap.SetValue(TreeMap.ItemsSourceProperty, itemsOld);
            Collection<Interpolator> collectionOld = new Collection<Interpolator> { new DoubleInterpolator() };
            treeMap.SetValue(TreeMap.InterpolatorsProperty, collectionOld);

            // Test TreeMapItemDefinitionSelectorProperty
            TreeMapItemDefinitionSelector selectorNew = new SampleTemplateSelector();
            bool calledItemDefinitionSelectorPropertyEvent = false;
            
            treeMap.OnItemDefinitionSelectorPropertyChangedEvent += (oldValue, newValue) =>
            {
                Assert.AreEqual(oldValue, selectorOld);
                Assert.AreEqual(newValue, selectorNew);
                Assert.AreNotEqual(newValue, oldValue);
                Assert.IsFalse(calledItemDefinitionSelectorPropertyEvent);
                calledItemDefinitionSelectorPropertyEvent = true;
            };
            treeMap.SetValue(TreeMap.ItemDefinitionSelectorProperty, selectorNew);
            Assert.IsTrue(calledItemDefinitionSelectorPropertyEvent);

            // Test ItemDefinitionProperty
            TreeMapItemDefinition definitionNew = new TreeMapItemDefinition { ChildItemPadding = new Thickness(1) };

            bool calledItemDefinitionPropertyyEvent = false;
            treeMap.OnItemDefinitionPropertyChangedEvent += (oldValue, newValue) =>
            {
                Assert.AreEqual(oldValue, definitionOld);
                Assert.AreEqual(newValue, definitionNew);
                Assert.AreNotEqual(newValue, oldValue);
                Assert.IsFalse(calledItemDefinitionPropertyyEvent);
                calledItemDefinitionPropertyyEvent = true;
            };
            treeMap.SetValue(TreeMap.ItemDefinitionProperty, definitionNew);
            Assert.IsTrue(calledItemDefinitionPropertyyEvent);

            // Test ItemsSourceProperty
            int[] itemsNew = new int[] { 1, 2, 3 };

            bool calledItemsSourceProperty = false;
            treeMap.OnItemsSourcePropertyChangedEvent += (oldValue, newValue) =>
            {
                Assert.AreEqual(oldValue, itemsOld);
                Assert.AreEqual(newValue, itemsNew);
                Assert.AreNotEqual(newValue, oldValue);
                Assert.AreNotEqual(newValue, oldValue);
                Assert.IsFalse(calledItemsSourceProperty);
                calledItemsSourceProperty = true;
            };
            treeMap.SetValue(TreeMap.ItemsSourceProperty, itemsNew);
            Assert.IsTrue(calledItemsSourceProperty);

            // Test InterpolatorsPropertyChanged
            Collection<Interpolator> collectionNew = new Collection<Interpolator> { new SolidColorBrushInterpolator() };

            bool calledOnInterpolatorsPropertyChangedEvent = false;
            treeMap.OnInterpolatorsPropertyChangedEvent += (oldValue, newValue) =>
            {
                Assert.AreEqual(oldValue, collectionOld);
                Assert.AreEqual(newValue, collectionNew);
                Assert.IsFalse(calledOnInterpolatorsPropertyChangedEvent);
                calledOnInterpolatorsPropertyChangedEvent = true;
            };
            treeMap.SetValue(TreeMap.InterpolatorsProperty, collectionNew);
            Assert.IsTrue(calledOnInterpolatorsPropertyChangedEvent);
        }

        /// <summary>
        /// Verifies we can bind DataMaximum and DataMinimum in the interpolator.
        /// </summary>
        [TestMethod, Asynchronous]
        [Description("Verifies we can bind DataMaximum and DataMinimum in the interpolator.")]
        [Bug("711854: Interpolators - From, To, DataMaximum and DataMinimum to DPs", Fixed = true)]
        public void InterpolatorsMinMaxBindingTreeMapTest()
        {
            DoubleInterpolator testInterpolator = new DoubleInterpolator
            {
                From = 11,
                To = 20,
                DataRangeBinding = new Binding("Points"),
                TargetName = "textB",
                TargetProperty = "FontSize"
            };

            testInterpolator.SetBinding(DoubleInterpolator.DataMinimumProperty, new Binding("Key"));
            testInterpolator.SetBinding(DoubleInterpolator.DataMaximumProperty, new Binding("Value"));

            TreeMap testTreeMap = InterpolatedTreeMapToTest;

            testTreeMap.Interpolators.Clear();

            // Tuple can't be used as it is internal
            testTreeMap.DataContext = new KeyValuePair<double, double>(15.0, 20.0);

            testTreeMap.Interpolators.Add(testInterpolator);

            TestAsync(
            testTreeMap,
            () => testTreeMap.ItemsSource = InterpolatedTreeMapItemsSource,
            () => Assert.AreEqual(20, testInterpolator.ActualDataMaximum),
            () => Assert.AreEqual(15, testInterpolator.ActualDataMinimum));
        }
    }
}
