using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public partial class InstallShieldCabinet : WrapperBase<SabreTools.Models.InstallShieldCabinet.Cabinet>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Cabinet";

        #endregion

        #region Pass-Through Properties

        #region Common Header

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.Signature"/>
        public string Signature => _model.CommonHeader.Signature;

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.Version"/>
        public uint Version => _model.CommonHeader.Version;

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.VolumeInfo"/>
        public uint VolumeInfo => _model.CommonHeader.VolumeInfo;

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.DescriptorOffset"/>
        public uint DescriptorOffset => _model.CommonHeader.DescriptorOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.DescriptorSize"/>
        public uint DescriptorSize => _model.CommonHeader.DescriptorSize;

        #endregion

        #region Volume Header

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.DataOffset"/>
        public uint DataOffset => _model.VolumeHeader.DataOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.DataOffsetHigh"/>
        public uint DataOffsetHigh => _model.VolumeHeader.DataOffsetHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileIndex"/>
        public uint FirstFileIndex => _model.VolumeHeader.FirstFileIndex;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileIndex"/>
        public uint LastFileIndex => _model.VolumeHeader.LastFileIndex;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileOffset"/>
        public uint FirstFileOffset => _model.VolumeHeader.FirstFileOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileOffsetHigh"/>
        public uint FirstFileOffsetHigh => _model.VolumeHeader.FirstFileOffsetHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeExpanded"/>
        public uint FirstFileSizeExpanded => _model.VolumeHeader.FirstFileSizeExpanded;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeExpandedHigh"/>
        public uint FirstFileSizeExpandedHigh => _model.VolumeHeader.FirstFileSizeExpandedHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeCompressed"/>
        public uint FirstFileSizeCompressed => _model.VolumeHeader.FirstFileSizeCompressed;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeCompressedHigh"/>
        public uint FirstFileSizeCompressedHigh => _model.VolumeHeader.FirstFileSizeCompressedHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileOffset"/>
        public uint LastFileOffset => _model.VolumeHeader.LastFileOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileOffsetHigh"/>
        public uint LastFileOffsetHigh => _model.VolumeHeader.LastFileOffsetHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeExpanded"/>
        public uint LastFileSizeExpanded => _model.VolumeHeader.LastFileSizeExpanded;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeExpandedHigh"/>
        public uint LastFileSizeExpandedHigh => _model.VolumeHeader.LastFileSizeExpandedHigh;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeCompressed"/>
        public uint LastFileSizeCompressed => _model.VolumeHeader.LastFileSizeCompressed;

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeCompressedHigh"/>
        public uint LastFileSizeCompressedHigh => _model.VolumeHeader.LastFileSizeCompressedHigh;

        #endregion

        #region Descriptor

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.StringsOffset"/>
        public uint StringsOffset => _model.Descriptor.StringsOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved0"/>
        public byte[] Reserved0 => _model.Descriptor.Reserved0;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.ComponentListOffset"/>
        public uint ComponentListOffset => _model.Descriptor.ComponentListOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileTableOffset"/>
        public uint FileTableOffset => _model.Descriptor.FileTableOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved1"/>
        public byte[] Reserved1 => _model.Descriptor.Reserved1;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileTableSize"/>
        public uint FileTableSize => _model.Descriptor.FileTableSize;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileTableSize2"/>
        public uint FileTableSize2 => _model.Descriptor.FileTableSize2;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.DirectoryCount"/>
        public ushort DirectoryCount => _model.Descriptor.DirectoryCount;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved2"/>
        public byte[] Reserved2 => _model.Descriptor.Reserved2;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved3"/>
        public byte[] Reserved3 => _model.Descriptor.Reserved3;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved4"/>
        public byte[] Reserved4 => _model.Descriptor.Reserved4;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileCount"/>
        public uint FileCount => _model.Descriptor.FileCount;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileTableOffset2"/>
        public uint FileTableOffset2 => _model.Descriptor.FileTableOffset2;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.ComponentTableInfoCount"/>
        public ushort ComponentTableInfoCount => _model.Descriptor.ComponentTableInfoCount;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.ComponentTableOffset"/>
        public uint ComponentTableOffset => _model.Descriptor.ComponentTableOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved5"/>
        public byte[] Reserved5 => _model.Descriptor.Reserved5;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved6"/>
        public byte[] Reserved6 => _model.Descriptor.Reserved6;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileGroupOffsets"/>
        public uint[] D_FileGroupOffsets => _model.Descriptor.FileGroupOffsets;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.ComponentOffsets"/>
        public uint[] D_ComponentOffsets => _model.Descriptor.ComponentOffsets;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.SetupTypesOffset"/>
        public uint SetupTypesOffset => _model.Descriptor.SetupTypesOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.SetupTableOffset"/>
        public uint SetupTableOffset => _model.Descriptor.SetupTableOffset;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved7"/>
        public byte[] Reserved7 => _model.Descriptor.Reserved7;

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved8"/>
        public byte[] Reserved8 => _model.Descriptor.Reserved8;

        #endregion

        #region File Descriptor Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileDescriptorOffsets"/>
        public uint[] FileDescriptorOffsets => _model.FileDescriptorOffsets;

        #endregion

        #region Directory Descriptors

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.DirectoryNames"/>
        public string[] DirectoryNames => _model.DirectoryNames;

        #endregion

        #region File Descriptors

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileDescriptors"/>
#if NET48
        public SabreTools.Models.InstallShieldCabinet.FileDescriptor[] FileDescriptors => _model.FileDescriptors;
