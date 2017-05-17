// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// A log provider that outputs failures as a string.
    /// </summary>
    public partial class TextFailuresLogProvider : LogProvider
    {
        /// <summary>
        /// A list of failing results.
        /// </summary>
        private List<ScenarioResult> _failures;

        /// <summary>
        /// Initializes a new TextFailuresLogProvider instance.
        /// </summary>
        public TextFailuresLogProvider()
            : base()
        {
            _failures = new List<ScenarioResult>();
            RegisterLogHandlers();
        }

        /// <summary>
        /// Retrieve the text log of all failures in the unit test run.
        /// </summary>
        /// <returns>Returns the log contents.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is more appropriate.")]
        public string GetFailuresLog()
        {
            StringBuilder s = new StringBuilder();
            s.Append(_failures.Count.ToString(CultureInfo.InvariantCulture));
            s.AppendLine(" non-passing results:");
            s.AppendLine();
            foreach (ScenarioResult result in _failures)
            {
                s.AppendLine(result.ToString());
            }
            return s.ToString();
        }
        
        /// <summary>
        /// Register the handler conditions of interest to this log provider.
        /// </summary>
        private void RegisterLogHandlers()
        {
            RegisterConditionalHandler(UnitTestMessageConditional.HasUnitTestOutcome, ProcessResult);
        }

        /// <summary>
        /// Process a UTF result message.
        /// </summary>
        /// <param name="logMessage">The log message object.</param>
        private void ProcessResult(LogMessage logMessage)
        {
            if (logMessage.HasDecorator(UnitTestLogDecorator.TestMethodMetadata))
            {
                ScenarioResult sr = (ScenarioResult)logMessage[UnitTestLogDecorator.ScenarioResult];
                if (sr.Result != TestOutcome.Passed)
                {
                    _failures.Add(sr);
                }
            }
        }
    }
}