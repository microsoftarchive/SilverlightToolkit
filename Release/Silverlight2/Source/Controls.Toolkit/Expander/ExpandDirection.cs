// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls
{
    /// <summary>
    /// Specifies the direction in which an Expander control opens.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    /// <example>
    /// The following example shows how to set the ExpandDirection property of 
    /// the Expander control in XAML to open in the up direction.
    /// <code language="XAML">
    /// <![CDATA[
    /// <!--An Expander control with the ExpandDirection property set to Up-->
    /// <controls:Expander Name="MyExpander2" Background="Beige" HorizontalAlignment="Left" Header="Up Expander" ExpandDirection="Up" IsExpanded="True" Width="150">
    ///     <TextBlock TextWrapping="Wrap">
    ///     Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua
    ///     </TextBlock>
    /// </controls:Expander>
    /// ]]>
    /// </code>
    /// </example>
    public enum ExpandDirection
    {
        /// <summary>
        /// Expander will expand to the down direction.
        /// </summary>
        Down = 0,

        /// <summary>
        /// Expander will expand to the up direction.
        /// </summary>
        Up = 1,

        /// <summary>
        /// Expander will expand to the left direction.
        /// </summary>
        Left = 2,

        /// <summary>
        /// Expander will expand to the right direction.
        /// </summary>
        Right = 3,
    }
}
