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

using System;
using LibGSF.Input;

namespace LibMSI
{
    internal class column_info
    {
        public string Table { get; set; }

        public string Column { get; set; }

        public int Type { get; set; }

        public bool Temporary { get; set; }

        public expr Val { get; set; }

        public column_info Next { get; set; }
    }

    internal abstract class LibmsiView
    {
        #region Properties

        public LibmsiDBError Error { get; set; }

        public string ErrorColumn { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Reads one integer from {row,col} in the table
        /// </summary>
        /// <remarks>
        /// This function should be called after the execute method.
        /// Data returned by the function should not change until
        ///  close or delete is called.
        /// To get a string value, query the database's string table with
        ///  the integer value returned from this function.
        /// </remarks>
        public virtual LibmsiResult FetchInt(int row, int col, out int val) => throw new NotImplementedException();

        /// <summary>
        /// Gets a stream from {row,col} in the table
        /// </summary>
        /// <remarks>
        /// This function is similar to FetchInt, except fetches a
        ///  stream instead of an integer.
        /// </remarks>
        public virtual LibmsiResult FetchStream(int row, int col, out GsfInput stm) => throw new NotImplementedException();

        /// <summary>
        /// Gets values from a row
        /// </summary>
        public virtual LibmsiResult GetRow(int row, out LibmsiRecord rec) => throw new NotImplementedException();

        /// <summary>
        /// Sets values in a row as specified by mask
        /// </summary>
        /// <remarks>Similar semantics to fetch_int</remarks>
        public virtual LibmsiResult SetRow(int row, LibmsiRecord rec, int mask) => throw new NotImplementedException();

        /// <summary>
        /// Inserts a new row into the database from the records contents
        /// </summary>
        public virtual LibmsiResult InsertRow(LibmsiRecord record, int row, bool temporary) => throw new NotImplementedException();

        /// <summary>
        /// Deletes a row from the database
        /// </summary>
        public virtual LibmsiResult DeleteRow(int row) => throw new NotImplementedException();

        /// <summary>
        /// Loads the underlying data into memory so it can be read
        /// </summary>
        public virtual LibmsiResult Execute(LibmsiRecord record) => throw new NotImplementedException();

        /// <summary>
        /// Clears the data read by execute from memory
        /// </summary>
        public virtual LibmsiResult Close() => throw new NotImplementedException();

        /// <summary>
        /// Returns the number of rows or columns in a table.
        /// </summary>
        /// <remarks>
        /// The number of rows can only be queried after the execute method
        ///  is called. The number of columns can be queried at any time.
        /// </remarks>
        public virtual LibmsiResult GetDimensions(out int rows, out int cols) => throw new NotImplementedException();

        /// <summary>
        /// Returns the name and type of a specific column
        /// </summary>
        /// <remarks>The column information can be queried at any time.</remarks>
        public virtual LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name) => throw new NotImplementedException();

        /// <summary>
        /// Destroys the structure completely
        /// </summary>
        public virtual LibmsiResult Delete() => throw new NotImplementedException();

        /// <summary>
        /// Iterates through rows that match a value
        /// </summary>
        /// <remarks>
        /// If the column type is a string then a string ID should be passed in.
        ///  If the value to be looked up is an integer then no transformation of
        ///  the input value is required, except if the column is a string, in which
        ///  case a string ID should be passed in.
        /// The handle is an input/output parameter that keeps track of the current
        ///  position in the iteration. It must be initialised to zero before the
        ///  first call and continued to be passed in to subsequent calls.
        /// </remarks>
        public virtual LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle) => throw new NotImplementedException();

        /// <summary>
        /// Increases the reference count of the table
        /// </summary>
        public virtual int AddRef() => throw new NotImplementedException();

        /// <summary>
        /// Decreases the reference count of the table
        /// </summary>
        public virtual int Release() => throw new NotImplementedException();

        /// <summary>
        /// Adds a column to the table
        /// </summary>
        public virtual LibmsiResult AddColumn(string table, int number, string column, int type, bool hold) => throw new NotImplementedException();

        /// <summary>
        /// Removes the column represented by table name and column number from the table
        /// </summary>
        public virtual LibmsiResult RemoveColumn(string table, int number) => throw new NotImplementedException();

        /// <summary>
        /// Orders the table by columns
        /// </summary>
        public virtual LibmsiResult Sort(column_info columns) => throw new NotImplementedException();

        /// <summary>
        /// Drops the table from the database
        /// </summary>
        public virtual LibmsiResult Drop() => throw new NotImplementedException();

        #endregion
    }
}