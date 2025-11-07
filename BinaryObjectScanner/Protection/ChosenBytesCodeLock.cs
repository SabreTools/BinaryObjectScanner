using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Code-Lock by ChosenBytes (https://web.archive.org/web/20040225072021/http://chosenbytes.com/) is a form of DRM that protects Visual Basic and .NET programs.
    /// It requires the use of a registration code that is unique to each individual user (https://web.archive.org/web/20031210093259/http://www.chosenbytes.com/codemain.php).
    /// As an advertising point, ChosenBytes apparently held a contest with a cash prize to see if a protected file could be cracked (https://web.archive.org/web/20031106163334/http://www.chosenbytes.com/challenge.php).
    ///
    /// Previous versions of BinaryObjectScanner incorrectly reported this DRM as "CodeLock / CodeLok / CopyLok". It was later discovered that due to the similar names, two entirely different DRM were erroneously lumped together.
    /// Not only is "CodeLok / CopyLok" an entirely separate form of DRM, but "CodeLock" (as opposed to "Code-Lock") appears to refer specifically to another unrelated DRM (https://web.archive.org/web/20031106033758/http://www.codelock.co.nz/).
    /// Also not to be confused with https://www.youtube.com/watch?v=AHqdgk0uJyc.
    ///
    /// Code-Lock FAQ: https://web.archive.org/web/20041205165232/http://www.chosenbytes.com/codefaq.php
    /// Download (Version 2.35 trial): https://web.archive.org/web/20060220121200/http://www.chosenbytes.com:80/Code-Lock_cnet.zip
    /// </summary>
    public class ChosenBytesCodeLock : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.ProductName;

            // Found in "Code-Lock.ocx" in Code-Lock version 2.35.
            // Also worth noting is the File Description for this file, which is "A future for you, a challenge for the rest.".
            if (name.OptionalStartsWith("Code-Lock", StringComparison.OrdinalIgnoreCase))
                return $"ChosenBytes Code-Lock {exe.ProductVersion}";

            // Get the .text section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("CODE-LOCK.OCX")))
                    return "ChosenBytes Code-Lock";

                if (strs.Exists(s => s.Contains("Code-Lock.ocx")))
                    return "ChosenBytes Code-Lock";

                if (strs.Exists(s => s.Contains("CodeLock.Secure")))
                    return "ChosenBytes Code-Lock";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the installation directory of Code-Lock version 2.35.
                new(new FilePathMatch("Code-Lock.chm"), "ChosenBytes Code-Lock"),
                new(new FilePathMatch("Code-Lock.DEP"), "ChosenBytes Code-Lock"),
                new(new FilePathMatch("Code-Lock.ocx"), "ChosenBytes Code-Lock"),
                new(new FilePathMatch("Code-Lock Wizard.exe"), "ChosenBytes Code-Lock"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the installation directory of Code-Lock version 2.35.
                new(new FilePathMatch("Code-Lock.chm"), "ChosenBytes Code-Lock"),
                new(new FilePathMatch("Code-Lock.DEP"), "ChosenBytes Code-Lock"),
                new(new FilePathMatch("Code-Lock.ocx"), "ChosenBytes Code-Lock"),
                new(new FilePathMatch("Code-Lock Wizard.exe"), "ChosenBytes Code-Lock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
