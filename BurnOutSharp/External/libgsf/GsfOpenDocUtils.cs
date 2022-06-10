/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-opendoc-utils.c:  Handle the application neutral portions of OpenDocument
 *
 * Author:  Luciano Wolf (luciano.wolf@indt.org.br)
 *
 * Copyright (C) 2006 Jody Goldberg (jody@gnome.org)
 * Copyright (C) 2005-2006 INdT - Instituto Nokia de Tecnologia
 * http://www.indt.org.br
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
using System.Xml;
using LibGSF.Input;
using static LibGSF.GsfMetaNames;

namespace LibGSF
{
    #region Enums

    public enum OpenDocType
    {
        OO_NS_OFFICE,
        OO_NS_STYLE,
        OO_NS_TEXT,
        OO_NS_TABLE,
        OO_NS_DRAW,
        OO_NS_NUMBER,
        OO_NS_CHART,
        OO_NS_DR3D,
        OO_NS_FORM,
        OO_NS_SCRIPT,
        OO_NS_CONFIG,
        OO_NS_MATH,
        OO_NS_FO,
        OO_NS_DC,
        OO_NS_META,
        OO_NS_XLINK,
        OO_NS_SVG,

        /* new in 2.0 */
        OO_NS_OOO,
        OO_NS_OOOW,
        OO_NS_OOOC,
        OO_NS_DOM,
        OO_NS_XFORMS,
        OO_NS_XSD,
        OO_NS_XSI,

        OO_NS_PRESENT,  /* added in gsf-1.14.8 */

        /* new in 3.0 */
        OO_NS_RPT,
        OO_NS_OF,
        OO_NS_RDFA,
        OO_NS_FIELD,
        OO_NS_FORMX,

        /* Other OpenDocument 1.1 */
        OO_NS_ANIM,
        OO_NS_DATASTYLE,
        OO_NS_MANIFEST,
        OO_NS_SMIL,

        /* Symphony 1.3 */
        OO_LOTUS_NS_PRODTOOLS,

        /* KOffice 1.6.3 */
        OO_KDE_NS_KOFFICE,

        /*CleverAge ODF Add-in for Microsoft Office 3.0.5224.0 (11.0.8302)*/
        OO_CLEVERAGE_NS_DC,

        /* Microsoft Excel Formulas */
        OO_MS_NS_MSOXL,

        /* Gnumeric ODF extensions */
        OO_GNUM_NS_EXT,

        /* New in ODF 3.2 */
        OO_NS_GRDDL,
        OO_NS_XHTML,
        OO_NS_TABLE_OOO,

        /* New in ODF 3.3 */
        OO_NS_CHART_OOO,

        /* New in LOCALC */
        OO_NS_LOCALC_EXT,
        OO_NS_LOCALC_EXT2
    }

    #endregion

    #region Classes

    public class GsfODFOut : GsfXMLOut
    {
        #region Internal Properties

        public int OdfVersion { get; set; } = 100;

        #endregion

        #region Functions

        public string GetVersionString() => $"{OdfVersion / 100}.{OdfVersion % 100}";

        #endregion
    }

    public class GsfOOMetaIn
    {
        public GsfDocMetaData MetaData { get; set; }

        public List<object> Keywords { get; set; }

        public Exception Err { get; set; }

        public string Name { get; set; }

        public Type Type { get; set; }

        public XmlDocument Doc { get; set; }
    }

    public static class GsfOpenDocUtils
    {
        #region Constants

        public const string OFFICE = "office:";

        #endregion

        #region Functions

        /// <summary>
        /// Gives the ODF version used by libgsf when writing Open Document files.
        /// </summary>
        /// <returns>The ODF version as a string: "1.2".</returns>
        public static string GetVersionString() => "1.2";

        /// <summary>
        /// Gives the ODF version used by libgsf when writing Open Document files.
        /// </summary>
        /// <returns>The ODF version: 102.</returns>
        public static short GetVersion() => 102;

        #endregion

        #region Extension Functions

        public static void od_meta_generator(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_GENERATOR, typeof(string));

        public static void od_meta_title(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_TITLE, typeof(string));

        public static void od_meta_description(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_DESCRIPTION, typeof(string));

        public static void od_meta_subject(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_SUBJECT, typeof(string));

