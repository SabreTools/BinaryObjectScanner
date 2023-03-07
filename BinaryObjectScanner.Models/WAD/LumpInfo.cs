namespace BinaryObjectScanner.Models.WAD
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/WADFile.h"/>
    public sealed class LumpInfo
    {
        public string Name;

        public uint Width;

        public uint Height;

        public uint PixelOffset;

        // 12 bytes of unknown data

        public byte[] PixelData;

        public uint PaletteSize;

        public byte[] PaletteData;
    }
}
