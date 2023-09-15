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
        public uint HeaderSize => this.Model.Header.HeaderSize;
#else
        public uint? HeaderSize => this.Model.Header?.HeaderSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.Type"/>
#if NET48
        public ushort Type => this.Model.Header.Type;
#else
        public ushort? Type => this.Model.Header?.Type;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.Version"/>
#if NET48
        public ushort Version => this.Model.Header.Version;
#else
        public ushort? Version => this.Model.Header?.Version;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.CertificateChainSize"/>
#if NET48
        public uint CertificateChainSize => this.Model.Header.CertificateChainSize;
#else
        public uint? CertificateChainSize => this.Model.Header?.CertificateChainSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.TicketSize"/>
#if NET48
        public uint TicketSize => this.Model.Header.TicketSize;
#else
        public uint? TicketSize => this.Model.Header?.TicketSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.TMDFileSize"/>
#if NET48
        public uint TMDFileSize => this.Model.Header.TMDFileSize;
#else
        public uint? TMDFileSize => this.Model.Header?.TMDFileSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.MetaSize"/>
#if NET48
        public uint MetaSize => this.Model.Header.MetaSize;
#else
        public uint? MetaSize => this.Model.Header?.MetaSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.ContentSize"/>
#if NET48
        public ulong ContentSize => this.Model.Header.ContentSize;
#else
        public ulong? ContentSize => this.Model.Header?.ContentSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CIAHeader.ContentIndex"/>
#if NET48
        public byte[] ContentIndex => this.Model.Header.ContentIndex;
#else
        public byte[]? ContentIndex => this.Model.Header?.ContentIndex;
#endif

        #endregion

        #region Certificate Chain

        /// <inheritdoc cref="Models.N3DS.CIA.CertificateChain"/>
#if NET48
        public SabreTools.Models.N3DS.Certificate[] CertificateChain => this.Model.CertificateChain;
#else
        public SabreTools.Models.N3DS.Certificate?[]? CertificateChain => this.Model.CertificateChain;
#endif

        #endregion

        #region Ticket

        /// <inheritdoc cref="Models.N3DS.Ticket.SignatureType"/>
#if NET48
        public SabreTools.Models.N3DS.SignatureType T_SignatureType => this.Model.Ticket.SignatureType;
#else
        public SabreTools.Models.N3DS.SignatureType? T_SignatureType => this.Model.Ticket?.SignatureType;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.SignatureSize"/>
#if NET48
        public ushort T_SignatureSize => this.Model.Ticket.SignatureSize;
#else
        public ushort? T_SignatureSize => this.Model.Ticket?.SignatureSize;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.PaddingSize"/>
#if NET48
        public byte T_PaddingSize => this.Model.Ticket.PaddingSize;
#else
        public byte? T_PaddingSize => this.Model.Ticket?.PaddingSize;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Signature"/>
#if NET48
        public byte[] T_Signature => this.Model.Ticket.Signature;
#else
        public byte[]? T_Signature => this.Model.Ticket?.Signature;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Padding"/>
#if NET48
        public byte[] T_Padding => this.Model.Ticket.Padding;
#else
        public byte[]? T_Padding => this.Model.Ticket?.Padding;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Issuer"/>
#if NET48
        public string T_Issuer => this.Model.Ticket.Issuer;
#else
        public string? T_Issuer => this.Model.Ticket?.Issuer;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.ECCPublicKey"/>
#if NET48
        public byte[] T_ECCPublicKey => this.Model.Ticket.ECCPublicKey;
#else
        public byte[]? T_ECCPublicKey => this.Model.Ticket?.ECCPublicKey;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Version"/>
#if NET48
        public byte T_Version => this.Model.Ticket.Version;
#else
        public byte? T_Version => this.Model.Ticket?.Version;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.CaCrlVersion"/>
#if NET48
        public byte T_CaCrlVersion => this.Model.Ticket.CaCrlVersion;
#else
        public byte? T_CaCrlVersion => this.Model.Ticket?.CaCrlVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.SignerCrlVersion"/>
#if NET48
        public byte T_SignerCrlVersion => this.Model.Ticket.SignerCrlVersion;
#else
        public byte? T_SignerCrlVersion => this.Model.Ticket?.SignerCrlVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.TitleKey"/>
