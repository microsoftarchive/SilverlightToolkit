// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Describes the level of difficulty of the sample.
    /// </summary>
    public enum DifficultyLevel 
    {
        /// <summary>
        /// Used when Sample does not need to be categorized.
        /// </summary>
        None = 0,

        /// <summary>
        /// Used for basic samples.
        /// </summary>
        Basic = 1,

        /// <summary>
        /// Used for intermediate samples.
        /// </summary>
        Intermediate = 2,

        /// <summary>
        /// Used for advanced samples.
        /// </summary>
        Advanced = 3,

        /// <summary>
        /// Used for scenario samples.
        /// </summary>
        Scenario = 4
    }
}

