using System.Text;
using SabreTools.Models.Nitro;

namespace BinaryObjectScanner.Printing
{
    public static class Nitro
    {
        public static void Print(StringBuilder builder, Cart cart)
        {
            builder.AppendLine("NDS Cart Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, cart.CommonHeader);
            Print(builder, cart.ExtendedDSiHeader);
            Print(builder, cart.SecureArea);
            Print(builder, cart.NameTable);
            Print(builder, cart.FileAllocationTable);
        }

#if NET48
        private static void Print(StringBuilder builder, CommonHeader header)
#else
        private static void Print(StringBuilder builder, CommonHeader? header)
#endif
        {
            builder.AppendLine("  Common Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No common header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.GameTitle, "  Game title");
            builder.AppendLine(header.GameCode, "  Game code");
            builder.AppendLine(header.MakerCode, "  Maker code");
            builder.AppendLine($"  Unit code: {header.UnitCode} (0x{header.UnitCode:X})");
            builder.AppendLine(header.EncryptionSeedSelect, "  Encryption seed select");
            builder.AppendLine(header.DeviceCapacity, "  Device capacity");
            builder.AppendLine(header.Reserved1, "  Reserved 1");
            builder.AppendLine(header.GameRevision, "  Game revision");
            builder.AppendLine(header.RomVersion, "  Rom version");
            builder.AppendLine(header.ARM9RomOffset, "  ARM9 rom offset");
            builder.AppendLine(header.ARM9EntryAddress, "  ARM9 entry address");
            builder.AppendLine(header.ARM9LoadAddress, "  ARM9 load address");
            builder.AppendLine(header.ARM9Size, "  ARM9 size");
            builder.AppendLine(header.ARM7RomOffset, "  ARM7 rom offset");
            builder.AppendLine(header.ARM7EntryAddress, "  ARM7 entry address");
            builder.AppendLine(header.ARM7LoadAddress, "  ARM7 load address");
            builder.AppendLine(header.ARM7Size, "  ARM7 size");
            builder.AppendLine(header.FileNameTableOffset, "  File name table offset");
            builder.AppendLine(header.FileNameTableLength, "  File name table length");
            builder.AppendLine(header.FileAllocationTableOffset, "  File allocation table offset");
            builder.AppendLine(header.FileAllocationTableLength, "  File allocation table length");
            builder.AppendLine(header.ARM9OverlayOffset, "  ARM9 overlay offset");
            builder.AppendLine(header.ARM9OverlayLength, "  ARM9 overlay length");
            builder.AppendLine(header.ARM7OverlayOffset, "  ARM7 overlay offset");
            builder.AppendLine(header.ARM7OverlayLength, "  ARM7 overlay length");
            builder.AppendLine(header.NormalCardControlRegisterSettings, "  Normal card control register settings");
            builder.AppendLine(header.SecureCardControlRegisterSettings, "  Secure card control register settings");
            builder.AppendLine(header.IconBannerOffset, "  Icon banner offset");
            builder.AppendLine(header.SecureAreaCRC, "  Secure area CRC");
            builder.AppendLine(header.SecureTransferTimeout, "  Secure transfer timeout");
            builder.AppendLine(header.ARM9Autoload, "  ARM9 autoload");
            builder.AppendLine(header.ARM7Autoload, "  ARM7 autoload");
            builder.AppendLine(header.SecureDisable, "  Secure disable");
            builder.AppendLine(header.NTRRegionRomSize, "  NTR region rom size");
            builder.AppendLine(header.HeaderSize, "  Header size");
            builder.AppendLine(header.Reserved2, "  Reserved 2");
            builder.AppendLine(header.NintendoLogo, "  Nintendo logo");
            builder.AppendLine(header.NintendoLogoCRC, "  Nintendo logo CRC");
            builder.AppendLine(header.HeaderCRC, "  Header CRC");
            builder.AppendLine(header.DebuggerReserved, "  Debugger reserved");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ExtendedDSiHeader header)
#else
        private static void Print(StringBuilder builder, ExtendedDSiHeader? header)
#endif
        {
            builder.AppendLine("  Extended DSi Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No extended DSi header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.GlobalMBK15Settings, "  Global MBK1..MBK5 settings");
            builder.AppendLine(header.LocalMBK68SettingsARM9, "  Local MBK6..MBK8 settings for ARM9");
            builder.AppendLine(header.LocalMBK68SettingsARM7, "  Local MBK6..MBK8 settings for ARM7");
            builder.AppendLine(header.GlobalMBK9Setting, "  Global MBK9 setting");
            builder.AppendLine(header.RegionFlags, "  Region flags");
            builder.AppendLine(header.AccessControl, "  Access control");
            builder.AppendLine(header.ARM7SCFGEXTMask, "  ARM7 SCFG EXT mask");
            builder.AppendLine(header.ReservedFlags, "  Reserved/flags?");
            builder.AppendLine(header.ARM9iRomOffset, "  ARM9i rom offset");
            builder.AppendLine(header.Reserved3, "  Reserved 3");
            builder.AppendLine(header.ARM9iLoadAddress, "  ARM9i load address");
            builder.AppendLine(header.ARM9iSize, "  ARM9i size");
            builder.AppendLine(header.ARM7iRomOffset, "  ARM7i rom offset");
            builder.AppendLine(header.Reserved4, "  Reserved 4");
            builder.AppendLine(header.ARM7iLoadAddress, "  ARM7i load address");
            builder.AppendLine(header.ARM7iSize, "  ARM7i size");
            builder.AppendLine(header.DigestNTRRegionOffset, "  Digest NTR region offset");
            builder.AppendLine(header.DigestNTRRegionLength, "  Digest NTR region length");
            builder.AppendLine(header.DigestTWLRegionOffset, "  Digest TWL region offset");
            builder.AppendLine(header.DigestTWLRegionLength, "  Digest TWL region length");
            builder.AppendLine(header.DigestSectorHashtableRegionOffset, "  Digest sector hashtable region offset");
            builder.AppendLine(header.DigestSectorHashtableRegionLength, "  Digest sector hashtable region length");
            builder.AppendLine(header.DigestBlockHashtableRegionOffset, "  Digest block hashtable region offset");
            builder.AppendLine(header.DigestBlockHashtableRegionLength, "  Digest block hashtable region length");
            builder.AppendLine(header.DigestSectorSize, "  Digest sector size");
            builder.AppendLine(header.DigestBlockSectorCount, "  Digest block sector count");
            builder.AppendLine(header.IconBannerSize, "  Icon banner size");
            builder.AppendLine(header.Unknown1, "  Unknown 1");
            builder.AppendLine(header.ModcryptArea1Offset, "  Modcrypt area 1 offset");
            builder.AppendLine(header.ModcryptArea1Size, "  Modcrypt area 1 size");
            builder.AppendLine(header.ModcryptArea2Offset, "  Modcrypt area 2 offset");
            builder.AppendLine(header.ModcryptArea2Size, "  Modcrypt area 2 size");
            builder.AppendLine(header.TitleID, "  Title ID");
            builder.AppendLine(header.DSiWarePublicSavSize, "  DSiWare 'public.sav' size");
            builder.AppendLine(header.DSiWarePrivateSavSize, "  DSiWare 'private.sav' size");
            builder.AppendLine(header.ReservedZero, "  Reserved (zero)");
            builder.AppendLine(header.Unknown2, "  Unknown 2");
            builder.AppendLine(header.ARM9WithSecureAreaSHA1HMACHash, "  ARM9 (with encrypted secure area) SHA1 HMAC hash");
            builder.AppendLine(header.ARM7SHA1HMACHash, "  ARM7 SHA1 HMAC hash");
            builder.AppendLine(header.DigestMasterSHA1HMACHash, "  Digest master SHA1 HMAC hash");
            builder.AppendLine(header.BannerSHA1HMACHash, "  Banner SHA1 HMAC hash");
            builder.AppendLine(header.ARM9iDecryptedSHA1HMACHash, "  ARM9i (decrypted) SHA1 HMAC hash");
            builder.AppendLine(header.ARM7iDecryptedSHA1HMACHash, "  ARM7i (decrypted) SHA1 HMAC hash");
            builder.AppendLine(header.Reserved5, "  Reserved 5");
            builder.AppendLine(header.ARM9NoSecureAreaSHA1HMACHash, "  ARM9 (without secure area) SHA1 HMAC hash");
            builder.AppendLine(header.Reserved6, "  Reserved 6");
            builder.AppendLine(header.ReservedAndUnchecked, "  Reserved and unchecked region");
            builder.AppendLine(header.RSASignature, "  RSA signature");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, byte[] secureArea)
#else
        private static void Print(StringBuilder builder, byte[]? secureArea)
#endif
        {
            builder.AppendLine("  Secure Area Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(secureArea, "  Secure Area");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, NameTable table)
#else
        private static void Print(StringBuilder builder, NameTable? table)
#endif
        {
            builder.AppendLine("  Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null)
            {
                builder.AppendLine("  No name table");
                builder.AppendLine();
                return;
            }
            builder.AppendLine();

            Print(builder, table.FolderAllocationTable);
            Print(builder, table.NameList);
        }

#if NET48
        private static void Print(StringBuilder builder, FolderAllocationTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, FolderAllocationTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Folder Allocation Table:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No folder allocation table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Folder Allocation Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.StartOffset, "    Start offset");
                builder.AppendLine(entry.FirstFileIndex, "    First file index");
                if (entry.Unknown == 0xF0)
                {
                    builder.AppendLine(entry.ParentFolderIndex, "    Parent folder index");
                    builder.AppendLine(entry.Unknown, "    Unknown");
                }
                else
                {
                    ushort totalEntries = (ushort)((entry.Unknown << 8) | entry.ParentFolderIndex);
                    builder.AppendLine(totalEntries, "    Total entries");
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, NameListEntry[] entries)
#else
        private static void Print(StringBuilder builder, NameListEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Name List:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No name list entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Name List Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Folder, "    Folder");
                builder.AppendLine(entry.Name, "    Name");
                if (entry.Folder)
                    builder.AppendLine(entry.Index, "    Index");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FileAllocationTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, FileAllocationTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  File Allocation Table:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file allocation table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  File Allocation Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.StartOffset, "    Start offset");
                builder.AppendLine(entry.EndOffset, "    End offset");
            }
            builder.AppendLine();
        }
    }
}