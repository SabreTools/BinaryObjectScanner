namespace BurnOutSharp.Compression.ADPCM
{
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/adpcm/adpcm.cpp"/>
    public unsafe struct ADPCM_DATA
    {
        public uint[] pValues;
        public int BitCount;
        public int field_8;
        public int field_C;
        public int field_10;
    }
}