        public static void od_meta_initial_creator(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_INITIAL_CREATOR, typeof(string));

        /// <summary>
        /// OD considers this the last person to modify the doc, rather than
        /// the DC convention of the person primarilly responsible for its creation
        /// </summary>
        public static void od_meta_creator(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_CREATOR, typeof(string));

        /// <summary>
        /// Last to print
        /// </summary>
        public static void od_meta_printed_by(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_PRINTED_BY, typeof(string));

        public static void od_meta_date_created(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_DATE_CREATED, typeof(DateTime));

        public static void od_meta_date_modified(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_DATE_MODIFIED, typeof(DateTime));

        public static void od_meta_print_date(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_LAST_PRINTED, typeof(DateTime));

        public static void od_meta_language(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_LANGUAGE, typeof(string));

        public static void od_meta_editing_cycles(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_REVISION_COUNT, typeof(uint));

        // FIXME FIXME FIXME should be durations using format 'PnYnMnDTnHnMnS'
        public static void od_meta_editing_duration(this GsfXMLIn xin, GsfXMLBlob blob) => xin.od_get_meta_prop(GSF_META_NAME_EDITING_DURATION, typeof(string));

        /// <summary>
        /// OD allows multiple keywords, accumulate things and make it an array
        /// </summary>
        public static void od_meta_keyword(this GsfXMLIn xin, GsfXMLBlob blob)
        {
            GsfOOMetaIn mi = (GsfOOMetaIn)xin.UserState;

            if (mi.Keywords == null)
                mi.Keywords = new List<object>();

            mi.Keywords.Add(xin.Content);
        }

        public static void od_meta_user_defined(this GsfXMLIn xin, string[] attrs)
        {
            GsfOOMetaIn mi = (GsfOOMetaIn)xin.UserState;
            mi.Type = null;
            mi.Name = null;

            for (int i = 0; i < attrs.Length - 1 && attrs[i] != null && attrs[i + 1] != null; i += 2)
            {
                if (attrs[i] == "meta:name")
                {
                    mi.Name = attrs[i + 1];
                }
                else if (attrs[i] == "meta:value-type" || attrs[i] == "meta:type")
                {
                    // "meta:type" is a typo on the write
                    // side that was fixed on 20110509.
                    if (attrs[i + 1] == "boolean")
                    {
                        mi.Type = typeof(bool);
                    }
                    else if (attrs[i + 1] == "float")
                    {
                        mi.Type = typeof(double);
                    }
                    else if (attrs[i + 1] == "string")
                    {
                        mi.Type = typeof(string);
                    }
                    else if (attrs[i + 1] == "date" || attrs[i + 1] == "data")
                    {
                        // "data" is a typo on the write side that was
                        // fixed on 20110311.
                        mi.Type = typeof(DateTime);
                    }
                    else if (attrs[i + 1] == "time")
                    {
                        mi.Type = typeof(string);
                        // We should be able to do better
                    }
                    else
                    {
                        // What?
                    }
                }
            }

            // This should not happen
            if (mi.Name == null)
                mi.Name = string.Empty;
        }

        public static void od_meta_user_defined_end(this GsfXMLIn xin, GsfXMLBlob blob)
        {
            GsfOOMetaIn mi = (GsfOOMetaIn)xin.UserState;
            if (mi.Name != null)
            {
                object res = new object();
                Type t = mi.Type;

                if (t == null)
                    t = typeof(string);

                if (!GsfLibXML.ValueFromString(ref res, t, xin.Content))
                {
                    mi.Name = null;
                    return;
                }

                if (mi.Name.StartsWith("GSF_DOCPROP_VECTOR:"))
                {
                    int true_name = mi.Name.IndexOf(':', 19);
                    if (true_name != -1 && mi.Name[++true_name] != 0)
                    {
                        GsfDocProp prop = mi.MetaData.Lookup(mi.Name.Substring(true_name));
                        if (prop == null)
                        {
                            List<GsfDocProp> vector = new List<GsfDocProp>();
                            vector.Add(prop);
                            mi.MetaData.Insert(mi.Name.Substring(true_name), vector);
                        }
                        else
                        {
                            object old = prop.Value;
                            if (old is List<GsfDocProp> oldList)
                            {
                                List<GsfDocProp> newObj = new List<GsfDocProp>();
                                newObj.AddRange(oldList);
                                newObj.Add(res as GsfDocProp);
                                prop.Value = newObj;
                            }
                            else
                            {
                                Console.Error.WriteLine($"Property \"{mi.Name.Substring(true_name)}\" used for multiple types!");
                            }

                        }

                        mi.Name = null;
                        return;
                    }
                }

                mi.MetaData.Insert(mi.Name, res);
                mi.Name = null;
            }
        }

