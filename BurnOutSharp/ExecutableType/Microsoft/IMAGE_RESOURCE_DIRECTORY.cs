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
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_RESOURCE_DIRECTORY
    {
        public uint Characteristics;
        public uint TimeDateStamp;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort NumberOfNamedEntries;
        public ushort NumberOfIdEntries;

        public static IMAGE_RESOURCE_DIRECTORY Deserialize(Stream stream)
        {
            var ird = new IMAGE_RESOURCE_DIRECTORY();

            ird.Characteristics = stream.ReadUInt32();
            ird.TimeDateStamp = stream.ReadUInt32();
            ird.MajorVersion = stream.ReadUInt16();
            ird.MinorVersion = stream.ReadUInt16();
            ird.NumberOfNamedEntries = stream.ReadUInt16();
            ird.NumberOfIdEntries = stream.ReadUInt16();

            return ird;
        }
    }
}