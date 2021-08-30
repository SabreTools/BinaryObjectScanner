using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;
        // {
        //     // TODO: Remove this if the below section check is proven
        //     return new List<ContentMatchSet>
        //     {
        //         // .cenega
        //         new ContentMatchSet(new byte?[] { 0x2E, 0x63, 0x65, 0x6E, 0x65, 0x67, 0x61 }, "Cenega ProtectDVD"),
        //     };
        // }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .cenega section, if it exists -- TODO: Confirm this check with a real disc
            var nicodeSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".cenega"));
            if (nicodeSection != null)
                return "Cenega ProtectDVD";

            return null;
        }
    }
}
