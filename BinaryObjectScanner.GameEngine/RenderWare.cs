using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BinaryObjectScanner.GameEngine
{
    /// <summary>
    /// RenderWare (https://web.archive.org/web/20070214132346/http://www.renderware.com/) is an API and graphics engine created by Criterion in 1993.
    /// It appears that version 4.X was exclusively used by EA internally, with version 3.X being the final public version (https://sigmaco.org/3782-renderware/).
    /// It was available to use on many different platforms, with it being particularly useful for the PS2 (https://en.wikipedia.org/wiki/RenderWare).
    /// 
    /// Additional resources and documentation:
    /// RenderWare interview: https://web.archive.org/web/20031208124348/http://www.homelanfed.com/index.php?id=9856
    /// RenderWare V2.1 API reference: http://www.tnlc.com/rw/api/rwdoc.htm
    /// RenderWare 2 official docs: https://github.com/electronicarts/RenderWare3Docs
    /// RenderWare 3.7 SDK: https://github.com/sigmaco/rwsdk-v37-pc
    /// Wikipedia list of RenderWare games: https://en.wikipedia.org/wiki/Category:RenderWare_games
    /// </summary>
    public class RenderWare : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Found in Redump entries 20138, 55823, and 102493.
            bool rwcsegSection = pex.ContainsSection("_rwcseg", exact: true);
            // Found in Redump entry 20138.
            bool rwdsegSection = pex.ContainsSection("_rwdseg", exact: true);

            // TODO: Check if this indicates a specific version, or if these sections are present in multiple.
            if (rwcsegSection || rwdsegSection) 
                return "RenderWare";

            return null;
        }
    }
}
