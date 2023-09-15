using System.Text;
using SabreTools.Models.N3DS;

namespace BinaryObjectScanner.Printing
{
    public static class N3DS
    {
        public static void Print(StringBuilder builder, Cart cart)
        {
            builder.AppendLine("3DS Cart Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, cart.Header);
            Print(builder, cart.CardInfoHeader);
            Print(builder, cart.DevelopmentCardInfoHeader);
            Print(builder, cart.Partitions);
            Print(builder, cart.ExtendedHeaders);
            Print(builder, cart.ExeFSHeaders);
            Print(builder, cart.RomFSHeaders);
        }

#if NET48
        private static void Print(StringBuilder builder, NCSDHeader header)
#else
        private static void Print(StringBuilder builder, NCSDHeader? header)
#endif
        {
            builder.AppendLine("  NCSD Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No NCSD header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.RSA2048Signature, "  RSA-2048 SHA-256 signature");
            builder.AppendLine(header.MagicNumber, "  Magic number");
            builder.AppendLine(header.ImageSizeInMediaUnits, "  Image size in media units");
            builder.AppendLine(header.MediaId, "  Media ID");
            builder.AppendLine($"  Partitions filesystem type: {header.PartitionsFSType} (0x{header.PartitionsFSType:X})");
            builder.AppendLine(header.PartitionsCryptType, "  Partitions crypt type");
            builder.AppendLine();

            builder.AppendLine("  Partition table:");
            builder.AppendLine("  -------------------------");
            if (header.PartitionsTable == null || header.PartitionsTable.Length == 0)
            {
                builder.AppendLine("  No partition table entries");
            }
            else
            {
                for (int i = 0; i < header.PartitionsTable.Length; i++)
                {
                    var partitionTableEntry = header.PartitionsTable[i];
                    builder.AppendLine($"  Partition table entry {i}");
                    if (partitionTableEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine(partitionTableEntry.Offset, "    Offset");
                    builder.AppendLine(partitionTableEntry.Length, "    Length");
                }
            }
            builder.AppendLine();

            // If we have a cart image
            if (header.PartitionsFSType == FilesystemType.Normal || header.PartitionsFSType == FilesystemType.None)
            {
                builder.AppendLine(header.ExheaderHash, "  Exheader SHA-256 hash");
                builder.AppendLine(header.AdditionalHeaderSize, "  Additional header size");
                builder.AppendLine(header.SectorZeroOffset, "  Sector zero offset");
                builder.AppendLine(header.PartitionFlags, "  Partition flags");
                builder.AppendLine();

                builder.AppendLine("  Partition ID table:");
                builder.AppendLine("  -------------------------");
                if (header.PartitionIdTable == null || header.PartitionIdTable.Length == 0)
                {
                    builder.AppendLine("  No partition ID table entries");
                }
                else
                {
                    for (int i = 0; i < header.PartitionIdTable.Length; i++)
                    {
                        builder.AppendLine(header.PartitionIdTable[i], $"  Partition {i} ID");
                    }
                }
                builder.AppendLine();

                builder.AppendLine(header.Reserved1, "  Reserved 1");
                builder.AppendLine(header.Reserved2, "  Reserved 2");
                builder.AppendLine(header.FirmUpdateByte1, "  Firmware update byte 1");
                builder.AppendLine(header.FirmUpdateByte2, "  Firmware update byte 2");
            }

            // If we have a firmware image
            else if (header.PartitionsFSType == FilesystemType.FIRM)
            {
                builder.AppendLine(header.Unknown, "  Unknown");
                builder.AppendLine(header.EncryptedMBR, "  Encrypted MBR");
            }

            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, CardInfoHeader header)
#else
        private static void Print(StringBuilder builder, CardInfoHeader? header)
#endif
        {
            builder.AppendLine("  Card Info Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No card info header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.WritableAddressMediaUnits, "  Writable address in media units");
            builder.AppendLine(header.CardInfoBitmask, "  Card info bitmask");
            builder.AppendLine(header.Reserved1, "  Reserved 1");
            builder.AppendLine(header.FilledSize, "  Filled size of cartridge");
            builder.AppendLine(header.Reserved2, "  Reserved 2");
            builder.AppendLine(header.TitleVersion, "  Title version");
            builder.AppendLine(header.CardRevision, "  Card revision");
            builder.AppendLine(header.Reserved3, "  Reserved 3");
            builder.AppendLine(header.CVerTitleID, "  Title ID of CVer in included update partition");
            builder.AppendLine(header.CVerVersionNumber, "  Version number of CVer in included update partition");
            builder.AppendLine(header.Reserved4, "  Reserved 4");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DevelopmentCardInfoHeader header)
#else
        private static void Print(StringBuilder builder, DevelopmentCardInfoHeader? header)
#endif
        {
            builder.AppendLine("  Development Card Info Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No development card info header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine();
            builder.AppendLine("  Initial Data:");
            builder.AppendLine("  -------------------------");
            if (header.InitialData == null)
            {
                builder.AppendLine("  No initial data");
            }
            else
            {
                builder.AppendLine(header.InitialData.CardSeedKeyY, "  Card seed keyY");
                builder.AppendLine(header.InitialData.EncryptedCardSeed, "  Encrypted card seed");
                builder.AppendLine(header.InitialData.CardSeedAESMAC, "  Card seed AES-MAC");
                builder.AppendLine(header.InitialData.CardSeedNonce, "  Card seed nonce");
                builder.AppendLine(header.InitialData.Reserved, "  Reserved");
                builder.AppendLine();

                builder.AppendLine("  Backup Header:");
                builder.AppendLine("  -------------------------");
                if (header.InitialData.BackupHeader == null)
                {
                    builder.AppendLine("  No backup header");
                }
                else
                {
                    builder.AppendLine(header.InitialData.BackupHeader.MagicID, "    Magic ID");
                    builder.AppendLine(header.InitialData.BackupHeader.ContentSizeInMediaUnits, "    Content size in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.PartitionId, "    Partition ID");
                    builder.AppendLine(header.InitialData.BackupHeader.MakerCode, "    Maker code");
                    builder.AppendLine(header.InitialData.BackupHeader.Version, "    Version");
                    builder.AppendLine(header.InitialData.BackupHeader.VerificationHash, "    Verification hash");
                    builder.AppendLine(header.InitialData.BackupHeader.ProgramId, "    Program ID");
                    builder.AppendLine(header.InitialData.BackupHeader.Reserved1, "    Reserved 1");
                    builder.AppendLine(header.InitialData.BackupHeader.LogoRegionHash, "    Logo region SHA-256 hash");
                    builder.AppendLine(header.InitialData.BackupHeader.ProductCode, "    Product code");
                    builder.AppendLine(header.InitialData.BackupHeader.ExtendedHeaderHash, "    Extended header SHA-256 hash");
                    builder.AppendLine(header.InitialData.BackupHeader.ExtendedHeaderSizeInBytes, "    Extended header size in bytes");
                    builder.AppendLine(header.InitialData.BackupHeader.Reserved2, "    Reserved 2");
                    builder.AppendLine($"    Flags: {header.InitialData.BackupHeader.Flags} (0x{header.InitialData.BackupHeader.Flags:X})");
                    builder.AppendLine(header.InitialData.BackupHeader.PlainRegionOffsetInMediaUnits, "    Plain region offset, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.PlainRegionSizeInMediaUnits, "    Plain region size, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.LogoRegionOffsetInMediaUnits, "    Logo region offset, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.LogoRegionSizeInMediaUnits, "    Logo region size, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.ExeFSOffsetInMediaUnits, "    ExeFS offset, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.ExeFSSizeInMediaUnits, "    ExeFS size, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.ExeFSHashRegionSizeInMediaUnits, "    ExeFS hash region size, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.Reserved3, "    Reserved 3");
                    builder.AppendLine(header.InitialData.BackupHeader.RomFSOffsetInMediaUnits, "    RomFS offset, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.RomFSSizeInMediaUnits, "    RomFS size, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.RomFSHashRegionSizeInMediaUnits, "    RomFS hash region size, in media units");
                    builder.AppendLine(header.InitialData.BackupHeader.Reserved4, "    Reserved 4");
                    builder.AppendLine(header.InitialData.BackupHeader.ExeFSSuperblockHash, "    ExeFS superblock SHA-256 hash");
                    builder.AppendLine(header.InitialData.BackupHeader.RomFSSuperblockHash, "    RomFS superblock SHA-256 hash");
                }
            }
            builder.AppendLine();

            builder.AppendLine(header.CardDeviceReserved1, "  Card device reserved 1");
            builder.AppendLine(header.TitleKey, "  Title key");
            builder.AppendLine(header.CardDeviceReserved2, "  Card device reserved 2");
            builder.AppendLine();

            builder.AppendLine("  Test Data:");
            builder.AppendLine("  -------------------------");
            if (header.TestData == null)
            {
                builder.AppendLine("  No test data");
            }
            else
            {
                builder.AppendLine(header.TestData.Signature, "  Signature");
                builder.AppendLine(header.TestData.AscendingByteSequence, "  Ascending byte sequence");
                builder.AppendLine(header.TestData.DescendingByteSequence, "  Descending byte sequence");
                builder.AppendLine(header.TestData.Filled00, "  Filled with 00");
                builder.AppendLine(header.TestData.FilledFF, "  Filled with FF");
                builder.AppendLine(header.TestData.Filled0F, "  Filled with 0F");
                builder.AppendLine(header.TestData.FilledF0, "  Filled with F0");
                builder.AppendLine(header.TestData.Filled55, "  Filled with 55");
                builder.AppendLine(header.TestData.FilledAA, "  Filled with AA");
                builder.AppendLine(header.TestData.FinalByte, "  Final byte");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, NCCHHeader[] entries)
#else
        private static void Print(StringBuilder builder, NCCHHeader?[]? entries)
#endif
        {
            builder.AppendLine("  NCCH Partition Header Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No NCCH partition headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  NCCH Partition Header {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                if (entry.MagicID == string.Empty)
                {
                    builder.AppendLine("    Empty partition, no data can be parsed");
                }
                else if (entry.MagicID != Constants.NCCHMagicNumber)
                {
                    builder.AppendLine("    Unrecognized partition data, no data can be parsed");
                }
                else
                {
                    builder.AppendLine(entry.RSA2048Signature, "    RSA-2048 SHA-256 signature");
                    builder.AppendLine(entry.MagicID, "    Magic ID");
                    builder.AppendLine(entry.ContentSizeInMediaUnits, "    Content size in media units");
                    builder.AppendLine(entry.PartitionId, "    Partition ID");
                    builder.AppendLine(entry.MakerCode, "    Maker code");
                    builder.AppendLine(entry.Version, "    Version");
                    builder.AppendLine(entry.VerificationHash, "    Verification hash");
                    builder.AppendLine(entry.ProgramId, "    Program ID");
                    builder.AppendLine(entry.Reserved1, "    Reserved 1");
                    builder.AppendLine(entry.LogoRegionHash, "    Logo region SHA-256 hash");
                    builder.AppendLine(entry.ProductCode, "    Product code");
                    builder.AppendLine(entry.ExtendedHeaderHash, "    Extended header SHA-256 hash");
                    builder.AppendLine(entry.ExtendedHeaderSizeInBytes, "    Extended header size in bytes");
                    builder.AppendLine(entry.Reserved2, "    Reserved 2");
                    builder.AppendLine("    Flags:");
                    if (entry.Flags == null)
                    {
                        builder.AppendLine("      [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.Flags.Reserved0, "      Reserved 0");
                        builder.AppendLine(entry.Flags.Reserved1, "      Reserved 1");
                        builder.AppendLine(entry.Flags.Reserved2, "      Reserved 2");
                        builder.AppendLine($"      Crypto method: {entry.Flags.CryptoMethod} (0x{entry.Flags.CryptoMethod:X})");
                        builder.AppendLine($"      Content platform: {entry.Flags.ContentPlatform} (0x{entry.Flags.ContentPlatform:X})");
                        builder.AppendLine($"      Content type: {entry.Flags.MediaPlatformIndex} (0x{entry.Flags.MediaPlatformIndex:X})");
                        builder.AppendLine(entry.Flags.ContentUnitSize, "      Content unit size");
                        builder.AppendLine($"      Bitmasks: {entry.Flags.BitMasks} (0x{entry.Flags.BitMasks:X})");
                    }
                    builder.AppendLine(entry.PlainRegionOffsetInMediaUnits, "    Plain region offset, in media units");
                    builder.AppendLine(entry.PlainRegionSizeInMediaUnits, "    Plain region size, in media units");
                    builder.AppendLine(entry.LogoRegionOffsetInMediaUnits, "    Logo region offset, in media units");
                    builder.AppendLine(entry.LogoRegionSizeInMediaUnits, "    Logo region size, in media units");
                    builder.AppendLine(entry.ExeFSOffsetInMediaUnits, "    ExeFS offset, in media units");
                    builder.AppendLine(entry.ExeFSSizeInMediaUnits, "    ExeFS size, in media units");
                    builder.AppendLine(entry.ExeFSHashRegionSizeInMediaUnits, "    ExeFS hash region size, in media units");
                    builder.AppendLine(entry.Reserved3, "    Reserved 3");
                    builder.AppendLine(entry.RomFSOffsetInMediaUnits, "    RomFS offset, in media units");
                    builder.AppendLine(entry.RomFSSizeInMediaUnits, "    RomFS size, in media units");
                    builder.AppendLine(entry.RomFSHashRegionSizeInMediaUnits, "    RomFS hash region size, in media units");
                    builder.AppendLine(entry.Reserved4, "    Reserved 4");
                    builder.AppendLine(entry.ExeFSSuperblockHash, "    ExeFS superblock SHA-256 hash");
                    builder.AppendLine(entry.RomFSSuperblockHash, "    RomFS superblock SHA-256 hash");
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, NCCHExtendedHeader[] entries)
#else
        private static void Print(StringBuilder builder, NCCHExtendedHeader?[]? entries)
#endif
        {
            builder.AppendLine("  NCCH Extended Header Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No NCCH extended headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  NCCH Extended Header {i}");
                if (entry == null)
                {
                    builder.AppendLine("    Unrecognized partition data, no data can be parsed");
                    continue;
                }

                builder.AppendLine("    System control info:");
                if (entry.SCI == null)
                {
                    builder.AppendLine("      [NULL]");
                }
                else
                {
                    builder.AppendLine(entry.SCI.ApplicationTitle, "      Application title");
                    builder.AppendLine(entry.SCI.Reserved1, "      Reserved 1");
                    builder.AppendLine(entry.SCI.Flag, "      Flag");
                    builder.AppendLine(entry.SCI.RemasterVersion, "      Remaster version");

                    builder.AppendLine("      Text code set info:");
                    if (entry.SCI.TextCodeSetInfo == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.SCI.TextCodeSetInfo.Address, "        Address");
                        builder.AppendLine(entry.SCI.TextCodeSetInfo.PhysicalRegionSizeInPages, "        Physical region size (in page-multiples)");
                        builder.AppendLine(entry.SCI.TextCodeSetInfo.SizeInBytes, "        Size (in bytes)");
                    }

                    builder.AppendLine(entry.SCI.StackSize, "      Stack size");

                    builder.AppendLine("      Read-only code set info:");
                    if (entry.SCI.ReadOnlyCodeSetInfo == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.SCI.ReadOnlyCodeSetInfo.Address, "        Address");
                        builder.AppendLine(entry.SCI.ReadOnlyCodeSetInfo.PhysicalRegionSizeInPages, "        Physical region size (in page-multiples)");
                        builder.AppendLine(entry.SCI.ReadOnlyCodeSetInfo.SizeInBytes, "        Size (in bytes)");
                    }

                    builder.AppendLine(entry.SCI.Reserved2, "      Reserved 2");

                    builder.AppendLine("      Data code set info:");
                    if (entry.SCI.DataCodeSetInfo == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.SCI.DataCodeSetInfo.Address, "        Address");
                        builder.AppendLine(entry.SCI.DataCodeSetInfo.PhysicalRegionSizeInPages, "        Physical region size (in page-multiples)");
                        builder.AppendLine(entry.SCI.DataCodeSetInfo.SizeInBytes, "        Size (in bytes)");
                    }

                    builder.AppendLine(entry.SCI.BSSSize, "      BSS size");
                    builder.AppendLine(entry.SCI.DependencyModuleList, "      Dependency module list");

                    builder.AppendLine("      System info:");
                    if (entry.SCI.SystemInfo == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.SCI.SystemInfo.SaveDataSize, "        SaveData size");
                        builder.AppendLine(entry.SCI.SystemInfo.JumpID, "        Jump ID");
                        builder.AppendLine(entry.SCI.SystemInfo.Reserved, "        Reserved");
                    }
                }

                builder.AppendLine("    Access control info:");
                if (entry.ACI == null)
                {
                    builder.AppendLine("      [NULL]");
                }
                else
                {
                    builder.AppendLine("      ARM11 local system capabilities:");
                    if (entry.ACI.ARM11LocalSystemCapabilities == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.ProgramID, "        Program ID");
                        builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.CoreVersion, "        Core version");
                        builder.AppendLine($"        Flag 1: {entry.ACI.ARM11LocalSystemCapabilities.Flag1} (0x{entry.ACI.ARM11LocalSystemCapabilities.Flag1:X})");
                        builder.AppendLine($"        Flag 2: {entry.ACI.ARM11LocalSystemCapabilities.Flag2} (0x{entry.ACI.ARM11LocalSystemCapabilities.Flag2:X})");
                        builder.AppendLine($"        Flag 0: {entry.ACI.ARM11LocalSystemCapabilities.Flag0} (0x{entry.ACI.ARM11LocalSystemCapabilities.Flag0:X})");
                        builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.Priority, "        Priority");
                        builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.ResourceLimitDescriptors, "        Resource limit descriptors");
                        builder.AppendLine("        Storage info:");
                        if (entry.ACI.ARM11LocalSystemCapabilities.StorageInfo == null)
                        {
                            builder.AppendLine("          [NULL]");
                        }
                        else
                        {
                            builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID, "          Extdata ID");
                            builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.SystemSavedataIDs, "          System savedata IDs");
                            builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.StorageAccessibleUniqueIDs, "          Storage accessible unique IDs");
                            builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.FileSystemAccessInfo, "          File system access info");
                            builder.AppendLine($"          Other attributes: {entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes} (0x{entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes:X})");
                        }

                        builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.ServiceAccessControl, "        Service access control");
                        builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.ExtendedServiceAccessControl, "        Extended service access control");
                        builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.Reserved, "        Reserved");
                        builder.AppendLine($"        Resource limit cateogry: {entry.ACI.ARM11LocalSystemCapabilities.ResourceLimitCategory} (0x{entry.ACI.ARM11LocalSystemCapabilities.ResourceLimitCategory:X})");
                    }

                    builder.AppendLine("      ARM11 kernel capabilities:");
                    if (entry.ACI.ARM11KernelCapabilities == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.ACI.ARM11KernelCapabilities.Descriptors, "        Descriptors");
                        builder.AppendLine(entry.ACI.ARM11KernelCapabilities.Reserved, "        Reserved");
                    }

                    builder.AppendLine("      ARM9 access control:");
                    if (entry.ACI.ARM9AccessControl == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.ACI.ARM9AccessControl.Descriptors, "        Descriptors");
                        builder.AppendLine(entry.ACI.ARM9AccessControl.DescriptorVersion, "        Descriptor version");
                    }

                    builder.AppendLine(entry.AccessDescSignature, "    AccessDec signature (RSA-2048-SHA256)");
                    builder.AppendLine(entry.NCCHHDRPublicKey, "    NCCH HDR RSA-2048 public key");
                }

                builder.AppendLine("    Access control info (for limitations of first ACI):");
                if (entry.ACIForLimitations == null)
                {
                    builder.AppendLine("      [NULL]");
                }
                else
                {
                    builder.AppendLine("      ARM11 local system capabilities:");
                    if (entry.ACIForLimitations.ARM11LocalSystemCapabilities == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.ProgramID, "        Program ID");
                        builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.CoreVersion, "        Core version");
                        builder.AppendLine($"        Flag 1: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag1} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag1:X})");
                        builder.AppendLine($"        Flag 2: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag2} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag2:X})");
                        builder.AppendLine($"        Flag 0: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag0} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag0:X})");
                        builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.Priority, "        Priority");
                        builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitDescriptors, "        Resource limit descriptors");

                        builder.AppendLine("        Storage info:");
                        if (entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo == null)
                        {
                            builder.AppendLine("          [NULL]");
                        }
                        else
                        {
                            builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID, "          Extdata ID");
                            builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.SystemSavedataIDs, "          System savedata IDs");
                            builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.StorageAccessibleUniqueIDs, "          Storage accessible unique IDs");
                            builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.FileSystemAccessInfo, "          File system access info");
                            builder.AppendLine($"          Other attributes: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes:X})");
                        }

                        builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.ServiceAccessControl, "        Service access control");
                        builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.ExtendedServiceAccessControl, "        Extended service access control");
                        builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.Reserved, "        Reserved");
                        builder.AppendLine($"        Resource limit cateogry: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitCategory} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitCategory:X})");
                    }

                    builder.AppendLine("      ARM11 kernel capabilities:");
                    if (entry.ACIForLimitations.ARM11KernelCapabilities == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.ACIForLimitations.ARM11KernelCapabilities.Descriptors, "        Descriptors");
                        builder.AppendLine(entry.ACIForLimitations.ARM11KernelCapabilities.Reserved, "        Reserved");
                    }

                    builder.AppendLine("      ARM9 access control:");
                    if (entry.ACIForLimitations.ARM9AccessControl == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {
                        builder.AppendLine(entry.ACIForLimitations.ARM9AccessControl.Descriptors, "        Descriptors");
                        builder.AppendLine(entry.ACIForLimitations.ARM9AccessControl.DescriptorVersion, "        Descriptor version");
                    }
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ExeFSHeader[] entries)
#else
        private static void Print(StringBuilder builder, ExeFSHeader?[]? entries)
