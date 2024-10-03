using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class CDSHiELDSE : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Indicates Hypertech Crack Proof as well?
            //// Get the import directory table
            //if (pex.Model.ImportTable?.ImportDirectoryTable != null)
            //{
            //    bool match = pex.Model.ImportTable.ImportDirectoryTable.Any(idte => idte.Name == "KeRnEl32.dLl");
            //    if (match)
            //        return "CDSHiELD SE";
            //}

            // Get the code/CODE section strings, if they exist
            var strs = pex.GetFirstSectionStrings("code") ?? pex.GetFirstSectionStrings("CODE");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("~0017.tmp")))
                    return "CDSHiELD SE";
            }

            return null;
        }
    }
}
