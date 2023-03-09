using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
#if NET48
using StormLibSharp;
#endif

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// MoPaQ game data archive
    /// </summary>
    public class MPQ : IExtractable
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

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public string Extract(Stream stream, string file)
        {
#if NET6_0_OR_GREATER
            // Not supported for .NET 6.0 due to Windows DLL requirements
            return null;
#else
            // Create a temp output directory
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            using (MpqArchive mpqArchive = new MpqArchive(file, FileAccess.Read))
            {
                // Try to open the listfile
                string listfile = null;
                MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");

                // If we can't read the listfile, we just return
                if (!listStream.CanRead)
                    return null;

                // Read the listfile in for processing
                using (StreamReader sr = new StreamReader(listStream))
                {
                    listfile = sr.ReadToEnd();
                }

                // Split the listfile by newlines
                string[] listfileLines = listfile.Replace("\r\n", "\n").Split('\n');

                // Loop over each entry
                foreach (string sub in listfileLines)
                {
                    string tempFile = Path.Combine(tempPath, sub);
                    Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                    mpqArchive.ExtractFile(sub, tempFile);
                }
            }

            return tempPath;
#endif
        }
    }
}
