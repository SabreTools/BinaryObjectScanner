using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class ThreeTwoOneStudios : IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Check the dialog box resources
            if (pex.FindDialogByTitle("321Studios Activation").Any())
                return $"321Studios Online Activation";
            else if (pex.FindDialogByTitle("321Studios Phone Activation").Any())
                return $"321Studios Online Activation";

            return null;
        }
    }
}
