// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

namespace Microsoft.Silverlight.Testing.Harness
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A dictionary that manages single instances of types for use across the 
    /// unit test system.
    /// </summary>
    public class TestClassInstanceDictionary : Dictionary<Type, object>
    {
        /// <summary>
        /// Returns the instance for a Type; if there is not an instance yet, 
        /// this will use Activator.CreateInstance to create it.
        /// </summary>
        /// <param name="type">The Type instance to retrieve.</param>
        /// <returns>Returns an instance of the Type.  Returns a new instance 
        /// if the Type has not yet been used.</returns>
        public object GetInstance(Type type)
        {
            if (!ContainsKey(type) || this[type] == null)
            {
                object instance = Activator.CreateInstance(type);
                this[type] = instance;
            }

            return this[type];
        }

        /// <summary>
        /// Remove any instance for the type.
        /// </summary>
        /// <param name="type">The Type of instance to remove.</param>
        public void ClearInstance(Type type)
        {
            if (ContainsKey(type)) 
            {
                Remove(type);
            }
        }
    }
}