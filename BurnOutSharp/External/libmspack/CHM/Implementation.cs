/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;
using System.IO;
using System.Linq;
using System.Text;
using LibMSPackSharp.Compression;

namespace LibMSPackSharp.CHM
{
    public class Implementation
    {
        #region Generic CHM Definitions

        #region Structure Offsets

        private const int chmhead_Signature = 0x0000;
        private const int chmhead_Version = 0x0004;
        private const int chmhead_HeaderLen = 0x0008;
        private const int chmhead_Unknown1 = 0x000C;
        private const int chmhead_Timestamp = 0x0010;
        private const int chmhead_LanguageID = 0x0014;
        private const int chmhead_GUID1 = 0x0018;
        private const int chmhead_GUID2 = 0x0028;
        private const int chmhead_SIZEOF = 0x0038;

        private const int chmhst_OffsetHS0 = 0x0000;
        private const int chmhst_LengthHS0 = 0x0008;
        private const int chmhst_OffsetHS1 = 0x0010;
        private const int chmhst_LengthHS1 = 0x0018;
        private const int chmhst_SIZEOF = 0x0020;
        private const int chmhst3_OffsetCS0 = 0x0020;
        private const int chmhst3_SIZEOF = 0x0028;

        private const int chmhs0_Unknown1 = 0x0000;
        private const int chmhs0_Unknown2 = 0x0004;
        private const int chmhs0_FileLen = 0x0008;
        private const int chmhs0_Unknown3 = 0x0010;
        private const int chmhs0_Unknown4 = 0x0014;
        private const int chmhs0_SIZEOF = 0x0018;

        private const int chmhs1_Signature = 0x0000;
        private const int chmhs1_Version = 0x0004;
        private const int chmhs1_HeaderLen = 0x0008;
        private const int chmhs1_Unknown1 = 0x000C;
        private const int chmhs1_ChunkSize = 0x0010;
        private const int chmhs1_Density = 0x0014;
        private const int chmhs1_Depth = 0x0018;
        private const int chmhs1_IndexRoot = 0x001C;
        private const int chmhs1_FirstPMGL = 0x0020;
        private const int chmhs1_LastPMGL = 0x0024;
        private const int chmhs1_Unknown2 = 0x0028;
        private const int chmhs1_NumChunks = 0x002C;
        private const int chmhs1_LanguageID = 0x0030;
        private const int chmhs1_GUID = 0x0034;
        private const int chmhs1_Unknown3 = 0x0044;
        private const int chmhs1_Unknown4 = 0x0048;
        private const int chmhs1_Unknown5 = 0x004C;
        private const int chmhs1_Unknown6 = 0x0050;
        private const int chmhs1_SIZEOF = 0x0054;

        private const int pmgl_Signature = 0x0000;
        private const int pmgl_QuickRefSize = 0x0004;
        private const int pmgl_Unknown1 = 0x0008;
        private const int pmgl_PrevChunk = 0x000C;
        private const int pmgl_NextChunk = 0x0010;
        private const int pmgl_Entries = 0x0014;
        private const int pmgl_headerSIZEOF = 0x0014;

        private const int pmgi_Signature = 0x0000;
        private const int pmgi_QuickRefSize = 0x0004;
        private const int pmgi_Entries = 0x0008;
        private const int pmgi_headerSIZEOF = 0x000C;

        private const int lzxcd_Length = 0x0000;
        private const int lzxcd_Signature = 0x0004;
        private const int lzxcd_Version = 0x0008;
        private const int lzxcd_ResetInterval = 0x000C;
        private const int lzxcd_WindowSize = 0x0010;
        private const int lzxcd_CacheSize = 0x0014;
        private const int lzxcd_Unknown1 = 0x0018;
        private const int lzxcd_SIZEOF = 0x001C;

        private const int lzxrt_Unknown1 = 0x0000;
        private const int lzxrt_NumEntries = 0x0004;
        private const int lzxrt_EntrySize = 0x0008;
        private const int lzxrt_TableOffset = 0x000C;
        private const int lzxrt_UncompLen = 0x0010;
        private const int lzxrt_CompLen = 0x0018;
        private const int lzxrt_FrameLen = 0x0020;
        private const int lzxrt_Entries = 0x0028;
        private const int lzxrt_headerSIZEOF = 0x0028;

        #endregion

        // filenames of the system files used for decompression.
        // Content and ControlData are essential.
        // ResetTable is preferred, but SpanInfo can be used if not available
        public const string ContentName = "::DataSpace/Storage/MSCompressed/Content";
        public const string ControlName = "::DataSpace/Storage/MSCompressed/ControlData";
        public const string SpanInfoName = "::DataSpace/Storage/MSCompressed/SpanInfo";
        public const string ResetTableName = "::DataSpace/Storage/MSCompressed/Transform/{7FC28940-9D31-11D0-9B27-00A0C91E9C7C}/InstanceData/ResetTable";

        #endregion

        #region CHMD_OPEN

        /// <summary>
        /// Opens a file and tries to read it as a CHM file.
        /// Calls RealOpen() with entire=1.
        /// </summary>
        public static Header Open(Decompressor decompressor, string filename)
        {
            return RealOpen(decompressor, filename, true);
        }

        #endregion

        #region CHMD_FAST_OPEN

        /// <summary>
        /// Opens a file and tries to read it as a CHM file, but does not read
        /// the file headers. Calls chmd_real_open() with entire=0
        /// </summary>
        public static Header FastOpen(Decompressor decompressor, string filename)
        {
            return RealOpen(decompressor, filename, false);
        }

        #endregion

        #region CHMD_REAL_OPEN

