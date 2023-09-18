using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
    public class DinamicMultimedia : IPathCheck
    {
        // Dinamic Multimedia Protection is the name used here to describe the protections created and used by Dinamic Multimedia for some of their media releases.
        // LockBlocks falls under this category, being created by and seemingly exclusively in Dinamic Multimedia products, but in every place I find it described online, it is said to very specifically have two rings on the data side of the disc.
        // Due to there being seemingly no absolute defining feature to LockBlocks other than this, any protected disc from Dinamic Multimedia that doesn't specifically have two rings is considered to have "Dinamic Multimedia Protection".
        // That being said, it may be entirely possible that LockBlocks is the name for all these protections as a whole, as some sources seem to consider games that don't seem to have two rings to have LockBlocks.

        // Resources:
        // https://www.cdmediaworld.com/hardware/cdrom/cd_protections_lockblocks.shtml
        // https://www.cdrinfo.com/d7/content/cd-protection-overview?page=6
        // https://www.gamecopyworld.com/games/pc_pc_real_madrid_2000.shtml
        // https://www.gamecopyworld.com/games/pc_combat_mission_shock_force.shtml
        // https://www.gamecopyworld.com/games/pc_pc_atletismo.shtml
        // https://www.gamecopyworld.com/games/pc_pc_calcio_2000.shtml
        // https://www.gamecopyworld.com/games/pc_pc_futbol_2000.shtml

        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Many more checks are likely possible based on the sources, but only ones that have been personally verified are getting added.

                // Uncopyable files found in at least http://redump.org/disc/70531/, and likely in multiple others.
                new PathMatchSet(new PathMatch(Path.Combine("XCONTROL", "COMPPLAY._01").Replace("\\", "/"), useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch(Path.Combine("XCONTROL", "LANDER.DA0").Replace("\\", "/"), useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch(Path.Combine("XCONTROL", "XSMGOP.DAP").Replace("\\", "/"), useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch(Path.Combine("XCONTROL", "XSMGOP.VBX").Replace("\\", "/"), useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),

                // Copyable file found in http://redump.org/disc/70531/ that seems to be exclusively associated with the protection and other files that are part of the protection.
                new PathMatchSet(new PathMatch(Path.Combine("XCONTROL", "COMPSCO._01").Replace("\\", "/"), useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Many more checks are likely possible based on the sources, but only ones that have been personally verified are getting added.

                // Uncopyable files found in at least http://redump.org/disc/70531/, and likely in multiple others.
                new PathMatchSet(new PathMatch("2kscore.sc0", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch("arrcalc.obj", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch("bdrvisa.drv", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch("gprinter.dll", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch("hstadium.ipx", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch("omanager.odl", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch("opublic.001", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch("spland.sc0", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new PathMatchSet(new PathMatch("uqprime.ipx", useEndsWith: true), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
