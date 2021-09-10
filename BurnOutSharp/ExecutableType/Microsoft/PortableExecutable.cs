using System;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft.Headers;
using BurnOutSharp.ExecutableType.Microsoft.Sections;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// The PE file header consists of a Microsoft MS-DOS stub, the PE signature, the COFF file header, and an optional header.
    /// A COFF object file header consists of a COFF file header and an optional header.
    /// In both cases, the file headers are followed immediately by section headers.
    /// </summary>
    public class PortableExecutable
    {
        #region Headers

        /// <summary>
        /// The MS-DOS stub is a valid application that runs under MS-DOS.
        /// It is placed at the front of the EXE image.
        /// The linker places a default stub here, which prints out the message "This program cannot be run in DOS mode" when the image is run in MS-DOS.
        /// The user can specify a different stub by using the /STUB linker option.
        /// At location 0x3c, the stub has the file offset to the PE signature.
        /// This information enables Windows to properly execute the image file, even though it has an MS-DOS stub.
        /// This file offset is placed at location 0x3c during linking.
        // </summary>
        public MSDOSExecutableHeader DOSStubHeader;

        /// <summary>
        /// At the beginning of an object file, or immediately after the signature of an image file, is a standard COFF file header in the following format.
        /// Note that the Windows loader limits the number of sections to 96.
        /// </summary>
        public CommonObjectFileFormatHeader ImageFileHeader;

        /// <summary>
        /// Every image file has an optional header that provides information to the loader.
        /// This header is optional in the sense that some files (specifically, object files) do not have it.
        /// For image files, this header is required.
        /// An object file can have an optional header, but generally this header has no function in an object file except to increase its size.
        /// </summary>
        public OptionalHeader OptionalHeader;

        /// <summary>
        /// Each row of the section table is, in effect, a section header.
        /// This table immediately follows the optional header, if any.
        /// This positioning is required because the file header does not contain a direct pointer to the section table.
        /// Instead, the location of the section table is determined by calculating the location of the first byte after the headers.
        /// Make sure to use the size of the optional header as specified in the file header.
        /// </summary>
        public SectionHeader[] SectionTable;

        #endregion

        #region Tables

        /// <summary>
        /// The export data section, named .edata, contains information about symbols that other images can access through dynamic linking.
        /// Exported symbols are generally found in DLLs, but DLLs can also import symbols.
        // </summary>
        public ExportDataSection ExportTable;

        /// <summary>
        /// All image files that import symbols, including virtually all executable (EXE) files, have an .idata section.
        // </summary>
        public ImportDataSection ImportTable;

        /// <summary>
        /// Resources are indexed by a multiple-level binary-sorted tree structure.
        // The general design can incorporate 2**31 levels.
        // By convention, however, Windows uses three levels
        // </summary>
        public ResourceSection ResourceSection;

        #endregion

        // TODO: Add more and more parts of a standard PE executable, not just the header
        // TODO: Add data directory table information here instead of in IMAGE_OPTIONAL_HEADER

        /// <summary>
        /// Determine if a section is contained within the section table
        /// </summary>
        /// <param name="sectionName">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>True if the section is in the executable, false otherwise</returns>
        public bool ContainsSection(string sectionName, bool exact = false)
        {
            // Get all section names first
            string[] sectionNames = GetSectionNames();
            if (sectionNames == null)
                return false;
            
            // If we're checking exactly, return only exact matches (with nulls trimmed)
            if (exact)
                return sectionNames.Any(n => n.Trim('\0').Equals(sectionName));
            
            // Otherwise, check if section name starts with the value
            else
                return sectionNames.Any(n => n.Trim('\0').StartsWith(sectionName));
        }

        /// <summary>
        /// Get the first section based on name, if possible
        /// </summary>
        /// <param name="sectionName">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public SectionHeader GetFirstSection(string sectionName, bool exact = false)
        {
            // If we have no sections, we can't do anything
            if (SectionTable == null || !SectionTable.Any())
                return null;
            
            // If we're checking exactly, return only exact matches (with nulls trimmed)
            if (exact)
                return SectionTable.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).Trim('\0').Equals(sectionName));
            
            // Otherwise, check if section name starts with the value
            else
                return SectionTable.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).Trim('\0').StartsWith(sectionName));
        }

        /// <summary>
        /// Get the last section based on name, if possible
        /// </summary>
        /// <param name="sectionName">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public SectionHeader GetLastSection(string sectionName, bool exact = false)
        {
            // If we have no sections, we can't do anything
            if (SectionTable == null || !SectionTable.Any())
                return null;
            
            // If we're checking exactly, return only exact matches (with nulls trimmed)
            if (exact)
                return SectionTable.LastOrDefault(s => Encoding.ASCII.GetString(s.Name).Trim('\0').Equals(sectionName));
            
            // Otherwise, check if section name starts with the value
            else
                return SectionTable.LastOrDefault(s => Encoding.ASCII.GetString(s.Name).Trim('\0').StartsWith(sectionName));
        }

        /// <summary>
        /// Get the list of section names
        /// </summary>
        public string[] GetSectionNames()
        {
            // Invalid table means no names are accessible
            if (SectionTable == null || SectionTable.Length == 0)
                return null;
            
            return SectionTable.Select(s => Encoding.ASCII.GetString(s.Name)).ToArray();
        }

        public static PortableExecutable Deserialize(Stream stream)
        {
            PortableExecutable pex = new PortableExecutable();

            try
            {
                // Attempt to read the DOS header first
                pex.DOSStubHeader = MSDOSExecutableHeader.Deserialize(stream); stream.Seek(pex.DOSStubHeader.NewExeHeaderAddr, SeekOrigin.Begin);
                if (pex.DOSStubHeader.Magic != Constants.IMAGE_DOS_SIGNATURE)
                    return null;
                
                // If the new header address is invalid for the file, it's not a PE
                if (pex.DOSStubHeader.NewExeHeaderAddr >= stream.Length)
                    return null;

                // Then attempt to read the PE header
                pex.ImageFileHeader = CommonObjectFileFormatHeader.Deserialize(stream);
                if (pex.ImageFileHeader.Signature != Constants.IMAGE_NT_SIGNATURE)
                    return null;

                // If the optional header is supposed to exist, read that as well
                if (pex.ImageFileHeader.SizeOfOptionalHeader > 0)
                    pex.OptionalHeader = OptionalHeader.Deserialize(stream);
                
                // Then read in the section table
                pex.SectionTable = new SectionHeader[pex.ImageFileHeader.NumberOfSections];
                for (int i = 0; i < pex.ImageFileHeader.NumberOfSections; i++)
                {
                    pex.SectionTable[i] = SectionHeader.Deserialize(stream);
                }

                // TODO: Uncomment these as the directories are understod and implemented
                // // Export Table
                // var table = pex.SectionTable[(byte)ImageDirectory.IMAGE_DIRECTORY_ENTRY_EXPORT];
                // if (table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)EVORE.ConvertVirtualAddress(table.VirtualAddress, pex.SectionTable);
                //     stream.Seek(tableAddress, SeekOrigin.Begin);
                //     pex.ExportTable = ExportDataSection.Deserialize(stream);
                // }

                // // Import Table
                // table = pex.SectionTable[(byte)ImageDirectory.IMAGE_DIRECTORY_ENTRY_IMPORT];
                // if (table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)EVORE.ConvertVirtualAddress(table.VirtualAddress, pex.SectionTable);
                //     stream.Seek(tableAddress, SeekOrigin.Begin);
                //     pex.ImportTable = ImportDataSection.Deserialize(stream, pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus, hintCount: 0); // TODO: Figure out where this count comes from
                // }

                // // Resource Table
                // var table = pex.SectionTable[(byte)ImageDirectory.IMAGE_DIRECTORY_ENTRY_RESOURCE];
                // if (table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)EVORE.ConvertVirtualAddress(table.VirtualAddress, pex.SectionTable);
                //     stream.Seek(tableAddress, SeekOrigin.Begin);
                //     pex.ResourceSection = ResourceSection.Deserialize(stream, pex.SectionTable);
                // }

                // // Export Table
                // var table = pex.GetSection(".edata", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)ConvertVirtualAddress(table.VirtualAddress, pex.SectionTable);
                //     stream.Seek(tableAddress, SeekOrigin.Begin);
                //     pex.ExportTable = ExportDataSection.Deserialize(stream);
                // }

                // // Import Table
                // table = pex.GetSection(".idata", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)ConvertVirtualAddress(table.VirtualAddress, pex.SectionTable);
                //     stream.Seek(tableAddress, SeekOrigin.Begin);
                //     pex.ImportTable = ImportDataSection.Deserialize(stream, pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus, hintCount: 0);
                // }

                // Resource Table
                var table = pex.GetLastSection(".rsrc", true);
                if (table != null && table.VirtualSize > 0)
                {
                    int tableAddress = (int)table.PointerToRawData;
                    pex.ResourceSection = ResourceSection.Deserialize(stream, pex.SectionTable);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Errored out on a file: {ex}");
                return null;
            }

            return pex;
        }

        public static PortableExecutable Deserialize(byte[] content, int offset)
        {
            PortableExecutable pex = new PortableExecutable();

            try
            {
                // Attempt to read the DOS header first
                pex.DOSStubHeader = MSDOSExecutableHeader.Deserialize(content, ref offset);
                offset = pex.DOSStubHeader.NewExeHeaderAddr;
                if (pex.DOSStubHeader.Magic != Constants.IMAGE_DOS_SIGNATURE)
                    return null;

                // If the new header address is invalid for the file, it's not a PE
                if (pex.DOSStubHeader.NewExeHeaderAddr >= content.Length)
                    return null;

                // Then attempt to read the PE header
                pex.ImageFileHeader = CommonObjectFileFormatHeader.Deserialize(content, ref offset);
                if (pex.ImageFileHeader.Signature != Constants.IMAGE_NT_SIGNATURE)
                    return null;

                // If the optional header is supposed to exist, read that as well
                if (pex.ImageFileHeader.SizeOfOptionalHeader > 0)
                    pex.OptionalHeader = OptionalHeader.Deserialize(content, ref offset);

                // Then read in the section table
                pex.SectionTable = new SectionHeader[pex.ImageFileHeader.NumberOfSections];
                for (int i = 0; i < pex.ImageFileHeader.NumberOfSections; i++)
                {
                    pex.SectionTable[i] = SectionHeader.Deserialize(content, ref offset);
                }

                // // Export Table
                // var table = pex.GetSection(".edata", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)ConvertVirtualAddress(table.VirtualAddress, pex.SectionTable);
                //     pex.ExportTable = ExportDataSection.Deserialize(content, tableAddress);
                // }

                // // Import Table
                // table = pex.GetSection(".idata", true);
                // if (table != null && table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)ConvertVirtualAddress(table.VirtualAddress, pex.SectionTable);
                //     pex.ImportTable = ImportDataSection.Deserialize(content, tableAddress, pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus, hintCount: 0);
                // }

                // Resource Table
                var table = pex.GetLastSection(".rsrc", true);
                if (table != null && table.VirtualSize > 0)
                {
                    int tableAddress = (int)table.PointerToRawData;
                    pex.ResourceSection = ResourceSection.Deserialize(content, ref tableAddress, pex.SectionTable);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Errored out on a file: {ex}");
                return null;
            }

            return pex;
        }

        /// <summary>
        /// Convert a virtual address to a physical one
        /// </summary>
        /// <param name="virtualAddress">Virtual address to convert</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <returns>Physical address, 0 on error</returns>
        public static uint ConvertVirtualAddress(uint virtualAddress, SectionHeader[] sections)
        {
            // Loop through all of the sections
            for (int i = 0; i < sections.Length; i++)
            {
                // If the section is invalid, just skip it
                if (sections[i] == null)
                    continue;

                // If the section "starts" at 0, just skip it
                if (sections[i].PointerToRawData == 0)
                    continue;

                // Attempt to derive the physical address from the current section
                var section = sections[i];
                if (virtualAddress >= section.VirtualAddress && virtualAddress <= section.VirtualAddress + section.VirtualSize)
                   return section.PointerToRawData + virtualAddress - section.VirtualAddress;
            }

            return 0;
        }
    }
}