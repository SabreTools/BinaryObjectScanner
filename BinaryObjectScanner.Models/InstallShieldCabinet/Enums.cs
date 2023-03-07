using System;

namespace BinaryObjectScanner.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    [Flags]
    public enum FileFlags : ushort
    {
        FILE_SPLIT = 1,
        FILE_OBFUSCATED = 2,
        FILE_COMPRESSED = 4,
        FILE_INVALID = 8,
    }

    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public enum LinkFlags : byte
    {
        LINK_NONE = 0,
        LINK_PREV = 1,
        LINK_NEXT = 2,
        LINK_BOTH = 3,
    }
}