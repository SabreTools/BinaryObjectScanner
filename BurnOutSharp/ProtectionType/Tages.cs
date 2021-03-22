using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Tages : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // protected-tages-runtime.exe
                new Matcher(new byte?[]
                {
                    0x70, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x65,
                    0x64, 0x2D, 0x74, 0x61, 0x67, 0x65, 0x73, 0x2D,
                    0x72, 0x75, 0x6E, 0x74, 0x69, 0x6D, 0x65, 0x2E,
                    0x65, 0x78, 0x65
                }, Utilities.GetFileVersion, "TAGES"),

                // tagesprotection.com
                new Matcher(new byte?[]
                {
                    0x74, 0x61, 0x67, 0x65, 0x73, 0x70, 0x72, 0x6F,
                    0x74, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x2E,
                    0x63, 0x6F, 0x6D
                }, Utilities.GetFileVersion, "TAGES"),

                // (char)0xE8 + u + (char)0x00 + (char)0x00 + (char)0x00 + (char)0xE8
                new Matcher(new byte?[] { 0xE8, 0x75, 0x00, 0x00, 0x00, 0xE8 }, GetVersion, "TAGES"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            List<string> protections = new List<string>();

            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("Tages.dll", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("Wave.aif", StringComparison.OrdinalIgnoreCase)))
            {
                protections.Add("TAGES");
            }
            if (files.Any(f => Path.GetFileName(f).Equals("tagesclient.exe", StringComparison.OrdinalIgnoreCase)))
            {
                string file = files.First(f => Path.GetFileName(f).Equals("tagesclient.exe", StringComparison.OrdinalIgnoreCase));
                protections.Add("TAGES Activation Client " + Utilities.GetFileVersion(file));
            }
            if (files.Any(f => Path.GetFileName(f).Equals("TagesSetup.exe", StringComparison.OrdinalIgnoreCase)))
            {
                string file = files.First(f => Path.GetFileName(f).Equals("TagesSetup.exe", StringComparison.OrdinalIgnoreCase));
                protections.Add("TAGES Setup " + Utilities.GetFileVersion(file));
            }
            if (files.Any(f => Path.GetFileName(f).Equals("TagesSetup_x64.exe", StringComparison.OrdinalIgnoreCase)))
            {
                string file = files.First(f => Path.GetFileName(f).Equals("TagesSetup_x64.exe", StringComparison.OrdinalIgnoreCase));
                protections.Add("TAGES Setup " + Utilities.GetFileVersion(file));
            }

            if (protections.Count() == 0)
                return null;
            else
                return string.Join(", ", protections);
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
            }

            return "";
        }
    }
}
