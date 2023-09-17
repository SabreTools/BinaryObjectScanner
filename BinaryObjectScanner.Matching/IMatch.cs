namespace BinaryObjectScanner.Matching
{
    public interface IMatch<T>
    {
#if NET48
        T Needle { get; set; }
#else
        T? Needle { get; init; }
#endif
    }
}
