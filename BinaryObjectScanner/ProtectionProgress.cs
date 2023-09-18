namespace BinaryObjectScanner
{
    /// <summary>
    /// Struct representing protection scanning progress
    /// </summary>
    public struct ProtectionProgress
    {
        /// <summary>
        /// Filename to report progress for
        /// </summary>
#if NET48
        public string Filename { get; private set; }
#else
        public string Filename { get; init; }
#endif

        /// <summary>
        /// Value between 0 and 1 representign the percentage completed
        /// </summary>
#if NET48
        public float Percentage { get; private set; }
#else
        public float Percentage { get; init; }
#endif

        /// <summary>
        /// Protection information to report
        /// </summary>
#if NET48
        public string Protection { get; private set; }
#else
        public string Protection { get; init; }
#endif

        public ProtectionProgress(string filename, float percentage, string protection)
        {
            this.Filename = filename;
            this.Percentage = percentage;
            this.Protection = protection;
        }
    }
}
