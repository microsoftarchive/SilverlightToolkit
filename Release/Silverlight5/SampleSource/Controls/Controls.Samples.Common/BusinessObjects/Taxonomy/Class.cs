// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a Class in a Linnaean taxonomy.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Class", Justification = "Will only be used from a C# sample.")]
    public sealed class Class : Taxonomy
    {
        /// <summary>
        /// Initializes a new instance of the Class class.
        /// </summary>
        public Class()
            : base()
        {
        }
    }
}