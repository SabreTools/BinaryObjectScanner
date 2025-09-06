using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Portable executable (PE)
    /// </summary>
    public class PortableExecutable : Executable<SabreTools.Serialization.Wrappers.PortableExecutable>
    {
        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Try to create a wrapper for the proper executable type
            SabreTools.Serialization.Interfaces.IWrapper? wrapper;
            try
            {
                wrapper = WrapperFactory.CreateExecutableWrapper(stream);
                if (wrapper == null)
                    return null;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return null;
            }

            // Only use generic content checks if we're in debug mode
            if (includeDebug)
            {
                var contentProtections = RunContentChecks(file, stream, includeDebug);
                protections.Append(file, contentProtections.Values);
            }

            // Only handle PE
            if (wrapper is not SabreTools.Serialization.Wrappers.PortableExecutable pex)
                return null;

            // Standard checks
            var subProtections
                = RunExecutableChecks(file, pex, StaticChecks.PortableExecutableCheckClasses, includeDebug);
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

            // Only handle PE
            if (wrapper is not SabreTools.Serialization.Wrappers.PortableExecutable pex)
                return false;

            // Create the output directory
            Directory.CreateDirectory(outDir);

            // Extract all files
            bool extractAny = false;
            if (new Packer.CExe().CheckExecutable(file, pex, includeDebug) != null)
                extractAny |= pex.ExtractCExe(outDir, includeDebug);

            if (new Packer.EmbeddedFile().CheckExecutable(file, pex, includeDebug) != null)
            {
                extractAny |= pex.ExtractFromOverlay(outDir, includeDebug);
                extractAny |= pex.ExtractFromResources(outDir, includeDebug);
            }

            if (new Packer.WiseInstaller().CheckExecutable(file, pex, includeDebug) != null)
                extractAny |= pex.ExtractWise(outDir, includeDebug);

            return extractAny;
        }
    }
}
