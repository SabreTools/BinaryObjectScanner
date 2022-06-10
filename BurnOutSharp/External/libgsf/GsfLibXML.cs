/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-libxml.c :
 *
 * Copyright (C) 2002-2006 Jody Goldberg (jody@gnome.org)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2.1 of the GNU Lesser General Public
 * License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using LibGSF.Input;
using LibGSF.Output;

namespace LibGSF
{
    #region Enums

    /// <summary>
    /// Controls the handling of character data within a parser node.
    /// </summary>
    public enum GsfXMLContent
    {
        /// <summary>
        /// Node has no cstr contents
        /// </summary>
        GSF_XML_NO_CONTENT = 0,

        /// <summary>
        /// Node has cstr contents
        /// </summary>
        GSF_XML_CONTENT,

        /// <summary>
        /// Node has contents that is shared with children
        /// </summary>
        GSF_XML_SHARED_CONTENT,

        /// <summary>
        /// Node is second or later occurrence
        /// </summary>
        GSF_XML_2ND
    }

    public enum GsfXMLOutState
    {
        GSF_XML_OUT_NOCONTENT,
        GSF_XML_OUT_CHILD,
        GSF_XML_OUT_CHILD_PRETTY,
        GSF_XML_OUT_CONTENT
    }

    #endregion

    #region Delegates

    public delegate void GsfXMLInExtDtor(GsfXMLIn xin, object old_state);

    public delegate bool GsfXMLInUnknownFunc(GsfXMLIn xin, string elem, string[] attrs);

    public delegate bool GsfXMLProbeFunc(string name, string prefix, string URI, int nb_namespaces, string[] namespaces, int nb_attributes, int nb_defaulted, string[] attributes);

    #endregion

    #region Classes

    // TODO: Figure out what the structure of this is
    public class GsfXMLBlob
    {

    }

    public class GsfXMLIn : GsfInput
    {
        #region Properties

        /// <summary>
        /// User data
        /// </summary>
        public object UserState { get; set; }

        /// <summary>
        /// The current node content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Current document being parsed
        /// </summary>
        public XmlDocument Doc { get; set; }

        /// <summary>
        /// Current node (not on the stack)
        /// </summary>
        public XmlNode Node { get; set; }

        public Stack<XmlNode> NodeStack { get; private set; }

        #endregion

        #region Internal Properties

        public GsfInput Input { get; set; }

        public Stack<string> ContentsStack { get; internal set; }

        public bool Initialized { get; internal set; }

        #endregion

        #region Functions

        /// <summary>
        /// Take the first node from <paramref name="doc"/> as the current node and call its start handler.
        /// </summary>
        /// <param name="new_state">Arbitrary content for the parser</param>
        public void PushState(XmlDocument doc, object new_state, GsfXMLInExtDtor dtor, string[] attrs)
        {
            if (doc == null)
                return;
            if (doc.DocumentType == null)
                return;

            // TODO: Figure out how to define Start here
            PushChild(doc.FirstChild, attrs, null);
        }

        public void StartElement(string name, string ns)
        {
            if ((name != null && Node.Name != name) || (ns != null && Node.NamespaceURI != ns))
                return;

            Node = Node.NextSibling;
        }

        public void EndElement()
        {
            if (Node.NodeType != XmlNodeType.EndElement)
                throw new XmlException();

            Node = Node.NextSibling;
        }

        /// <summary>
        /// This function will not be called when parsing an empty document.
        /// </summary>
        public void StartDocument()
        {
            Initialized = true;
            Node = Doc.FirstChild;
            NodeStack = new Stack<XmlNode>();
            ContentsStack = new Stack<string>();
        }

        public void EndDocument()
        {
            Content = null;
            if (Initialized)
            {
                NodeStack = null;
                Initialized = false;

                if (Node != Doc.DocumentType)
                    Console.Error.WriteLine("Document likely damaged.");
            }
        }

        #endregion

        #region Utilities

