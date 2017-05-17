// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Silverlight.Testing.Service
{
    // This simple class allows us to remove a dependency on XLinq.

    /// <summary>
    /// A simple implementation similar to XElement.
    /// </summary>
    public class SimpleXElement
    {
        /// <summary>
        /// Backing field for child elements.
        /// </summary>
        private List<SimpleXElement> _children;

        /// <summary>
        /// Backing field for attributes.
        /// </summary>
        private Dictionary<string, string> _attributes;

        /// <summary>
        /// Backing field for the element name.
        /// </summary>
        private string _name;

        /// <summary>
        /// Backing field for optional XML namespace.
        /// </summary>
        private string _namespace;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        private SimpleXElement()
        {
            _children = new List<SimpleXElement>();
            _attributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        public SimpleXElement(string elementName) : this()
        {
            _name = elementName;
        }

        /// <summary>
        /// Initializes a new instance of the element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="ns">The XML namespace of the element.</param>
        public SimpleXElement(string elementName, string ns) : this(elementName)
        {
            _namespace = ns;
        }

        /// <summary>
        /// Initializes a new instance of the class using the reader as the
        /// current root of the element.
        /// </summary>
        /// <param name="newElementReader">The XmlReader instance.</param>
        private SimpleXElement(XmlReader newElementReader)
            : this()
        {
            ParseInternal(newElementReader, true);
        }

        /// <summary>
        /// Gets the element name.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Gets or sets the element text, if any.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Sets the value of a new child element.
        /// </summary>
        /// <param name="elementName">The element name.</param>
        /// <param name="value">The value of the new element.</param>
        public void SetElementValue(string elementName, string value)
        {
            SimpleXElement child = new SimpleXElement(elementName);
            child.Value = value;
            Add(child);
        }

        /// <summary>
        /// Sets the value of a new child element with an XML namespace value.
        /// </summary>
        /// <param name="elementName">The element name.</param>
        /// <param name="value">The value of the new element.</param>
        /// <param name="xmlNamespace">The XML namespace.</param>
        public void SetElementValue(string elementName, string value, string xmlNamespace)
        {
            SimpleXElement child = new SimpleXElement(elementName, xmlNamespace);
            child.Value = value;
            Add(child);
        }

        /// <summary>
        /// Sets the value of an attribute.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        public void SetAttributeValue(string attribute, string value)
        {
            _attributes[attribute] = value;
        }

        /// <summary>
        /// Sets the value of the element.
        /// </summary>
        /// <param name="value">The new string value.</param>
        public void SetValue(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets an attribute value.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <returns>Returns an instance of the attribute value.</returns>
        public string Attribute(string name)
        {
            if (_attributes.ContainsKey(name))
            {
                return _attributes[name];
            }

            return null;
        }

        /// <summary>
        /// Adds a child element to the simple element instance.
        /// </summary>
        /// <param name="child">The child element instance.</param>
        public void Add(SimpleXElement child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Returns descendants.
        /// </summary>
        /// <param name="elementName">The element name to look for.</param>
        /// <returns>Returns an enumeration of elements.</returns>
        public IEnumerable<SimpleXElement> Descendants(string elementName)
        {
            if (_children != null)
            {
                foreach (SimpleXElement child in _children)
                {
                    if (child.Name == elementName)
                    {
                        yield return child;
                    }

                    foreach (SimpleXElement descendent in child.Descendants(elementName))
                    {
                        yield return descendent;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new SimpleXElement.
        /// </summary>
        /// <param name="xml">XML content.</param>
        /// <returns>Returns a new instance of the element and children.</returns>
        public static SimpleXElement Parse(string xml)
        {
            SimpleXElement sxe = new SimpleXElement();
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                sxe.ParseInternal(reader, true);
            }
            return sxe;
        }

        /// <summary>
        /// Parses the current level with the XmlReader instance.
        /// </summary>
        /// <param name="reader">The reader instance.</param>
        /// <param name="isFirst">A value indicating whether this is the first
        /// parse. Actually this will always be true for now.</param>
        private void ParseInternal(XmlReader reader, bool isFirst)
        {
            bool wasFirst = false;
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (isFirst)
                        {
                            _name = reader.Name;
                            isFirst = false;
                            wasFirst = true;

                            // Read attributes
                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    _attributes.Add(reader.Name, reader.Value);
                                }
                            }
                            if (reader.IsEmptyElement)
                            {
                                return;
                            }
                        }
                        else
                        {
                            _children.Add(new SimpleXElement(reader));
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (!wasFirst)
                        {
                            return;
                        }
                        break;

                    case XmlNodeType.Text:
                        Value = reader.Value;
                        break;

                    default:
                        break;
                }
            }
            while (reader.Read());
        }

        /// <summary>
        /// Generates the string representation of the element and its tree.
        /// </summary>
        /// <returns>Returns the string representation of the element.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                IndentChars = "  ",
                Indent = true
            };
            using (XmlWriter w = XmlWriter.Create(sb, settings))
            {
                w.WriteStartDocument();
                WriteElement(w);
                w.WriteEndDocument();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes to the XML writer without document start and ends.
        /// </summary>
        /// <param name="w">The writer instance.</param>
        private void WriteElement(XmlWriter w)
        {
            if (_namespace == null)
            {
                w.WriteStartElement(Name);
            }
            else
            {
                w.WriteStartElement(Name, _namespace);
            }
            foreach (string attribute in _attributes.Keys)
            {
                w.WriteAttributeString(attribute, _attributes[attribute]);
            }
            if (Value != null)
            {
                w.WriteValue(Value);
            }
            foreach (SimpleXElement child in _children)
            {
                child.WriteElement(w);
            }
            w.WriteEndElement();
        }
    }
}