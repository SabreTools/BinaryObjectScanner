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
    /// A decompressor for .LZX (Offline Address Book) files
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateOABDecompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyOABDecompressor(Decompressor)"/>
    public class Decompressor
    {
        /// <summary>
        /// Decompresses a full Offline Address Book file.
        ///
        /// If the input file is a valid compressed Offline Address Book file, 
        /// it will be read and the decompressed contents will be written to
        /// the output file.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the msoab_decompressor
        /// instance being called
        /// </param>
        /// <param name="input">
        /// the filename of the input file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="output">
        /// the filename of the output file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Func<Decompressor, string, string, Error> Decompress;

        /// <summary>
        /// Decompresses an Offline Address Book with an incremental patch file.
        ///
        /// This requires both a full UNCOMPRESSED Offline Address Book file to
        /// act as the "base", and a compressed incremental patch file as input.
        /// If the input file is valid, it will be decompressed with reference to
        /// the base file, and the decompressed contents will be written to the
        /// output file.
        ///
        /// There is no way to tell what the right base file is for the given
        /// incremental patch, but if you get it wrong, this will usually result
        /// in incorrect data being decompressed, which will then fail a checksum
        /// test.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the msoab_decompressor
        /// instance being called
        /// </param>
        /// <param name="input">
        /// the filename of the input file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="base">
        /// the filename of the base file to which the
        /// incremental patch shall be applied. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <param name="output">
        /// the filename of the output file. This is passed
        /// directly to mspack_system::open().
        /// </param>
        /// <returns>an error code, or MSPACK_ERR_OK if successful</returns>
        public Func<Decompressor, string, string, string, Error> DecompressIncremental;

        /// <summary>
        /// Sets an OAB decompression engine parameter. Available only in OAB
        /// decompressor version 2 and above.
        ///
        /// - #MSOABD_PARAM_DECOMPBUF: How many bytes should be used as an input
        ///   buffer by decompressors? The minimum value is 16. The default value
        ///   is 4096.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the msoab_decompressor
        /// instance being called
        /// </param>
        /// <param name="param">the parameter to set</param>
        /// <param name="value">the value to set the parameter to</param>
        /// <returns>
        /// MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS if there
        /// is a problem with either parameter or value.
        /// </returns>
        public Func<Decompressor, Parameters, int, Error> SetParam;
    }
}
