using System;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class CDKey : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetInternalName(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("CDKey", StringComparison.OrdinalIgnoreCase))
                return "CD-Key / Serial";

            return null;
        }
    }
}
