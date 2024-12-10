namespace BinaryObjectScanner
{
    /// <summary>
    /// Scanning options
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Determines whether archives are decompressed and scanned
        /// </summary>
        public bool ScanArchives { get; set; }

        /// <summary>
        /// Determines if content matches are used or not
        /// </summary>
        public bool ScanContents { get; set; }

        /// <summary>
        /// Determines if path matches are used or not
        /// </summary>
        public bool ScanPaths { get; set; }

        /// <summary>
        /// Determines if debug information is output or not
        /// </summary>
        public bool IncludeDebug { get; set; }
    }
}
