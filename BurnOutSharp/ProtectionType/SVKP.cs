using BurnOutSharp.ExecutableType.Microsoft.PE;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how versions/version ranges work for this protection
    public class SVKProtector : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
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
