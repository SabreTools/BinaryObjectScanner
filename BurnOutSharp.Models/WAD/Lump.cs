namespace BurnOutSharp.Models.WAD
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/WADFile.h"/>
    public sealed class Lump
    {
        public uint Offset;

        public uint DiskLength;

        public uint Length;

        public byte Type;

        public byte Compression;

        public byte Padding0;

        public byte Padding1;

        public string Name;
    }
}
