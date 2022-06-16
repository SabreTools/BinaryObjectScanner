/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002, 2005 Mike McCormack for CodeWeavers
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGSF.Input;
using static LibMSI.MsiPriv;

namespace LibMSI
{
    internal class LibmsiOLEVariant
    {
        public LibmsiOLEVariantType VariantType { get; set; }

        #region Union ()

        public int IntVal
        {
            get
            {
                try { return (int)o; }
                catch { return default; }
            }
            set => o = value;
        }

        public string StrVal
        {
            get => o as string;
            set => o = value;
        }

        public ulong FileTime
        {
            get
            {
                try { return (ulong)o; }
                catch { return default; }
            }
            set => o = value;
        }

        private object o = new object();

        #endregion
    }

    public class LibmsiSummaryInfo
    {
        #region Constants

        private static readonly string szSumInfo = (char)0x05 + "SummaryInformation";
        private static readonly byte[] fmtid_SummaryInformation = new byte[]
            { 0xe0, 0x85, 0x9f, 0xf2, 0xf9, 0x4f, 0x68, 0x10, 0xab, 0x91, 0x08, 0x00, 0x2b, 0x27, 0xb3, 0xd9};

        #endregion

        #region Properties

        internal LibmsiDatabase Database { get; set; }

        internal int UpdateCount { get; set; }

        internal LibmsiOLEVariant[] Property { get; set; } = new LibmsiOLEVariant[MSI_MAX_PROPS];

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiSummaryInfo() { }

