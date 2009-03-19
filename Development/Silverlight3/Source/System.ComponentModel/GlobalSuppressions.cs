// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Indexable", Scope = "type", Target = "System.ComponentModel.IIndexableCollection", Justification = "Spelling is correct.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Indexable", Scope = "type", Target = "System.ComponentModel.IIndexableCollection`1", Justification = "Spelling is correct.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Collections.ObjectModel", Justification = "ObservableCollection shipped in namespace System.Collections.ObjectModel.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "System.ComponentModel.IEditableCollection", Justification = "Framework interface.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "System.ComponentModel.IEditableCollection`1", Justification = "Framework interface.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Data")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "System.ComponentModel.IPagedCollection")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "System.Windows.Data.PagedCollectionView.#ProcessCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "System.Windows.Data.PagedCollectionView+SortFieldComparer`1+SortPropertyInfo.#GetValue(System.Object)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.Windows.Data.PropertyGroupDescription.#ctor(System.String,System.Windows.Data.IValueConverter)", Scope = "member", Target = "System.Windows.Data.PropertyGroupDescription.#.ctor(System.String,System.Windows.Data.IValueConverter,System.StringComparison)")]
