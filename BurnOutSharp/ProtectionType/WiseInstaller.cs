﻿using System;
using System.Collections.Generic;
using System.IO;
using Wise = WiseUnpacker.WiseUnpacker;

namespace BurnOutSharp.ProtectionType
{
    public class WiseInstaller
    {
        public static List<string> CheckContents(string file, byte[] fileContent)
        {
            // "WiseMain"
            byte[] check = new byte[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E };
            if (fileContent.Contains(check, out int position))
            {
                List<string> protections = new List<string> { $"Wise Installation Wizard Module (Index {position})" };

                if (!File.Exists(file))
                    return protections;

                protections.AddRange(WiseInstaller.Scan(file));

                return protections;
            }

            return null;
        }

        public static List<string> Scan(string file)
        {
            List<string> protections = new List<string>();

            // If the installer file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                Wise unpacker = new Wise();
                unpacker.ExtractTo(file, tempPath);

                foreach (string tempFile in Directory.EnumerateFiles(tempPath, "*", SearchOption.AllDirectories))
                {
                    string protection = ProtectionFind.ScanContent(tempFile);

                    // If tempfile cleanup fails
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(protection))
                        protections.Add($"\r\n{tempFile.Substring(tempPath.Length)} - {protection}");
                }
            }
            catch { }

            return protections;
        }

    }
}
