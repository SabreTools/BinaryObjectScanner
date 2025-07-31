#if NET20

namespace BinaryObjectScanner
{
    public delegate TResult Func<in T, out TResult>(T arg);
}

#endif
