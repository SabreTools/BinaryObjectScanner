using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Compression.zlib;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// SGA game archive
    /// </summary>
    public class SGA : IExtractable
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
                // Create the wrapper
                var sga = SabreTools.Serialization.Wrappers.SGA.Create(stream);
                if (sga == null)
                    return false;

                // Loop through and extract all files
                Directory.CreateDirectory(outDir);
                ExtractAll(sga, outDir);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract all files from the SGA to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(SabreTools.Serialization.Wrappers.SGA item, string outputDirectory)
        {
            // Get the file count
            int filesLength = item.Model.Directory switch
            {
                SabreTools.Models.SGA.Directory4 d4 => filesLength = d4.Files?.Length ?? 0,
                SabreTools.Models.SGA.Directory5 d5 => filesLength = d5.Files?.Length ?? 0,
                SabreTools.Models.SGA.Directory6 d6 => filesLength = d6.Files?.Length ?? 0,
                SabreTools.Models.SGA.Directory7 d7 => filesLength = d7.Files?.Length ?? 0,
                _ => 0,
            };

            // If we have no files
            if (filesLength == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < filesLength; i++)
            {
                allExtracted &= ExtractFile(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the SGA to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(SabreTools.Serialization.Wrappers.SGA item, int index, string outputDirectory)
        {
            // Get the file count
            int filesLength = item.Model.Directory switch
            {
                SabreTools.Models.SGA.Directory4 d4 => filesLength = d4.Files?.Length ?? 0,
                SabreTools.Models.SGA.Directory5 d5 => filesLength = d5.Files?.Length ?? 0,
                SabreTools.Models.SGA.Directory6 d6 => filesLength = d6.Files?.Length ?? 0,
                SabreTools.Models.SGA.Directory7 d7 => filesLength = d7.Files?.Length ?? 0,
                _ => 0,
            };

            // If we have no files
            if (filesLength == 0)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= filesLength)
                return false;

            // Get the files
            object? file = item.Model.Directory switch
            {
                SabreTools.Models.SGA.Directory4 d4 => d4.Files![index],
                SabreTools.Models.SGA.Directory5 d5 => d5.Files![index],
                SabreTools.Models.SGA.Directory6 d6 => d6.Files![index],
                SabreTools.Models.SGA.Directory7 d7 => d7.Files![index],
                _ => null,
            };

            // If the file is invalid
            if (file == null)
                return false;

            // Create the filename
            var filename = file switch
            {
                SabreTools.Models.SGA.File4 f4 => f4.Name,
                _ => null,
            };

            // If the filename is invalid
            if (filename == null)
                return false;

            // Loop through and get all parent directories
            var parentNames = new List<string> { filename };

            // Get the parent directory
            var folder = item.Model.Directory switch
            {
                SabreTools.Models.SGA.Directory4 d4 => Array.Find(d4.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                SabreTools.Models.SGA.Directory5 d5 => Array.Find(d5.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                SabreTools.Models.SGA.Directory6 d6 => Array.Find(d6.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                SabreTools.Models.SGA.Directory7 d7 => Array.Find(d7.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                _ => default(object),
            };

            // If we have a parent folder
            if (folder != null)
            {
                string folderName = folder switch
                {
                    SabreTools.Models.SGA.Folder4 f4 => f4.Name ?? string.Empty,
                    SabreTools.Models.SGA.Folder5 f5 => f5.Name ?? string.Empty,
                    _ => string.Empty,
                };
                parentNames.Add(folderName);
            }

            // TODO: Should the section name/alias be used in the path as well?

            // Reverse and assemble the filename
            parentNames.Reverse();
#if NET20 || NET35
            filename = parentNames[0];
            for (int i = 1; i < parentNames.Count; i++)
            {
                filename = Path.Combine(filename, parentNames[i]);
            }
#else
            filename = Path.Combine([.. parentNames]);
#endif

            // Get the file offset
            long fileOffset = file switch
            {
                SabreTools.Models.SGA.File4 f4 => f4.Offset,
                _ => -1,
            };

            // Adjust the file offset
            fileOffset += item.Model.Header switch
            {
                SabreTools.Models.SGA.Header4 h4 => h4.FileDataOffset,
                SabreTools.Models.SGA.Header6 h6 => h6.FileDataOffset,
                _ => -1,
            };

            // If the offset is invalid
            if (fileOffset < 0)
                return false;

            // Get the file sizes
            long fileSize, outputFileSize;
            switch (file)
            {
                case SabreTools.Models.SGA.File4 f4:
                    fileSize = f4.SizeOnDisk;
                    outputFileSize = f4.Size;
                    break;

                default:
                    return false;
            }

            // Read the compressed data directly
            var compressedData = item.ReadFromDataSource((int)fileOffset, (int)fileSize);
            if (compressedData == null)
                return false;

            // If the compressed and uncompressed sizes match
            byte[] data;
            if (fileSize == outputFileSize)
            {
                data = compressedData;
            }
            else
            {
                // Inflate the data into the buffer
                var zstream = new ZLib.z_stream_s();
                data = new byte[outputFileSize];
                unsafe
                {
                    fixed (byte* payloadPtr = compressedData)
                    fixed (byte* dataPtr = data)
                    {
                        zstream.next_in = payloadPtr;
                        zstream.avail_in = (uint)compressedData.Length;
                        zstream.total_in = (uint)compressedData.Length;
                        zstream.next_out = dataPtr;
                        zstream.avail_out = (uint)data.Length;
                        zstream.total_out = 0;

                        ZLib.inflateInit_(zstream, ZLib.zlibVersion(), compressedData.Length);
                        int zret = ZLib.inflate(zstream, 1);
                        ZLib.inflateEnd(zstream);
                    }
                }
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
