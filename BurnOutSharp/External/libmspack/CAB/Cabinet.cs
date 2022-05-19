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
    /// <summary>
    /// A structure which represents a single cabinet file.
    ///
    /// All fields are READ ONLY.
    ///
    /// If this cabinet is part of a merged cabinet set, the #files and #folders
    /// fields are common to all cabinets in the set, and will be identical.
    /// </summary>
    /// <see cref="Decompressor.Open"/>
    /// <see cref="Decompressor.Close"/>
    /// <see cref="Decompressor.Search"/>
    public class Cabinet
    {
        /// <summary>
        /// The next cabinet in a chained list, if this cabinet was opened with
        /// mscab_decompressor::search(). May be NULL to mark the end of the
        /// list.
        /// </summary>
        public Cabinet Next { get; set; }

        /// <summary>
        /// The filename of the cabinet. More correctly, the filename of the
        /// physical file that the cabinet resides in. This is given by the
        /// library user and may be in any format.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The file offset of cabinet within the physical file it resides in.
        /// </summary>
        public long BaseOffset { get; set; }

        /// <summary>
        /// The length of the cabinet file in bytes.
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// The previous cabinet in a cabinet set, or NULL.
        /// </summary>
        public Cabinet PreviousCabinet { get; set; }

        /// <summary>
        /// The next cabinet in a cabinet set, or NULL.
        /// </summary>
        public Cabinet NextCabinet { get; set; }

        /// <summary>
        /// The filename of the previous cabinet in a cabinet set, or NULL.
        /// </summary>
        public string PreviousName { get; set; }

        /// <summary>
        /// The filename of the next cabinet in a cabinet set, or NULL.
        /// </summary>
        public string NextName { get; set; }

        /// <summary>
        /// The name of the disk containing the previous cabinet in a cabinet, or NULL.
        /// </summary>
        public string PreviousInfo { get; set; }

        /// <summary>
        /// The name of the disk containing the next cabinet in a cabinet set, or NULL.
        /// </summary>
        public string NextInfo { get; set; }

        /// <summary>
        /// A list of all files in the cabinet or cabinet set.
        /// </summary>
        public InternalFile Files { get; set; }

        /// <summary>
        /// A list of all folders in the cabinet or cabinet set.
        /// </summary>
        public Folder Folders { get; set; }

        /// <summary>
        /// The set ID of the cabinet. All cabinets in the same set should have
        /// the same set ID.
        /// </summary>
        public ushort SetID { get; set; }

        /// <summary>
        /// The index number of the cabinet within the set. Numbering should
        /// start from 0 for the first cabinet in the set, and increment by 1 for
        /// each following cabinet.
        /// </summary>
        public ushort SetIndex { get; set; }

        /// <summary>
        /// The number of bytes reserved in the header area of the cabinet.
        /// 
        /// If this is non-zero and flags has MSCAB_HDR_RESV set, this data can
        /// be read by the calling application. It is of the given length,
        /// located at offset (base_offset + MSCAB_HDR_RESV_OFFSET) in the
        /// cabinet file.
        /// </summary>
        /// <see cref="Flags"/>
        public ushort HeaderResv { get; set; }

        /// <summary>
        /// Header flags.
        /// </summary>
        /// <see cref="PreviousName"/>
        /// <see cref="PreviousInfo"/>
        /// <see cref="NextName"/>
        /// <see cref="NextInfo"/>
        /// <see cref="HeaderResv"/>
        public HeaderFlags Flags { get; set; }
    }
}
