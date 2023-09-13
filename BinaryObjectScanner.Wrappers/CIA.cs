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
#if NET48
        public uint HeaderSize => _model.Header.HeaderSize;
#else
        public uint? HeaderSize => _model.Header?.HeaderSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.Type"/>
#if NET48
        public ushort Type => _model.Header.Type;
#else
        public ushort? Type => _model.Header?.Type;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.Version"/>
#if NET48
        public ushort Version => _model.Header.Version;
#else
        public ushort? Version => _model.Header?.Version;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.CertificateChainSize"/>
#if NET48
        public uint CertificateChainSize => _model.Header.CertificateChainSize;
#else
        public uint? CertificateChainSize => _model.Header?.CertificateChainSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.TicketSize"/>
#if NET48
        public uint TicketSize => _model.Header.TicketSize;
#else
        public uint? TicketSize => _model.Header?.TicketSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.TMDFileSize"/>
#if NET48
        public uint TMDFileSize => _model.Header.TMDFileSize;
#else
        public uint? TMDFileSize => _model.Header?.TMDFileSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.MetaSize"/>
#if NET48
        public uint MetaSize => _model.Header.MetaSize;
#else
        public uint? MetaSize => _model.Header?.MetaSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.ContentSize"/>
#if NET48
        public ulong ContentSize => _model.Header.ContentSize;
#else
        public ulong? ContentSize => _model.Header?.ContentSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.ContentIndex"/>
#if NET48
        public byte[] ContentIndex => _model.Header.ContentIndex;
#else
        public byte[]? ContentIndex => _model.Header?.ContentIndex;
#endif

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
#if NET48
        public SabreTools.Models.N3DS.SignatureType T_SignatureType => _model.Ticket.SignatureType;
#else
        public SabreTools.Models.N3DS.SignatureType? T_SignatureType => _model.Ticket?.SignatureType;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.SignatureSize"/>
#if NET48
        public ushort T_SignatureSize => _model.Ticket.SignatureSize;
#else
        public ushort? T_SignatureSize => _model.Ticket?.SignatureSize;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.PaddingSize"/>
#if NET48
        public byte T_PaddingSize => _model.Ticket.PaddingSize;
#else
        public byte? T_PaddingSize => _model.Ticket?.PaddingSize;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Signature"/>
#if NET48
        public byte[] T_Signature => _model.Ticket.Signature;
#else
        public byte[]? T_Signature => _model.Ticket?.Signature;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Padding"/>
#if NET48
        public byte[] T_Padding => _model.Ticket.Padding;
#else
        public byte[]? T_Padding => _model.Ticket?.Padding;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Issuer"/>
#if NET48
        public string T_Issuer => _model.Ticket.Issuer;
#else
        public string? T_Issuer => _model.Ticket?.Issuer;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.ECCPublicKey"/>
#if NET48
        public byte[] T_ECCPublicKey => _model.Ticket.ECCPublicKey;
#else
        public byte[]? T_ECCPublicKey => _model.Ticket?.ECCPublicKey;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Version"/>
#if NET48
        public byte T_Version => _model.Ticket.Version;
#else
        public byte? T_Version => _model.Ticket?.Version;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.CaCrlVersion"/>
#if NET48
        public byte T_CaCrlVersion => _model.Ticket.CaCrlVersion;
#else
        public byte? T_CaCrlVersion => _model.Ticket?.CaCrlVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.SignerCrlVersion"/>
#if NET48
        public byte T_SignerCrlVersion => _model.Ticket.SignerCrlVersion;
#else
        public byte? T_SignerCrlVersion => _model.Ticket?.SignerCrlVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.TitleKey"/>
#if NET48
        public byte[] T_TitleKey => _model.Ticket.TitleKey;
#else
        public byte[]? T_TitleKey => _model.Ticket?.TitleKey;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved1"/>
#if NET48
        public byte T_Reserved1 => _model.Ticket.Reserved1;
#else
        public byte? T_Reserved1 => _model.Ticket?.Reserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.TicketID"/>
#if NET48
        public ulong T_TicketID => _model.Ticket.TicketID;