#endif
        {
            builder.AppendLine("  ExeFS Header Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No ExeFS headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  ExeFS Header {i}");
                if (entry == null)
                {
                    builder.AppendLine("    Unrecognized partition data, no data can be parsed");
                    continue;
                }

                builder.AppendLine("    File headers:");
                if (entry.FileHeaders == null || entry.FileHeaders.Length == 0)
                {
                    builder.AppendLine("    No file headers");
                }
                else
                {
                    for (int j = 0; j < entry.FileHeaders.Length; j++)
                    {
                        var fileHeader = entry.FileHeaders[j];
                        builder.AppendLine($"    File Header {j}");
                        if (fileHeader == null)
                        {
                            builder.AppendLine("      [NULL]");
                            continue;
                        }

                        builder.AppendLine(fileHeader.FileName, "      File name");
                        builder.AppendLine(fileHeader.FileOffset, "      File offset");
                        builder.AppendLine(fileHeader.FileSize, "      File size");
                    }
                }

                builder.AppendLine(entry.Reserved, "    Reserved");

                builder.AppendLine("    File hashes:");
                if (entry.FileHashes == null || entry.FileHashes.Length == 0)
                {
                    builder.AppendLine("    No file hashes");
                }
                else
                {
                    for (int j = 0; j < entry.FileHashes.Length; j++)
                    {
                        var fileHash = entry.FileHashes[j];
                        builder.AppendLine($"    File Hash {j}");
                        if (fileHash == null)
                        {
                            builder.AppendLine("      [NULL]");
                            continue;
                        }

                        builder.AppendLine(fileHash, "      SHA-256");
                    }
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, RomFSHeader[] entries)
#else
        private static void Print(StringBuilder builder, RomFSHeader?[]? entries)
#endif
        {
            builder.AppendLine("  RomFS Header Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No RomFS headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var romFSHeader = entries[i];
                builder.AppendLine($"  RomFS Header {i}");
                if (romFSHeader == null)
                {
                    builder.AppendLine("    Unrecognized RomFS data, no data can be parsed");
                    continue;
                }

                builder.AppendLine(romFSHeader.MagicString, "    Magic string");
                builder.AppendLine(romFSHeader.MagicNumber, "    Magic number");
                builder.AppendLine(romFSHeader.MasterHashSize, "    Master hash size");
                builder.AppendLine(romFSHeader.Level1LogicalOffset, "    Level 1 logical offset");
                builder.AppendLine(romFSHeader.Level1HashdataSize, "    Level 1 hashdata size");
                builder.AppendLine(romFSHeader.Level1BlockSizeLog2, "    Level 1 block size");
                builder.AppendLine(romFSHeader.Reserved1, "    Reserved 1");
                builder.AppendLine(romFSHeader.Level2LogicalOffset, "    Level 2 logical offset");
                builder.AppendLine(romFSHeader.Level2HashdataSize, "    Level 2 hashdata size");
                builder.AppendLine(romFSHeader.Level2BlockSizeLog2, "    Level 2 block size");
                builder.AppendLine(romFSHeader.Reserved2, "    Reserved 2");
                builder.AppendLine(romFSHeader.Level3LogicalOffset, "    Level 3 logical offset");
                builder.AppendLine(romFSHeader.Level3HashdataSize, "    Level 3 hashdata size");
                builder.AppendLine(romFSHeader.Level3BlockSizeLog2, "    Level 3 block size");
                builder.AppendLine(romFSHeader.Reserved3, "    Reserved 3");
                builder.AppendLine(romFSHeader.Reserved4, "    Reserved 4");
                builder.AppendLine(romFSHeader.OptionalInfoSize, "    Optional info size");
            }
            builder.AppendLine();
        }
    }
}