        /// <summary>
        /// The real implementation of chmd_open() and chmd_fast_open(). It simply
        /// passes the "entire" parameter to chmd_read_headers(), which will then
        /// either read all headers, or a bare mininum.
        /// </summary>
        private static Header RealOpen(Decompressor d, string filename, bool entire)
        {
            DecompressorImpl self = d as DecompressorImpl;
            Header chm = null;

            if (d == null)
                return null;

            SystemImpl sys = self.System;

            FileStream fh;
            if ((fh = sys.Open(filename, OpenMode.MSPACK_SYS_OPEN_READ)) != null)
            {
                chm = new Header();
                chm.Filename = filename;
                Error error = ReadHeaders(sys, fh, chm, entire);
                if (error != Error.MSPACK_ERR_OK)
                {
                    // If the error is DATAFORMAT, and there are some results, return
                    // partial results with a warning, rather than nothing
                    if (error == Error.MSPACK_ERR_DATAFORMAT && (chm.Files != null || chm.SysFiles != null))
                    {
                        sys.Message(fh, "WARNING; contents are corrupt");
                        error = Error.MSPACK_ERR_OK;
                    }
                    else
                    {
                        Close(d, chm);
                        chm = null;
                    }
                }

                self.Error = error;
                sys.Close(fh);
            }
            else
            {
                self.Error = Error.MSPACK_ERR_OPEN;
            }

            return chm;
        }

        #endregion

        #region CHMD_CLOSE

        /// <summary>
        /// Frees all memory associated with a given mschmd_header
        /// </summary>
        public static void Close(Decompressor d, Header chm)
        {
            DecompressorImpl self = d as DecompressorImpl;
            DecompressFile fi, nfi;
            uint i;

            if (d == null)
                return;

            SystemImpl sys = self.System;

            self.Error = Error.MSPACK_ERR_OK;

            // Free files
            for (fi = chm.Files; fi != null; fi = nfi)
            {
                nfi = fi.Next;
            }

            for (fi = chm.SysFiles; fi != null; fi = nfi)
            {
                nfi = fi.Next;
            }

            // If this CHM was being decompressed, free decompression state
            if (self.State != null && (self.State.Header == chm))
            {
                sys.Close(self.State.InputFileHandle);
                sys.Close(self.State.OutputFileHandle);

                self.State = null;
            }
        }

        #endregion

        #region CHMD_READ_HEADERS

        /// <summary>
        /// The GUIDs found in CHM headers
        /// </summary>
        private static readonly byte[] guids =
        {
            /* {7C01FD10-7BAA-11D0-9E0C-00A0-C922-E6EC} */
            0x10, 0xFD, 0x01, 0x7C, 0xAA, 0x7B, 0xD0, 0x11,
            0x9E, 0x0C, 0x00, 0xA0, 0xC9, 0x22, 0xE6, 0xEC,

            /* {7C01FD11-7BAA-11D0-9E0C-00A0-C922-E6EC} */
            0x11, 0xFD, 0x01, 0x7C, 0xAA, 0x7B, 0xD0, 0x11,
            0x9E, 0x0C, 0x00, 0xA0, 0xC9, 0x22, 0xE6, 0xEC
        };

