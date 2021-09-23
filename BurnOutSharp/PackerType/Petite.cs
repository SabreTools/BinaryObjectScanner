using BurnOutSharp.ExecutableType.Microsoft;

namespace BurnOutSharp.PackerType
{
    public class PEtite : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
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
