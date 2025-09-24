using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // The official website for CExe also includes the source code (which does have to be retrieved by the Wayback Machine)
    // http://www.scottlu.com/Content/CExe.html
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class CExe : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // If there are exactly 2 resources with type 99
            if (exe.FindResourceByNamedType("99, ").Count == 2)
                return "CExe";

            if (exe.StubExecutableData != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    new(new byte?[]
                    {
                        0x25, 0x57, 0x6F, 0xC1, 0x61, 0x36, 0x01, 0x92,
                        0x61, 0x36, 0x01, 0x92, 0x61, 0x36, 0x01, 0x92,
                        0x61, 0x36, 0x00, 0x92, 0x7B, 0x36, 0x01, 0x92,
                        0x03, 0x29, 0x12, 0x92, 0x66, 0x36, 0x01, 0x92,
                        0x89, 0x29, 0x0A, 0x92, 0x60, 0x36, 0x01, 0x92,
                        0xD9, 0x30, 0x07, 0x92, 0x60, 0x36, 0x01, 0x92
                    }, "CExe")
                };

                var match = MatchUtil.GetFirstMatch(file, exe.StubExecutableData, matchers, includeDebug);
                if (!string.IsNullOrEmpty(match))
                    return match;
            }

            return null;
        }
    }
}
