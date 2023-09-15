using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class N3DS : WrapperBase<SabreTools.Models.N3DS.Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo 3DS Cart Image";

        #endregion

        #region Pass-Through Properties

        #region Header

        #region Common to all NCSD files

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.RSA2048Signature"/>
#if NET48
        public byte[] RSA2048Signature => _model.Header.RSA2048Signature;
#else
        public byte[]? RSA2048Signature => _model.Header?.RSA2048Signature;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.MagicNumber"/>
#if NET48
        public string MagicNumber => _model.Header.MagicNumber;
#else
        public string? MagicNumber => _model.Header?.MagicNumber;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.ImageSizeInMediaUnits"/>
#if NET48
        public uint ImageSizeInMediaUnits => _model.Header.ImageSizeInMediaUnits;
#else
        public uint? ImageSizeInMediaUnits => _model.Header?.ImageSizeInMediaUnits;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.MediaId"/>
#if NET48
        public byte[] MediaId => _model.Header.MediaId;
#else
        public byte[]? MediaId => _model.Header?.MediaId;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsFSType"/>
#if NET48
        public SabreTools.Models.N3DS.FilesystemType PartitionsFSType => _model.Header.PartitionsFSType;
#else
        public SabreTools.Models.N3DS.FilesystemType? PartitionsFSType => _model.Header?.PartitionsFSType;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsCryptType"/>
#if NET48
        public byte[] PartitionsCryptType => _model.Header.PartitionsCryptType;
#else
        public byte[]? PartitionsCryptType => _model.Header?.PartitionsCryptType;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsTable"/>
#if NET48
        public SabreTools.Models.N3DS.PartitionTableEntry[] PartitionsTable => _model.Header.PartitionsTable;
#else
        public SabreTools.Models.N3DS.PartitionTableEntry?[]? PartitionsTable => _model.Header?.PartitionsTable;
#endif

        #endregion

        #region CTR Cart Image (CCI) Specific

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.ExheaderHash"/>
#if NET48
        public byte[] ExheaderHash => _model.Header.ExheaderHash;
#else
        public byte[]? ExheaderHash => _model.Header?.ExheaderHash;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.AdditionalHeaderSize"/>
#if NET48
        public uint AdditionalHeaderSize => _model.Header.AdditionalHeaderSize;
#else
        public uint? AdditionalHeaderSize => _model.Header?.AdditionalHeaderSize;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.SectorZeroOffset"/>
#if NET48
        public uint SectorZeroOffset => _model.Header.SectorZeroOffset;
#else
        public uint? SectorZeroOffset => _model.Header?.SectorZeroOffset;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionFlags"/>
#if NET48
        public byte[] PartitionFlags => _model.Header.PartitionFlags;
#else
        public byte[]? PartitionFlags => _model.Header?.PartitionFlags;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionIdTable"/>
#if NET48
        public ulong[] PartitionIdTable => _model.Header.PartitionIdTable;
#else
        public ulong[]? PartitionIdTable => _model.Header?.PartitionIdTable;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Reserved1"/>
#if NET48
        public byte[] Reserved1 => _model.Header.Reserved1;
#else
        public byte[]? Reserved1 => _model.Header?.Reserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Reserved2"/>
#if NET48
        public byte[] Reserved2 => _model.Header.Reserved2;
#else
        public byte[]? Reserved2 => _model.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.FirmUpdateByte1"/>
#if NET48
        public byte FirmUpdateByte1 => _model.Header.FirmUpdateByte1;
#else
        public byte? FirmUpdateByte1 => _model.Header?.FirmUpdateByte1;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.FirmUpdateByte2"/>
#if NET48
        public byte FirmUpdateByte2 => _model.Header.FirmUpdateByte2;
#else
        public byte? FirmUpdateByte2 => _model.Header?.FirmUpdateByte2;
