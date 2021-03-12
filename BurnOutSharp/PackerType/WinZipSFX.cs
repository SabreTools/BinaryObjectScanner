using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace BurnOutSharp.PackerType
{
    public class WinZipSFX : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // Every single version has the string "WinZip Self-Extractor", and every 3+ version has a "version=" string due to internally including an XML

            // "WinZip Self-Extractor"

            byte[] check = new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70, 0x20, 0x53, 0x65, 0x6C, 0x66, 0x2D, 0x45, 0x78, 0x74, 0x72, 0x61, 0x63, 0x74, 0x6F, 0x72 };
            if (fileContent.Contains(check, out int position))
            {
                // "<?xml"
                byte[] check2 = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C };
                if (fileContent.Contains(check2, out int xmlStartPosition))
                    return $"WinZip SFX {GetV3PlusVersion(fileContent, position, xmlStartPosition)}" + (includePosition ? $" (Index {position})" : string.Empty);
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
        // Most of this code is literally just lifted from PKZIP.cs, so make sure that it works efficiently for this use
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the zip file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Should be using stream instead of file, but stream fails to extract anything. My guess is that the executable portion of the archive is causing stream to fail, but not file.
                using (ZipArchive zipFile = ZipArchive.Open(file))
                {
                    foreach (var entry in zipFile.Entries)
                    {
                        // If an individual entry fails
                        try
                        {
                            // If we have a directory, skip it
                            if (entry.IsDirectory)
                                continue;

                            string tempFile = Path.Combine(tempPath, entry.Key);
                            entry.WriteToFile(tempFile);
                        }
                        catch { }
                    }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch { }

            return null;
        }

        private static string GetV3PlusVersion(byte[] fileContent, int position, int xmlStartPosition)
        {
            /* If false positives do end up being an issue, there's one more consistnet string that may be used:

               "_winzip_" (Found in almost every WinZip SFX archive, and every single 3+ one.)
               Hex: 0x5F, 0x77, 0x69, 0x6E, 0x7A, 0x69, 0x70, 0x5F */

            // Find the end of the XML so it can be imported and parsed
            // </assembly>
            byte[] check = new byte[] { 0x3C, 0x2F, 0x61, 0x73, 0x73, 0x65, 0x6D, 0x62, 0x6C, 0x79, 0x3E };
            if (fileContent.Contains(check, out int position2))
            {
                int offset = position2 + 11 - xmlStartPosition;
                string XML = Encoding.ASCII.GetString(fileContent, xmlStartPosition, offset);

                // Load and parse the XML to find the version string, and run the Version 2.X finder if the XML fails to parse.
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(XML);
                    string xmlVersion = xmlDoc["assembly"]["assemblyIdentity"].GetAttributeNode("version").InnerXml;

                    // 3.0.7158 is the only 3+ version that doesn't contain the "W.i.n.Z.i.p. .S.e.l.f.-.E.x.t.r.a.c.t.o.r." string.
                    if (xmlVersion == "3.0.7158.0")
                        return "3.0.7158";

                    // "W.i.n.Z.i.p. .S.e.l.f.-.E.x.t.r.a.c.t.o.r."
                    check = new byte[] { 0x57, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x5A, 0x00, 0x69, 0x00, 0x70, 0x00, 0x20, 0x00, 0x53, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x66, 0x00, 0x2D, 0x00, 0x45, 0x00, 0x78, 0x00, 0x74, 0x00, 0x72, 0x00, 0x61, 0x00, 0x63, 0x00,0x74, 0x00, 0x6F, 0x00, 0x72, 0x00 };
                    if (fileContent.Contains(check, out int position3))
                    {
                        // Some version strings don't exactly match the public version number
                        switch (xmlVersion)
                        {
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
                    return GetV2Version(fileContent, position);
                }
                catch
                {
                    return GetV2Version(fileContent, position);
                }
  
            }
            return GetV2Version(fileContent, position);
        }

        private static string GetV2Version(byte[] fileContent, int position)
        {
            return "Version 2.x";
        }
    }
}
