using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to use path check framework here
    public class DVDMoviePROTECT : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                string[] bupfiles = files.Where(s => s.EndsWith(".bup")).ToArray();
                for (int i = 0; i < bupfiles.Length; i++)
                {
                    FileInfo bupfile = new FileInfo(bupfiles[i]);
                    FileInfo ifofile = new FileInfo(Path.Combine(bupfile.DirectoryName, bupfile.Name.Substring(0, bupfile.Name.Length - bupfile.Extension.Length) + ".ifo"));
                    if (bupfile.Length != ifofile.Length)
                        return new List<string>() { "DVD-Movie-PROTECT" };
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
