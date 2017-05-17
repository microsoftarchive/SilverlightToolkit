// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata
{
    /// <summary>
    /// Metadata representing a test class.
    /// </summary>
    public interface ITestClass
    {
        /// <summary>
        /// Gets the test class Type instance.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Retrieve a collection of the test method metadata objects setup by 
        /// the unit test provider.
        /// </summary>
        /// <returns>A collection of test method interfaces.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This call involves work including Reflection, naming it as such makes the non-trivial execution more clear to a developer from a discoverability standpoint")]
        ICollection<ITestMethod> GetTestMethods();

        /// <summary>
        /// Gets a value indicating whether the test class should be ignored.
        /// </summary>
        bool Ignore { get; }

        /// <summary>
        /// Gets the per-test initialization method.
        /// </summary>
        MethodInfo TestInitializeMethod { get; }

        /// <summary>
        /// Gets the per-test cleanup method.
        /// </summary>
        MethodInfo TestCleanupMethod { get; }

        /// <summary>
        /// Gets the one-time class initialization method.
        /// </summary>
        MethodInfo ClassInitializeMethod { get; }

        /// <summary>
        /// Gets the one-time class cleanup method.
        /// </summary>
        MethodInfo ClassCleanupMethod { get; }

        /// <summary>
        /// Gets the name of the test class.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the namespace of the test class.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Gets a reference to the parent test assembly metadata 
        /// instance.
        /// </summary>
        IAssembly Assembly { get; }
    }
}