// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of Viewbox.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Viewbox", Justification = "Consistency with WPF")]
    public interface IOverriddenViewbox : IOverriddenControl
    {
    }
}