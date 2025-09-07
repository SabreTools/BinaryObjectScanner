using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Easy Anti-Cheat is a form of DRM created by Kamu in 2006 (and later acquired by Epic Games in 2018) that prevents cheating in multiplayer games.
    /// The current official website is https://easy.ac, which began to be used by Easy Anti-Cheat by 2014.
    /// Care should be taken when viewing older versions of this URL, as before 2009 this domain had been used to promote a brand of condoms created by the RFSU, a Swedish sexual education non-profit organization.
    /// The first archive of this URL that was used by Easy Anti-Cheat appears to be https://web.archive.org/web/20140324145706/https://www.easy.ac/.
    /// The first URL used for Easy Anti-Cheat appears to be http://easyanticheat.net/, with seemingly the oldest archive being this: https://web.archive.org/web/20071024001450/http://easyanticheat.net/en/.
    /// There's a version of Easy Anti-Cheat that's bundled into the Epic Online Services, which is known as the EOS version of Easy Anti-Cheat (https://www.pcgamingwiki.com/wiki/Easy_Anti-Cheat).
    /// Further information and resources: 
    /// List of games protected by Easy Anti-Cheat: https://www.pcgamingwiki.com/wiki/Easy_Anti-Cheat and https://www.easy.ac/en-us/partners/
    /// https://dev.epicgames.com/docs/services/en-US/GameServices/AntiCheat/index.html
    /// https://www.unknowncheats.me/wiki/Easy_Anti_Cheat
    /// </summary>
    public class EasyAntiCheat : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        // TODO: Add support for detecting older versions, especially versions made before Easy Anti-Cheat was purchased by Epic Games.
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            var name = exe.FileDescription;
            // Found in "VideoHorrorSociety.exe" ("Video Horror Society", Patch 1.0.70309, Steam).
            if (!string.IsNullOrEmpty(name) && name!.Contains("Easy Anti-Cheat Bootstrapper (EOS)"))
                return "Easy Anti-Cheat (EOS Version)";

            // Found in "EasyAntiCheat_EOS_Setup.exe" ("Video Horror Society", Patch 1.0.70309, Steam) and "EasyAntiCheat_EOS.exe", which is found installed in "Program Files (x86)\EasyAntiCheat_EOS".
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Easy Anti-Cheat Service (EOS)"))
                return "Easy Anti-Cheat (EOS Version)";

            // These two generic checks are both general enough to detect the majority of files known to contain Easy Anti-Cheat, as well as specific enough to avoid false positives.
            else if (!string.IsNullOrEmpty(name) && name!.Contains("EasyAntiCheat"))
                return "Easy Anti-Cheat";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Easy Anti-Cheat"))
                return "Easy Anti-Cheat";

            // For documentation, known exact File Descriptions and their associated files are listed below:
            // "Easy Anti-Cheat Bootstrapper (EOS)" -> "VideoHorrorSociety.exe" ("Video Horror Society", Patch 1.0.70309, Steam).
            // "Easy Anti-Cheat Service (EOS)" -> "EasyAntiCheat_EOS_Setup.exe" ("Video Horror Society", Patch 1.0.70309, Steam) and "EasyAntiCheat_EOS.exe", which is found installed in "Program Files (x86)\EasyAntiCheat_EOS".
            // "EasyAntiCheat.Client" -> "EasyAntiCheat.Client.dll" from "Intruder" (Update 2287, Steam).
            // "EasyAntiCheat.Server" -> "EasyAntiCheat.Server.dll" from "Intruder" (Update 2287, Steam).
            // "EasyAntiCheat Client" -> "EasyAntiCheat.dll", "EasyAntiCheat_x64.dll", and "EasyAntiCheat_x86.dll" from "Intruder" (Update 2287, Steam) and "Rec Room" (Version 20220803, Oculus).
            // "EasyAntiCheat Driver" -> "EasyAntiCheat.sys", which is found installed in "Program Files (x86)\EasyAntiCheat".
            // "EasyAntiCheat Launcher" -> "IntruderLauncher.exe" ("Intruder", Update 2287, Steam) and "Recroom_Oculus.exe" ("Rec Room", Version 20220803, Oculus). The Original Filename for this file is "eac_launcher.exe".
            // "EasyAntiCheat Server" -> "eac_server.dll" from "Intruder" (Update 2287, Steam).
            // "EasyAntiCheat Service" -> "EasyAntiCheat.exe", which is found installed in "Program Files (x86)\EasyAntiCheat" and "EasyAntiCheat_Setup.exe" ("Intruder", Update 2287, Steam).

            name = exe.ProductName;
            // Found in multiple files, including "VideoHorrorSociety.exe" ("Video Horror Society", Patch 1.0.70309, Steam) and "start_protected_game.exe" ("VRChat", Version 2022.2.2p2, Oculus).
            if (!string.IsNullOrEmpty(name) && name!.Contains("Easy Anti-Cheat Bootstrapper (EOS)"))
                return "Easy Anti-Cheat (EOS Version)";

            // Found in multiple files, including "EasyAntiCheat_EOS_Setup.exe" ("Video Horror Society", Patch 1.0.70309, Steam; "VRChat", Version 2022.2.2p2, Oculus) and "EasyAntiCheat.exe", which is found installed in "Program Files (x86)\EasyAntiCheat_EOS".
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Easy Anti-Cheat Service (EOS)"))
                return "Easy Anti-Cheat (EOS Version)";

            // These two generic checks are both general enough to detect the majority of files known to contain Easy Anti-Cheat, as well as specific enough to avoid false positives.
            else if (!string.IsNullOrEmpty(name) && name!.Contains("EasyAntiCheat"))
                return "Easy Anti-Cheat";
            else if (!string.IsNullOrEmpty(name) && name!.Contains("Easy Anti-Cheat"))
                return "Easy Anti-Cheat";

            // For documentation, known exact Product Names and their associated files are listed below:
            // "Easy Anti-Cheat Bootstrapper (EOS)" -> "VideoHorrorSociety.exe" ("Video Horror Society", Patch 1.0.70309, Steam) and "start_protected_game.exe" ("VRChat", Version 2022.2.2p2, Oculus).
            // "Easy Anti-Cheat Service (EOS)" -> "EasyAntiCheat_EOS_Setup.exe" ("Video Horror Society", Patch 1.0.70309, Steam; "VRChat", Version 2022.2.2p2, Oculus) and "EasyAntiCheat.exe", which is found installed in "Program Files (x86)\EasyAntiCheat_EOS".
            // "EasyAntiCheat" -> "EasyAntiCheat.exe" and "EasyAntiCheat.sys", which are both found installed in "Program Files (x86)\EasyAntiCheat"; "eac_server.dll" and "EasyAntiCheat.dll", which are both found in Intruder (Version 2287, Steam); and others.
            // "EasyAntiCheat Launcher" -> "IntruderLauncher.exe" ("Intruder", Update 2287, Steam) and "Recroom_Oculus.exe" ("Rec Room", Version 20220803, Oculus). The Original Filename for this file, according to file properties, is "eac_launcher.exe".

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            // TODO: Search for the presence of the folder "EasyAntiCheat" specifically, which is present in every checked version so far.
            var matchers = new List<PathMatchSet>
            {
                // Found installed in "Program Files (x86)\EasyAntiCheat".
                new(new FilePathMatch("EasyAntiCheat.exe"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat.sys"), "Easy Anti-Cheat"),

                // Found installed in "Program Files (x86)\EasyAntiCheat_EOS".
                new(new FilePathMatch("EasyAntiCheat_EOS.exe"), "Easy Anti-Cheat (EOS Version)"),
                new(new FilePathMatch("EasyAntiCheat_EOS.sys"), "Easy Anti-Cheat (EOS Version)"),

                // Found installed in "AppData\Roaming\EasyAntiCheat".
                new(new FilePathMatch("easyanticheat_wow64_x64.eac"), "Easy Anti-Cheat"),
                new(new FilePathMatch("easyanticheat_wow64_x64.eac.metadata"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheatAnimation.png"), "Easy Anti-Cheat"),

                // Found in "Intruder" (Version 2287, Steam).
                new(new FilePathMatch("eac_server.dll"), "Easy Anti-Cheat"),
                new(new FilePathMatch("easyanticheat"), "Easy Anti-Cheat"),
                new(new FilePathMatch("easyanticheat.icns"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat.Client.dll"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat.Server.dll"), "Easy Anti-Cheat"),

                // Found in "Intruder" (Version 2287, Steam) and "Rec Room" (Version 20220803, Oculus).
                new(new FilePathMatch("EasyAntiCheat.dll"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat_Setup.exe"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat_x64.dll"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat_x86.dll"), "Easy Anti-Cheat"),

                // Found in "Video Horror Society" (Patch 1.0.70309, Steam).
                new(new FilePathMatch("EasyAntiCheat_EOS_Setup.exe"), "Easy Anti-Cheat (EOS Version)"),
                new(new FilePathMatch("InstallAntiCheat.bat"), "Easy Anti-Cheat"),
                new(new FilePathMatch("UninstallAntiCheat.bat"), "Easy Anti-Cheat"),

                // Found in "VRChat" (Version 2022.2.2p2, Oculus).
                new(new FilePathMatch("start_protected_game.exe"), "Easy Anti-Cheat"),

                // Found in "Apex Legends" (Build ID 12029216, Steam)
                new(new FilePathMatch("EasyAntiCheat_launcher.exe"), "Easy Anti-Cheat"),
                new(new FilePathMatch("easyanticheat_x64.so"), "Easy Anti-Cheat"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // TODO: Search for the presence of the folder "EasyAntiCheat" specifically, which is present in every checked version so far.
            var matchers = new List<PathMatchSet>
            {
                // Found installed in "Program Files (x86)\EasyAntiCheat".
                new(new FilePathMatch("EasyAntiCheat.exe"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat.sys"), "Easy Anti-Cheat"),

                // Found installed in "Program Files (x86)\EasyAntiCheat_EOS".
                new(new FilePathMatch("EasyAntiCheat_EOS.exe"), "Easy Anti-Cheat (EOS Version)"),
                new(new FilePathMatch("EasyAntiCheat_EOS.sys"), "Easy Anti-Cheat (EOS Version)"),

                // Found installed in "AppData\Roaming\EasyAntiCheat".
                new(new FilePathMatch("easyanticheat_wow64_x64.eac"), "Easy Anti-Cheat"),
                new(new FilePathMatch("easyanticheat_wow64_x64.eac.metadata"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheatAnimation.png"), "Easy Anti-Cheat"),

                // Found in "Intruder" (Version 2287, Steam).
                new(new FilePathMatch("eac_server.dll"), "Easy Anti-Cheat"),
                new(new FilePathMatch("easyanticheat"), "Easy Anti-Cheat"),
                new(new FilePathMatch("easyanticheat.icns"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat.Client.dll"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat.Server.dll"), "Easy Anti-Cheat"),
                // Found in "Intruder" (Version 2287, Steam) and "Rec Room" (Version 20220803, Oculus).
                new(new FilePathMatch("EasyAntiCheat.dll"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat_Setup.exe"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat_x64.dll"), "Easy Anti-Cheat"),
                new(new FilePathMatch("EasyAntiCheat_x86.dll"), "Easy Anti-Cheat"),

                // Found in "Video Horror Society" (Patch 1.0.70309, Steam).
                new(new FilePathMatch("EasyAntiCheat_EOS_Setup.exe"), "Easy Anti-Cheat (EOS Version)"),
                new(new FilePathMatch("InstallAntiCheat.bat"), "Easy Anti-Cheat"),
                new(new FilePathMatch("UninstallAntiCheat.bat"), "Easy Anti-Cheat"),

                // Found in "VRChat" (Version 2022.2.2p2, Oculus).
                new(new FilePathMatch("start_protected_game.exe"), "Easy Anti-Cheat"),

                // Found in "Apex Legends" (Build ID 12029216, Steam)
                new(new FilePathMatch("EasyAntiCheat_launcher.exe"), "Easy Anti-Cheat"),
                new(new FilePathMatch("easyanticheat_x64.so"), "Easy Anti-Cheat"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
