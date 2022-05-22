/* This file is part of libmspack.
 * © 2013 Intel Corporation
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

/* The Exchange Online Addressbook (OAB or sometimes OAL) is distributed
 * as a .LZX file in one of two forms. Either a "full download" containing
 * the entire address list, or an incremental binary patch which should be
 * applied to a previous version of the full decompressed data.
 *
 * The contents and format of the decompressed OAB are not handled here.
 *
 * For a complete description of the format, see the MSDN site:
 *
 * http://msdn.microsoft.com/en-us/library/cc463914 - [MS-OXOAB].pdf
 * http://msdn.microsoft.com/en-us/library/cc483133 - [MS-PATCH].pdf
 */

using System;
using System.IO;
using LibMSPackSharp.Compression;

namespace LibMSPackSharp.OAB
{
    public class Implementation
    {
        #region OAB decompression definitions

        private const int oabhead_VersionHi = 0x0000;
        private const int oabhead_VersionLo = 0x0004;
        private const int oabhead_BlockMax = 0x0008;
        private const int oabhead_TargetSize = 0x000c;
        private const int oabhead_SIZEOF = 0x0010;

        private const int oabblk_Flags = 0x0000;
        private const int oabblk_CompSize = 0x0004;
        private const int oabblk_UncompSize = 0x0008;
        private const int oabblk_CRC = 0x000c;
        private const int oabblk_SIZEOF = 0x0010;

        private const int patchhead_VersionHi = 0x0000;
        private const int patchhead_VersionLo = 0x0004;
        private const int patchhead_BlockMax = 0x0008;
        private const int patchhead_SourceSize = 0x000c;
        private const int patchhead_TargetSize = 0x0010;
        private const int patchhead_SourceCRC = 0x0014;
        private const int patchhead_TargetCRC = 0x0018;
        private const int patchhead_SIZEOF = 0x001c;

        private const int patchblk_PatchSize = 0x0000;
        private const int patchblk_TargetSize = 0x0004;
        private const int patchblk_SourceSize = 0x0008;
        private const int patchblk_CRC = 0x000c;
        private const int patchblk_SIZEOF = 0x0010;

        #endregion

        #region OABD_SYS_READ

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

        #endregion

        #region OABD_SYS_WRITE

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

        #region OABD_DECOMPRESS

