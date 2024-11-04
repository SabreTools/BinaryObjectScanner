using System.IO;
using BinaryObjectScanner.Interfaces;
#if (NET40_OR_GREATER || NETCOREAPP) && WIN
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
#if NET20 || NET35 || !WIN
            // Not supported for old .NET due to feature requirements
            // Not supported in non-Windows builds due to DLL requirements
            return false;
#else
            try
            {
                // Loop over each entry
                var cabArchive = new MSCabinet(file);
                foreach (var compressedFile in cabArchive.GetFiles())
                {
                    try
                    {
                        string tempFile = Path.Combine(tempPath, compressedFile.Filename);
                        var directoryName = Path.GetDirectoryName(tempFile);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        compressedFile.ExtractTo(tempFile);
                    }
                    catch (System.Exception ex)
                    {
                        if (includeDebug) System.Console.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                if (includeDebug) System.Console.WriteLine(ex);
                return false;
            }
#endif
        }
    }
}
