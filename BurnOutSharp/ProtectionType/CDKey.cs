using System;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class CDKey : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.InternalName;
            if (name?.Equals("CDKey", StringComparison.OrdinalIgnoreCase) == true)
                return "CD-Key / Serial";

            return null;
        }
    }
}
