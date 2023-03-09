using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class CDCheck : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.Comments;
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
