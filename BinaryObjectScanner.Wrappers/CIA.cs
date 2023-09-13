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
            Printing.CIA.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}