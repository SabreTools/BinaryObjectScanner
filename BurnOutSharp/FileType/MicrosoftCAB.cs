using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Microsoft cabinet file
    /// </summary>
    /// <remarks>Specification available at <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/></remarks>
    /// <see href="https://github.com/wine-mirror/wine/tree/master/dlls/cabinet"/>
    public class MicrosoftCAB : IExtractable
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
                // Open the cab file
                var cabFile = MicrosoftCabinet.Create(stream);
                if (cabFile == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // If entry extraction fails
                bool success = cabFile.ExtractAll(tempPath);
                if (!success)
                    return null;

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }
    }
}
