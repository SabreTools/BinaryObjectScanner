#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// CD-Lock (https://www.cdmediaworld.com/hardware/cdrom/cd_protections_dummy_files.shtml) is a copy protection most commonly used in Europe and active from 1998-2001 that seems to have a resemblance to Bitpool in a few ways.
    /// It makes use of padded dummy files with an AFP extension, with there being 4 such files on a disc.
    /// These files typically have a filename consistening of seemingly random 4 characters with an AFP extension, though there are known exceptions.
    /// These exceptions have "afp" files with the names "Gang1.afp", "Gang2.afp", "Gang3.afp", and "Gang4.afp" (Redump entries 37788 and 43221).
    /// There also always seems to be a "CONFIG.AFP" file present, which in most cases contains a marker relating to the other AFP files present on the disc.
    /// At least one game breaks this mould, having a "CONFIG.AFP" file present but is filled with 0x20 bytes instead of the expected data (Redump entries 37788 and 43221).
    /// Games protected with CD-Lock also appear to consistently have 3 additional tracks relating to the protection appended. These are 2 audio tracks, followed by a data track.
    /// Unlike Bitpool, it seems that these additional tracks are appended whether or not there are intentional audio tracks present on the disc as well. Compare Redump entries 37788 and 66321.
    /// It appears that this latter data track points to the data of the first data track, giving the appearance to some software that the disc contains 2 equally sized data tracks.
    /// It seems to me that this is intended to confuse dumping software, possibly attempting to lead it to believe that the disc has a larger capacity than it physically could have.
    /// https://inxs8.tripod.com/illegal_toc.htm seems to confirm that the software of the time especially seems to have had issues with this, though none of the technical information on the page seems to be particularly correct.
    /// Based off of Redump entries 31615 and 37700, it seems that there are cases where the main data track on two separate discs is identical, but the protection tracks differ. The reason for this is unknown.
    /// There are a few discs where one of the protection audio tracks is quite large, sometimes even larger than the main data track. The reason for this is also unknown (Redump entries 43221 and 66749).
    /// https://www.softpedia.com/get/Security/Encrypting/CD-Lock.shtml seems to be an unrelated program that protects CDs via encryption.
    /// Possible false positives include Redump entries 51241, 51373, 54397, 76437.
    /// Confirmed to be present on Redump entries 24287, 31615, 34448, 35967, 36627, 37700, 37788, 43221, 55788, and 66749.
    /// </summary>
    public class CDLock : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // Found in game executables protected with CD-Lock (Redump entries 24287 and 31615).
                    // TODO: Check for possible false postives (Redump entry 97942).
                    // 2 + (char)0xF2 + (char)0x02 + (char)0x82 + (char)0xC3 + (char)0xBC + (char)0x0B + $ + (char)0x99 + (char)0xAD + 'C + (char)0xE4 + (char)0x9D + st + (char)0x99 + (char)0xFA + 2$ + (char)0x9D + )4 + (char)0xFF + t
                    new(new byte?[]
                    {
                        0x32, 0xF2, 0x02, 0x82, 0xC3, 0xBC, 0x0B, 0x24,
                        0x99, 0xAD, 0x27, 0x43, 0xE4, 0x9D, 0x73, 0x74,
                        0x99, 0xFA, 0x32, 0x24, 0x9D, 0x29, 0x34, 0xFF,
                        0x74
                    }, "CD-Lock"),
                };

                var match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrEmpty(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // TODO: Determine if there's any consistency in the naming of the additional AFP files.

                // Found in every confirmed sample of CD-Lock, generally (but not always) appears to include markers relating to the additional AFP files present (Redump entries 24287 and 31615). 
                new(new PathMatch("CONFIG.AFP", useEndsWith: true), "CD-Lock"),

                // There is also a "$$$$$$$$.$$$" file present on some discs, but it isn't known if this is directly related to CD-Lock (Redump entries 37788 and 43221).
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // TODO: Determine if there's any consistency in the naming of the additional AFP files.

                // Found in every confirmed sample of CD-Lock, generally (but not always) appears to include markers relating to the additional AFP files present (Redump entries 24287 and 31615).
                new(new PathMatch("CONFIG.AFP", useEndsWith: true), "CD-Lock"),

                // There is also a "$$$$$$$$.$$$" file present on some discs, but it isn't known if this is directly related to CD-Lock (Redump entries 37788 and 43221).
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
