using System;

namespace BurnOutSharp.Models.MoPaQ
{
    [Flags]
    public enum MoPaQFileFlags : uint
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

    public enum MoPaQPatchType : uint
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
