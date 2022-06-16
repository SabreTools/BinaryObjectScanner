/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2008 James Hawkins
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

namespace LibMSI.Views
{
    internal class LibmsiDropView : LibmsiView
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
        /// </summary
        private LibmsiDropView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, string name)
        {
            view = null;

            LibmsiDropView dv = new LibmsiDropView();
            LibmsiResult r = LibmsiTableView.Create(db, name, out LibmsiView table);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                dv = null;
                return r;
            }

            dv.Table = table;
            dv.Database = db;

            view = dv;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = Table.Execute(record);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            return Table.Drop();
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
        public override LibmsiResult Delete()
        {
            if (Table != null)
                Table.Delete();

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion
    }
}