﻿using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // http://www.crypkey.com/products/cdlock/cdmain.html
    // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/CrypKey%20Installer.1.sg
    // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/CrypKey.2.sg
    // https://github.com/wolfram77web/app-peid/blob/master/userdb.txt
    public class CrypKey : IPortableExecutableCheck
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

            // Get the CrypKey version from the VersionInfo, if it exists
            string version = pex.GetVersionInfoString("CrypKey Version") ?? string.Empty;

            // Found in 'cki32k.dll'
            var name = pex.CompanyName;
            if (name?.StartsWith("CrypKey") == true)
                return $"CrypKey {version}".TrimEnd();

            // Found in 'cki32k.dll'
            name = pex.FileDescription;
            if (name?.StartsWith("CrypKey") == true)
                return $"CrypKey {version}".TrimEnd();

            // Found in 'cki32k.dll'
            name = pex.LegalCopyright;
            if (name?.Contains("CrypKey") == true)
                return $"CrypKey {version}".TrimEnd();

            // Found in 'cki32k.dll'
            if (!string.IsNullOrEmpty(version))
                return $"CrypKey {version}".TrimEnd();

            // TODO: Look into the `.loader`,`.wreloc`, `.widata`, and `.hooks` sections

            return null;
        }
    }
}