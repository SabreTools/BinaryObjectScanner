using System;
#if NET20 || NET35
using System.Collections.Generic;
#else
using System.Collections.Concurrent;
#endif
using System.IO;
using System.Linq;

namespace BinaryObjectScanner.Utilities
{
    /// <summary>
    /// Dictionary manipulation methods
    /// </summary>
    public static class Dictionary
    {
        /// <summary>
        /// Append one result to a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
#if NET20 || NET35
        public static void AppendToDictionary(Dictionary<string, Queue<string>> original, string key, string value)
#else
        public static void AppendToDictionary(ConcurrentDictionary<string, ConcurrentQueue<string>> original, string key, string value)
#endif
        {
            // If the value is empty, don't add it
            if (string.IsNullOrEmpty(value))
                return;

#if NET20 || NET35
            var values = new Queue<string>();
#else
            var values = new ConcurrentQueue<string>();
#endif
            values.Enqueue(value);
            AppendToDictionary(original, key, values);
        }

        /// <summary>
        /// Append one set of results to a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="key">Key to add information to</param>
        /// <param name="values">String value array to add</param>
#if NET20 || NET35
        public static void AppendToDictionary(Dictionary<string, Queue<string>> original, string key, string[] values)
#else
        public static void AppendToDictionary(ConcurrentDictionary<string, ConcurrentQueue<string>> original, string key, string[] values)
#endif
        {
            // If the dictionary is null, just return
            if (original == null)
                return;

            // Use a placeholder value if the key is null
            key ??= "NO FILENAME";

            // Add the key if needed and then append the lists
#if NET20 || NET35
            original[key] ??= new Queue<string>();
#else
            original.TryAdd(key, new ConcurrentQueue<string>());
#endif
            original[key].AddRange(values);
        }

        /// <summary>
        /// Append one set of results to a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
#if NET20 || NET35
        public static void AppendToDictionary(Dictionary<string, Queue<string>> original, string key, Queue<string> values)
#else
        public static void AppendToDictionary(ConcurrentDictionary<string, ConcurrentQueue<string>> original, string key, ConcurrentQueue<string> values)
#endif
        {
            // If the dictionary is null, just return
            if (original == null)
                return;

            // Use a placeholder value if the key is null
            key ??= "NO FILENAME";

            // Add the key if needed and then append the lists
#if NET20 || NET35
            original[key] ??= new Queue<string>();
#else
            original.TryAdd(key, new ConcurrentQueue<string>());
#endif
            original[key].AddRange(values);
        }

        /// <summary>
        /// Append one results dictionary to another
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="addition">Dictionary to pull from</param>
#if NET20 || NET35
        public static void AppendToDictionary(Dictionary<string, Queue<string>> original, Dictionary<string, Queue<string>> addition)
#else
        public static void AppendToDictionary(ConcurrentDictionary<string, ConcurrentQueue<string>> original, ConcurrentDictionary<string, ConcurrentQueue<string>> addition)
#endif
        {
            // If either dictionary is missing, just return
            if (original == null || addition == null)
                return;

            // Loop through each of the addition keys and add accordingly
            foreach (string key in addition.Keys)
            {
#if NET20 || NET35
                original[key] ??= new Queue<string>();
#else
                original.TryAdd(key, new ConcurrentQueue<string>());
#endif
                original[key].AddRange(addition[key]);
            }
        }

        /// <summary>
        /// Remove empty or null keys from a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to clean</param>
#if NET20 || NET35
        public static void ClearEmptyKeys(Dictionary<string, Queue<string>> original)
#else
        public static void ClearEmptyKeys(ConcurrentDictionary<string, ConcurrentQueue<string>> original)
#endif
        {
            // If the dictionary is missing, we can't do anything
            if (original == null)
                return;

            // Get a list of all of the keys
            var keys = original.Keys.ToList();

            // Iterate and reset keys
            for (int i = 0; i < keys.Count; i++)
            {
                // Get the current key
                string key = keys[i];

                // If the key is empty, remove it
                if (original[key] == null || !original[key].Any())
#if NET20 || NET35
                    original.Remove(key);
#else
                    original.TryRemove(key, out _);
#endif
            }
        }

        /// <summary>
        /// Prepend a parent path from dictionary keys, if possible
        /// </summary>
        /// <param name="original">Dictionary to strip values from</param>
        /// <param name="pathToPrepend">Path to strip from the keys</param>
#if NET20 || NET35
        public static void PrependToKeys(Dictionary<string, Queue<string>>? original, string pathToPrepend)
#else
        public static void PrependToKeys(ConcurrentDictionary<string, ConcurrentQueue<string>>? original, string pathToPrepend)
#endif
        {
            // If the dictionary is missing, we can't do anything
            if (original == null)
                return;

            // Use a placeholder value if the path is null
            pathToPrepend = (pathToPrepend ?? "ARCHIVE").TrimEnd(Path.DirectorySeparatorChar);

            // Get a list of all of the keys
            var keys = original.Keys.ToList();

            // Iterate and reset keys
            for (int i = 0; i < keys.Count; i++)
            {
                // Get the current key
                string currentKey = keys[i];

                // Otherwise, get the new key name and transfer over
                string newKey = $"{pathToPrepend}{Path.DirectorySeparatorChar}{currentKey.Trim(Path.DirectorySeparatorChar)}";
                original[newKey] = original[currentKey];
#if NET20 || NET35
                original.Remove(currentKey);
#else
                original.TryRemove(currentKey, out _);
#endif
            }
        }

        /// <summary>
        /// Strip a parent path from dictionary keys, if possible
        /// </summary>
        /// <param name="original">Dictionary to strip values from</param>
        /// <param name="pathToStrip">Path to strip from the keys</param>
#if NET20 || NET35
        public static void StripFromKeys(Dictionary<string, Queue<string>>? original, string? pathToStrip)
#else
        public static void StripFromKeys(ConcurrentDictionary<string, ConcurrentQueue<string>>? original, string? pathToStrip)
#endif
        {
            // If either is missing, we can't do anything
            if (original == null || string.IsNullOrEmpty(pathToStrip))
                return;

            // Get a list of all of the keys
            var keys = original.Keys.ToList();

            // Iterate and reset keys
            for (int i = 0; i < keys.Count; i++)
            {
                // Get the current key
                string currentKey = keys[i];

                // If the key doesn't start with the path, don't touch it
                if (!currentKey.StartsWith(pathToStrip, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Otherwise, get the new key name and transfer over
                string newKey = currentKey.Substring(pathToStrip!.Length);
                original[newKey] = original[currentKey];
#if NET20 || NET35
                original.Remove(currentKey);
#else
                original.TryRemove(currentKey, out _);
#endif
            }
        }
    }
}