        private static void od_get_meta_prop(this GsfXMLIn xin, string prop_name, Type g_type)
        {
            object res = new object();
            if (GsfLibXML.ValueFromString(ref res, g_type, xin.Content))
                (xin.UserState as GsfOOMetaIn).MetaData.Insert(prop_name, res);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generated based on:
        /// http://www.oasis-open.org/committees/download.php/12572/OpenDocument-v1.0-os.pdf
        /// and  OpenDocument-v1.1.pdf
        /// </summary>
        private static XmlNameTable CreateOpenDocumentNamespaces()
        {
            NameTable table = new NameTable();

            // OOo 1.0.x & 1.1.x
            table.Add("http://openoffice.org/2000/office");
            table.Add("http://openoffice.org/2000/style");
            table.Add("http://openoffice.org/2000/text");
            table.Add("http://openoffice.org/2000/table");
            table.Add("http://openoffice.org/2000/drawing");
            table.Add("http://openoffice.org/2000/datastyle");
            table.Add("http://openoffice.org/2000/chart");
            table.Add("http://openoffice.org/2000/dr3d");
            table.Add("http://openoffice.org/2000/form");
            table.Add("http://openoffice.org/2000/script");
            table.Add("http://openoffice.org/2001/config");
            table.Add("http://www.w3.org/1998/Math/MathML"); // also in 2.0
            table.Add("http://www.w3.org/1999/XSL/Format");
            table.Add("http://www.w3.org/1999/xlink"); // also in 2.0
            table.Add("http://www.w3.org/2000/svg");

            // OOo 1.9.x & 2.0.x
            table.Add("urn:oasis:names:tc:opendocument:xmlns:office:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:style:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:chart:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:form:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:script:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:presentation:1.0");

            table.Add("http://purl.org/dc/elements/1.1/");
            table.Add("http://openoffice.org/2004/office");
            table.Add("http://openoffice.org/2004/writer");
            table.Add("http://openoffice.org/2004/calc");
            table.Add("http://www.w3.org/2001/xml-events");
            table.Add("http://www.w3.org/2002/xforms");
            table.Add("http://www.w3.org/2001/XMLSchema");
            table.Add("http://www.w3.org/2001/XMLSchema-instance");

            // OOo 3.0.x
            table.Add("urn:oasis:names:tc:opendocument:xmlns:of:1.2");
            table.Add("urn:openoffice:names:experimental:ooo-ms-interop:xmlns:field:1.0");
            table.Add("urn:openoffice:names:experimental:ooxml-odf-interop:xmlns:field:1.0");
            table.Add("urn:openoffice:names:experimental:ooxml-odf-interop:xmlns:form:1.0");

            table.Add("http://openoffice.org/2005/report");
            table.Add("http://docs.oasis-open.org/opendocument/meta/rdfa#");

            // OOo 3.2.x
            table.Add("http://www.w3.org/2003/g/data-view#");
            table.Add("http://www.w3.org/1999/xhtml");
            table.Add("http://openoffice.org/2009/table");

            // OOo 3.3.x
            table.Add("http://openoffice.org/2010/chart");

            // Other OpenDocument v 1.1
            table.Add("urn:oasis:names:tc:opendocument:xmlns:config:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:animation:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:data style:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:manifest:1.0");
            table.Add("urn:oasis:names:tc:opendocument:xmlns:smil-compatible:1.0");

            // Symphony 1.3
            table.Add("http://www.ibm.com/xmlns/prodtools");

            // CleverAge ODF Add-in for Microsoft Office 3.0.5224.0 (11.0.8302)
            table.Add("http://purl.org/dc/terms/");

            // KOffice 1.6.3
            table.Add("http://www.koffice.org/2005/");

            // Microsoft Excel Formulas in ODF
            table.Add("http://schemas.microsoft.com/office/excel/formula");

            // Gnumeric ODF extensions
            table.Add("http://www.gnumeric.org/odf-extension/1.0");

            // LOCALC
            table.Add("urn:org:documentfoundation:names:experimental:office:xmlns:loext:1.0");
            table.Add("urn:org:documentfoundation:names:experimental:calc:xmlns:calcext:1.0");

            return table;
        }

        private static XmlDocumentType CreateOpenDocumentDTD(XmlDocument doc)
        {
            // Root node
            XmlDocumentType docType = doc.CreateDocumentType("meta", null, null, null);

            // OpenDocument TAGS
            docType.AppendChild(doc.CreateElement("generator"));
            docType.AppendChild(doc.CreateElement("title"));
            docType.AppendChild(doc.CreateElement("description"));
            docType.AppendChild(doc.CreateElement("subject"));
            docType.AppendChild(doc.CreateElement("keyword"));
            docType.AppendChild(doc.CreateElement("initial-creator"));
            docType.AppendChild(doc.CreateElement("creator"));
            docType.AppendChild(doc.CreateElement("printed-by"));
            docType.AppendChild(doc.CreateElement("creation-date"));
            docType.AppendChild(doc.CreateElement("date"));
            docType.AppendChild(doc.CreateElement("print-date"));
            docType.AppendChild(doc.CreateElement("template"));
            docType.AppendChild(doc.CreateElement("auto-reload"));
            docType.AppendChild(doc.CreateElement("hyperlink-behaviour"));
            docType.AppendChild(doc.CreateElement("document-statistic"));
            docType.AppendChild(doc.CreateElement("language"));
            docType.AppendChild(doc.CreateElement("editing-duration"));
            docType.AppendChild(doc.CreateElement("user-defined"));

            return docType;
        }

        public static void gsf_opendoc_metadata_subtree_free(GsfXMLIn xin, object old_state)
        {
            GsfOOMetaIn state = old_state as GsfOOMetaIn;
            if (state.Keywords != null)
                state.MetaData.Insert(GSF_META_NAME_KEYWORDS, state.Keywords);
        }

        /// <summary>
        /// Extend <paramref name="doc"/>> so that it can parse a subtree in OpenDoc metadata format
        /// </summary>
        public static void gsf_doc_meta_data_odf_subtree(GsfDocMetaData md, GsfXMLIn doc)
        {
            if (md == null)
                return;

            XmlDocument document = new XmlDocument(CreateOpenDocumentNamespaces());
            XmlDocumentType docType = CreateOpenDocumentDTD(document);
            document.AppendChild(docType);

            GsfOOMetaIn state = new GsfOOMetaIn
            {
                MetaData = md,
                Type = null,
                Doc = document,
            };

            doc.PushState(state.Doc, state, gsf_opendoc_metadata_subtree_free, null);
        }

        /// <summary>
        /// Extend <paramref name="xin"/> so that it can parse a subtree in OpenDoc metadata format
        /// The current user_state must be a  GsfOOMetaIn!
        /// </summary>
        public static void gsf_opendoc_metadata_subtree_internal(GsfXMLIn xin, string[] attrs)
        {
            GsfOOMetaIn mi = (GsfOOMetaIn)xin.UserState;
            if (mi.Doc == null)
            {
                XmlDocument document = new XmlDocument(CreateOpenDocumentNamespaces());
                XmlDocumentType docType = CreateOpenDocumentDTD(document);
                document.AppendChild(docType);
                mi.Doc = document;
            }

            xin.PushState(mi.Doc, null, null, null);
        }

        /**
         * gsf_doc_meta_data_read_from_odf:
         * @md: #GsfDocMetaData
         * @input: #GsfInput
         *
         * Read an OpenDocument metadata stream from @input and store the properties
         * into @md.  Overwrite any existing properties with the same id.
         *
         * Since: 1.14.24
         *
         * Returns: (transfer full): a #Exception if there is a problem.
         **/
        public static Exception gsf_doc_meta_data_read_from_odf(GsfDocMetaData md, GsfInput input)
        {
            GsfOOMetaIn state = new GsfOOMetaIn
            {
                MetaData = md,
                Keywords = null,
                Err = null,
                Name = null,
                Doc = null,
            };

            GsfXMLInParser parser = new GsfXMLInParser(input as GsfXMLIn);

            state.Doc = new XmlDocument(CreateOpenDocumentNamespaces());
            XmlDocumentType docType = CreateOpenDocumentDTD(state.Doc);
            state.Doc.AppendChild(docType);

            while (parser.Read()) { }

            parser.Close();

            if (state.Keywords != null)
                state.Keywords.Add(md);

            return state.Err;
        }

        /**
         * gsf_opendoc_metadata_read: (skip)
         * @input: #GsfInput
         * @md: #GsfDocMetaData
         *
         * Read an OpenDocument metadata stream from @input and store the properties
         * into @md.  Overwrite any existing properties with the same id.
         *
         * Deprecated: 1.14.24, use gsf_doc_meta_data_read_from_odf
         *
         * Returns: (transfer full): a #Exception if there is a problem.
         **/
        public static Exception gsf_opendoc_metadata_read(GsfInput input, GsfDocMetaData md) => gsf_doc_meta_data_read_from_odf(md, input);

        // Shared by all instances and never freed
        private static readonly Dictionary<string, string> od_prop_name_map = new Dictionary<string, string>
        {
            { GSF_META_NAME_GENERATOR,  "meta:generator" },
            { GSF_META_NAME_TITLE,      "dc:title" },
            { GSF_META_NAME_DESCRIPTION,    "dc:description" },
            { GSF_META_NAME_SUBJECT,    "dc:subject" },
            { GSF_META_NAME_INITIAL_CREATOR,"meta:initial-creator" },
            { GSF_META_NAME_CREATOR,    "dc:creator" },
            { GSF_META_NAME_PRINTED_BY, "meta:printed-by" },
            { GSF_META_NAME_DATE_CREATED,   "meta:creation-date" },
            { GSF_META_NAME_DATE_MODIFIED,  "dc:date" },
            { GSF_META_NAME_LAST_PRINTED,   "meta:print-date" },
            { GSF_META_NAME_LANGUAGE,   "dc:language" },
            { GSF_META_NAME_REVISION_COUNT, "meta:editing-cycles" },
            { GSF_META_NAME_EDITING_DURATION, "meta:editing-duration" }
        };

        public static string od_map_prop_name(string name)
        {
            if (!od_prop_name_map.ContainsKey(name))
                return null;

            return od_prop_name_map[name];
        }

        /*
            meta:page-count		GSF_META_NAME_PAGE_COUNT
            meta:table-count	GSF_META_NAME_TABLE_COUNT:
            meta:draw-count
            meta:image-count	GSF_META_NAME_IMAGE_COUNT:
            meta:ole-object-count	GSF_META_NAME_OBJECT_COUNT:
            meta:paragraph-count	GSF_META_NAME_PARAGRAPH_COUNT:
            meta:word-count
            meta:character-count	GSF_META_NAME_CHARACTER_COUNT
            meta:row-count		GSF_META_NAME_LINE_COUNT:
            meta:frame-count
            meta:sentence-count
            meta:syllable-count
            meta:non-whitespace-character-count

            meta:page-count
                GSF_META_NAME_SPREADSHEET_COUNT
            meta:table-count
                GSF_META_NAME_TABLE_COUNT:
            meta:image-count
                * GSF_META_NAME_IMAGE_COUNT:
            meta:cell-count
                GSF_META_NAME_CELL_COUNT
            meta:object-count
                GSF_META_NAME_OBJECT_COUNT:

            meta:page-count
                 GSF_META_NAME_SLIDE_COUNT:
            meta:image-count
                GSF_META_NAME_IMAGE_COUNT:
            meta:object-count
                GSF_META_NAME_OBJECT_COUNT:
        */

        /// <summary>
        /// ODF does not like "t" and "f" which we use normally
        /// </summary>
        public static void gsf_xml_out_add_gvalue_for_odf(this GsfXMLOut xout, string id, object val)
        {
            if (val is bool b)
                xout.AddString(id, b ? "true" : "false");
            else
                xout.AddValue(id, val);
        }

        static void meta_write_props_user_defined(string prop_name, object val, GsfXMLOut output)
        {
            string type_name = null;

            output.StartElement("meta:user-defined");
            output.AddStringUnchecked("meta:name", prop_name);

            if (null == val)
            {
                output.EndElement();
                return;
            }

            if (val.GetType() == typeof(char))
                type_name = "string";
            else if (val.GetType() == typeof(byte))
                type_name = "string";
            else if (val.GetType() == typeof(string))
                type_name = "string";
            else if (val.GetType().IsEnum)
                type_name = "string";
            else if (val.GetType() == typeof(bool))
                type_name = "boolean";
            else if (val.GetType() == typeof(int))
                type_name = "float";
            else if (val.GetType() == typeof(uint))
                type_name = "float";
            else if (val.GetType() == typeof(long))
                type_name = "float";
            else if (val.GetType() == typeof(ulong))
                type_name = "float";
            else if (val.GetType() == typeof(float))
                type_name = "float";
            else if (val.GetType() == typeof(double))
                type_name = "float";
            else if (val.GetType() == typeof(DateTime))
                type_name = "date";

            if (type_name != null)
                output.AddStringUnchecked("meta:value-type", type_name);
            if (val != null)
                output.gsf_xml_out_add_gvalue_for_odf(null, val);

            output.EndElement();
        }

        static void meta_write_props(string prop_name, GsfDocProp prop, GsfXMLOut output)
        {
            string mapped_name;
            object val = prop.Value;

            // Handle specially
            if (prop_name == GSF_META_NAME_KEYWORDS)
            {
                // OLE2 stores a single string, with no obvious
                // standard for seperator
                if (val.GetType() == typeof(string))
                {
                    string str = (string)val;
                    if (!string.IsNullOrEmpty(str))
                    {
                        output.StartElement("meta:keyword");
                        output.AddString(null, str);
                        output.EndElement();
                    }
                }
                else if (val is List<GsfDocProp> list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        string str = list[i].Name;
                        output.StartElement("meta:keyword");
                        output.AddString(null, str);
                        output.EndElement();
                    }
                }
                return;
            }

            if ((mapped_name = od_map_prop_name(prop_name)) == null)
            {
                if (val is List<GsfDocProp> list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        string new_name = $"GSF_DOCPROP_VECTOR:{i:4}:{prop_name}";
                        meta_write_props_user_defined(new_name, list[i], output);
                    }
                }
                else
                {
                    meta_write_props_user_defined(prop_name, val, output);
                }

                return;
            }

