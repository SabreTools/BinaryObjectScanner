/*
 * Copyright (C) 2002,2003 Mike McCormack
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301, USA
 */

namespace LibMSI
{
    /// <remarks>Also known as LibmsiResultError</remarks>
    public enum LibmsiResult
    {
        LIBMSI_RESULT_SUCCESS, /* FIXME: remove me */
        LIBMSI_RESULT_ACCESS_DENIED,
        LIBMSI_RESULT_INVALID_HANDLE,
        LIBMSI_RESULT_NOT_ENOUGH_MEMORY,
        LIBMSI_RESULT_INVALID_DATA,
        LIBMSI_RESULT_OUTOFMEMORY,
        LIBMSI_RESULT_INVALID_PARAMETER,
        LIBMSI_RESULT_OPEN_FAILED,
        LIBMSI_RESULT_CALL_NOT_IMPLEMENTED,
        LIBMSI_RESULT_MORE_DATA,
        LIBMSI_RESULT_NOT_FOUND,
        LIBMSI_RESULT_CONTINUE,
        LIBMSI_RESULT_UNKNOWN_PROPERTY,
        LIBMSI_RESULT_BAD_QUERY_SYNTAX,
        LIBMSI_RESULT_INVALID_FIELD,
        LIBMSI_RESULT_FUNCTION_FAILED,
        LIBMSI_RESULT_INVALID_TABLE,
        LIBMSI_RESULT_DATATYPE_MISMATCH,
        LIBMSI_RESULT_INVALID_DATATYPE,

        NO_MORE_ITEMS = int.MaxValue,
    }

    public enum LibmsiPropertyType
    {
        LIBMSI_PROPERTY_TYPE_EMPTY = 0,
        LIBMSI_PROPERTY_TYPE_INT = 1,
        LIBMSI_PROPERTY_TYPE_STRING = 2,
        LIBMSI_PROPERTY_TYPE_FILETIME = 3,
    }

    public enum LibmsiColInfo
    {
        LIBMSI_COL_INFO_NAMES = 0,
        LIBMSI_COL_INFO_TYPES = 1
    }

    public enum LibmsiDbFlags
    {
        LIBMSI_DB_FLAGS_READONLY   = 1 << 0,
        LIBMSI_DB_FLAGS_CREATE     = 1 << 1,
        LIBMSI_DB_FLAGS_TRANSACT   = 1 << 2,
        LIBMSI_DB_FLAGS_PATCH      = 1 << 3,
    }

    public enum LibmsiDBError
    {
        LIBMSI_DB_ERROR_SUCCESS, /* FIXME: remove me */
        LIBMSI_DB_ERROR_INVALIDARG,
        LIBMSI_DB_ERROR_MOREDATA,
        LIBMSI_DB_ERROR_FUNCTIONERROR,
        LIBMSI_DB_ERROR_DUPLICATEKEY,
        LIBMSI_DB_ERROR_REQUIRED,
        LIBMSI_DB_ERROR_BADLINK,
        LIBMSI_DB_ERROR_OVERFLOW,
        LIBMSI_DB_ERROR_UNDERFLOW,
        LIBMSI_DB_ERROR_NOTINSET,
        LIBMSI_DB_ERROR_BADVERSION,
        LIBMSI_DB_ERROR_BADCASE,
        LIBMSI_DB_ERROR_BADGUID,
        LIBMSI_DB_ERROR_BADWILDCARD,
        LIBMSI_DB_ERROR_BADIDENTIFIER,
        LIBMSI_DB_ERROR_BADLANGUAGE,
        LIBMSI_DB_ERROR_BADFILENAME,
        LIBMSI_DB_ERROR_BADPATH,
        LIBMSI_DB_ERROR_BADCONDITION,
        LIBMSI_DB_ERROR_BADFORMATTED,
        LIBMSI_DB_ERROR_BADTEMPLATE,
        LIBMSI_DB_ERROR_BADDEFAULTDIR,
        LIBMSI_DB_ERROR_BADREGPATH,
        LIBMSI_DB_ERROR_BADCUSTOMSOURCE,
        LIBMSI_DB_ERROR_BADPROPERTY,
        LIBMSI_DB_ERROR_MISSINGDATA,
        LIBMSI_DB_ERROR_BADCATEGORY,
        LIBMSI_DB_ERROR_BADKEYTABLE,
        LIBMSI_DB_ERROR_BADMAXMINVALUES,
        LIBMSI_DB_ERROR_BADCABINET,
        LIBMSI_DB_ERROR_BADSHORTCUT,
        LIBMSI_DB_ERROR_STRINGOVERFLOW,
        LIBMSI_DB_ERROR_BADLOCALIZEATTRIB
    }

    public enum LibmsiProperty
    {
        LIBMSI_PROPERTY_DICTIONARY = 0,
        LIBMSI_PROPERTY_CODEPAGE = 1,
        LIBMSI_PROPERTY_TITLE = 2,
        LIBMSI_PROPERTY_SUBJECT = 3,
        LIBMSI_PROPERTY_AUTHOR = 4,
        LIBMSI_PROPERTY_KEYWORDS = 5,
        LIBMSI_PROPERTY_COMMENTS = 6,
        LIBMSI_PROPERTY_TEMPLATE = 7,
        LIBMSI_PROPERTY_LASTAUTHOR = 8,
        LIBMSI_PROPERTY_UUID = 9,
        LIBMSI_PROPERTY_EDITTIME = 10,
        LIBMSI_PROPERTY_LASTPRINTED = 11,
        LIBMSI_PROPERTY_CREATED_TM = 12,
        LIBMSI_PROPERTY_LASTSAVED_TM = 13,
        LIBMSI_PROPERTY_VERSION = 14,
        LIBMSI_PROPERTY_SOURCE = 15,
        LIBMSI_PROPERTY_RESTRICT = 16,
        LIBMSI_PROPERTY_THUMBNAIL = 17,
        LIBMSI_PROPERTY_APPNAME = 18,
        LIBMSI_PROPERTY_SECURITY = 19
    }

    public static class LibmsiTypes
    {
        public const uint LIBMSI_NULL_INT = 0x80000000;
    }
}