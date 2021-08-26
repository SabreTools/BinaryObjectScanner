using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.ExecutableType.Microsoft.Sections;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// The PE file header consists of a Microsoft MS-DOS stub, the PE signature, the COFF file header, and an optional header.
    /// A COFF object file header consists of a COFF file header and an optional header.
    /// In both cases, the file headers are followed immediately by section headers.
    /// </summary>
    internal class PEExecutable
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
        public IMAGE_DOS_HEADER MSDOSStub;

        /// <summary>
        /// At the beginning of an object file, or immediately after the signature of an image file, is a standard COFF file header in the following format.
        /// Note that the Windows loader limits the number of sections to 96.
        /// </summary>
        public IMAGE_FILE_HEADER COFFFileHeader;

        /// <summary>
        /// Every image file has an optional header that provides information to the loader.
        /// This header is optional in the sense that some files (specifically, object files) do not have it.
        /// For image files, this header is required.
        /// An object file can have an optional header, but generally this header has no function in an object file except to increase its size.
        /// </summary>
        public IMAGE_OPTIONAL_HEADER OptionalHeader;

        /// <summary>
        /// Each row of the section table is, in effect, a section header.
        /// This table immediately follows the optional header, if any.
        /// This positioning is required because the file header does not contain a direct pointer to the section table.
        /// Instead, the location of the section table is determined by calculating the location of the first byte after the headers.
        /// Make sure to use the size of the optional header as specified in the file header.
        /// </summary>
        public IMAGE_SECTION_HEADER[] SectionHeaders;

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

        #endregion

        // TODO: Add more and more parts of a standard PE executable, not just the header
        // TODO: Add data directory table information here instead of in IMAGE_OPTIONAL_HEADER

        public static PEExecutable Deserialize(Stream stream)
        {
            PEExecutable pex = new PEExecutable();

            try
            {
                pex.MSDOSStub = IMAGE_DOS_HEADER.Deserialize(stream); stream.Seek(pex.MSDOSStub.NewExeHeaderAddr, SeekOrigin.Begin);
                pex.COFFFileHeader = IMAGE_FILE_HEADER.Deserialize(stream);
                if (pex.COFFFileHeader.SizeOfOptionalHeader > 0)
                    pex.OptionalHeader = IMAGE_OPTIONAL_HEADER.Deserialize(stream);
                
                pex.SectionHeaders = new IMAGE_SECTION_HEADER[pex.COFFFileHeader.NumberOfSections];
                for (int i = 0; i < pex.COFFFileHeader.NumberOfSections; i++)
                {
                    pex.SectionHeaders[i] = IMAGE_SECTION_HEADER.Deserialize(stream);
                }

                // TODO: Uncomment these when RVA conversion works
                // // Export Table
                // var table = pex.SectionHeaders[(byte)ImageDirectory.IMAGE_DIRECTORY_ENTRY_EXPORT];
                // if (table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)EVORE.ConvertVirtualAddress(table.VirtualAddress, pex.SectionHeaders);
                //     stream.Seek(tableAddress, SeekOrigin.Begin);
                //     pex.ExportTable = ExportDataSection.Deserialize(stream);
                // }

                // // Import Table
                // table = pex.SectionHeaders[(byte)ImageDirectory.IMAGE_DIRECTORY_ENTRY_IMPORT];
                // if (table.VirtualSize > 0)
                // {
                //     int tableAddress = (int)EVORE.ConvertVirtualAddress(table.VirtualAddress, pex.SectionHeaders);
                //     stream.Seek(tableAddress, SeekOrigin.Begin);
                //     pex.ImportTable = ImportDataSection.Deserialize(stream, pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus, hintCount: 0); // TODO: Figure out where this count comes from
                // }
            }
            catch
            {
                return null;
            }

            return pex;
        }

        public static PEExecutable Deserialize(byte[] content, int offset)
        {
            PEExecutable pex = new PEExecutable();

            try
            {
                unsafe
                {
                    pex.MSDOSStub = IMAGE_DOS_HEADER.Deserialize(content, offset); offset = pex.MSDOSStub.NewExeHeaderAddr;
                    pex.COFFFileHeader = IMAGE_FILE_HEADER.Deserialize(content, offset); offset += Marshal.SizeOf(pex.COFFFileHeader);
                    if (pex.COFFFileHeader.SizeOfOptionalHeader > 0)
                    {
                        pex.OptionalHeader = IMAGE_OPTIONAL_HEADER.Deserialize(content, offset); offset += pex.COFFFileHeader.SizeOfOptionalHeader;
                    }

                    pex.SectionHeaders = new IMAGE_SECTION_HEADER[pex.COFFFileHeader.NumberOfSections];
                    for (int i = 0; i < pex.COFFFileHeader.NumberOfSections; i++)
                    {
                        pex.SectionHeaders[i] = IMAGE_SECTION_HEADER.Deserialize(content, offset); offset += 40;
                    }

                    // TODO: Uncomment these when RVA conversion works
                    // // Export Table
                    // var table = pex.SectionHeaders[(byte)ImageDirectory.IMAGE_DIRECTORY_ENTRY_EXPORT];
                    // if (table.VirtualSize > 0)
                    // {
                    //     int tableAddress = (int)EVORE.ConvertVirtualAddress(table.VirtualAddress, pex.SectionHeaders);
                    //     pex.ExportTable = ExportDataSection.Deserialize(content, tableAddress);
                    // }

                    // // Import Table
                    // table = pex.SectionHeaders[(byte)ImageDirectory.IMAGE_DIRECTORY_ENTRY_IMPORT];
                    // if (table.VirtualSize > 0)
                    // {
                    //     int tableAddress = (int)EVORE.ConvertVirtualAddress(table.VirtualAddress, pex.SectionHeaders);
                    //     pex.ImportTable = ImportDataSection.Deserialize(content, tableAddress, pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus, hintCount: 0); offset += Marshal.SizeOf(pex.ImportTable); // TODO: Figure out where this count comes from
                    // }
                }
            }
            catch
            {
                return null;
            }

            return pex;
        }
    }
}