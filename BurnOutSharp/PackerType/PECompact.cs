using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;


namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction and better version detection
    public class PECompact : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // 0x4F434550 is "PECO"
            if (pex.ImageFileHeader.PointerToSymbolTable == 0x4F434550)
                return "PE Compact v1.x";

            // TODO: Get more granular version detection. PiD is somehow able to detect version ranges based
            // on the data in the file. This may be related to information in other fields

            // Get the pec1 section, if it exists
            var pec1Section = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith("pec1"));
            if (pec1Section != null)
                return "PE Compact v1.x";

            // Get the PEC2 section, if it exists -- TODO: Verify this comment since it's pulling the .text section
            var textSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".text"));
            if (textSection != null && textSection.PointerToRelocations == 0x32434550)
            {
                if (textSection.PointerToLinenumbers != 0)
                    return $"PE Compact v{textSection.PointerToLinenumbers} (internal version)";
                
                return "PE Compact v2.x (or newer)";
            }

            return null;
        }
    }
}
