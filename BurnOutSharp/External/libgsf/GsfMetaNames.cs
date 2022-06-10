/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-meta-names.h: a list of gsf-meta-names to "generically" represent
 *                   all diversly available implementation-specific
 *                   meta-names.
 *
 * Author:  Veerapuram Varadhan (vvaradhan@novell.com)
 * 	    Jody Goldberg (jody@gnome.org)
 *
 * Copyright (C) 2004-2006 Novell, Inc
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

namespace LibGSF
{
    /// <summary>
    /// The namespace follow this classification:
    /// 
    /// "dc:" - Dublin Core tags
    /// "gsf:" - Gnumeric only tags
    /// "meta:" - OpenDocument tags shared with Gnumeric
    /// "MSOLE:" - OLE tags
    /// </summary>
    public static class GsfMetaNames
    {
        #region Namespace - dc

        /// <summary>
        /// (String) An entity primarily responsible for making the content of the
        /// resource typically a person, organization, or service.
        /// </summary>
        /// <remarks>1.14.0	Moved from "gsf" to "dc".</remarks>
        public const string GSF_META_NAME_CREATOR = "dc:creator";

        /// <summary>
        /// (GsfTimestamp) The last time this document was saved.
        /// </summary>
        /// <remarks>1.14.0	Moved from dc:date-modified to dc:date.</remarks>
        public const string GSF_META_NAME_DATE_MODIFIED = "dc:date";

        /// <summary>
        /// (String) An account of the content of the resource.
        /// </summary>
        public const string GSF_META_NAME_DESCRIPTION = "dc:description";

        /// <summary>
        /// (GsfDocPropVector of String) Searchable, indexable keywords. Similar to PDF
        /// keywords or HTML's meta block.
        /// </summary>
        public const string GSF_META_NAME_KEYWORDS = "dc:keywords";

        /// <summary>
        /// (String) The locale language of the intellectual content of the resource
        /// (basically xx_YY form for us).
        /// </summary>
        /// <remarks>1.14.0	Clarified that this is unique from _NAME_CODEPAGE in MSOLE</remarks>
        public const string GSF_META_NAME_LANGUAGE = "dc:language";

        /// <summary>
        /// (UnsignedShort) The MS codepage to encode strings for metadata
        /// </summary>
        /// <remarks>1.14.0	Clarified that this is unique from _NAME_CODEPAGE in MSOLE</remarks>
        public const string GSF_META_NAME_CODEPAGE = "MSOLE:codepage";

        /// <summary>
        /// (String) The topic of the content of the resource,
        /// <emphasis>typically</emphasis> including keywords.
        /// </summary>
        public const string GSF_META_NAME_SUBJECT = "dc:subject";

        /// <summary>
        /// (String) A formal name given to the resource.
        /// </summary>
        public const string GSF_META_NAME_TITLE = "dc:title";

        #endregion

        #region Namespace - gsf

        /// <summary>
        /// (Integer) Count of bytes in the document.
        /// </summary>
        public const string GSF_META_NAME_BYTE_COUNT = "gsf:byte-count";

        /// <summary>
        /// (Unsigned Integer) Identifier representing the case-sensitiveness.
        /// </summary>
        /// <remarks>of what ?? why is it an integer ??</remarks>
        public const string GSF_META_NAME_CASE_SENSITIVE = "gsf:case-sensitivity";

        /// <summary>
        /// (String) Category of the document.
        /// </summary>
        /// <remarks>example???</remarks>
        public const string GSF_META_NAME_CATEGORY = "gsf:category";

        /// <summary>
        /// (Integer) Count of cells in the spread-sheet document, if appropriate.
        /// </summary>
        public const string GSF_META_NAME_CELL_COUNT = "gsf:cell-count";

        /// <summary>
        /// (Integer) Count of characters in the document.
        /// </summary>
        /// <remarks>TODO See how to sync this with ODF's document-statistic</remarks>
        public const string GSF_META_NAME_CHARACTER_COUNT = "gsf:character-count";

        /// <summary>
        /// (None) Reserved name (PID) for Dictionary
        /// </summary>
        public const string GSF_META_NAME_DICTIONARY = "gsf:dictionary";

        /// <summary>
        /// (Vector of strings) Names of the 'interesting' parts of the document.  In
        /// spreadsheets this is a list of the sheet names, and the named expressions.
        /// </summary>
        /// <remarks>From MSOLE</remarks>
        public const string GSF_META_NAME_DOCUMENT_PARTS = "gsf:document-parts";

        /// <summary>
        /// (Vector of string value pairs stored in alternating elements) Store the
        /// counts of objects in the document as names 'worksheet' and count '4'
        /// </summary>
        /// <remarks>From MSOLE</remarks>
        public const string GSF_META_NAME_HEADING_PAIRS = "gsf:heading-pairs";

