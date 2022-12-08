using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
using BurnOutSharp.Wrappers;
using Wise = WiseUnpacker.WiseUnpacker;

namespace BurnOutSharp.PackerType
{
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class WiseInstaller : INewExecutableCheck, IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            /// Check we have a valid executable
            if (nex == null)
                return null;

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

            // TODO: Investigate STUB32.EXE in export directory table

            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // WiseMain
                    new ContentMatchSet(new byte?[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E }, "Wise Installation Wizard Module"),
                };

                string match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .rdata section, if it exists
            if (pex.ContainsSection(".rdata"))
            {
                var matchers = new List<ContentMatchSet>
                {
                    // WiseMain
                    new ContentMatchSet(new byte?[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E }, "Wise Installation Wizard Module"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.GetFirstSectionData(".rdata"), matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
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
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
