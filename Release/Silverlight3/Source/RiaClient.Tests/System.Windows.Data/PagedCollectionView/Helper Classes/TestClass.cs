//-----------------------------------------------------------------------
// <copyright file="TestClass.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace System.ComponentModel.UnitTests
{
    /// <summary>
    /// Class used for testing.
    /// </summary>
    public class TestClass
    {
        /// <summary>
        /// Gets or sets an integer property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "This is a property used only within these specific unit tests and will never be seen outside of C#.")]
        public int IntProperty { get; set; }

        /// <summary>
        /// Gets or sets a value for the inner class property.
        /// </summary>
        public InnerClass InnerClassProperty { get; set; }

        /// <summary>
        /// Gets or sets a string property.
        /// </summary>
        public string StringProperty { get; set; }

        /// <summary>
        /// Overriding the ToString to help when debugging
        /// </summary>
        /// <returns>String that shows both the property values</returns>
        public override string ToString()
        {
            return String.Format("Int: {0} String: {1}", this.IntProperty, this.StringProperty);
        }
    }

    public class InnerClass
    {
        /// <summary>
        /// Gets or sets an integer property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "This is a property used only within these specific unit tests and will never be seen outside of C#.")]
        public int InnerIntProperty { get; set; }

        /// <summary>
        /// Gets or sets a string property.
        /// </summary>
        public string InnerStringProperty { get; set; }
    }

    public class ListOfTestClasses : List<TestClass>
    {
        public ListOfTestClasses(IEnumerable<TestClass> source)
            : base(source)
        {
        }
    }
}
