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
using System.Runtime.InteropServices;
using BurnOutSharp.ExecutableType.Microsoft;

namespace BurnOutSharp.Tools
{
    internal static class EVORE
    {
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
                IMAGE_DOS_HEADER idh = IMAGE_DOS_HEADER.Deserialize(fileContent, 0);
                IMAGE_FILE_HEADER ifh = IMAGE_FILE_HEADER.Deserialize(fileContent, idh.NewExeHeaderAddr);

                // Check if file is dll
                return (ifh.Characteristics & 0x2000) != 0x2000;
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
                    IMAGE_DOS_HEADER idh = IMAGE_DOS_HEADER.Deserialize(fileContent, 0);
                    IMAGE_FILE_HEADER ifh = IMAGE_FILE_HEADER.Deserialize(fileContent, idh.NewExeHeaderAddr);
                    IMAGE_OPTIONAL_HEADER ioh = IMAGE_OPTIONAL_HEADER.Deserialize(fileContent, idh.NewExeHeaderAddr + Marshal.SizeOf(ifh));
                    
                    // Find the import directory entry
                    IMAGE_DATA_DIRECTORY idei = ioh.DataDirectory[Constants.IMAGE_DIRECTORY_ENTRY_IMPORT];

                    // Set the table index and size
                    int tableIndex = (int)idei.VirtualAddress;
                    int tableSize = (int)idei.Size;
                    if (tableIndex <= 0 || tableSize <= 0)
                        return null;
                    
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
        public static Process StartSafe(string file)
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

        /// <summary>
        /// Read all section headers from a PE executable
        /// </summary>
        /// <param name="fileContent">Byte array representing the executable</param>
        /// <param name="ptr">Pointer to the location in the array to read from</param>
        /// <returns>An array of section headers, null on error</returns>
        private static IMAGE_SECTION_HEADER[] ReadSections(byte[] fileContent)
        {
            if (fileContent == null)
                return null;

            try
            {
                unsafe
                {
                    IMAGE_DOS_HEADER idh = IMAGE_DOS_HEADER.Deserialize(fileContent, 0);
                    IMAGE_FILE_HEADER ifh = IMAGE_FILE_HEADER.Deserialize(fileContent, idh.NewExeHeaderAddr);

                    var sections = new IMAGE_SECTION_HEADER[ifh.NumberOfSections];
                    
                    int index = idh.NewExeHeaderAddr + Marshal.SizeOf(ifh) + ifh.SizeOfOptionalHeader;
                    for (int i = 0; i < ifh.NumberOfSections; i++)
                    {
                        sections[i] = ReadSection(fileContent, index); index += 40;
                    }

                    return sections;
                }
                
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Read a single section header from a PE executable
        /// </summary>
        /// <param name="fileContent">Byte array representing the executable</param>
        /// <param name="ptr">Pointer to the location in the array to read from</param>
        /// <returns>Section header, null on error</returns>
        private static IMAGE_SECTION_HEADER ReadSection(byte[] fileContent, int ptr)
        {
            try
            {
                // Get the size of a section header for later
                int sectionSize = Marshal.SizeOf<IMAGE_SECTION_HEADER>();

                // If the contents are null or the wrong size, we can't read a section
                if (fileContent == null || fileContent.Length < sectionSize)
                    return null;

                // Create a new section and try our best to read one
                IMAGE_SECTION_HEADER section = null;
                IntPtr tempPtr = IntPtr.Zero;
                try
                {
                    // Get the pointer to where the section will go
                    tempPtr = Marshal.AllocHGlobal(sectionSize);
                    
                    // If we couldn't get the space, just return null
                    if (tempPtr == IntPtr.Zero)
                        return null;

                    // Copy from the array to the new space
                    Marshal.Copy(fileContent, ptr, tempPtr, sectionSize);

                    // Get the new section and return
                    section = Marshal.PtrToStructure<IMAGE_SECTION_HEADER>(tempPtr);
                }
                catch
                {
                    // We don't care what the error was
                    return null;
                }
                finally
                {
                    if (tempPtr != IntPtr.Zero)
                        Marshal.FreeHGlobal(tempPtr);
                }

                return section;
            }
            catch
            {
                return null;
            }
        }
    }
}