#else
        public SabreTools.Models.InstallShieldCabinet.FileDescriptor?[]? FileDescriptors => _model.FileDescriptors;
#endif

        #endregion

        #region File Group Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileGroupOffsets"/>
#if NET48
        public Dictionary<long, SabreTools.Models.InstallShieldCabinet.OffsetList> FileGroupOffsets => _model.FileGroupOffsets;
#else
        public Dictionary<long, SabreTools.Models.InstallShieldCabinet.OffsetList?>? FileGroupOffsets => _model.FileGroupOffsets;
#endif

        #endregion

        #region File Groups

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileGroups"/>
#if NET48
        public SabreTools.Models.InstallShieldCabinet.FileGroup[] FileGroups => _model.FileGroups;
#else
        public SabreTools.Models.InstallShieldCabinet.FileGroup?[]? FileGroups => _model.FileGroups;
#endif

        #endregion

        #region Component Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.ComponentOffsets"/>
#if NET48
        public Dictionary<long, SabreTools.Models.InstallShieldCabinet.OffsetList> ComponentOffsets => _model.ComponentOffsets;
#else
        public Dictionary<long, SabreTools.Models.InstallShieldCabinet.OffsetList?>? ComponentOffsets => _model.ComponentOffsets;
#endif

        #endregion

        #region Components

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.Components"/>
#if NET48
        public SabreTools.Models.InstallShieldCabinet.Component[] Components => _model.Components;
#else
        public SabreTools.Models.InstallShieldCabinet.Component?[]? Components => _model.Components;
