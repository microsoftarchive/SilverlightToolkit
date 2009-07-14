// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using Microsoft.Windows.Design.Model;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Design.Common
{
    /// <summary>
    /// Empty default initializer to prevent Blend from adding its own default initialization. 
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as attribute value.")]
    internal class EmptyDefaultInitializer : DefaultInitializer
    {
    }
}