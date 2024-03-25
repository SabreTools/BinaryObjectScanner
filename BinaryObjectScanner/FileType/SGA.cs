using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            try
            {
                // Create the wrapper
                var sga = SabreTools.Serialization.Wrappers.SGA.Create(stream);
                if (sga == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Loop through and extract all files
                ExtractAll(sga, tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Extract all files from the SGA to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(SabreTools.Serialization.Wrappers.SGA item, string outputDirectory)
        {
            // Get the number of files
            int filesLength;
            switch (item.Model.Header?.MajorVersion)
            {
                case 4: filesLength = (item.Model.Directory as SabreTools.Models.SGA.Directory4)?.Files?.Length ?? 0; break;
                case 5: filesLength = (item.Model.Directory as SabreTools.Models.SGA.Directory5)?.Files?.Length ?? 0; break;
                case 6: filesLength = (item.Model.Directory as SabreTools.Models.SGA.Directory6)?.Files?.Length ?? 0; break;
                case 7: filesLength = (item.Model.Directory as SabreTools.Models.SGA.Directory7)?.Files?.Length ?? 0; break;
                default: return false;
            }

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
            // Get the number of files
            int filesLength;
            switch (item.Model.Header?.MajorVersion)
            {
                case 4: filesLength = (item.Model.Directory as SabreTools.Models.SGA.Directory4)?.Files?.Length ?? 0; break;
                case 5: filesLength = (item.Model.Directory as SabreTools.Models.SGA.Directory5)?.Files?.Length ?? 0; break;
                case 6: filesLength = (item.Model.Directory as SabreTools.Models.SGA.Directory6)?.Files?.Length ?? 0; break;
                case 7: filesLength = (item.Model.Directory as SabreTools.Models.SGA.Directory7)?.Files?.Length ?? 0; break;
                default: return false;
            }

            // If we have no files
            if (filesLength == 0)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= filesLength)
                return false;

            // Get the files
            object? file;
            switch (item.Model.Header?.MajorVersion)
            {
                case 4: file = (item.Model.Directory as SabreTools.Models.SGA.Directory4)?.Files?[index]; break;
                case 5: file = (item.Model.Directory as SabreTools.Models.SGA.Directory5)?.Files?[index]; break;
                case 6: file = (item.Model.Directory as SabreTools.Models.SGA.Directory6)?.Files?[index]; break;
                case 7: file = (item.Model.Directory as SabreTools.Models.SGA.Directory7)?.Files?[index]; break;
                default: return false;
            }

            if (file == null)
                return false;

            // Create the filename
            var filename = string.Empty;
            switch (item.Model.Header?.MajorVersion)
            {
                case 4:
                case 5: filename = (file as SabreTools.Models.SGA.File4)?.Name; break;
                case 6: filename = (file as SabreTools.Models.SGA.File6)?.Name; break;
                case 7: filename = (file as SabreTools.Models.SGA.File7)?.Name; break;
                default: return false;
            }

            // Loop through and get all parent directories
            var parentNames = new List<string?> { filename };

            // Get the parent directory
            var folder = default(object);
            switch (item.Model.Header?.MajorVersion)
            {
                case 4: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory4)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 5: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory5)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 6: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory6)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 7: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory7)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                default: return false;
            }

            // If we have a parent folder
            if (folder != null)
            {
                switch (item.Model.Header?.MajorVersion)
                {
                    case 4: parentNames.Add((folder as SabreTools.Models.SGA.Folder4)?.Name); break;
                    case 5:
                    case 6:
                    case 7: parentNames.Add((folder as SabreTools.Models.SGA.Folder5)?.Name); break;
                    default: return false;
                }
            }

            // TODO: Should the section name/alias be used in the path as well?

            // Reverse and assemble the filename
            parentNames.Reverse();
#if NET20 || NET35
            var parentNamesArray = parentNames.Cast<string>().ToArray();
            filename = parentNamesArray[0];
            for (int i = 1; i < parentNamesArray.Length; i++)
            {
                filename = Path.Combine(filename, parentNamesArray[i]);
            }
#else
            filename = Path.Combine(parentNames.Cast<string>().ToArray());
#endif

            // Get the file offset
            long fileOffset;
            switch (item.Model.Header?.MajorVersion)
            {
                case 4:
                case 5: fileOffset = (file as SabreTools.Models.SGA.File4)?.Offset ?? 0; break;
                case 6: fileOffset = (file as SabreTools.Models.SGA.File6)?.Offset ?? 0; break;
                case 7: fileOffset = (file as SabreTools.Models.SGA.File7)?.Offset ?? 0; break;
                default: return false;
            }

            // Adjust the file offset
            switch (item.Model.Header?.MajorVersion)
            {
                case 4: fileOffset += (item.Model.Header as SabreTools.Models.SGA.Header4)?.FileDataOffset ?? 0; break;
                case 5: fileOffset += (item.Model.Header as SabreTools.Models.SGA.Header4)?.FileDataOffset ?? 0; break;
                case 6: fileOffset += (item.Model.Header as SabreTools.Models.SGA.Header6)?.FileDataOffset ?? 0; break;
                case 7: fileOffset += (item.Model.Header as SabreTools.Models.SGA.Header6)?.FileDataOffset ?? 0; break;
                default: return false;
            };

            // Get the file sizes
            long fileSize, outputFileSize;
            switch (item.Model.Header?.MajorVersion)
            {
                case 4:
                case 5:
                    fileSize = (file as SabreTools.Models.SGA.File4)?.SizeOnDisk ?? 0;
                    outputFileSize = (file as SabreTools.Models.SGA.File4)?.Size ?? 0;
                    break;
                case 6:
                    fileSize = (file as SabreTools.Models.SGA.File6)?.SizeOnDisk ?? 0;
                    outputFileSize = (file as SabreTools.Models.SGA.File6)?.Size ?? 0;
                    break;
                case 7:
                    fileSize = (file as SabreTools.Models.SGA.File7)?.SizeOnDisk ?? 0;
                    outputFileSize = (file as SabreTools.Models.SGA.File7)?.Size ?? 0;
                    break;
                default: return false;
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
                var state = new ZLib.inflate_state();
                data = new byte[outputFileSize];
                unsafe
                {
                    fixed (byte* payloadPtr = compressedData)
                    fixed (byte* dataPtr = data)
                    {
                        zstream.next_in = payloadPtr;
                        zstream.avail_in = (uint)compressedData.Length;
                        zstream.next_out = dataPtr;
                        zstream.avail_out = (uint)data.Length;

                        state.strm = zstream;
                        state.mode = ZLib.inflate_mode.HEAD;

                        zstream.i_state = state;
                        int zret = ZLib.inflate(zstream, 1);
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
