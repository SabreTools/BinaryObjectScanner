using System;
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

            builder.AppendLine($"  Header size: {header.HeaderSize} (0x{header.HeaderSize:X})");
            builder.AppendLine($"  Type: {header.Type} (0x{header.Type:X})");
            builder.AppendLine($"  Version: {header.Version} (0x{header.Version:X})");
            builder.AppendLine($"  Certificate chain size: {header.CertificateChainSize} (0x{header.CertificateChainSize:X})");
            builder.AppendLine($"  Ticket size: {header.TicketSize} (0x{header.TicketSize:X})");
            builder.AppendLine($"  TMD file size: {header.TMDFileSize} (0x{header.TMDFileSize:X})");
            builder.AppendLine($"  Meta size: {header.MetaSize} (0x{header.MetaSize:X})");
            builder.AppendLine($"  Content size: {header.ContentSize} (0x{header.ContentSize:X})");
            builder.AppendLine($"  Content index: {(header.ContentIndex == null ? "[NULL]" : BitConverter.ToString(header.ContentIndex).Replace('-', ' '))}");
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
                builder.AppendLine($"    Signature size: {certificate.SignatureSize} (0x{certificate.SignatureSize:X})");
                builder.AppendLine($"    Padding size: {certificate.PaddingSize} (0x{certificate.PaddingSize:X})");
                builder.AppendLine($"    Signature: {(certificate.Signature == null ? "[NULL]" : BitConverter.ToString(certificate.Signature).Replace('-', ' '))}");
                builder.AppendLine($"    Padding: {(certificate.Padding == null ? "[NULL]" : BitConverter.ToString(certificate.Padding).Replace('-', ' '))}");
                builder.AppendLine($"    Issuer: {certificate.Issuer ?? "[NULL]"}");
                builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                builder.AppendLine($"    Name: {certificate.Name ?? "[NULL]"}");
                builder.AppendLine($"    Expiration time: {certificate.ExpirationTime} (0x{certificate.ExpirationTime:X})");
                switch (certificate.KeyType)
                {
                    case PublicKeyType.RSA_4096:
                    case PublicKeyType.RSA_2048:
                        builder.AppendLine($"    Modulus: {(certificate.RSAModulus == null ? "[NULL]" : BitConverter.ToString(certificate.RSAModulus).Replace('-', ' '))}");
                        builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                        builder.AppendLine($"    Padding: {(certificate.RSAPadding == null ? "[NULL]" : BitConverter.ToString(certificate.RSAPadding).Replace('-', ' '))}");
                        break;
                    case PublicKeyType.EllipticCurve:
                        builder.AppendLine($"    Public key: {(certificate.ECCPublicKey == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' '))}");
                        builder.AppendLine($"    Padding: {(certificate.ECCPadding == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPadding).Replace('-', ' '))}");
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
            builder.AppendLine($"  Signature size: {ticket.SignatureSize} (0x{ticket.SignatureSize:X})");
            builder.AppendLine($"  Padding size: {ticket.PaddingSize} (0x{ticket.PaddingSize:X})");
            builder.AppendLine($"  Signature: {(ticket.Signature == null ? "[NULL]" : BitConverter.ToString(ticket.Signature).Replace('-', ' '))}");
            builder.AppendLine($"  Padding: {(ticket.Padding == null ? "[NULL]" : BitConverter.ToString(ticket.Padding).Replace('-', ' '))}");
            builder.AppendLine($"  Issuer: {ticket.Issuer ?? "[NULL]"}");
            builder.AppendLine($"  ECC public key: {(ticket.ECCPublicKey == null ? "[NULL]" : BitConverter.ToString(ticket.ECCPublicKey).Replace('-', ' '))}");
            builder.AppendLine($"  Version: {ticket.Version} (0x{ticket.Version:X})");
            builder.AppendLine($"  CaCrlVersion: {ticket.CaCrlVersion} (0x{ticket.CaCrlVersion:X})");
            builder.AppendLine($"  SignerCrlVersion: {ticket.SignerCrlVersion} (0x{ticket.SignerCrlVersion:X})");
            builder.AppendLine($"  Title key: {(ticket.TitleKey == null ? "[NULL]" : BitConverter.ToString(ticket.TitleKey).Replace('-', ' '))}");
            builder.AppendLine($"  Reserved 1: {ticket.Reserved1} (0x{ticket.Reserved1:X})");
            builder.AppendLine($"  Ticket ID: {ticket.TicketID} (0x{ticket.TicketID:X})");
            builder.AppendLine($"  Console ID: {ticket.ConsoleID} (0x{ticket.ConsoleID:X})");
            builder.AppendLine($"  Title ID {ticket.TitleID} (0x{ticket.TitleID:X})");
            builder.AppendLine($"  Reserved 2: {(ticket.Reserved2 == null ? "[NULL]" : BitConverter.ToString(ticket.Reserved2).Replace('-', ' '))}");
            builder.AppendLine($"  Ticket title version: {ticket.TicketTitleVersion} (0x{ticket.TicketTitleVersion:X})");
            builder.AppendLine($"  Reserved 3: {(ticket.Reserved3 == null ? "[NULL]" : BitConverter.ToString(ticket.Reserved3).Replace('-', ' '))}");
            builder.AppendLine($"  License type: {ticket.LicenseType} (0x{ticket.LicenseType:X})");
            builder.AppendLine($"  Common keY index: {ticket.CommonKeyYIndex} (0x{ticket.CommonKeyYIndex:X})");
            builder.AppendLine($"  Reserved 4: {(ticket.Reserved4 == null ? "[NULL]" : BitConverter.ToString(ticket.Reserved4).Replace('-', ' '))}");
            builder.AppendLine($"  eShop Account ID?: {ticket.eShopAccountID} (0x{ticket.eShopAccountID:X})");
            builder.AppendLine($"  Reserved 5: {ticket.Reserved5} (0x{ticket.Reserved5:X})");
            builder.AppendLine($"  Audit: {ticket.Audit} (0x{ticket.Audit:X})");
            builder.AppendLine($"  Reserved 6: {(ticket.Reserved6 == null ? "[NULL]" : BitConverter.ToString(ticket.Reserved6).Replace('-', ' '))}");
            builder.AppendLine($"  Limits:");
            if (ticket.Limits == null || ticket.Limits.Length == 0)
            {
                builder.AppendLine("    No limits");
            }
            else
            {
                for (int i = 0; i < ticket.Limits.Length; i++)
                {
                    builder.AppendLine($"    Limit {i}: {ticket.Limits[i]} (0x{ticket.Limits[i]:X})");
                }
            }
            builder.AppendLine($"  Content index size: {ticket.ContentIndexSize} (0x{ticket.ContentIndexSize:X})");
            builder.AppendLine($"  Content index: {(ticket.ContentIndex == null ? "[NULL]" : BitConverter.ToString(ticket.ContentIndex).Replace('-', ' '))}");
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
                    builder.AppendLine($"    Signature size: {certificate.SignatureSize} (0x{certificate.SignatureSize:X})");
                    builder.AppendLine($"    Padding size: {certificate.PaddingSize} (0x{certificate.PaddingSize:X})");
                    builder.AppendLine($"    Signature: {(certificate.Signature == null ? "[NULL]" : BitConverter.ToString(certificate.Signature).Replace('-', ' '))}");
                    builder.AppendLine($"    Padding: {(certificate.Padding == null ? "[NULL]" : BitConverter.ToString(certificate.Padding).Replace('-', ' '))}");
                    builder.AppendLine($"    Issuer: {certificate.Issuer ?? "[NULL]"}");
                    builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                    builder.AppendLine($"    Name: {certificate.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Expiration time: {certificate.ExpirationTime} (0x{certificate.ExpirationTime:X})");
                    switch (certificate.KeyType)
                    {
                        case PublicKeyType.RSA_4096:
                        case PublicKeyType.RSA_2048:
                            builder.AppendLine($"    Modulus: {(certificate.RSAModulus == null ? "[NULL]" : BitConverter.ToString(certificate.RSAModulus).Replace('-', ' '))}");
                            builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                            builder.AppendLine($"    Padding: {(certificate.RSAPadding == null ? "[NULL]" : BitConverter.ToString(certificate.RSAPadding).Replace('-', ' '))}");
                            break;
                        case PublicKeyType.EllipticCurve:
                            builder.AppendLine($"    Public key: {(certificate.ECCPublicKey == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' '))}");
                            builder.AppendLine($"    Padding: {(certificate.ECCPadding == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPadding).Replace('-', ' '))}");
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
            builder.AppendLine($"  Signature size: {tmd.SignatureSize} (0x{tmd.SignatureSize:X})");
            builder.AppendLine($"  Padding size: {tmd.PaddingSize} (0x{tmd.PaddingSize:X})");
            builder.AppendLine($"  Signature: {(tmd.Signature == null ? "[NULL]" : BitConverter.ToString(tmd.Signature).Replace('-', ' '))}");
            builder.AppendLine($"  Padding 1: {(tmd.Padding1 == null ? "[NULL]" : BitConverter.ToString(tmd.Padding1).Replace('-', ' '))}");
            builder.AppendLine($"  Issuer: {tmd.Issuer ?? "[NULL]"}");
            builder.AppendLine($"  Version: {tmd.Version} (0x{tmd.Version:X})");
            builder.AppendLine($"  CaCrlVersion: {tmd.CaCrlVersion} (0x{tmd.CaCrlVersion:X})");
            builder.AppendLine($"  SignerCrlVersion: {tmd.SignerCrlVersion} (0x{tmd.SignerCrlVersion:X})");
            builder.AppendLine($"  Reserved 1: {tmd.Reserved1} (0x{tmd.Reserved1:X})");
            builder.AppendLine($"  System version: {tmd.SystemVersion} (0x{tmd.SystemVersion:X})");
            builder.AppendLine($"  Title ID: {tmd.TitleID} (0x{tmd.TitleID:X})");
            builder.AppendLine($"  Title type: {tmd.TitleType} (0x{tmd.TitleType:X})");
            builder.AppendLine($"  Group ID: {tmd.GroupID} (0x{tmd.GroupID:X})");
            builder.AppendLine($"  Save data size: {tmd.SaveDataSize} (0x{tmd.SaveDataSize:X})");
            builder.AppendLine($"  SRL private save data size: {tmd.SRLPrivateSaveDataSize} (0x{tmd.SRLPrivateSaveDataSize:X})");
            builder.AppendLine($"  Reserved 2: {(tmd.Reserved2 == null ? "[NULL]" : BitConverter.ToString(tmd.Reserved2).Replace('-', ' '))}");
            builder.AppendLine($"  SRL flag: {tmd.SRLFlag} (0x{tmd.SRLFlag:X})");
            builder.AppendLine($"  Reserved 3: {(tmd.Reserved3 == null ? "[NULL]" : BitConverter.ToString(tmd.Reserved3).Replace('-', ' '))}");
            builder.AppendLine($"  Access rights: {tmd.AccessRights} (0x{tmd.AccessRights:X})");
            builder.AppendLine($"  Title version: {tmd.TitleVersion} (0x{tmd.TitleVersion:X})");
            builder.AppendLine($"  Content count: {tmd.ContentCount} (0x{tmd.ContentCount:X})");
            builder.AppendLine($"  Boot content: {tmd.BootContent} (0x{tmd.BootContent:X})");
            builder.AppendLine($"  Padding 2: {(tmd.Padding2 == null ? "[NULL]" : BitConverter.ToString(tmd.Padding2).Replace('-', ' '))}");
            builder.AppendLine($"  SHA-256 hash of the content info records: {(tmd.SHA256HashContentInfoRecords == null ? "[NULL]" : BitConverter.ToString(tmd.SHA256HashContentInfoRecords).Replace('-', ' '))}");
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

                    builder.AppendLine($"    Content index offset: {contentInfoRecord.ContentIndexOffset} (0x{contentInfoRecord.ContentIndexOffset:X})");
                    builder.AppendLine($"    Content command count: {contentInfoRecord.ContentCommandCount} (0x{contentInfoRecord.ContentCommandCount:X})");
                    builder.AppendLine($"    SHA-256 hash of the next {contentInfoRecord.ContentCommandCount} records not hashed: {(contentInfoRecord.UnhashedContentRecordsSHA256Hash == null ? "[NULL]" : BitConverter.ToString(contentInfoRecord.UnhashedContentRecordsSHA256Hash).Replace('-', ' '))}");
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

                    builder.AppendLine($"    Content ID: {contentChunkRecord.ContentId} (0x{contentChunkRecord.ContentId:X})");
                    builder.AppendLine($"    Content index: {contentChunkRecord.ContentIndex} (0x{contentChunkRecord.ContentIndex:X})");
                    builder.AppendLine($"    Content type: {contentChunkRecord.ContentType} (0x{contentChunkRecord.ContentType:X})");
                    builder.AppendLine($"    Content size: {contentChunkRecord.ContentSize} (0x{contentChunkRecord.ContentSize:X})");
                    builder.AppendLine($"    SHA-256 hash: {(contentChunkRecord.SHA256Hash == null ? "[NULL]" : BitConverter.ToString(contentChunkRecord.SHA256Hash).Replace('-', ' '))}");
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
                    builder.AppendLine($"    Signature size: {certificate.SignatureSize} (0x{certificate.SignatureSize:X})");
                    builder.AppendLine($"    Padding size: {certificate.PaddingSize} (0x{certificate.PaddingSize:X})");
                    builder.AppendLine($"    Signature: {(certificate.Signature == null ? "[NULL]" : BitConverter.ToString(certificate.Signature).Replace('-', ' '))}");
                    builder.AppendLine($"    Padding: {(certificate.Padding == null ? "[NULL]" : BitConverter.ToString(certificate.Padding).Replace('-', ' '))}");
                    builder.AppendLine($"    Issuer: {certificate.Issuer ?? "[NULL]"}");
                    builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                    builder.AppendLine($"    Name: {certificate.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Expiration time: {certificate.ExpirationTime} (0x{certificate.ExpirationTime:X})");
                    switch (certificate.KeyType)
                    {
                        case PublicKeyType.RSA_4096:
                        case PublicKeyType.RSA_2048:
                            builder.AppendLine($"    Modulus: {(certificate.RSAModulus == null ? "[NULL]" : BitConverter.ToString(certificate.RSAModulus).Replace('-', ' '))}");
                            builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                            builder.AppendLine($"    Padding: {(certificate.RSAPadding == null ? "[NULL]" : BitConverter.ToString(certificate.RSAPadding).Replace('-', ' '))}");
                            break;
                        case PublicKeyType.EllipticCurve:
                            builder.AppendLine($"    Public key: {(certificate.ECCPublicKey == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' '))}");
                            builder.AppendLine($"    Padding: {(certificate.ECCPadding == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPadding).Replace('-', ' '))}");
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
                    builder.AppendLine($"    Empty partition, no data can be parsed");
                    continue;
                }
                else if (partitionHeader.MagicID != Constants.NCCHMagicNumber)
                {
                    builder.AppendLine($"    Unrecognized partition data, no data can be parsed");
                    continue;
                }

                builder.AppendLine($"    RSA-2048 SHA-256 signature: {(partitionHeader.RSA2048Signature == null ? "[NULL]" : BitConverter.ToString(partitionHeader.RSA2048Signature).Replace('-', ' '))}");
                builder.AppendLine($"    Magic ID: {partitionHeader.MagicID} (0x{partitionHeader.MagicID:X})");
                builder.AppendLine($"    Content size in media units: {partitionHeader.ContentSizeInMediaUnits} (0x{partitionHeader.ContentSizeInMediaUnits:X})");
                builder.AppendLine($"    Partition ID: {partitionHeader.PartitionId} (0x{partitionHeader.PartitionId:X})");
                builder.AppendLine($"    Maker code: {partitionHeader.MakerCode} (0x{partitionHeader.MakerCode:X})");
                builder.AppendLine($"    Version: {partitionHeader.Version} (0x{partitionHeader.Version:X})");
                builder.AppendLine($"    Verification hash: {partitionHeader.VerificationHash} (0x{partitionHeader.VerificationHash:X})");
                builder.AppendLine($"    Program ID: {(partitionHeader.ProgramId == null ? "[NULL]" : BitConverter.ToString(partitionHeader.ProgramId).Replace('-', ' '))}");
                builder.AppendLine($"    Reserved 1: {(partitionHeader.Reserved1 == null ? "[NULL]" : BitConverter.ToString(partitionHeader.Reserved1).Replace('-', ' '))}");
                builder.AppendLine($"    Logo region SHA-256 hash: {(partitionHeader.LogoRegionHash == null ? "[NULL]" : BitConverter.ToString(partitionHeader.LogoRegionHash).Replace('-', ' '))}");
                builder.AppendLine($"    Product code: {partitionHeader.ProductCode} (0x{partitionHeader.ProductCode:X})");
                builder.AppendLine($"    Extended header SHA-256 hash: {(partitionHeader.ExtendedHeaderHash == null ? "[NULL]" : BitConverter.ToString(partitionHeader.ExtendedHeaderHash).Replace('-', ' '))}");
                builder.AppendLine($"    Extended header size in bytes: {partitionHeader.ExtendedHeaderSizeInBytes} (0x{partitionHeader.ExtendedHeaderSizeInBytes:X})");
                builder.AppendLine($"    Reserved 2: {(partitionHeader.Reserved2 == null ? "[NULL]" : BitConverter.ToString(partitionHeader.Reserved2).Replace('-', ' '))}");
                builder.AppendLine("    Flags:");
                if (partitionHeader.Flags == null)
                {
                    builder.AppendLine("      [NULL]");
                }
                else
                {
                    builder.AppendLine($"      Reserved 0: {partitionHeader.Flags.Reserved0} (0x{partitionHeader.Flags.Reserved0:X})");
                    builder.AppendLine($"      Reserved 1: {partitionHeader.Flags.Reserved1} (0x{partitionHeader.Flags.Reserved1:X})");
                    builder.AppendLine($"      Reserved 2: {partitionHeader.Flags.Reserved2} (0x{partitionHeader.Flags.Reserved2:X})");
                    builder.AppendLine($"      Crypto method: {partitionHeader.Flags.CryptoMethod} (0x{partitionHeader.Flags.CryptoMethod:X})");
                    builder.AppendLine($"      Content platform: {partitionHeader.Flags.ContentPlatform} (0x{partitionHeader.Flags.ContentPlatform:X})");
                    builder.AppendLine($"      Content type: {partitionHeader.Flags.MediaPlatformIndex} (0x{partitionHeader.Flags.MediaPlatformIndex:X})");
                    builder.AppendLine($"      Content unit size: {partitionHeader.Flags.ContentUnitSize} (0x{partitionHeader.Flags.ContentUnitSize:X})");
                    builder.AppendLine($"      Bitmasks: {partitionHeader.Flags.BitMasks} (0x{partitionHeader.Flags.BitMasks:X})");
                }
                builder.AppendLine($"    Plain region offset, in media units: {partitionHeader.PlainRegionOffsetInMediaUnits} (0x{partitionHeader.PlainRegionOffsetInMediaUnits:X})");
                builder.AppendLine($"    Plain region size, in media units: {partitionHeader.PlainRegionSizeInMediaUnits} (0x{partitionHeader.PlainRegionSizeInMediaUnits:X})");
                builder.AppendLine($"    Logo region offset, in media units: {partitionHeader.LogoRegionOffsetInMediaUnits} (0x{partitionHeader.LogoRegionOffsetInMediaUnits:X})");
                builder.AppendLine($"    Logo region size, in media units: {partitionHeader.LogoRegionSizeInMediaUnits} (0x{partitionHeader.LogoRegionSizeInMediaUnits:X})");
                builder.AppendLine($"    ExeFS offset, in media units: {partitionHeader.ExeFSOffsetInMediaUnits} (0x{partitionHeader.ExeFSOffsetInMediaUnits:X})");
                builder.AppendLine($"    ExeFS size, in media units: {partitionHeader.ExeFSSizeInMediaUnits} (0x{partitionHeader.ExeFSSizeInMediaUnits:X})");
                builder.AppendLine($"    ExeFS hash region size, in media units: {partitionHeader.ExeFSHashRegionSizeInMediaUnits} (0x{partitionHeader.ExeFSHashRegionSizeInMediaUnits:X})");
                builder.AppendLine($"    Reserved 3: {(partitionHeader.Reserved3 == null ? "[NULL]" : BitConverter.ToString(partitionHeader.Reserved3).Replace('-', ' '))}");
                builder.AppendLine($"    RomFS offset, in media units: {partitionHeader.RomFSOffsetInMediaUnits} (0x{partitionHeader.RomFSOffsetInMediaUnits:X})");
                builder.AppendLine($"    RomFS size, in media units: {partitionHeader.RomFSSizeInMediaUnits} (0x{partitionHeader.RomFSSizeInMediaUnits:X})");
                builder.AppendLine($"    RomFS hash region size, in media units: {partitionHeader.RomFSHashRegionSizeInMediaUnits} (0x{partitionHeader.RomFSHashRegionSizeInMediaUnits:X})");
                builder.AppendLine($"    Reserved 4: {(partitionHeader.Reserved4 == null ? "[NULL]" : BitConverter.ToString(partitionHeader.Reserved4).Replace('-', ' '))}");
                builder.AppendLine($"    ExeFS superblock SHA-256 hash: {(partitionHeader.ExeFSSuperblockHash == null ? "[NULL]" : BitConverter.ToString(partitionHeader.ExeFSSuperblockHash).Replace('-', ' '))}");
                builder.AppendLine($"    RomFS superblock SHA-256 hash: {(partitionHeader.RomFSSuperblockHash == null ? "[NULL]" : BitConverter.ToString(partitionHeader.RomFSSuperblockHash).Replace('-', ' '))}");
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

            builder.AppendLine($"  Title ID dependency list: {(metaData.TitleIDDependencyList == null ? "[NULL]" : BitConverter.ToString(metaData.TitleIDDependencyList).Replace('-', ' '))}");
            builder.AppendLine($"  Reserved 1: {(metaData.Reserved1 == null ? "[NULL]" : BitConverter.ToString(metaData.Reserved1).Replace('-', ' '))}");
            builder.AppendLine($"  Core version: {metaData.CoreVersion} (0x{metaData.CoreVersion:X})");
            builder.AppendLine($"  Reserved 2: {(metaData.Reserved2 == null ? "[NULL]" : BitConverter.ToString(metaData.Reserved2).Replace('-', ' '))}");
            builder.AppendLine($"  Icon data: {(metaData.IconData == null ? "[NULL]" : BitConverter.ToString(metaData.IconData).Replace('-', ' '))}");
            builder.AppendLine();
        }

    }
}