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
    internal class IMAGE_FILE_HEADER
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;

        public static IMAGE_FILE_HEADER Deserialize(Stream stream)
        {
            var ifh = new IMAGE_FILE_HEADER();

            ifh.Machine = stream.ReadUInt16();
            ifh.NumberOfSections = stream.ReadUInt16();
            ifh.TimeDateStamp = stream.ReadUInt32();
            ifh.PointerToSymbolTable = stream.ReadUInt32();
            ifh.NumberOfSymbols = stream.ReadUInt32();
            ifh.SizeOfOptionalHeader = stream.ReadUInt16();
            ifh.Characteristics = stream.ReadUInt16();

            return ifh;
        }
    }
}