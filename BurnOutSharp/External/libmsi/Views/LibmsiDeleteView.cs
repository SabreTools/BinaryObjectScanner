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

 using LibGSF.Input;

namespace LibMSI.Views
{
    /// <summary>
    /// Code to delete rows from a table.
    /// <summary>
    /// <remarks>
    /// We delete rows by blanking them out rather than trying to remove the row.
    /// This appears to be what the native MSI does (or tries to do). For the query:
    /// 
    /// delete from Property
    /// 
    /// some non-zero entries are left in the table by native MSI.  I'm not sure if
    /// that's a bug in the way I'm running the query, or a just a bug.
    internal class LibmsiDeleteView : LibmsiView
    {
        #region Properties

        public LibmsiDatabase Database { get; set; }

        public LibmsiView Table { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiDeleteView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, LibmsiView table)
        {
            LibmsiDeleteView dv = new LibmsiDeleteView
            {
                Database = db,
                Table = table,
            };

            view = dv;
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
        public override LibmsiResult FetchStream(int row, int col, out GsfInput stm)
        {
            stm = null;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = Table.Execute(record);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = Table.GetDimensions(out int rows, out int cols);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            // Blank out all the rows that match */
            for (int i = 0; i < rows; i++)
            {
                Table.DeleteRow(i);
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
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

            rows = 0;
            return Table.GetDimensions(out _, out cols);
        }

        /// <inheritdoc/>
        public override LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name)
        {
            name = null; type = 0; temporary = false; table_name = null;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return Table.GetColumnInfo(n, out name, out type, out temporary, out table_name);
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
    }
}