using System;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class CDKey : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
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
