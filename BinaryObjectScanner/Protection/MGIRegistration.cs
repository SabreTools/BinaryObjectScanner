using System;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class MGIRegistration : IPortableExecutableCheck
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

            var name = pex.ProductName;

            // Found in "Register.dll" in IA item "MGIPhotoSuite4.0AndPhotoVista2.02001".
            if (name?.Equals("MGI Registration Utility", StringComparison.Ordinal) == true)
                return $"MGI Registration {pex.GetInternalVersion()}";

            // Found in "Register.dll" from "VideoWaveIII" in IA item "mgi-videowave-iii-version-3.00-mgi-software-2000".
            var resources = pex.FindStringTableByEntry("MGI Registration");
            if (resources.Any())
                return "MGI Registration";

            // Found in "Register.dll" in IA item "MGIPhotoSuite4.0AndPhotoVista2.02001".
            resources = pex.FindStringTableByEntry("Register@register.mgisoft.com");
            if (resources.Any())
                return "MGI Registration";

            return null;
        }
    }
}
