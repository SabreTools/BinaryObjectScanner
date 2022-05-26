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

namespace LibMSPackSharp.CAB
{
    /// <summary>
    /// A decompressor for .CAB (Microsoft Cabinet) files
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateCABDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyCABDecompressor(Decompressor)"/>
    public class Decompressor : BaseDecompressor
    {
        #region Fields

        public DecompressState State { get; set; }

        public int SearchBufferSize { get; set; }

        public bool FixMSZip { get; set; }

        public bool Salvage { get; set; }

        public Error ReadError { get; set; }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Opens a cabinet file and reads its contents.
        /// 
        /// If the file opened is a valid cabinet file, all headers will be read
        /// and a Cabinet structure will be returned, with a full list of
        /// folders and files.
        /// 
        /// In the case of an error occuring, null is returned and the error code
        /// is available from last_error().
        /// 
        /// The filename pointer should be considered "in use" until close() is
        /// called on the cabinet.
        /// </summary>
        /// <param name="filename">The filename of the cabinet file. This is passed directly to SystemImpl::open().</param>
        /// <returns>A pointer to a Cabinet structure, or null on failure</returns>
        /// <see cref="Close(Cabinet)"/>
        /// <see cref="Search(string)"/>
        /// <see cref="Error"/>
        public Cabinet Open(string filename)
        {
            FileStream fileHandle = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_READ);
            if (fileHandle == null)
            {
                Error = Error.MSPACK_ERR_OPEN;
                return null;
            }

            Cabinet cab = new Cabinet() { Filename = filename };
            Error error = ReadHeaders(fileHandle, cab, 0, Salvage, false);
            if (error != Error.MSPACK_ERR_OK)
            {
                Close(cab);
                cab = null;
            }

            Error = error;
            System.Close(fileHandle);
            return cab;
        }

        /// <summary>
        /// Closes a previously opened cabinet or cabinet set.
        /// 
        /// This closes a cabinet, all cabinets associated with it via the
        /// Cabinet::next, Cabinet::prevcab and
        /// Cabinet::nextcab pointers, and all folders and files. All
        /// memory used by these entities is freed.
        /// 
        /// The cabinet pointer is now invalid and cannot be used again. All
        /// Folder and InternalFile pointers from that cabinet or cabinet
        /// set are also now invalid, and cannot be used again.
        /// 
        /// If the cabinet pointer given was created using search(), it MUST be
        /// the cabinet pointer returned by search() and not one of the later
        /// cabinet pointers further along the Cabinet::next chain.
        /// 
        /// If extra cabinets have been added using append() or prepend(), these
        /// will all be freed, even if the cabinet pointer given is not the first
        /// cabinet in the set. Do NOT close() more than one cabinet in the set.
        /// 
        /// The Cabinet::filename is not freed by the library, as it is
        /// not allocated by the library. The caller should free this itself if
        /// necessary, before it is lost forever.
        /// </summary>
        /// <param name="cabinet">The cabinet to close</param>
        /// <see cref="Open(string)"/>
        /// <see cref="Search(string)"/>
        /// <see cref="Append(Cabinet, Cabinet)"/>
        /// <see cref="Prepend(Cabinet, Cabinet)"/>
        public void Close(Cabinet cabinet)
        {
            Error = Error.MSPACK_ERR_OK;
            while (cabinet != null)
            {
                // Free folders
                for (Folder fol = cabinet.Folders; fol != null; fol = fol.Next)
                {
                    // Free folder decompression state if it has been decompressed
                    if (State != null && (State.Folder == fol))
                    {
                        System.Close(State.InputFileHandle);
                        System.Close(State.OutputFileHandle);

                        FreeDecompressionState();
                        State = null;
                    }
                }

                // Repeat full procedure again with the cab.Next pointer (if set)
                cabinet = cabinet.Next;
            }
        }

        /// <summary>
        /// Searches a regular file for embedded cabinets.
        /// 
        /// This opens a normal file with the given filename and will search the
        /// entire file for embedded cabinet files
        /// 
        /// If any cabinets are found, the equivalent of open() is called on each
        /// potential cabinet file at the offset it was found. All successfully
        /// open()ed cabinets are kept in a list.
        /// 
        /// The first cabinet found will be returned directly as the result of
        /// this method. Any further cabinets found will be chained in a list
        /// using the Cabinet::next field.
        /// 
        /// In the case of an error occuring anywhere other than the simulated
        /// open(), null is returned and the error code is available from
        /// last_error().
        /// 
        /// If no error occurs, but no cabinets can be found in the file, null is
        /// returned and last_error() returns MSPACK_ERR_OK.
        /// 
        /// The filename pointer should be considered in use until close() is
        /// called on the cabinet.
        /// 
        /// close() should only be called on the result of search(), not on any
        /// subsequent cabinets in the Cabinet::next chain.
        /// </summary>
        /// <param name="filename">The filename of the file to search for cabinets. This is passed directly to SystemImpl.Open().</param>
        /// <returns>A pointer to a Cabinet structure, or null</returns>
        /// <see cref="Close(Cabinet)"/>
        /// <see cref="Open(string)"/>
        /// <see cref="Error"/>
        public Cabinet Search(string filename)
        {
            // Allocate a search buffer
            byte[] searchBuffer = new byte[SearchBufferSize];

            // Attempt to open the file
            FileStream fh = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_READ);
            if (fh == null)
            {
                Error = Error.MSPACK_ERR_OPEN;
                return null;
            }

