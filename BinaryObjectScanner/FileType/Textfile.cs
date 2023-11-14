using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Various generic textfile formats
    /// </summary>
    public class Textfile : IDetectable
    {
        /// <inheritdoc/>
        public string? Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Detect(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Files can be protected in multiple ways
            var protections = new List<string>();

            try
            {
                // Load the current file content
                var fileContent = string.Empty;
#if NET40
                using (var sr = new StreamReader(stream, Encoding.Default, true, 1024 * 1024))
#else
                using (var sr = new StreamReader(stream, Encoding.Default, true, 1024 * 1024, true))
#endif
                {
                    fileContent = sr.ReadToEnd();
                }

                // AegiSoft License Manager
                // Found in "setup.ins" (Redump entry 73521/IA item "Nova_HoyleCasino99USA").
                if (fileContent.Contains("Failed to load the AegiSoft License Manager install program."))
                    protections.Add("AegiSoft License Manager");

                // CD-Key
                if (fileContent.Contains("a valid serial number is required"))
                    protections.Add("CD-Key / Serial");
                else if (fileContent.Contains("serial number is located"))
                    protections.Add("CD-Key / Serial");
                // Found in "Setup.Ins" ("Word Point 2002" in IA item "advantage-language-french-beginner-langmaster-2005").
                else if (fileContent.Contains("Please enter a valid registration number"))
                    protections.Add("CD-Key / Serial");

                // Freelock
                // Found in "FILE_ID.DIZ" distributed with Freelock.
                if (fileContent.Contains("FREELOCK 1.0"))
                    protections.Add("Freelock 1.0");
                else if (fileContent.Contains("FREELOCK 1.2"))
                    protections.Add("Freelock 1.2");
                else if (fileContent.Contains("FREELOCK 1.2a"))
                    protections.Add("Freelock 1.2a");
                else if (fileContent.Contains("FREELOCK 1.3"))
                    protections.Add("Freelock 1.3");
                else if (fileContent.Contains("FREELOCK"))
                    protections.Add("Freelock");

                // MediaCloQ
                if (fileContent.Contains("SunnComm MediaCloQ"))
                    protections.Add("MediaCloQ");
                else if (fileContent.Contains("http://download.mediacloq.com/"))
                    protections.Add("MediaCloQ");
                else if (fileContent.Contains("http://www.sunncomm.com/mediacloq/"))
                    protections.Add("MediaCloQ");

                // MediaMax
                if (fileContent.Contains("MediaMax technology"))
                    protections.Add("MediaMax CD-3");
                else if (fileContent.Contains("exclusive Cd3 technology"))
                    protections.Add("MediaMax CD-3");
                else if (fileContent.Contains("<PROTECTION-VENDOR>MediaMAX</PROTECTION-VENDOR>"))
                    protections.Add("MediaMax CD-3");
                else if (fileContent.Contains("MediaMax(tm)"))
                    protections.Add("MediaMax CD-3");

                // phenoProtect
                if (fileContent.Contains("phenoProtect"))
                    protections.Add("phenoProtect");

                // Rainbow Sentinel
                // Found in "SENTW95.HLP" and "SENTINEL.HLP" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
                if (fileContent.Contains("Rainbow Sentinel Driver Help"))
                    protections.Add("Rainbow Sentinel");

                // Found in "OEMSETUP.INF" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
                if (fileContent.Contains("Sentinel Driver Disk"))
                    protections.Add("Rainbow Sentinel");

                // The full line from a sample is as follows:
                //
                // The files securom_v7_01.dat and securom_v7_01.bak have been created during the installation of a SecuROM protected application.
                //
                // TODO: Use the filenames in this line to get the version out of it

                // SecuROM
                if (fileContent.Contains("SecuROM protected application"))
                    protections.Add("SecuROM");

                // Steam
                if (fileContent.Contains("All use of the Program is governed by the terms of the Steam Agreement as described below."))
                    protections.Add("Steam");

                // XCP
                if (fileContent.Contains("http://cp.sonybmg.com/xcp/"))
                    protections.Add("XCP");
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }

            return string.Join(";", protections);
        }
    }
}
