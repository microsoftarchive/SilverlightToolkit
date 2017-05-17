// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "Rating", Scope = "member", Target = "System.Windows.Controls.Testing.RatingTest.#GetRatingItem(System.Windows.Controls.Rating,System.Object)", Justification = "Temporary changes.")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "item", Scope = "member", Target = "System.Windows.Controls.Testing.RatingTest.#GetRatingItem(System.Windows.Controls.Rating,System.Object)", Justification = "Temporary changes.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "item", Scope = "member", Target = "System.Windows.Controls.Testing.RatingTest.#ShouldCreateRatingItems()", Justification = "Temporary changes.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "item", Scope = "member", Target = "System.Windows.Controls.Testing.RatingTest.#ShouldUpdateItemsWithExpandDirection()", Justification = "Temporary changes.")]

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Rating unit tests.
    /// </summary>
    [TestClass]
    public class RatingTest : ItemsControlTest
    {
        /// <summary>
        /// Gets a default instance of FrameworkElement (or a derived type) to
        /// test.
        /// </summary>
        /// <value></value>
        public override FrameworkElement DefaultFrameworkElementToTest
        {
            get
            {
                return new Rating();
            }
        }

        /// <summary>
        /// Gets a default instance of ItemsControl (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override ItemsControl DefaultItemsControlToTest
        {
            get { return new Rating(); }
        }

        /// <summary>
        /// Gets instances of ItemsControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<ItemsControl> ItemsControlsToTest
        {
            get { return RatingsToTest.OfType<ItemsControl>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenItemsControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenItemsControl> OverriddenItemsControlsToTest
        {
            get { yield break; }
        }

        /// <summary>
        /// Gets instances of Rating (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<Rating> RatingsToTest
        {
            get
            {
                yield return DefaultRatingToTest;
            }
        }

        /// <summary>
        /// Gets the default Rating to test.
        /// </summary>
        /// <value>The default Rating to test.</value>
        public virtual Rating DefaultRatingToTest
        {
            get
            {
                return new Rating();
            }
        }

        #region DependencyProperties
        /// <summary>
        /// Gets the SelectionMode dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Based on WPF MultiSelector.")]
        protected DependencyPropertyTest<Rating, RatingSelectionMode> SelectionModeProperty { get; private set; }

        /// <summary>
        /// Gets the ItemContainerStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Rating, Style> ItemContainerStyleProperty { get; private set; }

        #endregion DependencyProperties

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingTest"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Testing a complex control.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Testing a complex control.")]
        public RatingTest()
        {
            Func<Rating> initializer = () => new Rating();
            
            SelectionModeProperty = 
                new DependencyPropertyTest<Rating, RatingSelectionMode>(this, "SelectionMode")
                {
                     Property = Rating.SelectionModeProperty,
                     Initializer = initializer,
                     DefaultValue = RatingSelectionMode.Continuous,
                     OtherValues = 
                        new[]
                        { 
                            RatingSelectionMode.Individual
                        },
                     InvalidValues = 
                        new Dictionary<RatingSelectionMode, Type>
                        {
                            { (RatingSelectionMode)99, typeof(ArgumentOutOfRangeException) },
                            { (RatingSelectionMode)66, typeof(ArgumentOutOfRangeException) }
                        }
                };

            ItemContainerStyleProperty = 
                new DependencyPropertyTest<Rating, Style>(this, "ItemContainerStyle")
                {
                    Property = Rating.ItemContainerStyleProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = 
                        new[] 
                        {
                            new Style(typeof(HeaderedItemsControl)), 
                            new Style(typeof(ItemsControl)), 
                            new Style(typeof(Control)) 
                        }
                };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());
            
            // Remove certain tests that can't run because they compare on an instance
            tests.RemoveTests(tests.Where(t => t.Name == ItemsPanelProperty.CheckDefaultValueTest.Name).First());
            tests.RemoveTests(tests.Where(t => t.Name == ItemsPanelProperty.ClearValueResetsDefaultTest.Name).First());
            tests.RemoveTests(BackgroundProperty.CheckDefaultValueTest);
            tests.RemoveTests(BackgroundProperty.ClearValueResetsDefaultTest);
            tests.RemoveTests(ItemsSourceProperty.CheckDefaultValueTest);
            tests.RemoveTests(ItemsSourceProperty.ClearValueResetsDefaultTest);

            // SelectionMode tests
            tests.Add(SelectionModeProperty.CheckDefaultValueTest);
            tests.Add(SelectionModeProperty.ChangeSetValueTest);

            // Remove foreground property
            tests.RemoveTests(ForegroundProperty.CheckDefaultValueTest);
            tests.RemoveTests(ForegroundProperty.ClearValueResetsDefaultTest);
            tests.RemoveTests(BorderBrushProperty.CheckDefaultValueTest);
            tests.RemoveTests(BorderBrushProperty.ClearValueResetsDefaultTest);
            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        public override void StyleTypedPropertiesAreDefined()
        {
            Assert.AreEqual(1, DefaultRatingToTest.GetType().GetStyleTypedProperties().Count);
        }
        #endregion

        /// <summary>
        /// Tests that rating items have the correct value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that rating items have the correct value.")]
        public virtual void TestRatingItemsHaveCorrectValue()
        {
            Rating rating = DefaultRatingToTest;
            rating.ItemCount = 5;
            TestAsync(
                rating,
                () => rating.Value = 0.2,
                () => Test.Assert(() => GetRatingItems(rating).First().DisplayValue == 1.0),
                () => Test.Assert(() => GetRatingItems(rating).Take(2).Last().DisplayValue == 0.0));
        }

        /// <summary>
        /// Tests removing rating item causes values to update.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests removing rating item causes values to update.")]
        public virtual void TestRemovingRatingItemCausesValuesToUpdate()
        {
            Rating rating = DefaultRatingToTest;
            rating.ItemCount = 4;

            TestAsync(
                rating,
                () => rating.Value = 0.26,
                () => Test.Assert(() => GetRatingItems(rating).First().DisplayValue == 1.0),
                () => rating.ItemCount = 3,
                () => Test.Assert(() => GetRatingItems(rating).First().DisplayValue < 1.0));
        }

        /// <summary>
        /// Tests adding rating item causes values to update.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests adding rating item causes values to update.")]
        public virtual void TestAddingRatingItemCausesValuesToUpdate()
        {
            Rating rating = DefaultRatingToTest;
            rating.ItemCount = 3;
            TestAsync(
                rating,
                () => rating.Value = 0.26,
                () => Test.Assert(() => GetRatingItems(rating).First().DisplayValue < 1.0),
                () => rating.ItemCount = 4,
                () => Test.Assert(() => GetRatingItems(rating).First().DisplayValue == 1.0));
        }

        /// <summary>
        /// Tests setting value to null sets all display values to zero.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests setting value to null sets all display values to zero.")]
        public virtual void TestSettingValueToNullSetsAllDisplayValuesToZero()
        {
            Rating rating = DefaultRatingToTest;
            rating.ItemCount = 5;
            rating.Value = 0.25;
            TestAsync(
                rating,
                () => rating.Value = null,
                () => Test.Assert(() => GetRatingItems(rating).All(ratingItem => ratingItem.DisplayValue == 0.0)));
        }

        /// <summary>
        /// Tests setting ItemCount property changes Items collection to 
        /// appropriate size.
        /// </summary>
        [TestMethod]
        [Description("Tests setting ItemCount property changes Items collection to appropriate size.")]
        public virtual void TestSettingItemCountChangesItemsCollectionToCorrectSize()
        {
            Rating rating = DefaultRatingToTest;

            rating.ItemCount = 5;
            Test.Assert(() => rating.Items.Count == rating.ItemCount);
            rating.ItemCount = 0;
            Test.Assert(() => rating.Items.Count == rating.ItemCount);
        }

        /// <summary>
        /// Tests setting ItemCount property changes ItemsSource collection to 
        /// appropriate size.
        /// </summary>
        [TestMethod]
        [Description("Tests setting ItemCount property changes ItemsSource collection to appropriate size.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void TestSettingItemCountChangesItemsSourceToCorrectSize()
        {
            Rating rating = DefaultRatingToTest;

            ObservableCollection<object> collection = new ObservableCollection<object>();
            rating.ItemsSource = collection;

            rating.ItemCount = 4;
        }

        /// <summary>
        /// Setting ItemCount to a negative number throws an argument exception.
        /// </summary>
        [TestMethod]
        [Description("Setting ItemCount to a negative number throws an argument exception.")]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void SettingItemCountToNegativeNumberThrowsArgumentException()
        {
            Rating rating = DefaultRatingToTest;
            rating.ItemCount = -1;
        }

        /// <summary>
        /// Setting items using item source succeeds.
        /// </summary>
        [TestMethod]
        [Description("Setting items using item source succeeds.")]
        public virtual void SettingItemsUsingItemsSourceSucceeds()
        {
            Rating rating = DefaultRatingToTest;
            rating.ItemsSource = new[] { 1, 2, 3 };
            Test.Assert(() => rating.Items.Count == 3);
        }

        /// <summary>
        /// Setting items using items collection.
        /// </summary>
        [TestMethod]
        [Description("Setting items using items collection.")]
        public virtual void SettingItemsUsingItemsCollectionSucceeds()
        {
            Rating rating = DefaultRatingToTest;
            rating.Items.Add(new RatingItem());
            rating.Items.Add(new RatingItem());
            rating.Items.Add(new RatingItem());
            Test.Assert(() => rating.Items.Count == 3);
        }

        /// <summary>
        /// Setting items using item count property.
        /// </summary>
        [TestMethod]
        [Description("Setting items using item count property.")]
        public virtual void SettingItemsUsingItemCountSucceeds()
        {
            Rating rating = DefaultRatingToTest;
            rating.ItemCount = 3;
            Test.Assert(() => rating.Items.Count == 3);
        }

        #region helper methods
        /// <summary>
        /// Gets the Rating items.
        /// </summary>
        /// <param name="Rating">The Rating.</param>
        /// <returns>The RatingItems that wrap the items.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is called in an expression and is therefore not caught by the static analyzer.")]
        private static IList<RatingItem> GetRatingItems(Rating Rating)
        {
            return Enumerable.Range(0, Rating.Items.Count).Select(index => Rating.ItemContainerGenerator.ContainerFromIndex(index)).OfType<RatingItem>().ToList();
        }
        #endregion helper methods

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultRatingToTest.GetType().GetVisualStates();
            Assert.AreEqual(7, states.Count, "Incorrect number of template states");
            Assert.AreEqual("CommonStates", states["Normal"], "Failed to find expected state Normal!");
            Assert.AreEqual("CommonStates", states["MouseOver"], "Failed to find expected state MouseOver!");
            Assert.AreEqual("CommonStates", states["Pressed"], "Failed to find expected state Pressed!");
            Assert.AreEqual("CommonStates", states["Disabled"], "Failed to find expected state Disabled!");
            Assert.AreEqual("CommonStates", states["ReadOnly"], "Failed to find expected state ReadOnly!");
            Assert.AreEqual("FocusStates", states["Focused"], "Failed to find expected state Focused!");
            Assert.AreEqual("FocusStates", states["Unfocused"], "Failed to find expected state Unfocused!");
        }
    }
}