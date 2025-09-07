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
            bool mpress1 = exe.ContainsSection(".MPRESS1");
            bool mpress2 = exe.ContainsSection(".MPRESS2 ");

            // TODO: Confirm if both need to be present 
            if (mpress1 || mpress2)
                return "MPRESS"; // TODO: Figure out how to get version

            return null;
        }
    }
}