#else
        public ulong? T_TicketID => _model.Ticket?.TicketID;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.ConsoleID"/>
#if NET48
        public uint T_ConsoleID => _model.Ticket.ConsoleID;
#else
        public uint? T_ConsoleID => _model.Ticket?.ConsoleID;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.TitleID"/>
#if NET48
        public ulong T_TitleID => _model.Ticket.TitleID;
#else
        public ulong? T_TitleID => _model.Ticket?.TitleID;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved2"/>
#if NET48
        public byte[] T_Reserved2 => _model.Ticket.Reserved2;
#else
        public byte[]? T_Reserved2 => _model.Ticket?.Reserved2;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.TicketTitleVersion"/>
#if NET48
        public ushort T_TicketTitleVersion => _model.Ticket.TicketTitleVersion;
#else
        public ushort? T_TicketTitleVersion => _model.Ticket?.TicketTitleVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved3"/>
#if NET48
        public byte[] T_Reserved3 => _model.Ticket.Reserved3;
#else
        public byte[]? T_Reserved3 => _model.Ticket?.Reserved3;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.LicenseType"/>
#if NET48
        public byte T_LicenseType => _model.Ticket.LicenseType;
#else
        public byte? T_LicenseType => _model.Ticket?.LicenseType;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.CommonKeyYIndex"/>
#if NET48
        public byte T_CommonKeyYIndex => _model.Ticket.CommonKeyYIndex;
#else
        public byte? T_CommonKeyYIndex => _model.Ticket?.CommonKeyYIndex;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved4"/>
#if NET48
        public byte[] T_Reserved4 => _model.Ticket.Reserved4;
#else
        public byte[]? T_Reserved4 => _model.Ticket?.Reserved4;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.eShopAccountID"/>
#if NET48
        public uint T_eShopAccountID => _model.Ticket.eShopAccountID;
#else
        public uint? T_eShopAccountID => _model.Ticket?.eShopAccountID;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved5"/>
#if NET48
        public byte T_Reserved5 => _model.Ticket.Reserved5;
#else
        public byte? T_Reserved5 => _model.Ticket?.Reserved5;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Audit"/>
#if NET48
        public byte T_Audit => _model.Ticket.Audit;
#else
        public byte? T_Audit => _model.Ticket?.Audit;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved6"/>
#if NET48
        public byte[] T_Reserved6 => _model.Ticket.Reserved6;
#else
        public byte[]? T_Reserved6 => _model.Ticket?.Reserved6;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Limits"/>
#if NET48
        public uint[] T_Limits => _model.Ticket.Limits;
#else
        public uint[]? T_Limits => _model.Ticket?.Limits;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.ContentIndexSize"/>
#if NET48
        public uint T_ContentIndexSize => _model.Ticket.ContentIndexSize;
#else
        public uint? T_ContentIndexSize => _model.Ticket?.ContentIndexSize;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.ContentIndex"/>
#if NET48
        public byte[] T_ContentIndex => _model.Ticket.ContentIndex;
#else
        public byte[]? T_ContentIndex => _model.Ticket?.ContentIndex;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.CertificateChain"/>
#if NET48
        public SabreTools.Models.N3DS.Certificate[] T_CertificateChain => _model.Ticket.CertificateChain;
#else
        public SabreTools.Models.N3DS.Certificate?[]? T_CertificateChain => _model.Ticket?.CertificateChain;
#endif

        #endregion

        #region Title Metadata

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignatureType"/>
#if NET48
        public SabreTools.Models.N3DS.SignatureType TMD_SignatureType => _model.TMDFileData.SignatureType;
#else
        public SabreTools.Models.N3DS.SignatureType? TMD_SignatureType => _model.TMDFileData?.SignatureType;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignatureSize"/>
#if NET48
        public ushort TMD_SignatureSize => _model.TMDFileData.SignatureSize;
#else
        public ushort? TMD_SignatureSize => _model.TMDFileData?.SignatureSize;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.PaddingSize"/>
#if NET48
        public byte TMD_PaddingSize => _model.TMDFileData.PaddingSize;
