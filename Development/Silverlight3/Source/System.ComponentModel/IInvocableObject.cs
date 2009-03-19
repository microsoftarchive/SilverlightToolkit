//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace System.ComponentModel
{
    /// <summary>
    /// Defines a contract of an object that allows one to invoke a verb
    /// </summary>
    public interface IInvocableObject
    {
        /// <summary>
        /// Gets a value indicating whether a verb with specified name can be invoked
        /// </summary>
        /// <param name="name">name of the verb</param>
        /// <returns>true if the specified verb can be invoked</returns>
        bool CanInvoke(string name);

        /// <summary>
        /// Invokes the specified verb on the object
        /// </summary>
        /// <param name="verb">verb to be invoked</param>
        void Invoke(Verb verb);

        /// <summary>
        /// Gets a list of verb invocations
        /// </summary>
        IEnumerable<Verb> Invocations { get; }
    }
}
