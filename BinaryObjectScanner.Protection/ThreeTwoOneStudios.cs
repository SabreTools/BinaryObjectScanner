using System.Linq;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class ThreeTwoOneStudios : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
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
