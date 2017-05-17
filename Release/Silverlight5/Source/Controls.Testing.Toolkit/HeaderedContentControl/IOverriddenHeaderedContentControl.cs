// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of ContentControl.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
    public interface IOverriddenHeaderedContentControl : IOverriddenContentControl
    {
        /// <summary>
        /// Gets the OnHeaderChanged test actions.
        /// </summary>
        OverriddenMethod<object, object> HeaderChangedActions { get; }

        /// <summary>
        /// Gets the OnHeaderTemplateChanged test actions.
        /// </summary>
        OverriddenMethod<DataTemplate, DataTemplate> HeaderTemplateChangedActions { get; }
    }
}