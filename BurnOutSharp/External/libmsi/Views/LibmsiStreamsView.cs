/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2007 James Hawkins
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
using LibMSI.Internal;
using static LibMSI.LibmsiQuery;
using static LibMSI.Internal.LibmsiTable;
using static LibMSI.Internal.MsiPriv;

namespace LibMSI.Views
{
    internal class STREAM
    {
        public int StrIndex { get; set; }

        public GsfInput Stream { get; set; }
    }

    internal class LibmsiStreamsView : LibmsiView
    {
        #region Constants

        private const int NUM_STREAMS_COLS = 2;

        #endregion

        #region Properties

        public LibmsiDatabase Database { get; set; }

        public STREAM[] Streams { get; set; }

        public int MaxStreams { get; set; }

        public int NumRows { get; set; }

        public int RowSize { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiStreamsView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view)
        {
            view = null;
            LibmsiStreamsView sv = new LibmsiStreamsView
            {
                Database = db,
            };

            LibmsiResult r = sv.AddStreamsToTable();
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            view = sv;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        public override LibmsiResult FetchInt(int row, int col, out int val)
        {
            val = 0;
            if (col != 1)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (row >= NumRows)
                return LibmsiResult.NO_MORE_ITEMS;

            val = Streams[row].StrIndex;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FetchStream(int row, int col, out GsfInput stm)
        {
            stm = null;
            if (row >= NumRows)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            stm = Streams[row].Stream;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetRow(int row, out LibmsiRecord rec) => MsiViewGetRow(Database, this, row, out rec);

        /// <inheritdoc/>
        public override LibmsiResult SetRow(int row, LibmsiRecord rec, int mask)
        {
            if (row > NumRows)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = rec.GetGsfInput(2, out GsfInput stm);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            STREAM stream = null;
            string name = null;
            if (Streams[row] != null)
            {
                if ((mask & 1) != 0)
                {
                    Console.Error.WriteLine("FIXME: renaming stream via UPDATE on _Streams table");
                    return r;
                }

                stream = Streams[row];
                name = Database.Strings.LookupId(stream.StrIndex);
            }
            else
            {
                name = rec.GetStringRaw(1);
                if (name == null)
                {
                    Console.Error.WriteLine("Failed to retrieve stream name");
                    return r;
                }

                stream = CreateStream(name, false, stm);
            }

            if (stream == null)
                return r;

            r = Database.CreateStream(name, stm);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                Console.Error.WriteLine($"Failed to create stream: {r}");
                return r;
            }

            Streams[row] = stream;
            return r;
        }

        /// <inheritdoc/>
        public override LibmsiResult InsertRow(LibmsiRecord record, int row, bool temporary)
        {
            if (!StreamsSetTableSize(++NumRows))
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (row == -1)
                row = NumRows - 1;

            // Shift the rows to make room for the new row
            for (int i = NumRows - 1; i > row; i--)
            {
                Streams[i] = Streams[i - 1];
            }

            return SetRow(row, record, 0);
        }

        /// <inheritdoc/>
        public override LibmsiResult DeleteRow(int row)
        {
            string name;
            string encname;

            if (row > NumRows)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            name = Database.Strings.LookupId(Streams[row].StrIndex);
            if (name == null)
            {
                Console.Error.WriteLine("Failed to retrieve stream name");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            encname = EncodeStreamName(false, name).TrimEnd('\0');
            Database.DestroyStream(encname);

            // Shift the remaining rows
            for (int i = row + 1; i < NumRows; i++)
            {
                Streams[i - 1] = Streams[i];
            }

            NumRows--;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult Execute(LibmsiRecord record) => LibmsiResult.LIBMSI_RESULT_SUCCESS;

        /// <inheritdoc/>
        public override LibmsiResult Close() => LibmsiResult.LIBMSI_RESULT_SUCCESS;

        /// <inheritdoc/>
        public override LibmsiResult GetDimensions(out int rows, out int cols)
        {
            cols = NUM_STREAMS_COLS;
            rows = NumRows;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name)
        {
            name = null; type = 0; temporary = false; table_name = null;
            if (n == 0 || n > NUM_STREAMS_COLS)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            switch (n)
            {
                case 1:
                    name = szName;
                    type = MSITYPE_STRING | MSITYPE_VALID | MAX_STREAM_NAME_LEN;
                    break;

                case 2:
                    name = szData;
                    type = MSITYPE_STRING | MSITYPE_VALID | MSITYPE_NULLABLE;
                    break;

                default:
                    Console.Error.WriteLine($"Invalid value: {n}");
                    break;
            }

            table_name = szStreams;
            temporary = false;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult Delete() => LibmsiResult.LIBMSI_RESULT_SUCCESS;

        public override LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle)
        {
            row = 0;
            if (col == 0 || col > NUM_STREAMS_COLS)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            while (handle.Row < NumRows)
            {
                if (Streams[handle.Row].StrIndex == val)
                {
                    row = handle.Row;
                    break;
                }

                handle = handle.Next;
            }

            handle = handle.Next;
            if (handle.Row >= NumRows)
                return LibmsiResult.NO_MORE_ITEMS;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Helpers

        private bool StreamsSetTableSize(int size)
        {
            if (size >= MaxStreams)
            {
                STREAM[] temp = new STREAM[MaxStreams * 2];
                Array.Copy(Streams, temp, Streams.Length);
                Streams = temp;
            }

            return true;
        }

        private STREAM CreateStream(string name, bool encoded, GsfInput stm)
        {
            STREAM stream = new STREAM();
            string decoded = null;
            if (encoded)
            {
                decoded = DecodeStreamName(name);
                name = decoded;
            }

            stream.StrIndex = Database.Strings.AddString(name, -1, 1, StringPersistence.StringNonPersistent);
            stream.Stream = stm;
            return stream;
        }

        private static LibmsiResult AddStreamToTable(string name, GsfInput stm, object opaque)
        {
            LibmsiStreamsView sv = (LibmsiStreamsView)opaque;
            STREAM stream = sv.CreateStream(name, true, stm);
            if (stream == null)
                return LibmsiResult.LIBMSI_RESULT_NOT_ENOUGH_MEMORY;

            if (!sv.StreamsSetTableSize(++sv.NumRows))
                return LibmsiResult.LIBMSI_RESULT_NOT_ENOUGH_MEMORY;

            sv.Streams[sv.NumRows - 1] = stream;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult AddStreamsToTable()
        {
            MaxStreams = 1;
            Streams = new STREAM[1];
            return Database.EnumDbStreams(AddStreamToTable, this);
        }

        #endregion
    }
}