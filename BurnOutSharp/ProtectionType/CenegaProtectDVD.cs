using BurnOutSharp.ExecutableType.Microsoft.PE;

namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .cenega section, if it exists
            bool cenegaSection = pex.ContainsSection(".cenega", exact: true);
            if (cenegaSection)
                return "Cenega ProtectDVD";

            return null;
        }
    }
}
