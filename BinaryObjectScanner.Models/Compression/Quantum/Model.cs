namespace BinaryObjectScanner.Models.Compression.Quantum
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    /// <see href="http://www.russotto.net/quantumcomp.html"/>
    public sealed class Model
    {
        public int TimeToReorder;

        public int Entries;

        public ModelSymbol[] Symbols;

        public ushort[] LookupTable = new ushort[256];
    }
}