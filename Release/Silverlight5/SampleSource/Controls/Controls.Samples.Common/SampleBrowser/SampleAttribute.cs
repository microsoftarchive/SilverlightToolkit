// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// The SampleAttribute identifies sample name and level of difficulty.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]    
    public sealed partial class SampleAttribute : ExportAttribute, ISampleMetadata
    {
        /// <summary>
        /// Gets the path to the sample in the Sample Browser tree.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the difficulty level of the sample.
        /// </summary>
        public DifficultyLevel DifficultyLevel { get; private set; }

        /// <summary>
        /// Gets the category for this sample.
        /// </summary>
        public string Category { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SampleAttribute class.
        /// </summary>
        /// <param name="name">
        /// Name of the sample.
        /// </param>
        /// <param name="difficultyLevel">
        /// Difficulty Level of the sample.
        /// </param>
        /// <param name="category">
        /// Category of the sample.
        /// </param>
        public SampleAttribute(string name, DifficultyLevel difficultyLevel, string category)
            : base("Sample", typeof(FrameworkElement))
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "name should not be empty!");
            Name = name;
            DifficultyLevel = difficultyLevel;
            Category = category;
        }
    }
}