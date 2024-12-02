using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    public class BSP : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            try
            {
                // Create the wrapper
                var bsp = SabreTools.Serialization.Wrappers.BSP.Create(stream);
                if (bsp == null)
                    return false;

                // TODO: Introduce helper methods for all specialty lump types

                // Loop through and extract all files
                Directory.CreateDirectory(outDir);
                bsp.ExtractAllLumps(outDir);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }
    }
}
