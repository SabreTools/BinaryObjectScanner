using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using BurnOutSharp.Interfaces;
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Various generic textfile formats
    /// </summary>
    public class Textfile : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
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
                    AppendToDictionary(protections, file, "AegiSoft License Manager");

                // CD-Key
                if (fileContent.Contains("a valid serial number is required"))
                    AppendToDictionary(protections, file, "CD-Key / Serial");
                else if (fileContent.Contains("serial number is located"))
                    AppendToDictionary(protections, file, "CD-Key / Serial");
                // Found in "Setup.Ins" ("Word Point 2002" in IA item "advantage-language-french-beginner-langmaster-2005").
                else if (fileContent.Contains("Please enter a valid registration number"))
                    AppendToDictionary(protections, file, "CD-Key / Serial");

                // Freelock
                // Found in "FILE_ID.DIZ" distributed with Freelock.
                if (fileContent.Contains("FREELOCK 1.0"))
                    AppendToDictionary(protections, file, "Freelock 1.0");
                else if (fileContent.Contains("FREELOCK 1.2"))
                    AppendToDictionary(protections, file, "Freelock 1.2");
                else if (fileContent.Contains("FREELOCK 1.2a"))
                    AppendToDictionary(protections, file, "Freelock 1.2a");
                else if (fileContent.Contains("FREELOCK 1.3"))
                    AppendToDictionary(protections, file, "Freelock 1.3");
                else if (fileContent.Contains("FREELOCK"))
                    AppendToDictionary(protections, file, "Freelock");

                // MediaCloQ
                if (fileContent.Contains("SunnComm MediaCloQ"))
                    AppendToDictionary(protections, file, "MediaCloQ");
                else if (fileContent.Contains("http://download.mediacloq.com/"))
                    AppendToDictionary(protections, file, "MediaCloQ");
                else if (fileContent.Contains("http://www.sunncomm.com/mediacloq/"))
                    AppendToDictionary(protections, file, "MediaCloQ");

                // MediaMax
                if (fileContent.Contains("MediaMax technology"))
                    AppendToDictionary(protections, file, "MediaMax CD-3");
                else if (fileContent.Contains("exclusive Cd3 technology"))
                    AppendToDictionary(protections, file, "MediaMax CD-3");
                else if (fileContent.Contains("<PROTECTION-VENDOR>MediaMAX</PROTECTION-VENDOR>"))
                    AppendToDictionary(protections, file, "MediaMax CD-3");
                else if (fileContent.Contains("MediaMax(tm)"))
                    AppendToDictionary(protections, file, "MediaMax CD-3");

                // phenoProtect
                if (fileContent.Contains("phenoProtect"))
                    AppendToDictionary(protections, file, "phenoProtect");

                // Rainbow Sentinel
                // Found in "SENTW95.HLP" and "SENTINEL.HLP" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
                if (fileContent.Contains("Rainbow Sentinel Driver Help"))
                    AppendToDictionary(protections, file, "Rainbow Sentinel");

                // Found in "OEMSETUP.INF" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
                if (fileContent.Contains("Sentinel Driver Disk"))
                    AppendToDictionary(protections, file, "Rainbow Sentinel");

                // The full line from a sample is as follows:
                //
                // The files securom_v7_01.dat and securom_v7_01.bak have been created during the installation of a SecuROM protected application.
                //
                // TODO: Use the filenames in this line to get the version out of it

                // SecuROM
                if (fileContent.Contains("SecuROM protected application"))
                    AppendToDictionary(protections, file, "SecuROM");

                // Steam
                if (fileContent.Contains("All use of the Program is governed by the terms of the Steam Agreement as described below."))
                    AppendToDictionary(protections, file, "Steam");

                // XCP
                if (fileContent.Contains("http://cp.sonybmg.com/xcp/"))
                    AppendToDictionary(protections, file, "XCP");
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return protections;
        }
    }
}
