//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a piece of navigable content
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class Page : UserControl
    {
        #region Constructors

        /// <summary>
        /// Constructs a default Page instance
        /// </summary>
        public Page()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the navigation request context.
        /// </summary>
        public NavigationContext NavigationContext
        {
            get { return JournalEntry.GetNavigationContext(this); }
        }

        /// <summary>
        /// Gets the <see cref="NavigationService"/> that navigated to this page
        /// </summary>
        public NavigationService NavigationService
        {
            get { return NavigationService.GetNavigationService(this); }
        }
                
        /// <summary>
        /// Gets or sets the title of this page, which may be shown in the browser window if this page
        /// is hosted in a browser-integrated <see cref="Frame"/>.  Leave this property null to indicate
        /// it should not affect the title.
        /// </summary>
        public string Title
        {
            get { return JournalEntry.GetName(this); }
            set { JournalEntry.SetName(this, value); }
        }

        #endregion Properties

        #region Methods

        internal void InternalOnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            this.OnFragmentNavigation(e);
        }

        internal void InternalOnNavigatedTo(NavigationEventArgs e)
        {
            this.OnNavigatedTo(e);
        }

        internal void InternalOnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.OnNavigatingFrom(e);
        }

        internal void InternalOnNavigatedFrom(NavigationEventArgs e)
        {
            this.OnNavigatedFrom(e);
        }

        /// <summary>
        /// This method is called when fragment navigation occurs on a page - either because a fragment
        /// was present in the original Uri that navigated to this page, or because a later fragment
        /// navigation occurs.
        /// </summary>
        /// <remarks>
        /// This should be used rather than signing up for this.NavigationService.FragmentNavigation
        /// because that event may be difficult to sign up for in time to get the first fragment navigation.
        /// </remarks>
        /// <param name="e">The event arguments, containing the fragment navigated to</param>
        protected virtual void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            return;
        }

        /// <summary>
        /// This method is called when a Page has been navigated to, and becomes the active Page in a Frame.
        /// This method is the preferred place to inspect this.NavigationContext, and react to load-time
        /// information and prepare the page for viewing.
        /// </summary>
        /// <remarks>
        /// This should be used rather than Loaded because Loaded signifies you have been added to the visual
        /// tree, but that could potentially happen more than once during a logical navigation event, in
        /// some advanced scenarios.  This method is guaranteed to be called only once when the Page becomes
        /// active.
        /// </remarks>
        /// <param name="e">The event arguments</param>
        protected virtual void OnNavigatedTo(NavigationEventArgs e)
        {
            return;
        }

        /// <summary>
        /// This method is called when a Page is about to be navigated away from.
        /// </summary>
        /// <remarks>
        /// This is similar to signing up for this.NavigationService.Navigating, but this method is preferred
        /// as then you do not need to remove the event handler from NavigationService to avoid object lifetime
        /// issues.
        /// </remarks>
        /// <param name="e">The event arguments.  If Cancel is set to true, it will cancel the pending operation that triggered this method call.</param>
        protected virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            return;
        }

        /// <summary>
        /// This method is called when a Page has been navigated away from, and is no longer the active
        /// Page in a Frame.  This is a good time to save dirty data or otherwise react to being
        /// inactive.
        /// </summary>
        /// <remarks>
        /// This is similar to signing up for this.NavigationService.Navigated, but this method is preferred
        /// as then you do not need to remove the event handler from NavigationService to avoid object lifetime
        /// issues.
        /// </remarks>
        /// <param name="e">The event arguments</param>
        protected virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
            return;
        }
        
        #endregion
    }
}