        /// <summary>
        /// Reads the basic CHM file headers. If the "entire" parameter is
        /// non-zero, all file entries will also be read. fills out a pre-existing
        /// mschmd_header structure, allocates memory for files as necessary
        /// </summary>
        public static Error ReadHeaders(SystemImpl sys, FileStream fh, Header chm, bool entire)
        {
            uint section, nameLen, x, errors, numChunks;
            byte[] buf = new byte[0x54];
            int name, p, end;
            DecompressFile fi, link = null;
            long offset, length;
            int numEntries;

            // Initialise pointers
            chm.Files = null;
            chm.SysFiles = null;
            chm.ChunkCache = null;

            chm.Sec0.Header = chm;
            chm.Sec0.ID = 0;

            chm.Sec1.Header = chm;
            chm.Sec1.ID = 1;
            chm.Sec1.Content = null;
            chm.Sec1.Control = null;
            chm.Sec1.SpanInfo = null;
            chm.Sec1.ResetTable = null;

            // Read the first header
            if (sys.Read(fh, buf, 0, chmhead_SIZEOF) != chmhead_SIZEOF)
                return Error.MSPACK_ERR_READ;

            // Check ITSF signature
            if (BitConverter.ToUInt32(buf, chmhead_Signature) != 0x46535449)
                return Error.MSPACK_ERR_SIGNATURE;

            // Check both header GUIDs
            if (!buf.Skip(chmhead_GUID1).Take(32).SequenceEqual(guids))
            {
                Console.WriteLine("incorrect GUIDs");
                return Error.MSPACK_ERR_SIGNATURE;
            }

            chm.Version = BitConverter.ToUInt32(buf, chmhead_Version);
            chm.Timestamp = BitConverter.ToUInt32(buf, chmhead_Timestamp);
            chm.Language = BitConverter.ToUInt32(buf, chmhead_LanguageID);
            if (chm.Version > 3)
                sys.Message(fh, "WARNING; CHM version > 3");

            // Read the header section table
            if (sys.Read(fh, buf, 0, chmhst3_SIZEOF) != chmhst3_SIZEOF)
                return Error.MSPACK_ERR_READ;

            // chmhst3_OffsetCS0 does not exist in version 1 or 2 CHM files.
            // The offset will be corrected later, once HS1 is read.
            if ((offset = BitConverter.ToInt64(buf, chmhst_OffsetHS0)) != 0
                || (chm.DirOffset = BitConverter.ToInt64(buf, chmhst_OffsetHS1)) != 0
                || (chm.Sec0.Offset = BitConverter.ToInt64(buf, chmhst3_OffsetCS0)) != 0)
            {
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // Seek to header section 0
            if (!sys.Seek(fh, offset, SeekMode.MSPACK_SYS_SEEK_START))
                return Error.MSPACK_ERR_SEEK;

            // Read header section 0
            if (sys.Read(fh, buf, 0, chmhs0_SIZEOF) != chmhs0_SIZEOF)
                return Error.MSPACK_ERR_READ;

            if ((chm.Length = BitConverter.ToInt64(buf, chmhs0_FileLen)) != 0)
                return Error.MSPACK_ERR_DATAFORMAT;

            // Seek to header section 1
            if (!sys.Seek(fh, chm.DirOffset, SeekMode.MSPACK_SYS_SEEK_START))
                return Error.MSPACK_ERR_SEEK;

            // Read header section 1
            if (sys.Read(fh, buf, 0, chmhs1_SIZEOF) != chmhs1_SIZEOF)
                return Error.MSPACK_ERR_READ;

            chm.DirOffset = sys.Tell(fh);
            chm.ChunkSize = BitConverter.ToUInt32(buf, chmhs1_ChunkSize);
            chm.Density = BitConverter.ToUInt32(buf, chmhs1_Density);
            chm.Depth = BitConverter.ToUInt32(buf, chmhs1_Depth);
            chm.IndexRoot = BitConverter.ToUInt32(buf, chmhs1_IndexRoot);
            chm.NumChunks = BitConverter.ToUInt32(buf, chmhs1_NumChunks);
            chm.FirstPMGL = BitConverter.ToUInt32(buf, chmhs1_FirstPMGL);
            chm.LastPMGL = BitConverter.ToUInt32(buf, chmhs1_LastPMGL);

            if (chm.Version < 3)
            {
                // Versions before 3 don't have chmhst3_OffsetCS0
                chm.Sec0.Offset = chm.DirOffset + (chm.ChunkSize * chm.NumChunks);
            }

            // Check if content offset or file size is wrong
            if (chm.Sec0.Offset > chm.Length)
            {
                Console.WriteLine("content section begins after file has ended");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // Ensure there are chunks and that chunk size is
            // large enough for signature and num_entries
            if (chm.ChunkSize < (pmgl_Entries + 2))
            {
                Console.WriteLine("chunk size not large enough");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            if (chm.NumChunks == 0)
            {
                Console.WriteLine("no chunks");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // The ChunkCache data structure is not great; large values for NumChunks
            // or NumChunks*ChunkSize can exhaust all memory. Until a better chunk
            // cache is implemented, put arbitrary limits on NumChunks and chunk size.
            if (chm.NumChunks > 100000)
            {
                Console.WriteLine("more than 100,000 chunks");
                return Error.MSPACK_ERR_DATAFORMAT;
            }
            if (chm.ChunkSize > 8192)
            {
                Console.WriteLine("chunk size over 8192 (get in touch if this is valid)");
                return Error.MSPACK_ERR_DATAFORMAT;
            }
            if (chm.ChunkSize * (long)chm.NumChunks > chm.Length)
            {
                Console.WriteLine("chunks larger than entire file");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // Common sense checks on header section 1 fields
            if (chm.ChunkSize != 4096)
                sys.Message(fh, "WARNING; chunk size is not 4096");

            if (chm.FirstPMGL != 0)
                sys.Message(fh, "WARNING; first PMGL chunk is not zero");

            if (chm.FirstPMGL > chm.LastPMGL)
            {
                Console.WriteLine("first pmgl chunk is after last pmgl chunk");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            if (chm.IndexRoot != 0xFFFFFFFF && chm.IndexRoot >= chm.NumChunks)
            {
                Console.WriteLine("IndexRoot outside valid range");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // If we are doing a quick read, stop here!
            if (!entire)
                return Error.MSPACK_ERR_OK;

            // Seek to the first PMGL chunk, and reduce the number of chunks to read
            if ((x = chm.FirstPMGL) != 0)
            {
                if (!sys.Seek(fh, x * chm.ChunkSize, SeekMode.MSPACK_SYS_SEEK_CUR))
                    return Error.MSPACK_ERR_SEEK;
            }

            numChunks = chm.LastPMGL - x + 1;

            byte[] chunk = new byte[chm.ChunkSize];

            // Read and process all chunks from FirstPMGL to LastPMGL
            errors = 0;
            while (numChunks-- != 0)
            {
                // Read next chunk
                if (sys.Read(fh, chunk, 0, (int)chm.ChunkSize) != (int)chm.ChunkSize)
                    return Error.MSPACK_ERR_READ;

                // Process only directory (PMGL) chunks
                if (BitConverter.ToUInt32(chunk, pmgl_Signature) != 0x4C474D50)
                    continue;

                if (BitConverter.ToUInt32(chunk, pmgl_QuickRefSize) < 2)
                    sys.Message(fh, "WARNING; PMGL quickref area is too small");

                if (BitConverter.ToUInt32(chunk, pmgl_QuickRefSize) > chm.ChunkSize - pmgl_Entries)
                    sys.Message(fh, "WARNING; PMGL quickref area is too large");

                p = pmgl_Entries;
                end = (int)(chm.ChunkSize - 2);
                numEntries = BitConverter.ToUInt16(chunk, end);

                while (numEntries-- != 0)
                {
                    // READ_ENCINT(nameLen)
                    nameLen = 0;
                    do
                    {
                        if (p >= end)
                            goto chunk_end;

                        nameLen = (uint)((nameLen << 7) | (chunk[p] & 0x7F));
                    } while ((chunk[p++] & 0x80) != 0);

                    if (nameLen > (uint)(end - p))
                        goto chunk_end;

                    name = p; p += (int)nameLen;

                    // READ_ENCINT(section)
                    section = 0;
                    do
                    {
                        if (p >= end)
                            goto chunk_end;

                        section = (uint)((section << 7) | (chunk[p] & 0x7F));
                    } while ((chunk[p++] & 0x80) != 0);

                    // READ_ENCINT(offset)
                    offset = 0;
                    do
                    {
                        if (p >= end)
                            goto chunk_end;

                        offset = (offset << 7) | (chunk[p] & 0x7F);
                    } while ((chunk[p++] & 0x80) != 0);

                    // READ_ENCINT(length)
                    length = 0;
                    do
                    {
                        if (p >= end)
                            goto chunk_end;

                        length = (length << 7) | (chunk[p] & 0x7F);
                    } while ((chunk[p++] & 0x80) != 0);

                    // Ignore blank or one-char (e.g. "/") filenames we'd return as blank
                    if (nameLen < 2 || chunk[name + 0] == 0x00 || chunk[name + 1] == 0x00)
                        continue;

                    // Empty files and directory names are stored as a file entry at
                    // offset 0 with length 0. We want to keep empty files, but not
                    // directory names, which end with a "/"
                    if ((offset == 0) && (length == 0))
                    {
                        if ((nameLen > 0) && (chunk[name + nameLen - 1] == '/'))
                            continue;
                    }

                    if (section > 1)
                    {
                        sys.Message(fh, $"invalid section number '{section}'.");
                        continue;
                    }

                    fi = new DecompressFile();

                    fi.Next = null;
                    fi.Filename = Encoding.UTF8.GetString(chunk, name, (int)nameLen) + "\0";
                    fi.Section = (section == 0) ? chm.Sec0 as Section : chm.Sec1 as Section;
                    fi.Offset = offset;
                    fi.Length = length;

                    if (chunk[name + 0] == ':' && chunk[name + 1] == ':')
                    {
                        // System file
                        if (nameLen == 40 && fi.Filename.Trim().Equals(ContentName))
                            chm.Sec1.Content = fi;

                        else if (nameLen == 44 && fi.Filename.Trim().Equals(ControlName))
                            chm.Sec1.Control = fi;

                        else if (nameLen == 41 && fi.Filename.Trim().Equals(SpanInfoName))
                            chm.Sec1.SpanInfo = fi;

                        else if (nameLen == 105 && fi.Filename.Trim().Equals(ResetTableName))
                            chm.Sec1.ResetTable = fi;

                        fi.Next = chm.SysFiles;
                        chm.SysFiles = fi;
                    }
                    else
                    {
                        // Normal file
                        if (link != null)
                            link.Next = fi;
                        else
                            chm.Files = fi;

                        link = fi;
                    }
                }

            // This is reached either when num_entries runs out, or if
            // reading data from the chunk reached a premature end of chunk
            chunk_end:
                if (numEntries >= 0)
                {
                    Console.WriteLine("chunk ended before all entries could be read");
                    errors++;
                }
            }

            return (errors > 0) ? Error.MSPACK_ERR_DATAFORMAT : Error.MSPACK_ERR_OK;
        }

        #endregion

        #region CHMD_FAST_FIND

        /// <summary>
        /// uses PMGI index chunks and quickref data to quickly locate a file
        /// directly from the on-disk index.
        /// 
        /// TODO: protect against infinite loops in chunks (where pgml_NextChunk
        /// or a PMGI index entry point to an already visited chunk)
        /// </summary>
        public static Error FastFind(Decompressor d, Header chm, string filename, DecompressFile f_ptr)
        {
            DecompressorImpl self = d as DecompressorImpl;
            SystemImpl sys;
            FileStream fh;

            // p and end are initialised to prevent MSVC warning about "potentially"
            // uninitialised usage. This is provably untrue, but MS won't fix:
            // https://developercommunity.visualstudio.com/content/problem/363489/c4701-false-positive-warning.html
            byte[] chunk = new byte[0];
            int p = -1, end = -1, result = -1;
            Error err = Error.MSPACK_ERR_OK;
            uint n, sec;

            if (self == null || chm == null || f_ptr == null)
                return Error.MSPACK_ERR_ARGS;

            sys = self.System;

            // Clear the results structure
            f_ptr = new DecompressFile();

            if ((fh = sys.Open(chm.Filename, OpenMode.MSPACK_SYS_OPEN_READ)) == null)
                return Error.MSPACK_ERR_OPEN;

            // Go through PMGI chunk hierarchy to reach PMGL chunk
            if (chm.IndexRoot < chm.NumChunks)
            {
                n = chm.IndexRoot;
                for (; ; )
                {
                    if ((chunk = ReadChunk(self, chm, fh, n)) == null)
                    {
                        sys.Close(fh);
                        return self.Error;
                    }

                    // Search PMGI/PMGL chunk. exit early if no entry found
                    if ((result = SearchChunk(chm, chunk, filename, ref p, ref end)) <= 0)
                        break;

                    /* found result. loop around for next chunk if this is PMGI */
                    if (chunk[3] == 0x4C)
                    {
                        break;
                    }
                    else
                    {
                        // READ_ENCINT(n)
                        n = 0;
                        do
                        {
                            if (p >= end)
                                goto chunk_end;

                            n = (uint)((n << 7) | (chunk[p] & 0x7F));
                        } while ((chunk[p++] & 0x80) != 0);
                    }
                }
            }
            else
            {
                // PMGL chunks only, search from first_pmgl to last_pmgl
                for (n = chm.FirstPMGL; n <= chm.LastPMGL; n = BitConverter.ToUInt32(chunk, pmgl_NextChunk))
                {
                    if ((chunk = ReadChunk(self, chm, fh, n)) == null)
                    {
                        err = self.Error;
                        break;
                    }

                    // Search PMGL chunk. exit if file found
                    if ((result = SearchChunk(chm, chunk, filename, ref p, ref end)) > 0)
                        break;

                    // Stop simple infinite loops: can't visit the same chunk twice 
                    if (n == BitConverter.ToUInt32(chunk, pmgl_NextChunk))
                        break;
                }
            }

            // If we found a file, read it
            if (result > 0)
            {
                // READ_ENCINT(sec)
                sec = 0;
                do
                {
                    if (p >= end)
                        goto chunk_end;

                    sec = (uint)((sec << 7) | (chunk[p] & 0x7F));
                } while ((chunk[p++] & 0x80) != 0);

                f_ptr.Section = sec == 0 ? chm.Sec0 as Section : chm.Sec1 as Section;

                // READ_ENCINT(f_ptr.Offset)
                f_ptr.Offset = 0;
                do
                {
                    if (p >= end)
                        goto chunk_end;

                    f_ptr.Offset = (uint)((f_ptr.Offset << 7) | (chunk[p] & 0x7F));
                } while ((chunk[p++] & 0x80) != 0);

                // READ_ENCINT(f_ptr.Length)
                f_ptr.Length = 0;
                do
                {
                    if (p >= end)
                        goto chunk_end;

                    f_ptr.Length = (uint)((f_ptr.Length << 7) | (chunk[p] & 0x7F));
                } while ((chunk[p++] & 0x80) != 0);
            }
            else if (result < 0)
            {
                err = Error.MSPACK_ERR_DATAFORMAT;
            }

            sys.Close(fh);
            return self.Error = err;

        chunk_end:
            Console.WriteLine("read beyond end of chunk entries");
            sys.Close(fh);
            return self.Error = Error.MSPACK_ERR_DATAFORMAT;
        }

        /// <summary>
        /// Reads the given chunk into memory, storing it in a chunk cache
        /// so it doesn't need to be read from disk more than once
        /// </summary>
        public static byte[] ReadChunk(DecompressorImpl self, Header chm, FileStream fh, uint chunkNum)
        {
            SystemImpl sys = self.System;

            // Check arguments - most are already checked by chmd_fast_find
            if (chunkNum >= chm.NumChunks)
                return null;

            // ensure chunk cache is available
            if (chm.ChunkCache == null)
                chm.ChunkCache = new byte[chm.NumChunks][];

            // try to answer out of chunk cache */
            if (chm.ChunkCache[chunkNum] != null)
                return chm.ChunkCache[chunkNum];

            // Need to read chunk - allocate memory for it
            byte[] buf = new byte[chm.ChunkSize];

            // Seek to block and read it
            if (!sys.Seek(fh, (chm.DirOffset + (chunkNum * chm.ChunkSize)), SeekMode.MSPACK_SYS_SEEK_START))
            {
                self.Error = Error.MSPACK_ERR_SEEK;
                return null;
            }

            if (sys.Read(fh, buf, 0, (int)chm.ChunkSize) != (int)chm.ChunkSize)
            {
                self.Error = Error.MSPACK_ERR_READ;
                return null;
            }

            // Check the signature. Is is PMGL or PMGI?
            if (!((buf[0] == 0x50) && (buf[1] == 0x4D) && (buf[2] == 0x47) && ((buf[3] == 0x4C) || (buf[3] == 0x49))))
            {
                self.Error = Error.MSPACK_ERR_SEEK;
                return null;
            }

            // All OK. Store chunk in cache and return it
            return chm.ChunkCache[chunkNum] = buf;
        }

        /// <summary>
        /// searches a PMGI/PMGL chunk for a given filename entry. Returns -1 on
        /// data format error, 0 if entry definitely not found, 1 if entry
        /// found.In the latter case, * result and* result_end are set pointing
        /// to that entry's data (either the "next chunk" ENCINT for a PMGI or
        /// the section, offset and length ENCINTs for a PMGL).
        ///
        /// In the case of PMGL chunks, the entry has definitely been
        /// found.In the case of PMGI chunks, the entry which points to the
        /// chunk that may eventually contain that entry has been found.
        /// </summary>
        public static int SearchChunk(Header chm, byte[] chunk, string filename, ref int result, ref int resultEnd)
        {
            int p;
            uint nameLen;
            uint left, right, midpoint, entriesOff;
            bool is_pmgl;
            int cmp;

            // PMGL chunk or PMGI chunk? (note: read_chunk() has already
            // checked the rest of the characters in the chunk signature)
            if (chunk[3] == 0x4C)
            {
                is_pmgl = true;
                entriesOff = pmgl_Entries;
            }
            else
            {
                is_pmgl = false;
                entriesOff = pmgi_Entries;
            }

            // Step 1: binary search first filename of each QR entry
            // - target filename == entry
            //   found file
            // - target filename < all entries
            //   file not found
            // - target filename > all entries
            //   proceed to step 2 using final entry
            // - target filename between two searched entries
            // Proceed to step 2
            uint qrSize = BitConverter.ToUInt32(chunk, pmgl_QuickRefSize);
            int start = (int)(chm.ChunkSize - 2);
            int end = (int)(chm.ChunkSize - qrSize);
            ushort numEntries = BitConverter.ToUInt16(chunk, start);
            uint qrDensity = 1 + (uint)(1 << (int)chm.Density);
            uint qrEntries = (numEntries + qrDensity - 1) / qrDensity;

            if (numEntries == 0)
            {
                Console.Write("chunk has no entries");
                return -1;
            }

            if (qrSize > chm.ChunkSize)
            {
                Console.Write("quickref size > chunk size");
                return -1;
            }

            resultEnd = end;

            if (((int)qrEntries * 2) > (start - end))
            {
                Console.Write("WARNING; more quickrefs than quickref space");
                qrEntries = 0; // But we can live with it
            }

            if (qrEntries > 0)
            {
                left = 0;
                right = qrEntries - 1;
                do
                {
                    // Pick new midpoint
                    midpoint = (left + right) >> 1;

                    // Compare filename with entry QR points to
                    p = (int)(entriesOff + (midpoint != 0 ? BitConverter.ToUInt16(chunk, (int)(start - (midpoint << 1))) : 0));

                    // READ_ENCINT(nameLen)
                    nameLen = 0;
                    do
                    {
                        if (p >= end)
                            goto chunk_end;

                        nameLen = (uint)((nameLen << 7) | (chunk[p] & 0x7F));
                    } while ((chunk[p++] & 0x80) != 0);

                    if (nameLen > (uint)(end - p))
                        goto chunk_end;

                    cmp = string.Compare(filename, Encoding.ASCII.GetString(chunk, p, (int)nameLen), StringComparison.OrdinalIgnoreCase);

                    if (cmp == 0)
                    {
                        break;
                    }
                    else if (cmp < 0)
                    {
                        if (midpoint != 0)
                            right = midpoint - 1;
                        else
                            return 0;
                    }
                    else if (cmp > 0)
                    {
                        left = midpoint + 1;
                    }
                } while (left <= right);

                midpoint = (left + right) >> 1;

                if (cmp == 0)
                {
                    // Exact match!
                    p += (int)nameLen;
                    result = p;
                    return 1;
                }

                // Otherwise, read the group of entries for QR entry M
                p = (int)(entriesOff + (midpoint != 0 ? BitConverter.ToUInt16(chunk, (int)(start - (midpoint << 1))) : 0));
                numEntries -= (ushort)(midpoint * qrDensity);
                if (numEntries > qrDensity)
                    numEntries = (ushort)qrDensity;
            }
            else
            {
                p = (int)entriesOff;
            }

            // Step 2: linear search through the set of entries reached in step 1.
            // - filename == any entry
            //   found entry
            // - filename < all entries (PMGI) or any entry (PMGL)
            //   entry not found, stop now
            // - filename > all entries
            //   entry not found (PMGL) / maybe found (PMGI)
            result = -1;
            while (numEntries-- > 0)
            {
                // READ_ENCINT(nameLen)
                nameLen = 0;
                do
                {
                    if (p >= end)
                        goto chunk_end;

                    nameLen = (uint)((nameLen << 7) | (chunk[p] & 0x7F));
                } while ((chunk[p++] & 0x80) != 0);

                if (nameLen > (uint)(end - p))
                    goto chunk_end;

                cmp = string.Compare(filename, Encoding.ASCII.GetString(chunk, p, (int)nameLen), StringComparison.OrdinalIgnoreCase);
                p += (int)nameLen;

                if (cmp == 0)
                {
                    // Entry found
                    result = p;
                    return 1;
                }

                if (cmp < 0)
                {
                    // Entry not found (PMGL) / maybe found (PMGI)
                    break;
                }

                // Read and ignore the rest of this entry
                if (is_pmgl)
                {
                    // Skip section, offset, and length
                    for (int i = 0; i < 3; i++)
                    {
                        // READ_ENCINT(R)
                        right = 0;
                        do
                        {
                            if (p >= end)
                                goto chunk_end;

                            right = (uint)((right << 7) | (chunk[p] & 0x7F));
                        } while ((chunk[p++] & 0x80) != 0);
                    }
                }
                else
                {
                    result = p; // Store potential final result

                    // Skip chunk number
                    // READ_ENCINT(R)
                    right = 0;
                    do
                    {
                        if (p >= end)
                            goto chunk_end;

                        right = (uint)((right << 7) | (chunk[p] & 0x7F));
                    } while ((chunk[p++] & 0x80) != 0);
                }
            }

            // PMGL? not found. PMGI? maybe found
            return (is_pmgl) ? 0 : (result != 0 ? 1 : 0);

        chunk_end:
            Console.WriteLine("reached end of chunk data while searching");
            return -1;
        }

        #endregion

        #region CHMD_EXTRACT

        /// <summary>
        /// Extracts a file from a CHM helpfile
        /// </summary>
        public static Error Extract(Decompressor d, DecompressFile file, string filename)
        {
            DecompressorImpl self = d as DecompressorImpl;
            if (self == null)
                return Error.MSPACK_ERR_ARGS;

            if (file == null || file.Section == null)
                return self.Error = Error.MSPACK_ERR_ARGS;

            SystemImpl sys = self.System;
            Header chm = file.Section.Header;

            // Create decompression state if it doesn't exist
            if (self.State == null)
            {
                self.State = new DecompressState();
                self.State.Header = chm;
                self.State.Offset = 0;
                self.State.State = null;
                self.State.Sys = sys;
                self.State.Sys.Write = SysWrite;
                self.State.InputFileHandle = null;
                self.State.OutputFileHandle = null;
            }

            // Open input chm file if not open, or the open one is a different chm
            if (self.State.InputFileHandle == null || (self.State.Header != chm))
            {
                sys.Close(self.State.InputFileHandle);
                sys.Close(self.State.OutputFileHandle);

                self.State.Header = chm;
                self.State.Offset = 0;
                self.State.State = null;
                self.State.InputFileHandle = sys.Open(chm.Filename, OpenMode.MSPACK_SYS_OPEN_READ);
                if (self.State.InputFileHandle == null)
                    return self.Error = Error.MSPACK_ERR_OPEN;
            }

            // Open file for output
            FileStream fh;
            if ((fh = sys.Open(filename, OpenMode.MSPACK_SYS_OPEN_WRITE)) == null)
                return self.Error = Error.MSPACK_ERR_OPEN;

            // If file is empty, simply creating it is enough
            if (file.Length == 0)
            {
                sys.Close(fh);
                return self.Error = Error.MSPACK_ERR_OK;
            }

            self.Error = Error.MSPACK_ERR_OK;

            switch (file.Section.ID)
            {
                // Uncompressed section file
                case 0:
                    // Simple seek + copy
                    if (!sys.Seek(self.State.InputFileHandle, file.Section.Header.Sec0.Offset + file.Offset, SeekMode.MSPACK_SYS_SEEK_START))
                    {
                        self.Error = Error.MSPACK_ERR_SEEK;
                    }
                    else
                    {
                        byte[] buf = new byte[512];
                        long length = file.Length;
                        while (length > 0)
                        {
                            int run = 512;
                            if (run > length)
                                run = (int)length;

                            if (sys.Read(self.State.InputFileHandle, buf, 0, run) != run)
                            {
                                self.Error = Error.MSPACK_ERR_READ;
                                break;
                            }

                            if (sys.Write(fh, buf, 0, run) != run)
                            {
                                self.Error = Error.MSPACK_ERR_WRITE;
                                break;
                            }

                            length -= run;
                        }
                    }
                    break;

                // MSCompressed section file
                case 1:
                    // (Re)initialise compression state if we it is not yet initialised,
                    // or we have advanced too far and have to backtrack
                    if (self.State.State == null || (file.Offset < self.State.Offset))
                    {
                        if (self.State.State != null)
                            self.State.State = null;

                        if (InitDecompressor(self, file) != Error.MSPACK_ERR_OK)
                            break;
                    }

                    // Seek to input data
                    if (!sys.Seek(self.State.InputFileHandle, self.State.InOffset, SeekMode.MSPACK_SYS_SEEK_START))
                    {
                        self.Error = Error.MSPACK_ERR_SEEK;
                        break;
                    }

                    // Get to correct offset.
                    self.State.OutputFileHandle = null;
                    long bytes;
                    if ((bytes = file.Offset - self.State.Offset) != 0)
                        self.Error = LZX.Decompress(self.State.State, bytes);

                    // If getting to the correct offset was error free, unpack file
                    if (self.Error == Error.MSPACK_ERR_OK)
                    {
                        self.State.OutputFileHandle = fh;
                        self.Error = LZX.Decompress(self.State.State, file.Length);
                    }

                    // Save offset in input source stream, in case there is a section 0
                    // file between now and the next section 1 file extracted
                    self.State.InOffset = sys.Tell(self.State.InputFileHandle);

                    // If an LZX error occured, the LZX decompressor is now useless
                    if (self.Error != Error.MSPACK_ERR_OK)
                        self.State.State = null;

                    break;
            }

            sys.Close(fh);
            return self.Error;
        }

        #endregion

        #region CHMD_SYS_WRITE

        /// <summary>
        /// chmd_sys_write is the internal writer function which the decompressor
        /// uses. If either writes data to disk (self.State.OutputFileHandle) with the real
        /// sys.write() function, or does nothing with the data when
        /// self.State.OutputFileHandle == null. advances self.State.Offset.
        /// </summary>
        private static int SysWrite(object file, byte[] buffer, int offset, int bytes)
        {
            if (file is DecompressorImpl self)
            {
                self.State.Offset += (uint)bytes;
                if (self.State.OutputFileHandle != null)
                    return self.System.Write(self.State.OutputFileHandle, buffer, offset, bytes);

                return bytes;
            }
            else if (file is FileStream impl)
            {
                return SystemImpl.DefaultSystem.Write(impl, buffer, offset, bytes);
            }

            // Unknown file to write to
            return 0;
        }

        #endregion

        #region CHMD_INIT_DECOMP

        /// <summary>
        /// Initialises the LZX decompressor to decompress the compressed stream,
        /// from the nearest reset offset and length that is needed for the given
        /// file.
        /// </summary>
        public static Error InitDecompressor(DecompressorImpl self, DecompressFile file)
        {
            int window_size, window_bits, reset_interval, entry;
            SystemImpl sys = self.System;
            byte[] data;

            MSCompressedSection sec = file.Section as MSCompressedSection;

            // Ensure we have a mscompressed content section
            DecompressFile contentFile = null;
            Error err = FindSysFile(self, sec, ref contentFile, ContentName);
            if (err != Error.MSPACK_ERR_OK)
                return self.Error = err;

            sec.Content = contentFile;

            // Ensure we have a ControlData file
            DecompressFile controlFile = null;
            err = FindSysFile(self, sec, ref controlFile, ControlName);
            if (err != Error.MSPACK_ERR_OK)
                return self.Error = err;

            sec.Control = controlFile;

            // Read ControlData
            if (sec.Control.Length != lzxcd_SIZEOF)
            {
                Console.WriteLine("ControlData file is wrong size");
                return self.Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            if ((data = ReadSysFile(self, sec.Control)) == null)
            {
                Console.WriteLine("can't read mscompressed control data file");
                return self.Error;
            }

            // Check LZXC signature
            if (BitConverter.ToUInt32(data, lzxcd_Signature) != 0x43585A4C)
            {
                return self.Error = Error.MSPACK_ERR_SIGNATURE;
            }

            // Read reset_interval and window_size and validate version number
            switch (BitConverter.ToUInt32(data, lzxcd_Version))
            {
                case 1:
                    reset_interval = (int)BitConverter.ToUInt32(data, lzxcd_ResetInterval);
                    window_size = (int)BitConverter.ToUInt32(data, lzxcd_WindowSize);
                    break;
                case 2:
                    reset_interval = (int)BitConverter.ToUInt32(data, lzxcd_ResetInterval) * LZX.LZX_FRAME_SIZE;
                    window_size = (int)BitConverter.ToUInt32(data, lzxcd_WindowSize) * LZX.LZX_FRAME_SIZE;
                    break;
                default:
                    Console.WriteLine("bad controldata version");
                    return self.Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            // Find window_bits from window_size
            switch (window_size)
            {
                case 0x008000: window_bits = 15; break;
                case 0x010000: window_bits = 16; break;
                case 0x020000: window_bits = 17; break;
                case 0x040000: window_bits = 18; break;
                case 0x080000: window_bits = 19; break;
                case 0x100000: window_bits = 20; break;
                case 0x200000: window_bits = 21; break;
                default:
                    Console.WriteLine("bad controldata window size");
                    return self.Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            // Validate reset_interval
            if (reset_interval == 0 || (reset_interval % LZX.LZX_FRAME_SIZE) != 0)
            {
                Console.WriteLine("bad controldata reset interval");
                return self.Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            // Which reset table entry would we like?
            entry = (int)(file.Offset / reset_interval);

            // Convert from reset interval multiple (usually 64k) to 32k frames
            entry *= reset_interval / LZX.LZX_FRAME_SIZE;

            // Read the reset table entry
            if (ReadResetTable(self, sec, (uint)entry, out long length, out long offset))
            {
                // The uncompressed length given in the reset table is dishonest.
                // The uncompressed data is always padded out from the given
                // uncompressed length up to the next reset interval
                length += reset_interval - 1;
                length &= -reset_interval;
            }
            else
            {
                // if we can't read the reset table entry, just start from
                // the beginning. Use spaninfo to get the uncompressed length
                entry = 0;
                offset = 0;
                err = ReadSpanInfo(self, sec, out length);
            }

            if (err != Error.MSPACK_ERR_OK)
                return self.Error = err;

            // Get offset of compressed data stream:
            // = offset of uncompressed section from start of file
            // + offset of compressed stream from start of uncompressed section
            // + offset of chosen reset interval from start of compressed stream
            self.State.InOffset = file.Section.Header.Sec0.Offset + sec.Content.Offset + offset;

            // Set start offset and overall remaining stream length
            self.State.Offset = entry * LZX.LZX_FRAME_SIZE;
            length -= self.State.Offset;

            // Initialise LZX stream
            self.State.State = LZX.Init(self.State.Sys, self.State.InputFileHandle, self.State.OutputFileHandle, window_bits, reset_interval / LZX.LZX_FRAME_SIZE, 4096, length, false);

            if (self.State.State == null)
                self.Error = Error.MSPACK_ERR_NOMEMORY;

            return self.Error;
        }

        #endregion

        #region READ_RESET_TABLE

        /// <summary>
        /// Reads one entry out of the reset table. Also reads the uncompressed
        /// data length. Writes these to offset_ptr and length_ptr respectively.
        /// Returns non-zero for success, zero for failure.
        /// </summary>
        public static bool ReadResetTable(DecompressorImpl self, MSCompressedSection sec, uint entry, out long length_ptr, out long offset_ptr)
        {
            length_ptr = 0; offset_ptr = 0;
            SystemImpl sys = self.System;
            byte[] data;

            // Do we have a ResetTable file?
            DecompressFile resetTable = null;
            Error err = FindSysFile(self, sec, ref resetTable, ResetTableName);
            if (err != Error.MSPACK_ERR_OK)
                return false;

            sec.ResetTable = resetTable;

            // Read ResetTable file
            if (sec.ResetTable.Length < lzxrt_headerSIZEOF)
            {
                Console.WriteLine("ResetTable file is too short");
                return false;
            }

            if (sec.ResetTable.Length > 1000000)
            {
                // Arbitrary upper limit 
                Console.WriteLine($"ResetTable >1MB ({sec.ResetTable.Length}), report if genuine");
                return false;
            }

            if ((data = ReadSysFile(self, sec.ResetTable)) == null)
            {
                Console.WriteLine("can't read reset table");
                return false;
            }

            // Check sanity of reset table
            if (BitConverter.ToUInt32(data, lzxrt_FrameLen) != LZX.LZX_FRAME_SIZE)
            {
                Console.WriteLine(("bad reset table frame length"));
                return false;
            }

            // Get the uncompressed length of the LZX stream
            if ((length_ptr = BitConverter.ToInt64(data, lzxrt_UncompLen)) == 0)
                return false;

            uint entrysize = BitConverter.ToUInt32(data, lzxrt_EntrySize);
            uint pos = BitConverter.ToUInt32(data, lzxrt_TableOffset) + (entry * entrysize);

            // Ensure reset table entry for this offset exists
            if (entry < BitConverter.ToUInt32(data, lzxrt_NumEntries) && pos <= (sec.ResetTable.Length - entrysize))
            {
                switch (entrysize)
                {
                    case 4:
                        offset_ptr = BitConverter.ToUInt32(data, (int)pos);
                        err = 0;
                        break;
                    case 8:
                        offset_ptr = BitConverter.ToInt64(data, (int)pos);
                        break;
                    default:
                        Console.WriteLine("reset table entry size neither 4 nor 8");
                        err = Error.MSPACK_ERR_ARGS;
                        break;
                }
            }
            else
            {
                Console.WriteLine("bad reset interval");
                err = Error.MSPACK_ERR_ARGS;
            }

            // Return success
            return (err == Error.MSPACK_ERR_OK);
        }

        #endregion

        #region READ_SPANINFO

        /// <summary>
        /// Reads the uncompressed data length from the spaninfo file.
        /// Returns zero for success or a non-zero error code for failure.
        /// </summary>
        public static Error ReadSpanInfo(DecompressorImpl self, MSCompressedSection sec, out long length_ptr)
        {
            length_ptr = 0;
            SystemImpl sys = self.System;

            // Find SpanInfo file
            DecompressFile spanInfo = null;
            Error err = FindSysFile(self, sec, ref spanInfo, SpanInfoName);
            if (err != Error.MSPACK_ERR_OK)
                return Error.MSPACK_ERR_DATAFORMAT;

            sec.SpanInfo = spanInfo;

            // Check it's large enough
            if (sec.SpanInfo.Length != 8)
            {
                Console.WriteLine("SpanInfo file is wrong size");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // Read the SpanInfo file
            byte[] data;
            if ((data = ReadSysFile(self, sec.SpanInfo)) == null)
            {
                Console.WriteLine("can't read SpanInfo file");
                return self.Error;
            }

            // Get the uncompressed length of the LZX stream
            length_ptr = BitConverter.ToInt64(data, 0);
            if (length_ptr <= 0)
            {
                Console.WriteLine("output length is invalid");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region FIND_SYS_FILE

        /// <summary>
        /// Uses chmd_fast_find to locate a system file, and fills out that system
        /// file's entry and links it into the list of system files. Returns zero
        /// for success, non-zero for both failure and the file not existing.
        /// </summary>
        public static Error FindSysFile(DecompressorImpl self, MSCompressedSection sec, ref DecompressFile f_ptr, string name)
        {
            SystemImpl sys = self.System;
            DecompressFile result = null;

            // Already loaded
            if (f_ptr != null)
                return Error.MSPACK_ERR_OK;

            // Try using fast_find to find the file - return DATAFORMAT error if
            // it fails, or successfully doesn't find the file
            if (FastFind(self, sec.Header, name, result) != Error.MSPACK_ERR_OK || result.Section == null)
                return Error.MSPACK_ERR_DATAFORMAT;

            f_ptr = new DecompressFile();

            // Copy result
            f_ptr = result;
            f_ptr.Filename = name;

            // Link file into sysfiles list
            f_ptr.Next = sec.Header.SysFiles;
            sec.Header.SysFiles = f_ptr;

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region READ_SYS_FILE

        /// <summary>
        /// Allocates memory for a section 0 (uncompressed) file and reads it into memory.
        /// </summary>
        public static byte[] ReadSysFile(DecompressorImpl self, DecompressFile file)
        {
            SystemImpl sys = self.System;

            if (file == null || file.Section == null || (file.Section.ID != 0))
            {
                self.Error = Error.MSPACK_ERR_DATAFORMAT;
                return null;
            }

            int len = (int)file.Length;
            byte[] data = new byte[len];

            if (sys.Seek(self.State.InputFileHandle, file.Section.Header.Sec0.Offset + file.Offset, SeekMode.MSPACK_SYS_SEEK_START))
            {
                self.Error = Error.MSPACK_ERR_SEEK;
                return null;
            }

            if (sys.Read(self.State.InputFileHandle, data, 0, len) != len)
            {
                self.Error = Error.MSPACK_ERR_READ;
                return null;
            }

            return data;
        }

        #endregion

        #region CHMD_ERROR

        /// <summary>
        /// Returns the last error that occurred
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Error LastError(Decompressor d)
        {
            DecompressorImpl self = d as DecompressorImpl;
            return (self != null ? self.Error : Error.MSPACK_ERR_ARGS);
        }

        #endregion
    }
}
