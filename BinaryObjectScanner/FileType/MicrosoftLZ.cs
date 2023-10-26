using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Compression.LZ;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Microsoft LZ-compressed Files (LZ32)
    /// </summary>
    /// <remarks>This is treated like an archive type due to the packing style</remarks>
    public class MicrosoftLZ : IExtractable
    {
        /// <inheritdoc/>
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream? stream, string file, bool includeDebug)
#endif
        {
            try
            {
                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                var data = Decompressor.Decompress(stream);
                if (data == null)
                    return null;

                // Create the temp filename
                string tempFile = "temp.bin";
                if (!string.IsNullOrEmpty(file))
                {
                    var expandedFilePath = Decompressor.GetExpandedName(file, out _);
                    if (expandedFilePath != null)
                        tempFile = Path.GetFileName(expandedFilePath).TrimEnd('\0');
                    if (tempFile.EndsWith(".ex"))
                        tempFile += "e";
                    else if (tempFile.EndsWith(".dl"))
                        tempFile += "l";
                }

                tempFile = Path.Combine(tempPath, tempFile);

                // Write the file data to a temp file
                using (Stream tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    tempStream.Write(data, 0, data.Length);
                }

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
