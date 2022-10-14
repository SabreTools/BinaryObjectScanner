using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to use path check framework here
    public class ProtectDVDVideo : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var protections = new ConcurrentQueue<string>();

            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                string[] ifofiles = files.Where(s => s.EndsWith(".ifo")).ToArray();
                for (int i = 0; i < ifofiles.Length; i++)
                {
                    FileInfo ifofile = new FileInfo(ifofiles[i]);
                    if (ifofile.Length == 0)
                    {
                        protections.Enqueue("Protect DVD-Video (Unconfirmed - Please report to us on Github)");
                        break;
                    }
                }
            }
            
            return protections;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            return null;
        }
    }
}
