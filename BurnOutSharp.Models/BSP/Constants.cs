namespace BurnOutSharp.Models.BSP
{
    public static class Constants
    {
        /// <summary>
        /// Number of lumps in a BSP
        /// </summary>
        public const int HL_BSP_LUMP_COUNT = 15;

        /// <summary>
        /// Index for the entities lump
        /// </summary>
        public const int HL_BSP_LUMP_ENTITIES = 0;

        /// <summary>
        /// Index for the texture data lump
        /// </summary>
        public const int HL_BSP_LUMP_TEXTUREDATA = 2;

        /// <summary>
        /// Number of valid mipmap levels
        /// </summary>
        public const int HL_BSP_MIPMAP_COUNT = 4;
    }
}