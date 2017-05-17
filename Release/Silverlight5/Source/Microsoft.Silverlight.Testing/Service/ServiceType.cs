// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using Microsoft.Silverlight.Testing.Harness;

// NOTE: This entire namespace and its implementation is likely to be replaced
// in the future.

namespace Microsoft.Silverlight.Testing.Service
{
    /// <summary>
    /// The type of test service in use.  Used by the more advanced service 
    /// scenarios in SilverlightTestServiceProvider.
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// No service, or unknown service type.
        /// </summary>
        None,

        /// <summary>
        /// A direct connection, be it the file system, isolated storage, or 
        /// similar.
        /// </summary>
        Direct,

        /// <summary>
        /// A web service.
        /// </summary>
        WebService,
    }
}