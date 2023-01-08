using System;

namespace BurnOutSharp.Models.N3DS
{
    // TODO: Fix this, I don't think it's correct
    [Flags]
    public enum ARM9AccessControlDescriptors : byte
    {
        MountNandRoot = 0x01,
        MountNandroWriteAccess = 0x02,
        MountTwlnRoot = 0x04,
        MountWnandRoot = 0x08,
        MountCardSPI = 0x0F,
        UseSDIF3 = 0x10,
        CreateSeed = 0x20,
        UseCardSPI = 0x40,
        SDApplication = 0x80,
        MountSdmcWriteAccess = 0xF0,
    }

    [Flags]
    public enum ARM11LSCFlag0 : byte
    {
        IdealProcessor = 0x01 | 0x02,
        
        AffinityMask = 0x04 | 0x08,

        /// <summary>
        /// Value	Description
        /// 0	Prod (64MB of usable application memory)
        /// 1	Undefined (unusable)
        /// 2	Dev1 (96MB of usable application memory)
        /// 3	Dev2 (80MB of usable application memory)
        /// 4	Dev3 (72MB of usable application memory)
        /// 5	Dev4 (32MB of usable application memory)
        /// 6-7	Undefined Same as Prod?
        /// </summary>
        Old3DSSystemMode = 0x0F | 0x10 | 0x20 | 0x40,
    }

    [Flags]
    public enum ARM11LSCFlag1 : byte
    {
        EnableL2Cache = 0x01,
        Cpuspeed_804MHz = 0x02,
    }

    [Flags]
    public enum ARM11LSCFlag2 : byte
    {
        /// <summary>
        /// Value	Description
        /// 0	Legacy (use Old3DS system mode)
        /// 1	Prod (124MB of usable application memory)
        /// 2	Dev1 (178MB of usable application memory)
        /// 3	Dev2 (124MB of usable application memory)
        /// 4-7	Undefined Same as Prod?
        /// </summary>
        New3DSSystemMode = 0x01 | 0x02 | 0x04 | 0x08,
    }

    [Flags]
    public enum BitMasks : byte
    {
        FixedCryptoKey = 0x01,
        NoMountRomFs = 0x02,
        NoCrypto = 0x04,
        NewKeyYGenerator = 0x20,
    }

    public enum ContentIndex : ushort
    {
        /// <summary>
        /// Main Content (.CXI for 3DS executable content/.CFA for 3DS Data Archives/.SRL for TWL content)
        /// </summary>
        MainContent = 0x0000,

        /// <summary>
        /// Home Menu Manual (.CFA)
        /// </summary>
        HomeMenuManual = 0x0001,

        /// <summary>
        /// DLP Child Container (.CFA)
        /// </summary>
        DLPChildContainer = 0x0002,
    }

    public enum ContentPlatform : byte
    {
        CTR = 0x01,
        Snake = 0x02, // New3DS
    }

    [Flags]
    public enum ContentType : byte
    {
        Data = 0x01,
        Executable = 0x02,
        SystemUpdate = 0x04,
        Manual = 0x08,
        Child = 0x04 | 0x08,
        Trial = 0x10,
    }

    public enum CryptoMethod : byte
    {
        Original = 0x00,
        Seven = 0x01,
        NineThree = 0x0A,
        NineSix = 0x0B,
    }

    [Flags]
    public enum FilesystemAccessInfo : ulong
    {
        CategorySystemApplication = 0x1,
        CategoryHardwareCheck = 0x2,
        CategoryFilesystemTool = 0x4,
        Debug = 0x8,
        TWLCardBackup = 0x10,
        TWLNANDData = 0x20,
        BOSS = 0x40,
        sdmcRoot = 0x80,
        Core = 0x100,
        nandRootroReadOnly = 0x200,
        nandRootrw = 0x400,
        nandrootroWriteAccess = 0x800,
        CategorySystemSettings = 0x1000,
        Cardboard = 0x2000,
        ExportImportIVS = 0x4000,
        sdmcRootWriteOnly = 0x8000,
        SwitchCleanup = 0x10000, // Introduced in 3.0.0?
        SavedataMove = 0x20000, // Introduced in 5.0.0
        Shop = 0x40000, // Introduced in 5.0.0
        Shell = 0x80000, // Introduced in 5.0.0
        CategoryHomeMenu = 0x100000, // Introduced in 6.0.0
        SeedDB = 0x200000, // Introduced in 9.6.0-X FIRM. Home Menu has this bit set starting with 9.6.0-X.
    }

    public enum FilesystemType : ulong
    {
        None = 0,
        Normal = 1,
        FIRM = 3,
        AGB_FIRMSave = 4,
    }

    public enum MediaCardDeviceType : byte
    {
        NORFlash = 0x01,
        None = 0x02,
        BT = 0x03,
    }

    public enum MediaPlatformIndex : byte
    {
        CTR = 0x01,
    }

    public enum MediaTypeIndex : byte
    {
        InnerDevice = 0x00,
        Card1 = 0x01,
        Card2 = 0x02,
        ExtendedDevice = 0x03,
    }

    public enum NCCHFlags
    {
        CryptoMethod = 0x03,
        ContentPlatform = 0x04,
        ContentTypeBitMask = 0x05,
        ContentUnitSize = 0x06,
        BitMasks = 0x07,
    }

    public enum NCSDFlags
    {
        BackupWriteWaitTime = 0x00,
        MediaCardDevice3X = 0x03,
        MediaPlatformIndex = 0x04,
        MediaTypeIndex = 0x05,
        MediaUnitSize = 0x06,
        MediaCardDevice2X = 0x07,
    }

    public enum PublicKeyType : uint
    {
        RSA_4096 = 0x00000000,
        RSA_2048 = 0x01000000,
        ECDSA = 0x02000000,
    }

    public enum ResourceLimitCategory
    {
        APPLICATION = 0,
        SYS_APPLET = 1,
        LIB_APPLET = 2,
        OTHER = 3,
    }

    // Note: These are reversed because of how C# reads values
    public enum SignatureType : uint
    {
        RSA_4096_SHA1 = 0x00000100,
        RSA_2048_SHA1 = 0x01000100,
        ECDSA_SHA1 = 0x02000100,
        RSA_4096_SHA256 = 0x03000100,
        RSA_2048_SHA256 = 0x04000100,
        ECDSA_SHA256 = 0x05000100,
    }

    [Flags]
    public enum StorageInfoOtherAttributes : byte
    {
        NotUseROMFS = 0x01,
        UseExtendedSavedataAccess = 0x02,
    }

    [Flags]
    public enum TMDContentType : ushort
    {
        Encrypted = 0x0001,
        Disc = 0x0002,
        CFM = 0x0004,
        Optional = 0x4000,
        Shared = 0x8000,
    }
}
