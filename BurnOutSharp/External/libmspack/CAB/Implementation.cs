/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

/* Cabinet (.CAB) files are a form of file archive. Each cabinet contains
 * "folders", which are compressed spans of data. Each cabinet has
 * "files", whose metadata is in the cabinet header, but whose actual data
 * is stored compressed in one of the "folders". Cabinets can span more
 * than one physical file on disk, in which case they are a "cabinet set",
 * and usually the last folder of each cabinet extends into the next
 * cabinet.
 *
 * For a complete description of the format, see the MSDN site:
 *   http://msdn.microsoft.com/en-us/library/bb267310.aspx
 */

/* Notes on compliance with cabinet specification:
 *
 * One of the main changes between cabextract 0.6 and libmspack's cab
 * decompressor is the move from block-oriented decompression to
 * stream-oriented decompression.
 *
 * cabextract would read one data block from disk, decompress it with the
 * appropriate method, then write the decompressed data. The CAB
 * specification is specifically designed to work like this, as it ensures
 * compression matches do not span the maximum decompressed block size
 * limit of 32kb.
 *
 * However, the compression algorithms used are stream oriented, with
 * specific hacks added to them to enforce the "individual 32kb blocks"
 * rule in CABs. In other file formats, they do not have this limitation.
 *
 * In order to make more generalised decompressors, libmspack's CAB
 * decompressor has moved from being block-oriented to more stream
 * oriented. This also makes decompression slightly faster.
 *
 * However, this leads to incompliance with the CAB specification. The
 * CAB controller can no longer ensure each block of input given to the
 * decompressors is matched with their output. The "decompressed size" of
 * each individual block is thrown away.
 *
 * Each CAB block is supposed to be seen as individually compressed. This
 * means each consecutive data block can have completely different
 * "uncompressed" sizes, ranging from 1 to 32768 bytes. However, in
 * reality, all data blocks in a folder decompress to exactly 32768 bytes,
 * excepting the final block. 
 *
 * Given this situation, the decompression algorithms are designed to
 * realign their input bitstreams on 32768 output-byte boundaries, and
 * various other special cases have been made. libmspack will not
 * correctly decompress LZX or Quantum compressed folders where the blocks
 * do not follow this "32768 bytes until last block" pattern. It could be
 * implemented if needed, but hopefully this is not necessary -- it has
 * not been seen in over 3Gb of CAB archives.
 */

using System;
using System.Text;
using LibMSPackSharp.Compression;

namespace LibMSPackSharp.CAB
{
    public static class Implementation
    {
        #region Generic CAB Definitions

        #region Structure Offsets

        private const int cfhead_Signature = (0x00);
        private const int cfhead_CabinetSize = (0x08);
        private const int cfhead_FileOffset = (0x10);
        private const int cfhead_MinorVersion = (0x18);
        private const int cfhead_MajorVersion = (0x19);
        private const int cfhead_NumFolders = (0x1A);
        private const int cfhead_NumFiles = (0x1C);
        private const int cfhead_Flags = (0x1E);
        private const int cfhead_SetID = (0x20);
        private const int cfhead_CabinetIndex = (0x22);
        private const int cfhead_SIZEOF = (0x24);

        private const int cfheadext_HeaderReserved = (0x00);
        private const int cfheadext_FolderReserved = (0x02);
        private const int cfheadext_DataReserved = (0x03);
        private const int cfheadext_SIZEOF = (0x04);

        private const int cffold_DataOffset = (0x00);
        private const int cffold_NumBlocks = (0x04);
        private const int cffold_CompType = (0x06);
        private const int cffold_SIZEOF = (0x08);

        private const int cffile_UncompressedSize = (0x00);
        private const int cffile_FolderOffset = (0x04);
        private const int cffile_FolderIndex = (0x08);
        private const int cffile_Date = (0x0A);
        private const int cffile_Time = (0x0C);
        private const int cffile_Attribs = (0x0E);
        private const int cffile_SIZEOF = (0x10);

        private const int cfdata_CheckSum = (0x00);
        private const int cfdata_CompressedSize = (0x04);
        private const int cfdata_UncompressedSize = (0x06);
        private const int cfdata_SIZEOF = (0x08);

        #endregion

        // CAB data blocks are <= 32768 bytes in uncompressed form.Uncompressed
        // blocks have zero growth. MSZIP guarantees that it won't grow above
        // uncompressed size by more than 12 bytes.LZX guarantees it won't grow
        // more than 6144 bytes.Quantum has no documentation, but the largest
        // block seen in the wild is 337 bytes above uncompressed size.

        public const int CAB_BLOCKMAX = 32768;
        public const int CAB_INPUTMAX = CAB_BLOCKMAX + 6144;

        // input buffer needs to be CAB_INPUTMAX + 1 byte to allow for max-sized block
        // plus 1 trailer byte added by cabd_sys_read_block() for Quantum alignment.
        // 
        // When MSCABD_PARAM_SALVAGE is set, block size is not checked so can be
        // up to 65535 bytes, so max input buffer size needed is 65535 + 1

        public const int CAB_INPUTMAX_SALVAGE = 65535;
        public const int CAB_INPUTBUF = CAB_INPUTMAX_SALVAGE + 1;

        // There are no more than 65535 data blocks per folder, so a folder cannot
        // be more than 32768*65535 bytes in length.As files cannot span more than
        // one folder, this is also their max offset, length and offset+length limit.

        public const int CAB_FOLDERMAX = 65535;
        public const int CAB_LENGTHMAX = CAB_BLOCKMAX * CAB_FOLDERMAX;

        #endregion

        #region CABD_OPEN

        /// <summary>
        /// Opens a file and tries to read it as a cabinet file
        /// </summary>
        public static CabinetImpl Open(Decompressor d, string filename)
        {
            DecompressorImpl self = d as DecompressorImpl;
            CabinetImpl cab = null;

            if (self == null)
                return null;

            SystemImpl system = self.System;
            object fileHandle;
            if ((fileHandle = system.Open(system, filename, OpenMode.MSPACK_SYS_OPEN_READ)) != null)
            {
                cab = new CabinetImpl();
                cab.Filename = filename;
                Error error = ReadHeaders(system, fileHandle, cab, 0, self.Salvage, false);
                if (error != Error.MSPACK_ERR_OK)
                {
                    Close(self, cab);
                    cab = null;
                }

                self.Error = error;
                system.Close(fileHandle);
            }
            else
            {
                self.Error = Error.MSPACK_ERR_OPEN;
            }

            return cab;
        }

        #endregion

        #region CABD_CLOSE

