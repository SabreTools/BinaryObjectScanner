using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class ProtectDVDVideo : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                string[] ifofiles = files.Where(s => s.EndsWith(".ifo")).ToArray();
                for (int i = 0; i < ifofiles.Length; i++)
                {
                    FileInfo ifofile = new FileInfo(ifofiles[i]);
                    if (ifofile.Length == 0)
                        return "Protect DVD-Video";
                }
            }
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            return null;
        }
    }
}
