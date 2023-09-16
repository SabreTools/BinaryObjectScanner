using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip.Compression;
using SabreTools.IO;
using SabreTools.Serialization.Wrappers;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace BinaryObjectScanner.Wrappers
{
    public static class Extraction
    {
        #region BFPK

        /// <summary>
        /// Extract all files from the BFPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(this BFPK item, string outputDirectory)
        {
            // If we have no files
            if (item.Model.Files == null || item.Model.Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Files.Length; i++)
            {
                allExtracted &= item.ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the BFPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(this BFPK item, int index, string outputDirectory)
        {
            // If we have no files
            if (item.Model.Files == null || item.Model.Files.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= item.Model.Files.Length)
                return false;

            // Get the file information
            var file = item.Model.Files[index];
            if (file == null)
                return false;

            // Get the read index and length
            int offset = file.Offset + 4;
            int compressedSize = file.CompressedSize;

            // Some files can lack the length prefix
            if (compressedSize > item.GetEndOfFile())
            {
                offset -= 4;
                compressedSize = file.UncompressedSize;
            }

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.Name ?? $"file{index}");
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    // Read the data block
#if NET48
                    byte[] data = item.ReadFromDataSource(offset, compressedSize);
#else
                    byte[]? data = item.ReadFromDataSource(offset, compressedSize);
#endif
                    if (data == null)
                        return false;

                    // If we have uncompressed data
                    if (compressedSize == file.UncompressedSize)
                    {
                        fs.Write(data, 0, compressedSize);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(data);
                        ZlibStream zs = new ZlibStream(ms, CompressionMode.Decompress);
                        zs.CopyTo(fs);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region BSP

        /// <summary>
        /// Extract all lumps from the BSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public static bool ExtractAllLumps(this BSP item, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Lumps == null || item.Model.Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Lumps.Length; i++)
            {
                allExtracted &= item.ExtractLump(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the BSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public static bool ExtractLump(this BSP item, int index, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Lumps == null || item.Model.Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= item.Model.Lumps.Length)
                return false;

            // Get the lump
            var lump = item.Model.Lumps[index];
            if (lump == null)
                return false;

            // Read the data
#if NET48
            byte[] data = item.ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#else
            byte[]? data = item.ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#endif
            if (data == null)
                return false;

            // Create the filename
            string filename = $"lump_{index}.bin";
            switch (index)
            {
                case SabreTools.Models.BSP.Constants.HL_BSP_LUMP_ENTITIES:
                    filename = "entities.ent";
                    break;
                case SabreTools.Models.BSP.Constants.HL_BSP_LUMP_TEXTUREDATA:
                    filename = "texture_data.bin";
                    break;
            }

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Extract all textures from the BSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all textures extracted, false otherwise</returns>
        public static bool ExtractAllTextures(this BSP item, string outputDirectory)
        {
            // If we have no textures
            if (item.Model.TextureHeader?.Offsets == null || item.Model.TextureHeader.Offsets.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.TextureHeader.Offsets.Length; i++)
            {
                allExtracted &= item.ExtractTexture(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a texture from the BSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the texture extracted, false otherwise</returns>
        public static bool ExtractTexture(this BSP item, int index, string outputDirectory)
        {
            // If we have no textures
            if (item.Model.Textures == null || item.Model.Textures.Length == 0)
                return false;

            // If the texture index is invalid
            if (index < 0 || index >= item.Model.Textures.Length)
                return false;

            // Get the texture
            var texture = item.Model.Textures[index];
            if (texture == null)
                return false;

            // Read the data
#if NET48
            byte[] data = CreateTextureData(texture);
#else
            byte[]? data = CreateTextureData(texture);
#endif
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{texture.Name}.bmp";

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create a bitmap from the texture and palette data
        /// </summary>
        /// <param name="texture">Texture object to format</param>
        /// <returns>Byte array representing the texture as a bitmap</returns>
#if NET48
        private static byte[] CreateTextureData(SabreTools.Models.BSP.Texture texture)
#else
        private static byte[]? CreateTextureData(SabreTools.Models.BSP.Texture texture)
#endif
        {
            // If there's no palette data
            if (texture.PaletteData == null || texture.PaletteData.Length == 0)
                return null;

            // If there's no texture data
            if (texture.TextureData == null || texture.TextureData.Length == 0)
                return null;

            // Create the bitmap file header
            var fileHeader = new SabreTools.Models.BMP.BITMAPFILEHEADER()
            {
                Type = ('M' << 8) | 'B',
                Size = 14 + 40 + (texture.PaletteSize * 4) + (texture.Width * texture.Height),
                OffBits = 14 + 40 + (texture.PaletteSize * 4),
            };

            // Create the bitmap info header
            var infoHeader = new SabreTools.Models.BMP.BITMAPINFOHEADER
            {
                Size = 40,
                Width = (int)texture.Width,
                Height = (int)texture.Height,
                Planes = 1,
                BitCount = 8,
                SizeImage = 0,
                ClrUsed = texture.PaletteSize,
                ClrImportant = texture.PaletteSize,
            };

            // Reformat the palette data
            byte[] paletteData = new byte[texture.PaletteSize * 4];
            for (uint i = 0; i < texture.PaletteSize; i++)
            {
                paletteData[i * 4 + 0] = texture.PaletteData[i * 3 + 2];
                paletteData[i * 4 + 1] = texture.PaletteData[i * 3 + 1];
                paletteData[i * 4 + 2] = texture.PaletteData[i * 3 + 0];
                paletteData[i * 4 + 3] = 0;
            }

            // Reformat the pixel data
            byte[] pixelData = new byte[texture.Width * texture.Height];
            for (uint i = 0; i < texture.Width; i++)
            {
                for (uint j = 0; j < texture.Height; j++)
                {
                    pixelData[i + ((texture.Height - 1 - j) * texture.Width)] = texture.TextureData[i + j * texture.Width];
                }
            }

            // Build the file data
            List<byte> buffer = new List<byte>();

            // Bitmap file header
            buffer.AddRange(BitConverter.GetBytes(fileHeader.Type));
            buffer.AddRange(BitConverter.GetBytes(fileHeader.Size));
            buffer.AddRange(BitConverter.GetBytes(fileHeader.Reserved1));
            buffer.AddRange(BitConverter.GetBytes(fileHeader.Reserved2));
            buffer.AddRange(BitConverter.GetBytes(fileHeader.OffBits));

            // Bitmap info header
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Size));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Width));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Height));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Planes));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.BitCount));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Compression));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.SizeImage));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.XPelsPerMeter));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.YPelsPerMeter));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.ClrUsed));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.ClrImportant));

            // Palette data
            buffer.AddRange(paletteData);

            // Pixel data
            buffer.AddRange(pixelData);

            return buffer.ToArray();
        }

        #endregion

        #region GCF

        /// <summary>
        /// Extract all files from the GCF to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(this GCF item, string outputDirectory)
        {
            // If we have no files
            if (item.Files == null || item.Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Files.Length; i++)
            {
                allExtracted &= item.ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the GCF to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(this GCF item, int index, string outputDirectory)
        {
            // If we have no files
            if (item.Files == null || item.Files.Length == 0 || item.DataBlockOffsets == null)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= item.Files.Length)
                return false;

            // Get the file
            var file = item.Files[index];
            if (file?.BlockEntries == null || file.Size == 0)
                return false;

            // If the file is encrypted -- TODO: Revisit later
            if (file.Encrypted)
                return false;

            // Get all data block offsets needed for extraction
            var dataBlockOffsets = new List<long>();
            for (int i = 0; i < file.BlockEntries.Length; i++)
            {
                var blockEntry = file.BlockEntries[i];
                if (blockEntry == null)
                    continue;

                uint dataBlockIndex = blockEntry.FirstDataBlockIndex;
                long blockEntrySize = blockEntry.FileDataSize;
                while (blockEntrySize > 0)
                {
                    long dataBlockOffset = item.DataBlockOffsets[dataBlockIndex++];
                    dataBlockOffsets.Add(dataBlockOffset);
                    blockEntrySize -= item.Model.DataBlockHeader?.BlockSize ?? 0;
                }
            }

            // Create the filename
#if NET48
            string filename = file.Path;
#else
            string? filename = file.Path;
#endif

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename ?? $"file{index}");

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    // Now read the data sequentially and write out while we have data left
                    long fileSize = file.Size;
                    for (int i = 0; i < dataBlockOffsets.Count; i++)
                    {
                        int readSize = (int)Math.Min(item.Model.DataBlockHeader?.BlockSize ?? 0, fileSize);
#if NET48
                        byte[] data = item.ReadFromDataSource((int)dataBlockOffsets[i], readSize);
#else
                        byte[]? data = item.ReadFromDataSource((int)dataBlockOffsets[i], readSize);
#endif
                        if (data == null)
                            return false;

                        fs.Write(data, 0, data.Length);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region MS-CAB

        #region Folders

        /// <summary>
        /// Get the uncompressed data associated with a folder
        /// </summary>
        /// <param name="folderIndex">Folder index to check</param>
        /// <returns>Byte array representing the data, null on error</returns>
        /// <remarks>All but uncompressed are unimplemented</remarks>
#if NET48
        public static byte[] GetUncompressedData(this MicrosoftCabinet item, int folderIndex)
#else
        public static byte[]? GetUncompressedData(this MicrosoftCabinet item, int folderIndex)
#endif
        {
            // If we have an invalid folder index
            if (folderIndex < 0 || item.Model.Folders == null || folderIndex >= item.Model.Folders.Length)
                return null;

            // Get the folder header
            var folder = item.Model.Folders[folderIndex];
            if (folder == null)
                return null;

            // If we have invalid data blocks
            if (folder.DataBlocks == null || folder.DataBlocks.Length == 0)
                return null;

            // Setup LZX decompression
            var lzx = new Compression.LZX.State();
            Compression.LZX.Decompressor.Init(((ushort)folder.CompressionType >> 8) & 0x1f, lzx);

            // Setup MS-ZIP decompression
            Compression.MSZIP.State mszip = new Compression.MSZIP.State();

            // Setup Quantum decompression
            var qtm = new Compression.Quantum.State();
            Compression.Quantum.Decompressor.InitState(qtm, folder);

            List<byte> data = new List<byte>();
            foreach (var dataBlock in folder.DataBlocks)
            {
                if (dataBlock == null)
                    continue;

#if NET48
                byte[] decompressed = new byte[dataBlock.UncompressedSize];
#else
                byte[]? decompressed = new byte[dataBlock.UncompressedSize];
#endif
                switch (folder.CompressionType & SabreTools.Models.MicrosoftCabinet.CompressionType.MASK_TYPE)
                {
                    case SabreTools.Models.MicrosoftCabinet.CompressionType.TYPE_NONE:
                        decompressed = dataBlock.CompressedData;
                        break;
                    case SabreTools.Models.MicrosoftCabinet.CompressionType.TYPE_MSZIP:
                        decompressed = new byte[SabreTools.Models.Compression.MSZIP.Constants.ZIPWSIZE];
                        Compression.MSZIP.Decompressor.Decompress(mszip, dataBlock.CompressedSize, dataBlock.CompressedData, dataBlock.UncompressedSize, decompressed);
                        Array.Resize(ref decompressed, dataBlock.UncompressedSize);
                        break;
                    case SabreTools.Models.MicrosoftCabinet.CompressionType.TYPE_QUANTUM:
                        Compression.Quantum.Decompressor.Decompress(qtm, dataBlock.CompressedSize, dataBlock.CompressedData, dataBlock.UncompressedSize, decompressed);
                        break;
                    case SabreTools.Models.MicrosoftCabinet.CompressionType.TYPE_LZX:
                        Compression.LZX.Decompressor.Decompress(state: lzx, dataBlock.CompressedSize, dataBlock.CompressedData, dataBlock.UncompressedSize, decompressed);
                        break;
                    default:
                        return null;
                }

                if (decompressed != null)
                    data.AddRange(decompressed);
            }

            return data.ToArray();
        }

        #endregion

        #region Files

        /// <summary>
        /// Extract all files from the MS-CAB to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all filez extracted, false otherwise</returns>
        public static bool ExtractAll(this MicrosoftCabinet item, string outputDirectory)
        {
            // If we have no files
            if (item.Model.Files == null || item.Model.Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Files.Length; i++)
            {
                allExtracted &= item.ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the MS-CAB to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(this MicrosoftCabinet item, int index, string outputDirectory)
        {
            // If we have an invalid file index
            if (index < 0 || item.Model.Files == null || index >= item.Model.Files.Length)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Ensure the directory exists
            Directory.CreateDirectory(outputDirectory);

            // Get the file header
            var file = item.Model.Files[index];
            if (file == null || file.FileSize == 0)
                return false;

            // Create the output filename
            string fileName = Path.Combine(outputDirectory, file.Name ?? $"file{index}");

            // Get the file data, if possible
#if NET48
            byte[] fileData = item.GetFileData(index);
#else
            byte[]? fileData = item.GetFileData(index);
#endif
            if (fileData == null)
                return false;

            // Write the file data
            using (FileStream fs = File.OpenWrite(fileName))
            {
                fs.Write(fileData, 0, fileData.Length);
            }

            return true;
        }

        /// <summary>
        /// Get the uncompressed data associated with a file
        /// </summary>
        /// <param name="fileIndex">File index to check</param>
        /// <returns>Byte array representing the data, null on error</returns>
#if NET48
        public static byte[] GetFileData(this MicrosoftCabinet item, int fileIndex)
#else
        public static byte[]? GetFileData(this MicrosoftCabinet item, int fileIndex)
#endif
        {
            // If we have an invalid file index
            if (fileIndex < 0 || item.Model.Files == null || fileIndex >= item.Model.Files.Length)
                return null;

            // Get the file header
            var file = item.Model.Files[fileIndex];
            if (file == null || file.FileSize == 0)
                return null;

            // Get the parent folder data
#if NET48
            byte[] folderData = item.GetUncompressedData((int)file.FolderIndex);
#else
            byte[]? folderData = item.GetUncompressedData((int)file.FolderIndex);
#endif
            if (folderData == null || folderData.Length == 0)
                return null;

            // Create the output file data
            byte[] fileData = new byte[file.FileSize];
            if (folderData.Length < file.FolderStartOffset + file.FileSize)
                return null;

            // Get the segment that represents this file
            Array.Copy(folderData, file.FolderStartOffset, fileData, 0, file.FileSize);
            return fileData;
        }

        #endregion

        #endregion

        #region PAK

        /// <summary>
        /// Extract all files from the PAK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(this PAK item, string outputDirectory)
        {
            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.DirectoryItems.Length; i++)
            {
                allExtracted &= item.ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the PAK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(this PAK item, int index, string outputDirectory)
        {
            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // If the directory item index is invalid
            if (index < 0 || index >= item.Model.DirectoryItems.Length)
                return false;

            // Get the directory item
            var directoryItem = item.Model.DirectoryItems[index];
            if (directoryItem == null)
                return false;

            // Read the item data
#if NET48
            byte[] data = item.ReadFromDataSource((int)directoryItem.ItemOffset, (int)directoryItem.ItemLength);
#else
            byte[]? data = item.ReadFromDataSource((int)directoryItem.ItemOffset, (int)directoryItem.ItemLength);
#endif
            if (data == null)
                return false;

            // Create the filename
#if NET48
            string filename = directoryItem.ItemName;
#else
            string? filename = directoryItem.ItemName;
#endif

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename ?? $"file{index}");

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region PFF

        /// <summary>
        /// Extract all segments from the PFF to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all segments extracted, false otherwise</returns>
        public static bool ExtractAll(this PFF item, string outputDirectory)
        {
            // If we have no segments
            if (item.Model.Segments == null || item.Model.Segments.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Segments.Length; i++)
            {
                allExtracted &= item.ExtractSegment(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a segment from the PFF to an output directory by index
        /// </summary>
        /// <param name="index">Segment index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the segment extracted, false otherwise</returns>
        public static bool ExtractSegment(this PFF item, int index, string outputDirectory)
        {
            // If we have no segments
            if (item.Model.Header?.NumberOfFiles == null || item.Model.Header.NumberOfFiles == 0 || item.Model.Segments == null || item.Model.Segments.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= item.Model.Segments.Length)
                return false;

            // Get the segment information
            var file = item.Model.Segments[index];
            if (file == null)
                return false;

            // Get the read index and length
            int offset = (int)file.FileLocation;
            int size = (int)file.FileSize;

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.FileName ?? $"file{index}");
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    // Read the data block
#if NET48
                    byte[] data = item.ReadFromDataSource(offset, size);
#else
                    byte[]? data = item.ReadFromDataSource(offset, size);
#endif
                    if (data == null)
                        return false;

                    // Write the data -- TODO: Compressed data?
                    fs.Write(data, 0, size);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Quantum

        /// <summary>
        /// Extract all files from the Quantum archive to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(this Quantum item, string outputDirectory)
        {
            // If we have no files
            if (item.Model.FileList == null || item.Model.FileList.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.FileList.Length; i++)
            {
                allExtracted &= item.ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the Quantum archive to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(this Quantum item, int index, string outputDirectory)
        {
            // If we have no files
            if (item.Model.Header == null || item.Model.Header.FileCount == 0 || item.Model.FileList == null || item.Model.FileList.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= item.Model.FileList.Length)
                return false;

            // Get the file information
            var fileDescriptor = item.Model.FileList[index];

            // Read the entire compressed data
            int compressedDataOffset = (int)item.Model.CompressedDataOffset;
            int compressedDataLength = item.GetEndOfFile() - compressedDataOffset;
#if NET48
            byte[] compressedData = item.ReadFromDataSource(compressedDataOffset, compressedDataLength);
#else
            byte[]? compressedData = item.ReadFromDataSource(compressedDataOffset, compressedDataLength);
#endif

            // TODO: Figure out decompression
            // - Single-file archives seem to work
            // - Single-file archives with files that span a window boundary seem to work
            // - The first files in each archive seem to work
            return false;

            // // Setup the decompression state
            // State state = new State();
            // Decompressor.InitState(state, TableSize, CompressionFlags);

            // // Decompress the entire array
            // int decompressedDataLength = (int)FileList.Sum(fd => fd.ExpandedFileSize);
            // byte[] decompressedData = new byte[decompressedDataLength];
            // Decompressor.Decompress(state, compressedData.Length, compressedData, decompressedData.Length, decompressedData);

            // // Read the data
            // int offset = (int)FileList.Take(index).Sum(fd => fd.ExpandedFileSize);
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // Array.Copy(decompressedData, offset, data, 0, data.Length);

            // // Loop through all files before the current
            // for (int i = 0; i < index; i++)
            // {
            //     // Decompress the next block of data
            //     byte[] tempData = new byte[FileList[i].ExpandedFileSize];
            //     int lastRead = Decompressor.Decompress(state, compressedData.Length, compressedData, tempData.Length, tempData);
            //     compressedData = new ReadOnlySpan<byte>(compressedData, (lastRead), compressedData.Length - (lastRead)).ToArray();
            // }

            // // Read the data
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // _ = Decompressor.Decompress(state, compressedData.Length, compressedData, data.Length, data);

            // // Create the filename
            // string filename = fileDescriptor.FileName;

            // // If we have an invalid output directory
            // if (string.IsNullOrWhiteSpace(outputDirectory))
            //     return false;

            // // Create the full output path
            // filename = Path.Combine(outputDirectory, filename);

            // // Ensure the output directory is created
            // Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // // Try to write the data
            // try
            // {
            //     // Open the output file for writing
            //     using (Stream fs = File.OpenWrite(filename))
            //     {
            //         fs.Write(data, 0, data.Length);
            //     }
            // }
            // catch
            // {
            //     return false;
            // }

            return true;
        }

        #endregion

        #region SGA

        /// <summary>
        /// Extract all files from the SGA to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(this SGA item, string outputDirectory)
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
                allExtracted &= item.ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the SGA to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(this SGA item, int index, string outputDirectory)
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
#if NET48
            object file;
#else
            object? file;
#endif
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
#if NET48
            string filename;
#else
            string? filename;
#endif
            switch (item.Model.Header?.MajorVersion)
            {
                case 4:
                case 5: filename = (file as SabreTools.Models.SGA.File4)?.Name; break;
                case 6: filename = (file as SabreTools.Models.SGA.File6)?.Name; break;
                case 7: filename = (file as SabreTools.Models.SGA.File7)?.Name; break;
                default: return false;
            }

            // Loop through and get all parent directories
#if NET48
            var parentNames = new List<string> { filename };
#else
            var parentNames = new List<string?> { filename };
#endif

            // Get the parent directory
#if NET48
            object folder;
#else
            object? folder;
#endif
            switch (item.Model.Header?.MajorVersion)
            {
#if NET48
                case 4: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory4)?.Folders?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 5: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory5)?.Folders?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 6: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory6)?.Folders?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 7: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory7)?.Folders?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
#else
                case 4: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory4)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 5: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory5)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 6: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory6)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 7: folder = (item.Model.Directory as SabreTools.Models.SGA.Directory7)?.Folders?.FirstOrDefault(f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex); break;
#endif
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
            filename = Path.Combine(parentNames.Cast<string>().ToArray());

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
#if NET48
            byte[] compressedData = item.ReadFromDataSource((int)fileOffset, (int)fileSize);
#else
            byte[]? compressedData = item.ReadFromDataSource((int)fileOffset, (int)fileSize);
#endif
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
                // Decompress the data
                data = new byte[outputFileSize];
                Inflater inflater = new Inflater();
                inflater.SetInput(compressedData);
                inflater.Inflate(data);
            }

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        #endregion

        #region VBSP

        /// <summary>
        /// Extract all lumps from the VBSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public static bool ExtractAllLumps(this VBSP item, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Header?.Lumps == null || item.Model.Header.Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Header.Lumps.Length; i++)
            {
                allExtracted &= item.ExtractLump(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the VBSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public static bool ExtractLump(this VBSP item, int index, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Header?.Lumps == null || item.Model.Header.Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= item.Model.Header.Lumps.Length)
                return false;

            // Get the lump
            var lump = item.Model.Header.Lumps[index];
            if (lump == null)
                return false;

            // Read the data
#if NET48
            byte[] data = item.ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#else
            byte[]? data = item.ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#endif
            if (data == null)
                return false;

            // Create the filename
            string filename = $"lump_{index}.bin";
            switch (index)
            {
                case SabreTools.Models.VBSP.Constants.HL_VBSP_LUMP_ENTITIES:
                    filename = "entities.ent";
                    break;
                case SabreTools.Models.VBSP.Constants.HL_VBSP_LUMP_PAKFILE:
                    filename = "pakfile.zip";
                    break;
            }

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region VPK

        /// <summary>
        /// Extract all files from the VPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(this VPK item, string outputDirectory)
        {
            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.DirectoryItems.Length; i++)
            {
                allExtracted &= item.ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the VPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(this VPK item, int index, string outputDirectory)
        {
            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // If the directory item index is invalid
            if (index < 0 || index >= item.Model.DirectoryItems.Length)
                return false;

            // Get the directory item
            var directoryItem = item.Model.DirectoryItems[index];
            if (directoryItem?.DirectoryEntry == null)
                return false;

            // If we have an item with no archive
#if NET48
            byte[] data;
#else
            byte[]? data;
#endif
            if (directoryItem.DirectoryEntry.ArchiveIndex == SabreTools.Models.VPK.Constants.HL_VPK_NO_ARCHIVE)
            {
                if (directoryItem.PreloadData == null)
                    return false;

                data = directoryItem.PreloadData;
            }
            else
            {
                // If we have invalid archives
                if (item.ArchiveFilenames == null || item.ArchiveFilenames.Length == 0)
                    return false;

                // If we have an invalid index
                if (directoryItem.DirectoryEntry.ArchiveIndex < 0 || directoryItem.DirectoryEntry.ArchiveIndex >= item.ArchiveFilenames.Length)
                    return false;

                // Get the archive filename
                string archiveFileName = item.ArchiveFilenames[directoryItem.DirectoryEntry.ArchiveIndex];
                if (string.IsNullOrWhiteSpace(archiveFileName))
                    return false;

                // If the archive doesn't exist
                if (!File.Exists(archiveFileName))
                    return false;

                // Try to open the archive
#if NET48
                Stream archiveStream = null;
#else
                Stream? archiveStream = null;
#endif
                try
                {
                    // Open the archive
                    archiveStream = File.OpenRead(archiveFileName);

                    // Seek to the data
                    archiveStream.Seek(directoryItem.DirectoryEntry.EntryOffset, SeekOrigin.Begin);

                    // Read the directory item bytes
                    data = archiveStream.ReadBytes((int)directoryItem.DirectoryEntry.EntryLength);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    archiveStream?.Close();
                }

                // If we have preload data, prepend it
                if (data != null && directoryItem.PreloadData != null)
                    data = directoryItem.PreloadData.Concat(data).ToArray();
            }

            // If there is nothing to write out
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{directoryItem.Name}.{directoryItem.Extension}";
            if (!string.IsNullOrWhiteSpace(directoryItem.Path))
                filename = Path.Combine(directoryItem.Path, filename);

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region WAD

        /// <summary>
        /// Extract all lumps from the WAD to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public static bool ExtractAllLumps(this WAD item, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Lumps == null || item.Model.Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Lumps.Length; i++)
            {
                allExtracted &= item.ExtractLump(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the WAD to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public static bool ExtractLump(this WAD item, int index, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Lumps == null || item.Model.Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= item.Model.Lumps.Length)
                return false;

            // Get the lump
            var lump = item.Model.Lumps[index];
            if (lump == null)
                return false;

            // Read the data -- TODO: Handle uncompressed lumps (see BSP.ExtractTexture)
#if NET48
            byte[] data = item.ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#else
            byte[]? data = item.ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#endif
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{lump.Name}.lmp";

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region XZP

        /// <summary>
        /// Extract all files from the XZP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public static bool ExtractAll(this XZP item, string outputDirectory)
        {
            // If we have no directory entries
            if (item.Model.DirectoryEntries == null || item.Model.DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.DirectoryEntries.Length; i++)
            {
                allExtracted &= item.ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the XZP to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public static bool ExtractFile(this XZP item, int index, string outputDirectory)
        {
            // If we have no directory entries
            if (item.Model.DirectoryEntries == null || item.Model.DirectoryEntries.Length == 0)
                return false;

            // If we have no directory items
            if (item.Model.DirectoryItems == null || item.Model.DirectoryItems.Length == 0)
                return false;

            // If the directory entry index is invalid
            if (index < 0 || index >= item.Model.DirectoryEntries.Length)
                return false;

            // Get the directory entry
            var directoryEntry = item.Model.DirectoryEntries[index];
            if (directoryEntry == null)
                return false;

            // Get the associated directory item
            var directoryItem = item.Model.DirectoryItems.Where(di => di?.FileNameCRC == directoryEntry.FileNameCRC).FirstOrDefault();
            if (directoryItem == null)
                return false;

            // Load the item data
#if NET48
            byte[] data = item.ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);
#else
            byte[]? data = item.ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);
#endif
            if (data == null)
                return false;

            // Create the filename
#if NET48
            string filename = directoryItem.Name;
#else
            string? filename = directoryItem.Name;
#endif

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename ?? $"file{index}");

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}