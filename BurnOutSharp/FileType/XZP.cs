using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// XBox Package File
    /// </summary>
    public class XZP : IExtractable
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
            BinaryObjectScanner.Wrappers.XZP xzp = BinaryObjectScanner.Wrappers.XZP.Create(stream);
            if (xzp == null)
                return null;

            // Create a temp output directory
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            // Loop through and extract all files
            xzp.ExtractAll(tempPath);

            return tempPath;
        }
    }
}
