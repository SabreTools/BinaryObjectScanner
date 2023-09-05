using System.Collections.Generic;
using System.IO;
using System.Text;
using BinaryObjectScanner.Utilities;
using SabreTools.Models.InstallShieldCabinet;
using static SabreTools.Models.InstallShieldCabinet.Constants;

namespace BinaryObjectScanner.Builders
{
    // TODO: Add multi-cabinet reading
    public class InstallShieldCabinet
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a InstallShield Cabinet file
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled cabinet on success, null on error</returns>
        public static Cabinet ParseCabinet(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseCabinet(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a InstallShield Cabinet file
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled cabinet on success, null on error</returns>
        public static Cabinet ParseCabinet(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new cabinet to fill
            var cabinet = new Cabinet();

            #region Common Header

            // Try to parse the cabinet header
            var commonHeader = ParseCommonHeader(data);
            if (commonHeader == null)
                return null;

            // Set the cabinet header
            cabinet.CommonHeader = commonHeader;

            #endregion

            #region Volume Header

            // Try to parse the volume header
            var volumeHeader = ParseVolumeHeader(data, GetMajorVersion(commonHeader));
            if (volumeHeader == null)
                return null;

            // Set the volume header
            cabinet.VolumeHeader = volumeHeader;

            #endregion

            #region Descriptor

            // Get the descriptor offset
            uint descriptorOffset = commonHeader.DescriptorOffset;
            if (descriptorOffset < 0 || descriptorOffset >= data.Length)
                return null;

            // Seek to the descriptor
            data.Seek(descriptorOffset, SeekOrigin.Begin);

            // Try to parse the descriptor
            var descriptor = ParseDescriptor(data);
            if (descriptor == null)
                return null;

            // Set the descriptor
            cabinet.Descriptor = descriptor;

            #endregion

            #region File Descriptor Offsets

            // Get the file table offset
            uint fileTableOffset = commonHeader.DescriptorOffset + descriptor.FileTableOffset;
            if (fileTableOffset < 0 || fileTableOffset >= data.Length)
                return null;

            // Seek to the file table
            data.Seek(fileTableOffset, SeekOrigin.Begin);

            // Get the number of file table items
            uint fileTableItems;
            if (GetMajorVersion(commonHeader) <= 5)
                fileTableItems = descriptor.DirectoryCount + descriptor.FileCount;
            else
                fileTableItems = descriptor.DirectoryCount;

            // Create and fill the file table
            cabinet.FileDescriptorOffsets = new uint[fileTableItems];
            for (int i = 0; i < cabinet.FileDescriptorOffsets.Length; i++)
            {
                cabinet.FileDescriptorOffsets[i] = data.ReadUInt32();
            }

            #endregion

            #region Directory Descriptors

            // Create and fill the directory descriptors
            cabinet.DirectoryNames = new string[descriptor.DirectoryCount];
            for (int i = 0; i < descriptor.DirectoryCount; i++)
            {
                // Get the directory descriptor offset
                uint offset = descriptorOffset
                    + descriptor.FileTableOffset
                    + cabinet.FileDescriptorOffsets[i];

                // If we have an invalid offset
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the file descriptor offset
                data.Seek(offset, SeekOrigin.Begin);

                // Create and add the file descriptor
                string directoryName = ParseDirectoryName(data, GetMajorVersion(commonHeader));
                cabinet.DirectoryNames[i] = directoryName;
            }

            #endregion

            #region File Descriptors

            // Create and fill the file descriptors
            cabinet.FileDescriptors = new FileDescriptor[descriptor.FileCount];
            for (int i = 0; i < descriptor.FileCount; i++)
            {
                // Get the file descriptor offset
                uint offset;
                if (GetMajorVersion(commonHeader) <= 5)
                {
                    offset = descriptorOffset
                        + descriptor.FileTableOffset
                        + cabinet.FileDescriptorOffsets[descriptor.DirectoryCount + i];
                }
                else
                {
                    offset = descriptorOffset
                        + descriptor.FileTableOffset
                        + descriptor.FileTableOffset2
                        + (uint)(i * 0x57);
                }

                // If we have an invalid offset
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the file descriptor offset
                data.Seek(offset, SeekOrigin.Begin);

                // Create and add the file descriptor
                FileDescriptor fileDescriptor = ParseFileDescriptor(data, GetMajorVersion(commonHeader), descriptorOffset + descriptor.FileTableOffset);
                cabinet.FileDescriptors[i] = fileDescriptor;
            }

            #endregion

            #region File Group Offsets

            // Create and fill the file group offsets
            cabinet.FileGroupOffsets = new Dictionary<long, OffsetList>();
            for (int i = 0; i < descriptor.FileGroupOffsets.Length; i++)
            {
                // Get the file group offset
                uint offset = descriptor.FileGroupOffsets[i];
                if (offset == 0)
                    continue;

                // Adjust the file group offset
                offset += commonHeader.DescriptorOffset;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the file group offset
                data.Seek(offset, SeekOrigin.Begin);

                // Create and add the offset
                OffsetList offsetList = ParseOffsetList(data, GetMajorVersion(commonHeader), descriptorOffset);
                cabinet.FileGroupOffsets[descriptor.FileGroupOffsets[i]] = offsetList;

                // If we have a nonzero next offset
                uint nextOffset = offsetList.NextOffset;
                while (nextOffset != 0)
                {
                    // Get the next offset to read
                    uint internalOffset = nextOffset + commonHeader.DescriptorOffset;

                    // Seek to the file group offset
                    data.Seek(internalOffset, SeekOrigin.Begin);

                    // Create and add the offset
                    offsetList = ParseOffsetList(data, GetMajorVersion(commonHeader), descriptorOffset);
                    cabinet.FileGroupOffsets[nextOffset] = offsetList;

                    // Set the next offset
                    nextOffset = offsetList.NextOffset;
                }
            }

            #endregion

            #region File Groups

            // Create the file groups array
            cabinet.FileGroups = new FileGroup[cabinet.FileGroupOffsets.Count];

            // Create and fill the file groups
            int fileGroupId = 0;
            foreach (var kvp in cabinet.FileGroupOffsets)
            {
                // Get the offset
                OffsetList list = kvp.Value;
                if (list == null)
                {
                    fileGroupId++;
                    continue;
                }

                // If we have an invalid offset
                if (list.DescriptorOffset <= 0)
                {
                    fileGroupId++;
                    continue;
                }

                /// Seek to the file group
                data.Seek(list.DescriptorOffset + descriptorOffset, SeekOrigin.Begin);

                // Try to parse the file group
                var fileGroup = ParseFileGroup(data, GetMajorVersion(commonHeader), descriptorOffset);
                if (fileGroup == null)
                    return null;

                // Add the file group
                cabinet.FileGroups[fileGroupId++] = fileGroup;
            }

            #endregion

            #region Component Offsets

            // Create and fill the component offsets
            cabinet.ComponentOffsets = new Dictionary<long, OffsetList>();
            for (int i = 0; i < descriptor.ComponentOffsets.Length; i++)
            {
                // Get the component offset
                uint offset = descriptor.ComponentOffsets[i];
                if (offset == 0)
                    continue;

                // Adjust the component offset
                offset += commonHeader.DescriptorOffset;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the component offset
                data.Seek(offset, SeekOrigin.Begin);

                // Create and add the offset
                OffsetList offsetList = ParseOffsetList(data, GetMajorVersion(commonHeader), descriptorOffset);
                cabinet.ComponentOffsets[descriptor.ComponentOffsets[i]] = offsetList;

                // If we have a nonzero next offset
                uint nextOffset = offsetList.NextOffset;
                while (nextOffset != 0)
                {
                    // Get the next offset to read
                    uint internalOffset = nextOffset + commonHeader.DescriptorOffset;

                    // Seek to the file group offset
                    data.Seek(internalOffset, SeekOrigin.Begin);

                    // Create and add the offset
                    offsetList = ParseOffsetList(data, GetMajorVersion(commonHeader), descriptorOffset);
                    cabinet.ComponentOffsets[nextOffset] = offsetList;

                    // Set the next offset
                    nextOffset = offsetList.NextOffset;
                }
            }

            #endregion

            #region Components

            // Create the components array
            cabinet.Components = new Component[cabinet.ComponentOffsets.Count];

            // Create and fill the components
            int componentId = 0;
            foreach (KeyValuePair<long, OffsetList> kvp in cabinet.ComponentOffsets)
            {
                // Get the offset
                OffsetList list = kvp.Value;
                if (list == null)
                {
                    componentId++;
                    continue;
                }

                // If we have an invalid offset
                if (list.DescriptorOffset <= 0)
                {
                    componentId++;
                    continue;
                }

                // Seek to the component
                data.Seek(list.DescriptorOffset + descriptorOffset, SeekOrigin.Begin);

                // Try to parse the component
                var component = ParseComponent(data, GetMajorVersion(commonHeader), descriptorOffset);
                if (component == null)
                    return null;

                // Add the component
                cabinet.Components[componentId++] = component;
            }

            #endregion

            // TODO: Parse setup types

            return cabinet;
        }

        /// <summary>
        /// Parse a Stream into a common header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled common header on success, null on error</returns>
        private static CommonHeader ParseCommonHeader(Stream data)
        {
            CommonHeader commonHeader = new CommonHeader();

            byte[] signature = data.ReadBytes(4);
            commonHeader.Signature = Encoding.ASCII.GetString(signature);
            if (commonHeader.Signature != SignatureString)
                return null;

            commonHeader.Version = data.ReadUInt32();
            commonHeader.VolumeInfo = data.ReadUInt32();
            commonHeader.DescriptorOffset = data.ReadUInt32();
            commonHeader.DescriptorSize = data.ReadUInt32();

            return commonHeader;
        }

        /// <summary>
        /// Parse a Stream into a volume header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <returns>Filled volume header on success, null on error</returns>
        private static VolumeHeader ParseVolumeHeader(Stream data, int majorVersion)
        {
            VolumeHeader volumeHeader = new VolumeHeader();

            // Read the descriptor based on version
            if (majorVersion <= 5)
            {
                volumeHeader.DataOffset = data.ReadUInt32();
                _ = data.ReadBytes(0x04); // Skip 0x04 bytes, unknown data?
                volumeHeader.FirstFileIndex = data.ReadUInt32();
                volumeHeader.LastFileIndex = data.ReadUInt32();
                volumeHeader.FirstFileOffset = data.ReadUInt32();
                volumeHeader.FirstFileSizeExpanded = data.ReadUInt32();
                volumeHeader.FirstFileSizeCompressed = data.ReadUInt32();
                volumeHeader.LastFileOffset = data.ReadUInt32();
                volumeHeader.LastFileSizeExpanded = data.ReadUInt32();
                volumeHeader.LastFileSizeCompressed = data.ReadUInt32();
            }
            else
            {
                // TODO: Should standard and high values be combined?
                volumeHeader.DataOffset = data.ReadUInt32();
                volumeHeader.DataOffsetHigh = data.ReadUInt32();
                volumeHeader.FirstFileIndex = data.ReadUInt32();
                volumeHeader.LastFileIndex = data.ReadUInt32();
                volumeHeader.FirstFileOffset = data.ReadUInt32();
                volumeHeader.FirstFileOffsetHigh = data.ReadUInt32();
                volumeHeader.FirstFileSizeExpanded = data.ReadUInt32();
                volumeHeader.FirstFileSizeExpandedHigh = data.ReadUInt32();
                volumeHeader.FirstFileSizeCompressed = data.ReadUInt32();
                volumeHeader.FirstFileSizeCompressedHigh = data.ReadUInt32();
                volumeHeader.LastFileOffset = data.ReadUInt32();
                volumeHeader.LastFileOffsetHigh = data.ReadUInt32();
                volumeHeader.LastFileSizeExpanded = data.ReadUInt32();
                volumeHeader.LastFileSizeExpandedHigh = data.ReadUInt32();
                volumeHeader.LastFileSizeCompressed = data.ReadUInt32();
                volumeHeader.LastFileSizeCompressedHigh = data.ReadUInt32();
            }

            return volumeHeader;
        }

        /// <summary>
        /// Parse a Stream into a descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled descriptor on success, null on error</returns>
        private static Descriptor ParseDescriptor(Stream data)
        {
            Descriptor descriptor = new Descriptor();

            descriptor.StringsOffset = data.ReadUInt32();
            descriptor.Reserved0 = data.ReadBytes(4);
            descriptor.ComponentListOffset = data.ReadUInt32();
            descriptor.FileTableOffset = data.ReadUInt32();
            descriptor.Reserved1 = data.ReadBytes(4);
            descriptor.FileTableSize = data.ReadUInt32();
            descriptor.FileTableSize2 = data.ReadUInt32();
            descriptor.DirectoryCount = data.ReadUInt16();
            descriptor.Reserved2 = data.ReadBytes(4);
            descriptor.Reserved3 = data.ReadBytes(2);
            descriptor.Reserved4 = data.ReadBytes(4);
            descriptor.FileCount = data.ReadUInt32();
            descriptor.FileTableOffset2 = data.ReadUInt32();
            descriptor.ComponentTableInfoCount = data.ReadUInt16();
            descriptor.ComponentTableOffset = data.ReadUInt32();
            descriptor.Reserved5 = data.ReadBytes(4);
            descriptor.Reserved6 = data.ReadBytes(4);

            descriptor.FileGroupOffsets = new uint[MAX_FILE_GROUP_COUNT];
            for (int i = 0; i < descriptor.FileGroupOffsets.Length; i++)
            {
                descriptor.FileGroupOffsets[i] = data.ReadUInt32();
            }

            descriptor.ComponentOffsets = new uint[MAX_COMPONENT_COUNT];
            for (int i = 0; i < descriptor.ComponentOffsets.Length; i++)
            {
                descriptor.ComponentOffsets[i] = data.ReadUInt32();
            }

            descriptor.SetupTypesOffset = data.ReadUInt32();
            descriptor.SetupTableOffset = data.ReadUInt32();
            descriptor.Reserved7 = data.ReadBytes(4);
            descriptor.Reserved8 = data.ReadBytes(4);

            return descriptor;
        }

        /// <summary>
        /// Parse a Stream into an offset list
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <param name="descriptorOffset">Offset of the cabinet descriptor</param>
        /// <returns>Filled offset list on success, null on error</returns>
        private static OffsetList ParseOffsetList(Stream data, int majorVersion, uint descriptorOffset)
        {
            OffsetList offsetList = new OffsetList();

            offsetList.NameOffset = data.ReadUInt32();
            offsetList.DescriptorOffset = data.ReadUInt32();
            offsetList.NextOffset = data.ReadUInt32();

            // Cache the current offset
            long currentOffset = data.Position;

            // Seek to the name offset
            data.Seek(offsetList.NameOffset + descriptorOffset, SeekOrigin.Begin);

            // Read the string
            if (majorVersion >= 17)
                offsetList.Name = data.ReadString(Encoding.Unicode);
            else
                offsetList.Name = data.ReadString(Encoding.ASCII);

            // Seek back to the correct offset
            data.Seek(currentOffset, SeekOrigin.Begin);

            return offsetList;
        }

        /// <summary>
        /// Parse a Stream into a file group
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <param name="descriptorOffset">Offset of the cabinet descriptor</param>
        /// <returns>Filled file group on success, null on error</returns>
        private static FileGroup ParseFileGroup(Stream data, int majorVersion, uint descriptorOffset)
        {
            FileGroup fileGroup = new FileGroup();

            fileGroup.NameOffset = data.ReadUInt32();

            fileGroup.ExpandedSize = data.ReadUInt32();
            fileGroup.Reserved0 = data.ReadBytes(4);
            fileGroup.CompressedSize = data.ReadUInt32();
            fileGroup.Reserved1 = data.ReadBytes(4);
            fileGroup.Reserved2 = data.ReadBytes(2);
            fileGroup.Attribute1 = data.ReadUInt16();
            fileGroup.Attribute2 = data.ReadUInt16();

            // TODO: Figure out what data lives in this area for V5 and below
            if (majorVersion <= 5)
                data.Seek(0x36, SeekOrigin.Current);

            fileGroup.FirstFile = data.ReadUInt32();
            fileGroup.LastFile = data.ReadUInt32();
            fileGroup.UnknownOffset = data.ReadUInt32();
            fileGroup.Var4Offset = data.ReadUInt32();
            fileGroup.Var1Offset = data.ReadUInt32();
            fileGroup.HTTPLocationOffset = data.ReadUInt32();
            fileGroup.FTPLocationOffset = data.ReadUInt32();
            fileGroup.MiscOffset = data.ReadUInt32();
            fileGroup.Var2Offset = data.ReadUInt32();
            fileGroup.TargetDirectoryOffset = data.ReadUInt32();
            fileGroup.Reserved3 = data.ReadBytes(2);
            fileGroup.Reserved4 = data.ReadBytes(2);
            fileGroup.Reserved5 = data.ReadBytes(2);
            fileGroup.Reserved6 = data.ReadBytes(2);
            fileGroup.Reserved7 = data.ReadBytes(2);

            // Cache the current position
            long currentPosition = data.Position;

            // Read the name, if possible
            if (fileGroup.NameOffset != 0)
            {
                // Seek to the name
                data.Seek(fileGroup.NameOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the string
                if (majorVersion >= 17)
                    fileGroup.Name = data.ReadString(Encoding.Unicode);
                else
                    fileGroup.Name = data.ReadString(Encoding.ASCII);
            }

            // Seek back to the correct offset
            data.Seek(currentPosition, SeekOrigin.Begin);

            return fileGroup;
        }

        /// <summary>
        /// Parse a Stream into a component
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <param name="descriptorOffset">Offset of the cabinet descriptor</param>
        /// <returns>Filled component on success, null on error</returns>
        private static Component ParseComponent(Stream data, int majorVersion, uint descriptorOffset)
        {
            Component component = new Component();

            component.IdentifierOffset = data.ReadUInt32();
            component.DescriptorOffset = data.ReadUInt32();
            component.DisplayNameOffset = data.ReadUInt32();
            component.Reserved0 = data.ReadBytes(2);
            component.ReservedOffset0 = data.ReadUInt32();
            component.ReservedOffset1 = data.ReadUInt32();
            component.ComponentIndex = data.ReadUInt16();
            component.NameOffset = data.ReadUInt32();
            component.ReservedOffset2 = data.ReadUInt32();
            component.ReservedOffset3 = data.ReadUInt32();
            component.ReservedOffset4 = data.ReadUInt32();
            component.Reserved1 = data.ReadBytes(32);
            component.CLSIDOffset = data.ReadUInt32();
            component.Reserved2 = data.ReadBytes(28);
            component.Reserved3 = data.ReadBytes(majorVersion <= 5 ? 2 : 1);
            component.DependsCount = data.ReadUInt16();
            component.DependsOffset = data.ReadUInt32();
            component.FileGroupCount = data.ReadUInt16();
            component.FileGroupNamesOffset = data.ReadUInt32();
            component.X3Count = data.ReadUInt16();
            component.X3Offset = data.ReadUInt32();
            component.SubComponentsCount = data.ReadUInt16();
            component.SubComponentsOffset = data.ReadUInt32();
            component.NextComponentOffset = data.ReadUInt32();
            component.ReservedOffset5 = data.ReadUInt32();
            component.ReservedOffset6 = data.ReadUInt32();
            component.ReservedOffset7 = data.ReadUInt32();
            component.ReservedOffset8 = data.ReadUInt32();

            // Cache the current position
            long currentPosition = data.Position;

            // Read the identifier, if possible
            if (component.IdentifierOffset != 0)
            {
                // Seek to the identifier
                data.Seek(component.IdentifierOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the string
                if (majorVersion >= 17)
                    component.Identifier = data.ReadString(Encoding.Unicode);
                else
                    component.Identifier = data.ReadString(Encoding.ASCII);
            }

            // Read the display name, if possible
            if (component.DisplayNameOffset != 0)
            {
                // Seek to the name
                data.Seek(component.DisplayNameOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the string
                if (majorVersion >= 17)
                    component.DisplayName = data.ReadString(Encoding.Unicode);
                else
                    component.DisplayName = data.ReadString(Encoding.ASCII);
            }

            // Read the name, if possible
            if (component.NameOffset != 0)
            {
                // Seek to the name
                data.Seek(component.NameOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the string
                if (majorVersion >= 17)
                    component.Name = data.ReadString(Encoding.Unicode);
                else
                    component.Name = data.ReadString(Encoding.ASCII);
            }

            // Read the CLSID, if possible
            if (component.CLSIDOffset != 0)
            {
                // Seek to the CLSID
                data.Seek(component.CLSIDOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the GUID
                component.CLSID = data.ReadGuid();
            }

            // Read the file group names, if possible
            if (component.FileGroupCount != 0 && component.FileGroupNamesOffset != 0)
            {
                // Seek to the file group table offset
                data.Seek(component.FileGroupNamesOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the file group names table
                component.FileGroupNames = new string[component.FileGroupCount];
                for (int j = 0; j < component.FileGroupCount; j++)
                {
                    // Get the name offset
                    uint nameOffset = data.ReadUInt32();

                    // Cache the current offset
                    long preNameOffset = data.Position;

                    // Seek to the name offset
                    data.Seek(nameOffset + descriptorOffset, SeekOrigin.Begin);

                    if (majorVersion >= 17)
                        component.FileGroupNames[j] = data.ReadString(Encoding.Unicode);
                    else
                        component.FileGroupNames[j] = data.ReadString(Encoding.ASCII);

                    // Seek back to the original position
                    data.Seek(preNameOffset, SeekOrigin.Begin);
                }
            }

            // Seek back to the correct offset
            data.Seek(currentPosition, SeekOrigin.Begin);

            return component;
        }

        /// <summary>
        /// Parse a Stream into a directory name
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <returns>Filled directory name on success, null on error</returns>
        private static string ParseDirectoryName(Stream data, int majorVersion)
        {
            // Read the string
            if (majorVersion >= 17)
                return data.ReadString(Encoding.Unicode);
            else
                return data.ReadString(Encoding.ASCII);
        }

        /// <summary>
        /// Parse a Stream into a file descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <param name="descriptorOffset">Offset of the cabinet descriptor</param>
        /// <returns>Filled file descriptor on success, null on error</returns>
        private static FileDescriptor ParseFileDescriptor(Stream data, int majorVersion, uint descriptorOffset)
        {
            FileDescriptor fileDescriptor = new FileDescriptor();

            // Read the descriptor based on version
            if (majorVersion <= 5)
            {
                fileDescriptor.Volume = 0xFFFF; // Set by the header index
                fileDescriptor.NameOffset = data.ReadUInt32();
                fileDescriptor.DirectoryIndex = data.ReadUInt32();
                fileDescriptor.Flags = (FileFlags)data.ReadUInt16();
                fileDescriptor.ExpandedSize = data.ReadUInt32();
                fileDescriptor.CompressedSize = data.ReadUInt32();
                _ = data.ReadBytes(0x14); // Skip 0x14 bytes, unknown data?
                fileDescriptor.DataOffset = data.ReadUInt32();

                if (majorVersion == 5)
                    fileDescriptor.MD5 = data.ReadBytes(0x10);
            }
            else
            {
                fileDescriptor.Flags = (FileFlags)data.ReadUInt16();
                fileDescriptor.ExpandedSize = data.ReadUInt64();
                fileDescriptor.CompressedSize = data.ReadUInt64();
                fileDescriptor.DataOffset = data.ReadUInt64();
                fileDescriptor.MD5 = data.ReadBytes(0x10);
                _ = data.ReadBytes(0x10); // Skip 0x10 bytes, unknown data?
                fileDescriptor.NameOffset = data.ReadUInt32();
                fileDescriptor.DirectoryIndex = data.ReadUInt16();
                _ = data.ReadBytes(0x0C); // Skip 0x0C bytes, unknown data?
                fileDescriptor.LinkPrevious = data.ReadUInt32();
                fileDescriptor.LinkNext = data.ReadUInt32();
                fileDescriptor.LinkFlags = (LinkFlags)data.ReadByteValue();
                fileDescriptor.Volume = data.ReadUInt16();
            }

            // Cache the current position
            long currentPosition = data.Position;

            // Read the name, if possible
            if (fileDescriptor.NameOffset != 0)
            {
                // Seek to the name
                data.Seek(fileDescriptor.NameOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the string
                if (majorVersion >= 17)
                    fileDescriptor.Name = data.ReadString(Encoding.Unicode);
                else
                    fileDescriptor.Name = data.ReadString(Encoding.ASCII);
            }

            // Seek back to the correct offset
            data.Seek(currentPosition, SeekOrigin.Begin);

            return fileDescriptor;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get the major version of the cabinet
        /// </summary>
        /// <remarks>This should live in the wrapper but is needed during parsing</remarks>
        private static int GetMajorVersion(CommonHeader commonHeader)
        {
            uint majorVersion = commonHeader.Version;
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

        #endregion
    }
}
