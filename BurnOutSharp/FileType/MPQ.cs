﻿using System;
using System.Collections.Generic;
using System.IO;
using StormLibSharp;

namespace BurnOutSharp.FileType
{
    internal class MPQ
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x4d, 0x50, 0x51, 0x1a }))
                return true;

            return false;
        }

        // TODO: Add stream opening support
        public static List<string> Scan(string file)
        {
            List<string> protections = new List<string>();

            // If the mpq file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (MpqArchive mpqArchive = new MpqArchive(file, FileAccess.Read))
                {
                    string listfile = null;
                    MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");
                    bool canRead = listStream.CanRead;

                    using (StreamReader sr = new StreamReader(listStream))
                    {
                        listfile = sr.ReadToEnd();
                        Console.WriteLine(listfile);
                    }

                    string sub = string.Empty;
                    while ((sub = listfile) != null)
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempfile = Path.Combine(tempPath, sub);
                            Directory.CreateDirectory(Path.GetDirectoryName(tempfile));
                            mpqArchive.ExtractFile(sub, tempfile);
                            string protection = ProtectionFind.ScanContent(tempfile);

                            // If tempfile cleanup fails
                            try
                            {
                                File.Delete(tempfile);
                            }
                            catch { }

                            if (!string.IsNullOrEmpty(protection))
                                protections.Add($"\r\n{sub} - {protection}");
                        }
                        catch { }
                    }

                    // If temp directory cleanup fails
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch { }
                }
            }
            catch { }

            return protections;
        }
    }
}
