using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// NovaLogic Game Archive Format
    /// </summary>
    public class PFF : IExtractable
    {
        /// <inheritdoc/>
        public string Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file, bool includeDebug)
        {
            try
            {
                // Create the wrapper
                BinaryObjectScanner.Wrappers.PFF pff = BinaryObjectScanner.Wrappers.PFF.Create(stream);
                if (pff == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Extract all files
                pff.ExtractAll(tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
