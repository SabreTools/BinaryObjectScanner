using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    public class Armadillo : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Remove this if the below section check is proven
            return new List<ContentMatchSet>
            {
                // .nicode + (char)0x00
                new ContentMatchSet(new byte?[] { 0x2E, 0x6E, 0x69, 0x63, 0x6F, 0x64, 0x65, 0x00 }, "Armadillo"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PEExecutable pex = PEExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionHeaders;
            if (sections == null)
                return null;

            // Get the .nicode section, if it exists -- TODO: Confirm this check with a real disc
            var nicodeSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".nicode"));
            if (nicodeSection != null)
                return "Armadillo";

            // Get the .text1 section for scanning
            var textSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".text1"));
            if (textSection != null)
            {
                System.Console.WriteLine($"{Encoding.ASCII.GetString(textSection.Name)} {textSection.VirtualAddress}");

                int textSectionAddr = (int)EVORE.ConvertVirtualAddress(textSection.VirtualAddress, sections);
                int textSectionEnd = textSectionAddr + (int)textSection.VirtualSize;

                System.Console.WriteLine($"{Encoding.ASCII.GetString(textSection.Name)} {textSectionAddr} - {textSectionEnd}");

                var matchers = new List<ContentMatchSet>
                {
                    // ARMDEBUG
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x41, 0x52, 0x4D, 0x44, 0x45, 0x42, 0x55, 0x47 }, start: textSectionAddr, end: textSectionEnd),
                        "Armadillo"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
            }

            return null;
        }
    }
}
