//this file is part of BurnOut
//Copyright (C)2005-2010 Gernot Knippen
//Ported code with augments Copyright (C)2018 Matt Nadareski
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either
//version 2 of the License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU General Public License for more details.
//
//You can get a copy of the GNU General Public License
//by writing to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.ExecutableType.Microsoft.Sections;
using BurnOutSharp.ExecutableType.Microsoft.Tables;

namespace BurnOutSharp.Tools
{
    internal static class EVORE
    {
        /// <summary>
        /// Convert a virtual address to a physical one
        /// </summary>
        /// <param name="virtualAddress">Virtual address to convert</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <returns>Physical address, 0 on error</returns>
        internal static uint ConvertVirtualAddress(uint virtualAddress, IMAGE_SECTION_HEADER[] sections)
        {
            // Loop through all of the sections
            for (int i = 0; i < sections.Length; i++)
            {
                // If the section is invalid, just skip it
                if (sections[i] == null)
                    continue;

                // Attempt to derive the physical address from the current section
                var section = sections[i];
                if (virtualAddress >= section.VirtualAddress && virtualAddress <= section.VirtualAddress + section.VirtualSize)
                    return section.PointerToRawData + virtualAddress - section.VirtualAddress;
            }

            return 0;
        }

        /// <summary>
        /// Checks if the file contents represent a PE executable
        /// </summary>
        /// <param name="fileContent">File contents to check</param>
        /// <returns>True if the file is an EXE, false otherwise</returns>
        internal static bool IsPEExecutable(byte[] fileContent)
        {
            if (fileContent == null)
                return false;

            try
            {
                PEExecutable pex = PEExecutable.Deserialize(fileContent, 0);
                return pex.COFFFileHeader.Characteristics.HasFlag(ImageObjectCharacteristics.IMAGE_FILE_DLL);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Writes the file contents to a temporary file, if possible
        /// </summary>
        /// <param name="fileContent">File contents to write</param>
        /// <param name="extension">Optional extension for the temporary file, defaults to ".exe"</param>
        /// <returns>Name of the new temporary file, null on error</returns>
        internal static string MakeTempFile(byte[] fileContent, string extension = ".exe")
        {
            // Ensure the extension is set
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".exe";
            else if (!extension.StartsWith("."))
                extension = $".{extension}";

            // Get the temporary path to use
            string tempFileName = Guid.NewGuid().ToString();
            string tempPath = Path.Combine(Path.GetTempPath(), "tmp", $"{tempFileName}{extension}");

            // Create and fill the file, if possible
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(tempPath)))
                {
                    bw.Write(fileContent);
                }

                return Path.GetFullPath(tempPath);
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Copies all required DLLs for a given executable
        /// </summary>
        /// <param name="file">Temporary file path</param>
        /// <param name="fileContent">File contents to read</param>
        /// <returns>Paths for all of the copied DLLs, null on error</returns>
        internal static string[] CopyDependentDlls(string file, byte[] fileContent)
        {
            if (fileContent == null)
                return null;

            // Process each of the DLLs that it finds
            try
            {
                unsafe
                {
                    // Read all of the executable header information
                    PEExecutable pex = PEExecutable.Deserialize(fileContent, 0);

                    // Find the import directory entry
                    IMAGE_DATA_DIRECTORY idei = pex.OptionalHeader.DataDirectories[(byte)ImageDirectory.IMAGE_DIRECTORY_ENTRY_IMPORT];
                    
                    // Set the table index and size
                    int tableIndex = (int)ConvertVirtualAddress(idei.VirtualAddress, pex.SectionHeaders);
                    int tableSize = (int)idei.Size;
                    if (tableIndex <= 0 || tableSize <= 0)
                        return null;

                    // Load the table from index
                    ImportDataSection idata = ImportDataSection.Deserialize(fileContent, tableIndex, pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus, hintCount: 0);
                    ImportDirectoryTable idt = idata.ImportDirectoryTable;

                    // TODO: Use the known layout to determine the names in a more automated way instead of having to iterate
                    
                    // Retrieve the table data
                    byte[] tableData = new byte[tableSize];
                    Array.Copy(fileContent, tableIndex, tableData, 0, tableSize);
                    int entryCount = tableSize / 4; // Each entry is a UInt32

                    // TODO: Validate what this table actually looks like.
                    // My concern about this is that it seems like each entry might be 16 bytes?
                    // The original code does a += 12, reads the address, and then moves on.
                    // That being said, the way that this works _does_ come up with a valid table,
                    // at least something that looks like a valid table, since it shows up with
                    // `ntoskrnl.exe` on the dot
                    // 
                    // Unfortunately, for other programs, this comes up with nonsense data, so it's hard
                    // to tell if the table is accurate or not.

                    // Iterate through the table and get valid DLL names
                    List<string> dependentDlls = new List<string>();
                    for (int i = 0; i < entryCount; i++)
                    {
                        // Get and validate the virtual offset
                        uint entryVirtualOffset = BitConverter.ToUInt32(tableData, i * 4);
                        if (entryVirtualOffset == 0)
                            continue;

                        // Get the DLL name from the table and add it to the list if possible
                        string entryDllName = new string(fileContent.Skip((int)entryVirtualOffset).TakeWhile(b => b > 0).Select(b => (char)b).ToArray());

                        try
                        {
                            if (File.Exists(Path.Combine(Path.GetDirectoryName(file), entryDllName)))
                            {
                                FileInfo fiDLL = new FileInfo(Path.Combine(Path.GetDirectoryName(file), entryDllName));
                                dependentDlls.Add(fiDLL.CopyTo(Path.Combine(Path.GetTempPath(), entryDllName), true).FullName);
                            }
                        }
                        catch { }
                    }

                    return dependentDlls.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Attempt to run an executable
        /// </summary>
        /// <param name="file">Executable to attempt to run</param>
        /// <returns>Process representing the running executable, null on error</returns>
        internal static Process StartSafe(string file)
        {
            if (file == null || !File.Exists(file))
                return null;

            // Create the process to start safely
            Process safeProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = file,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                }
            };

            // Try to start the process, returning the handle if we can
            try
            {
                safeProcess.Start();
                return safeProcess;
            }
            catch
            {
                return null;
            }
        }
    }
}
