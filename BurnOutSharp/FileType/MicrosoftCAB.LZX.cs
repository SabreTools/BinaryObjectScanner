using System.Collections.Generic;

/// <see href="https://learn.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-patch/cc78752a-b4af-4eee-88cb-01f4d8a4c2bf"/>
/// <see href="https://interoperability.blob.core.windows.net/files/MS-PATCH/%5bMS-PATCH%5d.pdf"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzx.h"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzxc.c"/>
/// <see href="https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzxd.c"/>
/// <see href="https://wimlib.net/"/>
/// <see href="http://xavprods.free.fr/lzx/"/>
/// <see href="https://github.com/jhermsmeier/node-lzx"/>
/// <see href="https://github.com/jhermsmeier/node-cabarc"/>
namespace BurnOutSharp.FileType
{
    public class MSCABLZX
    {
        /// <summary>
        /// The window size determines the number of window subdivisions, or position slots
        /// </summary>
        public static readonly Dictionary<int, int> PositionSlots = new Dictionary<int, int>()
        {
            [128 * 1024] = 34, // 128 KB
            [256 * 1024] = 36, // 256 KB
            [512 * 1024] = 38, // 512 KB
            [1024 * 1024] = 42, // 1 MB
            [2 * 1024 * 1024] = 50, // 2 MB
            [4 * 1024 * 1024] = 66, // 4 MB
            [8 * 1024 * 1024] = 98, // 8 MB
            [16 * 1024 * 1024] = 162, // 16 MB
            [32 * 1024 * 1024] = 290, // 32 MB
        };
    }
}
