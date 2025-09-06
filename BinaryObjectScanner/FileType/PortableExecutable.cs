using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Portable executable (PE)
    /// </summary>
    public class PortableExecutable : Executable<SabreTools.Serialization.Wrappers.PortableExecutable>, IExtractable
    {
        /// <inheritdoc/>
        public PortableExecutable(SabreTools.Serialization.Wrappers.PortableExecutable? wrapper) : base(wrapper) { }

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
                = RunExecutableChecks(file, _wrapper, StaticChecks.PortableExecutableCheckClasses, includeDebug);
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

        /// <inheritdoc/>
        /// <remarks>Uses the already-generated wrapper</remarks>
        public bool Extract(string file, string outDir, bool includeDebug)
            => Extract(null, file, outDir, includeDebug);

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the output directory
            Directory.CreateDirectory(outDir);

            // Extract all files
            bool extractAny = false;
            if (new Packer.CExe().CheckExecutable(file, _wrapper, includeDebug) != null)
                extractAny |= _wrapper.ExtractCExe(outDir, includeDebug);

            if (new Packer.EmbeddedFile().CheckExecutable(file, _wrapper, includeDebug) != null)
            {
                extractAny |= _wrapper.ExtractFromOverlay(outDir, includeDebug);
                extractAny |= _wrapper.ExtractFromResources(outDir, includeDebug);
            }

            if (new Packer.WiseInstaller().CheckExecutable(file, _wrapper, includeDebug) != null)
                extractAny |= _wrapper.ExtractWise(outDir, includeDebug);

            return extractAny;
        }
    }
}
