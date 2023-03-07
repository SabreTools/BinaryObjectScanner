namespace BinaryObjectScanner.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public sealed class Header6 : Header
    {
        public string Name;

        public uint HeaderLength;

        public uint FileDataOffset;

        public uint Dummy0;
    }
}
