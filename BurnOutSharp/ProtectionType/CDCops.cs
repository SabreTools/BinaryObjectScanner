﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDCops
    {
        public static string CheckContents(string file, string fileContent)
        {
            string check = "CD-Cops,  ver. ";
            int position = fileContent.IndexOf(check);
            if (position > -1)
                return $"CD-Cops {GetVersion(file, position)} (Index {position})";

            check = ".grand" + (char)0x00;
            if (fileContent.Contains(check))
                return $"CD-Cops (Index {fileContent.IndexOf(check)})";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("CDCOPS.DLL", StringComparison.OrdinalIgnoreCase))
                    && (files.Any(f => Path.GetExtension(f).Trim('.').Equals("GZ_", StringComparison.OrdinalIgnoreCase))
                        || files.Any(f => Path.GetExtension(f).Trim('.').Equals("W_X", StringComparison.OrdinalIgnoreCase))
                        || files.Any(f => Path.GetExtension(f).Trim('.').Equals("Qz", StringComparison.OrdinalIgnoreCase))
                        || files.Any(f => Path.GetExtension(f).Trim('.').Equals("QZ_", StringComparison.OrdinalIgnoreCase))))
                {
                    return "CD-Cops";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("CDCOPS.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("GZ_", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("W_X", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("Qz", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("QZ_", StringComparison.OrdinalIgnoreCase))
                {
                    return "CD-Cops";
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
                br.BaseStream.Seek(position + 15, SeekOrigin.Begin); // Begin reading after "CD-Cops,  ver."
                char[] version = br.ReadChars(4);
                if (version[0] == 0x00)
                    return "";

                return new string(version);
            }
        }
    }
}
