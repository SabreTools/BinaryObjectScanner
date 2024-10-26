using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Technically not necessary, but just check for light/pro first and only if it isn't found look for the other. It should be an Or situation and not an And situation.
    // TODO: Figure out if Light and Professional are what designate rings and rings+disccheck
    // TODO: add documentation/comments to methods
    public class CopyX : IPathCheck, IPortableExecutableCheck
    {
        // There are four kinds of copy-X; Light, Profesisonal, audio, and Trial.
        // audio is Audio CD only and has no data, so it can't be checked for.
        // I have no samples of Trial at the moment, so it can't be checked for either.
        // There are two kinds of copy-X that I've observed; those with only rings, and those with rings and a disc check.
        // I am assuming based on 0 evidence that the former is Light and the latter is Professional.
        // Because I have no evidence, I am just returning copy-X for now. I have pre-emptively begun seperating the two,
        // just for when a designation can be applied for sure.

        // Overall:
        // Whenever I say "at the end of" or "at the start of" pertaining to the filesystem, I mean alphabetically, because this is how copy-X images seem to be mastered.
        // Both Light and Professional have a directory at the end of the image. The files within this directory are intersected by the physical ring.
        // This file is usually called ZDAT, but not always. At least one instance of Light calls it ZDATA. At least one instance of Professional calls it System.
        // Seemingly it can be anything. It doesn't help that most known samples are specifically from one company's games, Tivola. Still, most use ZDAT.

        // Professional:
        // All instances of professional contain a disc check, performed via optgraph.dll. 
        // All instances of professional contain in the directory at the end of the image 3 files. gov_[something].x64, iofile.x64, and sound.x64.
        // Due to gov's minor name variance, sound.x64 sometimes being intersected by a ring at the start, and iofile.x64 being referenced directly in optgraph.x64, I chose to just check iofile.x64.
        // TODO: optgraph.dll also contains DRM to prevent kernel debugger SoftICE from being used, via a process called SoftICE-Test. I don't know if this is specifically part of copy-X, or if it's an external solution employed by both copy-X and also other companies. If it's the latter, it should have its own check. It has none here since it wouldn't be necessary.


        // Light:
        // All instances of light contain 1 or more files in the directory at the end of the image. They all consist of either 0x00, or some data that matches between entries (and also is present in the 3 Professional files), except for the parts with the rings running through them.
        // TODO: Check the last directory alphabetically and not just ZDAT*

        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)//Checks for Professional
        {
            //I don't need this to check optgraph.dll, that's redundant. Unsure how to exclude that.
            if (pex.OverlayStrings != null)
            {
                // Checks if main executable contains reference to optgraph.dll. Emergency 4's is missing this for some reason. 
                // This might be better removed later, as Redump ID 82475 is a false positive, and also doesn't actually contain the actual optgraph.dll file.
                // TODO: Find a way to check for situations like Redump ID 48393/, where the string is spaced out with 0x00 between letters and does not show up on string checks.
                // TODO: This might need to check every single section. Unsure until more samples are acquired.
                if (pex.OverlayStrings.Any(s => s.Contains("optgraph.dll")))
                {
                    // TODO: TKKG also has an NE 3.1x executable with a reference. This can be added later.
                    // Samples: Redump ID 108150
                    return "copy-X";
                }
            }

            var strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("optgraph.dll")))
                {
                    // Samples: Redump ID 82475 (False positive, but probably catches original Emergency 2), would catch Redump ID 48393
                    return "copy-X";
                }
            }

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)// Checks for Light
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
            var zdatFiles = files.Where(f => f.Remove(0, path.Length + 1).StartsWith("ZDAT", StringComparison.OrdinalIgnoreCase));//Gets files in ZDAT*
            var fileList = zdatFiles.ToList();
            // Sorts list of files in ZDAT* so I can just pull the first one, later ones have a chance of the ring intersecting the start of the file.
            fileList.Sort();
            if (fileList.Count > 0)
            {
                // Checks for whatever this data is.
                // Samples: Redump ID 84759, Redump ID 107929. Professional discs also have this data, hence the exclusion check.
                byte[] compareMe = new byte[64]
                {
                    0x02, 0xFE, 0x4A, 0x4F, 0x52, 0x4B, 0x1C, 0xE0, 0x79, 0x8C, 0x7F, 0x85, 0x04, 0x00, 0x46, 0x46, 0x49, 0x46, 0x07, 0xF9, 0x9F, 0xA0, 0xA1, 0x9D, 0xDA, 0xB6, 0x2C, 0x2D, 0x2D, 0x2C, 0xFF, 0x00, 0x6F, 0x6E, 0x71, 0x6A, 0xFC, 0x06, 0x64, 0x62, 0x65, 0x5F, 0xFB, 0x06, 0x31, 0x31, 0x31, 0x31, 0x00, 0x00, 0x1D, 0x1D, 0x1F, 0x1D, 0xFE, 0xFD, 0x51, 0x57, 0x56, 0x51, 0xFB, 0x06, 0x33, 0x34
                };
                try
                {
                    FileStream stream = new FileStream(fileList[0], FileMode.Open, FileAccess.Read);
                    byte[] block = new byte[64];
                    stream.Read(block, 0, 64);
                    // Excludes files with .x64 extension to avoid flagging Professional files.
                    if (block.SequenceEqual(compareMe) && !fileList[0].EndsWith(".x64", StringComparison.OrdinalIgnoreCase))
                    {
                        protections.Enqueue("copy-X");
                    }
                    else
                    {
                        // Checks if the file contains 0x00
                        // Samples: Redump ID 81628
                        block = new byte[1024];
                        stream.Read(block, 0, 1024);
                        if (block.All(thisByte => thisByte.Equals(0x00)))
                        {
                            protections.Enqueue("copy-X");
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return protections;
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)//Checks for Professional
        {
            // Samples: Redump ID 108150, Redump ID 48393
            if (Path.GetFileName(path).Equals("optgraph.dll", StringComparison.OrdinalIgnoreCase))//Filename check for optgraph.dll disc check
            {
                return "copy-X";
            }

            if (Path.GetFileName(path).StartsWith("gov_", StringComparison.OrdinalIgnoreCase) && Path.GetFileName(path).EndsWith(".x64", StringComparison.OrdinalIgnoreCase))
            {
                return "copy-X";
            }

            if (Path.GetFileName(path).Equals("iofile.x64", StringComparison.OrdinalIgnoreCase))//Filename check for seemingly comorbid file.
            {
                return "copy-X";
            }

            if (Path.GetFileName(path).Equals("sound.x64", StringComparison.OrdinalIgnoreCase))
            {
                return "copy-X";
            }

            return null;
        }
    }
}
