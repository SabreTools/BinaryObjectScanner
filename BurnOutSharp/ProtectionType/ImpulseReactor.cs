using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // Note that this set of checks also contains "Stardock Product Activation"
    // This is intentional, as that protection is highly related to Impulse Reactor
    public class ImpulseReactor : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section, if it exists
            var rdataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rdata"));
            if (rdataSection != null)
            {
                int sectionAddr = (int)rdataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rdataSection.VirtualSize;

                // CVPInitializeClient
                byte?[] check = new byte?[]
                {
                    0x43, 0x56, 0x50, 0x49, 0x6E, 0x69, 0x74, 0x69,
                    0x61, 0x6C, 0x69, 0x7A, 0x65, 0x43, 0x6C, 0x69,
                    0x65, 0x6E, 0x74
                };
                bool containsCheck = fileContent.FirstPosition(check, out int position, start: sectionAddr, end: sectionEnd);

                // A + (char)0x00 + T + (char)0x00 + T + (char)0x00 + L + (char)0x00 + I + (char)0x00 + S + (char)0x00 + T + (char)0x00 + (char)0x00 + (char)0x00 + E + (char)0x00 + L + (char)0x00 + E + (char)0x00 + M + (char)0x00 + E + (char)0x00 + N + (char)0x00 + T + (char)0x00 + (char)0x00 + (char)0x00 + N + (char)0x00 + O + (char)0x00 + T + (char)0x00 + A + (char)0x00 + T + (char)0x00 + I + (char)0x00 + O + (char)0x00 + N + (char)0x00
                byte?[] check2 = new byte?[]
                {
                    0x41, 0x00, 0x54, 0x00, 0x54, 0x00, 0x4C, 0x00,
                    0x49, 0x00, 0x53, 0x00, 0x54, 0x00, 0x00, 0x00,
                    0x45, 0x00, 0x4C, 0x00, 0x45, 0x00, 0x4D, 0x00,
                    0x45, 0x00, 0x4E, 0x00, 0x54, 0x00, 0x00, 0x00,
                    0x4E, 0x00, 0x4F, 0x00, 0x54, 0x00, 0x41, 0x00,
                    0x54, 0x00, 0x49, 0x00, 0x4F, 0x00, 0x4E
                };
                bool containsCheck2 = fileContent.FirstPosition(check2, out int position2, start: sectionAddr, end: sectionEnd);

                if (containsCheck && containsCheck2)
                    return $"Impulse Reactor Core Module {Utilities.GetFileVersion(file, fileContent, new List<int> { position, position2 })}" + (includeDebug ? $" (Index {position}, {position2})" : string.Empty);
                else if (containsCheck && !containsCheck2)
                    return $"Impulse Reactor" + (includeDebug ? $" (Index {position})" : string.Empty);
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are AND or OR
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("ImpulseReactor.dll", useEndsWith: true), Utilities.GetFileVersion, "Impulse Reactor Core Module"),
                new PathMatchSet(new PathMatch("ReactorActivate.exe", useEndsWith: true), Utilities.GetFileVersion, "Stardock Product Activation"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: Verify if these are AND or OR
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("ImpulseReactor.dll", useEndsWith: true), Utilities.GetFileVersion, "Impulse Reactor Core Module"),
                new PathMatchSet(new PathMatch("ReactorActivate.exe", useEndsWith: true), Utilities.GetFileVersion, "Stardock Product Activation"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