#if NET48
        public byte[] T_TitleKey => this.Model.Ticket.TitleKey;
#else
        public byte[]? T_TitleKey => this.Model.Ticket?.TitleKey;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved1"/>
#if NET48
        public byte T_Reserved1 => this.Model.Ticket.Reserved1;
#else
        public byte? T_Reserved1 => this.Model.Ticket?.Reserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.TicketID"/>
#if NET48
        public ulong T_TicketID => this.Model.Ticket.TicketID;
#else
        public ulong? T_TicketID => this.Model.Ticket?.TicketID;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.ConsoleID"/>
#if NET48
        public uint T_ConsoleID => this.Model.Ticket.ConsoleID;
#else
        public uint? T_ConsoleID => this.Model.Ticket?.ConsoleID;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.TitleID"/>
#if NET48
        public ulong T_TitleID => this.Model.Ticket.TitleID;
#else
        public ulong? T_TitleID => this.Model.Ticket?.TitleID;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved2"/>
#if NET48
        public byte[] T_Reserved2 => this.Model.Ticket.Reserved2;
#else
        public byte[]? T_Reserved2 => this.Model.Ticket?.Reserved2;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.TicketTitleVersion"/>
#if NET48
        public ushort T_TicketTitleVersion => this.Model.Ticket.TicketTitleVersion;
#else
        public ushort? T_TicketTitleVersion => this.Model.Ticket?.TicketTitleVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved3"/>
#if NET48
        public byte[] T_Reserved3 => this.Model.Ticket.Reserved3;
#else
        public byte[]? T_Reserved3 => this.Model.Ticket?.Reserved3;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.LicenseType"/>
#if NET48
        public byte T_LicenseType => this.Model.Ticket.LicenseType;
#else
        public byte? T_LicenseType => this.Model.Ticket?.LicenseType;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.CommonKeyYIndex"/>
#if NET48
        public byte T_CommonKeyYIndex => this.Model.Ticket.CommonKeyYIndex;
#else
        public byte? T_CommonKeyYIndex => this.Model.Ticket?.CommonKeyYIndex;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved4"/>
#if NET48
        public byte[] T_Reserved4 => this.Model.Ticket.Reserved4;
#else
        public byte[]? T_Reserved4 => this.Model.Ticket?.Reserved4;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.eShopAccountID"/>
#if NET48
        public uint T_eShopAccountID => this.Model.Ticket.eShopAccountID;
#else
        public uint? T_eShopAccountID => this.Model.Ticket?.eShopAccountID;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved5"/>
#if NET48
        public byte T_Reserved5 => this.Model.Ticket.Reserved5;
#else
        public byte? T_Reserved5 => this.Model.Ticket?.Reserved5;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Audit"/>
#if NET48
        public byte T_Audit => this.Model.Ticket.Audit;
#else
        public byte? T_Audit => this.Model.Ticket?.Audit;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Reserved6"/>
#if NET48
        public byte[] T_Reserved6 => this.Model.Ticket.Reserved6;
#else
        public byte[]? T_Reserved6 => this.Model.Ticket?.Reserved6;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.Limits"/>
#if NET48
        public uint[] T_Limits => this.Model.Ticket.Limits;
#else
        public uint[]? T_Limits => this.Model.Ticket?.Limits;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.ContentIndexSize"/>
#if NET48
        public uint T_ContentIndexSize => this.Model.Ticket.ContentIndexSize;
#else
        public uint? T_ContentIndexSize => this.Model.Ticket?.ContentIndexSize;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.ContentIndex"/>
#if NET48
        public byte[] T_ContentIndex => this.Model.Ticket.ContentIndex;
#else
        public byte[]? T_ContentIndex => this.Model.Ticket?.ContentIndex;
#endif

        /// <inheritdoc cref="Models.N3DS.Ticket.CertificateChain"/>
#if NET48
        public SabreTools.Models.N3DS.Certificate[] T_CertificateChain => this.Model.Ticket.CertificateChain;
#else
        public SabreTools.Models.N3DS.Certificate?[]? T_CertificateChain => this.Model.Ticket?.CertificateChain;
#endif

        #endregion

        #region Title Metadata

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignatureType"/>
#if NET48
        public SabreTools.Models.N3DS.SignatureType TMD_SignatureType => this.Model.TMDFileData.SignatureType;
