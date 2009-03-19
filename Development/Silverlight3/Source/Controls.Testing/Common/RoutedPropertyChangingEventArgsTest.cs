// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests for the RoutedPropertyChangingEventArgs type.
    /// </summary>
    [TestClass]
    public class RoutedPropertyChangingEventArgsTest
    {
        /// <summary>
        /// Sets the Cancel property to a Cancelable event arguments instance.
        /// </summary>
        [TestMethod]
        [Description("Sets the Cancel property to a Cancelable event arguments instance.")]
        public void Cancel()
        {
            RoutedPropertyChangingEventArgs<bool> args = new RoutedPropertyChangingEventArgs<bool>(null, true, false, true);
            Assert.IsTrue(args.IsCancelable);
            args.Cancel = true;
        }

        /// <summary>
        /// Sets the Cancel property to an event arguments instance that cannot 
        /// be cancelled.
        /// </summary>
        [TestMethod]
        [Description("Sets the Cancel property to an event arguments instance that cannot be cancelled.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryCancel()
        {
            RoutedPropertyChangingEventArgs<bool> args = new RoutedPropertyChangingEventArgs<bool>(null, true, false, false);
            Assert.IsFalse(args.IsCancelable);
            args.Cancel = true;
        }

        /// <summary>
        /// Create a new instance of the RoutedPropertyChangingEventArgs type.
        /// </summary>
        [TestMethod]
        [Description("Create a new instance of the RoutedPropertyChangingEventArgs type.")]
        public void Instantiate()
        {
            RoutedPropertyChangingEventArgs<bool> args = new RoutedPropertyChangingEventArgs<bool>(null, true, false, true);
            Assert.IsTrue(args.IsCancelable);
        }
    }
}