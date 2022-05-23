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
using System.Linq;
using LibMSPackSharp.Compression;
using static LibMSPackSharp.Constants;

namespace LibMSPackSharp.SZDD
{
    /// <summary>
    /// A decompressor for SZDD compressed files.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateSZDDDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroySZDDDecompressor(Decompressor)"/>
    public class Decompressor : BaseDecompressor
    {
        #region Public Functionality

        /// <summary>
        /// Opens a SZDD file and reads the header.
        ///
        /// If the file opened is a valid SZDD file, all headers will be read and
        /// a msszddd_header structure will be returned.
        ///
        /// In the case of an error occuring, NULL is returned and the error code
        /// is available from last_error().
        /// 
        /// The filename pointer should be considered "in use" until close() is
        /// called on the SZDD file.
        /// </summary>
        /// <param name="filename">
        /// The filename of the SZDD compressed file. This is
        /// passed directly to mspack_system::open().
        /// </param>
        /// <returns>A pointer to a msszddd_header structure, or NULL on failure</returns>
        /// <see cref="Close(Header)"/>
        public Header Open(string filename)
        {
            FileStream fh = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_READ);
            Header hdr = new Header();
            if (fh != null && hdr != null)
            {
                hdr.FileHandle = fh;
                Error = ReadHeaders(fh, hdr);
            }
            else
            {
                if (fh == null)
                    Error = Error.MSPACK_ERR_OPEN;
                if (hdr == null)
                    Error = Error.MSPACK_ERR_NOMEMORY;
            }

            if (Error != Error.MSPACK_ERR_OK)
            {
                System.Close(fh);
                hdr = null;
            }

            return hdr;
        }

        /// <summary>
        /// Closes a previously opened SZDD file.
        ///
        /// This closes a SZDD file and frees the msszddd_header associated with
        /// it.
        ///
        /// The SZDD header pointer is now invalid and cannot be used again.
        /// </summary>
        /// <param name="szdd">The SZDD file to close</param>
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
        /// Extracts the compressed data from a SZDD file.
        ///
        /// This decompresses the compressed SZDD data stream and writes it to
        /// an output file.
        /// </summary>
        /// <param name="hdr">The SZDD file to extract data from</param>
        /// <param name="filename">
        /// The filename to write the decompressed data to. This
        /// is passed directly to mspack_system::open().
        /// </param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public Error Extract(Header hdr, string filename)
        {
            if (hdr == null)
                return Error = Error.MSPACK_ERR_ARGS;

            FileStream fh = hdr.FileHandle;
            if (fh == null)
                return Error.MSPACK_ERR_ARGS;

            // Seek to the compressed data
            long dataOffset = (hdr.Format == Format.MSSZDD_FMT_NORMAL) ? 14 : 12;
            if (System.Seek(fh, dataOffset, SeekMode.MSPACK_SYS_SEEK_START))
                return Error = Error.MSPACK_ERR_SEEK;

            // Open file for output
            FileStream outfh = System.Open(filename, OpenMode.MSPACK_SYS_OPEN_WRITE);
            if (outfh == null)
                return Error = Error.MSPACK_ERR_OPEN;

            // Decompress the data
            Error = LZSS.Decompress(
                System,
                fh,
                outfh,
                SZDD_INPUT_SIZE,
                hdr.Format == Format.MSSZDD_FMT_NORMAL
                    ? LZSSMode.LZSS_MODE_EXPAND
                    : LZSSMode.LZSS_MODE_QBASIC);

            // Close output file
            System.Close(outfh);

            return Error;
        }

        /// <summary>
        /// Decompresses an SZDD file to an output file in one step.
        ///
        /// This opens an SZDD file as input, reads the header, then decompresses
        /// the compressed data immediately to an output file, finally closing
        /// both the input and output file. It is more convenient to use than
        /// open() then extract() then close(), if you do not need to know the
        /// SZDD output size or missing character.
        /// </summary>
        /// <param name="input">
        /// The filename of the input SZDD file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="output">
        /// The filename to write the decompressed data to. This
        /// is passed directly to mspack_system::open().
        /// </param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public Error Decompress(string input, string output)
        {
            Header hdr = Open(input) as Header;
            if (hdr == null)
                return Error;

            Error error = Extract(hdr, output);
            Close(hdr);
            return Error = error;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Reads the headers of an SZDD format file
        /// </summary>
        private Error ReadHeaders(FileStream fh, Header hdr)
        {
            // Read and check signature
            byte[] buf = new byte[8];
            if (System.Read(fh, buf, 0, 8) != 8)
                return Error.MSPACK_ERR_READ;

            if (buf.SequenceEqual(expandSignature))
            {
                // Common SZDD
                hdr.Format = Format.MSSZDD_FMT_NORMAL;

                // Read the rest of the header
                if (System.Read(fh, buf, 0, 6) != 6)
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
                if (System.Read(fh, buf, 0, 4) != 4)
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
    }
}
