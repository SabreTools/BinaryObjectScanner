using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class CDSHiELDSE : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Indicates Hypertech Crack Proof as well?
            //// Get the import directory table
            //if (pex.Model.ImportTable?.ImportDirectoryTable != null)
            //{
            //    bool match = pex.Model.ImportTable.ImportDirectoryTable.Any(idte => idte.Name == "KeRnEl32.dLl");
            //    if (match)
            //        return "CDSHiELD SE";
            //}

            // Get the code/CODE section strings, if they exist
            var strs = FileType.Executable.GetFirstSectionStrings(pex, "code") ?? FileType.Executable.GetFirstSectionStrings(pex, "CODE");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("~0017.tmp")))
                    return "CDSHiELD SE";
            }

            return null;
        }
    }
}
