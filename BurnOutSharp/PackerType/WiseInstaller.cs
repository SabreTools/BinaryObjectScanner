using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;
using static BurnOutSharp.Utilities.Dictionary;
using Wise = WiseUnpacker.WiseUnpacker;

namespace BurnOutSharp.PackerType
{
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class WiseInstaller : INewExecutableCheck, IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            /// Check we have a valid executable
            if (nex == null)
                return null;

            // If we match a known header
            if (MatchesNEVersion(nex))
                return "Wise Installation Wizard Module";

            // TODO: Don't read entire file
            var data = nex.ReadArbitraryRange();
            if (data == null)
                return null;

            // TODO: Keep this around until it can be confirmed with NE checks as well
            // TODO: This _may_ actually over-match. See msvbvm50.exe for an example
            var neMatchSets = new List<ContentMatchSet>
            {
                // WiseMain
                new ContentMatchSet(new byte?[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E }, "Wise Installation Wizard Module"),
            };

            return MatchUtil.GetFirstMatch(file, data, neMatchSets, includeDebug);
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // If we match a known header
            if (MatchesPEVersion(pex))
                return "Wise Installation Wizard Module";

            // TODO: Investigate STUB32.EXE in export directory table

            // Get the .data/DATA section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
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
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the installer file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                Wise unpacker = new Wise();
                unpacker.ExtractTo(file, tempPath);

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        /// Checks an NE header to see if it matches a known signature
        /// </summary>
        /// <param name="nex">New executable to check</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
        private bool MatchesNEVersion(NewExecutable nex)
        {
            switch (nex.Stub_NewExeHeaderAddr)
            {
                // Dll = false, ArchiveStart = 0x11, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = true
                case 0x84b0: return true;

                // Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false
                case 0x3e10: return true;

                // Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false
                case 0x3e50: return true;

                // Dll = false, ArchiveStart = 0x1e, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false
                case 0x3c20: return true;

                // Dll = false, ArchiveStart = 0x22, ArchiveEnd = -1, InitText = false, FilenamePosition = 0x04, NoCrc = false
                case 0x3c30: return true;

                // Dll = false, ArchiveStart = 0x40, ArchiveEnd = 0x3c, InitText = false, FilenamePosition = 0x04, NoCrc = false
                case 0x3660: return true;

                // Dll = false, ArchiveStart = 0x48, ArchiveEnd = 0x44, InitText = false, FilenamePosition = 0x1c, NoCrc = false
                case 0x36f0: return true;

                // Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
                case 0x3770: return true;

                // Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
                case 0x3780: return true;

                // Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
                case 0x37b0: return true;

                // Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
                case 0x37d0: return true;

                // Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false
                case 0x3c80: return true;

                // Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false
                case 0x3bd0: return true;

                // Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false
                case 0x3c10: return true;

                default: return false;
            }
        }

        /// <summary>
        /// Checks a PE header to see if it matches a known signature
        /// </summary>
        /// <param name="pex">Portable executable to check</param>
        /// <returns>True if it matches a known version, false otherwise</returns>
        private bool MatchesPEVersion(PortableExecutable pex)
        {
            // Dll = false, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
            if (pex.Stub_NewExeHeaderAddr == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3cf4
                && pex.GetFirstSection(".data")?.VirtualSize == 0x1528)
                return true;

            // Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
            else if (pex.Stub_NewExeHeaderAddr == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3cf4
                && pex.GetFirstSection(".data")?.VirtualSize == 0x1568)
                return true;

            // Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
            else if (pex.Stub_NewExeHeaderAddr == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d54)
                return true;

            // Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
            else if (pex.Stub_NewExeHeaderAddr == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d44)
                return true;

            // Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
            else if (pex.Stub_NewExeHeaderAddr == 0x6e00
                && pex.GetFirstSection(".text")?.VirtualSize == 0x3d04)
                return true;

            // Dll = true, ArchiveStart = 0x50, ArchiveEnd = 0x4c, InitText = false, FilenamePosition = 0x1c, NoCrc = false
            else if (pex.Stub_NewExeHeaderAddr == 0x3000)
                return true;

            // Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false
            else if (pex.Stub_NewExeHeaderAddr == 0x3800)
                return true;

            // Dll = true, ArchiveStart = 0x5a, ArchiveEnd = 0x4c, InitText = true, FilenamePosition = 0x1c, NoCrc = false
            else if (pex.Stub_NewExeHeaderAddr == 0x3a00)
                return true;

            return false;
        }
    }
}
