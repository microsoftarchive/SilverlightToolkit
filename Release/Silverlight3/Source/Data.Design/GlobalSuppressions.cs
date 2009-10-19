// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Controls.Design.Common", Justification = "SL Toolkit compatibility.")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "System.Windows.Controls.Data.Design.MetadataRegistration.#AddAttributes(Microsoft.Windows.Design.Metadata.AttributeTableBuilder)", Justification = "No need to refactor. These are a bunch of attributes.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant", Justification = "Can't handle mixed references to both Silverlight and .NET.")]
