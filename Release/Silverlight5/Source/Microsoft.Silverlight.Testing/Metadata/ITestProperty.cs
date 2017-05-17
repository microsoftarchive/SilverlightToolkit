// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata
{
    /// <summary>
    /// A property for a test method.
    /// </summary>
    public interface ITestProperty
    {
        /// <summary>
        /// Gets the test property name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the test property value.
        /// </summary>
        string Value { get; }
    }
}