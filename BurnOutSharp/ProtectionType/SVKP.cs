using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how versions/version ranges work for this protection
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class SVKProtector : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the image file header from the executable, if possible
            if (pex?.ImageFileHeader == null)
                return null;
            
            // 0x504B5653 is "SVKP"
            if (pex.ImageFileHeader.PointerToSymbolTable == 0x504B5653)
                return "SVKP (Slovak Protector)";

            return null;
        }
    }
}
