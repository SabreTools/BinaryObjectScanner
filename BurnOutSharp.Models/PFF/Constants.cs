namespace BurnOutSharp.Models.PFF
{
    /// <see href="https://devilsclaws.net/download/file-pff-new-bz2"/>
    public static class Constants
    {
        // Version 1 not confirmed
        // public const string Version1SignatureString = "PFF1";
        // public const uint Version1HeaderSize = 0x00000000;

        public const string Version2SignatureString = "PFF2";
        public const uint Version2SegmentSize = 0x00000020;

        public const string Version3SignatureString = "PFF3";
        public const uint Version3SegmentSize = 0x00000024;

        public const string Version4SignatureString = "PFF4";
        public const uint Version4SegmentSize = 0x00000028;

        public const string FooterKingTag = "KING";
    }
}