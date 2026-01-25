using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.Protection
{
    // TODO: Figure out how to use path check framework here
    public class DVDMoviePROTECT : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var protections = new List<string>();
            if (files is null)
                return protections;

            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                var bupfiles = files.FindAll(s => s.EndsWith(".bup"));
                for (int i = 0; i < bupfiles.Count; i++)
                {
                    var bupfile = new FileInfo(bupfiles[i]);
                    if (bupfile.DirectoryName is null)
                        continue;

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
                    string ifofile = Path.Combine(bupfile.DirectoryName, bupfile.Name[..^bupfile.Extension.Length] + ".ifo");
#else
                    string ifofile = Path.Combine(bupfile.DirectoryName, bupfile.Name.Substring(0, bupfile.Name.Length - bupfile.Extension.Length) + ".ifo");
#endif
                    if (bupfile.Length != ifofile.FileSize())
                    {
                        protections.Add("DVD-Movie-PROTECT (Unconfirmed - Please report to us on Github)");
                        break;
                    }
                }
            }

            return protections;
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            return null;
        }
    }
}
