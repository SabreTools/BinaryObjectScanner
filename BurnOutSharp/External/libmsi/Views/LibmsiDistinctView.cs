/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002 Mike McCormack for CodeWeavers
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

namespace LibMSI.Views
{
    internal class LibmsiDistinctSet
    {
        public int Val { get; set; }

        public int Count { get; set; }

        public int Row { get; set; }

        public LibmsiDistinctSet NextRow { get; set; }

        public LibmsiDistinctSet NextCol { get; set; }
    }

    internal class LibmsiDistinctView : LibmsiView
    {
        #region Properties

        public LibmsiDatabase Database { get; set; }

        public LibmsiView Table { get; set; }

        public int RowCount { get; set; }

        public int[] Translation { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiDistinctView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view, LibmsiView table)
        {
            view = null;
            LibmsiResult r = table.GetDimensions(out _, out int count);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                Console.Error.WriteLine("Can't get table dimensions");
                return r;
            }

            LibmsiDistinctView dv = new LibmsiDistinctView
            {
                Database = db,
                Table = table,
                Translation = null,
                RowCount = 0,
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
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (row >= RowCount)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            row = Translation[row];

            return Table.FetchInt(row, col, out val);
        }

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record)
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = Table.Execute(record);
            if( r != LibmsiResult.LIBMSI_RESULT_SUCCESS )
                return r;

            r = Table.GetDimensions(out int r_count, out int c_count );
            if( r != LibmsiResult.LIBMSI_RESULT_SUCCESS )
                return r;

            Translation = new int[r_count];

            // Build it
            LibmsiDistinctSet rowset = null;
            for (int i = 0; i < r_count; i++ )
            {
                LibmsiDistinctSet x = rowset;

                for (int j = 1; j <= c_count; j++)
                {
                    r = Table.FetchInt(i, j, out int val);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    {
                        Console.Error.WriteLine($"Failed to fetch int at {i} {j}");
                        DistinctFree(rowset);
                        return r;
                    }

                    x = DistinctInsert(x, val, i);
                    if(x == null)
                    {
                        Console.Error.WriteLine($"Failed to insert at {i} {j}");
                        DistinctFree(rowset);
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                    }

                    if (j != c_count)
                        x = x.NextCol;
                }

                // Check if it was distinct and if so, include it
                if (x.Row == i )
                    Translation[RowCount++] = i;
            }

            DistinctFree( rowset );

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult Close()
        {
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            Translation = null;
            RowCount = 0;

            return Table.Close();
        }

        /// <inheritdoc/>
        public override LibmsiResult GetDimensions(out int rows, out int cols)
        {
            rows = 0; cols = 0;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (Translation == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            
            rows = RowCount;
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

            Translation = null;
            Database = null;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle)
        {
            row = 0;
            if (Table == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = Table.FindMatchingRows(col, val, out row, ref handle);
            if (row > RowCount)
                return LibmsiResult.NO_MORE_ITEMS;

            row = Translation[row];
            return r;
        }

        #endregion

        #region Utilities

        private static LibmsiDistinctSet DistinctInsert(LibmsiDistinctSet x, int val, int row)
        {
            // Horrible O(n) find
            while (x != null)
            {
                if (x.Val == val)
                {
                    x.Count++;
                    return x;
                }

                x = x.NextRow;
            }

            // Nothing found, so add one
            x = new LibmsiDistinctSet
            {
                Val = val,
                Count = 1,
                Row = row,
                NextRow = null,
                NextCol = null,
            };

            return x;
        }

        private static void DistinctFree(LibmsiDistinctSet x)
        {
            while (x != null)
            {
                LibmsiDistinctSet next = x.NextRow;
                DistinctFree(x.NextCol);
                x = next;
            }
        }

        #endregion
    }
}