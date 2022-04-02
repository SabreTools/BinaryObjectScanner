using System;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class OnlineRegistration : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // TODO: Is this too broad in general?
            string name = pex.GetInternalName();
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("EReg", StringComparison.OrdinalIgnoreCase))
                return $"Executable-Based Online Registration {Utilities.GetInternalVersion(pex)}";

            return null;
        }
    }
}
