using System.Collections.Concurrent;
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
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var protections = new ConcurrentQueue<string>();
            if (files == null)
                return protections;

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
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            return null;
        }
    }
}
