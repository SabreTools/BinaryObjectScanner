using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.ProtectionType
{
    public class OnlineRegistration : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Is this too broad in general?
            var name = pex.InternalName;
            if (name?.StartsWith("EReg", StringComparison.OrdinalIgnoreCase) == true)
                return $"Executable-Based Online Registration {pex.GetInternalVersion()}";

            return null;
        }
    }
}
