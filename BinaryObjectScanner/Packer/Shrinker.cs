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
            bool shrink0 = exe.ContainsSection(".shrink0", exact: true);
            bool shrink2 = exe.ContainsSection(".shrink2", exact: true);

            // TODO: Confirm if both need to be present 
            if (shrink0 || shrink2)
                return "Shrinker"; // TODO: Figure out how to get version

            return null;
        }
    }
}
