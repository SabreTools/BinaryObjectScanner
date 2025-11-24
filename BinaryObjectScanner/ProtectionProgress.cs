namespace BinaryObjectScanner
{
    /// <summary>
    /// Struct representing protection scanning progress
    /// </summary>
#if NET20 || NET35 || NET40
    public class ProtectionProgress : System.EventArgs
#else
    public readonly struct ProtectionProgress
#endif
    {
        /// <summary>
        /// Filename to report progress for
        /// </summary>
        public string? Filename { get; }

        /// <summary>
        /// Number of levels deep the file is
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Value between 0 and 1 representign the percentage completed
        /// </summary>
        public float Percentage { get; }

        /// <summary>
        /// Protection information to report
        /// </summary>
        public string? Protection { get; }

        public ProtectionProgress(string? filename, int depth, float percentage, string? protection)
        {
            Filename = filename;
            Depth = depth;
            Percentage = percentage;
            Protection = protection;
        }
    }
}
