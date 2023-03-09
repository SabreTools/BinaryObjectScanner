using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how versions/version ranges work for this protection
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    // The official website (https://web.archive.org/web/20010301183549/http://www.anticracking.sk/) contains installations for demo versions of SVKP, which themselves are also protected with SVKP. 
    // The site also contains useful information about various other copy protections from the era.
    // Additional info: https://www.cdmediaworld.com/hardware/cdrom/cd_protections_svkp.shtml
    public class SVKProtector : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // 0x504B5653 is "SVKP"
            if (pex.PointerToSymbolTable == 0x504B5653)
                return "SVKP (Slovak Protector)";

            return null;
        }
    }
}
