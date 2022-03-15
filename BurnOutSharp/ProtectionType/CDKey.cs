using System;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class CDKey : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, bool includeDebug, PortableExecutable pex)
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
