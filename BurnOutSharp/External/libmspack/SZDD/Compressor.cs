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

namespace LibMSPackSharp.SZDD
{
    /// <summary>
    /// A compressor for the SZDD file format.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateSZDDCompressor(SystemImpl)"/>
    /// <see cref="Library.DestroySZDDCompressor(Compressor)"/>
    public class Compressor
    {
        /// <summary>
        /// Reads an input file and creates a compressed output file in the
        /// SZDD compressed file format. The SZDD compression format is quick
        /// but gives poor compression. It is possible for the compressed output
        /// file to be larger than the input file.
        ///
        /// Conventionally, SZDD compressed files have the final character in
        /// their filename replaced with an underscore, to show they are
        /// compressed.  The missing character is stored in the compressed file
        /// itself. This is due to the restricted filename conventions of MS-DOS,
        /// most operating systems, such as UNIX, simply append another file
        /// extension to the existing filename. As mspack does not deal with
        /// filenames, this is left up to you. If you wish to set the missing
        /// character stored in the file header, use set_param() with the
        /// #MSSZDDC_PARAM_MISSINGCHAR parameter.
        ///
        /// "Stream" compression (where the length of the input data is not
        /// known) is not possible. The length of the input data is stored in the
        /// header of the SZDD file and must therefore be known before any data
        /// is compressed. Due to technical limitations of the file format, the
        /// maximum size of uncompressed file that will be accepted is 2147483647
        /// bytes.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the msszdd_compressor
        /// instance being called
        /// </param>
        /// <param name="input">
        /// the name of the file to compressed. This is passed
        /// passed directly to mspack_system::open()
        /// </param>
        /// <param name="output">
        /// the name of the file to write compressed data to.
        /// This is passed directly to mspack_system::open().
        /// </param>
        /// <param name="length">
        /// the length of the uncompressed file, or -1 to indicate
        /// that this should be determined automatically by using
        /// mspack_system::seek() on the input file.
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        /// <see cref="SetParam"/>
        public Func<Compressor, string, string, long, Error> Compress;

        /// <summary>
        /// Sets an SZDD compression engine parameter.
        ///
        /// The following parameters are defined:
        /// - #MSSZDDC_PARAM_CHARACTER: the "missing character", the last character
        ///   in the uncompressed file's filename, which is traditionally replaced
        ///   with an underscore to show the file is compressed. Traditionally,
        ///   this can only be a character that is a valid part of an MS-DOS,
        ///   filename, but libmspack permits any character between 0x00 and 0xFF
        ///   to be stored. 0x00 is the default, and it represents "no character
        ///   stored".
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the msszdd_compressor
        /// instance being called
        /// </param>
        /// <param name="param">the parameter to set</param>
        /// <param name="value">the value to set the parameter to</param>
        /// <returns>
        /// MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS if there
        /// is a problem with either parameter or value.
        /// </returns>
        /// <see cref="Compress"/>
        public Func<Compressor, Parameters, int, Error> SetParam;

        /// <summary>
        /// Returns the error code set by the most recently called method.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the msszdd_compressor
        /// instance being called
        /// </param>
        /// <returns>the most recent error code</returns>
        /// <see cref="Compress"/>
        public Func<Compressor, Error> LastError;
    }
}
