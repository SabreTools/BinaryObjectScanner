using System;
using System.IO;
using System.Text;
using BurnOutSharp.Models.N3DS;
using BinaryObjectScanner.Utilities;
using static BurnOutSharp.Models.N3DS.Constants;

namespace BurnOutSharp.Builders
{
    public class N3DS
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a 3DS cart image
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled cart image on success, null on error</returns>
        public static Cart ParseCart(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseCart(dataStream);
        }

        /// <summary>
        /// Parse a byte array into a CIA archive
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled CIA archive on success, null on error</returns>
        public static CIA ParseCIA(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseCIA(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a 3DS cart image
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled cart image on success, null on error</returns>
        public static Cart ParseCart(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new cart image to fill
            var cart = new Cart();

            #region NCSD Header

            // Try to parse the header
            var header = ParseNCSDHeader(data);
            if (header == null)
                return null;

            // Set the cart image header
            cart.Header = header;

            #endregion

            #region Card Info Header

            // Try to parse the card info header
            var cardInfoHeader = ParseCardInfoHeader(data);
            if (cardInfoHeader == null)
                return null;

            // Set the card info header
            cart.CardInfoHeader = cardInfoHeader;

            #endregion

            #region Development Card Info Header

            // Try to parse the development card info header
            var developmentCardInfoHeader = ParseDevelopmentCardInfoHeader(data);
            if (developmentCardInfoHeader == null)
                return null;

            // Set the development card info header
            cart.DevelopmentCardInfoHeader = developmentCardInfoHeader;

            #endregion

            #region Partitions

            // Create the partition table
            cart.Partitions = new NCCHHeader[8];

            // Iterate and build the partitions
            for (int i = 0; i < 8; i++)
            {
                cart.Partitions[i] = ParseNCCHHeader(data);
            }

            #endregion

            // Cache the media unit size for further use
            long mediaUnitSize = (uint)(0x200 * Math.Pow(2, header.PartitionFlags[(int)NCSDFlags.MediaUnitSize]));

            #region Extended Headers

            // Create the extended header table
            cart.ExtendedHeaders = new NCCHExtendedHeader[8];

            // Iterate and build the extended headers
            for (int i = 0; i < 8; i++)
            {
                // If we have an encrypted or invalid partition
                if (cart.Partitions[i].MagicID != NCCHMagicNumber)
                    continue;

                // Get the extended header offset
                long offset = (cart.Header.PartitionsTable[i].Offset * mediaUnitSize) + 0x200;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the extended header
                data.Seek(offset, SeekOrigin.Begin);

                // Parse the extended header
                cart.ExtendedHeaders[i] = ParseNCCHExtendedHeader(data);
            }

            #endregion

            #region ExeFS Headers

            // Create the ExeFS header table
            cart.ExeFSHeaders = new ExeFSHeader[8];

            // Iterate and build the ExeFS headers
            for (int i = 0; i < 8; i++)
            {
                // If we have an encrypted or invalid partition
                if (cart.Partitions[i].MagicID != NCCHMagicNumber)
                    continue;

                // Get the ExeFS header offset
                long offset = (cart.Header.PartitionsTable[i].Offset + cart.Partitions[i].ExeFSOffsetInMediaUnits) * mediaUnitSize;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the ExeFS header
                data.Seek(offset, SeekOrigin.Begin);

                // Parse the ExeFS header
                cart.ExeFSHeaders[i] = ParseExeFSHeader(data);
            }

            #endregion

            #region RomFS Headers

            // Create the RomFS header table
            cart.RomFSHeaders = new RomFSHeader[8];

            // Iterate and build the RomFS headers
            for (int i = 0; i < 8; i++)
            {
                // If we have an encrypted or invalid partition
                if (cart.Partitions[i].MagicID != NCCHMagicNumber)
                    continue;

                // Get the RomFS header offset
                long offset = (cart.Header.PartitionsTable[i].Offset + cart.Partitions[i].RomFSOffsetInMediaUnits) * mediaUnitSize;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the RomFS header
                data.Seek(offset, SeekOrigin.Begin);

                // Parse the RomFS header
                cart.RomFSHeaders[i] = ParseRomFSHeader(data);
            }

            #endregion

            return cart;
        }

        /// <summary>
        /// Parse a Stream into a CIA archive
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CIA archive on success, null on error</returns>
        public static CIA ParseCIA(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new CIA archive to fill
            var cia = new CIA();

            #region CIA Header

            // Try to parse the header
            var header = ParseCIAHeader(data);
            if (header == null)
                return null;

            // Set the CIA archive header
            cia.Header = header;

            #endregion

            // Align to 64-byte boundary, if needed
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

            #region Certificate Chain

            // Create the certificate chain
            cia.CertificateChain = new Certificate[3];

            // Try to parse the certificates
            for (int i = 0; i < 3; i++)
            {
                var certificate = ParseCertificate(data);
                if (certificate == null)
                    return null;

                cia.CertificateChain[i] = certificate;
            }

            #endregion

            // Align to 64-byte boundary, if needed
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

            #region Ticket

            // Try to parse the ticket
            var ticket = ParseTicket(data);
            if (ticket == null)
                return null;

            // Set the ticket
            cia.Ticket = ticket;

            #endregion

            // Align to 64-byte boundary, if needed
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

            #region Title Metadata

            // Try to parse the title metadata
            var titleMetadata = ParseTitleMetadata(data);
            if (titleMetadata == null)
                return null;

            // Set the title metadata
            cia.TMDFileData = titleMetadata;

            #endregion

            // Align to 64-byte boundary, if needed
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

            #region Content File Data

            // Create the partition table
            cia.Partitions = new NCCHHeader[8];

            // Iterate and build the partitions
            for (int i = 0; i < 8; i++)
            {
                cia.Partitions[i] = ParseNCCHHeader(data);
            }

            #endregion

            // Align to 64-byte boundary, if needed
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

            #region Meta Data

            // If we have a meta data
            if (header.MetaSize > 0)
            {
                // Try to parse the meta
                var meta = ParseMetaData(data);
                if (meta == null)
                    return null;

                // Set the meta
                cia.MetaData = meta;
            }

            #endregion

            return cia;
        }

        /// <summary>
        /// Parse a Stream into an NCSD header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCSD header on success, null on error</returns>
        private static NCSDHeader ParseNCSDHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            NCSDHeader header = new NCSDHeader();

            header.RSA2048Signature = data.ReadBytes(0x100);
            byte[] magicNumber = data.ReadBytes(4);
            header.MagicNumber = Encoding.ASCII.GetString(magicNumber).TrimEnd('\0'); ;
            if (header.MagicNumber != NCSDMagicNumber)
                return null;

            header.ImageSizeInMediaUnits = data.ReadUInt32();
            header.MediaId = data.ReadBytes(8);
            header.PartitionsFSType = (FilesystemType)data.ReadUInt64();
            header.PartitionsCryptType = data.ReadBytes(8);

            header.PartitionsTable = new PartitionTableEntry[8];
            for (int i = 0; i < 8; i++)
            {
                header.PartitionsTable[i] = ParsePartitionTableEntry(data);
            }

            if (header.PartitionsFSType == FilesystemType.Normal || header.PartitionsFSType == FilesystemType.None)
            {
                header.ExheaderHash = data.ReadBytes(0x20);
                header.AdditionalHeaderSize = data.ReadUInt32();
                header.SectorZeroOffset = data.ReadUInt32();
                header.PartitionFlags = data.ReadBytes(8);

                header.PartitionIdTable = new ulong[8];
                for (int i = 0; i < 8; i++)
                {
                    header.PartitionIdTable[i] = data.ReadUInt64();
                }

                header.Reserved1 = data.ReadBytes(0x20);
                header.Reserved2 = data.ReadBytes(0x0E);
                header.FirmUpdateByte1 = data.ReadByteValue();
                header.FirmUpdateByte2 = data.ReadByteValue();
            }
            else if (header.PartitionsFSType == FilesystemType.FIRM)
            {
                header.Unknown = data.ReadBytes(0x5E);
                header.EncryptedMBR = data.ReadBytes(0x42);
            }

            return header;
        }

        /// <summary>
        /// Parse a Stream into a partition table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled partition table entry on success, null on error</returns>
        private static PartitionTableEntry ParsePartitionTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            PartitionTableEntry partitionTableEntry = new PartitionTableEntry();

            partitionTableEntry.Offset = data.ReadUInt32();
            partitionTableEntry.Length = data.ReadUInt32();

            return partitionTableEntry;
        }

