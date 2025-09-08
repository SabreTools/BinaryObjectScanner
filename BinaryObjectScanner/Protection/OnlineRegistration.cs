using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class OnlineRegistration : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Is this too broad in general?
            var name = exe.InternalName;
            if (name.OptionalStartsWith("EReg", StringComparison.OrdinalIgnoreCase))
                return $"Executable-Based Online Registration {exe.GetInternalVersion()}";

            return null;
        }
    }
}
