using System;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Models.PortableExecutable;

namespace BurnOutSharp.Builder
{
    // TODO: Make Stream Data rely on Byte Data
    public static class PortableExecutable
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Portable Executable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = offset;

            // Create a new executable to fill
            var executable = new Executable();

            #region MS-DOS Stub

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data, offset);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            #endregion

            #region Signature

            offset = (int)(initialOffset + stub.Header.NewExeHeaderAddr);
            executable.Signature = new byte[4];
            for (int i = 0; i < executable.Signature.Length; i++)
            {
                executable.Signature[i] = data.ReadByte(ref offset);
            }
            if (executable.Signature[0] != 'P' || executable.Signature[1] != 'E' || executable.Signature[2] != '\0' || executable.Signature[3] != '\0')
                return null;

            #endregion

            #region COFF File Header

            // Try to parse the COFF file header
            var coffFileHeader = ParseCOFFFileHeader(data, ref offset);
            if (coffFileHeader == null)
                return null;

            // Set the COFF file header
            executable.COFFFileHeader = coffFileHeader;

            #endregion

            #region Optional Header

            // Try to parse the optional header
            var optionalHeader = ParseOptionalHeader(data, ref offset, coffFileHeader.SizeOfOptionalHeader);
            if (optionalHeader == null)
                return null;

            // Set the optional header
            executable.OptionalHeader = optionalHeader;

            #endregion

            #region Section Table

            // Try to parse the section table
            var sectionTable = ParseSectionTable(data, offset, coffFileHeader.NumberOfSections);
            if (sectionTable == null)
                return null;

            // Set the section table
            executable.SectionTable = sectionTable;

            #endregion

            #region COFF Symbol Table and COFF String Table

            // TODO: Validate that this is correct with an "old" PE
            if (coffFileHeader.PointerToSymbolTable != 0)
            {
                // If the offset for the COFF symbol table doesn't exist
                int tableAddress = initialOffset
                    + (int)coffFileHeader.PointerToSymbolTable.ConvertVirtualAddress(executable.SectionTable);
                if (tableAddress >= data.Length)
                    return executable;

                // Try to parse the COFF symbol table
                var coffSymbolTable = ParseCOFFSymbolTable(data, tableAddress, coffFileHeader.NumberOfSymbols);
                if (coffSymbolTable == null)
                    return null;

                // If the offset for the COFF string table doesn't exist
                tableAddress = initialOffset
                    + (int)coffFileHeader.PointerToSymbolTable.ConvertVirtualAddress(executable.SectionTable)
                    + (coffSymbolTable.Length * 18 /* sizeof(COFFSymbolTableEntry) */);
                if (tableAddress >= data.Length)
                    return executable;

                // Set the COFF symbol table
                executable.COFFSymbolTable = coffSymbolTable;

                // Try to parse the COFF string table
                var coffStringTable = ParseCOFFStringTable(data, tableAddress);
                if (coffStringTable == null)
                    return null;

                // Set the COFF string table
                executable.COFFStringTable = coffStringTable;
            }

            #endregion

            #region Attribute Certificate Table

            if (optionalHeader.CertificateTable != null && optionalHeader.CertificateTable.VirtualAddress != 0)
            {
                // If the offset for the COFF symbol table doesn't exist
                int certificateTableAddress = initialOffset
                    + (int)optionalHeader.CertificateTable.VirtualAddress;
                if (certificateTableAddress >= data.Length)
                    return executable;

                // Try to parse the attribute certificate table
                int endOffset = (int)(certificateTableAddress + optionalHeader.CertificateTable.Size);
                var attributeCertificateTable = ParseAttributeCertificateTable(data, certificateTableAddress, endOffset);
                if (attributeCertificateTable == null)
                    return null;

                // Set the attribute certificate table
                executable.AttributeCertificateTable = attributeCertificateTable;
            }

            #endregion

            #region Delay-Load Directory Table

            if (optionalHeader.DelayImportDescriptor != null && optionalHeader.DelayImportDescriptor.VirtualAddress != 0)
            {
                // If the offset for the delay-load directory table doesn't exist
                int delayLoadDirectoryTableAddress = initialOffset
                    + (int)optionalHeader.DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (delayLoadDirectoryTableAddress >= data.Length)
                    return executable;

                // Try to parse the delay-load directory table
                var delayLoadDirectoryTable = ParseDelayLoadDirectoryTable(data, delayLoadDirectoryTableAddress);
                if (delayLoadDirectoryTable == null)
                    return null;

                // Set the delay-load directory table
                executable.DelayLoadDirectoryTable = delayLoadDirectoryTable;
            }

            #endregion

            #region Resource Directory Table

            // Should also be in the '.rsrc' section
            if (optionalHeader.ResourceTable != null && optionalHeader.ResourceTable.VirtualAddress != 0)
            {
                // If the offset for the resource directory table doesn't exist
                int tableAddress = initialOffset
                    + (int)optionalHeader.ResourceTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (tableAddress >= data.Length)
                    return executable;

                // Try to parse the resource directory table
                var resourceDirectoryTable = ParseResourceDirectoryTable(data, tableAddress, tableAddress, executable.SectionTable);
                if (resourceDirectoryTable == null)
                    return null;

                // Set the resource directory table
                executable.ResourceDirectoryTable = resourceDirectoryTable;
            }

            #endregion

            // TODO: Finish implementing PE parsing
            return executable;
        }

        /// <summary>
        /// Parse a byte array into a Portable Executable COFF file header
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled COFF file header on success, null on error</returns>
        private static COFFFileHeader ParseCOFFFileHeader(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            var fileHeader = new COFFFileHeader();

            fileHeader.Machine = (MachineType)data.ReadUInt16(ref offset);
            fileHeader.NumberOfSections = data.ReadUInt16(ref offset);
            fileHeader.TimeDateStamp = data.ReadUInt32(ref offset);
            fileHeader.PointerToSymbolTable = data.ReadUInt32(ref offset);
            fileHeader.NumberOfSymbols = data.ReadUInt32(ref offset);
            fileHeader.SizeOfOptionalHeader = data.ReadUInt16(ref offset);
            fileHeader.Characteristics = (Characteristics)data.ReadUInt16(ref offset);

            return fileHeader;
        }

        /// <summary>
        /// Parse a byte array into an optional header
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="optionalSize">Size of the optional header</param>
        /// <returns>Filled optional header on success, null on error</returns>
        private static OptionalHeader ParseOptionalHeader(byte[] data, ref int offset, int optionalSize)
        {
            int initialOffset = offset;

            // TODO: Use marshalling here instead of building
            var optionalHeader = new OptionalHeader();

            #region Standard Fields

            optionalHeader.Magic = (OptionalHeaderMagicNumber)data.ReadUInt16(ref offset);
            optionalHeader.MajorLinkerVersion = data.ReadByte(ref offset);
            optionalHeader.MinorLinkerVersion = data.ReadByte(ref offset);
            optionalHeader.SizeOfCode = data.ReadUInt32(ref offset);
            optionalHeader.SizeOfInitializedData = data.ReadUInt32(ref offset);
            optionalHeader.SizeOfUninitializedData = data.ReadUInt32(ref offset);
            optionalHeader.AddressOfEntryPoint = data.ReadUInt32(ref offset);
            optionalHeader.BaseOfCode = data.ReadUInt32(ref offset);

            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.BaseOfData = data.ReadUInt32(ref offset);

            #endregion

            #region Windows-Specific Fields

            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.ImageBase_PE32 = data.ReadUInt32(ref offset);
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.ImageBase_PE32Plus = data.ReadUInt64(ref offset);
            optionalHeader.SectionAlignment = data.ReadUInt32(ref offset);
            optionalHeader.FileAlignment = data.ReadUInt32(ref offset);
            optionalHeader.MajorOperatingSystemVersion = data.ReadUInt16(ref offset);
            optionalHeader.MinorOperatingSystemVersion = data.ReadUInt16(ref offset);
            optionalHeader.MajorImageVersion = data.ReadUInt16(ref offset);
            optionalHeader.MinorImageVersion = data.ReadUInt16(ref offset);
            optionalHeader.MajorSubsystemVersion = data.ReadUInt16(ref offset);
            optionalHeader.MinorSubsystemVersion = data.ReadUInt16(ref offset);
            optionalHeader.Win32VersionValue = data.ReadUInt32(ref offset);
            optionalHeader.SizeOfImage = data.ReadUInt32(ref offset);
            optionalHeader.SizeOfHeaders = data.ReadUInt32(ref offset);
            optionalHeader.CheckSum = data.ReadUInt32(ref offset);
            optionalHeader.Subsystem = (WindowsSubsystem)data.ReadUInt16(ref offset);
            optionalHeader.DllCharacteristics = (DllCharacteristics)data.ReadUInt16(ref offset);
            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.SizeOfStackReserve_PE32 = data.ReadUInt32(ref offset);
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.SizeOfStackReserve_PE32Plus = data.ReadUInt64(ref offset);
            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.SizeOfStackCommit_PE32 = data.ReadUInt32(ref offset);
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.SizeOfStackCommit_PE32Plus = data.ReadUInt64(ref offset);
            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.SizeOfHeapReserve_PE32 = data.ReadUInt32(ref offset);
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.SizeOfHeapReserve_PE32Plus = data.ReadUInt64(ref offset);
            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.SizeOfHeapCommit_PE32 = data.ReadUInt32(ref offset);
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.SizeOfHeapCommit_PE32Plus = data.ReadUInt64(ref offset);
            optionalHeader.LoaderFlags = data.ReadUInt32(ref offset);
            optionalHeader.NumberOfRvaAndSizes = data.ReadUInt32(ref offset);

            #endregion

            #region Data Directories

            if (optionalHeader.NumberOfRvaAndSizes >= 1 && offset - initialOffset < optionalSize)
            {
                optionalHeader.ExportTable = new DataDirectory();
                optionalHeader.ExportTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.ExportTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 2 && offset - initialOffset < optionalSize)
            {
                optionalHeader.ImportTable = new DataDirectory();
                optionalHeader.ImportTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.ImportTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 3 && offset - initialOffset < optionalSize)
            {
                optionalHeader.ResourceTable = new DataDirectory();
                optionalHeader.ResourceTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.ResourceTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 4 && offset - initialOffset < optionalSize)
            {
                optionalHeader.ExceptionTable = new DataDirectory();
                optionalHeader.ExceptionTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.ExceptionTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 5 && offset - initialOffset < optionalSize)
            {
                optionalHeader.CertificateTable = new DataDirectory();
                optionalHeader.CertificateTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.CertificateTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 6 && offset - initialOffset < optionalSize)
            {
                optionalHeader.BaseRelocationTable = new DataDirectory();
                optionalHeader.BaseRelocationTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.BaseRelocationTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 7 && offset - initialOffset < optionalSize)
            {
                optionalHeader.Debug = new DataDirectory();
                optionalHeader.Debug.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.Debug.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 8 && offset - initialOffset < optionalSize)
            {
                optionalHeader.Architecture = data.ReadUInt64(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 9 && offset - initialOffset < optionalSize)
            {
                optionalHeader.GlobalPtr = new DataDirectory();
                optionalHeader.GlobalPtr.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.GlobalPtr.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 10 && offset - initialOffset < optionalSize)
            {
                optionalHeader.ThreadLocalStorageTable = new DataDirectory();
                optionalHeader.ThreadLocalStorageTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.ThreadLocalStorageTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 11 && offset - initialOffset < optionalSize)
            {
                optionalHeader.LoadConfigTable = new DataDirectory();
                optionalHeader.LoadConfigTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.LoadConfigTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 12 && offset - initialOffset < optionalSize)
            {
                optionalHeader.BoundImport = new DataDirectory();
                optionalHeader.BoundImport.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.BoundImport.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 13 && offset - initialOffset < optionalSize)
            {
                optionalHeader.ImportAddressTable = new DataDirectory();
                optionalHeader.ImportAddressTable.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.ImportAddressTable.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 14 && offset - initialOffset < optionalSize)
            {
                optionalHeader.DelayImportDescriptor = new DataDirectory();
                optionalHeader.DelayImportDescriptor.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.DelayImportDescriptor.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 15 && offset - initialOffset < optionalSize)
            {
                optionalHeader.CLRRuntimeHeader = new DataDirectory();
                optionalHeader.CLRRuntimeHeader.VirtualAddress = data.ReadUInt32(ref offset);
                optionalHeader.CLRRuntimeHeader.Size = data.ReadUInt32(ref offset);
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 16 && offset - initialOffset < optionalSize)
            {
                optionalHeader.Reserved = data.ReadUInt64(ref offset);
            }

            #endregion

            return optionalHeader;
        }

        /// <summary>
        /// Parse a byte array into a section table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="count">Number of section table entries to read</param>
        /// <returns>Filled section table on success, null on error</returns>
        private static SectionHeader[] ParseSectionTable(byte[] data, int offset, int count)
        {
            // TODO: Use marshalling here instead of building
            var sectionTable = new SectionHeader[count];

            for (int i = 0; i < count; i++)
            {
                var entry = new SectionHeader();
                entry.Name = data.ReadBytes(ref offset, 8);
                entry.VirtualSize = data.ReadUInt32(ref offset);
                entry.VirtualAddress = data.ReadUInt32(ref offset);
                entry.SizeOfRawData = data.ReadUInt32(ref offset);
                entry.PointerToRawData = data.ReadUInt32(ref offset);
                entry.PointerToRelocations = data.ReadUInt32(ref offset);
                entry.PointerToLinenumbers = data.ReadUInt32(ref offset);
                entry.NumberOfRelocations = data.ReadUInt16(ref offset);
                entry.NumberOfLinenumbers = data.ReadUInt16(ref offset);
                entry.Characteristics = (SectionFlags)data.ReadUInt32(ref offset);
                entry.COFFRelocations = new COFFRelocation[entry.NumberOfRelocations];
                for (int j = 0; i < entry.NumberOfRelocations; j++)
                {
                    // TODO: Seek to correct location and read data
                }
                entry.COFFLineNumbers = new COFFLineNumber[entry.NumberOfRelocations];
                for (int j = 0; i < entry.NumberOfRelocations; j++)
                {
                    // TODO: Seek to correct location and read data
                }
                sectionTable[i] = entry;
            }

            return sectionTable;
        }

        /// <summary>
        /// Parse a byte array into a COFF symbol table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="count">Number of COFF symbol table entries to read</param>
        /// <returns>Filled COFF symbol table on success, null on error</returns>
        private static COFFSymbolTableEntry[] ParseCOFFSymbolTable(byte[] data, int offset, uint count)
        {
            // TODO: Use marshalling here instead of building
            var coffSymbolTable = new COFFSymbolTableEntry[count];

            int auxSymbolsRemaining = 0;
            int currentSymbolType = 0;

            for (int i = 0; i < count; i++)
            {
                // Standard COFF Symbol Table Entry
                if (currentSymbolType == 0)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.ShortName = data.ReadBytes(ref offset, 8);
                    entry.Zeroes = BitConverter.ToUInt32(entry.ShortName, 0);
                    if (entry.Zeroes == 0)
                    {
                        entry.Offset = BitConverter.ToUInt32(entry.ShortName, 4);
                        entry.ShortName = null;
                    }
                    entry.Value = data.ReadUInt32(ref offset);
                    entry.SectionNumber = data.ReadUInt16(ref offset);
                    entry.SymbolType = (SymbolType)data.ReadUInt16(ref offset);
                    entry.StorageClass = (StorageClass)data.ReadByte(ref offset);
                    entry.NumberOfAuxSymbols = data.ReadByte(ref offset);
                    coffSymbolTable[i] = entry;

                    auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                    if (auxSymbolsRemaining == 0)
                        continue;

                    if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SymbolType == SymbolType.IMAGE_SYM_TYPE_FUNC
                        && entry.SectionNumber > 0)
                    {
                        currentSymbolType = 1;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_FUNCTION
                        && entry.ShortName != null
                        && ((entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x62 && entry.ShortName[2] == 0x66)  // .bf
                            || (entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x65 && entry.ShortName[2] == 0x66))) // .ef
                    {
                        currentSymbolType = 2;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SectionNumber == (ushort)SectionNumber.IMAGE_SYM_UNDEFINED
                        && entry.Value == 0)
                    {
                        currentSymbolType = 3;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_FILE)
                    {
                        // TODO: Symbol name should be ".file"
                        currentSymbolType = 4;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_STATIC)
                    {
                        // TODO: Should have the name of a section (like ".text")
                        currentSymbolType = 5;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_CLR_TOKEN)
                    {
                        currentSymbolType = 6;
                    }
                }

                // Auxiliary Format 1: Function Definitions
                else if (currentSymbolType == 1)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat1TagIndex = data.ReadUInt32(ref offset);
                    entry.AuxFormat1TotalSize = data.ReadUInt32(ref offset);
                    entry.AuxFormat1PointerToLinenumber = data.ReadUInt32(ref offset);
                    entry.AuxFormat1PointerToNextFunction = data.ReadUInt32(ref offset);
                    entry.AuxFormat1Unused = data.ReadUInt16(ref offset);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 2: .bf and .ef Symbols
                else if (currentSymbolType == 2)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat2Unused1 = data.ReadUInt32(ref offset);
                    entry.AuxFormat2Linenumber = data.ReadUInt16(ref offset);
                    entry.AuxFormat2Unused2 = data.ReadBytes(ref offset, 6);
                    entry.AuxFormat2PointerToNextFunction = data.ReadUInt32(ref offset);
                    entry.AuxFormat2Unused3 = data.ReadUInt16(ref offset);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 3: Weak Externals
                else if (currentSymbolType == 3)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat3TagIndex = data.ReadUInt32(ref offset);
                    entry.AuxFormat3Characteristics = data.ReadUInt32(ref offset);
                    entry.AuxFormat3Unused = data.ReadBytes(ref offset, 10);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 4: Files
                else if (currentSymbolType == 4)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat4FileName = data.ReadBytes(ref offset, 18);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 5: Section Definitions
                else if (currentSymbolType == 5)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat5Length = data.ReadUInt32(ref offset);
                    entry.AuxFormat5NumberOfRelocations = data.ReadUInt16(ref offset);
                    entry.AuxFormat5NumberOfLinenumbers = data.ReadUInt16(ref offset);
                    entry.AuxFormat5CheckSum = data.ReadUInt32(ref offset);
                    entry.AuxFormat5Number = data.ReadUInt16(ref offset);
                    entry.AuxFormat5Selection = data.ReadByte(ref offset);
                    entry.AuxFormat5Unused = data.ReadBytes(ref offset, 3);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 6: CLR Token Definition
                else if (currentSymbolType == 6)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat6AuxType = data.ReadByte(ref offset);
                    entry.AuxFormat6Reserved1 = data.ReadByte(ref offset);
                    entry.AuxFormat6SymbolTableIndex = data.ReadUInt32(ref offset);
                    entry.AuxFormat6Reserved2 = data.ReadBytes(ref offset, 12);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // If we hit the last aux symbol, go back to normal format
                if (auxSymbolsRemaining == 0)
                    currentSymbolType = 0;
            }

            return coffSymbolTable;
        }

        /// <summary>
        /// Parse a Stream into a COFF string table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled COFF string table on success, null on error</returns>
        private static COFFStringTable ParseCOFFStringTable(byte[] data, int offset)
        {
            // TODO: Use marshalling here instead of building
            var coffStringTable = new COFFStringTable();

            coffStringTable.TotalSize = data.ReadUInt32(ref offset);

            var strings = new List<string>();
            if (coffStringTable.TotalSize > 4)
            {
                uint totalSize = coffStringTable.TotalSize;
                while (totalSize > 0)
                {
                    int initialPosition = offset;
                    string str = data.ReadString(ref offset);
                    strings.Add(str);
                    totalSize -= (uint)(offset - initialPosition);
                }
            }

            coffStringTable.Strings = strings.ToArray();

            return coffStringTable;
        }

        /// <summary>
        /// Parse a byte array into an attribute certificate table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="endOffset">First address not part of the attribute certificate table</param>
        /// <returns>Filled attribute certificate on success, null on error</returns>
        private static AttributeCertificateTableEntry[] ParseAttributeCertificateTable(byte[] data, int offset, int endOffset)
        {
            var attributeCertificateTable = new List<AttributeCertificateTableEntry>();

            while (offset < endOffset)
            {
                var entry = new AttributeCertificateTableEntry();

                entry.Length = data.ReadUInt32(ref offset);
                entry.Revision = (WindowsCertificateRevision)data.ReadUInt16(ref offset);
                entry.CertificateType = (WindowsCertificateType)data.ReadUInt16(ref offset);
                if (entry.Length > 0)
                    entry.Certificate = data.ReadBytes(ref offset, (int)entry.Length);

                attributeCertificateTable.Add(entry);
            }

            return attributeCertificateTable.ToArray();
        }

        /// <summary>
        /// Parse a byte array into a delay-load directory table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled delay-load directory table on success, null on error</returns>
        private static DelayLoadDirectoryTable ParseDelayLoadDirectoryTable(byte[] data, int offset)
        {
            // TODO: Use marshalling here instead of building
            var delayLoadDirectoryTable = new DelayLoadDirectoryTable();

            delayLoadDirectoryTable.Attributes = data.ReadUInt32(ref offset);
            delayLoadDirectoryTable.Name = data.ReadUInt32(ref offset);
            delayLoadDirectoryTable.ModuleHandle = data.ReadUInt32(ref offset);
            delayLoadDirectoryTable.DelayImportAddressTable = data.ReadUInt32(ref offset);
            delayLoadDirectoryTable.DelayImportNameTable = data.ReadUInt32(ref offset);
            delayLoadDirectoryTable.BoundDelayImportTable = data.ReadUInt32(ref offset);
            delayLoadDirectoryTable.UnloadDelayImportTable = data.ReadUInt32(ref offset);
            delayLoadDirectoryTable.TimeStamp = data.ReadUInt32(ref offset);

            return delayLoadDirectoryTable;
        }

        /// <summary>
        /// Parse a byte array into a resource directory table
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled resource directory table on success, null on error</returns>
        private static ResourceDirectoryTable ParseResourceDirectoryTable(byte[] data, int offset, long initialOffset, SectionHeader[] sections)
        {
            // TODO: Use marshalling here instead of building
            var resourceDirectoryTable = new ResourceDirectoryTable();

            resourceDirectoryTable.Characteristics = data.ReadUInt32(ref offset);
            resourceDirectoryTable.TimeDateStamp = data.ReadUInt32(ref offset);
            resourceDirectoryTable.MajorVersion = data.ReadUInt16(ref offset);
            resourceDirectoryTable.MinorVersion = data.ReadUInt16(ref offset);
            resourceDirectoryTable.NumberOfNameEntries = data.ReadUInt16(ref offset);
            resourceDirectoryTable.NumberOfIDEntries = data.ReadUInt16(ref offset);

            // Perform top-level pass of data
            if (resourceDirectoryTable.NumberOfNameEntries > 0)
            {
                resourceDirectoryTable.NameEntries = new ResourceDirectoryEntry[resourceDirectoryTable.NumberOfNameEntries];
                for (int i = 0; i < resourceDirectoryTable.NumberOfNameEntries; i++)
                {
                    var entry = new ResourceDirectoryEntry();
                    entry.NameOffset = data.ReadUInt32(ref offset);

                    uint newOffset = data.ReadUInt32(ref offset);
                    if ((newOffset & 0xF0000000) != 0)
                        entry.SubdirectoryOffset = newOffset & ~0xF0000000;
                    else
                        entry.DataEntryOffset = newOffset;

                    // Read the name from the offset
                    int currentOffset = offset;
                    offset = (int)(entry.NameOffset + initialOffset);
                    var resourceDirectoryString = new ResourceDirectoryString();
                    resourceDirectoryString.Length = data.ReadUInt16(ref offset);
                    resourceDirectoryString.UnicodeString = data.ReadBytes(ref offset, resourceDirectoryString.Length);
                    entry.Name = resourceDirectoryString;
                    offset = currentOffset;

                    resourceDirectoryTable.NameEntries[i] = entry;
                }
            }
            if (resourceDirectoryTable.NumberOfIDEntries > 0)
            {
                resourceDirectoryTable.IDEntries = new ResourceDirectoryEntry[resourceDirectoryTable.NumberOfIDEntries];
                for (int i = 0; i < resourceDirectoryTable.NumberOfIDEntries; i++)
                {
                    var entry = new ResourceDirectoryEntry();
                    entry.IntegerID = data.ReadUInt32(ref offset);

                    uint newOffset = data.ReadUInt32(ref offset);
                    if ((newOffset & 0xF0000000) != 0)
                        entry.SubdirectoryOffset = newOffset & ~0xF0000000;
                    else
                        entry.DataEntryOffset = newOffset;

                    resourceDirectoryTable.IDEntries[i] = entry;
                }
            }

            // Read all leaves at this level
            if (resourceDirectoryTable.NumberOfNameEntries > 0)
            {
                foreach (var entry in resourceDirectoryTable.NameEntries)
                {
                    if (entry.SubdirectoryOffset != 0)
                        continue;

                    int newOffset = (int)(entry.DataEntryOffset + initialOffset);

                    var resourceDataEntry = new ResourceDataEntry();
                    resourceDataEntry.DataRVA = data.ReadUInt32(ref newOffset);
                    resourceDataEntry.Size = data.ReadUInt32(ref newOffset);
                    resourceDataEntry.Codepage = data.ReadUInt32(ref offset);
                    resourceDataEntry.Reserved = data.ReadUInt32(ref offset);

                    // Read the data from the offset
                    newOffset = (int)resourceDataEntry.DataRVA.ConvertVirtualAddress(sections);
                    if (newOffset > 0)
                        resourceDataEntry.Data = data.ReadBytes(ref newOffset, (int)resourceDataEntry.Size);

                    entry.DataEntry = resourceDataEntry;
                }
            }
            if (resourceDirectoryTable.NumberOfIDEntries > 0)
            {
                foreach (var entry in resourceDirectoryTable.IDEntries)
                {
                    if (entry.SubdirectoryOffset != 0)
                        continue;

                    int newOffset = (int)(entry.DataEntryOffset + initialOffset);

                    var resourceDataEntry = new ResourceDataEntry();
                    resourceDataEntry.DataRVA = data.ReadUInt32(ref newOffset);
                    resourceDataEntry.Size = data.ReadUInt32(ref newOffset);
                    resourceDataEntry.Codepage = data.ReadUInt32(ref newOffset);
                    resourceDataEntry.Reserved = data.ReadUInt32(ref newOffset);

                    // Read the data from the offset
                    newOffset = (int)resourceDataEntry.DataRVA.ConvertVirtualAddress(sections);
                    if (newOffset > 0)
                        resourceDataEntry.Data = data.ReadBytes(ref newOffset, (int)resourceDataEntry.Size);

                    entry.DataEntry = resourceDataEntry;
                }
            }

            // Now go one level lower
            if (resourceDirectoryTable.NumberOfNameEntries > 0)
            {
                foreach (var entry in resourceDirectoryTable.NameEntries)
                {
                    if (entry.DataEntryOffset != 0)
                        continue;

                    int newOffset = (int)(entry.SubdirectoryOffset + initialOffset);
                    entry.Subdirectory = ParseResourceDirectoryTable(data, newOffset, initialOffset, sections);
                }
            }
            if (resourceDirectoryTable.NumberOfIDEntries > 0)
            {
                foreach (var entry in resourceDirectoryTable.IDEntries)
                {
                    if (entry.DataEntryOffset != 0)
                        continue;

                    int newOffset = (int)(entry.SubdirectoryOffset + initialOffset);
                    entry.Subdirectory = ParseResourceDirectoryTable(data, newOffset, initialOffset, sections);
                }
            }

            return resourceDirectoryTable;
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Portable Executable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(Stream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new executable to fill
            var executable = new Executable();

            #region MS-DOS Stub

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            #endregion

            #region Signature

            data.Seek(initialOffset + stub.Header.NewExeHeaderAddr, SeekOrigin.Begin);
            executable.Signature = new byte[4];
            for (int i = 0; i < executable.Signature.Length; i++)
            {
                executable.Signature[i] = data.ReadByteValue();
            }
            if (executable.Signature[0] != 'P' || executable.Signature[1] != 'E' || executable.Signature[2] != '\0' || executable.Signature[3] != '\0')
                return null;

            #endregion

            #region COFF File Header

            // Try to parse the COFF file header
            var coffFileHeader = ParseCOFFFileHeader(data);
            if (coffFileHeader == null)
                return null;

            // Set the COFF file header
            executable.COFFFileHeader = coffFileHeader;

            #endregion

            #region Optional Header

            // Try to parse the optional header
            var optionalHeader = ParseOptionalHeader(data, coffFileHeader.SizeOfOptionalHeader);
            if (optionalHeader == null)
                return null;

            // Set the optional header
            executable.OptionalHeader = optionalHeader;

            #endregion

            #region Section Table

            // Try to parse the section table
            var sectionTable = ParseSectionTable(data, coffFileHeader.NumberOfSections);
            if (sectionTable == null)
                return null;

            // Set the section table
            executable.SectionTable = sectionTable;

            #endregion

            #region COFF Symbol Table and COFF String Table

            // TODO: Validate that this is correct with an "old" PE
            if (coffFileHeader.PointerToSymbolTable != 0)
            {
                // If the offset for the COFF symbol table doesn't exist
                int symbolTableAddress = initialOffset
                    + (int)coffFileHeader.PointerToSymbolTable.ConvertVirtualAddress(executable.SectionTable);
                if (symbolTableAddress >= data.Length)
                    return executable;

                // Try to parse the COFF symbol table
                data.Seek(symbolTableAddress, SeekOrigin.Begin);
                var coffSymbolTable = ParseCOFFSymbolTable(data, coffFileHeader.NumberOfSymbols);
                if (coffSymbolTable == null)
                    return null;

                // Set the COFF symbol table
                executable.COFFSymbolTable = coffSymbolTable;

                // Try to parse the COFF string table
                var coffStringTable = ParseCOFFStringTable(data);
                if (coffStringTable == null)
                    return null;

                // Set the COFF string table
                executable.COFFStringTable = coffStringTable;
            }

            #endregion

            #region Attribute Certificate Table

            if (optionalHeader.CertificateTable != null && optionalHeader.CertificateTable.VirtualAddress != 0)
            {
                // If the offset for the attribute certificate table doesn't exist
                int certificateTableAddress = initialOffset
                    + (int)optionalHeader.CertificateTable.VirtualAddress;
                 if (certificateTableAddress >= data.Length)
                        return executable;

                // Try to parse the attribute certificate table
                data.Seek(certificateTableAddress, SeekOrigin.Begin);
                int endOffset = (int)(certificateTableAddress + optionalHeader.CertificateTable.Size);
                var attributeCertificateTable = ParseAttributeCertificateTable(data, endOffset);
                if (attributeCertificateTable == null)
                    return null;

                // Set the attribute certificate table
                executable.AttributeCertificateTable = attributeCertificateTable;
            }

            #endregion

            #region Delay-Load Directory Table

            if (optionalHeader.DelayImportDescriptor != null && optionalHeader.DelayImportDescriptor.VirtualAddress != 0)
            {
                // If the offset for the delay-load directory table doesn't exist
                int delayLoadDirectoryTableAddress = initialOffset
                    + (int)optionalHeader.DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (delayLoadDirectoryTableAddress >= data.Length)
                    return executable;

                // Try to parse the delay-load directory table
                data.Seek(delayLoadDirectoryTableAddress, SeekOrigin.Begin);
                var delayLoadDirectoryTable = ParseDelayLoadDirectoryTable(data);
                if (delayLoadDirectoryTable == null)
                    return null;

                // Set the delay-load directory table
                executable.DelayLoadDirectoryTable = delayLoadDirectoryTable;
            }

            #endregion

            #region Resource Directory Table

            // Should also be in the '.rsrc' section
            if (optionalHeader.ResourceTable != null && optionalHeader.ResourceTable.VirtualAddress != 0)
            {
                // If the offset for the resource directory table doesn't exist
                int resourceTableAddress = initialOffset
                    + (int)optionalHeader.ResourceTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (resourceTableAddress >= data.Length)
                    return executable;

                // Try to parse the resource directory table
                data.Seek(resourceTableAddress, SeekOrigin.Begin);
                var resourceDirectoryTable = ParseResourceDirectoryTable(data, data.Position, executable.SectionTable);
                if (resourceDirectoryTable == null)
                    return null;

                // Set the resource directory table
                executable.ResourceDirectoryTable = resourceDirectoryTable;
            }

            #endregion

            // TODO: Finish implementing PE parsing
            return executable;
        }

        /// <summary>
        /// Parse a Stream into a Portable Executable COFF file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable header on success, null on error</returns>
        private static COFFFileHeader ParseCOFFFileHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var fileHeader = new COFFFileHeader();

            fileHeader.Machine = (MachineType)data.ReadUInt16();
            fileHeader.NumberOfSections = data.ReadUInt16();
            fileHeader.TimeDateStamp = data.ReadUInt32();
            fileHeader.PointerToSymbolTable = data.ReadUInt32();
            fileHeader.NumberOfSymbols = data.ReadUInt32();
            fileHeader.SizeOfOptionalHeader = data.ReadUInt16();
            fileHeader.Characteristics = (Characteristics)data.ReadUInt16();

            return fileHeader;
        }

        /// <summary>
        /// Parse a Stream into an optional header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="optionalSize">Size of the optional header</param>
        /// <returns>Filled optional header on success, null on error</returns>
        private static OptionalHeader ParseOptionalHeader(Stream data, int optionalSize)
        {
            long initialOffset = data.Position;

            // TODO: Use marshalling here instead of building
            var optionalHeader = new OptionalHeader();

            #region Standard Fields

            optionalHeader.Magic = (OptionalHeaderMagicNumber)data.ReadUInt16();
            optionalHeader.MajorLinkerVersion = data.ReadByteValue();
            optionalHeader.MinorLinkerVersion = data.ReadByteValue();
            optionalHeader.SizeOfCode = data.ReadUInt32();
            optionalHeader.SizeOfInitializedData = data.ReadUInt32();
            optionalHeader.SizeOfUninitializedData = data.ReadUInt32();
            optionalHeader.AddressOfEntryPoint = data.ReadUInt32();
            optionalHeader.BaseOfCode = data.ReadUInt32();

            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.BaseOfData = data.ReadUInt32();

            #endregion

            #region Windows-Specific Fields

            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.ImageBase_PE32 = data.ReadUInt32();
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.ImageBase_PE32Plus = data.ReadUInt64();
            optionalHeader.SectionAlignment = data.ReadUInt32();
            optionalHeader.FileAlignment = data.ReadUInt32();
            optionalHeader.MajorOperatingSystemVersion = data.ReadUInt16();
            optionalHeader.MinorOperatingSystemVersion = data.ReadUInt16();
            optionalHeader.MajorImageVersion = data.ReadUInt16();
            optionalHeader.MinorImageVersion = data.ReadUInt16();
            optionalHeader.MajorSubsystemVersion = data.ReadUInt16();
            optionalHeader.MinorSubsystemVersion = data.ReadUInt16();
            optionalHeader.Win32VersionValue = data.ReadUInt32();
            optionalHeader.SizeOfImage = data.ReadUInt32();
            optionalHeader.SizeOfHeaders = data.ReadUInt32();
            optionalHeader.CheckSum = data.ReadUInt32();
            optionalHeader.Subsystem = (WindowsSubsystem)data.ReadUInt16();
            optionalHeader.DllCharacteristics = (DllCharacteristics)data.ReadUInt16();
            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.SizeOfStackReserve_PE32 = data.ReadUInt32();
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.SizeOfStackReserve_PE32Plus = data.ReadUInt64();
            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.SizeOfStackCommit_PE32 = data.ReadUInt32();
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.SizeOfStackCommit_PE32Plus = data.ReadUInt64();
            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.SizeOfHeapReserve_PE32 = data.ReadUInt32();
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.SizeOfHeapReserve_PE32Plus = data.ReadUInt64();
            if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32)
                optionalHeader.SizeOfHeapCommit_PE32 = data.ReadUInt32();
            else if (optionalHeader.Magic == OptionalHeaderMagicNumber.PE32Plus)
                optionalHeader.SizeOfHeapCommit_PE32Plus = data.ReadUInt64();
            optionalHeader.LoaderFlags = data.ReadUInt32();
            optionalHeader.NumberOfRvaAndSizes = data.ReadUInt32();

            #endregion

            #region Data Directories

            if (optionalHeader.NumberOfRvaAndSizes >= 1 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.ExportTable = new DataDirectory();
                optionalHeader.ExportTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.ExportTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 2 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.ImportTable = new DataDirectory();
                optionalHeader.ImportTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.ImportTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 3 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.ResourceTable = new DataDirectory();
                optionalHeader.ResourceTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.ResourceTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 4 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.ExceptionTable = new DataDirectory();
                optionalHeader.ExceptionTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.ExceptionTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 5 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.CertificateTable = new DataDirectory();
                optionalHeader.CertificateTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.CertificateTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 6 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.BaseRelocationTable = new DataDirectory();
                optionalHeader.BaseRelocationTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.BaseRelocationTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 7 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.Debug = new DataDirectory();
                optionalHeader.Debug.VirtualAddress = data.ReadUInt32();
                optionalHeader.Debug.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 8 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.Architecture = data.ReadUInt64();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 9 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.GlobalPtr = new DataDirectory();
                optionalHeader.GlobalPtr.VirtualAddress = data.ReadUInt32();
                optionalHeader.GlobalPtr.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 10 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.ThreadLocalStorageTable = new DataDirectory();
                optionalHeader.ThreadLocalStorageTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.ThreadLocalStorageTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 11 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.LoadConfigTable = new DataDirectory();
                optionalHeader.LoadConfigTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.LoadConfigTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 12 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.BoundImport = new DataDirectory();
                optionalHeader.BoundImport.VirtualAddress = data.ReadUInt32();
                optionalHeader.BoundImport.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 13 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.ImportAddressTable = new DataDirectory();
                optionalHeader.ImportAddressTable.VirtualAddress = data.ReadUInt32();
                optionalHeader.ImportAddressTable.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 14 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.DelayImportDescriptor = new DataDirectory();
                optionalHeader.DelayImportDescriptor.VirtualAddress = data.ReadUInt32();
                optionalHeader.DelayImportDescriptor.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 15 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.CLRRuntimeHeader = new DataDirectory();
                optionalHeader.CLRRuntimeHeader.VirtualAddress = data.ReadUInt32();
                optionalHeader.CLRRuntimeHeader.Size = data.ReadUInt32();
            }
            if (optionalHeader.NumberOfRvaAndSizes >= 16 && data.Position - initialOffset < optionalSize)
            {
                optionalHeader.Reserved = data.ReadUInt64();
            }

            #endregion

            return optionalHeader;
        }

        /// <summary>
        /// Parse a Stream into a section table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of section table entries to read</param>
        /// <returns>Filled section table on success, null on error</returns>
        private static SectionHeader[] ParseSectionTable(Stream data, int count)
        {
            // TODO: Use marshalling here instead of building
            var sectionTable = new SectionHeader[count];

            for (int i = 0; i < count; i++)
            {
                var entry = new SectionHeader();
                entry.Name = data.ReadBytes(8);
                entry.VirtualSize = data.ReadUInt32();
                entry.VirtualAddress = data.ReadUInt32();
                entry.SizeOfRawData = data.ReadUInt32();
                entry.PointerToRawData = data.ReadUInt32();
                entry.PointerToRelocations = data.ReadUInt32();
                entry.PointerToLinenumbers = data.ReadUInt32();
                entry.NumberOfRelocations = data.ReadUInt16();
                entry.NumberOfLinenumbers = data.ReadUInt16();
                entry.Characteristics = (SectionFlags)data.ReadUInt32();
                entry.COFFRelocations = new COFFRelocation[entry.NumberOfRelocations];
                for (int j = 0; j < entry.NumberOfRelocations; j++)
                {
                    // TODO: Seek to correct location and read data
                }
                entry.COFFLineNumbers = new COFFLineNumber[entry.NumberOfLinenumbers];
                for (int j = 0; j < entry.NumberOfLinenumbers; j++)
                {
                    // TODO: Seek to correct location and read data
                }
                sectionTable[i] = entry;
            }

            return sectionTable;
        }

        /// <summary>
        /// Parse a Stream into a COFF symbol table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="count">Number of COFF symbol table entries to read</param>
        /// <returns>Filled COFF symbol table on success, null on error</returns>
        private static COFFSymbolTableEntry[] ParseCOFFSymbolTable(Stream data, uint count)
        {
            // TODO: Use marshalling here instead of building
            var coffSymbolTable = new COFFSymbolTableEntry[count];

            int auxSymbolsRemaining = 0;
            int currentSymbolType = 0;

            for (int i = 0; i < count; i++)
            {
                // Standard COFF Symbol Table Entry
                if (currentSymbolType == 0)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.ShortName = data.ReadBytes(8);
                    entry.Zeroes = BitConverter.ToUInt32(entry.ShortName, 0);
                    if (entry.Zeroes == 0)
                    {
                        entry.Offset = BitConverter.ToUInt32(entry.ShortName, 4);
                        entry.ShortName = null;
                    }
                    entry.Value = data.ReadUInt32();
                    entry.SectionNumber = data.ReadUInt16();
                    entry.SymbolType = (SymbolType)data.ReadUInt16();
                    entry.StorageClass = (StorageClass)data.ReadByte();
                    entry.NumberOfAuxSymbols = data.ReadByteValue();
                    coffSymbolTable[i] = entry;

                    auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                    if (auxSymbolsRemaining == 0)
                        continue;

                    if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SymbolType == SymbolType.IMAGE_SYM_TYPE_FUNC
                        && entry.SectionNumber > 0)
                    {
                        currentSymbolType = 1;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_FUNCTION
                        && entry.ShortName != null
                        && ((entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x62 && entry.ShortName[2] == 0x66)  // .bf
                            || (entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x65 && entry.ShortName[2] == 0x66))) // .ef
                    {
                        currentSymbolType = 2;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SectionNumber == (ushort)SectionNumber.IMAGE_SYM_UNDEFINED
                        && entry.Value == 0)
                    {
                        currentSymbolType = 3;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_FILE)
                    {
                        // TODO: Symbol name should be ".file"
                        currentSymbolType = 4;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_STATIC)
                    {
                        // TODO: Should have the name of a section (like ".text")
                        currentSymbolType = 5;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_CLR_TOKEN)
                    {
                        currentSymbolType = 6;
                    }
                }

                // Auxiliary Format 1: Function Definitions
                else if (currentSymbolType == 1)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat1TagIndex = data.ReadUInt32();
                    entry.AuxFormat1TotalSize = data.ReadUInt32();
                    entry.AuxFormat1PointerToLinenumber = data.ReadUInt32();
                    entry.AuxFormat1PointerToNextFunction = data.ReadUInt32();
                    entry.AuxFormat1Unused = data.ReadUInt16();
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 2: .bf and .ef Symbols
                else if (currentSymbolType == 2)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat2Unused1 = data.ReadUInt32();
                    entry.AuxFormat2Linenumber = data.ReadUInt16();
                    entry.AuxFormat2Unused2 = data.ReadBytes(6);
                    entry.AuxFormat2PointerToNextFunction = data.ReadUInt32();
                    entry.AuxFormat2Unused3 = data.ReadUInt16();
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 3: Weak Externals
                else if (currentSymbolType == 3)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat3TagIndex = data.ReadUInt32();
                    entry.AuxFormat3Characteristics = data.ReadUInt32();
                    entry.AuxFormat3Unused = data.ReadBytes(10);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 4: Files
                else if (currentSymbolType == 4)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat4FileName = data.ReadBytes(18);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 5: Section Definitions
                else if (currentSymbolType == 5)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat5Length = data.ReadUInt32();
                    entry.AuxFormat5NumberOfRelocations = data.ReadUInt16();
                    entry.AuxFormat5NumberOfLinenumbers = data.ReadUInt16();
                    entry.AuxFormat5CheckSum = data.ReadUInt32();
                    entry.AuxFormat5Number = data.ReadUInt16();
                    entry.AuxFormat5Selection = data.ReadByteValue();
                    entry.AuxFormat5Unused = data.ReadBytes(3);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // Auxiliary Format 6: CLR Token Definition
                else if (currentSymbolType == 6)
                {
                    var entry = new COFFSymbolTableEntry();
                    entry.AuxFormat6AuxType = data.ReadByteValue();
                    entry.AuxFormat6Reserved1 = data.ReadByteValue();
                    entry.AuxFormat6SymbolTableIndex = data.ReadUInt32();
                    entry.AuxFormat6Reserved2 = data.ReadBytes(12);
                    coffSymbolTable[i] = entry;
                    auxSymbolsRemaining--;
                }

                // If we hit the last aux symbol, go back to normal format
                if (auxSymbolsRemaining == 0)
                    currentSymbolType = 0;
            }

            return coffSymbolTable;
        }

        /// <summary>
        /// Parse a Stream into a COFF string table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled COFF string table on success, null on error</returns>
        private static COFFStringTable ParseCOFFStringTable(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var coffStringTable = new COFFStringTable();

            coffStringTable.TotalSize = data.ReadUInt32();

            var strings = new List<string>();
            if (coffStringTable.TotalSize > 4)
            {
                uint totalSize = coffStringTable.TotalSize;
                while (totalSize > 0)
                {
                    long initialPosition = data.Position;
                    string str = data.ReadString();
                    strings.Add(str);
                    totalSize -= (uint)(data.Position - initialPosition);
                }
            }

            coffStringTable.Strings = strings.ToArray();

            return coffStringTable;
        }

        /// <summary>
        /// Parse a Stream into an attribute certificate table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the attribute certificate table</param>
        /// <returns>Filled attribute certificate on success, null on error</returns>
        private static AttributeCertificateTableEntry[] ParseAttributeCertificateTable(Stream data, int endOffset)
        {
            var attributeCertificateTable = new List<AttributeCertificateTableEntry>();

            while (data.Position < endOffset)
            {
                var entry = new AttributeCertificateTableEntry();

                entry.Length = data.ReadUInt32();
                entry.Revision = (WindowsCertificateRevision)data.ReadUInt16();
                entry.CertificateType = (WindowsCertificateType)data.ReadUInt16();
                if (entry.Length > 0)
                    entry.Certificate = data.ReadBytes((int)entry.Length);

                attributeCertificateTable.Add(entry);
            }

            return attributeCertificateTable.ToArray();
        }

        /// <summary>
        /// Parse a byte array into a delay-load directory table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled delay-load directory table on success, null on error</returns>
        private static DelayLoadDirectoryTable ParseDelayLoadDirectoryTable(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var delayLoadDirectoryTable = new DelayLoadDirectoryTable();

            delayLoadDirectoryTable.Attributes = data.ReadUInt32();
            delayLoadDirectoryTable.Name = data.ReadUInt32();
            delayLoadDirectoryTable.ModuleHandle = data.ReadUInt32();
            delayLoadDirectoryTable.DelayImportAddressTable = data.ReadUInt32();
            delayLoadDirectoryTable.DelayImportNameTable = data.ReadUInt32();
            delayLoadDirectoryTable.BoundDelayImportTable = data.ReadUInt32();
            delayLoadDirectoryTable.UnloadDelayImportTable = data.ReadUInt32();
            delayLoadDirectoryTable.TimeStamp = data.ReadUInt32();

            return delayLoadDirectoryTable;
        }

        /// <summary>
        /// Parse a Stream into a resource directory table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled resource directory table on success, null on error</returns>
        private static ResourceDirectoryTable ParseResourceDirectoryTable(Stream data, long initialOffset, SectionHeader[] sections)
        {
            // TODO: Use marshalling here instead of building
            var resourceDirectoryTable = new ResourceDirectoryTable();

            resourceDirectoryTable.Characteristics = data.ReadUInt32();
            resourceDirectoryTable.TimeDateStamp = data.ReadUInt32();
            resourceDirectoryTable.MajorVersion = data.ReadUInt16();
            resourceDirectoryTable.MinorVersion = data.ReadUInt16();
            resourceDirectoryTable.NumberOfNameEntries = data.ReadUInt16();
            resourceDirectoryTable.NumberOfIDEntries = data.ReadUInt16();

            // Perform top-level pass of data
            if (resourceDirectoryTable.NumberOfNameEntries > 0)
            {
                resourceDirectoryTable.NameEntries = new ResourceDirectoryEntry[resourceDirectoryTable.NumberOfNameEntries];
                for (int i = 0; i < resourceDirectoryTable.NumberOfNameEntries; i++)
                {
                    var entry = new ResourceDirectoryEntry();
                    entry.NameOffset = data.ReadUInt32();

                    uint offset = data.ReadUInt32();
                    if ((offset & 0xF0000000) != 0)
                        entry.SubdirectoryOffset = offset & ~0xF0000000;
                    else
                        entry.DataEntryOffset = offset;

                    // Read the name from the offset
                    long currentOffset = data.Position;
                    offset = entry.NameOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);
                    var resourceDirectoryString = new ResourceDirectoryString();
                    resourceDirectoryString.Length = data.ReadUInt16();
                    resourceDirectoryString.UnicodeString = data.ReadBytes(resourceDirectoryString.Length);
                    entry.Name = resourceDirectoryString;
                    data.Seek(currentOffset, SeekOrigin.Begin);

                    resourceDirectoryTable.NameEntries[i] = entry;
                }
            }
            if (resourceDirectoryTable.NumberOfIDEntries > 0)
            {
                resourceDirectoryTable.IDEntries = new ResourceDirectoryEntry[resourceDirectoryTable.NumberOfIDEntries];
                for (int i = 0; i < resourceDirectoryTable.NumberOfIDEntries; i++)
                {
                    var entry = new ResourceDirectoryEntry();
                    entry.IntegerID = data.ReadUInt32();

                    uint offset = data.ReadUInt32();
                    if ((offset & 0xF0000000) != 0)
                        entry.SubdirectoryOffset = offset & ~0xF0000000;
                    else
                        entry.DataEntryOffset = offset;

                    resourceDirectoryTable.IDEntries[i] = entry;
                }
            }

            // Read all leaves at this level
            if (resourceDirectoryTable.NumberOfNameEntries > 0)
            {
                foreach (var entry in resourceDirectoryTable.NameEntries)
                {
                    if (entry.SubdirectoryOffset != 0)
                        continue;

                    uint offset = entry.DataEntryOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);

                    var resourceDataEntry = new ResourceDataEntry();
                    resourceDataEntry.DataRVA = data.ReadUInt32();
                    resourceDataEntry.Size = data.ReadUInt32();
                    resourceDataEntry.Codepage = data.ReadUInt32();
                    resourceDataEntry.Reserved = data.ReadUInt32();

                    // Read the data from the offset
                    offset = resourceDataEntry.DataRVA.ConvertVirtualAddress(sections);
                    if (offset > 0)
                    {
                        data.Seek(offset, SeekOrigin.Begin);
                        resourceDataEntry.Data = data.ReadBytes((int)resourceDataEntry.Size);
                    }

                    entry.DataEntry = resourceDataEntry;
                }
            }
            if (resourceDirectoryTable.NumberOfIDEntries > 0)
            {
                foreach (var entry in resourceDirectoryTable.IDEntries)
                {
                    if (entry.SubdirectoryOffset != 0)
                        continue;

                    uint offset = entry.DataEntryOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);

                    var resourceDataEntry = new ResourceDataEntry();
                    resourceDataEntry.DataRVA = data.ReadUInt32();
                    resourceDataEntry.Size = data.ReadUInt32();
                    resourceDataEntry.Codepage = data.ReadUInt32();
                    resourceDataEntry.Reserved = data.ReadUInt32();

                    // Read the data from the offset
                    offset = resourceDataEntry.DataRVA.ConvertVirtualAddress(sections);
                    if (offset > 0)
                    {
                        data.Seek(offset, SeekOrigin.Begin);
                        resourceDataEntry.Data = data.ReadBytes((int)resourceDataEntry.Size);
                    }

                    entry.DataEntry = resourceDataEntry;
                }
            }

            // Now go one level lower
            if (resourceDirectoryTable.NumberOfNameEntries > 0)
            {
                foreach (var entry in resourceDirectoryTable.NameEntries)
                {
                    if (entry.DataEntryOffset != 0)
                        continue;

                    uint offset = entry.SubdirectoryOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);
                    entry.Subdirectory = ParseResourceDirectoryTable(data, initialOffset, sections);
                }
            }
            if (resourceDirectoryTable.NumberOfIDEntries > 0)
            {
                foreach (var entry in resourceDirectoryTable.IDEntries)
                {
                    if (entry.DataEntryOffset != 0)
                        continue;

                    uint offset = entry.SubdirectoryOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);
                    entry.Subdirectory = ParseResourceDirectoryTable(data, initialOffset, sections);
                }
            }

            return resourceDirectoryTable;
        }

        #endregion
    }
}