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
    internal class IMAGE_DATA_DIRECTORY
    {
        public uint VirtualAddress { get; private set; }
        public uint Size { get; private set; }

        public static IMAGE_DATA_DIRECTORY Deserialize(Stream stream)
        {
            IMAGE_DATA_DIRECTORY idd = new IMAGE_DATA_DIRECTORY();

            idd.VirtualAddress = stream.ReadUInt32();
            idd.Size = stream.ReadUInt32();

            return idd;
        }
    }
}