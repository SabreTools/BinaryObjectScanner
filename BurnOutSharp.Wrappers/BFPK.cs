using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;
using static BurnOutSharp.Builder.Extensions;

namespace BurnOutSharp.Wrappers
{
    public class BFPK : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.BFPK.Header.Magic"/>
        public uint Magic => _archive.Header.Magic;

        /// <inheritdoc cref="Models.BFPK.Header.Version"/>
        public int Version => _archive.Header.Version;

        /// <inheritdoc cref="Models.BFPK.Header.Files"/>
        public int Files => _archive.Header.Files;

        #endregion

        #region Files

        /// <inheritdoc cref="Models.BFPK.Archive.Files"/>
        public Models.BFPK.FileEntry[] FileTable => _archive.Files;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the archive
        /// </summary>
        private Models.BFPK.Archive _archive;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private BFPK() { }

        /// <summary>
        /// Create a BFPK archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
        public static BFPK Create(byte[] data, int offset)
        {
            var archive = Builder.BFPK.ParseArchive(data, offset);
            if (archive == null)
                return null;

            var wrapper = new BFPK
            {
                _archive = archive,
                _dataSource = DataSource.ByteArray,
                _byteArrayData = data,
                _byteArrayOffset = offset,
            };
            return wrapper;
        }

