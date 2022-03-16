using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction
    public class Shrinker : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .shrink0 and .shrink2 sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            bool shrink0Section = pex.ContainsSection(".shrink0", true);
            bool shrink2Section = pex.ContainsSection(".shrink2", true);
            if (shrink0Section || shrink2Section)
                return "Shrinker";

            return null;
        }
    }
}
