// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A value converter for collapsing or showing elements based on the bound
    /// object's type name. Does not walk the hierarchy - it is explicit to the
    /// most specific class for the value. This class,
    /// InvertedTypeNameVisibilityConverter, offers the opposite behavior of the
    /// TypeNameVisibilityConverter class.
    /// </summary>
    public class InvertedTypeNameVisibilityConverter : TypeNameVisibilityConverter
    {
        /// <summary>
        /// Gets a value indicating whether the visibility value should be
        /// inverted.
        /// </summary>
        protected override bool IsInverted { get { return true; } }
    }
}