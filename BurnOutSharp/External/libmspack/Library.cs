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

/** \mainpage
 *
 * \section intro Introduction
 *
 * libmspack is a library which provides compressors and decompressors,
 * archivers and dearchivers for Microsoft compression formats.
 *
 * \section formats Formats supported
 *
 * The following file formats are supported:
 * - SZDD files, which use LZSS compression
 * - KWAJ files, which use LZSS, LZSS+Huffman or deflate compression
 * - .HLP (MS Help) files, which use LZSS compression
 * - .CAB (MS Cabinet) files, which use deflate, LZX or Quantum compression
 * - .CHM (HTML Help) files, which use LZX compression
 * - .LIT (MS EBook) files, which use LZX compression and DES encryption
 * - .LZX (Exchange Offline Addressbook) files, which use LZX compression
 *
 * To determine the capabilities of the library, and the binary
 * compatibility version of any particular compressor or decompressor, use
 * the mspack_version() function. The UNIX library interface version is
 * defined as the highest-versioned library component.
 *
 * \section starting Getting started
 *
 * The macro MSPACK_SYS_SELFTEST() should be used to ensure the library can
 * be used. In particular, it checks if the caller is using 32-bit file I/O
 * when the library is compiled for 64-bit file I/O and vice versa.
 *
 * If compiled normally, the library includes basic file I/O and memory
 * management functionality using the standard C library. This can be
 * customised and replaced entirely by creating a SystemImpl structure.
 *
 * A compressor or decompressor for the required format must be
 * instantiated before it can be used. Each construction function takes
 * one parameter, which is either a pointer to a custom SystemImpl
 * structure, or null to use the default. The instantiation returned, if
 * not null, contains function pointers (methods) to work with the given
 * file format.
 * 
 * For compression:
 * - CreateCABCompressor() creates a mscab_compressor
 * - CreateCHMCompressor() creates a mschm_compressor
 * - CreateLITCompressor() creates a mslit_compressor
 * - CreateHLPCompressor() creates a mshlp_compressor
 * - CreateSZDDCompressor() creates a msszdd_compressor
 * - CreateKWAJCompressor() creates a mskwaj_compressor
 * - CreateOABCompressor() creates a msoab_compressor
 *
 * For decompression:
 * - mspack_create_cab_decompressor() creates a mscab_decompressor
 * - mspack_create_chm_decompressor() creates a mschm_decompressor
 * - mspack_create_lit_decompressor() creates a mslit_decompressor
 * - mspack_create_hlp_decompressor() creates a mshlp_decompressor
 * - mspack_create_szdd_decompressor() creates a msszdd_decompressor
 * - mspack_create_kwaj_decompressor() creates a mskwaj_decompressor
 * - mspack_create_oab_decompressor() creates a msoab_decompressor
 *
 * Once finished working with a format, each kind of
 * compressor/decompressor has its own specific destructor:
 * - mspack_destroy_cab_compressor()
 * - mspack_destroy_cab_decompressor()
 * - mspack_destroy_chm_compressor()
 * - mspack_destroy_chm_decompressor()
 * - mspack_destroy_lit_compressor()
 * - mspack_destroy_lit_decompressor()
 * - mspack_destroy_hlp_compressor()
 * - mspack_destroy_hlp_decompressor()
 * - mspack_destroy_szdd_compressor()
 * - mspack_destroy_szdd_decompressor()
 * - mspack_destroy_kwaj_compressor()
 * - mspack_destroy_kwaj_decompressor()
 * - mspack_destroy_oab_compressor()
 * - mspack_destroy_oab_decompressor()
 *
 * Destroying a compressor or decompressor does not destroy any objects,
 * structures or handles that have been created using that compressor or
 * decompressor. Ensure that everything created or opened is destroyed or
 * closed before compressor/decompressor is itself destroyed.
 *
 * \section threading Multi-threading
 *
 * libmspack methods are reentrant and multithreading-safe when each
 * thread has its own compressor or decompressor.
 * You should not call multiple methods simultaneously on a single
 * compressor or decompressor instance.
 *
 * If this may happen, you can either use one compressor or
 * decompressor per thread, or you can use your preferred lock,
 * semaphore or mutex library to ensure no more than one method on a
 * compressor/decompressor is called simultaneously. libmspack will
 * not do this locking for you.
 *
 * Example of incorrect behaviour:
 * - thread 1 calls mspack_create_cab_decompressor()
 * - thread 1 calls open()
 * - thread 1 calls extract() for one file
 * - thread 2 simultaneously calls extract() for another file
 *
 * Correct behaviour:
 * - thread 1 calls mspack_create_cab_decompressor()
 * - thread 2 calls mspack_create_cab_decompressor()
 * - thread 1 calls its own open() / extract()
 * - thread 2 simultaneously calls its own open() / extract()
 *
 * Also correct behaviour:
 * - thread 1 calls mspack_create_cab_decompressor()
 * - thread 1 locks a mutex for with the decompressor before
 *   calling any methods on it, and unlocks the mutex after each
 *   method returns.
 * - thread 1 can share the results of open() with thread 2, and both
 *   can call extract(), provided they both guard against simultaneous
 *   use of extract(), and any other methods, with the mutex
 */

