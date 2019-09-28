using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDCops
    {
        public static string CheckContents(string file, string fileContent)
        {
            int position;
            if ((position = fileContent.IndexOf("CD-Cops,  ver. ")) > -1)
                return "CD-Cops " + GetVersion(file, position);

            if (fileContent.Contains(".grand" + (char)0x00))
                return "CD-Cops";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("CDCOPS.DLL", StringComparison.OrdinalIgnoreCase)) > 0
                    && (files.Count(f => Path.GetExtension(f).Trim('.').Equals("GZ_", StringComparison.OrdinalIgnoreCase)) > 0
                        || files.Count(f => Path.GetExtension(f).Trim('.').Equals("W_X", StringComparison.OrdinalIgnoreCase)) > 0
                        || files.Count(f => Path.GetExtension(f).Trim('.').Equals("Qz", StringComparison.OrdinalIgnoreCase)) > 0
                        || files.Count(f => Path.GetExtension(f).Trim('.').Equals("QZ_", StringComparison.OrdinalIgnoreCase)) > 0))
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
