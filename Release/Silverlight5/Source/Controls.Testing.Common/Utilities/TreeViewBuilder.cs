// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Utility that facilitates building a TreeView via simple method chaining.
    /// </summary>
    public sealed partial class TreeViewBuilder
    {
        /// <summary>
        /// Gets or sets the parent ItemsControl Container.
        /// </summary>
        private ItemsControl ParentContainer { get; set; }

        /// <summary>
        /// Gets or sets the parent TreeViewBuilder.
        /// </summary>
        private TreeViewBuilder ParentBuilder { get; set; }

        /// <summary>
        /// Gets the last TreeViewItem to have been added.
        /// </summary>
        private TreeViewItem LastTreeViewItem
        {
            get
            {
                ItemsControl parent = ParentContainer;
                object last = (parent.Items.Count > 0) ?
                    parent.Items[parent.Items.Count - 1] :
                    parent;
                return last as TreeViewItem;
            }
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewBuilder class.
        /// </summary>
        public TreeViewBuilder()
            : this(new TreeView())
        {
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewBuilder class.
        /// </summary>
        /// <param name="view">The TreeView to build.</param>
        public TreeViewBuilder(TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            ParentContainer = view;
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewBuilder class.
        /// </summary>
        /// <param name="item">The TreeViewItem to build.</param>
        public TreeViewBuilder(TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            ParentContainer = item;
        }

        /// <summary>
        /// Add a TreeViewItem to the parent TreeView.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        private void Add(TreeViewItem item)
        {
            ParentContainer.Items.Add(item);
        }

        /// <summary>
        /// Add a new TreeViewItem with no nested Items.
        /// </summary>
        /// <param name="header">The header of the TreeViewItem.</param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder Item(object header)
        {
            return Item(new TreeViewItem { Header = header });
        }

        /// <summary>
        /// Add a new TreeViewItem with no nested Items.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Justification = "Consistency with other overloads")]
        public TreeViewBuilder Item(TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Add(item);
            return this;
        }

        /// <summary>
        /// Add a new TreeViewItem with nested Items (which will be comprised of
        /// any new Item or Items calls that are chained before EndItems).
        /// </summary>
        /// <param name="header">The header of the TreeViewItem.</param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder Items(object header)
        {
            return Items(new TreeViewItem { Header = header });
        }

        /// <summary>
        /// Add a new TreeViewItem with nested Items (which will be comprised of
        /// any new Item or Items calls that are chained before EndItems).
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder Items(TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Add(item);
            return new TreeViewBuilder
                {
                    ParentBuilder = this,
                    ParentContainer = item
                };
        }

        /// <summary>
        /// Complete a sequence of nested Items.
        /// </summary>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder EndItems()
        {
            if (ParentBuilder == null)
            {
                throw new InvalidOperationException("EndItems must correspond to a call to Items!");
            }

            return ParentBuilder;
        }

        /// <summary>
        /// Set various properties on the last TreeViewItem added.
        /// </summary>
        /// <param name="setter">Action that sets the properties.</param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder Set(Action<TreeViewItem> setter)
        {
            TreeViewItem last = LastTreeViewItem;
            if (last == null)
            {
                throw new InvalidOperationException("You must add a TreeViewItem before calling Set!");
            }

            setter(last);
            return this;
        }

        /// <summary>
        /// Select the last TreeViewItem added.
        /// </summary>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder Select()
        {
            return Select(true);
        }

        /// <summary>
        /// Select the last TreeViewItem added.
        /// </summary>
        /// <param name="selected">
        /// A value indicating whether the last TreeViewItem added is selected.
        /// </param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder Select(bool selected)
        {
            TreeViewItem last = LastTreeViewItem;
            if (last == null)
            {
                throw new InvalidOperationException("You must add a TreeViewItem before calling Select!");
            }

            last.IsSelected = selected;
            return this;
        }

        /// <summary>
        /// Expand the last TreeViewItem added.
        /// </summary>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder Expand()
        {
            return Expand(true);
        }

        /// <summary>
        /// Expand the last TreeViewItem added.
        /// </summary>
        /// <param name="expanded">
        /// A value indicating whether the last TreeViewItem added is expanded.
        /// </param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        public TreeViewBuilder Expand(bool expanded)
        {
            TreeViewItem last = LastTreeViewItem;
            if (last == null)
            {
                throw new InvalidOperationException("You must add a TreeViewItem before calling Expand!");
            }

            last.IsExpanded = expanded;
            return this;
        }

        /// <summary>
        /// Associate a binding to the last TreeViewItem added.
        /// </summary>
        /// <param name="item">
        /// Variable to bind to the last TreeViewItem added.
        /// </param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Necessary to create a binding.")]
        public TreeViewBuilder Named(out TreeViewItem item)
        {
            TreeViewItem last = LastTreeViewItem;
            if (last == null)
            {
                throw new InvalidOperationException("You must add a TreeViewItem before calling Named!");
            }

            item = last;
            return this;
        }

        /// <summary>
        /// Implicitly cast a TreeViewBuilder to a TreeView.
        /// </summary>
        /// <param name="builder">The TreeViewBuilder.</param>
        /// <returns>The TreeView.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not necessary for test utility.")]
        public static implicit operator TreeView(TreeViewBuilder builder)
        {
            return (builder != null) ?
                builder.ParentContainer as TreeView :
                null;
        }

        /// <summary>
        /// Implicitly cast a TreeViewBuilder to a TreeViewItem.
        /// </summary>
        /// <param name="builder">The TreeViewBuilder.</param>
        /// <returns>The TreeViewItem.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not necessary for test utility.")]
        public static implicit operator TreeViewItem(TreeViewBuilder builder)
        {
            return (builder != null) ?
                builder.ParentContainer as TreeViewItem :
                null;
        }

        /// <summary>
        /// Implicitly cast a TreeView to a TreeViewBuilder.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not necessary for test utility.")]
        public static implicit operator TreeViewBuilder(TreeView view)
        {
            return new TreeViewBuilder(view);
        }

        /// <summary>
        /// Implicitly cast a TreeViewItem to a TreeViewBuilder.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>A TreeViewBuilder to continue construction.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not necessary for test utility.")]
        public static implicit operator TreeViewBuilder(TreeViewItem item)
        {
            return new TreeViewBuilder(item);
        }
    }
}