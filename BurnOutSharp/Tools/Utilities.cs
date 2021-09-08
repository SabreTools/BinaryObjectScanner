using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using BurnOutSharp.Matching;

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

        #region Byte Arrays

        /// <summary>
        /// Find all positions of one array in another, if possible, if possible
        /// </summary>
        public static List<int> FindAllPositions(this byte[] stack, byte?[] needle, int start = 0, int end = -1)
        {
            // Get the outgoing list
            List<int> positions = new List<int>();

            // Initialize the loop variables
            bool found = true;
            int lastPosition = start;
            var matcher = new ContentMatch(needle, end: end);

            // Loop over and get all positions
            while (found)
            {
                matcher.Start = lastPosition;
                (found, lastPosition) = matcher.Match(stack, false);
                if (found)
                    positions.Add(lastPosition);
            }

            return positions;
        }

        /// <summary>
        /// Find the first position of one array in another, if possible
        /// </summary>
        public static bool FirstPosition(this byte[] stack, byte?[] needle, out int position, int start = 0, int end = -1)
        {
            var matcher = new ContentMatch(needle, start, end);
            (bool found, int foundPosition) = matcher.Match(stack, false);
            position = foundPosition;
            return found;
        }

        /// <summary>
        /// Find the last position of one array in another, if possible
        /// </summary>
        public static bool LastPosition(this byte[] stack, byte?[] needle, out int position, int start = 0, int end = -1)
        {
            var matcher = new ContentMatch(needle, start, end);
            (bool found, int foundPosition) = matcher.Match(stack, true);
            position = foundPosition;
            return found;
        }

        /// <summary>
        /// See if a byte array starts with another
        /// </summary>
        public static bool StartsWith(this byte[] stack, byte?[] needle)
        {
            return stack.FirstPosition(needle, out int _, start: 0, end: 1);
        }

        /// <summary>
        /// See if a byte array ends with another
        /// </summary>
        public static bool EndsWith(this byte[] stack, byte?[] needle)
        {
            return stack.FirstPosition(needle, out int _, start: stack.Length - needle.Length);
        }

        #endregion

        #region Protection

        /// <summary>
        /// Get the file version info object related to a path, if possible
        /// </summary>
        /// <param name="file">File to get information for</param>
        /// <returns>FileVersionInfo object on success, null on error</returns>
        public static FileVersionInfo GetFileVersionInfo(string file)
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
        /// Get the file version as reported by the filesystem
        /// </summary>
        /// <param name="file">File to check for version</param>
        /// <returns>Version string, null on error</returns>
        public static string GetFileVersion(string file)
        {
            var fvinfo = GetFileVersionInfo(file);
            if (fvinfo?.FileVersion == null)
                return string.Empty;
            if (fvinfo.FileVersion != "")
                return fvinfo.FileVersion.Replace(", ", ".");
            else
                return fvinfo.ProductVersion.Replace(", ", ".");
        }

        /// <summary>
        /// Wrapper for GetFileVersion for use in content matching
        /// </summary>
        /// <param name="file">File to check for version</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="positions">Last matched positions in the contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetFileVersion(string file, byte[] fileContent, List<int> positions) => GetFileVersion(file);

        /// <summary>
        /// Wrapper for GetFileVersion for use in path matching
        /// </summary>
        /// <param name="firstMatchedString">File to check for version</param>
        /// <param name="files">Full list of input paths</param>
        /// <returns>Version string, null on error</returns>
        public static string GetFileVersion(string firstMatchedString, IEnumerable<string> files) => GetFileVersion(firstMatchedString);

        /// <summary>
        /// Get the assembly version as determined by an embedded assembly manifest
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetManifestVersion(byte[] fileContent)
        {
            // Read in the manifest to a string
            string manifestString = GetEmbeddedAssemblyManifest(fileContent);
            if (string.IsNullOrWhiteSpace(manifestString))
                return null;

            // Try to read the XML in from the string
            try
            {
                // Try to read the assembly
                var assemblyNode = GetAssemblyNode(manifestString);
                if (assemblyNode == null)
                    return null;

                // Try to read the assemblyIdentity
                var assemblyIdentityNode = assemblyNode["assemblyIdentity"];
                if (assemblyIdentityNode == null)
                    return null;
                
                // Return the version attribute, if possible
                return assemblyIdentityNode.GetAttribute("version");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the assembly version as determined by an embedded assembly manifest
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetManifestDescription(byte[] fileContent)
        {
            // Read in the manifest to a string
            string manifestString = GetEmbeddedAssemblyManifest(fileContent);
            if (string.IsNullOrWhiteSpace(manifestString))
                return null;

            // Try to read the XML in from the string
            try
            {
                // Try to read the assembly
                var assemblyNode = GetAssemblyNode(manifestString);
                if (assemblyNode == null)
                    return null;

                // Return the content of the description node, if possible
                var descriptionNode = assemblyNode["description"];
                if (descriptionNode == null)
                    return null;
                    
                return descriptionNode.InnerXml;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the embedded assembly manifest
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Embedded assembly manifest as a string, if possible</returns>
        /// <remarks>
        /// TODO: How do we find the manifest specifically better
        /// TODO: This should be derived specifically in the .rsrc section
        /// </remarks>
        private static string GetEmbeddedAssemblyManifest(byte[] fileContent)
        {
            // <?xml
            byte?[] manifestStart = new byte?[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C };
            if (!fileContent.LastPosition(manifestStart, out int manifestStartPosition))
                return null;

            // </assembly>
            byte?[] manifestEnd = new byte?[] { 0x3C, 0x2F, 0x61, 0x73, 0x73, 0x65, 0x6D, 0x62, 0x6C, 0x79, 0x3E };
            if (!fileContent.FirstPosition(manifestEnd, out int manifestEndPosition, start: manifestStartPosition))
                return null;

            // Read in the manifest to a string
            int manifestLength = manifestEndPosition + "</assembly>".Length - manifestStartPosition;
            return Encoding.ASCII.GetString(fileContent, manifestStartPosition, manifestLength);
        }

        /// <summary>
        /// Get the assembly identity node from an embedded manifest
        /// </summary>
        /// <param name="manifestString">String representing the XML document</param>
        /// <returns>Assembly identity node, if possible</returns>
        private static XmlElement GetAssemblyNode(string manifestString)
        {
            // An invalid string means we can't read it
            if (string.IsNullOrWhiteSpace(manifestString))
                return null;

            try
            {
                // Load the XML string as a document
                var manifestDoc = new XmlDocument();
                manifestDoc.LoadXml(manifestString);

                // If the XML has no children, it's invalid
                if (!manifestDoc.HasChildNodes)
                    return null;

                // Try to read the assembly node
                return manifestDoc["assembly"];
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
