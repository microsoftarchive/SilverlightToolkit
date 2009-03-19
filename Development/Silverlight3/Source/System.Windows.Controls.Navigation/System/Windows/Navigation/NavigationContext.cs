//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Represents the state of a navigation operation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class NavigationContext
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uri">Navigation operation URI value.</param>
        /// <param name="queryString">Collection of query string values.</param>
        internal NavigationContext(Uri uri, IDictionary<string, string> queryString)
        {
            this.Uri = uri;
            this.QueryString = queryString;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value of the navigation URI.
        /// </summary>
        public Uri Uri
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a collection of query string values.
        /// </summary>
        public IDictionary<string, string> QueryString
        {
            get;
            private set;
        }

        #endregion Properties
    }
}
