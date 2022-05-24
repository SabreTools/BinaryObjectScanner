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

namespace LibMSPackSharp.KWAJ
{
    /// <summary>
    /// A decompressor for KWAJ compressed files.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateKWAJDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyKWAJDecompressor(Decompressor)"/>
    public class Decompressor : BaseDecompressor
    {
        #region Public Functionality

        /// <summary>
        /// Opens a KWAJ file and reads the header.
        ///
        /// If the file opened is a valid KWAJ file, all headers will be read and
        /// a mskwajd_header structure will be returned.
        ///
        /// In the case of an error occuring, NULL is returned and the error code
        /// is available from last_error().
        ///
        /// The filename pointer should be considered "in use" until close() is
        /// called on the KWAJ file.
        /// </summary>
        /// <param name="filename">
        /// the filename of the KWAJ compressed file. This is
        /// passed directly to mspack_system::open().
        /// </param>
        /// <returns>A pointer to a mskwajd_header structure, or NULL on failure</returns>
        /// <see cref="Close(Header)"/>
        public Header Open(string filename)
        {
            FileStream fh = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_READ);
            if (fh == null)
            {
                Error = Error.MSPACK_ERR_OPEN;
                return null;
            }

            Header hdr = new Header() { FileHandle = fh };

            Error = ReadHeaders(fh, hdr);
            if (Error != Error.MSPACK_ERR_OK)
            {
                System.Close(fh);
                hdr = null;
            }

            return hdr;
        }

        /// <summary>
        /// Closes a previously opened KWAJ file.
        ///
        /// This closes a KWAJ file and frees the mskwajd_header associated
        /// with it. The KWAJ header pointer is now invalid and cannot be
        /// used again.
        /// </summary>
        /// <param name="hdr">The KWAJ file to close</param>
        /// <see cref="Open(string)"/>
        public void Close(Header hdr)
        {
            if (System == null || hdr == null)
                return;

            // Close the file handle associated
            System.Close(hdr.FileHandle);

            Error = Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Extracts the compressed data from a KWAJ file.
        ///
        /// This decompresses the compressed KWAJ data stream and writes it to
        /// an output file.
        /// </summary>
        /// <param name="hdr">The KWAJ file to extract data from</param>
        /// <param name="filename">
        /// the filename to write the decompressed data to. This
        /// is passed directly to mspack_system::open().
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Error Extract(Header hdr, string filename)
        {
            FileStream fh = hdr?.FileHandle;
            if (fh == null)
                return Error.MSPACK_ERR_ARGS;

            // Seek to the compressed data
            if (System.Seek(fh, hdr.KWAJHeader.DataOffset, SeekMode.MSPACK_SYS_SEEK_START))
                return Error = Error.MSPACK_ERR_SEEK;

            // Open file for output
            FileStream outfh;
            if ((outfh = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_WRITE)) == null)
                return Error = Error.MSPACK_ERR_OPEN;

            Error = Error.MSPACK_ERR_OK;

            // Decompress based on format
            if (hdr.KWAJHeader.CompressionType == CompressionType.MSKWAJ_COMP_NONE ||
                hdr.KWAJHeader.CompressionType == CompressionType.MSKWAJ_COMP_XOR)
            {
                // NONE is a straight copy. XOR is a copy xored with 0xFF
                byte[] buf = new byte[KWAJ_INPUT_SIZE];

                int read, i;
                while ((read = System.Read(fh, buf, 0, KWAJ_INPUT_SIZE)) > 0)
                {
                    if (hdr.KWAJHeader.CompressionType == CompressionType.MSKWAJ_COMP_XOR)
                    {
                        for (i = 0; i < read; i++)
                        {
                            buf[i] ^= 0xFF;
                        }
                    }

                    if (System.Write(outfh, buf, 0, read) != read)
                    {
                        Error = Error.MSPACK_ERR_WRITE;
                        break;
                    }
                }

                if (read < 0)
                    Error = Error.MSPACK_ERR_READ;
            }
            else if (hdr.KWAJHeader.CompressionType == CompressionType.MSKWAJ_COMP_SZDD)
            {
                Error = LZSS.Decompress(System, fh, outfh, KWAJ_INPUT_SIZE, LZSSMode.LZSS_MODE_EXPAND);
            }
            else if (hdr.KWAJHeader.CompressionType == CompressionType.MSKWAJ_COMP_LZH)
            {
                LZHKWAJ lzh = LZHKWAJ.Init(System, fh, outfh);
                Error = (lzh != null) ? lzh.Decompress() : Error.MSPACK_ERR_NOMEMORY;
            }
            else if (hdr.KWAJHeader.CompressionType == CompressionType.MSKWAJ_COMP_MSZIP)
            {
                MSZIP zip = MSZIP.Init(System, fh, outfh, KWAJ_INPUT_SIZE, false);
                Error = (zip != null) ? zip.DecompressKWAJ() : Error.MSPACK_ERR_NOMEMORY;
            }
            else
            {
                Error = Error.MSPACK_ERR_DATAFORMAT;
            }

            // Close output file 
            System.Close(outfh);

            return Error;
        }

