namespace BurnOutSharp
{
    public class Progress
    {
        public string Filename { get; private set; }
        public float Percentage { get; private set; }
        public string Protection { get; private set; }

        public Progress(string filename, float percentage, string protection)
        {
            this.Filename = filename;
            this.Percentage = percentage;
            this.Protection = protection;
        }
    }
}
