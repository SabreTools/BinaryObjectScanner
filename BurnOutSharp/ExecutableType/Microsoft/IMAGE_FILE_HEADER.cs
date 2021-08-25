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
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_FILE_HEADER
    {
        public uint Signature;
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

            ifh.Signature = stream.ReadUInt32();
            ifh.Machine = stream.ReadUInt16();
            ifh.NumberOfSections = stream.ReadUInt16();
            ifh.TimeDateStamp = stream.ReadUInt32();
            ifh.PointerToSymbolTable = stream.ReadUInt32();
            ifh.NumberOfSymbols = stream.ReadUInt32();
            ifh.SizeOfOptionalHeader = stream.ReadUInt16();
            ifh.Characteristics = stream.ReadUInt16();

            return ifh;
        }

        public static IMAGE_FILE_HEADER Deserialize(byte[] content, int offset)
        {
            var ifh = new IMAGE_FILE_HEADER();

            ifh.Signature = BitConverter.ToUInt32(content, offset); offset += 4;
            ifh.Machine = BitConverter.ToUInt16(content, offset); offset += 2;
            ifh.NumberOfSections = BitConverter.ToUInt16(content, offset); offset += 2;
            ifh.TimeDateStamp = BitConverter.ToUInt32(content, offset); offset += 4;
            ifh.PointerToSymbolTable = BitConverter.ToUInt32(content, offset); offset += 4;
            ifh.NumberOfSymbols = BitConverter.ToUInt32(content, offset); offset += 4;
            ifh.SizeOfOptionalHeader = BitConverter.ToUInt16(content, offset); offset += 2;
            ifh.Characteristics = BitConverter.ToUInt16(content, offset); offset += 2;

            return ifh;
        }
    }
}