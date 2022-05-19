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

namespace LibMSPackSharp.KWAJ
{
    /// <summary>
    /// A decompressor for KWAJ compressed files.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateKWAJDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyKWAJDecompressor(Decompressor)"/>
    public class Decompressor
    {
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
        /// <param name="self">
        /// a self-referential pointer to the mskwaj_decompressor
        /// instance being called
        /// </param>
        /// <param name="filename">
        /// the filename of the KWAJ compressed file. This is
        /// passed directly to mspack_system::open().
        /// </param>
        /// <returns>a pointer to a mskwajd_header structure, or NULL on failure</returns>
        /// <see cref="Close"/>
        public Func<Decompressor, string, Header> Open;

        /// <summary>
        /// Closes a previously opened KWAJ file.
        ///
        /// This closes a KWAJ file and frees the mskwajd_header associated
        /// with it. The KWAJ header pointer is now invalid and cannot be
        /// used again.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the mskwaj_decompressor
        /// instance being called
        /// </param>
        /// <param name="kwaj">the KWAJ file to close</param>
        /// <see cref="Open"/>
        public Action<Decompressor, Header> Close;

        /// <summary>
        /// Extracts the compressed data from a KWAJ file.
        ///
        /// This decompresses the compressed KWAJ data stream and writes it to
        /// an output file.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the mskwaj_decompressor
        /// instance being called
        /// </param>
        /// <param name="kwaj">the KWAJ file to extract data from</param>
        /// <param name="filename">
        /// the filename to write the decompressed data to. This
        /// is passed directly to mspack_system::open().
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Func<Decompressor, Header, string, Error> Extract;

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
        public Func<Decompressor, string, string, Error> Decompress;

        /// <summary>
        /// Returns the error code set by the most recently called method.
        /// 
        /// This is useful for open() which does not return an
        /// error code directly.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the mskwaj_decompressor
        /// instance being called
        /// </param>
        /// <returns>the most recent error code</returns>
        /// <see cref="Open"/>
        /// <see cref="Search"/>
        public Func<Decompressor, Error> LastError;
    }
}
