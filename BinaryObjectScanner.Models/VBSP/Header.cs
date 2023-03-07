namespace BinaryObjectScanner.Models.VBSP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VBSPFile.h"/>
    public sealed class Header
    {
        /// <summary>
        /// BSP file signature.
        /// </summary>
        public string Signature;

        /// <summary>
        /// BSP file version.
        /// </summary>
        /// <remarks>
        /// 19-20:			Source
        /// 21:				Source - The lump version property was moved to the start of the struct.
        /// 0x00040014:		Dark Messiah - Looks like the 32 bit version has been split into two 16 bit fields.
        /// </remarks>
        public int Version;

        /// <summary>
        /// Lumps.
        /// </summary>
        public Lump[] Lumps;

        /// <summary>
        /// The map's revision (iteration, version) number.
        /// </summary>
        public int MapRevision;
    }
}