        /// <summary>
        /// Decompresses an KWAJ file to an output file in one step.
        ///
        /// This opens an KWAJ file as input, reads the header, then decompresses
        /// the compressed data immediately to an output file, finally closing
        /// both the input and output file. It is more convenient to use than
        /// open() then extract() then close(), if you do not need to know the
        /// KWAJ output size or output filename.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the mskwaj_decompressor
        /// instance being called
        /// </param>
        /// <param name="input">
        /// the filename of the input KWAJ file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="output">
        /// the filename to write the decompressed data to. This
        /// is passed directly to mspack_system::open().
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Error Decompress(string input, string output)
        {
            Header hdr = Open(input);
            if (hdr == null)
                return Error;

            Error error = Extract(hdr, output);
            Close(hdr);
            return Error = error;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Reads the headers of a KWAJ format file
        /// </summary>
        private Error ReadHeaders(FileStream fh, Header hdr)
        {
            // Read in the header
            byte[] buf = new byte[16];
            if (System.Read(fh, buf, 0, _KWAJHeader.Size) != _KWAJHeader.Size)
                return Error.MSPACK_ERR_READ;

            // Create a new header based on that
            Error err = _KWAJHeader.Create(buf, out _KWAJHeader kwajHeader);
            if (err != Error.MSPACK_ERR_OK)
                return Error = err;

            // Assign the header
            hdr.KWAJHeader = kwajHeader;

            // Basic header fields
            hdr.Length = 0;
            hdr.Filename = null;
            hdr.Extra = null;
            hdr.ExtraLength = 0;

            // Optional headers

            // 4 bytes: length of unpacked file
            if (hdr.KWAJHeader.Flags.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASLENGTH))
            {
                if (System.Read(fh, buf, 0, 4) != 4)
                    return Error.MSPACK_ERR_READ;

                hdr.Length = BitConverter.ToUInt32(buf, 0);
            }

            // 2 bytes: unknown purpose
            if (hdr.KWAJHeader.Flags.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASUNKNOWN1))
            {
                if (System.Read(fh, buf, 0, 2) != 2)
                    return Error.MSPACK_ERR_READ;
            }

            // 2 bytes: length of section, then [length] bytes: unknown purpose
            if (hdr.KWAJHeader.Flags.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASUNKNOWN2))
            {
                if (System.Read(fh, buf, 0, 2) != 2)
                    return Error.MSPACK_ERR_READ;

                int i = BitConverter.ToUInt16(buf, 0);
                if (System.Seek(fh, i, SeekMode.MSPACK_SYS_SEEK_CUR))
                    return Error.MSPACK_ERR_SEEK;
            }

            // Filename and extension
            if (hdr.KWAJHeader.Flags.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASFILENAME) || hdr.KWAJHeader.Flags.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASFILEEXT))
            {
                int len;

                // Allocate memory for maximum length filename
                char[] fn = new char[13];
                int fnPtr = 0;

                // Copy filename if present
                if (hdr.KWAJHeader.Flags.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASFILENAME))
                {
                    // Read and copy up to 9 bytes of a null terminated string
                    if ((len = System.Read(fh, buf, 0, 9)) < 2)
                        return Error.MSPACK_ERR_READ;

                    int i = 0;
                    for (; i < len; i++)
                    {
                        if ((fn[fnPtr++] = (char)buf[i]) == '\0')
                            break;
                    }

                    // If string was 9 bytes with no null terminator, reject it
                    if (i == 9 && buf[8] != '\0')
                        return Error.MSPACK_ERR_DATAFORMAT;

                    // Seek to byte after string ended in file
                    if (System.Seek(fh, i + 1 - len, SeekMode.MSPACK_SYS_SEEK_CUR))
                        return Error.MSPACK_ERR_SEEK;

                    fnPtr--; // Remove the null terminator
                }

                // Copy extension if present
                if (hdr.KWAJHeader.Flags.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASFILEEXT))
                {
                    fn[fnPtr++] = '.';

                    // Read and copy up to 4 bytes of a null terminated string
                    if ((len = System.Read(fh, buf, 0, 4)) < 2)
                        return Error.MSPACK_ERR_READ;

                    int i = 0;
                    for (; i < len; i++)
                    {
                        if ((fn[fnPtr++] = (char)buf[i]) == '\0')
                            break;
                    }

                    // If string was 4 bytes with no null terminator, reject it
                    if (i == 4 && buf[3] != '\0')
                        return Error.MSPACK_ERR_DATAFORMAT;

                    // Seek to byte after string ended in file
                    if (System.Seek(fh, i + 1 - len, SeekMode.MSPACK_SYS_SEEK_CUR))
                        return Error.MSPACK_ERR_SEEK;

                    fnPtr--; // Remove the null terminator
                }

                fn[fnPtr] = '\0';
            }

            // 2 bytes: extra text length then [length] bytes of extra text data
            if (hdr.KWAJHeader.Flags.HasFlag(OptionalHeaderFlag.MSKWAJ_HDR_HASEXTRATEXT))
            {
                if (System.Read(fh, buf, 0, 2) != 2)
                    return Error.MSPACK_ERR_READ;

                int i = BitConverter.ToUInt16(buf, 0);
                byte[] extra = new byte[i + 1];
                if (System.Read(fh, extra, 0, i) != i)
                    return Error.MSPACK_ERR_READ;

                extra[i] = 0x00;
                hdr.Extra = Encoding.ASCII.GetString(extra, 0, extra.Length);
                hdr.ExtraLength = (ushort)i;
            }

            return Error.MSPACK_ERR_OK;
        }

        #endregion
    }
}
