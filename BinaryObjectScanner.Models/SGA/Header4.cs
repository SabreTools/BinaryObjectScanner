namespace BinaryObjectScanner.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public sealed class Header4 : Header
    {
        public byte[] FileMD5;

        public string Name;

        public byte[] HeaderMD5;

        public uint HeaderLength;

        public uint FileDataOffset;

        public uint Dummy0;
    }
}
