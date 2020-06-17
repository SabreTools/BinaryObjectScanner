using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Tages
    {
        public static string CheckContents(string file, byte[] fileContent)
        {
            // "protected-tages-runtime.exe"
            byte[] check = new byte[] { 0x70, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x65, 0x64, 0x2D, 0x74, 0x61, 0x67, 0x65, 0x73, 0x2D, 0x72, 0x75, 0x6E, 0x74, 0x69, 0x6D, 0x65, 0x2E, 0x65, 0x78, 0x65 };
            if (fileContent.Contains(check, out int position))
                return $"TAGES {Utilities.GetFileVersion(file)} (Index {position})";

            // "tagesprotection.com"
            check = new byte[] { 0x74, 0x61, 0x67, 0x65, 0x73, 0x70, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x2E, 0x63, 0x6F, 0x6D };
            if (fileContent.Contains(check, out position))
                return $"TAGES {Utilities.GetFileVersion(file)} (Index {position})";

            // (char)0xE8 + "u" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0xE8
            check = new byte[] { 0xE8, 0x75, 0x00, 0x00, 0x00, 0xE8 };
            if (fileContent.Contains(check, out position))
            {
                // (char)0xFF + (char)0xFF + "h"
                if (fileContent.Skip(--position + 8).Take(3).SequenceEqual(new byte[] { 0xFF, 0xFF, 0x68 })) // TODO: Verify this subtract
                    return $"TAGES {GetVersion(fileContent, position)} (Index {position})";
            }

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
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
            else
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
            }

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
