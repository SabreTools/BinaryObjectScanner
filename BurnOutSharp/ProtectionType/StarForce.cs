using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class StarForce
    {
        public static string CheckContents(string file, string fileContent)
        {
            string check = "(" + (char)0x00 + "c" + (char)0x00 + ")" + (char)0x00 + " " + (char)0x00 + "P" + (char)0x00 + "r" + (char)0x00 + "o" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + " " + (char)0x00 + "T" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "h" + (char)0x00 + "n" + (char)0x00 + "o" + (char)0x00 + "l" + (char)0x00 + "o" + (char)0x00 + "g" + (char)0x00 + "y" + (char)0x00;
            int position;
            if (fileContent.Contains(check))
            {
                //if (FileContent.Contains("PSA_GetDiscLabel")
                //if (FileContent.Contains("(c) Protection Technology")
                position = fileContent.IndexOf("TradeName") - 1;
                if (position != -1 && position != -2)
                    return $"StarForce {Utilities.GetFileVersion(file)} ({fileContent.Substring(position + 22, 30).Split((char)0x00)[0]}) (Index {fileContent.IndexOf(check)}, {position})";
                else
                    return $"StarForce {Utilities.GetFileVersion(file)} (Index {fileContent.IndexOf(check)}, {position})";
            }


            check = "Protection Technology, Ltd.";
            if (fileContent.Contains(check))
            {
                //if (FileContent.Contains("PSA_GetDiscLabel")
                //if (FileContent.Contains("(c) Protection Technology")
                position = fileContent.IndexOf("TradeName") - 1;
                if (position != -1 && position != -2)
                    return $"StarForce {Utilities.GetFileVersion(file)} ({fileContent.Substring(position + 22, 30).Split((char)0x00)[0]}) (Index {fileContent.IndexOf(check)}, {position})";
                else
                    return $"StarForce {Utilities.GetFileVersion(file)} (Index {fileContent.IndexOf(check)})";
            }

            check = ".sforce";
            if (fileContent.Contains(check))
                return $"StarForce 3-5 (Index {fileContent.IndexOf(check)})";

            check = ".brick";
            if (fileContent.Contains(check))
                return $"StarForce 3-5 (Index {fileContent.IndexOf(check)})";

            check = "P" + (char)0x00 + "r" + (char)0x00 + "o" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "d" + (char)0x00 + " " + (char)0x00 + "M" + (char)0x00 + "o" + (char)0x00 + "d" + (char)0x00 + "u" + (char)0x00 + "l" + (char)0x00 + "e";
            if (fileContent.Contains(check))
                return $"StarForce 5 (Index {fileContent.IndexOf(check)})";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("protect.dll", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("protect.exe", StringComparison.OrdinalIgnoreCase)))
                {
                    return "StarForce";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("protect.dll", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("protect.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "StarForce";
                }
            }

            return null;
        }
    }
}
