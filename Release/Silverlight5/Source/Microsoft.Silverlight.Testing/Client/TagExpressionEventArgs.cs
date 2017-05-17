// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A class for storing event information relating to a user's selected
    /// tag expression for a test run.
    /// </summary>
    public class TagExpressionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the tag expression stored in the event arguments.
        /// </summary>
        public string TagExpression { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a tag expression has been set.
        /// </summary>
        public bool HasTagExpression
        {
            get { return string.IsNullOrEmpty(TagExpression); }
        }

        /// <summary>
        /// Initializes a new instance of the TagExpression event arguments
        /// class.
        /// </summary>
        /// <param name="tagExpression">The tag expression.</param>
        public TagExpressionEventArgs(string tagExpression) : base()
        {
            TagExpression = tagExpression;
        }
    }
}