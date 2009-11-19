// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// Types of items with associated coverage information.
    /// </summary>
    public enum CoverageItemType
    {
        /// <summary>
        /// An assembly with code coverage information.
        /// </summary>
        Assembly,

        /// <summary>
        /// A namespace with code coverage information.
        /// </summary>
        Namespace,

        /// <summary>
        /// A type with code coverage information.
        /// </summary>
        Type,

        /// <summary>
        /// A method with code coverage information.
        /// </summary>
        Method,

        /// <summary>
        /// A block with code coverage information.
        /// </summary>
        Block
    }
}