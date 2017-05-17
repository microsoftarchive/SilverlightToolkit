// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.   

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.Theming;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the System.Windows.Controls.ResourceNotFoundException
    /// class.
    /// </summary>
    [TestClass]
    public class ResourceNotFoundExceptionTest
    {
        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundExceptionTest 
        /// class.
        /// </summary>
        public ResourceNotFoundExceptionTest()
        {
        }

        /// <summary>
        /// Tests constructor with a message.
        /// </summary>
        [TestMethod]
        [Description("Tests constructor with a message.")]
        public void TestsConstructorWithAMessage()
        {
            ResourceNotFoundException exception = new ResourceNotFoundException("message");
            Assert.AreSame("message", exception.Message);
        }

        /// <summary>
        /// Tests constructor with a message and uri.
        /// </summary>
        [TestMethod]
        [Description("Tests constructor with a message and uri.")]
        public void TestsConstructorWithAMessageAndUri()
        {
            Uri uri = new Uri("http://www.microsoft.com");

            ResourceNotFoundException exception = new ResourceNotFoundException("message", uri);
            Assert.AreSame("message", exception.Message);
            Assert.AreSame(uri, exception.Uri);
        }

        /// <summary>
        /// Tests constructor with a message and exception.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Exception will never be thrown.")]
        [Description("Tests constructor with a message and exception.")]
        public void TestsConstructorWithAMessageAndException()
        {
            Exception innerException = new Exception();
            ResourceNotFoundException exception = new ResourceNotFoundException("message", innerException);
            Assert.AreSame("message", exception.Message);
            Assert.AreSame(innerException, exception.InnerException);
        }

        /// <summary>
        /// Tests constructor with a message, uri, and exception.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Exception will never be thrown.")]
        [Description("Tests constructor with a message, uri, and exception.")]
        public void TestsConstructorWithAMessageUriAndException()
        {
            Exception innerException = new Exception();
            Uri uri = new Uri("http://www.microsoft.com");
            ResourceNotFoundException exception = new ResourceNotFoundException("message", uri, innerException);
            Assert.AreSame("message", exception.Message);
            Assert.AreSame(uri, exception.Uri);
            Assert.AreSame(innerException, exception.InnerException);
        }
    }
}