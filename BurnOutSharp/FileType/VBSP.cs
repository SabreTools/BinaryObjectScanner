using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Half-Life 2 Level
    /// </summary>
    public class VBSP : IExtractable
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
                BinaryObjectScanner.Wrappers.VBSP vbsp = BinaryObjectScanner.Wrappers.VBSP.Create(stream);
                if (vbsp == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Loop through and extract all files
                vbsp.ExtractAllLumps(tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
