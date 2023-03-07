using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Protection created by Oreans and in use since at least before 2009. Known to be used in Book/Music Collector (http://www.alwinhoogerdijk.com/2009/12/24/protecting-software-with-themida/).
    /// May possibly be used in the PC release of Dynasty Warriors 7 with Xtreme Legends (https://www.pcgamingwiki.com/wiki/Dynasty_Warriors_7_with_Xtreme_Legends).
    /// Themida/WinLicense/"Code Virtualize" seem to all be interconnected/related to each other, or are all at least part of the SecureEngine suite together (https://www.oreans.com/products.php).
    /// 
    /// Versions:
    /// 1.8x - 1.9x (Detect-It-Easy): ArcSoft TotalMedia 3 (http://downloads.fyxm.net/ArcSoft-TotalMedia-23085.html / https://web.archive.org/web/20221114042838/http://files.fyxm.net/23/23085/totalmediatheatre3platinum_retail_tbyb_all.exe).
    /// 
    /// Further links and resources:
    /// https://github.com/VenTaz/Themidie
    /// https://github.com/ergrelet/unlicense
    /// https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Themida.2.sg
    /// 
    /// TODO:
    /// Add/Confirm detection for WinLicense/"Code Virtualize".
    /// Investigate further ArcSoft programs.
    /// Investigate PUBG (possibly older versions) (https://www.pcgamingwiki.com/wiki/PUBG:_Battlegrounds).
    /// </summary>
    public class Themida : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the "Arcsoft " section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings("Arcsoft ");
            if (strs != null)
            {
                // Found in "uDigital Theatre.exe" in http://downloads.fyxm.net/ArcSoft-TotalMedia-23085.html (https://web.archive.org/web/20221114042838/http://files.fyxm.net/23/23085/totalmediatheatre3platinum_retail_tbyb_all.exe).
                // TODO: Investigate "uDRMCheck.dll" in the same product to see if it's related to Themida, or if it's a different form of DRM.
                if (strs.Any(s => s.Contains("Themida")))
                    return "Themida";
            }

            return null;
        }
    }
}
