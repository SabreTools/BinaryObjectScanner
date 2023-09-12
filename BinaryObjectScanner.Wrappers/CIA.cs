using System;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class CIA : WrapperBase<SabreTools.Models.N3DS.CIA>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "CTR Importable Archive (CIA)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.N3DS.CIAHeader.HeaderSize"/>
        public uint HeaderSize => _model.Header.HeaderSize;

        /// <inheritdoc cref="Models.N3DS.CIAHeader.Type"/>
        public ushort Type => _model.Header.Type;

        /// <inheritdoc cref="Models.N3DS.CIAHeader.Version"/>
        public ushort Version => _model.Header.Version;

        /// <inheritdoc cref="Models.N3DS.CIAHeader.CertificateChainSize"/>
        public uint CertificateChainSize => _model.Header.CertificateChainSize;

        /// <inheritdoc cref="Models.N3DS.CIAHeader.TicketSize"/>
        public uint TicketSize => _model.Header.TicketSize;

        /// <inheritdoc cref="Models.N3DS.CIAHeader.TMDFileSize"/>
        public uint TMDFileSize => _model.Header.TMDFileSize;

        /// <inheritdoc cref="Models.N3DS.CIAHeader.MetaSize"/>
        public uint MetaSize => _model.Header.MetaSize;

        /// <inheritdoc cref="Models.N3DS.CIAHeader.ContentSize"/>
        public ulong ContentSize => _model.Header.ContentSize;

        /// <inheritdoc cref="Models.N3DS.CIAHeader.ContentIndex"/>
        public byte[] ContentIndex => _model.Header.ContentIndex;

        #endregion

        #region Certificate Chain

        /// <inheritdoc cref="Models.N3DS.CIA.CertificateChain"/>
#if NET48
        public SabreTools.Models.N3DS.Certificate[] CertificateChain => _model.CertificateChain;
#else
        public SabreTools.Models.N3DS.Certificate?[]? CertificateChain => _model.CertificateChain;
#endif

        #endregion

        #region Ticket

        /// <inheritdoc cref="Models.N3DS.Ticket.SignatureType"/>
        public SabreTools.Models.N3DS.SignatureType T_SignatureType => _model.Ticket.SignatureType;

        /// <inheritdoc cref="Models.N3DS.Ticket.SignatureSize"/>
        public ushort T_SignatureSize => _model.Ticket.SignatureSize;

        /// <inheritdoc cref="Models.N3DS.Ticket.PaddingSize"/>
        public byte T_PaddingSize => _model.Ticket.PaddingSize;

        /// <inheritdoc cref="Models.N3DS.Ticket.Signature"/>
        public byte[] T_Signature => _model.Ticket.Signature;

        /// <inheritdoc cref="Models.N3DS.Ticket.Padding"/>
        public byte[] T_Padding => _model.Ticket.Padding;

        /// <inheritdoc cref="Models.N3DS.Ticket.Issuer"/>
        public string T_Issuer => _model.Ticket.Issuer;

        /// <inheritdoc cref="Models.N3DS.Ticket.ECCPublicKey"/>
        public byte[] T_ECCPublicKey => _model.Ticket.ECCPublicKey;

        /// <inheritdoc cref="Models.N3DS.Ticket.Version"/>
        public byte T_Version => _model.Ticket.Version;

        /// <inheritdoc cref="Models.N3DS.Ticket.CaCrlVersion"/>
        public byte T_CaCrlVersion => _model.Ticket.CaCrlVersion;

        /// <inheritdoc cref="Models.N3DS.Ticket.SignerCrlVersion"/>
        public byte T_SignerCrlVersion => _model.Ticket.SignerCrlVersion;

        /// <inheritdoc cref="Models.N3DS.Ticket.TitleKey"/>
        public byte[] T_TitleKey => _model.Ticket.TitleKey;

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved1"/>
        public byte T_Reserved1 => _model.Ticket.Reserved1;

        /// <inheritdoc cref="Models.N3DS.Ticket.TicketID"/>
        public ulong T_TicketID => _model.Ticket.TicketID;

        /// <inheritdoc cref="Models.N3DS.Ticket.ConsoleID"/>
        public uint T_ConsoleID => _model.Ticket.ConsoleID;

        /// <inheritdoc cref="Models.N3DS.Ticket.TitleID"/>
        public ulong T_TitleID => _model.Ticket.TitleID;

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved2"/>
        public byte[] T_Reserved2 => _model.Ticket.Reserved2;

        /// <inheritdoc cref="Models.N3DS.Ticket.TicketTitleVersion"/>
        public ushort T_TicketTitleVersion => _model.Ticket.TicketTitleVersion;

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved3"/>
        public byte[] T_Reserved3 => _model.Ticket.Reserved3;

        /// <inheritdoc cref="Models.N3DS.Ticket.LicenseType"/>
        public byte T_LicenseType => _model.Ticket.LicenseType;

        /// <inheritdoc cref="Models.N3DS.Ticket.CommonKeyYIndex"/>
        public byte T_CommonKeyYIndex => _model.Ticket.CommonKeyYIndex;

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved4"/>
        public byte[] T_Reserved4 => _model.Ticket.Reserved4;

        /// <inheritdoc cref="Models.N3DS.Ticket.eShopAccountID"/>
        public uint T_eShopAccountID => _model.Ticket.eShopAccountID;

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved5"/>
        public byte T_Reserved5 => _model.Ticket.Reserved5;

        /// <inheritdoc cref="Models.N3DS.Ticket.Audit"/>
        public byte T_Audit => _model.Ticket.Audit;

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved6"/>
        public byte[] T_Reserved6 => _model.Ticket.Reserved6;

        /// <inheritdoc cref="Models.N3DS.Ticket.Limits"/>
        public uint[] T_Limits => _model.Ticket.Limits;

        /// <inheritdoc cref="Models.N3DS.Ticket.ContentIndexSize"/>
        public uint T_ContentIndexSize => _model.Ticket.ContentIndexSize;

        /// <inheritdoc cref="Models.N3DS.Ticket.ContentIndex"/>
        public byte[] T_ContentIndex => _model.Ticket.ContentIndex;

        /// <inheritdoc cref="Models.N3DS.Ticket.CertificateChain"/>
