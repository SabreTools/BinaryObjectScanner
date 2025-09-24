using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Ghost Installer
    /// </summary>
    public class GhostInstaller : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // <see href="https://www.virustotal.com/gui/file/b2fc4cffe5131195baf419e96c9fa68c3f23208986fb14e3c5b458b1e7d6af89/details"/>
            var overlayData = exe.OverlayData;
            if (overlayData != null)
            {
                // GIPEND
                if (overlayData.EndsWith([0x47, 0x49, 0x50, 0x45, 0x4E, 0x44]))
                    return "Ghost Installer";
            }

            // <see href="https://www.virustotal.com/gui/file/b2fc4cffe5131195baf419e96c9fa68c3f23208986fb14e3c5b458b1e7d6af89/details"/>
            if (exe.FindDialogBoxByItemTitle("Ghost Installer initializing...").Count > 0)
                return "Ghost Installer";

            return null;
        }
    }
}