            // Get its full file length
            long firstlen = 0;
            if ((Error = System.GetFileLength(fh, out long filelen)) != Error.MSPACK_ERR_OK)
                return null;

            // Attempt to to find the cabinet
            Error = Find(searchBuffer, fh, filename, filelen, ref firstlen, out Cabinet cab);

            // Truncated / extraneous data warning:
            if (firstlen != 0 && (firstlen != filelen) && (cab == null || cab.BaseOffset == 0))
            {
                if (firstlen < filelen)
                    System.Message(fh, $"WARNING; possible {filelen - firstlen} extra bytes at end of file.");
                else
                    System.Message(fh, $"WARNING; file possibly truncated by {firstlen - filelen} bytes.");
            }

            System.Close(fh);
            return cab;
        }

        /// <summary>
        /// Appends one Cabinet to another, forming or extending a cabinet
        /// set.
        /// 
        /// This will attempt to append one cabinet to another such that
        /// <tt>(cab->nextcab == nextcab) && (nextcab->prevcab == cab)</tt> and
        /// any folders split between the two cabinets are merged.
        /// 
        /// The cabinets MUST be part of a cabinet set -- a cabinet set is a
        /// cabinet that spans more than one physical cabinet file on disk -- and
        /// must be appropriately matched.
        /// 
        /// It can be determined if a cabinet has further parts to load by
        /// examining the Cabinet::flags field:
        /// 
        /// - if <tt>(flags & MSCAB_HDR_PREVCAB)</tt> is non-zero, there is a
        ///   predecessor cabinet to open() and prepend(). Its MS-DOS
        ///   case-insensitive filename is Cabinet::prevname
        /// - if <tt>(flags & MSCAB_HDR_NEXTCAB)</tt> is non-zero, there is a
        ///   successor cabinet to open() and append(). Its MS-DOS case-insensitive
        ///   filename is Cabinet::nextname
        /// 
        /// If the cabinets do not match, an error code will be returned. Neither
        /// cabinet has been altered, and both should be closed seperately.
        /// 
        /// Files and folders in a cabinet set are a single entity. All cabinets
        /// in a set use the same file list, which is updated as cabinets in the
        /// set are added. All pointers to Folder and InternalFile
        /// structures in either cabinet must be discarded and re-obtained after
        /// merging.
        /// </summary>
        /// <param name="cab">The cabinet which will be appended to, predecessor of nextcab </param>
        /// <param name="nextcab">The cabinet which will be appended, successor of cab </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="Prepend(Cabinet, Cabinet)"/>
        /// <see cref="Open(string)"/>
        /// <see cref="Close(Cabinet)"/>
        public Error Append(Cabinet cab, Cabinet nextcab) => Merge(cab, nextcab);

        /// <summary>
        /// Prepends one Cabinet to another, forming or extending a
        /// cabinet set.
        /// 
        /// This will attempt to prepend one cabinet to another, such that
        /// <tt>(cab->prevcab == prevcab) && (prevcab->nextcab == cab)</tt>. In
        /// all other respects, it is identical to append(). See append() for the
        /// full documentation.
        /// </summary>
        /// <param name="cab">The cabinet which will be prepended to, successor of prevcab</param>
        /// <param name="prevcab">The cabinet which will be prepended, predecessor of cab</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="Append(Cabinet, Cabinet)"/>
        /// <see cref="Open(string)"/>
        /// <see cref="Close(Cabinet)"/>
        public Error Prepend(Cabinet cab, Cabinet prevcab) => Merge(prevcab, cab);

        /// <summary>
        /// <summary>
        /// Extracts a file from a cabinet or cabinet set.
        /// 
        /// This extracts a compressed file in a cabinet and writes it to the given
        /// filename.
        /// 
        /// The MS-DOS filename of the file, InternalFile::filename, is NOT USED
        /// by extract(). The caller must examine this MS-DOS filename, copy and
        /// change it as necessary, create directories as necessary, and provide
        /// the correct filename as a parameter, which will be passed unchanged
        /// to the decompressor's SystemImpl::open()
        /// 
        /// If the file belongs to a split folder in a multi-part cabinet set,
        /// and not enough parts of the cabinet set have been loaded and appended
        /// or prepended, an error will be returned immediately.
        /// </summary>
        /// <param name="file">the file to be decompressed</param>
        /// <param name="filename">the filename of the file being written to</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Error Extract(InternalFile file, string filename)
        {
            if (file == null)
                return Error = Error.MSPACK_ERR_ARGS;

            // If offset is beyond 2GB, nothing can be extracted
            if (file.Header.FolderOffset > CAB_LENGTHMAX)
                return Error = Error.MSPACK_ERR_DATAFORMAT;

            // If file claims to go beyond 2GB either error out,
            // or in salvage mode reduce file length so it fits 2GB limit
            long filelen = file.Header.UncompressedSize;
            if (filelen > CAB_LENGTHMAX || (file.Header.FolderOffset + filelen) > CAB_LENGTHMAX)
            {
                if (Salvage)
                    filelen = CAB_LENGTHMAX - file.Header.FolderOffset;
                else
                    return Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            // Extraction impossible if no folder, or folder needs predecessor
            if (file.Folder == null || file.Folder.MergePrev != null)
            {
                System.Message(null, $"ERROR; file \"{file.Filename}\" cannot be extracted, cabinet set is incomplete");
                return Error = Error.MSPACK_ERR_DECRUNCH;
            }

            // If file goes beyond what can be decoded, given an error.
            // In salvage mode, don't assume block sizes, just try decoding
            if (!Salvage)
            {
                long maxlen = file.Folder.Header.NumBlocks * CAB_BLOCKMAX;
                if ((file.Header.FolderOffset + filelen) > maxlen)
                {
                    System.Message(null, $"ERROR; file \"{file.Filename}\" cannot be extracted, cabinet set is incomplete");
                    return Error = Error.MSPACK_ERR_DECRUNCH;
                }
            }

            // Close existing output file handle in case of error
            System.Close(State?.OutputFileHandle);

            // Allocate generic decompression state
            if (State == null)
            {
                State = new DecompressState()
                {
                    Folder = null,
                    Data = null,
                    System = System,
                    DecompressorState = null,
                    InputFileHandle = null,
                    InputCabinet = null,
                };

                State.System.Read = SysRead;
                State.System.Write = SysWrite;
            }

            // Do we need to change folder or reset the current folder?
            if (State.Folder != file.Folder || State.Offset > file.Header.FolderOffset || State.DecompressorState == null)
            {
                // Do we need to open a new cab file?
                if (State.InputFileHandle == null || file.Folder.Data.Cab != State.InputCabinet)
                {
                    // Close previous file handle if from a different cab
                    System.Close(State?.InputFileHandle);

                    State.InputCabinet = file.Folder.Data.Cab;
                    State.InputFileHandle = System.Open(file.Folder.Data.Cab.Filename, OpenMode.MSPACK_SYS_OPEN_READ);
                    if (State.InputFileHandle == null)
                        return Error = Error.MSPACK_ERR_OPEN;
                }

                // Seek to start of data blocks
                if (!System.Seek(State.InputFileHandle, file.Folder.Data.Offset, SeekMode.MSPACK_SYS_SEEK_START))
                    return Error = Error.MSPACK_ERR_SEEK;

                // Set up decompressor
                if (InitDecompressionState(file.Folder.Header.CompType) != Error.MSPACK_ERR_OK)
                    return Error;

                // Initialize new folder state
                State.Folder = file.Folder;
                State.Data = file.Folder.Data;
                State.Offset = 0;
                State.Block = 0;
                State.Outlen = 0;
                State.InputPointer = 0; // State.Input[0]
                State.InputEnd = 0; // State.Input[0]

                // ReadError lasts for the lifetime of a decompressor
                ReadError = Error.MSPACK_ERR_OK;
            }

            Error = Error.MSPACK_ERR_OK;

            // If file has more than 0 bytes
            if (filelen != 0)
            {
                // Set the output file handle to null
                State.OutputFileHandle = null;
                UpdateDecompressionState();

                // Get to correct offset.
                // - Use null fh to say 'no writing' to cabd_sys_write()
                // - If SysRead() has an error, it will set self.ReadError
                //   and pass back MSPACK_ERR_READ
                long bytes = file.Header.FolderOffset - State.Offset;
                if (bytes != 0)
                {
                    Error error = State.Decompress(State.DecompressorState, bytes);
                    Error = (error == Error.MSPACK_ERR_READ) ? ReadError : error;
                }

                // Open the output file handle
                State.OutputFileHandle = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_WRITE);
                if (State.OutputFileHandle == null)
                    return Error = Error.MSPACK_ERR_OPEN;

                // If getting to the correct offset was error free, unpack file
                if (Error == Error.MSPACK_ERR_OK)
                {
                    UpdateDecompressionState();
                    Error error = State.Decompress(State.DecompressorState, filelen);
                    Error = (error == Error.MSPACK_ERR_READ) ? ReadError : error;
                }
            }

            // Close output file
            System.Close(State.OutputFileHandle);
            State.OutputFileHandle = null;

            return Error;
        }

        /// <summary>
        /// Sets a CAB decompression engine parameter.
        /// 
        /// The following parameters are defined:
        /// - #MSCABD_PARAM_SEARCHBUF: How many bytes should be allocated as a
        ///   buffer when using search()? The minimum value is 4.  The default
        ///   value is 32768.
        /// - #MSCABD_PARAM_FIXMSZIP: If non-zero, extract() will ignore bad
        ///   checksums and recover from decompression errors in MS-ZIP
        ///   compressed folders. The default value is 0 (don't recover).
        /// - #MSCABD_PARAM_DECOMPBUF: How many bytes should be used as an input
        ///   bit buffer by decompressors? The minimum value is 4. The default
        ///   value is 4096.
        /// </summary>
        /// <param name="param">the parameter to set</param>
        /// <param name="value">the value to set the parameter to</param>
        /// <returns>
        /// MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS if there
        /// is a problem with either parameter or value.
        /// </returns>
        /// <see cref="Search(string)"/>
        /// <see cref="Extract(InternalFile, string)"/>
        public Error SetParam(Parameters param, int value)
        {
            switch (param)
            {
                case Parameters.MSCABD_PARAM_SEARCHBUF:
                    if (value < 4)
                        return Error.MSPACK_ERR_ARGS;

                    SearchBufferSize = value;
                    break;

                case Parameters.MSCABD_PARAM_FIXMSZIP:
                    FixMSZip = value != 0;
                    break;

                case Parameters.MSCABD_PARAM_DECOMPBUF:
                    if (value < 4)
                        return Error.MSPACK_ERR_ARGS;

                    BufferSize = value;
                    break;

                case Parameters.MSCABD_PARAM_SALVAGE:
                    Salvage = value != 0;
                    break;

                default:
                    return Error.MSPACK_ERR_ARGS;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region Decompression State

        /// <summary>
        /// Initialises decompression state, according to which
        /// decompression method was used. relies on self.State.Folder being the same
        /// as when initialised.
        /// </summary>
        internal Error InitDecompressionState(CompressionType ct)
        {
            State.CompressionType = ct;
            switch (ct & CompressionType.COMPTYPE_MASK)
            {
                case CompressionType.COMPTYPE_NONE:
                    State.DecompressorState = None.Init(State.System, State.InputFileHandle, State.OutputFileHandle, BufferSize);
                    break;

                case CompressionType.COMPTYPE_MSZIP:
                    State.DecompressorState = MSZIP.Init(State.System, State.InputFileHandle, State.OutputFileHandle, BufferSize, FixMSZip);
                    break;

                case CompressionType.COMPTYPE_QUANTUM:
                    State.DecompressorState = QTM.Init(State.System, State.InputFileHandle, State.OutputFileHandle, ((ushort)ct >> 8) & 0x1f, BufferSize);
                    break;

                case CompressionType.COMPTYPE_LZX:
                    State.DecompressorState = LZX.Init(State.System, State.InputFileHandle, State.OutputFileHandle, ((ushort)ct >> 8) & 0x1f, 0, BufferSize, 0, false);
                    break;

                default:
                    return Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            return Error = (State.DecompressorState != null) ? Error.MSPACK_ERR_OK : Error.MSPACK_ERR_NOMEMORY;
        }

        /// <summary>
        /// Updates the decompression state with a new output
        /// </summary>
        internal Error UpdateDecompressionState()
        {
            switch (State.CompressionType & CompressionType.COMPTYPE_MASK)
            {
                case CompressionType.COMPTYPE_NONE:
                case CompressionType.COMPTYPE_MSZIP:
                case CompressionType.COMPTYPE_QUANTUM:
                case CompressionType.COMPTYPE_LZX:
                    State.DecompressorState.OutputFileHandle = State.OutputFileHandle;
                    break;

                default:
                    return Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            return Error = (State.DecompressorState != null) ? Error.MSPACK_ERR_OK : Error.MSPACK_ERR_NOMEMORY;
        }

        /// <summary>
        /// Frees decompression state, according to which method was used.
        /// </summary>
        /// <param name="self"></param>
        internal void FreeDecompressionState()
        {
            if (State?.DecompressorState == null)
                return;

            State.DecompressorState = null;
        }

        #endregion

        #region I/O Methods

        /// <summary>
        /// The internal reader function which the decompressors
        /// use. will read data blocks (and merge split blocks) from the cabinet
        /// and serve the read bytes to the decompressors
        /// </summary>
        internal int SysRead(object file, byte[] buffer, int pointer, int bytes)
        {
            int avail, todo, outlen = 0;

            bool ignore_cksum = Salvage ||
              (FixMSZip &&
               ((State.CompressionType & CompressionType.COMPTYPE_MASK) == CompressionType.COMPTYPE_MSZIP));
            bool ignore_blocksize = Salvage;

            todo = bytes;
            while (todo > 0)
            {
                avail = State.InputEnd - State.InputPointer;

                // If out of input data, read a new block
                if (avail != 0)
                {
                    // Copy as many input bytes available as possible
                    if (avail > todo)
                        avail = todo;

                    Array.Copy(State.Input, State.InputPointer, buffer, pointer, avail);
                    State.InputPointer += avail;
                    pointer += avail;
                    todo -= avail;
                }
                else
                {
                    // Out of data, read a new block

                    // Check if we're out of input blocks, advance block counter
                    if (State.Block++ >= State.Folder.Header.NumBlocks)
                    {
                        if (!Salvage)
                            ReadError = Error.MSPACK_ERR_DATAFORMAT;
                        else
                            Console.WriteLine("Ran out of CAB input blocks prematurely");

                        break;
                    }

                    // Read a block
                    ReadError = SysReadBlock(ref outlen, ignore_cksum, ignore_blocksize);
                    if (ReadError != Error.MSPACK_ERR_OK)
                        return -1;

                    State.Outlen += outlen;

                    // Special Quantum hack -- trailer byte to allow the decompressor
                    // to realign itself. CAB Quantum blocks, unlike LZX blocks, can have
                    // anything from 0 to 4 trailing null bytes.
                    if ((State.CompressionType & CompressionType.COMPTYPE_MASK) == CompressionType.COMPTYPE_QUANTUM)
                        State.Input[State.InputEnd++] = 0xFF;

                    // Is this the last block?
                    if (State.Block >= State.Folder.Header.NumBlocks)
                    {
                        if ((State.CompressionType & CompressionType.COMPTYPE_MASK) == CompressionType.COMPTYPE_LZX)
                        {
                            // Special LZX hack -- on the last block, inform LZX of the
                            // size of the output data stream.
                            (State.DecompressorState as LZX).SetOutputLength(State.Outlen);
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
        internal int SysWrite(object file, byte[] buffer, int pointer, int bytes)
        {
            State.Offset += (uint)bytes;
            if (State.OutputFileHandle != null)
                return SystemImpl.DefaultSystem.Write(State.OutputFileHandle, buffer, pointer, bytes);

            return bytes;
        }

        /// <summary>
        /// Reads a whole data block from a cab file. The block may span more than
        /// one cab file, if it does then the fragments will be reassembled
        /// </summary>
        private Error SysReadBlock(ref int output, bool ignore_cksum, bool ignore_blocksize)
        {
            byte[] hdr = new byte[_DataBlockHeader.Size];
            int full_len;

            // Reset the input block pointer and end of block pointer
            State.InputPointer = State.InputEnd = 0;

            do
            {
                // Read the block header
                if (SystemImpl.DefaultSystem.Read(State.InputFileHandle, hdr, 0, _DataBlockHeader.Size) != _DataBlockHeader.Size)
                    return Error.MSPACK_ERR_READ;

                // Skip any reserved block headers
                if (State.Data.Cab.Header.DataReserved != 0 && !System.Seek(State.InputFileHandle, State.Data.Cab.Header.DataReserved, SeekMode.MSPACK_SYS_SEEK_CUR))
                    return Error.MSPACK_ERR_SEEK;

                // Create a block header from the data
                Error err = _DataBlockHeader.Create(hdr, out _DataBlockHeader dataBlockHeader);
                if (err != Error.MSPACK_ERR_OK)
                    return err;

                // Blocks must not be over CAB_INPUTMAX in size
                full_len = (State.InputEnd - State.InputPointer) + dataBlockHeader.CompressedSize; // Include cab-spanning blocks
                if (full_len > CAB_INPUTMAX)
                {
                    Console.WriteLine($"Block size {full_len} > CAB_INPUTMAX");

                    // In salvage mode, blocks can be 65535 bytes but no more than that
                    if (!ignore_blocksize || full_len > CAB_INPUTMAX_SALVAGE)
                        return Error.MSPACK_ERR_DATAFORMAT;
                }

                // Blocks must not expand to more than CAB_BLOCKMAX 
                if (dataBlockHeader.UncompressedSize > CAB_BLOCKMAX)
                {
                    Console.WriteLine("block size > CAB_BLOCKMAX");
                    if (!ignore_blocksize)
                        return Error.MSPACK_ERR_DATAFORMAT;
                }

                // Read the block data
                if (SystemImpl.DefaultSystem.Read(State.InputFileHandle, State.Input, State.InputEnd, dataBlockHeader.CompressedSize) != dataBlockHeader.CompressedSize)
                    return Error.MSPACK_ERR_READ;

                // Perform checksum test on the block (if one is stored)
                if (dataBlockHeader.CheckSum != 0)
                {
                    uint sum2 = Checksum(State.Input, State.InputEnd, dataBlockHeader.CompressedSize, 0);
                    if (Checksum(hdr, 4, 4, sum2) != dataBlockHeader.CheckSum)
                    {
                        if (!ignore_cksum)
                            return Error.MSPACK_ERR_CHECKSUM;

                        System.Message(State.InputFileHandle, "WARNING; bad block checksum found");
                    }
                }

                // Advance end of block pointer to include newly read data
                State.InputEnd += dataBlockHeader.CompressedSize;

                // Uncompressed size == 0 means this block was part of a split block
                // and it continues as the first block of the next cabinet in the set.
                // Otherwise, this is the last part of the block, and no more block
                // reading needs to be done.

                // EXIT POINT OF LOOP -- uncompressed size != 0
                if ((output = dataBlockHeader.UncompressedSize) != 0)
                    return Error.MSPACK_ERR_OK;

                // Otherwise, advance to next cabinet

                // Close current file handle
                System.Close(State.InputFileHandle);
                State.InputFileHandle = null;

                // Advance to next member in the cabinet set
                if ((State.Data = State.Data.Next) == null)
                {
                    System.Message(State.InputFileHandle, "WARNING; ran out of cabinets in set. Are any missing?");
                    return Error.MSPACK_ERR_DATAFORMAT;
                }

                // Open next cab file
                State.InputCabinet = State.Data.Cab;
                if ((State.InputFileHandle = System.Open(State.InputCabinet.Filename, OpenMode.MSPACK_SYS_OPEN_READ)) == null)
                    return Error.MSPACK_ERR_OPEN;

                // Seek to start of data blocks
                if (!System.Seek(State.InputFileHandle, State.Data.Offset, SeekMode.MSPACK_SYS_SEEK_START))
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

        #region Helpers

        /// <summary>
        /// Decides if two folders are OK to merge
        /// </summary>
        private bool CanMergeFolders(Folder lfol, Folder rfol)
        {
            bool matching = true;

            // Check that both folders use the same compression method/settings
            if (lfol.Header.CompType != rfol.Header.CompType)
            {
                Console.WriteLine("Folder merge: compression type mismatch");
                return false;
            }

            // Check there are not too many data blocks after merging
            if ((lfol.Header.NumBlocks + rfol.Header.NumBlocks) > CAB_FOLDERMAX)
            {
                Console.WriteLine("Folder merge: too many data blocks in merged folders");
                return false;
            }

            // Assign the merge next and previous values
            InternalFile lfi = lfol.MergeNext;
            InternalFile rfi = rfol.MergePrev;

            // Check that we can merge the two cabinets
            if (lfi == null || rfi == null)
            {
                Console.WriteLine("Folder merge: one cabinet has no files to merge");
                return false;
            }

            // For all files in lfol (which is the last folder in whichever cab and
            // only has files to merge), compare them to the files from rfol. They
            // should be identical in number and order. to verify this, check the
            // offset and length of each file. 
            for (InternalFile l = lfi, r = rfi; l != null; l = l.Next, r = r.Next)
            {
                if (r == null || (l.Header.FolderOffset != r.Header.FolderOffset) || (l.Header.UncompressedSize != r.Header.UncompressedSize))
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
            for (InternalFile l = lfi; l != null; l = l.Next)
            {
                InternalFile r = rfi;
                for (; r != null; r = r.Next)
                {
                    if (l.Header.FolderOffset == r.Header.FolderOffset && l.Header.UncompressedSize == r.Header.UncompressedSize)
                    break;
                }

                if (r != null)
                    matching = true;
                else
                    System.Message(null, $"WARNING; merged file {l.Filename} not listed in both cabinets");
            }

            return matching;
        }

        /// <summary>
        /// The inner loop of <see cref="Search(Decompressor, string)"/>, to make it easier to
        /// break out of the loop and be sure that all resources are freed
        /// </summary>
        private Error Find(byte[] buf, FileStream fh, string filename, long flen, ref long firstlen, out Cabinet firstcab)
        {
            firstcab = null;
            Cabinet cab, link = null;
            long caboff, offset, length;
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
                if (length > SearchBufferSize)
                    length = SearchBufferSize;

                // Fill the search buffer with data from disk
                if (System.Read(fh, buf, 0, (int)length) != (int)length)
                    return Error.MSPACK_ERR_READ;

                // FAQ avoidance strategy
                if (offset == 0 && BitConverter.ToUInt32(buf, 0) == 0x28635349)
                    System.Message(fh, "WARNING; found InstallShield header. Use unshield (https://github.com/twogood/unshield) to unpack this file");

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
                                (((caboff + cablen_u32) < (flen + 32)) || Salvage))
                            {
                                // Likely cabinet found -- try reading it
                                cab = new Cabinet() { Filename = filename };

                                if (ReadHeaders(fh, cab, caboff, Salvage, quiet: true) != Error.MSPACK_ERR_OK)
                                {
                                    // Destroy the failed cabinet
                                    Close(cab);
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

                            if (!System.Seek(fh, offset, SeekMode.MSPACK_SYS_SEEK_START))
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

        /// <summary>
        /// Joins cabinets together, also merges split folders between these two
        /// cabinets only. This includes freeing the duplicate folder and file(s)
        /// and allocating a further mscabd_folder_data structure to append to the
        /// merged folder's data parts list.
        /// </summary>
        private Error Merge(Cabinet lcab, Cabinet rcab)
        {
            InternalFile rfi, lfi;

            // Basic args check
            if (lcab == null || rcab == null || (lcab == rcab))
            {
                Console.WriteLine("lcab null, rcab null or lcab = rcab");
                return Error = Error.MSPACK_ERR_ARGS;
            }

            // Check there's not already a cabinet attached
            if (lcab.NextCabinet != null || rcab.PreviousCabinet != null)
            {
                Console.WriteLine("Cabs already joined");
                return Error = Error.MSPACK_ERR_ARGS;
            }

            // Do not create circular cabinet chains
            for (Cabinet cab = lcab.PreviousCabinet; cab != null; cab = cab.PreviousCabinet)
            {
                if (cab == rcab)
                {
                    Console.WriteLine("circular!");
                    return Error = Error.MSPACK_ERR_ARGS;
                }
            }

            for (Cabinet cab = rcab.NextCabinet; cab != null; cab = cab.NextCabinet)
            {
                if (cab == lcab)
                {
                    Console.WriteLine("circular!");
                    return Error = Error.MSPACK_ERR_ARGS;
                }
            }

            // Warn about odd set IDs or indices
            if (lcab.Header.SetID != rcab.Header.SetID)
                System.Message(null, "WARNING; merged cabinets with differing Set IDs.");

            if (lcab.Header.CabinetIndex > rcab.Header.CabinetIndex)
                System.Message(null, "WARNING; merged cabinets with odd order.");

            // Merging the last folder in lcab with the first folder in rcab
            Folder lfol = lcab.Folders;
            Folder rfol = rcab.Folders;
            while (lfol.Next != null)
            {
                lfol = lfol.Next;
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
                InternalFile fi = lcab.Files;
                while (fi.Next != null)
                {
                    fi = fi.Next;
                }

                fi.Next = rcab.Files;
            }
            else
            {
                // Folder merge required - do the files match?
                if (!CanMergeFolders(lfol, rfol))
                    return Error = Error.MSPACK_ERR_DATAFORMAT;

                // Allocate a new folder data structure
                FolderData data = new FolderData();

                // Attach cabs
                lcab.NextCabinet = rcab;
                rcab.PreviousCabinet = lcab;

                // Append rfol's data to lfol
                FolderData ndata = lfol.Data;
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
                lfol.Header.NumBlocks += (ushort)(rfol.Header.NumBlocks - 1);
                if ((rfol.MergeNext == null) || (rfol.MergeNext.Folder != rfol))
                    lfol.MergeNext = rfol.MergeNext;

                // Attach the rfol's folder (except the merge folder)
                while (lfol.Next != null)
                {
                    lfol = lfol.Next;
                }

                lfol.Next = rfol.Next;

                // Attach rfol's files
                InternalFile fi = lcab.Files;
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
                    }
                    else
                    {
                        lfi = fi;
                    }
                }
            }

            // All done! fix files and folders pointers in alsl cabs so they all
            // point to the same list
            for (Cabinet cab = lcab.PreviousCabinet; cab != null; cab = cab.PreviousCabinet)
            {
                cab.Files = lcab.Files;
                cab.Folders = lcab.Folders;
            }

            for (Cabinet cab = lcab.NextCabinet; cab != null; cab = cab.NextCabinet)
            {
                cab.Files = lcab.Files;
                cab.Folders = lcab.Folders;
            }

            return Error = Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Reads the cabinet file header, folder list and file list.
        /// Fills out a pre-existing Cabinet structure, allocates memory
        /// for folders and files as necessary
        /// </summary>
        private Error ReadHeaders(FileStream fh, Cabinet cab, long offset, bool salvage, bool quiet)
        {
            Error err = Error.MSPACK_ERR_OK;
            Folder linkfol = null;
            InternalFile linkfile = null;

            // Initialise pointers
            if (cab == null)
                cab = new Cabinet();

            cab.Next = null;
            cab.Files = null;
            cab.Folders = null;
            cab.PreviousCabinet = cab.NextCabinet = null;
            cab.PreviousName = cab.NextName = null;
            cab.PreviousInfo = cab.NextInfo = null;

            cab.BaseOffset = offset;

            #region Cabinet Header

            // Seek to CFHEADER
            if (!System.Seek(fh, offset, SeekMode.MSPACK_SYS_SEEK_START))
                return Error = Error.MSPACK_ERR_SEEK;

            // Read in the CFHEADER
            byte[] buf = new byte[_CabinetHeader.Size];
            if (System.Read(fh, buf, 0, _CabinetHeader.Size) != _CabinetHeader.Size)
                return Error = Error.MSPACK_ERR_READ;

            // Create a new header based on that
            err = _CabinetHeader.Create(buf, out _CabinetHeader cabinetHeader);
            if (err != Error.MSPACK_ERR_OK)
                return Error = err;

            // Assign the header
            cab.Header = cabinetHeader;

            // Check for the extended header
            if (cab.Header.Flags.HasFlag(HeaderFlags.MSCAB_HDR_RESV))
            {
                buf = new byte[_CabinetHeader.ExtendedSize];
                if (System.Read(fh, buf, 0, _CabinetHeader.ExtendedSize) != _CabinetHeader.ExtendedSize)
                    return Error = Error.MSPACK_ERR_READ;

                // Populate the extended header
                cabinetHeader.PopulateExtendedHeader(buf);

                // Skip the reserved header
                if (cab.Header.HeaderReserved != 0)
                {
                    if (!System.Seek(fh, cab.Header.HeaderReserved, SeekMode.MSPACK_SYS_SEEK_CUR))
                        return Error = Error.MSPACK_ERR_SEEK;
                }
            }

            // Read name and info of preceeding cabinet in set, if present
            if (cab.Header.Flags.HasFlag(HeaderFlags.MSCAB_HDR_PREVCAB))
            {
                cab.PreviousName = ReadString(fh, false, ref err);
                if (err != Error.MSPACK_ERR_OK)
                    return Error = err;

                cab.PreviousInfo = ReadString(fh, true, ref err);
                if (err != Error.MSPACK_ERR_OK)
                    return Error = err;
            }

            // Read name and info of next cabinet in set, if present
            if (cab.Header.Flags.HasFlag(HeaderFlags.MSCAB_HDR_NEXTCAB))
            {
                cab.NextName = ReadString(fh, false, ref err);
                if (err != Error.MSPACK_ERR_OK)
                    return Error = err;

                cab.NextInfo = ReadString(fh, true, ref err);
                if (err != Error.MSPACK_ERR_OK)
                    return Error = err;
            }

            #endregion

            #region Folder and File Pointers

            // Read folders
            for (int i = 0; i < cab.Header.NumFolders; i++)
            {
                err = ReadSingleFolder(fh, cab, offset, ref linkfol);
                if (err != Error.MSPACK_ERR_OK)
                    return Error = err;
            }

            // Read files
            for (int i = 0; i < cab.Header.NumFiles; i++)
            {
                err = ReadSingleFile(fh, cab, salvage, ref linkfile);
                if (err != Error.MSPACK_ERR_OK)
                    return Error = err;
            }

            if (cab.Files == null)
            {
                // We never actually added any files to the file list.  Something went wrong.
                // The file header may have been invalid
                Console.WriteLine($"No files found, even though header claimed to have {cab.Header.NumFiles} files");
                return Error.MSPACK_ERR_DATAFORMAT;
            }

            #endregion

            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Read and process a single file
        /// </summary>
        private Error ReadSingleFile(FileStream fh, Cabinet cab, bool salvage, ref InternalFile linkfile)
        {
            // Read in the FIHEADER
            byte[] buf = new byte[64];
            if (System.Read(fh, buf, 0, _FileHeader.Size) != _FileHeader.Size)
                return Error.MSPACK_ERR_READ;

            InternalFile file = new InternalFile() { Next = null };

            // Create a new header based on that
            Error err = _FileHeader.Create(buf, out _FileHeader fileHeader);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            // Assign the header
            file.Header = fileHeader;

            // Set folder pointer
            if (file.Header.FolderIndex < FileFlags.CONTINUED_FROM_PREV)
            {
                // Normal folder index; count up to the correct folder
                if ((int)file.Header.FolderIndex < cab.Header.NumFolders)
                {
                    Folder ifol = cab.Folders;
                    while (file.Header.FolderIndex-- != 0)
                    {
                        if (ifol != null)
                            ifol = ifol.Next;
                    }

                    file.Folder = ifol;
                }
                else
                {
                    Console.WriteLine("Invalid folder index");
                    file.Folder = null;
                }
            }
            else
            {
                // Either CONTINUED_TO_NEXT, CONTINUED_FROM_PREV or CONTINUED_PREV_AND_NEXT
                if (file.Header.FolderIndex == FileFlags.CONTINUED_TO_NEXT || file.Header.FolderIndex == FileFlags.CONTINUED_PREV_AND_NEXT)
                {
                    // Get last folder
                    Folder ifol = cab.Folders;
                    while (ifol.Next != null)
                    {
                        ifol = ifol.Next;
                    }

                    file.Folder = ifol;

                    // Set "merge next" pointer
                    Folder fol = ifol;
                    if (fol.MergeNext == null)
                        fol.MergeNext = file;
                }

                if (file.Header.FolderIndex == FileFlags.CONTINUED_FROM_PREV || file.Header.FolderIndex == FileFlags.CONTINUED_PREV_AND_NEXT)
                {
                    // Get first folder
                    file.Folder = cab.Folders;

                    // Set "merge prev" pointer
                    Folder fol = file.Folder;
                    if (fol.MergePrev == null)
                        fol.MergePrev = file;
                }
            }

            // Get filename
            file.Filename = ReadString(fh, false, ref err);

            // If folder index or filename are bad, either skip it or fail
            if (err != Error.MSPACK_ERR_OK || file.Folder == null)
            {
                if (salvage)
                    return Error.MSPACK_ERR_OK;

                return err != Error.MSPACK_ERR_OK ? err : Error.MSPACK_ERR_DATAFORMAT;
            }

            // Link file entry into file list
            if (linkfile == null)
                cab.Files = file;
            else
                linkfile.Next = file;

            linkfile = file;
            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Read and process a single folder
        /// </summary>
        private Error ReadSingleFolder(FileStream fh, Cabinet cab, long offset, ref Folder linkfol)
        {
            // Read in the FOHEADER
            byte[] buf = new byte[64];
            if (System.Read(fh, buf, 0, _FolderHeader.Size) != _FolderHeader.Size)
                return Error.MSPACK_ERR_READ;

            if (cab.Header.FolderReserved != 0)
            {
                if (!System.Seek(fh, cab.Header.FolderReserved, SeekMode.MSPACK_SYS_SEEK_CUR))
                    return Error.MSPACK_ERR_SEEK;
            }

            // Create an empty folder
            Folder fol = new Folder()
            {
                Next = null,
                MergePrev = null,
                MergeNext = null,
            };

            // Create a new header based on that
            Error err = _FolderHeader.Create(buf, out _FolderHeader folderHeader);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            // Assign the header
            fol.Header = folderHeader;

            // Set the folder data fields
            fol.Data = new FolderData()
            {
                Next = null,
                Cab = cab,
                Offset = offset + fol.Header.DataOffset,
            };

            // Link folder into list of folders
            if (linkfol == null)
                cab.Folders = fol;
            else
                linkfol.Next = fol;

            linkfol = fol;

            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Read a possibly empty string from the input
        /// </summary>
        private string ReadString(FileStream fh, bool permitEmpty, ref Error error)
        {
            long position = System.Tell(fh);
            byte[] buf = new byte[256];
            int len, i;

            // Read up to 256 bytes
            if ((len = System.Read(fh, buf, 0, 256)) <= 0)
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
            if (!System.Seek(fh, position + len, SeekMode.MSPACK_SYS_SEEK_START))
            {
                error = Error.MSPACK_ERR_SEEK;
                return null;
            }

            error = Error.MSPACK_ERR_OK;
            return Encoding.ASCII.GetString(buf, 0, len);
        }

        #endregion
    }
}