#else
        public byte? TMD_PaddingSize => _model.TMDFileData?.PaddingSize;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Signature"/>
#if NET48
        public byte[] TMD_Signature => _model.TMDFileData.Signature;
#else
        public byte[]? TMD_Signature => _model.TMDFileData?.Signature;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Padding1"/>
#if NET48
        public byte[] TMD_Padding1 => _model.TMDFileData.Padding1;
#else
        public byte[]? TMD_Padding1 => _model.TMDFileData?.Padding1;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Issuer"/>
#if NET48
        public string TMD_Issuer => _model.TMDFileData.Issuer;
#else
        public string? TMD_Issuer => _model.TMDFileData?.Issuer;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Version"/>
#if NET48
        public byte TMD_Version => _model.TMDFileData.Version;
#else
        public byte? TMD_Version => _model.TMDFileData?.Version;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.CaCrlVersion"/>
#if NET48
        public byte TMD_CaCrlVersion => _model.TMDFileData.CaCrlVersion;
#else
        public byte? TMD_CaCrlVersion => _model.TMDFileData?.CaCrlVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignerCrlVersion"/>
#if NET48
        public byte TMD_SignerCrlVersion => _model.TMDFileData.SignerCrlVersion;
#else
        public byte? TMD_SignerCrlVersion => _model.TMDFileData?.SignerCrlVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved1"/>
#if NET48
        public byte TMD_Reserved1 => _model.TMDFileData.Reserved1;
#else
        public byte? TMD_Reserved1 => _model.TMDFileData?.Reserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SystemVersion"/>
#if NET48
        public ulong TMD_SystemVersion => _model.TMDFileData.SystemVersion;
#else
        public ulong? TMD_SystemVersion => _model.TMDFileData?.SystemVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleID"/>
#if NET48
        public ulong TMD_TitleID => _model.TMDFileData.TitleID;
#else
        public ulong? TMD_TitleID => _model.TMDFileData?.TitleID;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleType"/>
#if NET48
        public uint TMD_TitleType => _model.TMDFileData.TitleType;
#else
        public uint? TMD_TitleType => _model.TMDFileData?.TitleType;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.GroupID"/>
#if NET48
        public ushort TMD_GroupID => _model.TMDFileData.GroupID;
#else
        public ushort? TMD_GroupID => _model.TMDFileData?.GroupID;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SaveDataSize"/>
#if NET48
        public uint TMD_SaveDataSize => _model.TMDFileData.SaveDataSize;
#else
        public uint? TMD_SaveDataSize => _model.TMDFileData?.SaveDataSize;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SRLPrivateSaveDataSize"/>
#if NET48
        public uint TMD_SRLPrivateSaveDataSize => _model.TMDFileData.SRLPrivateSaveDataSize;
#else
        public uint? TMD_SRLPrivateSaveDataSize => _model.TMDFileData?.SRLPrivateSaveDataSize;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved2"/>
#if NET48
        public byte[] TMD_Reserved2 => _model.TMDFileData.Reserved2;
#else
        public byte[]? TMD_Reserved2 => _model.TMDFileData?.Reserved2;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SRLFlag"/>
#if NET48
        public byte TMD_SRLFlag => _model.TMDFileData.SRLFlag;
#else
        public byte? TMD_SRLFlag => _model.TMDFileData?.SRLFlag;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved3"/>
#if NET48
        public byte[] TMD_Reserved3 => _model.TMDFileData.Reserved3;
#else
        public byte[]? TMD_Reserved3 => _model.TMDFileData?.Reserved3;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.AccessRights"/>
#if NET48
        public uint TMD_AccessRights => _model.TMDFileData.AccessRights;
#else
        public uint? TMD_AccessRights => _model.TMDFileData?.AccessRights;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleVersion"/>
#if NET48
        public ushort TMD_TitleVersion => _model.TMDFileData.TitleVersion;
#else
        public ushort? TMD_TitleVersion => _model.TMDFileData?.TitleVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentCount"/>
#if NET48
        public ushort TMD_ContentCount => _model.TMDFileData.ContentCount;
#else
        public ushort? TMD_ContentCount => _model.TMDFileData?.ContentCount;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.BootContent"/>