        /// <summary>
        /// Frees all memory associated with a given Cabinet.
        /// </summary>
        public static void Close(Decompressor d, Cabinet origcab)
        {
            DecompressorImpl self = d as DecompressorImpl;

            FolderData dat, ndat;
            Cabinet cab, ncab;
            Folder fol, nfol;
            InternalFile fi, nfi;

            if (self == null)
                return;

            SystemImpl sys = self.System;

            self.Error = Error.MSPACK_ERR_OK;

            while (origcab != null)
            {
                // Free files
                for (fi = origcab.Files; fi != null; fi = nfi)
                {
                    nfi = fi.Next;
                    sys.Free(fi.Filename);
                    sys.Free(fi);
                }

                // Free folders
                for (fol = origcab.Folders; fol != null; fol = nfol)
                {
                    nfol = fol.Next;

                    // Free folder decompression state if it has been decompressed
                    if (self.State != null && (self.State.Folder == fol))
                    {
                        if (self.State.InputFileHandle != null)
                            sys.Close(self.State.InputFileHandle);

                        FreeDecompressionState(self);
                        sys.Free(self.State);
                        self.State = null;
                    }

                    // Free folder data segments
                    for (dat = (fol as FolderImpl).Data.Next; dat != null; dat = ndat)
                    {
                        ndat = dat.Next;
                        sys.Free(dat);
                    }

                    sys.Free(fol);
                }

                // Free predecessor cabinets (and the original cabinet's strings)
                for (cab = origcab; cab == null; cab = ncab)
                {
                    ncab = cab.PreviousCabinet;
                    sys.Free(cab.PreviousName);
                    sys.Free(cab.NextName);
                    sys.Free(cab.PreviousInfo);
                    sys.Free(cab.NextInfo);
                    if (cab != origcab)
                        sys.Free(cab);
                }

                // Free successor cabinets
                for (cab = origcab.NextCabinet; cab != null; cab = ncab)
                {
                    ncab = cab.NextCabinet;
                    sys.Free(cab.PreviousName);
                    sys.Free(cab.NextName);
                    sys.Free(cab.PreviousInfo);
                    sys.Free(cab.NextInfo);
                    sys.Free(cab);
                }

                // Free actual cabinet structure
                cab = origcab.Next;
                sys.Free(origcab);

                // Repeat full procedure again with the cab.Next pointer (if set)
                origcab = cab;
            }
        }

        #endregion

        #region CABD_READ_HEADERS

