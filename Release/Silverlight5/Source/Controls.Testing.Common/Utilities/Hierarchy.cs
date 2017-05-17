// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// The Hierarchy type is used as a sample hierarchical data source.
    /// </summary>
    [ContentProperty("Children")]
    public class Hierarchy
    {
        /// <summary>
        /// Gets or sets the parent of the element.
        /// </summary>
        private Hierarchy Parent { get; set; }

        /// <summary>
        /// Gets or sets the element name.
        /// </summary>
        public string Element { get; set; }

        /// <summary>
        /// Gets or sets the children of the element.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Required for easy initialization")]
        public Collection<Hierarchy> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the Hierarchy class.
        /// </summary>
        public Hierarchy()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Hierarchy class.
        /// </summary>
        /// <param name="element">The element name.</param>
        public Hierarchy(string element)
        {
            Children = new Collection<Hierarchy>();
            Element = element;
        }

        /// <summary>
        /// Add a single child to the hierarchy.
        /// </summary>
        /// <param name="element">The element name.</param>
        /// <returns>A Hierarchy object to continue construction.</returns>
        public Hierarchy Item(string element)
        {
            Hierarchy item = new Hierarchy { Element = element, Parent = this };
            Children.Add(item);
            return this;
        }

        /// <summary>
        /// Add a new child to the hierarchy that will contain its own children.
        /// </summary>
        /// <param name="element">The element name.</param>
        /// <returns>A Hierarchy object to continue construction.</returns>
        public Hierarchy Items(string element)
        {
            Hierarchy item = new Hierarchy { Element = element, Parent = this };
            Children.Add(item);
            return item;
        }

        /// <summary>
        /// Finish adding children to this element.
        /// </summary>
        /// <returns>A Hierarchy object to continue construction.</returns>
        public Hierarchy EndItems()
        {
            if (Parent == null)
            {
                throw new InvalidOperationException("EndItems must correspond to a call to Items!");
            }

            return Parent;
        }

        /// <summary>
        /// Associate a binding to the last element added.
        /// </summary>
        /// <param name="element">
        /// Variable to bind to the last element added.
        /// </param>
        /// <returns>A Hierarchy object to continue construction.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Necessary to create a binding.")]
        public Hierarchy Named(out Hierarchy element)
        {
            Hierarchy last = Children.Count > 0 ? Children[Children.Count - 1] : this;
            if (last == null)
            {
                throw new InvalidOperationException("You must add a Hierarchy element before calling Named!");
            }

            element = last;
            return this;
        }

        /// <summary>
        /// Get a HierarchicalDataTemplate that can be used to bind a Hierarchy.
        /// </summary>
        /// <returns>
        /// A HierarchicalDataTemplate that can be used to bind a Hierarchy.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not used like a property.")]
        public static HierarchicalDataTemplate GetDataTemplate()
        {
            string templateKey = "template";

            // Wrap the template in a Grid because the XAML parser doesn't like
            // namespaces declared on a DataTemplate
            XamlBuilder<Grid> builder = new XamlBuilder<Grid>
            {
                ExplicitNamespaces = new Dictionary<string, string>
                {
                    { "common", XamlBuilder.GetNamespace(typeof(HierarchicalDataTemplate)) },
                    { "ctl", XamlBuilder.GetNamespace(typeof(TreeViewItem)) },
                    { "sys", XamlBuilder.GetNamespace(typeof(int)) }
                },
                ElementProperties = new Dictionary<string, XamlBuilder>
                {
                    {
                        "Resources",
                        new XamlBuilder<HierarchicalDataTemplate>
                        {
                            Key = templateKey,
                            AttributeProperties = new Dictionary<string, string>
                            {
                                { "ItemsSource", "{Binding Children}" }
                            },
                            Children = new List<XamlBuilder>
                            {
                                new XamlBuilder<ContentControl>
                                {
                                    Name = "TemplateContent",
                                    AttributeProperties = new Dictionary<string, string>
                                    {
                                        { "Content", "{Binding Element}" },
                                        { "Foreground", "Red" }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Grid container = builder.Load();
            return container.Resources[templateKey] as HierarchicalDataTemplate;
        }
    }
}