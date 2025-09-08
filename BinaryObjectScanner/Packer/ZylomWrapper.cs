using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Zylom Wrapper
    /// </summary>
    public class ZylomWrapper : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the .zylmix section, if it exists
            // Found in "f126309095_Zylom_Games"
            if (exe.ContainsSection(".zylmix", exact: true))
                return "Zylom Wrapper";

            return null;
        }
    }
}
