﻿using System;
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

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Detect(fs, file, includeDebug);
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
#if NET20 || NET35 || NET40
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

                // Channelware
                // Found in "README.TXT" in Redump entry 116358.
                if (fileContent.Contains("This application is a Channelware-activated product."))
                    protections.Add("Channelware");
                // Found in "Swr.dat" in the "TOYSTORY" installation folder from Redump entry 12354.
                if (fileContent.Contains("cwsw.com/authts"))
                    protections.Add("Channelware");

                // CopyKiller
                // Found in "autorun.dat" in CopyKiller versions 3.62 and 3.64.
                if (fileContent.Contains("CopyKiller CD-Protection V3.6x"))
                    protections.Add("CopyKiller V3.62-V3.64");
                // Found in "autorun.dat" in CopyKiller versions 3.99 and 3.99a.
                else if (fileContent.Contains("CopyKiller V4 CD / DVD-Protection"))
                    protections.Add("CopyKiller V3.99+");
                // Found in "engine.wzc" in CopyKiller versions 3.62 and 3.64.
                else if (fileContent.Contains("CopyKiller V3.6x Protection Engine"))
                    protections.Add("CopyKiller V3.62-V3.64");
                // Found in "engine.wzc" in CopyKiller versions 3.99 and 3.99a.
                else if (fileContent.Contains("CopyKiller V3.99x Protection Engine"))
                            protections.Add("CopyKiller V3.99+");

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
                // Found in Redump entry 84082.
                if (fileContent.Contains("phenoProtect"))
                    protections.Add("phenoProtect");
                // Additional check to minimize overmatching.
                if (fileContent.Contains("InstallSHIELD Software Coporation"))
                    // Found in Redump entry 102493.
                    if (fileContent.Contains("COPYPROTECTION_FAILEDR"))
                        protections.Add("phenoProtect");

                // Rainbow Sentinel
                // Found in "SENTW95.HLP" and "SENTINEL.HLP" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
                if (fileContent.Contains("Rainbow Sentinel Driver Help"))
                    protections.Add("Rainbow Sentinel");
                // Found in "\disc4\cad\sdcc_200.zip\DISK1\_USER1.HDR\Language_Independent_Intel_32_Files\SNTNLUSB.INF" in "CICA 32 For Windows CD-ROM (Walnut Creek) (October 1999) (Disc 4).iso" in IA item "CICA_32_For_Windows_CD-ROM_Walnut_Creek_October_1999".
                if (fileContent.Contains("SNTNLUSB.SvcDesc=\"Rainbow Security Device\""))
                    protections.Add("Rainbow Sentinel USB Driver");
                if (fileContent.Contains("SntUsb95.SvcDesc=\"Rainbow Security Device\""))
                    protections.Add("Rainbow Sentinel USB Driver");

                // Found in "OEMSETUP.INF" in BA entry "Autodesk AutoCAD LT 98 (1998) (CD) [English] [Dutch]".
                if (fileContent.Contains("Sentinel Driver Disk"))
                    protections.Add("Rainbow Sentinel");

                // SafeCast
                // Found in "AdlmLog.xml" in IA item game-programming-in-c-start-to-finish-2006 after installing "3dsMax8_Demo.zip".
                if (fileContent.Contains("<NAME>SAFECAST</NAME>"))
                    protections.Add("SafeCast");

                // SafeDisc 
                // TODO: Add better version parsing.
                // Found in "Info.plist" in Redump entries 23983, 42762, 72713, 73070, and 89603.
                if (fileContent.Contains("<string>com.europevisionmacro.SafeDiscDVD</string>"))
                {
                    if (fileContent.Contains("<string>2.90.032</string>"))
                        protections.Add("SafeDiscDVD for Macintosh 2.90.032");
                    else
                        protections.Add("SafeDiscDVD for Macintosh (Unknown Version - Please report to us on GitHub)");
                }

                // Found in "Info.plist" in Redump entry 89649.
                if (fileContent.Contains("<string>com.macrovisioneurope.SafeDiscLT</string>"))
                {
                    // TODO: Investigate why "CFBundleGetInfoString" and "CFBundleShortVersionString" say version 2.70.020, but "CFBundleVersion" says version 2.70.010.
                    if (fileContent.Contains("<string>2.70.020</string"))
                        protections.Add("SafeDiscLT for Macintosh 2.70.020");
                    else
                        protections.Add("SafeDiscLT for Macintosh (Unknown Version - Please report to us on GitHub)");
                }


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

            return string.Join(";", [.. protections]);
        }
    }
}