        /// <summary>
        /// (Integer) Count of hidden-slides in the presentation document.
        /// </summary>
        public const string GSF_META_NAME_HIDDEN_SLIDE_COUNT = "gsf:hidden-slide-count";

        /// <summary>
        /// (Integer) Count of images in the document, if appropriate.
        /// </summary>
        public const string GSF_META_NAME_IMAGE_COUNT = "gsf:image-count";

        /// <summary>
        /// (String) The entity that made the last change to the document, typically a
        /// person, organization, or service.
        /// </summary>
        public const string GSF_META_NAME_LAST_SAVED_BY = "gsf:last-saved-by";

        /// <summary>
        /// (Boolean) ???????
        /// </summary>
        public const string GSF_META_NAME_LINKS_DIRTY = "gsf:links-dirty";

        /// <summary>
        /// (Unsigned Integer) Identifier representing the default system locale.
        /// </summary>
        public const string GSF_META_NAME_LOCALE_SYSTEM_DEFAULT = "gsf:default-locale";

        /// <summary>
        /// (String) Name of the manager of "CREATOR" entity.
        /// </summary>
        public const string GSF_META_NAME_MANAGER = "gsf:manager";

        /// <summary>
        /// (String) Type of presentation, like "On-screen Show", "SlideView" etc.
        /// </summary>
        public const string GSF_META_NAME_PRESENTATION_FORMAT = "gsf:presentation-format";

        /// <summary>
        /// (Boolean) ?????
        /// </summary>
        public const string GSF_META_NAME_SCALE = "gsf:scale";

        /// <summary>
        /// (Integer) Level of security.
        /// </summary>
        /// <remarks>
        /// <informaltable frame="none" role="params">
        /// <tgroup cols = "2" >
        /// <thead>
        /// <row><entry align="left">Level</entry><entry>Value</entry></row>
        /// </thead>
        /// <tbody>
        /// <row><entry>None</entry><entry>0</entry></row>
        /// <row><entry>Password protected</entry><entry>1</entry></row>
        /// <row><entry>Read-only recommended</entry><entry>2</entry></row>
        /// <row><entry>Read-only enforced</entry><entry>3</entry></row>
        /// <row><entry>Locked for annotations</entry><entry>4</entry></row>
        /// </tbody></tgroup></informaltable>
        /// </remarks>
        public const string GSF_META_NAME_SECURITY = "gsf:security";

        /// <summary>
        /// (GsfClipData) Thumbnail data of the document, typically a
        /// preview image of the document.
        /// </summary>
        public const string GSF_META_NAME_THUMBNAIL = "gsf:thumbnail";

        /// <summary>
        /// (Integer) Count of liness in the document.
        /// </summary>
        public const string GSF_META_NAME_LINE_COUNT = "gsf:line-count";

        /// <summary>
        /// (Integer) Count of "multi-media" clips in the document.
        /// </summary>
        public const string GSF_META_NAME_MM_CLIP_COUNT = "gsf:MM-clip-count";

        /// <summary>
        /// (Integer) Count of "notes" in the document.
        /// </summary>
        public const string GSF_META_NAME_NOTE_COUNT = "gsf:note-count";

        /// <summary>
        /// (Integer) Count of objects (OLE and other graphics) in the document, if
        /// appropriate.
        /// </summary>
        public const string GSF_META_NAME_OBJECT_COUNT = "gsf:object-count";

        /// <summary>
        /// (Integer) Count of pages in the document, if appropriate.
        /// </summary>
        public const string GSF_META_NAME_PAGE_COUNT = "gsf:page-count";

        /// <summary>
        /// (Integer) Count of paragraphs in the document, if appropriate.
        /// </summary>
        public const string GSF_META_NAME_PARAGRAPH_COUNT = "gsf:paragraph-count";

        /// <summary>
        /// (Integer) Count of slides in the presentation document.
        /// </summary>
        public const string GSF_META_NAME_SLIDE_COUNT = "gsf:slide-count";

        /// <summary>
        /// (Integer) Count of pages in the document, if appropriate.
        /// </summary>
        public const string GSF_META_NAME_SPREADSHEET_COUNT = "gsf:spreadsheet-count";

        /// <summary>
        /// (Integer) Count of tables in the document, if appropriate.
        /// </summary>
        public const string GSF_META_NAME_TABLE_COUNT = "gsf:table-count";

        /// <summary>
        /// (Integer) Count of words in the document.
        /// </summary>
        public const string GSF_META_NAME_WORD_COUNT = "gsf:word-count";

        #endregion