#else
        public SabreTools.Models.N3DS.SignatureType? TMD_SignatureType => this.Model.TMDFileData?.SignatureType;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignatureSize"/>
#if NET48
        public ushort TMD_SignatureSize => this.Model.TMDFileData.SignatureSize;
#else
        public ushort? TMD_SignatureSize => this.Model.TMDFileData?.SignatureSize;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.PaddingSize"/>
#if NET48
        public byte TMD_PaddingSize => this.Model.TMDFileData.PaddingSize;
#else
        public byte? TMD_PaddingSize => this.Model.TMDFileData?.PaddingSize;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Signature"/>
#if NET48
        public byte[] TMD_Signature => this.Model.TMDFileData.Signature;
#else
        public byte[]? TMD_Signature => this.Model.TMDFileData?.Signature;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Padding1"/>
#if NET48
        public byte[] TMD_Padding1 => this.Model.TMDFileData.Padding1;
#else
        public byte[]? TMD_Padding1 => this.Model.TMDFileData?.Padding1;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Issuer"/>
#if NET48
        public string TMD_Issuer => this.Model.TMDFileData.Issuer;
#else
        public string? TMD_Issuer => this.Model.TMDFileData?.Issuer;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Version"/>
#if NET48
        public byte TMD_Version => this.Model.TMDFileData.Version;
#else
        public byte? TMD_Version => this.Model.TMDFileData?.Version;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.CaCrlVersion"/>
#if NET48
        public byte TMD_CaCrlVersion => this.Model.TMDFileData.CaCrlVersion;
#else
        public byte? TMD_CaCrlVersion => this.Model.TMDFileData?.CaCrlVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SignerCrlVersion"/>
#if NET48
        public byte TMD_SignerCrlVersion => this.Model.TMDFileData.SignerCrlVersion;
#else
        public byte? TMD_SignerCrlVersion => this.Model.TMDFileData?.SignerCrlVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved1"/>
#if NET48
        public byte TMD_Reserved1 => this.Model.TMDFileData.Reserved1;
#else
        public byte? TMD_Reserved1 => this.Model.TMDFileData?.Reserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SystemVersion"/>
#if NET48
        public ulong TMD_SystemVersion => this.Model.TMDFileData.SystemVersion;
#else
        public ulong? TMD_SystemVersion => this.Model.TMDFileData?.SystemVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleID"/>
#if NET48
        public ulong TMD_TitleID => this.Model.TMDFileData.TitleID;
#else
        public ulong? TMD_TitleID => this.Model.TMDFileData?.TitleID;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleType"/>
#if NET48
        public uint TMD_TitleType => this.Model.TMDFileData.TitleType;
#else
        public uint? TMD_TitleType => this.Model.TMDFileData?.TitleType;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.GroupID"/>
#if NET48
        public ushort TMD_GroupID => this.Model.TMDFileData.GroupID;
#else
        public ushort? TMD_GroupID => this.Model.TMDFileData?.GroupID;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SaveDataSize"/>
#if NET48
        public uint TMD_SaveDataSize => this.Model.TMDFileData.SaveDataSize;
#else
        public uint? TMD_SaveDataSize => this.Model.TMDFileData?.SaveDataSize;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SRLPrivateSaveDataSize"/>
#if NET48
        public uint TMD_SRLPrivateSaveDataSize => this.Model.TMDFileData.SRLPrivateSaveDataSize;
#else
        public uint? TMD_SRLPrivateSaveDataSize => this.Model.TMDFileData?.SRLPrivateSaveDataSize;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved2"/>
#if NET48
        public byte[] TMD_Reserved2 => this.Model.TMDFileData.Reserved2;
#else
        public byte[]? TMD_Reserved2 => this.Model.TMDFileData?.Reserved2;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SRLFlag"/>
#if NET48
        public byte TMD_SRLFlag => this.Model.TMDFileData.SRLFlag;
#else
        public byte? TMD_SRLFlag => this.Model.TMDFileData?.SRLFlag;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Reserved3"/>
#if NET48
        public byte[] TMD_Reserved3 => this.Model.TMDFileData.Reserved3;
#else
        public byte[]? TMD_Reserved3 => this.Model.TMDFileData?.Reserved3;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.AccessRights"/>
