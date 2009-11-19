// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// This attribute is used to mark a Timestamp member of a Type. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class TimestampAttribute : Attribute {
    }
}
