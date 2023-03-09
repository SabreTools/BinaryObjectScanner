using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    public class BSP : IExtractable
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
            BinaryObjectScanner.Wrappers.BSP bsp = BinaryObjectScanner.Wrappers.BSP.Create(stream);
            if (bsp == null)
                return null;

            // Create a temp output directory
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            // Loop through and extract all files
            bsp.ExtractAllLumps(tempPath);
            bsp.ExtractAllTextures(tempPath);

            return tempPath;
        }
    }
}
