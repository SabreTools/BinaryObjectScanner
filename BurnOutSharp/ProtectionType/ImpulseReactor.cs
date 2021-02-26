using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class ImpulseReactor : IPathCheck
    {
        public static string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "CVPInitializeClient"
            byte[] check = new byte[] { 0x43, 0x56, 0x50, 0x49, 0x6E, 0x69, 0x74, 0x69, 0x61, 0x6C, 0x69, 0x7A, 0x65, 0x43, 0x6C, 0x69, 0x65, 0x6E, 0x74 };
            if (fileContent.Contains(check, out int position))
            {
                // "A" + (char)0x00 + "T" + (char)0x00 + "T" + (char)0x00 + "L" + (char)0x00 + "I" + (char)0x00 + "S" + (char)0x00 + "T" + (char)0x00 + (char)0x00 + (char)0x00 + "E" + (char)0x00 + "L" + (char)0x00 + "E" + (char)0x00 + "M" + (char)0x00 + "E" + (char)0x00 + "N" + (char)0x00 + "T" + (char)0x00 + (char)0x00 + (char)0x00 + "N" + (char)0x00 + "O" + (char)0x00 + "T" + (char)0x00 + "A" + (char)0x00 + "T" + (char)0x00 + "I" + (char)0x00 + "O" + (char)0x00 + "N"
                byte[] check2 = new byte[] { 0x41, 0x00, 0x54, 0x00, 0x54, 0x00, 0x4C, 0x00, 0x49, 0x00, 0x53, 0x00, 0x54, 0x00, 0x00, 0x00, 0x45, 0x00, 0x4C, 0x00, 0x45, 0x00, 0x4D, 0x00, 0x45, 0x00, 0x4E, 0x00, 0x54, 0x00, 0x00, 0x00, 0x4E, 0x00, 0x4F, 0x00, 0x54, 0x00, 0x41, 0x00, 0x54, 0x00, 0x49, 0x00, 0x4F, 0x00, 0x4E };
                if (fileContent.Contains(check2, out int position2))
                    return $"Impulse Reactor {Utilities.GetFileVersion(file)}" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                else
                    return "Impulse Reactor" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("ImpulseReactor.dll", StringComparison.OrdinalIgnoreCase)))
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
