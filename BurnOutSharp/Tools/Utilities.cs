using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using BurnOutSharp.ExecutableType.Microsoft.PE;

namespace BurnOutSharp.Tools
{
    internal static class Utilities
    {
        #region Dictionary Manipulation

        /// <summary>
        /// Append one result to a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
        public static void AppendToDictionary(ConcurrentDictionary<string, ConcurrentQueue<string>> original, string key, string value)
        {
            // If the value is empty, don't add it
            if (string.IsNullOrWhiteSpace(value))
                return;

            var values = new ConcurrentQueue<string>();
            values.Enqueue(value);
            AppendToDictionary(original, key, values);
        }

        /// <summary>
        /// Append one result to a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
        public static void AppendToDictionary(ConcurrentDictionary<string, ConcurrentQueue<string>> original, string key, ConcurrentQueue<string> values)
        {
            // If the dictionary is null, just return
            if (original == null)
                return;

            // Use a placeholder value if the key is null
            key = key ?? "NO FILENAME";

            // Add the key if needed and then append the lists
            original.TryAdd(key, new ConcurrentQueue<string>());
            original[key].AddRange(values);
        }

        /// <summary>
        /// Append one results dictionary to another
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="addition">Dictionary to pull from</param>
        public static void AppendToDictionary(ConcurrentDictionary<string, ConcurrentQueue<string>> original, ConcurrentDictionary<string, ConcurrentQueue<string>> addition)
        {
            // If either dictionary is missing, just return
            if (original == null || addition == null)
                return;

            // Loop through each of the addition keys and add accordingly
            foreach (string key in addition.Keys)
            {
                original.TryAdd(key, new ConcurrentQueue<string>());
                original[key].AddRange(addition[key]);
            }
        }

        /// <summary>
        /// Remove empty or null keys from a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to clean</param>
        public static void ClearEmptyKeys(ConcurrentDictionary<string, ConcurrentQueue<string>> original)
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
                    original.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Prepend a parent path from dictionary keys, if possible
        /// </summary>
        /// <param name="original">Dictionary to strip values from</param>
        /// <param name="pathToPrepend">Path to strip from the keys</param>
        public static void PrependToKeys(ConcurrentDictionary<string, ConcurrentQueue<string>> original, string pathToPrepend)
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
                original.TryRemove(currentKey, out _);
            }
        }

        /// <summary>
        /// Strip a parent path from dictionary keys, if possible
        /// </summary>
        /// <param name="original">Dictionary to strip values from</param>
        /// <param name="pathToStrip">Path to strip from the keys</param>
        public static void StripFromKeys(ConcurrentDictionary<string, ConcurrentQueue<string>> original, string pathToStrip)
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
                string newKey = currentKey.Substring(pathToStrip.Length);
                original[newKey] = original[currentKey];
                original.TryRemove(currentKey, out _);
            }
        }

        #endregion

        #region Concurrent Manipulation

        /// <summary>
        /// Add a range of values from one queue to another
        /// </summary>
        /// <param name="original">Queue to add data to</param>
        /// <param name="values">Queue to get data from</param>
        public static void AddRange(this ConcurrentQueue<string> original, ConcurrentQueue<string> values)
        {
            while (!values.IsEmpty)
            {
                if (!values.TryDequeue(out string value))
                    return;

                original.Enqueue(value);
            }
        }

        #endregion

        #region Processed Executable Information

        /// <summary>
        /// Get the internal version as reported by the resources
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetInternalVersion(byte[] fileContent)
        {
            if (fileContent == null || !fileContent.Any())
                return null;

            return GetInternalVersion(new PortableExecutable(fileContent, 0));
        }

        /// <summary>
        /// Get the internal version as reported by the resources
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetInternalVersion(PortableExecutable pex)
        {
            string version = pex.FileVersion;
            if (!string.IsNullOrWhiteSpace(version))
                return version;

            version = pex.ProductVersion;
            if (!string.IsNullOrWhiteSpace(version))
                return version;

            version = pex.ManifestVersion;
            if (!string.IsNullOrWhiteSpace(version))
                return version;

            return null;
        }

        /// <summary>
        /// Get the internal version as reported by the filesystem
        /// </summary>
        /// <param name="file">File to check for version</param>
        /// <returns>Version string, null on error</returns>
        public static string GetInternalVersion(string file)
        {
            var fvinfo = GetFileVersionInfo(file);
            if (fvinfo?.FileVersion == null)
                return string.Empty;
            if (fvinfo.FileVersion != "")
                return fvinfo.FileVersion.Replace(", ", ".");
            else
                return fvinfo.ProductVersion.Replace(", ", ".");
        }

        #endregion

        #region Executable Information

        /// <summary>
        /// Get the file version info object related to a path, if possible
        /// </summary>
        /// <param name="file">File to get information for</param>
        /// <returns>FileVersionInfo object on success, null on error</returns>
        private static FileVersionInfo GetFileVersionInfo(string file)
        {
            if (file == null || !File.Exists(file))
                return null;

            try
            {
                return FileVersionInfo.GetVersionInfo(file);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the SHA1 hash of a file, if possible
        /// </summary>
        /// <param name="path">Path to the file to be hashed</param>
        /// <returns>SHA1 hash as a string on success, null on error</returns>
        public static string GetFileSHA1(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                SHA1 sha1 = SHA1.Create();
                using (Stream fileStream = File.OpenRead(path))
                {
                    byte[] buffer = new byte[32768];
                    while (true)
                    {
                        int bytesRead = fileStream.Read(buffer, 0, 32768);
                        if (bytesRead == 32768)
                        {
                            sha1.TransformBlock(buffer, 0, bytesRead, null, 0);
                        }
                        else
                        {
                            sha1.TransformFinalBlock(buffer, 0, bytesRead);
                            break;
                        }
                    }

                    string hash = BitConverter.ToString(sha1.Hash);
                    hash = hash.Replace("-", string.Empty);
                    return hash;
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Wrappers for Matchers

        /// <summary>
        /// Wrapper for GetInternalVersion for use in content matching
        /// </summary>
        /// <param name="file">File to check for version</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="positions">Last matched positions in the contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetInternalVersion(string file, byte[] fileContent, List<int> positions) => GetInternalVersion(fileContent);

        /// <summary>
        /// Wrapper for GetInternalVersion for use in path matching
        /// </summary>
        /// <param name="firstMatchedString">File to check for version</param>
        /// <param name="files">Full list of input paths</param>
        /// <returns>Version string, null on error</returns>
        public static string GetInternalVersion(string firstMatchedString, IEnumerable<string> files) => GetInternalVersion(firstMatchedString);

        #endregion
    }
}
