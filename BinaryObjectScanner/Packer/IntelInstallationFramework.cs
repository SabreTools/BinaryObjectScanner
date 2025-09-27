using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// An MS-CAB based installer that stores the files in
    /// the resources table.
    /// </summary>
    public class IntelInstallationFramework : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            if (name.OptionalEquals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.OptionalEquals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase))
            {
                return $"Intel Installation Framework {exe.GetInternalVersion()}";
            }

            name = exe.ProductName;

            if (name.OptionalEquals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.OptionalEquals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase))
            {
                return $"Intel Installation Framework {exe.GetInternalVersion()}";
            }

            return null;
        }
    }
}
