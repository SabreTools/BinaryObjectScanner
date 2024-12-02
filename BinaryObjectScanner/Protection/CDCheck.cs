using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class CDCheck : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            var name = pex.Comments;
            if (name.OptionalContains("CDCheck utlity for Microsoft Game Studios"))
                return "Microsoft Game Studios CD Check";

            // To broad to be of use
            //name = pex.InternalName;
            //if (name.OptionalContains("CDCheck"))
            //    return "Microsoft Game Studios CD Check";

            // To broad to be of use
            //name = pex.OriginalFilename;
            //if (name.OptionalContains("CDCheck.exe"))
            //    return "Microsoft Game Studios CD Check";

            return null;
        }
    }
}
