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
                if (files.Count(f => Path.GetFileName(f).Equals("SteamInstall.exe", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("SteamInstall.ini", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("SteamInstall.msi", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("SteamRetailInstaller.dmg", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("SteamSetup.exe", StringComparison.OrdinalIgnoreCase)) > 0)
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
