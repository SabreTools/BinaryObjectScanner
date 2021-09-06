using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how versions/version ranges work for this protection
    public class SVKProtector : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            if (pex?.ImageFileHeader == null)
                return null;
            
            // 0x504B5653 is "SVKP"
            if (pex.ImageFileHeader.PointerToSymbolTable == 0x504B5653)
                return "SVKP (Slovak Protector)";

            return null;
        }
    }
}
