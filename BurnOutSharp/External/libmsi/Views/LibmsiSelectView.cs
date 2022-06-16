/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002-2004 Mike McCormack for CodeWeavers
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
using static LibMSI.LibmsiRecord;
using static LibMSI.Internal.MsiPriv;

namespace LibMSI.Views
{
    /// <summary>
    /// Below is the query interface to a table
    /// </summary>
    internal class LibmsiSelectView : LibmsiView
    {
        #region Properties

        public LibmsiDatabase Database { get; set; }

        public LibmsiView Table { get; set; }

        public int NumCols { get; set; }

        public int MaxCols { get; set; }

        public int[] Cols { get; set; } = new int[1];

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiSelectView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, LibmsiView table, column_info columns)
        {
            int count = SelectCountColumns(columns);
            LibmsiSelectView sv = new LibmsiSelectView
            {
                Database = db,
                Table = table,
                NumCols = 0,
                MaxCols = count,
            };

            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            while (columns != null)
            {
                r = sv.AddColumn(columns.Column, columns.Table);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    break;

                columns = columns.Next;
            }

            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS )
                view = sv;
            else
                view = null;

            return r;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        public override LibmsiResult FetchInt(int row, int col, out int val)
        {
            val = 0;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (col == 0 || col > NumCols)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            col = Cols[col - 1];
            if (col == 0)
            {
                val = 0;
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            return Table.FetchInt(row, col, out val);
        }

        /// <inheritdoc/>
        public override LibmsiResult FetchStream(int row, int col, out GsfInput stm)
        {
            stm = null;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (col == 0 || col > NumCols)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            col = Cols[col - 1];
            if (col == 0)
            {
                stm = null;
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            return Table.FetchStream(row, col, out stm);
        }

        /// <inheritdoc/>
        public override LibmsiResult GetRow(int row, out LibmsiRecord rec)
        {
            rec = null;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return MsiViewGetRow(Database, this, row, out rec);
        }

        /// <inheritdoc/>
        public override LibmsiResult SetRow(int row, LibmsiRecord rec, int mask)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            // Test if any of the mask bits are invalid 
            if (mask >= (1 << NumCols))
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            // Find the number of columns in the table below
            LibmsiResult r = Table.GetDimensions(out _, out int col_count);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            // Expand the record to the right size for the underlying table
            LibmsiRecord expanded = LibmsiRecord.Create(col_count);
            if (expanded == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            // Move the right fields across
            int expanded_mask = 0;
            for (int i = 0; i < NumCols; i++ )
            {
                r = RecordCopyField(rec, i + 1, expanded, Cols[i]);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    break;

                expanded_mask |= (1 << (Cols[i] - 1));
            }

            // Set the row in the underlying table
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                r = Table.SetRow(row, expanded, expanded_mask);

            return r;
        }

        /// <inheritdoc/>
        public override LibmsiResult InsertRow(LibmsiRecord record, int row, bool temporary)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            // Rearrange the record to suit the table 
            LibmsiResult r = Table.GetDimensions(out _, out int table_cols);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            LibmsiRecord outrec = LibmsiRecord.Create(table_cols + 1);

            for (int i = 0; i < NumCols; i++)
            {
                r = RecordCopyField(record, i + 1, outrec, Cols[i] );
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            return Table.InsertRow(outrec, row, temporary );
        }

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return Table.Execute(record);
        }

        /// <inheritdoc/>
        public override LibmsiResult Close()
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return Table.Close();
        }

        /// <inheritdoc/>
        public override LibmsiResult GetDimensions(out int rows, out int cols)
        {
            rows = 0; cols = 0;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            cols = NumCols;
            return Table.GetDimensions(out rows, out _);
        }

        /// <inheritdoc/>
        public override LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name)
        {
            name = null; type = 0; temporary = false; table_name = null;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (n == 0 || n > NumCols)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            n = Cols[ n - 1 ];
            if (n != 0)
            {
                name = string.Empty;
                type = MSITYPE_UNKNOWN | MSITYPE_VALID;
                temporary = false;
                table_name = szEmpty;
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            return Table.GetColumnInfo(n, out name, out type, out temporary, out table_name);
        }

        /// <inheritdoc/>
        public override LibmsiResult Delete()
        {
            if (Table != null)
                Table.Delete();

            Table = null;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle)
        {
            row = 0;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if ((col == 0) || (col > NumCols))
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            col = Cols[ col - 1 ];

            return Table.FindMatchingRows(col, val, out row, ref handle);
        }

        #endregion

        #region Utilities

        private LibmsiResult AddColumn(string name, string table_name)
        {
            LibmsiView table = Table;
            if (table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            try { table.GetDimensions(out _, out _); }
            catch { return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED; }

            try { table.GetColumnInfo(0, out _, out _, out _, out _); }
            catch { return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED; }

            if (NumCols >= MaxCols)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            int n;
            if (name[0] == '\0')
            {
                n = 0;
            }
            else
            {
                LibmsiResult r = ViewFindColumn(table, name, table_name, out n);
                if( r != LibmsiResult.LIBMSI_RESULT_SUCCESS )
                    return r;
            }

            Cols[NumCols++] = n;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private static int SelectCountColumns(column_info col)
        {
            int n;
            for (n = 0; col != null; col = col.Next)
            {
                n++;
            }

            return n;
        }

        #endregion
    }
}