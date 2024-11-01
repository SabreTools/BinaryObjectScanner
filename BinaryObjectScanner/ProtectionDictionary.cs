using System;
#if NET20 || NET35
using System.Collections.Generic;
#else
using System.Collections.Concurrent;
#endif
using System.IO;
using System.Linq;
using BinaryObjectScanner.Utilities;

namespace BinaryObjectScanner
{
#if NET20 || NET35
    public class ProtectionDictionary : Dictionary<string, Queue<string>>
#else
    public class ProtectionDictionary : ConcurrentDictionary<string, ConcurrentQueue<string>>
#endif
    {
        /// <summary>
        /// Append one result to a results dictionary
        /// </summary>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
        public void Append(string key, string value)
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
            Append(key, values);
        }

        /// <summary>
        /// Append one set of results to a results dictionary
        /// </summary>
        /// <param name="key">Key to add information to</param>
        /// <param name="values">String value array to add</param>
        public void Append(string key, string[] values)
        {
            // Use a placeholder value if the key is null
            key ??= "NO FILENAME";

            // Add the key if needed and then append the lists
#if NET20 || NET35
            if (!ContainsKey(key))
                this[key] = new Queue<string>();
#else
            TryAdd(key, new ConcurrentQueue<string>());
#endif
            this[key].AddRange(values);
        }

        /// <summary>
        /// Append one set of results to a results dictionary
        /// </summary>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
#if NET20 || NET35
        public void Append(string key, Queue<string> values)
#else
        public void Append(string key, ConcurrentQueue<string> values)
#endif
        {
            // Use a placeholder value if the key is null
            key ??= "NO FILENAME";

            // Add the key if needed and then append the lists
#if NET20 || NET35
            if (!ContainsKey(key))
                this[key] = new Queue<string>();
#else
            TryAdd(key, new ConcurrentQueue<string>());
#endif
            this[key].AddRange(values);
        }

        /// <summary>
        /// Append one results dictionary to another
        /// </summary>
        /// <param name="addition">Dictionary to pull from</param>
        public void Append(ProtectionDictionary? addition)
        {
            // If the dictionary is missing, just return
            if (addition == null)
                return;

            // Loop through each of the addition keys and add accordingly
            foreach (string key in addition.Keys)
            {
#if NET20 || NET35
                if (!ContainsKey(key))
                    this[key] = new Queue<string>();
#else
                TryAdd(key, new ConcurrentQueue<string>());
#endif
                this[key].AddRange(addition[key]);
            }
        }

        /// <summary>
        /// Remove empty or null keys from a results dictionary
        /// </summary>
        public void ClearEmptyKeys()
        {
            // Get a list of all of the keys
            var keys = Keys.ToList();

            // Iterate and reset keys
            for (int i = 0; i < keys.Count; i++)
            {
                // Get the current key
                string key = keys[i];

                // If the key is empty, remove it
                if (this[key] == null || !this[key].Any())
#if NET20 || NET35
                    Remove(key);
#else
                    TryRemove(key, out _);
#endif
            }
        }

        /// <summary>
        /// Prepend a parent path from dictionary keys, if possible
        /// </summary>
        /// <param name="pathToPrepend">Path to strip from the keys</param>
        public void PrependToKeys(string pathToPrepend)
        {
            // Use a placeholder value if the path is null
            pathToPrepend = (pathToPrepend ?? "ARCHIVE").TrimEnd(Path.DirectorySeparatorChar);

            // Get a list of all of the keys
            var keys = Keys.ToList();

            // Iterate and reset keys
            for (int i = 0; i < keys.Count; i++)
            {
                // Get the current key
                string currentKey = keys[i];

                // Otherwise, get the new key name and transfer over
                string newKey = $"{pathToPrepend}{Path.DirectorySeparatorChar}{currentKey.Trim(Path.DirectorySeparatorChar)}";
                this[newKey] = this[currentKey];
#if NET20 || NET35
                Remove(currentKey);
#else
                TryRemove(currentKey, out _);
#endif
            }
        }

        /// <summary>
        /// Strip a parent path from dictionary keys, if possible
        /// </summary>
        /// <param name="pathToStrip">Path to strip from the keys</param>
        public void StripFromKeys(string? pathToStrip)
        {
            // If the path is missing, we can't do anything
            if (string.IsNullOrEmpty(pathToStrip))
                return;

            // Get a list of all of the keys
            var keys = Keys.ToList();

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
                this[newKey] = this[currentKey];
#if NET20 || NET35
                Remove(currentKey);
#else
                TryRemove(currentKey, out _);
#endif
            }
        }
    }
}