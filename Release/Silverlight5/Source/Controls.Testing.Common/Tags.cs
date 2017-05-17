// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Silverlight.Testing;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Defined tag constants.
    /// </summary>
    public static class Tags
    {
        /// <summary>
        /// Defines a tag name used to identify tests that require the control
        /// to have focus in order to pass.
        /// </summary>
        public const string RequiresFocus = "RequiresFocus";

        /// <summary>
        /// Defines a tag name used to identify tests that generate mouse and
        /// keyboard input.
        /// </summary>
        public const string RequiresInput = "RequiresInput";

        /// <summary>
        /// Defines a tag name used to identify tests that use the visual
        /// verification.
        /// </summary>
        public const string VisualVerificationTest = "VisualVerification";
    }
}