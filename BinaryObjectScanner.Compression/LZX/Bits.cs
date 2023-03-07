namespace BinaryObjectScanner.Compression.LZX
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    public class Bits
    {
        public uint BitBuffer;

        public int BitsLeft;

        public int InputPosition; //byte*
    }
}