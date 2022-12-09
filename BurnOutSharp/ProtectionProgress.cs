namespace BurnOutSharp
{
    /// <summary>
    /// Struct representing protection scanning progress
    /// </summary>
    public struct ProtectionProgress
    {
        /// <summary>
        /// Filename to report progress for
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Value between 0 and 1 representign the percentage completed
        /// </summary>
        public float Percentage { get; private set; }

        /// <summary>
        /// Protection information to report
        /// </summary>
        public string Protection { get; private set; }

        public ProtectionProgress(string filename, float percentage, string protection)
        {
            this.Filename = filename;
            this.Percentage = percentage;
            this.Protection = protection;
        }
    }
}