#if NET48
        public SabreTools.Models.N3DS.Certificate[] T_CertificateChain => _model.Ticket.CertificateChain;
#else
        public SabreTools.Models.N3DS.Certificate?[]? T_CertificateChain => _model.Ticket.CertificateChain;
#endif

        #endregion

        #region Title Metadata

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignatureType"/>
        public SabreTools.Models.N3DS.SignatureType TMD_SignatureType => _model.TMDFileData.SignatureType;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignatureSize"/>
        public ushort TMD_SignatureSize => _model.TMDFileData.SignatureSize;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.PaddingSize"/>
        public byte TMD_PaddingSize => _model.TMDFileData.PaddingSize;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Signature"/>
        public byte[] TMD_Signature => _model.TMDFileData.Signature;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Padding1"/>
        public byte[] TMD_Padding1 => _model.TMDFileData.Padding1;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Issuer"/>
        public string TMD_Issuer => _model.TMDFileData.Issuer;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Version"/>
        public byte TMD_Version => _model.TMDFileData.Version;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.CaCrlVersion"/>
        public byte TMD_CaCrlVersion => _model.TMDFileData.CaCrlVersion;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignerCrlVersion"/>
        public byte TMD_SignerCrlVersion => _model.TMDFileData.SignerCrlVersion;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved1"/>
        public byte TMD_Reserved1 => _model.TMDFileData.Reserved1;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SystemVersion"/>
        public ulong TMD_SystemVersion => _model.TMDFileData.SystemVersion;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleID"/>
        public ulong TMD_TitleID => _model.TMDFileData.TitleID;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleType"/>
        public uint TMD_TitleType => _model.TMDFileData.TitleType;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.GroupID"/>
        public ushort TMD_GroupID => _model.TMDFileData.GroupID;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SaveDataSize"/>
        public uint TMD_SaveDataSize => _model.TMDFileData.SaveDataSize;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SRLPrivateSaveDataSize"/>
        public uint TMD_SRLPrivateSaveDataSize => _model.TMDFileData.SRLPrivateSaveDataSize;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved2"/>
        public byte[] TMD_Reserved2 => _model.TMDFileData.Reserved2;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SRLFlag"/>
        public byte TMD_SRLFlag => _model.TMDFileData.SRLFlag;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved3"/>
        public byte[] TMD_Reserved3 => _model.TMDFileData.Reserved3;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.AccessRights"/>
        public uint TMD_AccessRights => _model.TMDFileData.AccessRights;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleVersion"/>
        public ushort TMD_TitleVersion => _model.TMDFileData.TitleVersion;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentCount"/>
        public ushort TMD_ContentCount => _model.TMDFileData.ContentCount;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.BootContent"/>
        public ushort TMD_BootContent => _model.TMDFileData.BootContent;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Padding2"/>
        public byte[] TMD_Padding2 => _model.TMDFileData.Padding2;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SHA256HashContentInfoRecords"/>
        public byte[] TMD_SHA256HashContentInfoRecords => _model.TMDFileData.SHA256HashContentInfoRecords;

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentInfoRecords"/>
#if NET48
        public SabreTools.Models.N3DS.ContentInfoRecord[] TMD_ContentInfoRecords => _model.TMDFileData.ContentInfoRecords;
