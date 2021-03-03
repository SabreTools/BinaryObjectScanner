using System;
using System.Collections.Generic;
using System.IO;

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
            // "WinZip Self-Extractor"
            byte[] check = new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70, 0x20, 0x53, 0x65, 0x6C, 0x66, 0x2D, 0x45, 0x78, 0x74, 0x72, 0x61, 0x63, 0x74, 0x6F, 0x72 };
            if (fileContent.Contains(check, out int position))
            {
                // "version="
                byte[] check2 = new byte[] { 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D };
                if (fileContent.Contains(check2, out int position2))
                    return $"WinZip SFX {GetV3PlusVersion(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);
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

        private static string GetV3PlusVersion(byte[] fileContent, int position)
        {
            /* Every version above 3 contains an XML with a version string. Sometimes, the string doesnt exactly match the reported version number, but the internal version string can always be used to map to the public version number.
               Some generic version 3+ detection is possible (find the relevant "version=" string and stop at the ending quote), but every official 3+ is detected already, and with the version string not being reliable on it's own, this may not be worth implementing.
            
               If false positives do end up being an issue, there are a few extra strings that may be added:

               "_winzip_" (Found in almost every WinZip SFX archive, and every single 3+ one.)
               Hex: 0x5F, 0x77, 0x69, 0x6E, 0x7A, 0x69, 0x70, 0x5F

               "W.i.n.Z.i.p. .S.e.l.f.-.E.x.t.r.a.c.t.o.r." (Found in every 3+ version aside from 3.0.7158. This appears to be text for messages that WinZip SFX might display to a user, so much more text like it exists if an even more specfic check is needed for some reason.)
               Hex: 0x57, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x5A, 0x00, 0x69, 0x00, 0x70, 0x00, 0x20, 0x00, 0x53, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x66, 0x00, 0x2D, 0x00, 0x45, 0x00, 0x78, 0x00, 0x74, 0x00, 0x72, 0x00, 0x61, 0x00, 0x63, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x72, 0x00 */

            // "3.0.7158.0"
            byte[] check3 = new byte[] { 0x33, 0x2E, 0x30, 0x2E, 0x37, 0x31, 0x35, 0x38, 0x2E, 0x30 };
            if (fileContent.Contains(check3, out position))
            {
                string version = "3.0.7158";
                return version;
            }

            // "3.1.7556.0"
            byte[] check4 = new byte[] { 0x33, 0x2E, 0x31, 0x2E, 0x37, 0x35, 0x35, 0x36, 0x2E, 0x30 };
            if (fileContent.Contains(check4, out position))
            {
                string version = "3.1.7556";
                return version;
            }

            // "3.1.8421.0"
            byte[] check5 = new byte[] { 0x33, 0x2E, 0x31, 0x2E, 0x38, 0x34, 0x32, 0x31, 0x2E, 0x30 };
            if (fileContent.Contains(check5, out position))
            {
                string version = "4.0.8421";
                return version;
            }

            // "3.1.8672.0"
            byte[] check6 = new byte[] { 0x33, 0x2E, 0x31, 0x2E, 0x38, 0x36, 0x37, 0x32, 0x2E, 0x30 };
            if (fileContent.Contains(check6, out position))
            {
                string version = "4.0.8672";
                return version;
            }

            // "4.0.1221.0"
            byte[] check7 = new byte[] { 0x34, 0x2E, 0x30, 0x2E, 0x31, 0x32, 0x32, 0x31, 0x2E, 0x30 };
            if (fileContent.Contains(check7, out position))
            {
                string version = "4.0.12218";
                return version;
            }

            else
            {
                string version = "Unknown version above 3.0";
                return version;
            }
        }

        private static string GetV2Version(byte[] fileContent, int position)
        {
            string version = "Version 2.x";
            return version;
        }
    }
}
