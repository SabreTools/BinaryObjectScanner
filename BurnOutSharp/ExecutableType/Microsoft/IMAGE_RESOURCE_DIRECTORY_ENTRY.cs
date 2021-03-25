/*
 *	  NEWEXE.H (C) Copyright Microsoft Corp 1984-1987
 *
 *	  Data structure definitions for the OS/2 & Windows
 *	  executable file format.
 *
 *	  Modified by IVS on 24-Jan-1991 for Resource DeCompiler
 *	  (C) Copyright IVS 1991
 *
 *    http://csn.ul.ie/~caolan/pub/winresdump/winresdump/newexe.h
 */

using System.IO;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    internal class IMAGE_RESOURCE_DIRECTORY_ENTRY
    {
        public uint Name { get; private set; }
        public uint OffsetToData { get; private set; }

        public static IMAGE_RESOURCE_DIRECTORY_ENTRY Deserialize(Stream stream)
        {
            IMAGE_RESOURCE_DIRECTORY_ENTRY irde = new IMAGE_RESOURCE_DIRECTORY_ENTRY();

            irde.Name = stream.ReadUInt32();
            irde.OffsetToData = stream.ReadUInt32();

            return irde;
        }
    }
}