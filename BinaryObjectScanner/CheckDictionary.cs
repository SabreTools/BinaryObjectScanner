namespace BinaryObjectScanner
{
    /// <summary>
    /// Represents a mapping from checker to detected protection
    /// </summary>
#if NET20 || NET35
    public class CheckDictionary<T> : System.Collections.Generic.Dictionary<T, string> where T : notnull { }
#else
    public class CheckDictionary<T> : System.Collections.Concurrent.ConcurrentDictionary<T, string> where T : notnull { }
#endif
}