#else
        public SabreTools.Models.N3DS.ContentInfoRecord?[]? TMD_ContentInfoRecords => _model.TMDFileData.ContentInfoRecords;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentChunkRecords"/>
#if NET48
        public SabreTools.Models.N3DS.ContentChunkRecord[] TMD_ContentChunkRecords => _model.TMDFileData.ContentChunkRecords;
#else
        public SabreTools.Models.N3DS.ContentChunkRecord?[]? TMD_ContentChunkRecords => _model.TMDFileData.ContentChunkRecords;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.CertificateChain"/>
#if NET48
        public SabreTools.Models.N3DS.Certificate[] TMD_CertificateChain => _model.TMDFileData.CertificateChain;
#else
        public SabreTools.Models.N3DS.Certificate?[]? TMD_CertificateChain => _model.TMDFileData.CertificateChain;
#endif

        #endregion

        #region Partitions

        /// <inheritdoc cref="Models.N3DS.CIA.Partitions"/>
#if NET48
        public SabreTools.Models.N3DS.NCCHHeader[] Partitions => _model.Partitions;
#else
        public SabreTools.Models.N3DS.NCCHHeader?[]? Partitions => _model.Partitions;
#endif

        #endregion

        #region Meta Data

        /// <inheritdoc cref="Models.N3DS.MetaData.TitleIDDependencyList"/>
        public byte[] MD_TitleIDDependencyList => _model.MetaData?.TitleIDDependencyList;

        /// <inheritdoc cref="Models.N3DS.MetaData.Reserved1"/>
        public byte[] MD_Reserved1 => _model.MetaData?.Reserved1;

        /// <inheritdoc cref="Models.N3DS.MetaData.CoreVersion"/>
        public uint? MD_CoreVersion => _model.MetaData?.CoreVersion;

        /// <inheritdoc cref="Models.N3DS.MetaData.Reserved2"/>
        public byte[] MD_Reserved2 => _model.MetaData?.Reserved2;

        /// <inheritdoc cref="Models.N3DS.MetaData.IconData"/>
        public byte[] MD_IconData => _model.MetaData?.IconData;

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public CIA(SabreTools.Models.N3DS.CIA model, byte[] data, int offset)
#else
        public CIA(SabreTools.Models.N3DS.CIA? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public CIA(SabreTools.Models.N3DS.CIA model, Stream data)
#else
        public CIA(SabreTools.Models.N3DS.CIA? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a CIA archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A CIA archive wrapper on success, null on failure</returns>
        public static CIA Create(byte[] data, int offset)
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
        /// Create a CIA archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A CIA archive wrapper on success, null on failure</returns>
        public static CIA Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.CIA().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new CIA(archive, data);
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

            builder.AppendLine("CIA Archive Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintCertificateChain(builder);
            PrintTicket(builder);
            PrintTitleMetadata(builder);
            PrintPartitions(builder);
            PrintMetaData(builder);

            return builder;
        }

        /// <summary>
        /// Print CIA header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintHeader(StringBuilder builder)
        {
            builder.AppendLine("  CIA Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Header size: {HeaderSize} (0x{HeaderSize:X})");
            builder.AppendLine($"  Type: {Type} (0x{Type:X})");
            builder.AppendLine($"  Version: {Version} (0x{Version:X})");
            builder.AppendLine($"  Certificate chain size: {CertificateChainSize} (0x{CertificateChainSize:X})");
            builder.AppendLine($"  Ticket size: {TicketSize} (0x{TicketSize:X})");
            builder.AppendLine($"  TMD file size: {TMDFileSize} (0x{TMDFileSize:X})");
            builder.AppendLine($"  Meta size: {MetaSize} (0x{MetaSize:X})");
            builder.AppendLine($"  Content size: {ContentSize} (0x{ContentSize:X})");
            builder.AppendLine($"  Content index: {BitConverter.ToString(ContentIndex).Replace('-', ' ')}");
            builder.AppendLine();
        }

        /// <summary>
        /// Print NCCH partition header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintCertificateChain(StringBuilder builder)
        {
            builder.AppendLine("  Certificate Chain Information:");
            builder.AppendLine("  -------------------------");
            if (CertificateChain == null || CertificateChain.Length == 0)
            {
                builder.AppendLine("  No certificates, expected 3");
            }
            else
            {
                for (int i = 0; i < CertificateChain.Length; i++)
                {
                    var certificate = CertificateChain[i];

                    string certificateName = string.Empty;
                    switch (i)
                    {
                        case 0: certificateName = " (CA)"; break;
                        case 1: certificateName = " (Ticket)"; break;
                        case 2: certificateName = " (TMD)"; break;
                    }

                    builder.AppendLine($"  Certificate {i}{certificateName}");
                    builder.AppendLine($"    Signature type: {certificate.SignatureType} (0x{certificate.SignatureType:X})");
                    builder.AppendLine($"    Signature size: {certificate.SignatureSize} (0x{certificate.SignatureSize:X})");
                    builder.AppendLine($"    Padding size: {certificate.PaddingSize} (0x{certificate.PaddingSize:X})");
                    builder.AppendLine($"    Signature: {BitConverter.ToString(certificate.Signature).Replace('-', ' ')}");
                    builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.Padding).Replace('-', ' ')}");
                    builder.AppendLine($"    Issuer: {certificate.Issuer ?? "[NULL]"}");
                    builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                    builder.AppendLine($"    Name: {certificate.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Expiration time: {certificate.ExpirationTime} (0x{certificate.ExpirationTime:X})");
                    switch (certificate.KeyType)
                    {
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_4096:
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_2048:
                            builder.AppendLine($"    Modulus: {BitConverter.ToString(certificate.RSAModulus).Replace('-', ' ')}");
                            builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                            builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.RSAPadding).Replace('-', ' ')}");
                            break;
                        case SabreTools.Models.N3DS.PublicKeyType.EllipticCurve:
                            builder.AppendLine($"    Public key: {BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' ')}");
                            builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.ECCPadding).Replace('-', ' ')}");
                            break;
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print ticket information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintTicket(StringBuilder builder)
        {
            builder.AppendLine("  Ticket Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature type: {T_SignatureType} (0x{T_SignatureType:X})");
            builder.AppendLine($"  Signature size: {T_SignatureSize} (0x{T_SignatureSize:X})");
            builder.AppendLine($"  Padding size: {T_PaddingSize} (0x{T_PaddingSize:X})");
            builder.AppendLine($"  Signature: {BitConverter.ToString(T_Signature).Replace('-', ' ')}");
            builder.AppendLine($"  Padding: {BitConverter.ToString(T_Padding).Replace('-', ' ')}");
            builder.AppendLine($"  Issuer: {T_Issuer ?? "[NULL]"}");
            builder.AppendLine($"  ECC public key: {BitConverter.ToString(T_ECCPublicKey).Replace('-', ' ')}");
            builder.AppendLine($"  Version: {T_Version} (0x{T_Version:X})");
            builder.AppendLine($"  CaCrlVersion: {T_CaCrlVersion} (0x{T_CaCrlVersion:X})");
            builder.AppendLine($"  SignerCrlVersion: {T_SignerCrlVersion} (0x{T_SignerCrlVersion:X})");
            builder.AppendLine($"  Title key: {BitConverter.ToString(T_TitleKey).Replace('-', ' ')}");
            builder.AppendLine($"  Reserved 1: {T_Reserved1} (0x{T_Reserved1:X})");
            builder.AppendLine($"  Ticket ID: {T_TicketID} (0x{T_TicketID:X})");
            builder.AppendLine($"  Console ID: {T_ConsoleID} (0x{T_ConsoleID:X})");
            builder.AppendLine($"  Title ID {T_TitleID} (0x{T_TitleID:X})");
            builder.AppendLine($"  Reserved 2: {BitConverter.ToString(T_Reserved2).Replace('-', ' ')}");
            builder.AppendLine($"  Ticket title version: {T_TicketTitleVersion} (0x{T_TicketTitleVersion:X})");
            builder.AppendLine($"  Reserved 3: {BitConverter.ToString(T_Reserved3).Replace('-', ' ')}");
            builder.AppendLine($"  License type: {T_LicenseType} (0x{T_LicenseType:X})");
            builder.AppendLine($"  Common keY index: {T_CommonKeyYIndex} (0x{T_CommonKeyYIndex:X})");
            builder.AppendLine($"  Reserved 4: {BitConverter.ToString(T_Reserved4).Replace('-', ' ')}");
            builder.AppendLine($"  eShop Account ID?: {T_eShopAccountID} (0x{T_eShopAccountID:X})");
            builder.AppendLine($"  Reserved 5: {T_Reserved5} (0x{T_Reserved5:X})");
            builder.AppendLine($"  Audit: {T_Audit} (0x{T_Audit:X})");
            builder.AppendLine($"  Reserved 6: {BitConverter.ToString(T_Reserved6).Replace('-', ' ')}");
            builder.AppendLine($"  Limits:");
            for (int i = 0; i < T_Limits.Length; i++)
            {
                builder.AppendLine($"    Limit {i}: {T_Limits[i]} (0x{T_Limits[i]:X})");
            }
            builder.AppendLine($"  Content index size: {T_ContentIndexSize} (0x{T_ContentIndexSize:X})");
            builder.AppendLine($"  Content index: {BitConverter.ToString(T_ContentIndex).Replace('-', ' ')}");
            builder.AppendLine();

            builder.AppendLine("  Ticket Certificate Chain Information:");
            builder.AppendLine("  -------------------------");
            if (T_CertificateChain == null || T_CertificateChain.Length == 0)
            {
                builder.AppendLine("  No certificates, expected 2");
            }
            else
            {
                for (int i = 0; i < T_CertificateChain.Length; i++)
                {
                    var certificate = T_CertificateChain[i];

                    string certificateName = string.Empty;
                    switch (i)
                    {
                        case 0: certificateName = " (Ticket)"; break;
                        case 1: certificateName = " (CA)"; break;
                    }

                    builder.AppendLine($"  Certificate {i}{certificateName}");
                    builder.AppendLine($"    Signature type: {certificate.SignatureType} (0x{certificate.SignatureType:X})");
                    builder.AppendLine($"    Signature size: {certificate.SignatureSize} (0x{certificate.SignatureSize:X})");
                    builder.AppendLine($"    Padding size: {certificate.PaddingSize} (0x{certificate.PaddingSize:X})");
                    builder.AppendLine($"    Signature: {BitConverter.ToString(certificate.Signature).Replace('-', ' ')}");
                    builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.Padding).Replace('-', ' ')}");
                    builder.AppendLine($"    Issuer: {certificate.Issuer ?? "[NULL]"}");
                    builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                    builder.AppendLine($"    Name: {certificate.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Expiration time: {certificate.ExpirationTime} (0x{certificate.ExpirationTime:X})");
                    switch (certificate.KeyType)
                    {
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_4096:
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_2048:
                            builder.AppendLine($"    Modulus: {BitConverter.ToString(certificate.RSAModulus).Replace('-', ' ')}");
                            builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                            builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.RSAPadding).Replace('-', ' ')}");
                            break;
                        case SabreTools.Models.N3DS.PublicKeyType.EllipticCurve:
                            builder.AppendLine($"    Public key: {BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' ')}");
                            builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.ECCPadding).Replace('-', ' ')}");
                            break;
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print title metadata information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintTitleMetadata(StringBuilder builder)
        {
            builder.AppendLine("  Title Metadata Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature type: {TMD_SignatureType} (0x{TMD_SignatureType:X})");
            builder.AppendLine($"  Signature size: {TMD_SignatureSize} (0x{TMD_SignatureSize:X})");
            builder.AppendLine($"  Padding size: {TMD_PaddingSize} (0x{TMD_PaddingSize:X})");
            builder.AppendLine($"  Signature: {BitConverter.ToString(TMD_Signature).Replace('-', ' ')}");
            builder.AppendLine($"  Padding 1: {BitConverter.ToString(TMD_Padding1).Replace('-', ' ')}");
            builder.AppendLine($"  Issuer: {TMD_Issuer ?? "[NULL]"}");
            builder.AppendLine($"  Version: {TMD_Version} (0x{TMD_Version:X})");
            builder.AppendLine($"  CaCrlVersion: {TMD_CaCrlVersion} (0x{TMD_CaCrlVersion:X})");
            builder.AppendLine($"  SignerCrlVersion: {TMD_SignerCrlVersion} (0x{TMD_SignerCrlVersion:X})");
            builder.AppendLine($"  Reserved 1: {TMD_Reserved1} (0x{TMD_Reserved1:X})");
            builder.AppendLine($"  System version: {TMD_SystemVersion} (0x{TMD_SystemVersion:X})");
            builder.AppendLine($"  Title ID: {TMD_TitleID} (0x{TMD_TitleID:X})");
            builder.AppendLine($"  Title type: {TMD_TitleType} (0x{TMD_TitleType:X})");
            builder.AppendLine($"  Group ID: {TMD_GroupID} (0x{TMD_GroupID:X})");
            builder.AppendLine($"  Save data size: {TMD_SaveDataSize} (0x{TMD_SaveDataSize:X})");
            builder.AppendLine($"  SRL private save data size: {TMD_SRLPrivateSaveDataSize} (0x{TMD_SRLPrivateSaveDataSize:X})");
            builder.AppendLine($"  Reserved 2: {BitConverter.ToString(TMD_Reserved2).Replace('-', ' ')}");
            builder.AppendLine($"  SRL flag: {TMD_SRLFlag} (0x{TMD_SRLFlag:X})");
            builder.AppendLine($"  Reserved 3: {BitConverter.ToString(TMD_Reserved3).Replace('-', ' ')}");
            builder.AppendLine($"  Access rights: {TMD_AccessRights} (0x{TMD_AccessRights:X})");
            builder.AppendLine($"  Title version: {TMD_TitleVersion} (0x{TMD_TitleVersion:X})");
            builder.AppendLine($"  Content count: {TMD_ContentCount} (0x{TMD_ContentCount:X})");
            builder.AppendLine($"  Boot content: {TMD_BootContent} (0x{TMD_BootContent:X})");
            builder.AppendLine($"  Padding 2: {BitConverter.ToString(TMD_Padding2).Replace('-', ' ')}");
            builder.AppendLine($"  SHA-256 hash of the content info records: {BitConverter.ToString(TMD_SHA256HashContentInfoRecords).Replace('-', ' ')}");
            builder.AppendLine();

            builder.AppendLine("  Ticket Content Info Records Information:");
            builder.AppendLine("  -------------------------");
            if (TMD_ContentInfoRecords == null || TMD_ContentInfoRecords.Length == 0)
            {
                builder.AppendLine("  No content info records, expected 64");
            }
            else
            {
                for (int i = 0; i < TMD_ContentInfoRecords.Length; i++)
                {
                    var contentInfoRecord = TMD_ContentInfoRecords[i];
                    builder.AppendLine($"  Content Info Record {i}");
                    builder.AppendLine($"    Content index offset: {contentInfoRecord.ContentIndexOffset} (0x{contentInfoRecord.ContentIndexOffset:X})");
                    builder.AppendLine($"    Content command count: {contentInfoRecord.ContentCommandCount} (0x{contentInfoRecord.ContentCommandCount:X})");
                    builder.AppendLine($"    SHA-256 hash of the next {contentInfoRecord.ContentCommandCount} records not hashed: {BitConverter.ToString(contentInfoRecord.UnhashedContentRecordsSHA256Hash).Replace('-', ' ')}");
                }
            }
            builder.AppendLine();

            builder.AppendLine("  Ticket Content Chunk Records Information:");
            builder.AppendLine("  -------------------------");
            if (TMD_ContentChunkRecords == null || TMD_ContentChunkRecords.Length == 0)
            {
                builder.AppendLine($"  No content chunk records, expected {TMD_ContentCount}");
            }
            else
            {
                for (int i = 0; i < TMD_ContentChunkRecords.Length; i++)
                {
                    var contentChunkRecord = TMD_ContentChunkRecords[i];
                    builder.AppendLine($"  Content Chunk Record {i}");
                    builder.AppendLine($"    Content ID: {contentChunkRecord.ContentId} (0x{contentChunkRecord.ContentId:X})");
                    builder.AppendLine($"    Content index: {contentChunkRecord.ContentIndex} (0x{contentChunkRecord.ContentIndex:X})");
                    builder.AppendLine($"    Content type: {contentChunkRecord.ContentType} (0x{contentChunkRecord.ContentType:X})");
                    builder.AppendLine($"    Content size: {contentChunkRecord.ContentSize} (0x{contentChunkRecord.ContentSize:X})");
                    builder.AppendLine($"    SHA-256 hash: {BitConverter.ToString(contentChunkRecord.SHA256Hash).Replace('-', ' ')}");
                }
            }
            builder.AppendLine();

            builder.AppendLine("  Ticket Certificate Chain Information:");
            builder.AppendLine("  -------------------------");
            if (TMD_CertificateChain == null || TMD_CertificateChain.Length == 0)
            {
                builder.AppendLine("  No certificates, expected 2");
            }
            else
            {
                for (int i = 0; i < TMD_CertificateChain.Length; i++)
                {
                    var certificate = TMD_CertificateChain[i];

                    string certificateName = string.Empty;
                    switch (i)
                    {
                        case 0: certificateName = " (TMD)"; break;
                        case 1: certificateName = " (CA)"; break;
                    }

                    builder.AppendLine($"  Certificate {i}{certificateName}");
                    builder.AppendLine($"    Signature type: {certificate.SignatureType} (0x{certificate.SignatureType:X})");
                    builder.AppendLine($"    Signature size: {certificate.SignatureSize} (0x{certificate.SignatureSize:X})");
                    builder.AppendLine($"    Padding size: {certificate.PaddingSize} (0x{certificate.PaddingSize:X})");
                    builder.AppendLine($"    Signature: {BitConverter.ToString(certificate.Signature).Replace('-', ' ')}");
                    builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.Padding).Replace('-', ' ')}");
                    builder.AppendLine($"    Issuer: {certificate.Issuer ?? "[NULL]"}");
                    builder.AppendLine($"    Key type: {certificate.KeyType} (0x{certificate.KeyType:X})");
                    builder.AppendLine($"    Name: {certificate.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Expiration time: {certificate.ExpirationTime} (0x{certificate.ExpirationTime:X})");
                    switch (certificate.KeyType)
                    {
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_4096:
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_2048:
                            builder.AppendLine($"    Modulus: {BitConverter.ToString(certificate.RSAModulus).Replace('-', ' ')}");
                            builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                            builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.RSAPadding).Replace('-', ' ')}");
                            break;
                        case SabreTools.Models.N3DS.PublicKeyType.EllipticCurve:
                            builder.AppendLine($"    Public key: {BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' ')}");
                            builder.AppendLine($"    Padding: {BitConverter.ToString(certificate.ECCPadding).Replace('-', ' ')}");
                            break;
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print NCCH partition header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintPartitions(StringBuilder builder)
        {
            builder.AppendLine("  NCCH Partition Header Information:");
            builder.AppendLine("  -------------------------");
            if (Partitions == null || Partitions.Length == 0)
            {
                builder.AppendLine("  No NCCH partition headers");
            }
            else
            {
                for (int i = 0; i < Partitions.Length; i++)
                {
                    var partitionHeader = Partitions[i];
                    builder.AppendLine($"  NCCH Partition Header {i}");
                    if (partitionHeader.MagicID == string.Empty)
                    {
                        builder.AppendLine($"    Empty partition, no data can be parsed");
                    }
                    else if (partitionHeader.MagicID != SabreTools.Models.N3DS.Constants.NCCHMagicNumber)
                    {
                        builder.AppendLine($"    Unrecognized partition data, no data can be parsed");
                    }
                    else
                    {
                        builder.AppendLine($"    RSA-2048 SHA-256 signature: {BitConverter.ToString(partitionHeader.RSA2048Signature).Replace('-', ' ')}");
                        builder.AppendLine($"    Magic ID: {partitionHeader.MagicID} (0x{partitionHeader.MagicID:X})");
                        builder.AppendLine($"    Content size in media units: {partitionHeader.ContentSizeInMediaUnits} (0x{partitionHeader.ContentSizeInMediaUnits:X})");
                        builder.AppendLine($"    Partition ID: {partitionHeader.PartitionId} (0x{partitionHeader.PartitionId:X})");
                        builder.AppendLine($"    Maker code: {partitionHeader.MakerCode} (0x{partitionHeader.MakerCode:X})");
                        builder.AppendLine($"    Version: {partitionHeader.Version} (0x{partitionHeader.Version:X})");
                        builder.AppendLine($"    Verification hash: {partitionHeader.VerificationHash} (0x{partitionHeader.VerificationHash:X})");
                        builder.AppendLine($"    Program ID: {BitConverter.ToString(partitionHeader.ProgramId).Replace('-', ' ')}");
                        builder.AppendLine($"    Reserved 1: {BitConverter.ToString(partitionHeader.Reserved1).Replace('-', ' ')}");
                        builder.AppendLine($"    Logo region SHA-256 hash: {BitConverter.ToString(partitionHeader.LogoRegionHash).Replace('-', ' ')}");
                        builder.AppendLine($"    Product code: {partitionHeader.ProductCode} (0x{partitionHeader.ProductCode:X})");
                        builder.AppendLine($"    Extended header SHA-256 hash: {BitConverter.ToString(partitionHeader.ExtendedHeaderHash).Replace('-', ' ')}");
                        builder.AppendLine($"    Extended header size in bytes: {partitionHeader.ExtendedHeaderSizeInBytes} (0x{partitionHeader.ExtendedHeaderSizeInBytes:X})");
                        builder.AppendLine($"    Reserved 2: {BitConverter.ToString(partitionHeader.Reserved2).Replace('-', ' ')}");
                        builder.AppendLine("    Flags:");
                        builder.AppendLine($"      Reserved 0: {partitionHeader.Flags.Reserved0} (0x{partitionHeader.Flags.Reserved0:X})");
                        builder.AppendLine($"      Reserved 1: {partitionHeader.Flags.Reserved1} (0x{partitionHeader.Flags.Reserved1:X})");
                        builder.AppendLine($"      Reserved 2: {partitionHeader.Flags.Reserved2} (0x{partitionHeader.Flags.Reserved2:X})");
                        builder.AppendLine($"      Crypto method: {partitionHeader.Flags.CryptoMethod} (0x{partitionHeader.Flags.CryptoMethod:X})");
                        builder.AppendLine($"      Content platform: {partitionHeader.Flags.ContentPlatform} (0x{partitionHeader.Flags.ContentPlatform:X})");
                        builder.AppendLine($"      Content type: {partitionHeader.Flags.MediaPlatformIndex} (0x{partitionHeader.Flags.MediaPlatformIndex:X})");
                        builder.AppendLine($"      Content unit size: {partitionHeader.Flags.ContentUnitSize} (0x{partitionHeader.Flags.ContentUnitSize:X})");
                        builder.AppendLine($"      Bitmasks: {partitionHeader.Flags.BitMasks} (0x{partitionHeader.Flags.BitMasks:X})");
                        builder.AppendLine($"    Plain region offset, in media units: {partitionHeader.PlainRegionOffsetInMediaUnits} (0x{partitionHeader.PlainRegionOffsetInMediaUnits:X})");
                        builder.AppendLine($"    Plain region size, in media units: {partitionHeader.PlainRegionSizeInMediaUnits} (0x{partitionHeader.PlainRegionSizeInMediaUnits:X})");
                        builder.AppendLine($"    Logo region offset, in media units: {partitionHeader.LogoRegionOffsetInMediaUnits} (0x{partitionHeader.LogoRegionOffsetInMediaUnits:X})");
                        builder.AppendLine($"    Logo region size, in media units: {partitionHeader.LogoRegionSizeInMediaUnits} (0x{partitionHeader.LogoRegionSizeInMediaUnits:X})");
                        builder.AppendLine($"    ExeFS offset, in media units: {partitionHeader.ExeFSOffsetInMediaUnits} (0x{partitionHeader.ExeFSOffsetInMediaUnits:X})");
                        builder.AppendLine($"    ExeFS size, in media units: {partitionHeader.ExeFSSizeInMediaUnits} (0x{partitionHeader.ExeFSSizeInMediaUnits:X})");
                        builder.AppendLine($"    ExeFS hash region size, in media units: {partitionHeader.ExeFSHashRegionSizeInMediaUnits} (0x{partitionHeader.ExeFSHashRegionSizeInMediaUnits:X})");
                        builder.AppendLine($"    Reserved 3: {BitConverter.ToString(partitionHeader.Reserved3).Replace('-', ' ')}");
                        builder.AppendLine($"    RomFS offset, in media units: {partitionHeader.RomFSOffsetInMediaUnits} (0x{partitionHeader.RomFSOffsetInMediaUnits:X})");
                        builder.AppendLine($"    RomFS size, in media units: {partitionHeader.RomFSSizeInMediaUnits} (0x{partitionHeader.RomFSSizeInMediaUnits:X})");
                        builder.AppendLine($"    RomFS hash region size, in media units: {partitionHeader.RomFSHashRegionSizeInMediaUnits} (0x{partitionHeader.RomFSHashRegionSizeInMediaUnits:X})");
                        builder.AppendLine($"    Reserved 4: {BitConverter.ToString(partitionHeader.Reserved4).Replace('-', ' ')}");
                        builder.AppendLine($"    ExeFS superblock SHA-256 hash: {BitConverter.ToString(partitionHeader.ExeFSSuperblockHash).Replace('-', ' ')}");
                        builder.AppendLine($"    RomFS superblock SHA-256 hash: {BitConverter.ToString(partitionHeader.RomFSSuperblockHash).Replace('-', ' ')}");
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print metadata information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintMetaData(StringBuilder builder)
        {
            builder.AppendLine("  Meta Data Information:");
            builder.AppendLine("  -------------------------");
            if (_model.MetaData == null || MetaSize == 0)
            {
                builder.AppendLine(value: "  No meta file data");
            }
            else
            {
                builder.AppendLine(value: $"  Title ID dependency list: {BitConverter.ToString(MD_TitleIDDependencyList).Replace('-', ' ')}");
                builder.AppendLine($"  Reserved 1: {BitConverter.ToString(MD_Reserved1).Replace('-', ' ')}");
                builder.AppendLine($"  Core version: {MD_CoreVersion} (0x{MD_CoreVersion:X})");
                builder.AppendLine($"  Reserved 2: {BitConverter.ToString(MD_Reserved2).Replace('-', ' ')}");
                builder.AppendLine($"  Icon data: {BitConverter.ToString(MD_IconData).Replace('-', ' ')}");
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}