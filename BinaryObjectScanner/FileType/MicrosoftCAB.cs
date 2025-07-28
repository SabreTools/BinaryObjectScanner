using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
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
            // TODO: Use MicrosoftCabinet.OpenSet when Serialization is updated
            try
            {
                // Create the wrapper
                var cabArchive = SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(stream);
                if (cabArchive?.Model?.Folders == null || cabArchive.Model.Folders.Length == 0)
                    return false;

                // Loop through the folders
                for (int f = 0; f < cabArchive!.Model.Folders.Length; f++)
                {
                    // Decompress the blocks, if possible
                    var folder = cabArchive.Model.Folders[f];
                    var ms = DecompressBlocks(cabArchive, folder);
                    if (ms == null || ms.Length == 0)
                        continue;

                    // Ensure files
                    var files = GetFiles(cabArchive, f);
                    if (files.Length == 0)
                        continue;

                    // Loop through the files
                    foreach (var compressedFile in files)
                    {
                        try
                        {
                            byte[] fileData = new byte[compressedFile.FileSize];
                            Array.Copy(ms.ToArray(), compressedFile.FolderStartOffset, fileData, 0, compressedFile.FileSize);

                            string tempFile = Path.Combine(outDir, compressedFile.Name!);
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
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
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
                        string tempFile = Path.Combine(outDir, compressedFile.Filename);
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
        /// Decompress all blocks for a folder
        /// </summary>
        private MemoryStream? DecompressBlocks(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, CFFOLDER? folder)
        {
            // Ensure data blocks
            if (folder?.DataBlocks == null || folder.DataBlocks.Length == 0)
                return null;

            // Setup decompressors
            var mszip = SabreTools.Compression.MSZIP.Decompressor.Create();
            uint quantumWindowBits = (uint)(((ushort)folder.CompressionType >> 8) & 0x1f);

            // Loop through the data blocks
            var ms = new MemoryStream();
            foreach (var db in folder.DataBlocks)
            {
                if (db?.CompressedData == null)
                    continue;

                // Uncompressed data
                if ((folder.CompressionType & CompressionType.TYPE_NONE) != 0)
                {
                    ms.Write(db.CompressedData, 0, db.CompressedData.Length);
                    ms.Flush();
                }

                // MS-ZIP
                else if ((folder.CompressionType & CompressionType.TYPE_MSZIP) != 0)
                {
                    mszip.CopyTo(db.CompressedData, ms);
                }

                // Quantum
                else if ((folder.CompressionType & CompressionType.TYPE_QUANTUM) != 0)
                {
                    var quantum = SabreTools.Compression.Quantum.Decompressor.Create(db.CompressedData, quantumWindowBits);
                    byte[] data = quantum.Process();
                    ms.Write(data, 0, data.Length);
                    ms.Flush();
                }

                // LZX
                else if ((folder.CompressionType & CompressionType.TYPE_LZX) != 0)
                {
                    // TODO: Unsupported
                    continue;
                }

                // Unknown
                else
                {
                    continue;
                }
            }

            return ms;
        }

        /// <summary>
        /// Get all files for the current folder index
        /// </summary>
        private CFFILE[] GetFiles(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, int folderIndex)
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
        private int GetFolderIndex(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, CFFILE file)
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
        private SabreTools.Serialization.Wrappers.MicrosoftCabinet? OpenNext(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, string? file)
        {
            // Ignore invalid archives
            if (cabArchive.Model.Header == null)
                return null;

            // Normalize the filename, if one exists
            if (file != null)
                file = Path.GetFullPath(file);

            // Get if the cabinet has a next part
            string? next = cabArchive.Model.Header.CabinetNext;
            if (string.IsNullOrEmpty(next))
                return null;

            // Get the full next path, if possible
            if (file != null)
            {
                string? folder = Path.GetDirectoryName(file);
                if (folder != null)
                    next = Path.Combine(folder, next);
            }

            // Open and return the next cabinet
            var fs = File.Open(next, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(fs);
        }

        /// <summary>
        /// Open the previous archive, if possible
        /// </summary>
        private SabreTools.Serialization.Wrappers.MicrosoftCabinet? OpenPrevious(SabreTools.Serialization.Wrappers.MicrosoftCabinet cabArchive, string? file)
        {
            // Ignore invalid archives
            if (cabArchive.Model.Header == null)
                return null;

            // Normalize the filename, if one exists
            if (file != null)
                file = Path.GetFullPath(file);

            // Get if the cabinet has a previous part
            string? prev = cabArchive.Model.Header.CabinetPrev;
            if (string.IsNullOrEmpty(prev))
                return null;

            // Get the full previous path, if possible
            if (file != null)
            {
                string? folder = Path.GetDirectoryName(file);
                if (folder != null)
                    prev = Path.Combine(folder, prev);
            }

            // Open and return the previous cabinet
            var fs = File.Open(prev, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(fs);
        }
    }
}
