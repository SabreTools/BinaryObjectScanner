using System;
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
        public byte[] RSA2048Signature => _model.Header.RSA2048Signature;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.MagicNumber"/>
        public string MagicNumber => _model.Header.MagicNumber;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.ImageSizeInMediaUnits"/>
        public uint ImageSizeInMediaUnits => _model.Header.ImageSizeInMediaUnits;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.MediaId"/>
        public byte[] MediaId => _model.Header.MediaId;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsFSType"/>
        public SabreTools.Models.N3DS.FilesystemType PartitionsFSType => _model.Header.PartitionsFSType;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsCryptType"/>
        public byte[] PartitionsCryptType => _model.Header.PartitionsCryptType;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsTable"/>
#if NET48
        public SabreTools.Models.N3DS.PartitionTableEntry[] PartitionsTable => _model.Header.PartitionsTable;
#else
        public SabreTools.Models.N3DS.PartitionTableEntry?[]? PartitionsTable => _model.Header.PartitionsTable;
#endif

        #endregion

        #region CTR Cart Image (CCI) Specific

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.ExheaderHash"/>
        public byte[] ExheaderHash => _model.Header.ExheaderHash;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.AdditionalHeaderSize"/>
        public uint AdditionalHeaderSize => _model.Header.AdditionalHeaderSize;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.SectorZeroOffset"/>
        public uint SectorZeroOffset => _model.Header.SectorZeroOffset;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionFlags"/>
        public byte[] PartitionFlags => _model.Header.PartitionFlags;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionIdTable"/>
        public ulong[] PartitionIdTable => _model.Header.PartitionIdTable;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Reserved1"/>
        public byte[] Reserved1 => _model.Header.Reserved1;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Reserved2"/>
        public byte[] Reserved2 => _model.Header.Reserved2;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.FirmUpdateByte1"/>
        public byte FirmUpdateByte1 => _model.Header.FirmUpdateByte1;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.FirmUpdateByte2"/>
        public byte FirmUpdateByte2 => _model.Header.FirmUpdateByte2;

        #endregion

        #region Raw NAND Format Specific

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Unknown"/>
        public byte[] Unknown => _model.Header.Unknown;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.EncryptedMBR"/>
        public byte[] EncryptedMBR => _model.Header.EncryptedMBR;

        #endregion

        #endregion

        #region Card Info Header

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.WritableAddressMediaUnits"/>
        public uint CIH_WritableAddressMediaUnits => _model.CardInfoHeader.WritableAddressMediaUnits;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CardInfoBitmask"/>
        public uint CIH_CardInfoBitmask => _model.CardInfoHeader.CardInfoBitmask;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved1"/>
        public byte[] CIH_Reserved1 => _model.CardInfoHeader.Reserved1;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.FilledSize"/>
        public uint CIH_FilledSize => _model.CardInfoHeader.FilledSize;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved2"/>
        public byte[] CIH_Reserved2 => _model.CardInfoHeader.Reserved2;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.TitleVersion"/>
        public ushort CIH_TitleVersion => _model.CardInfoHeader.TitleVersion;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CardRevision"/>
        public ushort CIH_CardRevision => _model.CardInfoHeader.CardRevision;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved3"/>
        public byte[] CIH_Reserved3 => _model.CardInfoHeader.Reserved3;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CVerTitleID"/>
        public byte[] CIH_CVerTitleID => _model.CardInfoHeader.CVerTitleID;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CVerVersionNumber"/>
        public ushort CIH_CVerVersionNumber => _model.CardInfoHeader.CVerVersionNumber;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved4"/>
        public byte[] CIH_Reserved4 => _model.CardInfoHeader.Reserved4;

        #endregion

        #region Development Card Info Header

        #region Initial Data

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedKeyY"/>
        public byte[] DCIH_ID_CardSeedKeyY => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedKeyY;

        /// <inheritdoc cref="Models.N3DS.InitialData.EncryptedCardSeed"/>
        public byte[] DCIH_ID_EncryptedCardSeed => _model.DevelopmentCardInfoHeader?.InitialData?.EncryptedCardSeed;

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedAESMAC"/>
        public byte[] DCIH_ID_CardSeedAESMAC => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedAESMAC;

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedNonce"/>
        public byte[] DCIH_ID_CardSeedNonce => _model.DevelopmentCardInfoHeader?.InitialData?.CardSeedNonce;

        /// <inheritdoc cref="Models.N3DS.InitialData.Reserved3"/>
        public byte[] DCIH_ID_Reserved => _model.DevelopmentCardInfoHeader?.InitialData?.Reserved;

        /// <inheritdoc cref="Models.N3DS.InitialData.BackupHeader"/>
        public SabreTools.Models.N3DS.NCCHHeader DCIH_ID_BackupHeader => _model.DevelopmentCardInfoHeader?.InitialData?.BackupHeader;

        #endregion

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.CardDeviceReserved1"/>
        public byte[] DCIH_CardDeviceReserved1 => _model.DevelopmentCardInfoHeader?.CardDeviceReserved1;

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.TitleKey"/>
        public byte[] DCIH_TitleKey => _model.DevelopmentCardInfoHeader?.TitleKey;

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.CardDeviceReserved2"/>
        public byte[] DCIH_CardDeviceReserved2 => _model.DevelopmentCardInfoHeader?.CardDeviceReserved2;

        #region Test Data

        /// <inheritdoc cref="Models.N3DS.TestData.Signature"/>
        public byte[] DCIH_TD_Signature => _model.DevelopmentCardInfoHeader?.TestData?.Signature;

        /// <inheritdoc cref="Models.N3DS.TestData.AscendingByteSequence"/>
        public byte[] DCIH_TD_AscendingByteSequence => _model.DevelopmentCardInfoHeader?.TestData?.AscendingByteSequence;

        /// <inheritdoc cref="Models.N3DS.TestData.DescendingByteSequence"/>
        public byte[] DCIH_TD_DescendingByteSequence => _model.DevelopmentCardInfoHeader?.TestData?.DescendingByteSequence;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled00"/>
        public byte[] DCIH_TD_Filled00 => _model.DevelopmentCardInfoHeader?.TestData?.Filled00;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledFF"/>
        public byte[] DCIH_TD_FilledFF => _model.DevelopmentCardInfoHeader?.TestData?.FilledFF;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled0F"/>
        public byte[] DCIH_TD_Filled0F => _model.DevelopmentCardInfoHeader?.TestData?.Filled0F;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledF0"/>
        public byte[] DCIH_TD_FilledF0 => _model.DevelopmentCardInfoHeader?.TestData?.FilledF0;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled55"/>
        public byte[] DCIH_TD_Filled55 => _model.DevelopmentCardInfoHeader?.TestData?.Filled55;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledAA"/>
        public byte[] DCIH_TD_FilledAA => _model.DevelopmentCardInfoHeader?.TestData?.FilledAA;

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
        public static N3DS Create(byte[] data, int offset)
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
        public static N3DS Create(Stream data)
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

            builder.AppendLine("3DS Cart Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintNCSDHeader(builder);
            PrintCardInfoHeader(builder);
            PrintDevelopmentCardInfoHeader(builder);
            PrintPartitions(builder);
            PrintExtendedHeaders(builder);
            PrintExeFSHeaders(builder);
            PrintRomFSHeaders(builder);

            return builder;
        }

        /// <summary>
        /// Print NCSD header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintNCSDHeader(StringBuilder builder)
        {
            builder.AppendLine("  NCSD Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  RSA-2048 SHA-256 signature: {BitConverter.ToString(RSA2048Signature).Replace('-', ' ')}");
            builder.AppendLine($"  Magic number: {MagicNumber} (0x{MagicNumber:X})");
            builder.AppendLine($"  Image size in media units: {ImageSizeInMediaUnits} (0x{ImageSizeInMediaUnits:X})");
            builder.AppendLine($"  Media ID: {BitConverter.ToString(MediaId).Replace('-', ' ')}");
            builder.AppendLine($"  Partitions filesystem type: {PartitionsFSType} (0x{PartitionsFSType:X})");
            builder.AppendLine($"  Partitions crypt type: {BitConverter.ToString(PartitionsCryptType).Replace('-', ' ')}");
            builder.AppendLine();

            builder.AppendLine($"  Partition table:");
            builder.AppendLine("  -------------------------");
            for (int i = 0; i < PartitionsTable.Length; i++)
            {
                var partitionTableEntry = PartitionsTable[i];
                builder.AppendLine($"  Partition table entry {i}");
                builder.AppendLine($"    Offset: {partitionTableEntry.Offset} (0x{partitionTableEntry.Offset:X})");
                builder.AppendLine($"    Length: {partitionTableEntry.Length} (0x{partitionTableEntry.Length:X})");
            }
            builder.AppendLine();

            // If we have a cart image
            if (PartitionsFSType == SabreTools.Models.N3DS.FilesystemType.Normal || PartitionsFSType == SabreTools.Models.N3DS.FilesystemType.None)
            {
                builder.AppendLine($"  Exheader SHA-256 hash: {BitConverter.ToString(ExheaderHash).Replace('-', ' ')}");
                builder.AppendLine($"  Additional header size: {AdditionalHeaderSize} (0x{AdditionalHeaderSize:X})");
                builder.AppendLine($"  Sector zero offset: {SectorZeroOffset} (0x{SectorZeroOffset:X})");
                builder.AppendLine($"  Partition flags: {BitConverter.ToString(PartitionFlags).Replace('-', ' ')}");
                builder.AppendLine();

                builder.AppendLine($"  Partition ID table:");
                builder.AppendLine("  -------------------------");
                for (int i = 0; i < PartitionIdTable.Length; i++)
                {
                    builder.AppendLine($"  Partition {i} ID: {PartitionIdTable[i]} (0x{PartitionIdTable[i]:X})");
                }
                builder.AppendLine();

                builder.AppendLine($"  Reserved 1: {BitConverter.ToString(Reserved1).Replace('-', ' ')}");
                builder.AppendLine($"  Reserved 2: {BitConverter.ToString(Reserved2).Replace('-', ' ')}");
                builder.AppendLine($"  Firmware update byte 1: {FirmUpdateByte1} (0x{FirmUpdateByte1:X})");
                builder.AppendLine($"  Firmware update byte 2: {FirmUpdateByte2} (0x{FirmUpdateByte2:X})");
            }

            // If we have a firmware image
            else if (PartitionsFSType == SabreTools.Models.N3DS.FilesystemType.FIRM)
            {
                builder.AppendLine($"  Unknown: {BitConverter.ToString(Unknown).Replace('-', ' ')}");
                builder.AppendLine($"  Encrypted MBR: {BitConverter.ToString(EncryptedMBR).Replace('-', ' ')}");
            }

            builder.AppendLine();
        }

        /// <summary>
        /// Print card info header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintCardInfoHeader(StringBuilder builder)
        {
            builder.AppendLine("  Card Info Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Writable address in media units: {CIH_WritableAddressMediaUnits} (0x{CIH_WritableAddressMediaUnits:X})");
            builder.AppendLine($"  Card info bitmask: {CIH_CardInfoBitmask} (0x{CIH_CardInfoBitmask:X})");
            builder.AppendLine($"  Reserved 1: {BitConverter.ToString(CIH_Reserved1).Replace('-', ' ')}");
            builder.AppendLine($"  Filled size of cartridge: {CIH_FilledSize} (0x{CIH_FilledSize:X})");
            builder.AppendLine($"  Reserved 2: {BitConverter.ToString(CIH_Reserved2).Replace('-', ' ')}");
            builder.AppendLine($"  Title version: {CIH_TitleVersion} (0x{CIH_TitleVersion:X})");
            builder.AppendLine($"  Card revision: {CIH_CardRevision} (0x{CIH_CardRevision:X})");
            builder.AppendLine($"  Reserved 3: {BitConverter.ToString(CIH_Reserved3).Replace('-', ' ')}");
            builder.AppendLine($"  Title ID of CVer in included update partition: {BitConverter.ToString(CIH_CVerTitleID).Replace('-', ' ')}");
            builder.AppendLine($"  Version number of CVer in included update partition: {CIH_CVerVersionNumber} (0x{CIH_CVerVersionNumber:X})");
            builder.AppendLine($"  Reserved 4: {BitConverter.ToString(CIH_Reserved4).Replace('-', ' ')}");
            builder.AppendLine();
        }

        /// <summary>
        /// Print development card info header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDevelopmentCardInfoHeader(StringBuilder builder)
        {
            builder.AppendLine("  Development Card Info Header Information:");
            builder.AppendLine("  -------------------------");
            if (_model.DevelopmentCardInfoHeader == null)
            {
                builder.AppendLine("  No development card info header");
            }
            else
            {
                builder.AppendLine();
                builder.AppendLine("  Initial Data:");
                builder.AppendLine("  -------------------------");
                builder.AppendLine($"  Card seed keyY: {BitConverter.ToString(DCIH_ID_CardSeedKeyY).Replace('-', ' ')}");
                builder.AppendLine($"  Encrypted card seed: {BitConverter.ToString(DCIH_ID_EncryptedCardSeed).Replace('-', ' ')}");
                builder.AppendLine($"  Card seed AES-MAC: {BitConverter.ToString(DCIH_ID_CardSeedAESMAC).Replace('-', ' ')}");
                builder.AppendLine($"  Card seed nonce: {BitConverter.ToString(DCIH_ID_CardSeedNonce).Replace('-', ' ')}");
                builder.AppendLine($"  Reserved: {BitConverter.ToString(DCIH_ID_Reserved).Replace('-', ' ')}");
                builder.AppendLine();

                builder.AppendLine("    Backup Header:");
                builder.AppendLine("    -------------------------");
                builder.AppendLine($"    Magic ID: {DCIH_ID_BackupHeader.MagicID} (0x{DCIH_ID_BackupHeader.MagicID:X})");
                builder.AppendLine($"    Content size in media units: {DCIH_ID_BackupHeader.ContentSizeInMediaUnits} (0x{DCIH_ID_BackupHeader.ContentSizeInMediaUnits:X})");
                builder.AppendLine($"    Partition ID: {DCIH_ID_BackupHeader.PartitionId} (0x{DCIH_ID_BackupHeader.PartitionId:X})");
                builder.AppendLine($"    Maker code: {DCIH_ID_BackupHeader.MakerCode} (0x{DCIH_ID_BackupHeader.MakerCode:X})");
                builder.AppendLine($"    Version: {DCIH_ID_BackupHeader.Version} (0x{DCIH_ID_BackupHeader.Version:X})");
                builder.AppendLine($"    Verification hash: {DCIH_ID_BackupHeader.VerificationHash} (0x{DCIH_ID_BackupHeader.VerificationHash:X})");
                builder.AppendLine($"    Program ID: {BitConverter.ToString(DCIH_ID_BackupHeader.ProgramId).Replace('-', ' ')}");
                builder.AppendLine($"    Reserved 1: {BitConverter.ToString(DCIH_ID_BackupHeader.Reserved1).Replace('-', ' ')}");
                builder.AppendLine($"    Logo region SHA-256 hash: {BitConverter.ToString(DCIH_ID_BackupHeader.LogoRegionHash).Replace('-', ' ')}");
                builder.AppendLine($"    Product code: {DCIH_ID_BackupHeader.ProductCode} (0x{DCIH_ID_BackupHeader.ProductCode:X})");
                builder.AppendLine($"    Extended header SHA-256 hash: {BitConverter.ToString(DCIH_ID_BackupHeader.ExtendedHeaderHash).Replace('-', ' ')}");
                builder.AppendLine($"    Extended header size in bytes: {DCIH_ID_BackupHeader.ExtendedHeaderSizeInBytes} (0x{DCIH_ID_BackupHeader.ExtendedHeaderSizeInBytes:X})");
                builder.AppendLine($"    Reserved 2: {BitConverter.ToString(DCIH_ID_BackupHeader.Reserved2).Replace('-', ' ')}");
                builder.AppendLine($"    Flags: {DCIH_ID_BackupHeader.Flags} (0x{DCIH_ID_BackupHeader.Flags:X})");
                builder.AppendLine($"    Plain region offset, in media units: {DCIH_ID_BackupHeader.PlainRegionOffsetInMediaUnits} (0x{DCIH_ID_BackupHeader.PlainRegionOffsetInMediaUnits:X})");
                builder.AppendLine($"    Plain region size, in media units: {DCIH_ID_BackupHeader.PlainRegionSizeInMediaUnits} (0x{DCIH_ID_BackupHeader.PlainRegionSizeInMediaUnits:X})");
                builder.AppendLine($"    Logo region offset, in media units: {DCIH_ID_BackupHeader.LogoRegionOffsetInMediaUnits} (0x{DCIH_ID_BackupHeader.LogoRegionOffsetInMediaUnits:X})");
                builder.AppendLine($"    Logo region size, in media units: {DCIH_ID_BackupHeader.LogoRegionSizeInMediaUnits} (0x{DCIH_ID_BackupHeader.LogoRegionSizeInMediaUnits:X})");
                builder.AppendLine($"    ExeFS offset, in media units: {DCIH_ID_BackupHeader.ExeFSOffsetInMediaUnits} (0x{DCIH_ID_BackupHeader.ExeFSOffsetInMediaUnits:X})");
                builder.AppendLine($"    ExeFS size, in media units: {DCIH_ID_BackupHeader.ExeFSSizeInMediaUnits} (0x{DCIH_ID_BackupHeader.ExeFSSizeInMediaUnits:X})");
                builder.AppendLine($"    ExeFS hash region size, in media units: {DCIH_ID_BackupHeader.ExeFSHashRegionSizeInMediaUnits} (0x{DCIH_ID_BackupHeader.ExeFSHashRegionSizeInMediaUnits:X})");
                builder.AppendLine($"    Reserved 3: {BitConverter.ToString(DCIH_ID_BackupHeader.Reserved3).Replace('-', ' ')}");
                builder.AppendLine($"    RomFS offset, in media units: {DCIH_ID_BackupHeader.RomFSOffsetInMediaUnits} (0x{DCIH_ID_BackupHeader.RomFSOffsetInMediaUnits:X})");
                builder.AppendLine($"    RomFS size, in media units: {DCIH_ID_BackupHeader.RomFSSizeInMediaUnits} (0x{DCIH_ID_BackupHeader.RomFSSizeInMediaUnits:X})");
                builder.AppendLine($"    RomFS hash region size, in media units: {DCIH_ID_BackupHeader.RomFSHashRegionSizeInMediaUnits} (0x{DCIH_ID_BackupHeader.RomFSHashRegionSizeInMediaUnits:X})");
                builder.AppendLine($"    Reserved 4: {BitConverter.ToString(DCIH_ID_BackupHeader.Reserved4).Replace('-', ' ')}");
                builder.AppendLine($"    ExeFS superblock SHA-256 hash: {BitConverter.ToString(DCIH_ID_BackupHeader.ExeFSSuperblockHash).Replace('-', ' ')}");
                builder.AppendLine($"    RomFS superblock SHA-256 hash: {BitConverter.ToString(DCIH_ID_BackupHeader.RomFSSuperblockHash).Replace('-', ' ')}");
                builder.AppendLine();

                builder.AppendLine($"  Card device reserved 1: {BitConverter.ToString(DCIH_CardDeviceReserved1).Replace('-', ' ')}");
                builder.AppendLine($"  Title key: {BitConverter.ToString(DCIH_TitleKey).Replace('-', ' ')}");
                builder.AppendLine($"  Card device reserved 2: {BitConverter.ToString(DCIH_CardDeviceReserved2).Replace('-', ' ')}");
                builder.AppendLine();

                builder.AppendLine("  Test Data:");
                builder.AppendLine("  -------------------------");
                builder.AppendLine($"  Signature: {BitConverter.ToString(DCIH_TD_Signature).Replace('-', ' ')}");
                builder.AppendLine($"  Ascending byte sequence: {BitConverter.ToString(DCIH_TD_AscendingByteSequence).Replace('-', ' ')}");
                builder.AppendLine($"  Descending byte sequence: {BitConverter.ToString(DCIH_TD_DescendingByteSequence).Replace('-', ' ')}");
                builder.AppendLine($"  Filled with 00: {BitConverter.ToString(DCIH_TD_Filled00).Replace('-', ' ')}");
                builder.AppendLine($"  Filled with FF: {BitConverter.ToString(DCIH_TD_FilledFF).Replace('-', ' ')}");
                builder.AppendLine($"  Filled with 0F: {BitConverter.ToString(DCIH_TD_Filled0F).Replace('-', ' ')}");
                builder.AppendLine($"  Filled with F0: {BitConverter.ToString(DCIH_TD_FilledF0).Replace('-', ' ')}");
                builder.AppendLine($"  Filled with 55: {BitConverter.ToString(DCIH_TD_Filled55).Replace('-', ' ')}");
                builder.AppendLine($"  Filled with AA: {BitConverter.ToString(DCIH_TD_FilledAA).Replace('-', ' ')}");
                builder.AppendLine($"  Final byte: {DCIH_TD_FinalByte}");
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
        /// Print NCCH extended header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintExtendedHeaders(StringBuilder builder)
        {
            builder.AppendLine("  NCCH Extended Header Information:");
            builder.AppendLine("  -------------------------");
            if (ExtendedHeaders == null || ExtendedHeaders.Length == 0)
            {
                builder.AppendLine("  No NCCH extended headers");
            }
            else
            {
                for (int i = 0; i < ExtendedHeaders.Length; i++)
                {
                    var extendedHeader = ExtendedHeaders[i];
                    builder.AppendLine($"  NCCH Extended Header {i}");
                    if (extendedHeader == null)
                    {
                        builder.AppendLine($"    Unrecognized partition data, no data can be parsed");
                    }
                    else
                    {
                        builder.AppendLine($"    System control info:");
                        builder.AppendLine($"      Application title: {extendedHeader.SCI.ApplicationTitle}");
                        builder.AppendLine($"      Reserved 1: {BitConverter.ToString(extendedHeader.SCI.Reserved1).Replace('-', ' ')}");
                        builder.AppendLine($"      Flag: {extendedHeader.SCI.Flag} (0x{extendedHeader.SCI.Flag:X})");
                        builder.AppendLine($"      Remaster version: {extendedHeader.SCI.RemasterVersion} (0x{extendedHeader.SCI.RemasterVersion:X})");

                        builder.AppendLine($"      Text code set info:");
                        builder.AppendLine($"        Address: {extendedHeader.SCI.TextCodeSetInfo.Address} (0x{extendedHeader.SCI.TextCodeSetInfo.Address:X})");
                        builder.AppendLine($"        Physical region size (in page-multiples): {extendedHeader.SCI.TextCodeSetInfo.PhysicalRegionSizeInPages} (0x{extendedHeader.SCI.TextCodeSetInfo.PhysicalRegionSizeInPages:X})");
                        builder.AppendLine($"        Size (in bytes): {extendedHeader.SCI.TextCodeSetInfo.SizeInBytes} (0x{extendedHeader.SCI.TextCodeSetInfo.SizeInBytes:X})");

                        builder.AppendLine($"      Stack size: {extendedHeader.SCI.StackSize} (0x{extendedHeader.SCI.StackSize:X})");

                        builder.AppendLine($"      Read-only code set info:");
                        builder.AppendLine($"        Address: {extendedHeader.SCI.ReadOnlyCodeSetInfo.Address} (0x{extendedHeader.SCI.ReadOnlyCodeSetInfo.Address:X})");
                        builder.AppendLine($"        Physical region size (in page-multiples): {extendedHeader.SCI.ReadOnlyCodeSetInfo.PhysicalRegionSizeInPages} (0x{extendedHeader.SCI.ReadOnlyCodeSetInfo.PhysicalRegionSizeInPages:X})");
                        builder.AppendLine($"        Size (in bytes): {extendedHeader.SCI.ReadOnlyCodeSetInfo.SizeInBytes} (0x{extendedHeader.SCI.ReadOnlyCodeSetInfo.SizeInBytes:X})");

                        builder.AppendLine($"      Reserved 2: {BitConverter.ToString(extendedHeader.SCI.Reserved2).Replace('-', newChar: ' ')}");

                        builder.AppendLine($"      Data code set info:");
                        builder.AppendLine($"        Address: {extendedHeader.SCI.DataCodeSetInfo.Address} (0x{extendedHeader.SCI.DataCodeSetInfo.Address:X})");
                        builder.AppendLine($"        Physical region size (in page-multiples): {extendedHeader.SCI.DataCodeSetInfo.PhysicalRegionSizeInPages} (0x{extendedHeader.SCI.DataCodeSetInfo.PhysicalRegionSizeInPages:X})");
                        builder.AppendLine($"        Size (in bytes): {extendedHeader.SCI.DataCodeSetInfo.SizeInBytes} (0x{extendedHeader.SCI.DataCodeSetInfo.SizeInBytes:X})");

                        builder.AppendLine($"      BSS size: {extendedHeader.SCI.BSSSize} (0x{extendedHeader.SCI.BSSSize:X})");
                        builder.AppendLine($"      Dependency module list: {string.Join(", ", extendedHeader.SCI.DependencyModuleList)}");

                        builder.AppendLine($"      System info:");
                        builder.AppendLine($"        SaveData size: {extendedHeader.SCI.SystemInfo.SaveDataSize} (0x{extendedHeader.SCI.SystemInfo.SaveDataSize:X})");
                        builder.AppendLine($"        Jump ID: {extendedHeader.SCI.SystemInfo.JumpID} (0x{extendedHeader.SCI.SystemInfo.JumpID:X})");
                        builder.AppendLine($"        Reserved: {BitConverter.ToString(extendedHeader.SCI.SystemInfo.Reserved).Replace('-', newChar: ' ')}");

                        builder.AppendLine($"    Access control info:");
                        builder.AppendLine($"      ARM11 local system capabilities:");
                        builder.AppendLine($"        Program ID: {extendedHeader.ACI.ARM11LocalSystemCapabilities.ProgramID} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.ProgramID:X})");
                        builder.AppendLine($"        Core version: {extendedHeader.ACI.ARM11LocalSystemCapabilities.CoreVersion} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.CoreVersion:X})");
                        builder.AppendLine($"        Flag 1: {extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag1} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag1:X})");
                        builder.AppendLine($"        Flag 2: {extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag2} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag2:X})");
                        builder.AppendLine($"        Flag 0: {extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag0} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag0:X})");
                        builder.AppendLine($"        Priority: {extendedHeader.ACI.ARM11LocalSystemCapabilities.Priority} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.Priority:X})");
                        builder.AppendLine($"        Resource limit descriptors: {string.Join(", ", extendedHeader.ACI.ARM11LocalSystemCapabilities.ResourceLimitDescriptors)}");

                        builder.AppendLine($"        Storage info:");
                        builder.AppendLine($"          Extdata ID: {extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID:X})");
                        builder.AppendLine($"          System savedata IDs: {BitConverter.ToString(extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.SystemSavedataIDs).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"          Storage accessible unique IDs: {BitConverter.ToString(extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.StorageAccessibleUniqueIDs).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"          File system access info: {BitConverter.ToString(extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.FileSystemAccessInfo).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"          Other attributes: {extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes:X})");

                        builder.AppendLine($"        Service access control: {string.Join(", ", extendedHeader.ACI.ARM11LocalSystemCapabilities.ServiceAccessControl)}");
                        builder.AppendLine($"        Extended service access control: {string.Join(", ", extendedHeader.ACI.ARM11LocalSystemCapabilities.ExtendedServiceAccessControl)}");
                        builder.AppendLine($"        Reserved: {BitConverter.ToString(extendedHeader.ACI.ARM11LocalSystemCapabilities.Reserved).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"        Resource limit cateogry: {extendedHeader.ACI.ARM11LocalSystemCapabilities.ResourceLimitCategory} (0x{extendedHeader.ACI.ARM11LocalSystemCapabilities.ResourceLimitCategory:X})");

                        builder.AppendLine($"      ARM11 kernel capabilities:");
                        builder.AppendLine($"        Descriptors: {string.Join(", ", extendedHeader.ACI.ARM11KernelCapabilities.Descriptors)}");
                        builder.AppendLine($"        Reserved: {BitConverter.ToString(extendedHeader.ACI.ARM11KernelCapabilities.Reserved).Replace('-', newChar: ' ')}");

                        builder.AppendLine($"      ARM9 access control:");
                        builder.AppendLine($"        Descriptors: {BitConverter.ToString(extendedHeader.ACI.ARM9AccessControl.Descriptors).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"        Descriptor version: {extendedHeader.ACI.ARM9AccessControl.DescriptorVersion} (0x{extendedHeader.ACI.ARM9AccessControl.DescriptorVersion:X})");

                        builder.AppendLine($"    AccessDec signature (RSA-2048-SHA256): {BitConverter.ToString(extendedHeader.AccessDescSignature).Replace('-', ' ')}");
                        builder.AppendLine($"    NCCH HDR RSA-2048 public key: {BitConverter.ToString(extendedHeader.NCCHHDRPublicKey).Replace('-', ' ')}");


                        builder.AppendLine($"    Access control info (for limitations of first ACI):");
                        builder.AppendLine($"      ARM11 local system capabilities:");
                        builder.AppendLine($"        Program ID: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ProgramID} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ProgramID:X})");
                        builder.AppendLine($"        Core version: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.CoreVersion} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.CoreVersion:X})");
                        builder.AppendLine($"        Flag 1: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag1} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag1:X})");
                        builder.AppendLine($"        Flag 2: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag2} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag2:X})");
                        builder.AppendLine($"        Flag 0: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag0} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag0:X})");
                        builder.AppendLine($"        Priority: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Priority} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Priority:X})");
                        builder.AppendLine($"        Resource limit descriptors: {string.Join(", ", extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitDescriptors)}");

                        builder.AppendLine($"        Storage info:");
                        builder.AppendLine($"          Extdata ID: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID:X})");
                        builder.AppendLine($"          System savedata IDs: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.SystemSavedataIDs).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"          Storage accessible unique IDs: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.StorageAccessibleUniqueIDs).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"          File system access info: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.FileSystemAccessInfo).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"          Other attributes: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes:X})");

                        builder.AppendLine($"        Service access control: {string.Join(", ", extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ServiceAccessControl)}");
                        builder.AppendLine($"        Extended service access control: {string.Join(", ", extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ExtendedServiceAccessControl)}");
                        builder.AppendLine($"        Reserved: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Reserved).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"        Resource limit cateogry: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitCategory} (0x{extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitCategory:X})");

                        builder.AppendLine($"      ARM11 kernel capabilities:");
                        builder.AppendLine($"        Descriptors: {string.Join(", ", extendedHeader.ACIForLimitations.ARM11KernelCapabilities.Descriptors)}");
                        builder.AppendLine($"        Reserved: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11KernelCapabilities.Reserved).Replace('-', newChar: ' ')}");

                        builder.AppendLine($"      ARM9 access control:");
                        builder.AppendLine($"        Descriptors: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM9AccessControl.Descriptors).Replace('-', newChar: ' ')}");
                        builder.AppendLine($"        Descriptor version: {extendedHeader.ACIForLimitations.ARM9AccessControl.DescriptorVersion} (0x{extendedHeader.ACIForLimitations.ARM9AccessControl.DescriptorVersion:X})");
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print ExeFS header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintExeFSHeaders(StringBuilder builder)
        {
            builder.AppendLine("  ExeFS Header Information:");
            builder.AppendLine("  -------------------------");
            if (ExeFSHeaders == null || ExeFSHeaders.Length == 0)
            {
                builder.AppendLine("  No ExeFS headers");
            }
            else
            {
                for (int i = 0; i < ExeFSHeaders.Length; i++)
                {
                    var exeFSHeader = ExeFSHeaders[i];
                    builder.AppendLine($"  ExeFS Header {i}");
                    if (exeFSHeader == null)
                    {
                        builder.AppendLine($"    Unrecognized partition data, no data can be parsed");
                    }
                    else
                    {
                        builder.AppendLine($"    File headers:");
                        for (int j = 0; j < exeFSHeader.FileHeaders.Length; j++)
                        {
                            var fileHeader = exeFSHeader.FileHeaders[j];
                            builder.AppendLine(value: $"    File Header {j}");
                            builder.AppendLine(value: $"      File name: {fileHeader.FileName}");
                            builder.AppendLine(value: $"      File offset: {fileHeader.FileOffset} (0x{fileHeader.FileOffset:X})");
                            builder.AppendLine(value: $"      File size: {fileHeader.FileSize} (0x{fileHeader.FileSize:X})");
                        }

                        builder.AppendLine(value: $"    Reserved: {BitConverter.ToString(exeFSHeader.Reserved).Replace('-', ' ')}");
                        
                        builder.AppendLine($"    File hashes:");
                        for (int j = 0; j < exeFSHeader.FileHashes.Length; j++)
                        {
                            var fileHash = exeFSHeader.FileHashes[j];
                            builder.AppendLine(value: $"    File Hash {j}");
                            builder.AppendLine(value: $"      SHA-256: {BitConverter.ToString(fileHash).Replace('-', ' ')}");
                        }
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print RomFS header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintRomFSHeaders(StringBuilder builder)
        {
            builder.AppendLine("  RomFS Header Information:");
            builder.AppendLine("  -------------------------");
            if (RomFSHeaders == null || RomFSHeaders.Length == 0)
            {
                builder.AppendLine("  No RomFS headers");
            }
            else
            {
                for (int i = 0; i < RomFSHeaders.Length; i++)
                {
                    var romFSHeader = RomFSHeaders[i];
                    builder.AppendLine($"  RomFS Header {i}");
                    if (romFSHeader == null)
                    {
                        builder.AppendLine($"    Unrecognized RomFS data, no data can be parsed");
                    }
                    else
                    {
                        builder.AppendLine(value: $"    Magic string: {romFSHeader.MagicString}");
                        builder.AppendLine(value: $"    Magic number: {romFSHeader.MagicNumber} (0x{romFSHeader.MagicNumber:X})");
                        builder.AppendLine(value: $"    Master hash size: {romFSHeader.MasterHashSize} (0x{romFSHeader.MasterHashSize:X})");
                        builder.AppendLine(value: $"    Level 1 logical offset: {romFSHeader.Level1LogicalOffset} (0x{romFSHeader.Level1LogicalOffset:X})");
                        builder.AppendLine(value: $"    Level 1 hashdata size: {romFSHeader.Level1HashdataSize} (0x{romFSHeader.Level1HashdataSize:X})");
                        builder.AppendLine(value: $"    Level 1 block size: {romFSHeader.Level1BlockSizeLog2} (0x{romFSHeader.Level1BlockSizeLog2:X})");
                        builder.AppendLine(value: $"    Reserved 1: {BitConverter.ToString(romFSHeader.Reserved1).Replace('-', ' ')}");
                        builder.AppendLine(value: $"    Level 2 logical offset: {romFSHeader.Level2LogicalOffset} (0x{romFSHeader.Level2LogicalOffset:X})");
                        builder.AppendLine(value: $"    Level 2 hashdata size: {romFSHeader.Level2HashdataSize} (0x{romFSHeader.Level2HashdataSize:X})");
                        builder.AppendLine(value: $"    Level 2 block size: {romFSHeader.Level2BlockSizeLog2} (0x{romFSHeader.Level2BlockSizeLog2:X})");
                        builder.AppendLine(value: $"    Reserved 2: {BitConverter.ToString(romFSHeader.Reserved2).Replace('-', ' ')}");
                        builder.AppendLine(value: $"    Level 3 logical offset: {romFSHeader.Level3LogicalOffset} (0x{romFSHeader.Level3LogicalOffset:X})");
                        builder.AppendLine(value: $"    Level 3 hashdata size: {romFSHeader.Level3HashdataSize} (0x{romFSHeader.Level3HashdataSize:X})");
                        builder.AppendLine(value: $"    Level 3 block size: {romFSHeader.Level3BlockSizeLog2} (0x{romFSHeader.Level3BlockSizeLog2:X})");
                        builder.AppendLine(value: $"    Reserved 3: {BitConverter.ToString(romFSHeader.Reserved3).Replace('-', ' ')}");
                        builder.AppendLine(value: $"    Reserved 4: {BitConverter.ToString(romFSHeader.Reserved4).Replace('-', ' ')}");
                        builder.AppendLine(value: $"    Optional info size: {romFSHeader.OptionalInfoSize} (0x{romFSHeader.OptionalInfoSize:X})");
                    }
                }
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