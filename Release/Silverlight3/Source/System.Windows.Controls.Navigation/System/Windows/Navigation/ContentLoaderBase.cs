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
    /// ContentLoaderBase serves as the base class for all content loaders that plug into NavigationService.
    /// </summary>
    /// <remarks>ContentLoaderBase types should always generate and load content asychronously.</remarks>
    /// <seealso cref="PageResourceContentLoader"/>
    internal abstract class ContentLoaderBase
    {        
        #region Methods

        /// <summary>
        /// Begins asynchronous loading of the provided <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">A URI value to resolve and begin loading.</param>
        /// <param name="userCallback">A callback function that will be called when this asynchronous request is ready to have <see cref="EndLoad"/> called on it.</param>
        /// <param name="asyncState">A custom state object that will be returned in <see cref="IAsyncResult.AsyncState"/>, to correlate between multiple async calls.</param>
        /// <returns>An <see cref="IAsyncResult"/> that can be passed to <see cref="CancelLoad(IAsyncResult)"/> at any time, or <see cref="EndLoad(IAsyncResult)"/> after the <paramref name="userCallback"/> has been called.</returns>
        public abstract IAsyncResult BeginLoad(Uri uri, AsyncCallback userCallback, object asyncState);

        /// <summary>
        /// Attempts to cancel a pending load operation.
        /// </summary>
        /// <param name="asyncResult">The <see cref="IAsyncResult"/> returned from <see cref="BeginLoad(Uri,AsyncCallback,object)"/> for the operation you wish to cancel.</param>
        public abstract void CancelLoad(IAsyncResult asyncResult);

        /// <summary>
        /// Completes the asynchronous loading of content
        /// </summary>
        /// <param name="asyncResult">The result returned from <see cref="BeginLoad(Uri,AsyncCallback,object)"/>, and passed in to the callback function.</param>
        /// <returns>The content loaded, or null if content was not loaded</returns>
        public abstract object EndLoad(IAsyncResult asyncResult);

        #endregion Methods
    }
}
