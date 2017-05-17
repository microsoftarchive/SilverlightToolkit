// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// The XamlBuilder provides simplified programmatic creation of XAML
    /// markup for testing.
    /// </summary>
    public partial class XamlBuilder
    {
        /// <summary>
        /// Get the default Silverlight namespace.
        /// </summary>
        private const string DefaultNamespace = "http://schemas.microsoft.com/client/2007";

        /// <summary>
        /// Get the default XAML markup namespace.
        /// </summary>
        private const string MarkupNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

        /// <summary>
        /// Gets or sets the Type of the XAML element.
        /// </summary>
        public Type ElementType { get; set; }

        /// <summary>
        /// Gets or sets the name of the XAML element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the key of the XAML element.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the text content of the XAML element.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the XAML element is literal
        /// content.
        /// </summary>
        public bool IsLiteral { get; set; }

        /// <summary>
        /// Gets or sets a mapping of prefixes to namespaces that should be
        /// explicitly added.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Read/write to make test authoring easier")]
        public IDictionary<string, string> ExplicitNamespaces { get; set; }

        /// <summary>
        /// Gets or sets the attribute properties.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Read/write to make test authoring easier")]
        public IDictionary<string, string> AttributeProperties { get; set; }

        /// <summary>
        /// Gets or sets the attached attribute properties.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Read/write to make test authoring easier")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Acceptable for testing")]
        public IDictionary<KeyValuePair<Type, string>, string> AttachedAttributeProperties { get; set; }

        /// <summary>
        /// Gets or sets the element properties.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Read/write to make test authoring easier")]
        public IDictionary<string, XamlBuilder> ElementProperties { get; set; }

        /// <summary>
        /// Gets or sets the attached element properties.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Read/write to make test authoring easier")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Acceptable for testing")]
        public IDictionary<KeyValuePair<Type, string>, XamlBuilder> AttachedElementProperties { get; set; }

        /// <summary>
        /// Gets or sets the child elements.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Read/write to make test authoring easier")]
        public IList<XamlBuilder> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the XamlBuilder class.
        /// </summary>
        public XamlBuilder()
        {
        }

        /// <summary>
        /// Get the name of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Name of the assembly.</returns>
        private static string GetAssemblyName(Assembly assembly)
        {
            string name = assembly.FullName;
            int index = name.IndexOf(',');
            if (index > 0)
            {
                name = name.Substring(0, index);
            }
            return name;
        }

        /// <summary>
        /// Get the namespace for a Type.
        /// </summary>
        /// <param name="type">The Type to get a namespace for.</param>
        /// <returns>The namespace for a Type.</returns>
        public static string GetNamespace(Type type)
        {
            Debug.Assert(type != null, "Type should not be null!");
            return (type.Assembly != typeof(DependencyObject).Assembly) ?
                string.Format(CultureInfo.InvariantCulture, "clr-namespace:{0};assembly={1}", type.Namespace, GetAssemblyName(type.Assembly)) : 
                DefaultNamespace;
        }

        /// <summary>
        /// Convert the XamlBuilder tree into an XElement tree.
        /// </summary>
        /// <param name="root">
        /// A value indicating whether this element is the root of the XElement
        /// tree.
        /// </param>
        /// <returns>XElement tree representing the XAML.</returns>
        private XElement BuildElement(bool root)
        {
            Debug.Assert(!IsLiteral, "An element to build cannot be literal!");

            XElement element = new XElement(XName.Get(ElementType.Name, GetNamespace(ElementType)));
            if (root)
            {
                element.Add(new XAttribute(XName.Get("x", XNamespace.Xmlns.NamespaceName), MarkupNamespace));
            }

            if (!string.IsNullOrEmpty(Name))
            {
                element.Add(new XAttribute(XName.Get("Name", MarkupNamespace), Name));
            }
            if (!string.IsNullOrEmpty(Key))
            {
                element.Add(new XAttribute(XName.Get("Key", MarkupNamespace), Key));
            }

            if (ExplicitNamespaces != null)
            {
                foreach (KeyValuePair<string, string> namespacePair in ExplicitNamespaces)
                {
                    element.Add(new XAttribute(XName.Get(namespacePair.Key, XNamespace.Xmlns.NamespaceName), namespacePair.Value));
                }
            }

            if (AttributeProperties != null)
            {
                foreach (KeyValuePair<string, string> attributePair in AttributeProperties)
                {
                    element.Add(new XAttribute(attributePair.Key, attributePair.Value ?? "{x:Null}"));
                }
            }

            if (AttachedAttributeProperties != null)
            {
                foreach (KeyValuePair<KeyValuePair<Type, string>, string> attributePair in AttachedAttributeProperties)
                {
                    Type type = attributePair.Key.Key;
                    XName name = XName.Get(type.Name + "." + attributePair.Key.Value, GetNamespace(type));
                    element.Add(new XAttribute(name, attributePair.Value ?? "{x:Null}"));
                }
            }

            if (ElementProperties != null)
            {
                foreach (KeyValuePair<string, XamlBuilder> elementPair in ElementProperties)
                {
                    XName name = XName.Get(ElementType.Name + "." + elementPair.Key, GetNamespace(ElementType));
                    element.Add(BuildPropertyElement(name, elementPair.Value));
                }
            }

            if (AttachedElementProperties != null)
            {
                foreach (KeyValuePair<KeyValuePair<Type, string>, XamlBuilder> elementPair in AttachedElementProperties)
                {
                    Type type = elementPair.Key.Key;
                    XName name = XName.Get(type.Name + "." + elementPair.Key.Value, GetNamespace(type));
                    element.Add(BuildPropertyElement(name, elementPair.Value));
                }
            }

            if (Children != null)
            {
                foreach (XamlBuilder child in Children)
                {
                    element.Add(child.BuildElement(false));
                }
            }

            if (Content != null)
            {
                element.SetValue(Content);
            }

            return element;
        }

        /// <summary>
        /// Convert the XamlBuilder property into an XElement tree.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>XElement tree representing the property.</returns>
        private static XElement BuildPropertyElement(XName name, XamlBuilder value)
        {
            XElement wrapper = new XElement(name);
            if (value != null)
            {
                if (!value.IsLiteral)
                {
                    wrapper.Add(value.BuildElement(false));
                }
                else
                {
                    wrapper.SetValue(value.Content);
                }
            }
            return wrapper;
        }

        /// <summary>
        /// Create the XAML corresponding to the XamlBuilder tree.
        /// </summary>
        /// <returns>The XAML corresponding to the XamlBuilder tree.</returns>
        public string Build()
        {
            return BuildElement(true).ToString();
        }

        /// <summary>
        /// Load the object corresponding to the XamlBuilder tree.
        /// </summary>
        /// <returns>The object corresponding to the XamlBuilder tree.</returns>
        public object Load()
        {
            return XamlReader.Load(Build());
        }
    }
}