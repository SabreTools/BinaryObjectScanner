using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace BurnOutSharp.PackerType
{
    public class WinZipSFX : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // Every single version has the "WinZip Self-Extractor", and every 3+ version has a "version=" string due to internally including an XML
            // TODO: Harden detection to prevent a text file containing "version=" in a 2.x archive from being falsely detected as 3+ archive

            // "WinZip Self-Extractor"

            byte[] check = new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70, 0x20, 0x53, 0x65, 0x6C, 0x66, 0x2D, 0x45, 0x78, 0x74, 0x72, 0x61, 0x63, 0x74, 0x6F, 0x72 };
            if (fileContent.Contains(check, out int position))
            {
                // "<?xml"
                byte[] check2 = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C };
                if (fileContent.Contains(check2, out int position2))
                    return $"WinZip SFX {GetV3PlusVersion(fileContent, position, position2)}" + (includePosition ? $" (Index {position})" : string.Empty);
                else
                    return $"WinZip SFX {GetV2Version(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            return null;
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }
        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }

        private static string GetV3PlusVersion(byte[] fileContent, int position, int position2)
        {
            /* Every version above 3 contains an XML with a version string. Sometimes, the string doesnt exactly match the reported version number, but the internal version string can always be used to map to the public version number.
               Some generic version 3+ detection is possible (find the relevant "version=" string and stop at the ending quote), but every official 3+ is detected already, and with the version string not being reliable on it's own, this may not be worth implementing.
            
               If false positives do end up being an issue, there are a few extra strings that may be added:

               "_winzip_" (Found in almost every WinZip SFX archive, and every single 3+ one.)
               Hex: 0x5F, 0x77, 0x69, 0x6E, 0x7A, 0x69, 0x70, 0x5F

               "W.i.n.Z.i.p. .S.e.l.f.-.E.x.t.r.a.c.t.o.r." (Found in every 3+ version aside from 3.0.7158. This appears to be text for messages that WinZip SFX might display to a user, so much more text like it exists if an even more specfic check is needed for some reason.)
               Hex: 0x57, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x5A, 0x00, 0x69, 0x00, 0x70, 0x00, 0x20, 0x00, 0x53, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x66, 0x00, 0x2D, 0x00, 0x45, 0x00, 0x78, 0x00, 0x74, 0x00, 0x72, 0x00, 0x61, 0x00, 0x63, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x72, 0x00 */

            // Find the end of the XML so it can be imported and parsed
            // </assembly>
            byte[] check = new byte[] { 0x3C, 0x2F, 0x61, 0x73, 0x73, 0x65, 0x6D, 0x62, 0x6C, 0x79, 0x3E };
            if (fileContent.Contains(check, out int position3))
            {
                int offset = position3 + 11 - position2;
                string XML = Encoding.ASCII.GetString(fileContent, position2, offset);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(XML);
                string xmlVersion = xmlDoc["assembly"]["assemblyIdentity"].GetAttributeNode("version").InnerXml;
                switch (xmlVersion)
                {
                    case "3.0.7158.0":
                        return "3.0.7158";
                    case "3.1.7556.0":
                        return "3.1.7556";
                    case "3.1.8421.0":
                        return "4.0.8421";
                    case "3.1.8672.0":
                        return "4.0.8672";
                    case "4.0.1221.0":
                        return "4.0.12218";
                    default:
                        return $"(Unknown Version - {xmlVersion})";
                }

            }

            /* // "3.0.7158.0"
            check = new byte[] { 0x33, 0x2E, 0x30, 0x2E, 0x37, 0x31, 0x35, 0x38, 0x2E, 0x30 };
            if (fileContent.Contains(check, out position))
                return "3.0.7158";

            // "3.1.7556.0"
            check = new byte[] { 0x33, 0x2E, 0x31, 0x2E, 0x37, 0x35, 0x35, 0x36, 0x2E, 0x30 };
            if (fileContent.Contains(check, out position))
                return "3.1.7556";

            // "3.1.8421.0"
            check = new byte[] { 0x33, 0x2E, 0x31, 0x2E, 0x38, 0x34, 0x32, 0x31, 0x2E, 0x30 };
            if (fileContent.Contains(check, out position))
                return "4.0.8421";

            // "3.1.8672.0"
            check = new byte[] { 0x33, 0x2E, 0x31, 0x2E, 0x38, 0x36, 0x37, 0x32, 0x2E, 0x30 };
            if (fileContent.Contains(check, out position))
                return "4.0.8672";

            // "4.0.1221.0"
            check = new byte[] { 0x34, 0x2E, 0x30, 0x2E, 0x31, 0x32, 0x32, 0x31, 0x2E, 0x30 };
            if (fileContent.Contains(check, out position))
                return "4.0.12218";
            */
            return "Unknown version above 3.0";
        }

        private static string GetV2Version(byte[] fileContent, int position)
        {
            return "Version 2.x";
        }
    }
}