        private string NodeName(XmlNode node) => (node.Name != null) ? node.Name : "{catch all)}";

        private void PushChild(XmlNode node, string[] attrs, Action<GsfXMLIn, string[]> start)
        {
            if (node.InnerText != null)
            {
                if (Content.Length != 0)
                {
                    ContentsStack.Push(Content);
                    Content = new string('\0', 128);
                }
                else
                {
                    ContentsStack.Push(null);
                }
            }

            NodeStack.Push(Node);
            Node = node;

            start?.Invoke(this, attrs);
        }

        #endregion
    };

    public class GsfXMLInParser : XmlReader
    {
        #region Properties

        /// <summary>
        /// Parent GsfXMLIn for reading
        /// </summary>
        public GsfXMLIn Parent { get; internal set; }

        /// <summary>
        /// Current user state
        /// </summary>
        public GsfXMLIn UserState { get; internal set; }

        /// <summary>
        /// Internal reader instance for unhandled functionality
        /// </summary>
        private readonly XmlReader inst;

        #endregion

        #region Override Properties

        public override int AttributeCount => inst.AttributeCount;

        public override string BaseURI => inst.BaseURI;

        public override int Depth => inst.Depth;

        public override bool EOF => inst.EOF;

        public override bool IsEmptyElement => inst.IsEmptyElement;

        public override string LocalName => inst.LocalName;

        public override string NamespaceURI => inst.NamespaceURI;

        public override XmlNameTable NameTable => inst.NameTable;

        public override XmlNodeType NodeType => inst.NodeType;

        public override string Prefix => inst.Prefix;

        public override ReadState ReadState => inst.ReadState;

        public override string Value => inst.Value;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        public GsfXMLInParser(GsfXMLIn parent)
        {
            if (parent == null)
                return;

            Parent = parent;

            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.None,
                ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,
            };

            inst = Create(inputUri: null, settings);

            Parent.StartDocument();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfXMLInParser()
        {
            Parent.EndDocument();
        }

        #endregion

        #region XmlReader Custom Implementation

        /// <inheritdoc/>
        public override void ReadStartElement() => Parent.StartElement(null, null);

        /// <inheritdoc/>
        public override void ReadStartElement(string name) => Parent.StartElement(name, null);

        /// <inheritdoc/>
        public override void ReadStartElement(string localname, string ns) => Parent.StartElement(localname, ns);

        /// <inheritdoc/>
        public override void ReadEndElement() => Parent.EndElement();

        /// <inheritdoc/>
        public override string ReadElementContentAsString() => Parent.Node.InnerText;

        #endregion

        #region XmlReader Default Implementation

        /// <inheritdoc/>
        public override string GetAttribute(int i) => inst.GetAttribute(i);

        /// <inheritdoc/>
        public override string GetAttribute(string name) => inst.GetAttribute(name);

        /// <inheritdoc/>
        public override string GetAttribute(string name, string namespaceURI) => inst.GetAttribute(name, namespaceURI);

        /// <inheritdoc/>
        public override string LookupNamespace(string prefix) => inst.LookupNamespace(prefix);

        /// <inheritdoc/>
        public override bool MoveToAttribute(string name) => inst.MoveToAttribute(name);

        /// <inheritdoc/>
        public override bool MoveToAttribute(string name, string ns) => inst.MoveToAttribute(name, ns);

        /// <inheritdoc/>
        public override bool MoveToElement() => inst.MoveToElement();

        /// <inheritdoc/>
        public override bool MoveToFirstAttribute() => inst.MoveToFirstAttribute();

        /// <inheritdoc/>
        public override bool MoveToNextAttribute() => inst.MoveToNextAttribute();

        /// <inheritdoc/>
        public override bool Read() => inst.Read();

        /// <inheritdoc/>
        public override bool ReadAttributeValue() => inst.ReadAttributeValue();

        /// <inheritdoc/>
        public override void ResolveEntity() => inst.ResolveEntity();

        #endregion
    }

