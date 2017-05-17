// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.Silverlight.Testing;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata.VisualStudio;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Provide custom metadata for dependency property tests.
    /// </summary>
    public sealed class DependencyPropertyTestMethod : TestMethod
    {
        /// <summary>
        /// Gets the name of the property being tested.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets or sets an additional modifier for the property to be used in
        /// the name.
        /// </summary>
        public string PropertyDetail { get; set; }

        /// <summary>
        /// Gets the name of the test method.
        /// </summary>
        public override string Name
        {
            get
            {
                string format = PropertyDetail == null ?
                    "[DP {1}] {0}" :
                    "[DP {1}] {0} ({2})";
                return string.Format(CultureInfo.InvariantCulture, format, base.Name, PropertyName, PropertyDetail);
            }
        }

        /// <summary>
        /// Gets the test action to perform.
        /// </summary>
        public Action Test { get; private set; }

        /// <summary>
        /// Gets a list of dynamically defined bug attributes.
        /// </summary>
        public IList<BugAttribute> Bugs { get; private set; }

        /// <summary>
        /// Gets a list of dynamically defined tag attributes.
        /// </summary>
        public IList<TagAttribute> Tags { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DependencyPropertyTestMethod
        /// class.
        /// </summary>
        /// <param name="methodInfo">The method being tested.</param>
        /// <param name="propertyName">
        /// The name of the property being tested.
        /// </param>
        /// <param name="test">The test action to perform.</param>
        public DependencyPropertyTestMethod(MethodInfo methodInfo, string propertyName, Action test)
            : base(methodInfo)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            else if (test == null)
            {
                throw new ArgumentNullException("test");
            }

            PropertyName = propertyName;
            Test = test;

            // Initialize the collections of dynamic attributes
            Bugs = new List<BugAttribute>();
            Tags = new List<TagAttribute>();

            // Add an implicit tag for the property name
            Tags.Add(new TagAttribute(propertyName));
        }

        /// <summary>
        /// Get any attribute on the test method that are provided dynamically.
        /// </summary>
        /// <returns>
        /// Dynamically provided attributes on the test method.
        /// </returns>
        public override IEnumerable<Attribute> GetDynamicAttributes()
        {
            foreach (BugAttribute bug in Bugs)
            {
                yield return bug;
            }

            foreach (TagAttribute tag in Tags)
            {
                yield return tag;
            }
        }

        /// <summary>
        /// Invoke the test.
        /// </summary>
        /// <param name="instance">Instance of the test class.</param>
        public override void Invoke(object instance)
        {
            Test();
        }

        /// <summary>
        /// Associate a bug with the test method.
        /// </summary>
        /// <param name="description">Description of the bug.</param>
        /// <param name="isFixed">
        /// A value indicating whether or not the bug has been fixed.
        /// </param>
        /// <returns>The DependencyPropertyTestMethod.</returns>
        public DependencyPropertyTestMethod Bug(string description, bool isFixed)
        {
            Bugs.Add(new BugAttribute(description) { Fixed = isFixed });
            return this;
        }

        /// <summary>
        /// Associate a bug with the test method.
        /// </summary>
        /// <param name="description">Description of the bug.</param>
        /// <returns>The DependencyPropertyTestMethod.</returns>
        public DependencyPropertyTestMethod Bug(string description)
        {
            return Bug(description, false);
        }

        /// <summary>
        /// Associate a tag with the test method.
        /// </summary>
        /// <param name="tag">The tag to associate.</param>
        /// <returns>The DependencyPropertyTestMethod.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Justification = "Useful for a fluent API")]
        public DependencyPropertyTestMethod Tag(string tag)
        {
            Tags.Add(new TagAttribute(tag));
            return this;
        }
    }
}