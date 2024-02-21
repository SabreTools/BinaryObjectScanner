using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
#if ((NETFRAMEWORK && !NET20 && !NET35 && !NET40) || NETCOREAPP) && WIN
using LibMSPackN;
#endif

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Microsoft cabinet file
    /// </summary>
    /// <remarks>Specification available at <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/></remarks>
    /// <see href="https://github.com/wine-mirror/wine/tree/master/dlls/cabinet"/>
    public class MicrosoftCAB : IExtractable
    {
        /// <inheritdoc/>
        public string? Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, includeDebug);
        }

        /// <inheritdoc/>
        public string? Extract(Stream? stream, string file, bool includeDebug)
        {
#if NET20 || NET35 || NET40 || !WIN
            // Not supported for old .NET due to feature requirements
            // Not supported in non-Windows builds due to DLL requirements
            return null;
#else
            try
            {
                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (var cabArchive = new MSCabinet(file))
                {
                    // Loop over each entry
                    foreach (var compressedFile in cabArchive.GetFiles())
                    {
                        try
                        {
                            string tempFile = Path.Combine(tempPath, compressedFile.Filename);
                            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                            compressedFile.ExtractTo(tempFile);
                        }
                        catch (Exception ex)
                        {
                            if (includeDebug) Console.WriteLine(ex);
                        }
                    }
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
#endif
        }
    }
}
