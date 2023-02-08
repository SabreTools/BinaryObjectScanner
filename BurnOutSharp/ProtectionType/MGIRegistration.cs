using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class MGIRegistration : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Found in "Register.dll" from "VideoWaveIII" in IA item "mgi-videowave-iii-version-3.00-mgi-software-2000".
            var resources = pex.FindStringTableByEntry("MGI Registration");
            if (resources.Any())
                return "MGI Registration";

            return null;
        }
    }
}
