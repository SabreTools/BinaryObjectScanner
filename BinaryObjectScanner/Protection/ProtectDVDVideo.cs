using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.Protection
{
    // TODO: Figure out how to use path check framework here
    public class ProtectDVDVideo : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var protections = new List<string>();
            if (files == null)
                return protections;

            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                var ifofiles = files.FindAll(s => s.EndsWith(".ifo"));
                for (int i = 0; i < ifofiles.Count; i++)
                {
                    if (ifofiles[i].FileSize() == 0)
                    {
                        protections.Add("Protect DVD-Video (Unconfirmed - Please report to us on Github)");
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
