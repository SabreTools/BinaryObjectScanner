using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;

namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .cenega section, if it exists
            var cenegaSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".cenega"));
            if (cenegaSection != null)
                return "Cenega ProtectDVD";

            return null;
        }
    }
}
