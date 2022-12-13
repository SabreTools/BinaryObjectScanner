namespace BurnOutSharp
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
        /// BFPK custom archive
        /// </summary>
        BFPK,

        /// <summary>
        /// bzip2 archive
        /// </summary>
        BZip2,

        /// <summary>
        /// Executable or library
        /// </summary>
        Executable,

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
        /// Microsoft cabinet file
        /// </summary>
        MicrosoftCAB,

        /// <summary>
        /// MPQ game data archive
        /// </summary>
        MPQ,

        /// <summary>
        /// Microsoft installation package
        /// </summary>
        MSI,

        /// <summary>
        /// PKWARE ZIP archive and derivatives
        /// </summary>
        PKZIP,

        /// <summary>
        /// PlayJ audio file
        /// </summary>
        PLJ,

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
        /// Tape archive
        /// </summary>
        TapeArchive,

        /// <summary>
        /// Various generic textfile formats
        /// </summary>
        Textfile,

        /// <summary>
        /// Various Valve archive formats
        /// </summary>
        Valve,

        /// <summary>
        /// xz archive
        /// </summary>
        XZ,
    }
}