#endif

        #endregion

        #region Raw NAND Format Specific

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Unknown"/>
#if NET48
        public byte[] Unknown => _model.Header.Unknown;
#else
        public byte[]? Unknown => _model.Header?.Unknown;
#endif

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.EncryptedMBR"/>
#if NET48
        public byte[] EncryptedMBR => _model.Header.EncryptedMBR;
#else
        public byte[]? EncryptedMBR => _model.Header?.EncryptedMBR;
#endif

        #endregion

        #endregion

        #region Card Info Header

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.WritableAddressMediaUnits"/>
#if NET48
        public uint CIH_WritableAddressMediaUnits => _model.CardInfoHeader.WritableAddressMediaUnits;
#else
        public uint? CIH_WritableAddressMediaUnits => _model.CardInfoHeader?.WritableAddressMediaUnits;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CardInfoBitmask"/>
#if NET48
        public uint CIH_CardInfoBitmask => _model.CardInfoHeader.CardInfoBitmask;
#else
        public uint? CIH_CardInfoBitmask => _model.CardInfoHeader?.CardInfoBitmask;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved1"/>
#if NET48
        public byte[] CIH_Reserved1 => _model.CardInfoHeader.Reserved1;
#else
        public byte[]? CIH_Reserved1 => _model.CardInfoHeader?.Reserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.FilledSize"/>
#if NET48
        public uint CIH_FilledSize => _model.CardInfoHeader.FilledSize;
#else
        public uint? CIH_FilledSize => _model.CardInfoHeader?.FilledSize;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved2"/>
#if NET48
        public byte[] CIH_Reserved2 => _model.CardInfoHeader.Reserved2;
#else
        public byte[]? CIH_Reserved2 => _model.CardInfoHeader?.Reserved2;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.TitleVersion"/>
#if NET48
        public ushort CIH_TitleVersion => _model.CardInfoHeader.TitleVersion;
#else
        public ushort? CIH_TitleVersion => _model.CardInfoHeader?.TitleVersion;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CardRevision"/>
#if NET48
        public ushort CIH_CardRevision => _model.CardInfoHeader.CardRevision;
#else
        public ushort? CIH_CardRevision => _model.CardInfoHeader?.CardRevision;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved3"/>
#if NET48
        public byte[] CIH_Reserved3 => _model.CardInfoHeader.Reserved3;
#else
        public byte[]? CIH_Reserved3 => _model.CardInfoHeader?.Reserved3;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CVerTitleID"/>
#if NET48
        public byte[] CIH_CVerTitleID => _model.CardInfoHeader.CVerTitleID;
#else
        public byte[]? CIH_CVerTitleID => _model.CardInfoHeader?.CVerTitleID;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CVerVersionNumber"/>
#if NET48
        public ushort CIH_CVerVersionNumber => _model.CardInfoHeader.CVerVersionNumber;
#else
        public ushort? CIH_CVerVersionNumber => _model.CardInfoHeader?.CVerVersionNumber;
#endif

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved4"/>
#if NET48
        public byte[] CIH_Reserved4 => _model.CardInfoHeader.Reserved4;
#else
        public byte[]? CIH_Reserved4 => _model.CardInfoHeader?.Reserved4;
#endif

        #endregion

        #region Development Card Info Header

        #region Initial Data

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedKeyY"/>
#if NET48
        public byte[] DCIH_ID_CardSeedKeyY => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedKeyY;
#else
        public byte[]? DCIH_ID_CardSeedKeyY => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedKeyY;
#endif

        /// <inheritdoc cref="Models.N3DS.InitialData.EncryptedCardSeed"/>
#if NET48
        public byte[] DCIH_ID_EncryptedCardSeed => _model.DevelopmentCardInfoHeader?.InitialData?.EncryptedCardSeed;
#else
        public byte[]? DCIH_ID_EncryptedCardSeed => _model.DevelopmentCardInfoHeader?.InitialData?.EncryptedCardSeed;
