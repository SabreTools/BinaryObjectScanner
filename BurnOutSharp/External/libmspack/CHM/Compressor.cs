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
    /// A compressor for .CHM (Microsoft HTMLHelp) files.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateCHMCompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyCHMCompressor(Compressor)"/>
    public class Compressor
    {
        #region Fields

        public SystemImpl System { get; set; }

        public string TempFile { get; set; }

        public bool UseTempFile { get; set; }

        public Error Error { get; set; }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Generates a CHM help file.
        /// 
        /// The help file will contain up to two sections, an Uncompressed
        /// section and potentially an MSCompressed (LZX compressed)
        /// section.
        /// 
        /// While the contents listing of a CHM file is always in lexical order,
        /// the file list passed in will be taken as the correct order for files
        /// within the sections.  It is in your interest to place similar files
        /// together for better compression.
        /// 
        /// There are two modes of generation, to use a temporary file or not to
        /// use one. See use_temporary_file() for the behaviour of generate() in
        /// these two different modes.
        /// </summary>
        /// <param name="fileList">
        /// an array of mschmc_file structures, terminated
        /// with an entry whose mschmc_file::section field is
        /// #MSCHMC_ENDLIST. The order of the list is
        /// preserved within each section. The length of any
        /// mschmc_file::chm_filename string cannot exceed
        /// roughly 4096 bytes. Each source file must be able
        /// to supply as many bytes as given in the
        /// mschmc_file::length field.
        /// </param>
        /// <param name="outputFile">
        /// the file to write the generated CHM helpfile to.
        /// This is passed directly to mspack_system::open()
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="UseTemporaryFile"/>
        /// <see cref="SetParam"/>
        public Error Generate(CompressFile[] fileList, string outputFile) => throw new NotImplementedException();

        /// <summary>
        /// Specifies whether a temporary file is used during CHM generation.
        /// 
        /// The CHM file format includes data about the compressed section (such
        /// as its overall size) that is stored in the output CHM file prior to
        /// the compressed section itself. This unavoidably requires that the
        /// compressed section has to be generated, before these details can be
        /// set. There are several ways this can be handled. Firstly, the
        /// compressed section could be generated entirely in memory before
        /// writing any of the output CHM file.This approach is not used in
        /// libmspack, as the compressed section can exceed the addressable
        /// memory space on most architectures.
        ///
        /// libmspack has two options, either to write these unknowable sections
        /// with blank data, generate the compressed section, then re-open the
        /// output file for update once the compressed section has been
        /// completed, or to write the compressed section to a temporary file,
        /// then write the entire output file at once, performing a simple
        /// file-to-file copy for the compressed section.
        ///
        /// The simple solution of buffering the entire compressed section in
        /// memory can still be used, if desired.As the temporary file's
        /// filename is passed directly to mspack_system::open(), it is possible
        /// for a custom mspack_system implementation to hold this file in memory,
        /// without writing to a disk.
        ///
        /// If a temporary file is set, generate() performs the following
        /// sequence of events: the temporary file is opened for writing, the
        /// compression algorithm writes to the temporary file, the temporary
        /// file is closed.Then the output file is opened for writing and the
        /// temporary file is re-opened for reading.The output file is written
        /// and the temporary file is read from. Both files are then closed.The
        /// temporary file itself is not deleted. If that is desired, the
        /// temporary file should be deleted after the completion of generate(),
        /// if it exists.
        ///
        /// If a temporary file is set not to be used, generate() performs the
        /// following sequence of events: the output file is opened for writing,
        /// then it is written and closed.The output file is then re-opened for
        /// update, the appropriate sections are seek() ed to and re-written, then
        /// the output file is closed.
        /// </summary>
        /// <param name="useTempFile">
        /// non-zero if the temporary file should be used,
        /// zero if the temporary file should not be used.
        /// </param>
        /// <param name="tempFile">
        /// a file to temporarily write compressed data to,
        /// before opening it for reading and copying the
        /// contents to the output file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="Generate"/>
        public Error UseTemporaryFile(bool useTempFile, string tempFile) => throw new NotImplementedException();

        /// <summary>
        /// Sets a CHM compression engine parameter.
        /// </summary>
        /// <param name="param">the parameter to set</param>
        /// <param name="value">the value to set the parameter to</param>
        /// <returns>
        /// MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS if there
        /// is a problem with either parameter or value.
        /// </returns>
        /// <see cref="Generate"/>
        public Error SetParam(Parameters param, int value) => throw new NotImplementedException();

        #endregion
    }
}
