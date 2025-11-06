using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Portable executable (PE)
    /// </summary>
    public class PortableExecutable : Executable<SabreTools.Serialization.Wrappers.PortableExecutable>
    {
        /// <inheritdoc/>
        public PortableExecutable(SabreTools.Serialization.Wrappers.PortableExecutable wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Only use generic content checks if we're in debug mode
            if (includeDebug)
            {
                var contentProtections = RunContentChecks(file, stream, includeDebug);
                protections.Append(file, contentProtections.Values);
            }

            // Standard checks
            var subProtections
                = RunExecutableChecks(file, StaticChecks.PortableExecutableCheckClasses, includeDebug);
            protections.Append(file, subProtections.Values);

            // If there are no protections
            if (protections.Count == 0)
                return null;

            // Create the internal list
            var protectionList = new List<string>();
            foreach (string key in protections.Keys)
            {
                protectionList.AddRange(protections[key]);
            }

            return string.Join(";", [.. protectionList]);
        }
    }
}