        /// <summary>
        /// Create a BFPK archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
        public static BFPK Create(Stream data)
        {
            var archive = Builder.BFPK.ParseArchive(data);
            if (archive == null)
                return null;

            var wrapper = new BFPK
            {
                _archive = archive,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Data

        /// <summary>
        /// Extract a single file based on index
        /// </summary>
        /// <param name="index">Index of the file to extract</param>
        /// <param name="outputDirectory">Directory to write the file to</param>
        /// <returns>True if the extraction succeeded, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (Files == 0 || FileTable == null || FileTable.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= FileTable.Length)
                return false;

            // Get the file information
            var file = FileTable[index];

            // Get the read index and length
            int offset = file.Offset + 4;
            int compressedSize = file.CompressedSize;

            // Some files can lack the length prefix
            if (compressedSize > GetEndOfFile())
            {
                offset -= 4;
                compressedSize = file.UncompressedSize;
            }

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.Name);
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    // Read the data block
                    byte[] data = ReadFromDataSource(offset, compressedSize);

                    // If we have uncompressed data
                    if (compressedSize == file.UncompressedSize)
                    {
                        fs.Write(data, 0, compressedSize);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(data);
                        ZlibStream zs = new ZlibStream(ms, CompressionMode.Decompress);
                        zs.CopyTo(fs);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("New Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            // Stub
            PrintStubHeader();
            PrintStubExtendedHeader();

            // Header
            PrintHeader();

            // Tables
            PrintSegmentTable();
            PrintResourceTable();
            PrintResidentNameTable();
            PrintModuleReferenceTable();
            PrintImportedNameTable();
            PrintEntryTable();
            PrintNonresidentNameTable();
        }

        /// <summary>
        /// Print stub header information
        /// </summary>
        private void PrintStubHeader()
        {
            Console.WriteLine("  MS-DOS Stub Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {BitConverter.ToString(Stub_Magic).Replace("-", string.Empty)}");
            Console.WriteLine($"  Last page bytes: {Stub_LastPageBytes}");
            Console.WriteLine($"  Pages: {Stub_Pages}");
            Console.WriteLine($"  Relocation items: {Stub_RelocationItems}");
            Console.WriteLine($"  Header paragraph size: {Stub_HeaderParagraphSize}");
            Console.WriteLine($"  Minimum extra paragraphs: {Stub_MinimumExtraParagraphs}");
            Console.WriteLine($"  Maximum extra paragraphs: {Stub_MaximumExtraParagraphs}");
            Console.WriteLine($"  Initial SS value: {Stub_InitialSSValue}");
            Console.WriteLine($"  Initial SP value: {Stub_InitialSPValue}");
            Console.WriteLine($"  Checksum: {Stub_Checksum}");
            Console.WriteLine($"  Initial IP value: {Stub_InitialIPValue}");
            Console.WriteLine($"  Initial CS value: {Stub_InitialCSValue}");
            Console.WriteLine($"  Relocation table address: {Stub_RelocationTableAddr}");
            Console.WriteLine($"  Overlay number: {Stub_OverlayNumber}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print stub extended header information
        /// </summary>
        private void PrintStubExtendedHeader()
        {
            Console.WriteLine("  MS-DOS Stub Extended Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Reserved words: {string.Join(", ", Stub_Reserved1)}");
            Console.WriteLine($"  OEM identifier: {Stub_OEMIdentifier}");
            Console.WriteLine($"  OEM information: {Stub_OEMInformation}");
            Console.WriteLine($"  Reserved words: {string.Join(", ", Stub_Reserved2)}");
            Console.WriteLine($"  New EXE header address: {Stub_NewExeHeaderAddr}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {BitConverter.ToString(Magic).Replace("-", string.Empty)}");
            Console.WriteLine($"  Linker version: {LinkerVersion}");
            Console.WriteLine($"  Linker revision: {LinkerRevision}");
            Console.WriteLine($"  Entry table offset: {EntryTableOffset}");
            Console.WriteLine($"  Entry table size: {EntryTableSize}");
            Console.WriteLine($"  CRC checksum: {CrcChecksum}");
            Console.WriteLine($"  Flag word: {FlagWord}");
            Console.WriteLine($"  Automatic data segment number: {AutomaticDataSegmentNumber}");
            Console.WriteLine($"  Initial heap allocation: {InitialHeapAlloc}");
            Console.WriteLine($"  Initial stack allocation: {InitialStackAlloc}");
            Console.WriteLine($"  Initial CS:IP setting: {InitialCSIPSetting}");
            Console.WriteLine($"  Initial SS:SP setting: {InitialSSSPSetting}");
            Console.WriteLine($"  File segment count: {FileSegmentCount}");
            Console.WriteLine($"  Module reference table size: {ModuleReferenceTableSize}");
            Console.WriteLine($"  Non-resident name table size: {NonResidentNameTableSize}");
            Console.WriteLine($"  Segment table offset: {SegmentTableOffset}");
            Console.WriteLine($"  Resource table offset: {ResourceTableOffset}");
            Console.WriteLine($"  Resident name table offset: {ResidentNameTableOffset}");
            Console.WriteLine($"  Module reference table offset: {ModuleReferenceTableOffset}");
            Console.WriteLine($"  Imported names table offset: {ImportedNamesTableOffset}");
            Console.WriteLine($"  Non-resident name table offset: {NonResidentNamesTableOffset}");
            Console.WriteLine($"  Moveable entries count: {MovableEntriesCount}");
            Console.WriteLine($"  Segment alignment shift count: {SegmentAlignmentShiftCount}");
            Console.WriteLine($"  Resource entries count: {ResourceEntriesCount}");
            Console.WriteLine($"  Target operating system: {TargetOperatingSystem}");
            Console.WriteLine($"  Additional flags: {AdditionalFlags}");
            Console.WriteLine($"  Return thunk offset: {ReturnThunkOffset}");
            Console.WriteLine($"  Segment reference thunk offset: {SegmentReferenceThunkOffset}");
            Console.WriteLine($"  Minimum code swap area size: {MinCodeSwapAreaSize}");
            Console.WriteLine($"  Windows SDK revision: {WindowsSDKRevision}");
            Console.WriteLine($"  Windows SDK version: {WindowsSDKVersion}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print segment table information
        /// </summary>
        private void PrintSegmentTable()
        {
            Console.WriteLine("  Segment Table Information:");
            Console.WriteLine("  -------------------------");
            if (FileSegmentCount == 0 || SegmentTable.Length == 0)
            {
                Console.WriteLine("  No segment table items");
            }
            else
            {
                for (int i = 0; i < SegmentTable.Length; i++)
                {
                    var entry = SegmentTable[i];
                    Console.WriteLine($"  Segment Table Entry {i}");
                    Console.WriteLine($"    Offset = {entry.Offset}");
                    Console.WriteLine($"    Length = {entry.Length}");
                    Console.WriteLine($"    Flag word = {entry.FlagWord}");
                    Console.WriteLine($"    Minimum allocation size = {entry.MinimumAllocationSize}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print resource table information
        /// </summary>
        private void PrintResourceTable()
        {
            Console.WriteLine("  Resource Table Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Alignment shift count: {ResourceTable.AlignmentShiftCount}");
            if (ResourceEntriesCount == 0 || ResourceTable.ResourceTypes.Length == 0)
            {
                Console.WriteLine("  No resource table items");
            }
            else
            {
                for (int i = 0; i < ResourceTable.ResourceTypes.Length; i++)
                {
                    // TODO: If not integer type, print out name
                    var entry = ResourceTable.ResourceTypes[i];
                    Console.WriteLine($"  Resource Table Entry {i}");
                    Console.WriteLine($"    Type ID = {entry.TypeID} (Is Integer Type: {entry.IsIntegerType()})");
                    Console.WriteLine($"    Resource count = {entry.ResourceCount}");
                    Console.WriteLine($"    Reserved = {entry.Reserved}");
                    Console.WriteLine($"    Resources = ");
                    if (entry.ResourceCount == 0 || entry.Resources.Length == 0)
                    {
                        Console.WriteLine("      No resource items");
                    }
                    else
                    {
                        for (int j = 0; j < entry.Resources.Length; j++)
                        {
                            // TODO: If not integer type, print out name
                            var resource = entry.Resources[j];
                            Console.WriteLine($"      Resource Entry {i}");
                            Console.WriteLine($"        Offset = {resource.Offset}");
                            Console.WriteLine($"        Length = {resource.Length}");
                            Console.WriteLine($"        Flag word = {resource.FlagWord}");
                            Console.WriteLine($"        Resource ID = {resource.ResourceID} (Is Integer Type: {resource.IsIntegerType()})");
                            Console.WriteLine($"        Reserved = {resource.Reserved}");
                        }
                    }
                }
            }

            if (ResourceTable.TypeAndNameStrings.Count == 0)
            {
                Console.WriteLine("  No resource table type/name strings");
            }
            else
            {
                foreach (var typeAndNameString in ResourceTable.TypeAndNameStrings)
                {
                    Console.WriteLine($"  Resource Type/Name Offset {typeAndNameString.Key}");
                    Console.WriteLine($"    Length = {typeAndNameString.Value.Length}");
                    Console.WriteLine($"    Text = {Encoding.ASCII.GetString(typeAndNameString.Value.Text)}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print resident-name table information
        /// </summary>
        private void PrintResidentNameTable()
        {
            Console.WriteLine("  Resident-Name Table Information:");
            Console.WriteLine("  -------------------------");
            if (ResidentNameTableOffset == 0 || ResidentNameTable.Length == 0)
            {
                Console.WriteLine("  No resident-name table items");
            }
            else
            {
                for (int i = 0; i < ResidentNameTable.Length; i++)
                {
                    var entry = ResidentNameTable[i];
                    Console.WriteLine($"  Resident-Name Table Entry {i}");
                    Console.WriteLine($"    Length = {entry.Length}");
                    Console.WriteLine($"    Name string = {(entry.NameString != null ? Encoding.ASCII.GetString(entry.NameString) : "[EMPTY]")}");
                    Console.WriteLine($"    Ordinal number = {entry.OrdinalNumber}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print module-reference table information
        /// </summary>
        private void PrintModuleReferenceTable()
        {
            Console.WriteLine("  Module-Reference Table Information:");
            Console.WriteLine("  -------------------------");
            if (ModuleReferenceTableSize == 0 || ModuleReferenceTable.Length == 0)
            {
                Console.WriteLine("  No module-reference table items");
            }
            else
            {
                for (int i = 0; i < ModuleReferenceTable.Length; i++)
                {
                    // TODO: Read the imported names table and print value here
                    var entry = ModuleReferenceTable[i];
                    Console.WriteLine($"  Module-Reference Table Entry {i}");
                    Console.WriteLine($"    Offset = {entry.Offset} (adjusted to be {entry.Offset + Stub_NewExeHeaderAddr + ImportedNamesTableOffset})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print imported-name table information
        /// </summary>
        private void PrintImportedNameTable()
        {
            Console.WriteLine("  Imported-Name Table Information:");
            Console.WriteLine("  -------------------------");
            if (ImportedNamesTableOffset == 0 || ImportedNameTable.Count == 0)
            {
                Console.WriteLine("  No imported-name table items");
            }
            else
            {
                foreach (var entry in ImportedNameTable)
                {
                    Console.WriteLine($"  Imported-Name Table at Offset {entry.Key}");
                    Console.WriteLine($"    Length = {entry.Value.Length}");
                    Console.WriteLine($"    Name string = {(entry.Value.NameString != null ? Encoding.ASCII.GetString(entry.Value.NameString) : "[EMPTY]")}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print entry table information
        /// </summary>
        private void PrintEntryTable()
        {
            Console.WriteLine("  Entry Table Information:");
            Console.WriteLine("  -------------------------");
            if (EntryTableSize == 0 || EntryTable.Length == 0)
            {
                Console.WriteLine("  No entry table items");
            }
            else
            {
                for (int i = 0; i < EntryTable.Length; i++)
                {
                    var entry = EntryTable[i];
                    Console.WriteLine($"  Entry Table Entry {i}");
                    Console.WriteLine($"    Entry count = {entry.EntryCount}");
                    Console.WriteLine($"    Segment indicator = {entry.SegmentIndicator} ({entry.GetEntryType()})");
                    switch (entry.GetEntryType())
                    {
                        case BurnOutSharp.Models.NewExecutable.SegmentEntryType.FixedSegment:
                            Console.WriteLine($"    Flag word = {entry.FixedFlagWord}");
                            Console.WriteLine($"    Offset = {entry.FixedOffset}");
                            break;
                        case BurnOutSharp.Models.NewExecutable.SegmentEntryType.MoveableSegment:
                            Console.WriteLine($"    Flag word = {entry.MoveableFlagWord}");
                            Console.WriteLine($"    Reserved = {entry.MoveableReserved}");
                            Console.WriteLine($"    Segment number = {entry.MoveableSegmentNumber}");
                            Console.WriteLine($"    Offset = {entry.MoveableOffset}");
                            break;
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print nonresident-name table information
        /// </summary>
        private void PrintNonresidentNameTable()
        {
            Console.WriteLine("  Nonresident-Name Table Information:");
            Console.WriteLine("  -------------------------");
            if (NonResidentNameTableSize == 0 || NonResidentNameTable.Length == 0)
            {
                Console.WriteLine("  No nonresident-name table items");
            }
            else
            {
                for (int i = 0; i < NonResidentNameTable.Length; i++)
                {
                    var entry = NonResidentNameTable[i];
                    Console.WriteLine($"  Nonresident-Name Table Entry {i}");
                    Console.WriteLine($"    Length = {entry.Length}");
                    Console.WriteLine($"    Name string = {(entry.NameString != null ? Encoding.ASCII.GetString(entry.NameString) : "[EMPTY]")}");
                    Console.WriteLine($"    Ordinal number = {entry.OrdinalNumber}");
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}