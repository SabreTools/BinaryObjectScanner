﻿using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Compressors.Xz;

namespace BurnOutSharp.FileType
{
    internal class XZ
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00 }))
                return true;

            return false;
        }

        public static List<string> Scan(Stream stream)
        {
            List<string> protections = new List<string>();

            // If the 7-zip file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (XZStream xzFile = new XZStream(stream))
                {
                    // If an individual entry fails
                    try
                    {
                        string tempfile = Path.Combine(tempPath, Guid.NewGuid().ToString());
                        using (FileStream fs = File.OpenWrite(tempfile))
                        {
                            xzFile.CopyTo(fs);
                        }

                        string protection = ProtectionFind.ScanContent(tempfile);

                        // If tempfile cleanup fails
                        try
                        {
                            File.Delete(tempfile);
                        }
                        catch { }

                        if (!string.IsNullOrEmpty(protection))
                            protections.Add($"\r\n{protection}");
                    }
                    catch { }

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