#if NET48
        public ushort TMD_BootContent => _model.TMDFileData.BootContent;
#else
        public ushort? TMD_BootContent => _model.TMDFileData?.BootContent;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Padding2"/>
#if NET48
        public byte[] TMD_Padding2 => _model.TMDFileData.Padding2;
#else
        public byte[]? TMD_Padding2 => _model.TMDFileData?.Padding2;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SHA256HashContentInfoRecords"/>
#if NET48
        public byte[] TMD_SHA256HashContentInfoRecords => _model.TMDFileData.SHA256HashContentInfoRecords;
#else
        public byte[]? TMD_SHA256HashContentInfoRecords => _model.TMDFileData?.SHA256HashContentInfoRecords;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentInfoRecords"/>
#if NET48
        public SabreTools.Models.N3DS.ContentInfoRecord[] TMD_ContentInfoRecords => _model.TMDFileData.ContentInfoRecords;
#else
        public SabreTools.Models.N3DS.ContentInfoRecord?[]? TMD_ContentInfoRecords => _model.TMDFileData?.ContentInfoRecords;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentChunkRecords"/>
#if NET48
        public SabreTools.Models.N3DS.ContentChunkRecord[] TMD_ContentChunkRecords => _model.TMDFileData.ContentChunkRecords;
#else
        public SabreTools.Models.N3DS.ContentChunkRecord?[]? TMD_ContentChunkRecords => _model.TMDFileData?.ContentChunkRecords;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.CertificateChain"/>
#if NET48
        public SabreTools.Models.N3DS.Certificate[] TMD_CertificateChain => _model.TMDFileData.CertificateChain;
#else
        public SabreTools.Models.N3DS.Certificate?[]? TMD_CertificateChain => _model.TMDFileData?.CertificateChain;
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
#if NET48
        public byte[] MD_TitleIDDependencyList => _model.MetaData?.TitleIDDependencyList;
#else
        public byte[]? MD_TitleIDDependencyList => _model.MetaData?.TitleIDDependencyList;
#endif

        /// <inheritdoc cref="Models.N3DS.MetaData.Reserved1"/>
#if NET48
        public byte[] MD_Reserved1 => _model.MetaData?.Reserved1;
#else
        public byte[]? MD_Reserved1 => _model.MetaData?.Reserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.MetaData.CoreVersion"/>
        public uint? MD_CoreVersion => _model.MetaData?.CoreVersion;

        /// <inheritdoc cref="Models.N3DS.MetaData.Reserved2"/>
#if NET48
        public byte[] MD_Reserved2 => _model.MetaData?.Reserved2;
#else
        public byte[]? MD_Reserved2 => _model.MetaData?.Reserved2;
#endif

        /// <inheritdoc cref="Models.N3DS.MetaData.IconData"/>
#if NET48
        public byte[] MD_IconData => _model.MetaData?.IconData;
#else
        public byte[]? MD_IconData => _model.MetaData?.IconData;
#endif

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
#if NET48
        public static CIA Create(byte[] data, int offset)
#else
        public static CIA? Create(byte[]? data, int offset)
#endif
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
#if NET48
        public static CIA Create(Stream data)
#else
        public static CIA? Create(Stream? data)
