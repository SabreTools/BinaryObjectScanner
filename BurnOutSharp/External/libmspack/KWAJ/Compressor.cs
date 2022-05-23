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
    /// A compressor for the KWAJ file format.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    /// <see cref="Library.CreateKWAJCompressor(SystemImpl)"/>
    /// <see cref="Library.DestroyKWAJCompressor(Compressor)"/>
    public class Compressor
    {
        #region Fields

        public SystemImpl System { get; set; }

        /// <remarks>
        /// !!! MATCH THIS TO NUM OF PARAMS IN MSPACK.H !!!
        /// </remarks>
        public int[] Param { get; set; } = new int[2];

        public Error Error { get; set; }

        #endregion

        /// <summary>
        /// Reads an input file and creates a compressed output file in the
        /// KWAJ compressed file format.The KWAJ compression format is quick
        /// but gives poor compression.It is possible for the compressed output
        /// file to be larger than the input file.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the Compressor
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
        /// Sets an KWAJ compression engine parameter.
        ///
        /// The following parameters are defined:
        ///
        /// - #MSKWAJC_PARAM_COMP_TYPE: the compression method to use. Must
        ///   be one of #MSKWAJC_COMP_NONE, #MSKWAJC_COMP_XOR, #MSKWAJ_COMP_SZDD
        ///   or #MSKWAJ_COMP_LZH. The default is #MSKWAJ_COMP_LZH.
        ///
        /// - #MSKWAJC_PARAM_INCLUDE_LENGTH: a boolean; should the compressed
        ///   output file should include the uncompressed length of the input
        ///   file in the header? This adds 4 bytes to the size of the output
        ///   file. A value of zero says "no", non-zero says "yes". The default
        ///   is "no".
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the Compressor
        /// instance being called
        /// </param>
        /// <param name="param">the parameter to set</param>
        /// <param name="value">the value to set the parameter to</param>
        /// <returns>MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS if there
        /// is a problem with either parameter or value.
        /// </returns>
        /// <see cref="Generate"/>
        public Func<Compressor, Parameters, int, Error> SetParam;

        /// <summary>
        /// Sets the original filename of the file before compression,
        /// which will be stored in the header of the output file.
        ///
        /// The filename should be a null-terminated string, it must be an
        /// MS-DOS "8.3" type filename (up to 8 bytes for the filename, then
        /// optionally a "." and up to 3 bytes for a filename extension).
        ///
        /// If NULL is passed as the filename, no filename is included in the
        /// header. This is the default.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the Compressor
        /// instance being called
        /// </param>
        /// <param name="filename">the original filename to use</param>
        /// <returns>
        /// MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS if the
        /// filename is too long
        /// </returns>
        public Func<Compressor, string, Error> SetFilename;

        /// <summary>
        /// Sets arbitrary data that will be stored in the header of the
        /// output file, uncompressed. It can be up to roughly 64 kilobytes,
        /// as the overall size of the header must not exceed 65535 bytes.
        /// The data can contain null bytes if desired.
        ///
        /// If NULL is passed as the data pointer, or zero is passed as the
        /// length, no extra data is included in the header. This is the
        /// default.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the Compressor
        /// instance being called
        /// </param>
        /// <param name="data">a pointer to the data to be stored in the header</param>
        /// <param name="bytes">the length of the data in bytes</param>
        /// <returns>
        /// MSPACK_ERR_OK if all is OK, or MSPACK_ERR_ARGS extra data
        /// is too long
        /// </returns>
        public Func<Compressor, byte[], int, int, Error> SetExtraData;

        /// <summary>
        /// Returns the error code set by the most recently called method.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the Compressor
        /// instance being called
        /// </param>
        /// <returns>the most recent error code</returns>
        /// <see cref="Compress"/>
        public Func<Compressor, Error> LastError;
    }
}
