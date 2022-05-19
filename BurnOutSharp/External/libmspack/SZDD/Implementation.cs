/* This file is part of libmspack.
 * (C) 2003-2004 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;
using System.Linq;
using LibMSPackSharp.Compression;

namespace LibMSPackSharp.SZDD
{
    public class Implementation
    {
        /// <summary>
        /// Input buffer size during decompression - not worth parameterising IMHO
        /// </summary>
        private const int SZDD_INPUT_SIZE = 2048;

        #region SZDDD_OPEN

        /// <summary>
        /// Opens an SZDD file without decompressing, reads header
        /// </summary>
        public static Header Open(Decompressor d, string filename)
        {
            DecompressorImpl self = d as DecompressorImpl;
            if (self == null)
                return null;

            SystemImpl sys = self.System;

            object fh = sys.Open(sys, filename, OpenMode.MSPACK_SYS_OPEN_READ);
            HeaderImpl hdr = new HeaderImpl();
            if (fh != null && hdr != null)
            {
                hdr.FileHandle = fh;
                self.Error = ReadHeaders(sys, fh, hdr);
            }
            else
            {
                if (fh == null)
                    self.Error = Error.MSPACK_ERR_OPEN;
                if (hdr == null)
                    self.Error = Error.MSPACK_ERR_NOMEMORY;
            }

            if (self.Error != Error.MSPACK_ERR_OK)
            {
                if (fh != null)
                    sys.Close(fh);

                sys.Free(hdr);
                hdr = null;
            }

            return hdr;
        }

        #endregion

        #region SZDDD_CLOSE

        /// <summary>
        /// Closes an SZDD file
        /// </summary>
        public static void Close(Decompressor d, Header hdr)
        {
            DecompressorImpl self = d as DecompressorImpl;
            HeaderImpl hdr_p = hdr as HeaderImpl;

            if (self?.System == null || hdr == null)
                return;

            // Close the file handle associated
            self.System.Close(hdr_p.FileHandle);

            // Free the memory associated
            self.System.Free(hdr);

            self.Error = Error.MSPACK_ERR_OK;
        }

        #endregion

        #region SZDDD_READ_HEADERS

        private static byte[] expandSignature = new byte[8]
        {
            0x53, 0x5A, 0x44, 0x44, 0x88, 0xF0, 0x27, 0x33
        };

        private static byte[] qbasicSignature = new byte[8]
        {
            0x53, 0x5A, 0x20, 0x88, 0xF0, 0x27, 0x33, 0xD1
        };

        /// <summary>
        /// Reads the headers of an SZDD format file
        /// </summary>
        public static Error ReadHeaders(SystemImpl sys, object fh, Header hdr)
        {
            // Read and check signature
            byte[] buf = new byte[8];
            if (sys.Read(fh, buf, 0, 8) != 8)
                return Error.MSPACK_ERR_READ;

            if (buf.SequenceEqual(expandSignature))
            {
                // Common SZDD
                hdr.Format = Format.MSSZDD_FMT_NORMAL;

                // Read the rest of the header
                if (sys.Read(fh, buf, 0, 6) != 6)
                    return Error.MSPACK_ERR_READ;

                if (buf[0] != 0x41)
                    return Error.MSPACK_ERR_DATAFORMAT;

                hdr.MissingChar = (char)buf[1];
                hdr.Length = BitConverter.ToUInt32(buf, 2);
            }
            if (buf.SequenceEqual(qbasicSignature))
            {
                // Special QBasic SZDD
                hdr.Format = Format.MSSZDD_FMT_QBASIC;
                if (sys.Read(fh, buf, 0, 4) != 4)
                    return Error.MSPACK_ERR_READ;

                hdr.MissingChar = '\0';
                hdr.Length = BitConverter.ToUInt32(buf, 0);
            }
            else
            {
                return Error.MSPACK_ERR_SIGNATURE;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region SZDDD_EXTRACT

        /// <summary>
        /// Decompresses an SZDD file
        /// </summary>
        public static Error Extract(Decompressor d, Header hdr, string filename)
        {
            DecompressorImpl self = d as DecompressorImpl;
            if (self == null)
                return Error.MSPACK_ERR_ARGS;
            if (hdr == null)
                return self.Error = Error.MSPACK_ERR_ARGS;

            SystemImpl sys = self.System;

            object fh = (hdr as HeaderImpl)?.FileHandle;
            if (fh == null)
                return Error.MSPACK_ERR_ARGS;

            // Seek to the compressed data
            long dataOffset = (hdr.Format == Format.MSSZDD_FMT_NORMAL) ? 14 : 12;
            if (sys.Seek(fh, dataOffset, SeekMode.MSPACK_SYS_SEEK_START))
                return self.Error = Error.MSPACK_ERR_SEEK;

            // Open file for output
            object outfh;
            if ((outfh = sys.Open(sys, filename, OpenMode.MSPACK_SYS_OPEN_WRITE)) == null)
                return self.Error = Error.MSPACK_ERR_OPEN;

            // Decompress the data
            self.Error = LZSS.Decompress(
                sys,
                fh,
                outfh,
                SZDD_INPUT_SIZE,
                hdr.Format == Format.MSSZDD_FMT_NORMAL
                    ? LZSSMode.LZSS_MODE_EXPAND
                    : LZSSMode.LZSS_MODE_QBASIC);

            // Close output file
            sys.Close(outfh);

            return self.Error;
        }

        #endregion

        #region SZDDD_DECOMPRESS

        /// <summary>
        /// Unpacks directly from input to output
        /// </summary>
        public static Error Decompress(Decompressor d, string input, string output)
        {
            DecompressorImpl self = d as DecompressorImpl;
            if (self == null)
                return Error.MSPACK_ERR_ARGS;

            Header hdr;
            if ((hdr = Open(d, input)) == null)
                return self.Error;

            Error error = Extract(d, hdr, output);
            Close(d, hdr);
            return self.Error = error;
        }

        #endregion

        #region SZDDD_ERROR

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
