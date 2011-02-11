// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

// Spelling
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Firefox", Scope = "type", Target = "Microsoft.Silverlight.Testing.Tools.Firefox", Justification = "Firefox is spelled correctly.")]

// Unused code that might be used in a future release
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Silverlight.Testing.Tools.ServiceHelper.#Error(System.String)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Silverlight.Testing.Tools.ServiceHelper.#HtmlDecode(System.String)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Silverlight.Testing.Tools.ServiceHelper.#HtmlEncode(System.String)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Silverlight.Testing.Tools.ServiceHelper.#UrlDecode(System.String)")]

// Functionality that is still being implemented
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Silverlight.Testing.Tools.TestServiceEngine.#SaveResults(System.String,System.Net.HttpListenerRequest)")]