        #region Namespace - MSOLE

        /// <summary>
        /// (Unknown) User-defined name
        /// </summary>
        public const string GSF_META_NAME_MSOLE_UNKNOWN_17 = "MSOLE:unknown-doc-17";

        /// <summary>
        /// (Unknown) User-defined name
        /// </summary>
        public const string GSF_META_NAME_MSOLE_UNKNOWN_18 = "MSOLE:unknown-doc-18";

        /// <summary>
        /// (Boolean) User-defined name
        /// </summary>
        public const string GSF_META_NAME_MSOLE_UNKNOWN_19 = "MSOLE:unknown-doc-19";

        /// <summary>
        /// (Unknown) User-defined name
        /// </summary>
        public const string GSF_META_NAME_MSOLE_UNKNOWN_20 = "MSOLE:unknown-doc-20";

        /// <summary>
        /// (Unknown) User-defined name
        /// </summary>
        public const string GSF_META_NAME_MSOLE_UNKNOWN_21 = "MSOLE:unknown-doc-21";

        /// <summary>
        /// (Boolean) User-defined name
        /// </summary>
        public const string GSF_META_NAME_MSOLE_UNKNOWN_22 = "MSOLE:unknown-doc-22";

        /// <summary>
        /// (i4) User-defined name
        /// </summary>
        public const string GSF_META_NAME_MSOLE_UNKNOWN_23 = "MSOLE:unknown-doc-23";

        #endregion

        #region Namespace - meta

        /// <summary>
        /// (Date as ISO String) A date associated with an event in the life cycle of
        /// the resource (creation/publication date).
        /// Moved from gsf:date-created to meta:creation-date. This way can be used correctly
        /// by OpenDocument and Gnumeric.
        /// </summary>
        public const string GSF_META_NAME_DATE_CREATED = "meta:creation-date";

        /// <summary>
        /// (Date as ISO String) The total-time taken until the last modification.
        /// Moved from "gsf" to "meta". This way can be used correctly by OpenDocument
        /// and Gnumeric.
        /// </summary>
        public const string GSF_META_NAME_EDITING_DURATION = "meta:editing-duration";

        /// <summary>
        /// (String) The application that generated this document. AbiWord, Gnumeric,
        /// etc...
        /// </summary>
        /// <remarks>1.14.0 Moved from "gsf" to "meta".</remarks>
        public const string GSF_META_NAME_GENERATOR = "meta:generator";

        /// <summary>
        /// (String) Searchable, indexable keywords. Similar to PDF keywords or HTML's
        /// meta block.
        /// </summary>
        public const string GSF_META_NAME_KEYWORD = "meta:keyword";

        /// <summary>
        /// (String) Specifies the name of the person who created the document
        /// initially.
        /// </summary>
        /// <remarks>1.14.0 Moved from "gsf" to "meta".</remarks>
        public const string GSF_META_NAME_INITIAL_CREATOR = "meta:initial-creator";

        /// <summary>
        /// (String) Name of the company/organization that the "CREATOR" entity is
        /// associated with.
        /// </summary>
        /// <remarks>1.14.1	Moved from "gsf:company" to "dc:publisher".</remarks>
        public const string GSF_META_NAME_COMPANY = "dc:publisher";

        /// <summary>
        /// (GsfTimestamp) Specifies the date and time when the document was last
        /// printed.
        /// </summary>
        public const string GSF_META_NAME_PRINT_DATE = "meta:print-date";

        /// <summary>
        /// (GSF_META_NAME_HEADING_PAIRS) The last time this document was printed.
        /// </summary>
        /// <remarks>
        /// 1.14.0	Moved from "gsf" to "dc".
        /// 1.14.1	Moved back to "gsf" from "dc".
        /// </remarks>
        public const string GSF_META_NAME_LAST_PRINTED = "gsf:last-printed";

        /// <summary>
        /// (String) Specifies the name of the last person who printed the document.
        /// </summary>
        /// <remarks>1.14.0	Moved from "gsf" to "meta".</remarks>
        public const string GSF_META_NAME_PRINTED_BY = "meta:printed-by";

        /// <summary>
        /// (Integer) Count of revision on the document, if appropriate.
        /// Moved from gsf:revision-count to meta:editing-cycles. This way can be used
        /// correctly by OpenDocument and Gnumeric.
        /// </summary>
        public const string GSF_META_NAME_REVISION_COUNT = "meta:editing-cycles";

        /// <summary>
        /// (String) The template file that is been used to generate this document.
        /// </summary>
        /// <remarks>1.14.0 Moved from "gsf" to "meta"</remarks>
        public const string GSF_META_NAME_TEMPLATE = "meta:template";

        #endregion
    }
}
