﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Tages
    {
        public static string CheckContents(string file, string fileContent)
        {
            string check = "protected-tages-runtime.exe";
            if (fileContent.Contains(check))
                return $"TAGES {Utilities.GetFileVersion(file)} (Index {fileContent.IndexOf(check)})";

            check = "tagesprotection.com";
            if (fileContent.Contains(check))
                return $"TAGES {Utilities.GetFileVersion(file)} (Index {fileContent.IndexOf(check)})";

            check = "" + (char)0xE8 + "u" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0xE8;
            int position = fileContent.IndexOf(check);
            if (position > -1)
            {
                if (fileContent.Substring(--position + 8, 3) == "" + (char)0xFF + (char)0xFF + "h") // TODO: Verify this subtract
                    return $"TAGES {GetVersion(file, position)} (Index {position})";
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

        private static string GetVersion(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position + 7, SeekOrigin.Begin);
                byte bVersion = br.ReadByte();
                switch (bVersion)
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
}