#endif

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedAESMAC"/>
#if NET48
        public byte[] DCIH_ID_CardSeedAESMAC => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedAESMAC;
#else
        public byte[]? DCIH_ID_CardSeedAESMAC => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedAESMAC;
#endif

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedNonce"/>
#if NET48
        public byte[] DCIH_ID_CardSeedNonce => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedNonce;
#else
        public byte[]? DCIH_ID_CardSeedNonce => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedNonce;
#endif

        /// <inheritdoc cref="Models.N3DS.InitialData.Reserved3"/>
#if NET48
        public byte[] DCIH_ID_Reserved => _model.DevelopmentCardInfoHeader?.InitialData?.Reserved;
#else
        public byte[]? DCIH_ID_Reserved => _model.DevelopmentCardInfoHeader?.InitialData?.Reserved;
#endif

        /// <inheritdoc cref="Models.N3DS.InitialData.BackupHeader"/>
#if NET48
        public SabreTools.Models.N3DS.NCCHHeader DCIH_ID_BackupHeader => _model.DevelopmentCardInfoHeader?.InitialData?.BackupHeader;
#else
        public SabreTools.Models.N3DS.NCCHHeader? DCIH_ID_BackupHeader => _model.DevelopmentCardInfoHeader?.InitialData?.BackupHeader;
#endif

        #endregion

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.CardDeviceReserved1"/>
#if NET48
        public byte[] DCIH_CardDeviceReserved1 => _model.DevelopmentCardInfoHeader?.CardDeviceReserved1;
#else
        public byte[]? DCIH_CardDeviceReserved1 => _model.DevelopmentCardInfoHeader?.CardDeviceReserved1;
#endif

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.TitleKey"/>
#if NET48
        public byte[] DCIH_TitleKey => _model.DevelopmentCardInfoHeader?.TitleKey;
#else
        public byte[]? DCIH_TitleKey => _model.DevelopmentCardInfoHeader?.TitleKey;
#endif

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.CardDeviceReserved2"/>
#if NET48
        public byte[] DCIH_CardDeviceReserved2 => _model.DevelopmentCardInfoHeader?.CardDeviceReserved2;
#else
        public byte[]? DCIH_CardDeviceReserved2 => _model.DevelopmentCardInfoHeader?.CardDeviceReserved2;
#endif

        #region Test Data

        /// <inheritdoc cref="Models.N3DS.TestData.Signature"/>
#if NET48
        public byte[] DCIH_TD_Signature => _model.DevelopmentCardInfoHeader?.TestData?.Signature;
#else
        public byte[]? DCIH_TD_Signature => _model.DevelopmentCardInfoHeader?.TestData?.Signature;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.AscendingByteSequence"/>
#if NET48
        public byte[] DCIH_TD_AscendingByteSequence => _model.DevelopmentCardInfoHeader?.TestData?.AscendingByteSequence;
#else
        public byte[]? DCIH_TD_AscendingByteSequence => _model.DevelopmentCardInfoHeader?.TestData?.AscendingByteSequence;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.DescendingByteSequence"/>
#if NET48
        public byte[] DCIH_TD_DescendingByteSequence => _model.DevelopmentCardInfoHeader?.TestData?.DescendingByteSequence;
#else
        public byte[]? DCIH_TD_DescendingByteSequence => _model.DevelopmentCardInfoHeader?.TestData?.DescendingByteSequence;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.Filled00"/>
#if NET48
        public byte[] DCIH_TD_Filled00 => _model.DevelopmentCardInfoHeader?.TestData?.Filled00;
#else
        public byte[]? DCIH_TD_Filled00 => _model.DevelopmentCardInfoHeader?.TestData?.Filled00;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.FilledFF"/>
#if NET48
        public byte[] DCIH_TD_FilledFF => _model.DevelopmentCardInfoHeader?.TestData?.FilledFF;
