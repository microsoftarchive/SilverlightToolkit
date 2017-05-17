// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Globalization;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata.VisualStudio
{
    /// <summary>
    /// A simple wrapper for a priority integer value that overrides the 
    /// ToString method.
    /// </summary>
    public class Priority : IPriority
    {
        /// <summary>
        /// Gets the priority value.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Priority class.
        /// </summary>
        /// <param name="priority">The priority.</param>
        public Priority(int priority)
        {
            Value = priority;
        }

        /// <summary>
        /// Gets the priority as string.
        /// </summary>
        /// <returns>Returns the priority.</returns>
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}