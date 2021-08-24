using System;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IContentCheck
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var fileDetails = Utilities.GetFileVersionInfo(file);

            string name = fileDetails?.FileDescription.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase))
                return $"Intel Installation Framework {Utilities.GetFileVersion(file)}";

            name = fileDetails?.ProductName.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase))
                return $"Intel Installation Framework {Utilities.GetFileVersion(file)}";

            name = fileDetails?.ProductName.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase))
                return $"Intel Installation Framework {Utilities.GetFileVersion(file)}";

            name = fileDetails?.ProductName.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase))
                return $"Intel Installation Framework {Utilities.GetFileVersion(file)}";

            return null;
        }
    }
}