#else
        public byte[]? DCIH_TD_FilledFF => _model.DevelopmentCardInfoHeader?.TestData?.FilledFF;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.Filled0F"/>
#if NET48
        public byte[] DCIH_TD_Filled0F => _model.DevelopmentCardInfoHeader?.TestData?.Filled0F;
#else
        public byte[]? DCIH_TD_Filled0F => _model.DevelopmentCardInfoHeader?.TestData?.Filled0F;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.FilledF0"/>
#if NET48
        public byte[] DCIH_TD_FilledF0 => _model.DevelopmentCardInfoHeader?.TestData?.FilledF0;
#else
        public byte[]? DCIH_TD_FilledF0 => _model.DevelopmentCardInfoHeader?.TestData?.FilledF0;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.Filled55"/>
#if NET48
        public byte[] DCIH_TD_Filled55 => _model.DevelopmentCardInfoHeader?.TestData?.Filled55;
#else
        public byte[]? DCIH_TD_Filled55 => _model.DevelopmentCardInfoHeader?.TestData?.Filled55;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.FilledAA"/>
#if NET48
        public byte[] DCIH_TD_FilledAA => _model.DevelopmentCardInfoHeader?.TestData?.FilledAA;
#else
        public byte[]? DCIH_TD_FilledAA => _model.DevelopmentCardInfoHeader?.TestData?.FilledAA;
#endif

        /// <inheritdoc cref="Models.N3DS.TestData.FinalByte"/>
        public byte? DCIH_TD_FinalByte => _model.DevelopmentCardInfoHeader?.TestData?.FinalByte;

        #endregion

        #endregion

        #region Partitions

        /// <inheritdoc cref="Models.N3DS.Cart.Partitions"/>
#if NET48
        public SabreTools.Models.N3DS.NCCHHeader[] Partitions => _model.Partitions;
#else
        public SabreTools.Models.N3DS.NCCHHeader?[]? Partitions => _model.Partitions;
#endif

        #endregion

        #region Extended Headers

        /// <inheritdoc cref="Models.N3DS.Cart.ExtendedHeaders"/>
#if NET48
        public SabreTools.Models.N3DS.NCCHExtendedHeader[] ExtendedHeaders => _model.ExtendedHeaders;
#else
        public SabreTools.Models.N3DS.NCCHExtendedHeader?[]? ExtendedHeaders => _model.ExtendedHeaders;
#endif

        #endregion

        #region ExeFS Headers

        /// <inheritdoc cref="Models.N3DS.Cart.ExeFSHeaders"/>
#if NET48
        public SabreTools.Models.N3DS.ExeFSHeader[] ExeFSHeaders => _model.ExeFSHeaders;
#else
        public SabreTools.Models.N3DS.ExeFSHeader?[]? ExeFSHeaders => _model.ExeFSHeaders;
#endif

        #endregion

        #region RomFS Headers

        /// <inheritdoc cref="Models.N3DS.Cart.RomFSHeaders"/>
#if NET48
        public SabreTools.Models.N3DS.RomFSHeader[] RomFSHeaders => _model.RomFSHeaders;
#else
        public SabreTools.Models.N3DS.RomFSHeader?[]? RomFSHeaders => _model.RomFSHeaders;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public N3DS(SabreTools.Models.N3DS.Cart model, byte[] data, int offset)
#else
        public N3DS(SabreTools.Models.N3DS.Cart? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public N3DS(SabreTools.Models.N3DS.Cart model, Stream data)
#else
        public N3DS(SabreTools.Models.N3DS.Cart? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a 3DS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
#if NET48
        public static N3DS Create(byte[] data, int offset)
#else
        public static N3DS? Create(byte[]? data, int offset)
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
        /// Create a 3DS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
#if NET48
        public static N3DS Create(Stream data)
#else
        public static N3DS? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.N3DS().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new N3DS(archive, data);
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
            Printing.N3DS.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}