using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <remarks>
    /// PiD only looks for the `.ldr` section, from testing
    /// There don't seem to be any other signs that this is 3P-Lock anywhere in the example files
    /// No website has been found for 3P-Lock yet
    /// </remarks>
    public class ThreePLock : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // This produced false positives in some DirectX 9.0c installer files
            //"Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW"

            // Get the .ldr and .ldt sections, if they exist
            if (pex.ContainsSection(".ldr", exact: true) && pex.ContainsSection(".ldt", exact: true))
                return $"3P-Lock Copy Protection";

            return null;
        }
    }
}
