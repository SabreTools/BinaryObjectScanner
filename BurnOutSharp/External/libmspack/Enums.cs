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

namespace LibMSPackSharp
{
    /// <summary>
    /// All compressors and decompressors use the same set of error codes. Most
    /// methods return an error code directly.For methods which do not
    /// return error codes directly, the error code can be obtained with the
    /// last_error() method.
    /// </summary>
    public enum Error
    {
        #region MSPACK Errors

        /// <summary>
        /// Used to indicate success.
        /// This error code is defined as zero, all other code are non-zero.
        /// </summary>
        MSPACK_ERR_OK = 0,

        /// <summary>
        /// A method was called with inappropriate arguments.
        /// </summary>
        MSPACK_ERR_ARGS = 1,

        /// <summary>
        /// Error opening file
        /// </summary>
        MSPACK_ERR_OPEN = 2,

        /// <summary>
        /// Error reading file
        /// </summary>
        MSPACK_ERR_READ = 3,

        /// <summary>
        /// Error writing file
        /// </summary>
        MSPACK_ERR_WRITE = 4,

        /// <summary>
        /// Seek error
        /// </summary>
        MSPACK_ERR_SEEK = 5,

        /// <summary>
        /// Out of memory
        /// </summary>
        MSPACK_ERR_NOMEMORY = 6,

        /// <summary>
        /// Bad "magic id" in file
        /// </summary>
        MSPACK_ERR_SIGNATURE = 7,

        /// <summary>
        /// Bad or corrupt file format
        /// </summary>
        MSPACK_ERR_DATAFORMAT = 8,

        /// <summary>
        /// Bad checksum or CRC
        /// </summary>
        MSPACK_ERR_CHECKSUM = 9,

        /// <summary>
        /// Error during compression
        /// </summary>
        MSPACK_ERR_CRUNCH = 10,

        /// <summary>
        /// Error during decompression
        /// </summary>
        MSPACK_ERR_DECRUNCH = 11,

        #endregion

        #region Inflate Errors

        /// <summary>
        /// Unknown block type
        /// </summary>
        INF_ERR_BLOCKTYPE = -1,

        /// <summary>
        /// Block size complement mismatch
        /// </summary>
        INF_ERR_COMPLEMENT = -2,

        /// <summary>
        /// Error from flush_window callback
        /// </summary>
        INF_ERR_FLUSH = -3,

        /// <summary>
        /// Too many bits in bit buffer
        /// </summary>
        INF_ERR_BITBUF = -4,

        /// <summary>
        /// Too many symbols in blocktype 2 header
        /// </summary>
        INF_ERR_SYMLENS = -5,

        /// <summary>
        /// Failed to build bitlens huffman table
        /// </summary>
        INF_ERR_BITLENTBL = -6,

        /// <summary>
        /// Failed to build literals huffman table
        /// </summary>
        INF_ERR_LITERALTBL = -7,

        /// <summary>
        /// Failed to build distance huffman table
        /// </summary>
        INF_ERR_DISTANCETBL = -8,

        /// <summary>
        /// Bitlen RLE code goes over table size
        /// </summary>
        INF_ERR_BITOVERRUN = -9,

        /// <summary>
        /// Invalid bit-length code
        /// </summary>
        INF_ERR_BADBITLEN = -10,

        /// <summary>
        /// Out-of-range literal code
        /// </summary>
        INF_ERR_LITCODE = -11,

        /// <summary>
        /// Out-of-range distance code
        /// </summary>
        INF_ERR_DISTCODE = -12,

        /// <summary>
        /// Somehow, distance is beyond 32k
        /// </summary>
        INF_ERR_DISTANCE = -13,

        /// <summary>
        /// Out of bits decoding huffman symbol
        /// </summary>
        INF_ERR_HUFFSYM = -14,

        #endregion
    }

    /// <summary>
    /// The interface to request current version of
    /// </summary>
    public enum Interfaces
    {
        /// <summary>
        /// Pass to mspack_version() to get the overall library version
        /// </summary>
        MSPACK_VER_LIBRARY = 0,

        /// <summary>
        /// Pass to mspack_version() to get the mspack_system version
        /// </summary>
        MSPACK_VER_SYSTEM = 1,

        /// <summary>
        /// Pass to mspack_version() to get the mscab_decompressor version
        /// </summary>
        MSPACK_VER_MSCABD = 2,

        /// <summary>
        /// Pass to mspack_version() to get the mscab_compressor version
        /// </summary>
        MSPACK_VER_MSCABC = 3,

        /// <summary>
        /// Pass to mspack_version() to get the mschm_decompressor version
        /// </summary>
        MSPACK_VER_MSCHMD = 4,

        /// <summary>
        /// Pass to mspack_version() to get the mschm_compressor version
        /// </summary>
        MSPACK_VER_MSCHMC = 5,

        /// <summary>
        /// Pass to mspack_version() to get the mslit_decompressor version
        /// </summary>
        MSPACK_VER_MSLITD = 6,

        /// <summary>
        /// Pass to mspack_version() to get the mslit_compressor version
        /// </summary>
        MSPACK_VER_MSLITC = 7,

        /// <summary>
        /// Pass to mspack_version() to get the mshlp_decompressor version
        /// </summary>
        MSPACK_VER_MSHLPD = 8,

        /// <summary>
        /// Pass to mspack_version() to get the mshlp_compressor version
        /// </summary>
        MSPACK_VER_MSHLPC = 9,

        /// <summary>
        /// Pass to mspack_version() to get the msszdd_decompressor version
        /// </summary>
        MSPACK_VER_MSSZDDD = 10,

        /// <summary>
        /// Pass to mspack_version() to get the msszdd_compressor version
        /// </summary>
        MSPACK_VER_MSSZDDC = 11,

        /// <summary>
        /// Pass to mspack_version() to get the mskwaj_decompressor version
        /// </summary>
        MSPACK_VER_MSKWAJD = 12,

        /// <summary>
        /// Pass to mspack_version() to get the mskwaj_compressor version
        /// </summary>
        MSPACK_VER_MSKWAJC = 13,

        /// <summary>
        /// Pass to mspack_version() to get the msoab_decompressor version
        /// </summary>
        MSPACK_VER_MSOABD = 14,

        /// <summary>
        /// Pass to mspack_version() to get the msoab_compressor version
        /// </summary>
        MSPACK_VER_MSOABC = 15,
    }

    public enum OpenMode
    {
        /// <summary>
        /// mspack_system::open() mode: open existing file for reading.
        /// </summary>
        MSPACK_SYS_OPEN_READ = 0,

        /// <summary>
        /// mspack_system::open() mode: open new file for writing
        /// </summary>
        MSPACK_SYS_OPEN_WRITE = 1,

        /// <summary>
        /// mspack_system::open() mode: open existing file for writing
        /// </summary>
        MSPACK_SYS_OPEN_UPDATE = 2,

        /// <summary>
        /// mspack_system::open() mode: open existing file for writing
        /// </summary>
        MSPACK_SYS_OPEN_APPEND = 3,
    }

    public enum SeekMode
    {
        /// <summary>
        /// mspack_system::seek() mode: seek relative to start of file
        /// </summary>
        MSPACK_SYS_SEEK_START = 0,

        /// <summary>
        /// mspack_system::seek() mode: seek relative to current offset
        /// </summary>
        MSPACK_SYS_SEEK_CUR = 1,

        /// <summary>
        /// mspack_system::seek() mode: seek relative to end of file
        /// </summary>
        MSPACK_SYS_SEEK_END = 2,
    }
}
