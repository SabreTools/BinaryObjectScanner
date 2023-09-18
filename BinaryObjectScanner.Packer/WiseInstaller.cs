using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;
using Wise = WiseUnpacker.WiseUnpacker;

namespace BinaryObjectScanner.Packer
{
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class WiseInstaller : IExtractable, INewExecutableCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
#else
        public string? CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
#endif
        {
            // If we match a known header
            if (MatchesNEVersion(nex) != null)
                return "Wise Installation Wizard Module";

            // TODO: Investigate STUB.EXE in nonresident-name table

            // TODO: Don't read entire file
            var data = nex.ReadArbitraryRange();
            if (data == null)
                return null;

            var neMatchSets = new List<ContentMatchSet>
            {
                // WiseInst
                new ContentMatchSet(new byte?[] { 0x57, 0x69, 0x73, 0x65, 0x49, 0x6E, 0x73, 0x74 }, "Wise Installation Wizard Module"),

                // WiseMain
                new ContentMatchSet(new byte?[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E }, "Wise Installation Wizard Module"),
            };

            return MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // If we match a known header
            if (GetPEFormat(pex) != null)
                return "Wise Installation Wizard Module";

            // TODO: Investigate STUB32.EXE in export directory table

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("WiseMain")))
                    return "Wise Installation Wizard Module";
            }

            // Get the .rdata section strings, if they exist
            strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("WiseMain")))
                    return "Wise Installation Wizard Module";
            }

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream stream, string file, bool includeDebug)
#endif
        {
            try
            {
                // Try to parse as a New Executable
                var nex = NewExecutable.Create(stream);
                if (nex != null)
                    return ExtractNewExecutable(nex, file, includeDebug);

                // Try to parse as a Portable Executable
                var pex = PortableExecutable.Create(stream);
                if (pex != null)
                    return ExtractPortableExecutable(pex, file, includeDebug);

                return null;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Checks an NE header to see if it matches a known signature
        /// </summary>
        /// <param name="nex">New executable to check</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
#if NET48
        private FormatProperty MatchesNEVersion(NewExecutable nex)
#else
        private FormatProperty? MatchesNEVersion(NewExecutable nex)
#endif
        {
            // TODO: Offset is _not_ the EXE header address, rather where the data starts. Fix this.
            switch (nex.Model.Stub?.Header?.NewExeHeaderAddr)
            {
                case 0x84b0:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x11, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = true };

                case 0x3e10:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x3e50:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x3c20:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x3c30:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x22, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x3660:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x40, ArchiveEnd = 0x3c, InitText = false, FilenamePosition = 0x04, NoCrc = false };

                case 0x36f0:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x48, ArchiveEnd = 0x44, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3770:
                    return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3780:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x37b0:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x37d0:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3c80:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3bd0:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

                case 0x3c10:
                    return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

                default:
                    return null;
            }
        }

        /// <summary>
        /// Checks a PE header to see if it matches a known signature
        /// </summary>
        /// <param name="pex">Portable executable to check</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
#if NET48
        private FormatProperty GetPEFormat(PortableExecutable pex)
#else
        private FormatProperty? GetPEFormat(PortableExecutable pex)
#endif
        {
            if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3cf4
                && pex.GetFirstSection(".data")?.VirtualSize == 0x1528)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3cf4
                && pex.GetFirstSection(".data")?.VirtualSize == 0x1568)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d54)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d44)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d04)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            // Found in Binary.WiseCustomCalla
            else if (pex.OverlayAddress == 0x6200)
                return new FormatProperty { Dll = true, ArchiveStart = 0x62, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x3000)
                return new FormatProperty { Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x3800)
                return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

            else if (pex.OverlayAddress == 0x3a00)
                return new FormatProperty { Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false };

            return null;
        }

        /// <summary>
        /// Attempt to extract Wise data from a New Executable
        /// </summary>
        /// <param name="nex">New executable to check</param>
        /// <param name="file">Path to the input file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
#if NET48
        private string ExtractNewExecutable(NewExecutable nex, string file, bool includeDebug)
#else
        private string? ExtractNewExecutable(NewExecutable nex, string file, bool includeDebug)
#endif
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                // TODO: Try to find where the file data lives and how to get it
                Wise unpacker = new Wise();
                if (!unpacker.ExtractTo(file, tempPath))
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }

            return tempPath;
        }

        /// <summary>
        /// Attempt to extract Wise data from a Portable Executable
        /// </summary>
        /// <param name="pex">Portable executable to check</param>
        /// <param name="file">Path to the input file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
