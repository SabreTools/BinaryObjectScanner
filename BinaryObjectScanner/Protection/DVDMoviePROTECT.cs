#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.Protection
{
    // TODO: Figure out how to use path check framework here
    public class DVDMoviePROTECT : IPathCheck
    {
        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
#if NET20 || NET35
            var protections = new Queue<string>();
#else
            var protections = new ConcurrentQueue<string>();
#endif
            if (files == null)
                return protections;

            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                string[] bupfiles = files.Where(s => s.EndsWith(".bup")).ToArray();
                for (int i = 0; i < bupfiles.Length; i++)
                {
                    FileInfo bupfile = new FileInfo(bupfiles[i]);
                    if (bupfile.DirectoryName == null)
                        continue;

                    FileInfo ifofile = new FileInfo(Path.Combine(bupfile.DirectoryName, bupfile.Name.Substring(0, bupfile.Name.Length - bupfile.Extension.Length) + ".ifo"));
                    if (bupfile.Length != ifofile.Length)
                    {
                        protections.Enqueue("DVD-Movie-PROTECT (Unconfirmed - Please report to us on Github)");
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
