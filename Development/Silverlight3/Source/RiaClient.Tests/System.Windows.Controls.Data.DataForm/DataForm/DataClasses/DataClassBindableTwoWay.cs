//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataClassBindableTwoWay.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// A basic data class for testing the <see cref="DataForm"/>.
    /// </summary>
    [Bindable(true, BindingDirection.TwoWay)]
    public class DataClassBindableTwoWay : DataClass
    {
    }
}
