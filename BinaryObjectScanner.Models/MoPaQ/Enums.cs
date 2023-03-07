using System;

namespace BinaryObjectScanner.Models.MoPaQ
{
    /// <summary>
    /// Compression types for multiple compressions
    /// </summary>
    [Flags]
    public enum CompressionType : uint
    {
        /// <summary>
        /// Huffmann compression (used on WAVE files only)
        /// </summary>
        MPQ_COMPRESSION_HUFFMANN = 0x01,

        /// <summary>
        /// ZLIB compression
        /// </summary>
        MPQ_COMPRESSION_ZLIB = 0x02,

        /// <summary>
        /// PKWARE DCL compression
        /// </summary>
        MPQ_COMPRESSION_PKWARE = 0x08,

        /// <summary>
        /// BZIP2 compression (added in Warcraft III)
        /// </summary>
        MPQ_COMPRESSION_BZIP2 = 0x10,

        /// <summary>
        /// Sparse compression (added in Starcraft 2)
        /// </summary>
        MPQ_COMPRESSION_SPARSE = 0x20,

        /// <summary>
        /// IMA ADPCM compression (mono)
        /// </summary>
        MPQ_COMPRESSION_ADPCM_MONO = 0x40,

        /// <summary>
        /// IMA ADPCM compression (stereo)
        /// </summary>
        MPQ_COMPRESSION_ADPCM_STEREO = 0x80,

        /// <summary>
        /// LZMA compression. Added in Starcraft 2. This value is NOT a combination of flags.
        /// </summary>
        MPQ_COMPRESSION_LZMA = 0x12,

        /// <summary>
        /// Same compression
        /// </summary>
        MPQ_COMPRESSION_NEXT_SAME = 0xFFFFFFFF,
    }

    [Flags]
    public enum FileFlags : uint
    {
        /// <summary>
        /// File is compressed using PKWARE Data compression library
        /// </summary>
        MPQ_FILE_IMPLODE = 0x00000100,

        /// <summary>
        /// File is compressed using combination of compression methods
        /// </summary>
        MPQ_FILE_COMPRESS = 0x00000200,

        /// <summary>
        /// The file is encrypted
        /// </summary>
        MPQ_FILE_ENCRYPTED = 0x00010000,

        /// <summary>
        /// The decryption key for the file is altered according to the
        /// position of the file in the archive
        /// </summary>
        MPQ_FILE_FIX_KEY = 0x00020000,

        /// <summary>
        /// The file contains incremental patch for an existing file in base MPQ
        /// </summary>
        MPQ_FILE_PATCH_FILE = 0x00100000,

        /// <summary>
        /// Instead of being divided to 0x1000-bytes blocks, the file is stored
        /// as single unit
        /// </summary>
        MPQ_FILE_SINGLE_UNIT = 0x01000000,

        /// <summary>
        /// File is a deletion marker, indicating that the file no longer exists.
        /// This is used to allow patch archives to delete files present in
        /// lower-priority archives in the search chain. The file usually has
        /// length of 0 or 1 byte and its name is a hash
        /// </summary>
        MPQ_FILE_DELETE_MARKER = 0x02000000,

        /// <summary>
        /// File has checksums for each sector (explained in the File Data section).
        /// Ignored if file is not compressed or imploded.
        /// </summary>
        MPQ_FILE_SECTOR_CRC = 0x04000000,

        /// <summary>
        /// Set if file exists, reset when the file was deleted
        /// </summary>
        MPQ_FILE_EXISTS = 0x80000000,
    }

    public enum FormatVersion : ushort
    {
        /// <summary>
        /// Format 1 (up to The Burning Crusade)
        /// </summary>
        Format1 = 0,

        /// <summary>
        /// Format 2 (The Burning Crusade and newer)
        /// </summary>
        Format2 = 1,

        /// <summary>
        /// Format 3 (WoW - Cataclysm beta or newer)
        /// </summary>
        Format3 = 2,

        /// <summary>
        /// Format 4 (WoW - Cataclysm beta or newer)
        /// </summary>
        Format4 = 3,
    }

    public enum Locale : short
    {
        Neutral = 0,
        AmericanEnglish = 0,
        ChineseTaiwan = 0x404,
        Czech = 0x405,
        German = 0x407,
        English = 0x409,
        Spanish = 0x40A,
        French = 0x40C,
        Italian = 0x410,
        Japanese = 0x411,
        Korean = 0x412,
        Polish = 0x415,
        Portuguese = 0x416,
        Russian = 0x419,
        EnglishUK = 0x809,
    }

    public enum PatchType : uint
    {
        /// <summary>
        /// Blizzard-modified version of BSDIFF40 incremental patch
        /// </summary>
        BSD0 = 0x30445342,

        /// <summary>
        /// Unknown
        /// </summary>
        BSDP = 0x50445342,

        /// <summary>
        /// Plain replace
        /// </summary>
        COPY = 0x59504F43,

        /// <summary>
        /// Unknown
        /// </summary>
        COUP = 0x50554F43,

        /// <summary>
        /// Unknown
        /// </summary>
        CPOG = 0x474F5043,
    }
}
