// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata.VisualStudio
{
    /// <summary>
    /// Represents a simple test property with a key/value string pair.
    /// </summary>
    public class TestProperty : ITestProperty
    {
        /// <summary>
        /// Initializes a new instance of the TestProperty class.
        /// </summary>
        public TestProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestProperty class.
        /// </summary>
        /// <param name="name">The initial property name.</param>
        /// <param name="value">The initial property value.</param>
        public TestProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string Value { get; set; }
    }
}