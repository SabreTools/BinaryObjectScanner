/* This file is part of libmspack.
 * (C) 2003-2018 Stuart Caie.
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * For further details, see the file COPYING.LIB distributed with libmspack
 */

/* Cabinet (.CAB) files are a form of file archive. Each cabinet contains
 * "folders", which are compressed spans of data. Each cabinet has
 * "files", whose metadata is in the cabinet header, but whose actual data
 * is stored compressed in one of the "folders". Cabinets can span more
 * than one physical file on disk, in which case they are a "cabinet set",
 * and usually the last folder of each cabinet extends into the next
 * cabinet.
 *
 * For a complete description of the format, see the MSDN site:
 *   http://msdn.microsoft.com/en-us/library/bb267310.aspx
 */

/* Notes on compliance with cabinet specification:
 *
 * One of the main changes between cabextract 0.6 and libmspack's cab
 * decompressor is the move from block-oriented decompression to
 * stream-oriented decompression.
 *
 * cabextract would read one data block from disk, decompress it with the
 * appropriate method, then write the decompressed data. The CAB
 * specification is specifically designed to work like this, as it ensures
 * compression matches do not span the maximum decompressed block size
 * limit of 32kb.
 *
 * However, the compression algorithms used are stream oriented, with
 * specific hacks added to them to enforce the "individual 32kb blocks"
 * rule in CABs. In other file formats, they do not have this limitation.
 *
 * In order to make more generalised decompressors, libmspack's CAB
 * decompressor has moved from being block-oriented to more stream
 * oriented. This also makes decompression slightly faster.
 *
 * However, this leads to incompliance with the CAB specification. The
 * CAB controller can no longer ensure each block of input given to the
 * decompressors is matched with their output. The "decompressed size" of
 * each individual block is thrown away.
 *
 * Each CAB block is supposed to be seen as individually compressed. This
 * means each consecutive data block can have completely different
 * "uncompressed" sizes, ranging from 1 to 32768 bytes. However, in
 * reality, all data blocks in a folder decompress to exactly 32768 bytes,
 * excepting the final block. 
 *
 * Given this situation, the decompression algorithms are designed to
 * realign their input bitstreams on 32768 output-byte boundaries, and
 * various other special cases have been made. libmspack will not
 * correctly decompress LZX or Quantum compressed folders where the blocks
 * do not follow this "32768 bytes until last block" pattern. It could be
 * implemented if needed, but hopefully this is not necessary -- it has
 * not been seen in over 3Gb of CAB archives.
 */

namespace LibMSPackSharp.CAB
{
    public static class Constants
    {
        // CAB data blocks are <= 32768 bytes in uncompressed form.Uncompressed
        // blocks have zero growth. MSZIP guarantees that it won't grow above
        // uncompressed size by more than 12 bytes.LZX guarantees it won't grow
        // more than 6144 bytes.Quantum has no documentation, but the largest
        // block seen in the wild is 337 bytes above uncompressed size.

        public const int CAB_BLOCKMAX = 32768;
        public const int CAB_INPUTMAX = CAB_BLOCKMAX + 6144;

        // input buffer needs to be CAB_INPUTMAX + 1 byte to allow for max-sized block
        // plus 1 trailer byte added by cabd_sys_read_block() for Quantum alignment.
        // 
        // When MSCABD_PARAM_SALVAGE is set, block size is not checked so can be
        // up to 65535 bytes, so max input buffer size needed is 65535 + 1

        public const int CAB_INPUTMAX_SALVAGE = 65535;
        public const int CAB_INPUTBUF = CAB_INPUTMAX_SALVAGE + 1;

        // There are no more than 65535 data blocks per folder, so a folder cannot
        // be more than 32768*65535 bytes in length.As files cannot span more than
        // one folder, this is also their max offset, length and offset+length limit.

        public const int CAB_FOLDERMAX = 65535;
        public const int CAB_LENGTHMAX = CAB_BLOCKMAX * CAB_FOLDERMAX;
    }
}