        /// <summary>
        /// Reads the cabinet file header, folder list and file list.
        /// Fills out a pre-existing Cabinet structure, allocates memory
        /// for folders and files as necessary
        /// </summary>
        public static Error ReadHeaders(SystemImpl sys, object fh, CabinetImpl cab, long offset, bool salvage, bool quiet)
        {
            int num_folders, num_files, folder_resv, i, x;
            Error err = Error.MSPACK_ERR_OK;
            FileFlags fidx;
            FolderImpl fol, linkfol = null;
            InternalFile file, linkfile = null;
            byte[] buf = new byte[64];

            // Initialise pointers
            if (cab == null)
                cab = new CabinetImpl();

            cab.Next = null;
            cab.Files = null;
            cab.Folders = null;
            cab.PreviousCabinet = cab.NextCabinet = null;
            cab.PreviousName = cab.NextName = null;
            cab.PreviousInfo = cab.NextInfo = null;

            cab.BaseOffset = offset;

            // Seek to CFHEADER
            if (!sys.Seek(fh, offset, SeekMode.MSPACK_SYS_SEEK_START))
                return Error.MSPACK_ERR_SEEK;

            // Read in the CFHEADER
            if (sys.Read(fh, buf, 0, cfhead_SIZEOF) != cfhead_SIZEOF)
                return Error.MSPACK_ERR_READ;

            // Check for "MSCF" signature
            if (BitConverter.ToUInt32(buf, cfhead_Signature) != 0x4643534D)
                return Error.MSPACK_ERR_SIGNATURE;

            // Some basic header fields
            cab.Length = BitConverter.ToUInt32(buf, cfhead_CabinetSize);
            cab.SetID = BitConverter.ToUInt16(buf, cfhead_SetID);
            cab.SetIndex = BitConverter.ToUInt16(buf, cfhead_CabinetIndex);

            // Get the number of folders
            num_folders = BitConverter.ToUInt16(buf, cfhead_NumFolders);
            if (num_folders == 0)
            {
                if (!quiet) sys.Message(fh, "no folders in cabinet.");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // Get the number of files
            num_files = BitConverter.ToUInt16(buf, cfhead_NumFiles);
            if (num_files == 0)
            {
                if (!quiet) sys.Message(fh, "no files in cabinet.");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            // Check cabinet version
            if ((buf[cfhead_MajorVersion] != 1) && (buf[cfhead_MinorVersion] != 3))
            {
                if (!quiet) sys.Message(fh, "WARNING; cabinet version is not 1.3");
            }

            // Read the reserved-sizes part of header, if present
            cab.Flags = (HeaderFlags)BitConverter.ToUInt16(buf, cfhead_Flags);

            if (cab.Flags.HasFlag(HeaderFlags.MSCAB_HDR_RESV))
            {
                if (sys.Read(fh, buf, 0, cfheadext_SIZEOF) != cfheadext_SIZEOF)
                    return Error.MSPACK_ERR_READ;

                cab.HeaderResv = BitConverter.ToUInt16(buf, cfheadext_HeaderReserved);
                folder_resv = buf[cfheadext_FolderReserved];
                cab.BlockResverved = buf[cfheadext_DataReserved];

                if (cab.HeaderResv > 60000)
                {
                    if (!quiet) sys.Message(fh, "WARNING; reserved header > 60000.");
                }

                // Skip the reserved header
                if (cab.HeaderResv != 0)
                {
                    if (!sys.Seek(fh, cab.HeaderResv, SeekMode.MSPACK_SYS_SEEK_CUR))
                        return Error.MSPACK_ERR_SEEK;
                }
            }
            else
            {
                cab.HeaderResv = 0;
                folder_resv = 0;
                cab.BlockResverved = 0;
            }

            // Read name and info of preceeding cabinet in set, if present
            if (cab.Flags.HasFlag(HeaderFlags.MSCAB_HDR_PREVCAB))
            {
                cab.PreviousName = ReadString(sys, fh, false, ref err);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                cab.PreviousInfo = ReadString(sys, fh, true, ref err);
                if (err != Error.MSPACK_ERR_OK)
                    return err;
            }

            // Read name and info of next cabinet in set, if present
            if (cab.Flags.HasFlag(HeaderFlags.MSCAB_HDR_NEXTCAB))
            {
                cab.NextName = ReadString(sys, fh, false, ref err);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                cab.NextInfo = ReadString(sys, fh, true, ref err);
                if (err != Error.MSPACK_ERR_OK)
                    return err;
            }

            // Read folders
            for (i = 0; i < num_folders; i++)
            {
                if (sys.Read(fh, buf, 0, cffold_SIZEOF) != cffold_SIZEOF)
                    return Error.MSPACK_ERR_READ;

                if (folder_resv != 0)
                {
                    if (!sys.Seek(fh, folder_resv, SeekMode.MSPACK_SYS_SEEK_CUR))
                        return Error.MSPACK_ERR_SEEK;
                }

                fol = new FolderImpl();

                fol.Next = null;
                fol.CompressionType = (CompressionType)BitConverter.ToUInt16(buf, cffold_CompType);
                fol.NumBlocks = BitConverter.ToUInt16(buf, cffold_NumBlocks);
                fol.MergePrev = null;
                fol.MergeNext = null;

                fol.Data = new FolderData();
                fol.Data.Next = null;
                fol.Data.Cab = cab;
                fol.Data.Offset = offset + (int)(BitConverter.ToUInt32(buf, cffold_DataOffset));

                // Link folder into list of folders
                if (linkfol == null)
                    cab.Folders = fol;
                else
                    linkfol.Next = fol;

                linkfol = fol;
            }

            // Read files
            for (i = 0; i < num_files; i++)
            {
                if (sys.Read(fh, buf, 0, cffile_SIZEOF) != cffile_SIZEOF)
                    return Error.MSPACK_ERR_READ;

                file = new InternalFile();

                file.Next = null;
                file.Length = BitConverter.ToUInt32(buf, cffile_UncompressedSize);
                file.Attributes = (FileAttributes)BitConverter.ToUInt16(buf, cffile_Attribs);
                file.Offset = BitConverter.ToUInt32(buf, cffile_FolderOffset);

                // Set folder pointer
                fidx = (FileFlags)BitConverter.ToUInt16(buf, cffile_FolderIndex);
                if (fidx < FileFlags.CONTINUED_FROM_PREV)
                {
                    // Normal folder index; count up to the correct folder
                    if ((int)fidx < num_folders)
                    {
                        Folder ifol = cab.Folders;
                        while (fidx-- != 0)
                        {
                            if (ifol != null)
                                ifol = ifol.Next;
                        }

                        file.Folder = ifol;
                    }
                    else
                    {
                        Console.WriteLine("invalid folder index");
                        file.Folder = null;
                    }
                }
                else
                {
                    // Either CONTINUED_TO_NEXT, CONTINUED_FROM_PREV or CONTINUED_PREV_AND_NEXT
                    if (fidx == FileFlags.CONTINUED_TO_NEXT || fidx == FileFlags.CONTINUED_PREV_AND_NEXT)
                    {
                        // Get last folder
                        Folder ifol = cab.Folders;
                        while (ifol.Next != null)
                        {
                            ifol = ifol.Next;
                        }

                        file.Folder = ifol;

                        // Set "merge next" pointer
                        fol = ifol as FolderImpl;
                        if (fol.MergeNext == null)
                            fol.MergeNext = file;
                    }

                    if (fidx == FileFlags.CONTINUED_FROM_PREV || fidx == FileFlags.CONTINUED_PREV_AND_NEXT)
                    {
                        // Get first folder
                        file.Folder = cab.Folders;

                        // Set "merge prev" pointer
                        fol = file.Folder as FolderImpl;
                        if (fol.MergePrev == null)
                            fol.MergePrev = file;
                    }
                }

                // Get time
                x = BitConverter.ToUInt16(buf, cffile_Time);
                file.LastModifiedTimeHour = (byte)(x >> 11);
                file.LastModifiedTimeMinute = (byte)((x >> 5) & 0x3F);
                file.LastModifiedTimeSecond = (byte)((x << 1) & 0x3E);

                // Get date
                x = BitConverter.ToUInt16(buf, cffile_Date);
                file.LastModifiedDateDay = (byte)(x & 0x1F);
                file.LastModifiedDateMonth = (byte)((x >> 5) & 0xF);
                file.LastModifiedDateYear = (x >> 9) + 1980;

                // Get filename
                file.Filename = ReadString(sys, fh, false, ref err);

                // If folder index or filename are bad, either skip it or fail
                if (err != Error.MSPACK_ERR_OK || file.Folder == null)
                {
                    sys.Free(file.Filename);
                    sys.Free(file);
                    if (salvage)
                        continue;

                    return err != Error.MSPACK_ERR_OK ? err : Error.MSPACK_ERR_DATAFORMAT;
                }

                // Link file entry into file list
                if (linkfile == null)
                    cab.Files = file;
                else
                    linkfile.Next = file;

                linkfile = file;
            }

            if (cab.Files == null)
            {
                // We never actually added any files to the file list.  Something went wrong.
                // The file header may have been invalid */
                Console.WriteLine($"No files found, even though header claimed to have {num_files} files");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            return Error.MSPACK_ERR_OK;
        }

        private static string ReadString(SystemImpl sys, object fh, bool permitEmpty, ref Error error)
        {
            long position = sys.Tell(fh);
            byte[] buf = new byte[256];
            int len, i;

            // Read up to 256 bytes
            if ((len = sys.Read(fh, buf, 0, 256)) <= 0)
            {
                error = Error.MSPACK_ERR_READ;
                return null;
            }

            // Search for a null terminator in the buffer
            bool ok = false;
            for (i = 0; i < len; i++)
            {
                if (buf[i] == 0x00)
                {
                    ok = true;
                    break;
                }
            }

            // Optionally reject empty strings
            if (i == 0 && !permitEmpty)
                ok = false;

            if (!ok)
            {
                error = Error.MSPACK_ERR_DATAFORMAT;
                return null;
            }

            len = i + 1;

            // Set the data stream to just after the string and return
            if (!sys.Seek(fh, position + len, SeekMode.MSPACK_SYS_SEEK_START))
            {
                error = Error.MSPACK_ERR_SEEK;
                return null;
            }

            error = Error.MSPACK_ERR_OK;
            return Encoding.ASCII.GetString(buf, 0, len);
        }

        #endregion

        #region CABD_SEARCH, CABD_FIND

        /// <summary>
        /// Opens a file, finds its extent, allocates a search buffer,
        /// then reads through the whole file looking for possible cabinet headers.
        /// If it finds any, it tries to read them as real cabinets. Returns a linked
        /// list of results
        /// </summary>
        public static Cabinet Search(Decompressor d, string filename)
        {
            DecompressorImpl self = d as DecompressorImpl;

            if (self == null)
                return null;

            SystemImpl sys = self.System;

            // Allocate a search buffer
            byte[] search_buf = sys.Alloc(sys, self.SearchBufferSize);
            if (search_buf == null)
            {
                self.Error = Error.MSPACK_ERR_NOMEMORY;
                return null;
            }

            // Open file and get its full file length
            object fh; CabinetImpl cab = null;
            if ((fh = sys.Open(sys, filename, OpenMode.MSPACK_SYS_OPEN_READ)) != null)
            {
                long firstlen = 0;
                if ((self.Error = SystemImpl.GetFileLength(sys, fh, out long filelen)) == Error.MSPACK_ERR_OK)
                    self.Error = Find(self, search_buf, fh, filename, filelen, ref firstlen, out cab);

                // Truncated / extraneous data warning:
                if (firstlen != 0 && (firstlen != filelen) && (cab == null || cab.BaseOffset == 0))
                {
                    if (firstlen < filelen)
                        sys.Message(fh, $"WARNING; possible {filelen - firstlen} extra bytes at end of file.");
                    else
                        sys.Message(fh, $"WARNING; file possibly truncated by {firstlen - filelen} bytes.");
                }

                sys.Close(fh);
            }
            else
            {
                self.Error = Error.MSPACK_ERR_OPEN;
            }

            // Free the search buffer
            sys.Free(search_buf);

            return cab;
        }

        /// <summary>
        /// The inner loop of <see cref="Search(Decompressor, string)"/>, to make it easier to
        /// break out of the loop and be sure that all resources are freed
        /// </summary>
        public static Error Find(DecompressorImpl self, byte[] buf, object fh, string filename, long flen, ref long firstlen, out CabinetImpl firstcab)
        {
            firstcab = null;
            CabinetImpl cab, link = null;
            long caboff, offset, length;
            SystemImpl sys = self.System;
            long p, pend;
            byte state = 0;
            uint cablen_u32 = 0, foffset_u32 = 0;
            int false_cabs = 0;

            // Search through the full file length
            for (offset = 0; offset < flen; offset += length)
            {
                // Search length is either the full length of the search buffer, or the
                // amount of data remaining to the end of the file, whichever is less.
                length = flen - offset;
                if (length > self.SearchBufferSize)
                    length = self.SearchBufferSize;

                // Fill the search buffer with data from disk
                if (sys.Read(fh, buf, 0, (int)length) != (int)length)
                    return Error.MSPACK_ERR_READ;

                // FAQ avoidance strategy
                if (offset == 0 && BitConverter.ToUInt32(buf, 0) == 0x28635349)
                    sys.Message(fh, "WARNING; found InstallShield header. Use unshield (https://github.com/twogood/unshield) to unpack this file");

                // Read through the entire buffer.
                for (p = 0, pend = length; p < pend;)
                {
                    switch (state)
                    {
                        // Starting state
                        case 0:
                            // We spend most of our time in this while loop, looking for
                            // a leading 'M' of the 'MSCF' signature
                            while (p < pend && buf[p] != 0x4D)
                            {
                                p++;
                            }

                            // If we found that 'M', advance state
                            if (p++ < pend)
                                state = 1;

                            break;

                        // Verify that the next 3 bytes are 'S', 'C' and 'F'
                        case 1:
                            state = (byte)(buf[p++] == 0x53 ? 2 : 0);
                            break;
                        case 2:
                            state = (byte)(buf[p++] == 0x43 ? 3 : 0);
                            break;
                        case 3:
                            state = (byte)(buf[p++] == 0x46 ? 4 : 0);
                            break;

                        // We don't care about bytes 4-7 (see default: for action)

                        // Bytes 8-11 are the overall length of the cabinet
                        case 8:
                            cablen_u32 = buf[p++];
                            state++;
                            break;
                        case 9:
                            cablen_u32 |= (uint)buf[p++] << 8;
                            state++;
                            break;
                        case 10:
                            cablen_u32 |= (uint)buf[p++] << 16;
                            state++;
                            break;
                        case 11:
                            cablen_u32 |= (uint)buf[p++] << 24;
                            state++;
                            break;

                        // We don't care about bytes 12-15 (see default: for action)

                        // Bytes 16-19 are the offset within the cabinet of the filedata */
                        case 16:
                            foffset_u32 = buf[p++];
                            state++;
                            break;
                        case 17:
                            foffset_u32 |= (uint)buf[p++] << 8;
                            state++;
                            break;
                        case 18:
                            foffset_u32 |= (uint)buf[p++] << 16;
                            state++;
                            break;
                        case 19:
                            foffset_u32 |= (uint)buf[p++] << 24;

                            // Now we have recieved 20 bytes of potential cab header. work out
                            // the offset in the file of this potential cabinet
                            caboff = offset + p - 20;

                            // Should reading cabinet fail, restart search just after 'MSCF'
                            offset = caboff + 4;

                            // Vapture the "length of cabinet" field if there is a cabinet at
                            // offset 0 in the file, regardless of whether the cabinet can be
                            // read correctly or not
                            if (caboff == 0)
                                firstlen = cablen_u32;

                            // Check that the files offset is less than the alleged length of
                            // the cabinet, and that the offset + the alleged length are
                            // 'roughly' within the end of overall file length. In salvage
                            // mode, don't check the alleged length, allow it to be garbage */
                            if ((foffset_u32 < cablen_u32) &&
                                ((caboff + foffset_u32) < (flen + 32)) &&
                                (((caboff + cablen_u32) < (flen + 32)) || self.Salvage))
                            {
                                // Likely cabinet found -- try reading it
                                cab = new CabinetImpl();
                                cab.Filename = filename;

                                if (ReadHeaders(sys, fh, cab, caboff, self.Salvage, quiet: true) != Error.MSPACK_ERR_OK)
                                {
                                    // Destroy the failed cabinet
                                    Close(self, cab);
                                    false_cabs++;
                                }
                                else
                                {
                                    // Cabinet read correctly!

                                    // Link the cab into the list
                                    if (link == null)
                                        firstcab = cab;
                                    else
                                        link.Next = cab;

                                    link = cab;

                                    // Cause the search to restart after this cab's data.
                                    offset = caboff + cablen_u32;
                                }
                            }

                            // Restart search
                            if (offset >= flen)
                                return Error.MSPACK_ERR_OK;

                            if (!sys.Seek(fh, offset, SeekMode.MSPACK_SYS_SEEK_START))
                                return Error.MSPACK_ERR_SEEK;

                            length = 0;
                            p = pend;
                            state = 0;
                            break;

                        // For bytes 4-7 and 12-15, just advance state/pointer
                        default:
                            p++;
                            state++;
                            break;
                    }
                }
            }

            if (false_cabs != 0)
                Console.WriteLine($"{false_cabs} false cabinets found");

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region CABD_MERGE, CABD_PREPEND, CABD_APPEND

        /// <see cref="Merge"/>
        public static Error Prepend(Decompressor d, Cabinet cab, Cabinet prevcab)
        {
            return Merge(d, prevcab, cab);
        }

        /// <see cref="Merge"/>
        public static Error Append(Decompressor d, Cabinet cab, Cabinet nextcab)
        {
            return Merge(d, cab, nextcab);
        }

        /// <summary>
        /// Joins cabinets together, also merges split folders between these two
        /// cabinets only. This includes freeing the duplicate folder and file(s)
        /// and allocating a further mscabd_folder_data structure to append to the
        /// merged folder's data parts list.
        /// </summary>
        public static Error Merge(Decompressor d, Cabinet lcab, Cabinet rcab)
        {
            DecompressorImpl self = d as DecompressorImpl;

            FolderData data, ndata;
            FolderImpl lfol, rfol;
            InternalFile fi, rfi, lfi;

            if (self == null)
                return Error.MSPACK_ERR_ARGS;

            SystemImpl sys = self.System;

            // Basic args check
            if (lcab == null || rcab == null || (lcab == rcab))
            {
                Console.WriteLine("lcab null, rcab null or lcab = rcab");
                return self.Error = Error.MSPACK_ERR_ARGS;
            }

            // Check there's not already a cabinet attached
            if (lcab.NextCabinet != null || rcab.PreviousCabinet != null)
            {
                Console.WriteLine("cabs already joined");
                return self.Error = Error.MSPACK_ERR_ARGS;
            }

            // Do not create circular cabinet chains
            Cabinet cab;
            for (cab = lcab.PreviousCabinet; cab != null; cab = cab.PreviousCabinet)
            {
                if (cab == rcab)
                {
                    Console.WriteLine("circular!");
                    return self.Error = Error.MSPACK_ERR_ARGS;
                }
            }
            for (cab = rcab.NextCabinet; cab != null; cab = cab.NextCabinet)
            {
                if (cab == lcab)
                {
                    Console.WriteLine("circular!");
                    return self.Error = Error.MSPACK_ERR_ARGS;
                }
            }

            // Warn about odd set IDs or indices
            if (lcab.SetID != rcab.SetID)
                sys.Message(null, "WARNING; merged cabinets with differing Set IDs.");

            if (lcab.SetIndex > rcab.SetIndex)
                sys.Message(null, "WARNING; merged cabinets with odd order.");

            // Merging the last folder in lcab with the first folder in rcab
            lfol = lcab.Folders as FolderImpl;
            rfol = rcab.Folders as FolderImpl;
            while (lfol.Next != null)
            {
                lfol = lfol.Next as FolderImpl;
            }

            // Do we need to merge folders?
            if (lfol.MergeNext == null && rfol.MergePrev == null)
            {
                // No, at least one of the folders is not for merging

                // Attach cabs
                lcab.NextCabinet = rcab;
                rcab.PreviousCabinet = lcab;

                // Attach folders
                lfol.Next = rfol;

                // Attach files
                fi = lcab.Files;
                while (fi.Next != null)
                {
                    fi = fi.Next;
                }

                fi.Next = rcab.Files;
            }
            else
            {
                // Folder merge required - do the files match?
                if (!CanMergeFolders(sys, lfol, rfol))
                    return self.Error = Error.MSPACK_ERR_DATAFORMAT;

                // Allocate a new folder data structure
                data = new FolderData();

                // Attach cabs
                lcab.NextCabinet = rcab;
                rcab.PreviousCabinet = lcab;

                // Append rfol's data to lfol
                ndata = lfol.Data;
                while (ndata.Next != null)
                {
                    ndata = ndata.Next;
                }

                ndata.Next = data;
                data = rfol.Data;
                rfol.Data.Next = null;

                // lfol becomes rfol.
                // NOTE: special case, don't merge if rfol is merge prev and next,
                // rfol.MergeNext is going to be deleted, so keep lfol's version
                // instead
                lfol.NumBlocks += (ushort)(rfol.NumBlocks - 1);
                if ((rfol.MergeNext == null) || (rfol.MergeNext.Folder != rfol))
                    lfol.MergeNext = rfol.MergeNext;

                // Attach the rfol's folder (except the merge folder)
                while (lfol.Next != null)
                {
                    lfol = (FolderImpl)lfol.Next;
                }

                lfol.Next = rfol.Next;

                // Free disused merge folder
                sys.Free(rfol);

                // Attach rfol's files
                fi = lcab.Files;
                while (fi.Next != null)
                {
                    fi = fi.Next;
                }

                fi.Next = rcab.Files;

                // Delete all files from rfol's merge folder
                lfi = null;
                for (fi = lcab.Files; fi != null; fi = rfi)
                {
                    rfi = fi.Next;

                    // If file's folder matches the merge folder, unlink and free it
                    if (fi.Folder == rfol)
                    {
                        if (lfi != null)
                            lfi.Next = rfi;
                        else
                            lcab.Files = rfi;

                        sys.Free(fi.Filename);
                        sys.Free(fi);
                    }
                    else
                    {
                        lfi = fi;
                    }
                }
            }

            // All done! fix files and folders pointers in alsl cabs so they all
            // point to the same list
            for (cab = lcab.PreviousCabinet; cab != null; cab = cab.PreviousCabinet)
            {
                cab.Files = lcab.Files;
                cab.Folders = lcab.Folders;
            }

            for (cab = lcab.NextCabinet; cab != null; cab = cab.NextCabinet)
            {
                cab.Files = lcab.Files;
                cab.Folders = lcab.Folders;
            }

            return self.Error = Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Decides if two folders are OK to merge
        /// </summary>
        private static bool CanMergeFolders(SystemImpl sys, FolderImpl lfol, FolderImpl rfol)
        {
            InternalFile lfi, rfi, l, r;
            bool matching = true;

            // Check that both folders use the same compression method/settings
            if (lfol.CompressionType != rfol.CompressionType)
            {
                Console.WriteLine("folder merge: compression type mismatch");
                return false;
            }

            // Check there are not too many data blocks after merging
            if ((lfol.NumBlocks + rfol.NumBlocks) > CAB_FOLDERMAX)
            {
                Console.WriteLine("folder merge: too many data blocks in merged folders");
                return false;
            }

            if ((lfi = lfol.MergeNext) == null || (rfi = rfol.MergePrev) == null)
            {
                Console.WriteLine("folder merge: one cabinet has no files to merge");
                return false;
            }

            // For all files in lfol (which is the last folder in whichever cab and
            // only has files to merge), compare them to the files from rfol. They
            // should be identical in number and order. to verify this, check the
            // offset and length of each file. 
            for (l = lfi, r = rfi; l != null; l = l.Next, r = r.Next)
            {
                if (r == null || (l.Offset != r.Offset) || (l.Length != r.Length))
                {
                    matching = false;
                    break;
                }
            }

            if (matching)
                return true;

            // If rfol does not begin with an identical copy of the files in lfol, make
            // make a judgement call; if at least ONE file from lfol is in rfol, allow
            // the merge with a warning about missing files.
            matching = false;
            for (l = lfi; l != null; l = l.Next)
            {
                for (r = rfi; r != null; r = r.Next)
                {
                    if (l.Offset == r.Offset && l.Length == r.Length)
                        break;
                }

                if (r != null)
                    matching = true;
                else
                    sys.Message(null, $"WARNING; merged file {l.Filename} not listed in both cabinets");
            }

            return matching;
        }

        #endregion

        #region CABD_EXTRACT

        /// <summary>
        /// Extracts a file from a cabinet
        /// </summary>
        public static Error Extract(Decompressor d, InternalFile file, string filename)
        {
            DecompressorImpl self = d as DecompressorImpl;
            object fh;

            if (self == null)
                return Error.MSPACK_ERR_ARGS;
            if (file == null)
                return self.Error = Error.MSPACK_ERR_ARGS;

            SystemImpl sys = self.System;
            FolderImpl fol = file.Folder as FolderImpl;

            // If offset is beyond 2GB, nothing can be extracted
            if (file.Offset > CAB_LENGTHMAX)
                return self.Error = Error.MSPACK_ERR_DATAFORMAT;

            // If file claims to go beyond 2GB either error out,
            // or in salvage mode reduce file length so it fits 2GB limit
            long filelen = file.Length;
            if (filelen > CAB_LENGTHMAX || (file.Offset + filelen) > CAB_LENGTHMAX)
            {
                if (self.Salvage)
                    filelen = CAB_LENGTHMAX - file.Offset;
                else
                    return self.Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            // Extraction impossible if no folder, or folder needs predecessor
            if (fol == null || fol.MergePrev != null)
            {
                sys.Message(null, $"ERROR; file \"{file.Filename}\" cannot be extracted, cabinet set is incomplete");
                return self.Error = Error.MSPACK_ERR_DECRUNCH;
            }

            // If file goes beyond what can be decoded, given an error.
            // In salvage mode, don't assume block sizes, just try decoding
            if (!self.Salvage)
            {
                long maxlen = fol.NumBlocks * CAB_BLOCKMAX;
                if ((file.Offset + filelen) > maxlen)
                {
                    sys.Message(null, $"ERROR; file \"{file.Filename}\" cannot be extracted, cabinet set is incomplete");
                    return self.Error = Error.MSPACK_ERR_DECRUNCH;
                }
            }

            // Allocate generic decompression state
            if (self.State == null)
            {
                self.State = new DecompressState();
                self.State.Folder = null;
                self.State.Data = null;
                self.State.Sys = sys;
                self.State.Sys.Read = SysRead;
                self.State.Sys.Write = SysWrite;
                self.State.DecompressorState = null;
                self.State.InputFileHandle = null;
                self.State.InputCabinet = null;
            }

            // Do we need to change folder or reset the current folder?
            if ((self.State.Folder != fol) || (self.State.Offset > file.Offset) || self.State.DecompressorState == null)
            {
                // Free any existing decompressor
                FreeDecompressionState(self);

                // Do we need to open a new cab file?
                if (self.State.InputFileHandle == null || (fol.Data.Cab != self.State.InputCabinet))
                {
                    // Close previous file handle if from a different cab
                    if (self.State.InputFileHandle != null)
                        sys.Close(self.State.InputFileHandle);

                    self.State.InputCabinet = fol.Data.Cab;
                    self.State.InputFileHandle = sys.Open(sys, fol.Data.Cab.Filename, OpenMode.MSPACK_SYS_OPEN_READ);
                    if (self.State.InputFileHandle == null)
                        return self.Error = Error.MSPACK_ERR_OPEN;
                }

                // Seek to start of data blocks
                if (!sys.Seek(self.State.InputFileHandle, fol.Data.Offset, SeekMode.MSPACK_SYS_SEEK_START))
                    return self.Error = Error.MSPACK_ERR_SEEK;

                // Set up decompressor
                if (InitDecompressionState(self, fol.CompressionType) != Error.MSPACK_ERR_OK)
                    return self.Error;

                // Initialise new folder state
                self.State.Folder = fol;
                self.State.Data = fol.Data;
                self.State.Offset = 0;
                self.State.Block = 0;
                self.State.Outlen = 0;
                self.State.IPtr = self.State.IEnd = 0;

                // read_error lasts for the lifetime of a decompressor
                self.ReadError = Error.MSPACK_ERR_OK;
            }

            // Open file for output
            if ((fh = sys.Open(sys, filename, OpenMode.MSPACK_SYS_OPEN_WRITE)) == null)
                return self.Error = Error.MSPACK_ERR_OPEN;

            self.Error = Error.MSPACK_ERR_OK;

            // If file has more than 0 bytes
            if (filelen != 0)
            {
                long bytes;
                Error error;
                // Get to correct offset.
                // - use null fh to say 'no writing' to cabd_sys_write()
                // - if cabd_sys_read() has an error, it will set self.ReadError
                //   and pass back MSPACK_ERR_READ
                self.State.OutputFileHandle = null;
                if ((bytes = file.Offset - self.State.Offset) != 0)
                {
                    error = self.State.Decompress(self.State.DecompressorState, bytes);
                    self.Error = (error == Error.MSPACK_ERR_READ) ? self.ReadError : error;
                }

                // If getting to the correct offset was error free, unpack file
                if (self.Error == Error.MSPACK_ERR_OK)
                {
                    self.State.OutputFileHandle = fh;
                    error = self.State.Decompress(self.State.DecompressorState, filelen);
                    self.Error = (error == Error.MSPACK_ERR_READ) ? self.ReadError : error;
                }
            }

            // Close output file
            sys.Close(fh);
            self.State.OutputFileHandle = null;

            return self.Error;
        }

        #endregion

        #region CABD_INIT_DECOMP, CABD_FREE_DECOMP

        /// <summary>
        /// Initialises decompression state, according to which
        /// decompression method was used. relies on self.State.Folder being the same
        /// as when initialised.
        /// </summary>
        public static Error InitDecompressionState(DecompressorImpl self, CompressionType ct)
        {
            object fh = self;

            self.State.CompressionType = ct;

            switch (ct & CompressionType.COMPTYPE_MASK)
            {
                case CompressionType.COMPTYPE_NONE:
                    self.State.Decompress = NoneDecompress;
                    self.State.DecompressorState = NoneInit(self.State.Sys, fh, fh, self.BufferSize);
                    break;

                case CompressionType.COMPTYPE_MSZIP:
                    self.State.Decompress = MSZIP.Decompress;
                    self.State.DecompressorState = MSZIP.Init(self.State.Sys, fh, fh, self.BufferSize, self.FixMSZip);
                    break;

                case CompressionType.COMPTYPE_QUANTUM:
                    self.State.Decompress = QTM.Decompress;
                    self.State.DecompressorState = QTM.Init(self.State.Sys, fh, fh, ((ushort)ct >> 8) & 0x1f, self.BufferSize);
                    break;

                case CompressionType.COMPTYPE_LZX:
                    self.State.Decompress = LZX.Decompress;
                    self.State.DecompressorState = LZX.Init(self.State.Sys, fh, fh, ((ushort)ct >> 8) & 0x1f, 0, self.BufferSize, 0, false);
                    break;

                default:
                    return self.Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            return self.Error = (self.State.DecompressorState != null) ? Error.MSPACK_ERR_OK : Error.MSPACK_ERR_NOMEMORY;
        }

        /// <summary>
        /// Frees decompression state, according to which method was used.
        /// </summary>
        /// <param name="self"></param>
        public static void FreeDecompressionState(DecompressorImpl self)
        {
            if (self == null || self.State == null || self.State.DecompressorState == null)
                return;

            switch (self.State.CompressionType & CompressionType.COMPTYPE_MASK)
            {
                case CompressionType.COMPTYPE_NONE:
                    NoneFree(self.State.DecompressorState);
                    break;

                case CompressionType.COMPTYPE_MSZIP:
                    MSZIP.Free(self.State.DecompressorState);
                    break;

                case CompressionType.COMPTYPE_QUANTUM:
                    QTM.Free(self.State.DecompressorState);
                    break;

                case CompressionType.COMPTYPE_LZX:
                    LZX.Free(self.State.DecompressorState);
                    break;
            }

            self.State.Decompress = null;
            self.State.DecompressorState = null;
        }

        #endregion

        #region CABD_SYS_READ, CABD_SYS_WRITE

        /// <summary>
        /// The internal reader function which the decompressors
        /// use. will read data blocks (and merge split blocks) from the cabinet
        /// and serve the read bytes to the decompressors
        /// </summary>
        private static int SysRead(object file, byte[] buffer, int pointer, int bytes)
        {
            DecompressorImpl self = file as DecompressorImpl;
            SystemImpl sys = self.System;
            int avail, todo, outlen = 0;

            bool ignore_cksum = self.Salvage ||
              (self.FixMSZip &&
               ((self.State.CompressionType & CompressionType.COMPTYPE_MASK) == CompressionType.COMPTYPE_MSZIP));
            bool ignore_blocksize = self.Salvage;

            todo = bytes;
            while (todo > 0)
            {
                avail = self.State.IEnd - self.State.IPtr;

                // If out of input data, read a new block
                if (avail != 0)
                {
                    // Copy as many input bytes available as possible
                    if (avail > todo)
                        avail = todo;

                    sys.Copy(self.State.Input, self.State.IPtr, buffer, pointer, avail);
                    self.State.IPtr += avail;
                    pointer += avail;
                    todo -= avail;
                }
                else
                {
                    // Out of data, read a new block

                    // Check if we're out of input blocks, advance block counter
                    if (self.State.Block++ >= self.State.Folder.NumBlocks)
                    {
                        if (!self.Salvage)
                            self.ReadError = Error.MSPACK_ERR_DATAFORMAT;
                        else
                            Console.WriteLine("Ran out of CAB input blocks prematurely");

                        break;
                    }

                    // Read a block
                    self.ReadError = SysReadBlock(sys, self.State, ref outlen, ignore_cksum, ignore_blocksize);
                    if (self.ReadError != Error.MSPACK_ERR_OK)
                        return -1;

                    self.State.Outlen += outlen;

                    // Special Quantum hack -- trailer byte to allow the decompressor
                    // to realign itself. CAB Quantum blocks, unlike LZX blocks, can have
                    // anything from 0 to 4 trailing null bytes.
                    if ((self.State.CompressionType & CompressionType.COMPTYPE_MASK) == CompressionType.COMPTYPE_QUANTUM)
                        self.State.Input[self.State.IEnd++] = 0xFF;

                    // Is this the last block?
                    if (self.State.Block >= self.State.Folder.NumBlocks)
                    {
                        if ((self.State.CompressionType & CompressionType.COMPTYPE_MASK) == CompressionType.COMPTYPE_LZX)
                        {
                            // Special LZX hack -- on the last block, inform LZX of the
                            // size of the output data stream.
                            LZX.SetOutputLength(self.State.DecompressorState as LZXDStream, self.State.Outlen);
                        }
                    }
                }
            }

            return bytes - todo;
        }

        /// <summary>
        /// The internal writer function which the decompressors
        /// use. it either writes data to disk (self.State.OutputFileHandle) with the real
        /// sys.write() function, or does nothing with the data when
        /// self.State.OutputFileHandle == null. advances self.State.Offset
        /// </summary>
        private static int SysWrite(object file, byte[] buffer, int pointer, int bytes)
        {
            DecompressorImpl self = file as DecompressorImpl;
            self.State.Offset += (uint)bytes;
            if (self.State.OutputFileHandle != null)
                return self.System.Write(self.State.OutputFileHandle, buffer, pointer, bytes);

            return bytes;
        }

        #endregion

        #region CABD_SYS_READ_BLOCK

        /// <summary>
        /// Reads a whole data block from a cab file. The block may span more than
        /// one cab file, if it does then the fragments will be reassembled
        /// </summary>
        private static Error SysReadBlock(SystemImpl sys, DecompressState d, ref int output, bool ignore_cksum, bool ignore_blocksize)
        {
            byte[] hdr = new byte[cfdata_SIZEOF];
            uint cksum;
            int len, full_len;

            // Reset the input block pointer and end of block pointer
            d.IPtr = d.IEnd = 0;

            do
            {
                // Read the block header
                if (sys.Read(d.InputFileHandle, hdr, 0, cfdata_SIZEOF) != cfdata_SIZEOF)
                    return Error.MSPACK_ERR_READ;

                // Skip any reserved block headers
                if (d.Data.Cab.HeaderResv != 0 && !sys.Seek(d.InputFileHandle, d.Data.Cab.HeaderResv, SeekMode.MSPACK_SYS_SEEK_CUR))
                    return Error.MSPACK_ERR_SEEK;

                // Blocks must not be over CAB_INPUTMAX in size
                len = BitConverter.ToUInt16(hdr, cfdata_CompressedSize);
                full_len = (d.IEnd - d.IPtr) + len; // Include cab-spanning blocks
                if (full_len > CAB_INPUTMAX)
                {
                    Console.WriteLine($"block size {full_len} > CAB_INPUTMAX");

                    // In salvage mode, blocks can be 65535 bytes but no more than that
                    if (!ignore_blocksize || full_len > CAB_INPUTMAX_SALVAGE)
                        return Error.MSPACK_ERR_DATAFORMAT;
                }

                // Blocks must not expand to more than CAB_BLOCKMAX 
                if (BitConverter.ToUInt16(hdr, cfdata_UncompressedSize) > CAB_BLOCKMAX)
                {
                    Console.WriteLine("block size > CAB_BLOCKMAX");
                    if (!ignore_blocksize)
                        return Error.MSPACK_ERR_DATAFORMAT;
                }

                // Read the block data
                if (sys.Read(d.InputFileHandle, d.Input, d.IEnd, len) != len)
                    return Error.MSPACK_ERR_READ;

                // Perform checksum test on the block (if one is stored)
                if ((cksum = BitConverter.ToUInt32(hdr, cfdata_CheckSum)) != 0)
                {
                    uint sum2 = Checksum(d.Input, d.IEnd, (uint)len, 0);
                    if (Checksum(hdr, 4, 4, sum2) != cksum)
                    {
                        if (!ignore_cksum)
                            return Error.MSPACK_ERR_CHECKSUM;

                        sys.Message(d.InputFileHandle, "WARNING; bad block checksum found");
                    }
                }

                // Advance end of block pointer to include newly read data
                d.IEnd += len;

                // Uncompressed size == 0 means this block was part of a split block
                // and it continues as the first block of the next cabinet in the set.
                // Otherwise, this is the last part of the block, and no more block
                // reading needs to be done.

                // EXIT POINT OF LOOP -- uncompressed size != 0
                if ((output = BitConverter.ToUInt16(hdr, cfdata_UncompressedSize)) != 0)
                    return Error.MSPACK_ERR_OK;

                // Otherwise, advance to next cabinet

                // Close current file handle
                sys.Close(d.InputFileHandle);
                d.InputFileHandle = null;

                // Advance to next member in the cabinet set
                if ((d.Data = d.Data.Next) == null)
                {
                    sys.Message(d.InputFileHandle, "WARNING; ran out of cabinets in set. Are any missing?");
                    return Error.MSPACK_ERR_DATAFORMAT;
                }

                // Open next cab file
                d.InputCabinet = d.Data.Cab;
                if ((d.InputFileHandle = sys.Open(sys, d.InputCabinet.Filename, OpenMode.MSPACK_SYS_OPEN_READ)) == null)
                    return Error.MSPACK_ERR_OPEN;

                // Seek to start of data blocks
                if (!sys.Seek(d.InputFileHandle, d.Data.Offset, SeekMode.MSPACK_SYS_SEEK_START))
                    return Error.MSPACK_ERR_SEEK;
            } while (true);
        }

        private static uint Checksum(byte[] data, int pointer, uint bytes, uint cksum)
        {
            uint len, ul = 0;

            for (len = bytes >> 2; len-- != 0; pointer += 4)
            {
                cksum ^= (uint)((data[pointer + 0]) | (data[pointer + 1] << 8) | (data[pointer + 2] << 16) | (data[pointer + 3] << 24));
            }

            switch (bytes & 3)
            {
                case 3:
                    ul |= (uint)(data[pointer++] << 16);
                    ul |= (uint)(data[pointer++] << 8);
                    ul |= data[pointer];
                    break;

                case 2:
                    ul |= (uint)(data[pointer++] << 8);
                    ul |= data[pointer];
                    break;

                case 1:
                    ul |= data[pointer];
                    break;
            }

            cksum ^= ul;
            return cksum;
        }

        #endregion

        #region NONED_INIT, NONED_DECOMPRESS, NONED_FREE

        internal static NoneState NoneInit(SystemImpl sys, object input, object output, int bufsize)
        {
            NoneState state = new NoneState();
            byte[] buf = sys.Alloc(sys, bufsize);
            if (state != null && buf != null)
            {
                state.Sys = sys;
                state.Input = input;
                state.Output = output;
                state.Buffer = buf;
                state.BufferSize = bufsize;
            }
            else
            {
                sys.Free(buf);
                sys.Free(state);
                state = null;
            }

            return state;
        }

        internal static Error NoneDecompress(object s, long bytes)
        {
            NoneState state = (NoneState)s;
            if (state == null)
                return Error.MSPACK_ERR_ARGS;

            int run;
            while (bytes > 0)
            {
                run = (bytes > state.BufferSize) ? state.BufferSize : (int)bytes;

                if (state.Sys.Read(state.Input, state.Buffer, 0, run) != run)
                    return Error.MSPACK_ERR_READ;

                if (state.Sys.Write(state.Output, state.Buffer, 0, run) != run)
                    return Error.MSPACK_ERR_WRITE;

                bytes -= run;
            }
            return Error.MSPACK_ERR_OK;
        }

        internal static void NoneFree(object s)
        {
            NoneState state = s as NoneState;
            if (state != null)
            {
                SystemImpl sys = state.Sys;
                sys.Free(state.Buffer);
                sys.Free(state);
            }
        }

        #endregion

        #region CABD_PARAM

        /// <summary>
        /// Allows a parameter to be set
        /// </summary>
        public static Error Param(Decompressor d, Parameters param, int value)
        {
            DecompressorImpl self = d as DecompressorImpl;
            if (self == null)
                return Error.MSPACK_ERR_ARGS;

            switch (param)
            {
                case Parameters.MSCABD_PARAM_SEARCHBUF:
                    if (value < 4)
                        return Error.MSPACK_ERR_ARGS;

                    self.SearchBufferSize = value;
                    break;

                case Parameters.MSCABD_PARAM_FIXMSZIP:
                    self.FixMSZip = value != 0;
                    break;

                case Parameters.MSCABD_PARAM_DECOMPBUF:
                    if (value < 4)
                        return Error.MSPACK_ERR_ARGS;

                    self.BufferSize = value;
                    break;

                case Parameters.MSCABD_PARAM_SALVAGE:
                    self.Salvage = value != 0;
                    break;

                default:
                    return Error.MSPACK_ERR_ARGS;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region CABD_ERROR

        /// <summary>
        /// Returns the last error that occurred
        /// </summary>
        public static Error LastError(Decompressor d)
        {
            DecompressorImpl self = d as DecompressorImpl;
            return (self != null) ? self.Error : Error.MSPACK_ERR_ARGS;
        }

        #endregion
    }
}
