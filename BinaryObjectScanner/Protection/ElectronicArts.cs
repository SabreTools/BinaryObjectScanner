using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class ElectronicArts : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            var name = pex.FileDescription;
            if (name.OptionalContains("EReg MFC Application"))
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";
            else if (name.OptionalContains("Registration code installer program"))
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";
            else if (name.OptionalEquals("EA DRM Helper", StringComparison.OrdinalIgnoreCase))
                return $"EA DRM Protection {pex.GetInternalVersion()}";

            name = pex.InternalName;
            if (name.OptionalEquals("CDCode", StringComparison.Ordinal))
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";

            if (pex.FindDialogByTitle("About CDKey").Count > 0)
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";
            else if (pex.FindGenericResource("About CDKey").Count > 0)
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("EReg Config Form")))
                    return "EA CdKey Registration Module";
            }

            // Get the .rdata section strings, if they exist
            strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("GenericEA")) && strs.Exists(s => s.Contains("Activation")))
                    return "EA DRM Protection";
            }

            // Get the .rdata section strings, if they exist
            strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("GenericEA")) && strs.Exists(s => s.Contains("Activation")))
                    return "EA DRM Protection";
            }

            return null;
        }
    }
}