        /// <summary>
        /// If @database is provided, the summary informations will be
        /// populated during creation, and the libmsi_summary_info_persist()
        /// function will save the properties to it. If @database is null, you
        /// may still populate properties and then save them to a particular
        /// database with the libmsi_summary_info_save() function.
        /// </summary>
        /// <param name="database">An optional associated LibmsiDatabase</param>
        /// <param name="update_count">Number of changes allowed</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>A LibmsiSummaryInfo or null on failure</returns>
        public static LibmsiSummaryInfo Create(LibmsiDatabase database, int update_count, ref Exception error)
        {
            if (error != null)
                return null;

            LibmsiSummaryInfo self = new LibmsiSummaryInfo
            {
                Database = database,
                UpdateCount = update_count,
            };

            // Read the stream... if we fail, we'll start with an empty property set
            if (self.Database != null)
            {
                LibmsiResult r = self.Database.MsiGetRawStream(szSumInfo, out GsfInput stm);
                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    LoadSummaryInfo(self, stm);
            }

            return self;

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~LibmsiSummaryInfo()
        {
            for (int i = 0; i < MSI_MAX_PROPS; i++)
            {
                FreeProp(Property[i]);
            }
        }

        #endregion

        #region Functions

        /// <param name="prop">A LibmsiProperty to get</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>The property type associated for <param name="prop"/>.</returns.
        public LibmsiPropertyType GetPropertyType(LibmsiProperty prop, ref Exception error)
        {
            if (error != null)
                return LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_EMPTY;

            if ((int)prop >= MSI_MAX_PROPS)
            {
                error = new Exception($"Unknown property: {prop}");
                return LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_EMPTY;
            }

            switch (Property[(int)prop].VariantType)
            {
                case LibmsiOLEVariantType.OLEVT_I2:
                case LibmsiOLEVariantType.OLEVT_I4:
                    return LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_INT;
                case LibmsiOLEVariantType.OLEVT_LPSTR:
                    return LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_STRING;
                case LibmsiOLEVariantType.OLEVT_FILETIME:
                    return LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_FILETIME;
                case LibmsiOLEVariantType.OLEVT_EMPTY:
                    return LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_EMPTY;
                default:
                    error = new Exception($"Unknown type: {Property[(int)prop].VariantType}");
                    return LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_EMPTY;
            }
        }

        /// <param name="prop">A LibmsiProperty to get</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>The property value or null on failure.</returns.
        public string GetString(LibmsiProperty prop, ref Exception error)
        {
            if (error != null)
                return null;

            SummaryInfoGetProperty(this, prop, out LibmsiPropertyType type, out _, out _, out _, out _, out string val, ref error);
            return val;
        }

        /// <param name="prop">A LibmsiProperty to get</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>The property value or -1 on failure.</returns.
        public int GetInt(LibmsiProperty prop, ref Exception error)
        {
            if (error != null)
                return -1;

            SummaryInfoGetProperty(this, prop, out LibmsiPropertyType type, out int val, out _, out _, out _, out _, ref error);
            return val;
        }

        /// <param name="prop">A LibmsiProperty to get</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>The property value or 0 on failure.</returns.
        public ulong GetFiletime(LibmsiProperty prop, ref Exception error)
        {
            if (error != null)
                return 0;

            SummaryInfoGetProperty(this, prop, out LibmsiPropertyType type, out _, out ulong val, out _, out _, out _, ref error);
            return val;
        }

        /// <summary>
        /// Set string property <paramref name="prop"/>.
        /// </summary>
        /// <param name="prop">A LibmsiProperty to set</param>
        /// <param name="value">A string value</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool SetString(LibmsiProperty prop, string value, ref Exception error)
        {
            if (error != null)
                return false;

            if (GetType(prop) != LibmsiOLEVariantType.OLEVT_LPSTR)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {LibmsiResult.LIBMSI_RESULT_DATATYPE_MISMATCH}");
                return false;
            }

            LibmsiResult ret = SummaryInfoSetProperty(this, prop, LibmsiOLEVariantType.OLEVT_LPSTR, 0, 0, value);
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {ret}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set integer property <paramref name="prop"/>.
        /// </summary>
        /// <param name="prop">A LibmsiProperty to set</param>
        /// <param name="value">A value</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool SetInt(LibmsiProperty prop, int value, ref Exception error)
        {
            if (error != null)
                return false;

            LibmsiOLEVariantType type = GetType(prop);
            if (type != LibmsiOLEVariantType.OLEVT_I2 && type != LibmsiOLEVariantType.OLEVT_I4)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {LibmsiResult.LIBMSI_RESULT_DATATYPE_MISMATCH}");
                return false;
            }

            LibmsiResult ret = SummaryInfoSetProperty(this, prop, type, value, 0, null);
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {ret}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set integer property <paramref name="prop"/>.
        /// </summary>
        /// <param name="prop">A LibmsiProperty to set</param>
        /// <param name="value">A value</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool SetFiletime(LibmsiProperty prop, ulong value, ref Exception error)
        {
            if (error != null)
                return false;

            if (GetType(prop) != LibmsiOLEVariantType.OLEVT_FILETIME)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {LibmsiResult.LIBMSI_RESULT_DATATYPE_MISMATCH}");
                return false;
            }

            LibmsiResult ret = SummaryInfoSetProperty(this, prop, LibmsiOLEVariantType.OLEVT_FILETIME, 0, value, null);
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                error = new Exception($"LIBMSI_RESULT_ERROR: {ret}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Save summary informations to the associated database.
        /// </summary>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool Persist(ref Exception error)
        {
            if (error != null)
                return false;

            if (Database == null)
            {
                error = new Exception("No database associated");
                return false;
            }

            LibmsiResult ret = SumInfoPersist(this, Database);
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {ret}");

            return ret == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <summary>
        /// Save summary informations to the associated database.
        /// </summary>
        /// <param name="db">A LibmsiDatabase to save to</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool Save(LibmsiDatabase db, ref Exception error)
        {
            if (db == null)
                return false;
            if (error != null)
                return false;

            LibmsiResult ret = SumInfoPersist(this, db);
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {ret}");

            return ret == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <returns>A new list with the list of set properties</returns>
        public List<int> GetProperties()
        {
            List<int> props = new List<int>();
            for (int i = 0; i < MSI_MAX_PROPS; i++)
            {
                if (Property[i].VariantType != LibmsiOLEVariantType.OLEVT_EMPTY)
                    props.Add(i);
            }

            return props;
        }

        #endregion

        #region Internal Functions

        internal static string SummaryInfoAsString(LibmsiSummaryInfo si, int uiProperty)
        {
            LibmsiOLEVariant prop = si.Property[uiProperty];

            switch (prop.VariantType)
            {
                case LibmsiOLEVariantType.OLEVT_I2:
                case LibmsiOLEVariantType.OLEVT_I4:
                    return prop.IntVal.ToString();
                case LibmsiOLEVariantType.OLEVT_LPSTR:
                    return prop.StrVal;
                case LibmsiOLEVariantType.OLEVT_FILETIME:
                    return FileTimeToString(prop.FileTime);
                case LibmsiOLEVariantType.OLEVT_EMPTY:
                    return string.Empty;
                default:
                    Console.Error.WriteLine($"Unknown type {prop.VariantType}");
                    break;
            }

            return null;
        }

        internal static LibmsiResult MsiAddSumInfo(LibmsiDatabase db, string[][] records, int num_records, int num_columns)
        {
            Exception err = null;
            LibmsiSummaryInfo si = LibmsiSummaryInfo.Create(db, num_records * (num_columns / 2), ref err);
            if (si == null)
            {
                Console.Error.WriteLine("No summary information!");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            for (int i = 0; i < num_records; i++)
            {
                for (int j = 0; j < num_columns; j += 2)
                {
                    LibmsiResult r = ParseProp(records[i][j], records[i][j + 1], out LibmsiProperty pid, out int int_value, out ulong ft_value, out string str_value);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    //assert(GetType(pid) != LibmsiOLEVariantType.OLEVT_EMPTY);
                    r = SummaryInfoSetProperty(si, pid, GetType(pid), int_value, ft_value, str_value);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;
                }
            }

            return SumInfoPersist(si, db);
        }

        #endregion

        #region Utilities

        private static void FreeProp(LibmsiOLEVariant prop)
        {
            prop.VariantType = LibmsiOLEVariantType.OLEVT_EMPTY;
        }

        private static LibmsiOLEVariantType GetType(LibmsiProperty uiProperty)
        {
            switch (uiProperty)
            {
                case LibmsiProperty.LIBMSI_PROPERTY_CODEPAGE:
                    return LibmsiOLEVariantType.OLEVT_I2;

                case LibmsiProperty.LIBMSI_PROPERTY_SUBJECT:
                case LibmsiProperty.LIBMSI_PROPERTY_AUTHOR:
                case LibmsiProperty.LIBMSI_PROPERTY_KEYWORDS:
                case LibmsiProperty.LIBMSI_PROPERTY_COMMENTS:
                case LibmsiProperty.LIBMSI_PROPERTY_TEMPLATE:
                case LibmsiProperty.LIBMSI_PROPERTY_LASTAUTHOR:
                case LibmsiProperty.LIBMSI_PROPERTY_UUID:
                case LibmsiProperty.LIBMSI_PROPERTY_APPNAME:
                case LibmsiProperty.LIBMSI_PROPERTY_TITLE:
                    return LibmsiOLEVariantType.OLEVT_LPSTR;

                case LibmsiProperty.LIBMSI_PROPERTY_EDITTIME:
                case LibmsiProperty.LIBMSI_PROPERTY_LASTPRINTED:
                case LibmsiProperty.LIBMSI_PROPERTY_CREATED_TM:
                case LibmsiProperty.LIBMSI_PROPERTY_LASTSAVED_TM:
                    return LibmsiOLEVariantType.OLEVT_FILETIME;

                case LibmsiProperty.LIBMSI_PROPERTY_SOURCE:
                case LibmsiProperty.LIBMSI_PROPERTY_RESTRICT:
                case LibmsiProperty.LIBMSI_PROPERTY_SECURITY:
                case LibmsiProperty.LIBMSI_PROPERTY_VERSION:
                    return LibmsiOLEVariantType.OLEVT_I4;

                default:
                    Console.Error.WriteLine($"Invalid type {uiProperty}");
                    break;
            }

            return LibmsiOLEVariantType.OLEVT_EMPTY;
        }

        private static int GetPropertyCount(LibmsiOLEVariant[] property)
        {
            int n = 0;
            if (property == null)
                return n;

            for (int i = 0; i < MSI_MAX_PROPS; i++)
            {
                if (property[i].VariantType != LibmsiOLEVariantType.OLEVT_EMPTY)
                    n++;
            }

            return n;
        }

        private static ushort ReadWORD(byte[] data, ref int ofs)
        {
            ushort val = 0;
            val = data[ofs];
            val |= (ushort)(data[ofs + 1] << 8);
            ofs += 2;
            return val;
        }

        private static uint ReadDWORD(byte[] data, ref int ofs)
        {
            uint val = 0;
            val = data[ofs];
            val |= (uint)(data[ofs + 1] << 8);
            val |= (uint)(data[ofs + 2] << 16);
            val |= (uint)(data[ofs + 3] << 24);
            ofs += 4;
            return val;
        }

        private static string FileTimeToString(ulong ft)
        {
            DateTimeOffset dt = DateTimeOffset.FromUnixTimeSeconds((long)ft);
            return dt.ToString("yyyy/MM/dd hh:mm:ss");
        }

        private static void ParseFileTime(string str, out ulong ft)
        {
            ft = 0;
            if (!DateTimeOffset.TryParse(str, out DateTimeOffset dt))
                return;

            ft = (ulong)dt.ToUnixTimeSeconds();
        }

        // FIXME: doesn't deal with endian conversion
        private static void ReadPropertiesFromData(LibmsiOLEVariant[] prop, byte[] data, int sz, int cProperties)
        {
            int idofs = 8;

            // Now set all the properties
            for (int i = 0; i < cProperties; i++)
            {
                bool valid = true;
                LibmsiProperty propid = (LibmsiProperty)ReadDWORD(data, ref idofs);
                int dwOffset = (int)ReadDWORD(data, ref idofs);

                if ((int)propid >= MSI_MAX_PROPS)
                {
                    Console.Error.WriteLine($"Unknown property ID {propid}");
                    break;
                }

                LibmsiOLEVariantType type = GetType(propid);
                if (type == LibmsiOLEVariantType.OLEVT_EMPTY)
                {
                    Console.Error.WriteLine($"propid {propid} has unknown type");
                    break;
                }

                LibmsiOLEVariant property = prop[(int)propid];

                if (dwOffset + 4 > sz)
                {
                    Console.Error.WriteLine($"Not enough data for type {dwOffset} {sz}");
                    break;
                }

                LibmsiOLEVariantType proptype = (LibmsiOLEVariantType)ReadDWORD(data, ref dwOffset);
                if (dwOffset + 4 > sz)
                {
                    Console.Error.WriteLine($"Not enough data for type {dwOffset} {sz}");
                    break;
                }

                string str = null;
                switch (proptype)
                {
                    case LibmsiOLEVariantType.OLEVT_I2:
                    case LibmsiOLEVariantType.OLEVT_I4:
                        property.IntVal = (int)ReadDWORD(data, ref dwOffset);
                        break;

                    case LibmsiOLEVariantType.OLEVT_FILETIME:
                        if (dwOffset + 8 > sz)
                        {
                            Console.Error.WriteLine($"Not enough data for type {dwOffset} {sz}");
                            valid = false;
                            break;
                        }

                        property.FileTime = (ulong)ReadDWORD(data, ref dwOffset);
                        property.FileTime |= (ulong)ReadDWORD(data, ref dwOffset) << 32;
                        break;

                    case LibmsiOLEVariantType.OLEVT_LPSTR:
                        int len = (int)ReadDWORD(data, ref dwOffset);
                        if (len == 0 || dwOffset + len > sz)
                        {
                            Console.Error.WriteLine($"Not enough data for type {dwOffset} {len} {sz}");
                            valid = false;
                            break;
                        }

                        str = Encoding.ASCII.GetString(data, dwOffset, len - 1) + "\0";
                        break;

                    default:
                        Console.Error.WriteLine($"Invalid type {proptype}");
                        break;
                }

                if (valid == false)
                    break;

                // Check the type is the same as we expect
                if (type == LibmsiOLEVariantType.OLEVT_LPSTR && proptype == LibmsiOLEVariantType.OLEVT_LPSTR)
                {
                    property.StrVal = str;
                }
                else if (type == proptype)
                {
                    // No-op
                }
                else if( proptype == LibmsiOLEVariantType.OLEVT_LPSTR)
                {
                    if (str == null)
                        return;

                    if (type == LibmsiOLEVariantType.OLEVT_I2 || type == LibmsiOLEVariantType.OLEVT_I4)
                    {
                        property.IntVal = int.Parse(new string(str.SkipWhile(c => char.IsWhiteSpace(c)).TakeWhile(c => c == '+' || c == '-' || char.IsDigit(c)).ToArray()));
                    }
                    else if (type == LibmsiOLEVariantType.OLEVT_FILETIME)
                    {
                        ParseFileTime(str, out ulong ft);
                        property.FileTime = ft;
                    }
                    else
                    {
                        Console.Error.WriteLine($"Invalid type, it can't be converted");
                        break;
                    }

                    proptype = type;
                }
                else
                {
                    Console.Error.WriteLine("Invalid type");
                    break;
                }

                // Now we now the type is valid, store it
                property.VariantType = proptype;
            }
        }

        private static LibmsiResult LoadSummaryInfo(LibmsiSummaryInfo si, GsfInput stm)
        {
            long sz = stm.Size;
            if (sz == 0)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            byte[] data = new byte[stm.Size];
            if (stm.Read((int)sz, data) == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            // Process the set header
            int ofs = 0;
            if (ReadWORD(data, ref ofs) != 0xfffe)
            {
                Console.Error.WriteLine("property set not little-endian\n");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            // Process the format header

            // Check the format id is correct
            ofs = 28;
            if (fmtid_SummaryInformation.SequenceEqual(data.Skip(ofs).Take(16)))
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            // Seek to the location of the section
            ofs += 16;
            int dwOffset = (int)ReadDWORD(data, ref ofs);
            
            // Read the section itself
            ofs = dwOffset;
            int cbSection = (int)ReadDWORD(data, ref ofs);
            int cProperties = (int)ReadDWORD(data, ref ofs);

            if (cProperties > MSI_MAX_PROPS)
            {
                Console.Error.WriteLine($"Too many properties {cProperties}");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            // Read all the data in one go 
            ReadPropertiesFromData(si.Property, data.Skip(dwOffset).ToArray(), cbSection, cProperties);
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private static int WriteWORD(byte[] data, int ofs, ushort val)
        {
            if (data != null)
            {
                data[ofs++] = (byte)(val & 0xff);
                data[ofs++] = (byte)((val >> 8) & 0xff);
            }

            return 2;
        }

        private static int WriteDWORD(byte[] data, int ofs, uint val)
        {
            if (data != null)
            {
                data[ofs++] = (byte)(val & 0xff);
                data[ofs++] = (byte)((val >> 8) & 0xff);
                data[ofs++] = (byte)((val >> 16) & 0xff);
                data[ofs++] = (byte)((val >> 24) & 0xff);
            }

            return 4;
        }

        private static int WriteFileTime(byte[] data, int ofs, ulong ft)
        {
            WriteDWORD(data, ofs, (uint)(ft & 0xFFFFFFFFUL));
            WriteDWORD(data, ofs + 4, (uint)(ft >> 32));
            return 8;
        }

        private static int WriteString(byte[] data, int ofs, string str)
        {
            int len = str.Length + 1;
            WriteDWORD(data, ofs, (uint)len);
            if (data != null)
                Array.Copy(Encoding.ASCII.GetBytes(str + "\0"), 0, data, ofs + 4, len);

            return (7 + len) & ~3;
        }

        private static int WritePropertyToData(LibmsiOLEVariant prop, byte[] data, int ofs)
        {
            int sz = ofs;
            if (prop.VariantType == LibmsiOLEVariantType.OLEVT_EMPTY)
                return sz;

            // Add the type
            sz += WriteDWORD(data, sz, (uint)prop.VariantType);
            switch( prop.VariantType )
            {
                case LibmsiOLEVariantType.OLEVT_I2:
                case LibmsiOLEVariantType.OLEVT_I4:
                    sz += WriteDWORD(data, sz, (uint)prop.IntVal);
                    break;

                case LibmsiOLEVariantType.OLEVT_FILETIME:
                    sz += WriteFileTime(data, sz, prop.FileTime);
                    break;

                case LibmsiOLEVariantType.OLEVT_LPSTR:
                    sz += WriteString(data, sz, prop.StrVal);
                    break;

                default:
                    Console.Error.WriteLine($"Invalid type {prop.VariantType}");
                    break;
            }

            return sz;
        }

        private static LibmsiResult SumInfoPersist(LibmsiSummaryInfo si, LibmsiDatabase database)
        {
            // Add up how much space the data will take and calculate the offsets
            int cProperties = GetPropertyCount(si.Property);
            int cbSection = 8 + cProperties * 8;
            for (int i = 0; i < MSI_MAX_PROPS; i++)
            {
                cbSection += WritePropertyToData(si.Property[i], null, 0);
            }

            int sz = 28 + 20 + cbSection;
            byte[] data = new byte[sz];

            // Write the set header
            sz = 0;
            sz += WriteWORD(data, sz, 0xfffe);      // wByteOrder
            sz += WriteWORD(data, sz, 0);           // wFormat
            sz += WriteDWORD(data, sz, 0x00020005); // dwOSVer - build 5, platform id 2
            sz += 16;                               // clsID
            sz += WriteDWORD(data, sz, 1);          // reserved

            // Write the format header
            Array.Copy(fmtid_SummaryInformation, 0, data, sz, 16);
            sz += 16;

            sz += WriteDWORD(data, sz, 28 + 20);    // dwOffset
            //assert(sz == 28 + 20);

            // Write the section header
            sz += WriteDWORD(data, sz, (uint)cbSection);
            sz += WriteDWORD(data, sz, (uint)cProperties);
            //assert(sz == 28 + 20 + 8);

            int dwOffset = 8 + cProperties * 8;
            for (int i = 0; i < MSI_MAX_PROPS; i++)
            {
                int propsz = WritePropertyToData(si.Property[i], null, 0);
                if (propsz == 0)
                    continue;

                sz += WriteDWORD(data, sz, (uint)i);
                sz += WriteDWORD(data, sz, (uint)dwOffset);
                dwOffset += propsz;
            }

            //assert(dwOffset == cbSection);

            // Write out the data
            for (int i = 0; i < MSI_MAX_PROPS; i++)
            {
                sz += WritePropertyToData(si.Property[i], data, sz);
            }

            //assert(sz == 28 + 20 + cbSection);

            LibmsiResult r = database.WriteRawStreamData(szSumInfo, data, sz, out GsfInput stm);
            return r;
        }

        private static void SummaryInfoGetProperty(
            LibmsiSummaryInfo si,
            LibmsiProperty uiProperty,
            out LibmsiPropertyType puiDataType,
            out int pintvalue,
            out ulong pftValue,
            out string szValueBuf,
            out int pcchValueBuf,
            out string str,
            ref Exception error)
        {
            puiDataType = 0; pintvalue = 0; pftValue = 0; szValueBuf = null; pcchValueBuf = 0; str = null;
            if ((int)uiProperty >= MSI_MAX_PROPS)
            {
                error = new Exception($"Unknown property: {uiProperty}");
                return;
            }

            LibmsiOLEVariant prop = si.Property[(int)uiProperty];
            LibmsiPropertyType type;
            switch (prop.VariantType)
            {
                case LibmsiOLEVariantType.OLEVT_I2:
                case LibmsiOLEVariantType.OLEVT_I4:
                    type = LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_INT;
                    pintvalue = prop.IntVal;
                    break;

                case LibmsiOLEVariantType.OLEVT_LPSTR:
                    type = LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_STRING;
                    str = prop.StrVal;

                    int len = prop.StrVal.Length;
                    szValueBuf = prop.StrVal.Substring(Math.Min(len, pcchValueBuf));
                    if (len >= pcchValueBuf)
                        error = new Exception("The given string is too small");

                    pcchValueBuf = len;
                    break;

                case LibmsiOLEVariantType.OLEVT_FILETIME:
                    type = LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_FILETIME;
                    pftValue = prop.FileTime;
                    break;

                case LibmsiOLEVariantType.OLEVT_EMPTY:
                    // FIXME: should be replaced by a has_property() instead?
                    error = new Exception("Empty property");
                    type = LibmsiPropertyType.LIBMSI_PROPERTY_TYPE_EMPTY;
                    break;

                default:
                    return;
            }

            puiDataType = type;
        }

        private static LibmsiResult SummaryInfoSetProperty(
            LibmsiSummaryInfo si,
            LibmsiProperty uiProperty,
            LibmsiOLEVariantType type,
            int intvalue,
            ulong pftValue,
            string szValue)
        {
            if (type == LibmsiOLEVariantType.OLEVT_LPSTR && szValue == null)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (type == LibmsiOLEVariantType.OLEVT_FILETIME && pftValue == 0)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            LibmsiOLEVariant prop = si.Property[(int)uiProperty];

            if (prop.VariantType == LibmsiOLEVariantType.OLEVT_EMPTY)
            {
                LibmsiResult ret = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                if (si.UpdateCount == 0)
                    return ret;

                si.UpdateCount--;
            }
            else if (prop.VariantType != type )
            {
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            FreeProp(prop);
            prop.VariantType = type;
            switch (type)
            {
                case LibmsiOLEVariantType.OLEVT_I2:
                case LibmsiOLEVariantType.OLEVT_I4:
                    prop.IntVal = intvalue;
                    break;
                case LibmsiOLEVariantType.OLEVT_FILETIME:
                    prop.FileTime = pftValue;
                    break;
                case LibmsiOLEVariantType.OLEVT_LPSTR:
                    prop.StrVal = szValue;
                    break;
                default:
                    Console.Error.WriteLine($"Invalid type {type}");
                    break;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private static LibmsiResult ParseProp(string prop, string value, out LibmsiProperty pid, out int int_value, out ulong ft_value, out string str_value)
        {
            pid = (LibmsiProperty)int.Parse(new string(prop.SkipWhile(c => char.IsWhiteSpace(c)).TakeWhile(c => c == '+' || c == '-' || char.IsDigit(c)).ToArray()));
            int_value = 0; ft_value = 0; str_value = null;
            
            switch (pid)
            {
                case LibmsiProperty.LIBMSI_PROPERTY_CODEPAGE:
                case LibmsiProperty.LIBMSI_PROPERTY_SOURCE:
                case LibmsiProperty.LIBMSI_PROPERTY_RESTRICT:
                case LibmsiProperty.LIBMSI_PROPERTY_SECURITY:
                case LibmsiProperty.LIBMSI_PROPERTY_VERSION:
                    int_value = int.Parse(new string(value.SkipWhile(c => char.IsWhiteSpace(c)).TakeWhile(c => c == '+' || c == '-' || char.IsDigit(c)).ToArray()));
                    break;

                case LibmsiProperty.LIBMSI_PROPERTY_EDITTIME:
                case LibmsiProperty.LIBMSI_PROPERTY_LASTPRINTED:
                case LibmsiProperty.LIBMSI_PROPERTY_CREATED_TM:
                case LibmsiProperty.LIBMSI_PROPERTY_LASTSAVED_TM:
                    ParseFileTime(value, out ft_value);
                    break;

                case LibmsiProperty.LIBMSI_PROPERTY_SUBJECT:
                case LibmsiProperty.LIBMSI_PROPERTY_AUTHOR:
                case LibmsiProperty.LIBMSI_PROPERTY_KEYWORDS:
                case LibmsiProperty.LIBMSI_PROPERTY_COMMENTS:
                case LibmsiProperty.LIBMSI_PROPERTY_TEMPLATE:
                case LibmsiProperty.LIBMSI_PROPERTY_LASTAUTHOR:
                case LibmsiProperty.LIBMSI_PROPERTY_UUID:
                case LibmsiProperty.LIBMSI_PROPERTY_APPNAME:
                case LibmsiProperty.LIBMSI_PROPERTY_TITLE:
                    str_value = value;
                    break;

                default:
                    Console.Error.WriteLine($"Unhandled prop id {pid}");
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion
    }
}