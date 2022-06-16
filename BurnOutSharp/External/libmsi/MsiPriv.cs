/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002-2005 Mike McCormack for CodeWeavers
 * Copyright 2005 Aric Stewart for CodeWeavers
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
    #region Enums

    internal enum LibmsiCondition
    {
        LIBMSI_CONDITION_FALSE = 0,
        LIBMSI_CONDITION_TRUE  = 1,
        LIBMSI_CONDITION_NONE  = 2,
        LIBMSI_CONDITION_ERROR = 3,
    }

    internal enum LibmsiOLEVariantType
    {
        OLEVT_EMPTY = 0,
        OLEVT_NULL = 1,
        OLEVT_I2 = 2,
        OLEVT_I4 = 3,
        OLEVT_LPSTR = 30,
        OLEVT_FILETIME = 64,
    }

    internal enum StringPersistence
    {
        StringPersistent = 0,
        StringNonPersistent = 1
    }

    #endregion

    #region Delegates

    public delegate LibmsiResult record_func(LibmsiRecord record, object o);

    #endregion

    #region Classes

    internal static class MsiPriv
    {
        #region Constants

        public const ushort MSI_DATASIZEMASK = 0x00ff;
        public const ushort MSITYPE_VALID = 0x0100;
        public const ushort MSITYPE_LOCALIZABLE = 0x200;
        public const ushort MSITYPE_STRING = 0x0800;
        public const ushort MSITYPE_NULLABLE = 0x1000;
        public const ushort MSITYPE_KEY = 0x2000;
        public const ushort MSITYPE_TEMPORARY = 0x4000;
        public const ushort MSITYPE_UNKNOWN = 0x8000;

        public const int MAX_STREAM_NAME_LEN = 62;
        public const int LONG_STR_BYTES = 3;

        public const int MSI_INITIAL_MEDIA_TRANSFORM_OFFSET = 10000;
        public const int MSI_INITIAL_MEDIA_TRANSFORM_DISKID = 30000;

        public const int MSI_MAX_PROPS = 20;

        /* common strings */
        public static readonly string szEmpty = "";
        public static readonly string szStreams = "_Streams";
        public static readonly string szStorages = "_Storages";
        public static readonly string szStringData = "_StringData";
        public static readonly string szStringPool = "_StringPool";
        public static readonly string szName = "Name";
        public static readonly string szData = "Data";

        #endregion

        #region Functions

        public static bool MSITYPE_IS_BINARY(int type) => ((type & ~MSITYPE_NULLABLE) == (MSITYPE_STRING | MSITYPE_VALID));

        #endregion
    }

    #endregion
}