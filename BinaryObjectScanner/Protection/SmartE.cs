using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class SmartE : IPathCheck, IExecutableCheck<PortableExecutable>
    {
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
           // Only works on stub generated from running the program yourself
           if (pex.InternalName.OptionalEquals("SmarteSECURE"))
               return "SmartE";
           
           var sections = pex.Model.SectionTable ?? [];

           if (sections.Length > 0)
           {
               // Get the last section data, if it exists
               var lastSectionData = pex.GetSectionData(sections.Length - 1);
               if (lastSectionData != null)
               {
                   // All sections seen so far are the last sections, so this is "technically"
                   // the only known needed check so far. Others kept as backups if this fails
                   // on some future entry
                   var matchers = GenerateMatchers();
                   var match = MatchUtil.GetFirstMatch(file, lastSectionData, matchers, includeDebug);
                   if (!string.IsNullOrEmpty(match))
                       return match;
               }
           }
           
           // Specific known named sections:
           // .bss (Rise of Nations)
           // .tls (Zoo Tycoon 2)
           // .idata (http://redump.org/disc/58561/ and http://redump.org/disc/71983/)
           // .edata (http://redump.org/disc/36619/)
           
           return null;
        }
        
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new List<PathMatch>
                {
                    new FilePathMatch("00001.TMP"),
                    new FilePathMatch("00002.TMP")
                 }, "SmartE"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("00001.TMP"), "SmartE"),
                new(new FilePathMatch("00002.TMP"), "SmartE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
        
        /// <summary>
        /// Generate the set of matchers used for each section
        /// </summary>
        private static List<ContentMatchSet> GenerateMatchers()
        {
            return
            [
                // Matches most games, but a few like http://redump.org/disc/16541/ 
                // are only matched on the 00001/2.TMP files. PiD and other programs
                // don't detect this game either, though (Aside from the stub)
                new(new byte?[] 
                {
                    0xEB, 0x15, 0x03, 0x00, 0x00, 0x00, null, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x68, 0x00, 0x00, 0x00, 0x00, 0x55, 
                    0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81, 0xED, 
                    0x1D, 0x00, 0x00, 0x00, 0x8B, 0xC5, 0x55, 0x60, 
                    0x9C, 0x2B, 0x85, 0x8F, 0x07, 0x00, 0x00, 0x89, 
                    0x85, 0x83, 0x07, 0x00, 0x00, 0xFF, 0x74, 0x24, 
                    0x2C, 0xE8, 0xBB, 0x01, 0x00, 0x00, 0x0F, 0x82, 
                    0x2F, 0x06, 0x00, 0x00, 0xE8, 0x8E, 0x04, 0x00, 
                    0x00, 0x49, 0x0F, 0x88, 0x23, 0x06 
                }, "SmartE"),
            ];
        }
    }

}
