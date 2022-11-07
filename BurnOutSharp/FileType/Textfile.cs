using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;

namespace BurnOutSharp.FileType
{
    public class Textfile : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            return ShouldScan(magic, null);
        }

        /// <summary>
        /// Determine if a file signature or extension matches one of the expected values
        /// </summary>
        /// <param name="magic">Byte array representing the file header</param>
        /// <param name="extension">Extension for the file being checked</param>
        /// <returns>True if the signature is valid, false otherwise</returns>
        public bool ShouldScan(byte[] magic, string extension)
        {
            // Rich Text File
            if (magic.StartsWith(new byte?[] { 0x7b, 0x5c, 0x72, 0x74, 0x66, 0x31 }))
                return true;

            // HTML
            if (magic.StartsWith(new byte?[] { 0x3c, 0x68, 0x74, 0x6d, 0x6c }))
                return true;

            // HTML and XML
            if (magic.StartsWith(new byte?[] { 0x3c, 0x21, 0x44, 0x4f, 0x43, 0x54, 0x59, 0x50, 0x45 }))
                return true;

            // Microsoft Office File (old)
            if (magic.StartsWith(new byte?[] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 }))
                return true;

            // InstallShield Compiled Rules
            if (magic.StartsWith(new byte?[] { 0x61, 0x4C, 0x75, 0x5A }))
                return true;

            // Windows Help File
            if (magic.StartsWith(new byte?[] { 0x3F, 0x5F, 0x03, 0x00 }))
                return true;

            // "Description in Zip"
            if (string.Equals(extension?.TrimStart('.'), "diz", StringComparison.OrdinalIgnoreCase))
                return true;

            // Setup information
            if (string.Equals(extension?.TrimStart('.'), "inf", StringComparison.OrdinalIgnoreCase))
                return true;

            // InstallShield Script
            if (string.Equals(extension?.TrimStart('.'), "ins", StringComparison.OrdinalIgnoreCase))
                return true;

            // Generic textfile (no header)
            if (string.Equals(extension?.TrimStart('.'), "txt", StringComparison.OrdinalIgnoreCase))
                return true;

            // XML (multiple headers possible)
            if (string.Equals(extension?.TrimStart('.'), "xml", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // Files can be protected in multiple ways
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            try
            {
                // Load the current file content
                string fileContent = null;
                using (var sr = new StreamReader(stream, Encoding.Default, true, 1024 * 1024, true))
                {
                    fileContent = sr.ReadToEnd();
                }

                // AegiSoft License Manager
                // Found in "setup.ins" (Redump entry 73521/IA item "Nova_HoyleCasino99USA").
                if (fileContent.Contains("Failed to load the AegiSoft License Manager install program."))
                    Utilities.AppendToDictionary(protections, file, "AegiSoft License Manager");

                // CD-Key
                if (fileContent.Contains("a valid serial number is required"))
                    Utilities.AppendToDictionary(protections, file, "CD-Key / Serial");
                else if (fileContent.Contains("serial number is located"))
                    Utilities.AppendToDictionary(protections, file, "CD-Key / Serial");

                // Freelock
                // Found in "FILE_ID.DIZ" distributed with Freelock.
                if (fileContent.Contains("FREELOCK 1.0"))
                    Utilities.AppendToDictionary(protections, file, "Freelock 1.0");
                else if (fileContent.Contains("FREELOCK 1.2"))
                    Utilities.AppendToDictionary(protections, file, "Freelock 1.2");
                else if (fileContent.Contains("FREELOCK 1.2a"))
                    Utilities.AppendToDictionary(protections, file, "Freelock 1.2a");
                else if (fileContent.Contains("FREELOCK 1.3"))
                    Utilities.AppendToDictionary(protections, file, "Freelock 1.3");
                else if (fileContent.Contains("FREELOCK"))
                    Utilities.AppendToDictionary(protections, file, "Freelock");

                // MediaCloQ
                if (fileContent.Contains("SunnComm MediaCloQ"))
                    Utilities.AppendToDictionary(protections, file, "MediaCloQ");
                else if (fileContent.Contains("http://download.mediacloq.com/"))
                    Utilities.AppendToDictionary(protections, file, "MediaCloQ");
                else if (fileContent.Contains("http://www.sunncomm.com/mediacloq/"))
                    Utilities.AppendToDictionary(protections, file, "MediaCloQ");

                // MediaMax
                if (fileContent.Contains("MediaMax technology"))
                    Utilities.AppendToDictionary(protections, file, "MediaMax CD-3");
                else if (fileContent.Contains("exclusive Cd3 technology"))
                    Utilities.AppendToDictionary(protections, file, "MediaMax CD-3");
                else if (fileContent.Contains("<PROTECTION-VENDOR>MediaMAX</PROTECTION-VENDOR>"))
                    Utilities.AppendToDictionary(protections, file, "MediaMax CD-3");
                else if (fileContent.Contains("MediaMax(tm)"))
                    Utilities.AppendToDictionary(protections, file, "MediaMax CD-3");

                // phenoProtect
                if (fileContent.Contains("phenoProtect"))
                    Utilities.AppendToDictionary(protections, file, "phenoProtect");

                // Rainbow Sentinel
                // Found in "SENTW95.HLP" and "SENTINEL.HLP" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
                if (fileContent.Contains("Rainbow Sentinel Driver Help"))
                    Utilities.AppendToDictionary(protections, file, "Rainbow Sentinel");

                // Found in "OEMSETUP.INF" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
                if (fileContent.Contains("Sentinel Driver Disk"))
                    Utilities.AppendToDictionary(protections, file, "Rainbow Sentinel");

                // The full line from a sample is as follows:
                //
                // The files securom_v7_01.dat and securom_v7_01.bak have been created during the installation of a SecuROM protected application.
                //
                // TODO: Use the filenames in this line to get the version out of it

                // SecuROM
                if (fileContent.Contains("SecuROM protected application"))
                    Utilities.AppendToDictionary(protections, file, "SecuROM");

                // Steam
                if (fileContent.Contains("All use of the Program is governed by the terms of the Steam Agreement as described below."))
                    Utilities.AppendToDictionary(protections, file, "Steam");

                // XCP
                if (fileContent.Contains("http://cp.sonybmg.com/xcp/"))
                    Utilities.AppendToDictionary(protections, file, "XCP");
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return protections;
        }
    }
}
