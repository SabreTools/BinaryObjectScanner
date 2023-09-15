using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace BinaryObjectScanner.Wrappers
{
    public class SGA : WrapperBase<SabreTools.Models.SGA.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "SGA";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.SGA.Header4.Signature"/>
#if NET48
        public string Signature => _model.Header.Signature;
#else
        public string? Signature => _model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.SGA.Header4.MajorVersion"/>
#if NET48
        public ushort MajorVersion => _model.Header.MajorVersion;
#else
        public ushort? MajorVersion => _model.Header?.MajorVersion;
#endif

        /// <inheritdoc cref="Models.SGA.Header4.MinorVersion"/>
#if NET48
        public ushort MinorVersion => _model.Header.MinorVersion;
#else
        public ushort? MinorVersion => _model.Header?.MinorVersion;
#endif

        /// <inheritdoc cref="Models.SGA.Header4.FileMD5"/>
#if NET48
        public byte[] FileMD5
#else
        public byte[]? FileMD5
#endif
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Header as SabreTools.Models.SGA.Header4)?.FileMD5;
                    case 5: return (_model.Header as SabreTools.Models.SGA.Header4)?.FileMD5;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.Header4.Name"/>
#if NET48
        public string Name
#else
        public string? Name
#endif
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Header as SabreTools.Models.SGA.Header4)?.Name;
                    case 5: return (_model.Header as SabreTools.Models.SGA.Header4)?.Name;
                    case 6: return (_model.Header as SabreTools.Models.SGA.Header6)?.Name;
                    case 7: return (_model.Header as SabreTools.Models.SGA.Header6)?.Name;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.Header4.HeaderMD5"/>
#if NET48
        public byte[] HeaderMD5
#else
        public byte[]? HeaderMD5
#endif
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Header as SabreTools.Models.SGA.Header4)?.HeaderMD5;
                    case 5: return (_model.Header as SabreTools.Models.SGA.Header4)?.HeaderMD5;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.Header4.HeaderLength"/>
        public uint? HeaderLength
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Header as SabreTools.Models.SGA.Header4)?.HeaderLength;
                    case 5: return (_model.Header as SabreTools.Models.SGA.Header4)?.HeaderLength;
                    case 6: return (_model.Header as SabreTools.Models.SGA.Header6)?.HeaderLength;
                    case 7: return (_model.Header as SabreTools.Models.SGA.Header6)?.HeaderLength;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.Header4.FileDataOffset"/>
        public uint? FileDataOffset
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Header as SabreTools.Models.SGA.Header4)?.FileDataOffset;
                    case 5: return (_model.Header as SabreTools.Models.SGA.Header4)?.FileDataOffset;
                    case 6: return (_model.Header as SabreTools.Models.SGA.Header6)?.FileDataOffset;
                    case 7: return (_model.Header as SabreTools.Models.SGA.Header6)?.FileDataOffset;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.Header4.Dummy0"/>
        public uint? Dummy0
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Header as SabreTools.Models.SGA.Header4)?.Dummy0;
                    case 5: return (_model.Header as SabreTools.Models.SGA.Header4)?.Dummy0;
                    case 6: return (_model.Header as SabreTools.Models.SGA.Header6)?.Dummy0;
                    case 7: return (_model.Header as SabreTools.Models.SGA.Header6)?.Dummy0;
                    default: return null;
                };
            }
        }

        #endregion

        #region Directory

        #region Directory Header

        /// <inheritdoc cref="Models.SGA.DirectoryHeader{T}.SectionOffset"/>
        public uint? SectionOffset
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.DirectoryHeader?.SectionOffset;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.DirectoryHeader?.SectionOffset;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.DirectoryHeader?.SectionOffset;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.SectionOffset;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader{T}.SectionCount"/>
        public uint? SectionCount
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.DirectoryHeader?.SectionCount;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.DirectoryHeader?.SectionCount;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.DirectoryHeader?.SectionCount;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.SectionCount;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader{T}.FolderOffset"/>
        public uint? FolderOffset
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.DirectoryHeader?.FolderOffset;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.DirectoryHeader?.FolderOffset;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.DirectoryHeader?.FolderOffset;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.FolderOffset;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader{T}.FolderCount"/>
        public uint? FolderCount
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.DirectoryHeader?.FolderCount;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.DirectoryHeader?.FolderCount;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.DirectoryHeader?.FolderCount;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.FolderCount;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader{T}.FileOffset"/>
        public uint? FileOffset
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.DirectoryHeader?.FileOffset;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.DirectoryHeader?.FileOffset;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.DirectoryHeader?.FileOffset;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.FileOffset;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader{T}.FileCount"/>
        public uint? FileCount
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.DirectoryHeader?.FileCount;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.DirectoryHeader?.FileCount;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.DirectoryHeader?.FileCount;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.FileCount;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader{T}.StringTableOffset"/>
        public uint? StringTableOffset
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.DirectoryHeader?.StringTableOffset;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.DirectoryHeader?.StringTableOffset;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.DirectoryHeader?.StringTableOffset;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.StringTableOffset;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader{T}.StringTableCount"/>
        public uint? StringTableCount
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.DirectoryHeader?.StringTableCount;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.DirectoryHeader?.StringTableCount;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.DirectoryHeader?.StringTableCount;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.StringTableCount;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader7.HashTableOffset"/>
        public uint? HashTableOffset
        {
            get
            {
                switch (MajorVersion)
                {
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.HashTableOffset;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.DirectoryHeader7.BlockSize"/>
        public uint? BlockSize
        {
            get
            {
                switch (MajorVersion)
                {
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.DirectoryHeader?.BlockSize;
                    default: return null;
                };
            }
        }

        #endregion

        #region Sections

        /// <inheritdoc cref="Models.SGA.SpecializedDirectory{THeader, TDirectoryHeader, TSection, TFolder, TFile, U}.Sections"/>
#if NET48
        public object[] Sections
#else
        public object?[]? Sections
#endif
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.Sections;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.Sections;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.Sections;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.Sections;
                    default: return null;
                };
            }
        }

        #endregion

        #region Folders

        /// <inheritdoc cref="Models.SGA.SpecializedDirectory{THeader, TDirectoryHeader, TSection, TFolder, TFile, U}.Folders"/>
