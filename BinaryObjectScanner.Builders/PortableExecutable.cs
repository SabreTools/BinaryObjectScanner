using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryObjectScanner.Utilities;
using SabreTools.Models.PortableExecutable;
using static SabreTools.Models.PortableExecutable.Constants;

namespace BinaryObjectScanner.Builders
{
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

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseExecutable(dataStream);
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
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
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
            byte[] signature = data.ReadBytes(4);
            executable.Signature = Encoding.ASCII.GetString(signature);
            if (executable.Signature != SignatureString)
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
            if (coffFileHeader.PointerToSymbolTable.ConvertVirtualAddress(executable.SectionTable) != 0)
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

            if (optionalHeader.DelayImportDescriptor != null && optionalHeader.DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(executable.SectionTable) != 0)
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

            #region Base Relocation Table

            // Should also be in a '.reloc' section
            if (optionalHeader.BaseRelocationTable != null && optionalHeader.BaseRelocationTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable) != 0)
            {
                // If the offset for the base relocation table doesn't exist
                int baseRelocationTableAddress = initialOffset
                    + (int)optionalHeader.BaseRelocationTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (baseRelocationTableAddress >= data.Length)
                    return executable;

                // Try to parse the base relocation table
                data.Seek(baseRelocationTableAddress, SeekOrigin.Begin);
                int endOffset = (int)(baseRelocationTableAddress + optionalHeader.BaseRelocationTable.Size);
                var baseRelocationTable = ParseBaseRelocationTable(data, endOffset, executable.SectionTable);
                if (baseRelocationTable == null)
                    return null;

                // Set the base relocation table
                executable.BaseRelocationTable = baseRelocationTable;
            }

            #endregion

            #region Debug Table

            // Should also be in a '.debug' section
            if (optionalHeader.Debug != null && optionalHeader.Debug.VirtualAddress.ConvertVirtualAddress(executable.SectionTable) != 0)
            {
                // If the offset for the debug table doesn't exist
                int debugTableAddress = initialOffset
                    + (int)optionalHeader.Debug.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (debugTableAddress >= data.Length)
                    return executable;

                // Try to parse the debug table
                data.Seek(debugTableAddress, SeekOrigin.Begin);
                int endOffset = (int)(debugTableAddress + optionalHeader.Debug.Size);
                var debugTable = ParseDebugTable(data, endOffset, executable.SectionTable);
                if (debugTable == null)
                    return null;

                // Set the debug table
                executable.DebugTable = debugTable;
            }

            #endregion

            #region Export Table

            // Should also be in a '.edata' section
            if (optionalHeader.ExportTable != null && optionalHeader.ExportTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable) != 0)
            {
                // If the offset for the export table doesn't exist
                int exportTableAddress = initialOffset
                    + (int)optionalHeader.ExportTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (exportTableAddress >= data.Length)
                    return executable;

                // Try to parse the export table
                data.Seek(exportTableAddress, SeekOrigin.Begin);
                var exportTable = ParseExportTable(data, executable.SectionTable);
                if (exportTable == null)
                    return null;

                // Set the export table
                executable.ExportTable = exportTable;
            }

            #endregion

            #region Import Table

            // Should also be in a '.idata' section
            if (optionalHeader.ImportTable != null && optionalHeader.ImportTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable) != 0)
            {
                // If the offset for the import table doesn't exist
                int importTableAddress = initialOffset
                    + (int)optionalHeader.ImportTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (importTableAddress >= data.Length)
                    return executable;

                // Try to parse the import table
                data.Seek(importTableAddress, SeekOrigin.Begin);
                var importTable = ParseImportTable(data, optionalHeader.Magic, executable.SectionTable);
                if (importTable == null)
                    return null;

                // Set the import table
                executable.ImportTable = importTable;
            }

            #endregion

            #region Resource Directory Table

            // Should also be in a '.rsrc' section
            if (optionalHeader.ResourceTable != null && optionalHeader.ResourceTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable) != 0)
            {
                // If the offset for the resource directory table doesn't exist
                int resourceTableAddress = initialOffset
                    + (int)optionalHeader.ResourceTable.VirtualAddress.ConvertVirtualAddress(executable.SectionTable);
                if (resourceTableAddress >= data.Length)
                    return executable;

                // Try to parse the resource directory table
                data.Seek(resourceTableAddress, SeekOrigin.Begin);
                var resourceDirectoryTable = ParseResourceDirectoryTable(data, data.Position, executable.SectionTable, true);
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
            if (coffStringTable.TotalSize <= 4)
                return coffStringTable;

            var strings = new List<string>();

            uint totalSize = coffStringTable.TotalSize;
            while (totalSize > 0 && data.Position < data.Length)
            {
                long initialPosition = data.Position;
                string str = data.ReadString();
                strings.Add(str);
                totalSize -= (uint)(data.Position - initialPosition);
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

            while (data.Position < endOffset && data.Position != data.Length)
            {
                var entry = new AttributeCertificateTableEntry();

                entry.Length = data.ReadUInt32();
                entry.Revision = (WindowsCertificateRevision)data.ReadUInt16();
                entry.CertificateType = (WindowsCertificateType)data.ReadUInt16();

                int certificateDataLength = (int)(entry.Length - 8);
                if (certificateDataLength > 0)
                    entry.Certificate = data.ReadBytes(certificateDataLength);

                attributeCertificateTable.Add(entry);

                // Align to the 8-byte boundary
                while ((data.Position % 8) != 0 && data.Position < endOffset && data.Position != data.Length)
                    _ = data.ReadByteValue();
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
        /// Parse a Stream into a base relocation table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the base relocation table</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled base relocation table on success, null on error</returns>
        private static BaseRelocationBlock[] ParseBaseRelocationTable(Stream data, int endOffset, SectionHeader[] sections)
        {
            // TODO: Use marshalling here instead of building
            var baseRelocationTable = new List<BaseRelocationBlock>();

            while (data.Position < endOffset)
            {
                var baseRelocationBlock = new BaseRelocationBlock();

                baseRelocationBlock.PageRVA = data.ReadUInt32();
                baseRelocationBlock.BlockSize = data.ReadUInt32();

                var typeOffsetFieldEntries = new List<BaseRelocationTypeOffsetFieldEntry>();
                int totalSize = 8;
                while (totalSize < baseRelocationBlock.BlockSize && data.Position < data.Length)
                {
                    var baseRelocationTypeOffsetFieldEntry = new BaseRelocationTypeOffsetFieldEntry();

                    ushort typeAndOffsetField = data.ReadUInt16();
                    baseRelocationTypeOffsetFieldEntry.BaseRelocationType = (BaseRelocationTypes)(typeAndOffsetField >> 12);
                    baseRelocationTypeOffsetFieldEntry.Offset = (ushort)(typeAndOffsetField & 0x0FFF);

                    typeOffsetFieldEntries.Add(baseRelocationTypeOffsetFieldEntry);
                    totalSize += 2;
                }

                baseRelocationBlock.TypeOffsetFieldEntries = typeOffsetFieldEntries.ToArray();

                baseRelocationTable.Add(baseRelocationBlock);
            }

            return baseRelocationTable.ToArray();
        }

        /// <summary>
        /// Parse a Stream into a debug table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="endOffset">First address not part of the debug table</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled debug table on success, null on error</returns>
        private static DebugTable ParseDebugTable(Stream data, int endOffset, SectionHeader[] sections)
        {
            // TODO: Use marshalling here instead of building
            var debugTable = new DebugTable();

            var debugDirectoryTable = new List<DebugDirectoryEntry>();

            while (data.Position < endOffset)
            {
                var debugDirectoryEntry = new DebugDirectoryEntry();

                debugDirectoryEntry.Characteristics = data.ReadUInt32();
                debugDirectoryEntry.TimeDateStamp = data.ReadUInt32();
                debugDirectoryEntry.MajorVersion = data.ReadUInt16();
                debugDirectoryEntry.MinorVersion = data.ReadUInt16();
                debugDirectoryEntry.DebugType = (DebugType)data.ReadUInt32();
                debugDirectoryEntry.SizeOfData = data.ReadUInt32();
                debugDirectoryEntry.AddressOfRawData = data.ReadUInt32();
                debugDirectoryEntry.PointerToRawData = data.ReadUInt32();

                debugDirectoryTable.Add(debugDirectoryEntry);
            }

            debugTable.DebugDirectoryTable = debugDirectoryTable.ToArray();

            // TODO: Should we read the debug data in? Most of it is unformatted or undocumented
            // TODO: Implement .debug$F (Object Only) / IMAGE_DEBUG_TYPE_FPO

            return debugTable;
        }

        /// <summary>
        /// Parse a Stream into a export table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled export table on success, null on error</returns>
        private static ExportTable ParseExportTable(Stream data, SectionHeader[] sections)
        {
            // TODO: Use marshalling here instead of building
            var exportTable = new ExportTable();

            var exportDirectoryTable = new ExportDirectoryTable();

            exportDirectoryTable.ExportFlags = data.ReadUInt32();
            exportDirectoryTable.TimeDateStamp = data.ReadUInt32();
            exportDirectoryTable.MajorVersion = data.ReadUInt16();
            exportDirectoryTable.MinorVersion = data.ReadUInt16();
            exportDirectoryTable.NameRVA = data.ReadUInt32();
            exportDirectoryTable.OrdinalBase = data.ReadUInt32();
            exportDirectoryTable.AddressTableEntries = data.ReadUInt32();
            exportDirectoryTable.NumberOfNamePointers = data.ReadUInt32();
            exportDirectoryTable.ExportAddressTableRVA = data.ReadUInt32();
            exportDirectoryTable.NamePointerRVA = data.ReadUInt32();
            exportDirectoryTable.OrdinalTableRVA = data.ReadUInt32();

            exportTable.ExportDirectoryTable = exportDirectoryTable;

            // Name
            if (exportDirectoryTable.NameRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint nameAddress = exportDirectoryTable.NameRVA.ConvertVirtualAddress(sections);
                data.Seek(nameAddress, SeekOrigin.Begin);

                string name = data.ReadString(Encoding.ASCII);
                exportDirectoryTable.Name = name;
            }

            // Address table
            if (exportDirectoryTable.AddressTableEntries != 0 && exportDirectoryTable.ExportAddressTableRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint exportAddressTableAddress = exportDirectoryTable.ExportAddressTableRVA.ConvertVirtualAddress(sections);
                data.Seek(exportAddressTableAddress, SeekOrigin.Begin);

                var exportAddressTable = new ExportAddressTableEntry[exportDirectoryTable.AddressTableEntries];

                for (int i = 0; i < exportDirectoryTable.AddressTableEntries; i++)
                {
                    var addressTableEntry = new ExportAddressTableEntry();

                    // TODO: Use the optional header address and length to determine if export or forwarder
                    addressTableEntry.ExportRVA = data.ReadUInt32();
                    addressTableEntry.ForwarderRVA = addressTableEntry.ExportRVA;

                    exportAddressTable[i] = addressTableEntry;
                }

                exportTable.ExportAddressTable = exportAddressTable;
            }

            // Name pointer table
            if (exportDirectoryTable.NumberOfNamePointers != 0 && exportDirectoryTable.NamePointerRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint namePointerTableAddress = exportDirectoryTable.NamePointerRVA.ConvertVirtualAddress(sections);
                data.Seek(namePointerTableAddress, SeekOrigin.Begin);

                var namePointerTable = new ExportNamePointerTable();

                namePointerTable.Pointers = new uint[exportDirectoryTable.NumberOfNamePointers];
                for (int i = 0; i < exportDirectoryTable.NumberOfNamePointers; i++)
                {
                    uint pointer = data.ReadUInt32();
                    namePointerTable.Pointers[i] = pointer;
                }

                exportTable.NamePointerTable = namePointerTable;
            }

            // Ordinal table
            if (exportDirectoryTable.NumberOfNamePointers != 0 && exportDirectoryTable.OrdinalTableRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint ordinalTableAddress = exportDirectoryTable.OrdinalTableRVA.ConvertVirtualAddress(sections);
                data.Seek(ordinalTableAddress, SeekOrigin.Begin);

                var exportOrdinalTable = new ExportOrdinalTable();

                exportOrdinalTable.Indexes = new ushort[exportDirectoryTable.NumberOfNamePointers];
                for (int i = 0; i < exportDirectoryTable.NumberOfNamePointers; i++)
                {
                    ushort pointer = data.ReadUInt16();
                    exportOrdinalTable.Indexes[i] = pointer;
                }

                exportTable.OrdinalTable = exportOrdinalTable;
            }

            // Name table
            if (exportDirectoryTable.NumberOfNamePointers != 0 && exportDirectoryTable.NameRVA.ConvertVirtualAddress(sections) != 0)
            {
                uint nameTableAddress = exportDirectoryTable.NameRVA.ConvertVirtualAddress(sections);
                data.Seek(nameTableAddress, SeekOrigin.Begin);

                var exportNameTable = new ExportNameTable();

                exportNameTable.Strings = new string[exportDirectoryTable.NumberOfNamePointers];
                for (int i = 0; i < exportDirectoryTable.NumberOfNamePointers; i++)
                {
                    string str = data.ReadString(Encoding.ASCII);
                    exportNameTable.Strings[i] = str;
                }

                exportTable.ExportNameTable = exportNameTable;
            }

            return exportTable;
        }

        /// <summary>
        /// Parse a Stream into a import table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="magic">Optional header magic number indicating PE32 or PE32+</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <returns>Filled import table on success, null on error</returns>
        private static ImportTable ParseImportTable(Stream data, OptionalHeaderMagicNumber magic, SectionHeader[] sections)
        {
            // TODO: Use marshalling here instead of building
            var importTable = new ImportTable();

            // Import directory table
            var importDirectoryTable = new List<ImportDirectoryTableEntry>();

            // Loop until the last item (all nulls) are found
            while (true)
            {
                var importDirectoryTableEntry = new ImportDirectoryTableEntry();

                importDirectoryTableEntry.ImportLookupTableRVA = data.ReadUInt32();
                importDirectoryTableEntry.TimeDateStamp = data.ReadUInt32();
                importDirectoryTableEntry.ForwarderChain = data.ReadUInt32();
                importDirectoryTableEntry.NameRVA = data.ReadUInt32();
                importDirectoryTableEntry.ImportAddressTableRVA = data.ReadUInt32();

                importDirectoryTable.Add(importDirectoryTableEntry);

                // All zero values means the last entry
                if (importDirectoryTableEntry.ImportLookupTableRVA == 0
                    && importDirectoryTableEntry.TimeDateStamp == 0
                    && importDirectoryTableEntry.ForwarderChain == 0
                    && importDirectoryTableEntry.NameRVA == 0
                    && importDirectoryTableEntry.ImportAddressTableRVA == 0)
                    break;
            }

            importTable.ImportDirectoryTable = importDirectoryTable.ToArray();

            // Names
            for (int i = 0; i < importTable.ImportDirectoryTable.Length; i++)
            {
                var importDirectoryTableEntry = importTable.ImportDirectoryTable[i];
                if (importDirectoryTableEntry.NameRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                uint nameAddress = importDirectoryTableEntry.NameRVA.ConvertVirtualAddress(sections);
                data.Seek(nameAddress, SeekOrigin.Begin);

                string name = data.ReadString(Encoding.ASCII);
                importDirectoryTableEntry.Name = name;
            }

            // Lookup tables
            var importLookupTables = new Dictionary<int, ImportLookupTableEntry[]>();

            for (int i = 0; i < importTable.ImportDirectoryTable.Length; i++)
            {
                var importDirectoryTableEntry = importTable.ImportDirectoryTable[i];
                if (importDirectoryTableEntry.ImportLookupTableRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                uint tableAddress = importDirectoryTableEntry.ImportLookupTableRVA.ConvertVirtualAddress(sections);
                data.Seek(tableAddress, SeekOrigin.Begin);

                var entryLookupTable = new List<ImportLookupTableEntry>();

                while (true)
                {
                    var entryLookupTableEntry = new ImportLookupTableEntry();

                    if (magic == OptionalHeaderMagicNumber.PE32)
                    {
                        uint entryValue = data.ReadUInt32();
                        entryLookupTableEntry.OrdinalNameFlag = (entryValue & 0x80000000) != 0;
                        if (entryLookupTableEntry.OrdinalNameFlag)
                            entryLookupTableEntry.OrdinalNumber = (ushort)(entryValue & ~0x80000000);
                        else
                            entryLookupTableEntry.HintNameTableRVA = (uint)(entryValue & ~0x80000000);
                    }
                    else if (magic == OptionalHeaderMagicNumber.PE32Plus)
                    {
                        ulong entryValue = data.ReadUInt64();
                        entryLookupTableEntry.OrdinalNameFlag = (entryValue & 0x8000000000000000) != 0;
                        if (entryLookupTableEntry.OrdinalNameFlag)
                            entryLookupTableEntry.OrdinalNumber = (ushort)(entryValue & ~0x8000000000000000);
                        else
                            entryLookupTableEntry.HintNameTableRVA = (uint)(entryValue & ~0x8000000000000000);
                    }

                    entryLookupTable.Add(entryLookupTableEntry);

                    // All zero values means the last entry
                    if (entryLookupTableEntry.OrdinalNameFlag == false
                        && entryLookupTableEntry.OrdinalNumber == 0
                        && entryLookupTableEntry.HintNameTableRVA == 0)
                        break;
                }

                importLookupTables[i] = entryLookupTable.ToArray();
            }

            importTable.ImportLookupTables = importLookupTables;

            // Address tables
            var importAddressTables = new Dictionary<int, ImportAddressTableEntry[]>();

            for (int i = 0; i < importTable.ImportDirectoryTable.Length; i++)
            {
                var importDirectoryTableEntry = importTable.ImportDirectoryTable[i];
                if (importDirectoryTableEntry.ImportAddressTableRVA.ConvertVirtualAddress(sections) == 0)
                    continue;

                uint tableAddress = importDirectoryTableEntry.ImportAddressTableRVA.ConvertVirtualAddress(sections);
                data.Seek(tableAddress, SeekOrigin.Begin);

                var addressLookupTable = new List<ImportAddressTableEntry>();

                while (true)
                {
                    var addressLookupTableEntry = new ImportAddressTableEntry();

                    if (magic == OptionalHeaderMagicNumber.PE32)
                    {
                        uint entryValue = data.ReadUInt32();
                        addressLookupTableEntry.OrdinalNameFlag = (entryValue & 0x80000000) != 0;
                        if (addressLookupTableEntry.OrdinalNameFlag)
                            addressLookupTableEntry.OrdinalNumber = (ushort)(entryValue & ~0x80000000);
                        else
                            addressLookupTableEntry.HintNameTableRVA = (uint)(entryValue & ~0x80000000);
                    }
                    else if (magic == OptionalHeaderMagicNumber.PE32Plus)
                    {
                        ulong entryValue = data.ReadUInt64();
                        addressLookupTableEntry.OrdinalNameFlag = (entryValue & 0x8000000000000000) != 0;
                        if (addressLookupTableEntry.OrdinalNameFlag)
                            addressLookupTableEntry.OrdinalNumber = (ushort)(entryValue & ~0x8000000000000000);
                        else
                            addressLookupTableEntry.HintNameTableRVA = (uint)(entryValue & ~0x8000000000000000);
                    }

                    addressLookupTable.Add(addressLookupTableEntry);

                    // All zero values means the last entry
                    if (addressLookupTableEntry.OrdinalNameFlag == false
                        && addressLookupTableEntry.OrdinalNumber == 0
                        && addressLookupTableEntry.HintNameTableRVA == 0)
                        break;
                }

                importAddressTables[i] = addressLookupTable.ToArray();
            }

            importTable.ImportAddressTables = importAddressTables;

            // Hint/Name table
            var importHintNameTable = new List<HintNameTableEntry>();

            if ((importTable.ImportLookupTables != null && importTable.ImportLookupTables.Count > 0)
                || importTable.ImportAddressTables != null && importTable.ImportAddressTables.Count > 0)
            {
                // Get the addresses of the hint/name table entries
                List<int> hintNameTableEntryAddresses = new List<int>();

                // If we have import lookup tables
                if (importTable.ImportLookupTables != null && importLookupTables.Count > 0)
                {
                    var addresses = importTable.ImportLookupTables
                        .SelectMany(kvp => kvp.Value)
                        .Select(ilte => (int)ilte.HintNameTableRVA.ConvertVirtualAddress(sections));
                    hintNameTableEntryAddresses.AddRange(addresses);
                }

                // If we have import address tables
                if (importTable.ImportAddressTables != null && importTable.ImportAddressTables.Count > 0)
                {
                    var addresses = importTable.ImportAddressTables
                        .SelectMany(kvp => kvp.Value)
                        .Select(iate => (int)iate.HintNameTableRVA.ConvertVirtualAddress(sections));
                    hintNameTableEntryAddresses.AddRange(addresses);
                }

                // Sanitize the addresses
                hintNameTableEntryAddresses = hintNameTableEntryAddresses.Where(addr => addr != 0)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();

                // If we have any addresses, add them to the table
                if (hintNameTableEntryAddresses.Any())
                {
                    for (int i = 0; i < hintNameTableEntryAddresses.Count; i++)
                    {
                        int hintNameTableEntryAddress = hintNameTableEntryAddresses[i];
                        data.Seek(hintNameTableEntryAddress, SeekOrigin.Begin);

                        var hintNameTableEntry = new HintNameTableEntry();

                        hintNameTableEntry.Hint = data.ReadUInt16();
                        hintNameTableEntry.Name = data.ReadString(Encoding.ASCII);

                        importHintNameTable.Add(hintNameTableEntry);
                    }
                }
            }

            importTable.HintNameTable = importHintNameTable.ToArray();

            return importTable;
        }

        /// <summary>
        /// Parse a Stream into a resource directory table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="sections">Section table to use for virtual address translation</param>
        /// <param name="topLevel">Indicates if this is the top level or not</param>
        /// <returns>Filled resource directory table on success, null on error</returns>
        private static ResourceDirectoryTable ParseResourceDirectoryTable(Stream data, long initialOffset, SectionHeader[] sections, bool topLevel = false)
        {
            // TODO: Use marshalling here instead of building
            var resourceDirectoryTable = new ResourceDirectoryTable();

            resourceDirectoryTable.Characteristics = data.ReadUInt32();
            if (resourceDirectoryTable.Characteristics != 0)
                return null;

            resourceDirectoryTable.TimeDateStamp = data.ReadUInt32();
            resourceDirectoryTable.MajorVersion = data.ReadUInt16();
            resourceDirectoryTable.MinorVersion = data.ReadUInt16();
            resourceDirectoryTable.NumberOfNameEntries = data.ReadUInt16();
            resourceDirectoryTable.NumberOfIDEntries = data.ReadUInt16();

            // If we have no entries
            int totalEntryCount = resourceDirectoryTable.NumberOfNameEntries + resourceDirectoryTable.NumberOfIDEntries;
            if (totalEntryCount == 0)
                return resourceDirectoryTable;

            // Perform top-level pass of data
            resourceDirectoryTable.Entries = new ResourceDirectoryEntry[totalEntryCount];
            for (int i = 0; i < totalEntryCount; i++)
            {
                var entry = new ResourceDirectoryEntry();
                uint offset = data.ReadUInt32();
                if ((offset & 0x80000000) != 0)
                    entry.NameOffset = offset & ~0x80000000;
                else
                    entry.IntegerID = offset;

                offset = data.ReadUInt32();
                if ((offset & 0x80000000) != 0)
                    entry.SubdirectoryOffset = offset & ~0x80000000;
                else
                    entry.DataEntryOffset = offset;

                // Read the name from the offset, if needed
                if (entry.NameOffset > 0)
                {
                    long currentOffset = data.Position;
                    offset = entry.NameOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);

                    var resourceDirectoryString = new ResourceDirectoryString();

                    resourceDirectoryString.Length = data.ReadUInt16();
                    if (resourceDirectoryString.Length > 0)
                        resourceDirectoryString.UnicodeString = data.ReadBytes(resourceDirectoryString.Length * 2);

                    entry.Name = resourceDirectoryString;

                    data.Seek(currentOffset, SeekOrigin.Begin);
                }

                resourceDirectoryTable.Entries[i] = entry;
            }

            // Loop through and process the entries
            foreach (var entry in resourceDirectoryTable.Entries)
            {
                if (entry.DataEntryOffset > 0)
                {
                    uint offset = entry.DataEntryOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);

                    var resourceDataEntry = new ResourceDataEntry();
                    resourceDataEntry.DataRVA = data.ReadUInt32();
                    resourceDataEntry.Size = data.ReadUInt32();
                    resourceDataEntry.Codepage = data.ReadUInt32();
                    resourceDataEntry.Reserved = data.ReadUInt32();

                    // Read the data from the offset
                    offset = resourceDataEntry.DataRVA.ConvertVirtualAddress(sections);
                    if (offset > 0 && resourceDataEntry.Size > 0)
                    {
                        data.Seek(offset, SeekOrigin.Begin);
                        resourceDataEntry.Data = data.ReadBytes((int)resourceDataEntry.Size);
                    }

                    entry.DataEntry = resourceDataEntry;
                }
                else if (entry.SubdirectoryOffset > 0)
                {
                    uint offset = entry.SubdirectoryOffset + (uint)initialOffset;
                    data.Seek(offset, SeekOrigin.Begin);

                    entry.Subdirectory = ParseResourceDirectoryTable(data, initialOffset, sections);
                }
            }

            // If we are not at the top level
            if (!topLevel)
                return resourceDirectoryTable;

            // If we're not aligned to a section
            if (!sections.Any(s => s.PointerToRawData == initialOffset))
                return resourceDirectoryTable;

            // Get the section size
            int size = (int)sections.First(s => s.PointerToRawData == initialOffset).SizeOfRawData;

            // Align to the 512-byte boundary, we find the start of an MS-DOS header, or the end of the file
            while (data.Position - initialOffset < size && data.Position % 0x200 != 0 && data.Position < data.Length - 1)
            {
                // If we find the start of an MS-DOS header
                if (data.ReadUInt16() == SabreTools.Models.MSDOS.Constants.SignatureUInt16)
                {
                    data.Seek(-2, origin: SeekOrigin.Current);
                    break;
                }

                // Otherwise
                data.Seek(-1, origin: SeekOrigin.Current);
            }

            // If we have not used up the full size, parse the remaining chunk as a single resource
            if (data.Position - initialOffset < size)
            {
                Array.Resize(ref resourceDirectoryTable.Entries, totalEntryCount + 1);
                int length = (int)(size - (data.Position - initialOffset));

                resourceDirectoryTable.Entries[totalEntryCount] = new ResourceDirectoryEntry
                {
                    Name = new ResourceDirectoryString { UnicodeString = Encoding.ASCII.GetBytes("HIDDEN RESOURCE") },
                    IntegerID = uint.MaxValue,
                    DataEntryOffset = (uint)data.Position,
                    DataEntry = new ResourceDataEntry
                    {
                        Size = (uint)length,
                        Data = data.ReadBytes(length),
                        Codepage = (uint)Encoding.Unicode.CodePage,
                    },
                };
            }

            return resourceDirectoryTable;
        }

        #endregion
    }
}