#if NET48
        private string ExtractPortableExecutable(PortableExecutable pex, string file, bool includeDebug)
#else
        private string? ExtractPortableExecutable(PortableExecutable pex, string file, bool includeDebug)
#endif
        {
            try
            {
                // Get the matching PE format
                var format = GetPEFormat(pex);
                if (format == null)
                    return null;

                // Get the overlay data for easier reading
                int overlayOffset = 0, dataStart = 0;
                var overlayData = pex.OverlayData;
                if (overlayData == null)
                    return null;

                // Skip over the additional DLL name, if we expect it
                if (format.Dll)
                {
                    // Read the name length
                    byte dllNameLength = overlayData.ReadByte(ref overlayOffset);
                    dataStart++;

                    // Read the name, if it exists
                    if (dllNameLength != 0)
                    {
                        // Ignore the name for now
                        _ = overlayData.ReadBytes(ref overlayOffset, dllNameLength);
                        dataStart += dllNameLength;

                        // Named DLLs also have a DLL length that we ignore
                        _ = overlayData.ReadUInt32(ref overlayOffset);
                        dataStart += 4;
                    }
                }

                // Check if flags are consistent
                if (!format.NoCrc)
                {
                    // Unlike WiseUnpacker, we ignore the flag value here
                    _ = overlayData.ReadUInt32(ref overlayOffset);
                }

                // Ensure that we have an archive end
                if (format.ArchiveEnd > 0)
                {
                    overlayOffset = dataStart + format.ArchiveEnd;
                    int archiveEndLoaded = overlayData.ReadInt32(ref overlayOffset);
                    if (archiveEndLoaded != 0)
                        format.ArchiveEnd = archiveEndLoaded;
                }

                // Skip to the start of the archive
                overlayOffset = dataStart + format.ArchiveStart;

                // Skip over the initialization text, if we expect it
                if (format.InitText)
                {
                    int initTextLength = overlayData.ReadByte(ref overlayOffset);
                    _ = overlayData.ReadBytes(ref overlayOffset, initTextLength);
                }

                // Cache the current offset in the overlay as the "start of data"
                int offsetReal = overlayOffset;

                // If the first entry is PKZIP, we assume it's an embedded zipfile
                var magic = overlayData.ReadBytes(ref overlayOffset, 4); overlayOffset -= 4;
                bool pkzip = magic?.StartsWith(new byte?[] { (byte)'P', (byte)'K' }) ?? false;

                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // If we have PKZIP
                if (pkzip)
                {
                    string tempFile = Path.Combine(tempPath, "WISEDATA.zip");
                    using (Stream tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        tempStream.Write(overlayData, overlayOffset, overlayData.Length - overlayOffset);
                    }
                }

                // If we have DEFLATE -- TODO: Port implementation here or use DeflateStream
                else
                {
                    Wise unpacker = new Wise();
                    if (!unpacker.ExtractTo(file, tempPath))
                    {
                        try
                        {
                            Directory.Delete(tempPath, true);
                        }
                        catch (Exception ex)
                        {
                            if (includeDebug) Console.WriteLine(ex);
                        }

                        return null;
                    }
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Class representing the properties of each recognized Wise installer format
        /// </summary>
        /// <see href="https://github.com/mnadareski/WiseUnpacker/blob/master/WiseUnpacker/FormatProperty.cs"/>
        private class FormatProperty
        {
            /// <summary>
            /// Offset to the executable data
            /// </summary>
            public int ExecutableOffset { get; set; }

            /// <summary>
            /// Indicates if this format includes a DLL at the start or not
            /// </summary>
            public bool Dll { get; set; }

            /// <summary>
            /// Offset within the data where the archive starts
            /// </summary>
            public int ArchiveStart { get; set; }

            /// <summary>
            /// Position in the archive head of the archive end
            /// </summary>
            public int ArchiveEnd { get; set; }

            /// <summary>
            /// Format includes initialization text
            /// </summary>
            public bool InitText { get; set; }

            /// <summary>
            /// Position of the filename within the data
            /// </summary>
            public int FilenamePosition { get; set; }

            /// <summary>
            /// Format does not include a CRC
            /// </summary>
            public bool NoCrc { get; set; }
        }
    }
}
