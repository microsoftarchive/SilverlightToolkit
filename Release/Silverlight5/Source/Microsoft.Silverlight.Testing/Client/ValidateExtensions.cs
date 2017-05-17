// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System.Windows;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A set of simple extension methods for applications.
    /// </summary>
    internal static class ValidateExtensions
    {
        /// <summary>
        /// Checks whether the application host object is not null.
        /// </summary>
        /// <param name="app">The application instance.</param>
        /// <returns>Returns a value indicating whether the object is not null.</returns>
        public static bool IfApplicationHost(this Application app)
        {
            return (app != null && 
                    app.Host != null);
        }

        /// <summary>
        /// Checks whether the application host and its source object is not
        /// null.
        /// </summary>
        /// <param name="app">The application instance.</param>
        /// <returns>Returns a value indicating whether the object is not null.</returns>
        public static bool IfApplicationHostSource(this Application app)
        {
            return (app.IfApplicationHost() &&
                app.Host.Source != null);
        }
    }
}