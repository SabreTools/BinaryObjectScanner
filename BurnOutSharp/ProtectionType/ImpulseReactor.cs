using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class ImpulseReactor
    {
        public static string CheckContents(string file, string fileContent)
        {
            if (fileContent.Contains("CVPInitializeClient"))
            {
                if (fileContent.Contains("A" + (char)0x00 + "T" + (char)0x00 + "T" + (char)0x00 + "L" + (char)0x00 + "I"
                    + (char)0x00 + "S" + (char)0x00 + "T" + (char)0x00 + (char)0x00 + (char)0x00 + "E" + (char)0x00 + "L"
                    + (char)0x00 + "E" + (char)0x00 + "M" + (char)0x00 + "E" + (char)0x00 + "N" + (char)0x00 + "T" + (char)0x00
                    + (char)0x00 + (char)0x00 + "N" + (char)0x00 + "O" + (char)0x00 + "T" + (char)0x00 + "A" + (char)0x00 + "T"
                    + (char)0x00 + "I" + (char)0x00 + "O" + (char)0x00 + "N"))
                {
                    return "Impulse Reactor " + Utilities.GetFileVersion(file);
                }
                else
                {
                    return "Impulse Reactor";
                }
            }

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("ImpulseReactor.dll", StringComparison.OrdinalIgnoreCase)) > 0)
                    return "Impulse Reactor " + Utilities.GetFileVersion(files.First(f => Path.GetFileName(f).Equals("ImpulseReactor.dll", StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                if (Path.GetFileName(path).Equals("ImpulseReactor.dll", StringComparison.OrdinalIgnoreCase))
                    return "Impulse Reactor " + Utilities.GetFileVersion(path);
            }

            return null;
        }
    }
}
