namespace BinaryObjectScanner
{
    /// <summary>
    /// Represents a mapping from checker to detected protection
    /// </summary>
#if NET20 || NET35
    public class CheckDictionary<T> : System.Collections.Generic.Dictionary<T, string> where T : notnull
#else
    public class CheckDictionary<T> : System.Collections.Concurrent.ConcurrentDictionary<T, string> where T : notnull
#endif
    {
        /// <inheritdoc cref="System.Collections.Generic.Dictionary{TKey, TValue}.Add(TKey, TValue)"/>
        /// <remarks>Handles the proper Add implementation</remarks>
        public void Append(T key, string? value)
        {
            if (value == null)
                return;

#if NET20 || NET35
            this[key] = value;
#else
            TryAdd(key, value);
#endif
        }
    }
}