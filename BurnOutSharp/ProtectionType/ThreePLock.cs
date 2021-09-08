using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class ThreePLock : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;
        // {
        //     return new List<ContentMatchSet>
        //     {
        //         //This produced false positives in some DirectX 9.0c installer files
        //         //"Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW"
        //         new ContentMatchSet(new byte?[]
        //         {
        //             0x59, 0xC3, 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x30,
        //             0x53, 0x56, 0x57
        //         }, "3PLock"),
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

            // Get the .ldr and .ldt sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            var cmsdSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".ldr"));
            var cmstSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".ldt"));
            if (cmsdSection != null || cmstSection != null)
                return $"3PLock";

            return null;
        }
    }
}
