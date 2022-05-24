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
using LibMSPackSharp.Compression;
using static LibMSPackSharp.Constants;

namespace LibMSPackSharp.OAB
{
    /// <summary>
    /// A decompressor for .LZX (Offline Address Book) files
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateOABDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyOABDecompressor(Decompressor)"/>
    public class Decompressor : BaseDecompressor
    {
        #region Public Functionality

        /// <summary>
        /// Decompresses a full Offline Address Book file.
        ///
        /// If the input file is a valid compressed Offline Address Book file, 
        /// it will be read and the decompressed contents will be written to
        /// the output file.
        /// </summary>
        /// <param name="input">
        /// The filename of the input file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="output">
        /// The filename of the output file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public Error Decompress(string input, string output)
        {
            byte[] hdrbuf = new byte[oabhead_SIZEOF];
            LZX lzx = null;
            Error ret = Error.MSPACK_ERR_OK;

            FileStream infh = System.Open(input, OpenMode.MSPACK_SYS_OPEN_READ);
            if (infh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                System.Close(infh);
                return ret;
            }

            if (System.Read(infh, hdrbuf, 0, oabhead_SIZEOF) != oabhead_SIZEOF)
            {
                ret = Error.MSPACK_ERR_READ;
                System.Close(infh);
                return ret;
            }

            if (BitConverter.ToUInt32(hdrbuf, oabhead_VersionHi) != 3 ||
                BitConverter.ToUInt32(hdrbuf, oabhead_VersionLo) != 1)
            {
                ret = Error.MSPACK_ERR_SIGNATURE;
                System.Close(infh);
                return ret;
            }

            uint block_max = BitConverter.ToUInt32(hdrbuf, oabhead_BlockMax);
            uint target_size = BitConverter.ToUInt32(hdrbuf, oabhead_TargetSize);

            FileStream outfh = System.Open(output, OpenMode.MSPACK_SYS_OPEN_WRITE);
            if (outfh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                System.Close(outfh);
                System.Close(infh);
                return ret;
            }

            byte[] buf = new byte[BufferSize];

            SystemImpl oabd_sys = System;
            oabd_sys.Read = SysRead;
            oabd_sys.Write = SysWrite;

            InternalFile in_ofh = new InternalFile();
            in_ofh.OrigSys = System;
            in_ofh.OrigFile = infh;

            InternalFile out_ofh = new InternalFile();
            out_ofh.OrigSys = System;
            out_ofh.OrigFile = outfh;

            while (target_size != 0)
            {
                if (System.Read(infh, buf, 0, oabblk_SIZEOF) != oabblk_SIZEOF)
                {
                    ret = Error.MSPACK_ERR_READ;
                    System.Close(outfh);
                    System.Close(infh);
                    return ret;
                }

                uint blk_flags = BitConverter.ToUInt32(buf, oabblk_Flags);
                uint blk_csize = BitConverter.ToUInt32(buf, oabblk_CompSize);
                uint blk_dsize = BitConverter.ToUInt32(buf, oabblk_UncompSize);
                uint blk_crc = BitConverter.ToUInt32(buf, oabblk_CRC);

                if (blk_dsize > block_max || blk_dsize > target_size || blk_flags > 1)
                {
                    ret = Error.MSPACK_ERR_DATAFORMAT;
                    System.Close(outfh);
                    System.Close(infh);
                    return ret;
                }

                if (blk_flags == 0)
                {
                    // Uncompressed block
                    if (blk_dsize != blk_csize)
                    {
                        ret = Error.MSPACK_ERR_DATAFORMAT;
                        System.Close(outfh);
                        System.Close(infh);
                        return ret;
                    }

                    ret = CopyFileHandle(infh, outfh, (int)blk_dsize, buf, BufferSize);
                    if (ret != Error.MSPACK_ERR_OK)
                    {
                        System.Close(outfh);
                        System.Close(infh);
                        return ret;
                    }
                }
                else
                {
                    // LZX compressed block
                    int window_bits = 17;

                    while (window_bits < 25 && (1U << window_bits) < blk_dsize)
                    {
                        window_bits++;
                    }

                    in_ofh.Available = (int)blk_csize;
                    out_ofh.CRC = 0xffffffff;

                    lzx = LZX.Init(oabd_sys, in_ofh.OrigFile, out_ofh.OrigFile, window_bits, 0, BufferSize, blk_dsize, true);
                    if (lzx == null)
                    {
                        ret = Error.MSPACK_ERR_NOMEMORY;
                        System.Close(outfh);
                        System.Close(infh);
                        return ret;
                    }

                    ret = LZX.Decompress(lzx, blk_dsize);
                    if (ret != Error.MSPACK_ERR_OK)
                    {
                        System.Close(outfh);
                        System.Close(infh);
                        return ret;
                    }

                    lzx = null;

                    // Consume any trailing padding bytes before the next block
                    ret = CopyFileHandle(infh, null, in_ofh.Available, buf, BufferSize);
                    if (ret != Error.MSPACK_ERR_OK)
                    {
                        System.Close(outfh);
                        System.Close(infh);
                        return ret;
                    }

                    if (out_ofh.CRC != blk_crc)
                    {
                        ret = Error.MSPACK_ERR_CHECKSUM;
                        System.Close(outfh);
                        System.Close(infh);
                        return ret;
                    }
                }

                target_size -= blk_dsize;
            }

            System.Close(outfh);
            System.Close(infh);

            return ret;
        }

