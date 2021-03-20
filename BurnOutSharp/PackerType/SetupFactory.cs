using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace BurnOutSharp.PackerType
{
    public class SetupFactory : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            /* Longer version of the check that can be used if false positves become an issue:
               "S.e.t.u.p. .F.a.c.t.o.r.y. .i.s. .a. .t.r.a.d.e.m.a.r.k. .o.f. .I.n.d.i.g.o. .R.o.s.e. .C.o.r.p.o.r.a.t.i.o.n"
            byte[] check = new byte[] { 0x53, 0x00, 0x65, 0x00, 0x74, 0x00, 0x75, 0x00, 0x70, 0x00, 0x20, 0x00, 0x46, 0x00, 0x61, 0x00, 0x63, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x79, 0x00, 0x20, 0x00, 0x69, 0x00, 0x73, 0x00, 0x20, 0x00, 0x61, 0x00, 0x20, 0x00, 0x74, 0x00, 0x72, 0x00, 0x61, 0x00, 0x64, 0x00, 0x65, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x6B, 0x00, 0x20, 0x00, 0x6F, 0x00, 0x66, 0x00, 0x20, 0x00, 0x49, 0x00, 0x6E, 0x00, 0x64, 0x00, 0x69, 0x00, 0x67, 0x00, 0x6F, 0x00, 0x20, 0x00, 0x52, 0x00, 0x6F, 0x00, 0x73, 0x00, 0x65, 0x00, 0x20, 0x00, 0x43, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x70, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E }; */
            // "S.e.t.u.p. .F.a.c.t.o.r.y."
            byte[] check = new byte[] { 0x53, 0x00, 0x65, 0x00, 0x74, 0x00, 0x75, 0x00, 0x70, 0x00, 0x20, 0x00, 0x46, 0x00, 0x61, 0x00, 0x63, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x79, 0x00 };
            if (fileContent.Contains(check, out int position))
            {
                // "<?xml"
                byte[] check2 = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C };
                if (fileContent.Contains(check2, out int position2))
                {
                    string version = GetVersionXML(fileContent, position2);
                    if (version == null)
                        return $"Setup Factory (Unknown Version)" + (includePosition ? $" (Index {position})" : string.Empty);
                    
                    return $"Setup Factory {version}" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                }
                else
                {
                    string version = Utilities.GetFileVersion(file);
                    if (version == null)
                        return $"Setup Factory (Unknown Version)" + (includePosition ? $" (Index {position})" : string.Empty);
                    
                    return $"Setup Factory {version}" + (includePosition ? $" (Index {position})" : string.Empty);
                }
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
        // Add extraction, which is possible but the only tools available that can do this seem to be Universal Extractor 2 and InstallExplorer (https://totalcmd.net/plugring/InstallExplorer.html)
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }

        // I was only able to test version detection with version 9.1.0.0 and version 5.0.1.0, but any other versions that use XML or the "File Version" field to store the version number should be detected as well.
        // Version 5.0.1.0 contains the string "Setup Factory 32-Bit Setup Module 5.00", which adds confusion about the version but may be used as an additional check
        private static string GetVersionXML(byte[] fileContent, int xmlStartPosition)
        {

            // </assembly>
            byte[] check = new byte[] { 0x3C, 0x2F, 0x61, 0x73, 0x73, 0x65, 0x6D, 0x62, 0x6C, 0x79, 0x3E };
            if (fileContent.Contains(check, out int position, start: xmlStartPosition))
            {
                int offset = position + 11 - xmlStartPosition;
                string xmlString = Encoding.ASCII.GetString(fileContent, xmlStartPosition, offset);

                try
                {
                    // Load the XML string as a document
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlString);

                    // Get the version attribute, if possible
                    string xmlVersion = xmlDoc["assembly"]["assemblyIdentity"].GetAttributeNode("version").InnerXml;

                    return $"Version {xmlVersion}";
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}
