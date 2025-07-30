using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
#if (NET40_OR_GREATER || NETCOREAPP) && WINX86
using LibMSPackN;
#else
using SabreTools.Models.MicrosoftCabinet;
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
#if NET20 || NET35 || !WINX86
            // TODO: Remove once Serialization is updated

            // Get a wrapper for the set, if possible
            SabreTools.Serialization.Wrappers.MicrosoftCabinet? current;
            if (File.Exists(file))
                current = OpenSet(file);
            else
                current = SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(stream);

            // Validate the header exists
            if (current?.Model?.Header == null)
                return false;

            try
            {
                // Loop through the cabinets
                do
                {
                    ExtractCabinet(current, file, outDir, forwardOnly: true, includeDebug);
                    current = OpenNext(current, file);
                }
                while (current?.Model?.Header != null);
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }

            return true;
#else
            try
            {
                if (!File.Exists(file))
                    return false;

                // Loop over each entry
                var cabArchive = new MSCabinet(file);
                foreach (var compressedFile in cabArchive.GetFiles())
                {
                    try
                    {
                        // Ensure directory separators are consistent
                        string fileName = compressedFile.Filename;
                        if (Path.DirectorySeparatorChar == '\\')
                            fileName = fileName.Replace('/', '\\');
                        else if (Path.DirectorySeparatorChar == '/')
                            fileName = fileName.Replace('\\', '/');
                        
                        string tempFile = Path.Combine(outDir, fileName);
                        var directoryName = Path.GetDirectoryName(tempFile);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        compressedFile.ExtractTo(tempFile);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
#endif
        }

        /// <summary>
        /// Open a cabinet set for reading, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// <returns>Wrapper representing the set, null on error</returns>
        /// TODO: Remove once Serialization is updated
        private static SabreTools.Serialization.Wrappers.MicrosoftCabinet? OpenSet(string? filename)
        {
            // If the file is invalid
            if (string.IsNullOrEmpty(filename))
                return null;
            else if (!File.Exists(filename!))
                return null;

            // Get the full file path and directory
            filename = Path.GetFullPath(filename);

            // Read in the current file and try to parse
            var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var current = SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(stream);
            if (current?.Model?.Header == null)
                return null;

            // Seek to the first part of the cabinet set
            while (current.Model.Header?.CabinetPrev != null)
            {
                // Attempt to open the previous cabinet
                var prev = OpenPrevious(current, filename);
                if (prev?.Model?.Header == null)
                    break;

                // Assign previous as new current
                current = prev;
            }

            // Return the start of the set
            return current;
        }

        /// <summary>
        /// Extract a cabinet file to an output directory, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set, if available</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Indicates if all files were able to be extracted</returns>
        /// TODO: Remove once Serialization is updated
        private static bool ExtractCabinet(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, string? filename, string outDir, bool forwardOnly, bool includeDebug)
        {
            // If the archive is invalid
            if (cabArchive?.Model?.Folders == null || cabArchive.Model.Folders.Length == 0)
                return false;

            try
            {
                // Loop through the folders
                for (int f = 0; f < cabArchive.Model.Folders.Length; f++)
                {
                    var folder = cabArchive.Model.Folders[f];
                    ExtractFolder(cabArchive, filename, outDir, folder, f, includeDebug);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract the contents of a single folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set, if available</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// TODO: Remove once Serialization is updated
        private static void ExtractFolder(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive,
            string? filename,
            string outDir,
            CFFOLDER? folder,
            int folderIndex,
            bool includeDebug)
        {
            // Decompress the blocks, if possible
            using var blockStream = DecompressBlocks(cabArchive, filename, folder, folderIndex, includeDebug);
            if (blockStream == null || blockStream.Length == 0)
                return;

            // Loop through the files
            var files = GetFiles(cabArchive, folderIndex);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                ExtractFile(outDir, blockStream, file, includeDebug);
            }
        }

        /// <summary>
        /// Extract the contents of a single file
        /// </summary>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="blockStream">Stream representing the uncompressed block data</param>
        /// <param name="file">File information</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// TODO: Remove once Serialization is updated
        private static void ExtractFile(string outDir, Stream blockStream, CFFILE file, bool includeDebug)
        {
            try
            {
                blockStream.Seek(file.FolderStartOffset, SeekOrigin.Begin);
                byte[] fileData = blockStream.ReadBytes((int)file.FileSize);

                // Ensure directory separators are consistent
                string fileName = file.Name!;
                if (Path.DirectorySeparatorChar == '\\')
                    fileName = fileName.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    fileName = fileName.Replace('\\', '/');

                string tempFile = Path.Combine(outDir, fileName);
                var directoryName = Path.GetDirectoryName(tempFile);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                using var of = File.OpenWrite(tempFile);
                of.Write(fileData, 0, fileData.Length);
                of.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Decompress all blocks for a folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set, if available</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Stream representing the decompressed data on success, null otherwise</returns>
        /// TODO: Remove once Serialization is updated
        private static Stream? DecompressBlocks(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, string? filename, CFFOLDER? folder, int folderIndex, bool includeDebug)
        {
            // Ensure data blocks
            var dataBlocks = GetDataBlocks(cabArchive, filename, folder, folderIndex);
            if (dataBlocks == null || dataBlocks.Length == 0)
                return null;

            // Get the compression type
            var compressionType = GetCompressionType(folder!);

            // Setup decompressors
            var mszip = SabreTools.Compression.MSZIP.Decompressor.Create();
            //uint quantumWindowBits = (uint)(((ushort)folder.CompressionType >> 8) & 0x1f);

            // Loop through the data blocks
            var ms = new MemoryStream();
            for (int i = 0; i < dataBlocks.Length; i++)
            {
                var db = dataBlocks[i];
                if (db?.CompressedData == null)
                    continue;

                // Get the uncompressed data block
                byte[] data = compressionType switch
                {
                    CompressionType.TYPE_NONE => db.CompressedData,
                    CompressionType.TYPE_MSZIP => DecompressMSZIPBlock(folderIndex, mszip, i, db, includeDebug),

                    // TODO: Unsupported
                    CompressionType.TYPE_QUANTUM => [],
                    CompressionType.TYPE_LZX => [],

                    // Should be impossible
                    _ => [],
                };

                // Write the uncompressed data block
                ms.Write(data, 0, data.Length);
                ms.Flush();
            }

            return ms;
        }

        /// <summary>
        /// Decompress an MS-ZIP block using an existing decompressor
        /// </summary>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="mszip">MS-ZIP decompressor with persistent state</param>
        /// <param name="blockIndex">Index of the block within the folder</param>
        /// <param name="block">Block data to be used for decompression</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Byte array representing the decompressed data, empty on error</returns>
        /// TODO: Remove once Serialization is updated
        private static byte[] DecompressMSZIPBlock(int folderIndex, SabreTools.Compression.MSZIP.Decompressor mszip, int blockIndex, CFDATA block, bool includeDebug)
        {
            // Ignore invalid blocks
            if (block.CompressedData == null)
                return [];

            try
            {
                // Decompress to a temporary stream
                using var stream = new MemoryStream();
                mszip.CopyTo(block.CompressedData, stream);

                // Pad to the correct size but throw a warning about this
                if (stream.Length < block.UncompressedSize)
                {
                    if (includeDebug)
                        Console.Error.WriteLine($"Data block {blockIndex} in folder {folderIndex} had mismatching sizes. Expected: {block.UncompressedSize}, Got: {stream.Length}");

                    byte[] padding = new byte[block.UncompressedSize - stream.Length];
                    stream.Write(padding, 0, padding.Length);
                }

                // Return the byte array data
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return [];
            }
        }

        /// <summary>
        /// Get the unmasked compression type for a folder
        /// </summary>
        /// <param name="folder">Folder to get the compression type for</param>
        /// <returns>Compression type on success, <see cref="ushort.MaxValue"/> on error</returns>
        /// TODO: Remove once Serialization is updated
        private static CompressionType GetCompressionType(CFFOLDER folder)
        {
            if ((folder!.CompressionType & CompressionType.MASK_TYPE) == CompressionType.TYPE_NONE)
                return CompressionType.TYPE_NONE;
            else if ((folder.CompressionType & CompressionType.MASK_TYPE) == CompressionType.TYPE_MSZIP)
                return CompressionType.TYPE_MSZIP;
            else if ((folder.CompressionType & CompressionType.MASK_TYPE) == CompressionType.TYPE_QUANTUM)
                return CompressionType.TYPE_QUANTUM;
            else if ((folder.CompressionType & CompressionType.MASK_TYPE) == CompressionType.TYPE_LZX)
                return CompressionType.TYPE_LZX;
            else
                return (CompressionType)ushort.MaxValue;
        }

        /// <summary>
        /// Get the set of data blocks for a folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set, if available</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="skipPrev">Indicates if previous cabinets should be ignored</param>
        /// <param name="skipNext">Indicates if next cabinets should be ignored</param>
        /// <returns>Array of data blocks on success, null otherwise</returns>
        /// TODO: Remove once Serialization is updated
        private static CFDATA[]? GetDataBlocks(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, string? filename, CFFOLDER? folder, int folderIndex, bool skipPrev = false, bool skipNext = false)
        {
            // Skip invalid folders
            if (folder?.DataBlocks == null || folder.DataBlocks.Length == 0)
                return null;

            // Get all files for the folder
            var files = GetFiles(cabArchive, folderIndex);
            if (files.Length == 0)
                return folder.DataBlocks;

            // Check if the folder spans in either direction
            bool spanPrev = Array.Exists(files, f => f.FolderIndex == FolderIndex.CONTINUED_FROM_PREV || f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT);
            bool spanNext = Array.Exists(files, f => f.FolderIndex == FolderIndex.CONTINUED_TO_NEXT || f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT);

            // If the folder spans backward and Prev is not being skipped
            CFDATA[] prevBlocks = [];
            if (!skipPrev && spanPrev)
            {
                // Get all blocks from Prev
                var prev = OpenPrevious(cabArchive, filename);
                if (prev?.Model?.Header != null && prev.Model.Folders != null)
                {
                    int prevFolderIndex = prev.Model.Header.FolderCount;
                    var prevFolder = prev.Model.Folders[prevFolderIndex - 1];
                    prevBlocks = GetDataBlocks(prev, filename, prevFolder, prevFolderIndex, skipNext: true) ?? [];
                }
            }

            // If the folder spans forward and Next is not being skipped
            CFDATA[] nextBlocks = [];
            if (!skipNext && spanNext)
            {
                // Get all blocks from Prev
                var next = OpenNext(cabArchive, filename);
                if (next?.Model?.Header != null && next.Model.Folders != null)
                {
                    var nextFolder = next.Model.Folders[0];
                    nextBlocks = GetDataBlocks(next, filename, nextFolder, 0, skipPrev: true) ?? [];
                }
            }

            // Return all found blocks in order
            return [.. prevBlocks, .. folder.DataBlocks, .. nextBlocks];
        }

        /// <summary>
        /// Get all files for the current folder index
        /// </summary>
        /// TODO: Remove once Serialization is updated
        private static CFFILE[] GetFiles(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, int folderIndex)
        {
            // Ignore invalid archives
            if (cabArchive.Model.Files == null)
                return [];

            // Get all files with a name and matching index
            return Array.FindAll(cabArchive.Model.Files, f =>
            {
                if (string.IsNullOrEmpty(f.Name))
                    return false;

                int fileFolder = GetFolderIndex(cabArchive, f);
                return fileFolder == folderIndex;
            });
        }

        /// <summary>
        /// Get the corrected folder index
        /// </summary>
        /// TODO: Remove once Serialization is updated
        private static int GetFolderIndex(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, CFFILE file)
        {
            return file.FolderIndex switch
            {
                FolderIndex.CONTINUED_FROM_PREV => 0,
                FolderIndex.CONTINUED_TO_NEXT => (cabArchive.Model.Header?.FolderCount ?? 1) - 1,
                FolderIndex.CONTINUED_PREV_AND_NEXT => 0,
                _ => (int)file.FolderIndex,
            };
        }

        /// <summary>
        /// Open the next archive, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// TODO: Remove once Serialization is updated
        private static SabreTools.Serialization.Wrappers.MicrosoftCabinet? OpenNext(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, string? filename)
        {
            // Ignore invalid archives
            if (cabArchive.Model?.Header == null || string.IsNullOrEmpty(filename))
                return null;

            // Normalize the filename
            filename = Path.GetFullPath(filename);

            // Get if the cabinet has a next part
            string? next = cabArchive.Model.Header.CabinetNext;
            if (string.IsNullOrEmpty(next))
                return null;

            // Get the full next path
            string? folder = Path.GetDirectoryName(filename);
            if (folder != null)
                next = Path.Combine(folder, next);

            // Open and return the next cabinet
            var fs = File.Open(next, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(fs);
        }

        /// <summary>
        /// Open the previous archive, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// TODO: Remove once Serialization is updated
        private static SabreTools.Serialization.Wrappers.MicrosoftCabinet? OpenPrevious(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, string? filename)
        {
            // Ignore invalid archives
            if (cabArchive.Model?.Header == null || string.IsNullOrEmpty(filename))
                return null;

            // Normalize the filename
            filename = Path.GetFullPath(filename);

            // Get if the cabinet has a previous part
            string? prev = cabArchive.Model.Header.CabinetPrev;
            if (string.IsNullOrEmpty(prev))
                return null;

            // Get the full next path
            string? folder = Path.GetDirectoryName(filename);
            if (folder != null)
                prev = Path.Combine(folder, prev);

            // Open and return the previous cabinet
            var fs = File.Open(prev, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(fs);
        }
    }
}
