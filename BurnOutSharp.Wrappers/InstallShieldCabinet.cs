using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BurnOutSharp.Wrappers
{
    public partial class InstallShieldCabinet : WrapperBase
    {
        #region Pass-Through Properties

        #region Common Header

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.Signature"/>
        public string Signature => _cabinet.CommonHeader.Signature;

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.Version"/>
        public uint Version => _cabinet.CommonHeader.Version;

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.VolumeInfo"/>
        public uint VolumeInfo => _cabinet.CommonHeader.VolumeInfo;

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.CabDescriptorOffset"/>
        public uint CabDescriptorOffset => _cabinet.CommonHeader.CabDescriptorOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.CabDescriptorSize"/>
        public uint CabDescriptorSize => _cabinet.CommonHeader.CabDescriptorSize;

        #endregion

        #region Volume Header

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.DataOffset"/>
        public uint DataOffset => _cabinet.VolumeHeader.DataOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.DataOffsetHigh"/>
        public uint DataOffsetHigh => _cabinet.VolumeHeader.DataOffsetHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileIndex"/>
        public uint FirstFileIndex => _cabinet.VolumeHeader.FirstFileIndex;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileIndex"/>
        public uint LastFileIndex => _cabinet.VolumeHeader.LastFileIndex;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileOffset"/>
        public uint FirstFileOffset => _cabinet.VolumeHeader.FirstFileOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileOffsetHigh"/>
        public uint FirstFileOffsetHigh => _cabinet.VolumeHeader.FirstFileOffsetHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeExpanded"/>
        public uint FirstFileSizeExpanded => _cabinet.VolumeHeader.FirstFileSizeExpanded;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeExpandedHigh"/>
        public uint FirstFileSizeExpandedHigh => _cabinet.VolumeHeader.FirstFileSizeExpandedHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeCompressed"/>
        public uint FirstFileSizeCompressed => _cabinet.VolumeHeader.FirstFileSizeCompressed;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeCompressedHigh"/>
        public uint FirstFileSizeCompressedHigh => _cabinet.VolumeHeader.FirstFileSizeCompressedHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileOffset"/>
        public uint LastFileOffset => _cabinet.VolumeHeader.LastFileOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileOffsetHigh"/>
        public uint LastFileOffsetHigh => _cabinet.VolumeHeader.LastFileOffsetHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeExpanded"/>
        public uint LastFileSizeExpanded => _cabinet.VolumeHeader.LastFileSizeExpanded;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeExpandedHigh"/>
        public uint LastFileSizeExpandedHigh => _cabinet.VolumeHeader.LastFileSizeExpandedHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeCompressed"/>
        public uint LastFileSizeCompressed => _cabinet.VolumeHeader.LastFileSizeCompressed;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeCompressedHigh"/>
        public uint LastFileSizeCompressedHigh => _cabinet.VolumeHeader.LastFileSizeCompressedHigh;

        #endregion

        #region File Descriptor Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileDescriptorOffsets"/>
        public uint[] FileDescriptorOffsets => _cabinet.FileDescriptorOffsets;

        #endregion

        #region Directory Descriptors

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.DirectoryDescriptors"/>
        public Models.InstallShieldCabinet.FileDescriptor[] DirectoryDescriptors => _cabinet.DirectoryDescriptors;

        #endregion

        #region File Descriptors

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileDescriptors"/>
        public Models.InstallShieldCabinet.FileDescriptor[] FileDescriptors => _cabinet.FileDescriptors;

        #endregion

        #region File Group Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileGroupOffsets"/>
        public Dictionary<long, Models.InstallShieldCabinet.OffsetList> FileGroupOffsets => _cabinet.FileGroupOffsets;

        #endregion

        #region File Groups

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileGroups"/>
        public Models.InstallShieldCabinet.FileGroup[] FileGroups => _cabinet.FileGroups;

        #endregion

        #region Component Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.ComponentOffsets"/>
        public Dictionary<long, Models.InstallShieldCabinet.OffsetList> ComponentOffsets => _cabinet.ComponentOffsets;

        #endregion

        #region Components

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.Components"/>
        public Models.InstallShieldCabinet.Component[] Components => _cabinet.Components;

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// The major version of the cabinet
        /// </summary>
        public int MajorVersion
        {
            get
            {
                uint majorVersion = Version;
                if (majorVersion >> 24 == 1)
                {
                    majorVersion = (majorVersion >> 12) & 0x0F;
                }
                else if (majorVersion >> 24 == 2 || majorVersion >> 24 == 4)
                {
                    majorVersion = majorVersion & 0xFFFF;
                    if (majorVersion != 0)
                        majorVersion /= 100;
                }

                return (int)majorVersion;
            }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the cabinet
        /// </summary>
        private Models.InstallShieldCabinet.Cabinet _cabinet;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private InstallShieldCabinet() { }

        /// <summary>
        /// Create an InstallShield Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static InstallShieldCabinet Create(byte[] data, int offset)
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
        /// Create a InstallShield Cabinet from a Stream
        /// </summary>
        /// <param name="data">Stream representing the cabinet</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static InstallShieldCabinet Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var cabinet = Builders.InstallShieldCabinet.ParseCabinet(data);
            if (cabinet == null)
                return null;

            var wrapper = new InstallShieldCabinet
            {
                _cabinet = cabinet,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("InstallShield Cabinet Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Headers
            PrintCommonHeader(builder);
            PrintVolumeHeader(builder);

            // File Descriptors
            PrintFileDescriptorOffsets(builder);
            PrintDirectoryDescriptors(builder);
            PrintFileDescriptors(builder);

            // File Groups
            PrintFileGroupOffsets(builder);
            PrintFileGroups(builder);

            // Components
            PrintComponentOffsets(builder);
            PrintComponents(builder);

            return builder;
        }

        /// <summary>
        /// Print common header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintCommonHeader(StringBuilder builder)
        {
            builder.AppendLine("  Common Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Version: {Version} (0x{Version:X}) [{MajorVersion}]");
            builder.AppendLine($"  Volume info: {VolumeInfo} (0x{VolumeInfo:X})");
            builder.AppendLine($"  Cabinet descriptor offset: {CabDescriptorOffset} (0x{CabDescriptorOffset:X})");
            builder.AppendLine($"  Cabinet descriptor size: {CabDescriptorSize} (0x{CabDescriptorSize:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print volume header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintVolumeHeader(StringBuilder builder)
        {
            builder.AppendLine("  Volume Header Information:");
            builder.AppendLine("  -------------------------");
            if (MajorVersion <= 5)
            {
                builder.AppendLine($"  Data offset: {DataOffset} (0x{DataOffset:X})");
                builder.AppendLine($"  First file index: {FirstFileIndex} (0x{FirstFileIndex:X})");
                builder.AppendLine($"  Last file index: {LastFileIndex} (0x{LastFileIndex:X})");
                builder.AppendLine($"  First file offset: {FirstFileOffset} (0x{FirstFileOffset:X})");
                builder.AppendLine($"  First file size expanded: {FirstFileSizeExpanded} (0x{FirstFileSizeExpanded:X})");
                builder.AppendLine($"  First file size compressed: {FirstFileSizeCompressed} (0x{FirstFileSizeCompressed:X})");
                builder.AppendLine($"  Last file offset: {LastFileOffset} (0x{LastFileOffset:X})");
                builder.AppendLine($"  Last file size expanded: {LastFileSizeExpanded} (0x{LastFileSizeExpanded:X})");
                builder.AppendLine($"  Last file size compressed: {LastFileSizeCompressed} (0x{LastFileSizeCompressed:X})");
            }
            else
            {
                // TODO: Should standard and high values be combined?
                builder.AppendLine($"  Data offset: {DataOffset} (0x{DataOffset:X})");
                builder.AppendLine($"  Data offset high: {DataOffsetHigh} (0x{DataOffsetHigh:X})");
                builder.AppendLine($"  First file index: {FirstFileIndex} (0x{FirstFileIndex:X})");
                builder.AppendLine($"  Last file index: {LastFileIndex} (0x{LastFileIndex:X})");
                builder.AppendLine($"  First file offset: {FirstFileOffset} (0x{FirstFileOffset:X})");
                builder.AppendLine($"  First file offset high: {FirstFileOffsetHigh} (0x{FirstFileOffsetHigh:X})");
                builder.AppendLine($"  First file size expanded: {FirstFileSizeExpanded} (0x{FirstFileSizeExpanded:X})");
                builder.AppendLine($"  First file size expanded high: {FirstFileSizeExpandedHigh} (0x{FirstFileSizeExpandedHigh:X})");
                builder.AppendLine($"  First file size compressed: {FirstFileSizeCompressed} (0x{FirstFileSizeCompressed:X})");
                builder.AppendLine($"  First file size compressed high: {FirstFileSizeCompressedHigh} (0x{FirstFileSizeCompressedHigh:X})");
                builder.AppendLine($"  Last file offset: {LastFileOffset} (0x{LastFileOffset:X})");
                builder.AppendLine($"  Last file offset high: {LastFileOffsetHigh} (0x{LastFileOffsetHigh:X})");
                builder.AppendLine($"  Last file size expanded: {LastFileSizeExpanded} (0x{LastFileSizeExpanded:X})");
                builder.AppendLine($"  Last file size expanded high: {LastFileSizeExpandedHigh} (0x{LastFileSizeExpandedHigh:X})");
                builder.AppendLine($"  Last file size compressed: {LastFileSizeCompressed} (0x{LastFileSizeCompressed:X})");
                builder.AppendLine($"  Last file size compressed high: {LastFileSizeCompressedHigh} (0x{LastFileSizeCompressedHigh:X})");
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print file descriptor offsets information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFileDescriptorOffsets(StringBuilder builder)
        {
            builder.AppendLine("  File Descriptor Offsets:");
            builder.AppendLine("  -------------------------");
            if (FileDescriptorOffsets == null || FileDescriptorOffsets.Length == 0)
            {
                builder.AppendLine("  No file descriptor offsets");
            }
            else
            {
                for (int i = 0; i < FileDescriptorOffsets.Length; i++)
                {
                    builder.AppendLine($"    File Descriptor Offset {i}: {FileDescriptorOffsets[i]} (0x{FileDescriptorOffsets[i]:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory descriptors information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryDescriptors(StringBuilder builder)
        {
            builder.AppendLine("  Directory Descriptors:");
            builder.AppendLine("  -------------------------");
            if (DirectoryDescriptors == null || DirectoryDescriptors.Length == 0)
            {
                builder.AppendLine("  No directory descriptors");
            }
            else
            {
                for (int i = 0; i < DirectoryDescriptors.Length; i++)
                {
                    var directoryDescriptor = DirectoryDescriptors[i];
                    builder.AppendLine($"  Directory Descriptor {i}:");
                    builder.AppendLine($"    Name offset: {directoryDescriptor.NameOffset} (0x{directoryDescriptor.NameOffset:X})");
                    builder.AppendLine($"    Name: {directoryDescriptor.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Directory index: {directoryDescriptor.DirectoryIndex} (0x{directoryDescriptor.DirectoryIndex:X})");
                    builder.AppendLine($"    Flags: {directoryDescriptor.Flags} (0x{directoryDescriptor.Flags:X})");
                    builder.AppendLine($"    Expanded size: {directoryDescriptor.ExpandedSize} (0x{directoryDescriptor.ExpandedSize:X})");
                    builder.AppendLine($"    Compressed size: {directoryDescriptor.CompressedSize} (0x{directoryDescriptor.CompressedSize:X})");
                    builder.AppendLine($"    Data offset: {directoryDescriptor.DataOffset} (0x{directoryDescriptor.DataOffset:X})");
                    builder.AppendLine($"    MD5: {BitConverter.ToString(directoryDescriptor.MD5 ?? new byte[0]).Replace('-', ' ')}");
                    builder.AppendLine($"    Volume: {directoryDescriptor.Volume} (0x{directoryDescriptor.Volume:X})");
                    builder.AppendLine($"    Link previous: {directoryDescriptor.LinkPrevious} (0x{directoryDescriptor.LinkPrevious:X})");
                    builder.AppendLine($"    Link next: {directoryDescriptor.LinkNext} (0x{directoryDescriptor.LinkNext:X})");
                    builder.AppendLine($"    Link flags: {directoryDescriptor.LinkFlags} (0x{directoryDescriptor.LinkFlags:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print file descriptors information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFileDescriptors(StringBuilder builder)
        {
            builder.AppendLine("  File Descriptors:");
            builder.AppendLine("  -------------------------");
            if (FileDescriptors == null || FileDescriptors.Length == 0)
            {
                builder.AppendLine("  No file descriptors");
            }
            else
            {
                for (int i = 0; i < FileDescriptors.Length; i++)
                {
                    var fileDescriptor = FileDescriptors[i];
                    builder.AppendLine($"  File Descriptor {i}:");
                    builder.AppendLine($"    Name offset: {fileDescriptor.NameOffset} (0x{fileDescriptor.NameOffset:X})");
                    builder.AppendLine($"    Name: {fileDescriptor.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Directory index: {fileDescriptor.DirectoryIndex} (0x{fileDescriptor.DirectoryIndex:X})");
                    builder.AppendLine($"    Flags: {fileDescriptor.Flags} (0x{fileDescriptor.Flags:X})");
                    builder.AppendLine($"    Expanded size: {fileDescriptor.ExpandedSize} (0x{fileDescriptor.ExpandedSize:X})");
                    builder.AppendLine($"    Compressed size: {fileDescriptor.CompressedSize} (0x{fileDescriptor.CompressedSize:X})");
                    builder.AppendLine($"    Data offset: {fileDescriptor.DataOffset} (0x{fileDescriptor.DataOffset:X})");
                    builder.AppendLine($"    MD5: {BitConverter.ToString(fileDescriptor.MD5 ?? new byte[0]).Replace('-', ' ')}");
                    builder.AppendLine($"    Volume: {fileDescriptor.Volume} (0x{fileDescriptor.Volume:X})");
                    builder.AppendLine($"    Link previous: {fileDescriptor.LinkPrevious} (0x{fileDescriptor.LinkPrevious:X})");
                    builder.AppendLine($"    Link next: {fileDescriptor.LinkNext} (0x{fileDescriptor.LinkNext:X})");
                    builder.AppendLine($"    Link flags: {fileDescriptor.LinkFlags} (0x{fileDescriptor.LinkFlags:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print file group offsets information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFileGroupOffsets(StringBuilder builder)
        {
            builder.AppendLine("  File Group Offsets:");
            builder.AppendLine("  -------------------------");
            if (FileGroupOffsets == null || FileGroupOffsets.Count == 0)
            {
                builder.AppendLine("  No file group offsets");
            }
            else
            {
                foreach (var kvp in FileGroupOffsets)
                {
                    long offset = kvp.Key;
                    var offsetList = kvp.Value;
                    builder.AppendLine($"  File Group Offset {offset}:");
                    builder.AppendLine($"    Name offset: {offsetList.NameOffset} (0x{offsetList.NameOffset:X})");
                    builder.AppendLine($"    Name: {offsetList.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Descriptor offset: {offsetList.DescriptorOffset} (0x{offsetList.DescriptorOffset:X})");
                    builder.AppendLine($"    Next offset: {offsetList.NextOffset} (0x{offsetList.NextOffset:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print file groups information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFileGroups(StringBuilder builder)
        {
            builder.AppendLine("  File Groups:");
            builder.AppendLine("  -------------------------");
            if (FileGroups == null || FileGroups.Length == 0)
            {
                builder.AppendLine("  No file groups");
            }
            else
            {
                for (int i = 0; i < FileGroups.Length; i++)
                {
                    var fileGroup = FileGroups[i];
                    builder.AppendLine($"  File Group {i}:");
                    builder.AppendLine($"    Name offset: {fileGroup.NameOffset} (0x{fileGroup.NameOffset:X})");
                    builder.AppendLine($"    Name: {fileGroup.Name ?? "[NULL]"}");
                    builder.AppendLine($"    First file: {fileGroup.FirstFile} (0x{fileGroup.FirstFile:X})");
                    builder.AppendLine($"    Last file: {fileGroup.LastFile} (0x{fileGroup.LastFile:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print component offsets information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintComponentOffsets(StringBuilder builder)
        {
            builder.AppendLine("  Component Offsets:");
            builder.AppendLine("  -------------------------");
            if (ComponentOffsets == null || ComponentOffsets.Count == 0)
            {
                builder.AppendLine("  No component offsets");
            }
            else
            {
                foreach (var kvp in ComponentOffsets)
                {
                    long offset = kvp.Key;
                    var offsetList = kvp.Value;
                    builder.AppendLine($"  Component Offset {offset}:");
                    builder.AppendLine($"    Name offset: {offsetList.NameOffset} (0x{offsetList.NameOffset:X})");
                    builder.AppendLine($"    Name: {offsetList.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Descriptor offset: {offsetList.DescriptorOffset} (0x{offsetList.DescriptorOffset:X})");
                    builder.AppendLine($"    Next offset: {offsetList.NextOffset} (0x{offsetList.NextOffset:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print components information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintComponents(StringBuilder builder)
        {
            builder.AppendLine("  Components:");
            builder.AppendLine("  -------------------------");
            if (Components == null || Components.Length == 0)
            {
                builder.AppendLine("  No components");
            }
            else
            {
                for (int i = 0; i < Components.Length; i++)
                {
                    var component = Components[i];
                    builder.AppendLine($"  Component {i}:");
                    builder.AppendLine($"    Name offset: {component.NameOffset} (0x{component.NameOffset:X})");
                    builder.AppendLine($"    Name: {component.Name ?? "[NULL]"}");
                    builder.AppendLine($"    File group count: {component.FileGroupCount} (0x{component.FileGroupCount:X})");
                    builder.AppendLine($"    File group table offset: {component.FileGroupTableOffset} (0x{component.FileGroupTableOffset:X})");
                    builder.AppendLine($"    File group names:");
                    builder.AppendLine("    -------------------------");
                    if (component.FileGroupNames == null || component.FileGroupNames.Length == 0)
                    {
                        builder.AppendLine("    No file group names");
                    }
                    else
                    {
                        for (int j = 0; j < component.FileGroupNames.Length; j++)
                        {
                            builder.AppendLine($"      File Group Name {j}: {component.FileGroupNames[j] ?? "[NULL]"}");
                        }
                    }
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_cabinet, _jsonSerializerOptions);

#endif

        #endregion
    }
}
