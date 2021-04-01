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
 
namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_RESOURCE_DATA_ENTRY
    {
        public uint OffsetToData;
        public uint Size;
        public uint CodePage;
        public uint Reserved;

        public static IMAGE_RESOURCE_DATA_ENTRY Deserialize(Stream stream)
        {
            var irde = new IMAGE_RESOURCE_DATA_ENTRY();

            irde.OffsetToData = stream.ReadUInt32();
            irde.Size = stream.ReadUInt32();
            irde.CodePage = stream.ReadUInt32();
            irde.Reserved = stream.ReadUInt32();

            return irde;
        }
    }
}