namespace BurnOutSharp.Compression.bzip2
{
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/bzip2/bzlib.h"/>
    public unsafe struct bz_stream
    {
        public char* next_in;
        public uint avail_in;
        public uint total_in_lo32;
        public uint total_in_hi32;

        public char* next_out;
        public uint avail_out;
        public uint total_out_lo32;
        public uint total_out_hi32;

        public void* state;

        // void *(*bzalloc)(void *,int,int);
        // void (*bzfree)(void *,void *);
        // void *opaque;
    }
}