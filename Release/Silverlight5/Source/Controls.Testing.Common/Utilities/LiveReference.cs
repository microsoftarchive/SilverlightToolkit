// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Silverlight.Testing;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// LiveReference is used to track elements that are added to the testing
    /// surface canvas so they can automatically be removed once the test is
    /// complete.
    /// </summary>
    public sealed class LiveReference : IDisposable
    {
        /// <summary>
        /// Reference to the executing test class.
        /// </summary>
        private TestBase _test;

        /// <summary>
        /// Gets the element added to the testing surface canvas.
        /// </summary>
        public UIElement Element { get; private set; }

        /// <summary>
        /// Initializes a new instance of the LiveReference class.
        /// </summary>
        private LiveReference()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LiveReference class.
        /// </summary>
        /// <param name="test">Reference to the executing test class.</param>
        /// <param name="element">Element to add to the testing surface.</param>
        public LiveReference(TestBase test, UIElement element)
        {
            _test = test;

            Assert.IsNotNull(_test);
            Assert.IsNotNull(_test.TestPanel);

            Element = element;
            _test.TestPanel.Children.Add(Element);
        }

        /// <summary>
        /// Remove the element from the testing surface canvas when finished.
        /// </summary>
        public void Dispose()
        {
            Assert.IsNotNull(Element);
            Assert.IsNotNull(_test);
            Assert.IsNotNull(_test.TestPanel);
            bool removed = _test.TestPanel.Children.Remove(Element);
            Assert.IsTrue(removed);
        }
    }
}