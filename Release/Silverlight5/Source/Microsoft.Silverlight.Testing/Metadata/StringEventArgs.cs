// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata
{
    /// <summary>
    /// Event arguments that pass along a string value.
    /// </summary>
    public class StringEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new event argument instance.
        /// </summary>
        public StringEventArgs() { }

        /// <summary>
        /// Create a new event argument instance that stores a string value.
        /// </summary>
        /// <param name="value">String value to pass along.</param>
        public StringEventArgs(string value) : this()
        {
            _value = value;
        }

        /// <summary>
        /// String value stored in the event arguments.
        /// </summary>
        private string _value;

        /// <summary>
        /// Gets the stored string value.
        /// </summary>
        public string Value
        {
            get { return _value; }
        }
    }
}