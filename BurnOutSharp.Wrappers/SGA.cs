using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace BurnOutSharp.Wrappers
{
    public class SGA : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.SGA.Header4.Signature"/>
        public string Signature => _file.Header.Signature;

        /// <inheritdoc cref="Models.SGA.Header4.MajorVersion"/>
        public ushort MajorVersion => _file.Header.MajorVersion;

        /// <inheritdoc cref="Models.SGA.Header4.MinorVersion"/>
        public ushort MinorVersion => _file.Header.MinorVersion;

        /// <inheritdoc cref="Models.SGA.Header4.FileMD5"/>
        public byte[] FileMD5
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_file.Header as Models.SGA.Header4).FileMD5;
                    case 5: return (_file.Header as Models.SGA.Header4).FileMD5;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.Header4.Name"/>
        public string Name
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_file.Header as Models.SGA.Header4).Name;
                    case 5: return (_file.Header as Models.SGA.Header4).Name;
                    case 6: return (_file.Header as Models.SGA.Header6).Name;
                    case 7: return (_file.Header as Models.SGA.Header6).Name;
                    default: return null;
                };
            }
        }

        /// <inheritdoc cref="Models.SGA.Header4.HeaderMD5"/>
        public byte[] HeaderMD5
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_file.Header as Models.SGA.Header4).HeaderMD5;
                    case 5: return (_file.Header as Models.SGA.Header4).HeaderMD5;
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
                    case 4: return (_file.Header as Models.SGA.Header4).HeaderLength;
                    case 5: return (_file.Header as Models.SGA.Header4).HeaderLength;
                    case 6: return (_file.Header as Models.SGA.Header6).HeaderLength;
                    case 7: return (_file.Header as Models.SGA.Header6).HeaderLength;
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
                    case 4: return (_file.Header as Models.SGA.Header4).FileDataOffset;
                    case 5: return (_file.Header as Models.SGA.Header4).FileDataOffset;
                    case 6: return (_file.Header as Models.SGA.Header6).FileDataOffset;
                    case 7: return (_file.Header as Models.SGA.Header6).FileDataOffset;
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
                    case 4: return (_file.Header as Models.SGA.Header4).Dummy0;
                    case 5: return (_file.Header as Models.SGA.Header4).Dummy0;
                    case 6: return (_file.Header as Models.SGA.Header6).Dummy0;
                    case 7: return (_file.Header as Models.SGA.Header6).Dummy0;
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
                    case 4: return (_file.Directory as Models.SGA.Directory4).DirectoryHeader.SectionOffset;
                    case 5: return (_file.Directory as Models.SGA.Directory5).DirectoryHeader.SectionOffset;
                    case 6: return (_file.Directory as Models.SGA.Directory6).DirectoryHeader.SectionOffset;
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.SectionOffset;
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
                    case 4: return (_file.Directory as Models.SGA.Directory4).DirectoryHeader.SectionCount;
                    case 5: return (_file.Directory as Models.SGA.Directory5).DirectoryHeader.SectionCount;
                    case 6: return (_file.Directory as Models.SGA.Directory6).DirectoryHeader.SectionCount;
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.SectionCount;
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
                    case 4: return (_file.Directory as Models.SGA.Directory4).DirectoryHeader.FolderOffset;
                    case 5: return (_file.Directory as Models.SGA.Directory5).DirectoryHeader.FolderOffset;
                    case 6: return (_file.Directory as Models.SGA.Directory6).DirectoryHeader.FolderOffset;
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.FolderOffset;
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
                    case 4: return (_file.Directory as Models.SGA.Directory4).DirectoryHeader.FolderCount;
                    case 5: return (_file.Directory as Models.SGA.Directory5).DirectoryHeader.FolderCount;
                    case 6: return (_file.Directory as Models.SGA.Directory6).DirectoryHeader.FolderCount;
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.FolderCount;
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
                    case 4: return (_file.Directory as Models.SGA.Directory4).DirectoryHeader.FileOffset;
                    case 5: return (_file.Directory as Models.SGA.Directory5).DirectoryHeader.FileOffset;
                    case 6: return (_file.Directory as Models.SGA.Directory6).DirectoryHeader.FileOffset;
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.FileOffset;
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
                    case 4: return (_file.Directory as Models.SGA.Directory4).DirectoryHeader.FileCount;
                    case 5: return (_file.Directory as Models.SGA.Directory5).DirectoryHeader.FileCount;
                    case 6: return (_file.Directory as Models.SGA.Directory6).DirectoryHeader.FileCount;
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.FileCount;
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
                    case 4: return (_file.Directory as Models.SGA.Directory4).DirectoryHeader.StringTableOffset;
                    case 5: return (_file.Directory as Models.SGA.Directory5).DirectoryHeader.StringTableOffset;
                    case 6: return (_file.Directory as Models.SGA.Directory6).DirectoryHeader.StringTableOffset;
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.StringTableOffset;
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
                    case 4: return (_file.Directory as Models.SGA.Directory4).DirectoryHeader.StringTableCount;
                    case 5: return (_file.Directory as Models.SGA.Directory5).DirectoryHeader.StringTableCount;
                    case 6: return (_file.Directory as Models.SGA.Directory6).DirectoryHeader.StringTableCount;
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.StringTableCount;
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
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.HashTableOffset;
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
                    case 7: return (_file.Directory as Models.SGA.Directory7).DirectoryHeader.BlockSize;
                    default: return null;
                };
            }
        }

        #endregion

        #region Sections

        /// <inheritdoc cref="Models.SGA.SpecializedDirectory{THeader, TDirectoryHeader, TSection, TFolder, TFile, U}.Sections"/>
        public object[] Sections
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_file.Directory as Models.SGA.Directory4).Sections;
                    case 5: return (_file.Directory as Models.SGA.Directory5).Sections;
                    case 6: return (_file.Directory as Models.SGA.Directory6).Sections;
                    case 7: return (_file.Directory as Models.SGA.Directory7).Sections;
                    default: return null;
                };
            }
        }

        #endregion

        #region Folders

        /// <inheritdoc cref="Models.SGA.SpecializedDirectory{THeader, TDirectoryHeader, TSection, TFolder, TFile, U}.Folders"/>
        public object[] Folders
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_file.Directory as Models.SGA.Directory4).Folders;
                    case 5: return (_file.Directory as Models.SGA.Directory5).Folders;
                    case 6: return (_file.Directory as Models.SGA.Directory6).Folders;
                    case 7: return (_file.Directory as Models.SGA.Directory7).Folders;
                    default: return null;
                };
            }
        }

        #endregion

        #region Files

        /// <inheritdoc cref="Models.SGA.SpecializedDirectory{THeader, TDirectoryHeader, TSection, TFolder, TFile, U}.Files"/>
        public object[] Files
        {
            get
            {
                switch (MajorVersion)
                {
                    case 4: return (_file.Directory as Models.SGA.Directory4).Files;
                    case 5: return (_file.Directory as Models.SGA.Directory5).Files;
                    case 6: return (_file.Directory as Models.SGA.Directory6).Files;
                    case 7: return (_file.Directory as Models.SGA.Directory7).Files;
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

        #region Instance Variables

        /// <summary>
        /// Internal representation of the SGA
        /// </summary>
        private Models.SGA.File _file;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private SGA() { }

        /// <summary>
        /// Create an SGA from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the SGA</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
        public static SGA Create(byte[] data, int offset)
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
        public static SGA Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Builders.SGA.ParseFile(data);
            if (file == null)
                return null;

            var wrapper = new SGA
            {
                _file = file,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("SGA Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            // Header
            PrintHeader();

            // Directory
            PrintDirectoryHeader();
            PrintSections();
            PrintFolders();
            PrintFiles();
            // TODO: Should we print the string table?
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  Major version: {MajorVersion} (0x{MajorVersion:X})");
            Console.WriteLine($"  Minor version: {MinorVersion} (0x{MinorVersion:X})");
            Console.WriteLine($"  File MD5: {(FileMD5 == null ? "[NULL]" : BitConverter.ToString(FileMD5).Replace("-", string.Empty))}");
            Console.WriteLine($"  Name: {Name ?? "[NULL]"}");
            Console.WriteLine($"  Header MD5: {(HeaderMD5 == null ? "[NULL]" : BitConverter.ToString(HeaderMD5).Replace("-", string.Empty))}");
            Console.WriteLine($"  Header length: {HeaderLength?.ToString() ?? "[NULL]"} (0x{HeaderLength?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  File data offset: {FileDataOffset?.ToString() ?? "[NULL]"} (0x{FileDataOffset?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  Dummy 0: {Dummy0?.ToString() ?? "[NULL]"} (0x{Dummy0?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory header information
        /// </summary>
        private void PrintDirectoryHeader()
        {
            Console.WriteLine("  Directory Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Section offset: {SectionOffset?.ToString() ?? "[NULL]"} (0x{SectionOffset?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  Section count: {SectionCount?.ToString() ?? "[NULL]"} (0x{SectionCount?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  Folder offset: {FolderOffset?.ToString() ?? "[NULL]"} (0x{FolderOffset?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  Folder count: {FolderCount?.ToString() ?? "[NULL]"} (0x{FolderCount?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  File offset: {FileOffset?.ToString() ?? "[NULL]"} (0x{FileOffset?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  File count: {FileCount?.ToString() ?? "[NULL]"} (0x{FileCount?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  String table offset: {StringTableOffset?.ToString() ?? "[NULL]"} (0x{StringTableOffset?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  String table count: {StringTableCount?.ToString() ?? "[NULL]"} (0x{StringTableCount?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  Hash table offset: {HashTableOffset?.ToString() ?? "[NULL]"} (0x{HashTableOffset?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine($"  Block size: {BlockSize?.ToString() ?? "[NULL]"} (0x{BlockSize?.ToString("X") ?? "[NULL]"})");
            Console.WriteLine();
        }

        /// <summary>
        /// Print sections information
        /// </summary>
        private void PrintSections()
        {
            Console.WriteLine("  Sections Information:");
            Console.WriteLine("  -------------------------");
            if (Sections == null || Sections.Length == 0)
            {
                Console.WriteLine("  No sections");
            }
            else
            {
                for (int i = 0; i < Sections.Length; i++)
                {
                    Console.WriteLine($"  Section {i}");
                    switch (MajorVersion)
                    {
                        case 4:
                            var section4 = Sections[i] as Models.SGA.Section4;
                            Console.WriteLine($"    Alias: {section4.Alias ?? "[NULL]"}");
                            Console.WriteLine($"    Name: {section4.Name ?? "[NULL]"}");
                            Console.WriteLine($"    Folder start index: {section4.FolderStartIndex} (0x{section4.FolderStartIndex:X})");
                            Console.WriteLine($"    Folder end index: {section4.FolderEndIndex} (0x{section4.FolderEndIndex:X})");
                            Console.WriteLine($"    File start index: {section4.FileStartIndex} (0x{section4.FileStartIndex:X})");
                            Console.WriteLine($"    File end index: {section4.FileEndIndex} (0x{section4.FileEndIndex:X})");
                            Console.WriteLine($"    Folder root index: {section4.FolderRootIndex} (0x{section4.FolderRootIndex:X})");
                            break;

                        case 5:
                        case 6:
                        case 7:
                            var section5 = Sections[i] as Models.SGA.Section5;
                            Console.WriteLine($"    Alias: {section5.Alias ?? "[NULL]"}");
                            Console.WriteLine($"    Name: {section5.Name ?? "[NULL]"}");
                            Console.WriteLine($"    Folder start index: {section5.FolderStartIndex} (0x{section5.FolderStartIndex:X})");
                            Console.WriteLine($"    Folder end index: {section5.FolderEndIndex} (0x{section5.FolderEndIndex:X})");
                            Console.WriteLine($"    File start index: {section5.FileStartIndex} (0x{section5.FileStartIndex:X})");
                            Console.WriteLine($"    File end index: {section5.FileEndIndex} (0x{section5.FileEndIndex:X})");
                            Console.WriteLine($"    Folder root index: {section5.FolderRootIndex} (0x{section5.FolderRootIndex:X})");
                            break;
                        default:
                            Console.WriteLine($"    Unknown format for version {MajorVersion}");
                            break;
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print folders information
        /// </summary>
        private void PrintFolders()
        {
            Console.WriteLine("  Folders Information:");
            Console.WriteLine("  -------------------------");
            if (Folders == null || Folders.Length == 0)
            {
                Console.WriteLine("  No folders");
            }
            else
            {
                for (int i = 0; i < Folders.Length; i++)
                {
                    Console.WriteLine($"  Folder {i}");
                    switch (MajorVersion)
                    {
                        case 4:
                            var folder4 = Folders[i] as Models.SGA.Folder4;
                            Console.WriteLine($"    Name offset: {folder4.NameOffset} (0x{folder4.NameOffset:X})");
                            Console.WriteLine($"    Name: {folder4.Name ?? "[NULL]"}");
                            Console.WriteLine($"    Folder start index: {folder4.FolderStartIndex} (0x{folder4.FolderStartIndex:X})");
                            Console.WriteLine($"    Folder end index: {folder4.FolderEndIndex} (0x{folder4.FolderEndIndex:X})");
                            Console.WriteLine($"    File start index: {folder4.FileStartIndex} (0x{folder4.FileStartIndex:X})");
                            Console.WriteLine($"    File end index: {folder4.FileEndIndex} (0x{folder4.FileEndIndex:X})");
                            break;

                        case 5:
                        case 6:
                        case 7:
                            var folder5 = Folders[i] as Models.SGA.Folder5;
                            Console.WriteLine($"    Name offset: {folder5.NameOffset} (0x{folder5.NameOffset:X})");
                            Console.WriteLine($"    Name: {folder5.Name ?? "[NULL]"}");
                            Console.WriteLine($"    Folder start index: {folder5.FolderStartIndex} (0x{folder5.FolderStartIndex:X})");
                            Console.WriteLine($"    Folder end index: {folder5.FolderEndIndex} (0x{folder5.FolderEndIndex:X})");
                            Console.WriteLine($"    File start index: {folder5.FileStartIndex} (0x{folder5.FileStartIndex:X})");
                            Console.WriteLine($"    File end index: {folder5.FileEndIndex} (0x{folder5.FileEndIndex:X})");
                            break;
                        default:
                            Console.WriteLine($"    Unknown format for version {MajorVersion}");
                            break;
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print files information
        /// </summary>
        private void PrintFiles()
        {
            Console.WriteLine("  Files Information:");
            Console.WriteLine("  -------------------------");
            if (Files == null || Files.Length == 0)
            {
                Console.WriteLine("  No files");
            }
            else
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    Console.WriteLine($"  File {i}");
                    switch (MajorVersion)
                    {
                        case 4:
                        case 5:
                            var file4 = Files[i] as Models.SGA.File4;
                            Console.WriteLine($"    Name offset: {file4.NameOffset} (0x{file4.NameOffset:X})");
                            Console.WriteLine($"    Name: {file4.Name ?? "[NULL]"}");
                            Console.WriteLine($"    Offset: {file4.Offset} (0x{file4.Offset:X})");
                            Console.WriteLine($"    Size on disk: {file4.SizeOnDisk} (0x{file4.SizeOnDisk:X})");
                            Console.WriteLine($"    Size: {file4.Size} (0x{file4.Size:X})");
                            Console.WriteLine($"    Time modified: {file4.TimeModified} (0x{file4.TimeModified:X})");
                            Console.WriteLine($"    Dummy 0: {file4.Dummy0} (0x{file4.Dummy0:X})");
                            Console.WriteLine($"    Type: {file4.Type} (0x{file4.Type:X})");
                            break;

                        case 6:
                            var file6 = Files[i] as Models.SGA.File6;
                            Console.WriteLine($"    Name offset: {file6.NameOffset} (0x{file6.NameOffset:X})");
                            Console.WriteLine($"    Name: {file6.Name ?? "[NULL]"}");
                            Console.WriteLine($"    Offset: {file6.Offset} (0x{file6.Offset:X})");
                            Console.WriteLine($"    Size on disk: {file6.SizeOnDisk} (0x{file6.SizeOnDisk:X})");
                            Console.WriteLine($"    Size: {file6.Size} (0x{file6.Size:X})");
                            Console.WriteLine($"    Time modified: {file6.TimeModified} (0x{file6.TimeModified:X})");
                            Console.WriteLine($"    Dummy 0: {file6.Dummy0} (0x{file6.Dummy0:X})");
                            Console.WriteLine($"    Type: {file6.Type} (0x{file6.Type:X})");
                            Console.WriteLine($"    CRC32: {file6.CRC32} (0x{file6.CRC32:X})");
                            break;
                        case 7:
                            var file7 = Files[i] as Models.SGA.File7;
                            Console.WriteLine($"    Name offset: {file7.NameOffset} (0x{file7.NameOffset:X})");
                            Console.WriteLine($"    Name: {file7.Name ?? "[NULL]"}");
                            Console.WriteLine($"    Offset: {file7.Offset} (0x{file7.Offset:X})");
                            Console.WriteLine($"    Size on disk: {file7.SizeOnDisk} (0x{file7.SizeOnDisk:X})");
                            Console.WriteLine($"    Size: {file7.Size} (0x{file7.Size:X})");
                            Console.WriteLine($"    Time modified: {file7.TimeModified} (0x{file7.TimeModified:X})");
                            Console.WriteLine($"    Dummy 0: {file7.Dummy0} (0x{file7.Dummy0:X})");
                            Console.WriteLine($"    Type: {file7.Type} (0x{file7.Type:X})");
                            Console.WriteLine($"    CRC32: {file7.CRC32} (0x{file7.CRC32:X})");
                            Console.WriteLine($"    Hash offset: {file7.HashOffset} (0x{file7.HashOffset:X})");
                            break;
                        default:
                            Console.WriteLine($"    Unknown format for version {MajorVersion}");
                            break;
                    }
                }
            }
            Console.WriteLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_file, _jsonSerializerOptions);

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
            string filename;
            switch (MajorVersion)
            {
                case 4:
                case 5: filename = (file as Models.SGA.File4).Name; break;
                case 6: filename = (file as Models.SGA.File6).Name; break;
                case 7: filename = (file as Models.SGA.File7).Name; break;
                default: return false;
            }

            // Loop through and get all parent directories
            var parentNames = new List<string> { filename };

            // Get the parent directory
            object folder;
            switch (MajorVersion)
            {
                case 4: folder = (Folders as Models.SGA.Folder4[]).FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                case 5:
                case 6:
                case 7: folder = (Folders as Models.SGA.Folder5[]).FirstOrDefault(f => index >= f.FileStartIndex && index <= f.FileEndIndex); break;
                default: return false;
            }

            // If we have a parent folder
            if (folder != null)
            {
                switch (MajorVersion)
                {
                    case 4: parentNames.Add((folder as Models.SGA.Folder4).Name); break;
                    case 5:
                    case 6:
                    case 7: parentNames.Add((folder as Models.SGA.Folder5).Name); break;
                    default: return false;
                }
            }

            // TODO: Should the section name/alias be used in the path as well?

            // Reverse and assemble the filename
            parentNames.Reverse();
            filename = Path.Combine(parentNames.ToArray());

            // Get the file offset
            long fileOffset;
            switch (MajorVersion)
            {
                case 4:
                case 5: fileOffset = (file as Models.SGA.File4).Offset; break;
                case 6: fileOffset = (file as Models.SGA.File6).Offset; break;
                case 7: fileOffset = (file as Models.SGA.File7).Offset; break;
                default: return false;
            }

            // Adjust the file offset
            fileOffset += FileDataOffset.Value;

            // Get the file sizes
            long fileSize, outputFileSize;
            switch (MajorVersion)
            {
                case 4:
                case 5:
                    fileSize = (file as Models.SGA.File4).SizeOnDisk;
                    outputFileSize = (file as Models.SGA.File4).Size;
                    break;
                case 6:
                    fileSize = (file as Models.SGA.File6).SizeOnDisk;
                    outputFileSize = (file as Models.SGA.File6).Size;
                    break;
                case 7:
                    fileSize = (file as Models.SGA.File7).SizeOnDisk;
                    outputFileSize = (file as Models.SGA.File7).Size;
                    break;
                default: return false;
            }

            // Read the compressed data directly
            byte[] compressedData = ReadFromDataSource((int)fileOffset, (int)fileSize);
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
            Directory.CreateDirectory(Path.GetDirectoryName(filename));

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