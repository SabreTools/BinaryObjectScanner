using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to use path check framework here
    public class TAGES : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // protected-tages-runtime.exe
                new ContentMatchSet(new byte?[]
                {
                    0x70, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x65,
                    0x64, 0x2D, 0x74, 0x61, 0x67, 0x65, 0x73, 0x2D,
                    0x72, 0x75, 0x6E, 0x74, 0x69, 0x6D, 0x65, 0x2E,
                    0x65, 0x78, 0x65
                }, Utilities.GetFileVersion, "TAGES"),

                // (char)0xE8 + u + (char)0x00 + (char)0x00 + (char)0x00 + (char)0xE8
                new ContentMatchSet(new byte?[] { 0xE8, 0x75, 0x00, 0x00, 0x00, 0xE8 }, GetVersion, "TAGES"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the last section
            var lastSection = sections.LastOrDefault();
            if (lastSection != null)
            {
                int sectionAddr = (int)lastSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)lastSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // tagesprotection.com
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x74, 0x61, 0x67, 0x65, 0x73, 0x70, 0x72, 0x6F,
                            0x74, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x2E,
                            0x63, 0x6F, 0x6D
                        }, start: sectionEnd),
                    Utilities.GetFileVersion, "TAGES [tagesprotection.com]"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var protections = new ConcurrentQueue<string>();

            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("Tages.dll", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("Wave.aif", StringComparison.OrdinalIgnoreCase)))
            {
                protections.Enqueue("TAGES");
            }
            if (files.Any(f => Path.GetFileName(f).Equals("tagesclient.exe", StringComparison.OrdinalIgnoreCase)))
            {
                string file = files.First(f => Path.GetFileName(f).Equals("tagesclient.exe", StringComparison.OrdinalIgnoreCase));
                protections.Enqueue("TAGES Activation Client " + Utilities.GetFileVersion(file));
            }
            if (files.Any(f => Path.GetFileName(f).Equals("TagesSetup.exe", StringComparison.OrdinalIgnoreCase)))
            {
                string file = files.First(f => Path.GetFileName(f).Equals("TagesSetup.exe", StringComparison.OrdinalIgnoreCase));
                protections.Enqueue("TAGES Setup " + Utilities.GetFileVersion(file));
            }
            if (files.Any(f => Path.GetFileName(f).Equals("TagesSetup_x64.exe", StringComparison.OrdinalIgnoreCase)))
            {
                string file = files.First(f => Path.GetFileName(f).Equals("TagesSetup_x64.exe", StringComparison.OrdinalIgnoreCase));
                protections.Enqueue("TAGES Setup " + Utilities.GetFileVersion(file));
            }

            if (protections.Count == 0)
                return null;
            else
                return protections;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("Tages.dll", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("Wave.aif", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES";
            }
            else if (Path.GetFileName(path).Equals("tagesclient.exe", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Activation Client " + Utilities.GetFileVersion(path);
            }
            else if (Path.GetFileName(path).Equals("TagesSetup.exe", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Setup " + Utilities.GetFileVersion(path);
            }
            else if (Path.GetFileName(path).Equals("TagesSetup_x64.exe", StringComparison.OrdinalIgnoreCase))
            {
                return "TAGES Setup " + Utilities.GetFileVersion(path);
            }

            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            // (char)0xFF + (char)0xFF + "h"
            if (new ArraySegment<byte>(fileContent, --positions[0] + 8, 3).SequenceEqual(new byte[] { 0xFF, 0xFF, 0x68 })) // TODO: Verify this subtract
                return GetVersion(fileContent, positions[0]);
                
            return null;
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            switch (fileContent[position + 7])
            {
                case 0x1B:
                    return "5.3-5.4";
                case 0x14:
                    return "5.5.0";
                case 0x4:
                    return "5.5.2";
                default:
                    return string.Empty;
            }
        }
    }
}
