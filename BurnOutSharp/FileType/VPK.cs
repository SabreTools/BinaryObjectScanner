using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Valve Package File
    /// </summary>
    public class VPK : IExtractable
    {
        /// <inheritdoc/>
        public string Extract(string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file)
        {
            // Create the wrapper
            BinaryObjectScanner.Wrappers.VPK vpk = BinaryObjectScanner.Wrappers.VPK.Create(stream);
            if (vpk == null)
                return null;

            // Create a temp output directory
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            // Loop through and extract all files
            vpk.ExtractAll(tempPath);

            return tempPath;
        }
    }
}
