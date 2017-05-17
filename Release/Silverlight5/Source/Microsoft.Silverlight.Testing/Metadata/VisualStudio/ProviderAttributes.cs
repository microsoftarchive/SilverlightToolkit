// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using VS = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata.VisualStudio
{
    /// <summary>
    /// The set of provider attributes.
    /// </summary>
    internal static class ProviderAttributes
    {
        /// <summary>
        /// Initializes the VSTT metadata provider.
        /// </summary>
        static ProviderAttributes()
        {
            TestClass = typeof(VS.TestClassAttribute);
            IgnoreAttribute = typeof(VS.IgnoreAttribute);
            ClassInitialize = typeof(VS.ClassInitializeAttribute);
            ClassCleanup = typeof(VS.ClassCleanupAttribute);
            TestInitialize = typeof(VS.TestInitializeAttribute);
            TestCleanup = typeof(VS.TestCleanupAttribute);
            DescriptionAttribute = typeof(VS.DescriptionAttribute);
            TimeoutAttribute = typeof(VS.TimeoutAttribute);
            OwnerAttribute = typeof(VS.OwnerAttribute);
            ExpectedExceptionAttribute = typeof(VS.ExpectedExceptionAttribute);
            AssemblyInitialize = typeof(VS.AssemblyInitializeAttribute);
            AssemblyCleanup = typeof(VS.AssemblyCleanupAttribute);
            TestMethod = typeof(VS.TestMethodAttribute);
            Priority = typeof(VS.PriorityAttribute);
            TestProperty = typeof(VS.TestPropertyAttribute);
        }

        /// <summary>
        /// Gets VSTT [TestClass] attribute.
        /// </summary>
        public static Type TestClass { get; private set; }
        
        /// <summary>
        /// Gets VSTT [Ignore] attribute.
        /// </summary>
        public static Type IgnoreAttribute { get; private set; }
        
        /// <summary>
        /// Gets VSTT [ClassInitialize] attribute.
        /// </summary>
        public static Type ClassInitialize { get; private set; }

        /// <summary>
        /// Gets VSTT [Priority] attribute.
        /// </summary>
        public static Type Priority { get; private set; }

        /// <summary>
        /// Gets VSTT [ClassCleanup] attribute.
        /// </summary>
        public static Type ClassCleanup { get; private set; }
        
        /// <summary>
        /// Gets VSTT [TestInitialize] attribute.
        /// </summary>
        public static Type TestInitialize { get; private set; }
        
        /// <summary>
        /// Gets VSTT [TestCleanup] attribute.
        /// </summary>
        public static Type TestCleanup { get; private set; }
        
        /// <summary>
        /// Gets VSTT [Description] attribute.
        /// </summary>
        public static Type DescriptionAttribute { get; private set; }
        
        /// <summary>
        /// Gets VSTT [Timeout] attribute.
        /// </summary>
        public static Type TimeoutAttribute { get; private set; }
        
        /// <summary>
        /// Gets VSTT [Owner] attribute.
        /// </summary>
        public static Type OwnerAttribute { get; private set; }
        
        /// <summary>
        /// Gets VSTT [ExpectedException] attribute.
        /// </summary>
        public static Type ExpectedExceptionAttribute { get; private set; }
        
        /// <summary>
        /// Gets VSTT [AssemblyInitialize] attribute.
        /// </summary>
        public static Type AssemblyInitialize { get; private set; }
        
        /// <summary>
        /// Gets VSTT [AssemblyCleanup] attribute.
        /// </summary>
        public static Type AssemblyCleanup { get; private set; }
        
        /// <summary>
        /// Gets VSTT [TestMethod] attribute.
        /// </summary>
        public static Type TestMethod { get; private set; }

        /// <summary>
        /// Gets VSTT [TestProperty] attribute.
        /// </summary>
        public static Type TestProperty { get; private set; }
    }
}