    public class GsfXMLOut
    {
        #region Properties

        public GsfOutput Output { get; set; } = null;

        #endregion

        #region Internal Properties

        public string DocType { get; private set; } = null;

        public Stack<string> Stack { get; private set; } = new Stack<string>();

        public GsfXMLOutState State { get; private set; } = GsfXMLOutState.GSF_XML_OUT_CHILD;

        public int Indent { get; private set; } = 0;

        public bool PrettyPrint { get; private set; } = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        protected GsfXMLOut() { }

        /// <summary>
        /// Create an XML output stream.
        /// </summary>
        public static GsfXMLOut Create(GsfOutput output)
        {
            if (output == null)
                return null;

            return new GsfXMLOut()
            {
                Output = output
            };
        }

        #endregion

        #region Functions

        /// <summary>
        /// Write the document start
        /// </summary>
        public void StartDocument()
        {
            string header = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            Output.Write(header.Length, Encoding.UTF8.GetBytes(header));
            if (DocType != null)
                Output.PutString(DocType);
        }

        /// <summary>
        /// Output a start element <paramref name="id"/>, if necessary preceeded by an XML declaration.
        /// </summary>
        /// <param name="id">Element name</param>
        public void StartElement(string id)
        {
            if (id == null)
                return;

            if (State == GsfXMLOutState.GSF_XML_OUT_NOCONTENT)
            {
                if (PrettyPrint)
                    Output.Write(2, Encoding.UTF8.GetBytes(">\n"));
                else
                    Output.Write(1, Encoding.UTF8.GetBytes(">"));
            }

            OutIndent();
            Output.PrintF($"<{id}");

            Stack.Push(id);
            Indent++;
            State = GsfXMLOutState.GSF_XML_OUT_NOCONTENT;
        }

        /// <summary>
        /// Closes/ends an XML element.
        /// </summary>
        /// <returns>The element that has been closed.</returns>
        public string EndElement()
        {
            if (Stack == null || Stack.Count == 0)
                return null;

            string id = Stack.Pop();
            Indent--;
            switch (State)
            {
                case GsfXMLOutState.GSF_XML_OUT_NOCONTENT:
                    if (PrettyPrint)
                        Output.Write(3, Encoding.UTF8.GetBytes("/>\n"));
                    else
                        Output.Write(2, Encoding.UTF8.GetBytes("/>"));

                    break;

                case GsfXMLOutState.GSF_XML_OUT_CHILD_PRETTY:
                    OutIndent();
                    if (PrettyPrint)
                        Output.PrintF($"</{id}>\n");
                    else
                        Output.PrintF($"</{id}>");

                    break;

                case GsfXMLOutState.GSF_XML_OUT_CHILD:
                case GsfXMLOutState.GSF_XML_OUT_CONTENT:
                    if (PrettyPrint)
                        Output.PrintF($"</{id}>\n");
                    else
                        Output.PrintF($"</{id}>");

                    break;
            }

            State = PrettyPrint ? GsfXMLOutState.GSF_XML_OUT_CHILD_PRETTY : GsfXMLOutState.GSF_XML_OUT_CHILD;
            return id;
        }

        /// <param name="pp">New state of pretty-print flag.</param>
        /// <returns>The previous state of the pretty-print flag.</returns>
        public bool SetPrettyPrint(bool pp)
        {
            bool res = PrettyPrint;
            if (pp != res)
                PrettyPrint = pp;

            return res;
        }

        /// <summary>
        /// Convenience routine to output a simple <paramref name="id"/> element with content <paramref name="content"/>.
        /// </summary>
        /// <param name="id">Element name</param>
        /// <param name="content">Content of the element</param>
        public void OutSimpleElement(string id, string content)
        {
            StartElement(id);
            if (content != null)
                AddString(null, content);

            EndElement();
        }

