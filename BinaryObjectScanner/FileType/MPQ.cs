using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
#if NETFRAMEWORK && !NET20 && !NET35 && !NET40
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
        public string? Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, includeDebug);
        }

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public string? Extract(Stream? stream, string file, bool includeDebug)
        {
#if NET20 || NET35 || NET40 || NETCOREAPP || NET5_0_OR_GREATER
            // Not supported for .NET Core and modern .NET due to Windows DLL requirements
            return null;
#else
            try
            {
                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (var mpqArchive = new MpqArchive(file, FileAccess.Read))
                {
                    // Try to open the listfile
                    string? listfile = null;
                    MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");

                    // If we can't read the listfile, we just return
                    if (!listStream.CanRead)
                        return null;

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
                            string tempFile = Path.Combine(tempPath, sub);
                            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                            mpqArchive.ExtractFile(sub, tempFile);
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
