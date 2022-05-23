/* libmspack -- a library for working with Microsoft compression formats.
 * (C) 2003-2019 Stuart Caie <kyzer@cabextract.org.uk>
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 */

using System;
using System.IO;
using System.Text;
using LibMSPackSharp.Compression;
using static LibMSPackSharp.Constants;

namespace LibMSPackSharp.CHM
{
    /// <summary>
    /// A decompressor for .CHM (Microsoft HTMLHelp) files
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateCHMDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyCHMDecompressor(Decompressor)"/>
    public class Decompressor : BaseDecompressor
    {
        #region Fields

        public DecompressState State { get; set; }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Opens a CHM helpfile and reads its contents.
        /// 
        /// If the file opened is a valid CHM helpfile, all headers will be read
        /// and a mschmd_header structure will be returned, with a full list of
        /// files.
        /// 
        /// In the case of an error occuring, NULL is returned and the error code
        /// is available from last_error().
        /// 
        /// The filename pointer should be considered "in use" until close() is
        /// called on the CHM helpfile.
        /// </summary>
        /// <param name="filename">
        /// the filename of the CHM helpfile. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>a pointer to a mschmd_header structure, or NULL on failure</returns>
        /// <see cref="Close(CHM)"/>
        public CHM Open(string filename) => RealOpen(filename, true);

        /// <summary>
        /// Closes a previously opened CHM helpfile.
        /// 
        /// This closes a CHM helpfile, frees the mschmd_header and all
        /// mschmd_file structures associated with it (if any). This works on
        /// both helpfiles opened with open() and helpfiles opened with
        /// fast_open().
        /// 
        /// The CHM header pointer is now invalid and cannot be used again. All
        /// mschmd_file pointers referencing that CHM are also now invalid, and
        /// cannot be used again.
        /// </summary>
        /// <param name="chm">the CHM helpfile to close</param>
        /// <see cref="Open(string)"/>
        /// <see cref="FastOpen(string)"/>
        public void Close(CHM chm)
        {
            Error = Error.MSPACK_ERR_OK;

            // Free files
            DecompressFile fi, nfi;
            for (fi = chm.Files; fi != null; fi = nfi)
            {
                nfi = fi.Next;
            }

            for (fi = chm.SysFiles; fi != null; fi = nfi)
            {
                nfi = fi.Next;
            }

            // If this CHM was being decompressed, free decompression state
            if (State != null && (State.Header == chm))
            {
                System.Close(State.InputFileHandle);
                System.Close(State.OutputFileHandle);

                State = null;
            }
        }

        /// <summary>
        /// Extracts a file from a CHM helpfile.
        ///
        /// This extracts a file from a CHM helpfile and writes it to the given
        /// filename.The filename of the file, mscabd_file::filename, is not
        /// used by extract(), but can be used by the caller as a guide for
        /// constructing an appropriate filename.
        ///
        /// This method works both with files found in the mschmd_header::files
        /// and mschmd_header::sysfiles list and mschmd_file structures generated
        /// on the fly by fast_find().
        /// </summary>
        /// <param name="file">the file to be decompressed</param>
        /// <param name="filename">the filename of the file being written to</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Error Extract(DecompressFile file, string filename)
        {
            if (file == null || file.Section == null)
                return Error = Error.MSPACK_ERR_ARGS;

            CHM chm = file.Section.Header;

            // Create decompression state if it doesn't exist
            if (State == null)
            {
                State = new DecompressState();
                State.Header = chm;
                State.Offset = 0;
                State.State = null;
                State.System = System;
                State.System.Write = SysWrite;
                State.InputFileHandle = null;
                State.OutputFileHandle = null;
            }

            // Open input chm file if not open, or the open one is a different chm
            if (State.InputFileHandle == null || (State.Header != chm))
            {
                System.Close(State.InputFileHandle);
                System.Close(State.OutputFileHandle);

                State.Header = chm;
                State.Offset = 0;
                State.State = null;
                State.InputFileHandle = System.Open(chm.Filename, OpenMode.MSPACK_SYS_OPEN_READ);
                if (State.InputFileHandle == null)
                    return Error = Error.MSPACK_ERR_OPEN;
            }

            // Open file for output
            FileStream fh = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_WRITE);
            if (fh == null)
                return Error = Error.MSPACK_ERR_OPEN;

            // If file is empty, simply creating it is enough
            if (file.Length == 0)
            {
                System.Close(fh);
                return Error = Error.MSPACK_ERR_OK;
            }

            Error = Error.MSPACK_ERR_OK;

