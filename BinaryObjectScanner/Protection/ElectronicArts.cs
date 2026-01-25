using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class ElectronicArts : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            if (name.OptionalContains("EReg MFC Application"))
                return $"EA CdKey Registration Module {exe.GetInternalVersion()}";
            else if (name.OptionalContains("Registration code installer program"))
                return $"EA CdKey Registration Module {exe.GetInternalVersion()}";
            else if (name.OptionalEquals("EA DRM Helper", StringComparison.OrdinalIgnoreCase))
                return $"EA DRM Protection {exe.GetInternalVersion()}";

            name = exe.InternalName;

            if (name.OptionalEquals("CDCode", StringComparison.Ordinal))
                return $"EA CdKey Registration Module {exe.GetInternalVersion()}";

            if (exe.FindDialogByTitle("About CDKey").Count > 0)
                return $"EA CdKey Registration Module {exe.GetInternalVersion()}";
            else if (exe.FindGenericResource("About CDKey").Count > 0)
                return $"EA CdKey Registration Module {exe.GetInternalVersion()}";

            // Get the .data/DATA section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs is not null)
            {
                if (strs.Exists(s => s.Contains("EReg Config Form")))
                    return "EA CdKey Registration Module";
            }

            // Get the .rdata section strings, if they exist
            strs = exe.GetFirstSectionStrings(".rdata");
            if (strs is not null)
            {
                if (strs.Exists(s => s.Contains("GenericEA")) && strs.Exists(s => s.Contains("Activation")))
                    return "EA DRM Protection";
            }

            // Get the .rdata section strings, if they exist
            strs = exe.GetFirstSectionStrings(".text");
            if (strs is not null)
            {
                if (strs.Exists(s => s.Contains("GenericEA")) && strs.Exists(s => s.Contains("Activation")))
                    return "EA DRM Protection";
            }

            return null;
        }
    }
}
