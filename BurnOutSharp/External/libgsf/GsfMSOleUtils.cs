/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-MSOLE-utils.c:
 *
 * Copyright (C) 2002-2006 Jody Goldberg (jody@gnome.org)
 * Copyright (C) 2002-2006 Dom Lachowicz (cinamod@hotmail.com)
 * excel_iconv* family of functions (C) 2001 by Vlad Harchev <hvv@hippo.ru>
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
using System.IO;
using System.Text;
using LibGSF.Input;
using LibGSF.Output;
using static LibGSF.GsfMetaNames;

namespace LibGSF
{
    #region Enums

    public enum GsfMSOLEMetaDataType
    {
        /// <summary>
        /// In either summary or docsummary
        /// </summary>
        COMMON_PROP,

        /// <summary>
        /// SummaryInformation properties
        /// </summary>
        COMPONENT_PROP,

        /// <summary>
        /// DocumentSummaryInformation properties
        /// </summary>
        DOC_PROP,

        USER_PROP
    }

    [Flags]
    public enum GsfMSOLEVariantType
    {
        VT_EMPTY = 0,
        VT_NULL = 1,
        VT_I2 = 2,
        VT_I4 = 3,
        VT_R4 = 4,
        VT_R8 = 5,
        VT_CY = 6,
        VT_DATE = 7,
        VT_BSTR = 8,
        VT_DISPATCH = 9,
        VT_ERROR = 10,
        VT_BOOL = 11,
        VT_VARIANT = 12,
        VT_UNKNOWN = 13,
        VT_DECIMAL = 14,

        VT_I1 = 16,
        VT_UI1 = 17,
        VT_UI2 = 18,
        VT_UI4 = 19,
        VT_I8 = 20,
        VT_UI8 = 21,
        VT_INT = 22,
        VT_UINT = 23,
        VT_VOID = 24,
        VT_HRESULT = 25,
        VT_PTR = 26,
        VT_SAFEARRAY = 27,
        VT_CARRAY = 28,
        VT_USERDEFINED = 29,
        VT_LPSTR = 30,
        VT_LPWSTR = 31,

        VT_FILETIME = 64,
        VT_BLOB = 65,
        VT_STREAM = 66,
        VT_STORAGE = 67,
        VT_STREAMED_OBJECT = 68,
        VT_STORED_OBJECT = 69,
        VT_BLOB_OBJECT = 70,
        VT_CF = 71,
        VT_CLSID = 72,
        VT_VECTOR = 0x1000
    }

    #endregion

    #region Classes

    public static class GsfMSOleUtils
    {
        #region Constants

        /// <summary>
        /// The Format Identifier for Summary Information
        /// F29F85E0-4FF9-1068-AB91-08002B27B3D9
        /// </summary>
        public static readonly byte[] ComponentGUID =
        {
            0xe0, 0x85, 0x9f, 0xf2, 0xf9, 0x4f, 0x68, 0x10,
            0xab, 0x91, 0x08, 0x00, 0x2b, 0x27, 0xb3, 0xd9
        };

        /// <summary>
        /// The Format Identifier for Document Summary Information
        /// D5CDD502-2E9C-101B-9397-08002B2CF9AE
        /// </summary>
        public static readonly byte[] DocumentGUID =
        {
            0x02, 0xd5, 0xcd, 0xd5, 0x9c, 0x2e, 0x1b, 0x10,
            0x93, 0x97, 0x08, 0x00, 0x2b, 0x2c, 0xf9, 0xae
        };

        /// <summary>
        /// The Format Identifier for User-Defined Properties
        /// D5CDD505-2E9C-101B-9397-08002B2CF9AE
        /// </summary>
        public static readonly byte[] UserGUID =
        {
            0x05, 0xd5, 0xcd, 0xd5, 0x9c, 0x2e, 0x1b, 0x10,
            0x93, 0x97, 0x08, 0x00, 0x2b, 0x2c, 0xf9, 0xae
        };

        public static readonly GsfMSOleMetaDataPropMap[] BuiltInProperties =
        {
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Dictionary",
                Section = GsfMSOLEMetaDataType.COMMON_PROP,
                GsfName = GSF_META_NAME_DICTIONARY,
                Id = 0,
                PreferredType = 0, // Magic
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "CodePage",
                Section = GsfMSOLEMetaDataType.COMMON_PROP,
                GsfName = GSF_META_NAME_CODEPAGE,
                Id = 1,
                PreferredType = GsfMSOLEVariantType.VT_I2
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "LOCALE_SYSTEM_DEFAULT",
                Section = GsfMSOLEMetaDataType.COMMON_PROP,
                GsfName = GSF_META_NAME_LOCALE_SYSTEM_DEFAULT,
                Id = 0x80000000,
                PreferredType = GsfMSOLEVariantType.VT_UI4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "CASE_SENSITIVE",
                Section = GsfMSOLEMetaDataType.COMMON_PROP,
                GsfName = GSF_META_NAME_CASE_SENSITIVE,
                Id = 0x80000003,
                PreferredType = GsfMSOLEVariantType.VT_UI4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Category",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_CATEGORY,
                Id = 2,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "PresentationFormat",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_PRESENTATION_FORMAT,
                Id = 3,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumBytes",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_BYTE_COUNT,
                Id = 4,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumLines",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_LINE_COUNT,
                Id = 5,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumParagraphs",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_PARAGRAPH_COUNT,
                Id = 6,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumSlides",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_SLIDE_COUNT,
                Id = 7,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumNotes",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_NOTE_COUNT,
                Id = 8,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumHiddenSlides",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_HIDDEN_SLIDE_COUNT,
                Id = 9,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumMMClips",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MM_CLIP_COUNT,
                Id = 10,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Scale",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_SCALE,
                Id = 11,
                PreferredType = GsfMSOLEVariantType.VT_BOOL
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "HeadingPairs",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_HEADING_PAIRS,
                Id = 12,
                PreferredType = GsfMSOLEVariantType.VT_VECTOR | GsfMSOLEVariantType.VT_VARIANT
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "DocumentParts",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_DOCUMENT_PARTS,
                Id = 13,
                PreferredType = GsfMSOLEVariantType.VT_VECTOR | GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Manager",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MANAGER,
                Id = 14,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Company",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_COMPANY,
                Id = 15,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "LinksDirty",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_LINKS_DIRTY,
                Id = 16,
                PreferredType = GsfMSOLEVariantType.VT_BOOL
            },
            
            // Possible match: { 0x0011, 0x0003, "PIDDSI_CCHWITHSPACES", "Number of characters with white-space" },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "DocSumInfo_17",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MSOLE_UNKNOWN_17,
                Id = 17,
                PreferredType = GsfMSOLEVariantType.VT_UNKNOWN
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "DocSumInfo_18",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MSOLE_UNKNOWN_18,
                Id = 18,
                PreferredType = GsfMSOLEVariantType.VT_UNKNOWN
            },
            
