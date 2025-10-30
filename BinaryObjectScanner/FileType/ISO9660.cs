using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// ISO9660
    /// </summary>
    public class ISO9660 : ISO<SabreTools.Serialization.Wrappers.ISO9660>
    {
        /// <inheritdoc/>
        public ISO9660(SabreTools.Serialization.Wrappers.ISO9660 wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Standard checks
            var subProtections
                = RunISOChecks(file, _wrapper, StaticChecks.ISO9660CheckClasses, includeDebug);
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