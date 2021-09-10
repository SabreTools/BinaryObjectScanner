using System;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class OnlineRegistration : IContentCheck
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
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("EReg", StringComparison.OrdinalIgnoreCase))
                return $"Executable-Based Online Registration {Utilities.GetFileVersion(pex)}";

            return null;
        }
    }
}