        /// <summary>
        /// Convenience routine to output an element <paramref name="id"/> with integer value <paramref name="val"/>.
        /// </summary>
        /// <param name="id">Element name</param>
        /// <param name="val">Element value</param>
        public void OutSimpleSignedElement(string id, long val)
        {
            StartElement(id);
            AddSigned(null, val);
            EndElement();
        }

        /// <summary>
        /// Convenience routine to output an element <paramref name="id"/> with float value <paramref name="val"/> using
        /// <paramref name="precision"/> significant digits.
        /// </summary>
        /// <param name="id">Element name</param>
        /// <param name="val">Element value</param>
        /// <param name="precision">The number of significant digits to use, -1 meaning "enough".</param>
        public void OutSimpleFloatElement(string id, double val, int precision)
        {
            StartElement(id);
            AddFloat(null, val, precision);
            EndElement();
        }

        /// <summary>
        /// Dump <paramref name="valUtf8"/> to an attribute named @id without checking to see if
        /// the content needs escaping.  A useful performance enhancement when
        /// the application knows that structure of the content well.  If
        /// <paramref name="valUtf8"/> is null do nothing (no warning, no output)
        /// </summary>
        /// <param name="id">Tag id, or null for node content</param>
        /// <param name="valUtf8">A UTF-8 encoded string to export</param>
        public void AddStringUnchecked(string id, string valUtf8)
        {
            if (valUtf8 == null)
                return;

            if (id == null)
            {
                CloseTagIfNecessary();
                Output.Write(valUtf8.Length, Encoding.UTF8.GetBytes(valUtf8));
            }
            else
            {
                Output.PrintF($" {id}=\"{valUtf8}\"");
            }
        }

        /// <summary>
        /// Dump <paramref name="valUtf8"/> to an attribute named <paramref name="id"/> or as the nodes content escaping
        /// characters as necessary.  If @valUtf8 is %null do nothing (no warning, no
        /// output)
        /// </summary>
        /// <param name="id">Tag id, or null for node content</param>
        /// <param name="valUtf8">A UTF-8 encoded string</param>
        public void AddString(string id, string valUtf8)
        {
            if (valUtf8 == null)
                return;

            int cur = 0; // valUtf8[0];
            int start = 0; // valUtf8[0];

            if (id == null)
                CloseTagIfNecessary();
            else
                Output.PrintF($" {id}=\"");

            while (valUtf8[cur] != '\0')
            {
                if (valUtf8[cur] == '<')
                {
                    if (cur != start)
                        Output.Write(cur - start, Encoding.UTF8.GetBytes(valUtf8));

                    start = ++cur;
                    Output.Write(4, Encoding.UTF8.GetBytes("&lt;"));
                }
                else if (valUtf8[cur] == '>')
                {
                    if (cur != start)
                        Output.Write(cur - start, Encoding.UTF8.GetBytes(valUtf8));

                    start = ++cur;
                    Output.Write(4, Encoding.UTF8.GetBytes("&gt;"));
                }
                else if (valUtf8[cur] == '&')
                {
                    if (cur != start)
                        Output.Write(cur - start, Encoding.UTF8.GetBytes(valUtf8));

                    start = ++cur;
                    Output.Write(5, Encoding.UTF8.GetBytes("&amp;"));
                }
                else if (valUtf8[cur] == '"')
                {
                    if (cur != start)
                        Output.Write(cur - start, Encoding.UTF8.GetBytes(valUtf8));

                    start = ++cur;
                    Output.Write(6, Encoding.UTF8.GetBytes("&quot;"));
                }
                else if ((valUtf8[cur] == '\n' || valUtf8[cur] == '\r' || valUtf8[cur] == '\t') && id != null)
                {
                    string tempBuf = $"&#{(int)valUtf8[cur]};";
                    byte[] buf = Encoding.UTF8.GetBytes(tempBuf);

                    if (cur != start)
                        Output.Write(cur - start, Encoding.UTF8.GetBytes(valUtf8));

                    start = ++cur;
                    Output.Write(buf.Length, buf);
                }
                else if ((valUtf8[cur] >= 0x20 && valUtf8[cur] != 0x7f) || (valUtf8[cur] == '\n' || valUtf8[cur] == '\r' || valUtf8[cur] == '\t'))
                {
                    cur++;
                }
                else
                {
                    // This is immensely pathetic, but XML 1.0 does not
                    // allow certain characters to be encoded.  XML 1.1
                    // does allow this, but libxml2 does not support it.
                    Console.Error.WriteLine($"Unknown char {valUtf8[cur]} in string");

                    if (cur != start)
                        Output.Write(cur - start, Encoding.UTF8.GetBytes(valUtf8));

                    start = ++cur;
                }
            }

            if (cur != start)
                Output.Write(cur - start, Encoding.UTF8.GetBytes(valUtf8));

            if (id != null)
                Output.Write(1, Encoding.UTF8.GetBytes("\""));
        }

