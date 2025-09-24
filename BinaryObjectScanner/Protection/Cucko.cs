using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Do more research into the Cucko protection:
    //      - Reference to `EASTL` and `EAStdC` are standard for EA products and does not indicate Cucko by itself
    //      - There's little information outside of PiD detection that actually knows about Cucko
    //      - Cucko is confirmed to, at least, use DMI checks.
    public class Cucko : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the .text section, if it exists
            var textData = exe.GetFirstSectionData(".text");
            if (textData != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // Confirmed to detect most examples known of Cucko. The only known exception is the version of "TSLHost.dll" included on Redump entry 36119.
                    // ŠU‰8...…™...ŠUŠ8T...
                    new(new byte?[]
                    {
                        0x8A, 0x55, 0x89, 0x38, 0x14, 0x1E, 0x0F, 0x85,
                        0x99, 0x00, 0x00, 0x00, 0x8A, 0x55, 0x8A, 0x38,
                        0x54, 0x1E, 0x01, 0x0F
                    }, "Cucko (EA Custom)")
                };

                return MatchUtil.GetFirstMatch(file, textData, matchers, includeDebug);
            }

            return null;
        }
    }
}
