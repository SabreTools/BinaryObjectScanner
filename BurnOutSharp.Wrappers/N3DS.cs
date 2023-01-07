using System;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class N3DS : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        #region Common to all NCSD files

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.RSA2048Signature"/>
        public byte[] RSA2048Signature => _cart.Header.RSA2048Signature;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.MagicNumber"/>
        public string MagicNumber => _cart.Header.MagicNumber;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.ImageSizeInMediaUnits"/>
        public uint ImageSizeInMediaUnits => _cart.Header.ImageSizeInMediaUnits;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.MediaId"/>
        public byte[] MediaId => _cart.Header.MediaId;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsFSType"/>
        public Models.N3DS.FilesystemType PartitionsFSType => _cart.Header.PartitionsFSType;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsCryptType"/>
        public byte[] PartitionsCryptType => _cart.Header.PartitionsCryptType;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsTable"/>
        public Models.N3DS.PartitionTableEntry[] PartitionsTable => _cart.Header.PartitionsTable;

        #endregion

        #region CTR Cart Image (CCI) Specific

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.ExheaderHash"/>
        public byte[] ExheaderHash => _cart.Header.ExheaderHash;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.AdditionalHeaderSize"/>
        public uint AdditionalHeaderSize => _cart.Header.AdditionalHeaderSize;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.SectorZeroOffset"/>
        public uint SectorZeroOffset => _cart.Header.SectorZeroOffset;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionFlags"/>
        public byte[] PartitionFlags => _cart.Header.PartitionFlags;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionIdTable"/>
        public ulong[] PartitionIdTable => _cart.Header.PartitionIdTable;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Reserved1"/>
        public byte[] Reserved1 => _cart.Header.Reserved1;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Reserved2"/>
        public byte[] Reserved2 => _cart.Header.Reserved2;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.FirmUpdateByte1"/>
        public byte FirmUpdateByte1 => _cart.Header.FirmUpdateByte1;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.FirmUpdateByte2"/>
        public byte FirmUpdateByte2 => _cart.Header.FirmUpdateByte2;

        #endregion

        #region Raw NAND Format Specific

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Unknown"/>
        public byte[] Unknown => _cart.Header.Unknown;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.EncryptedMBR"/>
        public byte[] EncryptedMBR => _cart.Header.EncryptedMBR;

        #endregion

        #endregion

        #region Card Info Header

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.WritableAddressMediaUnits"/>
        public uint CIH_WritableAddressMediaUnits => _cart.CardInfoHeader.WritableAddressMediaUnits;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CardInfoBitmask"/>
        public uint CIH_CardInfoBitmask => _cart.CardInfoHeader.CardInfoBitmask;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved1"/>
        public byte[] CIH_Reserved1 => _cart.CardInfoHeader.Reserved1;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.FilledSize"/>
        public uint CIH_FilledSize => _cart.CardInfoHeader.FilledSize;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved2"/>
        public byte[] CIH_Reserved2 => _cart.CardInfoHeader.Reserved2;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.TitleVersion"/>
        public ushort CIH_TitleVersion => _cart.CardInfoHeader.TitleVersion;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CardRevision"/>
        public ushort CIH_CardRevision => _cart.CardInfoHeader.CardRevision;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved3"/>
        public byte[] CIH_Reserved3 => _cart.CardInfoHeader.Reserved3;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CVerTitleID"/>
        public byte[] CIH_CVerTitleID => _cart.CardInfoHeader.CVerTitleID;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CVerVersionNumber"/>
        public ushort CIH_CVerVersionNumber => _cart.CardInfoHeader.CVerVersionNumber;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved4"/>
        public byte[] CIH_Reserved4 => _cart.CardInfoHeader.Reserved4;

        #endregion

        #region Development Card Info Header

        #region Initial Data

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedKeyY"/>
        public byte[] DCIH_ID_CardSeedKeyY => _cart.DevelopmentCardInfoHeader?.InitialData?.CardSeedKeyY;

        /// <inheritdoc cref="Models.N3DS.InitialData.EncryptedCardSeed"/>
        public byte[] DCIH_ID_EncryptedCardSeed => _cart.DevelopmentCardInfoHeader?.InitialData?.EncryptedCardSeed;

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedAESMAC"/>
        public byte[] DCIH_ID_CardSeedAESMAC => _cart.DevelopmentCardInfoHeader?.InitialData?.CardSeedAESMAC;

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedNonce"/>
        public byte[] DCIH_ID_CardSeedNonce => _cart.DevelopmentCardInfoHeader?.InitialData?.CardSeedNonce;

        /// <inheritdoc cref="Models.N3DS.InitialData.Reserved3"/>
        public byte[] DCIH_ID_Reserved => _cart.DevelopmentCardInfoHeader?.InitialData?.Reserved;

        /// <inheritdoc cref="Models.N3DS.InitialData.BackupHeader"/>
        public Models.N3DS.NCCHHeader DCIH_ID_BackupHeader => _cart.DevelopmentCardInfoHeader?.InitialData?.BackupHeader;

        #endregion

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.CardDeviceReserved1"/>
        public byte[] DCIH_CardDeviceReserved1 => _cart.DevelopmentCardInfoHeader?.CardDeviceReserved1;

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.TitleKey"/>
        public byte[] DCIH_TitleKey => _cart.DevelopmentCardInfoHeader?.TitleKey;

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.CardDeviceReserved2"/>
        public byte[] DCIH_CardDeviceReserved2 => _cart.DevelopmentCardInfoHeader?.CardDeviceReserved2;

        #region Test Data

        /// <inheritdoc cref="Models.N3DS.TestData.Signature"/>
        public byte[] DCIH_TD_Signature => _cart.DevelopmentCardInfoHeader?.TestData?.Signature;

        /// <inheritdoc cref="Models.N3DS.TestData.AscendingByteSequence"/>
        public byte[] DCIH_TD_AscendingByteSequence => _cart.DevelopmentCardInfoHeader?.TestData?.AscendingByteSequence;

        /// <inheritdoc cref="Models.N3DS.TestData.DescendingByteSequence"/>
        public byte[] DCIH_TD_DescendingByteSequence => _cart.DevelopmentCardInfoHeader?.TestData?.DescendingByteSequence;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled00"/>
        public byte[] DCIH_TD_Filled00 => _cart.DevelopmentCardInfoHeader?.TestData?.Filled00;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledFF"/>
        public byte[] DCIH_TD_FilledFF => _cart.DevelopmentCardInfoHeader?.TestData?.FilledFF;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled0F"/>
        public byte[] DCIH_TD_Filled0F => _cart.DevelopmentCardInfoHeader?.TestData?.Filled0F;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledF0"/>
        public byte[] DCIH_TD_FilledF0 => _cart.DevelopmentCardInfoHeader?.TestData?.FilledF0;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled55"/>
        public byte[] DCIH_TD_Filled55 => _cart.DevelopmentCardInfoHeader?.TestData?.Filled55;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledAA"/>
        public byte[] DCIH_TD_FilledAA => _cart.DevelopmentCardInfoHeader?.TestData?.FilledAA;

        /// <inheritdoc cref="Models.N3DS.TestData.FinalByte"/>
        public byte? DCIH_TD_FinalByte => _cart.DevelopmentCardInfoHeader?.TestData?.FinalByte;

        #endregion

        #endregion

        #region Partitions

        /// <inheritdoc cref="Models.N3DS.Cart.Partitions"/>
        public Models.N3DS.NCCHHeader[] Partitions => _cart.Partitions;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the cart
        /// </summary>
        private Models.N3DS.Cart _cart;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private N3DS() { }

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

            var archive = Builders.N3DS.ParseCart(data);
            if (archive == null)
                return null;

            var wrapper = new N3DS
            {
                _cart = archive,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("3DS Cart Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintNCSDHeader();
            PrintCardInfoHeader();
            PrintDevelopmentCardInfoHeader();
            PrintPartitions();
        }

        /// <summary>
        /// Print NCSD header information
        /// </summary>
        private void PrintNCSDHeader()
        {
            Console.WriteLine("  NCSD Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  RSA-2048 SHA-256 signature: {BitConverter.ToString(RSA2048Signature).Replace('-', ' ')}");
            Console.WriteLine($"  Magic number: {MagicNumber}");
            Console.WriteLine($"  Image size in media units: {ImageSizeInMediaUnits}");
            Console.WriteLine($"  Media ID: {BitConverter.ToString(MediaId).Replace('-', ' ')}");
            Console.WriteLine($"  Partitions filesystem type: {PartitionsFSType}");
            Console.WriteLine($"  Partitions crypt type: {BitConverter.ToString(PartitionsCryptType).Replace('-', ' ')}");
            Console.WriteLine();

            Console.WriteLine($"  Partition table:");
            Console.WriteLine("  -------------------------");
            for (int i = 0; i < PartitionsTable.Length; i++)
            {
                var partitionTableEntry = PartitionsTable[i];
                Console.WriteLine($"  Partition table entry {i}");
                Console.WriteLine($"    Offset: {partitionTableEntry.Offset}");
                Console.WriteLine($"    Length: {partitionTableEntry.Length}");
            }
            Console.WriteLine();

            // If we have a cart image
            if (PartitionsFSType == Models.N3DS.FilesystemType.Normal || PartitionsFSType == Models.N3DS.FilesystemType.None)
            {
                Console.WriteLine($"  Exheader SHA-256 hash: {BitConverter.ToString(ExheaderHash).Replace('-', ' ')}");
                Console.WriteLine($"  Additional header size: {AdditionalHeaderSize}");
                Console.WriteLine($"  Sector zero offset: {SectorZeroOffset}");
                Console.WriteLine($"  Partition flags: {BitConverter.ToString(PartitionFlags).Replace('-', ' ')}");
                Console.WriteLine();

                Console.WriteLine($"  Partition ID table:");
                Console.WriteLine("  -------------------------");
                for (int i = 0; i < PartitionIdTable.Length; i++)
                {
                    Console.WriteLine($"  Partition {i} ID: {PartitionIdTable[i]}");
                }
                Console.WriteLine();

                Console.WriteLine($"  Reserved 1: {BitConverter.ToString(Reserved1).Replace('-', ' ')}");
                Console.WriteLine($"  Reserved 2: {BitConverter.ToString(Reserved2).Replace('-', ' ')}");
                Console.WriteLine($"  Firmware update byte 1: {FirmUpdateByte1}");
                Console.WriteLine($"  Firmware update byte 2: {FirmUpdateByte2}");
            }

            // If we have a firmware image
            else if (PartitionsFSType == Models.N3DS.FilesystemType.FIRM)
            {
                Console.WriteLine($"  Unknown: {BitConverter.ToString(Unknown).Replace('-', ' ')}");
                Console.WriteLine($"  Encrypted MBR: {BitConverter.ToString(EncryptedMBR).Replace('-', ' ')}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print card info header information
        /// </summary>
        private void PrintCardInfoHeader()
        {
            Console.WriteLine("  Card Info Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Writable address in media units: {CIH_WritableAddressMediaUnits}");
            Console.WriteLine($"  Card info bitmask: {CIH_CardInfoBitmask}");
            Console.WriteLine($"  Reserved 1: {BitConverter.ToString(CIH_Reserved1).Replace('-', ' ')}");
            Console.WriteLine($"  Filled size of cartridge: {CIH_FilledSize}");
            Console.WriteLine($"  Reserved 2: {BitConverter.ToString(CIH_Reserved2).Replace('-', ' ')}");
            Console.WriteLine($"  Title version: {CIH_TitleVersion}");
            Console.WriteLine($"  Card revision: {CIH_CardRevision}");
            Console.WriteLine($"  Reserved 3: {BitConverter.ToString(CIH_Reserved3).Replace('-', ' ')}");
            Console.WriteLine($"  Title ID of CVer in included update partition: {BitConverter.ToString(CIH_CVerTitleID).Replace('-', ' ')}");
            Console.WriteLine($"  Version number of CVer in included update partition: {CIH_CVerVersionNumber}");
            Console.WriteLine($"  Reserved 4: {BitConverter.ToString(CIH_Reserved4).Replace('-', ' ')}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print development card info header information
        /// </summary>
        private void PrintDevelopmentCardInfoHeader()
        {
            Console.WriteLine("  Development Card Info Header Information:");
            Console.WriteLine("  -------------------------");
            if (_cart.DevelopmentCardInfoHeader == null)
            {
                Console.WriteLine("  No development card info header");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("  Initial Data:");
                Console.WriteLine("  -------------------------");
                Console.WriteLine($"  Card seed keyY: {BitConverter.ToString(DCIH_ID_CardSeedKeyY).Replace('-', ' ')}");
                Console.WriteLine($"  Encrypted card seed: {BitConverter.ToString(DCIH_ID_EncryptedCardSeed).Replace('-', ' ')}");
                Console.WriteLine($"  Card seed AES-MAC: {BitConverter.ToString(DCIH_ID_CardSeedAESMAC).Replace('-', ' ')}");
                Console.WriteLine($"  Card seed nonce: {BitConverter.ToString(DCIH_ID_CardSeedNonce).Replace('-', ' ')}");
                Console.WriteLine($"  Reserved: {BitConverter.ToString(DCIH_ID_Reserved).Replace('-', ' ')}");
                // TODO: Print Backup Header?
                Console.WriteLine();

                Console.WriteLine($"  Card device reserved 1: {BitConverter.ToString(DCIH_CardDeviceReserved1).Replace('-', ' ')}");
                Console.WriteLine($"  Title key: {BitConverter.ToString(DCIH_TitleKey).Replace('-', ' ')}");
                Console.WriteLine($"  Card device reserved 2: {BitConverter.ToString(DCIH_CardDeviceReserved2).Replace('-', ' ')}");
                Console.WriteLine();

                Console.WriteLine("  Test Data:");
                Console.WriteLine("  -------------------------");
                Console.WriteLine($"  Signature: {BitConverter.ToString(DCIH_TD_Signature).Replace('-', ' ')}");
                Console.WriteLine($"  Ascending byte sequence: {BitConverter.ToString(DCIH_TD_AscendingByteSequence).Replace('-', ' ')}");
                Console.WriteLine($"  Descending byte sequence: {BitConverter.ToString(DCIH_TD_DescendingByteSequence).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with 00: {BitConverter.ToString(DCIH_TD_Filled00).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with FF: {BitConverter.ToString(DCIH_TD_FilledFF).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with 0F: {BitConverter.ToString(DCIH_TD_Filled0F).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with F0: {BitConverter.ToString(DCIH_TD_FilledF0).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with 55: {BitConverter.ToString(DCIH_TD_Filled55).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with AA: {BitConverter.ToString(DCIH_TD_FilledAA).Replace('-', ' ')}");
                Console.WriteLine($"  Final byte: {DCIH_TD_FinalByte}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print NCCH partition header information
        /// </summary>
        private void PrintPartitions()
        {
            Console.WriteLine("  NCCH Partition Header Information:");
            Console.WriteLine("  -------------------------");
            if (Partitions == null || Partitions.Length == 0)
            {
                Console.WriteLine("  No NCCH partition headers");
            }
            else
            {
                for (int i = 0; i < Partitions.Length; i++)
                {
                    var partitionHeader = Partitions[i];
                    Console.WriteLine($"  NCCH Partition Header {i}");
                    Console.WriteLine($"    RSA-2048 SHA-256 signature: {BitConverter.ToString(partitionHeader.RSA2048Signature).Replace('-', ' ')}");
                    Console.WriteLine($"    Magic ID: {partitionHeader.MagicID}");
                    Console.WriteLine($"    Content size in media units: {partitionHeader.ContentSizeInMediaUnits}");
                    Console.WriteLine($"    Partition ID: {partitionHeader.PartitionId}");
                    Console.WriteLine($"    Maker code: {partitionHeader.MakerCode}");
                    Console.WriteLine($"    Version: {partitionHeader.Version}");
                    Console.WriteLine($"    Verification hash: {partitionHeader.VerificationHash}");
                    Console.WriteLine($"    Program ID: {BitConverter.ToString(partitionHeader.ProgramId).Replace('-', ' ')}");
                    Console.WriteLine($"    Reserved 1: {BitConverter.ToString(partitionHeader.Reserved1).Replace('-', ' ')}");
                    Console.WriteLine($"    Logo region SHA-256 hash: {BitConverter.ToString(partitionHeader.LogoRegionHash).Replace('-', ' ')}");
                    Console.WriteLine($"    Product code: {BitConverter.ToString(partitionHeader.ProductCode).Replace('-', ' ')}");
                    Console.WriteLine($"    Extended header SHA-256 hash: {BitConverter.ToString(partitionHeader.ExtendedHeaderHash).Replace('-', ' ')}");
                    Console.WriteLine($"    Extended header size in bytes: {partitionHeader.ExtendedHeaderSizeInBytes}");
                    Console.WriteLine($"    Reserved 2: {BitConverter.ToString(partitionHeader.Reserved2).Replace('-', ' ')}");
                    Console.WriteLine($"    Flags: {partitionHeader.Flags}");
                    Console.WriteLine($"    Plain region offset, in media units: {partitionHeader.PlainRegionOffsetInMediaUnits}");
                    Console.WriteLine($"    Plain region size, in media units: {partitionHeader.PlainRegionSizeInMediaUnits}");
                    Console.WriteLine($"    Logo region offset, in media units: {partitionHeader.LogoRegionOffsetInMediaUnits}");
                    Console.WriteLine($"    Logo region size, in media units: {partitionHeader.LogoRegionSizeInMediaUnits}");
                    Console.WriteLine($"    ExeFS offset, in media units: {partitionHeader.ExeFSOffsetInMediaUnits}");
                    Console.WriteLine($"    ExeFS size, in media units: {partitionHeader.ExeFSSizeInMediaUnits}");
                    Console.WriteLine($"    ExeFS hash region size, in media units: {partitionHeader.ExeFSHashRegionSizeInMediaUnits}");
                    Console.WriteLine($"    Reserved 3: {BitConverter.ToString(partitionHeader.Reserved3).Replace('-', ' ')}");
                    Console.WriteLine($"    RomFS offset, in media units: {partitionHeader.RomFSOffsetInMediaUnits}");
                    Console.WriteLine($"    RomFS size, in media units: {partitionHeader.RomFSSizeInMediaUnits}");
                    Console.WriteLine($"    RomFS hash region size, in media units: {partitionHeader.RomFSHashRegionSizeInMediaUnits}");
                    Console.WriteLine($"    Reserved 4: {BitConverter.ToString(partitionHeader.Reserved4).Replace('-', ' ')}");
                    Console.WriteLine($"    ExeFS superblock SHA-256 hash: {BitConverter.ToString(partitionHeader.ExeFSSuperblockHash).Replace('-', ' ')}");
                    Console.WriteLine($"    RomFS superblock SHA-256 hash: {BitConverter.ToString(partitionHeader.RomFSSuperblockHash).Replace('-', ' ')}");
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}