// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Set of defined types of log messages to allow for some level of 
    /// filtering and selective processing of log messages.
    /// </summary>
    public enum LogMessageType
    {
        /// <summary>
        /// Pass, fail, etc.
        /// </summary>
        TestResult,

        /// <summary>
        /// Debug::WriteLine or tracing.
        /// </summary>
        Debug,

        /// <summary>
        /// Non-fatal log message.
        /// </summary>
        Warning,

        /// <summary>
        /// Fatal error message.
        /// </summary>
        Error,
        
        /// <summary>
        /// Information about a known defect.
        /// </summary>
        KnownIssue,
        
        /// <summary>
        /// General information similar to a Console::WriteLine.
        /// </summary>
        Information,
        
        /// <summary>
        /// Operating system setting or platform values.
        /// </summary>
        Environment,
        
        /// <summary>
        /// New unit of test or test run note.
        /// </summary>
        TestExecution,
        
        /// <summary>
        /// Related test systems or out-of-process communication information.
        /// </summary>
        TestInfrastructure,
    }
}