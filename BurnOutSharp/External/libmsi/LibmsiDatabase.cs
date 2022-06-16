/*
 * Implementation of the Microsoft Installer (msi.dll)
 *
 * Copyright 2002,2003,2004,2005 Mike McCormack for CodeWeavers
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibGSF.Input;
using LibGSF.Output;
using LibMSI.Internal;
using LibMSI.Views;
using static LibMSI.LibmsiQuery;
using static LibMSI.LibmsiRecord;
using static LibMSI.LibmsiSummaryInfo;
using static LibMSI.Internal.LibmsiTable;
using static LibMSI.Internal.MsiPriv;
using static LibMSI.Internal.StringTable;

namespace LibMSI
{
    internal class LibmsiTransform
    {
        public GsfInfile Stg { get; set; }
    }

    internal class LibmsiStorage
    {
        public string Name { get; set; }

        public GsfInfile Stg { get; set; }
    }

    internal class LibmsiStream
    {
        public string Name { get; set; }

        public GsfInput Stm { get; set; }
    }

    /// <summary>
    /// .MSI  file format
    /// 
    /// An .msi file is a structured storage file.
    /// It contains a number of streams.
    /// A stream for each table in the database.
    /// Two streams for the string table in the database.
    /// Any binary data in a table is a reference to a stream.
    /// </summary>
    public class LibmsiDatabase
    {
        #region Constants

        internal static readonly byte[] clsid_msi_transform = { 0x82, 0x10, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 };

        internal static readonly byte[] clsid_msi_database = { 0x84, 0x10, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 };

        internal static readonly byte[] clsid_msi_patch = { 0x86, 0x10, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 };

        #endregion

        #region Classes

        private class ExportRow
        {
            public Stream FD { get; set; }

            public string TableDir { get; set; }

            public Exception Error { get; set; }
        }

        private class MERGETABLE
        {
            public LinkedList<MERGEROW> Rows { get; set; }

            public string Name { get; set; }

            public int NumConflicts { get; set; }

            public string[] Columns { get; set; }

            public int NumColumns { get; set; }

            public string[] Types { get; set; }

            public int NumTypes { get; set; }

            public string[] Labels { get; set; }

            public int NumLabels { get; set; }
        }

        private class MERGEROW
        {
            public LibmsiRecord Data { get; set; }
        }

        private class MERGEDATA
        {
            public LibmsiDatabase Database { get; set; }

            public LibmsiDatabase Merge { get; set; }

            public MERGETABLE CurTable { get; set; }

            public LibmsiQuery CurView { get; set; }

            public LinkedList<MERGETABLE> TableData { get; set; }
        }

        private class MsiPrimaryKeyRecordInfo
        {
            public int N { get; set; }

            public LibmsiRecord Rec { get; set; }
        }

        #endregion

        #region Properties

        internal GsfInfile Infile { get; set; }

        internal GsfOutfile Outfile { get; set; }

        internal StringTable Strings { get; set; }

        internal int BytesPerStrref { get; set; }

        internal string Path { get; set; }

        internal string Outpath { get; set; }

        internal bool RenameOutpath { get; set; }

        internal LibmsiDbFlags Flags { get; set; }

        internal int MediaTransformOffset { get; set; }

        internal int MediaTransformDiskId { get; set; }

        internal LinkedList<LibmsiTable> Tables { get; set; }

        internal LinkedList<LibmsiTransform> Transforms { get; set; }

        internal LinkedList<LibmsiStream> Streams { get; set; }

        internal LinkedList<LibmsiStorage> Storages { get; set; }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private LibmsiDatabase() { }

        /// <summary>
        /// Create a MSI database or open from <paramref name="path"/>.
        /// </summary>
        /// <param name="path">An MST transform file path</param>
        /// <param name="flags">LibmsiDbFlags opening flags</param>
        /// <param name="persist">Path to output MSI file</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>A new LibmsiDatabase on success, null if fail.</returns>
        public static LibmsiDatabase Create(string path, LibmsiDbFlags flags, string persist, ref Exception error)
        {
            if (path == null)
                return null;
            if (error != null)
                return null;

            LibmsiDatabase self = new LibmsiDatabase
            {
                Path = path,
                Outpath = persist,
                Flags = flags,
            };

            if (!self.Init(ref error))
                return null;

            return self;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~LibmsiDatabase()
        {
            Close(false);
            FreeCachedTables(this);
            Transforms.Clear();
        }

        #endregion

        #region Functions

        /// <returns>True if the database is read-only</returns>
        public bool IsReadOnly() => Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_READONLY);

        /// <param name="table">An exisiting table name</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>A LibmsiRecord containing the names of all the primary key columns.</returns>
        public LibmsiRecord GetPrimaryKeys(string table, ref Exception error)
        {
            if (table == null)
                return null;
            if (error != null)
                return null;

            LibmsiResult r = GetPrimaryKeys(table, out LibmsiRecord rec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {r}");

            return rec;
        }

        /// <summary>
        /// FIXME
        /// </summary>
        /// <param name="file">An MST transform file path</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool ApplyTransform(string file, ref Exception error)
        {
            if (file == null)
                return false;
            if (error != null)
                return false;

            LibmsiResult r = ApplyTransform(file);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {r}");

            return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <summary>
        /// Writes a file containing the table data as tab separated ASCII.
        /// </summary>
        /// <param name="table">A table name</param>
        /// <param name="fd">A file descriptor</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        /// <remarks>
        /// The format is as follows:
        /// 
        /// row1 : colname1 <tab> colname2 <tab> .... colnameN <cr> <lf>
        /// row2 : coltype1 <tab> coltype2 <tab> .... coltypeN <cr> <lf>
        /// row3 : tablename <tab> key1 <tab> key2 <tab> ... keyM <cr> <lf>
        /// 
        /// Followed by the data, starting at row 1 with one row per line
        /// 
        /// row4 : data <tab> data <tab> data <tab> ... data <cr> <lf>
        /// </remarks>
        public bool Export(string table, Stream fd, ref Exception error)
        {
            if (table == null)
                return false;
            if (fd == null || !fd.CanWrite)
                return false;
            if (error != null)
                return false;

            LibmsiResult r = ExportImpl(table, fd, ref error);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {r}");

            return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <remarks>Adapted from WixToolset</remarks>
        public bool ExportAll(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            // SummaryInfo

            Exception err = null;
            string summaryInfoPath = System.IO.Path.Combine(path, "_SummaryInformation.idt");
            using (Stream summaryInfoStream = File.OpenWrite(summaryInfoPath))
            {
                Export("_SummaryInformation", summaryInfoStream, ref err);
            }

            // Tables

            err = null;
            LibmsiQuery tableQuery = LibmsiQuery.Create(this, $"SELECT `Name` FROM `_Tables`", ref err);
            tableQuery.Execute(null, ref err);

            LibmsiRecord tableRecord = tableQuery.Fetch(ref err);
            while (tableRecord != null)
            {
                err = null;
                string table = tableRecord.GetString(1);
                string tablePath = System.IO.Path.Combine(path, $"{table}.idt");
                using (Stream tableStream = File.OpenWrite(tablePath))
                {
                    Export(table, tableStream, ref err);
                }

                tableRecord = tableQuery.Fetch(ref err);
            }

            // Streams

            if (!Directory.Exists(System.IO.Path.Combine(path, "_Streams")))
                Directory.CreateDirectory(System.IO.Path.Combine(path, "_Streams"));

            err = null;
            LibmsiQuery streamsQuery = LibmsiQuery.Create(this, $"SELECT `Name`, `Data` FROM `_Streams`", ref err);
            streamsQuery.Execute(null, ref err);

            LibmsiRecord streamRecord = streamsQuery.Fetch(ref err);
            while (streamRecord != null)
            {
                err = null;
                string stream = streamRecord.GetString(1);
                if (stream.EndsWith("SummaryInformation", StringComparison.Ordinal))
                    continue;

                int i = stream.IndexOf('.');
                if (i >= 0)
                {
                    if (File.Exists(System.IO.Path.Combine(path, stream.Substring(0, i), stream.Substring(i + 1) + ".ibd")))
                        continue;
                }

                using (Stream streamRecordStream = streamRecord.GetStream(2))
                using (Stream streamRecordOutput = File.OpenWrite(System.IO.Path.Combine(path, "_Streams", stream)))
                {
                    streamRecordStream.CopyTo(streamRecordOutput);
                }

                streamRecord = tableQuery.Fetch(ref err);
            }

            return true;
        }

        /// <summary>
        /// Import a table to the database from file <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Path to a table file</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool Import(string path, ref Exception error)
        {
            if (path == null)
                return false;
            if (error != null)
                return false;

            LibmsiResult r = Import(path);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {r}");

            return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <param name="table">An exisiting table name</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True if the <paramref name="table"/> is persistent, false if it's temporary</returns>
        public bool IsTablePersistent(string table, ref Exception error)
        {
            if (table == null)
                return false;
            if (error != null)
                return false;

            LibmsiCondition r = LibmsiTable.IsTablePersistent(this, table);
            if (r == LibmsiCondition.LIBMSI_CONDITION_NONE)
                error = new InvalidDataException("The table is unknown");
            else if (r == LibmsiCondition.LIBMSI_CONDITION_ERROR)
                error = new Exception("Error");

            return r == LibmsiCondition.LIBMSI_CONDITION_TRUE;
        }

        /// <param name="merge">A LibmsiDatabase to merge</param>
        /// <param name="tablename">An optionnal table name</param>
        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool Merge(LibmsiDatabase merge, string tablename, ref Exception error)
        {
            if (merge == null)
                return false;
            if (tablename != null && string.IsNullOrEmpty(tablename))
                return false;
            if (error != null)
                return false;

            LinkedList<MERGETABLE> tabledata = new LinkedList<MERGETABLE>();
            bool conflicts;

            LibmsiResult r = GatherMergeData(merge, tabledata.First());
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto done;

            conflicts = false;

            foreach (MERGETABLE table in tabledata)
            {
                if (table.NumConflicts != 0)
                {
                    conflicts = true;
                    r = UpdateMergeErrors(tablename, table.Name, table.NumConflicts);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        break;
                }
                else
                {
                    r = MergeTable(table);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        break;
                }
            }

            foreach (MERGETABLE table in tabledata)
            {
                FreeMergeTable(table);
            }

            tabledata.Clear();

            if (conflicts)
                r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            done:
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                error = new Exception($"LIBMSI_RESULT_ERROR: {r}");

            return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        /// <param name="error">Exception to set on error, or null</param>
        /// <returns>True on success</returns>
        public bool Commit(ref Exception error)
        {
            if (error != null)
                return false;

            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            if (Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_READONLY))
                return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;

            // FIXME: lock the database

            r = Strings.SaveStringTable(this, out int bytes_per_strref);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                error = new Exception($"Failed to save string table r={r}");
                return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            r = EnumDbStorages(CommitStorage, this);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                error = new Exception($"Failed to save storages r={r}");
                return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            r = EnumDbStreams(CommitStream, this);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                error = new Exception($"Failed to save streams r={r}");
                return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            r = DatabseCommitTables(this, bytes_per_strref);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                error = new Exception($"Failed to save tables r={r}");
                return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            BytesPerStrref = bytes_per_strref;

            // FIXME: unlock the database

            Close(true);
            Flags &= ~LibmsiDbFlags.LIBMSI_DB_FLAGS_CREATE;
            Flags |= LibmsiDbFlags.LIBMSI_DB_FLAGS_TRANSACT;
            Open();
            StartTransaction();

            return r == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion

        #region Internal Functions

        internal LibmsiResult OpenStorage(string stname)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_NOT_ENOUGH_MEMORY;
            foreach (LibmsiStorage stg in Storages)
            {
                if (stname == stg.Name)
                    return r;
            }

            LibmsiStorage storage = new LibmsiStorage { Name = stname };

            Exception err = null;
            GsfInput input = Infile.ChildByName(stname, ref err);
            if (input == null || !(input is GsfInfile))
                return r;

            storage.Stg = input as GsfInfile;
            if (storage.Stg == null)
                return r;

            Storages.AddFirst(storage);
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult CreateStorage(string stname, GsfInput stm)
        {
            if (Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_READONLY))
                return LibmsiResult.LIBMSI_RESULT_ACCESS_DENIED;

            bool found = false;
            LibmsiStorage storage = null;
            foreach (LibmsiStorage stg in Storages)
            {
                if (stname == stg.Name)
                {
                    found = true;
                    storage = stg;
                    break;
                }
            }

            if (!found)
                storage = new LibmsiStorage { Name = stname };

            Exception err = null;
            GsfInfile origstg = GsfInfileMSOle.Create(stm, ref err);
            if (origstg == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (!found)
                Storages.AddFirst(storage);

            storage.Stg = origstg;

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal void DestroyStorage(string stname)
        {
            foreach (LibmsiStorage storage in Storages)
            {
                if (stname == storage.Name)
                {
                    Storages.Remove(storage);
                    break;
                }
            }
        }

        internal LibmsiResult WriteRawStreamData(string stname, byte[] data, int sz, out GsfInput outstm)
        {
            LibmsiResult ret = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            outstm = null;
            if (Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_READONLY))
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            foreach (LibmsiStream stream in Streams)
            {
                if (stname == stream.Name)
                {
                    DestroyStream(stname);
                    break;
                }
            }

            byte[] mem = new byte[sz == 0 ? 1 : sz];
            if (data != null || sz != 0)
                Array.Copy(data, mem, sz);

            GsfInput stm = GsfInputMemory.Create(mem, sz, true);
            ret = MsiAllocStream(stname, stm);
            outstm = stm;
            return ret;
        }

        internal LibmsiResult CreateStream(string stname, GsfInput stm)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            bool found = false;

            if (Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_READONLY))
                return LibmsiResult.LIBMSI_RESULT_ACCESS_DENIED;

            string encname = EncodeStreamName(false, stname);
            foreach (LibmsiStream stream in Streams)
            {
                if (encname == stream.Name)
                {
                    stream.Stm = stm;
                    found = true;
                    break;
                }
            }

            if (found)
                r = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            else
                r = MsiAllocStream(encname, stm);

            return r;
        }

        internal LibmsiResult EnumDbStreams(Func<string, GsfInput, object, LibmsiResult> fn, object opaque)
        {
            foreach (LibmsiStream stream in Streams)
            {
                GsfInput stm = stream.Stm;
                LibmsiResult r = fn(stream.Name, stm, opaque);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult EnumDbStorages(Func<string, GsfInfile, object, LibmsiResult> fn, object opaque)
        {
            foreach (LibmsiStorage storage in Storages)
            {
                GsfInfile stg = storage.Stg;
                LibmsiResult r = fn(storage.Name, stg, opaque);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult GetRawStream(string stname, out GsfInput stm)
        {
            string decoded = DecodeStreamName(stname);
            if (CloneInfileStream(stname, out stm) == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            LibmsiResult ret = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            foreach (LibmsiTransform transform in Transforms)
            {
                Exception err = null;
                stm = transform.Stg.ChildByName(stname, ref err);
                if (stm != null)
                {
                    ret = LibmsiResult.LIBMSI_RESULT_SUCCESS;
                    break;
                }
            }

            return ret;
        }

        internal void DestroyStream(string stname)
        {
            foreach (LibmsiStream stream in Streams)
            {
                if (stname == stream.Name)
                {
                    Streams.Remove(stream);
                    break;
                }
            }
        }

        internal void AppendStorageToDb(GsfInfile stg)
        {
            Transforms.AddLast(new LibmsiTransform { Stg = stg });
        }

        internal LibmsiResult Close(bool committed)
        {
            if (Strings != null)
            {
                Strings.Destroy();
                Strings = null;
            }

            if (Infile != null)
                Infile = null;

            if (Outfile != null)
            {
                Outfile.Close();
                Outfile = null;
            }

            Streams.Clear();
            Storages.Clear();

            if (Outpath != null)
            {
                if (!committed)
                {
                    //unlink(Outpath);
                }
                else if (RenameOutpath)
                {
                    //unlink(Path);
                    File.Move(Outpath, Path);
                }
                else
                {
                    Path = Outpath;
                }
            }

            Outpath = null;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult StartTransaction()
        {
            if (Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_READONLY))
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;

            RenameOutpath = false;
            if (Outpath == null)
            {
                string path = Path;
                if (Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_TRANSACT))
                {
                    path += ".tmp";
                    RenameOutpath = true;
                }

                Outpath = path;
            }

            Exception err = null;
            GsfOutput output = GsfOutputStdio.Create(Outpath, ref err);
            if (output == null)
            {
                Console.Error.WriteLine($"Open file failed for {Outpath}");
                return LibmsiResult.LIBMSI_RESULT_OPEN_FAILED;
            }

            GsfOutfileMSOle stg = GsfOutfileMSOle.Create(output);
            if (stg == null)
            {
                Console.Error.WriteLine($"Open failed for {Outpath}");
                return LibmsiResult.LIBMSI_RESULT_OPEN_FAILED;
            }

            if (!stg.SetClassID(Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_PATCH) ? clsid_msi_patch : clsid_msi_database))
            {
                Console.Error.WriteLine("Set guid failed");
                Outfile = null;
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            Outfile = stg;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        internal LibmsiResult Open()
        {
            byte[] uuid = new byte[16];
            LibmsiResult ret = LibmsiResult.LIBMSI_RESULT_OPEN_FAILED;

            Exception err = null;
            GsfInput input = GsfInputStdio.Create(Path, ref err);
            if (input == null)
            {
                Console.Error.WriteLine($"Open file failed for {Path}");
                return LibmsiResult.LIBMSI_RESULT_OPEN_FAILED;
            }

            GsfInfile stg = GsfInfileMSOle.Create(input, ref err);
            if (stg == null)
            {
                Console.Error.WriteLine($"Open file failed for {Path}");
                return LibmsiResult.LIBMSI_RESULT_OPEN_FAILED;
            }

            if (!(stg as GsfInfileMSOle).GetClassID(uuid))
            {
                Console.Error.WriteLine("FIXME: Failed to stat storage");
                goto end;
            }

            if (!uuid.SequenceEqual(clsid_msi_database) && !uuid.SequenceEqual(clsid_msi_patch) && !uuid.SequenceEqual(clsid_msi_transform))
            {
                Console.Error.WriteLine($"Storage GUID is not a MSI database GUID {BitConverter.ToString(uuid).Replace("-", string.Empty)}");
                goto end;
            }

            if (Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_PATCH) && !uuid.SequenceEqual(clsid_msi_patch))
            {
                Console.Error.WriteLine($"Storage GUID is not the MSI patch GUID {BitConverter.ToString(uuid).Replace("-", string.Empty)}");
                goto end;
            }

            Infile = stg;
            CacheInfileStructure();

            Strings = LoadStringTable(Infile, out int bytes_per_strref);
            if (Strings == null)
                goto end;

            BytesPerStrref = bytes_per_strref;
            ret = LibmsiResult.LIBMSI_RESULT_SUCCESS;

        end:
            if (ret != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                Infile = null;

            return ret;
        }

        internal LibmsiResult ApplyTransform(string szTransformFile)
        {
            LibmsiResult ret = LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            byte[] uuid = new byte[16];

            Exception err = null;
            GsfInput input = GsfInputStdio.Create(szTransformFile, ref err);
            if (input == null)
            {
                Console.Error.WriteLine($"Open file failed for transform {szTransformFile}");
                return LibmsiResult.LIBMSI_RESULT_OPEN_FAILED;
            }

            GsfInfile stg = GsfInfileMSOle.Create(input, ref err);
            if (!(stg as GsfInfileMSOle).GetClassID(uuid))
            {
                Console.Error.WriteLine("FIXME: Failed to stat storage");
                goto end;
            }

            if (!uuid.SequenceEqual(clsid_msi_transform))
                goto end;

            bool TRACE_ON = false; // TODO: Make configurable
            if (TRACE_ON && stg != null)
                EnumStreamNames(stg);

            ret = LibmsiTable.ApplyTransform(this, stg);

        end:
            return ret;
        }

        internal LibmsiResult GetPrimaryKeys(string table, out LibmsiRecord prec)
        {
            prec = null;
            if (!TableViewExists(this, table))
                return LibmsiResult.LIBMSI_RESULT_INVALID_TABLE;

            string sql = $"select * from `_Columns` where `Table` = '{table}'";
            LibmsiResult r = QueryOpen(this, out LibmsiQuery query, sql);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            // Count the number of primary key records
            MsiPrimaryKeyRecordInfo info = new MsiPrimaryKeyRecordInfo
            {
                N = 0,
                Rec = null,
            };

            int count = 0;
            r = query.IterateRecords(ref count, PrimaryKeyIterator, info);
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                // Allocate a record and fill in the names of the tables
                info.Rec = LibmsiRecord.Create(info.N);
                info.N = 0;

                count = 0;
                r = query.IterateRecords(ref count, PrimaryKeyIterator, info);
                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    prec = info.Rec;
            }

            return r;
        }

        #endregion

        #region Utilities

        private LibmsiResult FindInfileStream(string name, out GsfInput stm)
        {
            foreach (LibmsiStream stream in Streams)
            {
                if (name == stream.Name)
                {
                    stm = stream.Stm;
                    return LibmsiResult.LIBMSI_RESULT_SUCCESS;
                }
            }

            stm = null;
            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        private LibmsiResult MsiAllocStream(string name, GsfInput stm)
        {
            LibmsiStream stream = new LibmsiStream
            {
                Name = name,
                Stm = stm,
            };

            Streams.AddFirst(stream);
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult CloneInfileStream(string name, out GsfInput stm)
        {
            stm = null;
            if (FindInfileStream(name, out GsfInput stream) == LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                Exception err = null;
                stream = stream.Duplicate(ref err);
                if (stream == null)
                {
                    Console.Error.WriteLine("Failed to clone stream");
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }

                stream.Seek(0, SeekOrigin.Begin);
                stm = stream;
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
        }

        private string ReadTextArchive(string path, out int len)
        {
            try
            {
                string data = File.ReadAllText(path);
                len = data.Length;
                return data;
            }
            catch
            {
                len = 0;
                return null;
            }
        }

        private void ParseLine(ref string line, ref int linePtr, out string[] entries, out int num_entries, ref int len)
        {
            int ptr = linePtr;
            int save;
            int i, count = 1, chars_left = len;

            entries = null;

            // Stay on this line
            while (chars_left != 0 && line[ptr] != '\n')
            {
                // Entries are separated by tabs
                if (line[ptr] == '\t')
                    count++;

                ptr++;
                chars_left--;
            }


            entries = new string[count];

            // Store pointers into the data
            chars_left = len;
            for (i = 0, ptr = linePtr; i < count; i++)
            {
                while (chars_left != 0 && line[ptr] == '\r')
                {
                    ptr++;
                    chars_left--;
                }

                save = ptr;

                while (chars_left != 0 && line[ptr] != '\t' && line[ptr] != '\n' && line[ptr] != '\r')
                {
                    // Convert embedded nulls to \n
                    if (line[ptr] == '\0')
                    {
                        char[] temp = line.ToCharArray();
                        temp[ptr] = '\n';
                        line = new string(temp);
                    }

                    if (ptr > linePtr && line[ptr] == '\x19' && line[ptr - 1] == '\x11')
                    {
                        char[] temp = line.ToCharArray();
                        temp[ptr] = '\n';
                        temp[ptr - 1] = '\r';
                        line = new string(temp);
                    }

                    ptr++;
                    chars_left--;
                }

                // null-separate the data
                if (line[ptr] == '\n' || line[ptr] == '\r')
                {
                    while (chars_left != 0 && (line[ptr] == '\n' || line[ptr] == '\r'))
                    {
                        char[] temp = line.ToCharArray();
                        temp[ptr++] = '\0';
                        chars_left--;
                        line = new string(temp);
                    }
                }
                else if (line[ptr] != 0)
                {
                    char[] temp = line.ToCharArray();
                    temp[ptr++] = '\0';
                    chars_left--;
                    line = new string(temp);
                }

                entries[i] = new string(line.Skip(save).TakeWhile(c => c != '\0').ToArray());
            }

            // Move to the next line if there's more, else EOF
            linePtr = ptr;
            len = chars_left;
            num_entries = count;
        }

        private string BuildCreateSQLPrelude(string table) => $"CREATE TABLE `{table}` (";

        private string BuildCreateSQLColumns(string[] columns_data, string[] types, int num_columns)
        {
            int sql_size = 1;
            long len;

            string columns = string.Empty;
            for (int i = 0; i < num_columns; i++)
            {
                string comma;
                if (i == num_columns - 1)
                    comma = "\0";
                else
                    comma = ",";

                comma += '\0';

                int ptr = 1; // types[i][1];
                len = long.Parse(new string(types[i].Skip(1).SkipWhile(c => char.IsWhiteSpace(c)).TakeWhile(c => c == '+' || c == '-' || char.IsDigit(c)).ToArray()));

                string extra = "\0";
                string type = null;
                string size = "\0";

                switch (types[i][0])
                {
                    case 'l':
                        extra = " NOT null";
                        extra += " LOCALIZABLE";
                        type = "CHAR";
                        size = $"({ptr})";
                        break;

                    case 'L':
                        extra += " LOCALIZABLE";
                        type = "CHAR";
                        size = $"({ptr})";
                        break;

                    case 's':
                        extra = " NOT null";
                        type = "CHAR";
                        size = $"({ptr})";
                        break;

                    case 'S':
                        type = "CHAR";
                        size = $"({ptr})";
                        break;

                    case 'i':
                        extra = " NOT null";
                        if (len <= 2)
                        {
                            type = "INT";
                        }
                        else if (len == 4)
                        {
                            type = "LONG";
                        }
                        else
                        {
                            Console.Error.WriteLine($"Invalid int width {len}");
                            return null;
                        }

                        break;

                    case 'I':
                        if (len <= 2)
                        {
                            type = "INT";
                        }
                        else if (len == 4)
                        {
                            type = "LONG";
                        }
                        else
                        {
                            Console.Error.WriteLine($"Invalid int width {len}");
                            return null;
                        }

                        break;

                    case 'v':
                        extra = " NOT null";
                        type = "OBJECT";
                        break;

                    case 'V':
                        type = "OBJECT";
                        break;

                    default:
                        Console.Error.WriteLine($"Unknown type: {types[i][0]}");
                        return null;
                }

                string expanded = $"`{columns_data[i]}` {type}{size}{extra}{comma} ";

                sql_size += expanded.Length;
                columns += expanded;
            }

            return columns;
        }

        private string BuildCreateSQLPostlude(string[] primary_keys, int num_keys)
        {
            string keys = string.Join(", ", primary_keys.Select(s => $"`{s}`"));
            return $"PRIMARY KEY {keys})";
        }

        private LibmsiResult AddTableToDb(string[] columns, string[] types, string[] labels, int num_labels, int num_columns)
        {
            string prelude = BuildCreateSQLPrelude(labels[0]);
            string columns_sql = BuildCreateSQLColumns(columns, types, num_columns);
            string postlude = BuildCreateSQLPostlude(labels.Skip(1).ToArray(), num_labels - 1); // Skip over table name

            if (prelude == null || columns_sql == null || postlude == null)
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

            int size = prelude.Length + columns_sql.Length + postlude.Length + 1;
            string create_sql = prelude + columns_sql + postlude;

            Exception error = null; // FIXME: move error handling to caller
            LibmsiQuery view = LibmsiQuery.Create(this, create_sql, ref error);
            if (view == null)
            {
                if (error != null)
                    Console.Error.WriteLine(error.Message);

                error = null;
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;
            }

            LibmsiResult r = view.Execute(null);
            view.Close(ref error);
            if (error != null)
                Console.Error.WriteLine(error.Message);

            error = null;
            return r;
        }

        private LibmsiResult ConstructRecord(int num_columns, string[] types, string[] data, string name, out LibmsiRecord rec)
        {
            rec = LibmsiRecord.Create(num_columns);
            for (int i = 0; i < num_columns; i++)
            {
                switch (types[i][0])
                {
                    case 'L':
                    case 'l':
                    case 'S':
                    case 's':
                        rec.SetString(i + 1, data[i]);
                        break;
                    case 'I':
                    case 'i':
                        if (data[i] != null)
                            rec.SetInt(i + 1, int.Parse(new string(data[i].Skip(1).SkipWhile(c => char.IsWhiteSpace(c)).TakeWhile(c => c == '+' || c == '-' || char.IsDigit(c)).ToArray())));

                        break;
                    case 'V':
                    case 'v':
                        if (data[i] != null)
                        {
                            string file = System.IO.Path.Combine(name, data[i]);
                            LibmsiResult r = rec.LoadStreamFromFile(i + 1, file);
                            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                        }

                        break;

                    default:
                        Console.Error.WriteLine($"Unhandled column type: {types[i][0]}");
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult AddRecordsToTable(string[] columns, string[] types, string[] labels, string[][] records, int num_columns, int num_records)
        {
            LibmsiResult r = LibmsiTableView.Create(this, labels[0], out LibmsiView view);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = view.GetDimensions(out int num_rows, out int num_cols);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            while (num_rows > 0)
            {
                r = view.DeleteRow(--num_rows);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            for (int i = 0; i < num_records; i++)
            {
                r = ConstructRecord(num_columns, types, records[i], labels[0], out LibmsiRecord rec);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                r = view.InsertRow(rec, -1, false);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    return r;
                }
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult Import(string path)
        {
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;
            int num_records = 0;

            string suminfo = "_SummaryInformation";
            string forcecodepage = "_ForceCodepage";

            string data = ReadTextArchive(path, out int len);
            if (data == null)
                return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

            int ptr = 0; // data[0]
            ParseLine(ref data, ref ptr, out string[] columns, out int num_columns, ref len);
            ParseLine(ref data, ref ptr, out string[] types, out int num_types, ref len);
            ParseLine(ref data, ref ptr, out string[] labels, out int num_labels, ref len);

            if (num_columns == 1 && columns[0][0] == '\0' && num_labels == 1 && labels[0][0] == '\0' && num_types == 2 && types[1] == forcecodepage)
                return Strings.SetCodePage(int.Parse(new string(types[0].Skip(1).SkipWhile(c => char.IsWhiteSpace(c)).TakeWhile(c => c == '+' || c == '-' || char.IsDigit(c)).ToArray())));

            if (num_columns != num_types)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            List<string[]> records = new List<string[]>();

            // Read in the table records
            while (len != 0)
            {

                ParseLine(ref data, ref ptr, out string[] record, out _, ref len);
                records.Add(record);
            }

            if (labels[0] == suminfo)
            {
                r = AddSummaryInfo(this, records.ToArray(), num_records, num_columns);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }
            else
            {
                if (!TableViewExists(this, labels[0]))
                {
                    r = AddTableToDb(columns, types, labels, num_labels, num_columns);
                    if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }

                r = AddRecordsToTable(columns, types, labels, records.ToArray(), num_columns, num_records);
            }

            return r;
        }

        private static bool ExportStream(GsfInput gsfin, string table_dir, out string str, ref Exception error)
        {
            str = null;
            if (table_dir == null)
                return false;

            try
            {
                Directory.CreateDirectory(table_dir);
            }
            catch (Exception err)
            {
                error = err;
                return false;
            }

            str = gsfin.Name;
            string file = System.IO.Path.Combine(table_dir, str);

            try
            {
                Stream output = File.OpenWrite(file);
                LibmsiIStream input = LibmsiIStream.Create(gsfin);

                // TODO: Figure out how to do a "splice" on the streams
                // int spliced = g_output_stream_splice (output, input, 0, null, null);
                // return spliced != -1;
                return true;
            }
            catch (Exception err)
            {
                error = err;
                return false;
            }
        }

        private static LibmsiResult ExportRecord(Stream fd, LibmsiRecord row, int start, string table_dir, ref Exception error)
        {
            int count = row.GetFieldCount();
            for (int i = start; i <= count; i++)
            {
                string str;
                row.GetGsfInput(i, out GsfInput input);
                if (input != null)
                {
                    if (!ExportStream(input, table_dir, out str, ref error))
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

                    input = null;
                }
                else
                {
                    str = row.GetString(i);
                    if (str == null)
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }

                try
                {
                    // TODO full_write 
                    fd.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);

                    string sep = (i < count) ? "\t" : "\r\n";
                    fd.Write(Encoding.ASCII.GetBytes(sep), 0, sep.Length);
                }
                catch
                {
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                }
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Move to LibmsiRecord
        private static LibmsiResult ExportRowImpl(LibmsiRecord row, object arg)
        {
            ExportRow export = arg as ExportRow;
            Exception err = export.Error;
            LibmsiResult r = ExportRecord(export.FD, row, 1, export.TableDir, ref err);
            export.Error = err;
            return r;
        }

        private static LibmsiResult ExportForceCodePage(Stream fd, int codepage)
        {
            string data = $"\r\n\r\n{codepage}\t_ForceCodepage\r\n\0";
            int sz = data.Length;
            try
            {
                fd.Write(Encoding.ASCII.GetBytes(data), 0, sz);
                return LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }
            catch
            {
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }
        }

        private LibmsiResult ExportSummaryInfo(Stream fd, ref Exception error)
        {
            LibmsiSummaryInfo si = LibmsiSummaryInfo.Create(this, 0, ref error);
            if (si == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            string header = "PropertyId\tValue\r\ni2\tl255\r\n_SummaryInformation\tPropertyId\r\n";
            int sz = header.Length;

            try
            {
                fd.Write(Encoding.ASCII.GetBytes(header), 0, sz);
            }
            catch
            {
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            for (int i = 0; i < MSI_MAX_PROPS; i++)
            {
                if (si.Property[i].VariantType != LibmsiOLEVariantType.OLEVT_EMPTY)
                {
                    string val = si.SummaryInfoAsString(i);
                    if (val == null)
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

                    string str = $"{i}\t{val}\r\n";
                    sz = str.Length;
                    try
                    {
                        fd.Write(Encoding.ASCII.GetBytes(str), 0, sz);
                    }
                    catch
                    {
                        return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
                    }
                }
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult ExportImpl(string table, Stream fd, ref Exception error)
        {
            if (table == "_ForceCodepage")
            {
                int codepage = Strings.GetCodePage();
                return ExportForceCodePage(fd, codepage);
            }
            else if (table == "_SummaryInformation")
            {
                return ExportSummaryInfo(fd, ref error);
            }

            string query = $"select * from {table}";
            LibmsiResult r = QueryOpen(this, out LibmsiQuery view, query);
            if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                // Write out row 1, the column names
                r = view.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_NAMES, out LibmsiRecord rec);
                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    ExportRecord(fd, rec, 1, null, ref error);

                // Write out row 2, the column types
                r = view.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_TYPES, out rec);
                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    ExportRecord(fd, rec, 1, null, ref error);

                // Write out row 3, the table name + keys
                r = GetPrimaryKeys(table, out rec);
                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS)
                {
                    rec.SetString(0, table);
                    ExportRecord(fd, rec, 0, null, ref error);
                }

                // Write out row 4 onwards, the data
                ExportRow export = new ExportRow
                {
                    FD = fd,
                    TableDir = table,
                    Error = error,
                };

                int count = 0;
                r = view.IterateRecords(ref count, ExportRowImpl, export);
            }

            return r;
        }

        private static bool MergeTypeMatch(string type1, string type2)
        {
            if (((type1[0] == 'l') || (type1[0] == 's')) &&
                ((type2[0] == 'l') || (type2[0] == 's')))
                return true;

            if (((type1[0] == 'L') || (type1[0] == 'S')) &&
                ((type2[0] == 'L') || (type2[0] == 'S')))
                return true;

            return type1 == type2;
        }

        // TODO: Move to LibmsiQuery
        private static LibmsiResult MergeVerifyColnames(LibmsiQuery dbview, LibmsiQuery mergeview)
        {
            LibmsiResult r = dbview.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_NAMES, out LibmsiRecord dbrec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = mergeview.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_NAMES, out LibmsiRecord mergerec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            int count = dbrec.GetFieldCount();
            for (int i = 1; i <= count; i++)
            {
                if (mergerec.GetStringRaw(i) == null)
                    break;

                if (dbrec.GetStringRaw(i) != mergerec.GetStringRaw(i))
                    return LibmsiResult.LIBMSI_RESULT_DATATYPE_MISMATCH;
            }

            r = dbview.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_TYPES, out dbrec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = mergeview.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_TYPES, out mergerec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            count = dbrec.GetFieldCount();
            for (int i = 1; i <= count; i++)
            {
                if (mergerec.GetStringRaw(i) == null)
                    break;

                if (!MergeTypeMatch(dbrec.GetStringRaw(i), mergerec.GetStringRaw(i)))
                {
                    r = LibmsiResult.LIBMSI_RESULT_DATATYPE_MISMATCH;
                    break;
                }
            }

            return r;
        }

        private LibmsiResult MergeVerifyPrimaryKeys(LibmsiDatabase mergedb, string table)
        {
            LibmsiResult r = GetPrimaryKeys(table, out LibmsiRecord dbrec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = mergedb.GetPrimaryKeys(table, out LibmsiRecord mergerec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            int count = dbrec.GetFieldCount();
            if (count != mergerec.GetFieldCount())
                return LibmsiResult.LIBMSI_RESULT_DATATYPE_MISMATCH;

            for (int i = 1; i <= count; i++)
            {
                if (dbrec.GetStringRaw(i) != mergerec.GetStringRaw(i))
                    return LibmsiResult.LIBMSI_RESULT_DATATYPE_MISMATCH;
            }

            return r;
        }

        // TODO: Move to LibmsiRecord
        private static string GetKeyValue(LibmsiQuery view, string key, LibmsiRecord rec)
        {
            LibmsiResult r = view.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_NAMES, out LibmsiRecord colnames);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return null;

            int i = 0;
            bool cmp;
            do
            {
                string temp = colnames.DupRecordField(++i);
                cmp = (key == temp);
            } while (!cmp);

            // Check record field is a string
            string str;

            // Quote string record fields
            if (rec.GetStringRaw(i) != null)
                str = $"{rec.GetStringRaw(i)}";
            else
                str = rec.GetString(i);

            return str;
        }

        private string CreateDiffRowQuery(LibmsiQuery view, string table, LibmsiRecord rec)
        {
            LibmsiResult r = GetPrimaryKeys(table, out LibmsiRecord keys);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return null;

            string query = $"SELECT * FROM {table} WHERE ";
            int count = keys.GetFieldCount();
            for (int i = 1; i <= count; i++)
            {
                string key = keys.GetStringRaw(i);
                string val = GetKeyValue(view, key, rec);

                if (i == count)
                    query += $"`{key}` = {val}";
                else
                    query += $"`{key}` = {val} AND ";
            }

            return query;
        }

        // TODO: Move to LibmsiRecord
        private static LibmsiResult MergeDiffRow(LibmsiRecord rec, object param)
        {
            MERGEDATA data = param as MERGEDATA;
            MERGETABLE table = data.CurTable;
            MERGEROW mergerow;
            LibmsiResult r = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            Exception err = null;

            if (TableViewExists(data.Database, table.Name))
            {
                string query = data.Merge.CreateDiffRowQuery(data.CurView, table.Name, rec);
                if (query == null)
                    return LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;

                LibmsiQuery dbview = LibmsiQuery.Create(data.Database, query, ref err);
                if (err != null)
                {
                    Console.Error.WriteLine(err.Message);
                    return r;
                }

                r = dbview.Execute(null);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                r = dbview.Fetch(out LibmsiRecord row);
                if (r == LibmsiResult.LIBMSI_RESULT_SUCCESS && !RecordCompare(rec, row))
                {
                    table.NumConflicts++;
                    return r;
                }
                else if (r != LibmsiResult.NO_MORE_ITEMS)
                {
                    return r;
                }

                r = LibmsiResult.LIBMSI_RESULT_SUCCESS;
            }

            mergerow = new MERGEROW();
            mergerow.Data = rec.Clone();
            if (mergerow.Data == null)
            {
                r = LibmsiResult.LIBMSI_RESULT_OUTOFMEMORY;
                return r;
            }

            table.Rows.AddFirst(mergerow);
            return r;
        }

        private LibmsiResult GetTableLabels(string table, out string[] labels, out int numlabels)
        {
            labels = null; numlabels = 0;
            LibmsiResult r = GetPrimaryKeys(table, out LibmsiRecord prec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            int count = prec.GetFieldCount();
            numlabels = count + 1;
            labels = new string[numlabels];

            labels[0] = table;
            for (int i = 1; i <= count; i++)
            {
                labels[i] = prec.GetStringRaw(i);
            }

            return r;
        }

        // TODO: Move to LibmsiRecord
        private static LibmsiResult GetQueryColumns(LibmsiQuery query, out string[] columns, out int numcolumns)
        {
            columns = null; numcolumns = 0;
            LibmsiResult r = query.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_NAMES, out LibmsiRecord prec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            int count = prec.GetFieldCount();
            columns = new string[count];

            for (int i = 1; i <= count; i++)
            {
                columns[i - 1] = prec.GetStringRaw(i);
            }

            numcolumns = count;
            return r;
        }

        // TODO: Move to LibmsiRecord
        private static LibmsiResult GetQueryTypes(LibmsiQuery query, out string[] types, out int numtypes)
        {
            types = null; numtypes = 0;
            LibmsiResult r = query.GetColumnInfo(LibmsiColInfo.LIBMSI_COL_INFO_TYPES, out LibmsiRecord prec);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            int count = prec.GetFieldCount();
            types = new string[count];
            numtypes = count;
            for (int i = 1; i <= count; i++)
            {
                types[i - 1] = prec.GetStringRaw(i);
            }

            return r;
        }

        // TODO: Move to MERGETABLE
        private static void FreeMergeTable(MERGETABLE table)
        {
            table.Labels = null;
            table.Columns = null;
            table.Types = null;
            table.Name = null;
            table.Rows.Clear();
        }

        private LibmsiResult GetMergeTable(string name, out MERGETABLE ptable)
        {
            MERGETABLE table = new MERGETABLE();
            LibmsiResult r = GetTableLabels(name, out string[] labels, out int num_labels);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto err;

            table.Labels = labels;
            table.NumLabels = num_labels;

            string query = $"SELECT * FROM {name}";
            r = QueryOpen(this, out LibmsiQuery mergeview, query);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto err;

            r = GetQueryColumns(mergeview, out string[] columns, out int num_columns);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto err;

            table.Columns = columns;
            table.NumColumns = num_columns;

            r = GetQueryTypes(mergeview, out string[] types, out int num_types);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                goto err;

            table.Types = types;
            table.NumTypes = num_types;

            table.Rows = new LinkedList<MERGEROW>();

            table.Name = name;
            table.NumConflicts = 0;

            ptable = table;
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;

        err:
            FreeMergeTable(table);
            ptable = null;
            return r;
        }

        // TODO: Move to LibmsiRecord
        private static LibmsiResult MergeDiffTables(LibmsiRecord rec, object param)
        {
            MERGEDATA data = param as MERGEDATA;
            string name = rec.GetStringRaw(1);

            string query = $"SELECT * FROM {name}";
            LibmsiResult r = QueryOpen(data.Merge, out LibmsiQuery mergeview, query);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            if (TableViewExists(data.Database, name))
            {
                r = QueryOpen(data.Database, out LibmsiQuery dbview, query);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                r = MergeVerifyColnames(dbview, mergeview);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                r = data.Database.MergeVerifyPrimaryKeys(data.Merge, name);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            r = data.Merge.GetMergeTable(name, out MERGETABLE table);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            data.CurTable = table;
            data.CurView = mergeview;

            int count = 0;
            r = mergeview.IterateRecords(ref count, MergeDiffRow, data);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
            {
                FreeMergeTable(table);
                return r;
            }

            data.TableData.AddFirst(table);
            return r;
        }

        private LibmsiResult GatherMergeData(LibmsiDatabase merge, MERGETABLE tabledata)
        {
            string query = "SELECT * FROM _Tables";
            LibmsiResult r = DatabaseOpenQuery(merge, query, out LibmsiQuery view);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            MERGEDATA data = new MERGEDATA
            {
                Database = this,
                Merge = merge,
                TableData = new LinkedList<MERGETABLE>()
            };

            data.TableData.AddFirst(tabledata);

            int count = 0;
            r = view.IterateRecords(ref count, MergeDiffTables, data);
            return r;
        }

        private LibmsiResult MergeTable(MERGETABLE table)
        {
            LibmsiResult r;
            if (!TableViewExists(this, table.Name))
            {
                r = AddTableToDb(table.Columns, table.Types, table.Labels, table.NumLabels, table.NumColumns);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            foreach (MERGEROW row in table.Rows)
            {
                r = LibmsiTableView.Create(this, table.Name, out LibmsiView tv);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                r = tv.InsertRow(row.Data, -1, false);
                tv.Delete();

                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private LibmsiResult UpdateMergeErrors(string error, string table, int numconflicts)
        {
            LibmsiResult r;
            if (!TableViewExists(this, error))
            {
                string create = $"CREATE TABLE `{error}` (`Table` CHAR(255) NOT NULL, `NumRowMergeConflicts` SHORT NOT NULL PRIMARY KEY `Table`)";
                r = QueryOpen(this, out LibmsiQuery createView, create);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;

                r = createView.Execute(null);
                if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return r;
            }

            string insert = $"INSERT INTO `{error}` (`Table`, `NumRowMergeConflicts`) VALUES ('{table}', {numconflicts})";
            r = QueryOpen(this, out LibmsiQuery view, insert);
            if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                return r;

            r = view.Execute(null);
            return r;
        }

        private void CacheInfileStructure()
        {
            LibmsiResult r;

            int n = Infile.NumChildren();

            // TODO: error handling
            for (int i = 0; i < n; i++)
            {
                Exception err = null;
                GsfInput input = Infile.ChildByIndex(i, ref err);
                string name = input.Name;
                byte[] name8 = Encoding.ASCII.GetBytes(name);

                if (name == null)
                {
                    Console.Error.WriteLine("Name was null");
                    continue;
                }

                // Table streams are not in the _Streams table
                if (!(input is GsfInfile) || (input as GsfInfile).NumChildren() == -1)
                {
                    // UTF-8 encoding of 0x4840.
                    if (name8[0] == 0xe4 && name8[1] == 0xa1 && name8[2] == 0x80)
                    {
                        string decname = DecodeStreamName(name + 3);
                        if (decname == szStringPool || decname == szStringData)
                            continue;

                        r = OpenTable(this, decname, false);
                        if (r != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                            Console.Error.WriteLine($"Error opening table {decname}: {r}");
                    }
                    else
                    {
                        r = MsiAllocStream(name, input);
                    }
                }
                else
                {
                    OpenStorage(name);
                }
            }
        }

        private static bool GsfInfileCopy(GsfInfile inf, GsfOutfile outf)
        {
            int n = inf.NumChildren();
            for (int i = 0; i < n; i++)
            {
                Exception err = null;
                string name = inf.NameByIndex(i);
                GsfInput child = inf.ChildByName(name, ref err);
                GsfInfile childf = (child is GsfInfile) ? (child as GsfInfile) : null;
                bool is_dir = childf != null && childf.NumChildren() > 0;
                GsfOutput dest = outf.NewChild(name, is_dir);

                bool ok;
                if (is_dir)
                    ok = GsfInfileCopy(childf, dest as GsfOutfile);
                else
                    ok = child.Copy(dest);

                if (!ok)
                    return false;
            }

            return true;
        }

        // TODO: Can we avoid the use of `opaque`?
        private static LibmsiResult CommitStorage(string name, GsfInfile stg, object opaque)
        {
            LibmsiDatabase db = opaque as LibmsiDatabase;
            GsfOutfile outstg = (db.Outfile.NewChild(name, true) as GsfOutfile);
            if (outstg == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            if (!GsfInfileCopy(stg, outstg))
            {
                outstg.Close();
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            outstg.Close();
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Can we avoid the use of `opaque`?
        private static LibmsiResult CommitStream(string name, GsfInput stm, object opaque)
        {
            LibmsiDatabase db = opaque as LibmsiDatabase;
            string decname = DecodeStreamName(name);
            GsfOutput outstm = db.Outfile.NewChild(name, false);
            if (outstm == null)
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;

            stm.Seek(0, SeekOrigin.Begin);
            outstm.Seek(0, SeekOrigin.Begin);
            if (!stm.Copy(outstm))
            {
                outstm.Close();
                return LibmsiResult.LIBMSI_RESULT_FUNCTION_FAILED;
            }

            outstm.Close();
            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        // TODO: Can we avoid the use of `param`?
        private static LibmsiResult PrimaryKeyIterator(LibmsiRecord rec, object param)
        {
            MsiPrimaryKeyRecordInfo info = param as MsiPrimaryKeyRecordInfo;
            int type = rec.GetInt(4);
            if ((type & MSITYPE_KEY) != 0)
            {
                info.N++;
                if (info.Rec != null)
                {
                    if (info.N == 1)
                    {
                        string table = rec.GetStringRaw(1);
                        info.Rec.SetString(0, table);
                    }

                    string name = rec.GetStringRaw(3);
                    info.Rec.SetString(info.N, name);
                }
            }

            return LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        private bool Init(ref Exception err)
        {
            if (Flags.HasFlag(LibmsiDbFlags.LIBMSI_DB_FLAGS_CREATE))
            {
                Strings = InitStringTable(out int bytes_per_strref);
                BytesPerStrref = bytes_per_strref;
            }
            else
            {
                if (Open() != LibmsiResult.LIBMSI_RESULT_SUCCESS)
                    return false;
            }

            MediaTransformOffset = MSI_INITIAL_MEDIA_TRANSFORM_OFFSET;
            MediaTransformDiskId = MSI_INITIAL_MEDIA_TRANSFORM_DISKID;

            bool TRACE_ON = false; // TODO: Make configurable
            if (TRACE_ON && Infile != null)
                EnumStreamNames(Infile);

            LibmsiResult ret = StartTransaction();
            return ret == LibmsiResult.LIBMSI_RESULT_SUCCESS;
        }

        #endregion
    }
}