/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2006 Mike McCormack
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

using LibGSF.Input;
using static LibMSI.LibmsiQuery;

namespace LibMSI.Views
{
    internal class LibmsiAlterView : LibmsiView
    {
        #region Properties

        public LibmsiDatabase Database { get; set; }

        public LibmsiView Table { get; set; }

        public column_info ColInfo { get; set; }

        public int Hold { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiAlterView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, string name, column_info colinfo, int hold)
        {
            LibmsiAlterView av = new LibmsiAlterView();
            LibmsiResult r = LibmsiTableView.Create(db, name, out LibmsiView table);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                view = null;
                return r;
            }

            av.Table = table;

            if (colinfo != null)
                colinfo.Table = name;

            // Fill the structure
            av.Database = db;
            av.Hold = hold;
            av.ColInfo = colinfo;

            view = av;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        public override LibmsiResult FetchInt(int row, int col, out int val)
        {
            val = default;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        /// <inheritdoc/>
        public override LibmsiResult FetchStream(int row, int col, out GsfInput stm)
        {
            stm = null;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetRow(int row, out LibmsiRecord rec) => Table.GetRow(row, out rec);

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record)
        {
            if (Hold == 1)
            {
                Table.AddRef();
            }
            else if (Hold == -1)
            {
                int r = Table.Release();
                if (r == 0)
                    Table = null;
            }

            if (ColInfo != null)
                return AddColumn();

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult Close() => LibmsiResult.LIBMSI_RESULT_SUCCESS;

        /// <inheritdoc/>
        public override LibmsiResult GetDimensions(out int rows, out int cols)
        {
            rows = 0; cols = 0;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name)
        {
            name = null; type = 0; temporary = false; table_name = null;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        /// <inheritdoc/>
        public override LibmsiResult Delete()
        {
            if (Table != null)
                Table.Delete();

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle)
        {
            row = 0;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        #endregion

        #region Utilities

        private static LibmsiResult CountIter(LibmsiRecord row, object param)
        {
            if (param is int)
                param = (int)param + 1;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private bool CheckColumnExists(LibmsiDatabase db, string table, string column)
        {
            string query = $"SELECT * FROM `_Columns` WHERE `Table`='{table}' AND `Name`='{column}'";
            LibmsiResult r = QueryOpen(db, out LibmsiQuery view, query);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return false;

            r = view.Execute(null);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto done;

            r = view.Fetch(out LibmsiRecord rec);
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                rec = null;

        done:
            view = null;
            return (r == LibmsiResult.LIBMSI_RESULT_SUCCESS);
        }

        private LibmsiResult AddColumn()
        {
            int colnum = 1;

            string szColumns = "_Columns";
            LibmsiResult r = LibmsiTableView.Create(Database, szColumns, out LibmsiView columns);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            if (CheckColumnExists(Database, ColInfo.Table, ColInfo.Column))
            {
                columns.Delete();
                return LibmsiResult.LIBMSI_RESULT_BAD_QUERY_SYNTAX;
            }

            string query = $"SELECT * FROM `_Columns` WHERE `Table`='{ColInfo.Table}' ORDER BY `Number`";
            r = QueryOpen(Database, out LibmsiQuery view, query);
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                int count = 0;
                r = view.IterateRecords(ref count, CountIter, colnum);
                view = null;
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    columns.Delete();
                    return r;
                }
            }

            r = columns.AddColumn(ColInfo.Table, colnum, ColInfo.Column, ColInfo.Type, (Hold == 1));

            columns.Delete();
            return r;
        }

        #endregion
    }
}