using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;
using System.Collections.Generic;
using SabreTools.Matching;
using SabreTools.Matching.Content;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// .NET Reactor is a .NET obfuscator that was original released in 2004. https://web.archive.org/web/20040828162124/http://eziriz.com:80/
    /// It is currently still being updated and supported. https://www.eziriz.com/dotnet_reactor.htm
    /// While ProtectionID does detect .NET Reactor, it's currently unknown exactly how. 
    /// It seems to simply check for the string "<PrivateImplementationDetails>" in specific, and currently unknown, conditions but appears to be prone to false positives.
    /// A "Demo/Nag Screen" version is available for free, and may be able to be used to make samples to improve detections. https://www.eziriz.com/reactor_download.htm
    /// 
    /// Resource that could be useful for extraction: https://github.com/SychicBoy/NETReactorSlayer
    /// </summary>
    public class DotNetReactor : IExecutableCheck<PortableExecutable>, IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Detect version
            // TODO: Further refine checks using https://github.com/horsicq/Detect-It-Easy/blob/075a70b1484d1d84d1dc37c86aac16188d5a84e7/db/PE/NetReactor.2.sg and https://github.com/cod3nym/detection-rules/blob/main/yara/dotnet/obf_net_reactor.yar
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .text section, if it exists
            if (pex.ContainsSection(".text"))
            {
                var textData = pex.GetFirstSectionData(".text");
                if (textData != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // Adapted from https://github.com/cod3nym/detection-rules/blob/main/yara/dotnet/obf_net_reactor.yar and confirmed to work with "KalypsoLauncher.dll" from Redump entry 95617.
                        // <PrivateImplementationDetails>{[8]-[4]-[4]-[4]-[12]}
                        new(new byte?[]
                        {
                            0x3C, 0x50, 0x72, 0x69, 0x76, 0x61, 0x74, 0x65,
                            0x49, 0x6D, 0x70, 0x6C, 0x65, 0x6D, 0x65, 0x6E, 
                            0x74, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x44, 0x65,
                            0x74, 0x61, 0x69, 0x6C, 0x73, 0x3E, 0x7B, null, 
                            null, null, null, null, null, null, null, 0x2D,
                            null, null, null, null, 0x2D, null, null, null,
                            null, 0x2D, null, null, null, null, 0x2D, null,
                            null, null, null, null, null, null, null, null,
                            null, null, null, 0x7D
                        }, ".NET Reactor"),

                        // Modified from the previous detection to detect a presumably newer version of .NET Reactor found in "KalypsoLauncher.dll" version 2.0.4.2. 
                        // TODO: Check if this can/should be made more specific.
                        // <PrivateImplementationDetails>.RSA
                        new(new byte?[]
                        {
                            0x3C, 0x50, 0x72, 0x69, 0x76, 0x61, 0x74, 0x65,
                            0x49, 0x6D, 0x70, 0x6C, 0x65, 0x6D, 0x65, 0x6E, 
                            0x74, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x44, 0x65,
                            0x74, 0x61, 0x69, 0x6C, 0x73, 0x3E, 0x00, 0x52,
                            0x53, 0x41
                        }, ".NET Reactor"),

                        // Adapted from https://github.com/cod3nym/detection-rules/blob/main/yara/dotnet/obf_net_reactor.yar and confirmed to work with "KalypsoLauncher.dll" from Redump entry 95617.
                        // 3{.[9].-.[9].-.[9].}
                        new(new byte?[]
                        {
                            0x33, 0x7B, 0x00, null, null, null, null, null, 
                            null, null, null, null, 0x00, 0x2D, 0x00, null, 
                            null, null, null, null, null, null, null, null, 
                            0x00, 0x2D, 0x00, null, null, null, null, null, 
                            null, null, null, null, 0x00, 0x2D, 0x00, null, 
                            null, null, null, null, null, null, null, null, 
                            0x00, 0x7D, 0x00 
                        }, ".NET Reactor (Unconfirmed - Please report to us on GitHub)"),
                        
                        // Adapted from https://github.com/cod3nym/detection-rules/blob/main/yara/dotnet/obf_net_reactor.yar and confirmed to work with "KalypsoLauncher.dll" from Redump entry 95617.
                        // <Module>{[8]-[4]-[4]-[4]-[12]}
                        new(new byte?[]
                        {
                            0x3C, 0x4D, 0x6F, 0x64, 0x75, 0x6C, 0x65, 0x3E, 
                            0x7B, null, null, null, null, null, null, null, 
                            null, 0x2D, null, null, null, null, 0x2D, null, 
                            null, null, null, 0x2D, null, null, null, null, 
                            0x2D, null, null, null, null, null, null, null, 
                            null, null, null, null, null, 0x7D
                        }, ".NET Reactor (Unconfirmed - Please report to us on GitHub)")
                    };

                    return MatchUtil.GetFirstMatch(file, textData, matchers, includeDebug);
                }
            }


            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            // TODO: Add extraction
            return false;
        }
    }
}
