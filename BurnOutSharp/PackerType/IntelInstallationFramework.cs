using System;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var fvinfo = Utilities.GetFileVersionInfo(file);

            string name = fvinfo?.FileDescription.Trim();
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(file)}";
            }

            name = fvinfo?.ProductName.Trim();
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(file)}";
            }

            return null;
        }
    }
}