            // Standardized  ODF meta items
            output.StartElement(mapped_name);
            if (val != null)
                gsf_xml_out_add_gvalue_for_odf(output, null, val);

            output.EndElement();
        }

        /**
         * gsf_doc_meta_data_write_to_odf:
         * @md: #GsfDocMetaData
         * @output: (type GsfXMLOut): a pointer to a #GsfOutput.
         *
         * Since: 1.14.24
         *
         * Returns: %true if no error occured.
         **/
        static bool gsf_doc_meta_data_write_to_odf(GsfDocMetaData[] md, object output)
        {
            string ver_str;

            if (output == null)
                return false;

            // For compatibility we take a GsfXMLOut argument.  It really
            // ought to be a GsfODFOut.
            GsfXMLOut xout = output as GsfXMLOut;
            GsfODFOut oout = (output is GsfODFOut) ? output as GsfODFOut : null;

            ver_str = oout != null ? oout.GetVersionString() : GetVersionString();

            xout.StartElement($"{OFFICE}document-meta");
            xout.AddStringUnchecked("xmlns:office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
            xout.AddStringUnchecked("xmlns:xlink", "http://www.w3.org/1999/xlink");
            xout.AddStringUnchecked("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            xout.AddStringUnchecked("xmlns:meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
            xout.AddStringUnchecked("xmlns:ooo", "http://openoffice.org/2004/office");
            xout.AddStringUnchecked("office:version", ver_str);
            
            xout.StartElement($"{OFFICE}meta");
            foreach (GsfDocMetaData data in md)
            {
                foreach (KeyValuePair<string, GsfDocProp> kvp in data.Table)
                {
                    meta_write_props(kvp.Key, kvp.Value, xout);
                }
            }
            xout.EndElement();

            xout.EndElement();

            return true;
        }

        #endregion
    }

    #endregion
}
