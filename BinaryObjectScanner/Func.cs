#if NET20

namespace BinaryObjectScanner
{
    internal delegate TResult Func<in T, out TResult>(T arg);
}

#endif
