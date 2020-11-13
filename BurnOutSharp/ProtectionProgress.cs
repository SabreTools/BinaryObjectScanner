namespace BurnOutSharp
{
    public class ProtectionProgress
    {
        public string Filename { get; private set; }
        public float Percentage { get; private set; }
        public string Protection { get; private set; }

        public ProtectionProgress(string filename, float percentage, string protection)
        {
            this.Filename = filename;
            this.Percentage = percentage;
            this.Protection = protection;
        }
    }
}