        /// <summary>
        /// Parse a Stream into a card info header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled card info header on success, null on error</returns>
        private static CardInfoHeader ParseCardInfoHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            CardInfoHeader cardInfoHeader = new CardInfoHeader();

            cardInfoHeader.WritableAddressMediaUnits = data.ReadUInt32();
            cardInfoHeader.CardInfoBitmask = data.ReadUInt32();
            cardInfoHeader.Reserved1 = data.ReadBytes(0xF8);
            cardInfoHeader.FilledSize = data.ReadUInt32();
            cardInfoHeader.Reserved2 = data.ReadBytes(0x0C);
            cardInfoHeader.TitleVersion = data.ReadUInt16();
            cardInfoHeader.CardRevision = data.ReadUInt16();
            cardInfoHeader.Reserved3 = data.ReadBytes(0x0C);
            cardInfoHeader.CVerTitleID = data.ReadBytes(8);
            cardInfoHeader.CVerVersionNumber = data.ReadUInt16();
            cardInfoHeader.Reserved4 = data.ReadBytes(0xCD6);

            return cardInfoHeader;
        }

        /// <summary>
        /// Parse a Stream into a development card info header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled development card info header on success, null on error</returns>
        private static DevelopmentCardInfoHeader ParseDevelopmentCardInfoHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            DevelopmentCardInfoHeader developmentCardInfoHeader = new DevelopmentCardInfoHeader();

            developmentCardInfoHeader.InitialData = ParseInitialData(data);
            if (developmentCardInfoHeader.InitialData == null)
                return null;

            developmentCardInfoHeader.CardDeviceReserved1 = data.ReadBytes(0x200);
            developmentCardInfoHeader.TitleKey = data.ReadBytes(0x10);
            developmentCardInfoHeader.CardDeviceReserved2 = data.ReadBytes(0x1BF0);

            developmentCardInfoHeader.TestData = ParseTestData(data);
            if (developmentCardInfoHeader.TestData == null)
                return null;

            return developmentCardInfoHeader;
        }

        /// <summary>
        /// Parse a Stream into an initial data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled initial data on success, null on error</returns>
        private static InitialData ParseInitialData(Stream data)
        {
            // TODO: Use marshalling here instead of building
            InitialData initialData = new InitialData();

            initialData.CardSeedKeyY = data.ReadBytes(0x10);
            initialData.EncryptedCardSeed = data.ReadBytes(0x10);
            initialData.CardSeedAESMAC = data.ReadBytes(0x10);
            initialData.CardSeedNonce = data.ReadBytes(0xC);
            initialData.Reserved = data.ReadBytes(0xC4);
            initialData.BackupHeader = ParseNCCHHeader(data, true);
            if (initialData.BackupHeader == null)
                return null;

            return initialData;
        }

        /// <summary>
        /// Parse a Stream into an NCCH header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="skipSignature">Indicates if the signature should be skipped</param>
        /// <returns>Filled NCCH header on success, null on error</returns>
        private static NCCHHeader ParseNCCHHeader(Stream data, bool skipSignature = false)
        {
            // TODO: Use marshalling here instead of building
            NCCHHeader header = new NCCHHeader();

            if (!skipSignature)
                header.RSA2048Signature = data.ReadBytes(0x100);

            byte[] magicId = data.ReadBytes(4);
            header.MagicID = Encoding.ASCII.GetString(magicId).TrimEnd('\0');
            header.ContentSizeInMediaUnits = data.ReadUInt32();
            header.PartitionId = data.ReadUInt64();
            header.MakerCode = data.ReadUInt16();
            header.Version = data.ReadUInt16();
            header.VerificationHash = data.ReadUInt32();
            header.ProgramId = data.ReadBytes(8);
            header.Reserved1 = data.ReadBytes(0x10);
            header.LogoRegionHash = data.ReadBytes(0x20);
            byte[] productCode = data.ReadBytes(0x10);
            header.ProductCode = Encoding.ASCII.GetString(productCode).TrimEnd('\0');
            header.ExtendedHeaderHash = data.ReadBytes(0x20);
            header.ExtendedHeaderSizeInBytes = data.ReadUInt32();
            header.Reserved2 = data.ReadBytes(4);
            header.Flags = ParseNCCHHeaderFlags(data);
            header.PlainRegionOffsetInMediaUnits = data.ReadUInt32();
            header.PlainRegionSizeInMediaUnits = data.ReadUInt32();
            header.LogoRegionOffsetInMediaUnits = data.ReadUInt32();
            header.LogoRegionSizeInMediaUnits = data.ReadUInt32();
            header.ExeFSOffsetInMediaUnits = data.ReadUInt32();
            header.ExeFSSizeInMediaUnits = data.ReadUInt32();
            header.ExeFSHashRegionSizeInMediaUnits = data.ReadUInt32();
            header.Reserved3 = data.ReadBytes(4);
            header.RomFSOffsetInMediaUnits = data.ReadUInt32();
            header.RomFSSizeInMediaUnits = data.ReadUInt32();
            header.RomFSHashRegionSizeInMediaUnits = data.ReadUInt32();
            header.Reserved4 = data.ReadBytes(4);
            header.ExeFSSuperblockHash = data.ReadBytes(0x20);
            header.RomFSSuperblockHash = data.ReadBytes(0x20);

            return header;
        }

        /// <summary>
        /// Parse a Stream into an NCCH header flags
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCCH header flags on success, null on error</returns>
        private static NCCHHeaderFlags ParseNCCHHeaderFlags(Stream data)
        {
            // TODO: Use marshalling here instead of building
            NCCHHeaderFlags headerFlags = new NCCHHeaderFlags();

            headerFlags.Reserved0 = data.ReadByteValue();
            headerFlags.Reserved1 = data.ReadByteValue();
            headerFlags.Reserved2 = data.ReadByteValue();
            headerFlags.CryptoMethod = (CryptoMethod)data.ReadByteValue();
            headerFlags.ContentPlatform = (ContentPlatform)data.ReadByteValue();
            headerFlags.MediaPlatformIndex = (ContentType)data.ReadByteValue();
            headerFlags.ContentUnitSize = data.ReadByteValue();
            headerFlags.BitMasks = (BitMasks)data.ReadByteValue();

            return headerFlags;
        }

        /// <summary>
        /// Parse a Stream into an initial data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled initial data on success, null on error</returns>
        private static TestData ParseTestData(Stream data)
        {
            // TODO: Use marshalling here instead of building
            TestData testData = new TestData();

            // TODO: Validate some of the values
            testData.Signature = data.ReadBytes(8);
            testData.AscendingByteSequence = data.ReadBytes(0x1F8);
            testData.DescendingByteSequence = data.ReadBytes(0x200);
            testData.Filled00 = data.ReadBytes(0x200);
            testData.FilledFF = data.ReadBytes(0x200);
            testData.Filled0F = data.ReadBytes(0x200);
            testData.FilledF0 = data.ReadBytes(0x200);
            testData.Filled55 = data.ReadBytes(0x200);
            testData.FilledAA = data.ReadBytes(0x1FF);
            testData.FinalByte = data.ReadByteValue();

            return testData;
        }

        /// <summary>
        /// Parse a Stream into an NCCH extended header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCCH extended header on success, null on error</returns>
        private static NCCHExtendedHeader ParseNCCHExtendedHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            NCCHExtendedHeader extendedHeader = new NCCHExtendedHeader();

            extendedHeader.SCI = ParseSystemControlInfo(data);
            if (extendedHeader.SCI == null)
                return null;

            extendedHeader.ACI = ParseAccessControlInfo(data);
            if (extendedHeader.ACI == null)
                return null;

            extendedHeader.AccessDescSignature = data.ReadBytes(0x100);
            extendedHeader.NCCHHDRPublicKey = data.ReadBytes(0x100);

            extendedHeader.ACIForLimitations = ParseAccessControlInfo(data);
            if (extendedHeader.ACI == null)
                return null;

            return extendedHeader;
        }

        /// <summary>
        /// Parse a Stream into a system control info
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled system control info on success, null on error</returns>
        private static SystemControlInfo ParseSystemControlInfo(Stream data)
        {
            // TODO: Use marshalling here instead of building
            SystemControlInfo systemControlInfo = new SystemControlInfo();

            byte[] applicationTitle = data.ReadBytes(8);
            systemControlInfo.ApplicationTitle = Encoding.ASCII.GetString(applicationTitle).TrimEnd('\0');
            systemControlInfo.Reserved1 = data.ReadBytes(5);
            systemControlInfo.Flag = data.ReadByteValue();
            systemControlInfo.RemasterVersion = data.ReadUInt16();
            systemControlInfo.TextCodeSetInfo = ParseCodeSetInfo(data);
            systemControlInfo.StackSize = data.ReadUInt32();
            systemControlInfo.ReadOnlyCodeSetInfo = ParseCodeSetInfo(data);
            systemControlInfo.Reserved2 = data.ReadBytes(4);
            systemControlInfo.DataCodeSetInfo = ParseCodeSetInfo(data);
            systemControlInfo.BSSSize = data.ReadUInt32();
            systemControlInfo.DependencyModuleList = new ulong[48];
            for (int i = 0; i < 48; i++)
            {
                systemControlInfo.DependencyModuleList[i] = data.ReadUInt64();
            }
            systemControlInfo.SystemInfo = ParseSystemInfo(data);

            return systemControlInfo;
        }

        /// <summary>
        /// Parse a Stream into a code set info
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled code set info on success, null on error</returns>
        private static CodeSetInfo ParseCodeSetInfo(Stream data)
        {
            // TODO: Use marshalling here instead of building
            CodeSetInfo codeSetInfo = new CodeSetInfo();

            codeSetInfo.Address = data.ReadUInt32();
            codeSetInfo.PhysicalRegionSizeInPages = data.ReadUInt32();
            codeSetInfo.SizeInBytes = data.ReadUInt32();

            return codeSetInfo;
        }

        /// <summary>
        /// Parse a Stream into a system info
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled system info on success, null on error</returns>
        private static SystemInfo ParseSystemInfo(Stream data)
        {
            // TODO: Use marshalling here instead of building
            SystemInfo systemInfo = new SystemInfo();

            systemInfo.SaveDataSize = data.ReadUInt64();
            systemInfo.JumpID = data.ReadUInt64();
            systemInfo.Reserved = data.ReadBytes(0x30);

            return systemInfo;
        }

        /// <summary>
        /// Parse a Stream into an access control info
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled access control info on success, null on error</returns>
        private static AccessControlInfo ParseAccessControlInfo(Stream data)
        {
            // TODO: Use marshalling here instead of building
            AccessControlInfo accessControlInfo = new AccessControlInfo();

            accessControlInfo.ARM11LocalSystemCapabilities = ParseARM11LocalSystemCapabilities(data);
            accessControlInfo.ARM11KernelCapabilities = ParseARM11KernelCapabilities(data);
            accessControlInfo.ARM9AccessControl = ParseARM9AccessControl(data);

            return accessControlInfo;
        }

        /// <summary>
        /// Parse a Stream into an ARM11 local system capabilities
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ARM11 local system capabilities on success, null on error</returns>
        private static ARM11LocalSystemCapabilities ParseARM11LocalSystemCapabilities(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ARM11LocalSystemCapabilities arm11LocalSystemCapabilities = new ARM11LocalSystemCapabilities();

            arm11LocalSystemCapabilities.ProgramID = data.ReadUInt64();
            arm11LocalSystemCapabilities.CoreVersion = data.ReadUInt32();
            arm11LocalSystemCapabilities.Flag1 = (ARM11LSCFlag1)data.ReadByteValue();
            arm11LocalSystemCapabilities.Flag2 = (ARM11LSCFlag2)data.ReadByteValue();
            arm11LocalSystemCapabilities.Flag0 = (ARM11LSCFlag0)data.ReadByteValue();
            arm11LocalSystemCapabilities.Priority = data.ReadByteValue();
            arm11LocalSystemCapabilities.ResourceLimitDescriptors = new ushort[16];
            for (int i = 0; i < 16; i++)
            {
                arm11LocalSystemCapabilities.ResourceLimitDescriptors[i] = data.ReadUInt16();
            }
            arm11LocalSystemCapabilities.StorageInfo = ParseStorageInfo(data);
            arm11LocalSystemCapabilities.ServiceAccessControl = new ulong[32];
            for (int i = 0; i < 32; i++)
            {
                arm11LocalSystemCapabilities.ServiceAccessControl[i] = data.ReadUInt64();
            }
            arm11LocalSystemCapabilities.ExtendedServiceAccessControl = new ulong[2];
            for (int i = 0; i < 2; i++)
            {
                arm11LocalSystemCapabilities.ExtendedServiceAccessControl[i] = data.ReadUInt64();
            }
            arm11LocalSystemCapabilities.Reserved = data.ReadBytes(0x0F);
            arm11LocalSystemCapabilities.ResourceLimitCategory = (ResourceLimitCategory)data.ReadByteValue();

            return arm11LocalSystemCapabilities;
        }

        /// <summary>
        /// Parse a Stream into a storage info
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled storage info on success, null on error</returns>
        private static StorageInfo ParseStorageInfo(Stream data)
        {
            // TODO: Use marshalling here instead of building
            StorageInfo storageInfo = new StorageInfo();

            storageInfo.ExtdataID = data.ReadUInt64();
            storageInfo.SystemSavedataIDs = data.ReadBytes(8);
            storageInfo.StorageAccessibleUniqueIDs = data.ReadBytes(8);
            storageInfo.FileSystemAccessInfo = data.ReadBytes(7);
            storageInfo.OtherAttributes = (StorageInfoOtherAttributes)data.ReadByteValue();

            return storageInfo;
        }

        /// <summary>
        /// Parse a Stream into an ARM11 kernel capabilities
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ARM11 kernel capabilities on success, null on error</returns>
        private static ARM11KernelCapabilities ParseARM11KernelCapabilities(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ARM11KernelCapabilities arm11KernelCapabilities = new ARM11KernelCapabilities();

            arm11KernelCapabilities.Descriptors = new uint[28];
            for (int i = 0; i < 28; i++)
            {
                arm11KernelCapabilities.Descriptors[i] = data.ReadUInt32();
            }
            arm11KernelCapabilities.Reserved = data.ReadBytes(0x10);

            return arm11KernelCapabilities;
        }

        /// <summary>
        /// Parse a Stream into an ARM11 access control
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ARM11 access control on success, null on error</returns>
        private static ARM9AccessControl ParseARM9AccessControl(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ARM9AccessControl arm9AccessControl = new ARM9AccessControl();

            arm9AccessControl.Descriptors = data.ReadBytes(15);
            arm9AccessControl.DescriptorVersion = data.ReadByteValue();

            return arm9AccessControl;
        }

        /// <summary>
        /// Parse a Stream into an ExeFS header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExeFS header on success, null on error</returns>
        private static ExeFSHeader ParseExeFSHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ExeFSHeader exeFSHeader = new ExeFSHeader();

            exeFSHeader.FileHeaders = new ExeFSFileHeader[10];
            for (int i = 0; i < 10; i++)
            {
                exeFSHeader.FileHeaders[i] = ParseExeFSFileHeader(data);
            }
            exeFSHeader.Reserved = data.ReadBytes(0x20);
            exeFSHeader.FileHashes = new byte[10][];
            for (int i = 0; i < 10; i++)
            {
                exeFSHeader.FileHashes[i] = data.ReadBytes(0x20);
            }

            return exeFSHeader;
        }

        /// <summary>
        /// Parse a Stream into an ExeFS file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExeFS file header on success, null on error</returns>
        private static ExeFSFileHeader ParseExeFSFileHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ExeFSFileHeader exeFSFileHeader = new ExeFSFileHeader();

            byte[] fileName = data.ReadBytes(8);
            exeFSFileHeader.FileName = Encoding.ASCII.GetString(fileName).TrimEnd('\0');
            exeFSFileHeader.FileOffset = data.ReadUInt32();
            exeFSFileHeader.FileSize = data.ReadUInt32();

            return exeFSFileHeader;
        }

        /// <summary>
        /// Parse a Stream into an RomFS header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled RomFS header on success, null on error</returns>
        private static RomFSHeader ParseRomFSHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            RomFSHeader romFSHeader = new RomFSHeader();

            byte[] magicString = data.ReadBytes(4);
            romFSHeader.MagicString = Encoding.ASCII.GetString(magicString).TrimEnd('\0');
            if (romFSHeader.MagicString != RomFSMagicNumber)
                return null;

            romFSHeader.MagicNumber = data.ReadUInt32();
            if (romFSHeader.MagicNumber != RomFSSecondMagicNumber)
                return null;

            romFSHeader.MasterHashSize = data.ReadUInt32();
            romFSHeader.Level1LogicalOffset = data.ReadUInt64();
            romFSHeader.Level1HashdataSize = data.ReadUInt64();
            romFSHeader.Level1BlockSizeLog2 = data.ReadUInt32();
            romFSHeader.Reserved1 = data.ReadBytes(4);
            romFSHeader.Level2LogicalOffset = data.ReadUInt64();
            romFSHeader.Level2HashdataSize = data.ReadUInt64();
            romFSHeader.Level2BlockSizeLog2 = data.ReadUInt32();
            romFSHeader.Reserved2 = data.ReadBytes(4);
            romFSHeader.Level3LogicalOffset = data.ReadUInt64();
            romFSHeader.Level3HashdataSize = data.ReadUInt64();
            romFSHeader.Level3BlockSizeLog2 = data.ReadUInt32();
            romFSHeader.Reserved3 = data.ReadBytes(4);
            romFSHeader.Reserved4 = data.ReadBytes(4);
            romFSHeader.OptionalInfoSize = data.ReadUInt32();

            return romFSHeader;
        }

        /// <summary>
        /// Parse a Stream into a CIA header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CIA header on success, null on error</returns>
        private static CIAHeader ParseCIAHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            CIAHeader ciaHeader = new CIAHeader();

            ciaHeader.HeaderSize = data.ReadUInt32();
            ciaHeader.Type = data.ReadUInt16();
            ciaHeader.Version = data.ReadUInt16();
            ciaHeader.CertificateChainSize = data.ReadUInt32();
            ciaHeader.TicketSize = data.ReadUInt32();
            ciaHeader.TMDFileSize = data.ReadUInt32();
            ciaHeader.MetaSize = data.ReadUInt32();
            ciaHeader.ContentSize = data.ReadUInt64();
            ciaHeader.ContentIndex = data.ReadBytes(0x2000);

            return ciaHeader;
        }

        /// <summary>
        /// Parse a Stream into a certificate
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled certificate on success, null on error</returns>
        private static Certificate ParseCertificate(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Certificate certificate = new Certificate();

            certificate.SignatureType = (SignatureType)data.ReadUInt32();
            switch (certificate.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                    certificate.SignatureSize = 0x200;
                    certificate.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA1:
                    certificate.SignatureSize = 0x100;
                    certificate.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA1:
                    certificate.SignatureSize = 0x3C;
                    certificate.PaddingSize = 0x40;
                    break;
                case SignatureType.RSA_4096_SHA256:
                    certificate.SignatureSize = 0x200;
                    certificate.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA256:
                    certificate.SignatureSize = 0x100;
                    certificate.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA256:
                    certificate.SignatureSize = 0x3C;
                    certificate.PaddingSize = 0x40;
                    break;
                default:
                    return null;
            }

            certificate.Signature = data.ReadBytes(certificate.SignatureSize);
            certificate.Padding = data.ReadBytes(certificate.PaddingSize);
            byte[] issuer = data.ReadBytes(0x40);
            certificate.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            certificate.KeyType = (PublicKeyType)data.ReadUInt32();
            byte[] name = data.ReadBytes(0x40);
            certificate.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            certificate.ExpirationTime = data.ReadUInt32();

            switch (certificate.KeyType)
            {
                case PublicKeyType.RSA_4096:
                    certificate.RSAModulus = data.ReadBytes(0x200);
                    certificate.RSAPublicExponent = data.ReadUInt32();
                    certificate.RSAPadding = data.ReadBytes(0x34);
                    break;
                case PublicKeyType.RSA_2048:
                    certificate.RSAModulus = data.ReadBytes(0x100);
                    certificate.RSAPublicExponent = data.ReadUInt32();
                    certificate.RSAPadding = data.ReadBytes(0x34);
                    break;
                case PublicKeyType.EllipticCurve:
                    certificate.ECCPublicKey = data.ReadBytes(0x3C);
                    certificate.ECCPadding = data.ReadBytes(0x3C);
                    break;
                default:
                    return null;
            }

            return certificate;
        }

        /// <summary>
        /// Parse a Stream into a ticket
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="fromCdn">Indicates if the ticket is from CDN</param>
        /// <returns>Filled ticket on success, null on error</returns>
        private static Ticket ParseTicket(Stream data, bool fromCdn = false)
        {
            // TODO: Use marshalling here instead of building
            Ticket ticket = new Ticket();

            ticket.SignatureType = (SignatureType)data.ReadUInt32();
            switch (ticket.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                    ticket.SignatureSize = 0x200;
                    ticket.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA1:
                    ticket.SignatureSize = 0x100;
                    ticket.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA1:
                    ticket.SignatureSize = 0x3C;
                    ticket.PaddingSize = 0x40;
                    break;
                case SignatureType.RSA_4096_SHA256:
                    ticket.SignatureSize = 0x200;
                    ticket.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA256:
                    ticket.SignatureSize = 0x100;
                    ticket.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA256:
                    ticket.SignatureSize = 0x3C;
                    ticket.PaddingSize = 0x40;
                    break;
                default:
                    return null;
            }

            ticket.Signature = data.ReadBytes(ticket.SignatureSize);
            ticket.Padding = data.ReadBytes(ticket.PaddingSize);
            byte[] issuer = data.ReadBytes(0x40);
            ticket.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            ticket.ECCPublicKey = data.ReadBytes(0x3C);
            ticket.Version = data.ReadByteValue();
            ticket.CaCrlVersion = data.ReadByteValue();
            ticket.SignerCrlVersion = data.ReadByteValue();
            ticket.TitleKey = data.ReadBytes(0x10);
            ticket.Reserved1 = data.ReadByteValue();
            ticket.TicketID = data.ReadUInt64();
            ticket.ConsoleID = data.ReadUInt32();
            ticket.TitleID = data.ReadUInt64();
            ticket.Reserved2 = data.ReadBytes(2);
            ticket.TicketTitleVersion = data.ReadUInt16();
            ticket.Reserved3 = data.ReadBytes(8);
            ticket.LicenseType = data.ReadByteValue();
            ticket.CommonKeyYIndex = data.ReadByteValue();
            ticket.Reserved4 = data.ReadBytes(0x2A);
            ticket.eShopAccountID = data.ReadUInt32();
            ticket.Reserved5 = data.ReadByteValue();
            ticket.Audit = data.ReadByteValue();
            ticket.Reserved6 = data.ReadBytes(0x42);
            ticket.Limits = new uint[0x10];
            for (int i = 0; i < ticket.Limits.Length; i++)
            {
                ticket.Limits[i] = data.ReadUInt32();
            }

            // Seek to the content index size
            data.Seek(4, SeekOrigin.Current);

            // Read the size (big-endian)
            byte[] contentIndexSize = data.ReadBytes(4);
            Array.Reverse(contentIndexSize);
            ticket.ContentIndexSize = BitConverter.ToUInt32(contentIndexSize, 0);

            // Seek back to the start of the content index
            data.Seek(-8, SeekOrigin.Current);

            ticket.ContentIndex = data.ReadBytes((int)ticket.ContentIndexSize);

            // Certificates only exist in standalone CETK files
            if (fromCdn)
            {
                ticket.CertificateChain = new Certificate[2];
                for (int i = 0; i < 2; i++)
                {
                    var certificate = ParseCertificate(data);
                    if (certificate == null)
                        return null;

                    ticket.CertificateChain[i] = certificate;
                }
            }

            return ticket;
        }

        /// <summary>
        /// Parse a Stream into a title metadata
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="fromCdn">Indicates if the ticket is from CDN</param>
        /// <returns>Filled title metadata on success, null on error</returns>
        private static TitleMetadata ParseTitleMetadata(Stream data, bool fromCdn = false)
        {
            // TODO: Use marshalling here instead of building
            TitleMetadata titleMetadata = new TitleMetadata();

            titleMetadata.SignatureType = (SignatureType)data.ReadUInt32();
            switch (titleMetadata.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                    titleMetadata.SignatureSize = 0x200;
                    titleMetadata.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA1:
                    titleMetadata.SignatureSize = 0x100;
                    titleMetadata.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA1:
                    titleMetadata.SignatureSize = 0x3C;
                    titleMetadata.PaddingSize = 0x40;
                    break;
                case SignatureType.RSA_4096_SHA256:
                    titleMetadata.SignatureSize = 0x200;
                    titleMetadata.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA256:
                    titleMetadata.SignatureSize = 0x100;
                    titleMetadata.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA256:
                    titleMetadata.SignatureSize = 0x3C;
                    titleMetadata.PaddingSize = 0x40;
                    break;
                default:
                    return null;
            }

            titleMetadata.Signature = data.ReadBytes(titleMetadata.SignatureSize);
            titleMetadata.Padding1 = data.ReadBytes(titleMetadata.PaddingSize);
            byte[] issuer = data.ReadBytes(0x40);
            titleMetadata.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            titleMetadata.Version = data.ReadByteValue();
            titleMetadata.CaCrlVersion = data.ReadByteValue();
            titleMetadata.SignerCrlVersion = data.ReadByteValue();
            titleMetadata.Reserved1 = data.ReadByteValue();
            titleMetadata.SystemVersion = data.ReadUInt64();
            titleMetadata.TitleID = data.ReadUInt64();
            titleMetadata.TitleType = data.ReadUInt32();
            titleMetadata.GroupID = data.ReadUInt16();
            titleMetadata.SaveDataSize = data.ReadUInt32();
            titleMetadata.SRLPrivateSaveDataSize = data.ReadUInt32();
            titleMetadata.Reserved2 = data.ReadBytes(4);
            titleMetadata.SRLFlag = data.ReadByteValue();
            titleMetadata.Reserved3 = data.ReadBytes(0x31);
            titleMetadata.AccessRights = data.ReadUInt32();
            titleMetadata.TitleVersion = data.ReadUInt16();

            // Read the content count (big-endian)
            byte[] contentCount = data.ReadBytes(2);
            Array.Reverse(contentCount);
            titleMetadata.ContentCount = BitConverter.ToUInt16(contentCount, 0);

            titleMetadata.BootContent = data.ReadUInt16();
            titleMetadata.Padding2 = data.ReadBytes(2);
            titleMetadata.SHA256HashContentInfoRecords = data.ReadBytes(0x20);
            titleMetadata.ContentInfoRecords = new ContentInfoRecord[64];
            for (int i = 0; i < 64; i++)
            {
                titleMetadata.ContentInfoRecords[i] = ParseContentInfoRecord(data);
            }
            titleMetadata.ContentChunkRecords = new ContentChunkRecord[titleMetadata.ContentCount];
            for (int i = 0; i < titleMetadata.ContentCount; i++)
            {
                titleMetadata.ContentChunkRecords[i] = ParseContentChunkRecord(data);
            }

            // Certificates only exist in standalone TMD files
            if (fromCdn)
            {
                titleMetadata.CertificateChain = new Certificate[2];
                for (int i = 0; i < 2; i++)
                {
                    var certificate = ParseCertificate(data);
                    if (certificate == null)
                        return null;

                    titleMetadata.CertificateChain[i] = certificate;
                }
            }

            return titleMetadata;
        }

        /// <summary>
        /// Parse a Stream into a content info record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled content info record on success, null on error</returns>
        private static ContentInfoRecord ParseContentInfoRecord(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ContentInfoRecord contentInfoRecord = new ContentInfoRecord();

            contentInfoRecord.ContentIndexOffset = data.ReadUInt16();
            contentInfoRecord.ContentCommandCount = data.ReadUInt16();
            contentInfoRecord.UnhashedContentRecordsSHA256Hash = data.ReadBytes(0x20);

            return contentInfoRecord;
        }

        /// <summary>
        /// Parse a Stream into a content chunk record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled content chunk record on success, null on error</returns>
        private static ContentChunkRecord ParseContentChunkRecord(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ContentChunkRecord contentChunkRecord = new ContentChunkRecord();

            contentChunkRecord.ContentId = data.ReadUInt32();
            contentChunkRecord.ContentIndex = (ContentIndex)data.ReadUInt16();
            contentChunkRecord.ContentType = (TMDContentType)data.ReadUInt16();
            contentChunkRecord.ContentSize = data.ReadUInt64();
            contentChunkRecord.SHA256Hash = data.ReadBytes(0x20);

            return contentChunkRecord;
        }

        /// <summary>
        /// Parse a Stream into a meta data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled meta data on success, null on error</returns>
        private static MetaData ParseMetaData(Stream data)
        {
            // TODO: Use marshalling here instead of building
            MetaData metaData = new MetaData();

            metaData.TitleIDDependencyList = data.ReadBytes(0x180);
            metaData.Reserved1 = data.ReadBytes(0x180);
            metaData.CoreVersion = data.ReadUInt32();
            metaData.Reserved2 = data.ReadBytes(0xFC);
            metaData.IconData = data.ReadBytes(0x36C0);

            return metaData;
        }

        #endregion
    }
}
