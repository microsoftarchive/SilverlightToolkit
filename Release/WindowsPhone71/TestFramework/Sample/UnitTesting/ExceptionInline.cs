// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using Microsoft.Phone.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.UnitTesting
{
  /// <summary>
  /// A simple test class.
  /// </summary>
  [TestClass]
  public class ExceptionInline : WorkItemTest
  {
    /// <summary>
    /// A method that always fails.
    /// </summary>
    [TestMethod]
    [Description("A test method that always fails.")]
    public void AFailingTest()
    {
      Assert.IsFalse(true);
    }

    /// <summary>
    /// A method with a wrong expected exception type.
    /// </summary>
    [TestMethod]
    [Description("A test method with wrong expected exception.")]
    [ExpectedException(typeof(NotSupportedException))]
    public void WrongExceptionType()
    {
      throw new InvalidOperationException("The requested operation is not currently supported.");
    }

    /// <summary>
    /// A method with inner exception.
    /// </summary>
    [TestMethod]
    [Description("A test method with inner exception.")]
    [ExpectedException(typeof(Exception))]
    public void Inline1()
    {
      throw new InvalidOperationException("Something could be wrong but this is a negative test!", new ArgumentNullException("This is an argument null exception that is wrapped by an IOE."));
    }

    /// <summary>
    /// An asynchronous test method with exception.
    /// </summary>
    [TestMethod, Asynchronous]
    [Description("An asynchronous test method with exception.")]
    [ExpectedException(typeof(Exception))]
    public void InlineAsync1()
    {
      throw new ArgumentNullException();
    }

    /// <summary>
    /// An asynchronous test method with exception on enqueue task.
    /// </summary>
    [TestMethod, Asynchronous]
    [Description("An asynchronous test method with exception on enqueue task")]
    [ExpectedException(typeof(Exception))]
    [Tag("Asy")]
    public void InAsyncLine1()
    {
      EnqueueCallback(() => { throw new InvalidOperationException(); });
    }

    /// <summary>
    /// An asynchronous test method.
    /// </summary>
    [TestMethod, Asynchronous]
    [Description("An asynchronous test method.")]
    [ExpectedException(typeof(Exception))]
    [Tag("Asy")]
    public void InAsyncLine2()
    {
      EnqueueCallback(() => { throw new InvalidOperationException(); });
      EnqueueTestComplete();
    }

  }

  /// <summary>
  /// A more elaborate test class using the Bug test attribute.
  /// </summary>
  [TestClass]
  public class ExceptionInlineBugs : WorkItemTest
  {
    /// <summary>
    /// An empty method with bugs.
    /// </summary>
    [TestMethod]
    [Description("An empty method with bugs.")]
    [Bug("Bug 132534: Description goes here.")]
    [Bug("Bug 5: Description goes here.", Fixed = true)]
    [Bug("Bug 4: Description goes here.")]
    [Bug("Bug 3: Description goes here.")]
    [Bug("Bug 2: Description goes here.")]
    [Bug("Bug 1: Description goes here.")]
    [ExpectedException(typeof(Exception))]
    public void SuperBuggyMethod()
    {
    }

    /// <summary>
    /// A method with bugs that throws an exception.
    /// </summary>
    [TestMethod]
    [Description("A method with bugs that throws an exception.")]
    [Bug("Bug 132534: Description goes here.")]
    [Bug("Bug 5: Description goes here.", Fixed = true)]
    [Bug("Bug 4: Description goes here.")]
    [Bug("Bug 3: Description goes here.")]
    [Bug("Bug 2: Description goes here.")]
    [Bug("Bug 1: Description goes here.")]
    [ExpectedException(typeof(Exception))]
    public void SuperBuggyMethod2()
    {
      throw new Exception();
    }

    /// <summary>
    /// A failing method with exception and bug.
    /// </summary>
    [TestMethod]
    [Description("A failing method with exception and bug")]
    [Bug("This is a bug")]
    public void Inline1()
    {
      throw new InvalidOperationException();
    }

    /// <summary>
    /// Fails with bugs and expects an exception.
    /// </summary>
    [TestMethod]
    [Description("Fails with bugs and expects an exception.")]
    [Bug("There was an expected exception but is a bug...")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ExpectedExceptionBug()
    {
    }

    /// <summary>
    /// Asynchronous test method with exception.
    /// </summary>
    [TestMethod, Asynchronous]
    [Description("Asynchronous test method with exception.")]
    [Bug("This is a bug")]
    [Tag("Asy")]
    public void InlineAsync1()
    {
      throw new InvalidOperationException();
    }

    /// <summary>
    /// Asynchronous test method with exception on enqueue task.
    /// </summary>
    [TestMethod, Asynchronous]
    [Bug("This is a bug")]
    [Description("Asynchronous test method with exception on enqueue task.")]
    [Tag("Asy")]
    public void InAsyncLine1()
    {
      EnqueueCallback(() => { throw new InvalidOperationException(); });
    }

    /// <summary>
    /// Asynchronous test methods with exception on enqueue task.
    /// </summary>
    [TestMethod, Asynchronous]
    [Bug("This is a bug")]
    [Description("Asynchronous test methods with exception on enqueue task.")]
    [Tag("Asy")]
    public void InAsyncLine2()
    {
      EnqueueCallback(() => { throw new InvalidOperationException(); });
      EnqueueTestComplete();
    }

  }
}