#if NET48
        public uint TMD_AccessRights => this.Model.TMDFileData.AccessRights;
#else
        public uint? TMD_AccessRights => this.Model.TMDFileData?.AccessRights;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.TitleVersion"/>
#if NET48
        public ushort TMD_TitleVersion => this.Model.TMDFileData.TitleVersion;
#else
        public ushort? TMD_TitleVersion => this.Model.TMDFileData?.TitleVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentCount"/>
#if NET48
        public ushort TMD_ContentCount => this.Model.TMDFileData.ContentCount;
#else
        public ushort? TMD_ContentCount => this.Model.TMDFileData?.ContentCount;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.BootContent"/>
#if NET48
        public ushort TMD_BootContent => this.Model.TMDFileData.BootContent;
#else
        public ushort? TMD_BootContent => this.Model.TMDFileData?.BootContent;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.Padding2"/>
#if NET48
        public byte[] TMD_Padding2 => this.Model.TMDFileData.Padding2;
#else
        public byte[]? TMD_Padding2 => this.Model.TMDFileData?.Padding2;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.SHA256HashContentInfoRecords"/>
#if NET48
        public byte[] TMD_SHA256HashContentInfoRecords => this.Model.TMDFileData.SHA256HashContentInfoRecords;
#else
        public byte[]? TMD_SHA256HashContentInfoRecords => this.Model.TMDFileData?.SHA256HashContentInfoRecords;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentInfoRecords"/>
#if NET48
        public SabreTools.Models.N3DS.ContentInfoRecord[] TMD_ContentInfoRecords => this.Model.TMDFileData.ContentInfoRecords;
#else
        public SabreTools.Models.N3DS.ContentInfoRecord?[]? TMD_ContentInfoRecords => this.Model.TMDFileData?.ContentInfoRecords;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.ContentChunkRecords"/>
#if NET48
        public SabreTools.Models.N3DS.ContentChunkRecord[] TMD_ContentChunkRecords => this.Model.TMDFileData.ContentChunkRecords;
#else
        public SabreTools.Models.N3DS.ContentChunkRecord?[]? TMD_ContentChunkRecords => this.Model.TMDFileData?.ContentChunkRecords;
#endif

        /// <inheritdoc cref="Models.N3DS.TitleMetadata.CertificateChain"/>
#if NET48
        public SabreTools.Models.N3DS.Certificate[] TMD_CertificateChain => this.Model.TMDFileData.CertificateChain;
#else
        public SabreTools.Models.N3DS.Certificate?[]? TMD_CertificateChain => this.Model.TMDFileData?.CertificateChain;
#endif

        #endregion

        #region Partitions

        /// <inheritdoc cref="Models.N3DS.CIA.Partitions"/>
#if NET48
        public SabreTools.Models.N3DS.NCCHHeader[] Partitions => this.Model.Partitions;
#else
        public SabreTools.Models.N3DS.NCCHHeader?[]? Partitions => this.Model.Partitions;
#endif

        #endregion

        #region Meta Data

        /// <inheritdoc cref="Models.N3DS.MetaData.TitleIDDependencyList"/>
#if NET48
        public byte[] MD_TitleIDDependencyList => this.Model.MetaData?.TitleIDDependencyList;
#else
        public byte[]? MD_TitleIDDependencyList => this.Model.MetaData?.TitleIDDependencyList;
#endif

        /// <inheritdoc cref="Models.N3DS.MetaData.Reserved1"/>
#if NET48
        public byte[] MD_Reserved1 => this.Model.MetaData?.Reserved1;
#else
        public byte[]? MD_Reserved1 => this.Model.MetaData?.Reserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.MetaData.CoreVersion"/>
        public uint? MD_CoreVersion => this.Model.MetaData?.CoreVersion;

        /// <inheritdoc cref="Models.N3DS.MetaData.Reserved2"/>
#if NET48
        public byte[] MD_Reserved2 => this.Model.MetaData?.Reserved2;
#else
        public byte[]? MD_Reserved2 => this.Model.MetaData?.Reserved2;
#endif

        /// <inheritdoc cref="Models.N3DS.MetaData.IconData"/>
#if NET48
        public byte[] MD_IconData => this.Model.MetaData?.IconData;
#else
        public byte[]? MD_IconData => this.Model.MetaData?.IconData;
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
            Printing.CIA.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}