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

 using static LibMSI.LibmsiRecord;

namespace LibMSI
{
    /// <summary>
    /// Below is the query interface to a table
    /// </summary>
    internal class LibmsiUpdateView : LibmsiView
    {
        #region Properties

        public LibmsiDatabase Database { get; set; }

        public LibmsiView View { get; set; }

        public column_info Vals { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiUpdateView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, string table, column_info columns, expr expr)
        {
            view = null;            
            LibmsiResult r;
            LibmsiView sv = null, wv = null;
            if (expr != null)
                r = LibmsiWhereView.Create(db, out wv, table, expr);
            else
                r = LibmsiTableView.Create(db, table, out wv);

            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            // Then select the columns we want
            r = LibmsiSelectView.Create(db, out sv, wv, columns);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                wv.Delete();
                return r;
            }

            LibmsiUpdateView uv = new LibmsiUpdateView
            {
                Database = db,
                Vals = columns,
                View = sv,
            };

            view = uv;

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
            int i;
            LibmsiRecord where = null;
            int cols_count, where_count;
            column_info col = Vals;

            // Extract the where markers from the record
            if (record != null)
            {
                int s = record.GetFieldCount();
                for (i = 0; col != null; col = col.Next)
                {
                    i++;
                }

                cols_count = i;
                where_count = s - i;

                if (where_count > 0)
                {
                    where = LibmsiRecord.Create(where_count);
                    if (where != null)
                    {
                        for (i = 1; i <= where_count; i++)
                        {
                            RecordCopyField(record, cols_count + i, where, i);
                        }
                    }
                }
            }

            if (View == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = View.Execute(where);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = View.GetDimensions(out int row_count, out int col_count );
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            LibmsiRecord values = LibmsiInsertView.MsiQueryMergeRecord(col_count, Vals, record);
            if (values == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            for (i = 0; i < row_count; i++)
            {
                r = View.SetRow(i, values, (1 << col_count) - 1);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    break;
            }

            return r;
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
    }
}