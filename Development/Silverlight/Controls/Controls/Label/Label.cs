// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents the text label for a control.
    /// </summary>
    /// <remarks>
    /// Label does not provide a Target property because Silverlight controls
    /// do not currently respect the concept of access keys.
    /// </remarks>
    /// <QualityBand>Stable</QualityBand>
    public partial class Label : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the Label class.
        /// </summary>
        public Label()
        {
            DefaultStyleKey = typeof(Label);
        }
    }
}