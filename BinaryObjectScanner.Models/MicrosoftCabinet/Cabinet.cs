namespace BinaryObjectScanner.Models.MicrosoftCabinet
{
    /// <summary>
    /// Cabinet files are compressed packages containing a
    /// number of related files.The format of a cabinet file is optimized for maximum compression. Cabinet
    /// files support a number of compression formats, including MSZIP, LZX, or uncompressed. This
    /// document does not specify these internal compression formats.
    /// </summary>
    /// <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/>
    public sealed class Cabinet
    {
        /// <summary>
        /// Cabinet header
        /// </summary>
        public CFHEADER Header { get; set; }

        /// <summary>
        /// One or more CFFOLDER entries
        /// </summary>
        public CFFOLDER[] Folders { get; set; }

        /// <summary>
        /// A series of one or more cabinet file (CFFILE) entries
        /// </summary>
        public CFFILE[] Files { get; set; }
    }
}