        /// <summary>
        /// Decompresses an Offline Address Book with an incremental patch file.
        ///
        /// This requires both a full UNCOMPRESSED Offline Address Book file to
        /// act as the "base", and a compressed incremental patch file as input.
        /// If the input file is valid, it will be decompressed with reference to
        /// the base file, and the decompressed contents will be written to the
        /// output file.
        ///
        /// There is no way to tell what the right base file is for the given
        /// incremental patch, but if you get it wrong, this will usually result
        /// in incorrect data being decompressed, which will then fail a checksum
        /// test.
        /// </summary>
        /// <param name="input">
        /// The filename of the input file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="basePath">
        /// The filename of the base file to which the
        /// incremental patch shall be applied. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="output">
        /// The filename of the output file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public Error DecompressIncremental(string input, string basePath, string output)
        {
            byte[] hdrbuf = new byte[patchhead_SIZEOF];
            LZX lzx = null;
            int window_bits;
            uint window_size;
            Error ret = Error.MSPACK_ERR_OK;

            FileStream infh = System.Open(input, OpenMode.MSPACK_SYS_OPEN_READ);
            if (infh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                System.Close(infh);
                return ret;
            }

            if (System.Read(infh, hdrbuf, 0, patchhead_SIZEOF) != patchhead_SIZEOF)
            {
                ret = Error.MSPACK_ERR_READ;
                System.Close(infh);
                return ret;
            }

            if (BitConverter.ToUInt32(hdrbuf, patchhead_VersionHi) != 3 ||
                BitConverter.ToUInt32(hdrbuf, patchhead_VersionLo) != 2)
            {
                ret = Error.MSPACK_ERR_SIGNATURE;
                System.Close(infh);
                return ret;
            }

            uint block_max = BitConverter.ToUInt32(hdrbuf, patchhead_BlockMax);
            uint target_size = BitConverter.ToUInt32(hdrbuf, patchhead_TargetSize);

            // We use it for reading block headers too
            if (block_max < patchblk_SIZEOF)
                block_max = patchblk_SIZEOF;

            FileStream basefh = System.Open(basePath, OpenMode.MSPACK_SYS_OPEN_READ);
            if (basefh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                System.Close(basefh);
                System.Close(infh);
                return ret;
            }

            FileStream outfh = System.Open(output, OpenMode.MSPACK_SYS_OPEN_WRITE);
            if (outfh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                System.Close(outfh);
                System.Close(basefh);
                System.Close(infh);
                return ret;
            }

            byte[] buf = new byte[BufferSize];

            SystemImpl oabd_sys = System;
            oabd_sys.Read = SysRead;
            oabd_sys.Write = SysWrite;

            InternalFile in_ofh = new InternalFile();
            in_ofh.OrigSys = System;
            in_ofh.OrigFile = infh;

            InternalFile out_ofh = new InternalFile();
            out_ofh.OrigSys = System;
            out_ofh.OrigFile = outfh;

            while (target_size != 0)
            {
                if (System.Read(infh, buf, 0, patchblk_SIZEOF) != patchblk_SIZEOF)
                {
                    ret = Error.MSPACK_ERR_READ;
                    System.Close(outfh);
                    System.Close(basefh);
                    System.Close(infh);
                    return ret;
                }

                uint blk_csize = BitConverter.ToUInt32(buf, patchblk_PatchSize);
                uint blk_dsize = BitConverter.ToUInt32(buf, patchblk_TargetSize);
                uint blk_ssize = BitConverter.ToUInt32(buf, patchblk_SourceSize);
                uint blk_crc = BitConverter.ToUInt32(buf, patchblk_CRC);

                if (blk_dsize > block_max || blk_dsize > target_size || blk_ssize > block_max)
                {
                    ret = Error.MSPACK_ERR_DATAFORMAT;
                    System.Close(outfh);
                    System.Close(basefh);
                    System.Close(infh);
                    return ret;
                }

                window_size = (uint)((blk_ssize + 32767) & ~32767);
                window_size += blk_dsize;
                window_bits = 17;

                while (window_bits < 25 && (1U << window_bits) < window_size)
                    window_bits++;

                in_ofh.Available = (int)blk_csize;
                out_ofh.CRC = 0xffffffff;

                lzx = LZX.Init(oabd_sys, in_ofh.OrigFile, out_ofh.OrigFile, window_bits, 0, 4096, blk_dsize, true);
                if (lzx == null)
                {
                    ret = Error.MSPACK_ERR_NOMEMORY;
                    System.Close(outfh);
                    System.Close(basefh);
                    System.Close(infh);
                    return ret;
                }

                ret = lzx.SetReferenceData(System, basefh, blk_ssize);
                if (ret != Error.MSPACK_ERR_OK)
                {
                    System.Close(outfh);
                    System.Close(basefh);
                    System.Close(infh);
                    return ret;
                }

                ret = LZX.Decompress(lzx, blk_dsize);
                if (ret != Error.MSPACK_ERR_OK)
                {
                    System.Close(outfh);
                    System.Close(basefh);
                    System.Close(infh);
                    return ret;
                }

                lzx = null;

                // Consume any trailing padding bytes before the next block
                ret = CopyFileHandle(infh, null, in_ofh.Available, buf, BufferSize);
                if (ret != Error.MSPACK_ERR_OK)
                {
                    System.Close(outfh);
                    System.Close(basefh);
                    System.Close(infh);
                    return ret;
                }

                if (out_ofh.CRC != blk_crc)
                {
                    ret = Error.MSPACK_ERR_CHECKSUM;
                    System.Close(outfh);
                    System.Close(basefh);
                    System.Close(infh);
                    return ret;
                }

                target_size -= blk_dsize;
            }

            System.Close(outfh);
            System.Close(basefh);
            System.Close(infh);

            return ret;
        }

