using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Steam
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("SteamInstall.exe", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SteamInstall.ini", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SteamInstall.msi", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SteamRetailInstaller.dmg", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SteamSetup.exe", StringComparison.OrdinalIgnoreCase)))
                {
                    return "Steam";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("SteamInstall.exe", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SteamInstall.ini", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SteamInstall.msi", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SteamRetailInstaller.dmg", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SteamSetup.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "Steam";
                }
            }

            return null;
        }
    }
}
