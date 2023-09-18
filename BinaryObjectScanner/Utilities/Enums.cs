namespace BinaryObjectScanner.Utilities
{
    /// <summary>
    /// Subset of file types that are supported by the library
    /// </summary>
    public enum SupportedFileType
    {
        /// <summary>
        /// Unknown or unsupported
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// AACS media key block
        /// </summary>
        AACSMediaKeyBlock,

        /// <summary>
        /// BD+ SVM
        /// </summary>
        BDPlusSVM,

        /// <summary>
        /// BFPK custom archive
        /// </summary>
        BFPK,

        /// <summary>
        /// Half-Life Level
        /// </summary>
        BSP,

        /// <summary>
        /// bzip2 archive
        /// </summary>
        BZip2,

        /// <summary>
        /// Compound File Binary
        /// </summary>
        CFB,

        /// <summary>
        /// CTR Importable Archive
        /// </summary>
        CIA,

        /// <summary>
        /// Executable or library
        /// </summary>
        Executable,

        /// <summary>
        /// Half-Life Game Cache File
        /// </summary>
        GCF,

        /// <summary>
        /// gzip archive
        /// </summary>
        GZIP,

        /// <summary>
        /// Key-value pair INI file
        /// </summary>
        IniFile,

        /// <summary>
        /// InstallShield archive v3
        /// </summary>
        InstallShieldArchiveV3,

        /// <summary>
        /// InstallShield cabinet file
        /// </summary>
        InstallShieldCAB,

        /// <summary>
        /// Link Data Security encrypted file
        /// </summary>
        LDSCRYPT,

        /// <summary>
        /// Microsoft cabinet file
        /// </summary>
        MicrosoftCAB,

        /// <summary>
        /// Microsoft LZ-compressed file
        /// </summary>
        MicrosoftLZ,

        /// <summary>
        /// MPQ game data archive
        /// </summary>
        MPQ,

        /// <summary>
        /// Nintendo 3DS cart image
        /// </summary>
        N3DS,

        /// <summary>
        /// Half-Life No Cache File
        /// </summary>
        NCF,

        /// <summary>
        /// Nintendo DS/DSi cart image
        /// </summary>
        Nitro,

        /// <summary>
        /// Half-Life Package File
        /// </summary>
        PAK,

        /// <summary>
        /// NovaLogic Game Archive Format
        /// </summary>
        PFF,

        /// <summary>
        /// PKWARE ZIP archive and derivatives
        /// </summary>
        PKZIP,

        /// <summary>
        /// PlayJ audio file
        /// </summary>
        PLJ,

        /// <summary>
        /// Quantum archive
        /// </summary>
        Quantum,

        /// <summary>
        /// RAR archive
        /// </summary>
        RAR,

        /// <summary>
        /// 7-zip archive
        /// </summary>
        SevenZip,

        /// <summary>
        /// StarForce FileSystem file
        /// </summary>
        SFFS,

        /// <summary>
        /// SGA
        /// </summary>
        SGA,

        /// <summary>
        /// Tape archive
        /// </summary>
        TapeArchive,

        /// <summary>
        /// Various generic textfile formats
        /// </summary>
        Textfile,

        /// <summary>
        /// Half-Life 2 Level
        /// </summary>
        VBSP,

        /// <summary>
        /// Valve Package File
        /// </summary>
        VPK,

        /// <summary>
        /// Half-Life Texture Package File
        /// </summary>
        WAD,

        /// <summary>
        /// xz archive
        /// </summary>
        XZ,

        /// <summary>
        /// Xbox Package File
        /// </summary>
        XZP,
    }
}
