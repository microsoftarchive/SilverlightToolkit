//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Navigation
{
    /// <summary>
    /// Delegate for the Navigating event
    /// </summary>
    /// <param name="sender">The object sending this event</param>
    /// <param name="e">The event arguments</param>
    /// <QualityBand>Stable</QualityBand>
    public delegate void NavigatingCancelEventHandler(object sender, NavigatingCancelEventArgs e);

    /// <summary>
    /// Delegate for the NavigationFailed event
    /// </summary>
    /// <param name="sender">The object sending this event</param>
    /// <param name="e">The event arguments</param>
    /// <QualityBand>Stable</QualityBand>
    public delegate void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs e);

    /// <summary>
    /// Delegate for the Navigated event
    /// </summary>
    /// <param name="sender">The object sending this event</param>
    /// <param name="e">The event arguments</param>
    /// <QualityBand>Stable</QualityBand>
    public delegate void NavigatedEventHandler(object sender, NavigationEventArgs e);

    /// <summary>
    /// Delegate for the NavigationStopped event
    /// </summary>
    /// <param name="sender">The object sending this event</param>
    /// <param name="e">The event arguments</param>
    /// <QualityBand>Stable</QualityBand>
    public delegate void NavigationStoppedEventHandler(object sender, NavigationEventArgs e);

    /// <summary>
    /// Delegate for FragmentNavigation event
    /// </summary>
    /// <param name="sender">The object sending this event</param>
    /// <param name="e">The event arguments</param>
    /// <QualityBand>Stable</QualityBand>
    public delegate void FragmentNavigationEventHandler(object sender, FragmentNavigationEventArgs e);
}
