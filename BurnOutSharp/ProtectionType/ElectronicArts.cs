using System;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class ElectronicArts : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.Contains("EReg MFC Application") == true)
                return $"EA CdKey Registration Module {Tools.Utilities.GetInternalVersion(pex)}";
            else if (name?.Contains("Registration code installer program") == true)
                return $"EA CdKey Registration Module {Tools.Utilities.GetInternalVersion(pex)}";
            else if (name?.Equals("EA DRM Helper", StringComparison.OrdinalIgnoreCase) == true)
                return $"EA DRM Protection {Tools.Utilities.GetInternalVersion(pex)}";

            name = pex.InternalName;
            if (name?.Equals("CDCode", StringComparison.Ordinal) == true)
                return $"EA CdKey Registration Module {Tools.Utilities.GetInternalVersion(pex)}";

            if (pex.FindDialogByTitle("About CDKey").Any())
                return $"EA CdKey Registration Module {Tools.Utilities.GetInternalVersion(pex)}";
            else if (pex.FindGenericResource("About CDKey").Any())
                    return $"EA CdKey Registration Module {Tools.Utilities.GetInternalVersion(pex)}";

            // Get the .data/DATA section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
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