namespace LibMSPackSharp
{
    public static class Library
    {
        #region CAB

        /// <summary>
        /// Creates a new CAB compressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="CAB.Compressor"/> or null</returns>
        public static CAB.Compressor CreateCABCompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Creates a new CAB decompressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="CAB.Decompressor"/> or null</returns>
        public static CAB.Decompressor CreateCABDecompressor(SystemImpl sys)
        {
            if (sys == null)
                sys = SystemImpl.DefaultSystem;
            if (!SystemImpl.ValidSystem(sys))
                return null;

            return new CAB.Decompressor()
            {
                System = sys,
                State = null,
                Error = Error.MSPACK_ERR_OK,

                SearchBufferSize = 32768,
                FixMSZip = false,
                BufferSize = 4096,
                Salvage = false,
            };
        }

        /// <summary>
        /// Destroys an existing CAB compressor.
        /// </summary>
        /// <param name="self">the <see cref="CAB.Compressor"/> to destroy</param>
        public static void DestroyCABCompressor(CAB.Compressor self)
        {
            // TODO
        }

        /// <summary>
        /// Destroys an existing CAB decompressor.
        /// </summary>
        /// <param name="self">the <see cref="CAB.Decompressor"/> to destroy</param>
        public static void DestroyCABDecompressor(CAB.Decompressor self)
        {
            if (self != null)
            {
                SystemImpl sys = self.System;
                if (self.State != null)
                {
                    sys.Close(self.State.InputFileHandle);
                    sys.Close(self.State.OutputFileHandle);
                    self.FreeDecompressionState();
                }
            }
        }

        #endregion

        #region CHM

        /// <summary>
        /// Creates a new CHM compressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="CHM.Compressor"/> or null</returns>
        public static CHM.Compressor CreateCHMCompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Creates a new CHM decompressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="CHM.Decompressor"/> or null</returns>
        public static CHM.Decompressor CreateCHMDecompressor(SystemImpl sys)
        {
            if (sys == null)
                sys = SystemImpl.DefaultSystem;
            if (!SystemImpl.ValidSystem(sys))
                return null;

            return new CHM.DecompressorImpl()
            {
                Open = CHM.Implementation.Open,
                Close = CHM.Implementation.Close,
                Extract = CHM.Implementation.Extract,
                LastError = CHM.Implementation.LastError,
                FastOpen = CHM.Implementation.FastOpen,
                FastFind = CHM.Implementation.FastFind,
                System = sys,
                Error = Error.MSPACK_ERR_OK,
                State = null,
            };
        }

        /// <summary>
        /// Destroys an existing CHM compressor.
        /// </summary>
        /// <param name="c">the <see cref="CHM.Compressor"/> to destroy</param>
        public static void DestroyCHMCompressor(CHM.Compressor c)
        {
            // TODO
        }

        /// <summary>
        /// Destroys an existing CHM decompressor.
        /// </summary>
        /// <param name="d">the <see cref="CHM.Decompressor"/> to destroy</param>
        public static void DestroyCHMDecompressor(CHM.Decompressor d)
        {
            CHM.DecompressorImpl self = d as CHM.DecompressorImpl;
            if (self != null)
            {
                SystemImpl sys = self.System;
                if (self.State != null)
                {
                    sys.Close(self.State.InputFileHandle);
                    sys.Close(self.State.OutputFileHandle);
                }
            }
        }

        #endregion

        #region LIT

        /// <summary>
        /// Creates a new LIT compressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="LIT.Compressor"/> or null</returns>
        public static LIT.Compressor CreateLITCompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Creates a new LIT decompressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="LIT.Decompressor"/> or null</returns>
        public static LIT.Decompressor CreateLITDecompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Destroys an existing LIT compressor.
        /// </summary>
        /// <param name="c">the <see cref="LIT.Compressor"/> to destroy</param>
        public static void DestroyLITCompressor(LIT.Compressor c)
        {
            // TODO
        }

        /// <summary>
        /// Destroys an existing LIT decompressor.
        /// </summary>
        /// <param name="d">the <see cref="LIT.Decompressor"/> to destroy</param>
        public static void DestroyLITDecompressor(LIT.Decompressor d)
        {
            // TODO
        }

        #endregion

        #region HLP

        /// <summary>
        /// Creates a new HLP compressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="HLP.Compressor"/> or null</returns>
        public static HLP.Compressor CreateHLPCompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Creates a new HLP decompressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="HLP.Decompressor"/> or null</returns>
        public static HLP.Decompressor CreateHLPDecompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Destroys an existing HLP compressor.
        /// </summary>
        /// <param name="c">the <see cref="HLP.Compressor"/> to destroy</param>
        public static void DestroyHLPCompressor(HLP.Compressor c)
        {
            // TODO
        }

