using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // https://www.virustotal.com/gui/file/e55a7f27452c7e5a3f5f388e4b477a124f0bb8bead4c2b0f2d5031578922bc77/detection
    public class MPRESS : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Confirm if both need to be present 
            // TODO: Figure out how to get version

            if (exe.ContainsSection(".MPRESS1"))
                return "MPRESS";
            if (exe.ContainsSection(".MPRESS2"))
                return "MPRESS";

            return null;
        }
    }
}