using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;

namespace BurnOutSharp.PackerType
{
    public class PEtite : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .petite section, if it exists -- TODO: Is there a version number that can be found?
            bool nicodeSection = pex.ContainsSection(".petite", exact: true);
            if (nicodeSection)
                return "PEtite";

            return null;
        }
    }
}
