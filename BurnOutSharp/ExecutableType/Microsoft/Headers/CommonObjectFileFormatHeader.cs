using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Headers
{
    internal class CommonObjectFileFormatHeader
    {
        /// <summary>
        /// After the MS-DOS stub, at the file offset specified at offset 0x3c, is a 4-byte signature that identifies the file as a PE format image file.
        // This signature is "PE\0\0" (the letters "P" and "E" followed by two null bytes).
        /// </summary>
        public uint Signature;
        
        /// <summary>
        /// The number that identifies the type of target machine.
        /// </summary>
        public MachineType Machine;
        
        /// <summary>
        /// The number of sections.
        /// This indicates the size of the section table, which immediately follows the headers.
        /// </summary>
        public ushort NumberOfSections;
        
        /// <summary>
        /// The low 32 bits of the number of seconds since 00:00 January 1, 1970 (a C run-time time_t value), which indicates when the file was created.
        /// </summary>
        public uint TimeDateStamp;
        
        /// <summary>
        /// The file offset of the COFF symbol table, or zero if no COFF symbol table is present.
        /// This value should be zero for an image because COFF debugging information is deprecated.
        /// </summary>
        [Obsolete]
        public uint PointerToSymbolTable;
        
        /// <summary>
        /// The number of entries in the symbol table. This data can be used to locate the string table, which immediately follows the symbol table.
        /// This value should be zero for an image because COFF debugging information is deprecated.
        /// </summary>
        [Obsolete]
        public uint NumberOfSymbols;
        
        /// <summary>
        /// The size of the optional header, which is required for executable files but not for object files.
        // This value should be zero for an object file.
        /// </summary>
        public ushort SizeOfOptionalHeader;
        
        /// <summary>
        /// The flags that indicate the attributes of the file.
        /// </summary>
        public ImageObjectCharacteristics Characteristics;

        public static CommonObjectFileFormatHeader Deserialize(Stream stream)
        {
            var ifh = new CommonObjectFileFormatHeader();

            ifh.Signature = stream.ReadUInt32();
            ifh.Machine = (MachineType)stream.ReadUInt16();
            ifh.NumberOfSections = stream.ReadUInt16();
            ifh.TimeDateStamp = stream.ReadUInt32();
            ifh.PointerToSymbolTable = stream.ReadUInt32();
            ifh.NumberOfSymbols = stream.ReadUInt32();
            ifh.SizeOfOptionalHeader = stream.ReadUInt16();
            ifh.Characteristics = (ImageObjectCharacteristics)stream.ReadUInt16();

            return ifh;
        }

        public static CommonObjectFileFormatHeader Deserialize(byte[] content, ref int offset)
        {
            var ifh = new CommonObjectFileFormatHeader();

            ifh.Signature = BitConverter.ToUInt32(content, offset); offset += 4;
            ifh.Machine = (MachineType)BitConverter.ToUInt16(content, offset); offset += 2;
            ifh.NumberOfSections = BitConverter.ToUInt16(content, offset); offset += 2;
            ifh.TimeDateStamp = BitConverter.ToUInt32(content, offset); offset += 4;
            ifh.PointerToSymbolTable = BitConverter.ToUInt32(content, offset); offset += 4;
            ifh.NumberOfSymbols = BitConverter.ToUInt32(content, offset); offset += 4;
            ifh.SizeOfOptionalHeader = BitConverter.ToUInt16(content, offset); offset += 2;
            ifh.Characteristics = (ImageObjectCharacteristics)BitConverter.ToUInt16(content, offset); offset += 2;

            return ifh;
        }
    }
}