/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2004 Mike McCormack for CodeWeavers
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
 using static LibMSI.LibmsiRecord;
 using static LibMSI.Internal.LibmsiSQLInput;
 using static LibMSI.Internal.MsiPriv;

namespace LibMSI.Views
{
    internal class LibmsiInsertView : LibmsiView
    {
        #region Properties

        public LibmsiView Table { get; set; }

        public LibmsiDatabase Database { get; set; }

        public bool IsTemp { get; set; }

        public LibmsiView View { get; set; }

        public column_info Vals { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiInsertView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, string table, column_info columns, column_info values, bool temp)
        {
            view = null;

            // There should be one value for each column
            if (CountColumnInfo(columns) != CountColumnInfo(values))
                return LibmsiResult.LIBMSI_RESULT_BAD_QUERY_SYNTAX;

            LibmsiResult r = LibmsiTableView.Create(db, table, out LibmsiView tv);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = LibmsiSelectView.Create(db, out LibmsiView sv, tv, columns);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                if (tv != null)
                    tv.Delete();

                return r;
            }

            LibmsiInsertView iv = new LibmsiInsertView
            {
                Table = tv,
                Database = db,
                Vals = values,
                IsTemp = temp,
                View = sv,
            };

            view = iv;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        public override LibmsiResult FetchInt(int row, int col, out int val)
        {
            val = 0;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record)
        {
            if (View == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = View.Execute(null);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = View.GetDimensions(out _, out int col_count);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            // Merge the wildcard values into the list of values provided
            // in the query, and create a record containing both.
            LibmsiRecord values = MsiQueryMergeRecord(col_count, Vals, record);
            if (values == null)
                return r;

            r = ArrangeRecord(ref values);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            // Rows with NULL primary keys are inserted at the beginning of the table
            int row = -1;
            if (RowHasNullPrimaryKeys(values))
                row = 0;

            return Table.InsertRow(values, row, IsTemp);
        }

        /// <inheritdoc/>
        public override LibmsiResult Close()
        {
            if (View == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return View.Close();
        }

        /// <inheritdoc/>
        public override LibmsiResult GetDimensions(out int rows, out int cols)
        {
            rows = 0; cols = 0;
            if (View == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return View.GetDimensions(out rows, out cols);
        }

        /// <inheritdoc/>
        public override LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name)
        {
            name = null; type = 0; temporary = false; table_name = null;
            if (View == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return View.GetColumnInfo(n, out name, out type, out temporary, out table_name);
        }

        /// <inheritdoc/>
        public override LibmsiResult Delete()
        {
            if (View != null)
                View.Delete();

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle)
        {
            row = 0;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        #endregion

        #region Internal Functions

        /// <summary>
        /// Merge a value_list and a record to create a second record.
        /// Replace wildcard entries in the valuelist with values from the record
        /// </summary>
        internal static LibmsiRecord MsiQueryMergeRecord(int fields, column_info vl, LibmsiRecord rec)
        {
            int wildcard_count = 1;

            LibmsiRecord merged = LibmsiRecord.Create(fields);
            for (int i = 1; i <= fields; i++)
            {
                if (vl == null)
                    return null;

                switch (vl.Val.Type)
                {
                    case EXPR_SVAL:
                        merged.SetString(i, vl.Val.SVal);
                        break;
                    case EXPR_IVAL:
                        merged.SetInt(i, vl.Val.IVal);
                        break;
                    case EXPR_WILDCARD:
                        if (rec == null)
                            return null;

                        RecordCopyField(rec, wildcard_count, merged, i);
                        wildcard_count++;
                        break;
                    default:
                        Console.Error.WriteLine($"Unknown expression type {vl.Val.Type}");
                        break;
                }

                vl = vl.Next;
            }

            return merged;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Checks to see if the column order specified in the INSERT query
        /// matches the column order of the table
        /// </summary>
        private bool ColumnsInOrder(int col_count)
        {
            for (int i = 1; i <= col_count; i++)
            {
                View.GetColumnInfo(i, out string a, out _, out _, out _);
                Table.GetColumnInfo(i, out string b, out _, out _, out _);
                if (a != b)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Rearranges the data in the record to be inserted based on column order,
        /// and pads the record for any missing columns in the INSERT query
        /// </summary>
        private LibmsiResult ArrangeRecord(ref LibmsiRecord values)
        {
            LibmsiResult r = Table.GetDimensions(out _, out int col_count);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            int val_count = values.GetFieldCount();

            // Check to see if the columns are arranged already
            // to avoid unnecessary copying
            if (col_count == val_count && ColumnsInOrder(col_count))
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            LibmsiRecord padded = LibmsiRecord.Create(col_count);
            if (padded == null)
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

            for (int colidx = 1; colidx <= val_count; colidx++)
            {
                r = View.GetColumnInfo(colidx, out string a, out _, out _, out _);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                for (int i = 1; i <= col_count; i++)
                {
                    r = Table.GetColumnInfo(i, out string b, out _, out _, out _);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return r;

                    if (a == b)
                    {
                        RecordCopyField(values, colidx, padded, i);
                        break;
                    }
                }
            }

            values = padded;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private bool RowHasNullPrimaryKeys(LibmsiRecord row)
        {
            LibmsiResult r = Table.GetDimensions(out _, out int col_count );
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return false;

            for (int i = 1; i <= col_count; i++)
            {
                r = Table.GetColumnInfo(i, out _, out int type, out _, out _);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return false;

                if ((type & MSITYPE_KEY) == 0)
                    continue;

                if (row.IsNull(i))
                    return true;
            }

            return false;
        }

        private static int CountColumnInfo(column_info ci)
        {
            int n = 0;
            for (; ci != null; ci = ci.Next)
            {
                n++;
            }

            return n;
        }

        #endregion
    }
}