        /// <summary>
        /// Destroys an existing HLP decompressor.
        /// </summary>
        /// <param name="d">the <see cref="HLP.Decompressor"/> to destroy</param>
        public static void DestroyHLPDecompressor(HLP.Decompressor d)
        {
            // TODO
        }

        #endregion

        #region SZDD

        /// <summary>
        /// Creates a new SZDD compressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="SZDD.Compressor"/> or null</returns>
        public static SZDD.Compressor CreateSZDDCompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Creates a new SZDD decompressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="SZDD.Decompressor"/> or null</returns>
        public static SZDD.Decompressor CreateSZDDDecompressor(SystemImpl sys)
        {
            if (sys == null)
                sys = SystemImpl.DefaultSystem;
            if (!SystemImpl.ValidSystem(sys))
                return null;

            return new SZDD.DecompressorImpl()
            {
                Open = SZDD.Implementation.Open,
                Close = SZDD.Implementation.Close,
                Extract = SZDD.Implementation.Extract,
                Decompress = SZDD.Implementation.Decompress,
                LastError = SZDD.Implementation.LastError,
                System = sys,
                Error = Error.MSPACK_ERR_OK,
            };
        }

        /// <summary>
        /// Destroys an existing SZDD compressor.
        /// </summary>
        /// <param name="c">the <see cref="SZDD.Compressor"/> to destroy</param>
        public static void DestroySZDDCompressor(SZDD.Compressor c)
        {
            // TODO
        }

        /// <summary>
        /// Destroys an existing SZDD decompressor.
        /// </summary>
        /// <param name="d">the <see cref="SZDD.Decompressor"/> to destroy</param>
        public static void DestroySZDDDecompressor(SZDD.Decompressor d) { }

        #endregion

        #region KWAJ

        /// <summary>
        /// Creates a new KWAJ compressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="KWAJ.Compressor"/> or null</returns>
        public static KWAJ.Compressor CreateKWAJCompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Creates a new KWAJ decompressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="KWAJ.Decompressor"/> or null</returns>
        public static KWAJ.Decompressor CreateKWAJDecompressor(SystemImpl sys)
        {
            if (sys == null)
                sys = SystemImpl.DefaultSystem;
            if (!SystemImpl.ValidSystem(sys))
                return null;

            return new KWAJ.DecompressorImpl()
            {
                Open = KWAJ.Implementation.Open,
                Close = KWAJ.Implementation.Close,
                Extract = KWAJ.Implementation.Extract,
                Decompress = KWAJ.Implementation.Decompress,
                LastError = KWAJ.Implementation.LastError,
                System = sys,
                Error = Error.MSPACK_ERR_OK,
            };
        }

        /// <summary>
        /// Destroys an existing KWAJ compressor.
        /// </summary>
        /// <param name="c">the <see cref="KWAJ.Compressor"/> to destroy</param>
        public static void DestroyKWAJCompressor(KWAJ.Compressor c)
        {
            // TODO
        }

        /// <summary>
        /// Destroys an existing KWAJ decompressor.
        /// </summary>
        /// <param name="d">the <see cref="KWAJ.Decompressor"/> to destroy</param>
        public static void DestroyKWAJDecompressor(KWAJ.Decompressor d) { }

        #endregion

        #region OAB

        /// <summary>
        /// Creates a new OAB compressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="OAB.Compressor"/> or null</returns>
        public static OAB.Compressor CreateOABCompressor(SystemImpl sys)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Creates a new OAB decompressor.
        /// </summary>
        /// <param name="sys">a custom SystemImpl structure, or null to use the default</param>
        /// <returns>a <see cref="OAB.Decompressor"/> or null</returns>
        public static OAB.Decompressor CreateOABDecompressor(SystemImpl sys)
        {
            if (sys == null)
                sys = SystemImpl.DefaultSystem;
            if (!SystemImpl.ValidSystem(sys))
                return null;

            return new OAB.DecompressorImpl()
            {
                Decompress = OAB.Implementation.Decompress,
                DecompressIncremental = OAB.Implementation.DecompressIncremental,
                SetParam = OAB.Implementation.Param,
                System = sys,
                BufferSize = 4096,
            };
        }

        /// <summary>
        /// Destroys an existing OAB compressor.
        /// </summary>
        /// <param name="c">the <see cref="OAB.Compressor"/> to destroy</param>
        public static void DestroyOABCompressor(OAB.Compressor c)
        {
            // TODO
        }

        /// <summary>
        /// Destroys an existing OAB decompressor.
        /// </summary>
        /// <param name="d">the <see cref="OAB.Decompressor"/> to destroy</param>
        public static void DestroyOABDecompressor(OAB.Decompressor d) { }

        #endregion
    }
}
