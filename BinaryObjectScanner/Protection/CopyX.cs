using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Technically not necessary, but just check for light/pro first and only if it isn't found look for the other.
    // It should be an Or situation and not an And situation.
    // TODO: Figure out if Light and Professional are what designate rings and rings+disccheck
    public class CopyX : IPathCheck, IPortableExecutableCheck
    {
        // Previous check 'Tivola Ring Protect' removed because it was found to actually be copy-x. 
        // The checks were for ZDAT/webmast.dxx and ZDAT/webmast.dxx, for Redump IDs 81628 and 116418.   

        // https://web.archive.org/web/20011016234742/http://www.optimal-online.de:80/product/copy_x.htm
        // There are four kinds of copy-X; Light, Profesisonal, audio, and Trial Maker.
        // Audio is for Audio CDs. Might be scannable, might not. Samples needed to confirm.
        // No samples of Trial are known at the moment, so it can't be checked for either.
        // There are two kinds of copy-X generally observed; those with only rings, and those with rings and a disc check.
        // These comments assume with 0 evidence that the former is Light and the latter is Professional.
        // Because there is no evidence, only copy-X is being returned for now. This check has pre-emptively separated
        // the two, just for when a designation can be applied for sure.

        // Overall:
        // Whenever these comments state "at the end of" or "at the start of" pertaining to the filesystem, they refer
        // to alphabetical order, because this is how copy-X images seem to be mastered usually (not always, but w/e).
        // Both Light and Professional have a directory at the end of the image. The files within this directory are
        // intersected by the physical ring.
        // This file is usually called ZDAT, but not always. At least one instance of Light calls it ZDATA. At least one
        // instance of Light calls it System.
        // Seemingly it can be anything. It doesn't help that most known samples are specifically from one company's
        // games, Tivola. Still, most use ZDAT.

        // Professional:
        // All instances of professional contain a disc check, performed via optgraph.dll. 
        // All instances of professional contain in a directory usually (but not always, German Emergency 2 Deluxe has a
        // Videos folder as well, which isn't involved in rings/protection) at the end of the image, 3 files:
        // gov_[something].x64, iofile.x64, and sound.x64. So far, they have always been in a directory called "System".
        // Due to gov's minor name variance, sound.x64 sometimes being intersected by a ring at the start, and
        // iofile.x64 being referenced directly in optgraph.x64, only iofile.x64 is being checked for now.
        // TODO: optgraph.dll also contains DRM to prevent kernel debugger SoftICE from being used, via a process called
        // SoftICE-Test. It is not currently known if this is specifically part of copy-X, or if it's an external
        // solution employed by both copy-X and also other companies. If it's the latter, it should have its own check.
        // It has none here since it wouldn't be necessary.

        // Light:
        // All instances of light contain 1 or more files in the directory usually (but not always; Kenny's Adventure has
        // uses a System folder, and then has a non-protection Xtras folder on the disc as well) at the end of the image.
        // They all consist of either 0x00, or some data that matches between entries (and also is present in the 3
        // Professional files), except for the parts with the rings running through them.
        // TODO: Check the last directory alphabetically and not just ZDAT*

        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Checks for Professional
            // PEX checks intentionally only detect Professional

            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            if (pex.OverlayStrings != null)
            {
                // Checks if main executable contains reference to optgraph.dll. 
                // This might be better removed later, as Redump ID 82475 is a false positive, and also doesn't actually
                // contain the actual optgraph.dll file.
                // TODO: Find a way to check for situations like Redump ID 48393, where the string is spaced out with
                // 0x00 between letters and does not show up on string checks.
                // TODO: This might need to check every single section. Unsure until more samples are acquired.
                // TODO: TKKG also has an NE 3.1x executable with a reference. This can be added later.
                // Samples: Redump ID 108150
                if (pex.OverlayStrings.Any(s => s.Contains("optgraph.dll")))
                    return "copy-X [Check disc for physical ring]";
            }

            var strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                // Samples: Redump ID 82475, German Emergency 2 Deluxe, Redump ID 48393
                if (strs.Any(s => s.Contains("optgraph.dll")))
                    return "copy-X [Check disc for physical ring]";
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
#if NET20 || NET35
            var protections = new Queue<string>();
#else
            var protections = new ConcurrentQueue<string>();
#endif
            if (files == null)
                return protections;

            // Checks for Light
            // Directory checks intentionally only detect Light

            // Excludes files with .x64 extension to avoid flagging Professional files.
            // Sorts list of files in ZDAT* so just the first file gets pulled, later ones have a chance of the ring 
            // intersecting the start of the file.

            string[] dirs = ["ZDAT", "System/", "System\\"];//Kenny's Adventure uses System instead of ZDAT.
            List<string>? lightFiles = null;
            // TODO: Compensate for the check being run a directory or more higher
            var fileList = files.Where(f => !f.EndsWith(".x64", StringComparison.OrdinalIgnoreCase));
            {
                foreach (var dir in dirs)
                {
                    lightFiles = fileList.Where(f =>
                        {
                            f = f.Remove(0, path.Length);
                            f = f.TrimStart('/', '\\');
                            return f.StartsWith(dir, StringComparison.OrdinalIgnoreCase);
                        })
                        .OrderBy(f => f)
                        .ToList();
                    if (lightFiles.Any())
                    {
                        break;
                    }
                }
            }
            if (lightFiles.Count > 0)
            {
                try
                {
                    using var stream = File.OpenRead(lightFiles[0]);
                    byte[] block = stream.ReadBytes(1024);

                    var matchers = new List<ContentMatchSet>
                    {
                        // Checks if the file contains 0x00
                        // Samples: Redump ID 81628
                        new(Enumerable.Repeat<byte?>(0x00, 1024).ToArray(), "copy-X"),

                        // Checks for whatever this data is.
                        // Samples: Redump ID 84759, Redump ID 107929. Professional discs also have this data, hence the exclusion check.
                        new(
                        [
                            0x02, 0xFE, 0x4A, 0x4F, 0x52, 0x4B, 0x1C, 0xE0,
                            0x79, 0x8C, 0x7F, 0x85, 0x04, 0x00, 0x46, 0x46,
                            0x49, 0x46, 0x07, 0xF9, 0x9F, 0xA0, 0xA1, 0x9D,
                            0xDA, 0xB6, 0x2C, 0x2D, 0x2D, 0x2C, 0xFF, 0x00,
                            0x6F, 0x6E, 0x71, 0x6A, 0xFC, 0x06, 0x64, 0x62,
                            0x65, 0x5F, 0xFB, 0x06, 0x31, 0x31, 0x31, 0x31,
                            0x00, 0x00, 0x1D, 0x1D, 0x1F, 0x1D, 0xFE, 0xFD,
                            0x51, 0x57, 0x56, 0x51, 0xFB, 0x06, 0x33, 0x34,
                        ], "copy-X [Check disc for physical ring]"),
                    };

                    var match = MatchUtil.GetFirstMatch(lightFiles[0], block, matchers, false);
                    if (!string.IsNullOrEmpty(match))
                        protections.Enqueue(match!);
                }
                catch { }
            }

            return protections;
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // Checks for Professional
            // File Path checks intentionally only detect Professional

            var matchers = new List<PathMatchSet>
            {
                // Samples: Redump ID 108150, Redump ID 48393

                // File responsible for disc check
                new(new FilePathMatch("optgraph.dll"), "copy-X [Check disc for physical ring]"),

                // Seemingly comorbid file, referenced in above file
                new(new FilePathMatch("iofile.x64"), "copy-X [Check disc for physical ring]"),

                // Seemingly comorbid file
                new(new FilePathMatch("sound.x64"), "copy-X [Check disc for physical ring]"),

                // Seemingly comorbid file
                // Check commented out until implementation can be decided
                // At least one disc seen online calls it mov_05.x64
                // new(new FilePathMatch("gov_*.x64"), "copy-X [Check disc for physical ring]"),
            };
            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
