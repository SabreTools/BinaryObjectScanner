using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BurnOutSharp
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
        public static void AppendToDictionary(Dictionary<string, List<string>> original, string key, string value)
        {
            AppendToDictionary(original, key, new List<string> { value });
        }

        /// <summary>
        /// Append one result to a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="key">Key to add information to</param>
        /// <param name="value">String value to add</param>
        public static void AppendToDictionary(Dictionary<string, List<string>> original, string key, List<string> values)
        {
            // If the dictionary is null, just return
            if (original == null)
                return;

            // Use a placeholder value if the key is null
            key = key ?? "NO FILENAME";

            // Add the key if needed and then append the lists
            if (!original.ContainsKey(key))
                original[key] = new List<string>();

            original[key].AddRange(values);
        }

        /// <summary>
        /// Append one results dictionary to another
        /// </summary>
        /// <param name="original">Dictionary to append to</param>
        /// <param name="addition">Dictionary to pull from</param>
        public static void AppendToDictionary(Dictionary<string, List<string>> original, Dictionary<string, List<string>> addition)
        {
            // If either dictionary is missing, just return
            if (original == null || addition == null)
                return;

            // Loop through each of the addition keys and add accordingly
            foreach (string key in addition.Keys)
            {
                if (!original.ContainsKey(key))
                    original[key] = new List<string>();

                original[key].AddRange(addition[key]);
            }
        }

        /// <summary>
        /// Remove empty or null keys from a results dictionary
        /// </summary>
        /// <param name="original">Dictionary to clean</param>
        public static void ClearEmptyKeys(Dictionary<string, List<string>> original)
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
                    original.Remove(key);
            }
        }

        /// <summary>
        /// Prepend a parent path from dictionary keys, if possible
        /// </summary>
        /// <param name="original">Dictionary to strip values from</param>
        /// <param name="pathToPrepend">Path to strip from the keys</param>
        public static void PrependToKeys(Dictionary<string, List<string>> original, string pathToPrepend)
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
                original.Remove(currentKey);
            }
        }

        /// <summary>
        /// Strip a parent path from dictionary keys, if possible
        /// </summary>
        /// <param name="original">Dictionary to strip values from</param>
        /// <param name="pathToStrip">Path to strip from the keys</param>
        public static void StripFromKeys(Dictionary<string, List<string>> original, string pathToStrip)
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
                original.Remove(currentKey);
            }
        }

        #endregion

        #region Byte Arrays

        /// <summary>
        /// Find the first position of one array in another, if possible
        /// </summary>
        public static bool FirstPosition(this byte[] stack, byte[] needle, out int position, int start = 0, int end = -1)
        {
            (bool found, int foundPosition) = FindPosition(stack, needle, start, end, false);
            position = foundPosition;
            return found;
        }

        /// <summary>
        /// Find the last position of one array in another, if possible
        /// </summary>
        public static bool LastPosition(this byte[] stack, byte[] needle, out int position, int start = 0, int end = -1)
        {
            (bool found, int foundPosition) = FindPosition(stack, needle, start, end, true);
            position = foundPosition;
            return found;
        }

        /// <summary>
        /// See if a byte array starts with another
        /// </summary>
        public static bool StartsWith(this byte[] stack, byte[] needle)
        {
            return stack.FirstPosition(needle, out int _, start: 0, end: 1);
        }

        /// <summary>
        /// See if a byte array ends with another
        /// </summary>
        public static bool EndsWith(this byte[] stack, byte[] needle)
        {
            return stack.FirstPosition(needle, out int _, start: stack.Length - needle.Length);
        }
        
        /// <summary>
        /// Find the position of one array in another, if possible
        /// </summary>
        private static (bool, int) FindPosition(byte[] stack, byte[] needle, int start, int end, bool reverse)
        {
            // If either array is null or empty, we can't do anything
            if (stack == null || stack.Length == 0 || needle == null || needle.Length == 0)
                return (false, -1);

            // If the needle array is larger than the stack array, it can't be contained within
            if (needle.Length > stack.Length)
                return (false, -1);

            // If start or end are not set properly, set them to defaults
            if (start < 0)
                start = 0;
            if (end < 0)
                end = stack.Length - needle.Length;

            for (int i = reverse ? end : start; reverse ? i > start : i < end; i += reverse ? -1 : 1)
            {
                if (stack.EqualAt(needle, i))
                    return (true, i);
            }

            return (false, -1);
        }

        /// <summary>
        /// Get if a stack at a certain index is equal to a needle
        /// </summary>
        private static bool EqualAt(this byte[] stack, byte[] needle, int index)
        {
            // If we're too close to the end of the stack, return false
            if (needle.Length >= stack.Length - index)
                return false;

            for (int i = 0; i < needle.Length; i++)
            {
                if (stack[i + index] != needle[i])
                    return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Get the file version as reported by the filesystem
        /// </summary>
        public static string GetFileVersion(string file)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            FileVersionInfo fvinfo = FileVersionInfo.GetVersionInfo(file);
            if (fvinfo.FileVersion == null)
                return "";
            if (fvinfo.FileVersion != "")
                return fvinfo.FileVersion.Replace(", ", ".");
            else
                return fvinfo.ProductVersion.Replace(", ", ".");
        }

        /// <summary>
        /// Get the assembly version as determined by an embedded assembly manifest
        /// </summary>
        /// <remarks>TODO: How do we find the manifest specifically better?</remarks>
        public static string GetManifestVersion(byte[] fileContent)
        {
            // <?xml
            byte[] manifestStart = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C };
            if (!fileContent.LastPosition(manifestStart, out int manifestStartPosition))
                return null;
            
            // </assembly>
            byte[] manifestEnd = new byte[] { 0x3C, 0x2F, 0x61, 0x73, 0x73, 0x65, 0x6D, 0x62, 0x6C, 0x79, 0x3E };
            if (!fileContent.FirstPosition(manifestEnd, out int manifestEndPosition, start: manifestStartPosition))
                return null;
            
            // Read in the manifest to a string
            int manifestLength = manifestEndPosition + "</assembly>".Length - manifestStartPosition;
            string manifestString = Encoding.ASCII.GetString(fileContent, manifestStartPosition, manifestLength);

            // Try to read the XML in from the string
            try
            {
                // Load the XML string as a document
                var manifestDoc = new XmlDocument();
                manifestDoc.LoadXml(manifestString);

                // If the XML has no children, it's invalid
                if (!manifestDoc.HasChildNodes)
                    return null;
                
                // Try to read the assembly node
                var assemblyNode = manifestDoc["assembly"];
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
    }
}