            switch (file.Section.ID)
            {
                // Uncompressed section file
                case 0:
                    // Simple seek + copy
                    if (!System.Seek(State.InputFileHandle, file.Section.Header.Sec0.Offset + file.Offset, SeekMode.MSPACK_SYS_SEEK_START))
                    {
                        Error = Error.MSPACK_ERR_SEEK;
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

                            if (System.Read(State.InputFileHandle, buf, 0, run) != run)
                            {
                                Error = Error.MSPACK_ERR_READ;
                                break;
                            }

                            if (System.Write(fh, buf, 0, run) != run)
                            {
                                Error = Error.MSPACK_ERR_WRITE;
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
                    if (State.State == null || (file.Offset < State.Offset))
                    {
                        if (State.State != null)
                            State.State = null;

                        if (InitDecompressor(file) != Error.MSPACK_ERR_OK)
                            break;
                    }

                    // Seek to input data
                    if (!System.Seek(State.InputFileHandle, State.InOffset, SeekMode.MSPACK_SYS_SEEK_START))
                    {
                        Error = Error.MSPACK_ERR_SEEK;
                        break;
                    }

                    // Get to correct offset.
                    State.OutputFileHandle = null;
                    long bytes;
                    if ((bytes = file.Offset - State.Offset) != 0)
                        Error = LZX.Decompress(State.State, bytes);

                    // If getting to the correct offset was error free, unpack file
                    if (Error == Error.MSPACK_ERR_OK)
                    {
                        State.OutputFileHandle = fh;
                        Error = LZX.Decompress(State.State, file.Length);
                    }

                    // Save offset in input source stream, in case there is a section 0
                    // file between now and the next section 1 file extracted
                    State.InOffset = System.Tell(State.InputFileHandle);

                    // If an LZX error occured, the LZX decompressor is now useless
                    if (Error != Error.MSPACK_ERR_OK)
                        State.State = null;

                    break;
            }

            System.Close(fh);
            return Error;
        }

        /// <summary>
        /// Opens a CHM helpfile quickly.
        ///
        /// If the file opened is a valid CHM helpfile, only essential headers
        /// will be read.A mschmd_header structure will be still be returned, as
        /// with open(), but the mschmd_header::files field will be NULL.No
        /// files details will be automatically read.The fast_find() method
        /// must be used to obtain file details.
        ///
        /// In the case of an error occuring, NULL is returned and the error code
        /// is available from last_error().
        ///
        /// The filename pointer should be considered "in use" until close() is
        /// called on the CHM helpfile.
        /// </summary>
        /// <param name="filename">
        /// the filename of the CHM helpfile. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>a pointer to a mschmd_header structure, or NULL on failure</returns>
        /// <see cref="Open(string)"/>
        /// <see cref="Close(CHM)"/>
        /// <see cref="FastFind(CHM, string, DecompressFile)"/>
        /// <see cref="Extract(DecompressFile, string)"/>
        public CHM FastOpen(string filename) => RealOpen(filename, false);

        /// <summary>
        /// Finds file details quickly.
        ///
        /// Instead of reading all CHM helpfile headers and building a list of
        /// files, fast_open() and fast_find() are intended for finding file
        /// details only when they are needed.The CHM file format includes an
        /// on-disk file index to allow this.
        ///
        /// Given a case-sensitive filename, fast_find() will search the on-disk
        /// index for that file.
        ///
        /// If the file was found, the caller-provided mschmd_file structure will
        /// be filled out like so:
        /// - section: the correct value for the found file
        /// - offset: the correct value for the found file
        /// - length: the correct value for the found file
        /// - all other structure elements: NULL or 0
        ///
        /// If the file was not found, MSPACK_ERR_OK will still be returned as the
        /// result, but the caller-provided structure will be filled out like so:
        /// - section: NULL
        /// - offset: 0
        /// - length: 0
        /// - all other structure elements: NULL or 0
        ///
        /// This method is intended to be used in conjunction with CHM helpfiles
        /// opened with fast_open(), but it also works with helpfiles opened
        /// using the regular open().
        /// </summary>
        /// <param name="chm">the CHM helpfile to search for the file</param>
        /// <param name="filename">the filename of the file to search for</param>
        /// <param name="f_ptr">a pointer to a caller-provded mschmd_file structure</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="Open(string)"/>
        /// <see cref="Close(CHM)"/>
        /// <see cref="FastOpen(string)"/>
        /// <see cref="Extract(DecompressFile, string)"/>
        public Error FastFind(CHM chm, string filename, DecompressFile f_ptr)
        {
            // p and end are initialised to prevent MSVC warning about "potentially"
            // uninitialised usage. This is provably untrue, but MS won't fix:
            // https://developercommunity.visualstudio.com/content/problem/363489/c4701-false-positive-warning.html
            byte[] chunk = new byte[0];
            int p = -1, end = -1, result = -1;
            Error err = Error.MSPACK_ERR_OK;
            uint n, sec;

            if (chm == null || f_ptr == null)
                return Error.MSPACK_ERR_ARGS;

            // Clear the results structure
            f_ptr = new DecompressFile();

            FileStream fh = System.Open(chm.Filename, OpenMode.MSPACK_SYS_OPEN_READ);
            if (fh == null)
                return Error.MSPACK_ERR_OPEN;

            // Go through PMGI chunk hierarchy to reach PMGL chunk
            if (chm.HeaderSection1.IndexRoot < chm.HeaderSection1.NumChunks)
            {
                n = chm.HeaderSection1.IndexRoot;
                for (; ; )
                {
                    if ((chunk = ReadChunk(chm, fh, n)) == null)
                    {
                        System.Close(fh);
                        return Error;
                    }

                    // Search PMGI/PMGL chunk. exit early if no entry found
                    if ((result = SearchChunk(chm, chunk, filename, ref p, ref end)) <= 0)
                        break;

                    // Found result. loop around for next chunk if this is PMGI
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
                            {
                                Console.WriteLine("Read beyond end of chunk entries");
                                System.Close(fh);
                                return Error = Error.MSPACK_ERR_DATAFORMAT;
                            }

                            n = (uint)((n << 7) | (chunk[p] & 0x7F));
                        } while ((chunk[p++] & 0x80) != 0);
                    }
                }
            }
            else
            {
                // PMGL chunks only, search from first_pmgl to last_pmgl
                for (n = chm.HeaderSection1.FirstPMGL; n <= chm.HeaderSection1.LastPMGL; n = BitConverter.ToUInt32(chunk, 0x0010))
                {
                    if ((chunk = ReadChunk(chm, fh, n)) == null)
                    {
                        err = Error;
                        break;
                    }

                    // Search PMGL chunk. exit if file found
                    if ((result = SearchChunk(chm, chunk, filename, ref p, ref end)) > 0)
                        break;

                    // Stop simple infinite loops: can't visit the same chunk twice 
                    if (n == BitConverter.ToUInt32(chunk, 0x0010))
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
                    {
                        Console.WriteLine("Read beyond end of chunk entries");
                        System.Close(fh);
                        return Error = Error.MSPACK_ERR_DATAFORMAT;
                    }

                    sec = (uint)((sec << 7) | (chunk[p] & 0x7F));
                } while ((chunk[p++] & 0x80) != 0);

                f_ptr.Section = sec == 0 ? chm.Sec0 as Section : chm.Sec1 as Section;

                // READ_ENCINT(f_ptr.Offset)
                f_ptr.Offset = 0;
                do
                {
                    if (p >= end)
                    {
                        Console.WriteLine("Read beyond end of chunk entries");
                        System.Close(fh);
                        return Error = Error.MSPACK_ERR_DATAFORMAT;
                    }

                    f_ptr.Offset = (uint)((f_ptr.Offset << 7) | (chunk[p] & 0x7F));
                } while ((chunk[p++] & 0x80) != 0);

                // READ_ENCINT(f_ptr.Length)
                f_ptr.Length = 0;
                do
                {
                    if (p >= end)
                    {
                        Console.WriteLine("Read beyond end of chunk entries");
                        System.Close(fh);
                        return Error = Error.MSPACK_ERR_DATAFORMAT;
                    }

                    f_ptr.Length = (uint)((f_ptr.Length << 7) | (chunk[p] & 0x7F));
                } while ((chunk[p++] & 0x80) != 0);
            }
            else if (result < 0)
            {
                err = Error.MSPACK_ERR_DATAFORMAT;
            }

