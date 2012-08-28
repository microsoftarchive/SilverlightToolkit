// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using UnitTestHarness = Microsoft.Phone.Testing.Harness.UnitTestHarness;

namespace Microsoft.Phone.Testing.Metadata
{
  /// <summary>
  /// Test assembly metadata interface.
  /// </summary>
  public interface IAssembly
  {
    /// <summary>
    /// Gets the initialization method.
    /// </summary>
    MethodInfo AssemblyInitializeMethod { get; }

    /// <summary>
    /// Gets the cleanup method.
    /// </summary>
    MethodInfo AssemblyCleanupMethod { get; }

    /// <summary>
    /// Gets a collection of test class metadata objects.
    /// </summary>
    /// <returns>Returns a collection of metadata objects.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "From an API design standpoint, makes it clear that this method involves some level of work and is not trivial when the getter is called")]
    ICollection<ITestClass> GetTestClasses();

    /// <summary>
    /// Gets a reference to the unit test provider.
    /// </summary>
    IUnitTestProvider Provider { get; }

    /// <summary>
    /// Gets the name of the test assembly.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the test harness used to initialize the assembly.
    /// </summary>
    UnitTestHarness TestHarness { get; }
  }
}
