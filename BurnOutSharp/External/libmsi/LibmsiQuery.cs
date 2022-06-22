/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002-2005 Mike McCormack for CodeWeavers
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
using LibGSF.Input;
using LibMSI.Internal;
using LibMSI.Views;
using static LibMSI.Internal.MsiPriv;

namespace LibMSI
{
    public class LibmsiQuery
    {
        #region Properties

        internal LibmsiView View { get; set; }

        internal int Row { get; set; }

        internal LibmsiDatabase Database { get; set; }

        internal string Query { get; set; }

        internal LinkedList<object> Mem { get; set; } = new LinkedList<object>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiQuery() { }

        /// <summary>
        /// Create a SQL query for <paramref name="database"/>.
        /// </summary>
        /// <param name="database">A LibmsiDatabase</param>
        /// <param name="query">A SQL query</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>A new LibmsiQuery on success, null on failure</returns>
        public static LibmsiQuery Create(LibmsiDatabase database, string query, ref Exception error)
        {
            if (database == null)
                return null;
            if (query == null)
                return null;
            if (error != null)
                return null;

            LibmsiQuery self = new LibmsiQuery
            {
                Database = database,
                Query = query,
            };

            if (!self.Init(ref error))
                return null;

            return self;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~LibmsiQuery()
        {
            if (View != null)
                try { View.Delete(); } catch { }

            Mem.Clear();
        }

        #endregion

        #region Functions

        /// <summary>
        /// Return the next query result. null is returned when there
        /// is no more results.
        /// </summary>
        /// <param name="error">Return location for the error</param>
        /// <returns>A newly allocated LibmsiRecord or null when no results or failure.</returns>
        public LibmsiRecord Fetch(ref Exception error)
        {
            if (error != null)
                return null;

            LibmsiResult ret = Fetch(out LibmsiRecord record);

            // FIXME: raise error when it happens
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS && ret != LibmsiResult.NO_MORE_ITEMS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {ret}");

            return record;
        }

        /// <summary>
        /// Execute the query with the arguments from <paramref name="rec"/>.
        /// </summary>
        /// <param name="rec">A LibmsiRecord containing query arguments, or null if no arguments needed</param>
        /// <param name="error">Return location for the error</param>
        /// <returns>True on success</returns>
        public bool Execute(LibmsiRecord rec, ref Exception error)
        {
            if (error != null)
                return false;

            LibmsiResult ret = Execute(rec);

            // FIXME: raise error when it happens 
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {ret}");

            return ret == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <summary>
        /// Release the current result set.
        /// </summary>
        /// <param name="error">Return location for the error</param>
        /// <returns>True on success</returns>
        public bool Close(ref Exception error)
        {
            if (error != null)
                return false;

            if (View == null)
                return true;

            try
            {
                LibmsiResult ret = View.Close();

                // FIXME: raise error when it happens
                if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    error = new Exception($"LIBMSI_RESULT_ERROR: {ret}");

                return ret == LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Call this to get more information on the last query error.
        /// </summary>
        /// <param name="column">Location to store the allocated column name</param>
        /// <param name="error">Return location for the error</param>
        public void GetError(out string column, ref Exception error)
        {
            column = null;
            if (error != null)
                return;

            if (View.Error != LibmsiDBError.LIBMSI_DB_ERROR_SUCCESS)
            {
                // FIXME: view could have a GError with message?
                error = new Exception($"LIBMSI_DB_ERROR: {View.Error}");
                column = View.ErrorColumn;
            }
        }

        /// <summary>
        /// Get column informations, returned as record string fields.
        /// </summary>
        /// <param name="info">A LibmsiColInfo specifying the type of information to return</param>
        /// <param name="error">Return location for the error</param>
        /// <returns>A LibmsiRecord containing informations, or null on error</returns>
        public LibmsiRecord GetColumnInfo(LibmsiColInfo info, ref Exception error)
        {
            if (info != LibmsiColInfo.LIBMSI_COL_INFO_NAMES && info != LibmsiColInfo.LIBMSI_COL_INFO_TYPES)
                return null;
            if (error != null)
                return null;

            LibmsiResult r = GetColumnInfo(info, out LibmsiRecord rec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {r}");

            return rec;
        }

        #endregion

        #region Internal Functions

        // TODO: Move to LibmsiView
        internal static LibmsiResult ViewFindColumn(LibmsiView table, string name, string table_name, out int n)
        {
            n = 0;
            LibmsiResult r = table.GetDimensions(out _, out int count);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            for (int i = 1; i <= count; i++)
            {
                r = table.GetColumnInfo(i, out string col_name, out _, out _, out string haystack_table_name);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                bool x = name == col_name;
                if (table_name != null)
                    x |= table_name == haystack_table_name;

                if (!x)
                {
                    n = i;
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;
                }
            }

            return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;
        }

        // TODO: Move to LibmsiDatabase
        internal static LibmsiResult DatabaseOpenQuery(LibmsiDatabase db, string szQuery, out LibmsiQuery pView)
        {
            Exception err = null;
            pView = LibmsiQuery.Create(db, szQuery, ref err);
            return pView != null ? LibmsiResult.LIBMSI_RESULT_SUCCESS : LibmsiResult.LIBMSI_RESULT_BAD_QUERY_SYNTAX;
        }

        // TODO: Move to LibmsiDatabase
        internal static LibmsiResult QueryOpen(LibmsiDatabase db, out LibmsiQuery view, string query)
        {
            Exception err = null;
            view = Create(db, query, ref err);
            if (err != null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult IterateRecords(ref int count, record_func func, object param)
        {
            LibmsiResult r = Execute(null);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            int max = 0;
            if (count != 0)
                max = count;

            // Iterate a query
            int n = 0;
            for (; (max == 0) || (n < max); n++)
            {
                r = Fetch(out LibmsiRecord rec);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    break;

                if (func != null)
                    r = func(rec, param);

                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    break;
            }

            Exception error = null; // FIXME: move error handling to caller
            Close(ref error);
            if (error != null)
                Console.Error.WriteLine(error);

            count = n;
            if (r == LibmsiResult.NO_MORE_ITEMS)
                r = LibmsiResult.LIBMSI_RESULT_SUCCESS;

            return r;
        }

        /// <summary>
        /// Return a single record from a query
        /// </summary>
        // TODO: Move to LibmsiDatabase
        internal static LibmsiRecord QueryGetRecord(LibmsiDatabase db, string query)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_SUCCESS;

            Exception error = null; // FIXME: move error to caller
            LibmsiQuery view = Create(db, query, ref error);
            if (error != null)
                r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            error = null;
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                view.Execute(null);
                view.Fetch(out LibmsiRecord rec);
                view.Close(ref error);
                if (error != null)
                    Console.Error.WriteLine(error);

                return rec;
            }

            return null;
        }

        // TODO: Move to LibmsiDatabase
        internal static LibmsiResult MsiViewGetRow(LibmsiDatabase db, LibmsiView view, int row, out LibmsiRecord rec)
        {
            rec = null;
            LibmsiResult ret = view.GetDimensions(out int row_count, out int col_count);
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return ret;

            if (col_count == 0)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (row >= row_count)
                return LibmsiResult.NO_MORE_ITEMS;

            rec = LibmsiRecord.Create(col_count);
            if (rec == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            for (int i = 1; i <= col_count; i++)
            {
                ret = view.GetColumnInfo(i, out _, out int type, out _, out _);
                if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine($"Error getting column type for {i}");
                    continue;
                }

                if (MSITYPE_IS_BINARY(type))
                {
                    ret = view.FetchStream(row, i, out GsfInput stm);
                    if ((ret == LibmsiResult.LIBMSI_RESULT_SUCCESS) && stm != null)
                        rec.SetGsfInput(i, stm);
                    else
                        Console.Error.WriteLine("Failed to get stream");

                    continue;
                }

                ret = view.FetchInt(row, i, out int ival);
                if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    Console.Error.WriteLine($"Error fetching data for {i}");
                    continue;
                }

                if ((type & MSITYPE_VALID) == 0)
                    Console.Error.WriteLine("Invalid type!");

                // Check if it's nul (0) - if so, don't set anything
                if (ival == 0)
                    continue;

                if ((type & MSITYPE_STRING) != 0)
                {
                    string sval = db.Strings.LookupId(ival);
                    rec.SetString(i, sval);
                }
                else
                {
                    if ((type & MSI_DATASIZEMASK) == 2)
                        rec.SetInt(i, ival - (1 << 15));
                    else
                        rec.SetInt(i, ival - (1 << 31));
                }
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult Fetch(out LibmsiRecord prec)
        {
            prec = null;
            if (View == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = MsiViewGetRow(Database, View, Row, out prec);
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                Row++;

            return r;
        }

        internal LibmsiResult Execute(LibmsiRecord rec)
        {
            if (View == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            Row = 0;
            try
            {
                return View.Execute(rec);
            }
            catch
            {
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }
        }

        internal LibmsiResult GetColumnInfo(LibmsiColInfo info, out LibmsiRecord prec)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            prec = null;
            if (View == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            int count;
            try
            {
                r = View.GetDimensions(out _, out count);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }
            catch
            {
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            if (count == 0)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            LibmsiRecord rec = LibmsiRecord.Create(count);
            if (rec == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            for (int i = 0; i < count; i++)
            {
                r = View.GetColumnInfo(i + 1, out string name, out int type, out bool temporary, out _);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    continue;

                if (info == LibmsiColInfo.LIBMSI_COL_INFO_NAMES)
                    rec.SetString(i + 1, name);
                else
                    SetRecordTypeString(rec, i + 1, type, temporary);
            }

            prec = rec;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Utilities

        private bool Init(ref Exception error)
        {
            LibmsiResult r = LibmsiSQLInput.ParseSQL(Database, Query, out LibmsiView view, Mem);
            View = view;

            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {r}");

            return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Move to LibmsiRecord
        private static void SetRecordTypeString(LibmsiRecord rec, int field, int type, bool temporary)
        {
            string szType;
            if (MSITYPE_IS_BINARY(type))
                szType = "v";
            else if ((type & MSITYPE_LOCALIZABLE) != 0)
                szType = "l";
            else if ((type & MSITYPE_UNKNOWN) != 0)
                szType = "f";
            else if ((type & MSITYPE_STRING) != 0)
            {
                if (temporary)
                    szType = "g";
                else
                    szType = "s";
            }
            else
            {
                if (temporary)
                    szType = "j";
                else
                    szType = "i";
            }

            if ((type & MSITYPE_NULLABLE) != 0)
                szType = ((char)(szType[0] & ~0x20)).ToString();

            szType += (type & 0xff).ToString();

            rec.SetString(field, szType);
        }

        #endregion
    }
}