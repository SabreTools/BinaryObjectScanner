using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.Protection
{
    // TODO: Figure out how to use path check framework here
    public class ProtectDVDVideo : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var protections = new List<string>();
            if (files == null)
                return protections;

            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                string[] ifofiles = files.Where(s => s.EndsWith(".ifo")).ToArray();
                for (int i = 0; i < ifofiles.Length; i++)
                {
                    var ifofile = new FileInfo(ifofiles[i]);
                    if (ifofile.Length == 0)
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
