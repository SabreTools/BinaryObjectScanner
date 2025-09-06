using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // http://www.crypkey.com/products/cdlock/cdmain.html
    // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/CrypKey%20Installer.1.sg
    // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/CrypKey.2.sg
    // https://github.com/wolfram77web/app-peid/blob/master/userdb.txt
    public class CrypKey : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the code/CODE section strings, if they exist
            var strs = pex.GetFirstSectionStrings("code") ?? pex.GetFirstSectionStrings("CODE");
            if (strs != null)
            {
                // Found in "NECRO95.EXE" in IA item "NBECRORV11".
                // Full string:
                // *CrypKey Instant 2.0 security i(32 - bit)  *
                // *Copyright(c) 1996 Kenonic Controls Ltd.  *
                if (strs.Exists(s => s.Contains("CrypKey Instant 2.0 security")))
                    return "CrypKey Instant 2.0";

                // Generic check to catch unknown CrypKey Instant versions.
                if (strs.Exists(s => s.Contains("CrypKey Instant")))
                    return "CrypKey Instant (Unknown version - Please report to us on GitHub)";

                // Generic check to catch unknown CrypKey products.
                if (strs.Exists(s => s.Contains("CrypKey")))
                    return "CrypKey (Unknown version - Please report to us on GitHub)";
            }

            // Get the CrypKey version from the VersionInfo, if it exists
            string version = pex.GetVersionInfoString("CrypKey Version") ?? string.Empty;

            // Found in 'cki32k.dll'
            var name = pex.CompanyName;
            if (name.OptionalStartsWith("CrypKey"))
                return $"CrypKey {version}".TrimEnd();

            name = pex.FileDescription;

            // Found in "CKSEC_32.DLL" in IA item "NBECRORV11".
            if (name.OptionalStartsWith("CrypKey Instant security library"))
                return $"CrypKey Instant {pex.GetInternalVersion()}";

            // Found in 'cki32k.dll'
            if (name.OptionalStartsWith("CrypKey"))
                return $"CrypKey {version}".TrimEnd();

            // Found in 'cki32k.dll'
            name = pex.LegalCopyright;
            if (name.OptionalContains("CrypKey"))
                return $"CrypKey {version}".TrimEnd();

            // Found in 'cki32k.dll'
            if (!string.IsNullOrEmpty(version))
                return $"CrypKey {version}".TrimEnd();

            // TODO: Look into the `.loader`,`.wreloc`, `.widata`, and `.hooks` sections

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "NBECRORV11".
                new(new FilePathMatch("CKLICENS.HLP"), "CrypKey"),
                new(new FilePathMatch("CKSEC_32.DLL"), "CrypKey"),
                new(new FilePathMatch("CRYP95.DLL"), "CrypKey"),
                new(new FilePathMatch("CRYP9516.DLL"), "CrypKey"),
                new(new FilePathMatch("CRYPKEY.HLP"), "CrypKey"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in IA item "NBECRORV11".
                new(new FilePathMatch("CKLICENS.HLP"), "CrypKey"),
                new(new FilePathMatch("CKSEC_32.DLL"), "CrypKey"),
                new(new FilePathMatch("CRYP95.DLL"), "CrypKey"),
                new(new FilePathMatch("CRYP9516.DLL"), "CrypKey"),
                new(new FilePathMatch("CRYPKEY.HLP"), "CrypKey"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
