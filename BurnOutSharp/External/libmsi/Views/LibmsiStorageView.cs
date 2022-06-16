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

using System;
using LibGSF.Input;
using LibMSI.Internal;
using static LibMSI.LibmsiRecord;
using static LibMSI.Internal.MsiPriv;

namespace LibMSI.Views
{
    internal class STORAGE
    {
        public int StrIndex { get; set; }
    }

    internal class LibmsiStorageView : LibmsiView
    {
        #region Constants

        private const int NUM_STORAGES_COLS = 2;
        private const int MAX_STORAGES_NAME_LEN = 62;

        #endregion

        #region Properties

        public LibmsiDatabase Database { get; set; }

        public STORAGE[] Storages { get; set; }

        public int MaxStorages { get; set; }

        public int NumRows { get; set; }

        public int RowSize { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiStorageView() { }

        public static LibmsiResult Create(LibmsiDatabase db, out LibmsiView view)
        {
            view = null;
            LibmsiStorageView sv = new LibmsiStorageView
            {
                Database = db,
            };

            LibmsiResult r = sv.AddStoragesToTable();
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            view = sv;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Functions

        private bool StoragesSetTableSize(int size)
        {
            if (size >= MaxStorages)
            {
                MaxStorages *= 2;
                STORAGE[] temp = new STORAGE[MaxStorages];
                Array.Copy(Storages, temp, Storages.Length);
                Storages = temp;
            }

            return true;
        }

        private STORAGE CreateStorage(string name)
        {
            return new STORAGE
            {
                StrIndex = Database.Strings.AddString(name, -1, 1, StringPersistence.StringNonPersistent)
            };
        }

        /// <inheritdoc/>
        public override LibmsiResult FetchInt(int row, int col, out int val)
        {
            val = 0;
            if (col != 1)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            if (row >= NumRows)
                return LibmsiResult.NO_MORE_ITEMS;

            val = Storages[row].StrIndex;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FetchStream(int row, int col, out GsfInput stm)
        {
            stm = null;
            if (row >= NumRows)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            return LibmsiResult.LIBMSI_RESULT_INVALID_DATA;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetRow(int row, out LibmsiRecord rec)
        {
            rec = null;
            return LibmsiResult.LIBMSI_RESULT_CALL_NOT_IMPLEMENTED;
        }

        /// <inheritdoc/>
        public override LibmsiResult SetRow(int row, LibmsiRecord rec, int mask)
        {
            STORAGE storage;
            string name = null;

            if (row > NumRows)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            LibmsiResult r = GetGsfInput(rec, 2, out GsfInput stm);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            if (Storages[row] != null)
            {
                if ((mask & 1) != 0)
                {
                    Console.Error.WriteLine("FIXME: renaming storage via UPDATE on _Storages table");
                    return r;
                }

                storage = Storages[row];
                name = Database.Strings.LookupId(storage.StrIndex);
            }
            else
            {
                name = RecordGetStringRaw(rec, 1);
            }

            if (name == null)
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

            Database.CreateStorage(name, stm);
            storage = CreateStorage(name);
            if (storage == null)
                r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            Storages[row] = storage;
            return r;
        }

        /// <inheritdoc/>
        public override LibmsiResult InsertRow(LibmsiRecord record, int row, bool temporary)
        {
            if (!StoragesSetTableSize(++NumRows))
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (row == -1)
                row = NumRows - 1;

            // FIXME have to readjust rows

            return SetRow(row, record, 0);
        }

        /// <inheritdoc/>
        public override LibmsiResult DeleteRow(int row)
        {
            if (row > NumRows)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            string name = Database.Strings.LookupId(Storages[row].StrIndex);
            if (name == null)
            {
                Console.Error.WriteLine("Failed to retrieve storage name");
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            Database.DestroyStorage(name);

            // Shift the remaining rows
            for (int i = row + 1; i < NumRows; i++)
            {
                Storages[i - 1] = Storages[i];
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
            cols = NUM_STORAGES_COLS;
            rows = NumRows;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult GetColumnInfo(int n, out string name, out int type, out bool temporary, out string table_name)
        {
            name = null; type = 0; temporary = false; table_name = null;
            if (n == 0 || n > NUM_STORAGES_COLS)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            switch (n)
            {
                case 1:
                    name = szName;
                    type = MSITYPE_STRING | MSITYPE_VALID | MAX_STORAGES_NAME_LEN;
                    break;

                case 2:
                    name = szData;
                    type = MSITYPE_STRING | MSITYPE_VALID | MSITYPE_NULLABLE;
                    break;

                default:
                    Console.Error.WriteLine($"Invalid case {n}");
                    break;
            }

            table_name = szStorages;
            temporary = false;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult Delete()
        {
            Storages = null;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <inheritdoc/>
        public override LibmsiResult FindMatchingRows(int col, int val, out int row, ref LibmsiColumnHashEntry handle)
        {
            row = 0;
            if (col == 0 || col > NUM_STORAGES_COLS)
                return LibmsiResult.LIBMSI_RESULT_INVALID_PARAMETER;

            while (handle.Row < NumRows)
            {
                if (Storages[handle.Row].StrIndex == val)
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

        #region Utilities

        private static LibmsiResult AddStorageToTable(string name, GsfInfile stg, object opaque)
        {
            LibmsiStorageView sv = (LibmsiStorageView)opaque;
            STORAGE storage = sv.CreateStorage(name);
            if (storage == null)
                return LibmsiResult.LIBMSI_RESULT_NOT_ENOUGH_MEMORY;

            if (!sv.StoragesSetTableSize(++sv.NumRows))
                return LibmsiResult.LIBMSI_RESULT_NOT_ENOUGH_MEMORY;

            sv.Storages[sv.NumRows - 1] = storage;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult AddStoragesToTable()
        {
            MaxStorages = 1;
            Storages = new STORAGE[1];
            return Database.EnumDbStorages(AddStorageToTable, this);
        }

        #endregion
    }
}