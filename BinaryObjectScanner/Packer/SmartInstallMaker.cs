using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Smart Install Maker
    /// </summary>
    public class SmartInstallMaker : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.AssemblyDescription;

            // <see href="https://www.virustotal.com/gui/file/43b5791a7d87830025efae0db8f1cc5a02b6001f4703b0189adf1412cdbe22ac/details"/>
            if (name.OptionalContains("Smart Install Maker"))
                return "Smart Install Maker";

            return null;
        }
    }
}
