namespace BurnOutSharp.Models.VBSP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VBSPFile.h"/>
    public sealed class LumpHeader
    {
        public int LumpOffset;

        public int LumpID;

        public int LumpVersion;

        public int LumpLength;

        public int MapRevision;
    }
}
