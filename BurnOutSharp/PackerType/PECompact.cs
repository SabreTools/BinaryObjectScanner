using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;


namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction and better version detection
    public class PECompact : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;
            
            // TODO: Do something with this information -
            // PE Compact 1 uses the symbol table pointer in the file header to store the value 1329808720 / 50 45 43 4F / PECO
            // Console.WriteLine($"{file} symbol table pointer: {pex.ImageFileHeader.PointerToSymbolTable}");
            // Console.WriteLine($"{file} ptr as string: {Encoding.ASCII.GetString(BitConverter.GetBytes(pex.ImageFileHeader.PointerToSymbolTable))}");

            // TODO: Get more granular version detection. PiD is somehow able to detect version ranges based
            // on the data in the file. This may be related to information in other fields

            // Get the pec1 section, if it exists
            var pec1Section = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith("pec1"));
            if (pec1Section != null)
                return "PE Compact v1.x";

            // Get the PEC2 section, if it exists
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
