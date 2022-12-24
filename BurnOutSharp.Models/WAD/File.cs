namespace BurnOutSharp.Models.WAD
{
    /// <summary>
    /// Half-Life Texture Package File
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/WADFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Deserialized header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Deserialized lumps data
        /// </summary>
        public Lump[] Lumps { get; set; }

        /// <summary>
        /// Deserialized lump infos data
        /// </summary>
        public LumpInfo[] LumpInfos { get; set; }
    }
}
