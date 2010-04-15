// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Stores options for the test service.
    /// </summary>
    public class TestServiceOptions
    {
        /// <summary>
        /// The default host to use.
        /// </summary>
        private const string DefaultMachineName = "localhost";

        /// <summary>
        /// The default port to listen on.
        /// </summary>
        private const int DefaultPort = 8000;

        /// <summary>
        /// The default delay between receiving the final results and shutting
        /// down.
        /// </summary>
        private readonly TimeSpan DefaultShutdownDelay = TimeSpan.FromSeconds(1.5);

        /// <summary>
        /// Gets or sets the host to listen on.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the port to listen on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the delay to apply between the final results being
        /// reported and the test server being shutdown.
        /// </summary>
        public TimeSpan ShutdownDelay { get; set; }

        /// <summary>
        /// Initializes a new instance of TestServiceOptions.
        /// </summary>
        public TestServiceOptions() : this(DefaultMachineName, DefaultPort)
        {
        }

        /// <summary>
        /// Initializes a new instance of TestServiceOptions.
        /// </summary>
        /// <param name="machineName">The host to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        public TestServiceOptions(string machineName, int port)
        {
            ShutdownDelay = DefaultShutdownDelay;
            MachineName = machineName;
            Port = port;
        }
    }
}