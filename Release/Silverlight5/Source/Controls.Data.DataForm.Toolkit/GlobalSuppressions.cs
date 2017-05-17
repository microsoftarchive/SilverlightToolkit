//-----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Automation.Peers", Justification = "Exposing automation peers in conventional System.Windows.Automation.Peers namespace.")]
[assembly: SuppressMessage("Layout", "SWC3000:CanvasDoesNotRespectLayout", MessageId = "ControlTemplate<TargetType=ctl:DataForm>", Justification="Being reviewed with Dev10 Bug 665458.")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "e", Scope = "member", Target = "System.Windows.Controls.DataForm.#OnLabelPositionPropertyChanged(System.Windows.DependencyObject,System.Windows.DependencyPropertyChangedEventArgs)")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "item", Scope = "member", Target = "System.Windows.Controls.DataForm.#ItemsCount")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "System.Windows.Controls.DataForm.#ValidateItem(System.Boolean)")]
