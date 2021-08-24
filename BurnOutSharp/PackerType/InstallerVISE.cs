using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class InstallerVISE : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                //TODO: Add exact version detection for Windows builds, make sure versions before 3.X are detected as well, and detect the Mac builds.

                // ViseMain
                new ContentMatchSet(
                    new ContentMatch(new byte?[] { 0x56, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E }, start: 0xE0A4, end: 0xE0A5),
                    "Installer VISE"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }

        // TODO: Add Installer VISE extraction
        // https://github.com/Bioruebe/UniExtract2
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
            return null;
        }
    }
}
