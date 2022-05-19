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

namespace LibMSPackSharp.CHM
{
    /// <summary>
    /// A decompressor for .CHM (Microsoft HTMLHelp) files
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateCHMDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyCHMDecompressor(Decompressor)"/>
    public class Decompressor
    {
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
        /// <param name="self">
        /// a self-referential pointer to the mschm_decompressor
        /// instance being called
        /// </param>
        /// <param name="filename">
        /// the filename of the CHM helpfile. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>a pointer to a mschmd_header structure, or NULL on failure</returns>
        /// <see cref="Close"/>
        public Func<Decompressor, string, Header> Open;

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
        /// <param name="self">
        /// a self-referential pointer to the mschm_decompressor
        /// instance being called
        /// </param>
        /// <param name="chm">the CHM helpfile to close</param>
        /// <see cref="Open"/>
        /// <see cref="FastOpen"/>
        public Action<Decompressor, Header> Close;

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
        /// <param name="self">
        /// a self-referential pointer to the mschm_decompressor
        /// instance being called
        /// </param>
        /// <param name="file">the file to be decompressed</param>
        /// <param name="filename">the filename of the file being written to</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Func<Decompressor, DecompressFile, string, Error> Extract;

        /// <summary>
        /// Returns the error code set by the most recently called method.
        ///
        /// This is useful for open() and fast_open(), which do not return an
        /// error code directly.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the mschm_decompressor
        /// instance being called
        /// </param>
        /// <returns>the most recent error code</returns>
        /// <see cref="Open"/>
        /// <see cref="Extract"/>
        public Func<Decompressor, Error> LastError;

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
        /// <param name="self">
        /// a self-referential pointer to the mschm_decompressor
        /// instance being called
        /// </param>
        /// <param name="filename">
        /// the filename of the CHM helpfile. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>a pointer to a mschmd_header structure, or NULL on failure</returns>
        /// <see cref="Open"/>
        /// <see cref="Close"/>
        /// <see cref="FastFind"/>
        /// <see cref="Extract"/>
        public Func<Decompressor, string, Header> FastOpen;

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
        /// <param name="self">
        /// a self-referential pointer to the mschm_decompressor
        /// instance being called
        /// </param>
        /// <param name="chm">the CHM helpfile to search for the file</param>
        /// <param name="filename">the filename of the file to search for</param>
        /// <param name="f_ptr">a pointer to a caller-provded mschmd_file structure</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="Open"/>
        /// <see cref="Close"/>
        /// <see cref="FastFind"/>
        /// <see cref="Extract"/>
        public Func<Decompressor, Header, string, DecompressFile, Error> FastFind;
    }
}
