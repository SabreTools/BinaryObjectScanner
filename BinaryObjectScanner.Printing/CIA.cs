using System.Text;
using SabreTools.Models.N3DS;

namespace BinaryObjectScanner.Printing
{
    public static class CIA
    {
        public static void Print(StringBuilder builder, SabreTools.Models.N3DS.CIA cia)
        {
            builder.AppendLine("CIA Archive Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, cia.Header);
            Print(builder, cia.CertificateChain);
            Print(builder, cia.Ticket);
            Print(builder, cia.TMDFileData);
            Print(builder, cia.Partitions);
            Print(builder, cia.MetaData);
        }

#if NET48
        private static void Print(StringBuilder builder, CIAHeader header)
#else
        private static void Print(StringBuilder builder, CIAHeader? header)
#endif
        {
            builder.AppendLine("  CIA Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No CIA header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.HeaderSize, "  Header size");
            builder.AppendLine(header.Type, "  Type");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.CertificateChainSize, "  Certificate chain size");
            builder.AppendLine(header.TicketSize, "  Ticket size");
            builder.AppendLine(header.TMDFileSize, "  TMD file size");
            builder.AppendLine(header.MetaSize, "  Meta size");
            builder.AppendLine(header.ContentSize, "  Content size");
            builder.AppendLine(header.ContentIndex, "  Content index");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Certificate[] certificateChain)
#else
        private static void Print(StringBuilder builder, Certificate?[]? certificateChain)
#endif
        {
            builder.AppendLine("  Certificate Chain Information:");
            builder.AppendLine("  -------------------------");
            if (certificateChain == null || certificateChain.Length == 0)
            {
                builder.AppendLine("  No certificates, expected 3");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < certificateChain.Length; i++)
            {
                var certificate = certificateChain[i];

                string certificateName = string.Empty;
                switch (i)
                {
                    case 0: certificateName = " (CA)"; break;
                    case 1: certificateName = " (Ticket)"; break;
                    case 2: certificateName = " (TMD)"; break;
                }

                builder.AppendLine($"  Certificate {i}{certificateName}");
                if (certificate == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Signature type: {certificate.SignatureType} (0x{certificate.SignatureType:X})");
                builder.AppendLine(certificate.SignatureSize, "    Signature size");
                builder.AppendLine(certificate.PaddingSize, "    Padding size");
                builder.AppendLine(certificate.Signature, "    Signature");
                builder.AppendLine(certificate.Padding, "    Padding");
                builder.AppendLine(certificate.Issuer, "    Issuer");
                builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                builder.AppendLine(certificate.Name, "    Name");
                builder.AppendLine(certificate.ExpirationTime, "    Expiration time");
                switch (certificate.KeyType)
                {
                    case PublicKeyType.RSA_4096:
                    case PublicKeyType.RSA_2048:
                        builder.AppendLine(certificate.RSAModulus, "    Modulus");
                        builder.AppendLine(certificate.RSAPublicExponent, "    Public exponent");
                        builder.AppendLine(certificate.RSAPadding, "    Padding");
                        break;
                    case PublicKeyType.EllipticCurve:
                        builder.AppendLine(certificate.ECCPublicKey, "    Public key");
                        builder.AppendLine(certificate.ECCPadding, "    Padding");
                        break;
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Ticket ticket)
#else
        private static void Print(StringBuilder builder, Ticket? ticket)
#endif
        {
            builder.AppendLine("  Ticket Information:");
            builder.AppendLine("  -------------------------");
            if (ticket == null)
            {
                builder.AppendLine("  No ticket");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Signature type: {ticket.SignatureType} (0x{ticket.SignatureType:X})");
            builder.AppendLine(ticket.SignatureSize, "  Signature size");
            builder.AppendLine(ticket.PaddingSize, "  Padding size");
            builder.AppendLine(ticket.Signature, "  Signature");
            builder.AppendLine(ticket.Padding, "  Padding");
            builder.AppendLine(ticket.Issuer, "  Issuer");
            builder.AppendLine(ticket.ECCPublicKey, "  ECC public key");
            builder.AppendLine(ticket.Version, "  Version");
            builder.AppendLine(ticket.CaCrlVersion, "  CaCrlVersion");
            builder.AppendLine(ticket.SignerCrlVersion, "  SignerCrlVersion");
            builder.AppendLine(ticket.TitleKey, "  Title key");
            builder.AppendLine(ticket.Reserved1, "  Reserved 1");
            builder.AppendLine(ticket.TicketID, "  Ticket ID");
            builder.AppendLine(ticket.ConsoleID, "  Console ID");
            builder.AppendLine(ticket.TitleID, "  Title ID");
            builder.AppendLine(ticket.Reserved2, "  Reserved 2");
            builder.AppendLine(ticket.TicketTitleVersion, "  Ticket title version");
            builder.AppendLine(ticket.Reserved3, "  Reserved 3");
            builder.AppendLine(ticket.LicenseType, "  License type");
            builder.AppendLine(ticket.CommonKeyYIndex, "  Common key Y index");
            builder.AppendLine(ticket.Reserved4, "  Reserved 4");
            builder.AppendLine(ticket.eShopAccountID, "  eShop Account ID");
            builder.AppendLine(ticket.Reserved5, "  Reserved 5");
            builder.AppendLine(ticket.Audit, "  Audit");
            builder.AppendLine(ticket.Reserved6, "  Reserved 6");
            builder.AppendLine("  Limits:");
            if (ticket.Limits == null || ticket.Limits.Length == 0)
            {
                builder.AppendLine("    No limits");
            }
            else
            {
                for (int i = 0; i < ticket.Limits.Length; i++)
                {
                    builder.AppendLine(ticket.Limits[i], $"    Limit {i}");
                }
            }
            builder.AppendLine(ticket.ContentIndexSize, "    Content index size");
            builder.AppendLine(ticket.ContentIndex, "    Content index");
            builder.AppendLine();

            builder.AppendLine("  Ticket Certificate Chain Information:");
            builder.AppendLine("  -------------------------");
            if (ticket.CertificateChain == null || ticket.CertificateChain.Length == 0)
            {
                builder.AppendLine("  No certificates, expected 2");
            }
            else
            {
                for (int i = 0; i < ticket.CertificateChain.Length; i++)
                {
                    var certificate = ticket.CertificateChain[i];

                    string certificateName = string.Empty;
                    switch (i)
                    {
                        case 0: certificateName = " (Ticket)"; break;
                        case 1: certificateName = " (CA)"; break;
                    }

                    builder.AppendLine($"  Certificate {i}{certificateName}");
                    if (certificate == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    Signature type: {certificate.SignatureType} (0x{certificate.SignatureType:X})");
                    builder.AppendLine(certificate.SignatureSize, "    Signature size");
                    builder.AppendLine(certificate.PaddingSize, "    Padding size");
                    builder.AppendLine(certificate.Signature, "    Signature");
                    builder.AppendLine(certificate.Padding, "    Padding");
                    builder.AppendLine(certificate.Issuer, "    Issuer");
                    builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                    builder.AppendLine(certificate.Name, "    Name");
                    builder.AppendLine(certificate.ExpirationTime, "    Expiration time");
                    switch (certificate.KeyType)
                    {
                        case PublicKeyType.RSA_4096:
                        case PublicKeyType.RSA_2048:
                            builder.AppendLine(certificate.RSAModulus, "    Modulus");
                            builder.AppendLine(certificate.RSAPublicExponent, "    Public exponent");
                            builder.AppendLine(certificate.RSAPadding, "    Padding");
                            break;
                        case PublicKeyType.EllipticCurve:
                            builder.AppendLine(certificate.ECCPublicKey, "    Public key");
                            builder.AppendLine(certificate.ECCPadding, "    Padding");
                            break;
                    }
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, TitleMetadata tmd)
#else
        private static void Print(StringBuilder builder, TitleMetadata? tmd)
#endif
        {
            builder.AppendLine("  Title Metadata Information:");
            builder.AppendLine("  -------------------------");
            if (tmd == null)
            {
                builder.AppendLine("  No title metadata");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Signature type: {tmd.SignatureType} (0x{tmd.SignatureType:X})");
            builder.AppendLine(tmd.SignatureSize, "  Signature size");
            builder.AppendLine(tmd.PaddingSize, "  Padding size");
            builder.AppendLine(tmd.Signature, "  Signature");
            builder.AppendLine(tmd.Padding1, "  Padding 1");
            builder.AppendLine(tmd.Issuer, "  Issuer");
            builder.AppendLine(tmd.Version, "  Version");
            builder.AppendLine(tmd.CaCrlVersion, "  CaCrlVersion");
            builder.AppendLine(tmd.SignerCrlVersion, "  SignerCrlVersion");
            builder.AppendLine(tmd.Reserved1, "  Reserved 1");
            builder.AppendLine(tmd.SystemVersion, "  System version");
            builder.AppendLine(tmd.TitleID, "  Title ID");
            builder.AppendLine(tmd.TitleType, "  Title type");
            builder.AppendLine(tmd.GroupID, "  Group ID");
            builder.AppendLine(tmd.SaveDataSize, "  Save data size");
            builder.AppendLine(tmd.SRLPrivateSaveDataSize, "  SRL private save data size");
            builder.AppendLine(tmd.Reserved2, "  Reserved 2");
            builder.AppendLine(tmd.SRLFlag, "  SRL flag");
            builder.AppendLine(tmd.Reserved3, "  Reserved 3");
            builder.AppendLine(tmd.AccessRights, "  Access rights");
            builder.AppendLine(tmd.TitleVersion, "  Title version");
            builder.AppendLine(tmd.ContentCount, "  Content count");
            builder.AppendLine(tmd.BootContent, "  Boot content");
            builder.AppendLine(tmd.Padding2, "  Padding 2");
            builder.AppendLine(tmd.SHA256HashContentInfoRecords, "  SHA-256 hash of the content info records");
            builder.AppendLine();

            builder.AppendLine("  Ticket Content Info Records Information:");
            builder.AppendLine("  -------------------------");
            if (tmd.ContentInfoRecords == null || tmd.ContentInfoRecords.Length == 0)
            {
                builder.AppendLine("  No content info records, expected 64");
            }
            else
            {
                for (int i = 0; i < tmd.ContentInfoRecords.Length; i++)
                {
                    var contentInfoRecord = tmd.ContentInfoRecords[i];
                    builder.AppendLine($"  Content Info Record {i}");
                    if (contentInfoRecord == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine(contentInfoRecord.ContentIndexOffset, "    Content index offset");
                    builder.AppendLine(contentInfoRecord.ContentCommandCount, "    Content command count");
                    builder.AppendLine(contentInfoRecord.UnhashedContentRecordsSHA256Hash, $"    SHA-256 hash of the next {contentInfoRecord.ContentCommandCount} records not hashed");
                }
            }
            builder.AppendLine();

            builder.AppendLine("  Ticket Content Chunk Records Information:");
            builder.AppendLine("  -------------------------");
            if (tmd.ContentChunkRecords == null || tmd.ContentChunkRecords.Length == 0)
            {
                builder.AppendLine($"  No content chunk records, expected {tmd.ContentCount}");
            }
            else
            {
                for (int i = 0; i < tmd.ContentChunkRecords.Length; i++)
                {
                    var contentChunkRecord = tmd.ContentChunkRecords[i];
                    builder.AppendLine($"  Content Chunk Record {i}");
                    if (contentChunkRecord == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine(contentChunkRecord.ContentId, "    Content ID");
                    builder.AppendLine($"    Content index: {contentChunkRecord.ContentIndex} (0x{contentChunkRecord.ContentIndex:X})");
                    builder.AppendLine($"    Content type: {contentChunkRecord.ContentType} (0x{contentChunkRecord.ContentType:X})");
                    builder.AppendLine(contentChunkRecord.ContentSize, "    Content size");
                    builder.AppendLine(contentChunkRecord.SHA256Hash, "    SHA-256 hash");
                }
            }
            builder.AppendLine();

            builder.AppendLine("  Ticket Certificate Chain Information:");
            builder.AppendLine("  -------------------------");
            if (tmd.CertificateChain == null || tmd.CertificateChain.Length == 0)
            {
                builder.AppendLine("  No certificates, expected 2");
            }
            else
            {
                for (int i = 0; i < tmd.CertificateChain.Length; i++)
                {
                    var certificate = tmd.CertificateChain[i];

                    string certificateName = string.Empty;
                    switch (i)
                    {
                        case 0: certificateName = " (TMD)"; break;
                        case 1: certificateName = " (CA)"; break;
                    }

                    builder.AppendLine($"  Certificate {i}{certificateName}");
                    if (certificate == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    Signature type: {certificate.SignatureType} (0x{certificate.SignatureType:X})");
                    builder.AppendLine(certificate.SignatureSize, "    Signature size");
                    builder.AppendLine(certificate.PaddingSize, "    Padding size");
                    builder.AppendLine(certificate.Signature, "    Signature");
                    builder.AppendLine(certificate.Padding, "    Padding");
                    builder.AppendLine(certificate.Issuer, "    Issuer");
                    builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                    builder.AppendLine(certificate.Name, "    Name");
                    builder.AppendLine(certificate.ExpirationTime, "    Expiration time");
                    switch (certificate.KeyType)
                    {
                        case PublicKeyType.RSA_4096:
                        case PublicKeyType.RSA_2048:
                            builder.AppendLine(certificate.RSAModulus, "    Modulus");
                            builder.AppendLine(certificate.RSAPublicExponent, "    Public exponent");
                            builder.AppendLine(certificate.RSAPadding, "    Padding");
                            break;
                        case PublicKeyType.EllipticCurve:
                            builder.AppendLine(certificate.ECCPublicKey, "    Public key");
                            builder.AppendLine(certificate.ECCPadding, "    Padding");
                            break;
                    }
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, NCCHHeader[] partitions)
#else
        private static void Print(StringBuilder builder, NCCHHeader?[]? partitions)
#endif
        {
            builder.AppendLine("  NCCH Partition Header Information:");
            builder.AppendLine("  -------------------------");
            if (partitions == null || partitions.Length == 0)
            {
                builder.AppendLine("  No NCCH partition headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < partitions.Length; i++)
            {
                var partitionHeader = partitions[i];
                builder.AppendLine($"  NCCH Partition Header {i}");
                if (partitionHeader == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                if (partitionHeader.MagicID == string.Empty)
                {
                    builder.AppendLine("    Empty partition, no data can be parsed");
                    continue;
                }
                else if (partitionHeader.MagicID != Constants.NCCHMagicNumber)
                {
                    builder.AppendLine("    Unrecognized partition data, no data can be parsed");
                    continue;
                }

                builder.AppendLine(partitionHeader.RSA2048Signature, "    RSA-2048 SHA-256 signature");
                builder.AppendLine(partitionHeader.MagicID, "    Magic ID");
                builder.AppendLine(partitionHeader.ContentSizeInMediaUnits, "    Content size in media units");
                builder.AppendLine(partitionHeader.PartitionId, "    Partition ID");
                builder.AppendLine(partitionHeader.MakerCode, "    Maker code");
                builder.AppendLine(partitionHeader.Version, "    Version");
                builder.AppendLine(partitionHeader.VerificationHash, "    Verification hash");
                builder.AppendLine(partitionHeader.ProgramId, "    Program ID");
                builder.AppendLine(partitionHeader.Reserved1, "    Reserved 1");
                builder.AppendLine(partitionHeader.LogoRegionHash, "    Logo region SHA-256 hash");
                builder.AppendLine(partitionHeader.ProductCode, "    Product code");
                builder.AppendLine(partitionHeader.ExtendedHeaderHash, "    Extended header SHA-256 hash");
                builder.AppendLine(partitionHeader.ExtendedHeaderSizeInBytes, "    Extended header size in bytes");
                builder.AppendLine(partitionHeader.Reserved2, "    Reserved 2");
                builder.AppendLine("    Flags:");
                if (partitionHeader.Flags == null)
                {
                    builder.AppendLine("      [NULL]");
                }
                else
                {
                    builder.AppendLine(partitionHeader.Flags.Reserved0, "      Reserved 0");
                    builder.AppendLine(partitionHeader.Flags.Reserved1, "      Reserved 1");
                    builder.AppendLine(partitionHeader.Flags.Reserved2, "      Reserved 2");
                    builder.AppendLine($"      Crypto method: {partitionHeader.Flags.CryptoMethod} (0x{partitionHeader.Flags.CryptoMethod:X})");
                    builder.AppendLine($"      Content platform: {partitionHeader.Flags.ContentPlatform} (0x{partitionHeader.Flags.ContentPlatform:X})");
                    builder.AppendLine($"      Content type: {partitionHeader.Flags.MediaPlatformIndex} (0x{partitionHeader.Flags.MediaPlatformIndex:X})");
                    builder.AppendLine(partitionHeader.Flags.ContentUnitSize, "      Content unit size");
                    builder.AppendLine($"      Bitmasks: {partitionHeader.Flags.BitMasks} (0x{partitionHeader.Flags.BitMasks:X})");
                }
                builder.AppendLine(partitionHeader.PlainRegionOffsetInMediaUnits, "    Plain region offset, in media units");
                builder.AppendLine(partitionHeader.PlainRegionSizeInMediaUnits, "    Plain region size, in media units");
                builder.AppendLine(partitionHeader.LogoRegionOffsetInMediaUnits, "    Logo region offset, in media units");
                builder.AppendLine(partitionHeader.LogoRegionSizeInMediaUnits, "    Logo region size, in media units");
                builder.AppendLine(partitionHeader.ExeFSOffsetInMediaUnits, "    ExeFS offset, in media units");
                builder.AppendLine(partitionHeader.ExeFSSizeInMediaUnits, "    ExeFS size, in media units");
                builder.AppendLine(partitionHeader.ExeFSHashRegionSizeInMediaUnits, "    ExeFS hash region offset, in media units");
                builder.AppendLine(partitionHeader.Reserved3, "     Reserved 3");
                builder.AppendLine(partitionHeader.RomFSOffsetInMediaUnits, "    RomFS offset, in media units");
                builder.AppendLine(partitionHeader.RomFSSizeInMediaUnits, "    RomFS size, in media units");
                builder.AppendLine(partitionHeader.RomFSHashRegionSizeInMediaUnits, "    RomFS hash region offset, in media units");
                builder.AppendLine(partitionHeader.Reserved4, "     Reserved 4");
                builder.AppendLine(partitionHeader.ExeFSSuperblockHash, "     ExeFS superblock SHA-256 hash");
                builder.AppendLine(partitionHeader.RomFSSuperblockHash, "     RomFS superblock SHA-256 hash");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, MetaData metaData)
#else
        private static void Print(StringBuilder builder, MetaData? metaData)
#endif
        {
            builder.AppendLine("  Meta Data Information:");
            builder.AppendLine("  -------------------------");
            if (metaData == null)
            {
                builder.AppendLine("  No meta file data");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(metaData.TitleIDDependencyList, "  Title ID dependency list");
            builder.AppendLine(metaData.Reserved1, "  Reserved 1");
            builder.AppendLine(metaData.CoreVersion, "  Core version");
            builder.AppendLine(metaData.Reserved2, "  Reserved 2");
            builder.AppendLine(metaData.IconData, "  Icon data");
            builder.AppendLine();
        }

    }
}