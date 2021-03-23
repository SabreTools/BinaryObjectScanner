using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class SetupFactory : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // S.e.t.u.p. .F.a.c.t.o.r.y.
                new ContentMatchSet(new byte?[]
                {
                    0x53, 0x00, 0x65, 0x00, 0x74, 0x00, 0x75, 0x00,
                    0x70, 0x00, 0x20, 0x00, 0x46, 0x00, 0x61, 0x00,
                    0x63, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x72, 0x00,
                    0x79, 0x00
                }, GetVersion, "Setup Factory"),

                // Longer version of the check that can be used if false positves become an issue:
                // S.e.t.u.p. .F.a.c.t.o.r.y. .i.s. .a. .t.r.a.d.e.m.a.r.k. .o.f. .I.n.d.i.g.o. .R.o.s.e. .C.o.r.p.o.r.a.t.i.o.n.
                // new ContentMatchSet(new byte?[]
                // {
                //     0x53, 0x00, 0x65, 0x00, 0x74, 0x00, 0x75, 0x00,
                //     0x70, 0x00, 0x20, 0x00, 0x46, 0x00, 0x61, 0x00,
                //     0x63, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x72, 0x00,
                //     0x79, 0x00, 0x20, 0x00, 0x69, 0x00, 0x73, 0x00,
                //     0x20, 0x00, 0x61, 0x00, 0x20, 0x00, 0x74, 0x00,
                //     0x72, 0x00, 0x61, 0x00, 0x64, 0x00, 0x65, 0x00,
                //     0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x6B, 0x00,
                //     0x20, 0x00, 0x6F, 0x00, 0x66, 0x00, 0x20, 0x00,
                //     0x49, 0x00, 0x6E, 0x00, 0x64, 0x00, 0x69, 0x00,
                //     0x67, 0x00, 0x6F, 0x00, 0x20, 0x00, 0x52, 0x00,
                //     0x6F, 0x00, 0x73, 0x00, 0x65, 0x00, 0x20, 0x00,
                //     0x43, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x70, 0x00,
                //     0x6F, 0x00, 0x72, 0x00, 0x61, 0x00, 0x74, 0x00,
                //     0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00
                // }, GetVersion, "Setup Factory"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        // TODO: Add extraction, which is possible but the only tools available that can
        // do this seem to be Universal Extractor 2 and InstallExplorer (https://totalcmd.net/plugring/InstallExplorer.html)
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }
    
        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            // Check the manifest version first
            string version = Utilities.GetManifestVersion(fileContent);
            if (!string.IsNullOrEmpty(version))
                return version;
            
            // Then check the file version
            version = Utilities.GetFileVersion(file);
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}
