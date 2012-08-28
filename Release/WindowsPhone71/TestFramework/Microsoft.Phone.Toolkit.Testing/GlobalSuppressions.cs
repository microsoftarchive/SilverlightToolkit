// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant", Justification = "The Silverlight presentation APIs are not CLS Compliant.")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.AssemblyManager.#FilterByFilterClassParameter(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Harness.ITestClass>&)", Justification = "ToUpperInvariant doesn't existing in the Silverlight BCL")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.IndexOf(System.String)", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.AssemblyManager.#FilterByFilterClassParameter(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Harness.ITestClass>&)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.LazyMethodInfo.#MethodInfo")]
[assembly: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.TestClassHelper.#FilterExclusiveClasses(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Metadata.ITestClass>&,Microsoft.Phone.Testing.Harness.LogMessageWriter)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.TestClassHelper.#FilterTestsToRun(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Metadata.ITestClass>&,System.Collections.Generic.ICollection`1<System.String>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.TestMethodHelper.#FilterExclusiveMethods(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Metadata.ITestMethod>&,Microsoft.Phone.Testing.Harness.LogMessageWriter)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.TestRunFilter.#FilterTestClasses(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Metadata.ITestClass>&)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.TestRunFilter.#FilterTestMethods(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Metadata.ITestMethod>&)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.TestRunFilter.#SortTestClasses(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Metadata.ITestClass>&)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.TestRunFilter.#SortTestMethods(System.Collections.Generic.List`1<Microsoft.Phone.Testing.UnitTesting.Metadata.ITestMethod>&)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Metadata.VisualStudio.ProviderAttributes.#TimeoutAttribute")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "#eee")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Bidi")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "#ddd")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Zindex")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "br")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "coords")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "classid")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "fieldset")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "fieldset's")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "noframe")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "noscript")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Runnable")]
[assembly: SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.UnitTestHarness.#Results")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.VisualStudioLogProvider+Writer.#AddPendingWriteLine(System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.Phone.Testing.UnitTesting.Harness.VisualStudioLogProvider+Writer.#IncrementResults(Microsoft.Phone.Testing.Harness.TestOutcome)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.VisualStudio.QualityTools.UnitTesting")]