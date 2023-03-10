using System;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class OnlineRegistration : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // TODO: Is this too broad in general?
            string name = pex.InternalName;
            if (name?.StartsWith("EReg", StringComparison.OrdinalIgnoreCase) == true)
                return $"Executable-Based Online Registration {pex.GetInternalVersion()}";

            return null;
        }
    }
}
