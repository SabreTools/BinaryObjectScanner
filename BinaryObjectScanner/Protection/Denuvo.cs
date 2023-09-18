using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Denuvo (https://irdeto.com/denuvo/) is a family of DRM originally created in 2014 by Denuvo Software Solutions, which was acquired by Irdeto in 2018 (https://www.gamesindustry.biz/irdeto-acquires-denuvo-in-bid-to-beef-up-security-in-the-games-industry).
    /// Denuvo Anti-Tamper (https://irdeto.com/denuvo/anti-tamper/) is the most common, preventing game files from being modified and requiring online activation of games.
    /// Denuvo Anti-Tamper datasheet: https://resources.irdeto.com/video-games/datasheet-anti-tamper
    /// Lists of games with Denuvo Anti-Tamper:
    /// https://store.steampowered.com/curator/26095454-Denuvo-Games/
    /// https://www.pcgamingwiki.com/wiki/Denuvo
    /// Denuvo Anti-Cheat (https://irdeto.com/denuvo/anti-cheat/) is a form of anti-cheat, though information on what games use it is rather sparse.
    /// <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/Denuvo_Anti-Cheat/Denuvo_Anti-Cheat.md"/>
    /// Denuvo also has a number of products available, such as ones targeting mobile games:
    /// https://irdeto.com/denuvo/mobile-games-protection/
    /// https://resources.irdeto.com/video-games/datasheet-anti-tamper-and-anti-cheat-technology-for-mobile
    /// Additional information and resources:
    /// https://en.wikipedia.org/wiki/Denuvo
    /// https://www.wired.com/story/empress-drm-cracking-denuvo-video-game-piracy/
    /// </summary>

    public class Denuvo : IPathCheck, IPortableExecutableCheck
    {

        // TODO: Investigate possible filename checks for Denuvo Anti-Tamper.
        // https://www.pcgamingwiki.com/wiki/Denuvo#Redeem.exe

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

            // All current checks for Denuvo Anti-Cheat come from Doom Eternal Update 1 (Steam Depot 782332, Manifest 7064393210727378308).

            // Found in "denuvo-anti-cheat.sys".
            var name = pex.FileDescription;
            if (name?.Equals("Denuvo Anti-Cheat Driver", StringComparison.OrdinalIgnoreCase) == true)
                return $"Denuvo Anti-Cheat";

            // Found in "denuvo-anti-cheat-update-service.exe"/"Denuvo Anti-Cheat Installer.exe".
            if (name?.Equals("Denuvo Anti-Cheat Update Service", StringComparison.OrdinalIgnoreCase) == true)
                return $"Denuvo Anti-Cheat";

            // Found in "denuvo-anti-cheat-runtime.dll".
            if (name?.Equals("Denuvo Anti-Cheat Runtime", StringComparison.OrdinalIgnoreCase) == true)
                return $"Denuvo Anti-Cheat";

            // Data sourced from:
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Denuvo%20protector.2.sg
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/_denuvoComplete.2.sg

            // TODO: Re-enable all Entry Point checks after implementing
            // Denuvo Protector
            // if (pex.OptionalHeader.Magic == OptionalHeaderType.PE32Plus && pex.EntryPointRaw != null)
            // {
            //     byte?[] denuvoProtector = new byte?[]
            //     {
            //         0x48, 0x8D, 0x0D, null, null, null, null, null,
            //         null, null, null, 0xE9, null, null, null, null,
            //         0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //     };

            //     if (pex.EntryPointRaw.StartsWith(denuvoProtector))
            //         return "Denuvo Protector";
            // }

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

            // TODO: Re-enable all Entry Point checks after implementing
            // if (pex.ContainsSection(".arch") || pex.ContainsSection(".srdata") || !string.IsNullOrWhiteSpace(MatchUtil.GetFirstMatch(file, pex.EntryPointRaw, timingMatchers, includeDebug)))
            // {
            //     if (pex.OH_Magic == OptionalHeaderType.PE32Plus)
            //     {
            //         var matchers = new List<ContentMatchSet>
            //         {
            //             // Mad Max, Metal Gear Solid: TPP, Rise of the Tomb Raider
            //             new ContentMatchSet(
            //                 new ContentMatch(
            //                     new byte?[]
            //                     {
            //                         0x51, 0x52, 0x41, 0x50, 0x41, 0x51, 0x4C, 0x8D,
            //                         null, null, null, null, null, 0x4C, 0x8D, null,
            //                         null, null, null, null, 0x4D, 0x29, 0xC1,
            //                     },
            //                     end: 0
            //                 ),
            //             "Denuvo v1.0 (x64)"),

            //             // Lords of the Fallen, Batman: AK, Just Cause 3, Sherlock Holmes: TdD, Tales of Berseria etc
            //             new ContentMatchSet(
            //                 new ContentMatch(
            //                     new byte?[]
            //                     {
            //                         0x48, 0x8D, 0x0D, null, null, null, null, 0xE9,
            //                         null, null, null, null,
            //                     },
            //                     end: 0
            //                 ),
            //             "Denuvo v2.0a (x64)"),

            //             // Yesterday Origins
            //             new ContentMatchSet(
            //                 new ContentMatch(
            //                     new byte?[]
            //                     {
            //                         0x48, 0x89, null, null, null, null, null, 0x48,
            //                         0x89, null, null, null, null, null, 0x4C, 0x89,
            //                         null, null, null, null, null, 0x4C, 0x89, null,
            //                         null, null, null, null, 0x48, 0x83, 0xFA, 0x01,
            //                     },
            //                     end: 0
            //                 ),
            //             "Denuvo v2.0b (x64)"),

            //             // Sniper Ghost Warrior 3 (beta), Dead Rising 4 (SteamStub-free)
            //             new ContentMatchSet(
            //                 new ContentMatch(
            //                     new byte?[]
            //                     {
            //                         null, null, null, null, null, null, null, null,
            //                         0x4C, 0x89, 0x1C, 0x24, 0x49, 0x89, 0xE3,
            //                     },
            //                     end: 0
            //                 ),
            //             "Denuvo v3.0a (x64)"),

            //             // Train Sim World CSX Heavy Haul
            //             new ContentMatchSet(
            //                 new ContentMatch(
            //                     new byte?[]
            //                     {
            //                         0x4D, 0x8D, null, null, null, null, null, null,
            //                         null, null, null, 0x48, 0x89, null, null, null,
            //                         null, null, 0x48, 0x8D, null, null, 0x48, 0x89,
            //                         null, 0x48, 0x89, null, 0x48, 0x89,
            //                     },
            //                     end: 0
            //                 ),
            //             "Denuvo v3.0b (x64)"),
            //         };

            //         var match = MatchUtil.GetFirstMatch(file, pex.EntryPointRaw, matchers, includeDebug);
            //         if (!string.IsNullOrWhiteSpace(match))
            //             return match;

            //         return "Denuvo (Unknown x64 Version)";

            //         //// Check if steam_api64.dll present
            //         //if (PE.isLibraryPresent("steam_api64.dll"))
            //         //{
            //         //    // Override additional info
            //         //    sOptions = "x64 -> Steam";
            //         //    bDetected = 1;
            //         //}
            //         //// Check if uplay_r1_loader64.dll present
            //         //if (PE.isLibraryPresent("uplay_r1_loader64.dll"))
            //         //{
            //         //    // Override additional info
            //         //    sOptions = "x64 -> uPlay";
            //         //    bDetected = 1;
            //         //}
            //         //// Check if uplay_r2_loader64.dll present
            //         //if (PE.isLibraryPresent("uplay_r2_loader64.dll"))
            //         //{
            //         //    // Override additional info
            //         //    sOptions = "x64 -> uPlay";
            //         //    bDetected = 1;
            //         //}
            //         //// Check if Core/Activation64.dll present
            //         //if (PE.isLibraryPresent("Core/Activation64.dll"))
            //         //{
            //         //    // Override additional info
            //         //    sOptions = "x64 -> Origin";
            //         //    bDetected = 1;
            //         //}
            //     }
            //     else
            //     {
            //         var matchers = new List<ContentMatchSet>
            //         {
            //             // Pro Evolution Soccer 2017, Champions of Anteria
            //             new ContentMatchSet(
            //                 new ContentMatch(
            //                     new byte?[]
            //                     {
            //                         0x55, 0x89, 0xE5, 0x8D, null, null, null, null,
            //                         null, null, 0xE8, null, null, null, null, 0xE8,
            //                         null, null, null, null, 0xE8, null, null, null,
            //                         null, 0xE8, null, null, null, null,
            //                     },
            //                     end: 0
            //                 ),
            //             "Denuvo v1.0 (x86)"),

            //             // Romance of 13 Kingdoms, 2Dark
            //             new ContentMatchSet(
            //                 new ContentMatch(
            //                     new byte?[]
            //                     {
            //                         0x8D, null, null, null, null, null, null, 0x89,
            //                         0x7C, 0x24, 0x04, 0x89, 0xE7,
            //                     },
            //                     end: 0
            //                 ),
            //             "Denuvo v2.0 (x86)"),
            //         };

            //         var match = MatchUtil.GetFirstMatch(file, pex.EntryPointRaw, matchers, includeDebug);
            //         if (!string.IsNullOrWhiteSpace(match))
            //             return match;

            //         //// Check if steam_api64.dll present
            //         //if (PE.isLibraryPresent("steam_api.dll"))
            //         //{
            //         //    // Override additional info
            //         //    sOptions = "x86 -> Steam";
            //         //    bDetected = 1;
            //         //}
            //         //// Check if uplay_r1_loader.dll present
            //         //if (PE.isLibraryPresent("uplay_r1_loader.dll"))
            //         //{
            //         //    // Override additional info
            //         //    sOptions = "x86 -> uPlay";
            //         //    bDetected = 1;
            //         //}
            //         //// Check if Core/Activation.dll present
            //         //if (PE.isLibraryPresent("Core/Activation.dll"))
            //         //{
            //         //    // Override additional info
            //         //    sOptions = "x86 -> Origin";
            //         //    bDetected = 1;
            //         //}
            //     }
            // }

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Doom Eternal Update 1 (Steam Depot 782332, Manifest 7064393210727378308).

                // These files are automatically installed into an "Denuvo Anti-Cheat" folder when the game is installed. 
                new PathMatchSet(new PathMatch("denuvo-anti-cheat.sys", useEndsWith: true), "Denuvo Anti-Cheat"),
                new PathMatchSet(new PathMatch("denuvo-anti-cheat-update-service.exe", useEndsWith: true), "Denuvo Anti-Cheat"),
                new PathMatchSet(new PathMatch("denuvo-anti-cheat-runtime.dll", useEndsWith: true), "Denuvo Anti-Cheat"),

                // This file is a renamed copy of "denuvo-anti-cheat-update-service.exe" which is only seen in the folder of the main game executable after it has been run, but before Denuvo Anti-Cheat is finished installing.
                new PathMatchSet(new PathMatch("Denuvo Anti-Cheat Installer.exe", useEndsWith: true), "Denuvo Anti-Cheat"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Doom Eternal Update 1 (Steam Depot 782332, Manifest 7064393210727378308).

                // These files are automatically installed into an "Denuvo Anti-Cheat" folder when the game is installed. 
                new PathMatchSet(new PathMatch("denuvo-anti-cheat.sys", useEndsWith: true), "Denuvo Anti-Cheat"),
                new PathMatchSet(new PathMatch("denuvo-anti-cheat-update-service.exe", useEndsWith: true), "Denuvo Anti-Cheat"),
                new PathMatchSet(new PathMatch("denuvo-anti-cheat-runtime.dll", useEndsWith: true), "Denuvo Anti-Cheat"),

                // This file is a renamed copy of "denuvo-anti-cheat-update-service.exe" which is only seen in the folder of the main game executable after it has been run, but before Denuvo Anti-Cheat is finished installing.
                new PathMatchSet(new PathMatch("Denuvo Anti-Cheat Installer.exe", useEndsWith: true), "Denuvo Anti-Cheat"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
