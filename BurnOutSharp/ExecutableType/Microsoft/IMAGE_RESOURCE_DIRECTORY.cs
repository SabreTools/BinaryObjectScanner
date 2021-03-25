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
    internal class IMAGE_RESOURCE_DIRECTORY
    {
        public uint Characteristics { get; private set; }
        public uint TimeDateStamp { get; private set; }
        public ushort MajorVersion { get; private set; }
        public ushort MinorVersion { get; private set; }
        public ushort NumberOfNamedEntries { get; private set; }
        public ushort NumberOfIdEntries { get; private set; }

        public static IMAGE_RESOURCE_DIRECTORY Deserialize(Stream stream)
        {
            IMAGE_RESOURCE_DIRECTORY ird = new IMAGE_RESOURCE_DIRECTORY();

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