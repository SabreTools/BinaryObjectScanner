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
   // TODO: figure out if that's actually what light and professional mean
   // TODO: add documentation/comments to methods
    public class CopyX : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug) //checks for Professional
        {
            //I need to change this to check all executables, not just portable ones, but including portable ones. I think. I'll figure out how to do that later. Do i need to? does rdata work for this?
            //i dont need this to check optgraph.dll, that's redundant
            var sections = pex.Model.SectionTable; 
            if (sections == null)
                return null;

            // Get the .rdata section strings, if they exist
            var name = pex.FileDescription;

            
            if (pex.OverlayStrings != null)
            {
                if (pex.OverlayStrings.Any(s => s.Contains("optgraph.dll")))
                    return "copy-X";
            }
            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files) // checks for Light
#endif
        {
#if NET20 || NET35
            var protections = new Queue<string>();
#else
            var protections = new ConcurrentQueue<string>();
#endif
            if (files == null)
                return protections;
            var zdatFiles = files.Where(f => f.Remove(0, path.Length + 1).StartsWith("ZDAT"));
            var fileList = zdatFiles.ToList();
            fileList.Sort();
            FileStream stream = new FileStream(fileList[0], FileMode.Open, FileAccess.Read);         
            byte[] block = new byte[1024];
            stream.Read(block, 0, 1024);
            if (block.All(thisByte => thisByte.Equals(0x00)))
            {
                protections.Enqueue("x-Copy");
            }
            return protections;
        }
        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("optgraph.dll", StringComparison.OrdinalIgnoreCase))
            {
                return "copy-X";
            }
            return null;
        }
    }
}