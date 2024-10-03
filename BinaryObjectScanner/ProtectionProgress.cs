namespace BinaryObjectScanner
{
    /// <summary>
    /// Struct representing protection scanning progress
    /// </summary>
#if NET20 || NET35 || NET40
    public class ProtectionProgress : System.EventArgs
#else
    public struct ProtectionProgress
#endif
    {
        /// <summary>
        /// Filename to report progress for
        /// </summary>
        public readonly string? Filename { get; }

        /// <summary>
        /// Value between 0 and 1 representign the percentage completed
        /// </summary>
        public readonly float Percentage { get; }

        /// <summary>
        /// Protection information to report
        /// </summary>
        public readonly string? Protection { get; }

        public ProtectionProgress(string? filename, float percentage, string? protection)
        {
            Filename = filename;
            Percentage = percentage;
            Protection = protection;
        }
    }
}
