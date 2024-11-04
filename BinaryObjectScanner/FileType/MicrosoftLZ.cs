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
                var data = Decompressor.Decompress(stream);
                if (data == null)
                    return false;

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

                tempFile = Path.Combine(outDir, tempFile);
                var directoryName = Path.GetDirectoryName(tempFile);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                using Stream tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                tempStream.Write(data, 0, data.Length);

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
