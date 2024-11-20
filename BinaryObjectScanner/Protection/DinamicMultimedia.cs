using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;

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

        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Many more checks are likely possible based on the sources, but only ones that have been personally verified are getting added.

                // Uncopyable files found in at least http://redump.org/disc/70531/, and likely in multiple others.
                new(new FilePathMatch(Path.Combine("XCONTROL", "COMPPLAY._01")), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch(Path.Combine("XCONTROL", "LANDER.DA0")), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch(Path.Combine("XCONTROL", "XSMGOP.DAP")), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch(Path.Combine("XCONTROL", "XSMGOP.VBX")), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),

                // Copyable file found in http://redump.org/disc/70531/ that seems to be exclusively associated with the protection and other files that are part of the protection.
                new(new FilePathMatch(Path.Combine("XCONTROL", "COMPSCO._01")), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Many more checks are likely possible based on the sources, but only ones that have been personally verified are getting added.

                // Uncopyable files found in at least http://redump.org/disc/70531/, and likely in multiple others.
                new(new FilePathMatch("2kscore.sc0"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch("arrcalc.obj"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch("bdrvisa.drv"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch("gprinter.dll"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch("hstadium.ipx"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch("omanager.odl"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch("opublic.001"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch("spland.sc0"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
                new(new FilePathMatch("uqprime.ipx"), "Dinamic Multimedia Protection/LockBlocks [Check disc for 2 physical rings]"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