            // Possible match: { 0x0013, 0x000b, "PIDDSI_SHAREDDOC", "Shared document" },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "DocSumInfo_19",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MSOLE_UNKNOWN_19,
                Id = 19,
                PreferredType = GsfMSOLEVariantType.VT_BOOL
            },
            
            // Possible match: +  PIDDSI_LINKBASE  = 0x0014
            new GsfMSOleMetaDataPropMap
            {
                MsName = "DocSumInfo_20",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MSOLE_UNKNOWN_20,
                Id = 20,
                PreferredType = GsfMSOLEVariantType.VT_UNKNOWN
            },
            
            // Possible match: +  PIDDSI_HLINKS= 0x0015,
            new GsfMSOleMetaDataPropMap
            {
                MsName = "DocSumInfo_21",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MSOLE_UNKNOWN_21,
                Id = 21,
                PreferredType = GsfMSOLEVariantType.VT_UNKNOWN
            },
            
            // Possible match: { 0x0016, 0x000b, "PIDDSI_HYPERLINKSCHANGED", "Hyper links changed" },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "DocSumInfo_22",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MSOLE_UNKNOWN_22,
                Id = 22,
                PreferredType = GsfMSOLEVariantType.VT_BOOL
            },
            
            // Possible match: { 0x0017, 0x0003, "PIDDSI_VERSION", "Creating application version" },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "DocSumInfo_23",
                Section = GsfMSOLEMetaDataType.DOC_PROP,
                GsfName = GSF_META_NAME_MSOLE_UNKNOWN_23,
                Id = 23,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Title",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_TITLE,
                Id = 2,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Subject",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_SUBJECT,
                Id = 3,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Author",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_CREATOR,
                Id = 4,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Keywords",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_KEYWORDS,
                Id = 5,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Comments",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_DESCRIPTION,
                Id = 6,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Template",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_TEMPLATE,
                Id = 7,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "LastSavedBy",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_LAST_SAVED_BY,
                Id = 8,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "RevisionNumber",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_REVISION_COUNT,
                Id = 9,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "TotalEditingTime",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_EDITING_DURATION,
                Id = 10,
                PreferredType = GsfMSOLEVariantType.VT_FILETIME
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "LastPrinted",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_LAST_PRINTED,
                Id = 11,
                PreferredType = GsfMSOLEVariantType.VT_FILETIME
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "CreateTime",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_DATE_CREATED,
                Id = 12,
                PreferredType = GsfMSOLEVariantType.VT_FILETIME
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "LastSavedTime",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_DATE_MODIFIED,
                Id = 13,
                PreferredType = GsfMSOLEVariantType.VT_FILETIME
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumPages",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_PAGE_COUNT,
                Id = 14,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumWords",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_WORD_COUNT,
                Id = 15,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "NumCharacters",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_CHARACTER_COUNT,
                Id = 16,
                PreferredType = GsfMSOLEVariantType.VT_I4
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Thumbnail",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_THUMBNAIL,
                Id = 17,
                PreferredType = GsfMSOLEVariantType.VT_CF
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "AppName",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_GENERATOR,
                Id = 18,
                PreferredType = GsfMSOLEVariantType.VT_LPSTR
            },
            new GsfMSOleMetaDataPropMap
            {
                MsName = "Security",
                Section = GsfMSOLEMetaDataType.COMPONENT_PROP,
                GsfName = GSF_META_NAME_SECURITY,
                Id = 19,
                PreferredType = GsfMSOLEVariantType.VT_I4
            }
        };

        public static readonly LanguageId[] MSOLELanguageIds =
        {
            new LanguageId { Tag = "-none-", Id = 0x0000 }, /* none (language neutral) */
            new LanguageId { Tag = "-none-", Id = 0x0400 }, /* none */
            new LanguageId { Tag = "af_ZA",  Id = 0x0436 }, /* Afrikaans */
            new LanguageId { Tag = "am",     Id = 0x045e }, /* Amharic */
            new LanguageId { Tag = "sq_AL",  Id = 0x041c }, /* Albanian */
            new LanguageId { Tag = "ar_SA",  Id = 0x0401 }, /* Arabic (Saudi) */
            new LanguageId { Tag = "ar_IQ",  Id = 0x0801 }, /* Arabic (Iraq) */
            new LanguageId { Tag = "ar_EG",  Id = 0x0c01 }, /* Arabic (Egypt) */
            new LanguageId { Tag = "ar_LY",  Id = 0x1001 }, /* Arabic (Libya) */
            new LanguageId { Tag = "ar_DZ",  Id = 0x1401 }, /* Arabic (Algeria) */
            new LanguageId { Tag = "ar_MA",  Id = 0x1801 }, /* Arabic (Morocco) */
            new LanguageId { Tag = "ar_TN",  Id = 0x1c01 }, /* Arabic (Tunisia) */
            new LanguageId { Tag = "ar_OM",  Id = 0x2001 }, /* Arabic (Oman) */
            new LanguageId { Tag = "ar_YE",  Id = 0x2401 }, /* Arabic (Yemen) */
            new LanguageId { Tag = "ar_SY",  Id = 0x2801 }, /* Arabic (Syria) */
            new LanguageId { Tag = "ar_JO",  Id = 0x2c01 }, /* Arabic (Jordan) */
            new LanguageId { Tag = "ar_LB",  Id = 0x3001 }, /* Arabic (Lebanon) */
            new LanguageId { Tag = "ar_KW",  Id = 0x3401 }, /* Arabic (Kuwait) */
            new LanguageId { Tag = "ar_AE",  Id = 0x3801 }, /* Arabic (United Arab Emirates) */
            new LanguageId { Tag = "ar_BH",  Id = 0x3c01 }, /* Arabic (Bahrain) */
            new LanguageId { Tag = "ar_QA",  Id = 0x4001 }, /* Arabic (Qatar) */
            new LanguageId { Tag = "as",     Id = 0x044d }, /* Assamese */
            new LanguageId { Tag = "az",     Id = 0x042c }, /* Azerbaijani */
            new LanguageId { Tag = "hy_AM",  Id = 0x042b }, /* Armenian */
            new LanguageId { Tag = "az",     Id = 0x044c }, /* Azeri (Latin) az_ */
            new LanguageId { Tag = "az",     Id = 0x082c }, /* Azeri (Cyrillic) az_ */
            new LanguageId { Tag = "eu_ES",  Id = 0x042d }, /* Basque */
            new LanguageId { Tag = "be_BY",  Id = 0x0423 }, /* Belarussian */
            new LanguageId { Tag = "bn",     Id = 0x0445 }, /* Bengali bn_ */
            new LanguageId { Tag = "bg_BG",  Id = 0x0402 }, /* Bulgarian */
            new LanguageId { Tag = "ca_ES",  Id = 0x0403 }, /* Catalan */
            new LanguageId { Tag = "zh_TW",  Id = 0x0404 }, /* Chinese (Taiwan) */
            new LanguageId { Tag = "zh_CN",  Id = 0x0804 }, /* Chinese (PRC) */
            new LanguageId { Tag = "zh_HK",  Id = 0x0c04 }, /* Chinese (Hong Kong) */
            new LanguageId { Tag = "zh_SG",  Id = 0x1004 }, /* Chinese (Singapore) */
            new LanguageId { Tag = "ch_MO",  Id = 0x1404 }, /* Chinese (Macau SAR) */
            new LanguageId { Tag = "hr_HR",  Id = 0x041a }, /* Croatian */
            new LanguageId { Tag = "cs_CZ",  Id = 0x0405 }, /* Czech */
            new LanguageId { Tag = "da_DK",  Id = 0x0406 }, /* Danish */
            new LanguageId { Tag = "div",    Id = 0x465 }, /* Divehi div_*/
            new LanguageId { Tag = "nl_NL",  Id = 0x0413 }, /* Dutch (Netherlands) */
            new LanguageId { Tag = "nl_BE",  Id = 0x0813 }, /* Dutch (Belgium) */
            new LanguageId { Tag = "en_US",  Id = 0x0409 }, /* English (USA) */
            new LanguageId { Tag = "en_GB",  Id = 0x0809 }, /* English (UK) */
            new LanguageId { Tag = "en_AU",  Id = 0x0c09 }, /* English (Australia) */
            new LanguageId { Tag = "en_CA",  Id = 0x1009 }, /* English (Canada) */
            new LanguageId { Tag = "en_NZ",  Id = 0x1409 }, /* English (New Zealand) */
            new LanguageId { Tag = "en_IE",  Id = 0x1809 }, /* English (Ireland) */
            new LanguageId { Tag = "en_ZA",  Id = 0x1c09 }, /* English (South Africa) */
            new LanguageId { Tag = "en_JM",  Id = 0x2009 }, /* English (Jamaica) */
            new LanguageId { Tag = "en",     Id = 0x2409 }, /* English (Caribbean) */
            new LanguageId { Tag = "en_BZ",  Id = 0x2809 }, /* English (Belize) */
            new LanguageId { Tag = "en_TT",  Id = 0x2c09 }, /* English (Trinidad) */
            new LanguageId { Tag = "en_ZW",  Id = 0x3009 }, /* English (Zimbabwe) */
            new LanguageId { Tag = "en_PH",  Id = 0x3409 }, /* English (Phillipines) */
            new LanguageId { Tag = "et_EE",  Id = 0x0425 }, /* Estonian */
            new LanguageId { Tag = "fo",     Id = 0x0438 }, /* Faeroese fo_ */
            new LanguageId { Tag = "fa_IR",  Id = 0x0429 }, /* Farsi */
            new LanguageId { Tag = "fi_FI",  Id = 0x040b }, /* Finnish */
            new LanguageId { Tag = "fr_FR",  Id = 0x040c }, /* French (France) */
            new LanguageId { Tag = "fr_BE",  Id = 0x080c }, /* French (Belgium) */
            new LanguageId { Tag = "fr_CA",  Id = 0x0c0c }, /* French (Canada) */
            new LanguageId { Tag = "fr_CH",  Id = 0x100c }, /* French (Switzerland) */
            new LanguageId { Tag = "fr_LU",  Id = 0x140c }, /* French (Luxembourg) */
            new LanguageId { Tag = "fr_MC",  Id = 0x180c }, /* French (Monaco) */
            new LanguageId { Tag = "gl",     Id = 0x0456 }, /* Galician gl_ */
            new LanguageId { Tag = "ga_IE",  Id = 0x083c }, /* Irish Gaelic */
            new LanguageId { Tag = "gd_GB",  Id = 0x100c }, /* Scottish Gaelic */
            new LanguageId { Tag = "ka_GE",  Id = 0x0437 }, /* Georgian */
            new LanguageId { Tag = "de_DE",  Id = 0x0407 }, /* German (Germany) */
            new LanguageId { Tag = "de_CH",  Id = 0x0807 }, /* German (Switzerland) */
            new LanguageId { Tag = "de_AT",  Id = 0x0c07 }, /* German (Austria) */
            new LanguageId { Tag = "de_LU",  Id = 0x1007 }, /* German (Luxembourg) */
            new LanguageId { Tag = "de_LI",  Id = 0x1407 }, /* German (Liechtenstein) */
            new LanguageId { Tag = "el_GR",  Id = 0x0408 }, /* Greek */
            new LanguageId { Tag = "gu",     Id = 0x0447 }, /* Gujarati gu_ */
            new LanguageId { Tag = "ha",     Id = 0x0468 }, /* Hausa */
            new LanguageId { Tag = "he_IL",  Id = 0x040d }, /* Hebrew */
            new LanguageId { Tag = "hi_IN",  Id = 0x0439 }, /* Hindi */
            new LanguageId { Tag = "hu_HU",  Id = 0x040e }, /* Hungarian */
            new LanguageId { Tag = "is_IS",  Id = 0x040f }, /* Icelandic */
            new LanguageId { Tag = "id_ID",  Id = 0x0421 }, /* Indonesian */
            new LanguageId { Tag = "iu",     Id = 0x045d }, /* Inkutitut */
            new LanguageId { Tag = "it_IT",  Id = 0x0410 }, /* Italian (Italy) */
            new LanguageId { Tag = "it_CH",  Id = 0x0810 }, /* Italian (Switzerland) */
            new LanguageId { Tag = "ja_JP",  Id = 0x0411}, /* Japanese */
            new LanguageId { Tag = "kn",     Id = 0x044b }, /* Kannada kn_ */
            new LanguageId { Tag = "ks",     Id = 0x0860 }, /* Kashmiri (India) ks_ */
            new LanguageId { Tag = "kk",     Id = 0x043f }, /* Kazakh kk_ */
            new LanguageId { Tag = "kok",    Id = 0x0457 }, /* Konkani kok_ */
            new LanguageId { Tag = "ko_KR",  Id = 0x0412 }, /* Korean */
            new LanguageId { Tag = "ko",     Id = 0x0812 }, /* Korean (Johab) ko_ */
            new LanguageId { Tag = "kir",    Id = 0x0440 }, /* Kyrgyz */
            new LanguageId { Tag = "la",     Id = 0x0476 }, /* Latin */
            new LanguageId { Tag = "lo",     Id = 0x0454 }, /* Laothian */
            new LanguageId { Tag = "lv_LV",  Id = 0x0426 }, /* Latvian */
            new LanguageId { Tag = "lt_LT",  Id = 0x0427 }, /* Lithuanian */
            new LanguageId { Tag = "lt_LT",  Id = 0x0827 }, /* Lithuanian (Classic) */
            new LanguageId { Tag = "mk",     Id = 0x042f }, /* FYRO Macedonian */
            new LanguageId { Tag = "my_MY",  Id = 0x043e }, /* Malaysian */
            new LanguageId { Tag = "my_BN",  Id = 0x083e }, /* Malay Brunei Darussalam */
            new LanguageId { Tag = "ml",     Id = 0x044c }, /* Malayalam ml_ */
            new LanguageId { Tag = "mr",     Id = 0x044e }, /* Marathi mr_ */
            new LanguageId { Tag = "mt",     Id = 0x043a }, /* Maltese */
            new LanguageId { Tag = "mo",     Id = 0x0450 }, /* Mongolian */
            new LanguageId { Tag = "ne_NP",  Id = 0x0461 }, /* Napali (Nepal) */
            new LanguageId { Tag = "ne_IN",  Id = 0x0861 }, /* Nepali (India) */
            new LanguageId { Tag = "nb_NO",  Id = 0x0414 }, /* Norwegian (Bokmaal) */
            new LanguageId { Tag = "nn_NO",  Id = 0x0814 }, /* Norwegian (Nynorsk) */
            new LanguageId { Tag = "or",     Id = 0x0448 }, /* Oriya or_ */
            new LanguageId { Tag = "om",     Id = 0x0472 }, /* Oromo (Afan, Galla) */
            new LanguageId { Tag = "pl_PL",  Id = 0x0415 }, /* Polish */
            new LanguageId { Tag = "pt_BR",  Id = 0x0416 }, /* Portuguese (Brazil) */
            new LanguageId { Tag = "pt_PT",  Id = 0x0816 }, /* Portuguese (Portugal) */
            new LanguageId { Tag = "pa",     Id = 0x0446 }, /* Punjabi pa_ */
            new LanguageId { Tag = "ps",     Id = 0x0463 }, /* Pashto (Pushto) */
            new LanguageId { Tag = "rm",     Id = 0x0417 }, /* Rhaeto_Romanic rm_ */
            new LanguageId { Tag = "ro_RO",  Id = 0x0418 }, /* Romanian */
            new LanguageId { Tag = "ro_MD",  Id = 0x0818 }, /* Romanian (Moldova) */
            new LanguageId { Tag = "ru_RU",  Id = 0x0419 }, /* Russian */
            new LanguageId { Tag = "ru_MD",  Id = 0x0819 }, /* Russian (Moldova) */
            new LanguageId { Tag = "se",     Id = 0x043b }, /* Sami (Lappish) se_ */
            new LanguageId { Tag = "sa",     Id = 0x044f }, /* Sanskrit sa_ */
            new LanguageId { Tag = "sr",     Id = 0x0c1a }, /* Serbian (Cyrillic) sr_ */
            new LanguageId { Tag = "sr",     Id = 0x081a }, /* Serbian (Latin) sr_ */
            new LanguageId { Tag = "sd",     Id = 0x0459 }, /* Sindhi sd_ */
            new LanguageId { Tag = "sk_SK",  Id = 0x041b }, /* Slovak */
            new LanguageId { Tag = "sl_SI",  Id = 0x0424 }, /* Slovenian */
            new LanguageId { Tag = "wen",    Id = 0x042e }, /* Sorbian wen_ */
            new LanguageId { Tag = "so",     Id = 0x0477 }, /* Somali */
            new LanguageId { Tag = "es_ES",  Id = 0x040a }, /* Spanish (Spain, Traditional) */
            new LanguageId { Tag = "es_MX",  Id = 0x080a }, /* Spanish (Mexico) */
            new LanguageId { Tag = "es_ES",  Id = 0x0c0a }, /* Spanish (Modern) */
            new LanguageId { Tag = "es_GT",  Id = 0x100a }, /* Spanish (Guatemala) */
            new LanguageId { Tag = "es_CR",  Id = 0x140a }, /* Spanish (Costa Rica) */
            new LanguageId { Tag = "es_PA",  Id = 0x180a }, /* Spanish (Panama) */
            new LanguageId { Tag = "es_DO",  Id = 0x1c0a }, /* Spanish (Dominican Republic) */
            new LanguageId { Tag = "es_VE",  Id = 0x200a }, /* Spanish (Venezuela) */
            new LanguageId { Tag = "es_CO",  Id = 0x240a }, /* Spanish (Colombia) */
            new LanguageId { Tag = "es_PE",  Id = 0x280a }, /* Spanish (Peru) */
            new LanguageId { Tag = "es_AR",  Id = 0x2c0a }, /* Spanish (Argentina) */
            new LanguageId { Tag = "es_EC",  Id = 0x300a }, /* Spanish (Ecuador) */
            new LanguageId { Tag = "es_CL",  Id = 0x340a }, /* Spanish (Chile) */
            new LanguageId { Tag = "es_UY",  Id = 0x380a }, /* Spanish (Uruguay) */
            new LanguageId { Tag = "es_PY",  Id = 0x3c0a }, /* Spanish (Paraguay) */
            new LanguageId { Tag = "es_BO",  Id = 0x400a }, /* Spanish (Bolivia) */
            new LanguageId { Tag = "es_SV",  Id = 0x440a }, /* Spanish (El Salvador) */
            new LanguageId { Tag = "es_HN",  Id = 0x480a }, /* Spanish (Honduras) */
            new LanguageId { Tag = "es_NI",  Id = 0x4c0a }, /* Spanish (Nicaragua) */
            new LanguageId { Tag = "es_PR",  Id = 0x500a }, /* Spanish (Puerto Rico) */
            new LanguageId { Tag = "sx",     Id = 0x0430 }, /* Sutu */
            new LanguageId { Tag = "sw",     Id = 0x0441 }, /* Swahili (Kiswahili/Kenya) */
            new LanguageId { Tag = "sv_SE",  Id = 0x041d }, /* Swedish */
            new LanguageId { Tag = "sv_FI",  Id = 0x081d }, /* Swedish (Finland) */
            new LanguageId { Tag = "ta",     Id = 0x0449 }, /* Tamil ta_ */
            new LanguageId { Tag = "tt",     Id = 0x0444 }, /* Tatar (Tatarstan) tt_ */
            new LanguageId { Tag = "te",     Id = 0x044a }, /* Telugu te_ */
            new LanguageId { Tag = "th_TH",  Id = 0x041e }, /* Thai */
            new LanguageId { Tag = "ts",     Id = 0x0431 }, /* Tsonga ts_ */
            new LanguageId { Tag = "tn",     Id = 0x0432 }, /* Tswana tn_ */
            new LanguageId { Tag = "tr_TR",  Id = 0x041f }, /* Turkish */
            new LanguageId { Tag = "tl",     Id = 0x0464 }, /* Tagalog */
            new LanguageId { Tag = "tg",     Id = 0x0428 }, /* Tajik */
            new LanguageId { Tag = "bo",     Id = 0x0451 }, /* Tibetan */
            new LanguageId { Tag = "ti",     Id = 0x0473 }, /* Tigrinya */
            new LanguageId { Tag = "uk_UA",  Id = 0x0422 }, /* Ukrainian */
            new LanguageId { Tag = "ur_PK",  Id = 0x0420 }, /* Urdu (Pakistan) */
            new LanguageId { Tag = "ur_IN",  Id = 0x0820 }, /* Urdu (India) */
            new LanguageId { Tag = "uz",     Id = 0x0443 }, /* Uzbek (Latin) uz_ */
            new LanguageId { Tag = "uz",     Id = 0x0843 }, /* Uzbek (Cyrillic) uz_ */
            new LanguageId { Tag = "ven",    Id = 0x0433 }, /* Venda ven_ */
            new LanguageId { Tag = "vi_VN",  Id = 0x042a }, /* Vietnamese */
            new LanguageId { Tag = "cy_GB",  Id = 0x0452 }, /* Welsh */
            new LanguageId { Tag = "xh",     Id = 0x0434 }, /* Xhosa xh */
            new LanguageId { Tag = "yi",     Id = 0x043d }, /* Yiddish yi_ */
            new LanguageId { Tag = "yo",     Id = 0x046a }, /* Yoruba */
            new LanguageId { Tag = "zu",     Id = 0x0435 }, /* Zulu zu_ */
            new LanguageId { Tag = "en_US",  Id = 0x0800 } /* Default */
        };

        public const int VBA_COMPRESSION_WINDOW = 4096;

        #endregion

        #region Utilities

        private static Dictionary<string, GsfMSOleMetaDataPropMap> NameToPropHash = null;

        public static GsfMSOleMetaDataPropMap GsfNameToProp(string name)
        {
            if (NameToPropHash == null)
            {
                NameToPropHash = new Dictionary<string, GsfMSOleMetaDataPropMap>();
                foreach (GsfMSOleMetaDataPropMap prop in GsfMSOleUtils.BuiltInProperties)
                {
                    NameToPropHash[prop.GsfName] = prop;
                }
            }

            if (NameToPropHash.ContainsKey(name))
                return NameToPropHash[name];

            return null;
        }

        #endregion
    }

    public class GsfMSOleMetaDataPropMap
    {
        public string MsName { get; set; }

        public GsfMSOLEMetaDataType Section { get; set; }

        public string GsfName { get; set; }

        public uint Id { get; set; }

        public GsfMSOLEVariantType PreferredType { get; set; }
    }

    public class GsfMSOleMetaDataProp
    {
        public uint Id { get; set; }

        public long Offset { get; set; }
    }

    public class GsfMSOleMetaDataSection
    {
        #region Properties

        public GsfMSOLEMetaDataType Type { get; set; }

        public long Offset { get; set; }

        public uint Size { get; set; }

        public uint NumProps { get; set; }

        public Encoding IConvHandle { get; set; }

        public uint CharSize { get; set; }

        public Dictionary<uint, string> Dict { get; set; }

        #endregion

        #region Functions

        public object PropertyParse(GsfMSOLEVariantType type, byte[] data, ref int dataPtr, int data_end)
        {
            // Not valid in a prop set
            if (((int)type & ~0x1fff) != 0)
                return null;

            object res = null;
            string str = null;
            uint len;
            Exception error;
            int bytes_needed = 0;

            type &= (GsfMSOLEVariantType)0xfff;

            bool is_vector = (type & GsfMSOLEVariantType.VT_VECTOR) != 0;
            if (is_vector)
            {
                //  A vector is basically an array.  If the type associated with
                //  it is a variant, then each element can have a different
                //  variant type.  Otherwise, each element has the same variant
                //  type associated with the vector.

                if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                    return null;

                uint n = BitConverter.ToUInt32(data, dataPtr);
                dataPtr += bytes_needed;

                int size1 = PropMinSize(type);
                if (!NEED_RECS(n, (uint)size1, dataPtr, data_end, ref bytes_needed))
                    return null;

                List<GsfDocProp> vector = new List<GsfDocProp>();

                for (uint i = 0; i < n; i++)
                {
                    int data0 = dataPtr;
                    object v = PropertyParse(type, data, ref dataPtr, data_end);
                    if (v != null)
                        vector.Add(v as GsfDocProp);

                    if (dataPtr == data0)
                        break;
                }

                return vector;
            }

            res = new object();
            switch (type)
            {
                // A property with a type indicator of VT_EMPTY has no data
                // associated with it; that is, the size of the value is zero.
                case GsfMSOLEVariantType.VT_EMPTY:
                    // value::unset == empty
                    break;

                // This is like a pointer to null
                case GsfMSOLEVariantType.VT_NULL:
                    // value::unset == null too :-) do we need to distinguish ?
                    break;

                // 2-byte signed integer
                case GsfMSOLEVariantType.VT_I2:
                    if (!NEED_RECS(2, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = BitConverter.ToInt16(data, dataPtr);
                    dataPtr += bytes_needed;
                    break;

                // 4-byte signed integer
                case GsfMSOLEVariantType.VT_I4:
                    if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = BitConverter.ToInt32(data, dataPtr);
                    dataPtr += bytes_needed;
                    break;

                // 32-bit IEEE floating-point value
                case GsfMSOLEVariantType.VT_R4:
                    if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = BitConverter.ToSingle(data, dataPtr);
                    dataPtr += bytes_needed;
                    break;

                // 64-bit IEEE floating-point value
                case GsfMSOLEVariantType.VT_R8:
                    if (!NEED_RECS(8, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = BitConverter.ToDouble(data, dataPtr);
                    dataPtr += bytes_needed;
                    break;

                // 8-byte two's complement integer (scaled by 10,000)
                case GsfMSOLEVariantType.VT_CY:
                    if (!NEED_RECS(8, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    // CHEAT : just store as an int64 for now
                    res = BitConverter.ToInt64(data, dataPtr);
                    break;

                // 64-bit floating-point number representing the number of days
                // (not seconds) since December 31, 1899.
                case GsfMSOLEVariantType.VT_DATE:
                    if (!NEED_RECS(8, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    dataPtr += bytes_needed;
                    break;

                // Pointer to null-terminated Unicode string; the string is pre-
                // ceeded by a DWORD representing the byte count of the number
                // of bytes in the string (including the  terminating null).
                case GsfMSOLEVariantType.VT_BSTR:
                    if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    dataPtr += bytes_needed;
                    break;

                case GsfMSOLEVariantType.VT_DISPATCH:
                    break;

                // A boolean (WORD) value containg 0 (false) or -1 (true).
                case GsfMSOLEVariantType.VT_BOOL:
                    if (!NEED_RECS(1, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = (data[dataPtr] != 0x00);
                    dataPtr += bytes_needed;
                    break;

                // A type indicator (a DWORD) followed by the corresponding
                // value.  VT_VARIANT is only used in conjunction with
                // VT_VECTOR.
                case GsfMSOLEVariantType.VT_VARIANT:
                    if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = null;
                    type = (GsfMSOLEVariantType)BitConverter.ToUInt32(data, dataPtr);
                    dataPtr += bytes_needed;
                    return PropertyParse(type, data, ref dataPtr, data_end);

                // 1-byte unsigned integer
                case GsfMSOLEVariantType.VT_UI1:
                    if (!NEED_RECS(1, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = data[dataPtr];
                    dataPtr += bytes_needed;
                    break;

                // 1-byte signed integer
                case GsfMSOLEVariantType.VT_I1:
                    if (!NEED_RECS(1, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = (sbyte)data[dataPtr];
                    dataPtr += bytes_needed;
                    break;

                // 2-byte unsigned integer
                case GsfMSOLEVariantType.VT_UI2:
                    if (!NEED_RECS(2, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = BitConverter.ToUInt16(data, dataPtr);
                    dataPtr += bytes_needed;
                    break;

                // 4-type unsigned integer
                case GsfMSOLEVariantType.VT_UI4:
                    if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = BitConverter.ToUInt32(data, dataPtr);
                    dataPtr += bytes_needed;
                    break;

                // 8-byte signed integer
                case GsfMSOLEVariantType.VT_I8:
                    if (!NEED_RECS(8, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = BitConverter.ToInt64(data, dataPtr);
                    dataPtr += bytes_needed;
                    break;

                // 8-byte unsigned integer
                case GsfMSOLEVariantType.VT_UI8:
                    if (!NEED_RECS(8, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    res = BitConverter.ToUInt64(data, dataPtr);
                    dataPtr += bytes_needed;
                    break;

                // This is the representation of many strings.  It is stored in
                // the same representation as VT_BSTR.  Note that the serialized
                // representation of VP_LPSTR has a preceding byte count,
                // whereas the in-memory representation does not.
                case GsfMSOLEVariantType.VT_LPSTR:
                    {
                        if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                            return null;

                        len = BitConverter.ToUInt32(data, dataPtr);
                        dataPtr += bytes_needed;

                        if (len >= 0x10000)
                            return null;

                        uint need = len;
                        if (CharSize > 1 && (need & 3) != 0)
                            need = (uint)((need & ~3) + 4);

                        if (!NEED_RECS(need, 1, dataPtr, data_end, ref bytes_needed))
                            return null;

                        error = null;

                        try
                        {
                            byte[] lpstrTemp = Encoding.Convert(IConvHandle, Encoding.UTF8, data, dataPtr, (int)(len > CharSize ? len - CharSize : 0));
                            str = Encoding.UTF8.GetString(lpstrTemp);
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                        }

                        res = string.Empty;
                        if (str != null)
                        {
                            res = str;
                        }
                        else if (error != null)
                        {
                            Console.Error.WriteLine($"Error: {error.Message}");
                            error = null;
                        }
                        else
                        {
                            Console.Error.WriteLine("unknown error converting string property, using blank");
                        }

                        dataPtr += bytes_needed;
                        break;
                    }

                // A counted and null-terminated Unicode string; a DWORD character
                // count (where the count includes the terminating null) followed
                // by that many Unicode (16-bit) characters.  Note that the count
                // is character count, not byte count.
                case GsfMSOLEVariantType.VT_LPWSTR:
                    if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    len = BitConverter.ToUInt32(data, dataPtr);
                    dataPtr += bytes_needed;

                    if (!NEED_RECS(len, 2, dataPtr, data_end, ref bytes_needed))
                        return null;

                    if (len >= 0x10000)
                        return null;

                    error = null;

                    try
                    {
                        byte[] lpwstrTemp = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, data, dataPtr, (int)len * 2);
                        str = Encoding.UTF8.GetString(lpwstrTemp);
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }

                    res = string.Empty;
                    if (str != null)
                    {
                        res = str;
                    }
                    else if (error != null)
                    {
                        Console.Error.WriteLine($"Error: {error.Message}");
                        error = null;
                    }
                    else
                    {
                        Console.Error.WriteLine("unknown error converting string property, using blank");
                    }

                    dataPtr += bytes_needed;
                    break;

                // 64-bit FILETIME structure, as defined by Win32.
                case GsfMSOLEVariantType.VT_FILETIME:
                    {
                        if (!NEED_RECS(8, 1, dataPtr, data_end, ref bytes_needed))
                            return null;

                        // ft * 100ns since Jan 1 1601
                        ulong ft = BitConverter.ToUInt64(data, dataPtr);
                        res = DateTime.FromFileTime((long)ft);
                        dataPtr += bytes_needed;
                        break;
                    }

                // A DWORD count of bytes, followed by that many bytes of data.
                // The byte count does not include the four bytes for the length
                // of the count itself:  An empty blob would have a count of
                // zero, followed by zero bytes.  Thus the serialized represen-
                // tation of a VT_BLOB is similar to that of a VT_BSTR but does
                // not guarantee a null byte at the end of the data.
                case GsfMSOLEVariantType.VT_BLOB:
                    if (!NEED_RECS(4, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    dataPtr += bytes_needed;
                    res = null;
                    break;

                // Indicates the value is stored in a stream that is sibling
                // to the CONTENTS stream.  Following this type indicator is
                // data in the format of a serialized VT_LPSTR, which names
                // the stream containing the data.
                case GsfMSOLEVariantType.VT_STREAM:
                    res = null;
                    break;

                // Indicates the value is stored in an IStorage that is
                // sibling to the CONTENTS stream.  Following this type
                // indicator is data in the format of a serialized VT_LPSTR,
                // which names the IStorage containing the data.
                case GsfMSOLEVariantType.VT_STORAGE:
                    res = null;
                    break;

                // Same as VT_STREAM, but indicates that the stream contains a
                // serialized object, which is a class ID followed by initiali-
                // zation data for the class.
                case GsfMSOLEVariantType.VT_STREAMED_OBJECT:
                    res = null;
                    break;

                // Same as VT_STORAGE, but indicates that the designated
                // IStorage contains a loadable object.
                case GsfMSOLEVariantType.VT_STORED_OBJECT:
                    res = null;
                    break;

                // Contains a serialized object in the same representation as
                // would appear in a VT_STREAMED_OBJECT.  That is, following
                // the VT_BLOB_OBJECT tag is a DWORD byte count of the
                // remaining data (where the byte count does not include the
                // size of itself) which is in the format of a class ID
                // followed by initialization data for that class
                case GsfMSOLEVariantType.VT_BLOB_OBJECT:
                    res = null;
                    break;

                case GsfMSOLEVariantType.VT_CF:
                    error = null;
                    if (!ParseVariantCF(res, data, ref dataPtr, ref data_end, ref error))
                    {
                        // Suck, we can't propagate the error upwards
                        if (error != null)
                        {
                            Console.Error.WriteLine($"Error: {error.Message}");
                            error = null;
                        }
                        else
                        {
                            Console.Error.WriteLine("Unknown error parsing vt_cf");
                        }

                        res = null;
                    }

                    break;

                // A class ID (or other GUID)
                case GsfMSOLEVariantType.VT_CLSID:
                    if (!NEED_RECS(16, 1, dataPtr, data_end, ref bytes_needed))
                        return null;

                    dataPtr += bytes_needed;
                    res = null;
                    break;

                // A DWORD containing a status code.
                case GsfMSOLEVariantType.VT_ERROR:

                case GsfMSOLEVariantType.VT_UNKNOWN:
                case GsfMSOLEVariantType.VT_DECIMAL:
                case GsfMSOLEVariantType.VT_INT:
                case GsfMSOLEVariantType.VT_UINT:
                case GsfMSOLEVariantType.VT_VOID:
                case GsfMSOLEVariantType.VT_HRESULT:
                case GsfMSOLEVariantType.VT_PTR:
                case GsfMSOLEVariantType.VT_SAFEARRAY:
                case GsfMSOLEVariantType.VT_CARRAY:
                case GsfMSOLEVariantType.VT_USERDEFINED:
                    Console.Error.WriteLine($"Type {VariantName(type)} (0x{type:x}) is not permitted in property sets");
                    res = null;
                    break;

                default:
                    res = null;
                    break;
            }

            return res;
        }

        public bool PropertyRead(GsfInput input, GsfMSOleMetaDataProp[] props, uint i, GsfDocMetaData accum)
        {
            if (i >= NumProps)
                return false;

            long size = ((i + 1) >= NumProps) ? Size : props[i + 1].Offset;
            if (size < props[i].Offset + 4)
                return false;

            string name;
            object val;

            size -= props[i].Offset; // Includes the type id

            // From now on, size is actually a size.
            byte[] data;
            if (input.Seek(Offset + props[i].Offset, SeekOrigin.Begin) || (data = input.Read((int)size, null)) == null)
            {
                Console.Error.WriteLine("Failed to read prop #%d", i);
                return false;
            }

            int dataPtr = 0; // data[0]
            GsfMSOLEVariantType type = (GsfMSOLEVariantType)BitConverter.ToUInt32(data, dataPtr);
            dataPtr += 4;

            // Dictionary is magic
            if (props[i].Id == 0)
            {
                uint len, id;
                int gslen;
                int start = dataPtr; // data[dataPtr]
                int end = start + ((int)size - 4);

                if (Dict != null)
                    return false;

                Dict = new Dictionary<uint, string>();

                uint n = (uint)type;
                for (uint j = 0; j < n; j++)
                {
                    if (end - dataPtr < 8)
                        return false;

                    id = BitConverter.ToUInt32(data, dataPtr);
                    len = BitConverter.ToUInt32(data, dataPtr + 4);

                    if (len >= 0x10000)
                        return false;
                    if (len > end - dataPtr + 8)
                        return false;

                    gslen = 0;

                    try
                    {
                        byte[] convTemp = Encoding.Convert(IConvHandle, Encoding.UTF8, data, dataPtr + 8, (int)(len * CharSize));
                        name = Encoding.UTF8.GetString(convTemp);
                    }
                    catch
                    {
                        name = null;
                    }

                    len = (uint)gslen;
                    dataPtr += (int)(8 + len);

                    Dict[id] = name;

                    // MS documentation blows goats !
                    // The docs claim there are padding bytes in the dictionary.
                    // Their examples show padding bytes.
                    // In reality non-unicode strings do not seem to
                    // have padding.
                    if (CharSize != 1 && ((dataPtr - start) % 4) != 0)
                        dataPtr += 4 - ((dataPtr - start) % 4);
                }
            }
            else
            {
                bool linked = false;

                name = PropIdToGsf(props[i].Id, ref linked);
                val = PropertyParse(type, data, ref dataPtr, (int)(dataPtr + size - 4));

                if (name != null && val != null)
                {
                    if (linked)
                    {
                        GsfDocProp prop = accum.Lookup(name);
                        if (prop == null)
                            Console.Error.WriteLine($"Linking property '{(name != null ? name : "<null>")}' before it's value is specified");
                        else if (!(val is string))
                            Console.Error.WriteLine($"Linking property '{(name != null ? name : "<null>")}' before it's value is specified");
                        else
                            prop.LinkedTo = (val as string);
                    }
                    else
                    {
                        accum.Insert(name, val);
                        val = null;
                        name = null;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Utilities

        private bool NEED_RECS(uint _n, uint _size1, int dataPtr, int data_end, ref int bytes_needed)
        {
            bytes_needed = (int)_n;
            if (_size1 > 0 && (data_end - dataPtr) / _size1 < bytes_needed)
            {
                Console.Error.WriteLine("Invalid MS property or file truncated");
                return false;
            }

            bytes_needed *= (int)_size1;
            return true;
        }

        /// <summary>
        /// Can return errors from gsf_blob_new() and GSF_ERROR_INVALID_DATA
        /// </summary>
        private bool ParseVariantCF(object res, byte[] data, ref int dataPtr, ref int data_end, ref Exception error)
        {
            /* clipboard size		uint		sizeof (clipboard format tag) + sizeof (clipboard data)
             * clipboard format tag		int32		see below
             * clipboard data		byte[]		see below
             *
             * Clipboard format tag:
             * -1 - Windows clipboard format
             * -2 - Macintosh clipboard format
             * -3 - GUID that contains a format identifier (FMTID)
             * >0 - custom clipboard format name plus data (see msdn site below)
             *  0 - No data
             *
             * References:
             * http://msdn.microsoft.com/library/default.asp?url=/library/en-us/stg/stg/propvariant.asp
             * http://jakarta.apache.org/poi/hpsf/thumbnails.html
             * http://linux.com.hk/docs/poi/org/apache/poi/hpsf/Thumbnail.html
             * http://sparks.discreet.com/knowledgebase/public/solutions/ExtractThumbnailImg.htm
             */

            // Clipboard size field

            if (data_end < dataPtr + 4)
            {
                SetErrorMissingData(ref error, "VT_CF", 4, data_end - dataPtr);
                return false;
            }

            uint clip_size = BitConverter.ToUInt32(data, dataPtr);

            if (clip_size < 4)
            {
                // Must emcompass int32 format plus data size
                error = new Exception($"Corrupt data in the VT_CF property; clipboard data length must be at least 4 bytes, but the data says it only has {clip_size} bytes available.");
                return false;
            }

            dataPtr += 4;

            // Check clipboard format plus data size

            if (data_end < dataPtr + clip_size)
            {
                SetErrorMissingData(ref error, "VT_CF", (int)clip_size, data_end - dataPtr);
                return false;
            }

            GsfClipFormat clip_format = (GsfClipFormat)BitConverter.ToInt32(data, dataPtr);
            dataPtr += 4;

            switch (clip_format)
            {
                case GsfClipFormat.GSF_CLIP_FORMAT_WINDOWS_CLIPBOARD:
                case GsfClipFormat.GSF_CLIP_FORMAT_MACINTOSH_CLIPBOARD:
                case GsfClipFormat.GSF_CLIP_FORMAT_GUID:
                case GsfClipFormat.GSF_CLIP_FORMAT_NO_DATA:
                    // Everything is ok 
                    break;

                default:
                    if (clip_format > 0)
                        clip_format = GsfClipFormat.GSF_CLIP_FORMAT_CLIPBOARD_FORMAT_NAME;
                    else
                        clip_format = GsfClipFormat.GSF_CLIP_FORMAT_UNKNOWN;

                    break;
            }

            int clip_data_size = (int)clip_size - 4;

            GsfBlob blob = GsfBlob.Create(clip_data_size, data, dataPtr, ref error);

            dataPtr += clip_data_size;

            if (blob == null)
                return false;

            GsfClipData clip_data = GsfClipData.Create(clip_format, blob);
            res = clip_data;

            return true;
        }

        private string PropIdToGsf(uint id, ref bool linked)
        {
            linked = false;
            if (Dict != null)
            {
                if ((id & 0x1000000) != 0)
                {
                    linked = true;
                    unchecked { id &= (uint)~0x1000000; }
                }

                if (Dict.TryGetValue(id, out string res) && res != null)
                    return res;
            }

            GsfMSOleMetaDataPropMap[] map = GsfMSOleUtils.BuiltInProperties;
            int i = GsfMSOleUtils.BuiltInProperties.Length;
            while (i-- > 0)
            {
                if (map[i].Id == id && (map[i].Section == GsfMSOLEMetaDataType.COMMON_PROP || map[i].Section == Type))
                    return map[i].GsfName;
            }

            return null;
        }

        /// <summary>
        /// Return a number no bigger than the number of bytes used for a property
        /// value of a given type.  The returned number might be too small, but
        /// we try to return as big a value as possible.
        /// </summary>
        private int PropMinSize(GsfMSOLEVariantType type)
        {
            switch (type)
            {
                case GsfMSOLEVariantType.VT_EMPTY:
                case GsfMSOLEVariantType.VT_NULL:
                    return 0;

                case GsfMSOLEVariantType.VT_BOOL:
                case GsfMSOLEVariantType.VT_I1:
                case GsfMSOLEVariantType.VT_UI1:
                    return 1;

                case GsfMSOLEVariantType.VT_I2:
                case GsfMSOLEVariantType.VT_UI2:
                    return 2;

                case GsfMSOLEVariantType.VT_I4:
                case GsfMSOLEVariantType.VT_R4:
                case GsfMSOLEVariantType.VT_ERROR:
                case GsfMSOLEVariantType.VT_VARIANT:
                case GsfMSOLEVariantType.VT_UI4:
                case GsfMSOLEVariantType.VT_LPSTR:
                case GsfMSOLEVariantType.VT_LPWSTR:
                case GsfMSOLEVariantType.VT_BLOB:
                case GsfMSOLEVariantType.VT_BLOB_OBJECT:
                case GsfMSOLEVariantType.VT_CF:
                case GsfMSOLEVariantType.VT_VECTOR:
                    return 4;

                case GsfMSOLEVariantType.VT_BSTR:
                    return 5;

                case GsfMSOLEVariantType.VT_R8:
                case GsfMSOLEVariantType.VT_CY:
                case GsfMSOLEVariantType.VT_DATE:
                case GsfMSOLEVariantType.VT_I8:
                case GsfMSOLEVariantType.VT_UI8:
                case GsfMSOLEVariantType.VT_FILETIME:
                    return 8;

                case GsfMSOLEVariantType.VT_CLSID:
                    return 16;

                case GsfMSOLEVariantType.VT_DISPATCH:
                case GsfMSOLEVariantType.VT_UNKNOWN:
                case GsfMSOLEVariantType.VT_DECIMAL:
                case GsfMSOLEVariantType.VT_INT:
                case GsfMSOLEVariantType.VT_UINT:
                case GsfMSOLEVariantType.VT_VOID:
                case GsfMSOLEVariantType.VT_HRESULT:
                case GsfMSOLEVariantType.VT_PTR:
                case GsfMSOLEVariantType.VT_SAFEARRAY:
                case GsfMSOLEVariantType.VT_CARRAY:
                case GsfMSOLEVariantType.VT_USERDEFINED:
                case GsfMSOLEVariantType.VT_STREAM:
                case GsfMSOLEVariantType.VT_STORAGE:
                case GsfMSOLEVariantType.VT_STREAMED_OBJECT:
                case GsfMSOLEVariantType.VT_STORED_OBJECT:
                default:
                    return 0;
            }
        }

        private void SetErrorMissingData(ref Exception error, string property_name, int size_needed, int size_gotten)
        {
            error = new Exception($"Missing data when reading the {property_name} property; got {size_needed} bytes, but {size_gotten} bytes at least are needed.");
        }

        private string VariantName(GsfMSOLEVariantType type)
        {
            string[] names =
            {
                "VT_EMPTY", "VT_null",  "VT_I2",    "VT_I4",    "VT_R4",
                "VT_R8",    "VT_CY",    "VT_DATE",  "VT_BSTR",  "VT_DISPATCH",
                "VT_ERROR", "VT_BOOL",  "VT_VARIANT",   "VT_UNKNOWN",   "VT_DECIMAL",
                null,       "VT_I1",    "VT_UI1",   "VT_UI2",   "VT_UI4",
                "VT_I8",    "VT_UI8",   "VT_INT",   "VT_UINT",  "VT_VOID",
                "VT_HRESULT",   "VT_PTR",   "VT_SAFEARRAY", "VT_CARRAY",    "VT_USERDEFINED",
                "VT_LPSTR", "VT_LPWSTR",
            };

            string[] names2 =
            {
                "VT_FILETIME",
                "VT_BLOB",  "VT_STREAM",    "VT_STORAGE",   "VT_STREAMED_OBJECT",
                "VT_STORED_OBJECT", "VT_BLOB_OBJECT", "VT_CF",  "VT_CLSID"
            };

            type &= ~GsfMSOLEVariantType.VT_VECTOR;
            if (type <= GsfMSOLEVariantType.VT_LPWSTR)
                return names[(int)type];

            if (type < GsfMSOLEVariantType.VT_FILETIME)
                return "_UNKNOWN_";
            if (type > GsfMSOLEVariantType.VT_CLSID)
                return "_UNKNOWN_";

            return names2[type - GsfMSOLEVariantType.VT_FILETIME];
        }

        #endregion
    }

    public class WritePropState
    {
        #region Properties

        public GsfOutput Output { get; set; }

        public bool DocNotComponent { get; set; }

        public Dictionary<string, uint> Dict { get; set; }

        public WritePropStatePropList BuiltIn { get; set; }

        public WritePropStatePropList User { get; set; }

        public int CodePage { get; set; }

        public Encoding IConvHandle { get; set; }

        public int CharSize { get; set; }

        #endregion

        #region Functions

        public void GuessCodePage(bool user)
        {
            List<GsfDocProp> ptr = user ? User.Props : BuiltIn.Props;
            uint count = user ? User.Count : BuiltIn.Count;
            uint i = 0;

            if (i < count)
            {
                // CodePage
                i++;
            }

            if (user && i < count)
            {
                // Dictionary
                i++;
            }

            foreach (GsfDocProp prop in ptr)
            {
                string name = prop.Name;
                GuessCodePageString(name);
                GuessCodePageProperty(name, prop.Value);
            }
        }

        public void CountProperties(string name, GsfDocProp prop)
        {
            GsfMSOleMetaDataPropMap map = GsfMSOleUtils.GsfNameToProp(name);

            // Allocate predefined ids or add it to the dictionary
            if (map != null)
            {
                // Dictionary is handled elsewhere
                if (map.Id == 0)
                    return;

                if (map.Section == (DocNotComponent ? GsfMSOLEMetaDataType.COMPONENT_PROP : GsfMSOLEMetaDataType.DOC_PROP))
                    return;

                // CodePage
                if (map.Id == 1)
                {
                    object val = prop.Value;
                    if (val != null && val is int intVal)
                        CodePage = intVal;

                    return;
                }

                BuiltIn.Count += (uint)(prop.LinkedTo != null ? 2 : 1);
                BuiltIn.Props.Add(prop);
            }

            // Keep user props in the document
            else if (DocNotComponent)
            {
                if (Dict == null)
                    Dict = new Dictionary<string, uint>();

                Dict[name] = User.Count;
                User.Count += (uint)(prop.LinkedTo != null ? 2 : 1);
                User.Props.Add(prop);
            }
        }

        public bool WriteSection(bool user)
        {
            if (user && Dict == null)
                return true;

            List<GsfDocProp> ptr = user ? User.Props : BuiltIn.Props;
            uint count = user ? User.Count : BuiltIn.Count;
            long baseOffset = Output.CurrentOffset;

            // Skip past the size+count and id/offset pairs
            byte[] buf = BitConverter.GetBytes((uint)0);
            for (int j = 0; j < 1 + 1 + 2 * count; j++)
            {
                Output.Write(4, buf);
            }

            object scratch = string.Empty;

            GsfMSOleMetaDataProp[] offsets = new GsfMSOleMetaDataProp[count];

            // 0) CodePage
            uint i = 0;
            if (i < count)
            {
                offsets[0].Id = 1;
                offsets[0].Offset = Output.CurrentOffset;
                buf = BitConverter.GetBytes((uint)GsfMSOLEVariantType.VT_I2);
                byte[] buf2 = BitConverter.GetBytes(CodePage);
                Output.Write(4, buf); Output.Write(4, buf2);
                i++;
            }

            // 1) Dictionary
            if (user && i < count)
            {
                offsets[1].Id = 0;
                offsets[1].Offset = Output.CurrentOffset;
                buf = BitConverter.GetBytes((uint)Dict.Count);
                Output.Write(4, buf);
                foreach (var kvp in Dict)
                {
                    WriteDictionaryEntry(kvp.Key, kvp.Value);
                }

                i++;
            }

            // 2) Props
            foreach (GsfDocProp prop in ptr)
            {
                offsets[i].Offset = Output.CurrentOffset;
                string name = prop.Name;
                if (user)
                {
                    Dict.TryGetValue(name, out uint tmp);
                    offsets[i].Id = tmp;
                    if (offsets[i].Id < 2)
                    {
                        Console.Error.WriteLine($"Invalid ID (%d) for custom name '%s'", offsets[i].Id, name);
                        continue;
                    }
                }
                else
                {
                    GsfMSOleMetaDataPropMap map = GsfMSOleUtils.GsfNameToProp(name);
                    if (map == null)
                    {
                        Console.Error.WriteLine("Missing map for built-in property '%s'", name);
                        continue;
                    }

                    offsets[i].Id = map.Id;
                }

                WriteProperty(name, prop.Value, false);
                if (prop.LinkedTo != null)
                {
                    i++;
                    offsets[i].Id = offsets[i - 1].Id | 0x1000000;
                    offsets[i].Offset = Output.CurrentOffset;
                    scratch = prop.LinkedTo;
                    WriteProperty(null, scratch, false);
                }
            }

            bool warned = false;
            while (i < count)
            {
                if (!warned)
                {
                    warned = true;
                    Console.Error.WriteLine("Something strange in MetadataWriteSection");
                }

                offsets[i].Id = 0;
                offsets[i].Offset = offsets[i - 1].Offset;
                i++;
            }

            long len = Output.CurrentOffset - baseOffset;
            Output.Seek(baseOffset, SeekOrigin.Begin);
            buf = BitConverter.GetBytes(len);
            Output.Write(4, buf);
            buf = BitConverter.GetBytes(count);
            Output.Write(4, buf);

            for (i = 0; i < count; i++)
            {
                buf = BitConverter.GetBytes(offsets[i].Id);
                Output.Write(4, buf);
                buf = BitConverter.GetBytes(offsets[i].Offset - baseOffset);
                Output.Write(4, buf);
            }

            return Output.Seek(0, SeekOrigin.End);
        }

        #endregion

        #region Utilities

        private void GuessCodePageProperty(string name, object value)
        {
            GsfMSOleMetaDataPropMap map = (name != null) ? GsfMSOleUtils.GsfNameToProp(name) : null;
            GsfMSOLEVariantType type = ValueToMSOLEVariant(value, map);

            if (type.HasFlag(GsfMSOLEVariantType.VT_VECTOR))
            {
                GsfDocProp[] vector = value as GsfDocProp[];
                int n = vector.Length;
                for (int i = 0; i < n; i++)
                {
                    GuessCodePageProperty(null, vector[i]);
                }

                return;
            }

            switch (type)
            {
                case GsfMSOLEVariantType.VT_LPSTR:
                    GuessCodePageString((string)value);
                    return;

                default:
                    // Don't care.
                    return;
            }
        }

        private void GuessCodePageString(string str)
        {
            if (CodePage != 0)
                return;

            if (str == null)
                return;

            // Don't bother with ASCII strings
            bool is_ascii = true;
            for (int p = 0; p < str.Length && is_ascii; p++)
            {
                is_ascii = (str[p] & 0x80) == 0;
            }

            if (is_ascii)
                return;

            try
            {
                byte[] cstrTemp = IConvHandle.GetBytes(str);
                cstrTemp = Encoding.Convert(IConvHandle, Encoding.UTF8, cstrTemp);
                string cstr = Encoding.UTF8.GetString(cstrTemp);
            }
            catch
            {
                // Conversion failed.  Switch to UTF-8
                CodePage = -535;
            }
        }

        private GsfMSOLEVariantType ValueToMSOLEVariant(object value, GsfMSOleMetaDataPropMap map)
        {
            if (value == null)
                return GsfMSOLEVariantType.VT_EMPTY;

            if (value is bool)
                return GsfMSOLEVariantType.VT_BOOL;
            else if (value is char)
                return GsfMSOLEVariantType.VT_UI1;
            else if (value is float)
                return GsfMSOLEVariantType.VT_R4;
            else if (value is double)
                return GsfMSOLEVariantType.VT_R8;
            else if (value is string)
                return GsfMSOLEVariantType.VT_LPSTR;
            else if (value is int)
                return (map?.PreferredType == GsfMSOLEVariantType.VT_I2 ? GsfMSOLEVariantType.VT_I2 : GsfMSOLEVariantType.VT_I4);
            else if (value is uint)
                return (map?.PreferredType == GsfMSOLEVariantType.VT_UI2 ? GsfMSOLEVariantType.VT_UI2 : GsfMSOLEVariantType.VT_UI4);
            else if (value is DateTime)
                return GsfMSOLEVariantType.VT_FILETIME;

            if (value is List<GsfDocProp> vector)
            {
                GsfMSOLEVariantType type, tmp;

                if (vector == null)
                    return GsfMSOLEVariantType.VT_UNKNOWN;

                if (map != null)
                {
                    type = map.PreferredType & (~GsfMSOLEVariantType.VT_VECTOR);
                    if (type == GsfMSOLEVariantType.VT_VARIANT)
                        return GsfMSOLEVariantType.VT_VECTOR | GsfMSOLEVariantType.VT_VARIANT;
                }
                else
                {
                    type = GsfMSOLEVariantType.VT_UNKNOWN;
                }

                int n = vector.Count;
                for (int i = 0; i < n; i++)
                {
                    tmp = ValueToMSOLEVariant(vector[i], null);
                    if (type == GsfMSOLEVariantType.VT_UNKNOWN)
                        type = tmp;
                    else if (type != tmp)
                        return GsfMSOLEVariantType.VT_VECTOR | GsfMSOLEVariantType.VT_VARIANT;
                }

                return GsfMSOLEVariantType.VT_VECTOR | type;
            }

            return GsfMSOLEVariantType.VT_UNKNOWN;
        }

        private void WriteDictionaryEntry(string name, uint id)
        {
            byte[] buf = BitConverter.GetBytes(id);
            Output.Write(4, buf);
            WriteString(name);
        }

        /// <returns>True on success</returns>
        private bool WriteProperty(string name, object value, bool suppress_type)
        {
            if (value == null)
                return false;

            GsfMSOleMetaDataPropMap map = (name != null) ? GsfMSOleUtils.GsfNameToProp(name) : null;
            byte[] buf = new byte[8];

            GsfMSOLEVariantType type = ValueToMSOLEVariant(value, map);
            if (!suppress_type)
            {
                buf = BitConverter.GetBytes((uint)type);
                Output.Write(4, buf);
            }

            if (type.HasFlag(GsfMSOLEVariantType.VT_VECTOR))
            {
                GsfDocProp[] vector = value as GsfDocProp[];
                int n = vector.Length;
                bool res;

                buf = BitConverter.GetBytes(n);
                res = Output.Write(4, buf);
                for (int i = 0; i < n; i++)
                {
                    bool suppress = type != (GsfMSOLEVariantType.VT_VECTOR | GsfMSOLEVariantType.VT_VARIANT);
                    res &= WriteProperty(null, vector[i], suppress);
                }

                return res;
            }

            switch (type)
            {
                case GsfMSOLEVariantType.VT_BOOL:
                    if ((bool)value)
                        buf = BitConverter.GetBytes(0xffffffff);
                    else
                        buf = BitConverter.GetBytes(0);

                    return Output.Write(4, buf);

                case GsfMSOLEVariantType.VT_UI1:
                    buf = BitConverter.GetBytes((int)(byte)value);
                    return Output.Write(4, buf);

                case GsfMSOLEVariantType.VT_I2:
                    buf = BitConverter.GetBytes((short)value);
                    byte[] buf2 = BitConverter.GetBytes((short)0);
                    return Output.Write(2, buf) && Output.Write(2, buf2);

                case GsfMSOLEVariantType.VT_I4:
                    buf = BitConverter.GetBytes((int)value);
                    return Output.Write(4, buf);

                case GsfMSOLEVariantType.VT_UI2:
                case GsfMSOLEVariantType.VT_UI4:
                    buf = BitConverter.GetBytes((uint)value);
                    return Output.Write(4, buf);

                case GsfMSOLEVariantType.VT_R4:
                    buf = BitConverter.GetBytes((float)value);
                    return Output.Write(4, buf);

                case GsfMSOLEVariantType.VT_R8:
                    buf = BitConverter.GetBytes((double)value);
                    return Output.Write(8, buf);

                case GsfMSOLEVariantType.VT_LPSTR:
                    return WriteString((string)value);

                case GsfMSOLEVariantType.VT_FILETIME:
                    {
                        DateTime ts = (DateTime)value;
                        ulong ft = (ulong)ts.ToFileTime();
                        buf = BitConverter.GetBytes(ft);
                        return Output.Write(8, buf);
                    }

                default:
                    break;
            }

            Console.Error.WriteLine($"Ignoring property '{(name != null ? name : "<unnamed>")}', how do we export a property of type '{value.GetType()}'");
            return false;
        }

        private bool WriteString(string txt)
        {
            byte[] buf = new byte[4];
            int bytes_written;
            bool res;

            if (txt == null)
                txt = "";

            int len = txt.Length;

            byte[] ctxt;
            try
            {
                ctxt = Encoding.Unicode.GetBytes(txt);
                ctxt = Encoding.Convert(IConvHandle, Encoding.UTF8, ctxt);
                bytes_written = ctxt.Length;
            }
            catch
            {
                ctxt = null;
                bytes_written = 0;
            }

            if (ctxt == null)
            {
                // See bug #703952
                Console.Error.WriteLine("Failed to write metadata string");
                bytes_written = 0;
            }

            // *Bytes*, not characters, including the termination, but not the padding.
            buf = BitConverter.GetBytes(bytes_written + CharSize);
            res = Output.Write(4, buf);

            res = res && Output.Write(bytes_written, ctxt);

            buf = BitConverter.GetBytes((uint)0);
            res = res && Output.Write((int)CharSize, buf);

            if (CharSize > 1)
            {
                uint padding = (uint)(4 - (bytes_written + CharSize) % 4);
                if (padding < 4)
                    res = res && Output.Write((int)padding, buf);
            }

            return res;
        }

        #endregion
    }

    public class WritePropStatePropList
    {
        /// <summary>
        /// Includes 2nd prop for links
        /// </summary>
        public uint Count { get; set; }

        public List<GsfDocProp> Props { get; set; }
    }

    public class LanguageId
    {
        #region Properties

        public string Tag { get; set; }

        public uint Id { get; set; }

        #endregion

        #region Functions

        /// <param name="lang">Language id, i.e., locale name.</param>
        /// <returns>
        /// The LID (Language Identifier) for the input language.
        /// If lang is %null, return 0x0400 ("-none-"), and not 0x0000 ("no proofing")
        /// </returns>
        public static uint LanguageIdForLanguage(string lang)
        {
            if (lang == null)
                return 0x0400;   // Return -none-

            // Allow lang to match as a prefix (eg fr == fr_FR@euro)
            int len = lang.Length;
            for (int i = 0; i < GsfMSOleUtils.MSOLELanguageIds.Length; i++)
            {
                if (lang.Equals(GsfMSOleUtils.MSOLELanguageIds[i].Tag.Substring(0, Math.Min(len, GsfMSOleUtils.MSOLELanguageIds[i].Tag.Length))))
                    return GsfMSOleUtils.MSOLELanguageIds[i].Id;
            }

            return 0x0400;   // Return -none-
        }

        /// <param name="lid">Numerical language id</param>
        /// <returns>
        /// The xx_YY style string (can be just xx or
        /// xxx) for the given LID.  If the LID is not found, is set to 0x0400,
        /// or is set to 0x0000, will return "-none-"
        /// </returns>
        public static string LanguageForLanguageId(uint lid)
        {
            for (int i = 0; i < GsfMSOleUtils.MSOLELanguageIds.Length; i++)
            {
                if (GsfMSOleUtils.MSOLELanguageIds[i].Id == lid)
                    return GsfMSOleUtils.MSOLELanguageIds[i].Tag;
            }

            return "-none-"; // Default
        }

        /// <summary>Convert the the codepage into an applicable LID</summary>
        /// <param name="codepage">Character code page.</param>
        public static uint CodePageToLanguageId(int codepage)
        {
            switch (codepage)
            {
                case 77:        /* MAC_CHARSET */
                    return 0xFFF;   /* This number is a hack */
                case 128:       /* SHIFTJIS_CHARSET */
                    return 0x411;   /* Japanese */
                case 129:       /* HANGEUL_CHARSET */
                    return 0x412;   /* Korean */
                case 130:       /* JOHAB_CHARSET */
                    return 0x812;   /* Korean (Johab) */
                case 134:       /* GB2312_CHARSET - Chinese Simplified */
                    return 0x804;   /* China PRC - And others!! */
                case 136:       /* CHINESEBIG5_CHARSET - Chinese Traditional */
                    return 0x404;   /* Taiwan - And others!! */
                case 161:       /* GREEK_CHARSET */
                    return 0x408;   /* Greek */
                case 162:       /* TURKISH_CHARSET */
                    return 0x41f;   /* Turkish */
                case 163:       /* VIETNAMESE_CHARSET */
                    return 0x42a;   /* Vietnamese */
                case 177:       /* HEBREW_CHARSET */
                    return 0x40d;   /* Hebrew */
                case 178:       /* ARABIC_CHARSET */
                    return 0x01;    /* Arabic */
                case 186:       /* BALTIC_CHARSET */
                    return 0x425;   /* Estonian - And others!! */
                case 204:       /* RUSSIAN_CHARSET */
                    return 0x419;   /* Russian - And others!! */
                case 222:       /* THAI_CHARSET */
                    return 0x41e;   /* Thai */
                case 238:       /* EASTEUROPE_CHARSET */
                    return 0x405;   /* Czech - And many others!! */
            }

            /* default */
            return 0x0;
        }

        /// <param name="lid">Numerical language id</param>
        /// <returns>Our best guess at the codepage for the given language id</returns>
        public static int LanguageIdToCodePage(uint lid)
        {
            if (lid == 0x0FFF) /* Macintosh Hack */
                return 0x0FFF;

            switch (lid & 0xff)
            {
                case 0x01:      /* Arabic */
                    return 1256;
                case 0x02:      /* Bulgarian */
                    return 1251;
                case 0x03:      /* Catalan */
                    return 1252;
                case 0x04:      /* Chinese */
                    switch (lid)
                    {
                        case 0x1004:        /* Chinese (Singapore) */
                        case 0x0404:        /* Chinese (Taiwan) */
                        case 0x1404:        /* Chinese (Macau SAR) */
                        case 0x0c04:        /* Chinese (Hong Kong SAR, PRC) */
                            return 950;

                        case 0x0804:        /* Chinese (PRC) */
                            return 936;
                        default:
                            break;
                    }
                    break;
                case 0x05:      /* Czech */
                    return 1250;
                case 0x06:      /* Danish */
                    return 1252;
                case 0x07:      /* German */
                    return 1252;
                case 0x08:      /* Greek */
                    return 1253;
                case 0x09:      /* English */
                    return 1252;
                case 0x0a:      /* Spanish */
                    return 1252;
                case 0x0b:      /* Finnish */
                    return 1252;
                case 0x0c:      /* French */
                    return 1252;
                case 0x0d:      /* Hebrew */
                    return 1255;
                case 0x0e:      /* Hungarian */
                    return 1250;
                case 0x0f:      /* Icelandic */
                    return 1252;
                case 0x10:      /* Italian */
                    return 1252;
                case 0x11:      /* Japanese */
                    return 932;
                case 0x12:      /* Korean */
                    switch (lid)
                    {
                        case 0x0812:        /* Korean (Johab) */
                            return 1361;
                        case 0x0412:        /* Korean */
                            return 949;
                        default:
                            break;
                    }
                    break;
                case 0x13:      /* Dutch */
                    return 1252;
                case 0x14:      /* Norwegian */
                    return 1252;
                case 0x15:      /* Polish */
                    return 1250;
                case 0x16:      /* Portuguese */
                    return 1252;
                case 0x17:      /* Rhaeto-Romanic */
                    return 1252;
                case 0x18:      /* Romanian */
                    return 1250;
                case 0x19:      /* Russian */
                    return 1251;
                case 0x1a:      /* Serbian, Croatian, (Bosnian?) */
                    switch (lid)
                    {
                        case 0x041a:        /* Croatian */
                            return 1252;
                        case 0x0c1a:        /* Serbian (Cyrillic) */
                            return 1251;
                        case 0x081a:        /* Serbian (Latin) */
                            return 1252;
                        default:
                            break;
                    }
                    break;
                case 0x1b:      /* Slovak */
                    return 1250;
                case 0x1c:      /* Albanian */
                    return 1251;
                case 0x1d:      /* Swedish */
                    return 1252;
                case 0x1e:      /* Thai */
                    return 874;
                case 0x1f:      /* Turkish */
                    return 1254;
                case 0x20:      /* Urdu. This is Unicode only. */
                    return 0;
                case 0x21:      /* Bahasa Indonesian */
                    return 1252;
                case 0x22:      /* Ukrainian */
                    return 1251;
                case 0x23:      /* Byelorussian / Belarusian */
                    return 1251;
                case 0x24:      /* Slovenian */
                    return 1250;
                case 0x25:      /* Estonian */
                    return 1257;
                case 0x26:      /* Latvian */
                    return 1257;
                case 0x27:      /* Lithuanian */
                    return 1257;
                case 0x29:      /* Farsi / Persian. This is Unicode only. */
                    return 0;
                case 0x2a:      /* Vietnamese */
                    return 1258;
                case 0x2b:      /* Windows 2000: Armenian. This is Unicode only. */
                    return 0;
                case 0x2c:      /* Azeri */
                    switch (lid)
                    {
                        case 0x082c:        /* Azeri (Cyrillic) */
                            return 1251;
                        default:
                            break;
                    }
                    break;
                case 0x2d:      /* Basque */
                    return 1252;
                case 0x2f:      /* Macedonian */
                    return 1251;
                case 0x36:      /* Afrikaans */
                    return 1252;
                case 0x37:      /* Windows 2000: Georgian. This is Unicode only. */
                    return 0;
                case 0x38:      /* Faeroese */
                    return 1252;
                case 0x39:      /* Windows 2000: Hindi. This is Unicode only. */
                    return 0;
                case 0x3E:      /* Malaysian / Malay */
                    return 1252;
                case 0x41:      /* Swahili */
                    return 1252;
                case 0x43:      /* Uzbek */
                    switch (lid)
                    {
                        case 0x0843:        /* Uzbek (Cyrillic) */
                            return 1251;
                        default:
                            break;
                    }
                    break;
                case 0x45:      /* Windows 2000: Bengali. This is Unicode only. */
                case 0x46:      /* Windows 2000: Punjabi. This is Unicode only. */
                case 0x47:      /* Windows 2000: Gujarati. This is Unicode only. */
                case 0x48:      /* Windows 2000: Oriya. This is Unicode only. */
                case 0x49:      /* Windows 2000: Tamil. This is Unicode only. */
                case 0x4a:      /* Windows 2000: Telugu. This is Unicode only. */
                case 0x4b:      /* Windows 2000: Kannada. This is Unicode only. */
                case 0x4c:      /* Windows 2000: Malayalam. This is Unicode only. */
                case 0x4d:      /* Windows 2000: Assamese. This is Unicode only. */
                case 0x4e:      /* Windows 2000: Marathi. This is Unicode only. */
                case 0x4f:      /* Windows 2000: Sanskrit. This is Unicode only. */
                case 0x55:      /* Myanmar / Burmese. This is Unicode only. */
                case 0x57:      /* Windows 2000: Konkani. This is Unicode only. */
                case 0x61:      /* Windows 2000: Nepali (India). This is Unicode only. */
                    return 0;

#if TESTING
                /******************************************************************
                 * Below this line is untested, unproven, and are just guesses.   *
                 * Insert above and use at your own risk                          *
                 ******************************************************************/

                case 0x042c:    /* Azeri (Latin) */
                case 0x0443:    /* Uzbek (Latin) */
                case 0x30:      /* Sutu */
                    return 1252; /* UNKNOWN, believed to be CP1252 */

                case 0x3f:      /* Kazakh */
                    return 1251; /* JUST UNKNOWN, probably CP1251 */

                case 0x44:      /* Tatar */
                case 0x58:      /* Manipuri */
                case 0x59:      /* Sindhi */
                case 0x60:      /* Kashmiri (India) */
                    return 0; /* UNKNOWN, believed to be Unicode only */
#endif
            };

            // This is just a guess, but it will be a frequent guess
            return 1252;
        }

        /// <param name="lid">Numerical language id</param>
        /// <returns>The Iconv codepage string for the given LID.</returns>
        public static string LanguageIdToCodePageString(uint lid)
        {
            if (lid == 0x0FFF)  /* Macintosh Hack */
                return "MACINTOSH";

            int cp = LanguageIdToCodePage(lid);
            return $"CP{cp}";
        }

        public static List<string> GetCodePageStringList(int codepage)
        {
            List<string> cp_list = new List<string>();

            switch (codepage)
            {
                case 1200:
                    cp_list.Add("UTF-16LE");
                    break;

                case 1201:
                    cp_list.Add("UTF-16BE");
                    break;

                case 0x8000:
                case 10000:
                    cp_list.Add("MACROMAN");
                    cp_list.Add("MACINTOSH");
                    break;

                case -535:
                case 65001:
                    cp_list.Add("UTF-8");
                    break;

                case 0x8001:
                    // According to OOo docs 8001 is a synonym CP1252
                    codepage = 1252;
                    cp_list.Add($"CP{codepage}");
                    break;

                default:
                    cp_list.Add($"CP{codepage}");
                    break;
            }

            return cp_list;
        }

        #endregion
    }

    public class GsfMSOleSortingKey
    {
        #region Properties

        public string Name { get; set; }

        public int Length { get; set; }

        #endregion

        #region Functions

        public static GsfMSOleSortingKey Create(string name)
        {
            GsfMSOleSortingKey res = new GsfMSOleSortingKey();

            if (name == null)
                name = "";

            int name_len = name.Length;

            char[] resNameTemp = new char[name_len + 1];
            res.Length = 0;

            // This code is a bit like g_UTF-8_to_utf16.

            for (int p = 0; p < name.Length; p++)
            {
                char wc = name[p];
                if ((wc & 0x80000000) != 0)
                    break; // Something invalid or incomplete

                if (wc < 0x10000)
                {
                    wc = char.ToUpper(wc);

                    // Let's hope no uppercase char is above 0xffff!
                    resNameTemp[res.Length++] = wc;
                }
                else
                {
                    resNameTemp[res.Length++] = (char)((wc - 0x10000) / 0x400 + 0xd800);
                    resNameTemp[res.Length++] = (char)((wc - 0x10000) % 0x400 + 0xdc00);
                }
            }

            resNameTemp[res.Length] = '\0';
            res.Name = new string(resNameTemp);

            return res;
        }

        public GsfMSOleSortingKey Copy()
        {
            return new GsfMSOleSortingKey
            {
                Name = this.Name,
                Length = this.Length,
            };
        }

        #endregion
    }

    #endregion
}
