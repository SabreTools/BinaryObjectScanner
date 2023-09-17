using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// NeoLite (https://web.archive.org/web/20000815214147/http://www.neoworx.com/products/neolite/default.asp) was a packer created by NeoWorx.
    /// The most common version appears to be 2.0, with earlier versions existing but with no archived copies available.
    /// NeoWorx was acquired by McAfee in October 2001, who seemingly dropped support for NeoLite (https://web.archive.org/web/20020603224725/http://www.mcafee.com/myapps/neoworx/default.asp).
    /// 
    /// Additional references and documentation:
    /// NeoLite 2.0 evaluation installer: https://web.archive.org/web/20001012061916/http://www.neoworx.com/download/neolte20.exe
    /// PEiD scanning definitions that include NeoLite: https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    /// Website listing various packers, including NeoLite: http://protools.narod.ru/packers.htm
    /// </summary>
    public class NeoLite : IExtractable, IPortableExecutableCheck
    {
        // TODO: Find samples of NeoLite 1.X.
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .neolit section, if it exists.
            // TODO: Check if this section is also present in NeoLite 1.X.
            bool neolitSection = pex.ContainsSection(".neolit", exact: true);
            if (neolitSection)
                return "NeoLite";

            // If more specific or additional checks are needed, "NeoLite Executable File Compressor" should be present

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            // TODO: Add extraction
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream stream, string file, bool includeDebug)
#endif
        {
            return null;
        }
    }
}
