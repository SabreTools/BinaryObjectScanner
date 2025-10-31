using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;
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
    public class CopyLok : IExecutableCheck<PortableExecutable>, IISOCheck<ISO9660>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // If there are more than 2 icd-prefixed sections, then we have a match
            // Though this is the same name that SafeDisc uses for protected executables, this seems to be a coincidence.
            // Found in Redump entries 31557, 31674, 31675, 31708, 38239, 44210, and 53929.
            int icdSectionCount = Array.FindAll(exe.SectionNames ?? [], s => s.StartsWith("icd")).Length;
            if (icdSectionCount >= 2)
                return "CopyLok / CodeLok";

            return null;
        }
        
        public string? CheckISO(string file, ISO9660 iso, bool includeDebug)
        {
            #region Initial Checks
            
            var pvd = (PrimaryVolumeDescriptor)iso.VolumeDescriptorSet[0];
            
            if (!FileType.ISO9660.NoteworthyApplicationUse(pvd))
                return "None";
            
            if (FileType.ISO9660.NoteworthyReserved653Bytes(pvd))
                return "None";

            #endregion
            
            int offset = 0;
            
            #region Read Application Use
            
            var applicationUse = pvd.ApplicationUse;
            var constantValueOne = applicationUse.ReadUInt32LittleEndian(ref offset);
            var smallSizeBytes = applicationUse.ReadUInt16LittleEndian(ref offset);
            var constantValueTwo = applicationUse.ReadUInt16LittleEndian(ref offset);
            var finalSectionOneBytes = applicationUse.ReadUInt32LittleEndian(ref offset);
            var zeroByte = applicationUse.ReadByte(ref offset);
            var earlyCopyLokBytesOne = applicationUse.ReadUInt16LittleEndian(ref offset);
            var pairBytesOne = applicationUse.ReadUInt16LittleEndian(ref offset);
            var oneValueBytes =  applicationUse.ReadUInt32LittleEndian(ref offset);
            var earlyCopyLokBytesTwo = applicationUse.ReadUInt32LittleEndian(ref offset);
            var pairBytesTwo = applicationUse.ReadUInt32LittleEndian(ref offset);
            var endingZeroBytes = applicationUse.ReadBytes(ref offset, 483);
            
            #endregion
            
            #region Main Checks
            
            // Early return if the rest of the AU data isn't 0x00
            if (!Array.TrueForAll(endingZeroBytes, b => b == 0x00))
                return null;
            
            // Check first currently-observed constant value
            if (constantValueOne != 0x4ED38AE1)
                return null;
            
            // Check for early variant copylok
            if (earlyCopyLokBytesOne == 0x00)
            {
                // Redump ID 35908, 56433, 44526
                if (0 == pairBytesOne && 0 == oneValueBytes && 0 == earlyCopyLokBytesTwo && 0 == pairBytesTwo)
                    return "CopyLok / CodeLok (Early, ~1850 errors)";
                
                return "CopyLok / CodeLok - Unknown variant, please report to us on GitHub!";
            }
            
            // Check remaining currently-observed constant values
            if (constantValueTwo != 0x4ED3 || zeroByte != 0x00 || earlyCopyLokBytesOne != 0x0C76 || oneValueBytes != 0x00000001)
                return "CopyLok / CodeLok - Unknown variant, please report to us on GitHub!"; 
                
            // Always 0xD1AD, except in Redump ID 71985 (the only sample) where it's 0x6999
            // Update number be more accurate if more samples are acquired.
            if (smallSizeBytes < 0xADD1)
                return "CopyLok / CodeLok (Less errors, ~255)";

            if (pairBytesOne == 0x9425)
            {
                // Redump ID 37860, 37881, 38239, 100685, 108375
                if (pairBytesTwo != 0x00000000)
                    return "CopyLok / CodeLok (Pair errors, ~1500)";
                
                return "CopyLok / CodeLok - Unknown variant, please report to us on GitHub!"; 
            }
            
            // Redump ID 31557, 44210, 49087, 72183, 31675
            if (pairBytesOne == 0xF3ED && pairBytesTwo == 0x00000000)
                return "CopyLok / CodeLok (Solo errors, ~775)";
            
            #endregion
            
            return "CopyLok / CodeLok - Unknown variant, please report to us on GitHub!"; 
        }
    }
}
