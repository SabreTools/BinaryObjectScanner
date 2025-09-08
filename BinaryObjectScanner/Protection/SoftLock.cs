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
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            var name = exe.InternalName;
            if (name.OptionalEquals("Softlock Protected Application"))
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            name = exe.Comments;
            if (name.OptionalEquals("Softlock Protected Application"))
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (exe.FindStringTableByEntry("Softlock CD").Count > 0)
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (exe.FindStringTableByEntry("Softlock USB Key").Count > 0)
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (exe.FindDialogByTitle("Softlock Protection Kit").Count > 0)
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (exe.FindDialogByTitle("About Softlock Protected Application").Count > 0)
                return "SoftLock";

            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (exe.FindDialogBoxByItemTitle("www.softlock.net").Count > 0)
                return "SoftLock";

            // TODO: See if the version number is anywhere else
            // TODO: Parse the version number out of the dialog box item
            // Found in "IALib.DLL" in IA item "TAFSEERVER4SETUP"
            if (exe.FindDialogBoxByItemTitle("Softlock Protected Application Version 1.0").Count > 0)
                return "SoftLock";

            // There are many mentions of USB dongle and CD protection in the various string tables
            // and dialog boxes. See if any of those are unique to SoftLock.

            // Found in "TafseerVer4.exe" in IA item "TAFSEERVER4SETUP"
            var strings = exe.GetFirstSectionStrings(".section") ?? [];
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
