using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <remarks>
    /// PiD only looks for the `.ldr` section, from testing
    /// There don't seem to be any other signs that this is 3P-Lock anywhere in the example files
    /// No website has been found for 3P-Lock yet
    /// </remarks>
    public class ThreePLock : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            //This produced false positives in some DirectX 9.0c installer files
            //"Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW"

            // Get the .ldr and .ldt sections, if they exist
            if (pex.ContainsSection(".ldr", exact: true) && pex.ContainsSection(".ldt", exact: true))
                return $"3P-Lock Copy Protection";

            return null;
        }
    }
}
