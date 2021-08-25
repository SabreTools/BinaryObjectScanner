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

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_DATA_DIRECTORY
    {
        public uint VirtualAddress;
        public uint Size;

        public static IMAGE_DATA_DIRECTORY Deserialize(Stream stream)
        {
            var idd = new IMAGE_DATA_DIRECTORY();

            idd.VirtualAddress = stream.ReadUInt32();
            idd.Size = stream.ReadUInt32();

            return idd;
        }

        public static IMAGE_DATA_DIRECTORY Deserialize(byte[] content, int offset)
        {
            var idd = new IMAGE_DATA_DIRECTORY();

            idd.VirtualAddress = BitConverter.ToUInt32(content, offset); offset += 4;
            idd.Size = BitConverter.ToUInt32(content, offset); offset += 4;

            return idd;
        }
    }
}