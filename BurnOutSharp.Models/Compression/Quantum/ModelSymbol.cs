namespace BurnOutSharp.Models.Compression.Quantum
{
    /// <see href="http://www.russotto.net/quantumcomp.html"/>
    public class ModelSymbol
    {
        public ushort Symbol { get; private set; }

        public ushort CumulativeFrequency { get; private set; }
    }
}