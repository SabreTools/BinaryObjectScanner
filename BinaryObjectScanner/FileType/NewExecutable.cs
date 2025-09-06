using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// New executable (NE)
    /// </summary>
    public class NewExecutable : Executable<SabreTools.Serialization.Wrappers.NewExecutable>
    {
        /// <inheritdoc/>
        public NewExecutable(SabreTools.Serialization.Wrappers.NewExecutable? wrapper) : base(wrapper) { }

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
                = RunExecutableChecks(file, _wrapper, StaticChecks.NewExecutableCheckClasses, includeDebug);
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
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var wrapper = WrapperFactory.CreateExecutableWrapper(stream);
            if (wrapper == null)
                return false;

            // Only handle NE
            if (wrapper is not SabreTools.Serialization.Wrappers.NewExecutable nex)
                return false;

            // Create the output directory
            Directory.CreateDirectory(outDir);

            // Extract all files
            bool extractAny = false;
            if (new Packer.EmbeddedFile().CheckExecutable(file, nex, includeDebug) != null)
                extractAny |= nex.ExtractFromOverlay(outDir, includeDebug);

            if (new Packer.WiseInstaller().CheckExecutable(file, nex, includeDebug) != null)
                extractAny |= nex.ExtractWise(outDir, includeDebug);

            return extractAny;
        }
    }
}
