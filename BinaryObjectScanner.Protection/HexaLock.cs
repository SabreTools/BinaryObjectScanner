using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    ///  HexaLock AutoLock was a copy protection scheme that requied users to buy so-called "CD-RX" media that contained a special session pre-burned to it in order to burn their protect media.
    ///  Sales page for CD-RX media: http://www.image-src.com/services/hexalock.asp
    ///  Hexalock AutoLock was also able to be used with pressed CD-ROMs (Source: https://web.archive.org/web/20110828214830/http://hexalock.co.il/copyprotection/cdrom).
    ///  One pressed example has been found so far, though it seems that it may not have been hooked into the protection properly, as the game is able to be run directly from files extracted from the installer (Redump entry 95764/IA item "Nova_Spacebound_USA").
    ///  Though the protection seems unused in this sample, the final sectors of this disc are unable to be read by some software/drive combinations, which seems likely to be related to the protection.
    ///  It also allowed you to protect multimedia documents, such as documents or pictures. 
    ///  The official website is now dead, but there are a few archives made (https://web.archive.org/web/20110904233743/http://hexalock.co.il/).
    ///  There don't appear to be any archives of the "CD-RX" media available, though it appears that some are still for sale on Amazon:
    ///  https://www.amazon.cn/dp/B000F3RPCI + https://www.amazon.cn/dp/B000F3PJA8
    ///  CD-RX media makes use of twin sectors as one of the aspects of the formats copy protection (Source: https://twitter.com/RibShark/status/1551660315489730561)
    ///  These twin sectors are presumably what the Hexalock AutoLock marketing refers to as VDH (Virtual Digital Hologram) (https://web.archive.org/web/20120616004438/http://hexalock.co.il/copyprotection/vdh).
    ///  It appears that some versions of "Operation Flashpoint" contain Hexaock AutoLock (Source: https://www.cdmediaworld.com/hardware/cdrom/cd_protections_hexalock.shtml).
    ///  HexaLock AutoLock 4.5 official download archive: https://web.archive.org/web/20070228235538/http://hexalock.com:80/45/alw_45_march_3_2006.exe
    ///  HexaLock AutoLock 4.7 official download archive: https://web.archive.org/web/20140801060304/http://hexalock.co.il/downloads/files/Psetup.exe
    ///  There appears to be another form of copy protection created by HexaLock called HexDVDR, but I have not been able to find a copy of it preserved (Source: https://web.archive.org/web/20140801060150/http://hexalock.co.il/news/2008-03-20/).
    ///  There is an example EXE protected using HexDVDR provided that is still online (https://web.archive.org/web/20140802144000/http://hexalock.co.il/downloads/files/Protected%20Img.zip).
    ///  Patents relating to this protection: 
    ///  https://patentimages.storage.googleapis.com/64/d6/b1/91127b030d3503/US20060259975A1.pdf
    ///  https://patentimages.storage.googleapis.com/52/5b/3a/aee21ff4d987e9/US20060123483A1.pdf
    ///  Special thanks to Ribshark for looking into this protection and sharing his research on the topic!
    /// </summary>
    public class HexalockAutoLock : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // TODO: Fix the following checks, as this information is visible via Windows Explorer but isn't currently being seen by BOS.
            // Found in "HCPSMng.exe".
            string name = pex.FileDescription;
            if (name?.StartsWith("HCPS Manager", StringComparison.OrdinalIgnoreCase) == true)
                return $"Hexalock AutoLock 4.5";

            // Found in the file typically named "Start_Here.exe".
            if (name?.StartsWith("HCPS Loader", StringComparison.OrdinalIgnoreCase) == true)
                return $"Hexalock AutoLock 4.5";

            // Found in both "HCPSMng.exe" and in the file typically named "Start_Here.exe".
            name = pex.ProductName;
            if (name?.StartsWith("HCPS", StringComparison.OrdinalIgnoreCase) == true)
                return $"Hexalock AutoLock 4.5";

            // Get the .text section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                // Found in "The Sudoku Challenge Collection.exe" in "The Sudoku Challenge! Collection" by Play at Joe's.
                if (strs.Any(s => s.Contains("mfint.dll")))
                    return "Hexalock Autolock";
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // "Start_Here.exe" is the default name used in HexaLock AutoLock 4.5.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("Start_Here.exe", useEndsWith: true),
                    new PathMatch("MFINT.DLL", useEndsWith: true),
                    new PathMatch("MFIMP.DLL", useEndsWith: true),
                }, "Hexalock AutoLock 4.5"),

                // Used for PDF protection in HexaLock AutoLock 4.7. "Start.exe" likely has some internal strings that can be checked.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("kleft.ipf", useEndsWith: true),
                    new PathMatch("ReadPFile.exe", useEndsWith: true),
                    new PathMatch("Start.exe", useEndsWith: true),
                }, "HexaLock AutoLock 4.7 PDF DRM"),

                // Should be present in all known versions.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("MFINT.DLL", useEndsWith: true),
                    new PathMatch("MFIMP.DLL", useEndsWith: true),
                }, "HexaLock AutoLock"),

                // Found inside the file typically named "Start_Here.exe" in version 4.5.
                new PathMatchSet(new PathMatch("HCPSMng.exe", useEndsWith: true), "HexaLock AutoLock 4.5"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found to be the default name used in HexaLock AutoLock 4.5.
                new PathMatchSet(new PathMatch("Start_Here.exe", useEndsWith: true), "HexaLock AutoLock 4.5"),

                // Found to be contained in HexaLock AutoLock 4.5 and 4.7.
                new PathMatchSet(new PathMatch("MFINT.DLL", useEndsWith: true), "HexaLock AutoLock"),
                new PathMatchSet(new PathMatch("MFIMP.DLL", useEndsWith: true), "HexaLock AutoLock"),

                // Used for PDF protection in HexaLock AutoLock 4.7.
                new PathMatchSet(new PathMatch("kleft.ipf", useEndsWith: true), "HexaLock AutoLock 4.7 PDF DRM"),
                new PathMatchSet(new PathMatch("ReadPFile.exe", useEndsWith: true), "HexaLock AutoLock 4.7 PDF DRM"),

                // Found inside the file typically named "Start_Here.exe" in version 4.5.
                new PathMatchSet(new PathMatch("HCPSMng.exe", useEndsWith: true), "HexaLock AutoLock 4.5"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
