namespace BurnOutSharp.Models.BSP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/BSPFile.h"/>
    public sealed class Texture
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name;
    
        /// <summary>
        /// Width
        /// </summary>
        public uint Width;

        /// <summary>
        /// Height
        /// </summary>
        public uint Height;

        /// <summary>
        /// Offsets
        /// </summary>
        public uint[] Offsets;

        /// <summary>
        /// Texture data
        /// </summary>
        public byte[] TextureData;

        /// <summary>
        /// Palette size
        /// </summary>
        public uint PaletteSize;

        /// <summary>
        /// Palette data
        /// </summary>
        public byte[] PaletteData;
    }
}