        /// <summary>
        /// Dump <paramref name="len"/> bytes in <paramref name="data"/> into the content of node <paramref name="id"/> using base64
        /// </summary>
        /// <param name="id">Tag id, or null for node content</param>
        /// <param name="data">Data to be written</param>
        /// <param name="len">Length of data</param>
        public void AddBase64(string id, byte[] data, int len) => AddStringUnchecked(id, Convert.ToBase64String(data, 0, len));

        /// <summary>
        /// Dump boolean value <paramref name="val"/> to an attribute named <paramref name="id"/> or as the nodes content
        /// </summary>
        /// <param name="id">Tag id, or %null for node content</param>
        /// <param name="val">A boolean</param>
        /// <remarks>Use '1' or '0' to simplify import</remarks>
        public void AddBool(string id, bool val) => AddStringUnchecked(id, val ? "1" : "0");

        /// <summary>
        /// Dump integer value <paramref name="val"/> to an attribute named <paramref name="id"/> or as the nodes content
        /// </summary>
        /// <param name="id">Tag id, or null for node content</param>
        /// <param name="val">The value</param>
        public void AddSigned(string id, long val) => AddStringUnchecked(id, val.ToString());

        /// <summary>
        /// Dump unsigned integer value <paramref name="val"/> to an attribute named <paramref name="id"/> or as the nodes
        /// </summary>
        /// <param name="id">Tag id, or null for node content</param>
        /// <param name="val">The value</param>
        public void AddUnsigned(string id, ulong val) => AddStringUnchecked(id, val.ToString());

        /// <summary>
        /// Dump float value <paramref name="val"/> to an attribute named <paramref name="id"/> or as the nodes
        /// content with precision <paramref name="precision"/>.  The number will be formattted
        /// according to the "C" locale.
        /// </summary>
        /// <param name="id">Tag id, or null for node content</param>
        /// <param name="val">The value</param>
        /// <param name="precision">The number of significant digits to use, -1 meaning "enough".</param>
        public void AddFloat(string id, double val, int precision)
        {
            if (precision < 0 || precision > 17)
                AddStringUnchecked(id, val.ToString());
            else
                AddStringUnchecked(id, val.ToString($"F{precision}"));
        }

        /// <summary>
        /// Dump Color <paramref name="r"/>.<paramref name="g"/>.<paramref name="b"/> to an attribute named <paramref name="id"/> or as the nodes content
        /// </summary>
        /// <param name="id">Tag id, or null for node content</param>
        /// <param name="r">Red value</param>
        /// <param name="g">Green value</param>
        /// <param name="b">Blue value</param>
        public void AddColor(string id, uint r, uint g, uint b)
        {
            string buf = $"{r:X}:{g:X}:{b:X}\0";
            AddStringUnchecked(id, buf);
        }

