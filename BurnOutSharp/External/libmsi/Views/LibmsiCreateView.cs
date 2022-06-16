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

using LibMSI.Internal;
using static LibMSI.Internal.LibmsiTable;
using static LibMSI.Internal.MsiPriv;

namespace LibMSI.Views
{
    internal class LibmsiCreateView : LibmsiView
    {
        #region Properties

        public LibmsiDatabase Database { get; set; }

        public string Name { get; set; }

        public bool IsTemp { get; set; }

        public bool Hold { get; set; }

        public column_info ColInfo { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiCreateView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, string table, column_info col_info, bool hold)
        {
            view = null;

            LibmsiResult r = CheckColumns(col_info);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            LibmsiCreateView cv = new LibmsiCreateView();

            bool temp = true;
            bool tempprim = false;

            column_info col;
            for (col = col_info; col != null; col = col.Next)
            {
                if (col.Table == null)
                    col.Table = table;

                if (!col.Temporary)
                    temp = false;
                else if ((col.Type & MSITYPE_KEY) != 0)
                    tempprim = true;
            }

            if (!temp && tempprim)
            {
                cv = null;
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            // Fill the structure
            cv.Database = db;
            cv.Name = table;
            cv.ColInfo = col_info;
            cv.IsTemp = temp;
            cv.Hold = hold;

            view = cv;

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
            LibmsiCondition persist = (IsTemp) ? LibmsiCondition.LIBMSI_CONDITION_FALSE : LibmsiCondition.LIBMSI_CONDITION_TRUE;
            if (IsTemp && !Hold)
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            return CreateTable(Database, Name, ColInfo, persist);
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
            Database = null;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Utilities

        private static LibmsiResult CheckColumns(column_info col_info)
        {
            column_info c1, c2;

            // Check for two columns with the same name
            for (c1 = col_info; c1 != null; c1 = c1.Next )
            {
                for (c2 = c1.Next; c2 != null; c2 = c2.Next)
                {
                    if (c1.Column == c2.Column)
                        return LibmsiResult.LIBMSI_RESULT_BAD_QUERY_SYNTAX;
                }
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion
    }
}