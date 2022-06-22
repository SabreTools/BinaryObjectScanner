using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // Data sourced from:
    // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Denuvo%20protector.2.sg
    // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/_denuvoComplete.2.sg
    public class Denuvo : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Denuvo Protector
            if (pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus && pex.EntryPointRaw != null)
            {
                byte?[] denuvoProtector = new byte?[]
                {
                    0x48, 0x8D, 0x0D, null, null, null, null, null,
                    null, null, null, 0xE9, null, null, null, null,
                    0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                };

                if (pex.EntryPointRaw.StartsWith(denuvoProtector))
                    return "Denuvo Protector";
            }

            // Denuvo
            var timingMatchers = new List<ContentMatchSet>
            {
                // Denuvo Timing
                new ContentMatchSet(
                    new byte?[]
                    {
                        0x44, 0x65, 0x6E, 0x75, 0x76, 0x6F, 0x20, 0x54,
                        0x69, 0x6D, 0x69, 0x6E, 0x67,
                    }, "Denuvo")
            };

            if (pex.ContainsSection(".arch") || pex.ContainsSection(".srdata") || !string.IsNullOrWhiteSpace(MatchUtil.GetFirstMatch(file, pex.EntryPointRaw, timingMatchers, includeDebug)))
            {
                if (pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // Mad Max, Metal Gear Solid: TPP, Rise of the Tomb Raider
                        new ContentMatchSet(
                            new ContentMatch(
                                new byte?[]
                                {
                                    0x51, 0x52, 0x41, 0x50, 0x41, 0x51, 0x4C, 0x8D,
                                    null, null, null, null, null, 0x4C, 0x8D, null,
                                    null, null, null, null, 0x4D, 0x29, 0xC1,
                                },
                                end: 0
                            ),
                        "Denuvo v1.0 (x64)"),

                        // Lords of the Fallen, Batman: AK, Just Cause 3, Sherlock Holmes: TdD, Tales of Berseria etc
                        new ContentMatchSet(
                            new ContentMatch(
                                new byte?[]
                                {
                                    0x48, 0x8D, 0x0D, null, null, null, null, 0xE9,
                                    null, null, null, null,
                                },
                                end: 0
                            ),
                        "Denuvo v2.0a (x64)"),

                        // Yesterday Origins
                        new ContentMatchSet(
                            new ContentMatch(
                                new byte?[]
                                {
                                    0x48, 0x89, null, null, null, null, null, 0x48,
                                    0x89, null, null, null, null, null, 0x4C, 0x89,
                                    null, null, null, null, null, 0x4C, 0x89, null,
                                    null, null, null, null, 0x48, 0x83, 0xFA, 0x01,
                                },
                                end: 0
                            ),
                        "Denuvo v2.0b (x64)"),

                        // Sniper Ghost Warrior 3 (beta), Dead Rising 4 (SteamStub-free)
                        new ContentMatchSet(
                            new ContentMatch(
                                new byte?[]
                                {
                                    null, null, null, null, null, null, null, null,
                                    0x4C, 0x89, 0x1C, 0x24, 0x49, 0x89, 0xE3,
                                },
                                end: 0
                            ),
                        "Denuvo v3.0a (x64)"),

                        // Train Sim World CSX Heavy Haul
                        new ContentMatchSet(
                            new ContentMatch(
                                new byte?[]
                                {
                                    0x4D, 0x8D, null, null, null, null, null, null,
                                    null, null, null, 0x48, 0x89, null, null, null,
                                    null, null, 0x48, 0x8D, null, null, 0x48, 0x89,
                                    null, 0x48, 0x89, null, 0x48, 0x89,
                                },
                                end: 0
                            ),
                        "Denuvo v3.0b (x64)"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, pex.EntryPointRaw, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;

                    return "Denuvo (Unknown x64 Version)";

                    //// Check if steam_api64.dll present
                    //if (PE.isLibraryPresent("steam_api64.dll"))
                    //{
                    //    // Override additional info
                    //    sOptions = "x64 -> Steam";
                    //    bDetected = 1;
                    //}
                    //// Check if uplay_r1_loader64.dll present
                    //if (PE.isLibraryPresent("uplay_r1_loader64.dll"))
                    //{
                    //    // Override additional info
                    //    sOptions = "x64 -> uPlay";
                    //    bDetected = 1;
                    //}
                    //// Check if uplay_r2_loader64.dll present
                    //if (PE.isLibraryPresent("uplay_r2_loader64.dll"))
                    //{
                    //    // Override additional info
                    //    sOptions = "x64 -> uPlay";
                    //    bDetected = 1;
                    //}
                    //// Check if Core/Activation64.dll present
                    //if (PE.isLibraryPresent("Core/Activation64.dll"))
                    //{
                    //    // Override additional info
                    //    sOptions = "x64 -> Origin";
                    //    bDetected = 1;
                    //}
                }
                else
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // Pro Evolution Soccer 2017, Champions of Anteria
                        new ContentMatchSet(
                            new ContentMatch(
                                new byte?[]
                                {
                                    0x55, 0x89, 0xE5, 0x8D, null, null, null, null,
                                    null, null, 0xE8, null, null, null, null, 0xE8,
                                    null, null, null, null, 0xE8, null, null, null,
                                    null, 0xE8, null, null, null, null,
                                },
                                end: 0
                            ),
                        "Denuvo v1.0 (x86)"),

                        // Romance of 13 Kingdoms, 2Dark
                        new ContentMatchSet(
                            new ContentMatch(
                                new byte?[]
                                {
                                    0x8D, null, null, null, null, null, null, 0x89,
                                    0x7C, 0x24, 0x04, 0x89, 0xE7,
                                },
                                end: 0
                            ),
                        "Denuvo v2.0 (x86)"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, pex.EntryPointRaw, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;

                    //// Check if steam_api64.dll present
                    //if (PE.isLibraryPresent("steam_api.dll"))
                    //{
                    //    // Override additional info
                    //    sOptions = "x86 -> Steam";
                    //    bDetected = 1;
                    //}
                    //// Check if uplay_r1_loader.dll present
                    //if (PE.isLibraryPresent("uplay_r1_loader.dll"))
                    //{
                    //    // Override additional info
                    //    sOptions = "x86 -> uPlay";
                    //    bDetected = 1;
                    //}
                    //// Check if Core/Activation.dll present
                    //if (PE.isLibraryPresent("Core/Activation.dll"))
                    //{
                    //    // Override additional info
                    //    sOptions = "x86 -> Origin";
                    //    bDetected = 1;
                    //}
                }
            }

            return null;
        }
    }
}
