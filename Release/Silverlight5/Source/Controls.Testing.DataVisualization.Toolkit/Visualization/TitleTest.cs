// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the Title class.
    /// </summary>
    [TestClass]
    public partial class TitleTest : ContentControlTest
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override ContentControl DefaultContentControlToTest
        {
            get { return new Title(); }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<ContentControl> ContentControlsToTest
        {
            get { yield return DefaultContentControlToTest; }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest
        {
            get { yield break; }
        }

        /// <summary>
        /// Initializes a new instance of the TitleTest class.
        /// </summary>
        public TitleTest()
        {
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            return TagInherited(base.GetDependencyPropertyTests());
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        [TestMethod]
        [Description("Creates a new instance.")]
        public void NewInstance()
        {
            Title title = new Title();
            Assert.IsNotNull(title);
        }
    }
}
