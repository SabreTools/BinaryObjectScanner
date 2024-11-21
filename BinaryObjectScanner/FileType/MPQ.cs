using System.IO;
using BinaryObjectScanner.Interfaces;
#if (NET452_OR_GREATER || NETCOREAPP) && WIN
using StormLibSharp;
#endif

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// MoPaQ game data archive
    /// </summary>
    public class MPQ : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        // TODO: Add stream opening support
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
                // Try to open the archive and listfile
                var mpqArchive = new MpqArchive(file, FileAccess.Read);
                string? listfile = null;
                MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");

                // If we can't read the listfile, we just return
                if (!listStream.CanRead)
                    return false;

                // Read the listfile in for processing
                using (var sr = new StreamReader(listStream))
                {
                    listfile = sr.ReadToEnd();
                }

                // Split the listfile by newlines
                string[] listfileLines = listfile.Replace("\r\n", "\n").Split('\n');

                // Loop over each entry
                foreach (string sub in listfileLines)
                {
                    try
                    {
                        string tempFile = Path.Combine(outDir, sub);
                        var directoryName = Path.GetDirectoryName(tempFile);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        mpqArchive.ExtractFile(sub, tempFile);
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
