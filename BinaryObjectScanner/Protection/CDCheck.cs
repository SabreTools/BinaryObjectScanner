using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class CDCheck : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.Comments;
            if (name?.Contains("CDCheck utlity for Microsoft Game Studios") == true)
                return "Microsoft Game Studios CD Check";

            // To broad to be of use
            //name = pex.InternalName;
            //if (name?.Contains("CDCheck") == true)
            //    return "Microsoft Game Studios CD Check";

            // To broad to be of use
            //name = pex.OriginalFilename;
            //if (name?.Contains("CDCheck.exe") == true)
            //    return "Microsoft Game Studios CD Check";

            return null;
        }
    }
}
