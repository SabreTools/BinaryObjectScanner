namespace BinaryObjectScanner.Models.BSP
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/BSPFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Lumps
        /// </summary>
        public Lump[] Lumps { get; set; }

        /// <summary>
        /// Texture header data
        /// </summary>
        public TextureHeader TextureHeader { get; set; }

        /// <summary>
        /// Textures
        /// </summary>
        public Texture[] Textures { get; set; }
    }
}