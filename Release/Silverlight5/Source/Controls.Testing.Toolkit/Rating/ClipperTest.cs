// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Clipper unit tests.
    /// </summary>
    public abstract class ClipperTest : ContentControlTest
    {
        /// <summary>
        /// Gets a default instance of ContentControl (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override ContentControl DefaultContentControlToTest
        {
            get { return DefaultClipperToTest; }
        }

        /// <summary>
        /// Gets instances of ContentControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<ContentControl> ContentControlsToTest
        {
            get { return ClippersToTest.OfType<ContentControl>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest
        {
            get { return OverriddenClippersToTest.OfType<IOverriddenContentControl>(); }
        }

        #region Clippers to test
        /// <summary>
        /// Gets a default instance of Clipper (or a derived type) to test.
        /// </summary>
        public abstract Clipper DefaultClipperToTest
        {
            get;
        }

        /// <summary>
        /// Gets instances of Clipper (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<Clipper> ClippersToTest
        {
            get;
        }

        /// <summary>
        /// Gets instances of IOverriddenClipper (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenControl> OverriddenClippersToTest
        {
            get { yield break; }
        }
        #endregion Clippers to test

        /// <summary>
        /// Gets the RatioVisible dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Clipper, double> RatioVisibleProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ClipperTest"/> class.
        /// </summary>
        protected ClipperTest()
        {
            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Left;
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Top;

            Func<Clipper> initializer = () => DefaultClipperToTest;

            RatioVisibleProperty = new DependencyPropertyTest<Clipper, double>(this, "RatioVisible")
                                     {
                                         Property = Clipper.RatioVisibleProperty,
                                         Initializer = initializer,
                                         DefaultValue = 1.0,
                                         OtherValues = new[] { 0.0 }
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

            // Percentage tests
            tests.Add(RatioVisibleProperty.CheckDefaultValueTest);
            tests.Add(RatioVisibleProperty.ChangeClrSetterTest);
            tests.Add(RatioVisibleProperty.ChangeSetValueTest);
            tests.Add(RatioVisibleProperty.ClearValueResetsDefaultTest);

            return tests;
        }
    }
}
