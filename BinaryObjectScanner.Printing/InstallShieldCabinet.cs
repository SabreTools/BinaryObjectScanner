using System;
using System.Collections.Generic;
using System.Text;
using SabreTools.Models.InstallShieldCabinet;

namespace BinaryObjectScanner.Printing
{
    public static class InstallShieldCabinet
    {
        public static void Print(StringBuilder builder, Cabinet cabinet)
        {
            builder.AppendLine("InstallShield Cabinet Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Major Version
            int majorVersion = GetMajorVersion(cabinet.CommonHeader);

            // Headers
            Print(builder, cabinet.CommonHeader, majorVersion);
            Print(builder, cabinet.VolumeHeader, majorVersion);
            Print(builder, cabinet.Descriptor);

            // File Descriptors
            Print(builder, cabinet.FileDescriptorOffsets);
            Print(builder, cabinet.DirectoryNames);
            Print(builder, cabinet.FileDescriptors);

            // File Groups
            Print(builder, cabinet.FileGroupOffsets, "File Group");
            Print(builder, cabinet.FileGroups);

            // Components
            Print(builder, cabinet.ComponentOffsets, "Component");
            Print(builder, cabinet.Components);
        }

#if NET48
        private static int GetMajorVersion(CommonHeader header)
#else
        private static int GetMajorVersion(CommonHeader? header)
#endif
        {
#if NET48
            uint majorVersion = header.Version;
#else
            uint majorVersion = header?.Version ?? 0;
#endif
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

#if NET48
        private static void Print(StringBuilder builder, CommonHeader header, int majorVersion)
#else
        private static void Print(StringBuilder builder, CommonHeader? header, int majorVersion)
#endif
        {
            builder.AppendLine("  Common Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine(value: "  No common header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine($"  Version: {header.Version} (0x{header.Version:X}) [{majorVersion}]");
            builder.AppendLine(header.VolumeInfo, "  Volume info");
            builder.AppendLine(header.DescriptorOffset, "  Descriptor offset");
            builder.AppendLine(header.DescriptorSize, "  Descriptor size");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, VolumeHeader header, int majorVersion)
#else
        private static void Print(StringBuilder builder, VolumeHeader? header, int majorVersion)
#endif
        {
            builder.AppendLine("  Volume Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine(value: "  No volume header");
                builder.AppendLine();
                return;
            }

            if (majorVersion <= 5)
            {
                builder.AppendLine(header.DataOffset, "  Data offset");
                builder.AppendLine(header.FirstFileIndex, "  First file index");
                builder.AppendLine(header.LastFileIndex, "  Last file index");
                builder.AppendLine(header.FirstFileOffset, "  First file offset");
                builder.AppendLine(header.FirstFileSizeExpanded, "  First file size expanded");
                builder.AppendLine(header.FirstFileSizeCompressed, "  First file size compressed");
                builder.AppendLine(header.LastFileOffset, "  Last file offset");
                builder.AppendLine(header.LastFileSizeExpanded, "  Last file size expanded");
                builder.AppendLine(header.LastFileSizeCompressed, "  Last file size compressed");
            }
            else
            {
                builder.AppendLine(header.DataOffset, "  Data offset");
                builder.AppendLine(header.DataOffsetHigh, "  Data offset high");
                builder.AppendLine(header.FirstFileIndex, "  First file index");
                builder.AppendLine(header.LastFileIndex, "  Last file index");
                builder.AppendLine(header.FirstFileOffset, "  First file offset");
                builder.AppendLine(header.FirstFileOffsetHigh, "  First file offset high");
                builder.AppendLine(header.FirstFileSizeExpanded, "  First file size expanded");
                builder.AppendLine(header.FirstFileSizeExpandedHigh, "  First file size expanded high");
                builder.AppendLine(header.FirstFileSizeCompressed, "  First file size compressed");
                builder.AppendLine(header.FirstFileSizeCompressedHigh, "  First file size compressed high");
                builder.AppendLine(header.LastFileOffset, "  Last file offset");
                builder.AppendLine(header.LastFileOffsetHigh, "  Last file offset high");
                builder.AppendLine(header.LastFileSizeExpanded, "  Last file size expanded");
                builder.AppendLine(header.LastFileSizeExpandedHigh, "  Last file size expanded high");
                builder.AppendLine(header.LastFileSizeCompressed, "  Last file size compressed");
                builder.AppendLine(header.LastFileSizeCompressedHigh, "  Last file size compressed high");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Descriptor descriptor)
#else
        private static void Print(StringBuilder builder, Descriptor? descriptor)
#endif
        {
            builder.AppendLine("  Descriptor Information:");
            builder.AppendLine("  -------------------------");
            if (descriptor == null)
            {
                builder.AppendLine("  No descriptor");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(descriptor.StringsOffset, "  Strings offset");
            builder.AppendLine(descriptor.Reserved0, "  Reserved 0");
            builder.AppendLine(descriptor.ComponentListOffset, "  Component list offset");
            builder.AppendLine(descriptor.FileTableOffset, "  File table offset");
            builder.AppendLine(descriptor.Reserved1, "  Reserved 1");
            builder.AppendLine(descriptor.FileTableSize, "  File table size");
            builder.AppendLine(descriptor.FileTableSize2, "  File table size 2");
            builder.AppendLine(descriptor.DirectoryCount, "  Directory count");
            builder.AppendLine(descriptor.Reserved2, "  Reserved 2");
            builder.AppendLine(descriptor.Reserved3, "  Reserved 3");
            builder.AppendLine(descriptor.Reserved4, "  Reserved 4");
            builder.AppendLine(descriptor.FileCount, "  File count");
            builder.AppendLine(descriptor.FileTableOffset2, "  File table offset 2");
            builder.AppendLine(descriptor.ComponentTableInfoCount, "  Component table info count");
            builder.AppendLine(descriptor.ComponentTableOffset, "  Component table offset");
            builder.AppendLine(descriptor.Reserved5, "  Reserved 5");
            builder.AppendLine(descriptor.Reserved6, "  Reserved 6");
            builder.AppendLine();

            builder.AppendLine("  File group offsets:");
            builder.AppendLine("  -------------------------");
            if (descriptor.FileGroupOffsets == null || descriptor.FileGroupOffsets.Length == 0)
            {
                builder.AppendLine("  No file group offsets");
            }
            else
            {
                for (int i = 0; i < descriptor.FileGroupOffsets.Length; i++)
                {
                    builder.AppendLine(descriptor.FileGroupOffsets[i], $"      File Group Offset {i}");
                }
            }
            builder.AppendLine();

            builder.AppendLine("  Component offsets:");
            builder.AppendLine("  -------------------------");
            if (descriptor.ComponentOffsets == null || descriptor.ComponentOffsets.Length == 0)
            {
                builder.AppendLine("  No component offsets");
            }
            else
            {
                for (int i = 0; i < descriptor.ComponentOffsets.Length; i++)
                {
                    builder.AppendLine(descriptor.ComponentOffsets[i], $"      Component Offset {i}");
                }
            }
            builder.AppendLine();

            builder.AppendLine(descriptor.SetupTypesOffset, "  Setup types offset");
            builder.AppendLine(descriptor.SetupTableOffset, "  Setup table offset");
            builder.AppendLine(descriptor.Reserved7, "  Reserved 7");
            builder.AppendLine(descriptor.Reserved8, "  Reserved 8");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, uint[] entries)
#else
        private static void Print(StringBuilder builder, uint[]? entries)
#endif
        {
            builder.AppendLine("  File Descriptor Offsets:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file descriptor offsets");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                builder.AppendLine(entries[i], $"    File Descriptor Offset {i}");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, string[] entries)
#else
        private static void Print(StringBuilder builder, string?[]? entries)
#endif
        {
            builder.AppendLine("  Directory Names:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory names");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                builder.AppendLine(entries[i], $"    Directory Name {i}");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FileDescriptor[] entries)
#else
        private static void Print(StringBuilder builder, FileDescriptor?[]? entries)
#endif
        {
            builder.AppendLine("  File Descriptors:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file descriptors");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  File Descriptor {i}:");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.NameOffset, "    Name offset");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.DirectoryIndex, "    Directory index");
                builder.AppendLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
                builder.AppendLine(entry.ExpandedSize, "    Expanded size");
                builder.AppendLine(entry.CompressedSize, "    Compressed size");
                builder.AppendLine(entry.DataOffset, "    Data offset");
                builder.AppendLine(entry.MD5, "    MD5");
                builder.AppendLine(entry.Volume, "    Volume");
                builder.AppendLine(entry.LinkPrevious, "    Link previous");
                builder.AppendLine(entry.LinkNext, "    Link next");
                builder.AppendLine($"    Link flags: {entry.LinkFlags} (0x{entry.LinkFlags:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Dictionary<long, OffsetList> entries, string name)
#else
        private static void Print(StringBuilder builder, Dictionary<long, OffsetList?>? entries, string name)
#endif
        {
            builder.AppendLine($"  {name} Offsets:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Count == 0)
            {
                builder.AppendLine($"  No {name.ToLowerInvariant()} offsets");
                builder.AppendLine();
                return;
            }

            foreach (var kvp in entries)
            {
                long offset = kvp.Key;
                var value = kvp.Value;

                builder.AppendLine($"  {name} Offset {offset}:");
                if (value == null)
                {
                    builder.AppendLine($"    Unassigned {name.ToLowerInvariant()}");
                    continue;
                }

                builder.AppendLine(value.NameOffset, "    Name offset");
                builder.AppendLine(value.Name, "    Name");
                builder.AppendLine(value.DescriptorOffset, "    Descriptor offset");
                builder.AppendLine(value.NextOffset, "    Next offset");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FileGroup[] entries)
#else
        private static void Print(StringBuilder builder, FileGroup?[]? entries)
#endif
        {
            builder.AppendLine("  File Groups:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file groups");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var fileGroup = entries[i];
                builder.AppendLine($"  File Group {i}:");
                if (fileGroup == null)
                {
                    builder.AppendLine("    Unassigned file group");
                    continue;
                }

                builder.AppendLine(fileGroup.NameOffset, "    Name offset");
                builder.AppendLine(fileGroup.Name, "    Name");
                builder.AppendLine(fileGroup.ExpandedSize, "    Expanded size");
                builder.AppendLine(fileGroup.Reserved0, "    Reserved 0");
                builder.AppendLine(fileGroup.CompressedSize, "    Compressed size");
                builder.AppendLine(fileGroup.Reserved1, "    Reserved 1");
                builder.AppendLine(fileGroup.Reserved2, "    Reserved 2");
                builder.AppendLine(fileGroup.Attribute1, "    Attribute 1");
                builder.AppendLine(fileGroup.Attribute2, "    Attribute 2");
                builder.AppendLine(fileGroup.FirstFile, "    First file");
                builder.AppendLine(fileGroup.LastFile, "    Last file");
                builder.AppendLine(fileGroup.UnknownOffset, "    Unknown offset");
                builder.AppendLine(fileGroup.Var4Offset, "    Var 4 offset");
                builder.AppendLine(fileGroup.Var1Offset, "    Var 1 offset");
                builder.AppendLine(fileGroup.HTTPLocationOffset, "    HTTP location offset");
                builder.AppendLine(fileGroup.FTPLocationOffset, "    FTP location offset");
                builder.AppendLine(fileGroup.MiscOffset, "    Misc. offset");
                builder.AppendLine(fileGroup.Var2Offset, "    Var 2 offset");
                builder.AppendLine(fileGroup.TargetDirectoryOffset, "    Target directory offset");
                builder.AppendLine(fileGroup.Reserved3, "    Reserved 3");
                builder.AppendLine(fileGroup.Reserved4, "    Reserved 4");
                builder.AppendLine(fileGroup.Reserved5, "    Reserved 5");
                builder.AppendLine(fileGroup.Reserved6, "    Reserved 6");
                builder.AppendLine(fileGroup.Reserved7, "    Reserved 7");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Component[] entries)
#else
        private static void Print(StringBuilder builder, Component?[]? entries)
#endif
        {
            builder.AppendLine("  Components:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No components");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var component = entries[i];
                builder.AppendLine($"  Component {i}:");
                if (component == null)
                {
                    builder.AppendLine("    Unassigned component");
                    continue;
                }

                builder.AppendLine(component.IdentifierOffset, "    Identifier offset");
                builder.AppendLine(component.Identifier, "    Identifier");
                builder.AppendLine(component.DescriptorOffset, "    Descriptor offset");
                builder.AppendLine(component.DisplayNameOffset, "    Display name offset");
                builder.AppendLine(component.DisplayName, "    Display name");
                builder.AppendLine(component.Reserved0, "    Reserved 0");
                builder.AppendLine(component.ReservedOffset0, "    Reserved offset 0");
                builder.AppendLine(component.ReservedOffset1, "    Reserved offset 1");
                builder.AppendLine(component.ComponentIndex, "    Component index");
                builder.AppendLine(component.NameOffset, "    Name offset");
                builder.AppendLine(component.Name, "    Name");
                builder.AppendLine(component.ReservedOffset2, "    Reserved offset 2");
                builder.AppendLine(component.ReservedOffset3, "    Reserved offset 3");
                builder.AppendLine(component.ReservedOffset4, "    Reserved offset 4");
                builder.AppendLine(component.Reserved1, "    Reserved 1");
                builder.AppendLine(component.CLSIDOffset, "    CLSID offset");
                builder.AppendLine(component.CLSID, "    CLSID");
                builder.AppendLine(component.Reserved2, "    Reserved 2");
                builder.AppendLine(component.Reserved3, "    Reserved 3");
                builder.AppendLine(component.DependsCount, "    Depends count");
                builder.AppendLine(component.DependsOffset, "    Depends offset");
                builder.AppendLine(component.FileGroupCount, "    File group count");
                builder.AppendLine(component.FileGroupNamesOffset, "    File group names offset");
                builder.AppendLine();

                builder.AppendLine("    File group names:");
                builder.AppendLine("    -------------------------");
                if (component.FileGroupNames == null || component.FileGroupNames.Length == 0)
                {
                    builder.AppendLine("    No file group names");
                }
                else
                {
                    for (int j = 0; j < component.FileGroupNames.Length; j++)
                    {
                        builder.AppendLine(component.FileGroupNames[j], $"      File Group Name {j}");
                    }
                }
                builder.AppendLine();

                builder.AppendLine(component.X3Count, "    X3 count");
                builder.AppendLine(component.X3Offset, "    X3 offset");
                builder.AppendLine(component.SubComponentsCount, "    Sub-components count");
                builder.AppendLine(component.SubComponentsOffset, "    Sub-components offset");
                builder.AppendLine(component.NextComponentOffset, "    Next component offset");
                builder.AppendLine(component.ReservedOffset5, "    Reserved offset 5");
                builder.AppendLine(component.ReservedOffset6, "    Reserved offset 6");
                builder.AppendLine(component.ReservedOffset7, "    Reserved offset 7");
                builder.AppendLine(component.ReservedOffset8, "    Reserved offset 8");
            }
            builder.AppendLine();
        }
    }
}