            System.Close(fh);
            return Error = err;
        }

        #endregion

        #region Decompress State

        /// <summary>
        /// Initialises the LZX decompressor to decompress the compressed stream,
        /// from the nearest reset offset and length that is needed for the given
        /// file.
        /// </summary>
        internal Error InitDecompressor(DecompressFile file)
        {
            int entry;
            byte[] data;

            MSCompressedSection sec = file.Section as MSCompressedSection;

            // Ensure we have a mscompressed content section
            DecompressFile contentFile = null;
            Error err = FindSysFile(sec, ref contentFile, ContentName);
            if (err != Error.MSPACK_ERR_OK)
                return Error = err;

            sec.Content = contentFile;

            // Ensure we have a ControlData file
            DecompressFile controlFile = null;
            err = FindSysFile(sec, ref controlFile, ControlName);
            if (err != Error.MSPACK_ERR_OK)
                return Error = err;

            sec.Control = controlFile;

            // Read ControlData
            if (sec.Control.Length != _LZXControlData.Size)
            {
                Console.WriteLine("ControlData file is wrong size");
                return Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            if ((data = ReadSysFile(sec.Control)) == null)
            {
                Console.WriteLine("Can't read mscompressed control data file");
                return Error;
            }

            // Create a new control data based on that
            err = _LZXControlData.Create(data, out _LZXControlData lzxControlData);
            if (err != Error.MSPACK_ERR_OK)
                return Error = err;

            // Find window_bits from window_size
            err = lzxControlData.GetWindowBits(out int windowBits);
            if (err != Error.MSPACK_ERR_OK)
                return Error = err;

            // Validate reset_interval
            if (lzxControlData.ResetInterval == 0 || (lzxControlData.ResetInterval % LZX.LZX_FRAME_SIZE) != 0)
            {
                Console.WriteLine("Bad controldata reset interval");
                return Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            // Which reset table entry would we like?
            entry = (int)(file.Offset / lzxControlData.ResetInterval);

            // Convert from reset interval multiple (usually 64k) to 32k frames
            entry *= (int)lzxControlData.ResetInterval / LZX.LZX_FRAME_SIZE;

            // Read the reset table entry
            if (ReadResetTable(sec, (uint)entry, out long length, out long offset))
            {
                // The uncompressed length given in the reset table is dishonest.
                // The uncompressed data is always padded out from the given
                // uncompressed length up to the next reset interval
                length += lzxControlData.ResetInterval - 1;
                length &= -lzxControlData.ResetInterval;
            }
            else
            {
                // if we can't read the reset table entry, just start from
                // the beginning. Use spaninfo to get the uncompressed length
                entry = 0;
                offset = 0;
                err = ReadSpanInfo(sec, out length);
            }

            if (err != Error.MSPACK_ERR_OK)
                return Error = err;

            // Get offset of compressed data stream:
            // = offset of uncompressed section from start of file
            // + offset of compressed stream from start of uncompressed section
            // + offset of chosen reset interval from start of compressed stream
            State.InOffset = file.Section.Header.Sec0.Offset + sec.Content.Offset + offset;

            // Set start offset and overall remaining stream length
            State.Offset = entry * LZX.LZX_FRAME_SIZE;
            length -= State.Offset;

            // Initialise LZX stream
            State.State = LZX.Init(State.System, State.InputFileHandle, State.OutputFileHandle, windowBits, (int)lzxControlData.ResetInterval / LZX.LZX_FRAME_SIZE, 4096, length, false);

            if (State.State == null)
                Error = Error.MSPACK_ERR_NOMEMORY;

            return Error;
        }

        #endregion

        #region I/O Methods

        /// <summary>
        /// SysWrite is the internal writer function which the decompressor
        /// uses. If either writes data to disk (self.State.OutputFileHandle) with the real
        /// sys.write() function, or does nothing with the data when
        /// self.State.OutputFileHandle == null. advances self.State.Offset.
        /// </summary>
        private static int SysWrite(object file, byte[] buffer, int offset, int bytes)
        {
            // Null output file means skip those bytes
            if (file == null)
            {
                return bytes;
            }
            else if (file is Decompressor self)
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

        #region Helpers

        /// <summary>
        /// Uses chmd_fast_find to locate a system file, and fills out that system
        /// file's entry and links it into the list of system files. Returns zero
        /// for success, non-zero for both failure and the file not existing.
        /// </summary>
        private Error FindSysFile(MSCompressedSection sec, ref DecompressFile f_ptr, string name)
        {
            DecompressFile result = null;

            // Already loaded
            if (f_ptr != null)
                return Error.MSPACK_ERR_OK;

            // Try using fast_find to find the file - return DATAFORMAT error if
            // it fails, or successfully doesn't find the file
            if (FastFind(sec.Header, name, result) != Error.MSPACK_ERR_OK || result.Section == null)
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

        /// <summary>
        /// Reads the given chunk into memory, storing it in a chunk cache
        /// so it doesn't need to be read from disk more than once
        /// </summary>
        private byte[] ReadChunk(CHM chm, FileStream fh, uint chunkNum)
        {
            // Check arguments - most are already checked by chmd_fast_find
            if (chunkNum >= chm.HeaderSection1.NumChunks)
                return null;

            // Ensure chunk cache is available
            if (chm.ChunkCache == null)
                chm.ChunkCache = new byte[chm.HeaderSection1.NumChunks][];

            // Try to answer out of chunk cache
            if (chm.ChunkCache[chunkNum] != null)
                return chm.ChunkCache[chunkNum];

            // Need to read chunk - allocate memory for it
            byte[] buf = new byte[chm.HeaderSection1.ChunkSize];

            // Seek to block and read it
            if (!System.Seek(fh, (chm.HeaderSectionTable.OffsetHS1 + (chunkNum * chm.HeaderSection1.ChunkSize)), SeekMode.MSPACK_SYS_SEEK_START))
            {
                Error = Error.MSPACK_ERR_SEEK;
                return null;
            }

            if (System.Read(fh, buf, 0, (int)chm.HeaderSection1.ChunkSize) != (int)chm.HeaderSection1.ChunkSize)
            {
                Error = Error.MSPACK_ERR_READ;
                return null;
            }

            // Check the signature. Is is PMGL or PMGI?
            if (!((buf[0] == 0x50) && (buf[1] == 0x4D) && (buf[2] == 0x47) && ((buf[3] == 0x4C) || (buf[3] == 0x49))))
            {
                Error = Error.MSPACK_ERR_SEEK;
                return null;
            }

            // All OK. Store chunk in cache and return it
            return chm.ChunkCache[chunkNum] = buf;
        }

        /// <summary>
        /// Reads the basic CHM file headers. If the "entire" parameter is
        /// non-zero, all file entries will also be read. fills out a pre-existing
        /// mschmd_header structure, allocates memory for files as necessary
        /// </summary>
        private Error ReadHeaders(FileStream fh, CHM chm, bool entire)
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

            #region Header

            // Read the first header
            if (System.Read(fh, buf, 0, _CHMHeader.Size) != _CHMHeader.Size)
                return Error.MSPACK_ERR_READ;

            // Create a new header based on that
            Error err = _CHMHeader.Create(buf, out _CHMHeader chmHeader);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            // Assign the header
            chm.Header = chmHeader;
            if (chm.Header.Version > 3)
                System.Message(fh, "WARNING; CHM version > 3");

            #endregion

            #region Header Section Table

            // Read the header section table
            if (System.Read(fh, buf, 0, _HeaderSectionTable.V3Size) != _HeaderSectionTable.V3Size)
                return Error.MSPACK_ERR_READ;

            // Create a new secton table based on that
            err = _HeaderSectionTable.Create(buf, out _HeaderSectionTable sectionTable);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            // Assign the section table
            chm.HeaderSectionTable = sectionTable;

            // Assign the CS0 value to the Sec0.Offset for later
            chm.Sec0.Offset = chm.HeaderSectionTable.OffsetCS0;

            #endregion

            #region Header Section 0

            // Seek to header section 0
            if (!System.Seek(fh, chm.HeaderSectionTable.OffsetHS0, SeekMode.MSPACK_SYS_SEEK_START))
                return Error.MSPACK_ERR_SEEK;

            // Read header section 0
            if (System.Read(fh, buf, 0, _HeaderSection0.Size) != _HeaderSection0.Size)
                return Error.MSPACK_ERR_READ;

            // Create a new secton 0 based on that
            err = _HeaderSection0.Create(buf, out _HeaderSection0 section0);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            // Assign the section 0
            chm.HeaderSection0 = section0;

            #endregion

            #region Header Section 1

            // Seek to header section 1
            if (!System.Seek(fh, chm.HeaderSectionTable.OffsetHS1, SeekMode.MSPACK_SYS_SEEK_START))
                return Error.MSPACK_ERR_SEEK;

            // Read header section 1
            if (System.Read(fh, buf, 0, _HeaderSection1.Size) != _HeaderSection1.Size)
                return Error.MSPACK_ERR_READ;

            // Create a new secton 1 based on that
            err = _HeaderSection1.Create(buf, out _HeaderSection1 section1);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            // Assign the section 1
            chm.HeaderSection1 = section1;

            chm.HeaderSectionTable.OffsetHS1 = System.Tell(fh);

            // Versions before 3 don't have OffsetCS0
            if (chm.Header.Version < 3)
                chm.Sec0.Offset = chm.HeaderSectionTable.OffsetHS1 + (chm.HeaderSection1.ChunkSize * chm.HeaderSection1.NumChunks);

            // Check if content offset or file size is wrong
            if (chm.Sec0.Offset > chm.HeaderSection0.FileLength)
            {
                Console.WriteLine("Content section begins after file has ended");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // Ensure there are chunks and that chunk size is
            // large enough for signature and num_entries
            if (chm.HeaderSection1.ChunkSize < (_PMGHeader.PMGLSize + 2))
            {
                Console.WriteLine("Chunk size not large enough");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            if (chm.HeaderSection1.NumChunks == 0)
            {
                Console.WriteLine("No chunks");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // The ChunkCache data structure is not great; large values for NumChunks
            // or NumChunks*ChunkSize can exhaust all memory. Until a better chunk
            // cache is implemented, put arbitrary limits on NumChunks and chunk size.
            if (chm.HeaderSection1.NumChunks > 100000)
            {
                Console.WriteLine("More than 100,000 chunks");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            if (chm.HeaderSection1.ChunkSize > 8192)
            {
                Console.WriteLine("Chunk size over 8192 (get in touch if this is valid)");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            if (chm.HeaderSection1.ChunkSize * (long)chm.HeaderSection1.NumChunks > chm.HeaderSection0.FileLength)
            {
                Console.WriteLine("Chunks larger than entire file");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // Common sense checks on header section 1 fields
            if (chm.HeaderSection1.ChunkSize != 4096)
                System.Message(fh, "WARNING; chunk size is not 4096");

            if (chm.HeaderSection1.FirstPMGL != 0)
                System.Message(fh, "WARNING; first PMGL chunk is not zero");

            if (chm.HeaderSection1.FirstPMGL > chm.HeaderSection1.LastPMGL)
            {
                Console.WriteLine("First pmgl chunk is after last pmgl chunk");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            if (chm.HeaderSection1.IndexRoot != 0xFFFFFFFF && chm.HeaderSection1.IndexRoot >= chm.HeaderSection1.NumChunks)
            {
                Console.WriteLine("IndexRoot outside valid range");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            #endregion

            // If we are doing a quick read, stop here!
            if (!entire)
                return Error.MSPACK_ERR_OK;

            // Seek to the first PMGL chunk, and reduce the number of chunks to read
            if ((x = chm.HeaderSection1.FirstPMGL) != 0)
            {
                if (!System.Seek(fh, x * chm.HeaderSection1.ChunkSize, SeekMode.MSPACK_SYS_SEEK_CUR))
                    return Error.MSPACK_ERR_SEEK;
            }

            numChunks = chm.HeaderSection1.LastPMGL - x + 1;

            byte[] chunk = new byte[chm.HeaderSection1.ChunkSize];

            // Read and process all chunks from FirstPMGL to LastPMGL
            errors = 0;
            while (numChunks-- != 0)
            {
                // Read next chunk
                if (System.Read(fh, chunk, 0, (int)chm.HeaderSection1.ChunkSize) != (int)chm.HeaderSection1.ChunkSize)
                    return Error.MSPACK_ERR_READ;

                // Create a new header based on that
                err = _PMGHeader.Create(buf, out _PMGHeader pmgHeader);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                // Process only directory (PMGL) chunks
                if (!pmgHeader.IsPMGL())
                    continue;

                if (pmgHeader.QuickRefSize < 2)
                    System.Message(fh, "WARNING; PMGL quickref area is too small");

                if (pmgHeader.QuickRefSize > chm.HeaderSection1.ChunkSize - pmgHeader.PMGLEntries)
                    System.Message(fh, "WARNING; PMGL quickref area is too large");

                p = (int)pmgHeader.PMGLEntries;
                end = (int)(chm.HeaderSection1.ChunkSize - 2);
                numEntries = BitConverter.ToUInt16(chunk, end);

                while (numEntries-- != 0)
                {
                    // READ_ENCINT(nameLen)
                    nameLen = 0;
                    do
                    {
                        if (p >= end)
                        {
                            if (numEntries >= 0)
                            {
                                Console.WriteLine("Chunk ended before all entries could be read");
                                errors++;
                            }
                        }

                        nameLen = (uint)((nameLen << 7) | (chunk[p] & 0x7F));
                    } while ((chunk[p++] & 0x80) != 0);

                    if (nameLen > (uint)(end - p))
                    {
                        if (numEntries >= 0)
                        {
                            Console.WriteLine("Chunk ended before all entries could be read");
                            errors++;
                        }
                    }

                    name = p; p += (int)nameLen;

                    // READ_ENCINT(section)
                    section = 0;
                    do
                    {
                        if (p >= end)
                        {
                            if (numEntries >= 0)
                            {
                                Console.WriteLine("Chunk ended before all entries could be read");
                                errors++;
                            }
                        }

                        section = (uint)((section << 7) | (chunk[p] & 0x7F));
                    } while ((chunk[p++] & 0x80) != 0);

                    // READ_ENCINT(offset)
                    offset = 0;
                    do
                    {
                        if (p >= end)
                        {
                            if (numEntries >= 0)
                            {
                                Console.WriteLine("Chunk ended before all entries could be read");
                                errors++;
                            }
                        }

                        offset = (offset << 7) | (chunk[p] & 0x7F);
                    } while ((chunk[p++] & 0x80) != 0);

                    // READ_ENCINT(length)
                    length = 0;
                    do
                    {
                        if (p >= end)
                        {
                            if (numEntries >= 0)
                            {
                                Console.WriteLine("Chunk ended before all entries could be read");
                                errors++;
                            }
                        }

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
                        System.Message(fh, $"Invalid section number '{section}'.");
                        continue;
                    }

                    fi = new DecompressFile()
                    {
                        Next = null,
                        Filename = Encoding.UTF8.GetString(chunk, name, (int)nameLen) + "\0",
                        Section = (section == 0) ? chm.Sec0 as Section : chm.Sec1 as Section,
                        Offset = offset,
                        Length = length,
                    };

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
            }

            return (errors > 0) ? Error.MSPACK_ERR_DATAFORMAT : Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Reads one entry out of the reset table. Also reads the uncompressed
        /// data length. Writes these to offsetPointer and lengthPointer respectively.
        /// Returns non-zero for success, zero for failure.
        /// </summary>
        private bool ReadResetTable(MSCompressedSection sec, uint entry, out long lengthPointer, out long offsetPointer)
        {
            lengthPointer = 0; offsetPointer = 0;
            byte[] data;

            // Do we have a ResetTable file?
            DecompressFile resetTable = null;
            Error err = FindSysFile(sec, ref resetTable, ResetTableName);
            if (err != Error.MSPACK_ERR_OK)
                return false;

            sec.ResetTable = resetTable;

            // Read ResetTable file
            if (sec.ResetTable.Length < _LZXResetTable.Size)
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

            if ((data = ReadSysFile(sec.ResetTable)) == null)
            {
                Console.WriteLine("can't read reset table");
                return false;
            }

            // Create a new reset data based on that
            err = _LZXResetTable.Create(data, out _LZXResetTable lzxResetTable);
            if (err != Error.MSPACK_ERR_OK)
                return false;

            // Check sanity of reset table
            if (lzxResetTable.FrameLength != LZX.LZX_FRAME_SIZE)
            {
                Console.WriteLine("Bad reset table frame length");
                return false;
            }

            // Get the uncompressed length of the LZX stream
            if ((lengthPointer = lzxResetTable.UncompressedLength) == 0)
                return false;

            uint pos = lzxResetTable.TableOffset + (entry * lzxResetTable.EntrySize);

            // Ensure reset table entry for this offset exists
            if (entry < lzxResetTable.NumEntries && pos <= (sec.ResetTable.Length - lzxResetTable.EntrySize))
            {
                switch (lzxResetTable.EntrySize)
                {
                    case 4:
                        offsetPointer = BitConverter.ToUInt32(data, (int)pos);
                        err = Error.MSPACK_ERR_OK;
                        break;
                    case 8:
                        offsetPointer = BitConverter.ToInt64(data, (int)pos);
                        break;
                    default:
                        Console.WriteLine("Reset table entry size neither 4 nor 8");
                        err = Error.MSPACK_ERR_ARGS;
                        break;
                }
            }
            else
            {
                Console.WriteLine("Bad reset interval");
                err = Error.MSPACK_ERR_ARGS;
            }

            // Return success
            return (err == Error.MSPACK_ERR_OK);
        }

        /// <summary>
        /// Reads the uncompressed data length from the spaninfo file.
        /// Returns zero for success or a non-zero error code for failure.
        /// </summary>
        private Error ReadSpanInfo(MSCompressedSection sec, out long length_ptr)
        {
            length_ptr = 0;

            // Find SpanInfo file
            DecompressFile spanInfo = null;
            Error err = FindSysFile(sec, ref spanInfo, SpanInfoName);
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
            if ((data = ReadSysFile(sec.SpanInfo)) == null)
            {
                Console.WriteLine("can't read SpanInfo file");
                return Error;
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

        /// <summary>
        /// Allocates memory for a section 0 (uncompressed) file and reads it into memory.
        /// </summary>
        private byte[] ReadSysFile(DecompressFile file)
        {
            if (file == null || file.Section == null || (file.Section.ID != 0))
            {
                Error = Error.MSPACK_ERR_DATAFORMAT;
                return null;
            }

            int len = (int)file.Length;
            byte[] data = new byte[len];

            if (System.Seek(State.InputFileHandle, file.Section.Header.Sec0.Offset + file.Offset, SeekMode.MSPACK_SYS_SEEK_START))
            {
                Error = Error.MSPACK_ERR_SEEK;
                return null;
            }

            if (System.Read(State.InputFileHandle, data, 0, len) != len)
            {
                Error = Error.MSPACK_ERR_READ;
                return null;
            }

            return data;
        }

        /// <summary>
        /// The real implementation of chmd_open() and chmd_fast_open(). It simply
        /// passes the "entire" parameter to chmd_read_headers(), which will then
        /// either read all headers, or a bare mininum.
        /// </summary>
        private CHM RealOpen(string filename, bool entire)
        {
            FileStream fh = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_READ);
            if (fh != null)
            {
                CHM chm = new CHM() { Filename = filename };
                Error error = ReadHeaders(fh, chm, entire);
                if (error != Error.MSPACK_ERR_OK)
                {
                    // If the error is DATAFORMAT, and there are some results, return
                    // partial results with a warning, rather than nothing
                    if (error == Error.MSPACK_ERR_DATAFORMAT && (chm.Files != null || chm.SysFiles != null))
                    {
                        System.Message(fh, "WARNING; contents are corrupt");
                        error = Error.MSPACK_ERR_OK;
                    }
                    else
                    {
                        Close(chm);
                        chm = null;
                    }
                }

                Error = error;
                System.Close(fh);
                return chm;
            }
            else
            {
                Error = Error.MSPACK_ERR_OPEN;
                return null;
            }
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
        private int SearchChunk(CHM chm, byte[] chunk, string filename, ref int result, ref int resultEnd)
        {
            int p;
            uint nameLen;
            uint left, right, midpoint, entries;
            int cmp;

            // Create a new header based on the chunk
            Error err = _PMGHeader.Create(chunk, out _PMGHeader pmgHeader);
            if (err != Error.MSPACK_ERR_OK)
                return -1;

            // TODO: Figure out what `entriesOff` does. It feels like it's being used wrong

            // PMGL chunk or PMGI chunk? (note: read_chunk() has already
            // checked the rest of the characters in the chunk signature)
            if (pmgHeader.IsPMGL())
                entries = pmgHeader.PMGLEntries;
            else
                entries = pmgHeader.PMGIEntries;

            // Step 1: binary search first filename of each QR entry
            // - target filename == entry
            //   found file
            // - target filename < all entries
            //   file not found
            // - target filename > all entries
            //   proceed to step 2 using final entry
            // - target filename between two searched entries
            // Proceed to step 2
            uint qrSize = pmgHeader.QuickRefSize;
            int start = (int)(chm.HeaderSection1.ChunkSize - 2);
            int end = (int)(chm.HeaderSection1.ChunkSize - qrSize);
            ushort numEntries = BitConverter.ToUInt16(chunk, start);
            uint qrDensity = 1 + (uint)(1 << (int)chm.HeaderSection1.Density);
            uint qrEntries = (numEntries + qrDensity - 1) / qrDensity;

            if (numEntries == 0)
            {
                Console.Write("chunk has no entries");
                return -1;
            }

            if (qrSize > chm.HeaderSection1.ChunkSize)
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
                    p = (int)(entries + (midpoint != 0 ? BitConverter.ToUInt16(chunk, (int)(start - (midpoint << 1))) : 0));

                    // READ_ENCINT(nameLen)
                    nameLen = 0;
                    do
                    {
                        if (p >= end)
                        {
                            Console.WriteLine("reached end of chunk data while searching");
                            return -1;
                        }

                        nameLen = (uint)((nameLen << 7) | (chunk[p] & 0x7F));
                    } while ((chunk[p++] & 0x80) != 0);

                    if (nameLen > (uint)(end - p))
                    {
                        Console.WriteLine("reached end of chunk data while searching");
                        return -1;
                    }

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
                p = (int)(entries + (midpoint != 0 ? BitConverter.ToUInt16(chunk, (int)(start - (midpoint << 1))) : 0));
                numEntries -= (ushort)(midpoint * qrDensity);
                if (numEntries > qrDensity)
                    numEntries = (ushort)qrDensity;
            }
            else
            {
                p = (int)entries;
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
                    {
                        Console.WriteLine("reached end of chunk data while searching");
                        return -1;
                    }

                    nameLen = (uint)((nameLen << 7) | (chunk[p] & 0x7F));
                } while ((chunk[p++] & 0x80) != 0);

                if (nameLen > (uint)(end - p))
                {
                    Console.WriteLine("reached end of chunk data while searching");
                    return -1;
                }

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
                if (pmgHeader.IsPMGL())
                {
                    // Skip section, offset, and length
                    for (int i = 0; i < 3; i++)
                    {
                        // READ_ENCINT(R)
                        right = 0;
                        do
                        {
                            if (p >= end)
                            {
                                Console.WriteLine("reached end of chunk data while searching");
                                return -1;
                            }

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
                        {
                            Console.WriteLine("reached end of chunk data while searching");
                            return -1;
                        }

                        right = (uint)((right << 7) | (chunk[p] & 0x7F));
                    } while ((chunk[p++] & 0x80) != 0);
                }
            }

            // PMGL? not found. PMGI? maybe found
            return (pmgHeader.IsPMGL()) ? 0 : (result != 0 ? 1 : 0);
        }

        #endregion
    }
}
