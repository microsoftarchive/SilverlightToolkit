//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Enum describing the NavigationMode (New, Back, Forward)
    /// where New means a new navigation, Forward, and Back mean 
    /// the navigation was initiated from the GoForward or 
    /// GoBack methods on <see cref="System.Windows.Controls.Frame"/>.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "NavigationMode derives from byte in WPF, so it should do so in Silverlight as well.")]
    public enum NavigationMode : byte
    {
        /// <summary>
        /// New navigation.
        /// </summary>
        New = 0,

        /// <summary>
        /// Navigating back in history.
        /// </summary>
        Back = 1,

        /// <summary>
        /// Navigating forward in history.
        /// </summary>
        Forward = 2
    }
}
