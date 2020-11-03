using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BurnOutSharp.FileType
{
    internal class Textfile
    {
        public static bool ShouldScan(byte[] magic, string extension)
        {
            // Rich Text File
            if (magic.StartsWith(new byte[] { 0x7b, 0x5c, 0x72, 0x74, 0x66, 0x31 }))
                return true;

            // HTML
            if (magic.StartsWith(new byte[] { 0x3c, 0x68, 0x74, 0x6d, 0x6c }))
                return true;

            // HTML and XML
            if (magic.StartsWith(new byte[] { 0x3c, 0x21, 0x44, 0x4f, 0x43, 0x54, 0x59, 0x50, 0x45 }))
                return true;

            // Microsoft Office File (old)
            if (magic.StartsWith(new byte[] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 }))
                return true;

            // Generic textfile (no header)
            if (string.Equals(extension, "txt", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public static List<string> Scan(Stream stream)
        {
            List<string> protections = new List<string>();

            try
            {
                // Load the current file content
                string fileContent = null;
                using (StreamReader sr = new StreamReader(stream, Encoding.Default, false, 1024 * 1024, true))
                {
                    fileContent = sr.ReadToEnd();
                }

                // CD-Key
                if (fileContent.Contains("a valid serial number is required"))
                    protections.Add("CD-Key / Serial");
                else if (fileContent.Contains("serial number is located"))
                    protections.Add("CD-Key / Serial");

                // MediaMax
                if (fileContent.Contains("MediaMax technology"))
                    protections.Add("MediaMax CD-3");
            }
            catch
            {
                // We don't care what the error was
            }

            return protections;
        }
    }
}