#endif

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

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet model, byte[] data, int offset)
#else
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet model, Stream data)
#else
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

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

            var cabinet = new SabreTools.Serialization.Streams.InstallShieldCabinet().Deserialize(data);
            if (cabinet == null)
                return null;

            try
            {
                return new InstallShieldCabinet(cabinet, data);
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

            builder.AppendLine("InstallShield Cabinet Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Headers
            PrintCommonHeader(builder);
            PrintVolumeHeader(builder);
            PrintDescriptor(builder);

            // File Descriptors
            PrintFileDescriptorOffsets(builder);
            PrintDirectoryNames(builder);
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
            builder.AppendLine($"  Descriptor offset: {DescriptorOffset} (0x{DescriptorOffset:X})");
            builder.AppendLine($"  Descriptor size: {DescriptorSize} (0x{DescriptorSize:X})");
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
        /// Print descriptor information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDescriptor(StringBuilder builder)
        {
            builder.AppendLine("  Descriptor Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Strings offset: {StringsOffset} (0x{StringsOffset:X})");
            builder.AppendLine($"  Reserved 0: {BitConverter.ToString(Reserved0).Replace('-', ' ')}");
            builder.AppendLine($"  Component list offset: {ComponentListOffset} (0x{ComponentListOffset:X})");
            builder.AppendLine($"  File table offset: {FileTableOffset} (0x{FileTableOffset:X})");
            builder.AppendLine($"  Reserved 1: {BitConverter.ToString(Reserved1).Replace('-', ' ')}");
            builder.AppendLine($"  File table size: {FileTableSize} (0x{FileTableSize:X})");
            builder.AppendLine($"  File table size 2: {FileTableSize2} (0x{FileTableSize2:X})");
            builder.AppendLine($"  Directory count: {DirectoryCount} (0x{DirectoryCount:X})");
            builder.AppendLine($"  Reserved 2: {BitConverter.ToString(Reserved2).Replace('-', ' ')}");
            builder.AppendLine($"  Reserved 3: {BitConverter.ToString(Reserved3).Replace('-', ' ')}");
            builder.AppendLine($"  Reserved 4: {BitConverter.ToString(Reserved4).Replace('-', ' ')}");
            builder.AppendLine($"  File count: {FileCount} (0x{FileCount:X})");
            builder.AppendLine($"  File table offset 2: {FileTableOffset2} (0x{FileTableOffset2:X})");
            builder.AppendLine($"  Component table info count: {ComponentTableInfoCount} (0x{ComponentTableInfoCount:X})");
            builder.AppendLine($"  Component table offset: {ComponentTableOffset} (0x{ComponentTableOffset:X})");
            builder.AppendLine($"  Reserved 5: {BitConverter.ToString(Reserved5).Replace('-', ' ')}");
            builder.AppendLine($"  Reserved 6: {BitConverter.ToString(Reserved6).Replace('-', ' ')}");
            builder.AppendLine();

            builder.AppendLine($"  File group offsets:");
            builder.AppendLine("  -------------------------");
            if (D_FileGroupOffsets == null || D_FileGroupOffsets.Length == 0)
            {
                builder.AppendLine("  No file group offsets");
            }
            else
            {
                for (int i = 0; i < D_FileGroupOffsets.Length; i++)
                {
                    builder.AppendLine($"      File Group Offset {i}: {D_FileGroupOffsets[i]} (0x{D_FileGroupOffsets[i]:X})");
                }
            }
            builder.AppendLine();

            builder.AppendLine($"  Component offsets:");
            builder.AppendLine("  -------------------------");
            if (D_ComponentOffsets == null || D_ComponentOffsets.Length == 0)
            {
                builder.AppendLine("  No component offsets");
            }
            else
            {
                for (int i = 0; i < D_ComponentOffsets.Length; i++)
                {
                    builder.AppendLine($"      Component Offset {i}: {D_ComponentOffsets[i]} (0x{D_ComponentOffsets[i]:X})");
                }
            }
            builder.AppendLine();

            builder.AppendLine($"  Setup types offset: {SetupTypesOffset} (0x{SetupTypesOffset:X})");
            builder.AppendLine($"  Setup table offset: {SetupTableOffset} (0x{SetupTableOffset:X})");
            builder.AppendLine($"  Reserved 7: {BitConverter.ToString(Reserved7).Replace('-', ' ')}");
            builder.AppendLine($"  Reserved 8: {BitConverter.ToString(Reserved8).Replace('-', ' ')}");
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
        /// Print directory names information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryNames(StringBuilder builder)
        {
            builder.AppendLine("  Directory Names:");
            builder.AppendLine("  -------------------------");
            if (DirectoryNames == null || DirectoryNames.Length == 0)
            {
                builder.AppendLine("  No directory names");
            }
            else
            {
                for (int i = 0; i < DirectoryNames.Length; i++)
                {
                    builder.AppendLine($"    Directory Name {i}: {DirectoryNames[i] ?? "[NULL]"}");
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
                    if (offsetList == null)
                    {
                        builder.AppendLine($"    Unassigned file group");
                    }
                    else
                    {
                        builder.AppendLine($"    Name offset: {offsetList.NameOffset} (0x{offsetList.NameOffset:X})");
                        builder.AppendLine($"    Name: {offsetList.Name ?? "[NULL]"}");
                        builder.AppendLine($"    Descriptor offset: {offsetList.DescriptorOffset} (0x{offsetList.DescriptorOffset:X})");
                        builder.AppendLine($"    Next offset: {offsetList.NextOffset} (0x{offsetList.NextOffset:X})");
                    }
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
                builder.AppendLine();
            }
            else
            {
                for (int i = 0; i < FileGroups.Length; i++)
                {
                    var fileGroup = FileGroups[i];
                    builder.AppendLine($"  File Group {i}:");
                    if (fileGroup == null)
                    {
                        builder.AppendLine($"    Unassigned file group");
                    }
                    else
                    {
                        builder.AppendLine($"    Name offset: {fileGroup.NameOffset} (0x{fileGroup.NameOffset:X})");
                        builder.AppendLine($"    Name: {fileGroup.Name ?? "[NULL]"}");
                        builder.AppendLine($"    Expanded size: {fileGroup.ExpandedSize} (0x{fileGroup.ExpandedSize:X})");
                        builder.AppendLine($"    Reserved 0: {BitConverter.ToString(fileGroup.Reserved0).Replace('-', ' ')}");
                        builder.AppendLine($"    Compressed size: {fileGroup.CompressedSize} (0x{fileGroup.CompressedSize:X})");
                        builder.AppendLine($"    Reserved 1: {BitConverter.ToString(fileGroup.Reserved1).Replace('-', ' ')}");
                        builder.AppendLine($"    Reserved 2: {BitConverter.ToString(fileGroup.Reserved2).Replace('-', ' ')}");
                        builder.AppendLine($"    Attribute 1: {fileGroup.Attribute1} (0x{fileGroup.Attribute1:X})");
                        builder.AppendLine($"    Attribute 2: {fileGroup.Attribute2} (0x{fileGroup.Attribute2:X})");
                        builder.AppendLine($"    First file: {fileGroup.FirstFile} (0x{fileGroup.FirstFile:X})");
                        builder.AppendLine($"    Last file: {fileGroup.LastFile} (0x{fileGroup.LastFile:X})");
                        builder.AppendLine($"    Unknown offset: {fileGroup.UnknownOffset} (0x{fileGroup.UnknownOffset:X})");
                        builder.AppendLine($"    Var 4 offset: {fileGroup.Var4Offset} (0x{fileGroup.Var4Offset:X})");
                        builder.AppendLine($"    Var 1 offset: {fileGroup.Var1Offset} (0x{fileGroup.Var1Offset:X})");
                        builder.AppendLine($"    HTTP location offset: {fileGroup.HTTPLocationOffset} (0x{fileGroup.HTTPLocationOffset:X})");
                        builder.AppendLine($"    FTP location offset: {fileGroup.FTPLocationOffset} (0x{fileGroup.FTPLocationOffset:X})");
                        builder.AppendLine($"    Misc. offset: {fileGroup.MiscOffset} (0x{fileGroup.MiscOffset:X})");
                        builder.AppendLine($"    Var 2 offset: {fileGroup.Var2Offset} (0x{fileGroup.Var2Offset:X})");
                        builder.AppendLine($"    Target directory offset: {fileGroup.TargetDirectoryOffset} (0x{fileGroup.TargetDirectoryOffset:X})");
                        builder.AppendLine($"    Reserved 3: {BitConverter.ToString(fileGroup.Reserved3).Replace('-', ' ')}");
                        builder.AppendLine($"    Reserved 4: {BitConverter.ToString(fileGroup.Reserved4).Replace('-', ' ')}");
                        builder.AppendLine($"    Reserved 5: {BitConverter.ToString(fileGroup.Reserved5).Replace('-', ' ')}");
                        builder.AppendLine($"    Reserved 6: {BitConverter.ToString(fileGroup.Reserved6).Replace('-', ' ')}");
                        builder.AppendLine($"    Reserved 7: {BitConverter.ToString(fileGroup.Reserved7).Replace('-', ' ')}");
                    }
                    builder.AppendLine();
                }
            }
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
                    if (offsetList == null)
                    {
                        builder.AppendLine($"    Unassigned component");
                    }
                    else
                    {
                        builder.AppendLine($"    Name offset: {offsetList.NameOffset} (0x{offsetList.NameOffset:X})");
                        builder.AppendLine($"    Name: {offsetList.Name ?? "[NULL]"}");
                        builder.AppendLine($"    Descriptor offset: {offsetList.DescriptorOffset} (0x{offsetList.DescriptorOffset:X})");
                        builder.AppendLine($"    Next offset: {offsetList.NextOffset} (0x{offsetList.NextOffset:X})");
                    }
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
                builder.AppendLine();
            }
            else
            {
                for (int i = 0; i < Components.Length; i++)
                {
                    var component = Components[i];
                    builder.AppendLine($"  Component {i}:");
                    if (component == null)
                    {
                        builder.AppendLine($"    Unassigned component");
                    }
                    else
                    {
                        builder.AppendLine($"    Identifier offset: {component.IdentifierOffset} (0x{component.IdentifierOffset:X})");
                        builder.AppendLine($"    Identifier: {component.Identifier ?? "[NULL]"}");
                        builder.AppendLine($"    Descriptor offset: {component.DescriptorOffset} (0x{component.DescriptorOffset:X})");
                        builder.AppendLine($"    Display name offset: {component.DisplayNameOffset} (0x{component.DisplayNameOffset:X})");
                        builder.AppendLine($"    Display name: {component.DisplayName ?? "[NULL]"}");
                        builder.AppendLine($"    Reserved 0: {BitConverter.ToString(component.Reserved0).Replace('-', ' ')}");
                        builder.AppendLine($"    Reserved offset 0: {component.ReservedOffset0} (0x{component.ReservedOffset0:X})");
                        builder.AppendLine($"    Reserved offset 1: {component.ReservedOffset1} (0x{component.ReservedOffset1:X})");
                        builder.AppendLine($"    Component index: {component.ComponentIndex} (0x{component.ComponentIndex:X})");
                        builder.AppendLine($"    Name offset: {component.NameOffset} (0x{component.NameOffset:X})");
                        builder.AppendLine($"    Name: {component.Name ?? "[NULL]"}");
                        builder.AppendLine($"    Reserved offset 2: {component.ReservedOffset2} (0x{component.ReservedOffset2:X})");
                        builder.AppendLine($"    Reserved offset 3: {component.ReservedOffset3} (0x{component.ReservedOffset3:X})");
                        builder.AppendLine($"    Reserved offset 4: {component.ReservedOffset4} (0x{component.ReservedOffset4:X})");
                        builder.AppendLine($"    Reserved 1: {BitConverter.ToString(component.Reserved1).Replace('-', ' ')}");
                        builder.AppendLine($"    CLSID offset: {component.CLSIDOffset} (0x{component.CLSIDOffset:X})");
                        builder.AppendLine($"    CLSID: {component.CLSID}");
                        builder.AppendLine($"    Reserved 2: {BitConverter.ToString(component.Reserved2).Replace('-', ' ')}");
                        builder.AppendLine($"    Reserved 3: {BitConverter.ToString(component.Reserved3).Replace('-', ' ')}");
                        builder.AppendLine($"    Depends count: {component.DependsCount} (0x{component.DependsCount:X})");
                        builder.AppendLine($"    Depends offset: {component.DependsOffset} (0x{component.DependsOffset:X})");
                        builder.AppendLine($"    File group count: {component.FileGroupCount} (0x{component.FileGroupCount:X})");
                        builder.AppendLine($"    File group names offset: {component.FileGroupNamesOffset} (0x{component.FileGroupNamesOffset:X})");
                        builder.AppendLine();

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
                        builder.AppendLine();

                        builder.AppendLine($"    X3 count: {component.X3Count} (0x{component.X3Count:X})");
                        builder.AppendLine($"    X3 offset: {component.X3Offset} (0x{component.X3Offset:X})");
                        builder.AppendLine($"    Sub-components count: {component.SubComponentsCount} (0x{component.SubComponentsCount:X})");
                        builder.AppendLine($"    Sub-components offset: {component.SubComponentsOffset} (0x{component.SubComponentsOffset:X})");
                        builder.AppendLine($"    Next component offset: {component.NextComponentOffset} (0x{component.NextComponentOffset:X})");
                        builder.AppendLine($"    Reserved offset 5: {component.ReservedOffset5} (0x{component.ReservedOffset5:X})");
                        builder.AppendLine($"    Reserved offset 6: {component.ReservedOffset6} (0x{component.ReservedOffset6:X})");
                        builder.AppendLine($"    Reserved offset 7: {component.ReservedOffset7} (0x{component.ReservedOffset7:X})");
                        builder.AppendLine($"    Reserved offset 8: {component.ReservedOffset8} (0x{component.ReservedOffset8:X})");
                    }
                    builder.AppendLine();
                }
            }
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}
