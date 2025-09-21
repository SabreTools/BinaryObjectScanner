using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// GP-Install
    /// </summary>
    public class GPInstall : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            // <see href="https://www.virustotal.com/gui/file/0e0a93cba8163cef9c979cbb49a6f15604956b9441aba6fb9e9f0c6897cc73ed/details"/>
            if (name.OptionalContains("GP-Install"))
                return "GP-Install";

            return null;
        }
    }
}
