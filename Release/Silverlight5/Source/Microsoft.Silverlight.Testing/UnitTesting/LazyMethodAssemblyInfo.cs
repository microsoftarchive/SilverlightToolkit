// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Reflection;
using Microsoft.Silverlight.Testing.Harness;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// A lazy method type.
    /// </summary>
    public class LazyAssemblyMethodInfo : LazyMethodInfo
    {
        /// <summary>
        /// Underlying Assembly reflection object.
        /// </summary>
        private Assembly _assembly;

        /// <summary>
        /// Create a new lazy method from a MethodInfo instance.
        /// </summary>
        /// <param name="assembly">Assembly reflection object.</param>
        /// <param name="attributeType">Attribute Type instance.</param>
        public LazyAssemblyMethodInfo(Assembly assembly, Type attributeType) : base(attributeType)
        {
            _assembly = assembly;
        }

        /// <summary>
        /// Performs a search on the MethodInfo for the attributes needed.
        /// </summary>
        protected override void Search()
        {
            if (HasSearched)
            {
                return;
            }
            if (_assembly == null)
            {
                return;
            }
            Type[] types = _assembly.GetExportedTypes();
            foreach (Type type in types)
            {
                MethodInfo = ReflectionUtility.GetOneMethodWithAttribute(type, AttributeType);

                if (MethodInfo != null)
                {
                    break;
                }
            }
            HasSearched = true;
        }
    }
}