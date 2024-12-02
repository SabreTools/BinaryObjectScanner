using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <see href="https://zhidao.baidu.com/question/211454017.html"/>
    /// <see href="https://bbs.pediy.com/thread-62414-3.htm"/>
    /// <see href="https://bbs.pediy.com/thread-141554.htm"/>
    /// <see href="https://forum.arabhardware.net/showthread.php?t=45360"/>
    /// <see href="https://forum.arabhardware.net/showthread.php?t=45360&p=304085"/>
    public class SoftLock : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            var name = pex.InternalName;
            if (name.OptionalEquals("Softlock Protected Application"))
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            name = pex.Comments;
            if (name.OptionalEquals("Softlock Protected Application"))
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (pex.FindStringTableByEntry("Softlock CD").Count > 0)
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (pex.FindStringTableByEntry("Softlock USB Key").Count > 0)
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (pex.FindDialogByTitle("Softlock Protection Kit").Count > 0)
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (pex.FindDialogByTitle("About Softlock Protected Application").Count > 0)
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (pex.FindDialogBoxByItemTitle("www.softlock.net").Count > 0)
                return "SoftLock";

            // TODO: See if the version number is anywhere else
            // TODO: Parse the version number out of the dialog box item
            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (pex.FindDialogBoxByItemTitle("Softlock Protected Application Version 1.0").Count > 0)
                return "SoftLock";

            // There are many mentions of USB dongle and CD protection in the various string tables
            // and dialog boxes. See if any of those are unique to SoftLock.

            // Found in "TafseerVer4.exe" in IA item "TAFSEERVER4SETUP"
            var strings = pex.GetFirstSectionStrings(".section") ?? [];
            if (strings.Exists(s => s.Contains("SOFTLOCKPROTECTION")))
                return "SoftLock";

            // Investigate if the ".section" section is an indicator of SoftLock

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new List<PathMatch>
                {
                    new FilePathMatch("SOFTLOCKC.dat"),
                    new FilePathMatch("SOFTLOCKI.dat"),
                }, "SoftLock"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("SOFTLOCKC.dat"), "SoftLock"),
                new(new FilePathMatch("SOFTLOCKI.dat"), "SoftLock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
