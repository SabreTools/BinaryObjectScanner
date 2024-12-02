using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;
using OHMN = SabreTools.Models.PortableExecutable.OptionalHeaderMagicNumber;

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

    public class Denuvo : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        // TODO: Investigate possible filename checks for Denuvo Anti-Tamper.
        // https://www.pcgamingwiki.com/wiki/Denuvo#Redeem.exe

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // All current checks for Denuvo Anti-Cheat come from Doom Eternal Update 1 (Steam Depot 782332, Manifest 7064393210727378308).

            // Found in "denuvo-anti-cheat.sys".
            var name = pex.FileDescription;
            if (name.OptionalEquals("Denuvo Anti-Cheat Driver", StringComparison.OrdinalIgnoreCase))
                return $"Denuvo Anti-Cheat";

            // Found in "denuvo-anti-cheat-update-service.exe"/"Denuvo Anti-Cheat Installer.exe".
            if (name.OptionalEquals("Denuvo Anti-Cheat Update Service", StringComparison.OrdinalIgnoreCase))
                return $"Denuvo Anti-Cheat";

            // Found in "denuvo-anti-cheat-update-service-launcher.dll".
            if (name.OptionalEquals("Denuvo Anti-Cheat Update Service Launcher", StringComparison.OrdinalIgnoreCase))
                return $"Denuvo Anti-Cheat";

            // Found in "denuvo-anti-cheat-runtime.dll".
            if (name.OptionalEquals("Denuvo Anti-Cheat Runtime", StringComparison.OrdinalIgnoreCase))
                return $"Denuvo Anti-Cheat";

            // Found in "denuvo-anti-cheat-crash-report.exe".
            if (name.OptionalEquals("Denuvo Anti-Cheat Crash Report Tool", StringComparison.OrdinalIgnoreCase))
                return $"Denuvo Anti-Cheat";

            // Data sourced from:
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Denuvo%20protector.2.sg
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/_denuvoComplete.2.sg

            // Denuvo Protector
            if (pex.Model.OptionalHeader?.Magic == OHMN.PE32Plus
                && pex.EntryPointData != null)
            {
                byte?[] denuvoProtector =
                [
                    0x48, 0x8D, 0x0D, null, null, null, null, null,
                    null, null, null, 0xE9, null, null, null, null,
                    0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                ];

                if (pex.EntryPointData.StartsWith(denuvoProtector))
                    return "Denuvo Protector";
            }

            // Denuvo
            var timingMatchers = new List<ContentMatchSet>
            {
                // Denuvo Timing
                new(
                    new byte[] {
                        0x44, 0x65, 0x6E, 0x75, 0x76, 0x6F, 0x20, 0x54,
                        0x69, 0x6D, 0x69, 0x6E, 0x67,
            }, "Denuvo")
            };
            var timingMatch = MatchUtil.GetFirstMatch(file, pex.EntryPointData, timingMatchers, includeDebug);

            // TODO: Re-enable all Entry Point checks after implementing
            if (pex.ContainsSection(".arch")
                || pex.ContainsSection(".srdata")
                || !string.IsNullOrEmpty(timingMatch))
            {
                if (pex.Model.OptionalHeader?.Magic == OHMN.PE32Plus)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // Mad Max, Metal Gear Solid: TPP, Rise of the Tomb Raider
                        new(
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
                        new(
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
                        new(
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
                        new(
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
                        new(
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

                    var match = MatchUtil.GetFirstMatch(file, pex.EntryPointData, matchers, includeDebug);
                    if (!string.IsNullOrEmpty(match))
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
                        new(
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
                        new(
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

                    var match = MatchUtil.GetFirstMatch(file, pex.EntryPointData, matchers, includeDebug);
                    if (!string.IsNullOrEmpty(match))
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

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Doom Eternal Update 1 (Steam Depot 782332, Manifest 7064393210727378308).

                // These files are automatically installed into an "Denuvo Anti-Cheat" folder when the game is installed. 
                new(new FilePathMatch("denuvo-anti-cheat.sys"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-update-service.exe"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-runtime.dll"), "Denuvo Anti-Cheat"),

                // This file is a renamed copy of "denuvo-anti-cheat-update-service.exe" which is only seen in the folder of the main game executable after it has been run, but before Denuvo Anti-Cheat is finished installing.
                new(new FilePathMatch("Denuvo Anti-Cheat Installer.exe"), "Denuvo Anti-Cheat"),
                
                // Found in the Denuvo Anti-Cheat installer on their support website. (https://web.archive.org/web/20240130142033/https://support.codefusion.technology/anti-cheat/?l=ja&s=ac&e=2009)
                new(new FilePathMatch("denuvo-anti-cheat-installer.zip"), "Denuvo Anti-Cheat"),

                // Found in "denuvo-anti-cheat-installer.zip".
                new(new FilePathMatch("Denuvo-Anti-Cheat_install_run_as_Admin.bat"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-crash-report.exe"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-crash-report.exe.config"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-update-service-launcher.dll"), "Denuvo Anti-Cheat"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Doom Eternal Update 1 (Steam Depot 782332, Manifest 7064393210727378308).

                // These files are automatically installed into an "Denuvo Anti-Cheat" folder when the game is installed. 
                new(new FilePathMatch("denuvo-anti-cheat.sys"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-update-service.exe"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-runtime.dll"), "Denuvo Anti-Cheat"),

                // This file is a renamed copy of "denuvo-anti-cheat-update-service.exe" which is only seen in the folder of the main game executable after it has been run, but before Denuvo Anti-Cheat is finished installing.
                new(new FilePathMatch("Denuvo Anti-Cheat Installer.exe"), "Denuvo Anti-Cheat"),
                
                // Found in the Denuvo Anti-Cheat installer on their support website. (https://web.archive.org/web/20240130142033/https://support.codefusion.technology/anti-cheat/?l=ja&s=ac&e=2009)
                new(new FilePathMatch("denuvo-anti-cheat-installer.zip"), "Denuvo Anti-Cheat"),

                // Found in "denuvo-anti-cheat-installer.zip".
                new(new FilePathMatch("Denuvo-Anti-Cheat_install_run_as_Admin.bat"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-crash-report.exe"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-crash-report.exe.config"), "Denuvo Anti-Cheat"),
                new(new FilePathMatch("denuvo-anti-cheat-update-service-launcher.dll"), "Denuvo Anti-Cheat"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
