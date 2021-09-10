using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class InstallerVISE : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        //TODO: Add exact version detection for Windows builds, make sure versions before 3.X are detected as well, and detect the Mac builds.
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the DATA/.data section, if it exists
            var dataSection = sections.FirstOrDefault(s => {
                string name = Encoding.ASCII.GetString(s.Name).Trim('\0');
                return name.StartsWith("DATA") || name.StartsWith(".data");
            });
            if (dataSection != null)
            {
                int sectionAddr = (int)dataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)dataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // ViseMain
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x56, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E }, start: sectionAddr, end: sectionEnd),
                        "Installer VISE"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
            }
            
            return null;
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
