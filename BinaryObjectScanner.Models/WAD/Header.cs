namespace BinaryObjectScanner.Models.WAD
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/WADFile.h"/>
    public sealed class Header
    {
        public string Signature;
        
        public uint LumpCount;
        
        public uint LumpOffset;
    }
}
