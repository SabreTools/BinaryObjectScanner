using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class Shrinker : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Confirm if both need to be present
            // TODO: Figure out how to get version

            if (exe.ContainsSection(".shrink0", exact: true))
                return "Shrinker";
            if (exe.ContainsSection(".shrink2", exact: true))
                return "Shrinker";

            return null;
        }
    }
}
