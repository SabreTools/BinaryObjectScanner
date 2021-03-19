using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class StarForce : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "(" + (char)0x00 + "c" + (char)0x00 + ")" + (char)0x00 + " " + (char)0x00 + "P" + (char)0x00 + "r" + (char)0x00 + "o" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + " " + (char)0x00 + "T" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "h" + (char)0x00 + "n" + (char)0x00 + "o" + (char)0x00 + "l" + (char)0x00 + "o" + (char)0x00 + "g" + (char)0x00 + "y" + (char)0x00
            byte[] check = new byte[] { 0x28, 0x00, 0x63, 0x00, 0x29, 0x00, 0x20, 0x00, 0x50, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x74, 0x00, 0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x54, 0x00, 0x65, 0x00, 0x63, 0x00, 0x68, 0x00, 0x6E, 0x00, 0x6F, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x67, 0x00, 0x79, 0x00 };
            if (fileContent.Contains(check, out int position))
            {
                // "PSA_GetDiscLabel"
                // byte[] check2 = new byte[] { 0x50, 0x53, 0x41, 0x5F, 0x47, 0x65, 0x74, 0x44, 0x69, 0x73, 0x63, 0x4C, 0x61, 0x62, 0x65, 0x6C };

                // "(c) Protection Technology"
                // byte[] check2 = new byte[] { 0x28, 0x63, 0x29, 0x20, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x54, 0x65, 0x63, 0x68, 0x6E, 0x6F, 0x6C, 0x6F, 0x67, 0x79 };

                // "TradeName"
                byte[] check2 = new byte[] { 0x54, 0x72, 0x61, 0x64, 0x65, 0x4E, 0x61, 0x6D, 0x65 };
                if (fileContent.Contains(check2, out int position2) && position2 != 0)
                    return $"StarForce {Utilities.GetFileVersion(file)} ({fileContent.Skip(position2 + 22).TakeWhile(c => c != 0x00)})" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                else
                    return $"StarForce {Utilities.GetFileVersion(file)}" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
            }

            // "Protection Technology, Ltd."
            check = new byte[] { 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x54, 0x65, 0x63, 0x68, 0x6E, 0x6F, 0x6C, 0x6F, 0x67, 0x79, 0x2C, 0x20, 0x4C, 0x74, 0x64, 0x2E };
            if (fileContent.Contains(check, out position))
            {
                // "PSA_GetDiscLabel"
                // byte[] check2 = new byte[] { 0x50, 0x53, 0x41, 0x5F, 0x47, 0x65, 0x74, 0x44, 0x69, 0x73, 0x63, 0x4C, 0x61, 0x62, 0x65, 0x6C };

                // "(c) Protection Technology"
                // byte[] check2 = new byte[] { 0x28, 0x63, 0x29, 0x20, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x54, 0x65, 0x63, 0x68, 0x6E, 0x6F, 0x6C, 0x6F, 0x67, 0x79 };

                // "TradeName"
                byte[] check2 = new byte[] { 0x54, 0x72, 0x61, 0x64, 0x65, 0x4E, 0x61, 0x6D, 0x65 };
                if (fileContent.Contains(check2, out int position2) && position2 != 0)
                    return $"StarForce {Utilities.GetFileVersion(file)} ({fileContent.Skip(position2 + 22).TakeWhile(c => c != 0x00)})" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                else
                    return $"StarForce {Utilities.GetFileVersion(file)}" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
            }

            // ".sforce"
            check = new byte[] { 0x2E, 0x73, 0x66, 0x6F, 0x72, 0x63, 0x65 };
            if (fileContent.Contains(check, out position))
                return "StarForce 3-5" + (includePosition ? $" (Index {position})" : string.Empty);

            // ".brick"
            check = new byte[] { 0x2E, 0x62, 0x72, 0x69, 0x63, 0x6B };
            if (fileContent.Contains(check, out position))
                return "StarForce 3-5" + (includePosition ? $" (Index {position})" : string.Empty);

            // "P" + (char)0x00 + "r" + (char)0x00 + "o" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "d" + (char)0x00 + " " + (char)0x00 + "M" + (char)0x00 + "o" + (char)0x00 + "d" + (char)0x00 + "u" + (char)0x00 + "l" + (char)0x00 + "e"
            check = new byte[] { 0x50, 0x00, 0x72, 0x00, 0x6f, 0x00, 0x74, 0x00, 0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x65, 0x00, 0x64, 0x00, 0x20, 0x00, 0x4d, 0x00, 0x6f, 0x00, 0x64, 0x00, 0x75, 0x00, 0x6c, 0x00, 0x65 };
            if (fileContent.Contains(check, out position))
                return "StarForce 5" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("protect.dll", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("protect.exe", StringComparison.OrdinalIgnoreCase)))
            {
                return "StarForce";
            }
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("protect.dll", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("protect.exe", StringComparison.OrdinalIgnoreCase))
            {
                return "StarForce";
            }

            return null;
        }
    }
}
