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
            // Get the .shrink0 and .shrink2 sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            if (exe.ContainsSection(".shrink0", true) || exe.ContainsSection(".shrink2", true))
                return "Shrinker";

            return null;
        }
    }
}
