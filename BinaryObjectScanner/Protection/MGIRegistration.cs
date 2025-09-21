using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class MGIRegistration : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.ProductName;

            // Found in "Register.dll" in IA item "MGIPhotoSuite4.0AndPhotoVista2.02001".
            if (name.OptionalEquals("MGI Registration Utility", StringComparison.Ordinal))
                return $"MGI Registration {exe.GetInternalVersion()}";

            // Found in "Register.dll" from "VideoWaveIII" in IA item "mgi-videowave-iii-version-3.00-mgi-software-2000".
            if (exe.FindStringTableByEntry("MGI Registration").Count > 0)
                return "MGI Registration";

            // Found in "Register.dll" in IA item "MGIPhotoSuite4.0AndPhotoVista2.02001".
            if (exe.FindStringTableByEntry("Register@register.mgisoft.com").Count > 0)
                return "MGI Registration";

            return null;
        }
    }
}
