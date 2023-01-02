namespace BurnOutSharp.Models.Compression.Quantum
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    /// <see href="http://www.russotto.net/quantumcomp.html"/>
    public sealed class ModelSymbol
    {
        public ushort Symbol;

        public ushort CumulativeFrequency;
    }
}