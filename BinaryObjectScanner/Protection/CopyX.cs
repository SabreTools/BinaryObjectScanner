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
        //There are four kinds of copy-X; Light, Profesisonal, audio, and Trial.
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
        // Seemingly it can be anything. It doesn't help that most samples are specifically from one company's games, Tivola. Still, most use ZDAT.

        //Professional:
        //All instances of professional contain a disc check, performed via optgraph.dll. 
        //All instances of professional contain in the directory at the end of the image 3 files. gov_[something].x64, iofile.x64, and sound.x64.
        //Due to gov's minor name variance, sound.x64 sometimes being intersected by a ring at the start, and iofile.x64 being referenced directly in optgraph.x64, I chose to just check iofile.x64.
        //It always starts the same, so if there are false positives, a more advanced check can be used if necessary.
        // TODO: optgraph.dll also contains DRM to prevent kernel debugger SoftICE from being used, via a process called SoftICE-Test. I don't know if this is specifically part of copy-X, or if it's an external solution employed by both copy-X and also other companies. If it's the latter, it should have its own check. It has none here since it wouldn't be necessary.


        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)//Checks for Professional
        {
            //I don't need this to check optgraph.dll, that's redundant. Unsure how to exclude that.
            if (pex.OverlayStrings != null)
            {
                if (pex.OverlayStrings.Any(s => s.Contains("optgraph.dll")))//Checks if main executable contains reference to optgraph.dll. Emergency 4's is missing this for some reason. 
                    // TODO: TKKG also has an NE 3.1x executable with a reference. This can be added later.
                    return "copy-X";
            }

            return null;
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)//Checks for Professional
        {
            if (Path.GetFileName(path).Equals("optgraph.dll", StringComparison.OrdinalIgnoreCase))//Filename check for optgraph.dll disc check
            {
                return "copy-X";
            }

            if (Path.GetFileName(path).Equals("iofile.x64", StringComparison.OrdinalIgnoreCase))//Filename check for seemingly comorbid file.
            {
                return "copy-X";
            }

            return null;
        }

        //Light:
        //All instances of light contain 1 or more files in the directory at the end of the image. They all consist of 0x00, except for the parts with the rings running through them.

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)// Checks for Light
#endif
        {
#if NET20 || NET35
            var protections = new Queue<string>();
#else
            var protections = new ConcurrentQueue<string>();
#endif
            if (files == null)
                return protections;
            var zdatFiles = files.Where(f => f.Remove(0, path.Length + 1).StartsWith("ZDAT"));//Gets files in ZDAT*
            var fileList = zdatFiles.ToList();
            fileList.Sort();// Sorts list of files in ZDAT* so I can just pull the first one, later ones have a chance of the ring intersecting the start of the file.
            if (fileList.Count > 0)
            {
                FileStream stream = new FileStream(fileList[0], FileMode.Open, FileAccess.Read);
                byte[] block = new byte[1024];
                stream.Read(block, 0, 1024);
                if (block.All(thisByte => thisByte.Equals(0x00)))
                {
                    protections.Enqueue("x-Copy");
                }
            }

            return protections;
        }
    }
}
