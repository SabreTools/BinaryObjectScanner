using System;
using System.Collections.Generic;
using System.IO;
using HLExtract.Net;

namespace BurnOutSharp.FileType
{
    internal class Valve
    {
        public static bool ShouldScan(byte[] magic)
        {
            // GCF
            if (magic.StartsWith(new byte[] { 0x01, 0x00, 0x00, 0x00 }))
                return true;

            // PAK
            if (magic.StartsWith(new byte[] { 0x50, 0x41, 0x43, 0x4b }))
                return true;

            // SGA
            if (magic.StartsWith(new byte[] { 0x5f, 0x41, 0x52, 0x43, 0x48, 0x49, 0x56, 0x45 }))
                return true;

            // VPK
            if (magic.StartsWith(new byte[] { 0x55, 0xaa, 0x12, 0x34 }))
                return true;

            // WAD
            if (magic.StartsWith(new byte[] { 0x57, 0x41, 0x44, 0x33 }))
                return true;

            return false;
        }

        // TODO: Add stream opening support
        public static Dictionary<string, List<string>> Scan(Scanner parentScanner, string file)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            // Create a new scanner for the new temp path
            Scanner subScanner = new Scanner(parentScanner.FileProgress)
            {
                IncludePosition = parentScanner.IncludePosition,
                ScanAllFiles = parentScanner.ScanAllFiles,
                ScanArchives = parentScanner.ScanArchives,
            };

            string[] args = new string[]
            {
                "-p", file,
                "-x", "root",
                "-x", "'extract .'",
                "-x", "exit",
                "-d", tempPath,
            };

            HLExtractProgram.Process(args);

            // Collect and format all found protections
            var protections = subScanner.GetProtections(tempPath);

            // If temp directory cleanup fails
            try
            {
                Directory.Delete(tempPath, true);
            }
            catch { }

            return protections;
        }
    }
}
