using System;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class OnlineRegistration : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // TODO: Is this too broad in general?
            string name = Utilities.GetInternalName(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("EReg", StringComparison.OrdinalIgnoreCase))
                return $"Executable-Based Online Registration {Utilities.GetFileVersion(pex)}";

            return null;
        }
    }
}
