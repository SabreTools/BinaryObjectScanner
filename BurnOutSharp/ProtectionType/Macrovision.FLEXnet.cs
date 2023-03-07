using System;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// This is a placeholder FLEXnet (sub-Macrovision) specific functionality
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.IPortableExecutableCheck.CheckPortableExecutable(string, PortableExecutable, bool)"/>
        internal string FLEXnetCheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Found in "IsSvcInstDanceEJay7.dll" in IA item "computer200709dvd" (Dance eJay 7).
            string name = pex.ProductName;
            if (name?.Equals("FLEXnet Activation Toolkit", StringComparison.OrdinalIgnoreCase) == true)
                return $"FLEXnet";

            return null;
        }
    }
}
