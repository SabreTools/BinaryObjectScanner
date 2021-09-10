using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CodeLock : IContentCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // CODE-LOCK.OCX
                new ContentMatchSet(new byte?[]
                {
                    0x43, 0x4F, 0x44, 0x45, 0x2D, 0x4C, 0x4F, 0x43,
                    0x4B, 0x2E, 0x4F, 0x43, 0x58
                }, "CodeLock"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
            {
                var neMatchSets = GetContentMatchSets();
                if (neMatchSets != null && neMatchSets.Any())
                    return MatchUtil.GetFirstMatch(file, fileContent, neMatchSets, includeDebug);

                return null;
            }
            
            // If there are more than 2 icd-prefixed sections, then we have a match
            int icdSectionCount = sections.Count(s => Encoding.ASCII.GetString(s.Name).StartsWith("icd"));
            if (icdSectionCount >= 2)
                return "CodeLock";
            
            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }
    }
}
