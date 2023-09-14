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

        private static int GetMajorVersion(CommonHeader header)
        {
#if NET48
            uint majorVersion = header.Version;
#else
                uint majorVersion = header.Version ?? 0;
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

            builder.AppendLine($"  Signature: {header.Signature}");
            builder.AppendLine($"  Version: {header.Version} (0x{header.Version:X}) [{majorVersion}]");
            builder.AppendLine($"  Volume info: {header.VolumeInfo} (0x{header.VolumeInfo:X})");
            builder.AppendLine($"  Descriptor offset: {header.DescriptorOffset} (0x{header.DescriptorOffset:X})");
            builder.AppendLine($"  Descriptor size: {header.DescriptorSize} (0x{header.DescriptorSize:X})");
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
                builder.AppendLine($"  Data offset: {header.DataOffset} (0x{header.DataOffset:X})");
                builder.AppendLine($"  First file index: {header.FirstFileIndex} (0x{header.FirstFileIndex:X})");
                builder.AppendLine($"  Last file index: {header.LastFileIndex} (0x{header.LastFileIndex:X})");
                builder.AppendLine($"  First file offset: {header.FirstFileOffset} (0x{header.FirstFileOffset:X})");
                builder.AppendLine($"  First file size expanded: {header.FirstFileSizeExpanded} (0x{header.FirstFileSizeExpanded:X})");
                builder.AppendLine($"  First file size compressed: {header.FirstFileSizeCompressed} (0x{header.FirstFileSizeCompressed:X})");
                builder.AppendLine($"  Last file offset: {header.LastFileOffset} (0x{header.LastFileOffset:X})");
                builder.AppendLine($"  Last file size expanded: {header.LastFileSizeExpanded} (0x{header.LastFileSizeExpanded:X})");
                builder.AppendLine($"  Last file size compressed: {header.LastFileSizeCompressed} (0x{header.LastFileSizeCompressed:X})");
            }
            else
            {
                builder.AppendLine($"  Data offset: {header.DataOffset} (0x{header.DataOffset:X})");
                builder.AppendLine($"  Data offset high: {header.DataOffsetHigh} (0x{header.DataOffsetHigh:X})");
                builder.AppendLine($"  First file index: {header.FirstFileIndex} (0x{header.FirstFileIndex:X})");
                builder.AppendLine($"  Last file index: {header.LastFileIndex} (0x{header.LastFileIndex:X})");
                builder.AppendLine($"  First file offset: {header.FirstFileOffset} (0x{header.FirstFileOffset:X})");
                builder.AppendLine($"  First file offset high: {header.FirstFileOffsetHigh} (0x{header.FirstFileOffsetHigh:X})");
                builder.AppendLine($"  First file size expanded: {header.FirstFileSizeExpanded} (0x{header.FirstFileSizeExpanded:X})");
                builder.AppendLine($"  First file size expanded high: {header.FirstFileSizeExpandedHigh} (0x{header.FirstFileSizeExpandedHigh:X})");
                builder.AppendLine($"  First file size compressed: {header.FirstFileSizeCompressed} (0x{header.FirstFileSizeCompressed:X})");
                builder.AppendLine($"  First file size compressed high: {header.FirstFileSizeCompressedHigh} (0x{header.FirstFileSizeCompressedHigh:X})");
                builder.AppendLine($"  Last file offset: {header.LastFileOffset} (0x{header.LastFileOffset:X})");
                builder.AppendLine($"  Last file offset high: {header.LastFileOffsetHigh} (0x{header.LastFileOffsetHigh:X})");
                builder.AppendLine($"  Last file size expanded: {header.LastFileSizeExpanded} (0x{header.LastFileSizeExpanded:X})");
                builder.AppendLine($"  Last file size expanded high: {header.LastFileSizeExpandedHigh} (0x{header.LastFileSizeExpandedHigh:X})");
                builder.AppendLine($"  Last file size compressed: {header.LastFileSizeCompressed} (0x{header.LastFileSizeCompressed:X})");
                builder.AppendLine($"  Last file size compressed high: {header.LastFileSizeCompressedHigh} (0x{header.LastFileSizeCompressedHigh:X})");
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
            builder.AppendLine($"  Strings offset: {descriptor.StringsOffset} (0x{descriptor.StringsOffset:X})");
            builder.AppendLine($"  Reserved 0: {(descriptor.Reserved0 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved0).Replace('-', ' '))}");
            builder.AppendLine($"  Component list offset: {descriptor.ComponentListOffset} (0x{descriptor.ComponentListOffset:X})");
            builder.AppendLine($"  File table offset: {descriptor.FileTableOffset} (0x{descriptor.FileTableOffset:X})");
            builder.AppendLine($"  Reserved 1: {(descriptor.Reserved1 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved1).Replace('-', ' '))}");
            builder.AppendLine($"  File table size: {descriptor.FileTableSize} (0x{descriptor.FileTableSize:X})");
            builder.AppendLine($"  File table size 2: {descriptor.FileTableSize2} (0x{descriptor.FileTableSize2:X})");
            builder.AppendLine($"  Directory count: {descriptor.DirectoryCount} (0x{descriptor.DirectoryCount:X})");
            builder.AppendLine($"  Reserved 2: {(descriptor.Reserved2 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved2).Replace('-', ' '))}");
            builder.AppendLine($"  Reserved 3: {(descriptor.Reserved3 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved3).Replace('-', ' '))}");
            builder.AppendLine($"  Reserved 4: {(descriptor.Reserved4 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved4).Replace('-', ' '))}");
            builder.AppendLine($"  File count: {descriptor.FileCount} (0x{descriptor.FileCount:X})");
            builder.AppendLine($"  File table offset 2: {descriptor.FileTableOffset2} (0x{descriptor.FileTableOffset2:X})");
            builder.AppendLine($"  Component table info count: {descriptor.ComponentTableInfoCount} (0x{descriptor.ComponentTableInfoCount:X})");
            builder.AppendLine($"  Component table offset: {descriptor.ComponentTableOffset} (0x{descriptor.ComponentTableOffset:X})");
            builder.AppendLine($"  Reserved 5: {(descriptor.Reserved5 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved5).Replace('-', ' '))}");
            builder.AppendLine($"  Reserved 6: {(descriptor.Reserved6 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved6).Replace('-', ' '))}");
            builder.AppendLine();

            builder.AppendLine($"  File group offsets:");
            builder.AppendLine("  -------------------------");
            if (descriptor.FileGroupOffsets == null || descriptor.FileGroupOffsets.Length == 0)
            {
                builder.AppendLine("  No file group offsets");
            }
            else
            {
                for (int i = 0; i < descriptor.FileGroupOffsets.Length; i++)
                {
                    builder.AppendLine($"      File Group Offset {i}: {descriptor.FileGroupOffsets[i]} (0x{descriptor.FileGroupOffsets[i]:X})");
                }
            }
            builder.AppendLine();

            builder.AppendLine($"  Component offsets:");
            builder.AppendLine("  -------------------------");
            if (descriptor.ComponentOffsets == null || descriptor.ComponentOffsets.Length == 0)
            {
                builder.AppendLine("  No component offsets");
            }
            else
            {
                for (int i = 0; i < descriptor.ComponentOffsets.Length; i++)
                {
                    builder.AppendLine($"      Component Offset {i}: {descriptor.ComponentOffsets[i]} (0x{descriptor.ComponentOffsets[i]:X})");
                }
            }
            builder.AppendLine();

            builder.AppendLine($"  Setup types offset: {descriptor.SetupTypesOffset} (0x{descriptor.SetupTypesOffset:X})");
            builder.AppendLine($"  Setup table offset: {descriptor.SetupTableOffset} (0x{descriptor.SetupTableOffset:X})");
            builder.AppendLine($"  Reserved 7: {(descriptor.Reserved7 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved7).Replace('-', ' '))}");
            builder.AppendLine($"  Reserved 8: {(descriptor.Reserved8 == null ? "[NULL]" : BitConverter.ToString(descriptor.Reserved8).Replace('-', ' '))}");
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
                builder.AppendLine($"    File Descriptor Offset {i}: {entries[i]} (0x{entries[i]:X})");
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
                builder.AppendLine($"    Directory Name {i}: {entries[i] ?? "[NULL]"}");
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

                builder.AppendLine($"    Name offset: {entry.NameOffset} (0x{entry.NameOffset:X})");
                builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                builder.AppendLine($"    Directory index: {entry.DirectoryIndex} (0x{entry.DirectoryIndex:X})");
                builder.AppendLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
                builder.AppendLine($"    Expanded size: {entry.ExpandedSize} (0x{entry.ExpandedSize:X})");
                builder.AppendLine($"    Compressed size: {entry.CompressedSize} (0x{entry.CompressedSize:X})");
                builder.AppendLine($"    Data offset: {entry.DataOffset} (0x{entry.DataOffset:X})");
                builder.AppendLine($"    MD5: {(entry.MD5 == null ? "[NULL]" : BitConverter.ToString(entry.MD5).Replace('-', ' '))}");
                builder.AppendLine($"    Volume: {entry.Volume} (0x{entry.Volume:X})");
                builder.AppendLine($"    Link previous: {entry.LinkPrevious} (0x{entry.LinkPrevious:X})");
                builder.AppendLine($"    Link next: {entry.LinkNext} (0x{entry.LinkNext:X})");
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

                builder.AppendLine($"    Name offset: {value.NameOffset} (0x{value.NameOffset:X})");
                builder.AppendLine($"    Name: {value.Name ?? "[NULL]"}");
                builder.AppendLine($"    Descriptor offset: {value.DescriptorOffset} (0x{value.DescriptorOffset:X})");
                builder.AppendLine($"    Next offset: {value.NextOffset} (0x{value.NextOffset:X})");
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
                    builder.AppendLine($"    Unassigned file group");
                    continue;
                }

                builder.AppendLine($"    Name offset: {fileGroup.NameOffset} (0x{fileGroup.NameOffset:X})");
                builder.AppendLine($"    Name: {fileGroup.Name ?? "[NULL]"}");
                builder.AppendLine($"    Expanded size: {fileGroup.ExpandedSize} (0x{fileGroup.ExpandedSize:X})");
                builder.AppendLine($"    Reserved 0: {(fileGroup.Reserved0 == null ? "[NULL]" : BitConverter.ToString(fileGroup.Reserved0).Replace('-', ' '))}");
                builder.AppendLine($"    Compressed size: {fileGroup.CompressedSize} (0x{fileGroup.CompressedSize:X})");
                builder.AppendLine($"    Reserved 1: {(fileGroup.Reserved1 == null ? "[NULL]" : BitConverter.ToString(fileGroup.Reserved1).Replace('-', ' '))}");
                builder.AppendLine($"    Reserved 2: {(fileGroup.Reserved2 == null ? "[NULL]" : BitConverter.ToString(fileGroup.Reserved2).Replace('-', ' '))}");
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
                builder.AppendLine($"    Reserved 3: {(fileGroup.Reserved3 == null ? "[NULL]" : BitConverter.ToString(fileGroup.Reserved3).Replace('-', ' '))}");
                builder.AppendLine($"    Reserved 4: {(fileGroup.Reserved4 == null ? "[NULL]" : BitConverter.ToString(fileGroup.Reserved4).Replace('-', ' '))}");
                builder.AppendLine($"    Reserved 5: {(fileGroup.Reserved5 == null ? "[NULL]" : BitConverter.ToString(fileGroup.Reserved5).Replace('-', ' '))}");
                builder.AppendLine($"    Reserved 6: {(fileGroup.Reserved6 == null ? "[NULL]" : BitConverter.ToString(fileGroup.Reserved6).Replace('-', ' '))}");
                builder.AppendLine($"    Reserved 7: {(fileGroup.Reserved7 == null ? "[NULL]" : BitConverter.ToString(fileGroup.Reserved7).Replace('-', ' '))}");
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
                    builder.AppendLine($"    Unassigned component");
                    continue;
                }

                builder.AppendLine($"    Identifier offset: {component.IdentifierOffset} (0x{component.IdentifierOffset:X})");
                builder.AppendLine($"    Identifier: {component.Identifier ?? "[NULL]"}");
                builder.AppendLine($"    Descriptor offset: {component.DescriptorOffset} (0x{component.DescriptorOffset:X})");
                builder.AppendLine($"    Display name offset: {component.DisplayNameOffset} (0x{component.DisplayNameOffset:X})");
                builder.AppendLine($"    Display name: {component.DisplayName ?? "[NULL]"}");
                builder.AppendLine($"    Reserved 0: {(component.Reserved0 == null ? "[NULL]" : BitConverter.ToString(component.Reserved0).Replace('-', ' '))}");
                builder.AppendLine($"    Reserved offset 0: {component.ReservedOffset0} (0x{component.ReservedOffset0:X})");
                builder.AppendLine($"    Reserved offset 1: {component.ReservedOffset1} (0x{component.ReservedOffset1:X})");
                builder.AppendLine($"    Component index: {component.ComponentIndex} (0x{component.ComponentIndex:X})");
                builder.AppendLine($"    Name offset: {component.NameOffset} (0x{component.NameOffset:X})");
                builder.AppendLine($"    Name: {component.Name ?? "[NULL]"}");
                builder.AppendLine($"    Reserved offset 2: {component.ReservedOffset2} (0x{component.ReservedOffset2:X})");
                builder.AppendLine($"    Reserved offset 3: {component.ReservedOffset3} (0x{component.ReservedOffset3:X})");
                builder.AppendLine($"    Reserved offset 4: {component.ReservedOffset4} (0x{component.ReservedOffset4:X})");
                builder.AppendLine($"    Reserved 1: {(component.Reserved1 == null ? "[NULL]" : BitConverter.ToString(component.Reserved1).Replace('-', ' '))}");
                builder.AppendLine($"    CLSID offset: {component.CLSIDOffset} (0x{component.CLSIDOffset:X})");
                builder.AppendLine($"    CLSID: {component.CLSID}");
                builder.AppendLine($"    Reserved 2: {(component.Reserved2 == null ? "[NULL]" : BitConverter.ToString(component.Reserved2).Replace('-', ' '))}");
                builder.AppendLine($"    Reserved 3: {(component.Reserved3 == null ? "[NULL]" : BitConverter.ToString(component.Reserved3).Replace('-', ' '))}");
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