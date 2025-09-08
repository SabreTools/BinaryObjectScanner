using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// PKLITE and PKLITE32
    /// </summary>
    public class PKLite : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // <see href="https://www.virustotal.com/gui/file/601573f263115035921f621598f7a81ace998bf325e081165aa698b981822013/details"/>
            if (exe.ContainsSection(".pklstb"))
                return "PKLITE32"; // TODO: Figure out how to determine version

            return null;
        }
    }
}