#endif
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
            builder.AppendLine($"  Content index: {(ContentIndex == null ? "[NULL]" : BitConverter.ToString(ContentIndex).Replace('-', ' '))}");
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
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_4096:
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_2048:
                            builder.AppendLine($"    Modulus: {(certificate.RSAModulus == null ? "[NULL]" : BitConverter.ToString(certificate.RSAModulus).Replace('-', ' '))}");
                            builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                            builder.AppendLine($"    Padding: {(certificate.RSAPadding == null ? "[NULL]" : BitConverter.ToString(certificate.RSAPadding).Replace('-', ' '))}");
                            break;
                        case SabreTools.Models.N3DS.PublicKeyType.EllipticCurve:
                            builder.AppendLine($"    Public key: {(certificate.ECCPublicKey == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' '))}");
                            builder.AppendLine($"    Padding: {(certificate.ECCPadding == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPadding).Replace('-', ' '))}");
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
            builder.AppendLine($"  Signature: {(T_Signature == null ? "[NULL]" : BitConverter.ToString(T_Signature).Replace('-', ' '))}");
            builder.AppendLine($"  Padding: {(T_Padding == null ? "[NULL]" : BitConverter.ToString(T_Padding).Replace('-', ' '))}");
            builder.AppendLine($"  Issuer: {T_Issuer ?? "[NULL]"}");
            builder.AppendLine($"  ECC public key: {(T_ECCPublicKey == null ? "[NULL]" : BitConverter.ToString(T_ECCPublicKey).Replace('-', ' '))}");
            builder.AppendLine($"  Version: {T_Version} (0x{T_Version:X})");
            builder.AppendLine($"  CaCrlVersion: {T_CaCrlVersion} (0x{T_CaCrlVersion:X})");
            builder.AppendLine($"  SignerCrlVersion: {T_SignerCrlVersion} (0x{T_SignerCrlVersion:X})");
            builder.AppendLine($"  Title key: {(T_TitleKey == null ? "[NULL]" : BitConverter.ToString(T_TitleKey).Replace('-', ' '))}");
            builder.AppendLine($"  Reserved 1: {T_Reserved1} (0x{T_Reserved1:X})");
            builder.AppendLine($"  Ticket ID: {T_TicketID} (0x{T_TicketID:X})");
            builder.AppendLine($"  Console ID: {T_ConsoleID} (0x{T_ConsoleID:X})");
            builder.AppendLine($"  Title ID {T_TitleID} (0x{T_TitleID:X})");
            builder.AppendLine($"  Reserved 2: {(T_Reserved2 == null ? "[NULL]" : BitConverter.ToString(T_Reserved2).Replace('-', ' '))}");
            builder.AppendLine($"  Ticket title version: {T_TicketTitleVersion} (0x{T_TicketTitleVersion:X})");
            builder.AppendLine($"  Reserved 3: {(T_Reserved3 == null ? "[NULL]" : BitConverter.ToString(T_Reserved3).Replace('-', ' '))}");
            builder.AppendLine($"  License type: {T_LicenseType} (0x{T_LicenseType:X})");
            builder.AppendLine($"  Common keY index: {T_CommonKeyYIndex} (0x{T_CommonKeyYIndex:X})");
            builder.AppendLine($"  Reserved 4: {(T_Reserved4 == null ? "[NULL]" : BitConverter.ToString(T_Reserved4).Replace('-', ' '))}");
            builder.AppendLine($"  eShop Account ID?: {T_eShopAccountID} (0x{T_eShopAccountID:X})");
            builder.AppendLine($"  Reserved 5: {T_Reserved5} (0x{T_Reserved5:X})");
            builder.AppendLine($"  Audit: {T_Audit} (0x{T_Audit:X})");
            builder.AppendLine($"  Reserved 6: {(T_Reserved6 == null ? "[NULL]" : BitConverter.ToString(T_Reserved6).Replace('-', ' '))}");
            builder.AppendLine($"  Limits:");
            if (T_Limits == null || T_Limits.Length == 0)
            {
                builder.AppendLine("    No limits");
            }
            else
            {
                for (int i = 0; i < T_Limits.Length; i++)
                {
                    builder.AppendLine($"    Limit {i}: {T_Limits[i]} (0x{T_Limits[i]:X})");
                }
            }
            builder.AppendLine($"  Content index size: {T_ContentIndexSize} (0x{T_ContentIndexSize:X})");
            builder.AppendLine($"  Content index: {(T_ContentIndex == null ? "[NULL]" : BitConverter.ToString(T_ContentIndex).Replace('-', ' '))}");
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
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_4096:
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_2048:
                            builder.AppendLine($"    Modulus: {(certificate.RSAModulus == null ? "[NULL]" : BitConverter.ToString(certificate.RSAModulus).Replace('-', ' '))}");
                            builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                            builder.AppendLine($"    Padding: {(certificate.RSAPadding == null ? "[NULL]" : BitConverter.ToString(certificate.RSAPadding).Replace('-', ' '))}");
                            break;
                        case SabreTools.Models.N3DS.PublicKeyType.EllipticCurve:
                            builder.AppendLine($"    Public key: {(certificate.ECCPublicKey == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' '))}");
                            builder.AppendLine($"    Padding: {(certificate.ECCPadding == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPadding).Replace('-', ' '))}");
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
            builder.AppendLine($"  Signature: {(TMD_Signature == null ? "[NULL]" : BitConverter.ToString(TMD_Signature).Replace('-', ' '))}");
            builder.AppendLine($"  Padding 1: {(TMD_Padding1 == null ? "[NULL]" : BitConverter.ToString(TMD_Padding1).Replace('-', ' '))}");
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
            builder.AppendLine($"  Reserved 2: {(TMD_Reserved2 == null ? "[NULL]" : BitConverter.ToString(TMD_Reserved2).Replace('-', ' '))}");
            builder.AppendLine($"  SRL flag: {TMD_SRLFlag} (0x{TMD_SRLFlag:X})");
            builder.AppendLine($"  Reserved 3: {(TMD_Reserved3 == null ? "[NULL]" : BitConverter.ToString(TMD_Reserved3).Replace('-', ' '))}");
            builder.AppendLine($"  Access rights: {TMD_AccessRights} (0x{TMD_AccessRights:X})");
            builder.AppendLine($"  Title version: {TMD_TitleVersion} (0x{TMD_TitleVersion:X})");
            builder.AppendLine($"  Content count: {TMD_ContentCount} (0x{TMD_ContentCount:X})");
            builder.AppendLine($"  Boot content: {TMD_BootContent} (0x{TMD_BootContent:X})");
            builder.AppendLine($"  Padding 2: {(TMD_Padding2 == null ? "[NULL]" : BitConverter.ToString(TMD_Padding2).Replace('-', ' '))}");
            builder.AppendLine($"  SHA-256 hash of the content info records: {(TMD_SHA256HashContentInfoRecords == null ? "[NULL]" : BitConverter.ToString(TMD_SHA256HashContentInfoRecords).Replace('-', ' '))}");
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
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_4096:
                        case SabreTools.Models.N3DS.PublicKeyType.RSA_2048:
                            builder.AppendLine($"    Modulus: {(certificate.RSAModulus == null ? "[NULL]" : BitConverter.ToString(certificate.RSAModulus).Replace('-', ' '))}");
                            builder.AppendLine($"    Public exponent: {certificate.RSAPublicExponent} (0x{certificate.RSAPublicExponent:X})");
                            builder.AppendLine($"    Padding: {(certificate.RSAPadding == null ? "[NULL]" : BitConverter.ToString(certificate.RSAPadding).Replace('-', ' '))}");
                            break;
                        case SabreTools.Models.N3DS.PublicKeyType.EllipticCurve:
                            builder.AppendLine($"    Public key: {(certificate.ECCPublicKey == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPublicKey).Replace('-', ' '))}");
                            builder.AppendLine($"    Padding: {(certificate.ECCPadding == null ? "[NULL]" : BitConverter.ToString(certificate.ECCPadding).Replace('-', ' '))}");
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
                    if (partitionHeader == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                builder.AppendLine("  No meta file data");
            }
            else
            {
                builder.AppendLine($"  Title ID dependency list: {(MD_TitleIDDependencyList == null ? "[NULL]" : BitConverter.ToString(MD_TitleIDDependencyList).Replace('-', ' '))}");
                builder.AppendLine($"  Reserved 1: {(MD_Reserved1 == null ? "[NULL]" : BitConverter.ToString(MD_Reserved1).Replace('-', ' '))}");
                builder.AppendLine($"  Core version: {MD_CoreVersion} (0x{MD_CoreVersion:X})");
                builder.AppendLine($"  Reserved 2: {(MD_Reserved2 == null ? "[NULL]" : BitConverter.ToString(MD_Reserved2).Replace('-', ' '))}");
                builder.AppendLine($"  Icon data: {(MD_IconData == null ? "[NULL]" : BitConverter.ToString(MD_IconData).Replace('-', ' '))}");
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