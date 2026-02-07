using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public partial class CEG : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            var strs = exe.GetFirstSectionStrings(".rdata");
            if (strs is not null)
            {
                if (strs.Exists(s => s.Contains("STEAMSTART")/* && s.Contains("STEAM_DRM_IPC")*/))
                {
                    // get rid of this later
                    if (!strs.Exists(s => s.Contains("STEAM_DRM_IPC")))
                    {
                        Console.WriteLine("something has gone wrong!");
                        return  "CEG - something has gone wrong!";
                    }
                    if (strs.Exists(s => s.Contains("This file has been stripped")))
                    {
                        // get rid of this later


                        /*
                        var value = CEGDictionary.TryGetValue(exe.COFFFileHeader.TimeDateStamp, out var gameName);
                        if (value)
                        {
                            return $" - {gameName}";
                        }
                        */
                        var fileName = Path.GetFileName(file);
                        var parentDir = Directory.GetParent(file)?.Name;
                        if (parentDir != null)
                        {
                            parentDir = parentDir.Replace("_", " (v");
                            return $"{{ {exe.COFFFileHeader.TimeDateStamp}, \"{parentDir}) - {fileName}\" }},";
                        }

                        return "CEG - Stripless";
                    }
                    else if (strs.Exists(s => s.Contains("This file contains strips")))
                    {
                        return "CEG - Stripful";
                    }
                }
            }
            return "CEG - Could not check if stripless or stripful, please report to us on GitHub!";
        }
    }
}
