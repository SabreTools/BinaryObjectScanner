using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Protection that used to be offered by Valve for games on Steam. By default, protected executables are "stripped"
    /// of varying 4KiB "strips", and these "strips" would only be downloaded when the user attempted to run the game on
    /// Steam.
    /// </summary>
    public partial class CEG : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            var strs = exe.GetFirstSectionStrings(".rdata");
            if (strs is not null)
            {
                if (strs.Exists(s => s.Contains("STEAMSTART")) && strs.Exists(s => s.Contains("STEAM_DRM_IPC")))
                {
                    if (strs.Exists(s => s.Contains("This file has been stripped")))
                    {
                        return "CEG - Stripped";
                    }
                    else if (strs.Exists(s => s.Contains("This file contains strips")))
                    {
                        // TODO: Will be uncommented in the future when the rest of the CEG samples can be obtained.
                        /*var value = CEGDictionary.TryGetValue(exe.COFFFileHeader.TimeDateStamp, out var gameName);
                        if (value)
                        {
                            return $"CEG - Contains Strips - {gameName}";
                        }*/
                        return "CEG - Contains Strips";
                    }

                    return "CEG - Could not determine whether executable contains strips, please report to us on GitHub!";
                }
            }

            return null;
        }
    }
}
