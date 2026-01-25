using System;
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;

namespace BinaryObjectScanner.Data
{
    /// <summary>
    /// Represents a mapping from file to a set of protections
    /// </summary>
#if NET20 || NET35
    internal class ProtectionDictionary : Dictionary<string, Queue<string>>
#else
    internal class ProtectionDictionary : ConcurrentDictionary<string, ConcurrentQueue<string>>
#endif
    {
        #region Accessors

        /// <summary>
        /// Append one result to a results dictionary
        /// </summary>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
        public void Append(string key, string? value)
        {
            // If the value is empty, don't add it
            if (value is null || value.Trim().Length == 0)
                return;

            EnsureKey(key);
            foreach (string subValue in ProcessProtectionString(value))
            {
                this[key].Enqueue(subValue);
            }
        }

        /// <summary>
        /// Append one set of results to a results dictionary
        /// </summary>
        /// <param name="key">Key to add information to</param>
        /// <param name="values">String value array to add</param>
        public void Append(string key, string[] values)
        {
            // Add the key if needed and then append the lists
            EnsureKey(key);
            foreach (string value in values)
            {
                if (value is null || value.Trim().Length == 0)
                    continue;

                foreach (string subValue in ProcessProtectionString(value))
                {
                    this[key].Enqueue(subValue);
                }
            }
        }

        /// <summary>
        /// Append one set of results to a results dictionary
        /// </summary>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
        public void Append(string key, ICollection<string> values)
        {
            // Use a placeholder value if the key is null
            key ??= "NO FILENAME";

            // Add the key if needed and then append the lists
            EnsureKey(key);
            AddRangeToKey(key, values);
        }

        /// <summary>
        /// Append one results dictionary to another
        /// </summary>
        /// <param name="addition">Dictionary to pull from</param>
        public void Append(ProtectionDictionary? addition)
        {
            // If the dictionary is missing, just return
            if (addition is null)
                return;

            // Loop through each of the addition keys and add accordingly
            foreach (string key in addition.Keys)
            {
                EnsureKey(key);
                AddRangeToKey(key, addition[key]);
            }
        }

        /// <summary>
        /// Remove empty or null keys from a results dictionary
        /// </summary>
        public void ClearEmptyKeys()
        {
            // Get a list of all of the keys
            List<string> keys = [.. Keys];

            // Iterate and reset keys
            for (int i = 0; i < keys.Count; i++)
            {
                // Get the current key
                string key = keys[i];

                // If the key is empty, remove it
#if NET20 || NET35
                if (this[key] is null || this[key].Count == 0)
                    Remove(key);
#else
                if (this[key] is null || this[key].IsEmpty)
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
            List<string> keys = [.. Keys];

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
            List<string> keys = [.. Keys];

            // Iterate and reset keys
            for (int i = 0; i < keys.Count; i++)
            {
                // Get the current key
                string currentKey = keys[i];

                // If the key doesn't start with the path, don't touch it
                if (!currentKey.StartsWith(pathToStrip, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Otherwise, get the new key name and transfer over
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
                string newKey = currentKey[pathToStrip!.Length..];
#else
                string newKey = currentKey.Substring(pathToStrip!.Length);
#endif
                this[newKey] = this[currentKey];
#if NET20 || NET35
                Remove(currentKey);
#else
                TryRemove(currentKey, out _);
#endif
            }
        }

        /// <summary>
        /// Add a range of values from one queue to another
        /// </summary>
        /// <param name="original">Queue to add data to</param>
        /// <param name="values">Queue to get data from</param>
        private void AddRangeToKey(string key, IEnumerable<string> values)
        {
            if (values is null)
                return;

            EnsureKey(key);
            foreach (string value in values)
            {
                if (value is null || value.Trim().Length == 0)
                    continue;

                foreach (string subValue in ProcessProtectionString(value))
                {
                    this[key].Enqueue(subValue);
                }
            }
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Reformat a protection dictionary for standard output
        /// </summary>
        /// <returns>Reformatted dictionary on success, empty on error</returns>
        public Dictionary<string, List<string>> ToDictionary()
        {
            // Null or empty protections return empty
            if (Count == 0)
                return [];

            // Reformat each set into a List
            var newDict = new Dictionary<string, List<string>>();
            foreach (string key in Keys)
            {
                newDict[key] = [.. this[key]];
            }

            return newDict;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Process a protection string if it includes multiple protections
        /// </summary>
        /// <param name="protection">Protection string to process</param>
        /// <returns>Set of protections parsed, empty on error</returns>
        internal static List<string> ProcessProtectionString(string? protection)
        {
            // If we have an invalid protection string
            if (string.IsNullOrEmpty(protection))
                return [];

            // Setup the output queue
            var protections = new List<string>();

            // If we have an indicator of multiple protections
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
            if (protection!.Contains(';'))
#else
            if (protection!.Contains(";"))
#endif
            {
                var splitProtections = protection.Split(';');
                protections.AddRange(splitProtections);
            }
            else
            {
                protections.Add(protection);
            }

            return protections;
        }

        /// <summary>
        /// Ensure the collection for the given key exists
        /// </summary>
        private void EnsureKey(string key)
        {
#if NET20 || NET35
            if (!ContainsKey(key))
                this[key] = new Queue<string>();
#else
            TryAdd(key, new ConcurrentQueue<string>());
#endif
        }

        #endregion
    }
}
