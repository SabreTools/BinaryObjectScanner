using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    ///  HexaLock AutoLock was a copy protection scheme that requied users to buy so-called "CD-RX" media that contained a special session pre-burned to it in order to burn their protect media.
    ///  Sales page for CD-RX media: http://www.image-src.com/services/hexalock.asp
    ///  It also allowed you to protect multimedia documents, such as documents or pictures. 
    ///  The official website is now dead, but there are a few archives made (https://web.archive.org/web/20110904233743/http://hexalock.co.il/).
    ///  There don't appear to be any archives of the "CD-RX" media available, though it appears that some are still for sale on Amazon:
    ///  https://www.amazon.cn/dp/B000F3RPCI + https://www.amazon.cn/dp/B000F3PJA8
    ///  It appears that some versions of "Operation Flashpoint" contain HexaLock AutoLock (Source: https://www.cdmediaworld.com/hardware/cdrom/cd_protections_hexalock.shtml).
    ///  HexaLock AutoLock 4.5 official download archive: https://web.archive.org/web/20070228235538/http://hexalock.com:80/45/alw_45_march_3_2006.exe
    ///  HexaLock AutoLock 4.7 official download archive: https://web.archive.org/web/20140801060304/http://hexalock.co.il/downloads/files/Psetup.exe
    ///  There appears to be another form of copy protection created by HexaLock called HexDVDR, but I have not been able to find a copy of it preserved (Source: https://web.archive.org/web/20140801060150/http://hexalock.co.il/news/2008-03-20/).
    ///  There is an example EXE protected using HexDVDR provided that is still online (https://web.archive.org/web/20140802144000/http://hexalock.co.il/downloads/files/Protected%20Img.zip).
    /// </summary>
    public class HexalockAutoLock : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // "Start_Here.exe" is the default named used in HexaLock AutoLock 4.5.
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

                // Currently unverified.
                new PathMatchSet(new PathMatch("HCPSMng.exe", useEndsWith: true), "HexaLock AutoLock"),
                
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found to be the default named used in HexaLock AutoLock 4.5.
                new PathMatchSet(new PathMatch("Start_Here.exe", useEndsWith: true), "HexaLock AutoLock 4.5"),

                // Found to be contained in HexaLock AutoLock 4.5 and 4.7.
                new PathMatchSet(new PathMatch("MFINT.DLL", useEndsWith: true), "HexaLock AutoLock"),
                new PathMatchSet(new PathMatch("MFIMP.DLL", useEndsWith: true), "HexaLock AutoLock"),

                // Used for PDF protection in HexaLock AutoLock 4.7.
                new PathMatchSet(new PathMatch("kleft.ipf", useEndsWith: true), "HexaLock AutoLock 4.7 PDF DRM"),
                new PathMatchSet(new PathMatch("ReadPFile.exe", useEndsWith: true), "HexaLock AutoLock 4.7 PDF DRM"),

                // Currently unverified.
                new PathMatchSet(new PathMatch("HCPSMng.exe", useEndsWith: true), "HexaLock AutoLock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