#if NET48
        public object[] Folders
#else
        public object?[]? Folders
#endif
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.Folders;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.Folders;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.Folders;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.Folders;
                    default: return null;
                };
            }
        }

        #endregion

        #region Files

        /// <inheritdoc cref="Models.SGA.SpecializedDirectory{THeader, TDirectoryHeader, TSection, TFolder, TFile, U}.Files"/>
#if NET48
        public object[] Files
#else
        public object?[]? Files
#endif
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_model.Directory as SabreTools.Models.SGA.Directory4)?.Files;
                    case 5: return (_model.Directory as SabreTools.Models.SGA.Directory5)?.Files;
                    case 6: return (_model.Directory as SabreTools.Models.SGA.Directory6)?.Files;
                    case 7: return (_model.Directory as SabreTools.Models.SGA.Directory7)?.Files;
                    default: return null;
                };
            }
        }

        #endregion

        // TODO: Figure out how to deal with all of the parts of the directory
        // TODO: Should anything be passed through?

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Figure out what extension oroperties are needed

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public SGA(SabreTools.Models.SGA.File model, byte[] data, int offset)
#else
        public SGA(SabreTools.Models.SGA.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public SGA(SabreTools.Models.SGA.File model, Stream data)
#else
        public SGA(SabreTools.Models.SGA.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an SGA from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the SGA</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
#if NET48
        public static SGA Create(byte[] data, int offset)
#else
        public static SGA? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a SGA from a Stream
        /// </summary>
        /// <param name="data">Stream representing the SGA</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
#if NET48
        public static SGA Create(Stream data)
#else
        public static SGA? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.SGA().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new SGA(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.SGA.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the SGA to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Files.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the SGA to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= Files.Length)
                return false;

            // Get the files
            var file = Files[index];
            if (file == null)
                return false;

            // Create the filename
#if NET48
            string filename;
#else
            string? filename;
#endif
            switch (MajorVersion)
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
            switch (MajorVersion)
            {
                case 4: folder = (Folders as SabreTools.Models.SGA.Folder4[])?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 5:
                case 6:
                case 7: folder = (Folders as SabreTools.Models.SGA.Folder5[])?.FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                default: return false;
            }

            // If we have a parent folder
            if (folder != null)
            {
                switch (MajorVersion)
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
            switch (MajorVersion)
            {
                case 4:
                case 5: fileOffset = (file as SabreTools.Models.SGA.File4)?.Offset ?? 0; break;
                case 6: fileOffset = (file as SabreTools.Models.SGA.File6)?.Offset ?? 0; break;
                case 7: fileOffset = (file as SabreTools.Models.SGA.File7)?.Offset ?? 0; break;
                default: return false;
            }

            // Adjust the file offset
            fileOffset += FileDataOffset ?? 0;

            // Get the file sizes
            long fileSize, outputFileSize;
            switch (MajorVersion)
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
            byte[] compressedData = ReadFromDataSource((int)fileOffset, (int)fileSize);
#else
            byte[]? compressedData = ReadFromDataSource((int)fileOffset, (int)fileSize);
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
    }
}