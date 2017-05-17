// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// RatingItem unit tests.
    /// </summary>
    [TestClass]
    [Tag("RatingTest")]
    public class RatingItemTest : ContentControlTest
    {
        #region ContentControls to test
        /// <summary>
        /// Gets a default instance of RatingItem (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override ContentControl DefaultContentControlToTest
        {
            get
            {
                return DefaultRatingItemToTest;
            }
        }

        /// <summary>
        /// Gets instances of RatingItem (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<ContentControl> ContentControlsToTest
        {
            get
            {
                return RatingItemsToTest.OfType<ContentControl>();
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest
        {
            get
            {
                return OverriddenRatingItemsToTest.OfType<IOverriddenContentControl>();
            }
        } 
        #endregion ContentControls to test

        #region RatingItems to test
        /// <summary>
        /// Gets a default instance of RatingItem (or a derived type) to test.
        /// </summary>
        public virtual RatingItem DefaultRatingItemToTest
        {
            get { return new RatingItem(); }
        }

        /// <summary>
        /// Gets instances of RatingItem (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<RatingItem> RatingItemsToTest
        {
            get
            {
                yield return DefaultRatingItemToTest;

                yield return new RatingItem();
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenRatingItem (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenControl> OverriddenRatingItemsToTest
        {
            get { yield break; }
        }
        #endregion RatingItems to test

        /// <summary>
        /// Gets the DisplayValue dependency property test.
        /// </summary>
        protected DependencyPropertyTest<RatingItem, double> DisplayValueProperty { get; private set; }

        /// <summary>
        /// Gets the IsReadOnly dependency property test.
        /// </summary>
        protected DependencyPropertyTest<RatingItem, bool> IsReadOnlyProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingItemTest"/> class.
        /// </summary>
        public RatingItemTest()
        {
            Func<RatingItem> initializer = () => DefaultRatingItemToTest;

            DisplayValueProperty =
                new DependencyPropertyTest<RatingItem, double>(this, "DisplayValue")
                {
                    Initializer = initializer,
                    Property = RatingItem.DisplayValueProperty,
                    DefaultValue = 0.0,
                    OtherValues = new[] { 1.0, 0.5 }
                };

            IsReadOnlyProperty = new DependencyPropertyTest<RatingItem, bool>(this, "IsReadOnly")
            {
                Property = RatingItem.IsReadOnlyProperty,
                Initializer = () => DefaultRatingItemToTest,
                DefaultValue = false,
                OtherValues = new bool[] { true }
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

            tests.Add(IsEnabledProperty.ChangesVisualStateTest(false, true, "Normal"));
            tests.Add(IsEnabledProperty.ChangesVisualStateTest(true, false, "Disabled"));
            tests.Add(IsEnabledProperty.DoesNotChangeVisualStateTest(true, true));
            tests.Add(IsEnabledProperty.DoesNotChangeVisualStateTest(false, false));

            tests.Add(IsReadOnlyProperty.IsReadOnlyTest);

            BorderBrushProperty.DefaultValue = new SolidColorBrush(Color.FromArgb(0xff, 0x54, 0x54, 0x54));

            // reactivate these if possible
            tests.RemoveTests(ForegroundProperty.CheckDefaultValueTest);
            tests.RemoveTests(ForegroundProperty.ClearValueResetsDefaultTest);
            tests.RemoveTests(HorizontalContentAlignmentProperty.CheckDefaultValueTest);
            tests.RemoveTests(HorizontalContentAlignmentProperty.ClearValueResetsDefaultTest);
            tests.RemoveTests(VerticalContentAlignmentProperty.CheckDefaultValueTest);
            tests.RemoveTests(VerticalContentAlignmentProperty.ClearValueResetsDefaultTest);

            // DisplayValue tests
            tests.Add(DisplayValueProperty.CheckDefaultValueTest);
            tests.Add(DisplayValueProperty.IsReadOnlyTest);
            return tests;
        }

        #region control contract
        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultRatingItemToTest.GetType().GetVisualStates();
            Assert.AreEqual(10, states.Count, "Incorrect number of template states");
            Assert.AreEqual("CommonStates", states["Normal"], "Failed to find expected state Normal!");
            Assert.AreEqual("CommonStates", states["MouseOver"], "Failed to find expected state MouseOver!");
            Assert.AreEqual("CommonStates", states["Pressed"], "Failed to find expected state Pressed!");
            Assert.AreEqual("CommonStates", states["Disabled"], "Failed to find expected state Disabled!");
            Assert.AreEqual("CommonStates", states["ReadOnly"], "Failed to find expected state ReadOnly!");
            Assert.AreEqual("FocusStates", states["Focused"], "Failed to find expected state Focused!");
            Assert.AreEqual("FocusStates", states["Unfocused"], "Failed to find expected state Unfocused!");
            Assert.AreEqual("FillStates", states["Empty"], "Failed to find expected state ExpandDown!");
            Assert.AreEqual("FillStates", states["Partial"], "Failed to find expected state ExpandUp!");
            Assert.AreEqual("FillStates", states["Filled"], "Failed to find expected state ExpandLeft!");
        }
        #endregion control contract
    }
}