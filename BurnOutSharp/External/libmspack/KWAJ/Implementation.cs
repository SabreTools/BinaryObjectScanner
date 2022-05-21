/* This file is part of libmspack.
 * (C) 2003-2010 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

using System;
using System.IO;
using System.Text;
using LibMSPackSharp.Compression;

namespace LibMSPackSharp.KWAJ
{
    public class Implementation
    {
        #region Generic KWAJ Definitions

        private const byte kwajh_Signature1 = 0x00;
        private const byte kwajh_Signature2 = 0x04;
        private const byte kwajh_CompMethod = 0x08;
        private const byte kwajh_DataOffset = 0x0a;
        private const byte kwajh_Flags = 0x0c;
        private const byte kwajh_SIZEOF = 0x0e;

        #endregion

        #region KWAJ Decompression Definitions

        // Input buffer size during decompression - not worth parameterising IMHO
        private const int KWAJ_INPUT_SIZE = (2048);

        // Huffman codes that are 9 bits or less are decoded immediately
        public const int KWAJ_TABLEBITS = (9);

        // Number of codes in each huffman table

        public const int KWAJ_MATCHLEN1_SYMS = (16);
        public const int KWAJ_MATCHLEN2_SYMS = (16);
        public const int KWAJ_LITLEN_SYMS = (32);
        public const int KWAJ_OFFSET_SYMS = (64);
        public const int KWAJ_LITERAL_SYMS = (256);

        // Define decoding table sizes

        public const int KWAJ_TABLESIZE = (1 << KWAJ_TABLEBITS);

        //public const int KWAJ_MATCHLEN1_TBLSIZE = (KWAJ_MATCHLEN1_SYMS * 4);
        public const int KWAJ_MATCHLEN1_TBLSIZE = (KWAJ_TABLESIZE + (KWAJ_MATCHLEN1_SYMS * 2));

        //public const int KWAJ_MATCHLEN2_TBLSIZE = (KWAJ_MATCHLEN2_SYMS * 4);
        public const int KWAJ_MATCHLEN2_TBLSIZE = (KWAJ_TABLESIZE + (KWAJ_MATCHLEN2_SYMS * 2));

        //public const int KWAJ_LITLEN_TBLSIZE = (KWAJ_LITLEN_SYMS * 4);
        public const int KWAJ_LITLEN_TBLSIZE = (KWAJ_TABLESIZE + (KWAJ_LITLEN_SYMS * 2));

        //public const int KWAJ_OFFSET_TBLSIZE = (KWAJ_OFFSET_SYMS * 4);
        public const int KWAJ_OFFSET_TBLSIZE = (KWAJ_TABLESIZE + (KWAJ_OFFSET_SYMS * 2));

        //public const int KWAJ_LITERAL_TBLSIZE = (KWAJ_LITERAL_SYMS * 4);
        public const int KWAJ_LITERAL_TBLSIZE = (KWAJ_TABLESIZE + (KWAJ_LITERAL_SYMS * 2));

        #endregion

        #region KWAJD_OPEN

        /// <summary>
        /// Opens a KWAJ file without decompressing, reads header
        /// </summary>
        public static Header Open(Decompressor d, string filename)
        {
            DecompressorImpl self = d as DecompressorImpl;
            if (self == null)
                return null;

            SystemImpl sys = self.System;

            FileStream fh = sys.Open(filename, OpenMode.MSPACK_SYS_OPEN_READ);
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

                hdr = null;
            }

            return hdr;
        }

        #endregion

        #region KWAJD_CLOSE

        /// <summary>
        /// Closes a KWAJ file
        /// </summary>
        public static void Close(Decompressor d, Header hdr)
        {
            DecompressorImpl self = d as DecompressorImpl;
            HeaderImpl hdr_p = hdr as HeaderImpl;

            if (self?.System == null || hdr_p == null)
                return;

            // Close the file handle associated
            self.System.Close(hdr_p.FileHandle);

            self.Error = Error.MSPACK_ERR_OK;
        }

        #endregion

        #region KWAJD_READ_HEADERS

        /// <summary>
        /// Reads the headers of a KWAJ format file
        /// </summary>
        public static Error ReadHeaders(SystemImpl sys, FileStream fh, Header hdr)
        {
            int i;

            // Read in the header
            byte[] buf = new byte[16];
            if (sys.Read(fh, buf, 0, kwajh_SIZEOF) != kwajh_SIZEOF)
            {
                return Error.MSPACK_ERR_READ;
            }

            // Check for "KWAJ" signature
            if ((BitConverter.ToUInt32(buf, kwajh_Signature1) != 0x4A41574B) ||
                (BitConverter.ToUInt32(buf, kwajh_Signature2) != 0xD127F088))
            {
                return Error.MSPACK_ERR_SIGNATURE;
            }

            // Basic header fields
            hdr.CompressionType = (CompressionType)BitConverter.ToUInt16(buf, kwajh_CompMethod);
            hdr.DataOffset = BitConverter.ToUInt16(buf, kwajh_DataOffset);
            hdr.Headers = (OptionalHeaderFlag)BitConverter.ToUInt16(buf, kwajh_Flags);
            hdr.Length = 0;
            hdr.Filename = null;
            hdr.Extra = null;
            hdr.ExtraLength = 0;

            // Optional headers

            // 4 bytes: length of unpacked file
            if (hdr.Headers.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASLENGTH))
            {
                if (sys.Read(fh, buf, 0, 4) != 4)
                    return Error.MSPACK_ERR_READ;

                hdr.Length = BitConverter.ToUInt32(buf, 0);
            }

            // 2 bytes: unknown purpose
            if (hdr.Headers.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASUNKNOWN1))
            {
                if (sys.Read(fh, buf, 0, 2) != 2)
                    return Error.MSPACK_ERR_READ;
            }

            // 2 bytes: length of section, then [length] bytes: unknown purpose
            if (hdr.Headers.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASUNKNOWN2))
            {
                if (sys.Read(fh, buf, 0, 2) != 2)
                    return Error.MSPACK_ERR_READ;

                i = BitConverter.ToUInt16(buf, 0);
                if (sys.Seek(fh, i, SeekMode.MSPACK_SYS_SEEK_CUR))
                    return Error.MSPACK_ERR_SEEK;
            }

            // Filename and extension
            if (hdr.Headers.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASFILENAME) || hdr.Headers.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASFILEEXT))
            {
                int len;

                // Allocate memory for maximum length filename
                char[] fn = new char[13];
                int fnPtr = 0;

                // Copy filename if present
                if (hdr.Headers.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASFILENAME))
                {
                    // Read and copy up to 9 bytes of a null terminated string
                    if ((len = sys.Read(fh, buf, 0, 9)) < 2)
                        return Error.MSPACK_ERR_READ;

                    for (i = 0; i < len; i++)
                    {
                        if ((fn[fnPtr++] = (char)buf[i]) == '\0')
                            break;
                    }

                    // If string was 9 bytes with no null terminator, reject it
                    if (i == 9 && buf[8] != '\0')
                        return Error.MSPACK_ERR_DATAFORMAT;

                    // Seek to byte after string ended in file
                    if (sys.Seek(fh, i + 1 - len, SeekMode.MSPACK_SYS_SEEK_CUR))
                        return Error.MSPACK_ERR_SEEK;

                    fnPtr--; // Remove the null terminator
                }

                // Copy extension if present
                if (hdr.Headers.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASFILEEXT))
                {
                    fn[fnPtr++] = '.';

                    // Read and copy up to 4 bytes of a null terminated string
                    if ((len = sys.Read(fh, buf, 0, 4)) < 2)
                        return Error.MSPACK_ERR_READ;

                    for (i = 0; i < len; i++)
                    {
                        if ((fn[fnPtr++] = (char)buf[i]) == '\0')
                            break;
                    }

                    // If string was 4 bytes with no null terminator, reject it
                    if (i == 4 && buf[3] != '\0')
                        return Error.MSPACK_ERR_DATAFORMAT;

                    // Seek to byte after string ended in file
                    if (sys.Seek(fh, i + 1 - len, SeekMode.MSPACK_SYS_SEEK_CUR))
                        return Error.MSPACK_ERR_SEEK;

                    fnPtr--; // Remove the null terminator
                }

                fn[fnPtr] = '\0';
            }

            // 2 bytes: extra text length then [length] bytes of extra text data
            if (hdr.Headers.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASEXTRATEXT))
            {
                if (sys.Read(fh, buf, 0, 2) != 2)
                    return Error.MSPACK_ERR_READ;

                i = BitConverter.ToUInt16(buf, 0);
                byte[] extra = new byte[i + 1];
                if (sys.Read(fh, extra, 0, i) != i)
                    return Error.MSPACK_ERR_READ;

                extra[i] = 0x00;
                hdr.Extra = Encoding.ASCII.GetString(extra, 0, extra.Length);
                hdr.ExtraLength = (ushort)i;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion

        #region KWAJD_EXTRACT

        /// <summary>
        /// Decompresses a KWAJ file
        /// </summary>
        public static Error Extract(Decompressor d, Header hdr, string filename)
        {
            DecompressorImpl self = d as DecompressorImpl;
            if (self == null)
                return Error.MSPACK_ERR_ARGS;
            if (hdr == null)
                return self.Error = Error.MSPACK_ERR_ARGS;

            SystemImpl sys = self.System;
            FileStream fh = (hdr as HeaderImpl)?.FileHandle;
            if (fh == null)
                return Error.MSPACK_ERR_ARGS;

            // Seek to the compressed data
            if (sys.Seek(fh, hdr.DataOffset, SeekMode.MSPACK_SYS_SEEK_START))
                return self.Error = Error.MSPACK_ERR_SEEK;

            // Open file for output
            FileStream outfh;
            if ((outfh = sys.Open(filename, OpenMode.MSPACK_SYS_OPEN_WRITE)) == null)
                return self.Error = Error.MSPACK_ERR_OPEN;

            self.Error = Error.MSPACK_ERR_OK;

            // Decompress based on format
            if (hdr.CompressionType == CompressionType.MSKWAJ_COMP_NONE ||
                hdr.CompressionType == CompressionType.MSKWAJ_COMP_XOR)
            {
                // NONE is a straight copy. XOR is a copy xored with 0xFF
                byte[] buf = new byte[KWAJ_INPUT_SIZE];

                int read, i;
                while ((read = sys.Read(fh, buf, 0, KWAJ_INPUT_SIZE)) > 0)
                {
                    if (hdr.CompressionType == CompressionType.MSKWAJ_COMP_XOR)
                    {
                        for (i = 0; i < read; i++)
                        {
                            buf[i] ^= 0xFF;
                        }
                    }

                    if (sys.Write(outfh, buf, 0, read) != read)
                    {
                        self.Error = Error.MSPACK_ERR_WRITE;
                        break;
                    }
                }

                if (read < 0)
                    self.Error = Error.MSPACK_ERR_READ;
            }
            else if (hdr.CompressionType == CompressionType.MSKWAJ_COMP_SZDD)
            {
                self.Error = LZSS.Decompress(sys, fh, outfh, KWAJ_INPUT_SIZE, LZSSMode.LZSS_MODE_EXPAND);
            }
            else if (hdr.CompressionType == CompressionType.MSKWAJ_COMP_LZH)
            {
                InternalStream lzh = LZHInit(sys, fh, outfh);
                self.Error = (lzh != null) ? LZHDecompress(lzh) : Error.MSPACK_ERR_NOMEMORY;
            }
            else if (hdr.CompressionType == CompressionType.MSKWAJ_COMP_MSZIP)
            {
                MSZIPDStream zip = MSZIP.Init(sys, fh, outfh, KWAJ_INPUT_SIZE, false);
                self.Error = (zip != null) ? MSZIP.DecompressKWAJ(zip) : Error.MSPACK_ERR_NOMEMORY;
            }
            else
            {
                self.Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            // Close output file 
            sys.Close(outfh);

            return self.Error;
        }

        #endregion

        #region KWAJD_DECOMPRESS

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

        #region KWAJD_ERROR

        /// <summary>
        /// Returns the last error that occurred
        /// </summary>
        public static Error LastError(Decompressor d)
        {
            DecompressorImpl self = d as DecompressorImpl;
            return (self != null) ? self.Error : Error.MSPACK_ERR_ARGS;
        }

        #endregion

        #region LZH_INIT, LZH_DECOMPRESS, LZH_FREE

        /* In the KWAJ LZH format, there is no special 'eof' marker, it just
         * ends. Depending on how many bits are left in the final byte when
         * the stream ends, that might be enough to start another literal or
         * match. The only easy way to detect that we've come to an end is to
         * guard all bit-reading. We allow fake bits to be read once we reach
         * the end of the stream, but we check if we then consumed any of
         * those fake bits, after doing the READ_BITS / READ_HUFFSYM. This
         * isn't how the default readbits.h read_input() works (it simply lets
         * 2 fake bytes in then stops), so we implement our own.
         */

        private static InternalStream LZHInit(SystemImpl sys, FileStream input, FileStream output)
        {
            if (sys == null || input == null || output == null)
                return null;

            return new InternalStream()
            {
                Sys = sys,
                Input = input,
                Output = output,
            };
        }

        private static Error LZHDecompress(InternalStream lzh)
        {
            uint bit_buffer, len = 0, j = 0;
            int i, bits_left;
            int i_ptr, i_end;
            bool lit_run = false;
            int pos = 0, offset;
            int[] types = new int[6];

            // Reset global state

            //INIT_BITS
            lzh.InputPointer = 0;
            lzh.InputLength = 0;
            lzh.BitBuffer = 0;
            lzh.BitsLeft = 0;
            lzh.InputEnd = 0;

            //RESTORE_BITS
            i_ptr = lzh.InputPointer;
            i_end = lzh.InputLength;
            bit_buffer = lzh.BitBuffer;
            bits_left = lzh.BitsLeft;

            for (i = 0; i < LZSS.LZSS_WINDOW_SIZE; i++)
            {
                lzh.Window[i] = LZSS.LZSS_WINDOW_FILL;
            }

            // Read 6 encoding types (for byte alignment) but only 5 are needed
            for (i = 0; i < 6; i++)
            {
                //READ_BITS_SAFE(types[i], 4)

                //READ_BITS(types[i], 4)
                {
                    //ENSURE_BITS(nbits)
                    while (bits_left < (4))
                    {
                        READ_BYTES;
                    }

                    types[i] = (int)(bit_buffer >> (BITBUF_WIDTH - (4)));

                    // REMOVE_BITS(4);
                    bit_buffer <<= (4);
                    bits_left -= (4);
                }

                if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                    return Error.MSPACK_ERR_OK;
            }

            // Read huffman table symbol lengths and build huffman trees

            //BUILD_TREE(tbl, type)

            //STORE_BITS
            lzh.InputPointer = i_ptr;
            lzh.InputLength = i_end;
            lzh.BitBuffer = bit_buffer;
            lzh.BitsLeft = bits_left;

            Error err = LZHReadLens(lzh, (uint)types[0], KWAJ_MATCHLEN1_SYMS, lzh.MATCHLEN1_len);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            //RESTORE_BITS
            i_ptr = lzh.InputPointer;
            i_end = lzh.InputLength;
            bit_buffer = lzh.BitBuffer;
            bits_left = lzh.BitsLeft;

            if (!InternalStream.MakeDecodeTable(KWAJ_MATCHLEN1_SYMS, KWAJ_MATCHLEN1_TBLSIZE, lzh.MATCHLEN1_len, lzh.MATCHLEN1_table, msb: true))
                return Error.MSPACK_ERR_DATAFORMAT;

            //BUILD_TREE(tbl, type)

            //STORE_BITS
            lzh.InputPointer = i_ptr;
            lzh.InputLength = i_end;
            lzh.BitBuffer = bit_buffer;
            lzh.BitsLeft = bits_left;

            err = LZHReadLens(lzh, (uint)types[1], KWAJ_MATCHLEN2_SYMS, lzh.MATCHLEN2_len);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            //RESTORE_BITS
            i_ptr = lzh.InputPointer;
            i_end = lzh.InputLength;
            bit_buffer = lzh.BitBuffer;
            bits_left = lzh.BitsLeft;

            if (!InternalStream.MakeDecodeTable(KWAJ_MATCHLEN2_SYMS, KWAJ_MATCHLEN2_TBLSIZE, lzh.MATCHLEN2_len, lzh.MATCHLEN2_table, msb: true))
                return Error.MSPACK_ERR_DATAFORMAT;

            //BUILD_TREE(tbl, type)

            //STORE_BITS
            lzh.InputPointer = i_ptr;
            lzh.InputLength = i_end;
            lzh.BitBuffer = bit_buffer;
            lzh.BitsLeft = bits_left;

            err = LZHReadLens(lzh, (uint)types[2], KWAJ_LITLEN_SYMS, lzh.LITLEN_len);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            //RESTORE_BITS
            i_ptr = lzh.InputPointer;
            i_end = lzh.InputLength;
            bit_buffer = lzh.BitBuffer;
            bits_left = lzh.BitsLeft;

            if (!InternalStream.MakeDecodeTable(KWAJ_LITLEN_SYMS, KWAJ_LITLEN_TBLSIZE, lzh.LITLEN_len, lzh.LITLEN_table, msb: true))
                return Error.MSPACK_ERR_DATAFORMAT;

            //BUILD_TREE(tbl, type)

            //STORE_BITS
            lzh.InputPointer = i_ptr;
            lzh.InputLength = i_end;
            lzh.BitBuffer = bit_buffer;
            lzh.BitsLeft = bits_left;

            err = LZHReadLens(lzh, (uint)types[3], KWAJ_OFFSET_SYMS, lzh.OFFSET_len);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            //RESTORE_BITS
            i_ptr = lzh.InputPointer;
            i_end = lzh.InputLength;
            bit_buffer = lzh.BitBuffer;
            bits_left = lzh.BitsLeft;

            if (!InternalStream.MakeDecodeTable(KWAJ_OFFSET_SYMS, KWAJ_OFFSET_TBLSIZE, lzh.OFFSET_len, lzh.OFFSET_table, msb: true))
                return Error.MSPACK_ERR_DATAFORMAT;

            //BUILD_TREE(tbl, type)

            //STORE_BITS
            lzh.InputPointer = i_ptr;
            lzh.InputLength = i_end;
            lzh.BitBuffer = bit_buffer;
            lzh.BitsLeft = bits_left;

            err = LZHReadLens(lzh, (uint)types[4], KWAJ_LITERAL_SYMS, lzh.LITERAL_len);
            if (err != Error.MSPACK_ERR_OK)
                return err;

            //RESTORE_BITS
            i_ptr = lzh.InputPointer;
            i_end = lzh.InputLength;
            bit_buffer = lzh.BitBuffer;
            bits_left = lzh.BitsLeft;

            if (!InternalStream.MakeDecodeTable(KWAJ_LITERAL_SYMS, KWAJ_LITERAL_TBLSIZE, lzh.LITERAL_len, lzh.LITERAL_table, msb: true))
                return Error.MSPACK_ERR_DATAFORMAT;

            while (lzh.InputEnd == 0)
            {
                if (lit_run)
                {
                    //READ_HUFFSYM_SAFE(tbl, val)
                    if (lzh.READ_HUFFSYM(lzh.MATCHLEN2_table, ref len, KWAJ_MATCHLEN2_TBLSIZE, lzh.MATCHLEN2_len, KWAJ_MATCHLEN2_SYMS, ref i, ref i_ptr, ref i_end, ref bits_left, ref bit_buffer, msb: true) != 0)
                        return Error.MSPACK_ERR_DECRUNCH;
                    if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                        return Error.MSPACK_ERR_OK;
                }
                else
                {
                    //READ_HUFFSYM_SAFE(tbl, val)
                    if (lzh.READ_HUFFSYM(lzh.MATCHLEN1_table, ref len, KWAJ_MATCHLEN1_TBLSIZE, lzh.MATCHLEN1_len, KWAJ_MATCHLEN1_SYMS, ref i, ref i_ptr, ref i_end, ref bits_left, ref bit_buffer, msb: true) != 0)
                        return Error.MSPACK_ERR_DECRUNCH;
                    if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                        return Error.MSPACK_ERR_OK;
                }

                if (len > 0)
                {
                    len += 2;
                    lit_run = false; // Not the end of a literal run

                    //READ_HUFFSYM_SAFE(tbl, val)
                    if (lzh.READ_HUFFSYM(lzh.OFFSET_table, ref j, KWAJ_OFFSET_TBLSIZE, lzh.OFFSET_len, KWAJ_OFFSET_SYMS, ref i, ref i_ptr, ref i_end, ref bits_left, ref bit_buffer, msb: true) != 0)
                        return Error.MSPACK_ERR_DECRUNCH;
                    if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                        return Error.MSPACK_ERR_OK;

                    offset = (int)(j << 6);

                    //READ_BITS_SAFE(j, 6)

                    //READ_BITS(j, 6)
                    {
                        //ENSURE_BITS(6)
                        while (bits_left < (6))
                        {
                            READ_BYTES;
                        }

                        j = (int)(bit_buffer >> (BITBUF_WIDTH - (6)));

                        // REMOVE_BITS(6);
                        bit_buffer <<= (6);
                        bits_left -= (6);
                    }

                    if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                        return Error.MSPACK_ERR_OK;

                    offset |= j;

                    // Copy match as output and into the ring buffer
                    while (len-- > 0)
                    {
                        lzh.Window[pos] = lzh.Window[(pos + 4096 - offset) & 4095];
                        if (lzh.Sys.Write(lzh.Output, lzh.Window, pos, 1) != 1)
                            return Error.MSPACK_ERR_WRITE;

                        pos++; pos &= 4095;
                    }
                }
                else
                {
                    //READ_HUFFSYM_SAFE(tbl, val)
                    if (lzh.READ_HUFFSYM(lzh.LITLEN_table, ref len, KWAJ_LITLEN_TBLSIZE, lzh.LITLEN_len, KWAJ_LITLEN_SYMS, ref i, ref i_ptr, ref i_end, ref bits_left, ref bit_buffer, msb: true) != 0)
                        return Error.MSPACK_ERR_DECRUNCH;
                    if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                        return Error.MSPACK_ERR_OK;

                    len++;

                    lit_run = (len == 32) ? false : true; // End of a literal run?
                    while (len-- > 0)
                    {
                        //READ_HUFFSYM_SAFE(tbl, val)
                        if (lzh.READ_HUFFSYM(lzh.LITERAL_table, ref j, KWAJ_LITERAL_TBLSIZE, lzh.LITERAL_len, KWAJ_LITERAL_SYMS, ref i, ref i_ptr, ref i_end, ref bits_left, ref bit_buffer, msb: true) != 0)
                            return Error.MSPACK_ERR_DECRUNCH;
                        if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                            return Error.MSPACK_ERR_OK;

                        // Copy as output and into the ring buffer
                        lzh.Window[pos] = (byte)j;
                        if (lzh.Sys.Write(lzh.Output, lzh.Window, pos, 1) != 1)
                            return Error.MSPACK_ERR_WRITE;

                        pos++;
                        pos &= 4095;
                    }
                }
            }

            return Error.MSPACK_ERR_OK;
        }

        public static Error LZHReadLens(InternalStream lzh, uint type, uint numsyms, byte[] lens)
        {
            uint bit_buffer;
            int bits_left;
            int i_ptr, i_end;
            uint i;
            int c = 0, sel = 0;

            //RESTORE_BITS
            i_ptr = lzh.InputPointer;
            i_end = lzh.InputLength;
            bit_buffer = lzh.BitBuffer;
            bits_left = lzh.BitsLeft;

            switch (type)
            {
                case 0:
                    i = numsyms;
                    c = (i == 16) ? 4 : (i == 32) ? 5 : (i == 64) ? 6 : (i == 256) ? 8 : 0;
                    for (i = 0; i < numsyms; i++)
                    {
                        lens[i] = (byte)c;
                    }

                    break;

                case 1:
                    //READ_BITS_SAFE(c, 4)

                    //READ_BITS(c, 4)
                    {
                        //ENSURE_BITS(4)
                        while (bits_left < (4))
                        {
                            READ_BYTES;
                        }

                        c = (int)(bit_buffer >> (BITBUF_WIDTH - (4)));

                        // REMOVE_BITS(4);
                        bit_buffer <<= (4);
                        bits_left -= (4);
                    }

                    if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                        return Error.MSPACK_ERR_OK;

                    lens[0] = (byte)c;
                    for (i = 1; i < numsyms; i++)
                    {
                        //READ_BITS_SAFE(sel, 1)

                        //READ_BITS(sel, 1)
                        {
                            //ENSURE_BITS(1)
                            while (bits_left < (1))
                            {
                                READ_BYTES;
                            }

                            sel = (int)(bit_buffer >> (BITBUF_WIDTH - (1)));

                            // REMOVE_BITS(1);
                            bit_buffer <<= (1);
                            bits_left -= (1);
                        }

                        if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                            return Error.MSPACK_ERR_OK;

                        if (sel == 0)
                        {
                            lens[i] = (byte)c;
                        }
                        else
                        {
                            //READ_BITS_SAFE(sel, 1)

                            //READ_BITS(sel, 1)
                            {
                                //ENSURE_BITS(1)
                                while (bits_left < (1))
                                {
                                    READ_BYTES;
                                }

                                sel = (int)(bit_buffer >> (BITBUF_WIDTH - (1)));

                                // REMOVE_BITS(1);
                                bit_buffer <<= (1);
                                bits_left -= (1);
                            }

                            if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                                return Error.MSPACK_ERR_OK;

                            if (sel == 0)
                            {
                                lens[i] = (byte)++c;
                            }
                            else
                            {
                                //READ_BITS_SAFE(c, 4)

                                //READ_BITS(c, 4)
                                {
                                    //ENSURE_BITS(nbits)
                                    while (bits_left < (4))
                                    {
                                        READ_BYTES;
                                    }

                                    c = (int)(bit_buffer >> (BITBUF_WIDTH - (4)));

                                    // REMOVE_BITS(4);
                                    bit_buffer <<= (4);
                                    bits_left -= (4);
                                }

                                if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                                    return Error.MSPACK_ERR_OK;

                                lens[i] = (byte)c;
                            }
                        }
                    }
                    break;

                case 2:
                    //READ_BITS_SAFE(c, 4)

                    //READ_BITS(c, 4)
                    {
                        //ENSURE_BITS(4)
                        while (bits_left < (4))
                        {
                            READ_BYTES;
                        }

                        c = (int)(bit_buffer >> (BITBUF_WIDTH - (4)));

                        // REMOVE_BITS(4);
                        bit_buffer <<= (4);
                        bits_left -= (4);
                    }

                    if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                        return Error.MSPACK_ERR_OK;

                    lens[0] = (byte)c;
                    for (i = 1; i < numsyms; i++)
                    {
                        //READ_BITS_SAFE(sel, 2)

                        //READ_BITS(sel, 2)
                        {
                            //ENSURE_BITS(2)
                            while (bits_left < (2))
                            {
                                READ_BYTES;
                            }

                            sel = (int)(bit_buffer >> (BITBUF_WIDTH - (2)));

                            // REMOVE_BITS(2);
                            bit_buffer <<= (2);
                            bits_left -= (2);
                        }

                        if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                            return Error.MSPACK_ERR_OK;

                        if (sel == 3)
                        {
                            //READ_BITS_SAFE(c, 4)

                            //READ_BITS(c, 4)
                            {
                                //ENSURE_BITS(4)
                                while (bits_left < (4))
                                {
                                    READ_BYTES;
                                }

                                c = (int)(bit_buffer >> (BITBUF_WIDTH - (4)));

                                // REMOVE_BITS(4);
                                bit_buffer <<= (4);
                                bits_left -= (4);
                            }

                            if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                                return Error.MSPACK_ERR_OK;
                        }
                        else
                        {
                            c += (char)sel - 1;
                        }

                        lens[i] = (byte)c;
                    }

                    break;

                case 3:
                    for (i = 0; i < numsyms; i++)
                    {
                        //READ_BITS_SAFE(c, 4)

                        //READ_BITS(c, 4)
                        {
                            //ENSURE_BITS(4)
                            while (bits_left < (4))
                            {
                                READ_BYTES;
                            }

                            c = (int)(bit_buffer >> (BITBUF_WIDTH - (4)));

                            // REMOVE_BITS(4);
                            bit_buffer <<= (4);
                            bits_left -= (4);
                        }

                        if (lzh.InputEnd != 0 && bits_left < lzh.InputEnd)
                            return Error.MSPACK_ERR_OK;

                        lens[i] = (byte)c;
                    }

                    break;
            }

            //STORE_BITS
            lzh.InputPointer = i_ptr;
            lzh.InputLength = i_end;
            lzh.BitBuffer = bit_buffer;
            lzh.BitsLeft = bits_left;

            return Error.MSPACK_ERR_OK;
        }

        public static Error LZHReadInput(InternalStream lzh)
        {
            int read;
            if (lzh.InputEnd != 0)
            {
                lzh.InputEnd += 8;
                lzh.InputBuffer[0] = 0;
                read = 1;
            }
            else
            {
                read = lzh.Sys.Read(lzh.Input, lzh.InputBuffer, 0, KWAJ_INPUT_SIZE);
                if (read < 0)
                    return Error.MSPACK_ERR_READ;

                if (read == 0)
                {
                    lzh.InputLength = 8;
                    lzh.InputBuffer[0] = 0;
                    read = 1;
                }
            }

            // Update InputPointer and InputLength
            lzh.InputPointer = 0;
            lzh.InputLength = read;
            return Error.MSPACK_ERR_OK;
        }

        #endregion
    }
}
