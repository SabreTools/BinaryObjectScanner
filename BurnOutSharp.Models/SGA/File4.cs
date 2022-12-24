namespace BurnOutSharp.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public class File4
    {
        public uint NameOffset;

        public uint Offset;

        public uint SizeOnDisk;

        public uint Size;

        public uint TimeModified;

        public byte Dummy0;

        public byte Type;
    }
}