        /// <summary>
        /// Sets an OAB decompression engine parameter. Available only in OAB
        /// decompressor version 2 and above.
        ///
        /// - #MSOABD_PARAM_DECOMPBUF: How many bytes should be used as an input
        ///   buffer by decompressors? The minimum value is 16. The default value
        ///   is 4096.
        /// </summary>
        /// <param name="param">The parameter to set</param>
        /// <param name="value">The value to set the parameter to</param>
        /// <returns>
        /// MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS if there
        /// is a problem with either parameter or value.
        /// </returns>
        public Error SetParam(Parameters param, int value)
        {
            if (param == Parameters.MSOABD_PARAM_DECOMPBUF && value >= 16)
            {
                // Must be at least 16 bytes (patchblk_SIZEOF, oabblk_SIZEOF)
                BufferSize = value;
                return Error.MSPACK_ERR_OK;
            }

            return Error.MSPACK_ERR_ARGS;
        }

        #endregion

        #region I/O Methods

        private static int SysRead(object baseFile, byte[] buf, int pointer, int size)
        {
            InternalFile file = baseFile as InternalFile;
            if (file == null)
                return 0;

            int bytes_read;

            if (size > file.Available)
                size = file.Available;

            bytes_read = file.OrigSys.Read(file.OrigFile, buf, pointer, size);
            if (bytes_read < 0)
                return bytes_read;

            file.Available -= bytes_read;
            return bytes_read;
        }

        private static int SysWrite(object baseFile, byte[] buf, int pointer, int size)
        {
            // Null output file means skip those bytes
            if (baseFile == null)
            {
                return size;
            }
            else if (baseFile is InternalFile file)
            {
                int bytes_written = file.OrigSys.Write(file.OrigFile, buf, pointer, size);
                if (bytes_written > 0)
                    file.CRC = Checksum.CRC32(buf, 0, bytes_written, file.CRC);

                return bytes_written;
            }
            else if (baseFile is FileStream impl)
            {
                return SystemImpl.DefaultSystem.Write(impl, buf, pointer, size);
            }

            // Unknown file to write to
            return -1;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Copy between the input and output, if possible
        /// </summary>
        private Error CopyFileHandle(FileStream input, FileStream output, int bytesToCopy, byte[] buf, int bufferSize)
        {
            while (bytesToCopy != 0)
            {
                int run = bufferSize;
                if (run > bytesToCopy)
                    run = bytesToCopy;

                if (System.Read(input, buf, 0, run) != run)
                    return Error.MSPACK_ERR_READ;

                if (output != null && System.Write(output, buf, 0, run) != run)
                    return Error.MSPACK_ERR_WRITE;

                bytesToCopy -= run;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion
    }
}
