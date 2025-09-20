using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class CDKey : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.InternalName;

            if (name.OptionalEquals("CDKey", StringComparison.OrdinalIgnoreCase))
                return "CD-Key / Serial";

            return null;
        }
    }
}
