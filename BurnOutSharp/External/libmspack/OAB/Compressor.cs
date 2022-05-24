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

namespace LibMSPackSharp.OAB
{
    /// <summary>
    /// A compressor for the Offline Address Book (OAB) format.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateOABCompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyOABCompressor(Compressor)"/>
    public class Compressor
    {
        #region Fields

        public SystemImpl System { get; set; }

        #endregion

        /// <summary>
        /// Compress a full OAB file.
        ///
        /// The input file will be read and the compressed contents written to the
        /// output file.
        /// </summary>
        /// <param name="input">
        /// The filename of the input file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="output">
        /// The filename of the output file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public Error Compress(string input, string output) => throw new NotImplementedException();

        /// <summary>
        /// Generate a compressed incremental OAB patch file.
        ///
        /// The two uncompressed files "input" and "base" will be read, and an
        /// incremental patch to generate "input" from "base" will be written to
        /// the output file.
        /// </summary>
        /// <param name="input">
        /// The filename of the input file containing the new
        /// version of its contents. This is passed directly
        /// to mspack_system::open().
        /// </param>
        /// <param name="baseFile">
        /// The filename of the original base file containing
        /// the old version of its contents, against which the
        /// incremental patch shall generated. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="output">
        /// The filename of the output file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>An error code, or MSPACK_ERR_OK if successful</returns>
        public Error CompressIncremental(string input, string baseFile, string output) => throw new NotImplementedException();
    }
}
