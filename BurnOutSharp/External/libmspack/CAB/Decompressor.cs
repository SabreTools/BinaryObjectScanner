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

namespace LibMSPackSharp.CAB
{
    /// <summary>
    /// A decompressor for .CAB (Microsoft Cabinet) files
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateCABDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyCABDecompressor(Decompressor)"/>
    public class Decompressor
    {
        /// <summary>
        /// Opens a cabinet file and reads its contents.
        /// 
        /// If the file opened is a valid cabinet file, all headers will be read
        /// and a Cabinet structure will be returned, with a full list of
        /// folders and files.
        /// 
        /// In the case of an error occuring, NULL is returned and the error code
        /// is available from last_error().
        /// 
        /// The filename pointer should be considered "in use" until close() is
        /// called on the cabinet.
        /// </summary>
        /// <param name="decompressor">A self-referential pointer to the Decompressor instance being called</param>
        /// <param name="filename">The filename of the cabinet file. This is passed directly to SystemImpl::open().</param>
        /// <returns>A pointer to a Cabinet structure, or NULL on failure</returns>
        /// <see cref="Close"/>
        /// <see cref="Search"/>
        /// <see cref="LastError"/>
        public Func<Decompressor, string, Cabinet> Open;

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
        /// <param name="decompressor">A self-referential pointer to the Decompressor instance being called</param>
        /// <param name="cab">The cabinet to close</param>
        /// <see cref="Open"/>
        /// <see cref="Search"/>
        /// <see cref="Append"/>
        /// <see cref="Prepend"/>
        public Action<Decompressor, Cabinet> Close;

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
        /// open(), NULL is returned and the error code is available from
        /// last_error().
        /// 
        /// If no error occurs, but no cabinets can be found in the file, NULL is
        /// returned and last_error() returns MSPACK_ERR_OK.
        /// 
        /// The filename pointer should be considered in use until close() is
        /// called on the cabinet.
        /// 
        /// close() should only be called on the result of search(), not on any
        /// subsequent cabinets in the Cabinet::next chain.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the Decompressor
        /// instance being called
        /// </param>
        /// <param name="filename">
        /// the filename of the file to search for cabinets. This
        /// is passed directly to SystemImpl::open().
        /// </param>
        /// <returns>a pointer to a Cabinet structure, or NULL</returns>
        /// <see cref="Close"/>
        /// <see cref="Open"/>
        /// <see cref="LastError"/>
        public Func<Decompressor, string, Cabinet> Search;

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
        /// <param name="self">
        /// a self-referential pointer to the Decompressor
        /// instance being called
        /// </param>
        /// <param name="cab">
        /// the cabinet which will be appended to,
        /// predecessor of nextcab
        /// </param>
        /// <param name="nextcab">
        /// the cabinet which will be appended,
        /// successor of cab
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="Prepend"/>
        /// <see cref="Open"/>
        /// <see cref="Close"/>
        public Func<Decompressor, Cabinet, Cabinet, Error> Append;

        /// <summary>
        /// Prepends one Cabinet to another, forming or extending a
        /// cabinet set.
        /// 
        /// This will attempt to prepend one cabinet to another, such that
        /// <tt>(cab->prevcab == prevcab) && (prevcab->nextcab == cab)</tt>. In
        /// all other respects, it is identical to append(). See append() for the
        /// full documentation.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the Decompressor
        /// instance being called
        /// </param>
        /// <param name="cab">
        /// the cabinet which will be prepended to,
        /// successor of nextcab
        /// </param>
        /// <param name="nextcab">
        /// the cabinet which will be prepended,
        /// predecessor of cab
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="Append"/>
        /// <see cref="Open"/>
        /// <see cref="Close"/>
        public Func<Decompressor, Cabinet, Cabinet, Error> Prepend;

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
        /// <param name="self">
        /// a self-referential pointer to the Decompressor
        /// instance being called
        /// </param>
        /// <param name="file">the file to be decompressed</param>
        /// <param name="filename">the filename of the file being written to</param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Func<Decompressor, InternalFile, string, Error> Extract;

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
        /// <param name="self">
        /// a self-referential pointer to the Decompressor
        /// instance being called
        /// </param>
        /// <param name="param">the parameter to set</param>
        /// <param name="value">the value to set the parameter to</param>
        /// <returns>
        /// MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS if there
        /// is a problem with either parameter or value.
        /// </returns>
        /// <see cref="Search"/>
        /// <see cref="Extract"/>
        public Func<Decompressor, Parameters, int, Error> SetParam;

        /// <summary>
        /// Returns the error code set by the most recently called method.
        /// 
        /// This is useful for open() and search(), which do not return an error
        /// code directly.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the Decompressor
        /// instance being called
        /// </param>
        /// <returns>the most recent error code</returns>
        /// <see cref="Open"/>
        /// <see cref="Search"/>
        public Func<Decompressor, Error> LastError;
    }
}