        public static Error Decompress(Decompressor d, string input, string output)
        {
            DecompressorImpl self = d as DecompressorImpl;
            byte[] hdrbuf = new byte[oabhead_SIZEOF];
            LZXDStream lzx = null;
            Error ret = Error.MSPACK_ERR_OK;

            if (self == null)
                return Error.MSPACK_ERR_ARGS;

            SystemImpl sys = self.System;

            FileStream infh = sys.Open(input, OpenMode.MSPACK_SYS_OPEN_READ);
            if (infh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                sys.Close(infh);
                return ret;
            }

            if (sys.Read(infh, hdrbuf, 0, oabhead_SIZEOF) != oabhead_SIZEOF)
            {
                ret = Error.MSPACK_ERR_READ;
                sys.Close(infh);
                return ret;
            }

            if (BitConverter.ToUInt32(hdrbuf, oabhead_VersionHi) != 3 ||
                BitConverter.ToUInt32(hdrbuf, oabhead_VersionLo) != 1)
            {
                ret = Error.MSPACK_ERR_SIGNATURE;
                sys.Close(infh);
                return ret;
            }

            uint block_max = BitConverter.ToUInt32(hdrbuf, oabhead_BlockMax);
            uint target_size = BitConverter.ToUInt32(hdrbuf, oabhead_TargetSize);

            FileStream outfh = sys.Open(output, OpenMode.MSPACK_SYS_OPEN_WRITE);
            if (outfh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                sys.Close(outfh);
                sys.Close(infh);
                return ret;
            }

            byte[] buf = new byte[self.BufferSize];

            SystemImpl oabd_sys = sys;
            oabd_sys.Read = SysRead;
            oabd_sys.Write = SysWrite;

            InternalFile in_ofh = new InternalFile();
            in_ofh.OrigSys = sys;
            in_ofh.OrigFile = infh;

            InternalFile out_ofh = new InternalFile();
            out_ofh.OrigSys = sys;
            out_ofh.OrigFile = outfh;

            while (target_size != 0)
            {
                if (sys.Read(infh, buf, 0, oabblk_SIZEOF) != oabblk_SIZEOF)
                {
                    ret = Error.MSPACK_ERR_READ;
                    sys.Close(outfh);
                    sys.Close(infh);
                    return ret;
                }

                uint blk_flags = BitConverter.ToUInt32(buf, oabblk_Flags);
                uint blk_csize = BitConverter.ToUInt32(buf, oabblk_CompSize);
                uint blk_dsize = BitConverter.ToUInt32(buf, oabblk_UncompSize);
                uint blk_crc = BitConverter.ToUInt32(buf, oabblk_CRC);

                if (blk_dsize > block_max || blk_dsize > target_size || blk_flags > 1)
                {
                    ret = Error.MSPACK_ERR_DATAFORMAT;
                    sys.Close(outfh);
                    sys.Close(infh);
                    return ret;
                }

                if (blk_flags == 0)
                {
                    // Uncompressed block
                    if (blk_dsize != blk_csize)
                    {
                        ret = Error.MSPACK_ERR_DATAFORMAT;
                        sys.Close(outfh);
                        sys.Close(infh);
                        return ret;
                    }

                    ret = CopyFileHandle(sys, infh, outfh, (int)blk_dsize, buf, self.BufferSize);
                    if (ret != Error.MSPACK_ERR_OK)
                    {
                        sys.Close(outfh);
                        sys.Close(infh);
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

                    lzx = LZX.Init(oabd_sys, in_ofh.OrigFile, out_ofh.OrigFile, window_bits, 0, self.BufferSize, blk_dsize, true);
                    if (lzx == null)
                    {
                        ret = Error.MSPACK_ERR_NOMEMORY;
                        sys.Close(outfh);
                        sys.Close(infh);
                        return ret;
                    }

                    ret = LZX.Decompress(lzx, blk_dsize);
                    if (ret != Error.MSPACK_ERR_OK)
                    {
                        sys.Close(outfh);
                        sys.Close(infh);
                        return ret;
                    }

                    lzx = null;

                    // Consume any trailing padding bytes before the next block
                    ret = CopyFileHandle(sys, infh, null, in_ofh.Available, buf, self.BufferSize);
                    if (ret != Error.MSPACK_ERR_OK)
                    {
                        sys.Close(outfh);
                        sys.Close(infh);
                        return ret;
                    }

                    if (out_ofh.CRC != blk_crc)
                    {
                        ret = Error.MSPACK_ERR_CHECKSUM;
                        sys.Close(outfh);
                        sys.Close(infh);
                        return ret;
                    }
                }

                target_size -= blk_dsize;
            }

            sys.Close(outfh);
            sys.Close(infh);

            return ret;
        }

        #endregion

        #region OABD_DECOMPRESS_INCREMENTAL

        public static Error DecompressIncremental(Decompressor d, string input, string basePath, string output)
        {
            DecompressorImpl self = d as DecompressorImpl;
            byte[] hdrbuf = new byte[patchhead_SIZEOF];
            LZXDStream lzx = null;
            int window_bits;
            uint window_size;
            Error ret = Error.MSPACK_ERR_OK;

            if (self == null)
                return Error.MSPACK_ERR_ARGS;

            SystemImpl sys = self.System;

            FileStream infh = sys.Open(input, OpenMode.MSPACK_SYS_OPEN_READ);
            if (infh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                sys.Close(infh);
                return ret;
            }

            if (sys.Read(infh, hdrbuf, 0, patchhead_SIZEOF) != patchhead_SIZEOF)
            {
                ret = Error.MSPACK_ERR_READ;
                sys.Close(infh);
                return ret;
            }

            if (BitConverter.ToUInt32(hdrbuf, patchhead_VersionHi) != 3 ||
                BitConverter.ToUInt32(hdrbuf, patchhead_VersionLo) != 2)
            {
                ret = Error.MSPACK_ERR_SIGNATURE;
                sys.Close(infh);
                return ret;
            }

            uint block_max = BitConverter.ToUInt32(hdrbuf, patchhead_BlockMax);
            uint target_size = BitConverter.ToUInt32(hdrbuf, patchhead_TargetSize);

            // We use it for reading block headers too
            if (block_max < patchblk_SIZEOF)
                block_max = patchblk_SIZEOF;

            FileStream basefh = sys.Open(basePath, OpenMode.MSPACK_SYS_OPEN_READ);
            if (basefh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                sys.Close(basefh);
                sys.Close(infh);
                return ret;
            }

            FileStream outfh = sys.Open(output, OpenMode.MSPACK_SYS_OPEN_WRITE);
            if (outfh == null)
            {
                ret = Error.MSPACK_ERR_OPEN;
                sys.Close(outfh);
                sys.Close(basefh);
                sys.Close(infh);
                return ret;
            }

            byte[] buf = new byte[self.BufferSize];

            SystemImpl oabd_sys = sys;
            oabd_sys.Read = SysRead;
            oabd_sys.Write = SysWrite;

            InternalFile in_ofh = new InternalFile();
            in_ofh.OrigSys = sys;
            in_ofh.OrigFile = infh;

            InternalFile out_ofh = new InternalFile();
            out_ofh.OrigSys = sys;
            out_ofh.OrigFile = outfh;

            while (target_size != 0)
            {
                if (sys.Read(infh, buf, 0, patchblk_SIZEOF) != patchblk_SIZEOF)
                {
                    ret = Error.MSPACK_ERR_READ;
                    sys.Close(outfh);
                    sys.Close(basefh);
                    sys.Close(infh);
                    return ret;
                }

                uint blk_csize = BitConverter.ToUInt32(buf, patchblk_PatchSize);
                uint blk_dsize = BitConverter.ToUInt32(buf, patchblk_TargetSize);
                uint blk_ssize = BitConverter.ToUInt32(buf, patchblk_SourceSize);
                uint blk_crc = BitConverter.ToUInt32(buf, patchblk_CRC);

                if (blk_dsize > block_max || blk_dsize > target_size || blk_ssize > block_max)
                {
                    ret = Error.MSPACK_ERR_DATAFORMAT;
                    sys.Close(outfh);
                    sys.Close(basefh);
                    sys.Close(infh);
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
                    sys.Close(outfh);
                    sys.Close(basefh);
                    sys.Close(infh);
                    return ret;
                }

                ret = LZX.SetReferenceData(lzx, sys, basefh, blk_ssize);
                if (ret != Error.MSPACK_ERR_OK)
                {
                    sys.Close(outfh);
                    sys.Close(basefh);
                    sys.Close(infh);
                    return ret;
                }

                ret = LZX.Decompress(lzx, blk_dsize);
                if (ret != Error.MSPACK_ERR_OK)
                {
                    sys.Close(outfh);
                    sys.Close(basefh);
                    sys.Close(infh);
                    return ret;
                }

                lzx = null;

                // Consume any trailing padding bytes before the next block
                ret = CopyFileHandle(sys, infh, null, in_ofh.Available, buf, self.BufferSize);
                if (ret != Error.MSPACK_ERR_OK)
                {
                    sys.Close(outfh);
                    sys.Close(basefh);
                    sys.Close(infh);
                    return ret;
                }

                if (out_ofh.CRC != blk_crc)
                {
                    ret = Error.MSPACK_ERR_CHECKSUM;
                    sys.Close(outfh);
                    sys.Close(basefh);
                    sys.Close(infh);
                    return ret;
                }

                target_size -= blk_dsize;
            }

            sys.Close(outfh);
            sys.Close(basefh);
            sys.Close(infh);

            return ret;
        }

        private static Error CopyFileHandle(SystemImpl sys, FileStream infh, FileStream outfh, int bytes_to_copy, byte[] buf, int buf_size)
        {
            while (bytes_to_copy != 0)
            {
                int run = buf_size;
                if (run > bytes_to_copy)
                    run = bytes_to_copy;

                if (sys.Read(infh, buf, 0, run) != run)
                    return Error.MSPACK_ERR_READ;

                if (outfh != null && sys.Write(outfh, buf, 0, run) != run)
                    return Error.MSPACK_ERR_WRITE;

                bytes_to_copy -= run;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region OABD_PARAM

        public static Error Param(Decompressor d, Parameters param, int value)
        {
            DecompressorImpl self = d as DecompressorImpl;
            if (self != null && param == Parameters.MSOABD_PARAM_DECOMPBUF && value >= 16)
            {
                // must be at least 16 bytes (patchblk_SIZEOF, oabblk_SIZEOF)
                self.BufferSize = value;
                return Error.MSPACK_ERR_OK;
            }

            return Error.MSPACK_ERR_ARGS;
        }

        #endregion
    }
}
