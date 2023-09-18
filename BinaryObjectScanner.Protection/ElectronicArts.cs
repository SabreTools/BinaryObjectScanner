using System;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class ElectronicArts : IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name?.Contains("EReg MFC Application") == true)
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";
            else if (name?.Contains("Registration code installer program") == true)
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";
            else if (name?.Equals("EA DRM Helper", StringComparison.OrdinalIgnoreCase) == true)
                return $"EA DRM Protection {pex.GetInternalVersion()}";

            name = pex.InternalName;
            if (name?.Equals("CDCode", StringComparison.Ordinal) == true)
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";

            if (pex.FindDialogByTitle("About CDKey").Any())
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";
            else if (pex.FindGenericResource("About CDKey").Any())
                return $"EA CdKey Registration Module {pex.GetInternalVersion()}";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("EReg Config Form")))
                    return "EA CdKey Registration Module";
            }

            // Get the .rdata section strings, if they exist
            strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("GenericEA")) && strs.Any(s => s.Contains("Activation")))
                    return "EA DRM Protection";
            }

            // Get the .rdata section strings, if they exist
            strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("GenericEA")) && strs.Any(s => s.Contains("Activation")))
                    return "EA DRM Protection";
            }

            return null;
        }
    }
}