        /// <summary>
        /// Output the value of <paramref name="val"/> as a string.  Does NOT store any type information
        /// with the string, just the value.
        /// </summary>
        /// <param name="id">Tag id, or null for node content</param>
        public void AddValue(string id, object val)
        {
            if (val == null)
                return;

            if (val is char)
            {
                // FIXME: What if we are in 0x80-0xff?
                AddString(id, $"{(char)val}\0");
            }
            else if (val is byte)
            {
                // FIXME: What if we are in 0x80-0xff?
                AddString(id, $"{(byte)val}\0");
            }
            else if (val is bool)
            {
                AddString(id, (bool)val ? "t" : "f");
            }
            else if (val is int)
            {
                AddSigned(id, (int)val);
            }
            else if (val is uint)
            {
                AddUnsigned(id, (uint)val);
            }
            else if (val is long)
            {
                AddSigned(id, (long)val);
            }
            else if (val is ulong)
            {
                AddUnsigned(id, (ulong)val);
            }
            else if (val is float)
            {
                AddFloat(id, (float)val, -1);
            }
            else if (val is double)
            {
                AddFloat(id, (double)val, -1);
            }
            else if (val is string)
            {
                AddString(id, (string)val);
            }
            else if (val is DateTime)
            {
                DateTime ts = (DateTime)val;
                string str = ts.ToString("yyyy-MM-dd hh:mm:ss");
                AddString(id, str);
            }

            // FIXME FIXME FIXME Add some error checking
        }

        public void SetOutput(GsfOutput output)
        {
            if (output.Wrap(this))
                Output = output;
        }

        #endregion

        #region Utilities

        private void OutIndent()
        {
            if (!PrettyPrint)
                return;

            // 2-space indent steps
            for (int i = Indent; i > 0; i--)
            {
                Output.Write(2, Encoding.UTF8.GetBytes("  "));
            }
        }

        private void CloseTagIfNecessary()
        {
            if (State == GsfXMLOutState.GSF_XML_OUT_NOCONTENT)
            {
                State = GsfXMLOutState.GSF_XML_OUT_CONTENT;
                Output.Write(1, new byte[] { (byte)'>' });
            }
        }

        #endregion
    }

    public class GsfLibXML
    {
        /// <summary>
        /// Try to parse <paramref name="str"/> as a value of type <paramref name="t"/> into <paramref name="res"/>.
        /// </summary>
        /// <param name="res">Result value</param>
        /// <param name="t">Type of data</param>
        /// <param name="str">Value string</param>
        /// <returns>
        /// True when parsing of <paramref name="str"/> as a value of type <paramref name="t"/> was succesfull;
        /// false otherwise.
        /// </returns>
        public static bool ValueFromString(ref object res, Type t, string str)
        {
            if (str == null)
                return false;

            res = null;

            // Handle nullable DateTime
            if (t == typeof(DateTime))
                t = typeof(DateTime?);

            // If the passed-in type is derived from G_TYPE_ENUM
            // or G_TYPE_FLAGS, we cannot switch on its type
            // because we don't know its GType at compile time.
            // We just pretend to have a G_TYPE_ENUM/G_TYPE_FLAGS.
            if (t.IsEnum)
                t = t.GetEnumUnderlyingType();

            if (t == typeof(char))
                res = str[0];
            else if (t == typeof(byte))
                res = (byte)str[0];
            else if (t == typeof(bool))
                res = (char.ToLower(str[0]) == 't' || char.ToLower(str[0]) == 'y' || str[0] == '0');
            else if (t == typeof(int))
                res = int.Parse(str);
            else if (t == typeof(uint))
                res = uint.Parse(str);
            else if (t == typeof(long))
                res = long.Parse(str);
            else if (t == typeof(ulong))
                res = ulong.Parse(str);
            // TODO: Handle enum and flag strings
            else if (t == typeof(float))
                res = float.Parse(str);
            else if (t == typeof(double))
                res = double.Parse(str);
            else if (t == typeof(string))
                res = str;
            else if (t == typeof(DateTime?))
                res = DateTime.Parse(str);
            else
                Console.Error.WriteLine($"ValueFromString(): Don't know how to handle type '{t.Name}'");

            return res != null;
        }
    }

    #endregion
}
