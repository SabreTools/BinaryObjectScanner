using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{ 
    /// <summary>
    /// Armadillo was a license manager, packer, and DRM by "The Silicon Realm Toolworks": https://web.archive.org/web/20030203101931/http://www.siliconrealms.com/armadillo.shtml
    /// They were later bought by Digital River, and updated their website: https://web.archive.org/web/20031203021152/http://www.siliconrealms.com/armadillo.shtml
    /// A new updated version named "SoftwarePassport" was released: https://web.archive.org/web/20040423044529/http://siliconrealms.com/softwarepassport/popup.shtml
    /// Later copy of the website, with SoftwarePassport being named instead of Armadillo: https://web.archive.org/web/20040804032608/http://www.siliconrealms.com/armadillo.shtml
    /// It appears as though both Armadillo and SoftwarePassport were being released at the same time, possibly with Armadillo acting as the core component and SoftwarePassport being supplementary: https://web.archive.org/web/20050619013312/http://siliconrealms.com/srt-news.shtml
    /// Digital River itself also advertised Armadillo at first: https://web.archive.org/web/20040116043029/http://www.digitalriver.com:80/corporate/solutions06.shtml
    /// But then only advertised SoftwarePassport once it was released: https://web.archive.org/web/20040604065907/http://www.digitalriver.com/corporate/solutions06.shtml
    /// </summary>
    
    // TODO: Add extraction
    // TODO: Add version checking, if possible
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    
    public class Armadillo : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .nicode section, if it exists
            if (pex.ContainsSection(".nicode", exact: true))
                return "Armadillo";

            // Loop through all "extension" sections -- usually .data1 or .text1
            if (pex.SectionNames != null)
            {
                foreach (var sectionName in Array.FindAll(pex.SectionNames ?? [], s => s != null && s.EndsWith("1")))
                {
                    // Get the section strings, if they exist
                    var strs = pex.GetFirstSectionStrings(sectionName);
                    if (strs != null)
                    {
                        if (strs.Exists(s => s.Contains("ARMDEBUG")))
                            return "Armadillo";
                    }
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            return false;
        }
    }
}
