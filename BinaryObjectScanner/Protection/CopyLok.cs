using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// CopyLok (AKA CodeLok) is a DRM created by PAN Technology (https://web.archive.org/web/19991117100625/http://www.pantechnology.com/).
    /// More specifically, it may have been created a spin-off division known as "Panlok" (https://cdmediaworld.com/hardware/cdrom/cd_protections_copylok.shtml).
    /// Though it may also be that "PanLok" was an alternate name for the program itself (https://gamecopyworld.com/games/pc_generic_copylok.shtml).
    /// There was a PanLok website, but it's unknown what content it had hosted (https://web.archive.org/web/20041215075727/http://www.panlok.com/).
    /// 
    /// CopyLok made use of bad sectors, using an average of 720-750 per disc, and appears to have been a form of ring protection (http://forum.redump.org/topic/29842/issues-dumping-pc-disc-with-code-lock-copy-protection/).
    /// At least one disc with CopyLok appears to contain an excerpt of the poem "Jabberwocky" by Lewis Carroll in the raw sector data (http://forum.redump.org/post/54050/#p54050).
    /// According to the Readme for poxylok (https://gf.wiretarget.com/copylok.htm), some version of Gangsters 2 may have this protection.
    /// 
    /// Previous versions of BinaryObjectScanner incorrectly reported this DRM as "CodeLock / CodeLok / CopyLok". It was later discovered that due to the similar names, two entirely different DRM were erroneously lumped together.
    /// "CodeLock" (in this case actually referring to "Code-Lock") is an entirely separate form of DRM, with the existing check now getting used separately.
    /// Also not to be confused with https://en.wikipedia.org/wiki/Rob_Northen_copylock.
    /// 
    /// COPYLOK trademark: https://www.trademarkelite.com/europe/trademark/trademark-detail/000618512/COPYLOK.
    /// </summary>
    public class CopyLok : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // If there are more than 2 icd-prefixed sections, then we have a match
            // Though this is the same name that SafeDisc uses for protected executables, this seems to be a coincidence.
            // Found in Redump entries 31557, 31674, 31675, 31708, 38239, 44210, and 53929.
            int icdSectionCount = pex.SectionNames?.Count(s => s.StartsWith("icd")) ?? 0;
            if (icdSectionCount >= 2)
                return "CopyLok / CodeLok";

            return null;
        }
    }
}