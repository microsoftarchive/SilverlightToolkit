// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Set of metadata needed for each sample.
    /// </summary>
    public interface ISampleMetadata
    {
        /// <summary>
        /// Gets the Sample name.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the Sample DifficultyLevel.
        /// </summary>
        DifficultyLevel DifficultyLevel { get; }
        
        /// <summary>
        /// Gets the Sample category.
        /// </summary>
        string Category { get; }
    }
}