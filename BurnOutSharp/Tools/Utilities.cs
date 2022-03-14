using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.ExecutableType.Microsoft.PE.Entries;
using BurnOutSharp.ExecutableType.Microsoft.PE.Sections;
using BurnOutSharp.ExecutableType.Microsoft.PE.Tables;
using BurnOutSharp.ExecutableType.Microsoft.Resources;

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

        #region Executable Information

        /// <summary>
        /// Get the company name as reported by the filesystem
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Company name string, null on error</returns>
        public static string GetCompanyName(PortableExecutable pex) => GetResourceString(pex, "CompanyName");

        /// <summary>
        /// Get the file description as reported by the filesystem
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Description string, null on error</returns>
        public static string GetFileDescription(PortableExecutable pex) => GetResourceString(pex, "FileDescription");

        /// <summary>
        /// Get the file version as reported by the filesystem
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetFileVersion(byte[] fileContent)
        {
            if (fileContent == null || !fileContent.Any())
                return null;

            return GetFileVersion(new PortableExecutable(fileContent, 0));
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
        /// Get the file version as reported by the filesystem
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetFileVersion(PortableExecutable pex)
        {
            string version = GetResourceString(pex, "FileVersion");
            if (!string.IsNullOrWhiteSpace(version))
                return version.Replace(", ", ".");

            version = GetResourceString(pex, "ProductVersion");
            if (!string.IsNullOrWhiteSpace(version))
                return version.Replace(", ", ".");

            return null;
        }

        /// <summary>
        /// Wrapper for GetFileVersion for use in content matching
        /// </summary>
        /// <param name="file">File to check for version</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="positions">Last matched positions in the contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetFileVersion(string file, byte[] fileContent, List<int> positions) => GetFileVersion(fileContent);

        /// <summary>
        /// Wrapper for GetFileVersion for use in path matching
        /// </summary>
        /// <param name="firstMatchedString">File to check for version</param>
        /// <param name="files">Full list of input paths</param>
        /// <returns>Version string, null on error</returns>
        public static string GetFileVersion(string firstMatchedString, IEnumerable<string> files) => GetFileVersion(firstMatchedString);

        /// <summary>
        /// Get the internal name as reported by the filesystem
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Internal name string, null on error</returns>
        public static string GetInternalName(PortableExecutable pex) => GetResourceString(pex, "InternalName");

        /// <summary>
        /// Get the legal copyright as reported by the filesystem
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Legal copyright string, null on error</returns>
        public static string GetLegalCopyright(PortableExecutable pex) => GetResourceString(pex, "LegalCopyright");

        /// <summary>
        /// Get the assembly version as determined by an embedded assembly manifest
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetManifestDescription(PortableExecutable pex)
        {
            // If we don't have a PE executable, just return null
            var resourceSection = pex?.ResourceSection;
            if (resourceSection == null)
                return null;
            
            // Read in the manifest to a string
            string manifestString = FindAssemblyManifest(pex.ResourceSection);
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
        /// Get the assembly version as determined by an embedded assembly manifest
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetManifestVersion(PortableExecutable pex)
        {
            // If we don't have a PE executable, just return null
            var resourceSection = pex?.ResourceSection;
            if (resourceSection == null)
                return null;
            
            // Read in the manifest to a string
            string manifestString = FindAssemblyManifest(pex.ResourceSection);
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
        /// Get the original filename as reported by the filesystem
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Original filename string, null on error</returns>
        public static string GetOriginalFileName(PortableExecutable pex) => GetResourceString(pex, "OriginalFileName");

        /// <summary>
        /// Get the product name as reported by the filesystem
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Product name string, null on error</returns>
        public static string GetProductName(PortableExecutable pex) => GetResourceString(pex, "ProductName");

        /// <summary>
        /// Find resource data in a ResourceSection, if possible
        /// </summary>
        /// <param name="rs">ResourceSection from the executable</param>
        /// <param name="dataStart">String to use if checking for data starting with a string</param>
        /// <param name="dataContains">String to use if checking for data contains a string</param>
        /// <param name="dataEnd">String to use if checking for data ending with a string</param>
        /// <returns>Full encoded resource data, null on error</returns>
        public static ResourceDataEntry FindResourceInSection(ResourceSection rs, string dataStart = null, string dataContains = null, string dataEnd = null)
        {
            if (rs == null)
                return null;

            return FindResourceInTable(rs.ResourceDirectoryTable, dataStart, dataContains, dataEnd);
        }

        /// <summary>
        /// Find resource data in a ResourceDirectoryTable, if possible
        /// </summary>
        /// <param name="rdt">ResourceDirectoryTable representing a layer</param>
        /// <param name="dataStart">String to use if checking for data starting with a string</param>
        /// <param name="dataContains">String to use if checking for data contains a string</param>
        /// <param name="dataEnd">String to use if checking for data ending with a string</param>
        /// <returns>Full encoded resource data, null on error</returns>
        private static ResourceDataEntry FindResourceInTable(ResourceDirectoryTable rdt, string dataStart, string dataContains, string dataEnd)
        {
            if (rdt == null)
                return null;

            try
            {
                foreach (var rdte in rdt.NamedEntries)
                {
                    if (rdte.IsResourceDataEntry() && rdte.DataEntry != null)
                    {
                        if (dataStart != null && rdte.DataEntry.DataAsUTF8String.StartsWith(dataStart))
                            return rdte.DataEntry;
                        else if (dataContains != null && rdte.DataEntry.DataAsUTF8String.Contains(dataContains))
                            return rdte.DataEntry;
                        else if (dataEnd != null && rdte.DataEntry.DataAsUTF8String.EndsWith(dataStart))
                            return rdte.DataEntry;
                    }
                    else
                    {
                        var manifest = FindResourceInTable(rdte.Subdirectory, dataStart, dataContains, dataEnd);
                        if (manifest != null)
                            return manifest;
                    }
                }

                foreach (var rdte in rdt.IdEntries)
                {
                    if (rdte.IsResourceDataEntry() && rdte.DataEntry != null)
                    {
                        if (dataStart != null && rdte.DataEntry.DataAsUTF8String.StartsWith(dataStart))
                            return rdte.DataEntry;
                        else if (dataContains != null && rdte.DataEntry.DataAsUTF8String.Contains(dataContains))
                            return rdte.DataEntry;
                        else if (dataEnd != null && rdte.DataEntry.DataAsUTF8String.EndsWith(dataStart))
                            return rdte.DataEntry;
                    }
                    else
                    {
                        var manifest = FindResourceInTable(rdte.Subdirectory, dataStart, dataContains, dataEnd);
                        if (manifest != null)
                            return manifest;
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Find the assembly manifest from a resource section, if possible
        /// </summary>
        /// <param name="rs">ResourceSection from the executable</param>
        /// <returns>Full assembly manifest, null on error</returns>
        private static string FindAssemblyManifest(ResourceSection rs) => FindResourceInSection(rs, dataContains: "<assembly")?.DataAsUTF8String;

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
            catch (Exception ex)
            {
                return null;
            }
        }

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
        /// Get a resource string from the version info
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Original filename string, null on error</returns>
        private static string GetResourceString(PortableExecutable pex, string key)
        {
            var resourceStrings = GetVersionInfo(pex)?.ChildrenStringFileInfo?.Children?.Children;
            if (resourceStrings == null)
                return null;
            
            var value = resourceStrings.FirstOrDefault(s => s.Key == key);
            if (!string.IsNullOrWhiteSpace(value?.Value))
                return value.Value.Trim(' ', '\0');

            return null;
        }

        /// <summary>
        /// Get the version info object related to file contents, if possible
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>VersionInfo object on success, null on error</returns>
        private static VersionInfo GetVersionInfo(PortableExecutable pex)
        {
            // If we don't have a PE executable, just return null
            var resourceSection = pex?.ResourceSection;
            if (resourceSection == null)
                return null;

            // Try to get the matching resource
            var resource = FindResourceInSection(resourceSection, dataContains: "V\0S\0_\0V\0E\0R\0S\0I\0O\0N\0_\0I\0N\0F\0O\0");
            if (resource?.Data == null)
                return null;

            try
            {
                int index = 0;
                return VersionInfo.Deserialize(resource.Data, ref index);
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex);
                return null;
            }
        }

        #endregion
    }
}
